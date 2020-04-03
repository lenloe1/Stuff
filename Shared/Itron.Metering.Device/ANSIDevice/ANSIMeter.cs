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
//                           Copyright © 2013 - 2017
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.DST;
using Itron.Metering.TOU;
using Itron.Metering.Utilities;



namespace Itron.Metering.Device
{
    /// <summary>
    /// Class to handle the metering properties and methods common to all supported devices
    /// </summary>
    public abstract partial class ANSIMeter : CANSIDevice, IConfiguration
    {
        #region Constants

        private const uint MAX_SELF_READS = 4;
        private const uint MAX_DEMAND_RESETS = 2;
        private const uint SELF_READ_BUFFER_OFFSET = 0x00800000;
        private const uint DEMAND_RESET_BUFFER_OFFSET = 0x00800000;

        private const int ENERGY_ACT_OF_2005_START_YEAR = 2007;
        private const int MAX_CAL_EVENTS = 44;
        private const int FIRST_TOU_CAL_INDEX = 2; // 0 & 1 are reserved for DST
        private const int SATURN_ADVANCED_RATES = 7;
        /// <summary>The number of seconds in an hour</summary>
        protected const int SECONDS_PER_HOUR = 3600;

        private const uint NUM_LAST_DEMAND_RESETS = 2;
        private const int DAYS_IN_WEEK = 7;

        #endregion

        #region Definitions
        #endregion

        #region Public Methods

        /// <summary>
        /// The ANSIMeter constructor
        /// </summary>
        /// <param name="ceComm">Communications object to use</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CANSIDevice ANSIDevice = new CANSIDevice(comm);
        /// </code>
        /// </example>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/11/13 AF  3.50.02        Created
        //
        public ANSIMeter(Itron.Metering.Communications.ICommunications ceComm)
            : base(ceComm)
        {
        }

        /// <summary>
        /// The ANSIMeter constructor
        /// </summary>
        /// <param name="PSEM">Protocol instance to use</param>
        /// <example><code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// PSEM psem( comm )
        /// CANSIDevice ANSIDevice = new CANSIDevice(psem);
        /// </code></example>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/11/13 AF  3.50.02        Created
        //
        public ANSIMeter(CPSEM PSEM)
            : base(PSEM)
        {

        }

        /// <summary>
        /// Performs a Born Again on the meter. You must be logged
        /// in as a level 5 user in order to perform this operation.
        /// </summary>
        /// <returns>A PSEM Response for the write to Table 07</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/24/12 RCG 2.60.47 N/A    Created
        public PSEMResponse BornAgain()
        {
            byte[] ProcParam = GetMagicKey();
            PSEMResponse Response = PSEMResponse.Err;

            // Since we won't be able to get a return code we will just use Table07 to 
            // call the procedure
            Table07.Procedure = (ushort)Procedures.BORN_AGAIN;
            Table07.ParameterData = ProcParam;

            // Reset the meter
            Response = Table07.Write();

            return Response;
        }

        /// <summary>
        /// Performs a Cold Start (aka three button reset) on the meter. You must be logged
        /// in as a level 5 user in order to perform this operation. This function does not
        /// have a return value as the meter will not be reset after calling this procedure.
        /// </summary>
        /// <returns>A PSEM Response for the write to Table 07</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/19/07 RCG 8.10.15 N/A    Created
        public PSEMResponse ColdStart()
        {
            byte[] ProcParam = new byte[0];
            PSEMResponse Response = PSEMResponse.Err;

            // Since we won't be able to get a return code we will just use Table07 to 
            // call the procedure
            Table07.Procedure = (ushort)Procedures.COLD_START;
            Table07.ParameterData = ProcParam;

            // Reset the meter
            Response = Table07.Write();

            return Response;
        }

        /// <summary>
        /// Performs a clear Base procedure
        /// </summary>
        /// <returns>A PSEM Response for the write to Table 07</returns>
        // Revision History	
        // MM/DD/YY   who Version Issue# Description
        // --------   --- ------- ------ ---------------------------------------
        // 03/15/2010 MA  2.40.25  N/A    Created
        public ItronDeviceResult ClearBaseData()
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[0];  // No parameters for this procedure
            ProcResult = ExecuteProcedure(Procedures.CLEAR_BASE,
                ProcParam, out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ItronDeviceResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ItronDeviceResult.ERROR;
                        break;
                    }
            }

            return Result;

        } // End ClearBaseData()

        /// <summary>
        /// Calls the Seal Canadian procedure
        /// </summary>
        /// <returns>The result of the procedure</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/29/09 MA 2.40.27  N/A    Created
        //
        public ProcedureResultCodes SealCanadian()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] mKeyByts;
            byte[] ProcResponse;
            byte sealState = 0x01;

            ProcParam = new byte[0];

            mKeyByts = GetMagicKey();

            ProcParam = new byte[mKeyByts.Length + 1];
            mKeyByts.CopyTo(ProcParam, 0);

            //Add Seal State to ProcParam
            ProcParam.SetValue(sealState, mKeyByts.Length);

            ProcResult = ExecuteProcedure(Procedures.SEAL_CANADIAN,
                ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Calls the Seal Canadian procedure
        /// </summary>
        /// <returns>The result of the procedure</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/12/10 RCG 2.40.36 N/A   Created
        //
        public ProcedureResultCodes UnsealCanadian()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] mKeyByts;
            byte[] ProcResponse;
            byte sealState = 0x00;

            ProcParam = new byte[0];

            mKeyByts = GetMagicKey();

            ProcParam = new byte[mKeyByts.Length + 1];
            mKeyByts.CopyTo(ProcParam, 0);

            //Add Seal State to ProcParam
            ProcParam.SetValue(sealState, mKeyByts.Length);

            ProcResult = ExecuteProcedure(Procedures.SEAL_CANADIAN,
                ProcParam, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// This method clears the billing data on a connected ANSI device.
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the reset
        /// operation.</returns>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/22/06 mcm 7.35.00  N/A    Created
        /// 11/13/13 AF  3.50.03        Class re-architecture - promoted from CENTRON_AMI
        ///
        public virtual ItronDeviceResult ClearBillingData()
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[0];  // No parameters for this procedure
            ProcResult = ExecuteProcedure(Procedures.CLEAR_DATA,
                ProcParam, out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ItronDeviceResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ItronDeviceResult.ERROR;
                        break;
                    }
            }

            return Result;

        } // End ResetDemand()

        /// <summary>
        /// This method clears the billing data on a connected ANSI device.
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the reset
        /// operation.</returns>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/22/06 mcm 7.35.00  N/A    Created
        ///
        public virtual ItronDeviceResult ClearBillingDataAndWaitForResult()
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[0];  // No parameters for this procedure
            ProcResult = ExecuteProcedureAndWaitForResult(Procedures.CLEAR_DATA,
                ProcParam, out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ItronDeviceResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ItronDeviceResult.ERROR;
                        break;
                    }
            }

            return Result;

        }

        /// <summary>
        /// This method resets the demand registers on a connected ANSI device.
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the reset
        /// operation.</returns>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/22/06 mcm 7.35.00  N/A    Created
        /// 11/13/13 AF  3.50.03        Class re-architecture - promoted from CENTRON_AMI
        ///
        public virtual ItronDeviceResult ResetDemand()
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[1];
            ProcParam[0] = (byte)RemoteResetProcedureFlags.DEMAND_RESET;
            ProcResult = ExecuteProcedure(Procedures.REMOTE_RESET,
                ProcParam, out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ItronDeviceResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ItronDeviceResult.ERROR;
                        break;
                    }
            }

            return Result;

        } // End ResetDemand()

        /// <summary>
        /// This method resets the demand registers on a connected ANSI device.
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the reset
        /// operation.</returns>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/19/13 GK 2.80.21  N/A    Created
        ///
        public ItronDeviceResult ResetDemand(out byte[] ProcResponse)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;

            ProcParam = new byte[1];
            ProcParam[0] = (byte)RemoteResetProcedureFlags.DEMAND_RESET;
            ProcResult = ExecuteProcedure(Procedures.REMOTE_RESET,
                ProcParam, out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ItronDeviceResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ItronDeviceResult.ERROR;
                        break;
                    }
            }

            return Result;

        } // End ResetDemand()

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
        // 12/04/10 DEO 9.70.13         changed method access privilege from private to public
        // 11/18/13 jrf 3.50.04 TQ 9478 Minor updates to work with 25 year TOU schedule.
        // 01/13/14 jrf 3.50.24 TQ 9478 Handling DST Calendar events that are not the first two events.
        // 04/16/14 jrf 3.50.78 WR 489749 Modified to set typical week for each season.
        // 07/16/14 jrf  4.00.47 523549 Making sure the datetime set for each event in the TOU schedule
        //                              specifies local time.
        public static CTOUSchedule ReadTOUSchedule(TOUConfig TOUConfigTable, CalendarConfig CalendarConfigTable)
        {
            Int16Collection NormalDays;
            Int16Collection HolidayDays;
            ANSITOUSchedule TOUSchedule = new ANSITOUSchedule();
            TOUConfig.TOU_Season CurrentSeason = null;
            int iNextEventCounter = 0;
            int iPatternID = 0;
            string[] TypicalWeek = null;


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
                    TypicalWeek = new string[DAYS_IN_WEEK];

                    TypicalWeek[(int)eTypicalDay.SUNDAY] = TOUSchedule.NormalDays[CurrentSeason.TypicalSunday];
                    TypicalWeek[(int)eTypicalDay.MONDAY] = TOUSchedule.NormalDays[CurrentSeason.TypicalMonday];
                    TypicalWeek[(int)eTypicalDay.TUESDAY] = TOUSchedule.NormalDays[CurrentSeason.TypicalTuesday];
                    TypicalWeek[(int)eTypicalDay.WEDNESDAY] = TOUSchedule.NormalDays[CurrentSeason.TypicalWednesday];
                    TypicalWeek[(int)eTypicalDay.THURSDAY] = TOUSchedule.NormalDays[CurrentSeason.TypicalThursday];
                    TypicalWeek[(int)eTypicalDay.FRIDAY] = TOUSchedule.NormalDays[CurrentSeason.TypicalFriday];
                    TypicalWeek[(int)eTypicalDay.SATURDAY] = TOUSchedule.NormalDays[CurrentSeason.TypicalSaturday];

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
                    CSeason SchedSeason = new CSeason(iSeasonCounter + 1, "Season " + (iSeasonCounter + 1).ToString(CultureInfo.InvariantCulture), NormalDays, HolidayDays, TypicalWeek);

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

                    for (int iDayEvent = 0;
                        iDayEvent < CalendarConfigTable.EventsPerYear; iDayEvent++)
                    {
                        eEventType eType = CalendarConfigTable.GetEventType(CalEvents[iDayEvent].Type);
                        int iEventIndex = iDayEvent;

                        if (eEventType.NO_EVENT != eType)
                        {
                            // It is a valid event
                            DateTime dtDate = new DateTime(iYear, CalEvents[iDayEvent].Month + 1,
                                                         CalEvents[iDayEvent].Day + 1, 0, 0, 0, DateTimeKind.Local);

                            // Determine the index for the event
                            if (eType == eEventType.SEASON)
                            {
                                iEventIndex = CalendarConfigTable.GetSeasonIndex(CalEvents[iDayEvent].Type);
                            }
                            else if (eType == eEventType.HOLIDAY)
                            {
                                // Determine which Holiday day type to use
                                // Currently the ANSI devices only support 1 holiday day type so this is always 0
                                iEventIndex = 0;
                            }
                            else if (eType == eEventType.FROM_DST || eType == eEventType.TO_DST)
                            {
                                dtDate = new DateTime(dtDate.Year, dtDate.Month, dtDate.Day, CalendarConfigTable.DSTHour, CalendarConfigTable.DSTMinute, 0, DateTimeKind.Local);
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

        /// <summary>
        /// Determines whether or not the TOU Event is a rate change event
        /// </summary>
        /// <param name="usEvent">The event type to check</param>
        /// <returns>True if the TOU event is a rate change event, false otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/13/07 RCG 8.00.31 2919   Created
        // 12/04/10 DEO 9.70.13         changed method access privilege from private to public

        public static bool IsRateChangeEvent(ushort usEvent)
        {
            bool bResult = false;
            ushort usEventID = (ushort)(usEvent & TOUConfig.DayEvent.EVENT_MASK);

            if (usEventID >= (ushort)TOUConfig.DayEvent.TOUEvent.RateA
                && usEventID <= (ushort)TOUConfig.DayEvent.TOUEvent.RateG)
            {
                bResult = true;
            }

            return bResult;
        }

        /// <summary>
        /// Determine whether or not the TOU event is an Output on event.
        /// </summary>
        /// <param name="usEvent">The event type to check</param>
        /// <returns>True if the event is an Ouput on event, false otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/13/07 RCG 8.00.31 2919   Created
        // 12/04/10 DEO 9.70.13         changed method access privilege from private to public

        public static bool IsOutputOnEvent(ushort usEvent)
        {
            bool bResult = false;
            ushort usEventID = (ushort)(usEvent & TOUConfig.DayEvent.EVENT_MASK);

            if (usEventID >= (ushort)TOUConfig.DayEvent.TOUEvent.Output1
                && usEventID <= (ushort)TOUConfig.DayEvent.TOUEvent.Output4)
            {
                bResult = true;
            }

            return bResult;
        }

        /// <summary>
        /// Determine whether or not the TOU event is an Output off event.
        /// </summary>
        /// <param name="usEvent">The event type to check</param>
        /// <returns>True if the event is an Ouput off event, false otherwise</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/13/07 RCG 8.00.31 2919   Created

        public static bool IsOutputOffEvent(ushort usEvent)
        {
            bool bResult = false;
            ushort usEventID = (ushort)(usEvent & TOUConfig.DayEvent.EVENT_MASK);

            if (usEventID >= (ushort)TOUConfig.DayEvent.TOUEvent.Output1Off
                && usEventID <= (ushort)TOUConfig.DayEvent.TOUEvent.Output4Off)
            {
                bResult = true;
            }

            return bResult;
        }

        /// <summary>
        /// Determintes the Rate Index given the Event
        /// </summary>
        /// <param name="usEvent"></param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/20/06 KRC 7.36.00
        // 01/04/07 RCG 8.00.04		    Promoted from CENTRON_AMI
        // 12/04/10 DEO 9.70.13         changed method access privilege from private to public

        public static int GetRateIndex(ushort usEvent)
        {
            int iIndex = -1;
            ushort usEventID = (ushort)(usEvent & TOUConfig.DayEvent.EVENT_MASK);

            switch (usEventID)
            {
                case (ushort)TOUConfig.DayEvent.TOUEvent.RateA:
                    {
                        iIndex = 0;
                        break;
                    }
                case (ushort)TOUConfig.DayEvent.TOUEvent.RateB:
                    {
                        iIndex = 1;
                        break;
                    }
                case (ushort)TOUConfig.DayEvent.TOUEvent.RateC:
                    {
                        iIndex = 2;
                        break;
                    }
                case (ushort)TOUConfig.DayEvent.TOUEvent.RateD:
                    {
                        iIndex = 3;
                        break;
                    }
                case (ushort)TOUConfig.DayEvent.TOUEvent.RateE:
                    {
                        iIndex = 4;
                        break;
                    }
                case (ushort)TOUConfig.DayEvent.TOUEvent.RateF:
                    {
                        iIndex = 5;
                        break;
                    }
                case (ushort)TOUConfig.DayEvent.TOUEvent.RateG:
                    {
                        iIndex = 6;
                        break;
                    }
            }

            return iIndex;
        }

        /// <summary>
        /// Gets the Ouput index for the specified event
        /// </summary>
        /// <param name="usEvent">The event to check.</param>
        /// <returns>The index of the event.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/13/07 RCG 8.00.31 2919   Created
        // 12/04/10 DEO 9.70.13         changed method access privilege from private to public

        public static int GetOutputIndex(ushort usEvent)
        {
            int iIndex = -1;
            ushort usEventID = (ushort)(usEvent & TOUConfig.DayEvent.EVENT_MASK);

            switch (usEventID)
            {
                case (ushort)TOUConfig.DayEvent.TOUEvent.Output1:
                case (ushort)TOUConfig.DayEvent.TOUEvent.Output1Off:
                    {
                        iIndex = 0;
                        break;
                    }
                case (ushort)TOUConfig.DayEvent.TOUEvent.Output2:
                case (ushort)TOUConfig.DayEvent.TOUEvent.Output2Off:
                    {
                        iIndex = 1;
                        break;
                    }
                case (ushort)TOUConfig.DayEvent.TOUEvent.Output3:
                case (ushort)TOUConfig.DayEvent.TOUEvent.Output3Off:
                    {
                        iIndex = 2;
                        break;
                    }
                case (ushort)TOUConfig.DayEvent.TOUEvent.Output4:
                case (ushort)TOUConfig.DayEvent.TOUEvent.Output4Off:
                    {
                        iIndex = 3;
                        break;
                    }
            }

            return iIndex;
        }

        /// <summary>
        /// Change the Display Mode
        /// </summary>
        /// <returns>ItronDeviceResult.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/11/06 KRC 7.35.00 N/A    Created
        // 10/13/06 AF  7.40.00 N/A    Removed flush of m_NumOutages
        // 04/09/07 KRC 8.00.26 2831   Moved to ANSIDevice from AMIDevice
        // 11/13/13 AF  3.50.03        Class re-architecture - promoted from CENTRON_AMI
        //
        public virtual ItronDeviceResult ChangeDisplayMode(DisplayMode eMode)
        {
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            byte[] ProcResponse;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            DisplayType eSwitchType = DisplayType.NORMAL_MODE;
            DisplayMode eCurrentMode;

            //First we need to figure out the correct command based on what the user
            // wants to switch to.
            eCurrentMode = MeterDisplayMode;
            if (eMode == DisplayMode.NORMAL_MODE)
            {
                // User wants to go to Normal Mode
                if (eCurrentMode == DisplayMode.TEST_ALT_MODE ||
                   eCurrentMode == DisplayMode.TEST_MODE)
                {
                    //If we are currently in test mode we need to issue
                    //	the exit test mode commnad
                    eSwitchType = DisplayType.EXIT_TEST_TO_NORMAL;
                }
                else
                {
                    // Just call the switch to Normal Mode Command
                    eSwitchType = DisplayType.NORMAL_MODE;
                }
            }
            else if (eMode == DisplayMode.TEST_MODE)
            {
                eSwitchType = DisplayType.TEST_DISPLAY;
            }
            else if (eMode == DisplayMode.ALT_MODE)
            {
                eSwitchType = DisplayType.ALT_MODE;
            }
            else if (eMode == DisplayMode.TEST_ALT_MODE)
            {
                eSwitchType = DisplayType.TEST_MODE_TO_TEST_ALT;
            }

            //Execute the reset conters MFG procedure 
            byte[] byParameter = BitConverter.GetBytes((uint)eSwitchType);
            ProcResult = ExecuteProcedure(Procedures.CHANGE_DISPLAY_MODE, byParameter, out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ItronDeviceResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ItronDeviceResult.ERROR;
                        break;
                    }
            }


            return Result;
        }

        /// <summary>
        /// Clears the Self Reads stored in the meter
        /// </summary>
        /// <returns>The Result of the clear</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/21/10 RCG 2.42.06 N/A    Created
        //  10/11/10 RCG 2.45.03 160144 Fixing parameter length
        public virtual ItronDeviceResult ClearSelfReads()
        {
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            byte[] ProcResponse;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            //Execute the standard procedure 04 - reset list pointers procedure
            byte[] byParameter = new byte[] { (byte)ResetListPointerTypes.SelfReads };
            ProcResult = ExecuteProcedure(Procedures.RESET_LIST_PTRS, byParameter, out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ItronDeviceResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ItronDeviceResult.ERROR;
                        break;
                    }
            }

            return Result;
        }

        /// <summary>
        /// Updates DST in the connected meter. This method does not reconfigure
        /// DST. Only future dates in 2007 and beyond are updated.
        /// </summary>
        /// <param name="FileName">The filename including path for the DST file</param>
        /// <returns>A DSTUpdateResult</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/16/06 mcm 7.30.00 N/A    Created
        //	01/24/07 mrj 8.00.08		Flushed status flags when updating dst
        //  11/13/13 AF  3.50.03        Class re-architecture - promoted from CENTRON_AMI
        // 	
        public virtual DSTUpdateResult UpdateDST(string FileName)
        {
            DSTUpdateResult Result = DSTUpdateResult.ERROR;
            ProcedureResultCodes ProcResult =
                ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;


            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Functional,
                                   "Starting DST Update");

                // Check to see if the clock is running and if the meter is
                // configured for DST. This feature does not add DST to a meter
                // that doesn't have it. It only updates the DST dates beyond
                // the start of the Energy Act of 2005, which is 2007.
                Result = ValidateMeterForDSTUpdate();

                // If the meter supports it, update the dates in the 
                // configuration.
                if (DSTUpdateResult.SUCCESS == Result)
                {
                    Result = UpdateDSTDates(FileName);
                }

                // If we updated the dates, start the reconfiguration.  NOTE
                // that one reason the update of dates could have failed is
                // because the meter was already updated.
                if (DSTUpdateResult.SUCCESS == Result)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                        "Starting reconfiguration");

                    // Prepare the meter for reconfiguration
                    ProcParam = new byte[0];
                    ProcResult = ExecuteProcedure(Procedures.OPEN_CONFIG_FILE,
                        ProcParam, out ProcResponse);

                    // Write the CalendarConfig table to the meter.
                    if (ProcedureResultCodes.COMPLETED == ProcResult)
                    {
                        Result = GetDSTResult(Table2048.CalendarConfig.Write());
                    }
                    else if (ProcedureResultCodes.NO_AUTHORIZATION == ProcResult)
                    {
                        Result = DSTUpdateResult.INSUFFICIENT_SECURITY_ERROR;
                    }
                    else
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                            "Open config procedure failed with result = " +
                            ProcResult);
                        Result = DSTUpdateResult.ERROR;
                    }
                }

                // If everything has gone well, tell the meter we're done. 
                if (DSTUpdateResult.SUCCESS == Result)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                            "Finishing reconfiguration");

                    // Data reset bits - we don't want to reset any data, so 
                    // just initialize them to 0
                    ProcParam = new byte[4];
                    ProcParam.Initialize();

                    ProcResult = ExecuteProcedure(Procedures.CLOSE_CONFIG_FILE,
                        ProcParam, out ProcResponse);

                    // The moment of truth... did it work?
                    if (ProcedureResultCodes.COMPLETED != ProcResult)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "Close config procedure failed with result = "
                                + ProcResult);
                        Result = DSTUpdateResult.ERROR;
                    }
                }
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

            //Flush the in DST flag
            m_MeterInDST.Flush();

            m_lstDSTDates.Clear(); // Also flush the previously cached DST dates
            m_TOUSchedule = null;

            return Result;

        } // UpdateDST

        /// <summary>
        /// Resets the count of demand resets
        /// </summary>
        /// <returns>ItronDeviceResult</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/12/06 AF  7.40.00 N/A    Created
        //  01/29/07 MAH 8.00.09	    Promoted method to Itron Device class
        //  11/13/13 AF  3.50.03        Class re-architecture - promoted from CENTRON_AMI
        // 
        public virtual ItronDeviceResult ResetNumberDemandResets()
        {
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            byte[] ProcResponse;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            //Execute the reset conters MFG procedure 
            byte[] byParameter = BitConverter.GetBytes((uint)Reset_Counter_Types.RESET_NUM_DEMAND_RESETS);
            ProcResult = ExecuteProcedure(Procedures.RESET_COUNTERS, byParameter, out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ItronDeviceResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ItronDeviceResult.ERROR;
                        break;
                    }
            }

            return Result;
        }

        /// <summary>
        /// Resets the Number of Power Outages
        /// </summary>
        /// <returns>ItronDeviceResult.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/11/06 KRC 7.35.00 N/A    Created
        //  01/29/07 MAH 8.00.09	       Promoted method to Itron Device class
        //  11/13/13 AF  3.50.03        Class re-architecture - promoted from CENTRON_AMI
        // 
        public virtual ItronDeviceResult ResetNumberPowerOutages()
        {
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            byte[] ProcResponse;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;


            //Execute the reset conters MFG procedure 
            byte[] byParameter = BitConverter.GetBytes((uint)Reset_Counter_Types.RESET_NUM_POWER_OUTAGES);
            ProcResult = ExecuteProcedure(Procedures.RESET_COUNTERS, byParameter, out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ItronDeviceResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ItronDeviceResult.ERROR;
                        break;
                    }
            }

            // Reset the cached value so it will be read from the meter when
            //	requested again.
            m_NumOutages.Flush();

            return Result;
        }

        /// <summary>
        /// Resets the Number of Times Programmed
        /// </summary>
        /// <returns>ItronDeviceResult.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/11/06 KRC 7.35.00 N/A    Created
        //  01/29/07 MAH 8.00.09	    Promoted method to Itron Device class
        //  11/13/13 AF  3.50.03        Class re-architecture - promoted from CENTRON_AMI
        // 
        public virtual ItronDeviceResult ResetNumberTimesProgrammed()
        {
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            byte[] ProcResponse;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;


            //Execute the reset conters MFG procedure 
            byte[] byParameter = BitConverter.GetBytes((uint)Reset_Counter_Types.RESET_NUM_TIMES_PROGRAMMED);
            ProcResult = ExecuteProcedure(Procedures.RESET_COUNTERS, byParameter, out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        Result = ItronDeviceResult.SUCCESS;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;
                    }
                default:
                    {
                        //General Error
                        Result = ItronDeviceResult.ERROR;
                        break;
                    }
            }

            // Clear the cached value so it will be read from the meter the next
            //	time it is requested.
            m_NumTimesProgrammed.Flush();

            return Result;
        }

        /// <summary>
        /// FOR TESTING ONLY! Gets the list of Events from Table 74. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  12/04/13 DLG 3.50.10 WR442624 Created to provide a way to only retrieve table 74 events
        //                                and not be overriden. Only to be used in testing!
        public List<HistoryEntry> Table74Events
        {
            get
            {
                return Table74.HistoryLogEntries;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Proves access to a list of Energy Quantities
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/03/06 KRC 7.35.00 N/A    Created
        //  11/30/06 jrf 8.00.00 N/A    Added check for additional quantities
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
        public virtual List<Quantity> CurrentRegisters
        {
            get
            {
                List<Quantity> QuantityList = new List<Quantity>();
                Quantity Qty;

                // Add Watts Del
                Qty = WattsDelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Watts Rec
                Qty = WattsReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Watts Net
                Qty = WattsNet;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Watts Uni
                Qty = WattsUni;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add VA Del
                Qty = VADelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add VA Rec
                Qty = VAReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add VA Lag
                Qty = VALagging;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Del
                Qty = VarDelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Rec
                Qty = VarReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Net
                Qty = VarNet;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Net Del
                Qty = VarNetDelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Net Rec
                Qty = VarNetReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Q1
                Qty = VarQuadrant1;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Q2
                Qty = VarQuadrant2;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Q3
                Qty = VarQuadrant3;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Var Q4
                Qty = VarQuadrant4;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A (a)
                Qty = AmpsPhaseA;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A (b)
                Qty = AmpsPhaseB;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A (c)
                Qty = AmpsPhaseC;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A Neutral
                Qty = AmpsNeutral;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add A^2
                Qty = AmpsSquared;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V (a)
                Qty = VoltsPhaseA;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V (b)
                Qty = VoltsPhaseB;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V (c)
                Qty = VoltsPhaseC;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V Avg
                Qty = VoltsAverage;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add V^2
                Qty = VoltsSquared;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add PF
                Qty = PowerFactor;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Q Del
                Qty = QDelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }
                // Add Q Rec
                Qty = QReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }

                // Add the coincident values
                QuantityList.AddRange(CoincidentValues);

                return QuantityList;
            }
        }


        /// <summary>
        /// Provides access to the Amps Phase A Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get A (a)
        //  11/12/13 AF  3.50.03        Class re-architecture - replaced override
        //
        public virtual Quantity AmpsPhaseA
        {
            get
            {
                ANSIQuantity APhA = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_AH_PHA)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_A_PHA)))
                {
                    APhA = new ANSIQuantity("Amps (a)", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    APhA.TotalEnergyLID = m_LID.ENERGY_AH_PHA;
                    APhA.TotalMaxDemandLID = m_LID.DEMAND_MAX_A_PHA;
                    APhA.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_AH_PHA.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    APhA.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_A_PHA.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return APhA;
            }
        }

        /// <summary>
        /// Provides access to the Amps Phase B Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get A (b)
        //  11/12/13 AF  3.50.03        Class re-architecture - replaced override
        //
        public virtual Quantity AmpsPhaseB
        {
            get
            {
                ANSIQuantity APhB = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_AH_PHB)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_A_PHB)))
                {
                    APhB = new ANSIQuantity("Amps (b)", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    APhB.TotalEnergyLID = m_LID.ENERGY_AH_PHB;
                    APhB.TotalMaxDemandLID = m_LID.DEMAND_MAX_A_PHB;
                    APhB.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_AH_PHB.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    APhB.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_A_PHB.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return APhB;
            }
        }

        /// <summary>
        /// Provides access to the Amps Phase C Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get A (c)
        //  11/12/13 AF  3.50.03        Class re-architecture - replaced override
        //
        public virtual Quantity AmpsPhaseC
        {
            get
            {
                ANSIQuantity APhC = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_AH_PHC)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_A_PHC)))
                {
                    APhC = new ANSIQuantity("Amps (c)", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    APhC.TotalEnergyLID = m_LID.ENERGY_AH_PHC;
                    APhC.TotalMaxDemandLID = m_LID.DEMAND_MAX_A_PHC;
                    APhC.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_AH_PHC.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    APhC.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_A_PHC.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return APhC;
            }
        }

        /// <summary>
        /// Provides access to the Neutral Amps Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get Neutral Amps
        //
        public virtual Quantity AmpsNeutral
        {
            get
            {
                ANSIQuantity ANeut = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_AH_NEUTRAL)) ||
                        (true == ValidateDemand(m_LID.DEMAND_MAX_A_NEUTRAL)))
                {
                    ANeut = new ANSIQuantity("Neutral Amps", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    ANeut.TotalEnergyLID = m_LID.ENERGY_AH_NEUTRAL;
                    ANeut.TotalMaxDemandLID = m_LID.DEMAND_MAX_A_NEUTRAL;
                    ANeut.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_AH_NEUTRAL.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    ANeut.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_A_NEUTRAL.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return ANeut;
            }
        }

        /// <summary>
        /// Provides access to the Amps Squared Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get A^2
        //
        public virtual Quantity AmpsSquared
        {
            get
            {
                ANSIQuantity ASq = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_I2H_AGG)) ||
                        (true == ValidateDemand(m_LID.DEMAND_MAX_I2_AGG)))
                {
                    ASq = new ANSIQuantity("Amps Squared", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    ASq.TotalEnergyLID = m_LID.ENERGY_I2H_AGG;
                    ASq.TotalMaxDemandLID = m_LID.DEMAND_MAX_I2_AGG;
                    ASq.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_I2H_AGG.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    ASq.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_I2_AGG.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return ASq;
            }
        }

        /// <summary>
        /// Provides access to the Volts Phase A Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get V (a)
        //  11/12/13 AF  3.50.03        Class re-architecture - made virtual
        //
        public virtual Quantity VoltsPhaseA
        {
            get
            {
                ANSIQuantity VPhA = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_VH_PHA)) ||
                        (true == ValidateDemand(m_LID.DEMAND_MAX_V_PHA)))
                {
                    VPhA = new ANSIQuantity("Volts (a)", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VPhA.TotalEnergyLID = m_LID.ENERGY_VH_PHA;
                    VPhA.TotalMaxDemandLID = m_LID.DEMAND_MAX_V_PHA;
                    VPhA.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VH_PHA.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VPhA.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_V_PHA.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VPhA;
            }
        }

        /// <summary>
        /// Provides access to the Volts Phase B Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get V (b)
        //  11/12/13 AF  3.50.03        Class re-architecture - made virtual
        //
        public virtual Quantity VoltsPhaseB
        {
            get
            {
                ANSIQuantity VPhB = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_VH_PHB)) ||
                        (true == ValidateDemand(m_LID.DEMAND_MAX_V_PHB)))
                {
                    VPhB = new ANSIQuantity("Volts (b)", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VPhB.TotalEnergyLID = m_LID.ENERGY_VH_PHB;
                    VPhB.TotalMaxDemandLID = m_LID.DEMAND_MAX_V_PHB;
                    VPhB.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VH_PHB.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VPhB.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_V_PHB.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VPhB;
            }
        }

        /// <summary>
        /// Provides access to the Volts Phase C Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get V (c)
        //  11/12/13 AF  3.50.03        Class re-architecture - made virtual
        //
        public virtual Quantity VoltsPhaseC
        {
            get
            {
                ANSIQuantity VPhC = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_VH_PHC)) ||
                        (true == ValidateDemand(m_LID.DEMAND_MAX_V_PHC)))
                {
                    VPhC = new ANSIQuantity("Volts (c)", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VPhC.TotalEnergyLID = m_LID.ENERGY_VH_PHC;
                    VPhC.TotalMaxDemandLID = m_LID.DEMAND_MAX_V_PHC;
                    VPhC.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VH_PHC.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VPhC.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_V_PHC.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VPhC;
            }
        }

        /// <summary>
        /// Provides access to the Volts Average Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get V avg
        //
        public virtual Quantity VoltsAverage
        {
            get
            {
                ANSIQuantity VAvg = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_VH_AVG)) ||
                        (true == ValidateDemand(m_LID.DEMAND_MAX_V_AVG)))
                {
                    VAvg = new ANSIQuantity("Volts Average", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VAvg.TotalEnergyLID = m_LID.ENERGY_VH_AVG;
                    VAvg.TotalMaxDemandLID = m_LID.DEMAND_MAX_V_AVG;
                    VAvg.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VH_AVG.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VAvg.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_V_AVG.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VAvg;
            }
        }

        /// <summary>
        /// Provides access to the Volts Squared Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get V^2
        //
        public virtual Quantity VoltsSquared
        {
            get
            {
                ANSIQuantity VSq = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_V2H_AGG)) ||
                        (true == ValidateDemand(m_LID.DEMAND_MAX_V2_AGG)))
                {
                    VSq = new ANSIQuantity("Volts Squared", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VSq.TotalEnergyLID = m_LID.ENERGY_V2H_AGG;
                    VSq.TotalMaxDemandLID = m_LID.DEMAND_MAX_V2_AGG;
                    VSq.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_V2H_AGG.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VSq.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_V2_AGG.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VSq;
            }
        }

        /// <summary>
        /// Provides access to the Watts Delivered Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get Watts Del
        //  11/12/13 AF  3.50.03        Class re-architecture - removed override
        //
        public virtual Quantity WattsDelivered
        {
            get
            {
                ANSIQuantity WattsDel = null;
                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_WH_DEL)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_W_DEL)))
                {
                    WattsDel = new ANSIQuantity("Watts Delivered", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    WattsDel.TotalEnergyLID = m_LID.ENERGY_WH_DEL;
                    WattsDel.TotalMaxDemandLID = m_LID.DEMAND_MAX_W_DEL;
                    WattsDel.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_WH_DEL.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    WattsDel.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_W_DEL.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return WattsDel;
            }
        }

        /// <summary>
        /// Provides access to the Watts Received Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get Watts Rec
        //  11/12/13 AF  3.50.03        Class re-architecture - removed override
        //
        public virtual Quantity WattsReceived
        {
            get
            {
                ANSIQuantity WattsRec = null;
                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_WH_REC)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_W_REC)))
                {
                    WattsRec = new ANSIQuantity("Watts Received", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    WattsRec.TotalEnergyLID = m_LID.ENERGY_WH_REC;
                    WattsRec.TotalMaxDemandLID = m_LID.DEMAND_MAX_W_REC;
                    WattsRec.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_WH_REC.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    WattsRec.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_W_REC.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return WattsRec;
            }
        }


        /// <summary>
        /// Provides access to the Watts Net Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get Watts Net 
        //
        public virtual Quantity WattsNet
        {
            get
            {
                ANSIQuantity WattsNet = null;
                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_WH_NET)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_W_NET)))
                {
                    WattsNet = new ANSIQuantity("Watts Net", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    WattsNet.TotalEnergyLID = m_LID.ENERGY_WH_NET;
                    WattsNet.TotalMaxDemandLID = m_LID.DEMAND_MAX_W_NET;
                    WattsNet.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_WH_NET.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    WattsNet.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_W_NET.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }
                return WattsNet;
            }
        }

        /// <summary>
        /// Provides access to the Watts Uni Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get Watts Uni
        //
        public virtual Quantity WattsUni
        {
            get
            {
                ANSIQuantity WattsUniDir = null;
                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_WH_UNI)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_W_UNI)))
                {
                    WattsUniDir = new ANSIQuantity("Unidirectional Watts", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    WattsUniDir.TotalEnergyLID = m_LID.ENERGY_WH_UNI;
                    WattsUniDir.TotalMaxDemandLID = m_LID.DEMAND_MAX_W_UNI;
                    WattsUniDir.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_WH_UNI.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    WattsUniDir.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_W_UNI.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }
                return WattsUniDir;
            }
        }

        /// <summary>
        /// Provides access to the VA Delivered Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get VA Del
        //  11/30/06 jrf 8.00.00 N/A    Adding support for arithmetic and vectorial
        //                              calculated VA
        //  11/12/13 AF  3.50.03        Class re-architecture - removed override
        //
        public virtual Quantity VADelivered
        {
            get
            {
                ANSIQuantity VADel = null;

                // Make sure this quantity is supported before constructing it.
                // Check for Arithmetic value
                if ((true == ValidateEnergy(m_LID.ENERGY_VAH_DEL_ARITH)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_VA_DEL_ARITH)))
                {
                    VADel = new ANSIQuantity("VA Delivered", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VADel.TotalEnergyLID = m_LID.ENERGY_VAH_DEL_ARITH;
                    VADel.TotalMaxDemandLID = m_LID.DEMAND_MAX_VA_DEL_ARITH;
                    VADel.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VAH_DEL_ARITH.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VADel.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_VA_DEL_ARITH.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }
                // Check for Vectorial value
                else if ((true == ValidateEnergy(m_LID.ENERGY_VAH_DEL_VECT)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_VA_DEL_VECT)))
                {
                    VADel = new ANSIQuantity("VA Delivered", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VADel.TotalEnergyLID = m_LID.ENERGY_VAH_DEL_VECT;
                    VADel.TotalMaxDemandLID = m_LID.DEMAND_MAX_VA_DEL_VECT;
                    VADel.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VAH_DEL_VECT.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VADel.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_VA_DEL_VECT.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VADel;
            }
        }

        /// <summary>
        /// Provides access to the VA Received Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get VA Rec
        //  11/30/06 jrf 8.00.00 N/A    Adding support for arithmetic and vectorial
        //                              calculated VA
        // 11/12/13 AF  3.50.03        Class re-architecture - removed override
        //
        public virtual Quantity VAReceived
        {
            get
            {
                ANSIQuantity VARec = null;

                // Make sure this quantity is supported before constructing it.
                // Check for Arithmetic value
                if ((true == ValidateEnergy(m_LID.ENERGY_VAH_REC_ARITH)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_VA_REC_ARITH)))
                {
                    VARec = new ANSIQuantity("VA Received", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VARec.TotalEnergyLID = m_LID.ENERGY_VAH_REC_ARITH;
                    VARec.TotalMaxDemandLID = m_LID.DEMAND_MAX_VA_REC_ARITH;
                    VARec.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VAH_REC_ARITH.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VARec.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_VA_REC_ARITH.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }
                // Check for Vectorial value
                else if ((true == ValidateEnergy(m_LID.ENERGY_VAH_REC_VECT)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_VA_REC_VECT)))
                {
                    VARec = new ANSIQuantity("VA Received", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VARec.TotalEnergyLID = m_LID.ENERGY_VAH_REC_VECT;
                    VARec.TotalMaxDemandLID = m_LID.DEMAND_MAX_VA_REC_VECT;
                    VARec.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VAH_REC_VECT.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VARec.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_VA_REC_VECT.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VARec;
            }
        }

        /// <summary>
        /// Provides access to the VA Lagging
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get VA Lag
        //
        public virtual Quantity VALagging
        {
            get
            {
                ANSIQuantity VALag = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_VAH_LAG)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_VA_LAG)))
                {
                    VALag = new ANSIQuantity("VA Lagging", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VALag.TotalEnergyLID = m_LID.ENERGY_VAH_LAG;
                    VALag.TotalMaxDemandLID = m_LID.DEMAND_MAX_VA_LAG;
                    VALag.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VAH_LAG.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VALag.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_VA_LAG.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VALag;
            }
        }

        /// <summary>
        /// Provides access to the Var Delivered Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get Var Del
        //  11/12/13 AF  3.50.03        Class re-architecture - removed override
        //
        public virtual Quantity VarDelivered
        {
            get
            {
                ANSIQuantity VarDel = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_VARH_DEL)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_VAR_DEL)))
                {
                    VarDel = new ANSIQuantity("VAR Delivered", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VarDel.TotalEnergyLID = m_LID.ENERGY_VARH_DEL;
                    VarDel.TotalMaxDemandLID = m_LID.DEMAND_MAX_VAR_DEL;
                    VarDel.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VARH_DEL.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VarDel.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_VAR_DEL.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VarDel;
            }
        }

        /// <summary>
        /// Provides access to the Var Received Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get Var Rec
        //  11/12/13 AF  3.50.03        Class re-architecture - removed override
        //
        public virtual Quantity VarReceived
        {
            get
            {
                ANSIQuantity VarRec = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_VARH_REC)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_VAR_REC)))
                {
                    VarRec = new ANSIQuantity("VAR Received", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VarRec.TotalEnergyLID = m_LID.ENERGY_VARH_REC;
                    VarRec.TotalMaxDemandLID = m_LID.DEMAND_MAX_VAR_REC;
                    VarRec.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VARH_REC.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VarRec.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_VAR_REC.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VarRec;
            }
        }

        /// <summary>
        /// Provides access to the Var Net Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get Var Net
        //
        public virtual Quantity VarNet
        {
            get
            {
                ANSIQuantity VarNetQty = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_VARH_NET)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_VAR_NET)))
                {
                    VarNetQty = new ANSIQuantity("VAR Net", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VarNetQty.TotalEnergyLID = m_LID.ENERGY_VARH_NET;
                    VarNetQty.TotalMaxDemandLID = m_LID.DEMAND_MAX_VAR_NET;
                    VarNetQty.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VARH_NET.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VarNetQty.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_VAR_NET.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VarNetQty;
            }
        }

        /// <summary>
        /// Provides access to the Var Net Delivered Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get Var Net Del
        //
        public virtual Quantity VarNetDelivered
        {
            get
            {
                ANSIQuantity VarNetDel = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_VARH_NET_DEL)) ||
                        (true == ValidateDemand(m_LID.DEMAND_MAX_VAR_NET_DEL)))
                {
                    VarNetDel = new ANSIQuantity("VAR Net Delivered", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VarNetDel.TotalEnergyLID = m_LID.ENERGY_VARH_NET_DEL;
                    VarNetDel.TotalMaxDemandLID = m_LID.DEMAND_MAX_VAR_NET_DEL;
                    VarNetDel.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VARH_NET_DEL.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VarNetDel.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_VAR_NET_DEL.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VarNetDel;
            }
        }

        /// <summary>
        /// Provides access to the Var Net Received Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get Var Net Rec
        //
        public virtual Quantity VarNetReceived
        {
            get
            {
                ANSIQuantity VarNetRec = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_VARH_NET_REC)) ||
                        (true == ValidateDemand(m_LID.DEMAND_MAX_VAR_NET_REC)))
                {
                    VarNetRec = new ANSIQuantity("VAR Net Received", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VarNetRec.TotalEnergyLID = m_LID.ENERGY_VARH_NET_REC;
                    VarNetRec.TotalMaxDemandLID = m_LID.DEMAND_MAX_VAR_NET_REC;
                    VarNetRec.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VARH_NET_REC.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VarNetRec.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_VAR_NET_REC.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VarNetRec;
            }
        }

        /// <summary>
        /// Provides access to the Var Quadrant 1 Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get Var Q1
        //
        public virtual Quantity VarQuadrant1
        {
            get
            {
                ANSIQuantity VarQ1 = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_VARH_Q1)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_VAR_Q1)))
                {
                    VarQ1 = new ANSIQuantity("VAR Quadrant 1", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VarQ1.TotalEnergyLID = m_LID.ENERGY_VARH_Q1;
                    VarQ1.TotalMaxDemandLID = m_LID.DEMAND_MAX_VAR_Q1;
                    VarQ1.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VARH_Q1.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VarQ1.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_VAR_Q1.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VarQ1;
            }
        }

        /// <summary>
        /// Provides access to the Var Quadrant 2 Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get Var Q2
        //
        public virtual Quantity VarQuadrant2
        {
            get
            {
                ANSIQuantity VarQ2 = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_VARH_Q2)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_VAR_Q2)))
                {
                    VarQ2 = new ANSIQuantity("VAR Quadrant 2", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VarQ2.TotalEnergyLID = m_LID.ENERGY_VARH_Q2;
                    VarQ2.TotalMaxDemandLID = m_LID.DEMAND_MAX_VAR_Q2;
                    VarQ2.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VARH_Q2.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VarQ2.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_VAR_Q2.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VarQ2;
            }
        }

        /// <summary>
        /// Provides access to the Var Quadrant 3 Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get Var Q3
        //
        public virtual Quantity VarQuadrant3
        {
            get
            {
                ANSIQuantity VarQ3 = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_VARH_Q3)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_VAR_Q3)))
                {
                    VarQ3 = new ANSIQuantity("VAR Quadrant 3", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VarQ3.TotalEnergyLID = m_LID.ENERGY_VARH_Q3;
                    VarQ3.TotalMaxDemandLID = m_LID.DEMAND_MAX_VAR_Q3;
                    VarQ3.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VARH_Q3.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VarQ3.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_VAR_Q3.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VarQ3;
            }
        }

        /// <summary>
        /// Provides access to the Var Quadrant 4 Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get Var Q4
        //
        public virtual Quantity VarQuadrant4
        {
            get
            {
                ANSIQuantity VarQ4 = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_VARH_Q4)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_VAR_Q4)))
                {
                    VarQ4 = new ANSIQuantity("VAR Quadrant 4", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    VarQ4.TotalEnergyLID = m_LID.ENERGY_VARH_Q4;
                    VarQ4.TotalMaxDemandLID = m_LID.DEMAND_MAX_VAR_Q4;
                    VarQ4.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_VARH_Q4.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    VarQ4.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_VAR_Q4.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return VarQ4;
            }
        }

        /// <summary>
        /// Provides access to the Power Factor Quantity
        /// </summary>
        /// <remarks>This quantity is demand only</remarks>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get PF
        //  11/12/13 AF  3.50.03        Class re-architecture - removed override
        //
        public virtual Quantity PowerFactor
        {
            get
            {
                ANSIQuantity PF = null;

                // Make sure this quantity is supported before constructing it.
                // Check for Arithmetic value
                if (true == ValidateDemand(m_LID.DEMAND_MIN_PF_INTERVAL_ARITH))
                {
                    PF = new ANSIQuantity("Power Factor", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    PF.TotalMaxDemandLID = m_LID.DEMAND_MIN_PF_INTERVAL_ARITH;
                    PF.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MIN_PF_INTERVAL_ARITH.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }
                // Check for Vectorial value
                else if (true == ValidateDemand(m_LID.DEMAND_MIN_PF_INTERVAL_VECT))
                {
                    PF = new ANSIQuantity("Power Factor", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    PF.TotalMaxDemandLID = m_LID.DEMAND_MIN_PF_INTERVAL_VECT;
                    PF.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MIN_PF_INTERVAL_VECT.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return PF;
            }
        }

        /// <summary>
        /// Provides access to the Q Delivered Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get Q del
        //
        public virtual Quantity QDelivered
        {
            get
            {
                ANSIQuantity QDel = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_QH_DEL)) ||
                        (true == ValidateDemand(m_LID.DEMAND_MAX_Q_DEL)))
                {
                    QDel = new ANSIQuantity("Q Delivered", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    QDel.TotalEnergyLID = m_LID.ENERGY_QH_DEL;
                    QDel.TotalMaxDemandLID = m_LID.DEMAND_MAX_Q_DEL;
                    QDel.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_QH_DEL.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    QDel.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_Q_DEL.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return QDel;
            }
        }

        /// <summary>
        /// Provides access to the Q Received Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/30/06 jrf 8.00.00 N/A    Adding support to get Q rec
        //
        public virtual Quantity QReceived
        {
            get
            {
                ANSIQuantity QRec = null;

                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_QH_REC)) ||
                        (true == ValidateDemand(m_LID.DEMAND_MAX_Q_REC)))
                {
                    QRec = new ANSIQuantity("Q Received", m_PSEM, this);
                    // Set the LID Properties so the Quantity know what type he is
                    QRec.TotalEnergyLID = m_LID.ENERGY_QH_REC;
                    QRec.TotalMaxDemandLID = m_LID.DEMAND_MAX_Q_REC;
                    QRec.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_QH_REC.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    QRec.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_Q_REC.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return QRec;
            }
        }

        /// <summary>
        /// Returns whether or not the Wh rec quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_WhRecSupported
        {
            get
            {
                return MeterKeyTable.WhRecSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh Q1 quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VarhQ1Supported
        {
            get
            {
                return MeterKeyTable.VarhQ1Supported;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh Q2 quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VarhQ2Supported
        {
            get
            {
                return MeterKeyTable.VarhQ2Supported;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh Q3 quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VarhQ3Supported
        {
            get
            {
                return MeterKeyTable.VarhQ3Supported;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh Q4 quantity quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VarhQ4Supported
        {
            get
            {
                return MeterKeyTable.VarhQ4Supported;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh net del quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VarhNetDelSupported
        {
            get
            {
                return MeterKeyTable.VarhNetDelSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh net rec quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VarhNetRecSupported
        {
            get
            {
                return MeterKeyTable.VarhNetRecSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the VAh del arithmetic quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VAhDelArithSupported
        {
            get
            {
                return MeterKeyTable.VAhDelArithSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the VAh rec arithmetic quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VAhRecArithSupported
        {
            get
            {
                return MeterKeyTable.VAhRecArithSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the VAh del vectorial quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VAhDelVecSupported
        {
            get
            {
                return MeterKeyTable.VAhDelVecSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the VAh rec vectorial quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VAhRecVecSupported
        {
            get
            {
                return MeterKeyTable.VAhRecVecSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the VAh lag quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VAhLagSupported
        {
            get
            {
                return MeterKeyTable.VAhLagSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Qh del quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_QhDelSupported
        {
            get
            {
                return MeterKeyTable.QhDelSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Vh Phase A quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VhASupported
        {
            get
            {
                return MeterKeyTable.VhASupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Vh Phase B quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VhBSupported
        {
            get
            {
                return MeterKeyTable.VhBSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Vh Phase C quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VhCSupported
        {
            get
            {
                return MeterKeyTable.VhCSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Vh Phase Average quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VhPhaseAverageSupported
        {
            get
            {
                return MeterKeyTable.VhPhaseAverageSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Ah Phase A quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_AhPhaseASupported
        {
            get
            {
                return MeterKeyTable.AhPhaseASupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Ah Phase B quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_AhPhaseBSupported
        {
            get
            {
                return MeterKeyTable.AhPhaseBSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Ah Phase C quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_AhPhaseCSupported
        {
            get
            {
                return MeterKeyTable.AhPhaseCSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Ah Neutral quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_AhNeutralSupported
        {
            get
            {
                return MeterKeyTable.AhNeutralSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the V Squared h Aggregate quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_V2hAggregateSupported
        {
            get
            {
                return MeterKeyTable.V2hAggregateSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the I Squared h Aggregate quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_I2hAggregateSupported
        {
            get
            {
                return MeterKeyTable.I2hAggregateSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh del quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VarhDelSupported
        {
            get
            {
                return MeterKeyTable.VarhDelSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh rec quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VarhRecSupported
        {
            get
            {
                return MeterKeyTable.VarhRecSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Wh net quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_WhNetSupported
        {
            get
            {
                return MeterKeyTable.WhNetSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh net quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_VarhNetSupported
        {
            get
            {
                return MeterKeyTable.VarhNetSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Qh rec quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_QhRecSupported
        {
            get
            {
                return MeterKeyTable.QhRecSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the Wh unidirectional quantity is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_WhUnidirectionalSupported
        {
            get
            {
                return MeterKeyTable.WhUnidirectionalSupported;
            }
        }

        /// <summary>
        /// Returns the maximum number of peaks allowed.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.00 n/a	Created
        //  
        public UInt16 MeterKey_MaxPeaks
        {
            get
            {
                return MeterKeyTable.MaxPeaks;
            }
        }

        /// <summary>
        /// Returns whether or not coincident demand is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_CoincidentDemandSupported
        {
            get
            {
                return MeterKeyTable.CoincidentDemandSupported;
            }
        }

        /// <summary>
        /// Returns whether or not thermal demand is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_ThermalDemandSupported
        {
            get
            {
                return MeterKeyTable.ThermalDemandSupported;
            }
        }

        /// <summary>
        /// Returns whether or not demand thresholds are allowed.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_DemandThresholdsAllowed
        {
            get
            {
                return MeterKeyTable.DemandThresholdsAllowed;
            }
        }

        /// <summary>
        /// Returns the maximum number of scheduled demand resets.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public UInt16 MeterKey_MaxDemandResets
        {
            get
            {
                return MeterKeyTable.MaxDemandResets;
            }
        }

        /// <summary>
        /// Returns whether or not Scheduled Demand Resets are Allowed.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_ScheduledResetsAllowed
        {
            get
            {
                return MeterKeyTable.ScheduledResetsAllowed;
            }
        }

        /// <summary>
        /// Returns what the maximum load profile memory size supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public UInt16 MeterKey_MaximumLPMemorySize
        {
            get
            {
                return MeterKeyTable.MaximumLPMemorySize;
            }
        }

        /// <summary>
        /// Returns whether or not power quality is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_PQSupported
        {
            get
            {
                return MeterKeyTable.PQSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the meter will Enable 9S in 6S Service.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_Enable9SIn6SServiceSupported
        {
            get
            {
                return MeterKeyTable.Enable9SIn6SServiceSupported;
            }
        }

        /// <summary>
        /// Returns whether or not Option Board Configuration is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_OptionBoardConfigSupported
        {
            get
            {
                return MeterKeyTable.OptionBoardConfigSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the ANSI C12.21 Modem is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_ModemAllowed
        {
            get
            {
                return MeterKeyTable.ModemAllowed;
            }
        }

        /// <summary>
        /// Returns whether or not the CPC Instantaneous Vars are supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_CPCVarsSupported
        {
            get
            {
                return MeterKeyTable.CPCVarsSupported;
            }
        }

        /// <summary>
        /// Returns whether or not the CPC Instantaneous VA is supported.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_CPCVASupported
        {
            get
            {
                return MeterKeyTable.CPCVASupported;
            }
        }

        /// <summary>
        /// Returns whether or not Self Read Configuration is Allowed.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_SRConfigSupported
        {
            get
            {
                return MeterKeyTable.SRConfigSupported;
            }
        }

        /// <summary>
        /// Returns the number of additional self read buffers that are allowed.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public UInt16 MeterKey_NumberAdditionalSRs
        {
            get
            {
                return MeterKeyTable.NumberAdditionalSRs;
            }
        }

        /// <summary>
        /// Returns whether or not the Calendar Configuration is Allowed.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool MeterKey_CalendarConfigSupported
        {
            get
            {
                return MeterKeyTable.CalendarConfigSupported;
            }
        }


        /// <summary>
        /// Gets the list of Coincident Values
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/09 RCG 2.20.05 N/A    Created
        public virtual List<Quantity> CoincidentValues
        {
            get
            {
                return new List<Quantity>();
            }
        }

        /// <summary>
        /// Proves access to a list of Self Read Collections
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/29/06 KRC 7.35.00 N/A    Created
        //  12/01/06 jrf 8.00.00 N/A    Added checks for additional self read quantities
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
        public virtual List<QuantityCollection> SelfReadRegisters
        {
            get
            {
                List<QuantityCollection> SelfReadQtys = new List<QuantityCollection>();
                Quantity Qty;
                uint uiNumSelfReads = NumberOfSelfReads;

                if (uiNumSelfReads > MAX_SELF_READS)
                {
                    uiNumSelfReads = MAX_SELF_READS;
                }

                for (uint uiIndex = 0; uiIndex < uiNumSelfReads; uiIndex++)
                {
                    // Don't create the SR Quantities unless they have valid data.
                    if (true == SelfReadHasValidData(uiIndex))
                    {
                        QuantityCollection SRQuantities = new QuantityCollection();
                        // Add Watts Del
                        Qty = SRWattsDelivered(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Watts Rec
                        Qty = SRWattsReceived(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Watts Net
                        Qty = SRWattsNet(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Watts Uni
                        Qty = SRWattsUni(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add VA Del
                        Qty = SRVADelivered(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add VA Rec
                        Qty = SRVAReceived(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add VA Lag
                        Qty = SRVALagging(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Del
                        Qty = SRVarDelivered(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Rec
                        Qty = SRVarReceived(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Net
                        Qty = SRVarNet(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Net Del
                        Qty = SRVarNetDelivered(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Net Rec
                        Qty = SRVarNetReceived(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Q1
                        Qty = SRVarQuadrant1(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Q2
                        Qty = SRVarQuadrant2(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Q3
                        Qty = SRVarQuadrant3(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Q4
                        Qty = SRVarQuadrant4(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add A (a)
                        Qty = SRAmpsPhaseA(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add A (b)
                        Qty = SRAmpsPhaseB(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add A (c)
                        Qty = SRAmpsPhaseC(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Neutral Amps
                        Qty = SRAmpsNeutral(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add A^2
                        Qty = SRAmpsSquared(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add V (a)
                        Qty = SRVoltsPhaseA(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add V (b)
                        Qty = SRVoltsPhaseB(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add V (c)
                        Qty = SRVoltsPhaseC(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add V Avg
                        Qty = SRVoltsAverage(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add V^2)
                        Qty = SRVoltsSquared(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add PF
                        Qty = SRPowerFactor(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Q d
                        Qty = SRQDelivered(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }
                        // Add Q r
                        Qty = SRQReceived(uiIndex);
                        if (null != Qty)
                        {
                            SRQuantities.Quantities.Add(Qty);
                        }

                        SRQuantities.Quantities.AddRange(SRCoincidentValues(uiIndex));

                        //Add the Time of the Self Read
                        SRQuantities.DateTimeOfReading = DateTimeOfSelfRead(uiIndex);

                        SelfReadQtys.Add(SRQuantities);
                    }
                }
                return SelfReadQtys;
            }
        }

        /// <summary>
        /// Proves access to a list of Last Demand Reset Register Collections
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/29/06 KRC 7.35.00 N/A    Created
        //  02/17/11 RCG 2.50.04        Adding Support for Var quantities

        public List<QuantityCollection> LastDemandResetRegisters
        {
            get
            {
                List<QuantityCollection> LastDemandResetQtys = new List<QuantityCollection>();
                Quantity Qty;
                uint uiNumDemandResets = NumberofLastDemandResets;

                if (uiNumDemandResets > MAX_DEMAND_RESETS)
                {
                    uiNumDemandResets = MAX_DEMAND_RESETS;
                }

                for (uint uiIndex = 0; uiIndex < uiNumDemandResets; uiIndex++)
                {
                    // Don't create the Demand Reset quantities if they do not have valid data
                    if (true == DemandResetHasValidData(uiIndex))
                    {
                        QuantityCollection DRQuantities = new QuantityCollection();
                        // Add Watts Del
                        Qty = DRWattsDelivered(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add Watts Rec
                        Qty = DRWattsReceived(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add Watts Net
                        Qty = DRWattsNet(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add Watts Uni
                        Qty = DRWattsUni(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add VA Del
                        Qty = DRVADelivered(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add VA Rec
                        Qty = DRVAReceived(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Del
                        Qty = DRVarDelivered(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Rec
                        Qty = DRVarReceived(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Net
                        Qty = DRVarNet(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Net Del
                        Qty = DRVarNetDelivered(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Net Rec
                        Qty = DRVarNetReceived(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Q1
                        Qty = DRVarQuadrant1(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Q2
                        Qty = DRVarQuadrant2(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Q3
                        Qty = DRVarQuadrant3(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add Var Q4
                        Qty = DRVarQuadrant4(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }
                        // Add Vh (a)
                        Qty = DRVoltsPhaseA(uiIndex);
                        if (null != Qty)
                        {
                            DRQuantities.Quantities.Add(Qty);
                        }

                        DRQuantities.Quantities.AddRange(DRCoincidentValues(uiIndex));

                        //Add Date of Demand Reset
                        DRQuantities.DateTimeOfReading = DateTimeOfDemandReset(uiIndex);

                        LastDemandResetQtys.Add(DRQuantities);
                    }
                }
                return LastDemandResetQtys;
            }
        }

        /// <summary>
        /// Provides access to Normal Display List
        /// </summary>
        /// <returns>
        /// List of DisplayItems.  (Null if list does not exist
        /// </returns> 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/02/06 KRC  7.35.00			Created 
        //  02/28/07 KRC  8.00.14           Fixing for Edit Registers
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
        public virtual List<DisplayItem> NormalDisplayList
        {
            get
            {
                List<DisplayItem> NormalDisplayList = new List<DisplayItem>();
                List<ANSIDisplayItem> ReadList = new List<ANSIDisplayItem>(LIDRetriever.MAX_LIDS_PER_READ);
                ANSIDisplayData CurrentDisplayData;
                ANSIDisplayItem CurrentDisplayItem;

                // Now that we know we have the list of display items we need
                // to update the values for all of the display items

                for (int iIndex = 0; iIndex < Table2048.DisplayConfig.NormalDisplayData.Count; iIndex++)
                {
                    CurrentDisplayData = Table2048.DisplayConfig.NormalDisplayData[iIndex];

                    // Now we can creat our Display Item to add to the Display List.
                    CurrentDisplayItem = CreateDisplayItem(CreateLID(CurrentDisplayData.NumericLID), CurrentDisplayData.DisplayID,
                                                                CurrentDisplayData.DisplayFormat, CurrentDisplayData.DisplayDimension);

                    NormalDisplayList.Add(CurrentDisplayItem);

                    // Change the item description if needed
                    HandleIrregularDescription(CurrentDisplayItem);

                    if (HandleIrregularRead(CurrentDisplayItem) == false)
                    {
                        ReadList.Add(CurrentDisplayItem);

                        if (ReadList.Count == LIDRetriever.MAX_LIDS_PER_READ)
                        {
                            ReadDisplayData(ReadList);

                            ReadList.Clear();
                        }
                    }
                }

                // If we get to this point and we still have some data in ReadList then we need
                // to go ahead and read the remaining data
                if (ReadList.Count > 0)
                {
                    ReadDisplayData(ReadList);

                    ReadList.Clear();
                }

                return NormalDisplayList;
            }

        }

        /// <summary>
        /// Provides access to the Alt Display
        /// </summary>
        /// <returns>
        /// List of DisplayItems.  (Null if list does not exist
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/02/06 KRC  7.35.00			Created 
        //  02/28/07 KRC  8.00.14           Fixing for Edit Registers
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
        public virtual List<DisplayItem> AlternateDisplayList
        {
            get
            {
                List<DisplayItem> AltDisplayList = new List<DisplayItem>();
                List<ANSIDisplayItem> ReadList = new List<ANSIDisplayItem>(LIDRetriever.MAX_LIDS_PER_READ);
                ANSIDisplayData CurrentDisplayData;
                ANSIDisplayItem CurrentDisplayItem;

                // Now that we know we have the list of display items we need
                // to update the values for all of the display items

                for (int iIndex = 0; iIndex < Table2048.DisplayConfig.AlternateDisplayData.Count; iIndex++)
                {
                    CurrentDisplayData = Table2048.DisplayConfig.AlternateDisplayData[iIndex];

                    // Now we can creat our Display Item to add to the Display List.
                    CurrentDisplayItem = CreateDisplayItem(CreateLID(CurrentDisplayData.NumericLID), CurrentDisplayData.DisplayID,
                                                                CurrentDisplayData.DisplayFormat, CurrentDisplayData.DisplayDimension);
                    AltDisplayList.Add(CurrentDisplayItem);

                    // Change the item description if needed
                    HandleIrregularDescription(CurrentDisplayItem);

                    if (HandleIrregularRead(CurrentDisplayItem) == false)
                    {
                        ReadList.Add(CurrentDisplayItem);

                        if (ReadList.Count == LIDRetriever.MAX_LIDS_PER_READ)
                        {
                            ReadDisplayData(ReadList);

                            ReadList.Clear();
                        }
                    }
                }

                // If we get to this point and we still have some data in ReadList then we need
                // to go ahead and read the remaining data
                if (ReadList.Count > 0)
                {
                    ReadDisplayData(ReadList);

                    ReadList.Clear();
                }

                return AltDisplayList;
            }
        }

        /// <summary>
        /// Provides access to the Test Display.
        /// </summary>
        /// <returns>
        /// List of DisplayItems.  (Null if list does not exist
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/02/06 KRC  7.35.00			Created 
        //  02/28/07 KRC  8.00.14           Fixing for Edit Registers
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
        public virtual List<DisplayItem> TestDisplayList
        {
            get
            {
                List<DisplayItem> TestDisplayList = new List<DisplayItem>();
                List<ANSIDisplayItem> ReadList = new List<ANSIDisplayItem>(LIDRetriever.MAX_LIDS_PER_READ);
                ANSIDisplayData CurrentDisplayData;
                ANSIDisplayItem CurrentDisplayItem;

                // Now that we know we have the list of display items we need
                // to update the values for all of the display items

                for (int iIndex = 0; iIndex < Table2048.DisplayConfig.TestDisplayData.Count; iIndex++)
                {
                    CurrentDisplayData = Table2048.DisplayConfig.TestDisplayData[iIndex];

                    // Now we can creat our Display Item to add to the Display List.
                    CurrentDisplayItem = CreateDisplayItem(CreateLID(CurrentDisplayData.NumericLID), CurrentDisplayData.DisplayID,
                                                                CurrentDisplayData.DisplayFormat, CurrentDisplayData.DisplayDimension);
                    TestDisplayList.Add(CurrentDisplayItem);

                    // Change the item description if needed
                    HandleIrregularDescription(CurrentDisplayItem);

                    if (HandleIrregularRead(CurrentDisplayItem) == false)
                    {
                        ReadList.Add(CurrentDisplayItem);

                        if (ReadList.Count == LIDRetriever.MAX_LIDS_PER_READ)
                        {
                            ReadDisplayData(ReadList);

                            ReadList.Clear();
                        }
                    }
                }

                // If we get to this point and we still have some data in ReadList then we need
                // to go ahead and read the remaining data
                if (ReadList.Count > 0)
                {
                    ReadDisplayData(ReadList);

                    ReadList.Clear();
                }

                return TestDisplayList;
            }
        }

        /// <summary>
        /// Provides access to the TOU Schedule in the meter
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ ---------------------------------------------
        //  10/25/06 KRC 7.36.00
        //  01/05/07 RCG 8.00.05           Promoted from CENTRON_AMI
        //  11/13/13 AF  3.50.03           Class re-architecture - Moved definition from ItronDevice
        //  11/14/13 jrf 3.50.04 TQ 9478   Using new properties to extract TOU and Calendar configurations.
        //
        public virtual CTOUSchedule TimeOfUseSchedule
        {
            get
            {
                if (null == m_TOUSchedule)
                {
                    m_TOUSchedule = ReadTOUSchedule(TOUConfiguration, CalendarConfiguration);
                }

                return (CTOUSchedule)m_TOUSchedule;
            }
        }

        /// <summary>
        /// Gets the current TOU configuration.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version ID Number Description
        //  -------- --- ------- -- ------ ---------------------------------------
        //  11/14/13 jrf 3.50.04 TQ 9478  Created
        // 
        public virtual TOUConfig TOUConfiguration
        {
            get
            {
                TOUConfig TOUConfigData = null;

                if (null != Table2048)
                {
                    TOUConfigData = Table2048.TOUConfig;
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
        //  11/14/13 jrf 3.50.04 TQ 9478  Created
        // 
        public virtual CalendarConfig CalendarConfiguration
        {
            get
            {
                CalendarConfig CalendarConfigData = null;

                if (null != Table2048)
                {
                    CalendarConfigData = Table2048.CalendarConfig;
                }

                return CalendarConfigData;
            }
        }

        /// <summary>
        /// TOU is considered enabled if the clock is running and the meter
        /// is configured to follow a TOU schedule.  TOU does not have to be
        /// running for this property to return true.  For example an expired
        /// TOU schedule is enabled but not running.
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 								Created
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        // 		
        public virtual bool TOUEnabled
        {
            get
            {
                bool Enabled = false;

                if ((ClockRunning) && (0 != Table2048.TOU_ID))
                {
                    Enabled = true;
                }

                return Enabled;
            }
        }

        /// <summary>
        /// Provides access to the number Of Energies
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Created
        //
        public virtual uint NumberEnergies
        {
            get
            {
                if (false == m_uiNumEnergies.Cached)
                {
                    // Do read if item is not cached
                    CapabilitiesRead();
                }

                return m_uiNumEnergies.Value;
            }
        }

        /// <summary>
        /// Provides access to the number Of Demands
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Created
        //
        public virtual uint NumberDemands
        {
            get
            {
                if (false == m_uiNumDemands.Cached)
                {
                    // Do read if item is not cached
                    CapabilitiesRead();
                }

                return m_uiNumDemands.Value;
            }
        }



        /// <summary>
        /// Provides access to the number Of TOU Rates
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Created
        //
        public virtual uint NumberTOURates
        {
            get
            {
                if (false == m_uiNumTOURates.Cached)
                {
                    // Do read if item is not cached
                    CapabilitiesRead();
                }

                return m_uiNumTOURates.Value;
            }
        }

        /// <summary>
        /// Gets the Number of TOU Rates supported by this meter
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  038/12/07 KRC 8.00.18 2424    Adding ability to get MeterKey TOU info
        // 		
        public uint MeterKey_RatesSupported
        {
            get
            {
                uint uiNumRatesSupported;

                uiNumRatesSupported = MeterKeyTable.TOURatesSupported;

                return uiNumRatesSupported;
            }
        }

        /// <summary>
        /// This property returns the meterkeytable. (Creates it if needed)
        /// </summary>
        /// <remarks>
        /// This method must be overriden by the device classes.
        /// </remarks>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/20/06 KRC 8.00.00 N/A    Created
        // 
        public CMeterKeyTable MeterKeyTable
        {
            get
            {
                if (null == m_MeterKeyTable)
                {
                    m_MeterKeyTable = new CMeterKeyTable(m_PSEM);
                }

                return m_MeterKeyTable;
            }
        }

        /// <summary>
        /// Returns the Date and Time of the the Self Read at the given index.
        /// </summary>
        /// <param name="uiIndex">Index of the Self Read (Between 0 and 3)</param>
        /// <returns></returns>
        protected virtual DateTime DateTimeOfSelfRead(uint uiIndex)
        {
            DateTime dtSelfRead = MeterReferenceTime;
            PSEMResponse Result = PSEMResponse.Ok;
            uint MeterSeconds;
            byte[] Data = null;
            LID lidSRDateTime;
            uint[] uiarSRBuff = {(uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1, (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_2,
								 (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_3, (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_4};

            // Assign the LID
            lidSRDateTime = CreateLID((uint)m_LID.BASE_TIME_DATE_OF_PERIODIC_READ.lidValue |
                                             uiarSRBuff[uiIndex]);

            if (uiIndex < NumberOfSelfReads)
            {
                Result = m_lidRetriever.RetrieveLID(lidSRDateTime, out Data);

                if (PSEMResponse.Ok == Result)
                {
                    //Convert the data to seconds
                    MeterSeconds = m_lidRetriever.DataReader.ReadUInt32();

                    // Add the seconds to the reference time (1/1/2000)
                    dtSelfRead = dtSelfRead.AddSeconds((double)MeterSeconds);
                }
            }

            if (uiIndex > (NumberOfSelfReads - 1) || PSEMResponse.Ok != Result)
            {
                throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the date of the Self Read; LID = " + lidSRDateTime.lidValue.ToString(CultureInfo.CurrentCulture)));
            }

            return dtSelfRead;
        }

        /// <summary>
        /// Returns the Date and Time of the the Demand Reset at the given index.
        /// </summary>
        /// <param name="uiIndex">Index of the Demand Reset (Between 0 and 1)</param>
        /// <returns></returns>
        protected virtual DateTime DateTimeOfDemandReset(uint uiIndex)
        {
            DateTime dtDemandReset = MeterReferenceTime;
            PSEMResponse Result = PSEMResponse.Ok;
            uint MeterSeconds;
            byte[] Data = null;
            LID lidDRDateTime;
            uint[] uiarSRBuff = { (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR, (uint)DefinedLIDs.SlfRd_Data.SR_2ND_LAST_DR };

            // Assign the LID.
            lidDRDateTime = CreateLID((uint)m_LID.BASE_TIME_DATE_OF_PERIODIC_READ.lidValue |
                                             uiarSRBuff[uiIndex]);

            if (uiIndex < NumberofLastDemandResets)
            {
                Result = m_lidRetriever.RetrieveLID(lidDRDateTime, out Data);

                if (PSEMResponse.Ok == Result)
                {
                    //Convert the data to seconds
                    MeterSeconds = m_lidRetriever.DataReader.ReadUInt32();

                    // Add the seconds to the reference time (1/1/2000)
                    dtDemandReset = dtDemandReset.AddSeconds((double)MeterSeconds);
                }
            }

            if (uiIndex > (NumberofLastDemandResets - 1) || PSEMResponse.Ok != Result)
            {
                throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the date of the Demand Reset; LID = " + lidDRDateTime.lidValue.ToString(CultureInfo.CurrentCulture)));
            }

            return dtDemandReset;

        }

        /// <summary>
        /// Returns status of indicated Self Read.
        /// </summary>
        /// <param name="uiIndex">Index of the Self Read (Between 0 and 3)</param>
        /// <returns>bool - True = Has valid Data; False = No valid Data</returns>
        protected virtual bool SelfReadHasValidData(uint uiIndex)
        {
            PSEMResponse Result = PSEMResponse.Ok;
            bool bValidData = false;
            byte[] Data = null;
            LID lidSRValid;
            uint[] uiarSRBuff = {(uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1, (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_2,
								 (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_3, (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_4};

            lidSRValid = CreateLID((uint)m_LID.BASE_VALID_DATA_IN_PERIODIC_READ.lidValue |
                                             uiarSRBuff[uiIndex]);

            if (uiIndex < (NumberOfSelfReads))
            {
                Result = m_lidRetriever.RetrieveLID(lidSRValid, out Data);

                if (PSEMResponse.Ok == Result)
                {
                    if (0 == Data[0])
                    {
                        bValidData = false;
                    }
                    else
                    {
                        bValidData = true;
                    }
                }
            }

            if (uiIndex > (NumberOfSelfReads - 1) || PSEMResponse.Ok != Result)
            {
                throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error determining if the Self Read data is valid; LID = " + lidSRValid.lidValue.ToString(CultureInfo.InvariantCulture)));
            }

            return bValidData;
        }

        /// <summary>
        /// Returns the Date and Time of the the Demand Reset at the given index.
        /// </summary>
        /// <param name="uiIndex">Index of the Demand Reset (Between 0 and 1)</param>
        /// <returns>bool - True = Has valid Data; False = No valid Data</returns>
        protected virtual bool DemandResetHasValidData(uint uiIndex)
        {
            DateTime dtDemandReset = MeterReferenceTime;
            PSEMResponse Result = PSEMResponse.Ok;
            bool bValidData = false;
            byte[] Data = null;
            LID lidDRValid;
            uint[] uiarDRBuff = { (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR, (uint)DefinedLIDs.SlfRd_Data.SR_2ND_LAST_DR };

            // Assign the LID
            lidDRValid = CreateLID((uint)m_LID.BASE_VALID_DATA_IN_PERIODIC_READ.lidValue |
                                             uiarDRBuff[uiIndex]);

            if (uiIndex < (NumberofLastDemandResets))
            {
                Result = m_lidRetriever.RetrieveLID(lidDRValid, out Data);

                if (PSEMResponse.Ok == Result)
                {
                    if (0 == Data[0])
                    {
                        bValidData = false;
                    }
                    else
                    {
                        bValidData = true;
                    }
                }
            }

            if (uiIndex > (NumberofLastDemandResets - 1) || PSEMResponse.Ok != Result)
            {
                throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading if the Demand Reset data is valid; LID = " + lidDRValid.lidValue.ToString(CultureInfo.CurrentCulture)));
            }

            return bValidData;
        }

        /// <summary>
        /// Provides access to the SR Watts Delivered Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get Watts Del
        //
        protected virtual Quantity SRWattsDelivered(uint uiIndex)
        {
            ANSIQuantity SRWattsDel = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_WH_DEL) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_W_DEL))
            {
                SRWattsDel = new ANSIQuantity("Watts Delivered", m_PSEM, this);

                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRWattsDel.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_WH_DEL.lidValue | uiBufferOffset);
                SRWattsDel.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_W_DEL.lidValue | uiBufferOffset);
                SRWattsDel.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_WH_DEL.lidValue | uiBufferOffset);
                SRWattsDel.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_W_DEL.lidValue | uiBufferOffset);
            }

            return SRWattsDel;
        }

        /// <summary>
        /// Provides access to the DR Watts Delivered Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get Watts Del
        //
        protected virtual Quantity DRWattsDelivered(uint uiIndex)
        {
            ANSIQuantity DRWattsDel = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_WH_DEL) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_W_DEL))
            {
                DRWattsDel = new ANSIQuantity("Watts Delivered", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRWattsDel.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_WH_DEL.lidValue | uiBufferOffset);
                DRWattsDel.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_W_DEL.lidValue | uiBufferOffset);
                DRWattsDel.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_WH_DEL.lidValue | uiBufferOffset);
                DRWattsDel.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_W_DEL.lidValue | uiBufferOffset);

            }

            return DRWattsDel;
        }

        /// <summary>
        /// Provides access to the SR Watts Received Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get SR Watts Rec
        //
        protected virtual Quantity SRWattsReceived(uint uiIndex)
        {
            ANSIQuantity SRWattsRec = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_WH_REC) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_W_REC))
            {
                SRWattsRec = new ANSIQuantity("Watts Received", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRWattsRec.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_WH_REC.lidValue | uiBufferOffset);
                SRWattsRec.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_W_REC.lidValue | uiBufferOffset);
                SRWattsRec.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_WH_REC.lidValue | uiBufferOffset);
                SRWattsRec.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_W_REC.lidValue | uiBufferOffset);
            }

            return SRWattsRec;
        }

        /// <summary>
        /// Provides access to the DR Watts Received Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get Watts Del
        //
        protected virtual Quantity DRWattsReceived(uint uiIndex)
        {
            ANSIQuantity DRWattsRec = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_WH_REC) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_W_REC))
            {
                DRWattsRec = new ANSIQuantity("Watts Received", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRWattsRec.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_WH_REC.lidValue | uiBufferOffset);
                DRWattsRec.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_W_REC.lidValue | uiBufferOffset);
                DRWattsRec.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_WH_REC.lidValue | uiBufferOffset);
                DRWattsRec.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_W_REC.lidValue | uiBufferOffset);
            }

            return DRWattsRec;
        }

        /// <summary>
        /// Provides access to the SR Watts Delivered Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get SR Watts Net
        //
        protected virtual Quantity SRWattsNet(uint uiIndex)
        {
            ANSIQuantity SRWattsNet = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_WH_NET) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_W_NET))
            {
                SRWattsNet = new ANSIQuantity("Watts Net", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRWattsNet.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_WH_NET.lidValue | uiBufferOffset);
                SRWattsNet.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_W_NET.lidValue | uiBufferOffset);
                SRWattsNet.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_WH_NET.lidValue | uiBufferOffset);
                SRWattsNet.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_W_NET.lidValue | uiBufferOffset);
            }

            return SRWattsNet;
        }

        /// <summary>
        /// Provides access to the DR Watts Delivered Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get DR Watts Net
        //
        protected virtual Quantity DRWattsNet(uint uiIndex)
        {
            ANSIQuantity DRWattsNet = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_WH_NET) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_W_NET))
            {
                DRWattsNet = new ANSIQuantity("Watts Net", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRWattsNet.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_WH_NET.lidValue | uiBufferOffset);
                DRWattsNet.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_W_NET.lidValue | uiBufferOffset);
                DRWattsNet.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_WH_NET.lidValue | uiBufferOffset);
                DRWattsNet.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_W_NET.lidValue | uiBufferOffset);
            }

            return DRWattsNet;
        }

        /// <summary>
        /// Provides access to the SR Watts Uni Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get Sr Watts Del
        //
        protected virtual Quantity SRWattsUni(uint uiIndex)
        {
            ANSIQuantity SRWattsUniDir = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_WH_UNI) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_W_UNI))
            {
                SRWattsUniDir = new ANSIQuantity("Unidirectional Watts", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRWattsUniDir.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_WH_UNI.lidValue | uiBufferOffset);
                SRWattsUniDir.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_W_UNI.lidValue | uiBufferOffset);
                SRWattsUniDir.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_WH_UNI.lidValue | uiBufferOffset);
                SRWattsUniDir.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_W_UNI.lidValue | uiBufferOffset);
            }

            return SRWattsUniDir;
        }

        /// <summary>
        /// Gets the list of Coincident Values for the specified Self Read
        /// </summary>
        /// <param name="uiIndex">The index of the Self Read to get.</param>
        /// <returns>The list of Coincidents</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/09 RCG 2.20.05 N/A    Created

        protected virtual List<Quantity> SRCoincidentValues(uint uiIndex)
        {
            return new List<Quantity>();
        }

        /// <summary>
        /// Gets the list of Coincident Values for the specified Demand Resets
        /// </summary>
        /// <param name="uiIndex">The index of the Demand Reset to get.</param>
        /// <returns>The list of Coincidents</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/09 RCG 2.20.05 N/A    Created

        protected virtual List<Quantity> DRCoincidentValues(uint uiIndex)
        {
            return new List<Quantity>();
        }

        /// <summary>
        /// Provides access to the DR Watts Uni Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get DR Watts Uni
        //
        protected virtual Quantity DRWattsUni(uint uiIndex)
        {
            ANSIQuantity DRWattsUniDir = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_WH_UNI) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_W_UNI))
            {
                DRWattsUniDir = new ANSIQuantity("Unidirectional Watts", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRWattsUniDir.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_WH_UNI.lidValue | uiBufferOffset);
                DRWattsUniDir.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_W_UNI.lidValue | uiBufferOffset);
                DRWattsUniDir.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_WH_UNI.lidValue | uiBufferOffset);
                DRWattsUniDir.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_W_UNI.lidValue | uiBufferOffset);
            }

            return DRWattsUniDir;
        }

        /// <summary>
        /// Provides access to the SR VA Delivered Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get Sr VA Del
        //  12/01/06 jrf 8.00.00 N/A    Adding support for arithmetic and vectorial
        //                              calculated VA
        //
        protected virtual Quantity SRVADelivered(uint uiIndex)
        {
            ANSIQuantity SRVADel = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            // Check for Arithmetic value
            if (true == ValidateEnergy(m_LID.ENERGY_VAH_DEL_ARITH) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VA_DEL_ARITH))
            {
                SRVADel = new ANSIQuantity("VA Delivered", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVADel.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VAH_DEL_ARITH.lidValue | uiBufferOffset);
                SRVADel.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VA_DEL_ARITH.lidValue | uiBufferOffset);
                SRVADel.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VAH_DEL_ARITH.lidValue | uiBufferOffset);
                SRVADel.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VA_DEL_ARITH.lidValue | uiBufferOffset);
            }
            // Check for Vectorial value
            else if (true == ValidateEnergy(m_LID.ENERGY_VAH_DEL_VECT) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VA_DEL_VECT))
            {
                SRVADel = new ANSIQuantity("VA Delivered", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVADel.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VAH_DEL_VECT.lidValue | uiBufferOffset);
                SRVADel.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VA_DEL_VECT.lidValue | uiBufferOffset);
                SRVADel.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VAH_DEL_VECT.lidValue | uiBufferOffset);
                SRVADel.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VA_DEL_VECT.lidValue | uiBufferOffset);
            }

            return SRVADel;
        }
        /// <summary>
        /// Provides access to the DR VA Del Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get DR VA Del
        //
        protected virtual Quantity DRVADelivered(uint uiIndex)
        {
            ANSIQuantity DRVADel = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VAH_DEL) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VA_DEL))
            {
                DRVADel = new ANSIQuantity("VA Delivered", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRVADel.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VAH_DEL.lidValue | uiBufferOffset);
                DRVADel.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VA_DEL.lidValue | uiBufferOffset);
                DRVADel.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VAH_DEL.lidValue | uiBufferOffset);
                DRVADel.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VA_DEL.lidValue | uiBufferOffset);
            }

            return DRVADel;
        }

        /// <summary>
        /// Provides access to the SR VA Received Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get SR VA Rec
        //  12/01/06 jrf 8.00.00 N/A    Adding support for arithmetic and vectorial
        //                              calculated VA
        //
        protected virtual Quantity SRVAReceived(uint uiIndex)
        {
            ANSIQuantity SRVARec = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            // Check for Arithmetic value
            if (true == ValidateEnergy(m_LID.ENERGY_VAH_REC_ARITH) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VA_REC_ARITH))
            {
                SRVARec = new ANSIQuantity("VA Received", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVARec.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VAH_REC_ARITH.lidValue | uiBufferOffset);
                SRVARec.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VA_REC_ARITH.lidValue | uiBufferOffset);
                SRVARec.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VAH_REC_ARITH.lidValue | uiBufferOffset);
                SRVARec.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VA_REC_ARITH.lidValue | uiBufferOffset);
            }
            // Check for Vectorial value
            else if (true == ValidateEnergy(m_LID.ENERGY_VAH_REC_VECT) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VA_REC_VECT))
            {
                SRVARec = new ANSIQuantity("VA Received", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVARec.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VAH_REC_VECT.lidValue | uiBufferOffset);
                SRVARec.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VA_REC_VECT.lidValue | uiBufferOffset);
                SRVARec.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VAH_REC_VECT.lidValue | uiBufferOffset);
                SRVARec.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VA_REC_VECT.lidValue | uiBufferOffset);
            }

            return SRVARec;
        }

        /// <summary>
        /// Provides access to the DR VA Rec Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/26/06 KRC 7.35.00 N/A    Adding support to get DR VA Rec
        //
        protected virtual Quantity DRVAReceived(uint uiIndex)
        {
            ANSIQuantity DRVARec = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VAH_REC) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VA_REC))
            {
                DRVARec = new ANSIQuantity("VA Received", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRVARec.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VAH_REC.lidValue | uiBufferOffset);
                DRVARec.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VA_REC.lidValue | uiBufferOffset);
                DRVARec.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VAH_REC.lidValue | uiBufferOffset);
                DRVARec.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VA_REC.lidValue | uiBufferOffset);
            }

            return DRVARec;
        }

        /// <summary>
        /// Provides access to the SR VA Lagging Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR VA Lag
        //
        protected virtual Quantity SRVALagging(uint uiIndex)
        {
            ANSIQuantity SRVALag = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VAH_LAG) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VA_LAG))
            {
                SRVALag = new ANSIQuantity("VA Lagging", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVALag.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VAH_LAG.lidValue | uiBufferOffset);
                SRVALag.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VA_LAG.lidValue | uiBufferOffset);
                SRVALag.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VAH_LAG.lidValue | uiBufferOffset);
                SRVALag.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VA_LAG.lidValue | uiBufferOffset);
            }

            return SRVALag;
        }

        /// <summary>
        /// Provides access to the SR Var Delivered Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR Var Del
        //
        protected virtual Quantity SRVarDelivered(uint uiIndex)
        {
            ANSIQuantity SRVarDel = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_DEL) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_DEL))
            {
                SRVarDel = new ANSIQuantity("Var Delivered", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVarDel.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_DEL.lidValue | uiBufferOffset);
                SRVarDel.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_DEL.lidValue | uiBufferOffset);
                SRVarDel.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_DEL.lidValue | uiBufferOffset);
                SRVarDel.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_DEL.lidValue | uiBufferOffset);
            }

            return SRVarDel;
        }

        /// <summary>
        /// Provides access to the DR Var Delivered Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/17/11 RCG 2.50.04 N/A    Created

        protected virtual Quantity DRVarDelivered(uint uiIndex)
        {
            ANSIQuantity DRVarDel = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_DEL) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_DEL))
            {
                DRVarDel = new ANSIQuantity("Var Delivered", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRVarDel.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_DEL.lidValue | uiBufferOffset);
                DRVarDel.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_DEL.lidValue | uiBufferOffset);
                DRVarDel.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_DEL.lidValue | uiBufferOffset);
                DRVarDel.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_DEL.lidValue | uiBufferOffset);
            }

            return DRVarDel;
        }

        /// <summary>
        /// Provides access to the SR Var Received Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR Var Rec
        //
        protected virtual Quantity SRVarReceived(uint uiIndex)
        {
            ANSIQuantity SRVarRec = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_REC) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_REC))
            {
                SRVarRec = new ANSIQuantity("Var Received", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVarRec.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_REC.lidValue | uiBufferOffset);
                SRVarRec.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_REC.lidValue | uiBufferOffset);
                SRVarRec.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_REC.lidValue | uiBufferOffset);
                SRVarRec.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_REC.lidValue | uiBufferOffset);
            }

            return SRVarRec;
        }

        /// <summary>
        /// Provides access to the DR Var Received Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/17/11 RCG 2.50.04 N/A    Created

        protected virtual Quantity DRVarReceived(uint uiIndex)
        {
            ANSIQuantity DRVarRec = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_REC) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_REC))
            {
                DRVarRec = new ANSIQuantity("Var Received", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRVarRec.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_REC.lidValue | uiBufferOffset);
                DRVarRec.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_REC.lidValue | uiBufferOffset);
                DRVarRec.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_REC.lidValue | uiBufferOffset);
                DRVarRec.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_REC.lidValue | uiBufferOffset);
            }

            return DRVarRec;
        }

        /// <summary>
        /// Provides access to the SR Var Net Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR Var Net
        //
        protected virtual Quantity SRVarNet(uint uiIndex)
        {
            ANSIQuantity SRVarNetQty = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_NET) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_NET))
            {
                SRVarNetQty = new ANSIQuantity("Var Net", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVarNetQty.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_NET.lidValue | uiBufferOffset);
                SRVarNetQty.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_NET.lidValue | uiBufferOffset);
                SRVarNetQty.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_NET.lidValue | uiBufferOffset);
                SRVarNetQty.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_NET.lidValue | uiBufferOffset);
            }

            return SRVarNetQty;
        }

        /// <summary>
        /// Provides access to the DR Var Net Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/17/11 RCG 2.50.04 N/A    Created

        protected virtual Quantity DRVarNet(uint uiIndex)
        {
            ANSIQuantity DRVarNet = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_NET) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_NET))
            {
                DRVarNet = new ANSIQuantity("Var Net", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRVarNet.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_NET.lidValue | uiBufferOffset);
                DRVarNet.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_NET.lidValue | uiBufferOffset);
                DRVarNet.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_NET.lidValue | uiBufferOffset);
                DRVarNet.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_NET.lidValue | uiBufferOffset);
            }

            return DRVarNet;
        }

        /// <summary>
        /// Provides access to the SR Var Net Delivered Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR Var Net Del
        //
        protected virtual Quantity SRVarNetDelivered(uint uiIndex)
        {
            ANSIQuantity SRVarNetDel = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_NET_DEL) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_NET_DEL))
            {
                SRVarNetDel = new ANSIQuantity("Var Net Delivered", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVarNetDel.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_NET_DEL.lidValue | uiBufferOffset);
                SRVarNetDel.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_NET_DEL.lidValue | uiBufferOffset);
                SRVarNetDel.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_NET_DEL.lidValue | uiBufferOffset);
                SRVarNetDel.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_NET_DEL.lidValue | uiBufferOffset);
            }

            return SRVarNetDel;
        }

        /// <summary>
        /// Provides access to the DR Var Net Delivered Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/17/11 RCG 2.50.04 N/A    Created

        protected virtual Quantity DRVarNetDelivered(uint uiIndex)
        {
            ANSIQuantity DRVarNetDel = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_NET_DEL) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_NET_DEL))
            {
                DRVarNetDel = new ANSIQuantity("Var Net Delivered", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRVarNetDel.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_NET_DEL.lidValue | uiBufferOffset);
                DRVarNetDel.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_NET_DEL.lidValue | uiBufferOffset);
                DRVarNetDel.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_NET_DEL.lidValue | uiBufferOffset);
                DRVarNetDel.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_NET_DEL.lidValue | uiBufferOffset);
            }

            return DRVarNetDel;
        }

        /// <summary>
        /// Provides access to the SR Var Net Received Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR Var Net Rec
        //
        protected virtual Quantity SRVarNetReceived(uint uiIndex)
        {
            ANSIQuantity SRVarNetRec = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_NET_REC) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_NET_REC))
            {
                SRVarNetRec = new ANSIQuantity("Var Net Received", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVarNetRec.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_NET_REC.lidValue | uiBufferOffset);
                SRVarNetRec.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_NET_REC.lidValue | uiBufferOffset);
                SRVarNetRec.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_NET_REC.lidValue | uiBufferOffset);
                SRVarNetRec.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_NET_REC.lidValue | uiBufferOffset);
            }

            return SRVarNetRec;
        }

        /// <summary>
        /// Provides access to the DR Var Net Received Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/17/11 RCG 2.50.04 N/A    Created

        protected virtual Quantity DRVarNetReceived(uint uiIndex)
        {
            ANSIQuantity DRVarNetRec = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_NET_REC) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_NET_REC))
            {
                DRVarNetRec = new ANSIQuantity("Var Net Received", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRVarNetRec.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_NET_REC.lidValue | uiBufferOffset);
                DRVarNetRec.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_NET_REC.lidValue | uiBufferOffset);
                DRVarNetRec.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_NET_REC.lidValue | uiBufferOffset);
                DRVarNetRec.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_NET_REC.lidValue | uiBufferOffset);
            }

            return DRVarNetRec;
        }

        /// <summary>
        /// Provides access to the SR Var Quadrant 1 Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR Var Q1
        //
        protected virtual Quantity SRVarQuadrant1(uint uiIndex)
        {
            ANSIQuantity SRVarQ1 = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_Q1) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_Q1))
            {
                SRVarQ1 = new ANSIQuantity("Var Quadrant 1", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVarQ1.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_Q1.lidValue | uiBufferOffset);
                SRVarQ1.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_Q1.lidValue | uiBufferOffset);
                SRVarQ1.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_Q1.lidValue | uiBufferOffset);
                SRVarQ1.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_Q1.lidValue | uiBufferOffset);
            }

            return SRVarQ1;
        }

        /// <summary>
        /// Provides access to the DR Var Quadrant 1 Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/17/11 RCG 2.50.04 N/A    Created

        protected virtual Quantity DRVarQuadrant1(uint uiIndex)
        {
            ANSIQuantity DRVarQ1 = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_Q1) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_Q1))
            {
                DRVarQ1 = new ANSIQuantity("Var Quadrant 1", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRVarQ1.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_Q1.lidValue | uiBufferOffset);
                DRVarQ1.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_Q1.lidValue | uiBufferOffset);
                DRVarQ1.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_Q1.lidValue | uiBufferOffset);
                DRVarQ1.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_Q1.lidValue | uiBufferOffset);
            }

            return DRVarQ1;
        }

        /// <summary>
        /// Provides access to the SR Var Quadrant 2 Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR Var Q2
        //
        protected virtual Quantity SRVarQuadrant2(uint uiIndex)
        {
            ANSIQuantity SRVarQ2 = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_Q2) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_Q2))
            {
                SRVarQ2 = new ANSIQuantity("Var Quadrant 2", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVarQ2.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_Q2.lidValue | uiBufferOffset);
                SRVarQ2.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_Q2.lidValue | uiBufferOffset);
                SRVarQ2.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_Q2.lidValue | uiBufferOffset);
                SRVarQ2.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_Q2.lidValue | uiBufferOffset);
            }

            return SRVarQ2;
        }

        /// <summary>
        /// Provides access to the DR Var Quadrant 2 Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/17/11 RCG 2.50.04 N/A    Created

        protected virtual Quantity DRVarQuadrant2(uint uiIndex)
        {
            ANSIQuantity DRVarQ2 = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_Q2) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_Q2))
            {
                DRVarQ2 = new ANSIQuantity("Var Quadrant 2", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRVarQ2.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_Q2.lidValue | uiBufferOffset);
                DRVarQ2.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_Q2.lidValue | uiBufferOffset);
                DRVarQ2.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_Q2.lidValue | uiBufferOffset);
                DRVarQ2.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_Q2.lidValue | uiBufferOffset);
            }

            return DRVarQ2;
        }

        /// <summary>
        /// Provides access to the SR Var Quadrant 3 Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR Var Q3
        //
        protected virtual Quantity SRVarQuadrant3(uint uiIndex)
        {
            ANSIQuantity SRVarQ3 = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_Q3) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_Q3))
            {
                SRVarQ3 = new ANSIQuantity("Var Quadrant 3", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVarQ3.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_Q3.lidValue | uiBufferOffset);
                SRVarQ3.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_Q3.lidValue | uiBufferOffset);
                SRVarQ3.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_Q3.lidValue | uiBufferOffset);
                SRVarQ3.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_Q3.lidValue | uiBufferOffset);
            }

            return SRVarQ3;
        }

        /// <summary>
        /// Provides access to the DR Var Quadrant 3 Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/17/11 RCG 2.50.04 N/A    Created

        protected virtual Quantity DRVarQuadrant3(uint uiIndex)
        {
            ANSIQuantity DRVarQ3 = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_Q3) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_Q3))
            {
                DRVarQ3 = new ANSIQuantity("Var Quadrant 3", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRVarQ3.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_Q3.lidValue | uiBufferOffset);
                DRVarQ3.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_Q3.lidValue | uiBufferOffset);
                DRVarQ3.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_Q3.lidValue | uiBufferOffset);
                DRVarQ3.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_Q3.lidValue | uiBufferOffset);
            }

            return DRVarQ3;
        }

        /// <summary>
        /// Provides access to the SR Var Quadrant 4 Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR Var Q4
        //
        protected virtual Quantity SRVarQuadrant4(uint uiIndex)
        {
            ANSIQuantity SRVarQ4 = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_Q4) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_Q4))
            {
                SRVarQ4 = new ANSIQuantity("Var Quadrant 4", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVarQ4.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_Q4.lidValue | uiBufferOffset);
                SRVarQ4.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_Q4.lidValue | uiBufferOffset);
                SRVarQ4.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VARH_Q4.lidValue | uiBufferOffset);
                SRVarQ4.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_VAR_Q4.lidValue | uiBufferOffset);
            }

            return SRVarQ4;
        }

        /// <summary>
        /// Provides access to the DR Var Quadrant 4 Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/17/11 RCG 2.50.04 N/A    Created

        protected virtual Quantity DRVarQuadrant4(uint uiIndex)
        {
            ANSIQuantity DRVarQ4 = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VARH_Q4) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_VAR_Q4))
            {
                DRVarQ4 = new ANSIQuantity("Var Quadrant 4", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRVarQ4.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_Q4.lidValue | uiBufferOffset);
                DRVarQ4.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_Q4.lidValue | uiBufferOffset);
                DRVarQ4.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VARH_Q4.lidValue | uiBufferOffset);
                DRVarQ4.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_VAR_Q4.lidValue | uiBufferOffset);
            }

            return DRVarQ4;
        }

        /// <summary>
        /// Provides access to the SR Amps Phase A Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR A (a)
        //
        protected virtual Quantity SRAmpsPhaseA(uint uiIndex)
        {
            ANSIQuantity SRAPhA = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_AH_PHA) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_A_PHA))
            {
                SRAPhA = new ANSIQuantity("Amps (a)", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRAPhA.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_AH_PHA.lidValue | uiBufferOffset);
                SRAPhA.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_A_PHA.lidValue | uiBufferOffset);
                SRAPhA.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_AH_PHA.lidValue | uiBufferOffset);
                SRAPhA.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_A_PHA.lidValue | uiBufferOffset);
            }

            return SRAPhA;
        }

        /// <summary>
        /// Provides access to the SR Amps Phase B Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR A (b)
        //
        protected virtual Quantity SRAmpsPhaseB(uint uiIndex)
        {
            ANSIQuantity SRAPhB = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_AH_PHB) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_A_PHB))
            {
                SRAPhB = new ANSIQuantity("Amps (b)", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRAPhB.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_AH_PHB.lidValue | uiBufferOffset);
                SRAPhB.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_A_PHB.lidValue | uiBufferOffset);
                SRAPhB.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_AH_PHB.lidValue | uiBufferOffset);
                SRAPhB.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_A_PHB.lidValue | uiBufferOffset);
            }

            return SRAPhB;
        }

        /// <summary>
        /// Provides access to the SR Amps Phase C Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR A (c)
        //
        protected virtual Quantity SRAmpsPhaseC(uint uiIndex)
        {
            ANSIQuantity SRAPhC = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_AH_PHC) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_A_PHC))
            {
                SRAPhC = new ANSIQuantity("Amps (c)", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRAPhC.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_AH_PHC.lidValue | uiBufferOffset);
                SRAPhC.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_A_PHC.lidValue | uiBufferOffset);
                SRAPhC.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_AH_PHC.lidValue | uiBufferOffset);
                SRAPhC.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_A_PHC.lidValue | uiBufferOffset);
            }

            return SRAPhC;
        }

        /// <summary>
        /// Provides access to the SR Neutral Amps Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR Neutral Amps
        //
        protected virtual Quantity SRAmpsNeutral(uint uiIndex)
        {
            ANSIQuantity SRANeut = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_AH_NEUTRAL) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_A_NEUTRAL))
            {
                SRANeut = new ANSIQuantity("Neutral Amps", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRANeut.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_AH_NEUTRAL.lidValue | uiBufferOffset);
                SRANeut.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_A_NEUTRAL.lidValue | uiBufferOffset);
                SRANeut.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_AH_NEUTRAL.lidValue | uiBufferOffset);
                SRANeut.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_A_NEUTRAL.lidValue | uiBufferOffset);
            }

            return SRANeut;
        }

        /// <summary>
        /// Provides access to the SR Amps Squared Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR A^2
        //
        protected virtual Quantity SRAmpsSquared(uint uiIndex)
        {
            ANSIQuantity SRASq = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_I2H_AGG) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_I2_AGG))
            {
                SRASq = new ANSIQuantity("Amps Squared", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRASq.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_I2H_AGG.lidValue | uiBufferOffset);
                SRASq.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_I2_AGG.lidValue | uiBufferOffset);
                SRASq.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_I2H_AGG.lidValue | uiBufferOffset);
                SRASq.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_I2_AGG.lidValue | uiBufferOffset);
            }

            return SRASq;
        }

        /// <summary>
        /// Provides access to the DR Volts (a) Quantity
        /// </summary>
        /// <param name="uiIndex">Which Demand Reset to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue#         Description
        //  -------- --- ------- -------------  ---------------------------------------------
        //  07/04/08 KRC 1.51.02 itron00116942    Adding support to get DR Volts (a)
        //
        protected virtual Quantity DRVoltsPhaseA(uint uiIndex)
        {
            ANSIQuantity DRVoltsPhaseA = null;

            if (uiIndex >= MAX_DEMAND_RESETS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_DEMAND_RESETS.ToString(CultureInfo.CurrentCulture) + " Demand Resets can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VH_PHA) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_V_PHA))
            {
                DRVoltsPhaseA = new ANSIQuantity("Volts (a)", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR + DEMAND_RESET_BUFFER_OFFSET * uiIndex;

                DRVoltsPhaseA.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VH_PH_A.lidValue | uiBufferOffset);
                DRVoltsPhaseA.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_V_PH_A.lidValue | uiBufferOffset);
                DRVoltsPhaseA.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_DR_VH_PH_A.lidValue | uiBufferOffset);
                DRVoltsPhaseA.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_DR_V_PH_A.lidValue | uiBufferOffset);
            }

            return DRVoltsPhaseA;
        }

        /// <summary>
        /// Provides access to the SR Volts Phase A Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR V (a)
        //
        protected virtual Quantity SRVoltsPhaseA(uint uiIndex)
        {
            ANSIQuantity SRVPhA = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VH_PHA) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_V_PHA))
            {
                SRVPhA = new ANSIQuantity("Volts (a)", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVPhA.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VH_PHA.lidValue | uiBufferOffset);
                SRVPhA.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_V_PHA.lidValue | uiBufferOffset);
                SRVPhA.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VH_PHA.lidValue | uiBufferOffset);
                SRVPhA.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_V_PHA.lidValue | uiBufferOffset);
            }

            return SRVPhA;
        }

        /// <summary>
        /// Provides access to the SR Volts Phase B Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR V (b)
        //
        protected virtual Quantity SRVoltsPhaseB(uint uiIndex)
        {
            ANSIQuantity SRVPhB = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VH_PHB) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_V_PHB))
            {
                SRVPhB = new ANSIQuantity("Volts (b)", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVPhB.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VH_PHB.lidValue | uiBufferOffset);
                SRVPhB.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_V_PHB.lidValue | uiBufferOffset);
                SRVPhB.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VH_PHB.lidValue | uiBufferOffset);
                SRVPhB.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_V_PHB.lidValue | uiBufferOffset);
            }

            return SRVPhB;
        }

        /// <summary>
        /// Provides access to the SR Volts Phase C Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR V (c)
        //
        protected virtual Quantity SRVoltsPhaseC(uint uiIndex)
        {
            ANSIQuantity SRVPhC = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VH_PHC) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_V_PHC))
            {
                SRVPhC = new ANSIQuantity("Volts (c)", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVPhC.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VH_PHC.lidValue | uiBufferOffset);
                SRVPhC.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_V_PHC.lidValue | uiBufferOffset);
                SRVPhC.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VH_PHC.lidValue | uiBufferOffset);
                SRVPhC.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_V_PHC.lidValue | uiBufferOffset);
            }

            return SRVPhC;
        }

        /// <summary>
        /// Provides access to the SR Volts Average Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR V avg
        //
        protected virtual Quantity SRVoltsAverage(uint uiIndex)
        {
            ANSIQuantity SRVAvg = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_VH_AVG) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_V_AVG))
            {
                SRVAvg = new ANSIQuantity("Volts Average", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVAvg.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VH_AVG.lidValue | uiBufferOffset);
                SRVAvg.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_V_AVG.lidValue | uiBufferOffset);
                SRVAvg.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_VH_AVG.lidValue | uiBufferOffset);
                SRVAvg.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_V_AVG.lidValue | uiBufferOffset);
            }

            return SRVAvg;
        }

        /// <summary>
        /// Provides access to the SR Volts Squared Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR V^2
        //
        protected virtual Quantity SRVoltsSquared(uint uiIndex)
        {
            ANSIQuantity SRVSq = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_V2H_AGG) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_V2_AGG))
            {
                SRVSq = new ANSIQuantity("Volts Squared", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRVSq.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_V2H_AGG.lidValue | uiBufferOffset);
                SRVSq.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_V2_AGG.lidValue | uiBufferOffset);
                SRVSq.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_V2H_AGG.lidValue | uiBufferOffset);
                SRVSq.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_V2_AGG.lidValue | uiBufferOffset);
            }

            return SRVSq;
        }

        /// <summary>
        /// Provides access to the SR Power Factor Quantity
        /// </summary>
        /// <remarks>This quantity is demand only</remarks>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR PF
        //
        protected virtual Quantity SRPowerFactor(uint uiIndex)
        {
            ANSIQuantity SRPF = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            // Check for Arithmetic value
            if (true == ValidateDemand(m_LID.DEMAND_MIN_PF_INTERVAL_ARITH))
            {
                SRPF = new ANSIQuantity("Power Factor", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRPF.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MIN_SR_PF_INTERVAL_ARITH.lidValue | uiBufferOffset);
                SRPF.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MIN_SR_PF_INTERVAL_ARITH.lidValue | uiBufferOffset);
            }
            // Check for Vectorial value
            else if (true == ValidateDemand(m_LID.DEMAND_MIN_PF_INTERVAL_VECT))
            {
                SRPF = new ANSIQuantity("Power Factor", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRPF.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MIN_SR_PF_INTERVAL_VECT.lidValue | uiBufferOffset);
                SRPF.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MIN_SR_PF_INTERVAL_VECT.lidValue | uiBufferOffset);
            }

            return SRPF;
        }

        /// <summary>
        /// Provides access to the SR Q Delivered Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR Q Del
        //
        protected virtual Quantity SRQDelivered(uint uiIndex)
        {
            ANSIQuantity SRQDel = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_QH_DEL) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_Q_DEL))
            {
                SRQDel = new ANSIQuantity("Q Delivered", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRQDel.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_QH_DEL.lidValue | uiBufferOffset);
                SRQDel.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_Q_DEL.lidValue | uiBufferOffset);
                SRQDel.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_QH_DEL.lidValue | uiBufferOffset);
                SRQDel.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_Q_DEL.lidValue | uiBufferOffset);
            }

            return SRQDel;
        }

        /// <summary>
        /// Provides access to the SR Q Received Quantity
        /// </summary>
        /// <param name="uiIndex">Which Self Read to Retrieve</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 jrf 8.00.00 N/A    Adding support to get SR Q Rec
        //
        protected virtual Quantity SRQReceived(uint uiIndex)
        {
            ANSIQuantity SRQRec = null;

            if (uiIndex >= MAX_SELF_READS)
            {
                throw new ArgumentOutOfRangeException("uiIndex", "Only " + MAX_SELF_READS.ToString(CultureInfo.InvariantCulture) + " Self Reads can be read");
            }

            // Check to make sure this quantity is supported before constructing it.
            if (true == ValidateEnergy(m_LID.ENERGY_QH_REC) ||
               true == ValidateDemand(m_LID.DEMAND_MAX_Q_REC))
            {
                SRQRec = new ANSIQuantity("Q Received", m_PSEM, this);
                // Set the LID Properties so the Quantity know what type he is
                uint uiBufferOffset = (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1 + SELF_READ_BUFFER_OFFSET * uiIndex;

                SRQRec.TotalEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_QH_REC.lidValue | uiBufferOffset);
                SRQRec.TotalMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_Q_REC.lidValue | uiBufferOffset);
                SRQRec.TOUBaseEnergyLID = CreateLID((uint)m_LID.ENERGY_SR_QH_REC.lidValue | uiBufferOffset);
                SRQRec.TOUBaseMaxDemandLID = CreateLID((uint)m_LID.DEMAND_MAX_SR_Q_REC.lidValue | uiBufferOffset);
            }

            return SRQRec;
        }

        /// <summary>
        /// Provides access to the DST Dates in the meter
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/25/06 KRC 7.36.00
        //  01/05/07 RCG 8.00.05        Promoted from CENTRON_AMI
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        // 
        public virtual List<CDSTDatePair> DST
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

        /// <summary>
        /// Gets the Date Programmed out of the header of 2048
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/18/06 KRC 7.35.00 N/A    Created
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        // 		
        public virtual DateTime DateProgrammed
        {
            get
            {
                DateTime dtTimeProgrammed = MeterReferenceTime;

                //Get the Date Programmed out of 2048
                uint usDateProgrammed = Table2048.Table2048Header.DateProgrammed;

                // Value in 2048 is the number of seconds since Jan. 1, 2000, so to get
                //  the value returned to Jan. 1, 2000.
                dtTimeProgrammed = dtTimeProgrammed.AddSeconds((double)usDateProgrammed);

                return dtTimeProgrammed;
            }
        }

        /// <summary>
        /// Gets the Number of Times Programmed 
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/18/06 KRC 7.35.00 N/A    Created
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //  10/18/17 AF  1.2017.10.52 WR796114   Removed the check for cached value - we need to read the LID
        // 		
        public virtual int NumTimeProgrammed
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                byte[] Data = null;

                //Get the Number of Times Programmed
                Result = m_lidRetriever.RetrieveLID(m_LID.NUMBER_TIMES_PROGRAMMED, out Data);
                if (PSEMResponse.Ok == Result)
                {
                    m_NumTimesProgrammed.Value = (int)Data[0];
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the Number of Times Programmed"));
                }

                return m_NumTimesProgrammed.Value;
            }
        }

        /// <summary>
        /// Gets the Date of Last Demand reset
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/18/06 KRC 7.35.00 N/A    Created
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        // 		
        public virtual DateTime DateLastDemandReset
        {
            get
            {
                DateTime dtLastDemandReset = MeterReferenceTime;
                PSEMResponse Result = PSEMResponse.Ok;
                uint MeterSeconds;
                object objData = null;

                Result = m_lidRetriever.RetrieveLID(m_LID.LAST_DEMAND_RESET_DATE, out objData);
                if (PSEMResponse.Ok == Result)
                {
                    MeterSeconds = (UInt32)objData;

                    // Add the seconds to the reference time (1/1/2000)
                    dtLastDemandReset = dtLastDemandReset.AddSeconds((double)MeterSeconds);
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading the date of the last demand reset"));
                }

                return dtLastDemandReset;
            }
        }

        /// <summary>
        /// Gets the Date of Last Outage
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/18/06 KRC 7.35.00 N/A    Created
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //  10/18/17 AF  1.2017.10.52 WR796114   Removed the check for cached value - we need to read the LID
        // 		
        public virtual DateTime DateLastOutage
        {
            get
            {
                DateTime dtLastOutage = MeterReferenceTime;
                PSEMResponse Result = PSEMResponse.Ok;
                uint MeterSeconds;
                object objData = null;

                Result = m_lidRetriever.RetrieveLID(m_LID.OUTAGE_TIME, out objData);
                if (PSEMResponse.Ok == Result)
                {
                    //Convert the data to seconds
                    MeterSeconds = (uint)objData;

                    // Add the seconds to the reference time (1/1/2000)
                    m_DateLastOutage.Value = dtLastOutage.AddSeconds((double)MeterSeconds);
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the date of the last outage"));
                }

                return m_DateLastOutage.Value;
            }
        }

        /// <summary>
        /// Gets the number of Demand resets
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/18/06 KRC 7.35.00 N/A    Created
        //  02/11/13 AF  2.70.66 321054 Use BitConverter to grab the whole 2-byte demand
        //                              reset count instead of only the LSB.
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        // 		
        public virtual int NumDemandResets
        {
            get
            {
                int iNumDemandResets = 0;
                byte[] Data = null;
                PSEMResponse Result = PSEMResponse.Ok;

                Result = m_lidRetriever.RetrieveLID(m_LID.NUMBER_DEMAND_RESETS, out Data);
                if (PSEMResponse.Ok == Result)
                {
                    iNumDemandResets = (int)BitConverter.ToInt16(Data, 0);
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the number of demand resets"));
                }

                return iNumDemandResets;
            }
        }

        /// <summary>
        /// Gets the number of Outages
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version      Issue# Description
        //  -------- --- -------      ------ ---------------------------------------
        //  08/18/06 KRC 7.35.00      N/A    Created
        //  11/13/13 AF  3.50.03      Class re-architecture - Moved definition from ItronDevice
        //  10/18/17 AF  1.2017.10.52 WR796114   Removed the check for cached value - we need to read the LID
        // 		
        public virtual int NumOutages
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte[] Data = null;

                Result = m_lidRetriever.RetrieveLID(m_LID.NUMBER_POWER_OUTAGES, out Data);
                if (PSEMResponse.Ok == Result)
                {
                    m_NumOutages.Value = (int)Data[0];
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the number of outages"));
                }

                return m_NumOutages.Value;
            }
        }

        /// <summary>
        /// Property to retrieve the Number of Minutes on battery.
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/06 KRC  7.35.00		Created 
        //  09/08/06 MAH  7.35.00 		Promoted the method to ItronDevice
        //  							and added the override keyword
        //  11/13/13 AF   3.50.03       Class re-architecture - Moved definition from ItronDevice
        //  10/18/17 AF  1.2017.10.52 WR796114   Removed the check for cached value - we need to read the LID
        // 
        public virtual uint NumberOfMinutesOnBattery
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte[] Data = null;

                Result = m_lidRetriever.RetrieveLID(m_LID.MINUTES_ON_BATTERY, out Data);
                if (PSEMResponse.Ok == Result)
                {
                    //Convert the data to seconds
                    MemoryStream TempStream = new MemoryStream(Data);
                    BinaryReader TempBReader = new BinaryReader(TempStream);
                    m_MinutesOnBattery.Value = TempBReader.ReadUInt32();
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the number of minutes on battery"));
                }

                return m_MinutesOnBattery.Value;
            }
        }

        /// <summary>
        /// Gets the Cold Load Pickup Time in minutes
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/11/06 RCG  7.40.00			Created 
        // 11/13/13 AF   3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
        public virtual uint ColdLoadPickupTime
        {
            get
            {
                return (uint)Table2048.DemandConfig.ColdLoadPickupTime;
            }
        }

        /// <summary>
        ///  Gets the demand reset lockout time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/13 AF  2.80.26 TR7887 Created
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
        public virtual UInt16 DemandResetLockoutTime
        {
            get
            {
                return Table2048.ModeControl.DRLockoutTime;
            }
        }

        /// <summary>
        /// Gets the Number of Test Mode Sub Intervals for Demands
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/11/06 RCG  7.40.00			Created 
        // 11/13/13 AF   3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
        public virtual int NumberOfTestModeSubIntervals
        {
            get
            {
                return Table2048.DemandConfig.NumberOfTestModeSubIntervals;
            }
        }

        /// <summary>
        /// Gets the Test Mode Interval Length for Demands
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/11/06 RCG  7.40.00			Created 
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
        public virtual int TestModeIntervalLength
        {
            get
            {
                return Table2048.DemandConfig.TestModeIntervalLength;
            }
        }

        /// <summary>
        /// Gets the CT Ratio for the current device
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/10/06 RCG  7.40.00			Created 
        //
        public virtual float CTRatio
        {
            get
            {
                return Table2048.CoefficientsConfig.CTMultiplier;
            }
        }

        /// <summary>
        /// Gets the VT Ratio for the current device
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/04/06 RCG  7.40.00			Created 
        public virtual float VTRatio
        {
            get
            {
                return Table2048.CoefficientsConfig.VTMultiplier;
            }
        }

        /// <summary>
        /// Gets the Register Multiplier for the current device
        /// </summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 10/04/06 RCG  7.40.00			Created 
        /// </remarks>
        public virtual float RegisterMultiplier
        {
            get
            {
                return Table2048.CoefficientsConfig.RegisterMultiplier;
            }
        }

        /// <summary>
        /// Gets the Number of Sub Intervals for Demands
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/11/06 RCG  7.40.00			Created 
        // 11/13/13 AF   3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
        public virtual int NumberOfSubIntervals
        {
            get
            {
                return Table2048.DemandConfig.NumberOfSubIntervals;
            }
        }

        /// <summary>
        /// Gets the Interval Length for Demands
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/11/06 RCG  7.40.00			Created 
        // 11/13/13 AF   3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
        public virtual int DemandIntervalLength
        {
            get
            {
                return Table2048.DemandConfig.IntervalLength;
            }
        }

        /// <summary>
        /// Gets the Outage Length before Cold Load Pickup in seconds
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/11/06 RCG  7.40.00			Created 
        public virtual int OutageLength
        {
            get
            {
                return Table2048.DemandConfig.OutageLength;
            }
        }

        /// <summary>
        /// Gets the Display mode timeout in minutes
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/11/06 RCG  7.40.00			Created 
        public virtual int DisplayModeTimeout
        {
            get
            {
                PSEMResponse Result;
                object objValue;
                byte byValue;

                Result = m_lidRetriever.RetrieveLID(m_LID.DISP_MODE_TIMEOUT, out objValue);


                if (PSEMResponse.Ok == Result && objValue != null && objValue.GetType() == typeof(byte))
                {
                    byValue = (byte)objValue;
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Display Mode Timeout"));
                }

                return (int)byValue;
            }
        }

        /// <summary>
        /// Property to get the transformer ratio from the device.  This code actually
        /// returns the Meter Multiplier since the devices servers did the same.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/08/07 mrj 8.00.11		Created
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //  
        public virtual float TransformerRatio
        {
            get
            {
                PSEMResponse Result;
                object objValue;
                float fValue = 0.0f;

                Result = m_lidRetriever.RetrieveLID(m_LID.METER_MULTIPLIER, out objValue);

                if (PSEMResponse.Ok == Result && objValue != null && objValue.GetType() == typeof(float))
                {
                    fValue = (float)objValue;
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Meter Multiplier"));
                }

                return fValue;
            }
        }

        /// <summary>
        /// Property to get the line frequency from the device.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/08/07 mrj 8.00.11		Created
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //  
        public virtual float LineFrequency
        {
            get
            {
                PSEMResponse Result;
                object objValue;
                float fValue = 0.0f;

                Result = m_lidRetriever.RetrieveLID(m_LID.LINE_FREQUENCY, out objValue);

                if (PSEMResponse.Ok == Result && objValue != null && objValue is float)
                {
                    fValue = (float)objValue;
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                             "Error reading Line Frequency"));
                }

                return fValue;
            }
        }

        /// <summary>
        /// Property to determine the Display Mode
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/13/06 KRC 7.35.00	    Created
        //  06/05/09 jrf 2.20.08 135495 Made method virtual.
        // 
        public virtual DisplayMode MeterDisplayMode
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte[] Data = null;
                DisplayMode eDisplay;

                Result = m_lidRetriever.RetrieveLID(m_LID.DISP_MODE, out Data);
                if (PSEMResponse.Ok == Result)
                {
                    eDisplay = (DisplayMode)Data[0];
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Meter Display Mode"));
                }

                return eDisplay;
            }
        }

        /// <summary>
        /// Determines whether or not the meter is currently in test mode.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/14/07 RCG 8.10.04        Created.
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
        public virtual bool IsInTestMode
        {
            get
            {
                return MeterDisplayMode == DisplayMode.TEST_ALT_MODE || MeterDisplayMode == DisplayMode.TEST_MODE;
            }
        }

        /// <summary>
        /// Gets the Date of the TOU Expiration
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/06 KRC 7.35.00 N/A    Created
        //  01/03/07 RCG 8.00.00 N/A    Fixing the handling of the LID data so that
        //                              it handles the 6 byte format unique to this LID
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        // 		
        public virtual DateTime TOUExpirationDate
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                DateTime dtTOUExpire = MeterReferenceTime;
                uint uiUnusedData;
                ushort usExpirationYears;
                byte[] Data = null;

                if (!m_TOUExpireDate.Cached)
                {
                    Result = m_lidRetriever.RetrieveLID(m_LID.TOU_EXPIRATION_DATE, out Data);
                    if (PSEMResponse.Ok == Result)
                    {
                        //Convert the data to seconds
                        MemoryStream TempStream = new MemoryStream(Data);
                        BinaryReader TempBReader = new BinaryReader(TempStream);

                        // The result of this LID request returns 3 2 byte values we only need
                        // the last value which contains the number of years
                        uiUnusedData = TempBReader.ReadUInt32();
                        usExpirationYears = TempBReader.ReadUInt16();

                        // Add the number of years to the reference time (1/1/2000)
                        m_TOUExpireDate.Value = dtTOUExpire.AddYears((int)usExpirationYears);
                    }
                    else
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading TOU Expiration date"));
                    }
                }

                return m_TOUExpireDate.Value;
            }
        }

        /// <summary>
        /// Gets the TOU Schedule ID from the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/03/07 RCG 8.00.00 N/A    Created
        //	01/24/07 mrj 8.00.08		Changed to read from 2048, AMI now overrides
        //								and reads from table 6
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
        public virtual string TOUScheduleID
        {
            get
            {
                return m_Table2048.TOU_ID.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets whether the meter supports load profile.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/8/13 jrf 2.70.66 288156 Created
        //
        public bool LoadProfileSupported
        {
            get
            {
                bool blnSupported = false;

                blnSupported = (true == Table00.IsTableUsed(61) && true == Table00.IsTableUsed(62)
                    && true == Table00.IsTableUsed(63) && true == Table00.IsTableUsed(64));

                return blnSupported;
            }
        }

        /// <summary>
        /// Gets the Load Profile Status object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/04/06 RCG  7.40.00			Created 
        //
        public virtual LoadProfileStatusLIDS LoadProfileStatus
        {
            get
            {
                return m_LoadProfileStatus;
            }
        }

        /// <summary>
        /// Indicates whether or not the meter is currently recording
        /// load profile data
        /// </summary>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  12/05/06 MAH 8.00.00
        ///  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        /// </remarks>
        public virtual bool LPRunning
        {
            get
            {
                return m_LoadProfileStatus.IsRunning;
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
        ///  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        /// </remarks>
        public virtual int LPIntervalLength
        {
            get
            {
                return m_LoadProfileStatus.IntervalLength;
            }
        }

        /// <summary>
        /// Returns the number of load profile channels the meter is 
        /// currently recording
        /// </summary>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  12/05/06 MAH 8.00.00
        ///  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        /// </remarks>
        public virtual int NumberLPChannels
        {
            get
            {
                return m_LoadProfileStatus.NumberOfChannels;
            }
        }


        /// <summary>
        /// Property to get the custom schedule name from the device.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/14/07 jrf 8.00.18 2521	Created
        //  
        public virtual string CustomScheduleName
        {
            get
            {
                //Get the schedule name out of Table 2048
                string strCustomScheduleName = m_Table2048.BillingSchedConfig.ScheduleName;

                //Trim the end, need to specifically remove nulls
                strCustomScheduleName = strCustomScheduleName.Trim(new char[2] { ' ', '\0' });

                return strCustomScheduleName;
            }
        }

        /// <summary>
        /// Gets the Date of the Last 
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/29/07 mcm 8.00.23 2540   New item for Device Activity screen
        // 		
        public DateTime DateLastCalibration
        {
            get
            {
                DateTime dtLastCalibration = MeterReferenceTime;
                PSEMResponse Result = PSEMResponse.Ok;
                uint MeterSeconds;
                object objData = null;

                if (!m_DateLastCalibration.Cached)
                {
                    Result = m_lidRetriever.RetrieveLID(m_LID.METER_CALIBRATION_DATE, out objData);
                    if (PSEMResponse.Ok == Result)
                    {
                        //Convert the data to seconds
                        MeterSeconds = (uint)objData;

                        // Add the seconds to the reference time (1/1/2000)
                        m_DateLastCalibration.Value = dtLastCalibration.AddSeconds((double)MeterSeconds);
                    }
                    else
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading the date of the last calibration"));
                    }
                }

                return m_DateLastCalibration.Value;
            }
        }

        /// <summary>
        /// Gets the Date of the Last 
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/29/07 mcm 8.00.23 2540   New item for Device Activity screen
        //	05/15/08 mrj 1.50.25		Bug itron00107508, set date to UTC
        //  10/18/17 AF  1.2017.10.52 WR796114   Removed the check for cached value - we need to read the LID
        // 		
        public DateTime DateLastTestMode
        {
            get
            {
                DateTime dtLastTest = UTCMeterReferenceTime;
                PSEMResponse Result = PSEMResponse.Ok;
                uint MeterSeconds;
                object objData = null;

                Result = m_lidRetriever.RetrieveLID(m_LID.LAST_TEST_TIME, out objData);
                if (PSEMResponse.Ok == Result)
                {
                    //Convert the data to seconds
                    MeterSeconds = (uint)objData;

                    // Add the seconds to the reference time (1/1/2000)
                    m_DateLastTest.Value = dtLastTest.AddSeconds((double)MeterSeconds);

                    m_DateLastTest.Value = GetLocalDeviceTime(m_DateLastTest.Value);
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the date of the test"));
                }

                return m_DateLastTest.Value;
            }
        }

        /// <summary>
        /// Property to retrieve the Number of Minutes on battery.
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/06 KRC  7.35.00			Created
        // 
        public string DayOfTheWeek
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte[] Data = null;
                int iWeekDay;

                if (!m_DayOfTheWeek.Cached)
                {
                    Result = m_lidRetriever.RetrieveLID(m_LID.DAY_OF_WEEK, out Data);
                    if (PSEMResponse.Ok == Result)
                    {
                        iWeekDay = (int)Data[0];

                        // Now convert this to a string
                        switch (iWeekDay)
                        {
                            case 0:
                                {
                                    m_DayOfTheWeek.Value = "Sunday";
                                    break;
                                }
                            case 1:
                                {
                                    m_DayOfTheWeek.Value = "Monday";
                                    break;
                                }
                            case 2:
                                {
                                    m_DayOfTheWeek.Value = "Tuesday";
                                    break;
                                }
                            case 3:
                                {
                                    m_DayOfTheWeek.Value = "Wednesday";
                                    break;
                                }
                            case 4:
                                {
                                    m_DayOfTheWeek.Value = "Thursday";
                                    break;
                                }
                            case 5:
                                {
                                    m_DayOfTheWeek.Value = "Friday";
                                    break;
                                }
                            case 6:
                                {
                                    m_DayOfTheWeek.Value = "Saturday";
                                    break;
                                }
                        }
                    }
                    else
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading day of the week"));
                    }
                }

                return m_DayOfTheWeek.Value;
            }
        }


        /// <summary>
        /// This property returns a list of user data strings.  If the meter has 3 user data fields
        /// then the list will contain 3 strings corresponding to each user data  field
        /// </summary>        
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/16/06 mah 8.00    N/A    Created
        //  03/28/07 mrj 8.00.22 2788   Leaving caching up to the tables so that
        //								user data fields show correct data after
        //								custom schedule reconfigure.
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //  
        public virtual List<String> UserData
        {
            get
            {
                List<String> UserDataList = new List<String>();

                UserDataList.Add(Table2048.ConstantsConfig.UserData1);
                UserDataList.Add(Table2048.ConstantsConfig.UserData2);
                UserDataList.Add(Table2048.ConstantsConfig.UserData3);

                return UserDataList;
            }
        }


        /// <summary>
        /// Gets Vender Field 1 Display data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/06 KRC 7.36.00			Created
        // 04/19/10 AF  2.40.39        Made virtual for M2 Gateway override
        // 11/14/13 AF  3.50.03	        Class re-architecture - promoted from CENTRON_AMI
        //
        public virtual string DisplayVenderField1
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                object objData = null;
                string strValue = "";

                Result = m_lidRetriever.RetrieveLID(m_LID.DISP_VENDOR_FIELD_1, out objData);
                if (PSEMResponse.Ok == Result)
                {
                    strValue = objData.ToString();
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the Display Vender Field 1"));
                }

                return strValue;
            }
        }

        /// <summary>
        /// Gets Vender Field 2 Display data
        /// </summary> 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/06 KRC 7.36.00			Created
        // 04/19/10 AF  2.40.39         Made virtual for M2 Gateway override
        // 11/14/13 AF  3.50.03	        Class re-architecture - promoted from CENTRON_AMI
        //
        public virtual string DisplayVenderField2
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                object objData = null;
                string strValue = "";

                Result = m_lidRetriever.RetrieveLID(m_LID.DISP_VENDOR_FIELD_2, out objData);
                if (PSEMResponse.Ok == Result)
                {
                    strValue = objData.ToString();
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the Display Vender Field 2"));
                }

                return strValue;
            }
        }

        /// <summary>
        /// Gets Vender Field 3 Display data
        /// </summary> 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/06 KRC 7.36.00			Created
        // 04/19/10 AF  2.40.39         Made virtual for M2 Gateway override
        // 11/14/13 AF  3.50.03	        Class re-architecture - promoted from CENTRON_AMI
        //
        public virtual string DisplayVenderField3
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                object objData = null;
                string strValue = "";

                Result = m_lidRetriever.RetrieveLID(m_LID.DISP_VENDOR_FIELD_3, out objData);
                if (PSEMResponse.Ok == Result)
                {
                    strValue = objData.ToString();
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading the Display Vender Field 3"));
                }

                return strValue;
            }
        }

        /// <summary>
        /// Returns the number of Self Reads in the meter
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/29/06 KRC 7.35.00 N/A    Created
        // 		
        public virtual uint NumberOfSelfReads
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte[] Data = null;
                uint uiNumSelfReads = 0;

                Result = m_lidRetriever.RetrieveLID(
                                        m_LID.NUMBER_OF_VALID_SELF_READS, out Data);

                if (PSEMResponse.Ok == Result)
                {
                    uiNumSelfReads = (uint)Data[0];
                }
                else
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Is Canadian"));
                }

                return uiNumSelfReads;
            }
        }

        /// <summary>
        /// Returns the number of Last Demand Resets in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/29/06 KRC 7.35.00 N/A    Created
        // 		
        public virtual uint NumberofLastDemandResets
        {
            get
            {
                return NUM_LAST_DEMAND_RESETS;
            }
        }

        /// <summary>
        /// Returns true if the meter is configured for Canada
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/23/06 mcm 7.35.00 N/A    Created
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        //
        public virtual bool IsCanadian
        {
            get
            {
                if (!m_IsCanadian.Cached)
                {
                    PSEMResponse Result = PSEMResponse.Ok;

                    byte[] Data = null;

                    //Get the current time
                    Result = m_lidRetriever.RetrieveLID(m_LID.CANADIAN, out Data);
                    if (PSEMResponse.Ok == Result)
                    {
                        m_IsCanadian.Value = Data[0] > 0;
                    }
                    else
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Is Canadian"));
                    }
                }

                return m_IsCanadian.Value;
            }
        }


        /// <summary>
        /// Gets whether or not the meter is Sealed for Canada
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/18/10 RCG 2.40.26 N/A    Created

        public virtual bool IsSealedCanadian
        {
            get
            {
                if (!m_IsSealedCanadian.Cached)
                {
                    PSEMResponse Result = PSEMResponse.Ok;

                    byte[] Data = null;

                    //Get the current time
                    Result = m_lidRetriever.RetrieveLID(m_LID.SEALED_CANADIAN, out Data);
                    if (PSEMResponse.Ok == Result)
                    {
                        m_IsSealedCanadian.Value = Data[0] > 0;
                    }
                    else
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Is Sealed Canadian"));
                    }
                }

                return m_IsSealedCanadian.Value;
            }
        }

        /// <summary>
        /// Returns true if IO is supported, false if not.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 04/22/09 jrf 2.20.02 N/A    Created
        /// 04/19/10 AF  2.40.38        Made virtual so it can be overridden in M2 Gateway
        ///
        public virtual bool IOSupported
        {
            get
            {
                return MeterKeyTable.IOSupported;
            }
        }

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
        ///  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        ///
        ///</remarks>
        public virtual String SWRevision
        {
            get
            {
                return Table2048.Table2048Header.SWVerRev.ToString("0.00", CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Get program ID from the meter
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/18/06 mrj 7.30.00 N/A    Created
        //  11/13/13 AF  3.50.03        Class re-architecture - Moved definition from ItronDevice
        // 
        public virtual int ProgramID
        {
            get
            {
                return Table2048.ConstantsConfig.ProgramID;
            }
        }

        #endregion

        #region Events
        #endregion

        #region Internal Methods

        /// <summary>
        /// Validates that Energy LID is supported in meter
        /// </summary>
        /// <param name="EnergyLID">Energy LID to check for</param>
        /// <returns>bool - True: LID is supported; False: LID is not supported</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/27/06 KRC 7.35.00 N/A    Created
        //  11/30/06 jrf 8.00.00 N/A    Added check for null LID
        //
        internal bool ValidateEnergy(LID EnergyLID)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            byte[] Data = null;
            bool bResult = false;
            int index = 0;

            if (null == EnergyLID)
            {
                bResult = false;
            }
            //KRC: TODO - Is this the best way to handle this issue?
            // First, check to make sure the EnergyLID supplies is of type Energy
            else if ((EnergyLID.lidValue & (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK) ==
                ((uint)DefinedLIDs.BaseLIDs.ENERGY_DATA & (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK))
            {
                // If the list is not populated, then add the LIDs to the list
                if (0 == m_lstSupportedEnergies.Count && 0 != NumberEnergies)
                {
                    PSEMResult = m_lidRetriever.RetrieveLID(m_LID.ALL_SEC_ENERGIES_TOTAL,
                                    LIDRetriever.RequestMode.LIDOnly, out Data);

                    if (PSEMResponse.Ok == PSEMResult)
                    {
                        for (index = 0; index < (int)NumberEnergies; index++)
                        {
                            m_lstSupportedEnergies.Add(m_lidRetriever.DataReader.ReadUInt32());
                        }
                    }
                    else
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                                    "Error Reading Supported Energies");
                    }
                }

                // The list has been created, so now check to see if our LID is in the list
                for (index = 0; index < (int)NumberEnergies; index++)
                {
                    if (EnergyLID.lidValue == m_lstSupportedEnergies[index])
                    {
                        bResult = true;
                    }
                }
            }
            else
            {
                // If the value EnergyLID is not of type Energy, then this must be a Self Read
                // or Last Demand reset, and we know the Quantity is supported, so we can skip the
                // test.
                bResult = true;
            }

            return bResult;
        }

        /// <summary>
        /// Determine if provided LID is in meter
        /// </summary>
        /// <param name="DemandLID">Demand LID to check for</param>
        /// <returns>bool - True: LID is supported; False: LID is not supported</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/27/06 KRC 7.35.00 N/A    Created
        //  11/30/06 jrf 8.00.00 N/A    Added check for null LID
        //
        internal bool ValidateDemand(LID DemandLID)
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            byte[] Data = null;
            bool bResult = false;
            int index = 0;

            if (null == DemandLID)
            {
                bResult = false;
            }
            else
            {
                // If the list is not populated, then add the LIDs to the list
                if (0 == m_lstSupportedDemands.Count && 0 != NumberDemands)
                {
                    PSEMResult = m_lidRetriever.RetrieveLID(
                                        m_LID.ALL_SEC_DEMANDS_TOTAL,
                                        LIDRetriever.RequestMode.LIDOnly, out Data);

                    if (PSEMResponse.Ok == PSEMResult)
                    {
                        for (index = 0; index < (int)NumberDemands; index++)
                        {
                            m_lstSupportedDemands.Add(m_lidRetriever.DataReader.ReadUInt32());
                        }
                    }
                    else
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                                    "Error Reading Supported Demands");
                    }

                }

                if ((DemandLID.lidValue & (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK) ==
                ((uint)DefinedLIDs.BaseLIDs.DEMAND_DATA & (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK))
                {
                    // If it is of type Demand then we can do a straight check.
                    // The list has been created, so now check to see if our LID is in the list
                    for (index = 0; index < (int)NumberDemands; index++)
                    {
                        if (DemandLID.lidValue == m_lstSupportedDemands[index])
                        {
                            bResult = true;
                        }
                    }
                }
                else
                {
                    // If the value DemandLID is not of type Demand, then this must be a Self Read
                    // or Last Demand reset.  All we can check is the Demand Type and hope this is
                    // enough.
                    for (index = 0; index < (int)NumberDemands; index++)
                    {
                        if ((DemandLID.lidValue & (uint)DefinedLIDs.WhichOneEnergyDemand.WHICH_ONE_MASK) ==
                            ((uint)m_lstSupportedDemands[index] & (uint)DefinedLIDs.WhichOneEnergyDemand.WHICH_ONE_MASK))
                        {
                            bResult = true;
                        }
                    }
                }
            }

            return bResult;
        }

        /// <summary>
        /// Creates a new DisplayItem
        /// </summary>
        /// <returns>ANSIDisplayItem</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------  ---------------------------------------
        // 03/29/07 KRC 8.00.22		    Added to handle SENTINEL specific display item behavior
        //
        internal virtual ANSIDisplayItem CreateDisplayItem(LID lid, string strDisplayID, ushort usFormat, byte byDim)
        {
            return new ANSIDisplayItem(lid, strDisplayID, usFormat, byDim);
        }

        /// <summary>
        /// Reads up to 8 Display Items from the meter and updates the data
        /// </summary>
        /// <param name="ItemsToRead">The list of up to 8 Display Items to update</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/24/07 RCG 8.00.09 N/A    Created
        //
        internal virtual void ReadDisplayData(List<ANSIDisplayItem> ItemsToRead)
        {
            PSEMResponse Response = PSEMResponse.Ok;
            LID[] LIDsToRead = new LID[ItemsToRead.Count];
            List<object> DataList = new List<object>();

            for (int iIndex = 0; iIndex < ItemsToRead.Count; iIndex++)
            {
                LIDsToRead[iIndex] = ItemsToRead[iIndex].DisplayLID;
            }

            // Read the LID values
            if (LIDsToRead.Length > 0)
            {
                Response = m_lidRetriever.RetrieveMulitpleLIDs(LIDsToRead, out DataList);

                if (Response != PSEMResponse.Ok)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response, "Error Reading Display LIDs");
                }

            }

            // Format the data for the Display Items
            for (int iIndex = 0; iIndex < ItemsToRead.Count; iIndex++)
            {
                if (HandleIrregularFormatting(ItemsToRead[iIndex], DataList[iIndex]) == false)
                {
                    ItemsToRead[iIndex].FormatData(DataList[iIndex]);
                }
            }
        }

        /// <summary>
        /// Creates a LID object from the given 32-bit number
        /// </summary>
        /// <param name="uiLIDNumber">The 32-bit number that represents the LID</param>
        /// <returns>The LID object for the specified LID</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/07/07 RCG 8.00.11 N/A    Created
        public virtual LID CreateLID(uint uiLIDNumber)
        {
            return new LID(uiLIDNumber);
        }

        #endregion

        #region Internal Properties
        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the magic key from the meter.
        /// </summary>
        /// <returns>The magic key</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/12/10 MA 2.40.36  N/A    Created

        protected byte[] GetMagicKey()
        {
            byte[] mKeyByts = null;

            //Reading Table 2082 to get the special code for Canadian Sealing
            OpenWayMFGTable2082 Table2082 = new OpenWayMFGTable2082(m_PSEM);

            //calculating CRC from the data read from Table 2082
            ushort mkey = CRC.CalculateCRC(Table2082.Key);

            mKeyByts = BitConverter.GetBytes(mkey);

            return mKeyByts;
        }

        /// <summary>
        /// Initialize all of our member variables for the Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/28/06 KRC 7.35.00 N/A    Created
        //
        protected override void InitializeData()
        {
            base.InitializeData();

            m_LoadProfileStatus = new LoadProfileStatusLIDS(m_lidRetriever, LPPulseWeightMultiplier);

            // Supported Quantities start out empty.
            m_lstSupportedEnergies = new List<uint>();
            m_lstSupportedDemands = new List<uint>();
        }

        /// <summary>
        /// Translates a PSEMResponse into a DSTUpdateResult.
        /// </summary>
        /// <param name="PSEMCode">PSEM response to translate</param>
        /// <returns>DSTUpdateResult that corresponds to the PSEM response.
        /// SUCCESS, ERROR, and INSUFFICIENT_SECURITY_ERROR are the only
        /// translations.</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/01/06 mcm 7.30.00 N/A	Created
        protected DSTUpdateResult GetDSTResult(PSEMResponse PSEMCode)
        {
            DSTUpdateResult Result = DSTUpdateResult.ERROR;

            switch (PSEMCode)
            {
                case PSEMResponse.Ok:
                    {
                        Result = DSTUpdateResult.SUCCESS;
                        break;
                    }
                case PSEMResponse.Isss:
                    {
                        Result = DSTUpdateResult.INSUFFICIENT_SECURITY_ERROR;
                        break;
                    }
                case PSEMResponse.Bsy:
                case PSEMResponse.Dlk:
                case PSEMResponse.Dnr:
                case PSEMResponse.Err:
                case PSEMResponse.Iar:
                case PSEMResponse.Isc:
                case PSEMResponse.Onp:
                case PSEMResponse.Rno:
                case PSEMResponse.Sns:
                default:
                    {
                        Result = DSTUpdateResult.ERROR;
                        break;
                    }
            }

            return Result;

        } // GetDSTResult



        /// <summary>
        /// Checks to see if the meter's clock is running and if it is 
        /// configured for DST time adjustment and TOU rates.
        /// </summary>
        /// <returns>A device type string that can be used to check to see if
        /// this device is supported by a TOU schedule</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/22/06 mcm 7.30.00 N/A	Created
        protected string GetExtendedDeviceType()
        {
            string DeviceType = Table01.Model;

            DeviceType = DeviceType.ToUpper(CultureInfo.InvariantCulture);

            if ("CENTRONP" == DeviceType)
            {
                DeviceType = "CENTRON (V&&I)";
            }
            else if ("CENTRONM" == DeviceType)
            {
                DeviceType = "CENTRON (C12.19)";
            }
            else if ("SENTINEL" == DeviceType)
            {
                if (SATURN_ADVANCED_RATES == MeterKeyTable.TOURatesSupported)
                {
                    DeviceType = "SENTINEL - Advanced";
                }
                else
                {
                    DeviceType = "SENTINEL - Basic";
                }
            }

            return DeviceType;

        } // GetExtendedDeviceType

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the multiplier used to calculate the Load Profile Pulse Weight
        /// </summary>		
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/11/07 KRC 8.00.27 2864   Created
        //
        protected virtual float LPPulseWeightMultiplier
        {
            get
            {
                return 0.1f;
            }
        }

        /// <summary>
        /// Object for retrieving Load Profile status information
        /// </summary>
        protected LoadProfileStatusLIDS m_LoadProfileStatus;

        #endregion

        #region Private Methods

        /// <summary>
        /// Does read of Capability items
        /// </summary>
        private void CapabilitiesRead()
        {
            PSEMResponse PSEMResult = PSEMResponse.Ok;
            byte[] Data = null;
            BinaryReader DataReader;
            LID[] arLids = new LID[3];

            arLids[0] = m_LID.NUM_TOU_RATES;
            arLids[1] = m_LID.NUMBER_ENERGIES;
            arLids[2] = m_LID.NUMBER_DEMANDS;

            PSEMResult = m_lidRetriever.RetrieveMulitpleLIDs(arLids, out Data);

            if (PSEMResponse.Ok == PSEMResult)
            {
                DataReader = new BinaryReader(new MemoryStream(Data));
                m_uiNumTOURates.Value = (uint)DataReader.ReadByte();
                m_uiNumEnergies.Value = (uint)DataReader.ReadByte();
                m_uiNumDemands.Value = (uint)DataReader.ReadByte();
            }
            else
            {
                throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error with Capabilities Read"));
            }
        }

        /// <summary>
        /// Checks to see if the meter's clock is running and if it is 
        /// configured for DST time adjustment.
        /// </summary>
        /// <returns>SUCCESS, CLOCK_ERROR, SUCCESS_NOT_CONFIGURED_FOR_DST</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/01/06 mcm 7.30.00 N/A	Created
        private DSTUpdateResult ValidateMeterForDSTUpdate()
        {
            DSTUpdateResult Result = DSTUpdateResult.ERROR;


            m_Logger.WriteLine(Logger.LoggingLevel.Functional,
                                "Validating Meter for DST Update");

            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                    "Has Clock = " + ClockRunning.ToString());


                if (!ClockRunning)
                {
                    Result = DSTUpdateResult.CLOCK_ERROR;
                }
                else if (false == Table2048.HasDST)
                {
                    Result = DSTUpdateResult.SUCCESS_NOT_CONFIGURED_FOR_DST;
                }
                else
                {
                    Result = DSTUpdateResult.SUCCESS;
                }
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

            return Result;

        } // ValidateMeterForDSTUpdate

        /// <summary>
        /// Updates DST dates in the Table 2048 Calendar Configuration area.
        /// Only future dates that are in or beyond 2007 and that differ from
        /// the corresponding date in the given DST file are changed.  This 
        /// method does not write the table to the meter.  It just updates the
        /// soft copy of the table.
        /// </summary>
        /// <param name="FileName">PC-PRO+ DST file to use for update</param>
        /// <returns>SUCCESS, SUCCESS_PREVIOUSLY_UPDATED, ERROR, 
        /// ERROR_DST_DATA_MISSING, ERROR_DST_DATES_EXPIRED</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/01/06 mcm 7.30.00 N/A	Created
        private DSTUpdateResult UpdateDSTDates(string FileName)
        {
            DSTUpdateResult Result = DSTUpdateResult.ERROR;
            byte YearIndex = 0;
            byte DSTIndex = 0;
            int StartYear = ENERGY_ACT_OF_2005_START_YEAR;
            bool Updated = false;
            int Year = 0;
            CDSTSchedule DSTDates;
            DateTime ToDate;
            DateTime FromDate;
            CalendarYear CalYear;


            try
            {
                // Open the DST.xml file in the DST server
                try
                {
                    DSTDates = new CDSTSchedule(FileName);
                }
                catch
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                        "Error opening DST file: " + FileName);

                    // If the file is invalid, don't throw the 
                    // excecption. Just return the appropriate result.
                    return DSTUpdateResult.ERROR_DST_DATA_MISSING;
                }

                // Our start year is this year or 2007, whichever is later.
                if (DateTime.Now.Year > ENERGY_ACT_OF_2005_START_YEAR)
                {
                    StartYear = DateTime.Now.Year;
                }

                // Find the index of the start year in the calendar config's
                // years.
                if (false == Table2048.CalendarConfig.FindYearIndex
                    (StartYear, out YearIndex))
                {
                    Result = DSTUpdateResult.ERROR_DST_DATES_EXPIRED;
                }
                else if (false == DSTDates.FindDSTIndex(StartYear, out DSTIndex))
                {
                    Result = DSTUpdateResult.ERROR_DST_DATA_MISSING;
                }
                else
                {
                    Result = DSTUpdateResult.SUCCESS;
                }

                // Update the DST dates
                while ((DSTUpdateResult.SUCCESS == Result) &&
                    (YearIndex < Table2048.CalendarConfig.MaxYears))
                {
                    // CalYear is a reference to this year in the configuration.
                    // It is used just to make the code a little more readable.
                    CalYear = Table2048.CalendarConfig.Years[YearIndex];

                    // The meter stores the year as a byte referenced to YKK
                    Year = CalYear.Year + CalendarConfig.CALENDAR_REFERENCE_YEAR;

                    if (CalendarConfig.CALENDAR_REFERENCE_YEAR == Year)
                    {
                        // This doesn't mean we failed. The meter can be
                        // configured with less than the max years, so we would
                        // want to update until we hit the first unconfigured
                        // year.
                        break;
                    }
                    // If we don't have the year in our DST file, 
                    // fail the operation
                    else if (DSTIndex >= DSTDates.DSTDatePairs.Count)
                    {
                        Result = DSTUpdateResult.ERROR_DST_DATA_MISSING;
                    }
                    else
                    {
                        // Local date variables are used to make the code a 
                        // little more readable. These are from the XML file.
                        ToDate = DSTDates.DSTDatePairs[DSTIndex].ToDate;
                        FromDate = DSTDates.DSTDatePairs[DSTIndex].FromDate;

                        // Update any dates that are wrong
                        if (CalYear.Events[0].IsDST() &&
                            ((CalYear.Events[0].Month != ToDate.Month - 1) ||
                             (CalYear.Events[0].Day != ToDate.Day - 1)))
                        {
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "Updating Spring DST from " +
                                (CalYear.Events[0].Month + 1) +
                                "/" + (CalYear.Events[0].Day + 1) + "/" +
                                Year + " to " + ToDate.ToString("d", CultureInfo.InvariantCulture));

                            // Set the meter's DST start date for this year
                            CalYear.Events[0].Month = (byte)(ToDate.Month - 1);
                            CalYear.Events[0].Day = (byte)(ToDate.Day - 1);

                            // Note that we changed a date
                            Updated = true;
                        }

                        if (CalYear.Events[1].IsDST() &&
                            ((CalYear.Events[1].Month != FromDate.Month - 1) ||
                             (CalYear.Events[1].Day != FromDate.Day - 1)))
                        {
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "Updating Fall DST from " +
                                (CalYear.Events[1].Month + 1) +
                                "/" + (CalYear.Events[1].Day + 1) +
                                "/" + Year + " to " + FromDate.ToString("d", CultureInfo.InvariantCulture));

                            // Set the meter's DST stop date for this year
                            CalYear.Events[1].Month = (byte)(FromDate.Month - 1);
                            CalYear.Events[1].Day = (byte)(FromDate.Day - 1);

                            // Note that we changed a date
                            Updated = true;
                        }
                    }

                    // Increment the DST server's index and the meter 
                    // configuration year index.
                    DSTIndex++;
                    YearIndex++;
                }

                // If we went through all of the years without a problem, but
                // we didn't change any dates, the meter must have already been
                // updated. This will likely happen in the field as utilities
                // schedule updates for thousands of meters.
                if ((DSTUpdateResult.SUCCESS == Result) && (false == Updated))
                {
                    Result = DSTUpdateResult.SUCCESS_PREVIOUSLY_UPDATED;
                }
            }
            catch (Exception e)
            {
                //check for file error exception from the DST server
                Result = DSTUpdateResult.ERROR_DST_DATA_MISSING;
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

            return Result;

        } // UpdateDSTDates        

        /// <summary>
        /// Reads the DST dates from the meter
        /// </summary>
        /// <returns>The TOU Schedule object.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------  ---------------------------------------
        // 01/04/07 RCG 8.00.04		    Made more generic and promoted from CENTRON_AMI
        // 02/13/09 jrf 2.10.04 N/A     Adding the hour and minute to the DST dates.
        // 04/19/11 RCG 2.50.30 170876  Adding check to make sure we have a TO and From DST date
        private List<CDSTDatePair> ReadDSTDates()
        {
            List<CDSTDatePair> lstDSTDates = new List<CDSTDatePair>();

            for (int iYearCounter = 0; iYearCounter < Table2048.CalendarConfig.MaxYears; iYearCounter++)
            {
                bool bFoundToDate = false;
                bool bFoundFromDate = false;
                CalendarEvent[] CalEvents = Table2048.CalendarConfig.Years[iYearCounter].Events;
                int iYear = 2000 + (int)Table2048.CalendarConfig.Years[iYearCounter].Year;

                CDSTDatePair DST = new CDSTDatePair();
                // Index 0 and Index 1 are the To and From DST Date respectively
                for (int iDayEvent = 0; iDayEvent <= Table2048.CalendarConfig.DSTEventsPerYear; iDayEvent++)
                {
                    eEventType eType = Table2048.CalendarConfig.GetEventType(CalEvents[iDayEvent].Type);
                    if (eEventType.TO_DST == eType)
                    {
                        // It is a valid event
                        DST.ToDate = new DateTime(iYear, CalEvents[iDayEvent].Month + 1,
                                                     CalEvents[iDayEvent].Day + 1, Table2048.CalendarConfig.DSTHour,
                                                     Table2048.CalendarConfig.DSTMinute, 0);
                        bFoundToDate = true;
                    }
                    else if (eEventType.FROM_DST == eType)
                    {
                        // It is a valid event
                        DST.FromDate = new DateTime(iYear, CalEvents[iDayEvent].Month + 1,
                                                     CalEvents[iDayEvent].Day + 1, Table2048.CalendarConfig.DSTHour,
                                                     Table2048.CalendarConfig.DSTMinute, 0);
                        bFoundFromDate = true;
                    }
                }

                // Make sure there is a valid To and From DST date
                if (bFoundToDate && bFoundFromDate)
                {
                    lstDSTDates.Add(DST);
                }

                // It may be possible that some of the years are not filled in so we need to
                // make sure that the year is valid by checking to see if the next year is
                // greater than the current
                if (iYearCounter + 1 < Table2048.CalendarConfig.MaxYears &&
                    (int)Table2048.CalendarConfig.Years[iYearCounter + 1].Year + 2000 < iYear)
                {
                    break;
                }
            }

            return lstDSTDates;
        }

        /// <summary>
        /// Checks to see if the meter's clock is running and if it is 
        /// configured for DST time adjustment and TOU rates.
        /// </summary>
        /// <param name="TOUFileName">PC-PRO+ TOU schedule file name</param>
        /// <param name="DSTFileName">PC-PRO+ DST file name (DST.xml). Pass
        /// an empty string if the meter doesn't support DST.</param>
        /// <param name="TOUSchedule">Returns an instance of the TOU server
        /// with the TOUFileName file open if successful</param>
        /// <param name="DSTSchedule">Returns an instance of the DST server
        /// with the DSTFileName file open if successful. NOTE that it is valid
        /// to pass an empty DST file name for meters not configured with DST.
        /// In that case, the returned DSTSchedule will be null.</param>
        /// <returns>SUCCESS, SUCCESS_NOT_CONFIGURED_FOR_TOU, CLOCK_ERROR, 
        /// ERROR_DST_DATA_MISSING, ERROR_SCHED_NOT_SUPPORTED</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/22/06 mcm 7.30.00 N/A	Created
        //  10/20/06 mcm 7.35.07 SCR 99 Expired TOU schedule should ret err 30
        //  03/13/07 mrj 8.00.18		Removed wait, keep alive is now used
        //  04/05/07 mrj 8.00.25 2842	Allow Sentinel Advanced to use basic
        //								schedules.
        // 
        private TOUReconfigResult ValidateForTOUReconfigure(string TOUFileName,
            string DSTFileName, out CTOUSchedule TOUSchedule,
            out CDSTSchedule DSTSchedule)
        {
            TOUReconfigResult Result = TOUReconfigResult.ERROR;
            bool HasDST = false;
            string DeviceType = "";
            bool bMeterSupported = true;


            m_Logger.WriteLine(Logger.LoggingLevel.Functional,
                "Validating Meter for TOU Reconfigure");

            // Initialize out parameters
            TOUSchedule = null;
            DSTSchedule = null;

            try
            {
                // The extended device type matches the TOU schedule's names
                // for supported devices.
                DeviceType = GetExtendedDeviceType();

                if (!ClockRunning)
                {
                    Result = TOUReconfigResult.CLOCK_ERROR;
                }
                else if (0 == Table2048.TOU_ID)
                {
                    Result = TOUReconfigResult.SUCCESS_NOT_CONFIGURED_FOR_TOU;
                }
                else
                {
                    HasDST = Table2048.HasDST;

                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                        "Opening TOU Schedule: " + TOUFileName);

                    // Open the TOU file
                    try
                    {
                        TOUSchedule = new CTOUSchedule(TOUFileName);
                    }
                    catch
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                            "Error opening TOU Schedule");

                        // If the file is invalid, don't throw the 
                        // excecption. Just return the appropriate result.
                        return TOUReconfigResult.ERROR_TOU_NOT_VALID;
                    }

                    //Check to see if the meter supports this schedule.
                    if (false == TOUSchedule.IsSupported(DeviceType))
                    {
                        //The meter does not support this schedule unless
                        //this is and advanced Sentinel and this is a 
                        //basic schedule
                        if ("SENTINEL - Advanced" == DeviceType &&
                            TOUSchedule.IsSupported("SENTINEL - Basic"))
                        {
                            bMeterSupported = true;
                        }
                        else
                        {
                            bMeterSupported = false;
                        }
                    }

                    // Is our meter supported?
                    if (!bMeterSupported)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                            "This Schedule does not support " + DeviceType
                            + " Devices");
                        Result = TOUReconfigResult.ERROR_SCHED_NOT_SUPPORTED;
                    }
                    else if (HasDST &&
                        ((null == DSTFileName) || (0 == DSTFileName.Length)))
                    {
                        Result = TOUReconfigResult.ERROR_DST_DATA_MISSING;
                    }
                    else if (HasDST)
                    {
                        // Open the DST file
                        try
                        {
                            DSTSchedule = new CDSTSchedule(DSTFileName);
                        }
                        catch
                        {
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "Error opening DST file: " + DSTFileName);

                            // If the file is invalid, don't throw the 
                            // excecption. Just return the appropriate result.
                            return TOUReconfigResult.ERROR_DST_DATA_MISSING;
                        }

                        Result = TOUReconfigResult.SUCCESS;
                    }
                    else
                    {
                        Result = TOUReconfigResult.SUCCESS;
                    }

                    // mcm 10/20/2006 - SCR 99 - Expired TOU schedule should
                    // fail with error 30 instead of causing exception.
                    if (TOUReconfigResult.SUCCESS == Result)
                    {
                        int iLastYearOfSchedule =
                            TOUSchedule.StartYear + TOUSchedule.Duration - 1;

                        if (DateTime.Now.Year > iLastYearOfSchedule)
                        {
                            Result = TOUReconfigResult.ERROR_TOU_EXPIRED;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

            return Result;

        } // ValidateForTOUReconfigure

        /// <summary>
        /// Clears Table2048.m_CalendarConfig and write the TOU and DST info
        /// to it to prepare it for writing to the meter.  The CalendarConfig
        /// contains season start dates, holdays, and DST dates.  The rest of 
        /// the TOU configuration is in the TOUConfig table.
        /// </summary>
        /// <remarks>This method ASSUMES the TOUSchedule and DSTSchedule 
        /// provided are valid for the meter. If the DSTSchedule object is be 
        /// null, the meter will not be configured with DST</remarks>
        /// <param name="TOUSchedule">TOU server with a valid TOU file open</param>
        /// <param name="DSTSchedule">DST server with a DST file open. Note 
        /// that this object can be null for meters that will not have DST</param>
        /// <returns>SUCCESS, ERROR_DST_DATA_MISSING</returns>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/26/06 mcm 7.30.00 N/A	Created
        /// 10/13/06 mcm 7.35.04 55,64,67 - Insert season start date didn't work
        /// 
        private TOUReconfigResult OverwriteCalendarConfig(CTOUSchedule TOUSchedule,
                                                        CDSTSchedule DSTSchedule)
        {
            TOUReconfigResult Result = TOUReconfigResult.SUCCESS;
            CalendarYear CalYear;
            TOU.CYear TOUYear;
            int YearsToConfigure;
            int StartYear = DateTime.Now.Year;

            // Are 10,000 indices really needed? We're looping through the 
            // meter's years, the year's events, the TOU server, and the DST 
            // server. 
            int TOUIndex = 0;	 // Index of TOU Years
            byte DSTStartIndex = 0;	// Index of this years' DST dates
            byte DSTIndex = 0;	 // Index of DST Years
            int YearIndex = 0;	 // Index of CalendarConfig years
            int TOUEventIndex = 0; // Index of schedule events
            int CalEventIndex = FIRST_TOU_CAL_INDEX; // Index of meter's calendar events
            bool bInsertSeasonStartDate = true;

            try
            {
                // Write the year definitions to the calendar config area.

                // Clear the table so we don't have to write over unused areas
                Table2048.CalendarConfig.Clear();

                if (null != TOUSchedule)
                {
                    Table2048.CalendarConfig.CalendarID =
                        (ushort)TOUSchedule.TOUID;

                    // Only set the offset if there's DST.
                    if (null != DSTSchedule)
                    {
                        Table2048.CalendarConfig.DSTHour =
                            (byte)(DSTSchedule.ToTime / SECONDS_PER_HOUR);
                        Table2048.CalendarConfig.DSTMinute =
                            (byte)(DSTSchedule.ToTime % SECONDS_PER_HOUR);
                        Table2048.CalendarConfig.DSTOffset =
                            (byte)DSTSchedule.JumpLength;

                        // Get the index of this year's DST dates
                        if (false == DSTSchedule.FindDSTIndex(StartYear,
                            out DSTStartIndex))
                        {
                            return TOUReconfigResult.ERROR_DST_DATA_MISSING;
                        }
                    }
                }

                // The count of years to configure is depends on the schedule's
                // start year, duration, and the meter's capacity.
                // ASSUMES the schedule starts in the past.
                YearsToConfigure = TOUSchedule.Duration + TOUSchedule.StartYear
                    - StartYear;
                if (YearsToConfigure > Table2048.CalendarConfig.MaxYears)
                {
                    YearsToConfigure = Table2048.CalendarConfig.MaxYears;
                }

                // Get the index of this year's DST dates
                // mcm 4/25/2007 - CTOUSchedule will throw an exception if the year
                // isn't in the schedule. Catch it so we can give a better error code.
                try
                {
                    TOUIndex = TOUSchedule.Years.IndexOf(new CYear(StartYear, null));
                }
                catch
                {
                    return TOUReconfigResult.ERROR_TOU_NOT_VALID;
                }

                // Calendar events aren't always sorted. Search the first year
                // for a season start date on 1/1
                TOUYear = TOUSchedule.Years[TOUIndex];
                for (TOUEventIndex = 0; TOUEventIndex < TOUYear.Events.Count;
                    TOUEventIndex++)
                {
                    CalendarEvent CalEvent =
                        GetYearEvent(TOUYear.Events[TOUEventIndex]);

                    if (CalEvent.IsSeason() &&
                         (0 == CalEvent.Month) &&
                         (0 == CalEvent.Day))
                    {
                        // We have one, stop searching
                        bInsertSeasonStartDate = false;
                        break;
                    }
                }

                for (YearIndex = 0; YearIndex < YearsToConfigure; YearIndex++)
                {
                    TOUYear = TOUSchedule.Years[TOUIndex++];
                    CalYear = Table2048.CalendarConfig.Years[YearIndex];

                    CalYear.Year = (byte)(StartYear + YearIndex -
                        CalendarConfig.CALENDAR_REFERENCE_YEAR);

                    // If we're configuring DST, it has to come first.
                    if (null != DSTSchedule)
                    {
                        DSTIndex = (byte)(DSTStartIndex + YearIndex);
                        CalYear.Events[0].Type = (byte)
                            CalendarEvent.CalendarEventType.ADD_DST;
                        CalYear.Events[0].Month = (byte)
                            (DSTSchedule.DSTDatePairs[DSTIndex].ToDate.Month - 1);
                        CalYear.Events[0].Day = (byte)
                            (DSTSchedule.DSTDatePairs[DSTIndex].ToDate.Day - 1);

                        CalYear.Events[1].Type = (byte)
                            CalendarEvent.CalendarEventType.SUB_DST;
                        CalYear.Events[1].Month = (byte)
                            (DSTSchedule.DSTDatePairs[DSTIndex].FromDate.Month - 1);
                        CalYear.Events[1].Day = (byte)
                            (DSTSchedule.DSTDatePairs[DSTIndex].FromDate.Day - 1);
                    }

                    // 0 & 1 are reserved for DST even if the meter is using it
                    CalEventIndex = FIRST_TOU_CAL_INDEX;

                    for (TOUEventIndex = 0;
                        TOUEventIndex < TOUYear.Events.Count;
                        TOUEventIndex++)
                    {
                        CalendarEvent CalEvent =
                            GetYearEvent(TOUYear.Events[TOUEventIndex]);

                        // 10/13/06 mcm - SCRs 55,64,67 - Insert season start 
                        // date code didn't work, and it caused the first year 
                        // to be skipped.

                        if (bInsertSeasonStartDate)
                        {
                            byte byFirstSeasonIndex;

                            // We need to insert a season start date.  Is there
                            // going to be room for it?  
                            if (MAX_CAL_EVENTS <= TOUYear.Events.Count + FIRST_TOU_CAL_INDEX)
                            {
                                // We're already maxed out. We can't add another
                                // event, so fail it.
                                return TOUReconfigResult.ERROR_SCHED_NOT_SUPPORTED;
                            }

                            // Find the index of the season that should start 
                            // our first year.
                            byFirstSeasonIndex = FindStartSeason(TOUSchedule, TOUIndex - 1);

                            CalendarEvent FirstSeasonEvent = new CalendarEvent();

                            FirstSeasonEvent.Month = 0;
                            FirstSeasonEvent.Day = 0;
                            FirstSeasonEvent.Type = byFirstSeasonIndex;
                            CalYear.Events[CalEventIndex++] = FirstSeasonEvent;

                            // OK, that was fun, but let's not do it again.
                            bInsertSeasonStartDate = false;
                        }

                        CalYear.Events[CalEventIndex++] = CalEvent;
                    }
                }
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

            return Result;

        } // OverwriteCalendarConfig

        /// <summary>
        /// Clears Table2048.m_TOUConfig and writes the TOU scheudule info
        /// into the table to prepare it for writing to the meter. The TOUConfig
        /// contains configuration data not related to a particular year. This
        /// table has the Typical Week and pattern definitions for all of the
        /// seasons. The per-year info (season start dates, holdays, and DST 
        /// dates) are in the CalendarConfig table.
        /// </summary>
        /// <remarks>ASSUMES the TOU schedule is valid for this meter</remarks>
        /// <param name="TOUSchedule">TOU server with a supported TOU file 
        /// open.</param>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/22/06 mcm 7.30.00 N/A	Created
        /// 10/23/06 mcm 7.35.07 105    Need to add end output switchpoints
        /// 
        private void OverwriteTOUConfig(CTOUSchedule TOUSchedule)
        {
            ushort TypicalWeek;
            TOUConfig.TOU_Season Season;
            CSeasonCollection SchedSeasons;
            CSeason SchedSeason;
            CPatternCollection Patterns;
            int PatternIndex;
            int TODEvent;
            bool EventsNeedSorting = false;

            try
            {
                // Write the season definitions to the TOU configuration section.

                // Clear the table so we don't have to write over unused areas
                Table2048.TOUConfig.Clear();

                if (null != TOUSchedule)
                {
                    SchedSeasons = TOUSchedule.Seasons;
                    Patterns = TOUSchedule.Patterns;

                    // The Day to DayType association is the same for all years
                    TypicalWeek = GetTypicalWeek(TOUSchedule);

                    for (int SeasonIndex = 0; SeasonIndex < SchedSeasons.Count;
                        SeasonIndex++)
                    {
                        SchedSeason = SchedSeasons[SeasonIndex];
                        Season = Table2048.TOUConfig.Seasons[SeasonIndex];
                        Season.IsProgrammed = 1;
                        Season.Daytypes = TypicalWeek;

                        // Configure the normal daytypes (0..2)
                        for (int DaytypeIndex = 0;
                            DaytypeIndex < SchedSeason.NormalDays.Count;
                            DaytypeIndex++)
                        {
                            // Get the index of the pattern in the schedule's 
                            // pattern collection that's assigned to this 
                            // daytype, so we can add its switchpoints
                            PatternIndex = Patterns.SearchID(
                                SchedSeason.NormalDays[DaytypeIndex]);

                            TODEvent = 0;
                            for (int EventIndex = 0;
                                EventIndex < Patterns[PatternIndex].SwitchPoints.Count;
                                EventIndex++)
                            {
                                // mcm 10/23/2006 - SCR 105 - Output end events not added.
                                // Outputs switchpoints are a special case. A start and end 
                                // event must be added for outputs.
                                CSwitchPoint SP = Patterns[PatternIndex].SwitchPoints[EventIndex];

                                Season.TimeOfDayEvents[DaytypeIndex, TODEvent++] =
                                    GetDayEvent(SP);

                                // Unlike rate switchpoints, outputs can overlap, so we 
                                // have to add the end point too.
                                if (eSwitchPointType.OUTPUT == SP.SwitchPointType)
                                {
                                    Season.TimeOfDayEvents[DaytypeIndex, TODEvent++] =
                                        GetOutputOffEvent(SP);

                                    // We'll need to sort the events since we added one
                                    EventsNeedSorting = true;
                                }

                            } // For each switchpoint
                        }

                        // Configure the Holiday daytype if it exists
                        if (0 != SchedSeason.Holidays.Count)
                        {
                            // Get the index of the pattern in the schedule's 
                            // pattern collection that's assigned to this 
                            // (holiday) daytype, so we can add its switchpoints
                            PatternIndex = Patterns.SearchID(SchedSeason.Holidays[0]);

                            TODEvent = 0;
                            for (int EventIndex = 0;
                                EventIndex < Patterns[PatternIndex].SwitchPoints.Count;
                                EventIndex++)
                            {
                                // mcm 10/23/2006 - SCR 105 - Output end events not added.
                                // Outputs switchpoints are a special case. A start and end 
                                // event must be added for outputs.
                                CSwitchPoint SP = Patterns[PatternIndex].SwitchPoints[EventIndex];

                                Season.TimeOfDayEvents[TOUConfig.HOLIDAY_TYPE_INDEX, TODEvent++] =
                                    GetDayEvent(SP);

                                // Unlike rate switchpoints, outputs can overlap, so we 
                                // have to add the end point too.
                                if (eSwitchPointType.OUTPUT == SP.SwitchPointType)
                                {
                                    Season.TimeOfDayEvents[TOUConfig.HOLIDAY_TYPE_INDEX, TODEvent++] =
                                        GetOutputOffEvent(SP);

                                    // We'll need to sort the events since we added one
                                    EventsNeedSorting = true;
                                }

                            } // For each switchpoint
                        }

                        // Sort the events if we added any
                        if (EventsNeedSorting)
                        {
                            Season.Sort();
                        }
                    } // For each season
                } // IF the TOU server is instantiated
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

        } // OverwriteTOUConfig

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
            ushort TypicalWeek = 0;
            ushort DTIndex;


            // Day to Day Type Assignments: 
            // 2 bits for each day (Sun – Sat & Holiday)

            TypicalWeek = GetDaytypeIndex(TOUSchedule, eTypicalDay.SUNDAY);
            DTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.MONDAY);
            TypicalWeek = (ushort)(TypicalWeek + DTIndex * 0x0004);
            DTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.TUESDAY);
            TypicalWeek = (ushort)(TypicalWeek + DTIndex * 0x0010);
            DTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.WEDNESDAY);
            TypicalWeek = (ushort)(TypicalWeek + DTIndex * 0x0040);
            DTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.THURSDAY);
            TypicalWeek = (ushort)(TypicalWeek + DTIndex * 0x0100);
            DTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.FRIDAY);
            TypicalWeek = (ushort)(TypicalWeek + DTIndex * 0x0400);
            DTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.SATURDAY);
            TypicalWeek = (ushort)(TypicalWeek + DTIndex * 0x1000);

            // Holiday type always gets the holiday index
            TypicalWeek = (ushort)(TypicalWeek +
                TOUConfig.HOLIDAY_TYPE_INDEX * 0x4000);

            return TypicalWeek;

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

        /// <summary>
        /// Builds a CaledarConfig Day Event from a TOU schedule's year event.
        /// </summary>
        /// <param name="TOUEvent">TOU server applied season start date or 
        /// applied holiday event</param>
        /// <returns>CalendarConfig CalendarEvent that represents the TOU
        /// event</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/26/06 mcm 7.30.00 N/A	Created
        private CalendarEvent GetYearEvent(TOU.CEvent TOUEvent)
        {
            CalendarEvent Event = new CalendarEvent();

            // Translate TOU schedule event to CalendarConfig event
            if (TOU.eEventType.HOLIDAY == TOUEvent.Type)
            {
                Event.Type = (byte)CalendarEvent.CalendarEventType.HOLIDAY;
            }
            else
            {
                Event.Type = (byte)((int)CalendarEvent.CalendarEventType.SEASON1 +
                    TOUEvent.Index);
            }

            Event.Month = (byte)(TOUEvent.Date.Month - 1);
            Event.Day = (byte)(TOUEvent.Date.Day - 1);

            return Event;

        } // GetYearEvent

        /// <summary>
        /// Finds the index of the season that should be in effect at the start
        /// the new configuration. Customers usually have a Summer and Winter
        /// season. They expect Winter to be in	effect until Summer starts that
        /// first year, but it won't be unless we insert a start date.
        /// This method ASSUMES that every year has a season start date.
        /// </summary>
        /// <param name="TOUSchedule">TOU server with the schedule open</param>
        /// <param name="YearIndex">Index of the first year to configure</param>
        /// <returns>The season index that should be used on 1/1 of the 
        /// first year.</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/26/06 mcm 7.30.00 N/A	Created
        private byte FindStartSeason(CTOUSchedule TOUSchedule, int YearIndex)
        {
            byte SeasonIndex = 0;
            TOU.CYear TOUYear;

            // If the TOUIndex given is not 0, then we have at least one year
            // defined in the schedule prior to this year. Search last year for
            // the last season start date.  If this is the first year of the 
            // calendar, we'll make the assumption that the last season of
            // this year should have started the year.
            if (0 < YearIndex)
            {
                // Decrement the index. We'll search last year.
                --YearIndex;
            }

            TOUYear = TOUSchedule.Years[YearIndex];

            // Search the schedule for season start events. We could search
            // backwards and stop at the first one we find, but there are only
            // 44 events, and I think the logic is cleaner this way. Besides
            // the XML DOM object doesn't like going backwards, so it might
            // even be faster this way.
            for (int EventIndex = 0;
                EventIndex < TOUYear.Events.Count;
                EventIndex++)
            {
                CalendarEvent CalEvent =
                    GetYearEvent(TOUYear.Events[EventIndex]);

                // If this is the first year configured and the schedule 
                // doesn't have a season starting on the first day of
                // year, insert one
                if (CalEvent.IsSeason())
                {
                    SeasonIndex = CalEvent.Type;
                }
            }
            // probably want to modify this to return the TOU server's index or a meter event object
            // depending on whether we insert a start date or use a procedure call to start it after configuration
            return SeasonIndex;

        } // FindStartSeason

        #endregion

        #region Private Properties

        private List<uint> m_lstSupportedEnergies;
        private List<uint> m_lstSupportedDemands;


        #endregion

        #region Members
        #endregion

    }
}
