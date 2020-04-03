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
//                              Copyright © 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Itron.Common.C1219Tables.ANSIStandardII;
using Itron.Common.C1219Tables.CentronII;
using Itron.Metering.AMIConfiguration;
using Itron.Metering.Progressable;
using Itron.Metering.Communications.PSEM;
using Itron.Metering;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.TOU;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    public partial class CENTRON2_MONO
    {
        // Note: The following constants are for TOU and DST reconfiguration.
        // Note: Due to circular dependency issue, those constants are here temporarily, later we need to do reflection.

        /// <summary>
        /// The season entry index.
        /// </summary>
        protected const int MAX_DST_YEARS = 25;
        /// <summary>
        /// The season entry index.
        /// </summary>
        protected const int SEASON_INDEX = 0;
        /// <summary>
        /// The daytype entry index.
        /// </summary>
        protected const int DAY_TYPE_INDEX = 1;
        /// <summary>
        /// The day type event entry index.
        /// </summary>
        protected const int DAY_TYPE_EVENT_INDEX = 2;
        
        /// <summary>
        /// The register key for CENTRONII Replica
        /// </summary>
        private const string CentronII_REPLICA = "Replica";
        /// <summary>
        /// The meter program path for CENTRONII Replica
        /// </summary>
        private const string CentronII_PROGRAM_FOLDER = @"Programs\CentronII\";
        /// <summary>
        /// The defaul EDL file name
        /// </summary>
        private const string DEFAULT_PROGRAM = "Default.xml";

        #region Public Events

        // These events are overridden so that the event handlers can be passed to
        // the configuration object, rather than handling the event here and then
        // raising it again. This also requires the OnShowProgress, OnStepProgress
        // and OnHideProgress methods to be overriden, otherwise the base class events
        // will be raised.

        /// <summary>
        /// Event that shows the progress bar
        /// </summary>
        public override event ShowProgressEventHandler ShowProgressEvent;
        /// <summary>
        /// Event that hides the progress bar
        /// </summary>
        public override event HideProgressEventHandler HideProgressEvent;
        /// <summary>
        /// Event that causes the progress bar to perform a step
        /// </summary>
        public override event StepProgressEventHandler StepProgressEvent;
        
        #endregion
        
        #region Public Methods

        /// <summary>
        /// Configures the meter with the specified program
        /// </summary>
        /// <param name="sProgramName">The path to the program file</param>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/08/10 AF  2.41.07		Created
#if (!WindowsCE)
        public override ConfigurationResult Configure(string sProgramName)
#else
		public ConfigurationResult Configure(string sProgramName)
#endif
        {
            AMIConfigureCentronII ConfigureDevice = new AMIConfigureCentronII(m_PSEM);
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;

            // Set up the progress bar event handlers
            ConfigureDevice.ShowProgressEvent += this.ShowProgressEvent;
            ConfigureDevice.StepProgressEvent += this.StepProgressEvent;
            ConfigureDevice.HideProgressEvent += this.HideProgressEvent;

            ConfigureDevice.IsCanadian = IsCanadian;

            // We always need to set the Prompt for data so we should just use what is
            // currently in the meter.

            ConfigureDevice.UnitID = UnitID;
            ConfigureDevice.CustomerSerialNumber = SerialNumber;
            ConfigureDevice.InitialDateTime = null;

            ConfigError = ConfigureDevice.Configure(sProgramName);

            // Translate to the ItronDevice ConfigurationResult error code since 
            // the factory is using ConfigurationError and we do not want to always
            // rely on having the version in AMIConfiguration.dll

            return TranslateConfigError(ConfigError);
        }

        /// <summary>
        /// Configures the meter with the specified program
        /// </summary>
        /// <param name="sProgramName">The path to the program file</param>
        /// <param name="PFData">Prompt For data Object</param>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/08/10 AF  2.41.07		Created
#if (!WindowsCE)
        public override ConfigurationResult Configure(string sProgramName, PromptForData PFData)
#else
		public ConfigurationResult Configure(string sProgramName, PromptForData PFData)
#endif
        {
            AMIConfigureCentronII ConfigureDevice = new AMIConfigureCentronII(m_PSEM);
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;

            // Set up the progress bar event handlers
            ConfigureDevice.ShowProgressEvent += this.ShowProgressEvent;
            ConfigureDevice.StepProgressEvent += this.StepProgressEvent;
            ConfigureDevice.HideProgressEvent += this.HideProgressEvent;

            ConfigureDevice.IsCanadian = IsCanadian;

            ConfigureDevice.UnitID = PFData.UnitID;
            ConfigureDevice.CustomerSerialNumber = PFData.SerialNumber;
            ConfigureDevice.InitialDateTime = PFData.InitialDateTime;

            ConfigError = ConfigureDevice.Configure(sProgramName);

            // Translate to the ItronDevice ConfigurationResult error code since 
            // the factory is using ConfigurationError and we do not want to always
            // rely on having the version in AMIConfiguration.dll

            return TranslateConfigError(ConfigError);
        }

        /// <summary>
        /// Reconfigures TOU in the connected meter
        /// </summary>
        /// <param name="TOUFileName">The filename including path for the TOU</param>
        /// <param name="DSTFileName">The filename including path for the DST 
        /// file.  If this parameter is emtpy then client only wants to 
        /// reconfigure TOU, not DST. The DSTFileName MUST be included if the 
        /// meter is configured for DST. If the meter is not configured for DST
        /// and this filename is given, the operation will succeed, but it will
        /// return a conditional success code.</param>
        /// <returns>A TOUReconfigResult</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        // 	12/04/10 SCW 9.70.14        Created 

        public override TOUReconfigResult ReconfigureTOU(string TOUFileName, string DSTFileName)
        {
            FileStream EDLFileStream;
            XmlTextReader EDLXMLReader;
            string defaultEDLFileName;

            try
            {
                if (m_TOUTables == null)
                {
                    m_TOUTables = new CentronTables();
                }

                defaultEDLFileName = 
                    CRegistryHelper.GetFilePath(CentronII_REPLICA) + CentronII_PROGRAM_FOLDER + DEFAULT_PROGRAM;
                if (File.Exists(defaultEDLFileName))
                {
                    EDLFileStream = new FileStream(defaultEDLFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    EDLXMLReader = new XmlTextReader(EDLFileStream);

                    // Load the Defaul EDL File as the file stream to process TOU and DST data 
                    m_TOUTables.LoadEDLFile(EDLXMLReader);
                }
                
                if (TOUFileName != null)
                {
                    ClearTOU(); // Key step to clear TOU first
                    ImportTOU(TOUFileName);
                    TOUDataSet = m_TOUTables.BuildPSEMStreams(2090);
                }

                
            }
            catch (Exception)
            {
                throw new ArgumentException("Failure in importing EDL or DST files !!");
            }
            CTable2048Header m_2048Header = new CTable2048Header(m_PSEM);
            AMIConfigureCentronII configureDevice = new AMIConfigureCentronII(m_PSEM);
            TOUReconfigResult result = configureDevice.ReconfigureTOU(TOUDataSet, m_2048Header.CalendarOffset, m_2048Header.TOUOffset) ? 
                                                           TOUReconfigResult.SUCCESS : TOUReconfigResult.ERROR;
             
            return result;
        }

        /// <summary>
        /// Updates DST in the connected meter. This method does not reconfigure
        /// DST. Only future dates in 2007 and beyond are updated.
        /// </summary>
        /// <param name="DSTFileName">The filename including path for the DST file</param>
        /// <returns>A DSTUpdateResult</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/16/06 mcm 7.30.00 N/A    Created
        //	01/24/07 mrj 8.00.08		Flushed status flags when updating dst
        // 	
        public override DSTUpdateResult UpdateDST(string DSTFileName)
        {
            FileStream EDLFileStream;
            XmlTextReader EDLXMLReader;
            string defaultEDLFileName;

            if (m_TOUTables == null)
            {
                m_TOUTables = new CentronTables();
            }

            defaultEDLFileName =
                   CRegistryHelper.GetFilePath(CentronII_REPLICA) + CentronII_PROGRAM_FOLDER + DEFAULT_PROGRAM;
            if (File.Exists(defaultEDLFileName))
            {
                EDLFileStream = new FileStream(defaultEDLFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                EDLXMLReader = new XmlTextReader(EDLFileStream);

                // Load the Defaul EDL File as the file stream to process TOU and DST data 
                m_TOUTables.LoadEDLFile(EDLXMLReader);
            }

            if (DSTFileName != null)
            {
                ClearDST(); // Key step to clear DST first
                ImportDST();
                DSTDataSet = m_TOUTables.BuildPSEMStreams(2260);
            }
            
            AMIConfigureCentronII configureDevice = new AMIConfigureCentronII(m_PSEM); 
            DSTUpdateResult result = configureDevice.ReconfigureDST(DSTDataSet) ? DSTUpdateResult.SUCCESS : DSTUpdateResult.ERROR;
            return result;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raises the event to show the progress bar.
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/07/06 RCG 7.35.00    N/A Created

        protected override void OnShowProgress(ShowProgressEventArgs e)
        {
            if (ShowProgressEvent != null)
            {
                ShowProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the event that causes the progress bar to perform a step
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/07/06 RCG 7.35.00    N/A Created

        protected override void OnStepProgress(ProgressEventArgs e)
        {
            if (StepProgressEvent != null)
            {
                StepProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the event that hides or closes the progress bar
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/07/06 RCG 7.35.00    N/A Created

        protected override void OnHideProgress(EventArgs e)
        {
            if (HideProgressEvent != null)
            {
                HideProgressEvent(this, e);
            }
        }

        #endregion

        #region Private Methods

        // Note: The following source codes are for TOU and DST reconfiguration.
        // Note: Due to circular dependency issue, those codes are here temporarily, later we need to do reflection.

        /// <summary>
        /// This method imports the TOU configuration into the file.
        /// </summary>
        /// <param name="strTOUFileName">The TOU schedule file name.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/10 jrf 2.45.13  	   Created
        // 12/01/10 deo 9.70.12        fix for CQ165091
        private void ImportTOU(string strTOUFileName)
        {
            CTOUSchedule TOUSchedule = new CTOUSchedule(strTOUFileName);
            int[] aiIndex1 = { 0 };
            int[] aiIndex2 = { 0, 0 };
            int[] aiIndex3 = { 0, 0, 0 };

            //TODO: Update CentronTblEnum values based on updates Steve makes...
            //Calendar ID
            m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_ID, null, TOUSchedule.TOUID);
            
            //DemandReset/Season Change Options
            int SeasonChgOption = 1; //  Demand reset at the season change.
            m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_DEMAND_RESET, null, (byte)SeasonChgOption);

            //DST Hour
            m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_DST_HOUR, null, 0);
            //DST Minute
            m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_DST_MINUTE, null, 0);
            //DST Offset
            m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_DST_OFFSET, null, 0);

            //Calendar Years
            for (int iYear = 0; iYear < TOUSchedule.Years.Count; iYear++)
            {
                CYear Year = TOUSchedule.Years[iYear];
                aiIndex1[0] = iYear;
                aiIndex2[0] = iYear;

                //Year
                m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_YEAR, aiIndex1, Year.Year - 2000);

                //Day Events
                for (int iEvent = 0; iEvent < TOUSchedule.Years[iYear].Events.Count; iEvent++)
                {
                    CEvent Event = Year.Events[iEvent];
                    aiIndex2[1] = iEvent;
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
                    m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_EVENT, aiIndex2, bytCalEvent);
                    //Month
                    m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_MONTH, aiIndex2, Event.Date.Month - 1);
                    //Day Of Month
                    m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_DAY_OF_MONTH, aiIndex2, Event.Date.Day - 1);
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
                m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_PROGRAMMED, aiIndex1, 1);

                //Day to Day Types
                m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_DAY_TO_DAY_TYPE, aiIndex1, usDayToDayType);

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

                        m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_RATE_EVENT, aiIndex3, DayEvt.EventType);
                        m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_HOUR, aiIndex3, DayEvt.Hour);
                        m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_MINUTE, aiIndex3, DayEvt.Minute);

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

                        m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_RATE_EVENT, aiIndex3, DayEvt.EventType);
                        m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_HOUR, aiIndex3, DayEvt.Hour);
                        m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_MINUTE, aiIndex3, DayEvt.Minute);
                    }
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
        private void ImportDST()
        {
            DST.CDSTSchedule DSTSchedule = new Itron.Metering.DST.CDSTSchedule();
            int iStartYear = DateTime.Now.Year;
            int[] anIndex1 = { 0 };
            int[] anIndex2 = { 0, 0 };

            //DST Hour
            m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_DST_HOUR, null, DSTSchedule.NextDSTToDate.Hour);

            //DST Minute
            m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_DST_MINUTE, null, DSTSchedule.NextDSTToDate.Minute);

            //DST Offset
            m_TOUTables.SetValue(CentronTblEnum.MFGTB212_DST_OFFSET, null, DSTSchedule.JumpLength);

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
                    m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_DST_CALENDAR_YEAR, anIndex1, DSTDatePair.FromDate.Year - 2000);

                    //DST On Event
                    anIndex2[1] = 0;
                    m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_EVENT, anIndex2, 1);
                    m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_MONTH, anIndex2, DSTDatePair.ToDate.Month - 1);
                    m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_DAY_OF_MONTH, anIndex2, DSTDatePair.ToDate.Day - 1);

                    //DST Off Event
                    anIndex2[1] = 1;
                    m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_EVENT, anIndex2, 2);
                    m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_MONTH, anIndex2, DSTDatePair.FromDate.Month - 1);
                    m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_DAY_OF_MONTH, anIndex2, DSTDatePair.FromDate.Day - 1);
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
        private void ClearTOU()
        {
            int[] aiIndex1 = { 0 };
            int[] aiIndex2 = { 0, 0 };
            int[] aiIndex3 = { 0, 0, 0 };

            //Calendar ID
            m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_ID, null, 0);

            //DemandReset/Season Change Options
            m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_DEMAND_RESET, null, 0);

            //DST Hour
            m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_DST_HOUR, null, 0);
            //DST Minute
            m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_DST_MINUTE, null, 0);
            //DST Offset
            m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_DST_OFFSET, null, 0);

            int iCurrentYear = DateTime.Now.Year - 2000;

            //Calendar Years
            for (int iYear = 0; iYear < 25; iYear++)
            {
                aiIndex1[0] = iYear;
                aiIndex2[0] = iYear;

                //Year
                m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_YEAR, aiIndex1, iCurrentYear + iYear);

                //Day Events
                for (int iEvent = 0; iEvent < 44; iEvent++)
                {
                    aiIndex2[1] = iEvent;

                    //Event
                    m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_EVENT, aiIndex2, 0);
                    //Month
                    m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_MONTH, aiIndex2, 0);
                    //Day Of Month
                    m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_CALENDAR_DAY_OF_MONTH, aiIndex2, 0);
                }
            }

            //Seasons
            for (int iSeason = 0; iSeason < 8; iSeason++)
            {
                //Set season
                aiIndex3[SEASON_INDEX] = iSeason;
                aiIndex1[SEASON_INDEX] = iSeason;

                //Programmed
                m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_PROGRAMMED, aiIndex1, 0);

                //Day to Day Types
                m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_DAY_TO_DAY_TYPE, aiIndex1, 0);

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

                        m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_RATE_EVENT, aiIndex3, 0);
                        m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_HOUR, aiIndex3, 0);
                        m_TOUTables.SetValue(CentronTblEnum.MFGTBL42_TOU_DAY_TYPE_MINUTE, aiIndex3, 0);

                    } // For each switchpoint
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
        private void ClearDST()
        {
            int iStartYear = DateTime.Now.Year;
            int[] anIndex1 = { 0 };
            int[] anIndex2 = { 0, 0 };

            //DST Hour
            m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_DST_HOUR, null, 0);

            //DST Minute
            m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_DST_MINUTE, null, 0);

            //DST Offset
            m_TOUTables.SetValue(CentronTblEnum.MFGTB212_DST_OFFSET, null, 0);

            //DST Date Config
            for (int i = 0; i < MAX_DST_YEARS; i++)
            {
                anIndex1[0] = i;
                anIndex2[0] = i;

                //DST Year
                m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_DST_CALENDAR_YEAR, anIndex1, iStartYear + i - 2000);

                //DST On Event
                anIndex2[1] = 0;
                m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_EVENT, anIndex2, 0);
                m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_MONTH, anIndex2, 0);
                m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_DAY_OF_MONTH, anIndex2, 0);

                //DST Off Event
                anIndex2[1] = 1;
                m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_EVENT, anIndex2, 0);
                m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_MONTH, anIndex2, 0);
                m_TOUTables.SetValue(CentronTblEnum.MFGTBL212_CALENDAR_DAY_OF_MONTH, anIndex2, 0);
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
        private ushort GetTypicalWeek(CTOUSchedule TOUSchedule)
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
        private ushort GetDaytypeIndex(CTOUSchedule TOUSchedule,
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
        private TOUConfig.DayEvent GetDayEvent(CSwitchPoint SP)
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
        private TOUConfig.DayEvent GetOutputOffEvent(CSwitchPoint SP)
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

        private CentronTables m_TOUTables = null;
        /// <summary>
        /// TOU Data Set for partial TOU reconfiguration.
        /// </summary>
        public TableData[] TOUDataSet;
        /// <summary>
        /// DST Data Set for partial DST reconfiguration.
        /// </summary>
        public TableData[] DSTDataSet;

        #endregion
    }
}
