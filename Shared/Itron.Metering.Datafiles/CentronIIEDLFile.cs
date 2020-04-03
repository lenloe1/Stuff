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
//                           Copyright © 2006 - 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Itron.Common.C1219Tables.ANSIStandardII;
using Itron.Common.C1219Tables.CentronII;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Device;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Progressable;
using Itron.Metering.TOU;
using Itron.Metering.Utilities;

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// This class provides a representation of an OpenWay meter EDL file.  EDL stands for End Device
	/// Language and the file is essentially an XML file that defines the current state of
	/// a meter.  The EDL format is defined by the ANSI C12.19 format.  
	/// 
	/// This class does NOT provide a complete representation of a EDL file.  It provides
	/// read-only access to some of the data contained in the file, and it is intended
	/// to be used as means to validate meter operation.  The class should be extended as 
	/// needed to expose different data types.
    /// </summary>
    public class CentronIIEDLFile : EDLFile
    {
        #region Constants

        private const int MAX_DST_YEARS = 25;
        private const int UNUSED_DST_SLOTS = 2;

        #endregion Constants

        #region Public Methods

        /// <summary>
        /// Constructor to make an EDLFile object
        /// </summary>
        /// <param name="FileName">full path to the EDL file</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/30/06 RDB				   Created
        public CentronIIEDLFile(string FileName)
            :base(FileName)
        {

        }

        /// <summary>
        /// This method imports passwords into the file.
        /// </summary>
        /// <param name="strPrimaryPassword">The primary password</param>
        /// <param name="strSecondaryPassword">The secondary password</param>
        /// <param name="TertiaryPassword">The tertiary password</param>
        /// <param name="strQuaternaryPassword">The quaternary password</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void ImportPasswords(string strPrimaryPassword, string strSecondaryPassword, 
            string TertiaryPassword, string strQuaternaryPassword)
        {
            int[] anIndex1 = { 0 };
            System.Text.UTF8Encoding Encoding = new System.Text.UTF8Encoding();

            //Set primary security code 
            anIndex1[0] = PRIMARY_SEC_CODE_INDEX;
            SetValue((long)StdTableEnum.STDTBL42_PASSWORD, anIndex1, Encoding.GetBytes(strPrimaryPassword));
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_0, anIndex1, true);
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_1, anIndex1, true);
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_2, anIndex1, true);
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_3, anIndex1, true);

            //Set limited reconfigure security code 
            anIndex1[0] = LIMITED_SEC_CODE_INDEX;
            SetValue((long)StdTableEnum.STDTBL42_PASSWORD, anIndex1, Encoding.GetBytes(strSecondaryPassword));
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_0, anIndex1, false);
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_1, anIndex1, true);
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_2, anIndex1, true);
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_3, anIndex1, true);

            //Set secondary security code 
            anIndex1[0] = SECONDARY_SEC_CODE_INDEX;
            SetValue((long)StdTableEnum.STDTBL42_PASSWORD, anIndex1, Encoding.GetBytes(TertiaryPassword));
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_0, anIndex1, false);
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_1, anIndex1, false);
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_2, anIndex1, true);
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_3, anIndex1, true);

            //Set tertiary security code 
            anIndex1[0] = TERTIARY_SEC_CODE_INDEX;
            SetValue((long)StdTableEnum.STDTBL42_PASSWORD, anIndex1, Encoding.GetBytes(strQuaternaryPassword));
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_0, anIndex1, false);
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_1, anIndex1, false);
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_2, anIndex1, false);
            SetValue((long)StdTableEnum.STDTBL42_GROUP_PERM_3, anIndex1, true);

            m_CenTables.EncodeData(null, 22);
        }

        /// <summary>
        /// This method imports the TOU configuration into the file.
        /// </summary>
        /// <param name="strTOUFileName">The TOU schedule file name.</param>
        /// <param name="SeasonChgOption">The season change/demand reset option</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        // 12/01/10 deo 9.70.12        fix for CQ165091
        // 12/06/10 jrf 9.70.15        Adjusting for firmware change that requires two empty dst
        //                             events at the beginning of every calendar year.
        public override void ImportTOU(string strTOUFileName, SeasonChangeOptions SeasonChgOption)
        {
            CTOUSchedule TOUSchedule = new CTOUSchedule(strTOUFileName);
            int[] aiIndex1 = { 0 };
            int[] aiIndex2 = { 0, 0 };
            int[] aiIndex3 = { 0, 0, 0 };

            //TODO: Update CentronTblEnum values based on updates Steve makes...
            //Calendar ID
            m_CenTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_ID, null, TOUSchedule.TOUID);

            //DemandReset/Season Change Options
            m_CenTables.SetValue(CentronTblEnum.MFGTBL42_DEMAND_RESET, null, (byte)SeasonChgOption);

            //DST Hour
            m_CenTables.SetValue(CentronTblEnum.MFGTBL42_DST_HOUR, null, 0);
            //DST Minute
            m_CenTables.SetValue(CentronTblEnum.MFGTBL42_DST_MINUTE, null, 0);
            //DST Offset
            m_CenTables.SetValue(CentronTblEnum.MFGTBL42_DST_OFFSET, null, 0);

            //Calendar Years
            for (int iYear = 0; iYear < TOUSchedule.Years.Count; iYear++)
            {
                CYear Year = TOUSchedule.Years[iYear];
                aiIndex1[0] = iYear;
                aiIndex2[0] = iYear;

                //Year
                m_CenTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_YEAR, aiIndex1, Year.Year - 2000);

                //Leave two zero fill entries for unused DST slots
                for (int iEvent = 0; iEvent < UNUSED_DST_SLOTS; iEvent++)
                {
                    aiIndex2[1] = iEvent;

                    //Event
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_EVENT, aiIndex2, 0);
                    //Month
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_MONTH, aiIndex2, 0);
                    //Day Of Month
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_DAY_OF_MONTH, aiIndex2, 0);
                }
                
                //Day Events
                for (int iEvent = 0; iEvent < TOUSchedule.Years[iYear].Events.Count; iEvent++)
                {
                    CEvent Event = Year.Events[iEvent];
                    aiIndex2[1] = iEvent + UNUSED_DST_SLOTS;  //Start after the unused DST events
                    byte bytCalEvent = 0;

                    // Translate TOU schedule event to CalendarConfig event
                    // fix for CQ 165091

                    if (Itron.Metering.TOU.eEventType.HOLIDAY == Event.Type)
                    {
                        bytCalEvent = (byte)CalendarEvent.CalendarEventType.HOLIDAY + 1;
                    }
                    else if (Itron.Metering.TOU.eEventType.SEASON == Event.Type)
                    {
                         bytCalEvent = (byte)((int)CalendarEvent.CalendarEventType.SEASON1 + Event.Index + 1);
                    }
                    else
                    {
                         bytCalEvent = 0;
                    }
                    //Event
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_EVENT, aiIndex2, bytCalEvent);
                    //Month
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_MONTH, aiIndex2, Event.Date.Month - 1);
                    //Day Of Month
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_DAY_OF_MONTH, aiIndex2, Event.Date.Day - 1);
                }
            }

            //This value is the same for all seasons
            ushort usDayToDayType = GetTypicalWeek(TOUSchedule);

            //Seasons
            for (int iSeason = 0; iSeason < TOUSchedule.Seasons.Count; iSeason++)
            {
                CSeason Season = TOUSchedule.Seasons[iSeason];
                int iTODEvent = 0;
                int iPatternIndex = 0;
                bool blnEventsNeedSorting = false;
                TOUConfig.TOU_Season ConfigSeason = new TOUConfig.TOU_Season(24, 4);

                aiIndex1[SEASON_INDEX] = iSeason;
                aiIndex3[SEASON_INDEX] = iSeason;

                //Programmed
                m_CenTables.SetValue(CentronTblEnum.MFGTBL42_PROGRAMMED, aiIndex1, 1);

                //Day to Day Types
                m_CenTables.SetValue(CentronTblEnum.MFGTBL42_DAY_TO_DAY_TYPE, aiIndex1, usDayToDayType);

                // Configure the normal daytypes (0..2)
                for (int DaytypeIndex = 0;
                    DaytypeIndex < Season.NormalDays.Count;
                    DaytypeIndex++)
                {
                    // Get the index of the pattern in the schedule's 
                    // pattern collection that's assigned to this 
                    // daytype, so we can add its switchpoints
                    iPatternIndex = TOUSchedule.Patterns.SearchID(
                        Season.NormalDays[DaytypeIndex]);

                    iTODEvent = 0;
                    for (int EventIndex = 0;
                        EventIndex < TOUSchedule.Patterns[iPatternIndex].SwitchPoints.Count;
                        EventIndex++)
                    {
                        // Outputs switchpoints are a special case. A start and end 
                        // event must be added for outputs.
                        CSwitchPoint SP = TOUSchedule.Patterns[iPatternIndex].SwitchPoints[EventIndex];

                        ConfigSeason.TimeOfDayEvents[DaytypeIndex, iTODEvent++] = GetDayEvent(SP);

                        // Unlike rate switchpoints, outputs can overlap, so we 
                        // have to add the end point too.
                        if (eSwitchPointType.OUTPUT == SP.SwitchPointType)
                        {
                            ConfigSeason.TimeOfDayEvents[DaytypeIndex, iTODEvent++] = GetOutputOffEvent(SP);

                            // We'll need to sort the events since we added one
                            blnEventsNeedSorting = true;
                        }

                    } // For each switchpoint
                }

                // Configure the Holiday daytype if it exists
                if (0 != Season.Holidays.Count)
                {
                    // Get the index of the pattern in the schedule's 
                    // pattern collection that's assigned to this 
                    // (holiday) daytype, so we can add its switchpoints
                    iPatternIndex = TOUSchedule.Patterns.SearchID(Season.Holidays[0]);

                    iTODEvent = 0;
                    for (int EventIndex = 0;
                        EventIndex < TOUSchedule.Patterns[iPatternIndex].SwitchPoints.Count;
                        EventIndex++)
                    {
                        // Outputs switchpoints are a special case. A start and end 
                        // event must be added for outputs.
                        CSwitchPoint SP = TOUSchedule.Patterns[iPatternIndex].SwitchPoints[EventIndex];

                        ConfigSeason.TimeOfDayEvents[TOUConfig.HOLIDAY_TYPE_INDEX, iTODEvent++] = GetDayEvent(SP);

                        // Unlike rate switchpoints, outputs can overlap, so we 
                        // have to add the end point too.
                        if (eSwitchPointType.OUTPUT == SP.SwitchPointType)
                        {

                            ConfigSeason.TimeOfDayEvents[TOUConfig.HOLIDAY_TYPE_INDEX, iTODEvent++] = GetOutputOffEvent(SP);

                            // We'll need to sort the events since we added one
                            blnEventsNeedSorting = true;
                        }

                    } // For each switchpoint
                }

                // Sort the events if we added any
                if (blnEventsNeedSorting)
                {
                    ConfigSeason.Sort();
                }

                for (int DaytypeIndex = 0; DaytypeIndex < ConfigSeason.TimeOfDayEvents.GetLength(0); DaytypeIndex++)
                {
                    for (int EventIndex = 0; EventIndex < ConfigSeason.TimeOfDayEvents.GetLength(1);
                         EventIndex++)
                    {
                        TOUConfig.DayEvent DayEvt = ConfigSeason.TimeOfDayEvents[DaytypeIndex, EventIndex];

                        aiIndex3[DAY_TYPE_INDEX] = DaytypeIndex;
                        aiIndex3[DAY_TYPE_EVENT_INDEX] = EventIndex;

                        m_CenTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_RATE_EVENT, aiIndex3, DayEvt.EventType);
                        m_CenTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_HOUR, aiIndex3, DayEvt.Hour);
                        m_CenTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_MINUTE, aiIndex3, DayEvt.Minute);

                    }
                }

                if (0 != Season.Holidays.Count)
                {
                    for (int EventIndex = 0; EventIndex < ConfigSeason.TimeOfDayEvents.GetLength(1);
                         EventIndex++)
                    {
                        TOUConfig.DayEvent DayEvt = ConfigSeason.TimeOfDayEvents[TOUConfig.HOLIDAY_TYPE_INDEX, EventIndex];

                        aiIndex3[DAY_TYPE_INDEX] = TOUConfig.HOLIDAY_TYPE_INDEX;
                        aiIndex3[DAY_TYPE_EVENT_INDEX] = EventIndex;

                        m_CenTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_RATE_EVENT, aiIndex3, DayEvt.EventType);
                        m_CenTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_HOUR, aiIndex3, DayEvt.Hour);
                        m_CenTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_MINUTE, aiIndex3, DayEvt.Minute);
                    }
                }
            }
        }

        /// <summary>
        /// This method clears the TOU configuration from the file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void ClearTOU()
        {
            int[] aiIndex1 = { 0 };
            int[] aiIndex2 = { 0, 0 };
            int[] aiIndex3 = { 0, 0, 0 };

            //Calendar ID
            m_CenTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_ID, null, 0);

            //DemandReset/Season Change Options
            m_CenTables.SetValue(CentronTblEnum.MFGTBL42_DEMAND_RESET, null, 0);

            //DST Hour
            m_CenTables.SetValue(CentronTblEnum.MFGTBL42_DST_HOUR, null, 0);
            //DST Minute
            m_CenTables.SetValue(CentronTblEnum.MFGTBL42_DST_MINUTE, null, 0);
            //DST Offset
            m_CenTables.SetValue(CentronTblEnum.MFGTBL42_DST_OFFSET, null, 0);

            int iCurrentYear = DateTime.Now.Year - 2000;

            //Calendar Years
            for (int iYear = 0; iYear < 25; iYear++)
            {
                aiIndex1[0] = iYear;
                aiIndex2[0] = iYear;

                //Year
                m_CenTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_YEAR, aiIndex1, iCurrentYear + iYear);

                //Day Events
                for (int iEvent = 0; iEvent < 44; iEvent++)
                {
                    aiIndex2[1] = iEvent;

                    //Event
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_EVENT, aiIndex2, 0);
                    //Month
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_MONTH, aiIndex2, 0);
                    //Day Of Month
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_DAY_OF_MONTH, aiIndex2, 0);
                }
            }

            //Seasons
            for (int iSeason = 0; iSeason < 8; iSeason++)
            {
                //Set season
                aiIndex3[SEASON_INDEX] = iSeason;
                aiIndex1[SEASON_INDEX] = iSeason;

                //Programmed
                m_CenTables.SetValue(CentronTblEnum.MFGTBL42_PROGRAMMED, aiIndex1, 0);

                //Day to Day Types
                m_CenTables.SetValue(CentronTblEnum.MFGTBL42_DAY_TO_DAY_TYPE, aiIndex1, 0);

                // Configure the normal daytypes (0..2)
                for (int DaytypeIndex = 0;
                    DaytypeIndex < 4;
                    DaytypeIndex++)
                {
                    for (int EventIndex = 0;
                        EventIndex < 24;
                        EventIndex++)
                    {
                        aiIndex3[DAY_TYPE_INDEX] = DaytypeIndex;
                        aiIndex3[DAY_TYPE_EVENT_INDEX] = EventIndex;

                        m_CenTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_RATE_EVENT, aiIndex3, 0);
                        m_CenTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_HOUR, aiIndex3, 0);
                        m_CenTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_MINUTE, aiIndex3, 0);

                    } // For each switchpoint
                }
            }
        }

        /// <summary>
        /// This method imports DST schedule from the replica files into the file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        // 11/24/10 jrf 9.70.09        Changed DST month and day of month's to be zero based.
        //
        public override void ImportDST()
        {
            DST.CDSTSchedule DSTSchedule = new Itron.Metering.DST.CDSTSchedule();
            int iStartYear = DateTime.Now.Year;
            int[] anIndex1 = { 0 };
            int[] anIndex2 = { 0, 0 };

            //DST Hour
            m_CenTables.SetValue(CentronTblEnum.MFGTBL212_DST_HOUR, null, DSTSchedule.NextDSTToDate.Hour);

            //DST Minute
            m_CenTables.SetValue(CentronTblEnum.MFGTBL212_DST_MINUTE, null, DSTSchedule.NextDSTToDate.Minute);

            //DST Offset
            m_CenTables.SetValue(CentronTblEnum.MFGTB212_DST_OFFSET, null, DSTSchedule.JumpLength);

            //DST Date Config
            for (int i = 0; i < MAX_DST_YEARS; i++)
            {
                DST.CDSTDatePair DSTDatePair = null;

                try
                {
                    DSTDatePair = DSTSchedule.DSTDatePairs[DSTSchedule.DSTDatePairs.FindYear(i + iStartYear)];
                }
                catch
                {
                    DSTDatePair = null;
                }

                if (null != DSTDatePair)
                {
                    anIndex1[0] = i;
                    anIndex2[0] = i;

                    //DST Year
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL212_DST_CALENDAR_YEAR, anIndex1, DSTDatePair.FromDate.Year - 2000);

                    //DST On Event
                    anIndex2[1] = 0;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_EVENT, anIndex2, 1);
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_MONTH, anIndex2, DSTDatePair.ToDate.Month-1);
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_DAY_OF_MONTH, anIndex2, DSTDatePair.ToDate.Day-1);

                    //DST Off Event
                    anIndex2[1] = 1;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_EVENT, anIndex2, 2);
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_MONTH, anIndex2, DSTDatePair.FromDate.Month-1);
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_DAY_OF_MONTH, anIndex2, DSTDatePair.FromDate.Day-1);
                }
            }
        }

        /// <summary>
        /// This method clears DST schedule from the file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void ClearDST()
        {
            int iStartYear = DateTime.Now.Year;
            int[] anIndex1 = { 0 };
            int[] anIndex2 = { 0, 0 };

            //DST Hour
            m_CenTables.SetValue(CentronTblEnum.MFGTBL212_DST_HOUR, null, 0);

            //DST Minute
            m_CenTables.SetValue(CentronTblEnum.MFGTBL212_DST_MINUTE, null, 0);

            //DST Offset
            m_CenTables.SetValue(CentronTblEnum.MFGTB212_DST_OFFSET, null, 0);

            //DST Date Config
            for (int i = 0; i < MAX_DST_YEARS; i++)
            {
                anIndex1[0] = i;
                anIndex2[0] = i;

                //DST Year
                m_CenTables.SetValue(CentronTblEnum.MFGTBL212_DST_CALENDAR_YEAR, anIndex1, iStartYear + i - 2000);

                //DST On Event
                anIndex2[1] = 0;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_EVENT, anIndex2, 0);
                m_CenTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_MONTH, anIndex2, 0);
                m_CenTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_DAY_OF_MONTH, anIndex2, 0);

                //DST Off Event
                anIndex2[1] = 1;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_EVENT, anIndex2, 0);
                m_CenTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_MONTH, anIndex2, 0);
                m_CenTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_DAY_OF_MONTH, anIndex2, 0);
            }
        }

        /// <summary>
        /// This method sets non fatal error display options.
        /// </summary>
        /// <param name="NonFatalErr">The non-fatal error being referred to.</param>
        /// <param name="ErrorDispOpt">How to display the error.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void SetNonFatalErrorDisplayOptions(NonFatalErrors NonFatalErr, ErrorDisplayOptions ErrorDispOpt)
        {
            bool blnScroll = false;
            bool blnLock = false;
            CentronTblEnum ScrollItemID = CentronTblEnum.MFGTBL0_DISPLAY;
            CentronTblEnum LockItemID = CentronTblEnum.MFGTBL0_DISPLAY;

            //Set how we want the error displayed
            if (ErrorDispOpt == ErrorDisplayOptions.Scroll)
            {
                blnScroll = true;
            }
            else if (ErrorDispOpt == ErrorDisplayOptions.Lock)
            {
                blnLock = true;
            }

            //Determine what error we are dealing with
            switch (NonFatalErr)
            {
                case NonFatalErrors.LowBattery:
                    {
                        ScrollItemID = CentronTblEnum.MFGTBL0_SCROLL_LOW_BATTERY;
                        LockItemID = CentronTblEnum.MFGTBL0_LOCK_LOW_BATTERY;
                        m_strLowBatteryError.Flush();
                        break;
                    }
                case NonFatalErrors.LossOfPhase:
                    {
                        ScrollItemID = CentronTblEnum.MFGTBL0_SCROLL_LOSS_PHASE;
                        LockItemID = CentronTblEnum.MFGTBL0_LOCK_LOSS_PHASE;
                        m_strLossOfPhaseError.Flush();
                        break;
                    }
                case NonFatalErrors.ClockTOU:
                    {
                        ScrollItemID = CentronTblEnum.MFGTBL0_SCROLL_TOU_SCHEDULE_ERROR;
                        LockItemID = CentronTblEnum.MFGTBL0_LOCK_TOU_SCHEDULE_ERROR;
                        m_strClockTOUError.Flush();
                        break;
                    }
                case NonFatalErrors.ReversePowerFlow:
                    {
                        ScrollItemID = CentronTblEnum.MFGTBL0_SCROLL_REVERSE_POWER_FLOW;
                        LockItemID = CentronTblEnum.MFGTBL0_LOCK_REVERSE_POWER_FLOW;
                        m_strReversePowerError.Flush();
                        break;
                    }
                case NonFatalErrors.MassMemory:
                    {
                        ScrollItemID = CentronTblEnum.MFGTBL0_SCROLL_MASS_MEMORY;
                        LockItemID = CentronTblEnum.MFGTBL0_LOCK_MASS_MEMORY;
                        m_strLoadProfileError.Flush();
                        break;
                    }
                case NonFatalErrors.RegisterFullScale:
                    {
                        ScrollItemID = CentronTblEnum.MFGTBL0_SCROLL_REGISTER_FULL_SCALE;
                        LockItemID = CentronTblEnum.MFGTBL0_LOCK_REGISTER_FULL_SCALE;
                        m_strFullScaleError.Flush();
                        break;
                    }
                case NonFatalErrors.Sitescan:
                    {
                        ScrollItemID = CentronTblEnum.MFGTBL0_SCROLL_SITESCAN_ERROR;
                        LockItemID = CentronTblEnum.MFGTBL0_LOCK_SITESCAN_ERROR;
                        m_strSiteScanError.Flush();
                        break;
                    }
                default:
                    break;
            }

            //These IDs should be different. If they are not then this means they were not updated correctly
            //and we should not set them.
            if (ScrollItemID != LockItemID)
            {
                //Set error scroll value
                SetMFGEDLBool(blnScroll, ScrollItemID);

                //Set error lock value
                SetMFGEDLBool(blnLock, LockItemID);
            }
        }

        /// <summary>
        /// This method enables/disables the history log event in the configuration.
        /// </summary>
        /// <param name="iEvent"></param>
        /// <param name="blnEnabled"></param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void SetEvent(int iEvent, bool blnEnabled)
        {
            object objValue = blnEnabled;
            int[] aiIndex = { iEvent };

            m_CenTables.SetValue(CentronTblEnum.MFGTBL0_HISTORY_LOG_EVENTS, aiIndex, objValue);
        }

        /// <summary>
        /// This method removes an item from the display list.
        /// </summary>
        /// <param name="DisplayList">Specifies which display list to remove the item from.</param>
        /// <param name="iIndex">The list specific index of the item to rmemove.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void RemoveDisplayItem(DisplayListType DisplayList, int iIndex)
        {
            byte bytNormalEnd = 0;
            byte bytAlternateStart = 0;
            byte bytAlternateEnd = 0;
            byte bytTestStart = 0;
            byte bytTestEnd = 0;
            int[] aiIndex = { 0 };
            bool blnUpdateNormal = false;
            bool blnUpdateAlternate = false;
            List<OpenWayDisplayItem> NormalDisplay = NormalDisplayConfig;
            List<OpenWayDisplayItem> AlternateDisplay = AlternateDisplayConfig;
            object objValue;

            //Get starts/stops for all display lists except of normal start since it will always be 0
            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_NORMAL_LIST_STOP, null, out objValue);
            bytNormalEnd = (byte)objValue;
            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_ALT_LIST_START, null, out objValue);
            bytAlternateStart = (byte)objValue;
            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_ALT_LIST_STOP, null, out objValue);
            bytAlternateEnd = (byte)objValue;
            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_TEST_LIST_START, null, out objValue);
            bytTestStart = (byte)objValue;
            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_TEST_LIST_STOP, null, out objValue);
            bytTestEnd = (byte)objValue;

            //Remove the last item
            aiIndex[0] = (int)bytTestEnd;
            m_CenTables.SetValue((long)CentronTblEnum.MFGTBL0_DISPLAY_ITEMS, aiIndex, null);

            //Remove the item from the appropriate list
            if (DisplayList == DisplayListType.Normal)
            {
                NormalDisplay.RemoveAt(iIndex);
                blnUpdateNormal = true;
                blnUpdateAlternate = true;
            }
            else if (DisplayList == DisplayListType.Alternate)
            {
                AlternateDisplay.RemoveAt(iIndex);
                blnUpdateAlternate = true;
            }

            //Update the start/stops based on which list item was removed from
            if (true == blnUpdateNormal)
            {
                bytNormalEnd--;
                objValue = bytNormalEnd;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL0_NORMAL_LIST_STOP, null, objValue);
                bytAlternateStart--;
                objValue = bytAlternateStart;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL0_ALT_LIST_START, null, objValue);
            }

            if (true == blnUpdateAlternate)
            {
                bytAlternateEnd--;
                objValue = bytAlternateEnd;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL0_ALT_LIST_STOP, null, objValue);
                bytTestStart--;
                objValue = bytTestStart;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL0_TEST_LIST_START, null, objValue);
            }

            bytTestEnd--;
            objValue = bytTestEnd;
            m_CenTables.SetValue(CentronTblEnum.MFGTBL0_TEST_LIST_STOP, null, objValue);

            //Save all normal items in their new places
            for (int i = 0; i < NormalDisplay.Count; i++)
            {
                SetDisplayItem(DisplayListType.Normal, i, NormalDisplay[i]);
            }

            //Save all alternate items in their new places
            for (int i = 0; i < AlternateDisplay.Count; i++)
            {
                SetDisplayItem(DisplayListType.Alternate, i, AlternateDisplay[i]);
            }

        }

        /// <summary>
        /// This method adds an item to the display list.
        /// </summary>
        /// <param name="DisplayList">Specifies which display list to remove the item from.</param>
        /// <param name="NewDisplayItem">The display item to be added.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void AddDisplayItem(DisplayListType DisplayList, ANSIDisplayItem NewDisplayItem)
        {
            byte bytNormalEnd = 0;
            byte bytAlternateStart = 0;
            byte bytAlternateEnd = 0;
            byte bytTestStart = 0;
            byte bytTestEnd = 0;
            object objValue;
            bool blnUpdateNormal = false;
            bool blnUpdateAlternate = false;
            List<OpenWayDisplayItem> NormalDisplay = NormalDisplayConfig;
            List<OpenWayDisplayItem> AlternateDisplay = AlternateDisplayConfig;

            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_NORMAL_LIST_STOP, null, out objValue);
            bytNormalEnd = (byte)objValue;
            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_ALT_LIST_START, null, out objValue);
            bytAlternateStart = (byte)objValue;
            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_ALT_LIST_STOP, null, out objValue);
            bytAlternateEnd = (byte)objValue;
            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_TEST_LIST_START, null, out objValue);
            bytTestStart = (byte)objValue;
            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_TEST_LIST_STOP, null, out objValue);
            bytTestEnd = (byte)objValue;

            switch (DisplayList)
            {
                case DisplayListType.Normal:
                    {
                        blnUpdateNormal = true;
                        blnUpdateAlternate = true;
                        break;
                    }
                case DisplayListType.Alternate:
                    {
                        blnUpdateAlternate = true;
                        break;
                    }
                default:
                    break;
            }

            if (true == blnUpdateNormal)
            {
                bytNormalEnd++;
                objValue = bytNormalEnd;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL0_NORMAL_LIST_STOP, null, objValue);
                bytAlternateStart++;
                objValue = bytAlternateStart;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL0_ALT_LIST_START, null, objValue);
            }

            if (true == blnUpdateAlternate)
            {
                bytAlternateEnd++;
                objValue = bytAlternateEnd;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL0_ALT_LIST_STOP, null, objValue);
                bytTestStart++;
                objValue = bytTestStart;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL0_TEST_LIST_START, null, objValue);
            }

            bytTestEnd++;
            objValue = bytTestEnd;
            m_CenTables.SetValue(CentronTblEnum.MFGTBL0_TEST_LIST_STOP, null, objValue);

            //Create the new display item
            DefinedLIDs LIDs = new DefinedLIDs();
            ANSIDisplayItem DefaultDisplayItem = new ANSIDisplayItem(LIDs.SEGMENT_TEST, "", (ushort)ANSIDisplayItem.DisplayType.ALL_SEGMENTS, 0);
            SetDisplayItem(DisplayListType.Test, 0, DefaultDisplayItem);

            if (DisplayList == DisplayListType.Normal)
            {
                //Add the new item to the end of the normal display list
                SetDisplayItem(DisplayListType.Normal, (int)bytNormalEnd, NewDisplayItem);

                //Save all alternate items in their new places
                for (int i = 0; i < AlternateDisplay.Count; i++)
                {
                    SetDisplayItem(DisplayListType.Alternate, i, AlternateDisplay[i]);
                }
            }
            else if (DisplayList == DisplayListType.Alternate)
            {
                //Add the new item to the end of the alternate display list
                SetDisplayItem(DisplayListType.Alternate, (int)(bytAlternateEnd - bytAlternateStart), NewDisplayItem);
            }
        }

        /// <summary>
        /// This method sets the LID for a particular display item.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="lidDisplayItem">The LID to of the display item to update</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void SetDisplayItemLID(DisplayListType DisplayList, int iListSpecificIndex, LID lidDisplayItem)
        {
            object objValue = lidDisplayItem.lidValue;
            int[] aiIndex = { GetDisplayItemIndex(DisplayList, iListSpecificIndex) };

            m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DISPLAY_LID, aiIndex, objValue);
        }

        /// <summary>
        /// This method sets the display item's demand type.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="bytDemandType">The demand type of the display item to update</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void SetDisplayItemDemandType(DisplayListType DisplayList, int iListSpecificIndex, byte bytDemandType)
        {
            object objValue = bytDemandType;
            int[] aiIndex = { GetDisplayItemIndex(DisplayList, iListSpecificIndex) };

            m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DISPLAY_DEMAND_TYPE, aiIndex, objValue);
        }

        /// <summary>
        /// This method sets the display item's rate.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="bytRate">the rate of the display item to update</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void SetDisplayItemRate(DisplayListType DisplayList, int iListSpecificIndex, byte bytRate)
        {
            object objValue = bytRate;
            int[] aiIndex = { GetDisplayItemIndex(DisplayList, iListSpecificIndex) };

            m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DISPLAY_RATE, aiIndex, objValue);
        }

        /// <summary>
        /// This method sets whether the display items will round values.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="blnRound">Whether or not to round.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void SetDisplayItemRound(DisplayListType DisplayList, int iListSpecificIndex, bool blnRound)
        {
            object objValue = blnRound;
            int[] aiIndex = { GetDisplayItemIndex(DisplayList, iListSpecificIndex) };

            m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DISPLAY_ROUND, aiIndex, objValue);
        }

        /// <summary>
        /// This method gets the generic index of the display item.
        /// </summary>
        /// <param name="DisplayList">The list of the display item</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <returns>The generic index of the display item.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        protected override int GetDisplayItemIndex(DisplayListType DisplayList, int iListSpecificIndex)
        {
            int iDisplayListOffset = 0;
            object objValue;
            CentronTblEnum DisplayListTableID = CentronTblEnum.MFGTBL0_NORMAL_LIST_START;

            switch (DisplayList)
            {
                case DisplayListType.Normal:
                    {
                        DisplayListTableID = CentronTblEnum.MFGTBL0_NORMAL_LIST_START;
                        break;
                    }
                case DisplayListType.Alternate:
                    {
                        DisplayListTableID = CentronTblEnum.MFGTBL0_ALT_LIST_START;
                        break;
                    }
                case DisplayListType.Test:
                    {
                        DisplayListTableID = CentronTblEnum.MFGTBL0_TEST_LIST_START;
                        break;
                    }
            }

            m_CenTables.GetValue(DisplayListTableID, null, out objValue);
            iDisplayListOffset = Convert.ToInt32(objValue, CultureInfo.InvariantCulture);

            return iDisplayListOffset + iListSpecificIndex;
        }

        /// <summary>
        /// This method sets the display item's ID.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="strID">The ID of the display item</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void SetDisplayItemID(DisplayListType DisplayList, int iListSpecificIndex, string strID)
        {
            object objValue = strID;
            int[] aiIndex = { GetDisplayItemIndex(DisplayList, iListSpecificIndex) };

            m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DISPLAY_USERID, aiIndex, objValue);
        }

        /// <summary>
        /// This method sets the display item's decimal digits.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="iDecimalDigits">The display item's decimal digits</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void SetDisplayDecimalDigits(DisplayListType DisplayList, int iListSpecificIndex, int iDecimalDigits)
        {
            object objValue = iDecimalDigits;
            int[] aiIndex = { GetDisplayItemIndex(DisplayList, iListSpecificIndex) };

            m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DISPLAY_DECIMAL_PLACES, aiIndex, objValue);
        }

        /// <summary>
        /// This method sets the display item's total digits.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="iTotalDigits">The display item's total digits</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void SetDisplayTotalDigits(DisplayListType DisplayList, int iListSpecificIndex, int iTotalDigits)
        {
            object objValue = iTotalDigits;
            int[] aiIndex = { GetDisplayItemIndex(DisplayList, iListSpecificIndex) };

            m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DISPLAY_DIGITS_TOTAL, aiIndex, objValue);
        }

        /// <summary>
        /// This method sets the display item's units.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="bytUnits">The display item's units.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void SetDisplayUnits(DisplayListType DisplayList, int iListSpecificIndex, byte bytUnits)
        {
            object objValue = bytUnits;
            int[] aiIndex = { GetDisplayItemIndex(DisplayList, iListSpecificIndex) };

            m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DISPLAY_UNITS, aiIndex, objValue);
        }

        /// <summary>
        /// This method sets the display item's type.
        /// </summary>
        /// <param name="DisplayList">The list to update</param>
        /// <param name="iListSpecificIndex">The list specific index of the item</param>
        /// <param name="bytType">The display item's type.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        //
        public override void SetDisplayType(DisplayListType DisplayList, int iListSpecificIndex, byte bytType)
        {
            object objValue = bytType;
            int[] aiIndex = { GetDisplayItemIndex(DisplayList, iListSpecificIndex) };

            m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DISPLAY_TYPE, aiIndex, objValue);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Determines if the file is a program file
        /// </summary>
        /// <returns>true if the file is a program; false, otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/23/08 AF  10.0           Created for OpenWay Data Manager
        //  10/07/09 RCG 2.30.07 141466 Changed to a public property
        //  06/09/10 AF  2.41.08        M2 Gateway config files do not have table 2048 items so
        //                              we need another field to identify the file as a config file
        //  08/03/10 AF  2.42.11        Updated support for Gateway
        //
        public override bool IsProgramFile
        {
            get
            {
                bool bIsPgm = false;

                // Check on the existence of a field that will always be in a program
                // but not in a TOU or data file
                if (m_CenTables != null &&
                    (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DEMAND_INTERVAL_LENGTH, null) ||
                    m_CenTables.IsCached((long)CentronTblEnum.MFGTBL2047_ENCRYPTION_TYPE, null)))
                {
                    // We now know that the the file has configuration.  We only want this
                    // to return true if is not a data file, which also contains configuration
                    if (!m_CenTables.IsCached((long)StdTableEnum.STDTBL1_MFG_SERIAL_NUMBER, null))
                    {
                        bIsPgm = true;
                    }
                }
                return bIsPgm;
            }
        }//IsEDLProgramFile

        /// <summary>
        /// Determines whether the file is a TOU schedule file
        /// </summary>
        /// <returns>true if the file is a TOU schedule; false, otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/23/08 AF  10.0           Created for OpenWay Data Manager
        //  10/07/09 RCG 2.30.07 141466 Changed to a public property
        //  06/09/10 AF  2.41.08        M2 Gateway config files do not have table 2048 items so
        //                              we need another field to identify the file as a config file
        //
        public override bool IsTOUFile
        {
            get
            {
                bool bIsTOU = false;

                if (m_CenTables != null && m_CenTables.IsCached((long)StdTableEnum.STDTBL6_TARIFF_ID, null))
                {
                    // At this point, we have a file that is an EDL file and
                    // contains TOU.  Make sure it's not just a program file
                    if ((!(m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DEMAND_INTERVAL_LENGTH, null))) ||
                        (!(m_CenTables.IsCached((long)CentronTblEnum.MFGTBL2047_ENCRYPTION_TYPE, null))))
                    {
                        bIsTOU = true;
                    }
                }

                return bIsTOU;
            }
        }

        /// <summary>
        /// Return true if the EDL file contains Load Profile data. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/11/07 RCG 1.00.00        Created
        public override bool ContainsLoadProfile
        {
            get
            {
                bool bContainsLP = false;
                object objValue = null;
                byte byChannels = 0;
                ushort usUsedBlocks = 0;
                ushort usUsedIntervals = 0;

                if (m_CenTables != null)
                {
                    if (m_CenTables.IsCached((long)StdTableEnum.STDTBL61_NBR_CHNS_SET1, null))
                    {
                        // Make sure that the LP Data contains some channels
                        m_CenTables.GetValue(StdTableEnum.STDTBL61_NBR_CHNS_SET1, null, out objValue);
                        byChannels = (byte)objValue;

                        if (byChannels > 0)
                        {
                            // Now make sure that there are valid blocks
                            if (m_CenTables.IsCached((long)StdTableEnum.STDTBL63_NBR_VALID_BLOCKS, null))
                            {
                                m_CenTables.GetValue(StdTableEnum.STDTBL63_NBR_VALID_BLOCKS, null, out objValue);
                                usUsedBlocks = (ushort)objValue;

                                if (usUsedBlocks > 0)
                                {
                                    // Finally make sure that there are intervals present.
                                    if (m_CenTables.IsCached((long)StdTableEnum.STDTBL63_NBR_VALID_INT, null))
                                    {
                                        m_CenTables.GetValue(StdTableEnum.STDTBL63_NBR_VALID_INT, null, out objValue);
                                        usUsedIntervals = (ushort)objValue;

                                        if (usUsedIntervals > 0)
                                        {
                                            bContainsLP = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return bContainsLP;
            }
        }

        /// <summary>
        /// Returns true if the EDL file contains Voltage Monitoring data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/08 RCG 1.50.23 N/A    Created

        public override bool ContainsVoltageMonitoring
        {
            get
            {
                bool bContainsVMData = false;
                bool bVMEnabled = false;
                object objValue;
                ushort usUsedBlocks;

                if (m_CenTables != null)
                {
                    if(m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_ENABLE_FLAG, null))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL102_ENABLE_FLAG, null, out objValue);
                        bVMEnabled = (bool)objValue;

                        if (bVMEnabled == true)
                        {
                            // Make sure there are valid blocks
                            if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL103_NBR_VALID_BLOCKS, null))
                            {
                                m_CenTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_BLOCKS, null, out objValue);
                                usUsedBlocks = (ushort)objValue;

                                if (usUsedBlocks > 0)
                                {
                                    bContainsVMData = true;
                                }
                            }
                        }
                    }
                }
                return bContainsVMData;
            }
        }

       
        
		/// <summary>
        /// Return true if the EDL file contains Configuration data. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/11/07 RCG 1.00.00        Created
        //  06/09/10 AF  2.41.08        M2 Gateway config files do not have table 2048 items so
        //                              we need another field to identify the file as a config file
        //
        public override bool ContainsConfiguration
        {
            get
            {
                bool bContainsConfigurationData = false;

                if ((m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DEMAND_INTERVAL_LENGTH, null)) ||
                    ((m_CenTables.IsCached((long)CentronTblEnum.MFGTBL2047_ENCRYPTION_TYPE, null))))
                {
                    bContainsConfigurationData = true;
                }

                return bContainsConfigurationData;
            }
        }

      

        /// <summary>
        /// Reads the standard table 06 tariff id out of the EDL file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#         Description
        //  -------- --- ------- -------------- -------------------------------------------
        //  01/21/08 AF  10.0                   Created for OpenWay DataManager
        //  06/11/08 KRC 1.50.34 itron00116044  TOU ID does not make sense so just show it is enabled.
        //
        public override string TOUScheduleID
        {
            get
            {
                if (!m_strTOUID.Cached)
                {
                    string TOUID;
                    TOUID = GetSTDEDLString(StdTableEnum.STDTBL6_TARIFF_ID);

                    // Remove any nulls that might be at the end
                    TOUID = TOUID.TrimEnd('\0');
                    m_strTOUID.Value = TOUID.Length > 0 ? TOUID : "";
                }
                return m_strTOUID.Value;
            }
        }

       
        /// <summary>
        /// Returns true of the EDL has Device Status Data in it.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public override bool ContainsDeviceStatus
        {
            get
            {
                bool bContainsDeviceStatus = false;

                if (m_CenTables.IsCached((long)StdTableEnum.STDTBL1_MFG_SERIAL_NUMBER, null))
                {
                    bContainsDeviceStatus = true;
                }

                return bContainsDeviceStatus;
            }
        }

        /// <summary>
        /// Gets/sets the Min Time to elapse before marking a Power Outage
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.11        Added set.
        //
        public override int LPMinPowerOutage
        {
            get
            {
                if (!m_iLPMinPowerOutage.Cached)
                {
                    m_iLPMinPowerOutage.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_LP_MIN_POWER_OUTAGE);
                }

                return m_iLPMinPowerOutage.Value;
            }
            set
            {
                m_iLPMinPowerOutage.Value = value;

                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_LP_MIN_POWER_OUTAGE);
            }
        }

        /// <summary>
        /// Returns the load profile memory size
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //
        public override int LPMemorySize
        {
            get
            {
                if (!m_iLPMemorySize.Cached)
                {
                    m_iLPMemorySize.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_LP_MEMORY_SIZE);
                }

                return m_iLPMemorySize.Value;
            }
        }

        /// <summary>
        /// Returns the Clock Synchronziation method
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  
        //
        public override string ClockSynch
        {
            get
            {
                if (!m_strClockSynch.Cached)
                {
                    int iClockSynch = GetMFGEDLInt(CentronTblEnum.MFGTBL0_CLOCK_SYNC);
                    if (iClockSynch == 0)
                    {
                        m_strClockSynch.Value = CRYSTAL_SYNC;
                    }
                    else
                    {
                        m_strClockSynch.Value = LINE_SYNC;
                    }
                }

                return m_strClockSynch.Value;
            }
        }

        /// <summary>
        /// Sets the clock synchronization method.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created
        //
        public override ClockSynchronization ClockSynchValue
        {
            set
            {
                switch (value)
                {
                    case ClockSynchronization.Crystal:
                        {
                            m_strClockSynch.Value = CRYSTAL_SYNC;
                            SetMFGEDLInt((int)value, CentronTblEnum.MFGTBL0_CLOCK_SYNC);
                            break;
                        }
                    default: //Assume all other are line synchronization
                        {
                            m_strClockSynch.Value = LINE_SYNC;
                            SetMFGEDLInt((int)value, CentronTblEnum.MFGTBL0_CLOCK_SYNC);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Gets/sets if the EOI indicator is set to be displayed
        /// </summary>
        public override bool DisplayEOI
        {
            get
            {
                if (!m_blnDisplayEOI.Cached)
                {
                    m_blnDisplayEOI.Value = GetMFGEDLBool(CentronTblEnum.MFGTBL0_DISPLAY_EOI);
                }

                return m_blnDisplayEOI.Value;
            }
            set
            {
                SetMFGEDLBool(value, CentronTblEnum.MFGTBL0_DISPLAY_EOI);
                m_blnDisplayEOI.Value = value;
            }
        }

        /// <summary>
        /// Gets/sets if the Watt Load Indicator is set to be displayed
        /// </summary>
        public override bool DisplayWattLoadIndicator
        {
            get
            {
                if (!m_blnWattIndicator.Cached)
                {
                    m_blnWattIndicator.Value = GetMFGEDLBool(CentronTblEnum.MFGTBL0_WATT_LOAD_INDICATOR);
                }

                return m_blnWattIndicator.Value;
            }
            set
            {
                SetMFGEDLBool(value, CentronTblEnum.MFGTBL0_WATT_LOAD_INDICATOR);
                m_blnWattIndicator.Value = value;
            }
        }

        /// <summary>
        /// Tells us if the Remote Disconnect OFF Message is Enabled
        /// </summary>
        public override bool DisplayRemoteDisconnectOFFMessage
        {
            get
            {
                if (!m_blnDisonnectOFFMessage.Cached)
                {
                    m_blnDisonnectOFFMessage.Value = GetMFGEDLBool(CentronTblEnum.MFGTBL0_DISPLAY_REMOTE_DISCONNECT_MESSAGE_FLAG);
                }

                return m_blnDisonnectOFFMessage.Value;
            }
            set
            {
                SetMFGEDLBool(value, CentronTblEnum.MFGTBL0_DISPLAY_REMOTE_DISCONNECT_MESSAGE_FLAG);
                m_blnDisonnectOFFMessage.Value = true;
            }
        }

        /// <summary>
        /// Returns the Time each display Item spends on display in seconds
        /// </summary>
        public override int DisplayOnTime
        {
            get
            {
                if (!m_iDisplayOnTime.Cached)
                {
                    m_iDisplayOnTime.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_ITEM_DISPLAY_TIME);
                    m_iDisplayOnTime.Value = m_iDisplayOnTime.Value / 4;
                }

                return m_iDisplayOnTime.Value;
            }
            set
            {
                int iDisplayOnTime = value * 4;
                SetMFGEDLInt(iDisplayOnTime, CentronTblEnum.MFGTBL0_ITEM_DISPLAY_TIME);
                m_iDisplayOnTime.Value = value;
            }
        }

        /// <summary>
        /// Get the setting for the Low Battery Error
        /// </summary>
        public override string LowBatteryError
        {
            get
            {
                if (!m_strLowBatteryError.Cached)
                {
                    if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_SCROLL_LOW_BATTERY))
                    {
                        m_strLowBatteryError.Value = "Scroll";
                    }
                    else if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_LOCK_LOW_BATTERY))
                    {
                        m_strLowBatteryError.Value = "Lock";
                    }
                    else
                    {
                        m_strLowBatteryError.Value = "Ignore";
                    }
                }

                return m_strLowBatteryError.Value;
            }
        }

        /// <summary>
        /// Get the setting for the Loss of Phase Error
        /// </summary>
        public override string LossOfPhaseError
        {
            get
            {
                if (!m_strLossOfPhaseError.Cached)
                {
                    if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_SCROLL_LOSS_PHASE))
                    {
                        m_strLossOfPhaseError.Value = "Scroll";
                    }
                    else if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_LOCK_LOSS_PHASE))
                    {
                        m_strLossOfPhaseError.Value = "Lock";
                    }
                    else
                    {
                        m_strLossOfPhaseError.Value = "Ignore";
                    }
                }

                return m_strLossOfPhaseError.Value;
            }
        }

        /// <summary>
        /// Get the setting of the Clock TOU Error
        /// </summary>
        public override string ClockTOUError
        {
            get
            {
                if (!m_strClockTOUError.Cached)
                {
                    if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_SCROLL_TOU_SCHEDULE_ERROR))
                    {
                        m_strClockTOUError.Value = "Scroll";
                    }
                    else if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_LOCK_TOU_SCHEDULE_ERROR))
                    {
                        m_strClockTOUError.Value = "Lock";
                    }
                    else
                    {
                        m_strClockTOUError.Value = "Ignore";
                    }
                }

                return m_strClockTOUError.Value;
            }
        }

        /// <summary>
        /// Get the setting for the Reverse Power Flow Error
        /// </summary>
        public override string ReversePowerError
        {
            get
            {
                if (!m_strReversePowerError.Cached)
                {
                    if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_SCROLL_REVERSE_POWER_FLOW))
                    {
                        m_strReversePowerError.Value = "Scroll";
                    }
                    else if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_LOCK_REVERSE_POWER_FLOW))
                    {
                        m_strReversePowerError.Value = "Lock";
                    }
                    else
                    {
                        m_strReversePowerError.Value = "Ignore";
                    }
                }

                return m_strReversePowerError.Value;
            }
        }

        /// <summary>
        /// Get the setting for the Load Profile (Mass Memory) error
        /// </summary>
        public override string LoadProfileError
        {
            get
            {
                if (!m_strLoadProfileError.Cached)
                {
                    if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_SCROLL_MASS_MEMORY))
                    {
                        m_strLoadProfileError.Value = "Scroll";
                    }
                    else if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_LOCK_MASS_MEMORY))
                    {
                        m_strLoadProfileError.Value = "Lock";
                    }
                    else
                    {
                        m_strLoadProfileError.Value = "Ignore";
                    }
                }

                return m_strLoadProfileError.Value;
            }
        }

        /// <summary> 
        /// Gets the Time Zone applied Flag. 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/08 jrf                Created.
        public override bool TimeZoneEnabled
        {
            get
            {
                if (!m_blnTimeZoneEnabled.Cached)
                {
                    m_blnTimeZoneEnabled.Value = GetSTDEDLBool(StdTableEnum.STDTBL52_TM_ZN_APPLIED_FLAG);
                }

                return m_blnTimeZoneEnabled.Value;
            }
        }

        /// <summary> 
        /// Gets the current day of the week cooresponding to the current 
        /// date of the device.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/08 jrf                Created.
        public override DaysOfWeek DayOfWeek
        {
            get
            {
                if (DaysOfWeek.UNREAD == m_eDayOfWeek)
                {
                    m_eDayOfWeek = (DaysOfWeek)GetSTDEDLInt(StdTableEnum.STDTBL52_DAY_OF_WEEK);
                }

                return m_eDayOfWeek;
            }
        }
        /// <summary> 
        /// Gets the GMT Flag. 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/08 jrf                Created.
        public override bool DeviceInGMT
        {
            get
            {
                if (!m_blnDeviceInGMT.Cached)
                {
                    m_blnDeviceInGMT.Value = GetSTDEDLBool(StdTableEnum.STDTBL52_GMT_FLAG);
                }

                return m_blnDeviceInGMT.Value;
            }
        }


        /// <summary>
        /// Get the Setting for the Register Full Scale Error
        /// </summary>
        public override string FullScaleError
        {
            get
            {
                if (!m_strFullScaleError.Cached)
                {
                    if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_SCROLL_REGISTER_FULL_SCALE))
                    {
                        m_strFullScaleError.Value = "Scroll";
                    }
                    else if (GetMFGEDLBool(CentronTblEnum.MFGTBL0_LOCK_REGISTER_FULL_SCALE))
                    {
                        m_strFullScaleError.Value = "Lock";
                    }
                    else
                    {
                        m_strFullScaleError.Value = "Ignore";
                    }
                }

                return m_strFullScaleError.Value;
            }
        }

        /// <summary>
        /// Gets/sets the season change options for the TOU schedule.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/17/10 jrf 2.45.13 N/A    Created
        //
        public override SeasonChangeOptions SeasonChangeOption
        {
            get
            {
                object objValue;

                m_CenTables.GetValue(CentronTblEnum.MFGTBL42_DEMAND_RESET, null, out objValue);

                return (SeasonChangeOptions)objValue;
            }
            set
            {
                m_CenTables.SetValue(CentronTblEnum.MFGTBL42_DEMAND_RESET, null, (byte)value);
            }
        }

        
        /// <summary>
        /// Gets the TOU Schedule
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/12/08 KRC                Adding Current TOU Schedule to EDL Translation
        //  04/11/08 RCG 1.50.16        Fixing issue with displaying TOU Schedules from program/TOU files
        public override CTOUSchedule TOUSchedule
        {
            get
            {
                CTOUSchedule TOUSchedule = null;
                MemoryStream TOUStream = new MemoryStream();
                CENTRON2_MONO_CalendarConfig CalendarConfig = null;
                CENTRON2_MONO_TOUConfig TOUConfig = null;

                if (ContainsTOU)
                {
                    // First determine if the TOU is in Table 2090 (meter program file)
                    if (m_CenTables.IsAllCached(2090))
                    {
                        DateTime dtSeasonStart;
                        DateTime dtNextSeasonStart;
                        PSEMBinaryReader Reader;
                        bool bDemandReset;
                        bool bSelfRead;

                        // We must have a program file so we need to move it to 2090 first
                        m_CenTables.UpdateTOUSeasonFromStandardTables(DateTime.Now, 0, out dtSeasonStart, out bDemandReset, 
                                                      out bSelfRead, out dtNextSeasonStart);

                        m_CenTables.BuildPSEMStream(2090, TOUStream, 0, 
                            (CENTRON2_MONO_CalendarConfig.CENTRON2_MONO_CAL_SIZE + CENTRON_AMI_TOUConfig.TOU_CONFIG_SIZE));
                        Reader = new PSEMBinaryReader(TOUStream);

                        CalendarConfig = new CENTRON2_MONO_CalendarConfig(Reader, 0, CENTRON2_MONO_CalendarConfig.CENTRON2_MONO_CAL_SIZE, 
                                                CENTRON2_MONO_CalendarConfig.CENTRON2_MONO_CAL_YEARS);
                        TOUConfig = new CENTRON2_MONO_TOUConfig(Reader, CENTRON2_MONO_CalendarConfig.CENTRON2_MONO_CAL_SIZE);
                    }
                    else // then read TOU from Table 2048 (meter data file)
                    {
                        if (ConfigHeader.CalendarOffset != 0)
                        {
                            m_CenTables.BuildPSEMStream(2048, TOUStream, ConfigHeader.CalendarOffset,
                               (CENTRON2_MONO_CalendarConfig.CENTRON2_MONO_CAL_SIZE + CENTRON_AMI_TOUConfig.TOU_CONFIG_SIZE));
                            PSEMBinaryReader TOUReader = new PSEMBinaryReader(TOUStream);

                            CalendarConfig = new CENTRON2_MONO_CalendarConfig(TOUReader, 0, 
                                                    CENTRON2_MONO_CalendarConfig.CENTRON2_MONO_CAL_SIZE,
                                                    CENTRON_AMI_CalendarConfig.CENTRON_AMI_CAL_YEARS);
                            TOUConfig = new CENTRON2_MONO_TOUConfig(TOUReader,
                                                                    CENTRON2_MONO_CalendarConfig.CENTRON2_MONO_CAL_SIZE);
                        }
                    }
                    TOUSchedule = CENTRON2_MONO.ReadCENTRON2TOUSchedule(TOUConfig, CalendarConfig);
                }

                return TOUSchedule;
            }
        }

        /// <summary>
        /// Get the Normal Display Configuration List
        /// </summary>
        public override List<OpenWayDisplayItem> NormalDisplayConfig
        {
            get
            {
//TODO - add support for M2 Gateway
                IList<TableData> DisplayTables = m_CenTables.BuildPSEMStreams(2048, ConfigHeader.DisplayOffset, CENTRON2_MONO_DisplayConfig.DISPLAY_CONFIG_SIZE);
                List<OpenWayDisplayItem> DisplayItems = new List<OpenWayDisplayItem>();

                //Assembly the multiple streams that I received to one stream that I can use.
                Stream DisplayStream = BuildStream(DisplayTables, ConfigHeader.DisplayOffset, CENTRON2_MONO_DisplayConfig.DISPLAY_CONFIG_SIZE);

                PSEMBinaryReader DisplayReader = new PSEMBinaryReader(DisplayStream);
                CENTRON2_MONO_DisplayConfig DisplayConfigTable = new CENTRON2_MONO_DisplayConfig(DisplayReader, ConfigHeader.DisplayOffset);

                foreach (ANSIDisplayData DisplayData in DisplayConfigTable.NormalDisplayData)
                {
                    DisplayItems.Add(new OpenWayDisplayItem(new CentronMonoLID(DisplayData.NumericLID),
                        DisplayData.DisplayID, DisplayData.DisplayFormat, DisplayData.DisplayDimension));
                }

                return DisplayItems;
            }
        }

        /// <summary>
        /// Gets Actual Meter Value for Device Class
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#          Description
        // -------- --- ------- --------------  ---------------------------------------
        // 07/04/08 KRC	1.51.02	itron00117146	Adding Device Class to Program View
        // 08/02/10 AF  2.42.11                 Added support for the M2 Gateway
        //
        public override string DeviceClass
        {
            get
            {
                if (!m_strDeviceClass.Cached)
                {
                    object Value;
                    m_strDeviceClass.Value = "";
                    if (m_CenTables.IsCached((long)StdTableEnum.STDTBL0_DEVICE_CLASS, null))
                    {
                        m_CenTables.GetValue(StdTableEnum.STDTBL0_DEVICE_CLASS, null, out Value);
                        ASCIIEncoding AE = new ASCIIEncoding();
                        m_strDeviceClass.Value = AE.GetString((byte[])Value, 0, ((byte[])Value).Length);
                    }
                }
                return m_strDeviceClass.Value;
            }
        }

        /// <summary>
        /// Get the Test Display Configuration List
        /// </summary>
        public override List<OpenWayDisplayItem> TestDisplayConfig
        {
            get
            {
//TODO - add support for M2 Gateway
                IList<TableData> DisplayTables = m_CenTables.BuildPSEMStreams(2048, ConfigHeader.DisplayOffset, CENTRON2_MONO_DisplayConfig.DISPLAY_CONFIG_SIZE);
                List<OpenWayDisplayItem> DisplayItems = new List<OpenWayDisplayItem>();

                //Assembly the multiple streams that I received to one stream that I can use.
                Stream DisplayStream = BuildStream(DisplayTables, ConfigHeader.DisplayOffset, CENTRON2_MONO_DisplayConfig.DISPLAY_CONFIG_SIZE);

                PSEMBinaryReader DisplayReader = new PSEMBinaryReader(DisplayStream);
                CENTRON2_MONO_DisplayConfig DisplayConfigTable = new CENTRON2_MONO_DisplayConfig(DisplayReader, ConfigHeader.DisplayOffset);

                foreach (ANSIDisplayData DisplayData in DisplayConfigTable.TestDisplayData)
                {
                    DisplayItems.Add(new OpenWayDisplayItem(new CentronMonoLID(DisplayData.NumericLID),
                        DisplayData.DisplayID, DisplayData.DisplayFormat, DisplayData.DisplayDimension));
                }

                return DisplayItems;
            }
        }

        /// <summary>
        /// Get the Alternate Display Configuration List
        /// </summary>
        public override List<OpenWayDisplayItem> AlternateDisplayConfig
        {
            get
            {
                IList<TableData> DisplayTables = m_CenTables.BuildPSEMStreams(2048, ConfigHeader.DisplayOffset, CENTRON_AMI_DisplayConfig.DISPLAY_CONFIG_SIZE);
                List<OpenWayDisplayItem> DisplayItems = new List<OpenWayDisplayItem>();

                //Assembly the multiple streams that I received to one stream that I can use.
                Stream DisplayStream = BuildStream(DisplayTables, ConfigHeader.DisplayOffset, CENTRON_AMI_DisplayConfig.DISPLAY_CONFIG_SIZE);

                PSEMBinaryReader DisplayReader = new PSEMBinaryReader(DisplayStream);
                CENTRON_AMI_DisplayConfig DisplayConfigTable = new CENTRON_AMI_DisplayConfig(DisplayReader, ConfigHeader.DisplayOffset);

                foreach (ANSIDisplayData DisplayData in DisplayConfigTable.AlternateDisplayData)
                {
                    DisplayItems.Add(new OpenWayDisplayItem(new CentronMonoLID(DisplayData.NumericLID),
                        DisplayData.DisplayID, DisplayData.DisplayFormat, DisplayData.DisplayDimension));
                }

                return DisplayItems;
            }
        }
        
        /// <summary>
        /// Get the DST Switch Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public override string DSTSwitchTime
        {
            get
            {
                int iDSTHour = 0;
                int iDSTMin = 0;

                if (!m_strDSTSwitch.Cached)
                {
                    if (m_CenTables.IsAllCached(2090))
                    {
                        iDSTHour = GetMFGEDLInt(CentronTblEnum.MFGTBL42_DST_HOUR);
                        iDSTMin = GetMFGEDLInt(CentronTblEnum.MFGTBL42_DST_MINUTE);
                    }

                    m_strDSTSwitch.Value = iDSTHour.ToString(CultureInfo.CurrentCulture)
                        + ":" + iDSTMin.ToString("00", CultureInfo.CurrentCulture);
                }

                return m_strDSTSwitch.Value;
            }
        }
        
        /// <summary>
        /// Gets whether or not the PF Load Indicator will be shown on the display
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 RCG 2.20.03    N/A    Created

        public override bool DisplayPFLoadIndicator
        {
            get
            {
                bool bDisplayIndicator = false;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_VAR_LOAD_INDICATOR, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_VAR_LOAD_INDICATOR, null, out objValue);

                    bDisplayIndicator = (bool)objValue;
                }

                return bDisplayIndicator;
            }
        }

        /// <summary>
        /// Gets whether or not the Missing Phase Indicators will be shown on the display
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 RCG 2.20.03    N/A    Created

        public override bool DisplayPhaseIndicators
        {
            get
            {
                bool bDisplayIndicator = false;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_ENABLE_PHASE_INDICATORS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_ENABLE_PHASE_INDICATORS, null, out objValue);

                    bDisplayIndicator = (bool)objValue;
                }

                return bDisplayIndicator;
            }
        }

        /// <summary>
        /// Gets whether or not the Missing Phase Indicator should blink
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 RCG 2.20.03    N/A    Created

        public override bool BlinkMissingPhaseIndicator
        {
            get
            {
                bool bBlinkIndicator = false;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_BLINK_MISSING_PHASES, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_BLINK_MISSING_PHASES, null, out objValue);

                    bBlinkIndicator = (bool)objValue;
                }

                return bBlinkIndicator;
            }
        }
        
        /// <summary>
        /// Gets the list of Load Profile Quanitites
        /// </summary>
        public override List<string> LPQuantityList
        {
            get
            {
                object Value;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays
                CentronMonoLID LPQuantityLID;
                List<String> LPConfigList = new List<String>();

                for (i = 0; i < NumberLPChannels; i++)
                {
                    anIndex1[0] = i;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_LP_LID, anIndex1, out Value);
                    LPQuantityLID = new CentronMonoLID((uint)Value);
                    LPConfigList.Add(LPQuantityLID.lidDescription);
                }

                return LPConfigList;
            }
        }

        /// <summary>
        /// Gets/sets a list of Load Profile Quanitity LIDs.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created
        //  11/05/10 jrf 2.45.11        Added setting of unused channels to default values.
        //
        public override  List<LID> LPQuantityLIDs
        {
            get
            {
                object Value;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays
                LID LPQuantityLID;
                List<LID> LPConfigList = new List<LID>();

                for (i = 0; i < NumberLPChannels; i++)
                {
                    anIndex1[0] = i;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_LP_LID, anIndex1, out Value);
                    LPQuantityLID = new LID((uint)Value);
                    LPConfigList.Add(LPQuantityLID);
                }

                return LPConfigList;
            }
            set
            {
                object Value;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays

                NumberLPChannels = value.Count;

                for (i = 0; i < NumberLPChannels; i++)
                {
                    Value = value[i].lidValue;
                    anIndex1[0] = i;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_LP_LID, anIndex1, Value);
                }

                for (i = NumberLPChannels; i < DetermineMaximumLPChannels(); i++)
                {
                    Value = 0;
                    anIndex1[0] = i;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_LP_LID, anIndex1, Value);
                }
            }
        }

        /// <summary>
        /// Gets the list of Load Profile Pulse Weight
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.11        Added set.
        //
        public override  List<float> LPPulseWeightList
        {
            get
            {
                object Value;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays
                float fltPulseWeight;

                List<float> LPPulseWeightList = new List<float>();

                for (i = 0; i < NumberLPChannels; i++)
                {
                    anIndex1[0] = i;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT, anIndex1, out Value);
                    fltPulseWeight = float.Parse(Value.ToString(), CultureInfo.InvariantCulture);
                    LPPulseWeightList.Add((float)(fltPulseWeight * 0.01));
                }

                return LPPulseWeightList;
            }
            set
            {
                object Value;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays

                for (i = 0; i < NumberLPChannels; i++)
                {
                    anIndex1[0] = i;
                    int iPulseWeight = Convert.ToInt32(value[i] / 0.01);
                    Value = iPulseWeight;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT, anIndex1, Value);
                }

                for (i = NumberLPChannels; i < DetermineMaximumLPChannels(); i++)
                {
                    anIndex1[0] = i;
                    Value = 100;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT, anIndex1, Value);
                }
            }
        }

        /// <summary>
        /// Returns the Full Configuration Header Object
        /// </summary>
        public override CTable2048Header ConfigHeader
        {
            get
            {
                Stream strm2048Header = new MemoryStream();
                
                // Get a list of Stream data from the EDL file
                IList<TableData> lstTableData = m_CenTables.BuildPSEMStreams((long)CentronTblEnum.MFGTBL0_DATA_SIZE, null, (long)CentronTblEnum.MFGTBL0_SUBTABLE_OFFSETS, null);

                //Assembly the multiple streams that I received to one stream that I can use.
                strm2048Header = BuildStream(lstTableData, 0, CTable2048Header.HEADER_LENGTH_2048);
                // Now that I have a stream, I can create my Binary Reader
                PSEMBinaryReader EDLReader = new PSEMBinaryReader(strm2048Header);
                //Finally, I can send the Binary Reader to the 2048 class, where it can be used to read the header data.
                CTable2048Header ConfigurationHeader = new CTable2048Header(EDLReader);

                return ConfigurationHeader;
            }
        }
              
        /// <summary>
        /// Get the length of the DST Switch (minutes)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public override int DSTSwitchLength
        {
            get
            {
                if (!m_iDSTLength.Cached)
                {
                    if (m_CenTables.IsAllCached(2090))
                    {
                        m_iDSTLength.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL42_DST_OFFSET);
                    }
                    else
                    {
                        m_iDSTLength.Value = 0;
                    }
                }

                return m_iDSTLength.Value;
            }
        }

        /// <summary>
        /// Gets whether or not Voltage Monitoring is enabled in the EDL file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 RCG	1.50.26		   Created

        public override bool VMEnabled
        {
            get
            {
                bool bEnabled = false;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_ENABLE_FLAG, null) == true)
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL102_ENABLE_FLAG, null, out objValue);
                    bEnabled = (bool)objValue;
                }

                return bEnabled;
            }
            set
            {
                Object objValue = value;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL102_ENABLE_FLAG, null, objValue);
            }
        }

        /// <summary>
        /// Gets the number of phases used for Voltage Monitoring.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 RCG	1.50.26		   Created

        public override byte VMNumPhases
        {
            get
            {
                byte byNumPhases = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL101_NBR_PHASES, null) == true)
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL101_NBR_PHASES, null, out objValue);
                    byNumPhases = (byte)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Voltage Monitoring");
                }

                return byNumPhases;
            }
        }

        /// <summary>
        /// Gets the Voltage Monitoring interval length
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 RCG	1.50.26		   Created

        public override TimeSpan VMIntervalLength
        {
            get
            {
                byte byMinutes = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL101_VM_INT_LEN, null) == true)
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL101_VM_INT_LEN, null, out objValue);
                    byMinutes = (byte)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Voltage Monitoring");
                }

                return TimeSpan.FromMinutes((double)byMinutes);
            }
            set
            {
                object objValue = value.TotalMinutes;
                double dblThreshold;
                double dblPrevVMIntLength = VMIntervalLength.TotalMinutes;

                m_CenTables.SetValue(CentronTblEnum.MFGTBL101_VM_INT_LEN, null, objValue);

                //Need to update stored Vh low/high thresholds
                //Low Threshold
                m_CenTables.GetValue(CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD, null, out objValue);
                dblThreshold = Convert.ToDouble(objValue, CultureInfo.InvariantCulture);

                dblThreshold = dblThreshold / dblPrevVMIntLength * value.TotalMinutes;
                objValue = dblThreshold;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD, null, objValue);

                //High Threshold
                m_CenTables.GetValue(CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD, null, out objValue);
                dblThreshold = Convert.ToDouble(objValue, CultureInfo.InvariantCulture);

                dblThreshold = dblThreshold / dblPrevVMIntLength * value.TotalMinutes;
                objValue = dblThreshold;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD, null, objValue);

            }
        }

        /// <summary>
        /// Determine the Percentage of Nominal for VMVhLowThreshold
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#            Description
        // -------- --- ------- --------------- ---------------------------------------
        // 07/16/08 KRC	1.51.05	 itron00116930  Created
        //
        public override ushort VMVhLowPercentage
        {
            get
            {
                ushort usPercentage = 0;

                if (DeviceType == EDLDeviceTypes.OpenWayCentronAdvPoly || DeviceType == EDLDeviceTypes.OpenWayCentronBasicPoly)
                {
                    object objValue;

                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD, null))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD, null, out objValue);
                        usPercentage = (ushort)objValue;
                    }
                }
                else
                {
                    float fltLowVhPerHour = (float)(60 / VMIntervalLength.TotalMinutes) * VMVhLowThreshold;
                    float fltPercentage = (fltLowVhPerHour / (float)DetermineNominalVoltage());
                    usPercentage = (ushort)Math.Round(fltPercentage * 100);
                }

                return usPercentage;
            }
            set
            {
                if (DeviceType == EDLDeviceTypes.CentronII)
                {
                    object objValue;
                    double dblThreshold = value / 100f * 160 * VMIntervalLength.TotalMinutes;

                    objValue = dblThreshold;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD, null, objValue);
                }
            }
        }

        /// <summary>
        /// Determine the Percentage of Nominal for VMVhHighThreshold
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#            Description
        // -------- --- ------- --------------- ---------------------------------------
        // 07/16/08 KRC	1.51.05	 itron00116930  Created
        //
        public override ushort VMVhHighPercentage
        {
            get
            {
                ushort usPercentage = 0;

                if (DeviceType == EDLDeviceTypes.OpenWayCentronAdvPoly || DeviceType == EDLDeviceTypes.OpenWayCentronBasicPoly)
                {
                    object objValue;

                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD, null))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD, null, out objValue);
                        usPercentage = (ushort)objValue;
                    }
                }
                else
                {
                    float fltHighVhPerHour = (float)(60.0 / VMIntervalLength.TotalMinutes) * VMVhHighThreshold;
                    float fltPercentage = (fltHighVhPerHour / (float)DetermineNominalVoltage());
                    usPercentage = (ushort)Math.Round(fltPercentage * 100);
                }

                return usPercentage;
            }
            set
            {
                if (DeviceType == EDLDeviceTypes.CentronII)
                {
                    object objValue;
                    double dblThreshold = value / 100f * 160 * VMIntervalLength.TotalMinutes;

                    objValue = dblThreshold;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD, null, objValue);
                }
            }
        }

        /// <summary>
        /// Gets the Low RMS Threshold for Voltage Monitoring
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 RCG	1.50.26		   Created

        public override ushort VMRMSLowThreshold
        {
            get
            {
                ushort usThreshold = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_RMS_VOLT_LOW_THRESHOLD, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL102_RMS_VOLT_LOW_THRESHOLD, null, out objValue);
                    usThreshold = (ushort)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Voltage Monitoring");
                }

                return usThreshold;
            }
            set
            {
                object objValue = value;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL102_RMS_VOLT_LOW_THRESHOLD, null, objValue);
            }
        }

        /// <summary>
        /// Gets the High RMS Threshold for Voltage Monitoring
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 RCG	1.50.26		   Created

        public override ushort VMRMSHighThreshold
        {
            get
            {
                ushort usThreshold = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_RMS_VOLT_LOW_THRESHOLD, null))
                {

                    m_CenTables.GetValue(CentronTblEnum.MFGTBL102_RMS_VOLT_HIGH_THRESHOLD, null, out objValue);
                    usThreshold = (ushort)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Voltage Monitoring");
                }

                return usThreshold;
            }
            set
            {
                object objValue = value;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL102_RMS_VOLT_HIGH_THRESHOLD, null, objValue);
            }
        }
 
        /// <summary>
        /// Gets whether or not user intervention is required after a connection.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public override bool SLConnectsUsingUserIntervention
        {
            get
            {
                bool bUsesIntervention = false;
                object objValue;

                if(m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_CONNECT_WITH_USER_INTERVENTION_FLAG, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_CONNECT_WITH_USER_INTERVENTION_FLAG, null, out objValue);
                    bUsesIntervention = (bool)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return bUsesIntervention;
            }
        }

        /// <summary>
        /// Gets whether or not the Override Connect/Disconnect Switch is enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue#             Description
        //  -------- --- ------- ---------------  -------------------------------------------
        //  07/03/08 KRC 1.51.01  itron00116660    Created
        //
        public override bool SLOverrideSwitch
        {
            get
            {
                bool bOverrideSwitch = false;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL94_OVERRIDE_FLAG, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL94_OVERRIDE_FLAG, null, out objValue);
                    bOverrideSwitch = (bool)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return bOverrideSwitch;
            }
        }
        /// <summary>
        /// Gets the maximum number of disconnects allowed in the configured period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public override byte SLMaxDisconnects
        {
            get
            {
                byte byMax = 0;
                object objValue;

                if(m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_MAX_SWITCH_COUNT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_MAX_SWITCH_COUNT, null, out objValue);
                    byMax = (byte)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return byMax;
            }
        }

        /// <summary>
        /// Gets the period of time when the alarm will be raised after a disconnect.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public override TimeSpan SLDisconnectRandomizationAlarmPeriod
        {
            get
            {
                DateTime dtValue;
                TimeSpan tsPeriod = new TimeSpan();
                object objValue;

                if(m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_RANDOMIZATION_ALARM, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_RANDOMIZATION_ALARM, null, out objValue);
                    dtValue = (DateTime)objValue;
                    tsPeriod = dtValue.TimeOfDay;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return tsPeriod;
            }
        }

        /// <summary>
        /// Gets the minimum amount of time to wait before reconnecting.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public override TimeSpan SLReconnectStartDelay
        {
            get
            {
                DateTime dtValue;
                TimeSpan tsDelay = new TimeSpan();
                object objValue;

                if(m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_RESTORATION_START_DELAY, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_RESTORATION_START_DELAY, null, out objValue);
                    dtValue = (DateTime)objValue;
                    tsDelay = dtValue.TimeOfDay;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return tsDelay;
            }
        }

        /// <summary>
        /// Gets the period of time where the meter will be reconnected a
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public override TimeSpan SLReconnectRandomDelay
        {
            get
            {
                DateTime dtValue;
                TimeSpan tsDelay = new TimeSpan();
                object objValue;

                if(m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_RESTORATION_RANDOM_DELAY, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_RESTORATION_RANDOM_DELAY, null, out objValue);
                    dtValue = (DateTime)objValue;
                    tsDelay = dtValue.TimeOfDay;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return tsDelay;
            }
        }

        /// <summary>
        /// Gets the amount of time the switch will remain open after a service limiting disconnect.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public override TimeSpan SLDisconnectOpenDelay
        {
            get
            {
                DateTime dtValue;
                TimeSpan tsDelay = new TimeSpan();
                object objValue;

                if(m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_OPEN_TIME, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_OPEN_TIME, null, out objValue);
                    dtValue = (DateTime)objValue;
                    tsDelay = dtValue.TimeOfDay;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return tsDelay;
            }
        }

        /// <summary>
        /// Gets the number of Service Limiting disconnect retry attempts
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/02/09 RCG 2.30.16 N/A    Created

        public override byte SLRetryAttemtps
        {
            get
            {
                byte byValue = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_RETRY_ATTEMPTS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_RETRY_ATTEMPTS, null, out objValue);
                    byValue = (byte)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return byValue;
            }
        }

        /// <summary>
        /// Gets the amount of time the failsafe is enabled after a failsafe event occurs.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/02/09 RCG 2.30.16 N/A    Created

        public override TimeSpan SLFailsafeDuration
        {
            get
            {
                ushort usDuration = 0;
                TimeSpan tsDuration = new TimeSpan();
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl95FailsafeDuration, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl95FailsafeDuration, null, out objValue);
                    usDuration = (ushort)objValue;
                    tsDuration = TimeSpan.FromMinutes(usDuration);
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return tsDuration;
            }
        }

        /// <summary>
        /// Gets the quantity for the normal mode threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public override string SLNormalModeThresholdQuantity
        {
            get
            {
                string strDemand = "None";
                int[] iaThresholdIndex = { 0 };
                byte byIndex;
                byte byNumThresholds;
                object objValue;
                CentronMonoLID LID;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_QUANTITY, iaThresholdIndex) &&
                    m_CenTables.IsCached((long)CentronTblEnum.MFGTBL91_NBR_THRESHOLDS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL91_NBR_THRESHOLDS, null, out objValue);
                    byNumThresholds = (byte)objValue;

                    if (byNumThresholds > 0)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL93_QUANTITY, iaThresholdIndex, out objValue);
                        byIndex = (byte)objValue;

                        if (byIndex != 255)
                        {
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DEMAND_DEFINITION, new int[] { byIndex }, out objValue);
                            LID = new CentronMonoLID((uint)objValue);

                            strDemand = LID.lidDescription;
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return strDemand;
            }
        }

        /// <summary>
        /// Gets the threshold value for normal mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public override double SLNormalModeThreshold
        {
            get
            {
                double dThreshold = 0.0;
                object objValue;
                int[] iaThresholdIndex = { 0 };

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_THRESHOLD, iaThresholdIndex))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_THRESHOLD, iaThresholdIndex, out objValue);
                    dThreshold = (double)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return dThreshold;
            }
        }

        /// <summary>
        /// Gets the quantity for the critical mode threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public override string SLCriticalModeThresholdQuantity
        {
            get
            {
                string strDemand = "None";
                int[] iaThresholdIndex = { 1 };
                byte byIndex;
                byte byNumThresholds;
                object objValue;
                CentronMonoLID LID;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_QUANTITY, iaThresholdIndex) &&
                    m_CenTables.IsCached((long)CentronTblEnum.MFGTBL91_NBR_THRESHOLDS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL91_NBR_THRESHOLDS, null, out objValue);
                    byNumThresholds = (byte)objValue;

                    if (byNumThresholds > 1)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL93_QUANTITY, iaThresholdIndex, out objValue);
                        byIndex = (byte)objValue;

                        if (byIndex != 255)
                        {
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DEMAND_DEFINITION, new int[] { byIndex }, out objValue);
                            LID = new CentronMonoLID((uint)objValue);

                            strDemand = LID.lidDescription;
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return strDemand;
            }
        }

        /// <summary>
        /// Gets the threshold value for critical mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public override double SLCriticalModeThreshold
        {
            get
            {
                double dThreshold = 0.0;
                object objValue;
                int[] iaThresholdIndex = { 1 };

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_THRESHOLD, iaThresholdIndex))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_THRESHOLD, iaThresholdIndex, out objValue);
                    dThreshold = (double)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return dThreshold;
            }
        }

        /// <summary>
        /// Gets the quantity for the emergency mode threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public override string SLEmergencyModeThresholdQuantity
        {
            get
            {
                string strDemand = "None";
                int[] iaThresholdIndex = { 2 };
                byte byIndex;
                byte byNumThresholds;
                object objValue;
                CentronMonoLID LID;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_QUANTITY, iaThresholdIndex) &&
                    m_CenTables.IsCached((long)CentronTblEnum.MFGTBL91_NBR_THRESHOLDS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL91_NBR_THRESHOLDS, null, out objValue);
                    byNumThresholds = (byte)objValue;

                    if (byNumThresholds > 2)
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL93_QUANTITY, iaThresholdIndex, out objValue);
                        byIndex = (byte)objValue;

                        if (byIndex != 255)
                        {
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DEMAND_DEFINITION, new int[] { byIndex }, out objValue);
                            LID = new CentronMonoLID((uint)objValue);

                            strDemand = LID.lidDescription;
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return strDemand;
            }
        }

        /// <summary>
        /// Gets the threshold value for emergency mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public override double SLEmergencyModeThreshold
        {
            get
            {
                double dThreshold = 0.0;
                object objValue;
                int[] iaThresholdIndex = { 2 };

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL93_THRESHOLD, iaThresholdIndex))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL93_THRESHOLD, iaThresholdIndex, out objValue);
                    dThreshold = (double)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Service Limiting");
                }

                return dThreshold;
            }
        }

        /// <summary>
        /// Gets the number of optical login attempts before a lockout.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/08 RCG	1.50.27		   Created
        // 08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public override byte LockoutOpticalAttempts
        {
            get
            {
                byte byAttempts = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_OPTICAL, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_OPTICAL, null, out objValue);
                    byAttempts = (byte)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Communications Configuration");
                }

                return byAttempts;
            }
            set
            {
                Object objValue = value;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_OPTICAL, null, objValue);
            }
        }

        /// <summary>
        /// Gets the optical lockout time
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/08 RCG	1.50.27		   Created
        // 08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public override byte LockoutOpticalMinutes
        {
            get
            {
                byte byAttempts = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_OPTICAL, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_OPTICAL, null, out objValue);
                    byAttempts = (byte)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Communications Configuration");
                }

                return byAttempts;
            }
            set
            {
                Object objValue = value;
                m_CenTables.SetValue(CentronTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_OPTICAL, null, objValue);
            }
        }

        /// <summary>
        /// Gets the number of LAN attempts before a lockout.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/08 RCG	1.50.27		   Created
        // 08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public override byte LockoutLanAttempts
        {
            get
            {
                byte byAttempts = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_LAN, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_LAN, null, out objValue);
                    byAttempts = (byte)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Communications Configuration");
                }

                return byAttempts;
            }
        }

        /// <summary>
        /// Gets the LAN lockout time
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/08 RCG	1.50.27		   Created
        // 08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public override byte LockoutLanMinutes
        {
            get
            {
                byte byAttempts = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_LAN, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_LAN, null, out objValue);
                    byAttempts = (byte)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Communications Configuration");
                }

                return byAttempts;
            }
        }

        /// <summary>
        /// Gets the LAN message failure limit.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/08 RCG	1.50.27		   Created
        // 08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public override byte LanMessageFailureLimit
        {
            get
            {
                byte byAttempts = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL142_FAILURES_BEFORE_FAILURE_EVENT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL142_FAILURES_BEFORE_FAILURE_EVENT, null, out objValue);
                    byAttempts = (byte)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Communications Configuration");
                }

                return byAttempts;
            }
        }

        /// <summary>
        /// Gets the LAN link metric period
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/08 RCG	1.50.27		   Created
        // 08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public override ushort LanLinkMetricPeriod
        {
            get
            {
                ushort usSeconds = 0;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL142_LAN_LINK_METRIC_PERIOD_SECONDS, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL142_LAN_LINK_METRIC_PERIOD_SECONDS, null, out objValue);
                    usSeconds = (ushort)objValue;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Communications Configuration");
                }

                return usSeconds;
            }
        }

 
        /// <summary>
        /// Gets the exception security model for the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/04/08 RCG 2.00.02        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public override OpenWayMFGTable2193.SecurityFormat? ExceptionSecurityModel
        {
            get
            {
                OpenWayMFGTable2193.SecurityFormat? SecurityFormat = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL145_EXCEPTION_SECURITY_MODEL, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL145_EXCEPTION_SECURITY_MODEL, null, out objValue);
                    SecurityFormat = (OpenWayMFGTable2193.SecurityFormat)(byte)objValue;
                }

                return SecurityFormat;
            }
        }

        /// <summary>
        /// Gets whether or not enhanced security is required.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/04/08 RCG 2.00.02        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public override bool? IsEnhancedSecurityRequired
        {
            get
            {
                bool? bRequiresEnhancedSecurity = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL145_REQUIRE_ENHANCED_SECURITY, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL145_REQUIRE_ENHANCED_SECURITY, null, out objValue);
                    bRequiresEnhancedSecurity = (bool)objValue;
                }

                return bRequiresEnhancedSecurity;
            }
        }

        /// <summary>
        /// Gets whether or not C12.18 communications are enabled over ZigBee.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/03/09 RCG 2.21.06        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public override bool? IsC1218OverZigBeeEnabled
        {
            get
            {
                bool? bC1218OverZigBeeEnabled = null;
                object objValue;

                if (IsProgramFile == false && (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_2_SP5) < 0
                    || (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_2_SP5) == 0 && FirmwareBuild < 56)))
                {
                    // This is a data file and it was created with a meter prior to FW 2.5.56 so
                    // we should always report this as true since it was always enabled in this FW
                    bC1218OverZigBeeEnabled = true;
                }
                else if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl145C1218OverZigBee, null))
                {
                    // This is a program file that has the value cached (created with SP5 CE) or 
                    // this is a data file created on a meter 2.5.56 or later.
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl145C1218OverZigBee, null, out objValue);
                    bC1218OverZigBeeEnabled = (bool)objValue;
                }

                return bC1218OverZigBeeEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not ZigBee Private Profile is Enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/28/09 MMD 2.30.15        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public override bool? DisableZigbeePrivateProfile
        {
            get
            {
                bool? bDisableZigBeePrivateProfile = null;
                object objValue;

                if (IsProgramFile == false && (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_2_SP5_1) < 0))
                {
                   bDisableZigBeePrivateProfile = false;
                }
                else if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl145DisableZigBeePrivateProfile, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl145DisableZigBeePrivateProfile, null, out objValue);
                    bDisableZigBeePrivateProfile = (bool)objValue;
                }

                return bDisableZigBeePrivateProfile;
            }
        }

        /// <summary>
        /// Gets whether or not ZigBee Radio is Enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/28/09 MMD 2.30.15        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public override bool? DisableZigbeeRadio
        {
            get
            {
                bool? bDisableZigbeeRadio = null;
                object objValue;

                if (IsProgramFile == false && (VersionChecker.CompareTo(FWRevision, CENTRON_AMI.VERSION_2_SP5_1) < 0))
                {
                    bDisableZigbeeRadio = false;
                }
                else if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl145DisableZigBeeRadio, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl145DisableZigBeeRadio, null, out objValue);
                    bDisableZigbeeRadio = (bool)objValue;
                }

                return bDisableZigbeeRadio;
            }
        }

        /// <summary>
        /// IO Configuration Data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/23/09 jrf 2.20.03    N/A    Created
        //
        public override KYZData IOData
        {
            get
            {
                Stream strmIOConfig = new MemoryStream();
                m_CenTables.BuildPSEMStream(2048, strmIOConfig, ConfigHeader.IOOffset, CENTRON_AMI_IOConfig.IO_CONFIG_TBL_SIZE);
                PSEMBinaryReader EDLReader = new PSEMBinaryReader(strmIOConfig);
                CENTRON_AMI_IOConfig IOConfig = new CENTRON_AMI_IOConfig(EDLReader);

                return IOConfig.IOData;
            }
        }
        
        /// <summary>
        /// Gets whether or not SiteScan Diag 1 is enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public override bool? IsSiteScanDiag1Enabled
        {
            get
            {
                bool? bIsEnabled = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SITE_SCAN_1_ENABLE, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SITE_SCAN_1_ENABLE, null, out objValue);
                    bIsEnabled = (bool)objValue;
                }

                return bIsEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 2 is enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public override bool? IsSiteScanDiag2Enabled
        {
            get
            {
                bool? bIsEnabled = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SITE_SCAN_2_ENABLE, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SITE_SCAN_2_ENABLE, null, out objValue);
                    bIsEnabled = (bool)objValue;
                }

                return bIsEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 3 is enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public override bool? IsSiteScanDiag3Enabled
        {
            get
            {
                bool? bIsEnabled = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SITE_SCAN_3_ENABLE, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SITE_SCAN_3_ENABLE, null, out objValue);
                    bIsEnabled = (bool)objValue;
                }

                return bIsEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 4 is enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public override bool? IsSiteScanDiag4Enabled
        {
            get
            {
                bool? bIsEnabled = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SITE_SCAN_4_ENABLE, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SITE_SCAN_4_ENABLE, null, out objValue);
                    bIsEnabled = (bool)objValue;
                }

                return bIsEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 1 will scroll 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public override bool? ScrollSiteScanDiag1
        {
            get
            {
                bool? bWillScroll = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SCROLL_POLARITY, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_POLARITY, null, out objValue);
                    bWillScroll = (bool)objValue;
                }

                return bWillScroll;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 2 will scroll 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public override bool? ScrollSiteScanDiag2
        {
            get
            {
                bool? bWillScroll = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SCROLL_PHASE_VOLTAGE_DEVIATION, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_PHASE_VOLTAGE_DEVIATION, null, out objValue);
                    bWillScroll = (bool)objValue;
                }

                return bWillScroll;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 3 will scroll 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public override bool? ScrollSiteScanDiag3
        {
            get
            {
                bool? bWillScroll = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SCROLL_INACTIVE_PHASE_CURRENT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_INACTIVE_PHASE_CURRENT, null, out objValue);
                    bWillScroll = (bool)objValue;
                }

                return bWillScroll;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan Diag 4 will scroll 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/09 RCG 2.20.03    N/A    Created

        public override bool? ScrollSiteScanDiag4
        {
            get
            {
                bool? bWillScroll = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SCROLL_PHASE_ANGLE_DEVIATION, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_PHASE_ANGLE_DEVIATION, null, out objValue);
                    bWillScroll = (bool)objValue;
                }

                return bWillScroll;
            }
        }

        /// <summary>
        /// Gets the LED Normal mode quantity
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created

        public override string LEDNormalModeQuantity
        {
            get
            {
                string strQuantity = "";                
                object objValue;
                LEDQuantity Quantity;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_PULSE_OUTPUT1_QUANTITY_NORMAL, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_PULSE_OUTPUT1_QUANTITY_NORMAL, null, out objValue);
                    Quantity = new LEDQuantity((uint)objValue);
                    strQuantity = Quantity.Description;
                }

                return strQuantity;
            }
        }

        /// <summary>
        /// Gets the LED Test mode quantity
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created

        public override string LEDTestModeQuantity
        {
            get
            {
                string strQuantity = "";
                object objValue;
                LEDQuantity Quantity;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_PULSE_OUTPUT1_QUANTITY_TEST, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_PULSE_OUTPUT1_QUANTITY_TEST, null, out objValue);
                    Quantity = new LEDQuantity((uint)objValue);
                    strQuantity = Quantity.Description;
                }

                return strQuantity;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 9S meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created

        public override float LEDPulseWeight9S
        {
            get
            {
                float fPulseWeight = 0.0f;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_9S_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_9S_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 12S Class 200 meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created

        public override float LEDPulseWeight12SClass200
        {
            get
            {
                float fPulseWeight = 0.0f;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_12S_C200_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_12S_C200_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 12S Class 320 meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created

        public override float LEDPulseWeight12SClass320
        {
            get
            {
                float fPulseWeight = 0.0f;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_12S_C320_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_12S_C320_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 16S Class 200 meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created

        public override float LEDPulseWeight16SClass200
        {
            get
            {
                float fPulseWeight = 0.0f;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_16S_C200_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_16S_C200_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 16S Class 320 meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created

        public override float LEDPulseWeight16SClass320
        {
            get
            {
                float fPulseWeight = 0.0f;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_16S_C320_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_16S_C320_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the LED pulse weight for 45S meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/09 RCG 2.20.03    N/A    Created

        public override float LEDPulseWeight45S
        {
            get
            {
                float fPulseWeight = 0.0f;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL121_FORM_45S_PULSE_WEIGHT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL121_FORM_45S_PULSE_WEIGHT, null, out objValue);
                    fPulseWeight = (ushort)objValue * 0.025f;
                }

                return fPulseWeight;
            }
        }

        /// <summary>
        /// Gets the HAN Security Profile
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/09 RCG 2.30.16        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public override string HANSecurityProfile
        {
            get
            {
                string strValue = null;
                object objValue = null;

                byte bySecurityMode = 0;
                byte byDeviceAuthMode = 0;
                byte byCBKEMode = 0;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl58SecurityMode, null)
                    && m_CenTables.IsCached((long)CentronTblEnum.MfgTbl58DeviceAuthMode, null)
                    && m_CenTables.IsCached((long)CentronTblEnum.MfgTbl58CbkeMode, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl58SecurityMode, null, out objValue);
                    bySecurityMode = (byte)objValue;

                    m_CenTables.GetValue(CentronTblEnum.MfgTbl58DeviceAuthMode, null, out objValue);
                    byDeviceAuthMode = (byte)objValue;

                    m_CenTables.GetValue(CentronTblEnum.MfgTbl58CbkeMode, null, out objValue);
                    byCBKEMode = (byte)objValue;

                    strValue = CHANMfgTable2106.GetHANSecurityProfile(bySecurityMode, byDeviceAuthMode, byCBKEMode);
                }

                return strValue;
            }
        }

        /// <summary>
        /// Gets the Inter PAN Mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/06/09 RCG 2.30.16 144719 Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public override string InterPANMode
        {
            get
            {
                string strValue = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl58InterPanMode, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl58InterPanMode, null, out objValue);
                    strValue = CHANMfgTable2106.GetInterPANMode((byte)objValue);
                }

                return strValue;
            }
        }

        /// <summary>
        /// Gets whether or not Fatal Recovery Mode is configured to be enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/23/10 RCG 2.40.28        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public override bool? IsFatalRecoveryModeConfigured
        {
            get
            {
                bool? bConfigured = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212FatalErrorRecoveryEnabled, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl212FatalErrorRecoveryEnabled, null, out objValue);
                    bConfigured = (bool)objValue;
                }

                return bConfigured;
            }
        }

        /// <summary>
        /// Gets whether or not Asset Synch is enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/23/10 RCG 2.40.28        Created
        //  11/05/10 jrf 2.45.10        Added set.
        //
        public override bool? IsAssetSynchEnabled
        {
            get
            {
                bool? bEnabled = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl212AssetSynchronizationEnabled, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl212AssetSynchronizationEnabled, null, out objValue);
                    bEnabled = (bool)objValue;
                }

                return bEnabled;
            }
            set
            {
                object objValue = value;

                m_CenTables.SetValue(CentronTblEnum.MfgTbl212AssetSynchronizationEnabled, null, objValue);
            }
        }

        /// <summary>
        /// Gets whether or not the meter is connected
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/12/10 RCG 2.45.03        Created

        public override bool? IsConnected
        {
            get
            {
                bool? bEnabled = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL92_IS_CONNECTED_FLAG, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL92_IS_CONNECTED_FLAG, null, out objValue);
                    bEnabled = (bool)objValue;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is armed for connection
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/12/10 RCG 2.45.03        Created

        public override bool? IsMeterArmed
        {
            get
            {
                bool? bArmed = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL92_IS_METER_ARMED, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL92_IS_METER_ARMED, null, out objValue);
                    bArmed = (bool)objValue;
                }

                return bArmed;
            }
        }

        /// <summary>
        /// Gets whether or not load voltage is currently present
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/12/10 RCG 2.45.03        Created

        public override bool? IsLoadVoltagePresent
        {
            get
            {
                bool? bPresent = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL92_LOAD_VOLTAGE_PRESENT, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL92_LOAD_VOLTAGE_PRESENT, null, out objValue);
                    bPresent = (bool)objValue;
                }

                return bPresent;
            }
        }

        /// <summary>
        /// Gets whether or not the last connect or disconnect switch attempt failed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/12/10 RCG 2.45.03        Created

        public override bool? DidLastDisconnectAttemptFail
        {
            get
            {
                bool? bLastAttemptFailed = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL92_LAST_ATTEMPT_FAIL_FLAG, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL92_LAST_ATTEMPT_FAIL_FLAG, null, out objValue);
                    bLastAttemptFailed = (bool)objValue;
                }

                return bLastAttemptFailed;
            }
        }

        /// <summary>
        /// This property gets and sets the misc id field of the EDL file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/19/10 jrf 9.70.07 N/A    Created
        //
        public string MiscID
        {
            get
            {
                object objValue;

                m_CenTables.GetValue(StdTableEnum.STDTBL6_MISC_ID, null, out objValue);

                return objValue.ToString();
            }
            set
            {
                m_CenTables.SetValue(StdTableEnum.STDTBL6_MISC_ID, null, value);
            }
        }

        #endregion

        #region Itron Device Configuration

        /// <summary> 
        /// Gets the DST Flag 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/10/08 KRC
        public override bool DSTEnabled
        {
            get
            {
                if (!m_blnDSTEnabled.Cached)
                {
                    m_blnDSTEnabled.Value = GetSTDEDLBool(StdTableEnum.STDTBL52_DST_APPLIED_FLAG);
                }

                return m_blnDSTEnabled.Value;
            }
        }

        /// <summary> 
        /// Gets the DST supported Flag, i.e. indicates if the meter will change 
        /// its time based on DST.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/17/10 jrf 2.45.13 N/A    Created
        //
        public override bool DSTSupported
        {
            get
            {
                return GetSTDEDLBool(StdTableEnum.STDTBL52_DST_SUPPORTED_FLAG);
            }
            set
            {
                SetSTDEDLBool(value, StdTableEnum.STDTBL52_DST_SUPPORTED_FLAG);
            }
        }

        /// <summary>
        /// Property used to get the device time (DateTime) from the meter
        /// </summary>
        public override DateTime DeviceTime
        {
            get
            {
                if (!m_dtCurrentTime.Cached)
                {
                    string strTemp = GetSTDEDLString(StdTableEnum.STDTBL52_CLOCK_CALENDAR);
                    m_dtCurrentTime.Value = (DateTime)DateTime.Parse(strTemp, CultureInfo.InvariantCulture);
                }

                return m_dtCurrentTime.Value;
            }
        }

        /// <summary>Returns the Full Firmware Version and Revision</summary>
        public override float FWRevision
        {
            get
            {
                if (!m_fltFWRevision.Cached)
                {
                    string strVersion;
                    string strRevision;

                    strVersion = GetSTDEDLString(StdTableEnum.STDTBL1_FW_VERSION_NUMBER);
                    strRevision = GetSTDEDLString(StdTableEnum.STDTBL1_FW_REVISION_NUMBER);
                    m_fltFWRevision.Value = (float)float.Parse(strVersion, CultureInfo.InvariantCulture) +
                        ((float)float.Parse(strRevision, CultureInfo.InvariantCulture) / (int)1000);
                }

                return m_fltFWRevision.Value;
            }
        }

        /// <summary>Returns the Full Firmware Version and Revision</summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/14/11 SCW         Added support for CENTRON II meter
        //
        public override byte FirmwareBuild
        {
            get
            {
                object Value;

                if (!m_byFWBuild.Cached && m_CenTables.IsCached((long)CentronTblEnum.MFGTBL60_REGISTER_FW_BUILD, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL60_REGISTER_FW_BUILD, null, out Value);
                    m_byFWBuild.Value = (byte)Value;
                }
                return m_byFWBuild.Value;
            }
        }

        /// <summary>
        /// Get the Exception Configuration
        /// </summary>
        public override List<ushort> ExceptionConfig
        {
            get
            {
                int[] anLimits = m_CenTables.GetElementLimits(StdTableEnum.STDTBL123_EVENT_REPORTED);
                List<ushort> lstAlarms = new List<ushort>();
                for (int ndxHost = 0; ndxHost < anLimits[0]; ndxHost++)
                {
                    for (int ndxEvent = 0; ndxEvent < anLimits[1]; ndxEvent++)
                    {
                        ushort alarm = GetSTDEDLUShort(StdTableEnum.STDTBL123_EVENT_REPORTED, ndxHost, ndxEvent);
                        if (alarm != 0)
                        {
                            lstAlarms.Add(alarm);
                        }
                    }
                }
                return lstAlarms;
            }
        }

        /// <summary>Returns the device ID read from table 5</summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/11/07 mcm 8.10.04		   Created
        // 01/23/08 AF  10.0           Added check to see if item exists - we
        //                             get an exception if it doesn't
        public override string UnitID
        {
            get
            {
                if (!m_strDeviceID.Cached)
                {
                    m_strDeviceID.Value = GetSTDEDLString(StdTableEnum.STDTBL5_IDENTIFICATION).Trim('\0');
                }

                return m_strDeviceID.Value.Trim();

            }//get
        }//UnitID

        /// <summary>
        /// Returns the Customer Serial Number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/10/08 KRC
        //
        public override string SerialNumber
        {
            get
            {
                if (!m_strSerialNumber.Cached)
                {
                    m_strSerialNumber.Value = GetSTDEDLString(StdTableEnum.STDTBL6_UTIL_SER_NO).Trim('\0');
                }

                return m_strSerialNumber.Value;
            }
        }
        /// <summary>
        /// Property used to get the program ID (int) from the meter
        /// </summary>
        public override int ProgramID
        {
            get
            {
                if (!m_iProgramID.Cached)
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_PROGRAM_ID, null))
                    {
                        m_iProgramID.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_PROGRAM_ID);
                    }
                }

                return m_iProgramID.Value;
            }
        }

        /// <summary>Returns the Full Software Version and Revision</summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/10 AF  2.40.49        If 2048 is unavailable make sure this doesn't crash
        //  08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public override string SWRevision
        {
            get
            {
                if (!m_strSWRevision.Cached)
                {
                    string strTemp = ".";

                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_SW_VERSION, null))
                    {
                        strTemp = GetMFGEDLString(CentronTblEnum.MFGTBL0_SW_VERSION);
                        strTemp += "." + GetMFGEDLString(CentronTblEnum.MFGTBL0_SW_REVISION);
                    }

                    if (strTemp == ".")
                    {
                        m_strSWRevision.Value = "Unavailable";
                    }
                    else
                    {
                        float fltSWVersion = (float)float.Parse(strTemp, CultureInfo.InvariantCulture);
                        m_strSWRevision.Value = fltSWVersion.ToString("F2", CultureInfo.CurrentCulture);
                    }
                }

                return m_strSWRevision.Value;
            }
        }
        
        /// <summary>
        /// Gets and Sets the list of configured Demands Thresholds.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created.
        //
        public override  List<LID> DemandThresholdLIDs
        {
            get
            {
                object Value;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays
                LID DemandLID;
                List<LID> DemandThresholdLIDs = new List<LID>();

                for (i = 0; i < DetermineDemandThresholdCount(); i++)
                {
                    anIndex1[0] = i;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_THRESHOLD_SOURCE, anIndex1, out Value);

                    if (0 == (uint)Value)
                    {
                        //This means the threshold is not configured.  Let's return an not progrmammed LID.
                        DefinedLIDs LIDs = new DefinedLIDs();
                        DemandLID = LIDs.DEMAND_NOT_PROGRAMMED;
                    }
                    else
                    {
                        DemandLID = new LID((uint)Value);
                    }

                    DemandThresholdLIDs.Add(DemandLID);
                }

                return DemandThresholdLIDs;
            }
            set
            {
                object Value;
                int[] anIndex1 = { 0 };

                for (int i = 0; i < DetermineDemandThresholdCount(); i++)
                {
                    anIndex1[0] = i;
                    
                    if (true == value[i].IsMaxDemand && false == value[i].IsCoincident)
                    {
                        Value = value[i].lidValue;
                    }
                    else
                    {
                        //Threshold must be set to 0 when it is not configured.
                        Value = 0;
                    }

                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_THRESHOLD_SOURCE, anIndex1, Value);
                }
            }
        }

        /// <summary>
        /// Gets/sets the standard table 06 tariff id of the EDL file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#         Description
        //  -------- --- ------- -------------- -------------------------------------------
        //  10/25/10 jrf 2.45.10                 Created
        //
        public override string TarrifID
        {
            get
            {
                if (!m_strTarrifID.Cached)
                {
                    string TOUID;
                    TOUID = GetSTDEDLString(StdTableEnum.STDTBL6_TARIFF_ID);

                    // Remove any nulls that might be at the end
                    TOUID = TOUID.TrimEnd('\0');
                    if (TOUID.Length > 0)
                    {
                        m_strTarrifID.Value = TOUID;
                    }
                    else
                    {
                        m_strTarrifID.Value = "";
                    }
                }
                return m_strTarrifID.Value;
            }
            set
            {
                string strValue = "";

                if (TARRIF_ID_MAX_LENGTH < value.Length)
                {
                    strValue = value.Substring(0, 8);
                }
                else
                {
                    strValue = value;
                }

                SetSTDEDLString(strValue, StdTableEnum.STDTBL6_TARIFF_ID);
                m_strTarrifID.Value = value;
            }
        }

        /// <summary>
        /// This property returns a list of user data strings.  If the meter has 3 user data fields
        /// then the list will contain 3 strings corresponding to each user data  field
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/06 mah 8.00    N/A    Created
        // 11/05/10 jrf 2.45.11        Added set.
        //
        public override List<String> UserData
        {
            get
            {
                List<String> UserDataList = new List<String>();

                if (!m_strUserData1.Cached)
                {
                    m_strUserData1.Value = GetMFGEDLString(CentronTblEnum.MFGTBL0_USER_DEFINED_FIELDS, 0);
                    m_strUserData2.Value = GetMFGEDLString(CentronTblEnum.MFGTBL0_USER_DEFINED_FIELDS, 1);
                    m_strUserData3.Value = GetMFGEDLString(CentronTblEnum.MFGTBL0_USER_DEFINED_FIELDS, 2);
                }

                UserDataList.Add(m_strUserData1.Value);
                UserDataList.Add(m_strUserData2.Value);
                UserDataList.Add(m_strUserData3.Value);

                return UserDataList;
            }
            set
            {
                if (value.Count > 0)
                {
                    m_strUserData1.Value = value[0];
                    SetMFGEDLString(value[0], CentronTblEnum.MFGTBL0_USER_DEFINED_FIELDS, 0);
                }

                if (value.Count > 1)
                {
                    m_strUserData2.Value = value[1];
                    SetMFGEDLString(value[1], CentronTblEnum.MFGTBL0_USER_DEFINED_FIELDS, 1);
                }

                if (value.Count > 2)
                {
                    m_strUserData3.Value = value[2];
                    SetMFGEDLString(value[2], CentronTblEnum.MFGTBL0_USER_DEFINED_FIELDS, 2);
                }
            }
        }

        /// <summary>
        /// Gets/sets the Cold Load Pickup Time in minutes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Added set.
        //
        public override uint ColdLoadPickupTime
        {
            get
            {
                if (!m_uiColdLoadPickupTime.Cached)
                {
                    m_uiColdLoadPickupTime.Value = (uint)GetMFGEDLInt(CentronTblEnum.MFGTBL0_COLD_LOAD_PICKUP);
                }

                return m_uiColdLoadPickupTime.Value;
            }
            set
            {
                m_uiColdLoadPickupTime.Value = value;
                SetMFGEDLInt((int)value, CentronTblEnum.MFGTBL0_COLD_LOAD_PICKUP);
            }
        }

        /// <summary>
        /// Gets/Sets the Interval Length for Demands
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Added set.
        //
        public override int DemandIntervalLength
        {
            get
            {
                if (!m_iDemandIntervalLength.Cached)
                {
                    m_iDemandIntervalLength.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_DEMAND_INTERVAL_LENGTH);
                }

                return m_iDemandIntervalLength.Value;
            }
            set
            {
                m_iDemandIntervalLength.Value = value;
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_DEMAND_INTERVAL_LENGTH);
            }
        }

        /// <summary>
        /// Gets/Sets the Number of Sub Intervals for Demands
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Added set.
        //
        public override int NumberOfSubIntervals
        {
            get
            {
                if (!m_iNumDemandSubIntervals.Cached)
                {
                    m_iNumDemandSubIntervals.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_NUM_SUB_INTERVALS);
                }

                return m_iNumDemandSubIntervals.Value;
            }
            set
            {
                m_iNumDemandSubIntervals.Value = value;
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_NUM_SUB_INTERVALS);
            }

        }

        /// <summary>
        /// Gets/sets the hour the demand reset is scheduled on.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created.
        //
        public override int DemandResetScheduledHour
        {
            get
            {
                if (false == m_iDemandResetScheduledHour.Cached)
                {
                    m_iDemandResetScheduledHour.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_DR_SCHEDULED_HOUR);
                }

                return m_iDemandResetScheduledHour.Value;
            }
            set
            {
                m_iDemandResetScheduledHour.Value = value;
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_DR_SCHEDULED_HOUR);
            }
        }

        /// <summary>
        /// Gets/sets the hour the demand reset is scheduled on.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created.
        //
        public override int DemandResetScheduledMinute
        {
            get
            {
                if (false == m_iDemandResetScheduledMinute.Cached)
                {
                    m_iDemandResetScheduledMinute.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_DR_SCHEDULED_MINUTE);
                }

                return m_iDemandResetScheduledMinute.Value;
            }
            set
            {
                m_iDemandResetScheduledMinute.Value = value;
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_DR_SCHEDULED_MINUTE);
            }
        }


        /// <summary>
        /// Gets/Sets the demand reset schedule information.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created.
        //
        public override DemandResetSchedule DemandSchedulingControl
        {
            get
            {
                DemandResetSchedule DRSchedule = DemandResetSchedule.Disabled;

                if (false == m_iDemandSchedulingControl.Cached)
                {
                    m_iDemandSchedulingControl.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_SCHEDULING_CONTROL);
                }

                if (true == Enum.IsDefined(typeof(DemandResetSchedule), m_iDemandSchedulingControl.Value))
                {
                    DRSchedule = (DemandResetSchedule)m_iDemandSchedulingControl.Value;
                }

                return DRSchedule;
            }
            set
            {
                m_iDemandSchedulingControl.Value = (int)value;

                if (true == Enum.IsDefined(typeof(DemandResetSchedule), value))
                {
                    SetMFGEDLInt((int)value, CentronTblEnum.MFGTBL0_SCHEDULING_CONTROL);
                }
                else
                {
                    SetMFGEDLInt((int)DemandResetSchedule.Disabled, CentronTblEnum.MFGTBL0_SCHEDULING_CONTROL);
                }
            }
        }

        /// <summary>
        /// Gets/Sets the day the demand reset is scheduled on.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created.
        //
        public override int DemandResetScheduledDay
        {
            get
            {
                if (false == m_iDemandResetScheduledDay.Cached)
                {
                    m_iDemandResetScheduledDay.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_DR_SCHEDULED_DAY);
                }

                return m_iDemandResetScheduledDay.Value;
            }
            set
            {
                m_iDemandResetScheduledDay.Value = value;
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_DR_SCHEDULED_DAY);
            }
        }

        /// <summary>
        /// Gets/Sets the demand threshold values.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Created.
        //
        public override List<double> DemandThresholdValues
        {
            get
            {
                object objValue;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays
                List<double> lstdblDemandThresholdValues = new List<double>();

                for (i = 0; i < DetermineDemandThresholdCount(); i++)
                {
                    anIndex1[0] = i;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_THRESHOLD_LEVEL, anIndex1, out objValue);

                    lstdblDemandThresholdValues.Add((double)objValue / 1000.0);
                }

                return lstdblDemandThresholdValues;
            }
            set
            {
                object objValue;
                int[] anIndex1 = { 0 };

                for (int i = 0; i < DetermineDemandThresholdCount(); i++)
                {
                    anIndex1[0] = i;

                    objValue = value[i] * 1000;

                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_THRESHOLD_LEVEL, anIndex1, objValue);
                }
            }
        }

        /// <summary>
        /// Gets the Number of Test Mode Sub Intervals for Demands
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //
        public override int NumberOfTestModeSubIntervals
        {
            get
            {
                if (!m_iNumTestModeDemandSubIntervals.Cached)
                {
                    m_iNumTestModeDemandSubIntervals.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_NUM_TEST_MODE_SUBINTERVALS);
                }

                return m_iNumTestModeDemandSubIntervals.Value;
            }
        }

        /// <summary>
        /// Gets the Test Mode Interval Length for Demands
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //
        public override int TestModeIntervalLength
        {
            get
            {
                if (!m_iTestModeDemandIntervalLength.Cached)
                {
                    m_iTestModeDemandIntervalLength.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_TEST_MODE_INTERVAL_LENGTH);
                }

                return m_iTestModeDemandIntervalLength.Value;
            }
        }

        /// <summary>
        /// Returns the number of minutes per load profile interval
        /// </summary>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  12/05/06 MAH 8.00.00
        /// </remarks>
        public override int LPIntervalLength
        {
            get
            {
                if (!m_iLPIntervalLength.Cached)
                {
                    m_iLPIntervalLength.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_LP_INTERVAL_LENGTH);
                }

                return m_iLPIntervalLength.Value;
            }
            set
            {
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_LP_INTERVAL_LENGTH);
                m_iLPIntervalLength.Value = value;
            }
        }

        /// <summary>
        /// Gets/sets the number of load profile channels the meter is 
        /// currently recording
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Added set.
        //
        public override int NumberLPChannels
        {
            get
            {
                if (!m_iNumLPChannels.Cached)
                {
                    m_iNumLPChannels.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_LP_NBR_CHANNELS);
                }

                return m_iNumLPChannels.Value;
            }
            set
            {
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_LP_NBR_CHANNELS);
                m_iNumLPChannels.Value = value;
            }
        }
        #endregion Itron Device Configuration

        #region ANSI Device Configuration

        /// <summary>Returns the Manufacturer Serial Number</summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/10/08 KRC
        //
        public override string MFGSerialNumber
        {
            get
            {
                if (!m_strMFGSerialNumber.Cached)
                {
                    m_strMFGSerialNumber.Value = GetSTDEDLString(StdTableEnum.STDTBL1_MFG_SERIAL_NUMBER);
                }

                return m_strMFGSerialNumber.Value;
            }
        }

        /// <summary>Gets the CT Ratio for the current device</summary>
        public override float CTRatio
        {
            get
            {
                if (!m_fltCTRatio.Cached)
                {
                    m_fltCTRatio.Value = GetMFGEDLFloat(CentronTblEnum.MFGTBL0_CT_MULTIPLIER);
                }

                return m_fltCTRatio.Value;
            }
        }

        /// <summary>Gets the VT Ratio for the current device</summary>
        public override float VTRatio
        {
            get
            {
                if (!m_fltVTRatio.Cached)
                {
                    m_fltVTRatio.Value = GetMFGEDLFloat(CentronTblEnum.MFGTBL0_VT_MULTIPLIER);
                }

                return m_fltVTRatio.Value;
            }
        }

        /// <summary>Gets the Register Multiplier for the current device</summary>
        public override float RegisterMultiplier
        {
            get
            {
                if (!m_fltRegisterMultiplier.Cached)
                {
                    m_fltRegisterMultiplier.Value = GetMFGEDLFloat(CentronTblEnum.MFGTBL0_REGISTER_MULTIPLIER);
                }

                return m_fltRegisterMultiplier.Value;
            }
        }
        
        /// <summary>
        /// Gets the Register Fullscale value from the EDL file in KW.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/10/08 RCG	2.00.06	122632 Created
        //  11/05/10 jrf 2.45.11        Added set.
        //
        public override double? RegisterFullscale
        {
            get
            {
                double? dRegFullscale = null;
                object objValue;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_REGISTER_FULL_SCALE, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_REGISTER_FULL_SCALE, null, out objValue);

                    dRegFullscale = (double)objValue / 1000.0;
                }

                return dRegFullscale;
            }
            set
            {
                object objValue = null;

                if (null != value)
                {
                    objValue = value * 1000.0;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_REGISTER_FULL_SCALE, null, objValue);
                }
            }
        }
                /// <summary>
        /// Gets the Outage Length before Cold Load Pickup in seconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.10        Added set.
        //
        public  override int OutageLength
        {
            get
            {
                if (!m_iCLPUOutageTime.Cached)
                {
                    m_iCLPUOutageTime.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_OUTAGE_LENGTH_BEFORE_CLPU);
                }

                return m_iCLPUOutageTime.Value;
            }
            set
            {
                m_iCLPUOutageTime.Value = value;
                SetMFGEDLInt(value, CentronTblEnum.MFGTBL0_OUTAGE_LENGTH_BEFORE_CLPU);
            }
        }

        /// <summary>Gets the Display mode timeout in minutes</summary>
        public override int DisplayModeTimeout
        {
            get
            {
                if (!m_iModeTimeout.Cached)
                {
                    m_iModeTimeout.Value = GetMFGEDLInt(CentronTblEnum.MFGTBL0_MODE_TIMEOUT);
                }

                return m_iModeTimeout.Value;
            }

        }
        
        #endregion ANSI Device Configuration

        #region CENTRON AMI Device Configuration

        /// <summary>
        /// Determines if User Intervention is required after a load limiting disconnect
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/20/08 KRC 10.00.00        
        // 		
        public override string LoadLimitingConnectWithoutUserIntervetion
        {
            get
            {
                if (!m_strLoadControlReconnect.Cached)
                {
                    int iDemandControl = GetMFGEDLInt(CentronTblEnum.MFGTBL0_DEMAND_CONTROL);
                    m_strLoadControlReconnect.Value = CENTRON_AMI.TranslateLoadLimitingConnectWithoutUserIntervetion(iDemandControl);
                }
                return m_strLoadControlReconnect.Value;
            }
        }

        /// <summary>
        /// Determines if Load Control is enabled and what the Threshold is if it is enabled
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/20/07 KRC 8.10.15        Adding Load Limiting summary support
        // 	06/23/08 RCG 1.50.41 116607 Changing to use correct enumeration for the threshold
        public override string LoadControlDisconnectThreshold
        {
            get
            {
                if (!m_strLoadControlThreshold.Cached)
                {
                    // This item is bit 7 of the Demand Type.
                    float fLoadControlThreshold = GetMFGEDLFloat(CentronTblEnum.MFGTBL0_THRESHOLD_LEVEL, 0);

                    m_strLoadControlThreshold.Value = CENTRON_AMI.TranslateLoadControlDisconnectThreshold(fLoadControlThreshold);
                }
                return m_strLoadControlThreshold.Value;
            }
        }
        /// <summary>
        /// Gets whether or not Daily Self read is configured.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/02/08 RCG	1.50.14		   Created
        // 11/05/10 jrf 2.45.10        Added set.
        //
        public override  bool DailySelfReadEnabled
        {
            get
            {
                if (!m_bDailySelfReadEnabled.Cached)
                {
                    string strDailySelfRead = GetMFGEDLString(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME);

                    if (strDailySelfRead == "" || strDailySelfRead == "0")
                    {
                        m_bDailySelfReadEnabled.Value = false;
                    }
                    else
                    {
                        m_bDailySelfReadEnabled.Value = true;
                    }
                }

                return m_bDailySelfReadEnabled.Value;
            }
            set
            {
                object objValue;
                byte bytValue = 0;

                if (false == value)
                {
                    bytValue = DSRT_DISABLED;
                }
                else
                {
                    bytValue = DSRT_MIDNIGHT;
                }

                objValue = bytValue;
                m_bDailySelfReadEnabled.Value = value;

                //Need to update the cached time also.
                m_strDailySelfReadTime.Value = CENTRON_AMI.DetermineDailySelfRead(bytValue);

                m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, objValue);
            }
        }

        /// <summary>
        /// Gets the configured daily self read time.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/08 KRC 
        // 06/11/08 jrf 1.50.34               Switched to call byte.Parse() that used 
        //                                    the number styles parameter to be compatible
        //                                    with the compact framework.
        public override string DailySelfReadTime
        {
            get
            {
                if (!m_strDailySelfReadTime.Cached)
                {
                    byte bySelfReadTime = 0;
                    object objValue;

                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, out objValue);
                        bySelfReadTime = (byte)objValue;
                    }

                    // Use a static method in the CENTRON AMI Device code to do the translation.
                    m_strDailySelfReadTime.Value = CENTRON_AMI.DetermineDailySelfRead(bySelfReadTime);
                }

                return m_strDailySelfReadTime.Value;
            }
        }

        /// <summary>
        /// Gets/sets the daily self read hour.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/21/10 jrf	2.45.10		   Created
        //
        public override  byte DailySelfReadHour
        {
            get
            {
                if (false == m_bytDailySelfReadHour.Cached)
                {
                    byte bytSelfReadTime = 0;
                    byte bytSelfReadHour = 0;
                    object objValue;

                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, out objValue);
                        bytSelfReadTime = (byte)objValue;
                        
                    }

                    //Self read time byte contains both the hour and minute values.
                    bytSelfReadHour = (byte)(bytSelfReadTime & DSRT_HR_MASK);

                    //Midnight is stored internaly as 24 instead of 0
                    if (DSRT_MIDNIGHT == bytSelfReadHour)
                    {
                        bytSelfReadHour = 0;
                    }

                    m_bytDailySelfReadHour.Value = bytSelfReadHour;
                }

                return m_bytDailySelfReadHour.Value;
            }
            set
            {
                object objValue;
                byte bytSelfReadTime = 0;
                byte bytSelfReadMinute = 0;
                byte bytSelfReadHour = value;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, out objValue);
                    bytSelfReadTime = (byte)objValue;
                    bytSelfReadMinute = (byte)(bytSelfReadTime & DSRT_MIN_MASK);
                }

                //Update the cached value of hour before changing it.
                m_bytDailySelfReadHour.Value = bytSelfReadHour;
                
                //Midnight is stored internally as 24 instead of 0
                if (0 == bytSelfReadHour)
                {
                    bytSelfReadHour = DSRT_MIDNIGHT;
                }

                bytSelfReadTime = (byte)(bytSelfReadHour | bytSelfReadMinute);

                objValue = bytSelfReadTime;

                //Need to update the cached time values.
                m_strDailySelfReadTime.Value = CENTRON_AMI.DetermineDailySelfRead(bytSelfReadTime);
                
                m_bytDailySelfReadMinute.Value = ConvertFromInternalDailySelfReadMinute(bytSelfReadMinute);

                m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, objValue);
            }
        }
        
        /// <summary>
        /// Gets/sets the daily self read minute.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/21/10 jrf	2.45.10		   Created
        //
        public override  byte DailySelfReadMinute
        {
            get
            {
                if (false == m_bytDailySelfReadMinute.Cached)
                {
                    byte bytSelfReadTime = 0;
                    byte bytSelfReadMinute = 0;
                    object objValue;

                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null))
                    {
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, out objValue);
                        bytSelfReadTime = (byte)objValue;

                    }

                    //Self read time byte contains both the hour and minute values.
                    bytSelfReadMinute = (byte)(bytSelfReadTime & DSRT_MIN_MASK);
                    m_bytDailySelfReadMinute.Value = ConvertFromInternalDailySelfReadMinute(bytSelfReadMinute);
                }

                return m_bytDailySelfReadMinute.Value;
            }
            set
            {
                object objValue;
                byte bytSelfReadTime = 0;
                byte bytSelfReadMinute = ConvertToInternalDailySelfReadMinute(value);
                byte bytSelfReadHour = 0;

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, out objValue);
                    bytSelfReadTime = (byte)objValue;
                    bytSelfReadHour = (byte)(bytSelfReadTime & DSRT_HR_MASK);
                }

                bytSelfReadTime = (byte)(bytSelfReadHour | bytSelfReadMinute);

                objValue = bytSelfReadTime;

                //Midnight is stored as 24 instead of 0.
                //Need to change before caching below.
                if (DSRT_MIDNIGHT == bytSelfReadHour)
                {
                    bytSelfReadHour = 0;
                }

                //Need to update the cached time values.
                m_strDailySelfReadTime.Value = CENTRON_AMI.DetermineDailySelfRead(bytSelfReadTime);
                m_bytDailySelfReadHour.Value = bytSelfReadHour;
                m_bytDailySelfReadMinute.Value = value;

                m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME, null, objValue);
            }
        }

        /// <summary>
        /// Gets a TimeSpan object that represents the Time Zone Offset
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/06 RCG 7.40.00 N/A    Created
        //
        public override TimeSpan TimeZoneOffset
        {
            get
            {
                if (!m_tsTimeZoneOffset.Cached)
                {
                    int iFlag = GetSTDEDLInt(StdTableEnum.STDTBL51_TIME_FUNC_FLAG2);
                    if (!StdTable51.TranslateIsTimeZoneAvailable((byte)iFlag))
                    {
                        throw new NotSupportedException("This device does not support time zone offset");
                    }
                    else
                    {
                        short sTimeZoneOffset = GetSTDEDLShort(StdTableEnum.STDTBL53_TIME_ZONE_OFFSET);

                        m_tsTimeZoneOffset.Value =
                            TimeSpan.FromMinutes((double)sTimeZoneOffset);
                    }
                }
                
                return m_tsTimeZoneOffset.Value;
            }

            set
            {
                short sTimeZoneOffset = (short)(value.TotalMinutes);
                int iFlag = GetSTDEDLInt(StdTableEnum.STDTBL51_TIME_FUNC_FLAG2);

                if (true == StdTable51.TranslateIsTimeZoneAvailable((byte)iFlag))
                {
                    SetSTDEDLShort(sTimeZoneOffset, StdTableEnum.STDTBL53_TIME_ZONE_OFFSET);
                    SetMFGEDLInt(sTimeZoneOffset, CentronTblEnum.MFGTBL0_TIME_ZONE_OFFSET);
                 }

                m_tsTimeZoneOffset.Flush();
            }
        }
        
        /// <summary>
        /// Gets the list of configured energy items.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG	2.20.19		   Created

        public override List<LID> EnergyConfigLIDs
        {
            get
            {

                CentronMonoLID EnergyLID;
                object Value;
                int[] anIndex1 = { 0 };
                List<LID> EnergyConfig = new List<LID>();

                try
                {

                    for (int i = 0; i < DetermineEnergyConfigCount(); i++)
                    {
                        anIndex1[0] = i;
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL0_ENERGY_LID, anIndex1, out Value);
                        EnergyLID = new CentronMonoLID(SEC_ENERGY_LID_BASE + (byte)Value);
                        EnergyConfig.Add(EnergyLID);
                    }
                }
                catch (Exception)
                {
                    EnergyConfig = null;
                }

                return EnergyConfig;
            }
            set
            {
                object Value;
                int[] anIndex1 = { 0 };

                //foreach (LID lid in value)
                for (int i = 0; i < DetermineEnergyConfigCount(); i++)
                {
                    anIndex1[0] = i;
                    Value = value[i].lidValue - SEC_ENERGY_LID_BASE;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_ENERGY_LID, anIndex1, Value);
                }
            }

        }

        /// <summary>
        /// Gets the list of configured Demands LIDs
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG	2.20.19		   Created

        public override List<LID> DemandConfigLIDs
        {
            get
            {
                object Value;
                int[] anIndex1 = { 0 };
                int i; //Index into Summation arrays
                CentronMonoLID DemandLID;
                List<LID> DemandConfigLID = new List<LID>();

                for (i = 0; i < DetermineDemandConfigCount(); i++)
                {
                    anIndex1[0] = i;
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL0_DEMAND_DEFINITION, anIndex1, out Value);
                    if (null != Value)
                    {
                        DemandLID = new CentronMonoLID((uint)Value);
                        DemandConfigLID.Add(DemandLID);
                    }
                }

                return DemandConfigLID;
            }
            set
            {
                object Value;
                int[] anIndex1 = { 0 };
                
                //foreach (LID lid in value)
                for (int i = 0; i < DetermineDemandConfigCount(); i++)
                {
                    anIndex1[0] = i;
                    Value = value[i].lidValue;
                    m_CenTables.SetValue(CentronTblEnum.MFGTBL0_DEMAND_DEFINITION, anIndex1, Value);
                }
            }
        }


        #endregion CENTRON AMI Device Configuration

        #region Itron Device Status

        /// <summary>
        /// Property used to get the Number of Demand Resets from the meter
        /// </summary>
        public override int NumDemandResets
        {
            get
            {
                if (!m_iNumDemandResets.Cached)
                {
                    // This item is bit 7 of the Demand Type.
                    m_iNumDemandResets.Value = GetSTDEDLInt(StdTableEnum.STDTBL23_NBR_DEMAND_RESETS);
                }

                return m_iNumDemandResets.Value;
            }
        }

        /// <summary>
        /// Gets the date that the device was programmed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/08/08 KRC 2.10.00        Fixed reference time to be frim 2000

        public override DateTime DateProgrammed
        {
            get
            {
                if (!m_dtDateProgrammed.Cached)
                {
                    uint uiTemp = 0;

                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL0_CONFIG_TIME, null))
                    {
                        uiTemp = GetMFGEDLUint(CentronTblEnum.MFGTBL0_CONFIG_TIME);

                        m_dtDateProgrammed.Value = MeterConfigurationReferenceTime;

                        // Value in 2048 is the number of seconds since Jan. 1, 2000, so to get
                        //  the value returned to Jan. 1, 2000.
                        m_dtDateProgrammed.Value = m_dtDateProgrammed.Value.AddSeconds((double)uiTemp);
                    }
                    else
                    {
                        m_dtDateProgrammed.Value = MeterConfigurationReferenceTime;
                    }
                   
                }

                return m_dtDateProgrammed.Value;
            }
        }
        #endregion Itron Device Status

        #region ANSI Device Status


        /// <summary>
        /// Property to get the hardware version from table 01. 
        /// need this item.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/24/10 AF  2.43.01 160368 The hardware version needs to mask off the Prism
        //                              Lite upper nibble
        //
        public override float HWRevision
        {
            get
            {
                if (!m_fltHWRevision.Cached)
                {
                    string strVersion;
                    string strRevision;

                    strVersion = GetSTDEDLString(StdTableEnum.STDTBL1_HW_VERSION_NUMBER);
                    strRevision = GetSTDEDLString(StdTableEnum.STDTBL1_HW_REVISION_NUMBER);
                    m_fltHWRevision.Value = (float)float.Parse(strVersion, CultureInfo.InvariantCulture) +
                            ((float)float.Parse(strRevision, CultureInfo.InvariantCulture) / (int)1000);

                    if (VersionChecker.CompareTo(m_fltHWRevision.Value, PRISM_LITE_REVISION) >= 0)
                    {
                        // It's a Prism Lite meter so subtract to get the real version
                        m_fltHWRevision.Value -= PRISM_LITE_REVISION;
                    }
                }

                return m_fltHWRevision.Value;
            }
        }

        /// <summary>
        /// Property to determine if the meter is in DST
        /// </summary>
        public override bool IsMeterInDST
        {
            get
            {
                if (!m_blnMeterInDST.Cached)
                {
                    m_blnMeterInDST.Value = GetSTDEDLBool(StdTableEnum.STDTBL52_DST_FLAG);
                }

                return m_blnMeterInDST.Value;
            }
        }

        /// <summary>
        /// Property to determine if the meter is in Test Mode
        /// </summary>
        public override bool IsMeterInTestMode
        {
            get
            {
                if (!m_blnMeterInTestMode.Cached)
                {
                    m_blnMeterInTestMode.Value = GetSTDEDLBool(StdTableEnum.STDTBL3_TEST_MODE_FLAG);
                }

                return m_blnMeterInTestMode.Value;
            }
        }
        #endregion ANSI Device Status

        #region CENTRON AMI Device Status

        /// <summary>
        /// Gets the Han module type (Zigbee)
        /// </summary>
        public override string HanModType
        {
            get
            {
                if (!m_strHanModType.Cached)
                {
                    byte byHANType = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_HAN_MOD_TYPE);
                    m_strHanModType.Value = OpenWayMfgTable2108.TranslationHanModType(byHANType);
                }

                return m_strHanModType.Value;
            }
        }

        /// <summary>
        /// Gets the Han module version.revision
        /// </summary>
        public override string HanModVer
        {
            get
            {
                if (!m_strHanModVer.Cached)
                {
                    byte byVersion = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_HAN_MOD_VER);
                    byte byRevison = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_HAN_MOD_REV);

                    m_strHanModVer.Value = byVersion.ToString(CultureInfo.CurrentCulture)
                            + "." + byRevison.ToString("d3", CultureInfo.CurrentCulture);
                }

                return m_strHanModVer.Value;
            }
        }

        /// <summary>
        /// Gets the Han module build number
        /// </summary>
        public override string HanModBuild
        {
            get
            {
                if (!m_strHanModBuild.Cached)
                {
                    byte byBuild = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_HAN_MOD_BUILD);
                    m_strHanModBuild.Value = byBuild.ToString("d3", CultureInfo.CurrentCulture);
                }

                return m_strHanModBuild.Value;
            }
        }

        /// <summary>
        /// Gets the display version.revision fom MFG Table 2108
        /// </summary>
        public override string DisplayModVer
        {
            get
            {
                if (!m_strDispModVer.Cached)
                {
                    byte byVersion = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_DISPLAY_FW_VER);
                    byte byRevison = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_DISPLAY_FW_REV);

                    m_strDispModVer.Value = byVersion.ToString(CultureInfo.CurrentCulture)
                            + "." + byRevison.ToString("d3", CultureInfo.CurrentCulture);
                }

                return m_strDispModVer.Value;
            }
        }

        /// <summary>
        /// Gets the display Build fom MFG Table 2108
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 12/08/08 RCG 2.10.00 N/A		Created

        public override string DisplayModBuild
        {
            get
            {
                if (!m_strDispModBuild.Cached)
                {
                    byte byBuild = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_DISPLAY_FW_BUILD);
                    m_strDispModBuild.Value = byBuild.ToString("d3", CultureInfo.CurrentCulture);
                }

                return m_strDispModBuild.Value;
            }
        }

        /// <summary>
        /// Retrieves the HAN MAC Address from table 2104
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/09/08 AF  1.51.04 116680 Created
        //
        public override string HanMACAddress
        {
            get
            {
                string strMAC = "";

                // Check on the existence of the data
                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL56_SRV_MAC_ADDRESS, null))
                {
                    object objValue = null;
                    ulong ulMAC = 0;

                    m_CenTables.GetValue(CentronTblEnum.MFGTBL56_SRV_MAC_ADDRESS, null, out objValue);
                    ulMAC = (ulong)objValue;
                    strMAC = ulMAC.ToString("X16", CultureInfo.CurrentCulture);
                }

                return strMAC;
            }
        }

        /// <summary>
        /// Gets whether or not Signed Authorization is required.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/09 RCG 2.30.16        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway

        public override bool? RequireSignedAuthorization
        {
            get
            {
                bool? bRequired = null;
                object objValue = null;

                if (m_CenTables.IsCached((long)CentronTblEnum.MfgTbl145RequireSignedAuthorization, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MfgTbl145RequireSignedAuthorization, null, out objValue);
                    bRequired = (bool)objValue;
                }
                return bRequired;
            }
        }

        /// <summary>Returns the current register data from table 23</summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/15/07 mcm 8.10.05  	    Created
        // 07/29/09 RCG 2.20.19 134394 Rewriting to use new standard tables and support coincidents

        public override List<Quantity> CurrentRegisters
        {
            get
            {
                if (null == m_Registers)
                {
                    GetCurrentRegisters();
                }

                return m_Registers;
            } // get
        } // CurrentRegisters

        /// <summary>
        /// Return true if the EDL file contains Network Statistics data. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/11/07 RCG 1.00.00        Created
        //  06/18/07 RCG 1.50.38        Adding check for 122
        //  07/30/10 AF  2.42.09        Added support for the M2 Gateway meter
        //
        public override bool ContainsNetworkStatistics
        {
            get
            {
                bool bContainsNetworkStats = false;
                ushort usNbrEntries = 0;
                object objValue = null;

                // Table 121 is now always included so we need to check table 122 as well.
                if (m_CenTables.IsCached((long)StdTableEnum.STDTBL121_NBR_STATISTICS, null) &&
                    m_CenTables.IsAllCached(122))
                {
                    m_CenTables.GetValue(StdTableEnum.STDTBL121_NBR_STATISTICS, null, out objValue);
                    usNbrEntries = (ushort)objValue;
                }

                if (usNbrEntries > 0)
                {
                    bContainsNetworkStats = true;
                }

                return bContainsNetworkStats;
            }
        }

        /// <summary>
        /// Returns true of the EDL has Device Status Data in it.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/05/08 jrf 1.50.32 114519 Created.
        // 08/02/10 AF  2.42.11         Added support for M2 Gateway
        //    
        public override bool ContainsRFLANNeighbors
        {
            get
            {
                bool bContainsNeighbors = false;

                if (m_CenTables.IsAllCached(2078))
                {
                    bContainsNeighbors = true;
                }
                return bContainsNeighbors;
            }
        }

        /// <summary>
        /// Gets whether or not the EDL file contains the SiteScan Toolbox data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        public override bool ContainsSiteScanToolbox
        {
            get
            {
                return Table2091 != null;
            }
        }

        /// <summary>
        /// Gets the Comm module version.revision
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/03/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public override string CommModVer
        {
            get
            {
                byte byVersion = 0;
                byte byRevison = 0;

                if (!m_strCommModVer.Cached)
                {
                    if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL60_COMM_MOD_VER, null))
                    {
                        byVersion = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_COMM_MOD_VER);
                        byRevison = (byte)GetMFGEDLInt(CentronTblEnum.MFGTBL60_COMM_MOD_REV);
                    }

                    m_strCommModVer.Value = byVersion.ToString(CultureInfo.CurrentCulture) +
                        "." + byRevison.ToString("d3", CultureInfo.CurrentCulture);
                }

                return m_strCommModVer.Value;
            }
        }

        /// <summary>
        /// Retrieves the RFLAN MAC Address from standard table 122.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/09/08 AF  1.51.04 116680 Created
        //
        public override string RFLANMACAddress
        {
            get
            {
                string strMAC = "";
                //array given to GetValue that represents the block index of the value
                //that is being accessed
                int[] IndexArray = { 0 };

                // Check on the existence of the data
                if (m_CenTables.IsCached((long)StdTableEnum.STDTBL122_NATIVE_ADDRESS, IndexArray))
                {
                    object objValue = null;
                    byte[] byaData;

                    m_CenTables.GetValue(StdTableEnum.STDTBL122_NATIVE_ADDRESS, IndexArray, out objValue);
                    byaData = (byte[])objValue;
                    ulong ulMAC = (ulong)(byaData[0] + (byaData[1] << 8) + (byaData[2] << 16) + (byaData[3] << 24));
                    strMAC = ulMAC.ToString("X8", CultureInfo.CurrentCulture);
                }

                return strMAC;
            }
        }


 #endregion CENTRON AMI Device Status

        #region Private Methods

        /// <summary>
        /// Gets the short value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        /// <returns>Requested short</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/??                    Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        private  short GetSTDEDLShort(StdTableEnum StdTableEnumValue)
        {
            short sTemp = 0;
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                sTemp = (short)Value;
            }
            return sTemp;
        }

        /// <summary>
        /// Gets the float value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <returns>Requested float</returns>
        private float GetMFGEDLFloat(CentronTblEnum CentronTblEnumValue)
        {
            return GetMFGEDLFloat(CentronTblEnumValue, -1);
        }

        /// <summary>
        /// Sets the short value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="sValue">Value to set.</param>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/23/10 jrf 2.44.04        Created
        //
        private void SetSTDEDLShort(short sValue, StdTableEnum StdTableEnumValue)
        {
            object Value = sValue;

            m_CenTables.SetValue(StdTableEnumValue, null, Value);
        }

        /// <summary>
        /// Gets the string value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        /// <returns>Requested String</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/??                    Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        private string GetSTDEDLString(StdTableEnum StdTableEnumValue)
        {
            string strTemp = "";
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                strTemp = Value.ToString();
            }
            return strTemp;
        }
              
        
        /// <summary>
        /// Sets the string value of the EDL defined by the supplied enumeration.
        /// </summary>
        /// <param name="strValue">Value to set.</param>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        /// <returns>Requested String</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/25/10 jrf 2.45.10        Created
        //
        private void SetSTDEDLString(String strValue, StdTableEnum StdTableEnumValue)
        {
            object Value = strValue;

            m_CenTables.SetValue(StdTableEnumValue, null, Value);
        }

        /// <summary>
        /// Gets the string value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="stdTableEnum">Centron AMI specific Enumberation Value</param>
        /// <param name="aiIndex">Parameterized index.</param>
        /// <returns>Requested ushort</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#        Description
        // -------- --- ------- ------------- ---------------------------------------
        // 06/11/08 jrf 1.50.34               Switched to call ushort.Parse() that used 
        //                                    the number styles parameter to be compatible
        //                                    with the compact framework.
        //  08/02/10 AF  2.42.11              Added support for M2 Gateway
        //
        private ushort GetSTDEDLUShort(StdTableEnum stdTableEnum, params int[] aiIndex)
        {
            ushort usTemp = 0;
            object Value;

            if (m_CenTables.IsCached((long)stdTableEnum, aiIndex))
            {
                m_CenTables.GetValue(stdTableEnum, aiIndex, out Value);
                usTemp = ushort.Parse(Value.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture);
            }
            return usTemp;
        }

        /// <summary>
        /// Gets the int value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        /// <returns>Requested int</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/??                    Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        private int GetSTDEDLInt(StdTableEnum StdTableEnumValue)
        {
            int intTemp = 0;
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                intTemp = (int)int.Parse(Value.ToString(), CultureInfo.CurrentCulture);
            }
            return intTemp;
        }

        /// <summary>
        /// Gets the float value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        /// <returns>Requested float</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/??                    Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        private float GetSTDEDLFloat(StdTableEnum StdTableEnumValue)
        {
            float fltTemp = 0;
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                fltTemp = float.Parse(Value.ToString(), CultureInfo.CurrentCulture);
            }
            return fltTemp;
        }

        /// <summary>
        /// Sets the int value in the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="iValue"></param>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <returns>Requested int</returns>
        private void SetMFGEDLInt(int iValue, CentronTblEnum CentronTblEnumValue)
        {
            SetMFGEDLInt(iValue, CentronTblEnumValue, -1);
        }

        /// <summary>
        /// Sets the int value in the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="iValue"></param>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <param name="Index">The index of the value.</param>
        /// <returns>Requested int</returns>
        private void SetMFGEDLInt(int iValue, CentronTblEnum CentronTblEnumValue, int Index)
        {
            int[] aiIndex = { 0 };

            if (-1 == Index)
            {
                aiIndex = null;
            }
            else
            {
                aiIndex[0] = Index;
            }

            if (m_CenTables.IsCached((long)CentronTblEnumValue, aiIndex))
            {
                m_CenTables.SetValue(CentronTblEnumValue, aiIndex, iValue);
            }
        }

        /// <summary>
        /// Gets the bool value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        /// <returns>Requested bool</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/??                    Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        private bool GetSTDEDLBool(StdTableEnum StdTableEnumValue)
        {
            bool blnTemp = false;
            object Value;
            if (m_CenTables.IsCached((long)StdTableEnumValue, null))
            {
                m_CenTables.GetValue(StdTableEnumValue, null, out Value);
                blnTemp = (bool)Value;
            }
            return blnTemp;
        }

        /// <summary>
        /// Sets the bool value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="blnValue">The value to set</param>
        /// <param name="StdTableEnumValue">Standard Enumberation Value</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  
        private void SetSTDEDLBool(bool blnValue, StdTableEnum StdTableEnumValue)
        {
            m_CenTables.SetValue(StdTableEnumValue, null, blnValue);
        }

        /// <summary>
        /// Gets the int value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <returns>Requested int</returns>
        private int GetMFGEDLInt(CentronTblEnum CentronTblEnumValue)
        {
            return GetMFGEDLInt(CentronTblEnumValue, -1);
        }

        /// <summary>
        /// This method will take a list of streams that have holes in them and create one full stream.
        /// The unknown parts will be filled with null data.
        /// </summary>
        /// <param name="lstTableData"></param>
        /// <param name="uiOffset"></param>
        /// <param name="uiSize"></param>
        /// <returns>Stream - A complete stream with unknown data set to null</returns>
        private Stream BuildStream(IList<TableData> lstTableData, uint uiOffset, uint uiSize)
        {
            MemoryStream strmNewStream = new MemoryStream();
            uint uiStreamOffset = uiOffset;

            foreach (TableData td in lstTableData)
            {
                if (td.Offset != uiStreamOffset)
                {
                    //The offset of our present stream does not match where we think we should be so we need to
                    //  add in some filler data.
                    int iByteCount = (int)td.Offset - (int)uiStreamOffset;
                    byte[] byFiller = new byte[iByteCount];

                    MemoryStream strmFiller = new MemoryStream(byFiller);

                    strmFiller.WriteTo(strmNewStream);
                    uiStreamOffset += (uint)strmFiller.Length;
                }

                td.PSEM.WriteTo(strmNewStream);
                uiStreamOffset += (uint)td.PSEM.Length;
            }

            // Now we have gone through each of the Streams passed in, we need to make sure there
            //  was no data missing from the end.
            if ((uint)strmNewStream.Length < uiSize)
            {
                int iByteCount = (int)uiSize - (int)strmNewStream.Length;
                byte[] byFiller = new byte[iByteCount];

                MemoryStream strmFiller = new MemoryStream(byFiller);

                strmFiller.WriteTo(strmNewStream);
                uiStreamOffset += (uint)strmFiller.Length;
            }

            strmNewStream.Position = 0;

            return strmNewStream;
        }

        /// <summary>
        /// Gets the string value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <returns>Requested String</returns>
        private string GetMFGEDLString(CentronTblEnum CentronTblEnumValue)
        {
            return GetMFGEDLString(CentronTblEnumValue, -1);
        }


        /// <summary>
        /// Gets the string value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <param name="Index">The index of the value.</param>
        /// <returns>Requested String</returns>
        private string GetMFGEDLString(CentronTblEnum CentronTblEnumValue, int Index)
        {
            string strTemp = "";
            object Value;
            int[] aiIndex = { 0 };

            if (-1 == Index)
            {
                aiIndex = null;
            }
            else
            {
                aiIndex[0] = Index;
            }

            if (m_CenTables.IsCached((long)CentronTblEnumValue, aiIndex))
            {
                m_CenTables.GetValue(CentronTblEnumValue, aiIndex, out Value);
                strTemp = Value.ToString();
            }

            return strTemp;
        }

        /// <summary>
        /// Sets the string value in the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="strValue">Value to set.</param>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <param name="Index">The index of the value.</param>
        /// <returns>Requested String</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/05/10 jrf 2.45.11        Created.
        //
        private void SetMFGEDLString(string strValue, CentronTblEnum CentronTblEnumValue, int Index)
        {
            object objValue = strValue;
            int[] aiIndex = { 0 };

            if (-1 == Index)
            {
                aiIndex = null;
            }
            else
            {
                aiIndex[0] = Index;
            }

            m_CenTables.SetValue(CentronTblEnumValue, aiIndex, objValue);
        }

        /// <summary>
        /// Gets the int value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <param name="Index">The index of the value.</param>
        /// <returns>Requested int</returns>
        private int GetMFGEDLInt(CentronTblEnum CentronTblEnumValue, int Index)
        {
            int intTemp = 0;
            object Value;
            int[] aiIndex = { 0 };

            if (-1 == Index)
            {
                aiIndex = null;
            }
            else
            {
                aiIndex[0] = Index;
            }

            if (m_CenTables.IsCached((long)CentronTblEnumValue, aiIndex))
            {
                m_CenTables.GetValue(CentronTblEnumValue, aiIndex, out Value);
                intTemp = int.Parse(Value.ToString(), CultureInfo.CurrentCulture);
            }

            return intTemp;
        }

        
        /// <summary>
        /// Gets the float value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <param name="Index">The index of the value.</param>
        /// <returns>Requested float</returns>
        private float GetMFGEDLFloat(CentronTblEnum CentronTblEnumValue, int Index)
        {
            float fltTemp = 0.0F;
            object Value;
            int[] aiIndex = { 0 };

            if (-1 == Index)
            {
                aiIndex = null;
            }
            else
            {
                aiIndex[0] = Index;
            }

            if (m_CenTables.IsCached((long)CentronTblEnumValue, aiIndex))
            {
                m_CenTables.GetValue(CentronTblEnumValue, aiIndex, out Value);
                fltTemp = float.Parse(Value.ToString(), CultureInfo.InvariantCulture);
            }

            return fltTemp;
        }

        /// <summary>
        /// Gets the bool value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <returns>Requested bool</returns>
        private bool GetMFGEDLBool(CentronTblEnum CentronTblEnumValue)
        {
            bool blnTemp = false;
            object Value;
            if (m_CenTables.IsCached((long)CentronTblEnumValue, null))
            {
                m_CenTables.GetValue(CentronTblEnumValue, null, out Value);
                blnTemp = (bool)Value;
            }

            return blnTemp;
        }

        /// <summary>
        /// Sets the bool value of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="blnValue">Value to set</param>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#         Description
        //  -------- --- ------- -------------- -------------------------------------------
        //  11/16/10 jrf 2.45.13                 Created
        //
        private void SetMFGEDLBool(bool blnValue, CentronTblEnum CentronTblEnumValue)
        {
            m_CenTables.SetValue(CentronTblEnumValue, null, blnValue);
        }

        /// <summary>
        /// Gets the uint value out of the EDL defined by the supplied Enumeration
        /// </summary>
        /// <param name="CentronTblEnumValue">Centron AMI specific Enumberation Value</param>
        /// <returns>Requested uint</returns>
        private uint GetMFGEDLUint(CentronTblEnum CentronTblEnumValue)
        {
            uint uiTemp = 0;
            object Value;
            if (m_CenTables.IsCached((long)CentronTblEnumValue, null))
            {
                m_CenTables.GetValue(CentronTblEnumValue, null, out Value);
                uiTemp = (uint)Value;
            }

            return uiTemp;
        }


        #endregion

        #region Protected Methods

        /// <summary>
        /// This method sets a value in the Centron Tables.
        /// </summary>
        /// <param name="idElement">The ID of the element to set</param>
        /// <param name="anIndex">The index(es) to find the element</param>
        /// <param name="objValue">The value to set</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#         Description
        //  -------- --- ------- -------------- -------------------------------------------
        //  11/16/10 jrf 2.45.13                 Created
        //
        protected override void SetValue(long idElement, int[] anIndex, object objValue)
        {
            m_CenTables.SetValue(idElement, anIndex, objValue);
        }

        /// <summary>
        /// Load the EDL file into the centron table variable
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/26/06 RDB				   Created
        // 08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected override void LoadFile()
        {
            XmlReader xmlReader;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.CheckCharacters = false;

            xmlReader = XmlReader.Create(m_strEDLFile, settings);

            m_CenTables = new CentronTables();

            if (EDLDeviceTypes.M2GatewayDevice != EDLFile.DetermineDeviceType(m_strEDLFile))
            {
                m_CenTables.LoadEDLFile(xmlReader);
            }
        }//LoadFile

        /// <summary>
        /// Save the EDL file from the centron table variable
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/26/06 RDB				   Created
        // 08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected override void SaveFile()
        {
            XmlWriter xmlWriter;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.ASCII;
            settings.Indent = true;
            settings.CheckCharacters = false;


            xmlWriter = XmlWriter.Create(m_strEDLFile, settings);

            m_CenTables.SaveEDLFile(xmlWriter, null, AllowTablesForSaving, AllowFieldExport);


        }//SaveFile

        /// <summary>
        /// Used to determine which tables will be written to the EDL file.  We are 
        /// excluding decade 4. 
        /// </summary>
        /// <param name="usTableID">Table ID to check.</param>
        /// <returns>True if the table can be written, false otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/19/10 jrf	9.70.07        Created
        //
        private bool AllowTablesForSaving(ushort usTableID)
        {
            bool blnAllow = false;

            if (usTableID >= 41 && usTableID < 50)
            {
                blnAllow = false;
            }
            else
            {
                blnAllow = AllowTableExport(usTableID);
            }

            return blnAllow;
        }

        /// <summary>
        /// Build sup the Network Statistic Structure
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected override void BuildNetworkStatistics()
        {
            IList<TableData> lsttblData121 = null;
            IList<TableData> lsttblData127 = null;

            //Get Table 121
            if (m_CenTables.IsAllCached(121))
            {
                lsttblData121 = m_CenTables.BuildPSEMStreams(121);
            }

            lsttblData121[0].PSEM.Position = 0;
            PSEMBinaryReader Tbl121BinaryReader = new PSEMBinaryReader(lsttblData121[0].PSEM);
            CStdTable121 tbl121 = new CStdTable121(Tbl121BinaryReader);


            if (m_CenTables.IsAllCached(127))
            {
                lsttblData127 = m_CenTables.BuildPSEMStreams(127);
            }

            lsttblData127[0].PSEM.Position = 0;
            PSEMBinaryReader Tbl127BinaryReader = new PSEMBinaryReader(lsttblData127[0].PSEM);
            CStdTable127 tbl127 = new CStdTable127(Tbl127BinaryReader, tbl121);

            m_lstNetworkStatistic = tbl127.GetStatistics();
            //Get Table 127
        }

        /// <summary>
        /// Get the list of supported Std and Mfg Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected override  void GetSupportedCommunicationsEvents()
        {
            // Make sure that the EDL file contains the Log tables
            if (m_CenTables.IsTableKnown(2159) && m_CenTables.IsAllCached(2159))
            {
                // Get 2159 - HAN/LAN Actual Limiting Table
                IList<TableData> lsttblData2159 = m_CenTables.BuildPSEMStreams(2159);
                lsttblData2159[0].PSEM.Position = 0;
                PSEMBinaryReader Tbl2159BinaryReader = new PSEMBinaryReader(lsttblData2159[0].PSEM);
                MFGTable2159 tbl2159 = new MFGTable2159(Tbl2159BinaryReader);

                // Make sure that the EDL file contains the Log tables
                if (m_CenTables.IsTableKnown(2160) && m_CenTables.IsAllCached(2160))
                {
                    //Get Table 2160 - Supported Events
                    IList<TableData> lsttblData2160 = m_CenTables.BuildPSEMStreams(2160);
                    lsttblData2160[0].PSEM.Position = 0;
                    PSEMBinaryReader Tbl2160BinaryReader = new PSEMBinaryReader(lsttblData2160[0].PSEM);
                    MFGTable2160 tbl2160 = new MFGTable2160(Tbl2160BinaryReader, tbl2159);

                    m_lstSupportedStdEvents = tbl2160.StdEventSupported;
                    m_lstSupportedMFGEvents = tbl2160.MfgEventSupported;
                }
            }
        }

        /// <summary>
        /// Reads the array of event entries read from table 76.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/25/08 jrf                Created.
        // 08/03/10 AF  2.42.11        Added M2 Gateway support
        // 08/10/10 AF  2.42.17        Added M2 Gateway event dictionary
        //
        protected override  void GetEventEntries()
        {
            object Value = null;
            ushort NbrValidEntries = 0;
            int[] anIndex = { 0 };
            bool EventDateTimeFlag = true; //always true
            bool EventNbrFlag = false;
            bool EventSeqNbrFlag = true; //always true
            byte EventDataLength = 0;


            if (m_CenTables.IsAllCached(71))
            {
                m_CenTables.GetValue(StdTableEnum.STDTBL71_EVENT_NUMBER_FLAG, null, out Value);
                EventNbrFlag = (bool)Value;

                m_CenTables.GetValue(StdTableEnum.STDTBL71_HIST_DATA_LENGTH, null, out Value);
                EventDataLength = (byte)Value;
                m_CenTables.GetValue(StdTableEnum.STDTBL76_NBR_VALID_ENTRIES, null, out Value);
                NbrValidEntries = (ushort)Value;
                m_EventEntries = new HistoryEntry[NbrValidEntries];

                for (int i = 0; i < NbrValidEntries; i++)
                {
                    anIndex[0] = i;
                    m_EventEntries[i] = new HistoryEntry(EventDateTimeFlag, EventNbrFlag, EventSeqNbrFlag, EventDataLength,
                                m_EventDictionary);

                    m_CenTables.GetValue(StdTableEnum.STDTBL76_EVENT_TIME, anIndex, out Value);
                    m_EventEntries[i].HistoryTime = (DateTime)Value;


                    m_CenTables.GetValue(StdTableEnum.STDTBL76_EVENT_CODE, anIndex, out Value);
                    m_EventEntries[i].HistoryCode = (ushort)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL76_EVENT_ARGUMENT, anIndex, out Value);
                    m_EventEntries[i].HistoryArgument = (byte[])Value;
                }
            }
        }

  
        /// <summary>
        /// Gets the Vh Low Threshold for Voltage Monitoring.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 RCG	1.50.26		   Created

        protected override float VMVhLowThreshold
        {
            get
            {
                ushort usDivisor;
                float fThreshold = 0.0F;
                object objValue;

                if (DeviceType == EDLDeviceTypes.OpenWayCentronBasicPoly 
                    || DeviceType == EDLDeviceTypes.OpenWayCentronAdvPoly)
                {
                    throw new InvalidOperationException("This method not supported for the current device type");
                }

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL103_DIVISOR, null)
                    && m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL103_DIVISOR, null, out objValue);
                    usDivisor = (ushort)objValue;

                    // Get the thresholds.
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD, null, out objValue);
                    fThreshold = (ushort)objValue / (float)usDivisor;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Voltage Monitoring");
                }

                return fThreshold;
            }
        }

        /// <summary>
        /// Gets the Vh High Threshold for Voltage Monitoring.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 RCG	1.50.26		   Created

        protected override float VMVhHighThreshold
        {
            get
            {
                ushort usDivisor;
                float fThreshold = 0.0F;
                object objValue;

                if (DeviceType == EDLDeviceTypes.OpenWayCentronBasicPoly 
                    || DeviceType == EDLDeviceTypes.OpenWayCentronAdvPoly)
                {
                    throw new InvalidOperationException("This method not supported for the current device type");
                }

                if (m_CenTables.IsCached((long)CentronTblEnum.MFGTBL103_DIVISOR, null)
                    && m_CenTables.IsCached((long)CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD, null))
                {
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL103_DIVISOR, null, out objValue);
                    usDivisor = (ushort)objValue;

                    // Get the thresholds.
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD, null, out objValue);
                    fThreshold = (ushort)objValue / (float)usDivisor;
                }
                else
                {
                    throw new InvalidOperationException("This EDL file does not contain Voltage Monitoring");
                }

                return fThreshold;
            }
        }
        
        /// <summary>
        /// Creates a VMData object from the EDL file.
        /// </summary>
        /// <returns>The VMData object.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/08 RCG 1.50.23 N/A    Created

        protected override VMData GetVoltageMonitoringData()
        {
            VMData VoltageData = null;
            object objValue;
            bool bEnabled = false;
            ushort usMaxBlocks;
            ushort usUsedBlocks;
            ushort usIntervalsPerBlock;
            ushort usValidIntervals;
            ushort usLastBlock;
            ushort usStartBlock;
            ushort usDivisor;
            byte[] byaStatus;
            byte byIntervalLength;
            byte byNumPhases;
            List<VMInterval> Intervals = new List<VMInterval>();
            VMStatusFlags LastIntervalStatus;

            // Make sure that the EDL file contains the VM tables
            if (m_CenTables.IsTableKnown(2149) && m_CenTables.IsAllCached(2149))
            {
                // Tables are there now check to see if VM is enabled.
                m_CenTables.GetValue(CentronTblEnum.MFGTBL102_ENABLE_FLAG, null, out objValue);
                bEnabled = (bool)objValue;

                if (bEnabled == true)
                {
                    // VM is enabled so we should be able to create the object.
                    VoltageData = new VMData();

                    // Get the data necessary for reading the data from 2152
                    m_CenTables.GetValue(CentronTblEnum.MFGTBL101_NBR_BLK_INTS, null, out objValue);
                    usIntervalsPerBlock = (ushort)objValue;

                    m_CenTables.GetValue(CentronTblEnum.MFGTBL101_VM_INT_LEN, null, out objValue);
                    byIntervalLength = (byte)objValue;
                    VoltageData.IntervalLength = TimeSpan.FromMinutes((double)byIntervalLength);

                    m_CenTables.GetValue(CentronTblEnum.MFGTBL101_NBR_PHASES, null, out objValue);
                    byNumPhases = (byte)objValue;
                    VoltageData.NumberOfPhases = byNumPhases;

                    m_CenTables.GetValue(CentronTblEnum.MFGTBL103_NBR_BLOCKS, null, out objValue);
                    usMaxBlocks = (ushort)objValue;

                    m_CenTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_BLOCKS, null, out objValue);
                    usUsedBlocks = (ushort)objValue;

                    m_CenTables.GetValue(CentronTblEnum.MFGTBL103_NBR_VALID_INT, null, out objValue);
                    usValidIntervals = (ushort)objValue;

                    m_CenTables.GetValue(CentronTblEnum.MFGTBL103_LAST_BLOCK_ELEMENT, null, out objValue);
                    usLastBlock = (ushort)objValue;

                    m_CenTables.GetValue(CentronTblEnum.MFGTBL103_DIVISOR, null, out objValue);
                    usDivisor = (ushort)objValue;

                    if (DeviceType == EDLDeviceTypes.OpenWayCentronBasicPoly
                        || DeviceType == EDLDeviceTypes.OpenWayCentronAdvPoly)
                    {
                        VoltageData.NominalVoltages = new ushort[byNumPhases];

                        for (int iIndex = 0; iIndex < byNumPhases; iIndex++)
                        {
                            m_CenTables.GetValue(CentronTblEnum.MfgTbl103NominalVoltages, new int[]{iIndex}, out objValue);
                            ushort NomVoltage = (ushort)objValue;

                            VoltageData.NominalVoltages[iIndex] = NomVoltage;
                        }
                    }
                    else
                    {
                        ushort NomVoltage = DetermineNominalVoltage();
                        VoltageData.NominalVoltages = new ushort[byNumPhases];

                        for(int iIndex = 0; iIndex < byNumPhases; iIndex++)
                        {
                            VoltageData.NominalVoltages[iIndex] = NomVoltage;
                        }
                    }

                    VoltageData.VhLowPercentage = VMVhLowPercentage;
                    VoltageData.VhHighPercentage = VMVhHighPercentage;
                    VoltageData.RMSVoltageLowPercentage = VMRMSLowPercentage;
                    VoltageData.RMSVoltageHighPercentage = VMRMSHighPercentage;

                    // Determine the starting block (We are always assuming circular lists
                    if (usUsedBlocks == usMaxBlocks)
                    {
                        // The data has wrapped
                        usStartBlock = (ushort)((usLastBlock + 1) % usMaxBlocks);
                    }
                    else
                    {
                        // The data has not wrapped so start at 0
                        usStartBlock = 0;
                    }

                    OnShowProgress(new ShowProgressEventArgs(1, (usUsedBlocks - 1) * usIntervalsPerBlock + usValidIntervals,
                        "", "Retrieving Voltage Monitoring Data..."));

                    // Get the data
                    for (ushort usBlockIndex = 0; usBlockIndex < usUsedBlocks; usBlockIndex++)
                    {
                        DateTime dtBlockEndTime;
                        ushort usActualBlockIndex = (ushort)((usStartBlock + usBlockIndex) % usMaxBlocks);
                        ushort usNumIntervals;
                        int[] IndexArray;

                        IndexArray = new int[] { usActualBlockIndex };

                        m_CenTables.GetValue(CentronTblEnum.MFGTBL104_BLK_END_TIME, IndexArray, out objValue);
                        dtBlockEndTime = (DateTime)objValue;

                        if (usActualBlockIndex != usLastBlock)
                        {
                            // Always usIntervalsPerBlock intervals in these blocks
                            usNumIntervals = usIntervalsPerBlock;
                        }
                        else
                        {
                            usNumIntervals = usValidIntervals;
                        }

                        // Determine whether the last interval is in DST or not so we know to adjust it properly.
                        IndexArray = new int[] { usActualBlockIndex, usNumIntervals - 1};

                        // Get the status - Kevin's code returns these as an array of bytes
                        m_CenTables.GetValue(CentronTblEnum.MFGTBL104_EXTENDED_INT_STATUS, IndexArray, out objValue);
                        byaStatus = (byte[])objValue;

                        LastIntervalStatus = (VMStatusFlags)((byaStatus[1] << 8) | byaStatus [0]);

                        // Get the interval data
                        for (ushort usIntervalIndex = 0; usIntervalIndex < usNumIntervals; usIntervalIndex++)
                        {
                            List<float> fDataList = new List<float>();
                            TimeSpan tsTimeDifference = TimeSpan.FromMinutes((double)((usNumIntervals - usIntervalIndex - 1) * byIntervalLength));
                            DateTime dtIntervalEndTime = dtBlockEndTime - tsTimeDifference;
                            VMStatusFlags IntervalStatus;

                            IndexArray = new int[] { usActualBlockIndex, usIntervalIndex};

                            // Get the status - Kevin's code returns these as an array of bytes
                            m_CenTables.GetValue(CentronTblEnum.MFGTBL104_EXTENDED_INT_STATUS, IndexArray, out objValue);
                            byaStatus = (byte[])objValue;

                            IntervalStatus = (VMStatusFlags)((byaStatus[1] << 8) | byaStatus[0]);

                            // Adjust the time if there is a difference in DST status
                            if ((IntervalStatus & VMStatusFlags.DST) == VMStatusFlags.DST
                                && (LastIntervalStatus & VMStatusFlags.DST) != VMStatusFlags.DST)
                            {
                                // We need to adjust forward an hour since the time we have has been
                                // adjusted backwards for DST
                                dtIntervalEndTime = dtIntervalEndTime.Add(new TimeSpan(1, 0, 0));
                            }
                            else if ((IntervalStatus & VMStatusFlags.DST) != VMStatusFlags.DST
                                && (LastIntervalStatus & VMStatusFlags.DST) == VMStatusFlags.DST)
                            {
                                // We need to adjust back an hour since the time we have has been
                                // adjusted forward for DST
                                dtIntervalEndTime = dtIntervalEndTime.Subtract(new TimeSpan(1, 0, 0));
                            }

                            // Get the values
                            for (byte byPhaseIndex = 0; byPhaseIndex < byNumPhases; byPhaseIndex++)
                            {
                                ushort usValue;

                                IndexArray = new int[] { usActualBlockIndex, usIntervalIndex, byPhaseIndex };

                                m_CenTables.GetValue(CentronTblEnum.MFGTBL104_VH_DATA_ITEM, IndexArray, out objValue);
                                usValue = (ushort)objValue;
                                fDataList.Add((float)usValue / (float)usDivisor);
                            }

                            // The first interval of the DST change is always marked opposite of what we think so
                            // we need to go back and adjust that one if the previous DST status does not match the
                            // current DST status. The first check will prevent adjustments across blocks.
                             if (usIntervalIndex - 1 >= 0 && (IntervalStatus & VMStatusFlags.DST)
                                != (Intervals[Intervals.Count - 1].IntervalStatus & VMStatusFlags.DST))
                            {
                                VMInterval PreviousInterval = Intervals[Intervals.Count - 1];

                                if ((PreviousInterval.IntervalStatus & VMStatusFlags.DST) == VMStatusFlags.DST)
                                {
                                    // Interval was in DST so subtract an hour
                                    Intervals[Intervals.Count - 1] = new VMInterval(PreviousInterval.IntervalStatus, PreviousInterval.VhData,
                                        PreviousInterval.IntervalEndTime.Subtract(new TimeSpan(1, 0, 0)));
                                }
                                else
                                {
                                    // Interval was not in DST so add an hour
                                    Intervals[Intervals.Count - 1] = new VMInterval(PreviousInterval.IntervalStatus, PreviousInterval.VhData,
                                        PreviousInterval.IntervalEndTime.Add(new TimeSpan(1, 0, 0)));
                                }
                            }

                            Intervals.Add(new VMInterval(IntervalStatus, fDataList, dtIntervalEndTime));
                            OnStepProgress(new ProgressEventArgs());
                        }
                    }

                    VoltageData.Intervals = Intervals;
                }
            }

            OnHideProgress(new EventArgs());

            return VoltageData;
        }

        /// <summary>
        /// Returns a LoadProfile object built from the information in the EDL file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#        Description
        // -------- --- ------- ------------- ---------------------------------------
        // 10/30/06 RDB				          Created
        // 04/30/08 RCG 1.50.19 itron00114106 Fixing issues with wrapped Load Profile
        // 06/11/08 jrf 1.50.34               Switched to IndexOf() since Contains() is 
        //                                    not supported in the compact framework.
        protected override void GetLoadProfileData()
        {
            //used to store value returned from the CentronTables' GetValue method
            object objValue;

            //array given to GetValue that represents the block index, interval
            //index, and channel index of the value that is being accessed
            int[] aiBlockIntChannel = { 0, 0, 0 };

            //array given to GetValue that represents the block index of the value
            //that is being accessed
            int[] aiBlock = { 0 };

            DateTime dtIntEnd = new DateTime();
            DateTime dtBlockEnd;
            UInt16 iUsedBlocks;
            UInt16 iUsedIntervals;
            UInt16 iMaxBlocks;
            UInt16 iLastBlock;
            int iStartBlock;
            int iChannels;
            int iIntLength;
            UInt16 iScalar;
            UInt16 iDivisor;
            string strStatus;

            // read max number of blocks
            m_CenTables.GetValue(StdTableEnum.STDTBL61_NBR_BLKS_SET1, null, out objValue);
            iMaxBlocks = (UInt16)objValue;

            //read number of used blocks
            m_CenTables.GetValue(StdTableEnum.STDTBL63_NBR_VALID_BLOCKS, null, out objValue);
            iUsedBlocks = (UInt16)objValue;

            //read index of last block
            m_CenTables.GetValue(StdTableEnum.STDTBL63_LAST_BLOCK_ELEMENT, null, out objValue);
            iLastBlock = (UInt16)objValue;

            // Determine the first block in the table
            if (iUsedBlocks == iMaxBlocks)
            {
                // The circular list has rolled over so calculate the starting block
                iStartBlock = (iLastBlock + 1) % iMaxBlocks; 
            }
            else
            {
                // The list has not rolled over so it is always zero
                iStartBlock = 0;
            }

            //read number of intervals in last block
            m_CenTables.GetValue(StdTableEnum.STDTBL63_NBR_VALID_INT, null, out objValue);
            iUsedIntervals = (UInt16)objValue;

            //read number of channels
            m_CenTables.GetValue(StdTableEnum.STDTBL61_NBR_CHNS_SET1, null, out objValue);
            iChannels = (int)((byte)objValue);

            //read length of one interval in minutes
            m_CenTables.GetValue(StdTableEnum.STDTBL61_MAX_INT_TIME_SET1, null, out objValue);
            iIntLength = (int)((byte)objValue);

            double[] aobjItem = new double[iChannels];
            string[] astrStatus = new string[iChannels];
            //double[] adblPulseMultiplier = new double[iChannels];
            string strIntervalStatus = "";

            //Setup Progress Event
            OnShowProgress(new ShowProgressEventArgs(1, (iUsedBlocks * 128), "", "Retrieving Profile Data..."));

            m_LoadProfile = new LoadProfilePulseData(iIntLength);

            //make channels and add them to the load profile
            for (int i = 0; i < iChannels; i++)
            {
                aiBlock[0] = i;
                astrStatus[i] = "";

                //get scalar and divisor to calculate pulse multiplier
                m_CenTables.GetValue(StdTableEnum.STDTBL62_SCALARS_SET1,aiBlock, out objValue);
                iScalar = (UInt16)objValue;

                m_CenTables.GetValue(StdTableEnum.STDTBL62_DIVISOR_SET1,aiBlock, out objValue);
                iDivisor = (UInt16)objValue;

                //lpChannel.PulseWeight = (float)iScalar / (float)iDivisor;
				float fPulseWeight = (float)iScalar / (float)iDivisor;

				m_LoadProfile.AddChannel(LPQuantityList[i], fPulseWeight, 1.0f);
            }

            //go through each block of memory that is being used
            for (int iRelativeBlock = 0; iRelativeBlock < iUsedBlocks; iRelativeBlock++)
            {
                int iActualBlock = (iStartBlock + iRelativeBlock) % iMaxBlocks;
                int iNumIntervals = 0;
                bool bEndTimeInDST = false;

                //read the end time of the last interval in the current block
                aiBlock[0] = iActualBlock;
                m_CenTables.GetValue(StdTableEnum.STDTBL64_BLK_END_TIME, aiBlock, out objValue);
                dtBlockEnd = (DateTime)objValue;

                //we're not in the last used block so there are 128 intervals in it -
                //subtract 127 * the length of one interval to get the end of the 
                //first interval
                if (iActualBlock != iLastBlock)
                {
                    iNumIntervals = 128;
                }
                //we are in the last block and so there are only iUsedIntervals 
                //intervals in it
                else
                {
                    iNumIntervals = iUsedIntervals;
                }

                // Determine if the end time is in DST
                m_CenTables.GetValue(StdTableEnum.STDTBL64_EXTENDED_INT_STATUS, new int[] { iActualBlock, iNumIntervals - 1, 0 }, out objValue);

                //Can't use Contains() in compact framework
                if (-1 != GetChannelStatus((byte)objValue, 2).IndexOf("D", StringComparison.OrdinalIgnoreCase))
                {
                    bEndTimeInDST = true;
                }

                //for each interval in the block
                for (int interval = 0; interval < iNumIntervals; interval++)
                {
                    dtIntEnd = dtBlockEnd.AddMinutes(-1 * (iNumIntervals - interval - 1) * iIntLength);

                    //specify which block and interval the data should be taken from
                    aiBlockIntChannel[0] = iActualBlock;
                    aiBlockIntChannel[1] = interval;
                    aiBlockIntChannel[2] = 0;

                    //get the interval status
                    m_CenTables.GetValue(StdTableEnum.STDTBL64_EXTENDED_INT_STATUS, aiBlockIntChannel, out objValue);

                    if (objValue != null)
                    {
                        strIntervalStatus =
                            GetChannelStatus((byte)objValue, 2);

                        // Do not do time adjustments unless Meter is using DST
                        if (DSTEnabled)
                        {
                            //Can't use Contains() in compact framework
                            if (-1 != strIntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase) && bEndTimeInDST == false)
                            {
                                // We need to adjust forward an hour since the time we have has been
                                // adjusted backwards for DST
                                dtIntEnd = dtIntEnd.Add(new TimeSpan(1, 0, 0));
                            }
                            else if (-1 == strIntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase) && bEndTimeInDST == true)
                            {
                                // We need to adjust back an hour since the time we have has been
                                // adjusted forward for DST
                                dtIntEnd = dtIntEnd.Subtract(new TimeSpan(1, 0, 0));
                            }
                        }
                    }

                    //get the interval data and status for each channel and put them
                    //in aiItem and aiStatus so that they can be put in the load
                    //profile object
                    for (aiBlockIntChannel[2] = 0;
                         aiBlockIntChannel[2] < iChannels; aiBlockIntChannel[2]++)
                    {
                        //get value for channel
                        m_CenTables.GetValue(StdTableEnum.STDTBL64_ITEM, aiBlockIntChannel, out objValue);
                        if (objValue != null)
                        {
                            aobjItem[aiBlockIntChannel[2]] =
                                Convert.ToDouble(objValue, CultureInfo.InvariantCulture);
                        }

                        int[] aiIndexArray = {aiBlockIntChannel[0],
                                aiBlockIntChannel[1],
                                aiBlockIntChannel[2]};
                        aiIndexArray[2] = ((aiBlockIntChannel[2] + 1) / 2);

                        //get status for channel
                        m_CenTables.GetValue(
                            StdTableEnum.STDTBL64_EXTENDED_INT_STATUS, aiIndexArray, out objValue);

                        if (objValue != null)
                        {
                            strStatus = GetChannelStatus((byte)objValue,
                                aiBlockIntChannel[2] % 2);

                            astrStatus[aiBlockIntChannel[2]] = strStatus;

                            //Can't use Contains() in compact framework
                            if (-1 == strIntervalStatus.IndexOf(strStatus, StringComparison.OrdinalIgnoreCase))  
                            {
                                strIntervalStatus += strStatus;
                            }
                        }    
                    }

                    // Do not do time adjustments unless Meter is using DST
                    if (DSTEnabled)
                    {
                        // The first interval of the DST change is always marked opposite of what we think so
                        // we need to go back and adjust that one if the previous DST status does not match the
                        // current DST status. The first check will prevent adjustments across blocks.
                        //Can't use Contains() in compact framework, so using IndexOf()
                        if (interval - 1 >= 0 && ((-1 != strIntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase))
                            != (-1 != m_LoadProfile.Intervals[m_LoadProfile.Intervals.Count - 1].IntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase))))
                        {
                            LPInterval PreviousInterval = m_LoadProfile.Intervals[m_LoadProfile.Intervals.Count - 1];

                            if (-1 != PreviousInterval.IntervalStatus.IndexOf("D", StringComparison.OrdinalIgnoreCase))
                            {
                                // Interval was in DST so subtract an hour
                                m_LoadProfile.Intervals.Remove(PreviousInterval);
                                m_LoadProfile.AddInterval(PreviousInterval.Data, PreviousInterval.ChannelStatuses, PreviousInterval.IntervalStatus,
                                    PreviousInterval.Time.Subtract(new TimeSpan(1, 0, 0)), PreviousInterval.DisplayScale);
                            }
                            else
                            {
                                // Interval was in DST so subtract an hour
                                m_LoadProfile.Intervals.Remove(PreviousInterval);
                                m_LoadProfile.AddInterval(PreviousInterval.Data, PreviousInterval.ChannelStatuses, PreviousInterval.IntervalStatus,
                                    PreviousInterval.Time.Add(new TimeSpan(1, 0, 0)), PreviousInterval.DisplayScale);
                            }
                        }
                    }

                    OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                    //add the interval to the end of the interval list in the load
                    //profile object
                    m_LoadProfile.AddInterval(aobjItem, astrStatus, strIntervalStatus, dtIntEnd, DisplayScaleOptions.UNITS);

                    aobjItem = new double[iChannels];
                    astrStatus = new string[iChannels];
                }
            }

            OnHideProgress(new EventArgs());

        }//LP


        /// <summary>Reads the current register data from table 23</summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/15/07 mcm 8.10.05  	    Created
        // 07/29/09 RCG 2.20.19 134394 Rewriting to use new standard tables and support coincidents

        protected override void GetCurrentRegisters()
        {
            m_Registers = new List<Quantity>();

            foreach (LID EnergyLID in EnergyConfigLIDs)
            {
                Quantity NewQuantity = null;
                int? EnergySelectionIndex = null;
                double Value;

                if (EnergyLID != null)
                {
                    EnergySelectionIndex = FindEnergySelectionIndex(EnergyLID);
                }

                if (EnergySelectionIndex != null)
                {
                    NewQuantity = new Quantity(EnergyLID.lidDescription);

                    // Add the energy data items
                    Value = Table23.CurrentRegisters.TotalDataBlock.Summations[(int)EnergySelectionIndex];
                    NewQuantity.TotalEnergy = new Measurement(Value, EnergyLID.lidDescription);
                    NewQuantity.TOUEnergy = new List<Measurement>();

                    for (int iRate = 0; iRate < Table21.NumberOfTiers; iRate++)
                    {
                        Value = Table23.CurrentRegisters.TierDataBlocks[iRate].Summations[(int)EnergySelectionIndex];
                        NewQuantity.TOUEnergy.Add(new Measurement(Value, GetTOUEnergyLID(EnergyLID, iRate).lidDescription));
                    }

                    m_Registers.Add(NewQuantity);
                }
            }

            foreach (LID DemandLID in DemandConfigLIDs)
            {
                Quantity DemandQuantity = null;
                int? DemandSelectionIndex = null;
                int? CoincidentSelectionIndex = null;
                double Value;
                DateTime TimeOfOccurance;

                if (DemandLID != null)
                {
                    DemandSelectionIndex = FindDemandSelectionIndex(DemandLID);
                    CoincidentSelectionIndex = FindCoincidentSelectionIndex(DemandLID);
                }

                if (DemandSelectionIndex != null)
                {
                    DemandQuantity = GetDemandQuantity(DemandLID);

                    LID CumDemandLID = GetCumDemandLID(DemandLID);
                    LID CCumDemandLID = GetCCumDemandLID(DemandLID);
                    DemandRecord CurrentDemandRecord = Table23.CurrentRegisters.TotalDataBlock.Demands[(int)DemandSelectionIndex];

                    // Add the demand data items
                    // The quantity object only supports 1 occurence so always use occurence 0
                    Value = CurrentDemandRecord.Demands[0];
                    TimeOfOccurance = CurrentDemandRecord.TimeOfOccurances[0];

                    DemandQuantity.TotalMaxDemand = new DemandMeasurement(Value, DemandLID.lidDescription);
                    DemandQuantity.TotalMaxDemand.TimeOfOccurance = TimeOfOccurance;

                    Value = CurrentDemandRecord.Cum;
                    DemandQuantity.CummulativeDemand = new Measurement(Value, CumDemandLID.lidDescription);

                    Value = CurrentDemandRecord.CCum;
                    DemandQuantity.ContinuousCummulativeDemand = new Measurement(Value, CCumDemandLID.lidDescription);

                    // Add TOU rates
                    if (Table21.NumberOfTiers > 0)
                    {
                        DemandQuantity.TOUMaxDemand = new List<DemandMeasurement>();
                        DemandQuantity.TOUCummulativeDemand = new List<Measurement>();
                        DemandQuantity.TOUCCummulativeDemand = new List<Measurement>();

                        for (int iRate = 0; iRate < Table21.NumberOfTiers; iRate++)
                        {
                            CurrentDemandRecord = Table23.CurrentRegisters.TierDataBlocks[iRate].Demands[(int)DemandSelectionIndex];

                            Value = CurrentDemandRecord.Demands[0];
                            TimeOfOccurance = CurrentDemandRecord.TimeOfOccurances[0];

                            DemandQuantity.TOUMaxDemand.Add(new DemandMeasurement(Value, GetTOUDemandLid(DemandLID, iRate).lidDescription));
                            DemandQuantity.TOUMaxDemand[iRate].TimeOfOccurance = TimeOfOccurance;

                            Value = CurrentDemandRecord.Cum;
                            DemandQuantity.TOUCummulativeDemand.Add(new Measurement(Value, GetTOUDemandLid(CumDemandLID, iRate).lidDescription));

                            Value = CurrentDemandRecord.CCum;
                            DemandQuantity.TOUCCummulativeDemand.Add(new Measurement(Value, GetTOUDemandLid(CCumDemandLID, iRate).lidDescription));
                        }
                    }
                }

                if (CoincidentSelectionIndex != null)
                {
                    byte bySelection = Table22.CoincidentSelection[(int)CoincidentSelectionIndex];
                    byte byDemandSelection = Table22.CoincidentDemandAssocations[(int)CoincidentSelectionIndex];
                    Quantity CoincQuantity = new Quantity(DemandLID.lidDescription);

                    // Add the total values
                    CoincQuantity.TotalMaxDemand = new DemandMeasurement(Table23.CurrentRegisters.TotalDataBlock.Coincidents[(int)CoincidentSelectionIndex].Coincidents[0], DemandLID.lidDescription);
                    CoincQuantity.TotalMaxDemand.TimeOfOccurance = Table23.CurrentRegisters.TotalDataBlock.Demands[byDemandSelection].TimeOfOccurances[0];

                    if (Table21.NumberOfTiers > 0)
                    {
                        CoincQuantity.TOUMaxDemand = new List<DemandMeasurement>();

                        // Add the rate values
                        for (int iRateIndex = 0; iRateIndex < Table21.NumberOfTiers; iRateIndex++)
                        {
                            LID RateLID = GetCoincLIDForRate(DemandLID, iRateIndex);

                            CoincQuantity.TOUMaxDemand.Add(new DemandMeasurement(Table23.CurrentRegisters.TierDataBlocks[iRateIndex].Coincidents[(int)CoincidentSelectionIndex].Coincidents[0], RateLID.lidDescription));
                            CoincQuantity.TOUMaxDemand[iRateIndex].TimeOfOccurance = Table23.CurrentRegisters.TierDataBlocks[iRateIndex].Demands[byDemandSelection].TimeOfOccurances[0];
                        }
                    }

                    m_Registers.Add(CoincQuantity);

                }
            }
        }

       
        
        /// <summary>
        /// Reads the current TOU Calendar data from table 54
        /// </summary>
        /// <remarks>
        /// TODO - Get the GetComment() working and break this into several
        /// functions.
        /// </remarks>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/30/08 AF  10.0           Created for OpenWay
        //
        protected override void GetTOUCalendar()
        {
            object Value;
            string strComment;
            
            bool blnSeparateSumDemandsFlag;
            bool blnAnchorDateFlag;
            bool blnSeparateWeekdaysFlag;

            byte byNbrNonRecurringDates;
            byte byNbrRecurringDates;
            UInt16 usNbrTierSwitches;
            byte byNbrSeasons;
            byte byNbrSpecialSchedules;

            //array given to GetValue that represents the block index of the value
            //that is being accessed
            int[] aiIndex = { 0 };
            byte[] aVal = { 0 };

            int iIndex;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_SEPARATE_SUM_DEMANDS_FLAG, null, out Value);
            blnSeparateSumDemandsFlag = (bool)Value;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_ANCHOR_DATE_FLAG, null, out Value);
            blnAnchorDateFlag = (bool)Value;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_SEPARATE_WEEKDAYS_FLAG, null, out Value);
            blnSeparateWeekdaysFlag = (bool)Value;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_NBR_NON_RECURR_DATES, null, out Value);
            byNbrNonRecurringDates = (byte)Value;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_NBR_RECURR_DATES,null, out Value);
            byNbrRecurringDates = (byte)Value;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_NBR_SEASONS, null, out Value);
            byNbrSeasons = (byte)Value;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_NBR_SPECIAL_SCHED, null, out Value);
            byNbrSpecialSchedules = (byte)Value;

            m_CenTables.GetValue(StdTableEnum.STDTBL51_NBR_TIER_SWITCHES, null, out Value);
            usNbrTierSwitches = (UInt16)Value;

            m_Calendar = new C1219_CalendarRcd(blnSeparateSumDemandsFlag,
                blnAnchorDateFlag, byNbrNonRecurringDates, byNbrRecurringDates,
                usNbrTierSwitches, blnSeparateWeekdaysFlag, byNbrSeasons,
                byNbrSpecialSchedules);

            if (null != m_Calendar)
            {
                if (blnAnchorDateFlag)
                {
                    m_CenTables.GetValue(StdTableEnum.STDTBL54_ANCHOR_DATE, null, out Value);
                    m_Calendar.AnchorDate = (DateTime)Value;
                }

                for (iIndex = 0; iIndex < m_Calendar.NbrNonRecurringDates; iIndex++)
                {
                    aiIndex[0] = iIndex;

                    //TODO - this doesn't work - there are comments in the EDL file that give a
                    //title to the date.  It would be nice to be able to get it.
                    strComment = m_CenTables.GetComment((long)StdTableEnum.STDTBL54_NON_RECURR_DATE, aiIndex);
                    m_Calendar.CalendarRecord.NonRecurringDates[iIndex].Comment = strComment;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_NON_RECURR_DATE, aiIndex, out Value);
                    m_Calendar.CalendarRecord.NonRecurringDates[iIndex].NonRecurrDate = (DateTime)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_NON_RECURR_CALENDAR_CTRL,aiIndex, out Value);
                    m_Calendar.CalendarRecord.NonRecurringDates[iIndex].CalendarControl = (byte)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_NON_RECURR_DEMAND_RESET_FLAG, aiIndex, out Value);
                    m_Calendar.CalendarRecord.NonRecurringDates[iIndex].DemandResetFlag = (bool)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_NON_RECURR_SELF_READ_FLAG, aiIndex, out Value);
                    m_Calendar.CalendarRecord.NonRecurringDates[iIndex].SelfReadFlag = (bool)Value;
                }

                for (iIndex = 0; iIndex < m_Calendar.NbrRecurringDates; iIndex++)
                {
                    aiIndex[0] = iIndex;
                    m_CenTables.GetValue(StdTableEnum.STDTBL54_RECURR_DATE, aiIndex, out Value);
                    m_Calendar.CalendarRecord.RecurringDates[iIndex].RecurrDateBfld = (UInt16)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_RECURR_CALENDAR_CTRL, aiIndex, out Value);
                    m_Calendar.CalendarRecord.RecurringDates[iIndex].CalendarControl = (byte)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_RECURR_DEMAND_RESET_FLAG, aiIndex, out Value);
                    m_Calendar.CalendarRecord.RecurringDates[iIndex].DemandResetFlag = (bool)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_RECURR_SELF_READ_FLAG, aiIndex, out Value);
                    m_Calendar.CalendarRecord.RecurringDates[iIndex].SelfReadFlag = (bool)Value;
                }

                for (iIndex = 0; iIndex < m_Calendar.NbrTierSwitches; iIndex++)
                {
                    aiIndex[0] = iIndex;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_NEW_TIER, aiIndex, out Value);
                    m_Calendar.CalendarRecord.TierSwitches[iIndex].NewTier = (byte)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_TIER_SWITCH_MIN, aiIndex, out Value);
                    m_Calendar.CalendarRecord.TierSwitches[iIndex].SwitchMinute = (byte)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_TIER_SWITCH_HOUR,aiIndex, out Value);
                    m_Calendar.CalendarRecord.TierSwitches[iIndex].SwitchHour = (byte)Value;

                    m_CenTables.GetValue(StdTableEnum.STDTBL54_DAY_SCH_NUM,aiIndex, out Value);
                    m_Calendar.CalendarRecord.TierSwitches[iIndex].DaySchedNum = (byte)Value;
                }

                if (blnSeparateWeekdaysFlag)
                {
                    //array size: # of seasons X (7 day types + number of special schedules)
                    for (iIndex = 0; iIndex < m_Calendar.NbrSeasons; iIndex++)
                    {
                        aiIndex[0] = iIndex;
                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_SUNDAY_SCHEDULE, aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 0] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_MONDAY_SCHEDULE, aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 1] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_TUESDAY_SCHEDULE, aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 2] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_WEDNESDAY_SCHEDULE, aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 3] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_THURSDAY_SCHEDULE, aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 4] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_FRIDAY_SCHEDULE, aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 5] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_SATURDAY_SCHEDULE, aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 6] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_DAILY_SPECIAL_SCHEDULE,aiIndex, out Value);
                        aVal = (byte[])Value;
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 7] = aVal[0];
                    }
                }
                else
                {
                    //array size: # of seasons X (3 day types + number of special schedules)
                    for (iIndex = 0; iIndex < m_Calendar.NbrSeasons; iIndex++)
                    {
                        aiIndex[0] = iIndex;
                        //strComment = m_CenTables.GetComment((long)StdTableEnum.STDTBL54_DAILY_SCHEDULE_ID_MATRIX, aiIndex);

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_WEEKDAY_SATURDAY_SCHEDULE,aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 0] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_WEEKDAY_SUNDAY_SCHEDULE,aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 1] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_WEEKDAY_WEEKDAY_SCHEDULE,aiIndex, out Value);
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 2] = (byte)Value;

                        m_CenTables.GetValue(StdTableEnum.STDTBL54_WEEKDAY_SPECIAL_SCHEDULE,aiIndex, out Value);
                        aVal = (byte[])Value;
                        m_Calendar.CalendarRecord.DailyScheduleIDMatrix[iIndex, 3] = aVal[0];
                    }
                }
            }
        }

        /// <summary>
        /// This method reads standard table 3 and retrieves the fatal and non-fatal
        /// errors occurring in the device from it.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/09 jrf 2.10.02 125997 Created
        //
        protected override void GetErrorList()
        {
            m_astrErrorList = null;
            IList<TableData> lsttblData3 = null;
            PSEMBinaryReader Tbl3BinaryReader = null;
            CTable03 tbl3 = null;

            // Make sure that the EDL file contains the table
            if (m_CenTables.IsTableKnown(3) && m_CenTables.IsAllCached(3))
            {
                lsttblData3 = m_CenTables.BuildPSEMStreams(3);
                if (null != lsttblData3)
                {
                    lsttblData3[0].PSEM.Position = 0;
                    Tbl3BinaryReader = new PSEMBinaryReader(lsttblData3[0].PSEM);
                }

                if (null != Tbl3BinaryReader)
                {
                    tbl3 = new CTable03(Tbl3BinaryReader);
                }

                if (null != tbl3)
                {
                    m_astrErrorList = tbl3.ErrorsList;
                }
            }
        }

        /// <summary>
        /// Returns the configuration value for the schedule's day to daytipe
        /// assignments.
        /// </summary>
        /// <param name="TOUSchedule">A TOU server instance with a TOU schedule
        /// open</param>
        /// <returns>A TOU server's typical week packed into a 2 byte value to
        /// be used in the Sentinel or Image configuration</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/22/06 mcm 7.30.00 N/A	Created
        protected ushort GetTypicalWeek(CTOUSchedule TOUSchedule)
        {
            ushort usTypicalWeek = 0;
            ushort usDTIndex;


            // Day to Day Type Assignments: 
            // 2 bits for each day (Sun – Sat & Holiday)
            usTypicalWeek = GetDaytypeIndex(TOUSchedule, eTypicalDay.SUNDAY);
            usDTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.MONDAY);
            usTypicalWeek = (ushort)(usTypicalWeek + usDTIndex * 0x0004);
            usDTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.TUESDAY);
            usTypicalWeek = (ushort)(usTypicalWeek + usDTIndex * 0x0010);
            usDTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.WEDNESDAY);
            usTypicalWeek = (ushort)(usTypicalWeek + usDTIndex * 0x0040);
            usDTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.THURSDAY);
            usTypicalWeek = (ushort)(usTypicalWeek + usDTIndex * 0x0100);
            usDTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.FRIDAY);
            usTypicalWeek = (ushort)(usTypicalWeek + usDTIndex * 0x0400);
            usDTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.SATURDAY);
            usTypicalWeek = (ushort)(usTypicalWeek + usDTIndex * 0x1000);

            // Holiday type always gets the holiday index
            usTypicalWeek = (ushort)(usTypicalWeek +
                TOUConfig.HOLIDAY_TYPE_INDEX * 0x4000);

            return usTypicalWeek;

        } // GetTypicalWeek

        /// <summary>
        /// Returns the schedule's Daytype Type and index into a index (1..4).
        /// Normal Daytype 1 = 1, Normal Daytype 3 = 3, Holiday type 1 = 4.
        /// </summary>
        /// <param name="TOUSchedule">A TOU server instance with a TOU schedule
        /// open</param>
        /// <param name="Day">Day of the week to translate</param>
        /// <returns>Daytype index used by GetTypicalWeek</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/22/06 mcm 7.30.00 N/A	Created
        protected ushort GetDaytypeIndex(CTOUSchedule TOUSchedule,
                                        eTypicalDay Day)
        {
            CDayType DayType;
            ushort DaytypeIndex = 0;


            // The Image and Sentinel meters store the day of the week to 
            // daytype associations for the season as a ushort
            DayType = TOUSchedule.GetDayType(TOUSchedule.TypicalWeek[(int)Day]);
            if (eDayType.NORMAL == DayType.Type)
            {
                DaytypeIndex = (ushort)DayType.Index;
            }
            else
            {
                DaytypeIndex = TOUConfig.HOLIDAY_TYPE_INDEX;
            }

            return DaytypeIndex;

        } // GetDaytypeIndex

        /// <summary>
        /// Builds a TOUConfig time of day event from a TOU schedule's 
        /// switchpoint.  See the TIME_MAN_DESIGN.doc, WindChill document
        /// #D0209255 for more info.
        /// </summary>
        /// <param name="SP">TOU server switchpoint to translate</param>
        /// <returns>Time of Day event</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/22/06 mcm 7.30.00 N/A	Created
        protected TOUConfig.DayEvent GetDayEvent(CSwitchPoint SP)
        {
            TOUConfig.DayEvent.TOUEvent EventType;
            byte Hour;
            byte Minutes;


            if (eSwitchPointType.RATE == SP.SwitchPointType)
            {
                // Translate the rate index into a TOUEvent value
                EventType = (TOUConfig.DayEvent.TOUEvent)(SP.RateOutputIndex +
                    TOUConfig.DayEvent.TOUEvent.RateA);
            }
            else
            {
                // Translate the rate index into a TOUEvent value
                EventType = (TOUConfig.DayEvent.TOUEvent)(SP.RateOutputIndex +
                    TOUConfig.DayEvent.TOUEvent.Output1);
            }

            // Translate minutes since midnight to 0-based hours and minutes
            Hour = (byte)(SP.StartTime / 60);
            Minutes = (byte)(SP.StartTime % 60);

            return new TOUConfig.DayEvent(EventType, Hour, Minutes);

        } // GetDayEvent

        /// <summary>
        /// Builds a TOUConfig time of day event from a TOU schedule's 
        /// OUTPUT switchpoint.  Calling this method with a rate switchpoint 
        /// will cause an exception to be thrown. 
        /// See the TIME_MAN_DESIGN.doc, WindChill document
        /// #D0209255 for more info.
        /// </summary>
        /// <param name="SP">TOU server output switchpoint to translate</param>
        /// <returns>Time of Day event</returns>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 10/23/06 mcm 7.35.07 105	Support for adding output off switchpoints
        /// 
        protected TOUConfig.DayEvent GetOutputOffEvent(CSwitchPoint SP)
        {
            TOUConfig.DayEvent.TOUEvent EventType;
            byte Hour;
            byte Minutes;


            if (eSwitchPointType.OUTPUT == SP.SwitchPointType)
            {
                // Translate the rate index into a TOUEvent value
                EventType = (TOUConfig.DayEvent.TOUEvent)(SP.RateOutputIndex +
                    TOUConfig.DayEvent.TOUEvent.Output1Off);
            }
            else
            {
                // This method only handles outputs! There are no rate off events.
                throw (new ApplicationException("Invalid Switchpoint Type"));
            }

            // Translate minutes since midnight to 0-based hours and minutes
            Hour = (byte)(SP.StopTime / 60);
            Minutes = (byte)(SP.StopTime % 60);

            return new TOUConfig.DayEvent(EventType, Hour, Minutes);

        } // GetOutputOffEvent
 

        /// <summary>
        /// Gets the History Log Configuration table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/09 RCG 2.20.03    N/A    Created
        //  06/04/10 AF  2.41.06        Config data is not in table 2048 for the M2 Gateway
        //  10/14/10 jrf 2.45.04 N/A    Added ability to retrieve CENTRON II history config data.

        protected override CENTRON_AMI_HistoryLogConfig HistoryConfig
        {
            get
            {
                if (m_HistoryConfig == null)
                {
                    Stream strmHistory = new MemoryStream();
                    m_CenTables.BuildPSEMStream(2048, strmHistory, HistoryConfigOffset, CENTRON_AMI_HistoryLogConfig.EVENT_CONFIG_SIZE);
                    PSEMBinaryReader EDLReader = new PSEMBinaryReader(strmHistory);

                    if (DeviceType == EDLDeviceTypes.OpenWayCentronAdvPoly || DeviceType == EDLDeviceTypes.OpenWayCentronBasicPoly)
                    {
                        m_HistoryConfig = new OpenWayBasicPoly_HistoryLogConfig(EDLReader);
                    }
                    else if (EDLDeviceTypes.CentronII == DeviceType)
                    {
                        m_HistoryConfig = new CENTRON2_MONO_HistoryLogConfig(EDLReader);
                    }
                    else
                    {
                        m_HistoryConfig = new CENTRON_AMI_HistoryLogConfig(EDLReader);
                    }

                }
                
                return m_HistoryConfig;
            }
        }

        /// <summary>
        /// Gets the table 2091 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created

        protected override OpenWayPolyMFGTable2091 Table2091
        {
            get
            {
                if (m_Table2091 == null && m_CenTables.IsAllCached(2091))
                {
                    Stream Table2091Stream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(2091, Table2091Stream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(Table2091Stream);

                    m_Table2091 = new OpenWayPolyMFGTable2091(Reader);
                }

                return m_Table2091;
            }
        }

        /// <summary>
        /// Gets the table 11 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected override StdTable11 Table11
        {
            get
            {
                if (m_Table11 == null && m_CenTables.IsAllCached(11))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(11, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table11 = new StdTable11(Reader);
                }

                return m_Table11;
            }
        }

        /// <summary>
        /// Gets the table 14 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected override StdTable14 Table14
        {
            get
            {
                if (m_Table14 == null && m_CenTables.IsAllCached(14))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(14, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table14 = new StdTable14(Reader, Table11);
                }

                return m_Table14;
            }
        }

        /// <summary>
        /// Gets the table 21 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected override StdTable21 Table21
        {
            get
            {
                if (m_Table21 == null && m_CenTables.IsAllCached(21))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(21, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table21 = new StdTable21(Reader);
                }

                return m_Table21;
            }
        }

        /// <summary>
        /// Gets the table 22 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected override StdTable22 Table22
        {
            get
            {
                if (m_Table22 == null && Table21 != null)
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(22, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table22 = new StdTable22(Reader, Table21);
                }

                return m_Table22;
            }
        }

        /// <summary>
        /// Gets the table 23 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected override StdTable23 Table23
        {
            get
            {
                if (m_Table23 == null && m_CenTables.IsAllCached(23))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(23, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table23 = new StdTable23(Reader, Table0, Table21);
                }

                return m_Table23;
            }
        }

        /// <summary>
        /// Gets the table 24 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected override StdTable24 Table24
        {
            get
            {
                if (m_Table24 == null && m_CenTables.IsAllCached(24))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(24, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table24 = new StdTable24(Reader, Table0, Table21);
                }

                return m_Table24;
            }
        }

        /// <summary>
        /// Gets the table 25 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected override StdTable25 Table25
        {
            get
            {
                if (m_Table25 == null && m_CenTables.IsAllCached(25))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(25, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table25 = new StdTable25(Reader, Table0, Table21);
                }

                return m_Table25;
            }
        }

        /// <summary>
        /// Gets the table 26 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected override StdTable26 Table26
        {
            get
            {
                if (m_Table26 == null && m_CenTables.IsAllCached(26))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(26, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table26 = new StdTable26(Reader, Table0, Table21);
                }

                return m_Table26;
            }
        }

        /// <summary>
        /// Gets the table 27 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected override StdTable27 Table27
        {
            get
            {
                if (m_Table27 == null && m_CenTables.IsAllCached(27))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(27, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table27 = new StdTable27(Reader, Table21);
                }

                return m_Table27;
            }
        }

        /// <summary>
        /// Gets the table 28 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created

        protected override StdTable28 Table28
        {
            get
            {
                if (m_Table28 == null && m_CenTables.IsAllCached(28))
                {
                    Stream TableStream = new MemoryStream();
                    m_CenTables.BuildPSEMStream(28, TableStream);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_Table28 = new StdTable28(Reader, Table0, Table21);
                }

                return m_Table28;
            }
        }

        /// <summary>
        /// Gets the Table2078 object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/29/10 RCG 2.40.30        Created

        protected override OpenWayMfgTable2078 Table2078
        {
            get
            {
                if (m_Table2078 == null && m_CenTables.IsAllCached(2078))
                {
                    Stream strmRFLANNeighbors = new MemoryStream();
                    PSEMBinaryReader EDLReader = new PSEMBinaryReader(strmRFLANNeighbors);

                    if (IsHighDataRate)
                    {
                        m_CenTables.BuildPSEMStream(2078, strmRFLANNeighbors, 0, OpenWayMfgTable2078HDR.HDR_TABLE_LENGTH);
                        m_Table2078 = new OpenWayMfgTable2078HDR(EDLReader);
                    }
                    else
                    {
                        m_CenTables.BuildPSEMStream(2078, strmRFLANNeighbors, 0, OpenWayMfgTable2078.ACT_RFLAN_NEIGHBOR_LIST_TBL_LENGTH);
                        m_Table2078 = new OpenWayMfgTable2078(EDLReader);
                    }
                }

                return m_Table2078;
            }
        }

        /// <summary>
        /// Return true if the EDL file contains History entries. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/11/07 RCG 1.00.00        Created
        //  07/30/10 AF  2.42.09        Added support for the M2 Gateway meter
        //
        public override bool ContainsHistoryEntries
        {
            get
            {
                bool bContainsHistoryEntries = false;
                ushort usNbrValidEntries = 0;
                object objValue = null;

                if (m_CenTables.IsCached((long)StdTableEnum.STDTBL74_NBR_VALID_ENTRIES, null))
                {
                    m_CenTables.GetValue(StdTableEnum.STDTBL74_NBR_VALID_ENTRIES, null, out objValue);
                    usNbrValidEntries = (ushort)objValue;
                }

                if (usNbrValidEntries > 0)
                {
                    bContainsHistoryEntries = true;
                }

                return bContainsHistoryEntries;
            }
        }

        /// <summary>
        /// Return true if the EDL file contains Network Statistics data. Returns false otherwise.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/20/08 KRC 1.50.26        Created
        //  06/03/08 KRC 1.50.31        Check for actual log table, not config table.
        //  08/02/10 AF  2.42.11        Added support for the M2 Gateway
        //
        public override bool ContainsCommLogs
        {
            get
            {
                bool bResult = false;

                //Check to make sure that one of the log tables is in the file (if not then the other will not be)
                if (m_CenTables.IsTableKnown(2162) && m_CenTables.IsAllCached(2162))
                {
                    bResult = true;
                }
                return bResult;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the table 0 object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/09 RCG 2.20.19        Created
        //  08/02/10 AF  2.42.19        Added M2 Gateway support

        protected override CTable00 Table0
        {
            get
            {
                if (m_Table00 == null)
                {
                    Stream TableStream = new MemoryStream();
                    if (m_CenTables.IsAllCached(0))
                    {
                        m_CenTables.BuildPSEMStream(0, TableStream);
                    }

                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);
                    m_Table00 = new CTable00(Reader, (uint)TableStream.Length);
                }
                return m_Table00;
            }
        }

        /// <summary>
        /// Gets the Table 71 object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/10 RCG 2.40.31 151959 Created
        // 08/02/10 AF  2.42.19        Added M2 Gateway support 

        protected override StdTable71 Table71
        {
            get
            {
                if (m_Table71 == null)
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    if (m_CenTables.IsAllCached(71))
                    {
                        m_CenTables.BuildPSEMStream(71, TableStream);
                    }
                    m_Table71 = new StdTable71(Reader, Table0.StdVersion);
                }

                return m_Table71;
            }
        }

        /// <summary>
        /// Gets the standard table 72 object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/10 AF  2.41.06        Created
        //
        protected override StdTable72 Table72
        {
            get
            {
                if (m_Table72 == null)
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    if (m_CenTables.IsAllCached(72))
                    {
                        m_CenTables.BuildPSEMStream(72, TableStream);
                    }
                    m_Table72 = new StdTable72(Reader, Table71);
                }

                return m_Table72;
            }
        }

        /// <summary>
        /// Gets the standard table 73 object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/10 AF  2.41.06        Created
        //  08/02/10 AF  2.42.11        Added support for M2 Gateway
        //
        protected override  StdTable73 Table73
        {
            get
            {
                if (m_Table73 == null && m_CenTables.IsAllCached(73))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(73, TableStream);

                    m_Table73 = new StdTable73(Reader, Table72, Table71, Table0);
                }
                return m_Table73;
            }
        }

        /// <summary>
        /// Gets the Table 74 object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/30/10 RCG 2.40.31 151959 Created
        // 08/02/10 AF  2.42.11        Added support for M2 Gateway
        // 08/10/10 AF  2.42.17        Added M2 Gateway event dictionary
        //
        protected override StdTable74 Table74
        {
            get
            {
                if (m_Table74 == null && m_CenTables.IsAllCached(74))
                {
                    MemoryStream TableStream = new MemoryStream();
                    PSEMBinaryReader Reader = new PSEMBinaryReader(TableStream);

                    m_CenTables.BuildPSEMStream(74, TableStream);

                    m_Table74 = new StdTable74(Reader, Table71, m_EventDictionary, Table0.TimeFormat);
                }
                return m_Table74;
            }
        }

        #endregion

        private new Itron.Common.C1219Tables.CentronII.CentronTables m_CenTables;

    }//CentronIIEDLFile
}//Itron.Metering.Datafiles
