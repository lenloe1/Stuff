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
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Xml;
using Itron.Common.C1219Tables.CentronII;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.DST;
using Itron.Metering.Progressable;
using Itron.Metering.TOU;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
	  /// <summary>
	  /// This class represents the CENTRON C12.19 meter (a.k.a. IMAGE Mono)
	  /// As the name implies this meter is a single phase ANSI C12.19 compliant meter.
	  /// </summary>
	  //  Revision History	
	  //  MM/DD/YY who Version Issue# Description
	  //  -------- --- ------- ------ ---------------------------------------
	  //  08/21/10 SCW         Created
    //
    public partial class CENTRON2_MONO : REGISTER_BASE
    {
        #region Constants

        /// <summary>
        /// Meter type identifier
        /// </summary>
        private const string CENTRON2MONO = "CENTRN2M";
        /// <summary>
        /// Human readable name of meter
        /// </summary>
        private const string CENTRON2MONO_NAME = "CENTRON II (C12.19)";
        /// <summary>
        /// Manufacturer ID
        /// </summary>
        private const string MANUFACTURER = "ITRA";
        /// <summary>
        /// Device class string for HW 3.0 Centron II (MaxImage) - Mono meters.
        /// </summary>
        public const string ITRA_DEVICE_CLASS = "ITRA";
        /// <summary>
        /// Device class string for HW 3.0 Centron II (MaxImage) - Basic Poly meters.
        /// </summary>
        public const string ITRB_DEVICE_CLASS = "ITRB";
        /// <summary>
        /// Device class string for HW 3.0 Centron II (MaxImage) - Advanced Poly  meters.
        /// </summary>
        public const string ITRC_DEVICE_CLASS = "ITRC";
       
        #endregion

        #region Public Methods

        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="PSEM">Protocol obj used to identify the meter</param>
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/22/06 mrj 7.30.00 N/A    Created
		///
		public CENTRON2_MONO( CPSEM PSEM )
			: base(PSEM) 
		{
			//Use the Centron LIDs
			m_LID = new CentronMonoDefinedLIDs();
        }

        /// <summary>
        /// The PasswordReconfigResult reconfigures passwords. 
        /// </summary>
        /// <param name="Passwords">A list of passwords to write to the meter. 
        /// The Primary password should be listed first followed by the secondary
        /// password and so on.  Use empty strings for null passwords.  Passwords
        /// will be truncated or null filled as needed to fit in the device.</param>
        /// <returns>A PasswordReconfigResult object</returns>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 08/21/06 mcm 7.35.00 N/A    Created
        ///	
        public override PasswordReconfigResult ReconfigurePasswords(
                            System.Collections.Generic.List<string> Passwords)
        {
            return STDReconfigurePasswords( Passwords );

        } // ReconfigurePasswords

        #endregion

        #region Public Property

        #region Public Property - DailySelfReadTime
        /// <summary>
        /// Gets the configured daily self read time.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 08/21/07 RCG 8.10.21 N/A    Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        //
        public virtual string DailySelfReadTime
        {
            get
            {
                CENTRON2_MONO_ModeControl AMIModeControl = Table2048.ModeControl as CENTRON2_MONO_ModeControl;
                string strSelfReadTime = "Not Supported";

                if (AMIModeControl != null)
                {
                    strSelfReadTime = AMIModeControl.DailySelfReadTime;
                }
                return strSelfReadTime;
            }
        }
        #endregion DailySelfReadTime

        #region Public Property - DateProgrammed
        /// <summary>
        /// Gets the Date Programmed out of the header of 2048
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue#     Description
        //  -------- --- ------- ---------------------------------------------
        //  10/28/08 KRC 2.00.03 00121848    Created
        // 		
        public override DateTime DateProgrammed
        {
            get
            {
                DateTime dtTimeProgrammed = MeterConfigurationReferenceTime;

                //Get the Date Programmed out of 2048
                uint usDateProgrammed = Table2048.Table2048Header.DateProgrammed;

                // Value in 2048 is the number of seconds since Jan. 1, 2000, so to get
                //  the value returned to Jan. 1, 2000.
                dtTimeProgrammed = dtTimeProgrammed.AddSeconds((double)usDateProgrammed);

                return dtTimeProgrammed;
            }
        }
        #endregion Public Property - DateProgrammed

        #region Public Property - SWRevision
        /// <summary>
        /// Property used to get the software version from the meter
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  03/12/07 mcm 8.00.18 2514   SW Rev not supported for ANSI devices
        ///
        ///</remarks>
        public override String SWRevision
        {
            get
            {
                return Table2048.Table2048Header.SWVerRev.ToString("0.00", CultureInfo.InvariantCulture);
            }
        }
        #endregion SWRevision

        #region Public Property - TOUScheduleID
        /// <summary>
        /// Provides access to the meter's time of use schedule ID.
        /// Note that this is returned as a string since one or more
        /// meters allow non-numeric TOU schedule identifiers
        /// </summary>
        /// <returns>
        /// A string representing the time of use schedule identifier
        /// </returns> 
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  12/14/06 MAH  8.00.00			Created 
        ///  12/03/10 SCW  9.70.13          Added the support to CENTRON II meter
        /// </remarks>
        public override String TOUScheduleID
        {
            get
            {
                if (null == m_Table2048)
                    m_Table2048 = new CTable2048_MaxImage(m_PSEM);
                return m_Table2048.TOU_ID.ToString(CultureInfo.InvariantCulture);
            }
        }
        #endregion TOUScheduleID

        /// <summary>
        /// Property used to get the human readable meter name 
        /// (string).  Use this property when 
        /// displaying the name of the meter to the user.  
        /// This should not be confused with the MeterType 
        /// which is used for meter determination and comparison.
        /// </summary>
        /// <returns>A string representing the human readable name of the 
        /// meter.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/21/11 RCG 2.45.25        Created
        
        public override string MeterName
        {
            get
            {
                return CENTRON2MONO_NAME;
            }
        }

        #endregion Public Property

        #region Internal Methods

        #region Internal Methods - CreateLID
        /// <summary>
        /// Creates a LID object from the given 32-bit number
        /// </summary>
        /// <param name="uiLIDNumber">The 32-bit number that represents the LID</param>
        /// <returns>The LID object for the specified LID</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/07/07 RCG 8.00.11 N/A    Created

        internal override LID CreateLID(uint uiLIDNumber)
        {
            return new CentronMonoLID(uiLIDNumber);
        }
        #endregion Internal Methods - CreateLID

        #endregion Internal Methods

        #region Internal Property

        #region Intrenal Property - Table2048
        /// <summary>
        /// This property returns the correct version of the 2048 table for the
        /// Centron Mono meter.  (Creates it if necessary)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/06 mrj 7.30.00 N/A    Created
        // 11/20/06 KRC 8.00.00 N/A    Changed to Property
        //
        internal override CTable2048 Table2048
        {
            get
            {
                if (null == m_Table2048)
                {
                    m_Table2048 = new CTable2048_MaxImage(m_PSEM);
                }

                return m_Table2048;
            }
        }
        #endregion Intrenal Property - Table2048

        #region Internal Property - DST
        /// <summary>
        /// Provides access to the DST Dates in the meter
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/25/06 KRC 7.36.00
        //  01/05/07 RCG 8.00.05        Promoted from CENTRON_AMI
        // 
        public override List<CDSTDatePair> DST
        {
            get
            {
                if (0 == m_lstDSTDates.Count)
                {
                    m_lstDSTDates = ReadDSTDates();
                }

                return m_lstDSTDates;
            }
        }
        #endregion Internal Property - DST

        #endregion Internal Property

        #region Protected Methods

        #region Protected Methods - ResetDiagnosticCounters
        /// <summary>
        /// Diagnostics are not supported on the Mono meter
        /// </summary>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/26/06 mrj 7.30.00 N/A    Created
        ///
        protected override ItronDeviceResult ResetDiagnosticCounters()
        {
            throw (new NotSupportedException());
        }
        #endregion Protected Methods - ResetDiagnosticCounters

        #endregion Protected Methods

        #region Protected Property

        #region Protected Property - DefaultMeterType
        /// <summary>
        /// Gets the meter type "CENTRN2M
        /// </summary>		
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        //
        protected override string DefaultMeterType
        {
            get
            {
                return CENTRON2MONO;
            }
        }
        #endregion Protected Property - DefaultMeterType

        #region Protected Property - LPPulseWeightMultiplier
        /// <summary>
        /// Gets the multiplier used to calculate the Load Profile Pulse Weight
        /// </summary>		
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        //
        protected override float LPPulseWeightMultiplier
        {
            get
            {
                return 0.01f;
            }
        }
        #endregion Protected Property - LPPulseWeightMultiplier

        #endregion Protected Property

        #region Private Methods

        #region Private Methods - ReadDSTDates
        /// <summary>
        /// Reads the DST dates from the meter from Table 2260
        /// </summary>
        /// <returns>The TOU Schedule object.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------  ---------------------------------------
        // 11/20/10 SCW         N/Q	    Created
        // 11/30/10 jrf 9.70.11         Offseting DST month and day by 1.
        private List<CDSTDatePair> ReadDSTDates()
        {
            if (m_DSTCalendar == null)
                m_DSTCalendar = new CENTRON2_MONO_DSTCalendarConfig(m_PSEM);

            List<CDSTDatePair> lstDSTDates = new List<CDSTDatePair>();
            for (int iYearCounter = 0; iYearCounter < m_DSTCalendar.MaxYears; iYearCounter++)
            {
                CalendarEvent[] CalEvents = m_DSTCalendar.Years[iYearCounter].Events;
                int iYear = 2000 + (int)m_DSTCalendar.Years[iYearCounter].Year;

                CDSTDatePair DST = new CDSTDatePair();
                // Index 0 and Index 1 are the To and From DST Date respectively
                for (int iDayEvent = 0; iDayEvent < m_DSTCalendar.EventsPerYear ; iDayEvent++)
                {
                    eEventType eType = m_DSTCalendar.GetEventType(CalEvents[iDayEvent].Type);
                    if (eEventType.TO_DST == eType)
                    {
                        // It is a valid event
                        // Month and day are zero based so we are adding one.
                        DST.ToDate = new DateTime(iYear, CalEvents[iDayEvent].Month + 1,
                            CalEvents[iDayEvent].Day + 1, m_DSTCalendar.DSTHour, m_DSTCalendar.DSTMinute, 0);
                    }
                    else if (eEventType.FROM_DST == eType)
                    {
                        // It is a valid event
                        // Month and day are zero based so we are adding one.
                        DST.FromDate = new DateTime(iYear, CalEvents[iDayEvent].Month + 1,
                            CalEvents[iDayEvent].Day + 1, m_DSTCalendar.DSTHour, m_DSTCalendar.DSTMinute, 0);
                    }
                }

                lstDSTDates.Add(DST);

                // It may be possible that some of the years are not filled in so we need to
                // make sure that the year is valid by checking to see if the next year is
                // greater than the current
                if (iYearCounter + 1 < m_DSTCalendar.MaxYears && (int)m_DSTCalendar.Years[iYearCounter + 1].Year + 2000 < iYear)
                {
                    break;
                }

            }
            return lstDSTDates;
        }
        #endregion Private Methods - ReadDSTDates

        #endregion

        #region Public Static Method

        #region Public Static Method - CENTRONIIDetermineDailySelfRead
        /// <summary>
        /// Method that takes the Daily Self Read Byte and returns a string
        ///   containing the Daily Self Read time in human readable format.
        /// </summary>
        /// <param name="byDailySelfRead">The Byte from the Meter that has the values</param>
        /// <returns>string - Daily Self Read Time</returns>
        public static string CENTRONIIDetermineDailySelfRead(byte byDailySelfRead)
        {
            return CENTRON2_MONO_ModeControl.DetermineDailySelfRead(byDailySelfRead);
        }
        #endregion Public Static Method - CENTRONIIDetermineDailySelfRead

        #region Public Static Method - TimeOfUseSchedule
        /// <summary>
        /// Provides access to the TOU Schedule in the meter
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/25/06 KRC 7.36.00
        //  01/05/07 RCG 8.00.05        Promoted from CENTRON_AMI
        //  12/03/10 DEO 9.70.13        Promoted from ANSIDevice
        //
        public override CTOUSchedule TimeOfUseSchedule
        {
            get
            {
                if (null == m_TOUSchedule)
                {
                    m_TOUSchedule = ReadCENTRON2TOUSchedule(Table2048.TOUConfig, Table2048.CalendarConfig);
                }

                return (CTOUSchedule)m_TOUSchedule;
            }
        }
        #endregion Public Static Method - TimeOfUseSchedule

        #region Public Static Method - ReadCENTRON2TOUSchedule
        /// <summary>
        /// Reads the TOU Schedule from the meter into a TOUSchedule object
        /// </summary>
        /// <returns>The TOU Schedule object.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------  ---------------------------------------
        // 01/04/07 RCG 8.00.04		    Made more generic and promoted from CENTRON_AMI
        // 04/13/07 RCG 8.00.31 2919    Adding support for Output events.
        // 03/10/08 KRC 1.50.02         Adding Ability to create TOU Schedule from EDL file
        //  12/03/10 DEO 9.70.13        Promoted from ANSIDevice

        public static CTOUSchedule ReadCENTRON2TOUSchedule(TOUConfig TOUConfigTable, CalendarConfig CalendarConfigTable)
        {
            Int16Collection NormalDays;
            Int16Collection HolidayDays;
            ANSITOUSchedule TOUSchedule = new ANSITOUSchedule();
            TOUConfig.TOU_Season CurrentSeason;
            int iNextEventCounter = 0;
            int iPatternID = 0;

            try
            {
                // First set up the typical week so that we know which day corresponds to which daytype
                if (TOUConfigTable.NumberOfSupportedSeasons > 0)
                {
                    CurrentSeason = TOUConfigTable.Seasons[0];

                    // We have to assume that the Typical week is that same for all seasons. NOTE: The Day to Daytype is 1 based
                    // so we need to subtract 1
                    TOUSchedule.TypicalWeek[(int)eTypicalDay.SUNDAY] = TOUSchedule.NormalDays[CurrentSeason.TypicalSunday];
                    TOUSchedule.TypicalWeek[(int)eTypicalDay.MONDAY] = TOUSchedule.NormalDays[CurrentSeason.TypicalMonday];
                    TOUSchedule.TypicalWeek[(int)eTypicalDay.TUESDAY] = TOUSchedule.NormalDays[CurrentSeason.TypicalTuesday];
                    TOUSchedule.TypicalWeek[(int)eTypicalDay.WEDNESDAY] = TOUSchedule.NormalDays[CurrentSeason.TypicalWednesday];
                    TOUSchedule.TypicalWeek[(int)eTypicalDay.THURSDAY] = TOUSchedule.NormalDays[CurrentSeason.TypicalThursday];
                    TOUSchedule.TypicalWeek[(int)eTypicalDay.FRIDAY] = TOUSchedule.NormalDays[CurrentSeason.TypicalFriday];
                    TOUSchedule.TypicalWeek[(int)eTypicalDay.SATURDAY] = TOUSchedule.NormalDays[CurrentSeason.TypicalSaturday];
                }

                for (int iSeasonCounter = 0; iSeasonCounter < TOUConfigTable.NumberOfSupportedSeasons; iSeasonCounter++)
                {
                    // Get the Season that we are dealing with.
                    CurrentSeason = TOUConfigTable.Seasons[iSeasonCounter];
                    NormalDays = new Int16Collection();
                    HolidayDays = new Int16Collection();

                    for (int iDayTypeCounter = 0; iDayTypeCounter < TOUConfigTable.DayTypesPerSeason; iDayTypeCounter++)
                    {
                        CSwitchPointCollection SPColl = new CSwitchPointCollection();
                        for (int iEventCounter = 0; iEventCounter < TOUConfigTable.EventsPerDayType; iEventCounter++)
                        {
                            // Get the Day Event
                            TOUConfig.DayEvent DayEvent = CurrentSeason.TimeOfDayEvents[iDayTypeCounter, iEventCounter];
                            ushort usEvent = DayEvent.Event;
                            if (usEvent != (ushort)TOUConfig.DayEvent.TOUEvent.NoMoreChanges)
                            {
                                if (IsRateChangeEvent(usEvent) == true)
                                {
                                    // We have a valid Event, so proceed with createing a SwitchPoint
                                    int iHour = (int)DayEvent.Hour;
                                    int iMinute = (int)DayEvent.Minute;
                                    int iStartTime = (iHour * 60) + iMinute;
                                    int iEndTime = 24 * 60;

                                    iNextEventCounter = iEventCounter + 1;

                                    while (iNextEventCounter < TOUConfigTable.EventsPerDayType)
                                    {
                                        TOUConfig.DayEvent NextDayEvent = CurrentSeason.TimeOfDayEvents[iDayTypeCounter, iNextEventCounter];

                                        if (IsRateChangeEvent(NextDayEvent.Event) == true)
                                        {
                                            iHour = (int)NextDayEvent.Hour;
                                            iMinute = (int)NextDayEvent.Minute;
                                            iEndTime = (iHour * 60) + iMinute;

                                            // We need to stop looking once we find the next rate change event.
                                            break;
                                        }

                                        iNextEventCounter++;
                                    }

                                    // Add the rate change event 
                                    int iRateIndex = GetRateIndex(usEvent);
                                    // Finally figure out the Switchpoint type
                                    CSwitchPoint SchedSwitchPoint = new CSwitchPoint(iStartTime, iEndTime,
                                                                        iRateIndex, eSwitchPointType.RATE);

                                    SPColl.Add(SchedSwitchPoint);
                                }
                                else if (IsOutputOnEvent(usEvent) == true)
                                {
                                    // We have a valid output on Event, so proceed with createing a SwitchPoint
                                    int iHour = (int)DayEvent.Hour;
                                    int iMinute = (int)DayEvent.Minute;
                                    int iStartTime = (iHour * 60) + iMinute;
                                    int iEndTime = 24 * 60;

                                    int iOutputIndex = GetOutputIndex(usEvent);

                                    // Find the OutputOff event for this rate if one exists
                                    iNextEventCounter = iEventCounter + 1;

                                    while (iNextEventCounter < TOUConfigTable.EventsPerDayType)
                                    {
                                        TOUConfig.DayEvent NextDayEvent = CurrentSeason.TimeOfDayEvents[iDayTypeCounter, iNextEventCounter];

                                        if (IsOutputOffEvent(NextDayEvent.Event) == true)
                                        {
                                            // Check to see if the index matches
                                            if (iOutputIndex == GetOutputIndex(NextDayEvent.Event))
                                            {
                                                iHour = (int)NextDayEvent.Hour;
                                                iMinute = (int)NextDayEvent.Minute;
                                                iEndTime = (iHour * 60) + iMinute;

                                                // We need to stop looking once we find the next rate change event.
                                                break;
                                            }
                                        }

                                        iNextEventCounter++;
                                    }

                                    // Finally figure out the Switchpoint type
                                    CSwitchPoint SchedSwitchPoint = new CSwitchPoint(iStartTime, iEndTime,
                                                                        iOutputIndex, eSwitchPointType.OUTPUT);

                                    SPColl.Add(SchedSwitchPoint);
                                }

                                // We do not need to handle the OutputOff event since they get handled by the OutputOn check
                            }
                        }

                        // Since we have no way of knowing whether the the patterns for the current season are related
                        // to the patterns in other seasons we need to add the patterns regardless of whether or not it 
                        // has already been duplicated in another season

                        // To keep the patterns unique we need to add in an offset for the season number

                        iPatternID = iDayTypeCounter + iSeasonCounter * TOUConfigTable.DayTypesPerSeason;

                        CPattern SchedPattern = new CPattern(iPatternID, "Pattern " + iDayTypeCounter.ToString(CultureInfo.InvariantCulture),
                                                                SPColl);

                        NormalDays.Add((short)iPatternID);

                        // The Day to Daytype conversions are 1's based so subract 1
                        if (iDayTypeCounter == CurrentSeason.TypicalHoliday)
                        {
                            // This Day Type is a holiday
                            HolidayDays.Add((short)iPatternID);
                        }

                        TOUSchedule.Patterns.Add(SchedPattern);
                    }

                    // Add the season to the schedule
                    CSeason SchedSeason = new CSeason(iSeasonCounter + 1, "Season " + (iSeasonCounter + 1).ToString(CultureInfo.InvariantCulture), NormalDays, HolidayDays);

                    TOUSchedule.Seasons.Add(SchedSeason);
                }

                // Now deal with the Calendar part of the config
                TOUSchedule.TOUID = CalendarConfigTable.CalendarID;

                for (int iYearCounter = 0; iYearCounter < CalendarConfigTable.MaxYears; iYearCounter++)
                {
                    CEventCollection EventColl = new CEventCollection();
                    CalendarEvent[] CalEvents =
                            CalendarConfigTable.Years[iYearCounter].Events;
                    int iYear = 2000 + (int)CalendarConfigTable.Years[iYearCounter].Year;

                    // Start at Index 2, which is the first non-DST Event
                    for (int iDayEvent = CalendarConfigTable.DSTEventsPerYear;
                        iDayEvent < CalendarConfigTable.EventsPerYear; iDayEvent++)
                    {
                        eEventType eType = CalendarConfigTable.GetEventType(CalEvents[iDayEvent].Type);
                        int iEventIndex = iDayEvent;

                        if (eEventType.NO_EVENT != eType)
                        {
                            // It is a valid event
                            DateTime dtDate = new DateTime(iYear, CalEvents[iDayEvent].Month + 1,
                                                         CalEvents[iDayEvent].Day + 1);

                            // Determine the index for the event
                            if (eType == eEventType.SEASON)
                            {
                                iEventIndex = CalEvents[iDayEvent].Type - (int)CalendarEvent.CalendarEventType.SEASON1-1;
                            }
                            else if (eType == eEventType.HOLIDAY)
                            {
                                // Determine which Holiday day type to use
                                // Currently the ANSI devices only support 1 holiday day type so this is always 0
                                iEventIndex = 0;
                            }

                            CEvent Event = new CEvent(dtDate, eType,
                                        iEventIndex, "Event " + iDayEvent.ToString(CultureInfo.InvariantCulture));

                            EventColl.Add(Event);
                        }
                    }
                    CYear Year = new CYear(iYear, EventColl);

                    TOUSchedule.Years.Add(Year);

                    // It may be possible that some of the years are not filled in so we need to
                    // make sure that the year is valid by checking to see if the next year is
                    // greater than the current
                    if (iYearCounter + 1 < CalendarConfigTable.MaxYears &&
                        (int)CalendarConfigTable.Years[iYearCounter + 1].Year + 2000 < iYear)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                throw (e);
            }

            return TOUSchedule;
        }
        #endregion Public Static Method - ReadCENTRON2TOUSchedule

        #endregion Public Static Method

        #region Private Properties

        // private DateTime m_dtLastActivationDate;
        // private List<PendingEventActivationRecord> m_lstPendingTableRecords;
        // private SecurityLevel? m_CurrentSecurityLevel = null;
        private CENTRON2_MONO_DSTCalendarConfig m_DSTCalendar = null;

        #endregion Private Properties
    }
}
