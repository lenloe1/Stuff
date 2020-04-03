///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
// embodying substantial creative efforts and trade secrets, confidential 
// information, ideas and expressions. No part of which may be reproduced or 
// transmitted in any form or by any means electronic, mechanical, or 
// otherwise.  Including photocopying and recording or in connection with any
// information storage or retrieval system without the permission in writing 
// from Itron, Inc.
//
//                              Copyright © 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;

namespace Itron.Metering.Device
{
    /// <summary>
    /// This class represents a C12.19 Calendar record (table 54) structure.
    /// The table contains the schedule definition for TOU metering activities
    /// </summary>
    public class C1219_CalendarRcd
    {
        #region Definitions

        /// <summary>
        /// Enumeration for the actions that can take place when a specific date occurs
        /// </summary>
        /// <remarks>
        /// This enum is currently not in use.
        /// </remarks>
        private enum CalendarControlMasks : byte
        {
            NO_ACTION_FLAG = 0x00,      //No Action
            DST_ON_FLAG = 0x01,         //DST On
            DST_OFF_FLAG = 0x02,        //DST Off
            SEASON_0_FLAG = 0x03,       //Select Season 0
            SEASON_1_FLAG = 0x04,       //Select Season 1
            SEASON_2_FLAG = 0x05,       //Select Season 2
            SEASON_3_FLAG = 0x06,       //Select Season 3
            SEASON_4_FLAG = 0x07,       //Select Season 4
            SEASON_5_FLAG = 0x08,       //Select Season 5
            SEASON_6_FLAG = 0x09,       //Select Season 6
            SEASON_7_FLAG = 0x0A,       //Select Season 7
            SEASON_8_FLAG = 0x0B,       //Select Season 8
            SEASON_9_FLAG = 0x0C,       //Select Season 9
            SEASON_10_FLAG = 0x0D,      //Select Season 10
            SEASON_11_FLAG = 0x0E,      //Select Season 11
            SEASON_12_FLAG = 0x0F,      //Select Season 12
            SEASON_13_FLAG = 0x10,      //Select Season 13
            SEASON_14_FLAG = 0x11,      //Select Season 14
            SEASON_15_FLAG = 0x12,      //Select Season 15
            HOLIDAY = 0x13,             //Special Schedule 0
            SPECIAL_SCHED_1 = 0x14,     //Special Schedule 1
            SPECIAL_SCHED_2 = 0x15,     //Special Schedule 2
            SPECIAL_SCHED_3 = 0x16,     //Special Schedule 3
            SPECIAL_SCHED_4 = 0x17,     //Special Schedule 4
            SPECIAL_SCHED_5 = 0x18,     //Special Schedule 5
            SPECIAL_SCHED_6 = 0x19,     //Special Schedule 6
            SPECIAL_SCHED_7 = 0x1A,     //Special Schedule 7
            SPECIAL_SCHED_8 = 0x1B,     //Special Schedule 8
            SPECIAL_SCHED_9 = 0x1C,     //Special Schedule 9
            SPECIAL_SCHED_10 = 0x1D,     //Special Schedule 10
            SPECIAL_SCHED_11 = 0x1E,     //Special Schedule 11
            EOL = 0x1F,                 //End of List
        }

        /// <summary>
        /// Bit mask for the TOU rates
        /// </summary>
        /// <remarks>
        /// This enum is currently not in use.
        /// </remarks>
        private enum RatesMasks : byte
        {
            RATE_A_MASK = 0x01,
            RATE_B_MASK = 0x02,
            RATE_C_MASK = 0x03,
            RATE_D_MASK = 0x04,
        }

        /// <summary>
        /// Structure for holding the data of one recurring event date
        /// </summary>
        public struct RECURR_DATE_RCD
        {
            /// <summary>
            /// Recurring date in an RDATE format
            /// </summary>
            public UInt16 RecurrDateBfld;
            /// <summary>
            /// 
            /// </summary>
            public byte CalendarControl;
            /// <summary>
            /// 
            /// </summary>
            public bool DemandResetFlag;
            /// <summary>
            /// 
            /// </summary>
            public bool SelfReadFlag;
            /// <summary>
            /// Comment that may identify the recurring date
            /// </summary>
            public string Comment;
        }

        /// <summary>
        /// Structure for holding the data of one non-recurring event date
        /// </summary>
        public struct NON_RECURR_DATE_RCD
        {
            /// <summary>
            /// Non-recurring event date.  Action shall occur at midnight
            /// </summary>
            public DateTime NonRecurrDate;
            /// <summary>
            /// 
            /// </summary>
            public byte CalendarControl;
            /// <summary>
            /// 
            /// </summary>
            public bool DemandResetFlag;
            /// <summary>
            /// 
            /// </summary>
            public bool SelfReadFlag;
            /// <summary>
            /// Comment that may identify the non-recurring date
            /// </summary>
            public string Comment;
        }

        /// <summary>
        /// Structure for holding the data of a rate change event (tier switch)
        /// </summary>
        public struct TIER_SWITCH_RCD
        {
            ///// <summary>
            ///// Defines a switch point for a certain day type
            ///// </summary>
            //public UInt16 TierSwitchBfld;
            /// <summary>
            /// 
            /// </summary>
            public byte NewTier;
            /// <summary>
            /// 
            /// </summary>
            public byte SwitchMinute;
            /// <summary>
            /// 
            /// </summary>
            public byte SwitchHour;
            /// <summary>
            /// Specifies the daily schedule type associated with the tier switch
            /// </summary>
            public byte DaySchedNum;
        }

        /// <summary>
        /// This struct is used if the Table 51 Separate Weekdays Flag is false;
        /// i.e., the meter does not support separate schedules for each weekday.
        /// </summary>
        public struct WEEKDAY_SCHEDULE_RCD
        {
            /// <summary></summary>
            public byte SaturdaySchedule;
            /// <summary></summary>
            public byte SundaySchedule;
            /// <summary></summary>
            public byte WeekdaySchedule;
            /// <summary></summary>
            public byte HolidaySchedule;
        }

        /// <summary>
        /// This struct is used if the table 51 Separate Weekdays Flag is true;
        /// i.e., the meter supports separate schedules for each weekday.
        /// </summary>
        public struct FULL_WEEKDAY_SCHEDULE_RCD
        {
            /// <summary></summary>
            public byte SaturdaySchedule;
            /// <summary></summary>
            public byte SundaySchedule;
            /// <summary></summary>
            public byte MondaySchedule;
            /// <summary></summary>
            public byte TuesdaySchedule;
            /// <summary></summary>
            public byte WednesdaySchedule;
            /// <summary></summary>
            public byte ThursdaySchedule;
            /// <summary></summary>
            public byte FridaySchedule;
            /// <summary></summary>
            public byte HolidaySchedule;
        }

        /// <summary>
        /// This structure represents a C12.19 table 54 CALENDAR_RCD.  The Anchor Date 
        /// may or may not be there depending on the table 51 ANCHOR_DATE_FLAG.  The 
        /// sizes of the arrays depends on various table 51 flags.
        /// </summary>
        public struct CALENDAR_RCD
        {
            /// <summary>
            /// If specified, any recurring date using the period/offset
            /// type shall use this date as a starting point
            /// </summary>
            public DateTime AnchorDate;
            /// <summary>
            /// Array containing non-recurring dates
            /// </summary>
            public NON_RECURR_DATE_RCD[] NonRecurringDates;
            /// <summary>
            /// Array containing recurring dates
            /// </summary>
            public RECURR_DATE_RCD[] RecurringDates;
            /// <summary>
            /// Array that contains the tier switches
            /// </summary>
            public TIER_SWITCH_RCD[] TierSwitches;
            /// <summary>
            /// 2-dimensional array containing daily schedule type identifiers.
            /// TODO - I'm not sure this is the right data structure to hold
            /// the daily schedule matrix.  Would it be any better to have
            /// 2 1-dimensional arrays -- one an array of WEEKDAY_SCHEDULE_RCD
            /// and one, an array of FULL_WEEKDAY_SCHEDULE_RCD?
            /// </summary>
            public byte[,] DailyScheduleIDMatrix;
            // TODO - Alternatively, instead of a 2-dimensional array of bytes, 
            // we could have a 1-dimensional array of an enumerated type
            //
            ///// <summary>
            ///// 
            ///// </summary>
            //public WEEKDAY_SCHEDULE_RCD[] DailySchedIDMatrix1;
            ///// <summary>
            ///// 
            ///// </summary>
            //public FULL_WEEKDAY_SCHEDULE_RCD[] DailySchedIDMatrix2;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructs a table 54 CALENDAR_RCD based on table 51 values.
        /// </summary>
        /// <param name="SeparateSumDemandsFlag">Table 51 SEPARATE_SUM_DEMANDS_FLAG.
        /// Determines whether or not end device is switching summation and demands
        /// independently.</param>
        /// <param name="AnchorDateFlag">Table 51 ANCHOR_DATE_FLAG. Whether or not
        /// end device accepts an anchor date for the Period/Delta RDATE type</param>
        /// <param name="NbrNonRecurringDates">Table 51 NBR_NON_RECURR_DATES.
        /// Actual number of nonrecurring dates supported by the end device
        /// calendar</param>
        /// <param name="NbrRecurringDates">Table 51 NBR_RECURR_DATES.  Actual
        /// number of recurring dates supported by the end device calendar</param>
        /// <param name="NbrTierSwitches">Table 51 NBR_TIER_SWITCHES.  Actual
        /// number of tier switches supported by the end device calendar</param>
        /// <param name="SeparateWeekdaysFlag">Table 51 SEPARATE_WEEKDAYS_FLAG.
        /// Whether or not device is capable of having a different schedule for
        /// each of the 5 weekdays</param>
        /// <param name="NbrSeasons">Table 51 NBR_SEASONS.  Actual number of 
        /// seasons in use by the end device</param>
        /// <param name="NbrSpecialScheds">Table 51 NBR_SPECIAL_SCHED.  Actual 
        /// number of special schedules in use by the end device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/08 AF                 Created
        //
        public C1219_CalendarRcd(bool SeparateSumDemandsFlag, bool AnchorDateFlag,
            byte NbrNonRecurringDates, byte NbrRecurringDates, UInt16 NbrTierSwitches,
            bool SeparateWeekdaysFlag, byte NbrSeasons, byte NbrSpecialScheds)
        {
            m_blnSeparateSumDemandsFlag = SeparateSumDemandsFlag;
            m_blnAnchorDateFlag = AnchorDateFlag;
            if (m_blnAnchorDateFlag)
            {
                m_Calendar.AnchorDate = new DateTime();
            }
            m_blnSeparateWeekdaysFlag = SeparateWeekdaysFlag;

            m_byNbrNonRecurringDates = NbrNonRecurringDates;
            m_byNbrRecurringDates = NbrRecurringDates;
            m_usNbrTierSwitches = NbrTierSwitches;
            m_byNbrSeasons = NbrSeasons;
            m_byNbrSpecialSchedules = NbrSpecialScheds;

            if (0 < m_byNbrNonRecurringDates)
            {
                m_Calendar.NonRecurringDates = new NON_RECURR_DATE_RCD[m_byNbrNonRecurringDates];
            }
            if (0 < m_byNbrRecurringDates)
            {
                m_Calendar.RecurringDates = new RECURR_DATE_RCD[m_byNbrRecurringDates];
            }
            if (0 < m_usNbrTierSwitches)
            {
                m_Calendar.TierSwitches = new TIER_SWITCH_RCD[m_usNbrTierSwitches];
            }

            //TODO - not sure how to handle this.  There may be a better data structure
            //than a 2-dimensional array of bytes - see commented out code
            if (!m_blnSeparateWeekdaysFlag)
            {
                m_Calendar.DailyScheduleIDMatrix = new byte[m_byNbrSeasons, 3 + m_byNbrSpecialSchedules];
                //m_Calendar.DailySchedIDMatrix1 = new WEEKDAY_SCHEDULE_RCD[m_byNbrSeasons];
            }
            else
            {
                m_Calendar.DailyScheduleIDMatrix = new byte[m_byNbrSeasons, 7 + m_byNbrSpecialSchedules];
                //m_Calendar.DailySchedIDMatrix2 = new FULL_WEEKDAY_SCHEDULE_RCD[m_byNbrSeasons];
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Table 51 NBR_NON_RECURR_DATES - actual number of non-recurring dates
        /// supported by the end device calendar
        /// </summary>
        public byte NbrNonRecurringDates
        {
            get { return m_byNbrNonRecurringDates; }
            set { m_byNbrNonRecurringDates = value; }
        }

        /// <summary>
        /// Table 51 NBR_RECURR_DATES - actual number of recurring dates
        /// supported by the end device calendar
        /// </summary>
        public byte NbrRecurringDates
        {
            get { return m_byNbrRecurringDates; }
            set { m_byNbrRecurringDates = value; }
        }

        /// <summary>
        /// Table 51 NBR_TIER_SWITCHES - actual number of tier switches
        /// supported by the end device calendar
        /// </summary>
        public UInt16 NbrTierSwitches
        {
            get { return m_usNbrTierSwitches; }
            set { m_usNbrTierSwitches = value; }
        }

        /// <summary>
        /// Table 54 ANCHOR_DATE - If specified, any recurring date using the 
        /// PERIOD/OFFSET type shall use this date as a starting date
        /// </summary>
        public DateTime AnchorDate
        {
            get { return m_Calendar.AnchorDate; }
            set { m_Calendar.AnchorDate = value; }
        }

        /// <summary>
        /// Table 51 NBR_SEASONS - Actual number of seasons in use by the end 
        /// device
        /// </summary>
        public byte NbrSeasons
        {
            get { return m_byNbrSeasons; }
            set { m_byNbrSeasons = value; }
        }

        /// <summary>
        /// Table 51 NBR_SPECIAL_SCHED - Actual number of special schedules in use
        /// by the end device
        /// </summary>
        public byte NbrSpecialSchedules
        {
            get { return m_byNbrSpecialSchedules; }
            set { m_byNbrSpecialSchedules = value; }
        }

        /// <summary>
        /// Table 54 CALENDAR_RCD - Accesses the entire table
        /// </summary>
        public CALENDAR_RCD CalendarRecord
        {
            get { return m_Calendar; }
        }

        #endregion

        #region Private Members

        private bool m_blnSeparateSumDemandsFlag;
        private bool m_blnAnchorDateFlag;
        private bool m_blnSeparateWeekdaysFlag;
        private byte m_byNbrNonRecurringDates;
        private byte m_byNbrRecurringDates;
        private UInt16 m_usNbrTierSwitches;
        private byte m_byNbrSeasons;
        private byte m_byNbrSpecialSchedules;
        private CALENDAR_RCD m_Calendar;

        #endregion
    }
}
