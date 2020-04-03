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
//                             Copyright © 20?? - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    #region StdTable51 Class
    /// <summary>
    /// Actual Time and TOU Limiting Table - Table 51
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    // 10/11/06 RCG 7.35.00 N/A    Created

    public class StdTable51 : AnsiTable
    {
        #region Constants
        private const uint TABLE_51_SIZE = 9;
        private const int NBR_SPECIAL_SCHED_SHIFT = 4;

        #endregion

        #region Definitions
        private enum TimeFuncFlag1Masks : byte
        {
            TOU_SELF_READ_FLAG = 0x01,
            SEASON_SELF_READ_FLAG = 0x02,
            SEASON_DEMAND_RESET_FLAG = 0x04,
            SEASON_CHNG_ARMED_FLAG = 0x08,
            SORT_DATES_FLAG = 0x10,
            ANCHOR_DATE_FLAG = 0x20,
        }

        private enum TimeFuncFlag2Masks : byte
        {
            CAP_DST_AUTO_FLAG = 0x01,
            SEPARATE_WEEKDAYS_FLAG = 0x02,
            SEPARATE_SUM_DEMANDS_FLAG = 0x04,
            SORT_TIER_SWITCHES_FLAG = 0x08,
            CAP_TM_ZN_OFFSET_FLAG = 0x10,
        }

        private enum CalendarBitfieldMasks : byte
        {
            NBR_SEASON = 0x0F,
            NBR_SPECIAL_SCHED = 0xF0,
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for this session.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/11/06 RCG 7.35.00 N/A    Created

        public StdTable51(CPSEM psem)
            : base (psem, 51, TABLE_51_SIZE)
        {
        }

        /// <summary>
        /// Reads the data for this table out of the meter.
        /// </summary>
        /// <returns>The PSEMResponse code for the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/11/06 RCG 7.35.00 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable51.Read");

            // Read the table 
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                // Parse the data
                m_byTimeFuncFlag1 = m_Reader.ReadByte();
                m_byTimeFuncFlag2 = m_Reader.ReadByte();
                m_byCalenderFunc = m_Reader.ReadByte();
                m_byNumberOfNonRecurringDates = m_Reader.ReadByte();
                m_byNumberOfRecurringDates = m_Reader.ReadByte();
                m_usNumberOfTierSwitches = m_Reader.ReadUInt16();
                m_usCalendarTableSize = m_Reader.ReadUInt16();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns true if the meter is using TOU triggered self reads.
        /// Otherwise returns false.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public bool IsUsingTOUSelfReads
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte byMasked;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Using TOU Self Reads"));
                    }
                }

                // Mask off the bit so we only have the one we need
                byMasked = (byte)(m_byTimeFuncFlag1 & (byte)TimeFuncFlag1Masks.TOU_SELF_READ_FLAG);

                // If the masked off byte is the same as the mask itself then
                // we know the bit is set (true).
                return byMasked == (byte)TimeFuncFlag1Masks.TOU_SELF_READ_FLAG;
            }
        }

        /// <summary>
        /// Returns true of the meter is using the ability to perform a self read
        /// on a season change. Returns false otherwise.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public bool IsUsingSeasonSelfReads
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte byMasked;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Using Season Self Reads"));
                    }
                }

                // Mask off the bit so we only have the one we need
                byMasked = (byte)(m_byTimeFuncFlag1 & (byte)TimeFuncFlag1Masks.SEASON_SELF_READ_FLAG);

                // If the masked off byte is the same as the mask itself then
                // we know the bit is set (true).
                return byMasked == (byte)TimeFuncFlag1Masks.SEASON_SELF_READ_FLAG;
            }
        }

        /// <summary>
        /// Returns true if the meter is using the ability to cause a demand reset
        /// on a season change. Returns false otherwise.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public bool IsUsingSeasonDemandReset
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte byMasked;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Using Season Demand Reset"));
                    }
                }

                // Mask off the bit so we only have the one we need
                byMasked = (byte)(m_byTimeFuncFlag1 & (byte)TimeFuncFlag1Masks.SEASON_DEMAND_RESET_FLAG);

                // If the masked off byte is the same as the mask itself then
                // we know the bit is set (true).
                return byMasked == (byte)TimeFuncFlag1Masks.SEASON_DEMAND_RESET_FLAG;
            }
        }

        /// <summary>
        /// Returns true if the meter requires that all non recurring dates to be 
        /// pre-sorted by date before being placed in the meter. Returns false otherwise
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public bool RequiresNonRecurringDateSorting
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte byMasked;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Requires Non Recurring Date Sorting"));
                    }
                }

                // Mask off the bit so we only have the one we need
                byMasked = (byte)(m_byTimeFuncFlag1 & (byte)TimeFuncFlag1Masks.SORT_DATES_FLAG);

                // If the masked off byte is the same as the mask itself then
                // we know the bit is set (true).
                return byMasked == (byte)TimeFuncFlag1Masks.SORT_DATES_FLAG;
            }
        }

        /// <summary>
        /// Returms true if the meter accepts an anchor date. Returns false otherwise
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public bool AcceptsAnchorDates
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte byMasked;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Accepts Anchor Dates"));
                    }
                }

                // Mask off the bit so we only have the one we need
                byMasked = (byte)(m_byTimeFuncFlag1 & (byte)TimeFuncFlag1Masks.ANCHOR_DATE_FLAG);

                // If the masked off byte is the same as the mask itself then
                // we know the bit is set (true).
                return byMasked == (byte)TimeFuncFlag1Masks.ANCHOR_DATE_FLAG;
            }
        }

        /// <summary>
        /// Returns true if the meter handles DST changes independently of
        /// dates supplied in table 54. Returns false otherwise.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public bool AutoHandleDSTChangeEnabled
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte byMasked;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Auto Handle DST"));
                    }
                }

                // Mask off the bit so we only have the one we need
                byMasked = (byte)(m_byTimeFuncFlag2 & (byte)TimeFuncFlag2Masks.CAP_DST_AUTO_FLAG);

                // If the masked off byte is the same as the mask itself then
                // we know the bit is set (true).
                return byMasked == (byte)TimeFuncFlag2Masks.CAP_DST_AUTO_FLAG;
            }
        }

        /// <summary>
        /// Returns true if the meter is using different schedules for each of the
        /// five weekdays. Returns false otherwise.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public bool IsUsingScheduleForEachWeekday
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte byMasked;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Seperste Schedules for Weekdays"));
                    }
                }

                // Mask off the bit so we only have the one we need
                byMasked = (byte)(m_byTimeFuncFlag2 & (byte)TimeFuncFlag2Masks.SEPARATE_WEEKDAYS_FLAG);

                // If the masked off byte is the same as the mask itself then
                // we know the bit is set (true).
                return byMasked == (byte)TimeFuncFlag2Masks.SEPARATE_WEEKDAYS_FLAG;
            }
        }

        /// <summary>
        /// Returns true if the meter is switching summation and demands
        /// seperately. Returns false otherwise.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public bool IsSeperateSummationAndDemand
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte byMasked;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Summation and Demand Seperation"));
                    }
                }

                // Mask off the bit so we only have the one we need
                byMasked = (byte)(m_byTimeFuncFlag2 & (byte)TimeFuncFlag2Masks.SEPARATE_SUM_DEMANDS_FLAG);

                // If the masked off byte is the same as the mask itself then
                // we know the bit is set (true).
                return byMasked == (byte)TimeFuncFlag2Masks.SEPARATE_SUM_DEMANDS_FLAG;
            }
        }

        /// <summary>
        /// Returns true if the meter requires that Tier switches be pre-sorted.
        /// Returns false otherwise.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public bool RequiresTierSwitchSorting
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte byMasked;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Tier Switch Sorting"));
                    }
                }

                // Mask off the bit so we only have the one we need
                byMasked = (byte)(m_byTimeFuncFlag2 & (byte)TimeFuncFlag2Masks.SORT_TIER_SWITCHES_FLAG);

                // If the masked off byte is the same as the mask itself then
                // we know the bit is set (true).
                return byMasked == (byte)TimeFuncFlag2Masks.SORT_TIER_SWITCHES_FLAG;
            }
        }

        /// <summary>
        /// Returns true if the time zone offset capability is available int the meter.
        /// Returns false otherwise.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public bool IsTimeZoneAvailable
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Time Zone Availability"));
                    }
                }

                return TranslateIsTimeZoneAvailable(m_byTimeFuncFlag2);
            }
        }

        /// <summary>
        /// Gets the number of seasons in use by the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public int NumberOfSeason
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte byValue;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Seasons"));
                    }
                }

                // This value is the lower four bits of the Calender Func byte so we need
                // to translate this value into an int
                byValue = (byte)(m_byCalenderFunc & (byte)CalendarBitfieldMasks.NBR_SEASON);

                return (int)byValue;
            }
        }

        /// <summary>
        /// Returns the number of special schedules in use by the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public int NumberOfSpecialSchedules
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte byValue;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Special Schedules"));
                    }
                }

                // This value is the upper four bits of the Calender Func byte so we need
                // to translate this value into an int
                byValue = (byte)(m_byCalenderFunc & (byte)CalendarBitfieldMasks.NBR_SPECIAL_SCHED);
                byValue = (byte)(byValue >> NBR_SPECIAL_SCHED_SHIFT);

                return (int)byValue;
            }
        }

        /// <summary>
        /// Gets the number of non recurring dates supported by the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public int NumberOfNonRecurringDates
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Non Recurring Dates"));
                    }
                }

                return (int)m_byNumberOfNonRecurringDates;
            }
        }

        /// <summary>
        /// Gets the number of recurring dates supported by the meter
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public int NumberOfRecurringDates
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Recurring Dates"));
                    }
                }

                return (int)m_byNumberOfRecurringDates;
            }
        }

        /// <summary>
        /// Gets the number of tier switches supported by the calendar
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public ushort NumberOfTierSwitches
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Tier Switches"));
                    }
                }

                return m_usNumberOfTierSwitches;
            }
        }

        /// <summary>
        /// Gets the size of the calendar table (table 54) in bytes
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public ushort CalendarTableSize
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Calendar Table Size"));
                    }
                }

                return m_usCalendarTableSize;
            }
        }

        #endregion

        #region Static Public Methods (Translation Methods)

        /// <summary>
        /// Method to conver the Time Func Flag from Table 51 to the bool value
        ///   that tells us if Time Zone is Available in the device
        /// </summary>
        /// <param name="byTimeFuncFlag2">Value from Table 51</param>
        /// <returns>bool - Does meter support Time Zone</returns>
        public static bool TranslateIsTimeZoneAvailable(byte byTimeFuncFlag2)
        {
            // Mask off the bit so we only have the one we need
            byte byMasked = (byte)(byTimeFuncFlag2 & (byte)TimeFuncFlag2Masks.CAP_TM_ZN_OFFSET_FLAG);

            // If the masked off byte is the same as the mask itself then
            // we know the bit is set (true).
            return byMasked == (byte)TimeFuncFlag2Masks.CAP_TM_ZN_OFFSET_FLAG;
        }
        #endregion

        #region Member Variables
        private byte m_byTimeFuncFlag1;
        private byte m_byTimeFuncFlag2;
        private byte m_byCalenderFunc;
        private byte m_byNumberOfNonRecurringDates;
        private byte m_byNumberOfRecurringDates;
        private ushort m_usNumberOfTierSwitches;
        private ushort m_usCalendarTableSize;

        #endregion
    }

    #endregion

    #region StdTable52 Class
    /// <summary>
    /// Clock - Table 52
    /// <remarks>
    /// This table is not cached but some of the fields are.
    /// </remarks>
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    // 10/12/06 RCG 7.35.00 N/A    Created

    public class StdTable52 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 4000;

        #endregion

        #region Definitions
        /// <summary>
        /// Enumeration for encapsulating the day of the week as represented
        /// in the TIME_DATE_QUAL bit field
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public enum DayOfWeek : byte
        {
            /// <summary>Sunday</summary>
            Sunday = 0,
            /// <summary>Monday</summary>
            Monday = 1,
            /// <summary>Tuesday</summary>
            Tuesday = 2,
            /// <summary>Wednesday</summary>
            Wednesday = 3,
            /// <summary>Thursday</summary>
            Thursday = 4,
            /// <summary>Friday</summary>
            Friday = 5,
            /// <summary>Saturday</summary>
            Saturday = 6,
        }

        /// <summary>
        /// Contains the maks for the Time Date Qual bit field
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        private enum TimeDateQualMask : byte
        {
            DAY_OF_WEEK = 0x07,
            DST_FLAG = 0x08,
            GMT_FLAG = 0x10,
            TM_ZN_APPLIED_FLAG = 0x20,
            DST_APPLIED_FLAG = 0x40,
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for this session</param>
        /// <param name="Table0">The table object for table 0.</param>
        /// <param name="usTableID">The C12.19 table ID.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00    N/A    Created
        // 07/18/13 jrf 2.80.54 WR 411418 Added parameter to set table ID and defaulted to 52.
        // 03/11/16 AF  4.50.236 WR651410 Adding a table timeout so that we can cache some table values (not time)
        //                                for a short while
        //
        public StdTable52(CPSEM psem, CTable00 Table0, ushort usTableID = 52)
            : base(psem, usTableID, GetTableSize(Table0), TABLE_TIMEOUT) 
        {
        }

        /// <summary>
        /// Reads the data for this table out of the meter.
        /// </summary>
        /// <returns>The PSEMResponse code for the read.</returns>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ -------------------------------------------
        // 10/11/06 RCG 7.35.00    N/A    Created
        // 07/18/13 jrf 2.80.54 WR 411418 Using stored table ID to identify table in log.
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Table" 
                + m_TableID.ToString(CultureInfo.InvariantCulture) + ".Read");

            // Read the table 
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                // Parse the data
                m_dtCurrentTime = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                m_byTimeDateQual = m_Reader.ReadByte();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current time from the meter
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public DateTime CurrentTime
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                Result = Read();
                if (PSEMResponse.Ok != Result)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the Current Time"));
                }

                return m_dtCurrentTime;
            }
        }

        /// <summary>
        /// Gets the current day of the week
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created
        // 03/11/16 AF  4.50.236 WR651410 Changed to allow caching of the value
        //
        public DayOfWeek Day
        {
            get
            {
                byte byValue;

                ReadUnloadedTable();

                // Mask off the bits we need
                byValue = (byte)(m_byTimeDateQual & (byte)TimeDateQualMask.DAY_OF_WEEK);

                return (DayOfWeek)byValue;
            }
        }

        /// <summary>
        /// Returns true if the meter is currently in DST. Otherwise returns false.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created
        // 03/11/16 AF  4.50.236 WR651410 Changed to allow caching of the value
        //
        public bool IsInDST
        {
            get
            {
                byte byValue;

                ReadUnloadedTable();

                // Mask off the bits we need
                byValue = (byte)(m_byTimeDateQual & (byte)TimeDateQualMask.DST_FLAG);

                return (byValue == (byte)TimeDateQualMask.DST_FLAG);
            }
        }

        /// <summary>
        /// Returns true if the current time is reported in GMT. Otherwise
        /// return false.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created
        // 03/11/16 AF  4.50.236 WR651410 Changed to allow caching of the value
        //
        public bool IsGMT
        {
            get
            {
                byte byValue;

                ReadUnloadedTable();

                // Mask off the bits we need
                byValue = (byte)(m_byTimeDateQual & (byte)TimeDateQualMask.GMT_FLAG);

                return (byValue == (byte)TimeDateQualMask.GMT_FLAG);
            }
        }

        /// <summary>
        /// Returns true if the time zone has been applied to the current time.
        /// Otherwise returns false.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created
        // 03/11/16 AF  4.50.236 WR651410 Changed to allow caching of the value
        //
        public bool IsTimeZoneApplied
        {
            get
            {
                byte byValue;

                ReadUnloadedTable();

                // Mask off the bits we need
                byValue = (byte)(m_byTimeDateQual & (byte)TimeDateQualMask.TM_ZN_APPLIED_FLAG);

                return (byValue == (byte)TimeDateQualMask.TM_ZN_APPLIED_FLAG);
            }
        }

        /// <summary>
        /// Returns true if DST has been applied to the current time.
        /// Otherwise returns false
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        // 03/11/16 AF  4.50.236 WR651410 Changed to allow caching of the value
        //
        public bool IsDSTApplied
        {
            get
            {
                byte byValue;

                ReadUnloadedTable();

                // Mask off the bits we need
                byValue = (byte)(m_byTimeDateQual & (byte)TimeDateQualMask.DST_APPLIED_FLAG);

                return (byValue == (byte)TimeDateQualMask.DST_APPLIED_FLAG);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the size of the table.
        /// </summary>
        /// <param name="Table0">The table object for table 0.</param>
        /// <returns>The size of the table in bytes.</returns>
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00    N/A    Created
        // 07/18/13 jrf 2.80.54 WR 411418 Made protected.
        //
        protected static uint GetTableSize(CTable00 Table0)
        {
            uint uiSize = 0;

            uiSize += Table0.LTIMESize; // CLOCK_CALENDAR
            uiSize += 1; // TIME_DATE_QUAL

            return uiSize;
        }

        #endregion

        #region Member Variables
        private DateTime m_dtCurrentTime;
        private byte m_byTimeDateQual;

        #endregion
    }

    #endregion

    #region StdTable53 Class
    /// <summary>
    /// Time Offset - Table 53
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    // 10/12/06 RCG 7.35.00 N/A    Created

    public class StdTable53 : AnsiTable
    {
        #region Public Methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for this session</param>
        /// <param name="Table0">The Table 0 object for the current device</param>
        /// <param name="Table51">The Table 51 object for the current device</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public StdTable53(CPSEM psem, CTable00 Table0, StdTable51 Table51)
            : base(psem, 53, StdTable53.GetTableSize(Table0, Table51))
        {
            m_Table51 = Table51;
        }

        /// <summary>
        /// Reads the table from the device
        /// </summary>
        /// <returns>A PSEM response code that represents the result of the read</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable53.Read");

            // Read the table 
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                // Parse the data
                m_tsDSTTimeEffective = m_Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                m_byDSTTimeAmount = m_Reader.ReadByte();

                // Only read the time zone offset if it is supported
                if (m_Table51.IsTimeZoneAvailable == true)
                {
                    m_sTimeZoneOffset = m_Reader.ReadInt16();
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the time of the day when a DST time change will occur
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public TimeSpan DSTTimeEffective
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading DST Time Effective"));
                    }
                }

                return m_tsDSTTimeEffective;
            }
        }

        /// <summary>
        /// Gets a TimeSpan that represents the amount of time to change the clock
        /// when a DST time change occurs
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public TimeSpan DSTAdjustAmount
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                TimeSpan Span;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading DST Adjust Amount"));
                    }
                }

                Span = TimeSpan.FromMinutes((double)m_byDSTTimeAmount);

                return Span;
            }
        }

        /// <summary>
        /// Gets the Time Zone Offset for the time zone the device is located in
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public TimeSpan TimeZoneOffset
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                TimeSpan Span;

                // Make sure that Time Zone Offset is supported in the meter
                if (m_Table51.IsTimeZoneAvailable != true)
                {
                    throw new NotSupportedException("This device does not support time zone offset");
                }

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Time Zone Offset"));
                    }
                }

                Span = TimeSpan.FromMinutes((double)m_sTimeZoneOffset);

                return Span;
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the size of the table in bytes.
        /// </summary>
        /// <param name="Table0">The Table 0 object.</param>
        /// <param name="Table51">The Table 51 object.</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        private static uint GetTableSize(CTable00 Table0, StdTable51 Table51)
        {
            uint uiSize = 0;

            // DST_TIME_EFF
            uiSize += Table0.TIMESize;
            // DST_TIME_AMT
            uiSize += 1;

            if (Table51.IsTimeZoneAvailable == true)
            {
                // TIME_ZONE_OFFSET
                uiSize += 2; 
            }

            return uiSize;
        }
        #endregion

        #region Member Variables
        private StdTable51 m_Table51;

        private TimeSpan m_tsDSTTimeEffective;
        private byte m_byDSTTimeAmount;
        private short m_sTimeZoneOffset;

        #endregion

    }

    #endregion
}
