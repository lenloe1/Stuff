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
//                              Copyright © 2006 - 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

//#define TOU_RECONFIG_TEST


using System;
using System.Threading;
using System.Collections;
using System.Resources;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using Itron.Metering.Communications;
using Itron.Metering.Communications.SCS;
using Itron.Metering.DST;
using Itron.Metering.TOU;
using Itron.Metering.TIM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{

    /// <summary>
    /// This class represents an TOU event from the TOU Calendar of 
    /// an SCS Device.
    /// </summary>
    /// <remarks>
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 06/06/06 jrf 7.30.00  N/A	Created
    /// </remarks>
    /// 
    public class SCSTOUEvent
    {
        #region Definitions

        /// <summary>
        /// EventTypes enumeration encapsulates the SCS TOU Event Types.
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 jrf 7.30.00 N/A	Created
        /// 
        public enum EventTypes : int
        {
            /// <summary>
            /// Start Year for following events.
            /// </summary>
            StartYear = 0x800,
            /// <summary>
            /// Indicates that a season change should occur.
            /// </summary>
            SeasonSelect = 0x2000,
            /// <summary>
            /// Indicates that a DST change should occur.
            /// </summary>
            DSTChange = 0x4000,
            /// <summary>
            /// Indicates a day that should be a holiday.
            /// </summary>
            HolidaySelect = 0x8000,
            /// <summary>
            /// Indicates the end of the calendar.
            /// </summary>
            CalendarEnd = 0x0000,
            /// <summary>
            /// An unknown event.
            /// </summary>
            Unknown
        };

        /// <summary>
        /// EventMask enumeration encapsulates the SCS TOU Event Masks.
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 jrf 7.30.00 N/A	Created
        /// 
        private enum EventMask : int
        {
            StartYear = 0xF800,
            SeasonSelect = 0xE000,
            DSTChange = 0xF000,
            HolidaySelect = 0xF000,
            CalendarEnd = 0xF800,
            Year = 0x00FF
        };

        /// <summary>
        /// DateMask enumeration encapsulates the SCS TOU Date Masks.
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/21/06 jrf 7.30.00 N/A	Created
        /// 
        private enum DateMask : int
        {
            RetrieveMonth = 0x000F,
            RetrieveDirection = 0x0003,
            RetrieveDay = 0x001F,
            InsertMonth = 0x0000000F,
            ClearMonth = 0xF0FF,
            InsertDay = 0x0000001F,
            ClearDay = 0xFFE0,
        };

        /// <summary>
        /// Enumeration encapsulates the Holiday event Masks.
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00 N/A	Created
        /// 
        private enum HolidayMask : int
        {
            RetrieveDaytype = 0x0060,
            HolidayDaytype = 0x0060,
        };

        /// <summary>
        /// Enumeration encapsulates the Season event Masks.
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00 N/A	Created
        /// 
        private enum SeasonMask : int
        {
            RetrieveSeason = 0x00E0,
            ResetDemand = 0x1000,
        };

        /// <summary>
        /// Direction enumeration encapsulates the SCS TOU DST direction to 
        /// shift time to reflect DST.
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 jrf 7.30.00 N/A	Created
        /// 07/06/06 mcm 7.30.00 N/A	Added explicit values
        /// 
        public enum DirectionTypes
        {
            /// <summary>
            /// Shift time forward.
            /// </summary>
            Advance = 0x0040,
            /// <summary>
            /// Shift time backwards.
            /// </summary>
            Retard = 0x0020
        };

        /// <summary>OLDEST_SUPPORTED_YEAR</summary>
        public const int OLDEST_SUPPORTED_YEAR = 95;

        #endregion Definitions

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sEvent">The short representing the SCS TOU event.
        /// </param>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 jrf 7.30.00  N/A   Created
        ///
        public SCSTOUEvent(short sEvent)
        {
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                this.GetType().Assembly);

            m_sEvent = sEvent;
            m_eventType = EventTypes.Unknown;

            if ((int)EventTypes.StartYear ==
                (sEvent & (int)EventMask.StartYear))
            {
                m_eventType = EventTypes.StartYear;
            }
            else if ((int)EventTypes.SeasonSelect ==
                (sEvent & (int)EventMask.SeasonSelect))
            {
                m_eventType = EventTypes.SeasonSelect;
            }
            else if ((int)EventTypes.DSTChange ==
                (sEvent & (int)EventMask.DSTChange))
            {
                m_eventType = EventTypes.DSTChange;
            }
            else if ((int)EventTypes.HolidaySelect ==
                (sEvent & (int)EventMask.HolidaySelect))
            {
                m_eventType = EventTypes.HolidaySelect;
            }
            else if ((int)EventTypes.CalendarEnd ==
                (sEvent & (int)EventMask.CalendarEnd))
            {
                m_eventType = EventTypes.CalendarEnd;
            }
            else m_eventType = EventTypes.Unknown;

        }

        /// <summary>Constructs an end of calendar event</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A   Created
        ///
        public SCSTOUEvent()
        {
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                this.GetType().Assembly);

            m_sEvent = (short)EventTypes.CalendarEnd;
            m_eventType = EventTypes.CalendarEnd;

        }

        /// <summary>Constructs a Start Year event</summary>
        /// <param name="StartYear">The modulo 100 Start Year value</param>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A   Created
        ///
        public SCSTOUEvent(byte StartYear)
        {
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                this.GetType().Assembly);

            m_sEvent = (short)((ushort)EventTypes.StartYear | StartYear);
            m_eventType = EventTypes.StartYear;

        }

        /// <summary>Constructs a Season event</summary>
        /// <param name="Season">Season index (0..7)</param>
        /// <param name="StartDate">Season start date</param>
        /// <param name="ResetDemand">Reset Demand on season change</param>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A   Created
        ///
        public SCSTOUEvent(short Season, DateTime StartDate, bool ResetDemand)
        {
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                this.GetType().Assembly);

            m_sEvent = (short)((ushort)EventTypes.SeasonSelect | StartDate.Day);
            m_sEvent = (short)((ushort)m_sEvent | (Season << 5));
            m_sEvent = (short)((ushort)m_sEvent | (StartDate.Month << 8));

            // The reset demand bit is defined as 0 == reset, 1 == don't reset
            if (!ResetDemand)
            {
                m_sEvent = (short)((ushort)m_sEvent |
                                    (ushort)SeasonMask.ResetDemand);
            }

            m_eventType = EventTypes.SeasonSelect;

        }

        /// <summary>Constructs a DST event</summary>
        /// <param name="Direction">Advance/Retard direction for the DST date</param>
        /// <param name="DSTDate">DST date</param>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A   Created
        ///
        public SCSTOUEvent(DirectionTypes Direction, DateTime DSTDate)
        {
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                this.GetType().Assembly);

            m_sEvent = (short)((ushort)EventTypes.DSTChange | DSTDate.Day);
            m_sEvent = (short)((ushort)m_sEvent | (ushort)Direction);
            m_sEvent = (short)((ushort)m_sEvent | (DSTDate.Month << 8));
            m_eventType = EventTypes.DSTChange;
        }


        /// <summary>Constructs a Holiday event. NOTE that the year is ignored,
        /// so make sure you add it within the right year</summary>
        /// <param name="HolidayDate">Holiday Date</param>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A   Created
        ///
        public SCSTOUEvent(DateTime HolidayDate)
        {
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                this.GetType().Assembly);

            m_sEvent = (short)((ushort)EventTypes.HolidaySelect | HolidayDate.Day);
            m_sEvent = (short)((ushort)m_sEvent | (ushort)HolidayMask.HolidayDaytype);
            m_sEvent = (short)((ushort)m_sEvent | (HolidayDate.Month << 8));
            m_eventType = EventTypes.HolidaySelect;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/21/06 jrf 7.30.00  N/A   Created
        /// 
        ~SCSTOUEvent()
        {
            m_rmStrings.ReleaseAllResources();
            m_rmStrings = null;
        }

        /// <summary>
        /// Overloaded less than operator.
        /// </summary>
        /// <param name="firstEvent">The left hand side.</param>
        /// <param name="secondEvent">The right hand side.</param>
        /// <returns>A boolean indicating the result of the comparison.</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 jrf 7.30.00  N/A   Created
        ///
        public static bool operator <(SCSTOUEvent firstEvent, SCSTOUEvent secondEvent)
        {
            bool bReturn = false;
            if (!firstEvent.IsDateEvent() || !secondEvent.IsDateEvent())
            {
                // If event is not a date event (DST, Season, Holiday)
                // then just assume that the order is correct.  This will
                // prevent a tou event from being sorted out of its 
                // calendar year.
                bReturn = false;
            }
            else if (firstEvent.Month < secondEvent.Month)
            {
                bReturn = true;
            }
            else if (firstEvent.Month == secondEvent.Month &&
                firstEvent.Day < secondEvent.Day)
            {
                bReturn = true;
            }
            else if (firstEvent.Month == secondEvent.Month &&
                firstEvent.Day == secondEvent.Day &&
                EventTypes.SeasonSelect == firstEvent.m_eventType)
            {
                // Season start dates come first
                bReturn = true;
            }

            return bReturn;

        }// End operator<

        /// <summary>
        /// Overloaded greater than operator.
        /// </summary>
        /// <param name="firstEvent">The left hand side.</param>
        /// <param name="secondEvent">The right hand side.</param>
        /// <returns>A boolean indicating the result of the comparison.</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 jrf 7.30.00  N/A   Created
        ///
        public static bool operator >(SCSTOUEvent firstEvent, SCSTOUEvent secondEvent)
        {
            bool bReturn = false;
            if (!firstEvent.IsDateEvent() || !secondEvent.IsDateEvent())
            {
                // If event is not a date event (DST, Season, Holiday)
                // then just assume that the order is correct.  This will
                // prevent a tou event from being sorted out of its 
                // calendar year.
                bReturn = false;
            }
            else if (firstEvent.Month > secondEvent.Month)
            {
                bReturn = true;
            }
            else if (firstEvent.Month == secondEvent.Month &&
                firstEvent.Day > secondEvent.Day)
            {
                bReturn = true;
            }
            else if (firstEvent.Month == secondEvent.Month &&
                firstEvent.Day == secondEvent.Day &&
                EventTypes.SeasonSelect == secondEvent.m_eventType)
            {
                // Season start dates come first
                bReturn = true;
            }

            return bReturn;

        }// End operator>

        /// <summary>
        /// Determines if event has a date associated with it.
        /// </summary>
        /// <returns>A boolean indicating if event is a date event. 
        /// with it.</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 jrf 7.30.00  N/A   Created
        ///
        public bool IsDateEvent()
        {
            bool bReturn = false;
            if (EventTypes.DSTChange == m_eventType ||
                EventTypes.HolidaySelect == m_eventType ||
                EventTypes.SeasonSelect == m_eventType)
            {
                bReturn = true;
            }
            return bReturn;
        }


        #endregion Public Methods

        #region Public Properties

        /// <summary>This property gets or sets the TOU event value.</summary>
        /// <returns>
        /// A short representing the value of the event.
        /// </returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 jrf 7.30.00  N/A   Created
        /// 
        public short Value
        {
            get { return m_sEvent; }
            set { m_sEvent = value; }
        }

        /// <summary>This property gets or sets the TOU event type.</summary>
        /// <returns>
        /// An EventTypes representing the event type.
        /// </returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 jrf 7.30.00  N/A   Created
        /// 
        public EventTypes Type
        {
            get { return m_eventType; }
            set { m_eventType = value; }
        }

        /// <summary>This property gets the TOU event year, if the event is a 
        /// StartYear.</summary>
        /// <returns>
        /// An int representing the year.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the event type is not a StartYear</exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 jrf 7.30.00  N/A   Created
        ///
        public int Year
        {
            get
            {
                if (EventTypes.StartYear == m_eventType)
                {
                    return m_sEvent & (int)EventMask.Year;
                }
                else
                {
                    throw new ApplicationException(m_rmStrings.GetString("EVENT_NOT_A_START_YEAR"));
                }
            }
        }

        /// <summary>This property gets the TOU event month, if the event has 
        /// a date associated with it.</summary>
        /// <returns>
        /// An int representing the month.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the event type is not date event</exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 jrf 7.30.00  N/A   Created
        ///
        public int Month
        {
            get
            {
                if (this.IsDateEvent())
                {
                    return (int)((m_sEvent >> 8) & (int)DateMask.RetrieveMonth);
                }
                else
                {
                    throw new ApplicationException(m_rmStrings.GetString("EVENT_HAS_NO_MONTH"));
                }
            }
            set
            {
                short sMonth;
                if (this.IsDateEvent())
                {
                    sMonth = (short)((value & (int)DateMask.InsertMonth) << 8);
                    m_sEvent = (short)(ushort)(m_sEvent & (int)DateMask.ClearMonth);
                    m_sEvent = (short)((ushort)m_sEvent | (ushort)sMonth);
                }
                else
                {
                    throw new ApplicationException(m_rmStrings.GetString("EVENT_HAS_NO_MONTH"));
                }
            }
        }

        /// <summary>This property gets the TOU event day, if the event has a date
        /// associated with it.</summary>
        /// <returns>
        /// An int representing the day.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the event type is not date event</exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 jrf 7.30.00  N/A   Created
        ///
        public int Day
        {
            get
            {
                if (this.IsDateEvent())
                {
                    return (int)(m_sEvent & (int)DateMask.RetrieveDay);
                }
                else
                {
                    throw new ApplicationException(m_rmStrings.GetString("EVENT_HAS_NO_DAY"));
                }
            }
            set
            {
                int sDay;
                if (this.IsDateEvent())
                {
                    sDay = (short)(value & (int)DateMask.InsertDay);
                    m_sEvent = (short)(m_sEvent & (int)DateMask.ClearDay);
                    m_sEvent = (short)((ushort)m_sEvent | (ushort)sDay);
                }
                else
                {
                    throw new ApplicationException(m_rmStrings.GetString("EVENT_HAS_NO_DAY"));
                }
            }
        }

        /// <summary>This property gets the Holiday Day Type, if the event 
        /// is a holiday.</summary>
        /// <returns>
        /// An int representing the daytype (0..3).
        /// </returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the event type is not date event</exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A   Created
        ///
        public int HolidayType
        {
            get
            {
                if (EventTypes.HolidaySelect == Type)
                {
                    return (int)((m_sEvent & (int)HolidayMask.RetrieveDaytype) >> 5);
                }
                else
                {
                    throw new ApplicationException("Event is not a Holiday");
                }
            }
        }

        /// <summary>Gets the Season index, if the event is a season.</summary>
        /// 
        /// <returns>
        /// An int representing the season (0..7).
        /// </returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the event type is not date event</exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A   Created
        ///
        public int Season
        {
            get
            {
                if (EventTypes.SeasonSelect == Type)
                {
                    return (int)((m_sEvent & (int)SeasonMask.RetrieveSeason) >> 5);
                }
                else
                {
                    throw new ApplicationException("Event is not a Season");
                }
            }
        }

        /// <summary>Returns true if demand reset is configured for the season
        /// change event.</summary>
        /// <exception cref="ApplicationException">
        /// Thrown when the event type is not date event</exception>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00  N/A   Created
        ///
        public bool DemandReset
        {
            get
            {
                if (EventTypes.SeasonSelect == Type)
                {
                    return (0 == (m_sEvent & (int)SeasonMask.ResetDemand));
                }
                else
                {
                    throw new ApplicationException("Event is not a Season");
                }
            }
        }

        /// <summary>This property gets the TOU event direction, if the event is 
        /// a DSTChange event.</summary>
        /// <returns>
        /// An DirectionTypes representing the direction.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the event type is not a DSTChange event or it is not 
        /// in the correct format.</exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 jrf 7.30.00  N/A   Created
        /// 
        public DirectionTypes Direction
        {
            get
            {
                int iDirection = 0;
                if (EventTypes.DSTChange == m_eventType)
                {
                    iDirection = (int)((m_sEvent >> 5) & (int)DateMask.RetrieveDirection);
                    if (1 == iDirection) return DirectionTypes.Retard;
                    else if (2 == iDirection) return DirectionTypes.Advance;
                    else throw new ApplicationException(m_rmStrings.GetString("DST_EVENT_INCORRECT_FORMAT"));
                }
                else
                {
                    throw new ApplicationException(m_rmStrings.GetString("EVENT_NOT_A_DST_CHANGE"));
                }
            }
        }


        /// <summary>
        /// Returns a string description of the event for debugging.
        /// </summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A   Created
        ///
        public string Description
        {
            get
            {
                string Description = "";

                switch (Type)
                {
                    case EventTypes.StartYear:
                        {
                            Description = "Start Year = " + Year;
                            break;
                        }
                    case EventTypes.CalendarEnd:
                        {
                            Description = "***** End of Calendar *****";
                            break;
                        }
                    case EventTypes.DSTChange:
                        {
                            if (DirectionTypes.Advance == Direction)
                            {
                                Description = "To DST " + Month + "/" + Day;
                            }
                            else
                            {
                                Description = "From DST " + Month + "/" + Day;
                            }
                            break;
                        }
                    case EventTypes.HolidaySelect:
                        {
                            Description = "Holiday type " + HolidayType + ", " +
                                Month + "/" + Day;
                            break;
                        }
                    case EventTypes.SeasonSelect:
                        {
                            Description = "Season " + Season + ", " + Month + "/" + Day;

                            // The reset demand bit is defined as 0 == reset, 
                            // 1 == don't reset
                            if (0 == (m_sEvent & (short)SeasonMask.ResetDemand))
                            {
                                Description += " with Demand Reset";
                            }

                            break;
                        }
                    case EventTypes.Unknown:
                        {
                            Description = "Event type is Unknown";
                            break;
                        }
                    default:
                        {
                            Description = "Event is undescribable";
                            break;
                        }
                }

                Description = Description + ", 0x" + m_sEvent.ToString("X4", CultureInfo.InvariantCulture);

                return Description;
            }

        } // Description

        #endregion

        #region Members

        // Member variables	
        private short m_sEvent;
        private EventTypes m_eventType;

        //Members to support resource strings:
        private static readonly string RESOURCE_FILE_PROJECT_STRINGS =
            "Itron.Metering.Device.SCSDevice.SCSDeviceStrings";
        /// <summary>
        /// Resourse Manager object that supports extracting strings from the 
        /// resourse file.
        /// </summary>
        protected System.Resources.ResourceManager m_rmStrings = null;

        #endregion Members
    }

    /// <summary>
    /// This class represents a collection of SCSTOUEvents.
    /// The CEventCollection class was used as a template.
    /// </summary>
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 06/06/06 jrf 7.30.00  N/A	Created
    /// 
    public class SCSTOUEventCollection : CollectionBase
    {
        #region Constants

        // Constants
        private const int TOU_EVENT_SIZE = 2;

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates an instance of the SCSTOUEvent collection object.
        /// </summary>
        /// <example>
        /// <code>
        /// SCSTOUEventList coll = new SCSTOUEventList();
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class 
        /// 06/06/06 jrf 7.30.00  N/A	Converted class
        public SCSTOUEventCollection()
        {
            m_Logger = Logger.TheInstance;

        }//SCSTOUEventCollection


        /// <summary>
        /// Adds an Event to the end of the SCSTOUEventCollection
        /// </summary>
        /// <param name="objToAdd">
        /// The Event to be added
        /// </param>
        /// <returns>
        /// The zero base index of the Event added
        /// </returns>
        /// <example>
        /// <code>
        /// SCSTOUEventCollection coll = new SCSTOUEventCollection();
        /// coll.Add(new SCSTOUEvent(2074));
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class 
        /// 06/06/06 jrf 7.30.00  N/A	Converted class  
        public int Add(SCSTOUEvent objToAdd)
        {
            m_IsSorted = false;
            return InnerList.Add(objToAdd);
        }//Add

        /// <summary>
        /// Checks to see if the first year has a start date for the first day
        /// of the year.  Adds a start date for the last season in the first 
        /// year to the first day if needed.
        /// ASSUMES the season start dates are sorted.
        /// </summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/11/06 mcm 7.30.00 N/A	Creation
        /// 
        public void AddFirstStartDate()
        {
            bool HasStartDate = false;
            bool FirstYear = true;
            SCSTOUEvent LastSeason = new SCSTOUEvent();


            // Is a season start date on the first day of the first year?
            foreach (SCSTOUEvent Event in InnerList)
            {
                if (SCSTOUEvent.EventTypes.StartYear == Event.Type)
                {
                    if (true == FirstYear)
                    {
                        FirstYear = false;
                    }
                    else
                    {
                        // We're out of the first year, quit looking
                        break;
                    }
                }
                else if (SCSTOUEvent.EventTypes.SeasonSelect == Event.Type)
                {
                    if ((Event.Month == 1) && (Event.Day == 1))
                    {
                        HasStartDate = true;
                        break;
                    }
                    else
                    {
                        LastSeason = Event;
                    }
                }
            }

            // If there wasn't a season start date on the first day, 
            // add the last one we found in the year
            if (false == HasStartDate)
            {
                LastSeason = new SCSTOUEvent(LastSeason.Value);
                LastSeason.Month = 1;
                LastSeason.Day = 1;
                Insert(1, LastSeason);
            }

        } // AddFirstStartDate

        /// <summary>
        /// Adds an Event to the SCSTOUEventCollection at the given index
        /// </summary>
        /// <param name="iIndex">
        /// Index to insert the Event into in the collection
        /// </param>
        /// <param name="objToAdd">
        /// The Event to be added
        /// </param>
        /// <example>
        /// <code>
        /// SCSTOUEventCollection coll = new SCSTOUEventCollection();
        /// coll.Insert(3, new SCSTOUEvent(2074));
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class  
        /// 06/06/06 jrf 7.30.00  N/A	Converted class
        public void Insert(int iIndex, SCSTOUEvent objToAdd)
        {
            m_IsSorted = false;
            InnerList.Insert(iIndex, objToAdd);
        }//Insert

        /// <summary>
        /// Used to sort the event collection based on the date of the SCSTOUEvent 
        /// objects.</summary>
        /// <example>
        /// <code>
        /// SCSTOUEventCollection coll = new SCSTOUEventCollection();
        /// SCSTOUEvent temp = new SCSTOUEvent(2075);
        /// SCSTOUEvent temp1 = new SCSTOUEvent(2076);
        /// SCSTOUEvent temp2 = new SCSTOUEvent(2077);
        /// coll.Add(temp);
        /// coll.Add(temp1);
        /// coll.Add(temp2);
        /// coll.Sort();
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class
        /// 06/06/06 jrf 7.30.00  N/A	Converted class  
        public void Sort()
        {
            SCSTOUEvent tempEvent;

            for (int iIterate = 0; iIterate < InnerList.Count; iIterate++)
            {
                // Checks for out of order events
                while ((iIterate > 0) &&
                    (((SCSTOUEvent)InnerList[iIterate]) <
                    ((SCSTOUEvent)InnerList[iIterate - 1])))
                {
                    // Swap the out of order events
                    tempEvent = ((SCSTOUEvent)InnerList[iIterate - 1]);
                    InnerList[iIterate - 1] = InnerList[iIterate];
                    InnerList[iIterate] = tempEvent;

                    // On loop back check the order of swapped event against 
                    // its new previous event 
                    iIterate--;
                }
            }

            m_IsSorted = true;

        }//Sort

        /// <summary>Truncates the event list to fit in the space available.
        /// Truncation will not split any years. The calendar will be truncated
        /// down to the most whole years that will fit into the space 
        /// available. If the first year will not fit into the space available,
        /// this method will return false and no changes will be made to the 
        /// event list.</summary>
        /// <returns>true if successful, false if the schedule could not be
        /// truncated</returns>
        /// <param name="SpaceAvailable">Total space available for the event 
        /// list portion (calendar) of the TOU configuration data.  This does
        /// NOT include the space used by the Season definitions.</param>
        /// 
        /// <remarks>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Converted class
        /// 09/11/06 mcm 7.35.00  N/A   Correcting truncation of last start year
        /// </remarks>  
        /// 
        public bool TruncateToFit(int SpaceAvailable)
        {
            bool Successful = false;
            int Index;
            SCSTOUEvent Event;


            if (false == IsSorted)
            {
                Sort();
            }

            // If we're too big, remove the years that won't fit
            if (InnerList.Count * 2 > SpaceAvailable)
            {
                // Find the previous start year event starting one event less
                // than would fit in the space available because we have to
                // save room for the end of calendar event. Note that
                // subracting two accounts for it being 0 indexed + 1 event for
                // the end of calendar event.
                Index = (SpaceAvailable / 2) - 2;

                // Don't go all the way down to the first event. There must be
                // at least 1 year.
                while ((Index > 1) && (false == Successful))
                {
                    Event = (SCSTOUEvent)InnerList[Index];
                    if (SCSTOUEvent.EventTypes.StartYear == Event.Type)
                    {
                        // Remove up to but not including the end of calendar event
                        InnerList.RemoveRange(Index + 1,
                            InnerList.Count - Index - 2);
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                            "Calendar WAS truncated");

                        Successful = true;
                    }
                    else
                    {
                        // Check the previous event
                        Index--;
                    }
                }
            }
            else
            {
                Successful = true;
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                    "Calendar was NOT truncated");
            }

            return Successful;

        } // TruncateToFit

        /// <summary>Dumps the contents of the collection for debugging</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Converted class  
        public void Dump()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "Dump of the SCSTOUEventCollection");

            foreach (SCSTOUEvent Event in InnerList)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    Event.Description);
            }

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "End Dump of the SCSTOUEventCollection");
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Gets an Event at an index of the SCSTOUEventCollection.  Allows access
        /// to elements like an array
        /// </summary>
        /// <example>
        /// <code>
        /// SCSTOUEventCollection coll = new SCSTOUEventCollection();
        /// SCSTOUEvent temp = coll[0];
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/15/06 rrr N/A	 N/A	Creation of class 
        /// 06/06/06 jrf 7.30.00  N/A	Converted class 
        public SCSTOUEvent this[int iIndex]
        {
            get
            {
                return (SCSTOUEvent)InnerList[iIndex];
            }
        }//this[]

        /// <summary>This property gets the SCSTOUEvent collection as an array 
        /// of bytes.</summary>
        /// <returns>
        /// A byte[] representing the SCSTOUEvent collection.
        /// </returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/12/06 jrf 7.30.00  N/A   Created
        /// 
        public byte[] ByteList
        {
            get
            {
                byte[] byTOUEventBytes =
                    new byte[(TOU_EVENT_SIZE * InnerList.Count)];
                int iTemp = 0;
                for (int iIndex = 0; iIndex < InnerList.Count; iIndex++)
                {
                    byTOUEventBytes[iIndex * TOU_EVENT_SIZE + 1] =
                        (byte)(((SCSTOUEvent)InnerList[iIndex]).Value & 0x00FF);
                    iTemp = ((SCSTOUEvent)InnerList[iIndex]).Value & 0xFF00;
                    byTOUEventBytes[iIndex * TOU_EVENT_SIZE] = (byte)(iTemp >> 8);
                }

                return byTOUEventBytes;
            }

        } //ByteList

        /// <summary>Returns true if the list is sorted.</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A   Created
        /// 
        public bool IsSorted
        {
            get
            {
                return m_IsSorted;
            }
        } // IsSorted

        /// <summary>Returns the expiration date of the calendar. This is the 
        /// first day of the year after our last configured year.</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A   Created
        /// 
        public DateTime ExpirationDate
        {
            get
            {
                SCSTOUEvent Event;
                DateTime ExpirDate = new DateTime(1980, 1, 1);

                // Search for the last Start Year event in the list
                for (int Index = InnerList.Count - 1; Index > 0; Index--)
                {
                    Event = (SCSTOUEvent)InnerList[Index];

                    if (SCSTOUEvent.EventTypes.StartYear == Event.Type)
                    {
                        // Year values are modulo 100, so we need to find
                        // out the century.
                        if (Event.Year < SCSTOUEvent.OLDEST_SUPPORTED_YEAR)
                        {
                            ExpirDate = new DateTime(2000 + Event.Year, 1, 1);
                        }
                        else
                        {
                            ExpirDate = new DateTime(1900 + Event.Year, 1, 1);
                        }

                        // We found what we were looking for, quit looking
                        break;
                    }
                }

                return ExpirDate;
            }
        } // ExpirationDate

        /// <summary>Returns the size in bytes of the calendar. </summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A   Created
        /// 
        public int Size
        {
            get
            {
                return InnerList.Count * 2;
            }
        }

        #endregion

        #region Members

        private Logger m_Logger;
        private bool m_IsSorted = false;

        #endregion Member

    } //SCSTOUEventList 


    /// <summary>
    /// This class represents an SCS Season Switchpoint.
    /// </summary>
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 07/05/06 mcm 7.30.00  N/A	Created
    /// 
    public class SCSSwitchpoint
    {
        #region Definitions

        /// <summary>UNSHIFTED Switchpoint type, rate or output</summary>
        public enum SwitchpointType : ushort
        {
            /// <summary>Rate</summary>
            Rate = 0x0000,
            /// <summary>Output</summary>
            Output = 0x8000,
        }

        /// <summary>UNSHIFTED Rate/Output assigned to the switchpoint</summary>
        public enum RateOutputType : ushort
        {
            /// <summary>RateA</summary>
            RateA = 0x0080,
            /// <summary>RateB</summary>
            RateB = 0x0040,
            /// <summary>RateC</summary>
            RateC = 0x0020,
            /// <summary>RateD</summary>
            RateD = 0x0010,
            /// <summary>RateE</summary>
            RateE = 0x00F0,
            /// <summary>Output1</summary>
            Output1 = 0x0080,
            /// <summary>Output2</summary>
            Output2 = 0x0040,
            /// <summary>Output3</summary>
            Output3 = 0x0020,
            /// <summary>Output4</summary>
            Output4 = 0x0010,
            /// <summary>NoRateOutput</summary>
            NoRateOutput = 0,
        }

        /// <summary>UNSHIFTED Rate/Output assigned to the switchpoint</summary>
        public enum FulcrumOutputs : ushort
        {
            /// <summary>Output1</summary>
            Output1 = 0x0040,
            /// <summary>Output2</summary>
            Output2 = 0x0020,
            /// <summary>Output3</summary>
            Output3 = 0x0010,
            /// <summary>Output4</summary>
            Output4 = 0x0080,
            /// <summary>NoRateOutput</summary>
            NoRateOutput = 0,
        }

        /// <summary>Masks, shifts, and other defined values for SCS 
        /// switchpoints</summary>
        public enum Definitions : ushort
        {
            /// <summary>TypeMask</summary>
            TypeMask = 0x8000,
            /// <summary>DayTypeMask</summary>
            DayTypeMask = 0x6000,
            /// <summary>RateOutputMask</summary>
            RateOutputMask = 0x00F0,
            /// <summary>MinuteMask</summary>
            MinuteMask = 0x000F,
            /// <summary>HourMask</summary>
            HourMask = 0x1F00,
            /// <summary>HourShift</summary>
            HourShift = 8,
            /// <summary>RateOutputShift</summary>
            RateOutputShift = 4,
            /// <summary>EndOfSeason</summary>
            EndOfSeason = 0xFFFF,
            /// <summary>MinuteMask</summary>
            ClearMinuteMask = 0xFFF0,
            /// <summary>HourMask</summary>
            ClearHourMask = 0xE0FF,
        }

        /// <summary>UNSHIFTED DayType index</summary>
        public enum DayType : ushort
        {
            /// <summary>NormalDay1</summary>
            NormalDay1 = 0x0000,
            /// <summary>NormalDay2</summary>
            NormalDay2 = 0x2000,
            /// <summary>NormalDay3</summary>
            NormalDay3 = 0x4000,
            /// <summary>Holiday</summary>
            Holiday = 0x6000,
        }


        #endregion Definitions

        #region Public Methods

        /// <summary>Constructs an EndOfSeason SCSSwitchpoint</summary>
        /// <param name="IsFulcrum">true if this is a Fulcrum SP</param>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public SCSSwitchpoint(bool IsFulcrum)
        {
            m_IsFulcrum = IsFulcrum;
            m_Switchpoint = (ushort)Definitions.EndOfSeason;
        }


        /// <summary>Constructs a SCSSwitchpoint from it's ushort value</summary>
        /// <param name="IsFulcrum">true if this is a Fulcrum SP</param>
        /// <param name="Switchpoint">ushort representation of the SP</param>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public SCSSwitchpoint(bool IsFulcrum, ushort Switchpoint)
        {
            m_IsFulcrum = IsFulcrum;
            m_Switchpoint = Switchpoint;
        }

        /// <summary>Constructs a SCSSwitchpoint from a TOU schedule 
        /// switchpoint. NOTE that the Fulcrum defines output switchpoints 
        /// differently than the other SCS devices. Isn't that special?
        /// </summary>
        /// <param name="IsFulcrum">true if this is a Fulcrum SP</param>
        /// <param name="DaytypeIndex">Daytype index 0..3 (3==Holiday)</param>
        /// <param name="Switchpoint">Switchpoint from the TOU server</param>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public SCSSwitchpoint(bool IsFulcrum, DayType DaytypeIndex,
            TOU.CSwitchPoint Switchpoint)
        {
            int MinutesSinceMidnight = Switchpoint.StartTime;
            ushort Hour = (ushort)(MinutesSinceMidnight / 60);
            ushort Minutes = (ushort)((MinutesSinceMidnight % 60) / 5);
            ushort RateOutputIndex = (ushort)(Switchpoint.RateOutputIndex);


            m_IsFulcrum = IsFulcrum;
            m_Switchpoint = (ushort)((ushort)DaytypeIndex | Minutes |
                (Hour << (ushort)Definitions.HourShift));

            // Rates are 0 indexed in the TOU schedule, but our values are 
            // bit positions
            switch (RateOutputIndex)
            {
                // The rate values correspond to the output values for all SCS
                // devices except the FULCRUM.
                case 0:
                    {
                        if (IsFulcrum &&
                            (TOU.eSwitchPointType.OUTPUT == Switchpoint.SwitchPointType))
                        {
                            m_Switchpoint = (ushort)(m_Switchpoint |
                                (ushort)FulcrumOutputs.Output1);
                        }
                        else
                        {
                            // For non-Fulcrum devices, the rate and output values match
                            m_Switchpoint = (ushort)(m_Switchpoint |
                                (ushort)RateOutputType.RateA);
                        }
                        break;
                    }
                case 1:
                    {
                        if (IsFulcrum &&
                            (TOU.eSwitchPointType.OUTPUT == Switchpoint.SwitchPointType))
                        {
                            m_Switchpoint = (ushort)(m_Switchpoint |
                                (ushort)FulcrumOutputs.Output2);
                        }
                        else
                        {
                            // For non-Fulcrum devices, the rate and output values match
                            m_Switchpoint = (ushort)(m_Switchpoint |
                                (ushort)RateOutputType.RateB);
                        }
                        break;
                    }
                case 2:
                    {
                        if (IsFulcrum &&
                            (TOU.eSwitchPointType.OUTPUT == Switchpoint.SwitchPointType))
                        {
                            m_Switchpoint = (ushort)(m_Switchpoint |
                                (ushort)FulcrumOutputs.Output3);
                        }
                        else
                        {
                            // For non-Fulcrum devices, the rate and output values match
                            m_Switchpoint = (ushort)(m_Switchpoint |
                                (ushort)RateOutputType.RateC);
                        }
                        break;
                    }
                case 3:
                    {
                        if (IsFulcrum &&
                            (TOU.eSwitchPointType.OUTPUT == Switchpoint.SwitchPointType))
                        {
                            m_Switchpoint = (ushort)(m_Switchpoint |
                                (ushort)FulcrumOutputs.Output4);
                        }
                        else
                        {
                            // For non-Fulcrum devices, the rate and output values match
                            m_Switchpoint = (ushort)(m_Switchpoint |
                                (ushort)RateOutputType.RateD);
                        }
                        break;
                    }
                default:
                    {
                        throw (new Exception("Unsupported switchpoint rate"));
                    }
            }
            if (TOU.eSwitchPointType.OUTPUT == Switchpoint.SwitchPointType)
            {
                m_Switchpoint = (ushort)(m_Switchpoint |
                    (ushort)SwitchpointType.Output);
            }
        }


        /// <summary>Constructs an All Outputs OFF switchpoint at 00:00</summary>
        /// <param name="IsFulcrum">true if this is a Fulcrum SP</param>
        /// <param name="DaytypeIndex">Daytype index 0..3 (3==Holiday)</param>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public SCSSwitchpoint(bool IsFulcrum, DayType DaytypeIndex)
        {
            m_IsFulcrum = IsFulcrum;
            m_Switchpoint = (ushort)((ushort)DaytypeIndex |
                                     (ushort)SwitchpointType.Output);
        }

        /// <summary>Overloaded less than operator.</summary>
        /// <param name="firstSP">The left hand side.</param>
        /// <param name="secondSP">The right hand side.</param>
        /// <returns>A boolean indicating the result of the comparison.</returns>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00  N/A   Created
        ///
        public static bool operator <(SCSSwitchpoint firstSP,
                                      SCSSwitchpoint secondSP)
        {
            bool LessThan = false;

            if (firstSP.Hour < secondSP.Hour)
            {
                LessThan = true;
            }
            else if (firstSP.Hour == secondSP.Hour &&
                     firstSP.Minute < secondSP.Minute)
            {
                LessThan = true;
            }

            return LessThan;

        }// End operator<

        /// <summary>Overloaded greater than operator.</summary>
        /// <param name="firstSP">The left hand side.</param>
        /// <param name="secondSP">The right hand side.</param>
        /// <returns>A boolean indicating the result of the comparison.</returns>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00  N/A   Created
        ///
        public static bool operator >(SCSSwitchpoint firstSP,
                                        SCSSwitchpoint secondSP)
        {
            bool GreaterThan = false;

            if (firstSP.Hour > secondSP.Hour)
            {
                GreaterThan = true;
            }
            else if (firstSP.Hour == secondSP.Hour &&
                     firstSP.Minute > secondSP.Minute)
            {
                GreaterThan = true;
            }

            return GreaterThan;

        }// End operator>

        #endregion Public Methods

        #region Public Properties

        /// <summary>Returns the unshifted value of the Rate/Output 
        /// selector</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public RateOutputType RateOutput
        {
            get
            {
                return (RateOutputType)
                    (m_Switchpoint & (ushort)Definitions.RateOutputMask);
            }
        }

        /// <summary>Returns the unshifted value of the switchpoint 
        /// type</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public SwitchpointType Type
        {
            get
            {
                return (SwitchpointType)
                    (m_Switchpoint & (ushort)Definitions.TypeMask);
            }
        }

        /// <summary>Returns the hour (0..24) the switchpoint starts.
        /// The value of 24 is used to mark the end of the season.</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public ushort Hour
        {
            get
            {
                return (ushort)
                    ((m_Switchpoint & (ushort)Definitions.HourMask) >>
                    (ushort)Definitions.HourShift);
            }
            set
            {
                // Clear the current hour
                m_Switchpoint = (ushort)(m_Switchpoint &
                    (ushort)Definitions.ClearHourMask);

                // Set the new hour
                m_Switchpoint = (ushort)(m_Switchpoint |
                    (value << (ushort)Definitions.HourShift));
            }
        }

        /// <summary>Returns the minute the switchpoint starts (0..59)</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public ushort Minute
        {
            // Minutes are defined as the number of 5 minute increments
            get
            {
                return (ushort)((m_Switchpoint & (ushort)Definitions.MinuteMask) * 5);
            }
            set
            {
                // Clear the current hour
                m_Switchpoint = (ushort)(m_Switchpoint &
                    (ushort)Definitions.ClearMinuteMask);

                // Set the new hour
                m_Switchpoint = (ushort)(m_Switchpoint | (value / 5));
            }
        }

        /// <summary>Returns the minute the switchpoint starts (0..59)</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public DayType TypeOfDay
        {
            get
            {
                return (DayType)(m_Switchpoint & (ushort)Definitions.DayTypeMask);
            }
        }

        /// <summary>Returns the description of the switchpoint for 
        /// debugging</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public string Description
        {
            get
            {
                string Description = "End Of Season";

                if ((ushort)Definitions.EndOfSeason != m_Switchpoint)
                {
                    switch (TypeOfDay)
                    {
                        case DayType.NormalDay1:
                            {
                                Description = "DayType 0: ";
                                break;
                            }
                        case DayType.NormalDay2:
                            {
                                Description = "DayType 1: ";
                                break;
                            }
                        case DayType.NormalDay3:
                            {
                                Description = "DayType 2: ";
                                break;
                            }
                        case DayType.Holiday:
                        default:
                            {
                                Description = "Holiday 0: ";
                                break;
                            }
                    }
                    if (SwitchpointType.Rate == Type)
                    {
                        switch (RateOutput)
                        {
                            case RateOutputType.RateA:
                                {
                                    Description += "RateA ";
                                    break;
                                }
                            case RateOutputType.RateB:
                                {
                                    Description += "RateB ";
                                    break;
                                }
                            case RateOutputType.RateC:
                                {
                                    Description += "RateC ";
                                    break;
                                }
                            case RateOutputType.RateD:
                                {
                                    Description += "RateD ";
                                    break;
                                }
                            case RateOutputType.RateE:
                                {
                                    Description += "RateE ";
                                    break;
                                }
                            case RateOutputType.NoRateOutput:
                                {
                                    Description += "All Rates OFF ";
                                    break;
                                }
                            default:
                                {
                                    Description += "Odd Selection of Rates ";
                                    break;
                                }
                        } // SWITCH for output type
                    } // IF Rate type
                    else if (m_IsFulcrum)
                    {
                        switch (RateOutput)
                        {
                            case (RateOutputType)FulcrumOutputs.Output1:
                                {
                                    Description += "Output1 ";
                                    break;
                                }
                            case (RateOutputType)FulcrumOutputs.Output2:
                                {
                                    Description += "Output2 ";
                                    break;
                                }
                            case (RateOutputType)FulcrumOutputs.Output3:
                                {
                                    Description += "Output3 ";
                                    break;
                                }
                            case (RateOutputType)FulcrumOutputs.Output4:
                                {
                                    Description += "Output4 ";
                                    break;
                                }
                            case (RateOutputType)FulcrumOutputs.NoRateOutput:
                                {
                                    Description += "All Independent Outputs OFF ";
                                    break;
                                }
                            default:
                                {
                                    Description += "Odd Outputs Selected ";
                                    break;
                                }
                        } // SWITCH for output type
                    }
                    else
                    {
                        switch (RateOutput)
                        {
                            case RateOutputType.Output1:
                                {
                                    Description += "Output1 ";
                                    break;
                                }
                            case RateOutputType.Output2:
                                {
                                    Description += "Output2 ";
                                    break;
                                }
                            case RateOutputType.Output3:
                                {
                                    Description += "Output3 ";
                                    break;
                                }
                            case RateOutputType.Output4:
                                {
                                    Description += "Output4 ";
                                    break;
                                }
                            case RateOutputType.NoRateOutput:
                                {
                                    Description += "All Independent Outputs OFF ";
                                    break;
                                }
                            default:
                                {
                                    Description += "Odd Outputs Selected ";
                                    break;
                                }
                        } // SWITCH for output type
                    } // ELSE if Output

                    Description = Description + Hour.ToString("D2", CultureInfo.InvariantCulture) + ":" +
                        Minute.ToString("D2", CultureInfo.InvariantCulture) + ", 0x" +
                        m_Switchpoint.ToString("X4", CultureInfo.InvariantCulture) + " ";

                } // IF it's not the end of the season

                return Description;
            }
        }


        /// <summary>Returns a byte[2] array representing the switchpoint data 
        /// </summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A	Created
        /// 
        public byte[] Data
        {
            get
            {
                byte[] Data = new byte[2];

                Data[0] = (byte)(m_Switchpoint >> 8);
                Data[1] = (byte)m_Switchpoint;

                return Data;
            }
        }


        /// <summary>Returns true if this is an End of Season 
        /// switchpoint</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00  N/A	Created
        /// 
        public bool IsEndOfSeason
        {
            get
            {
                return (ushort)Definitions.EndOfSeason == m_Switchpoint;
            }
        }

        #endregion Properties

        #region Members

        ushort m_Switchpoint;
        bool m_IsFulcrum;

        #endregion Members
    } // SCSSwitchpoint class

    /// <summary>
    /// This class represents a season configuration for SCS devices. A
    /// season is a collection of time sorted switchpoints. The last 
    /// switchpoint has an hour = 24 to designate the end of the season
    /// </summary>
    /// 
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 07/05/06 mcm 7.30.00  N/A	Created
    /// 
    public class SCSSeason : CollectionBase
    {
        #region Public Methods

        /// <summary>SCSSeason constructor</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public SCSSeason()
        {
            m_Logger = Logger.TheInstance;
        }

        /// <summary>SCSSeason constructor</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public SCSSeason(bool IsFulcrum, byte[] SeasonData)
        {
            m_Logger = Logger.TheInstance;
            ushort SwitchPoint;

            for (int i = 0; i < SeasonData.Length; )
            {
                SwitchPoint = (ushort)(SeasonData[i++] * 0x100 + SeasonData[i++]);
                Add(new SCSSwitchpoint(IsFulcrum, SwitchPoint));
            }
        }

        /// <summary>Adds a switchpoint to the end of the season. Sorting will
        /// be added if needed</summary>
        /// <param name="Switchpoint">SCSSwitchpoint to add to the season</param>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public void Add(SCSSwitchpoint Switchpoint)
        {
            InnerList.Add(Switchpoint);
        }

        /// <summary>Sorts the switchpoint chronologically</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public void Sort()
        {
            SCSSwitchpoint SP;

            for (int iIterate = 0; iIterate < InnerList.Count; iIterate++)
            {
                // Checks for out of order events
                while ((iIterate > 0) &&
                    (((SCSSwitchpoint)InnerList[iIterate]) <
                     ((SCSSwitchpoint)InnerList[iIterate - 1])))
                {
                    // Swap the out of order events
                    SP = ((SCSSwitchpoint)InnerList[iIterate - 1]);
                    InnerList[iIterate - 1] = InnerList[iIterate];
                    InnerList[iIterate] = SP;

                    // On loop back check the order of swapped event against 
                    // its new previous event 
                    iIterate--;
                }
            }
        }

        /// <summary>Removes All Outputs Off switchpoints that fall on an 
        /// output on switchpoint. Does not remove All Outputs Off SPs that 
        /// start at midnight.</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/18/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public void RemoveExtraOutputs()
        {
            SCSSwitchpoint SP1;
            SCSSwitchpoint SP2;
            bool ExtraOutput;


            // Check all outputs off switchpoints. If there's another 
            // output SP with the same daytype at the same time, remove
            // the all outputs off SP.
            for (int I = 0; I < InnerList.Count; I++)
            {
                SP1 = (SCSSwitchpoint)InnerList[I];

                if (((0 != SP1.Hour) || (0 != SP1.Minute)) &&
                   (SCSSwitchpoint.SwitchpointType.Output == SP1.Type) &&
                   (SCSSwitchpoint.RateOutputType.NoRateOutput == SP1.RateOutput))
                {
                    // SP might be an extra, search for a match.
                    ExtraOutput = false;

                    for (int J = I + 1; J < InnerList.Count; J++)
                    {
                        SP2 = (SCSSwitchpoint)InnerList[J];

                        if ((SP1.TypeOfDay == SP2.TypeOfDay) &&
                           (SP1.Hour == SP2.Hour) &&
                           (SP1.Minute == SP2.Minute) &&
                           (SCSSwitchpoint.SwitchpointType.Output == SP2.Type))
                        {
                            ExtraOutput = true;
                            break;
                        }
                    }

                    if (ExtraOutput)
                    {
                        InnerList.RemoveAt(I);
                        I--;  // decrement I so we don't skip one
                    }
                }
            }
        }

        /// <summary>Dumps the contents of the collection for debugging</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public void Dump()
        {
            foreach (SCSSwitchpoint Switchpoint in InnerList)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    Switchpoint.Description);
            }
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>Returns the size in bytes for this season</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public ushort Size
        {
            get
            {
                return (ushort)(InnerList.Count * 2);
            }
        }


        /// <summary>Returns an array representing this season's configuraton
        /// </summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public byte[] Data
        {
            get
            {
                byte[] Data = new byte[InnerList.Count * 2];
                SCSSwitchpoint SP;

                for (int Index = 0; Index < InnerList.Count; Index++)
                {
                    SP = (SCSSwitchpoint)InnerList[Index];
                    Array.Copy(SP.Data, 0, Data, Index * 2, 2);
                }

                return Data;
            }
        }


        #endregion Properties

        #region Members

        private Logger m_Logger;

        #endregion

    } // SCSSeason


    /// <summary>
    /// This class represents the collection of seasons for configuration of 
    /// SCS devices. </summary>
    /// 
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ -------------------------------------------
    /// 07/05/06 mcm 7.30.00  N/A	Created
    /// 
    public class SCSSeasonCollection : CollectionBase
    {
        #region Public Methods

        /// <summary>SCSSeason constructor</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public SCSSeasonCollection()
        {
            m_Logger = Logger.TheInstance;
        }

        /// <summary>Add a season to the end of the collections. Seasons are
        /// defined in the order they are added.</summary>
        /// <param name="Season">Season to add to the collection</param>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public void Add(SCSSeason Season)
        {
            InnerList.Add(Season);
        }

        /// <summary>Add a season to the end of the collections. Seasons are
        /// defined in the order they are added.</summary>
        /// <param name="IsFulcrum">true if this is a Fulcrum</param>
        /// <param name="SeasonData">byte stream read from the meter of the
        /// season to add to the collection</param>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Created
        /// 
        public void Add(bool IsFulcrum, byte[] SeasonData)
        {
            InnerList.Add(new SCSSeason(IsFulcrum, SeasonData));
        }

        /// <summary>Sorts the Switchpoints in each season</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00  N/A	Converted class  
        public void Sort()
        {
            foreach (SCSSeason Season in InnerList)
            {
                Season.Sort();
            }
        }

        /// <summary>Removes All Outputs Off switchpoints that fall on an 
        /// output on switchpoint</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/18/06 mcm 7.30.00  N/A	Converted class  
        public void RemoveExtraOutputs()
        {
            foreach (SCSSeason Season in InnerList)
            {
                Season.RemoveExtraOutputs();
            }
        }


        /// <summary>Dumps the contents of the collection for debugging</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/05/06 mcm 7.30.00  N/A	Converted class  
        public void Dump()
        {
            int Index = 0;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "Dump of the SCSSeasonCollection");

            foreach (SCSSeason Season in InnerList)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "Season " + Index++);

                Season.Dump();
            }

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "End Dump of the SCSSeasonCollection");
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>Returns the size in bytes for all seasons in the 
        /// collection</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public ushort Size
        {
            get
            {
                ushort Size = 0;

                foreach (SCSSeason Season in InnerList)
                {
                    Size += Season.Size;
                }

                return Size;
            }
        }

        /// <summary>Returns the size in bytes of Season0</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public ushort Season0Size
        {
            get
            {
                if (InnerList.Count > 0)
                {
                    return (ushort)(((SCSSeason)InnerList[0]).Count * 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>Returns the size in bytes of Season 1</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public ushort Season1Size
        {
            get
            {
                if (InnerList.Count > 1)
                {
                    return (ushort)(((SCSSeason)InnerList[1]).Count * 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>Returns the size in bytes of Season 2</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public ushort Season2Size
        {
            get
            {
                if (InnerList.Count > 2)
                {
                    return (ushort)(((SCSSeason)InnerList[2]).Count * 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>Returns the size in bytes of Season 3</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public ushort Season3Size
        {
            get
            {
                if (InnerList.Count > 3)
                {
                    return (ushort)(((SCSSeason)InnerList[3]).Count * 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>Returns the size in bytes of Season 4</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public ushort Season4Size
        {
            get
            {
                if (InnerList.Count > 4)
                {
                    return (ushort)(((SCSSeason)InnerList[4]).Count * 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>Returns the size in bytes of Season 5</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public ushort Season5Size
        {
            get
            {
                if (InnerList.Count > 5)
                {
                    return (ushort)(((SCSSeason)InnerList[5]).Count * 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>Returns the size in bytes of Season 6</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public ushort Season6Size
        {
            get
            {
                if (InnerList.Count > 6)
                {
                    return (ushort)(((SCSSeason)InnerList[6]).Count * 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>Returns the size in bytes of Season 7</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public ushort Season7Size
        {
            get
            {
                if (InnerList.Count > 7)
                {
                    return (ushort)(((SCSSeason)InnerList[7]).Count * 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>Returns the Season collection as an array of bytes</summary>
        /// 
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A	Converted class  
        /// 
        public byte[] Data
        {
            get
            {
                byte[] Data = new byte[Size];
                SCSSeason Season;
                int DataIndex = 0;

                for (int Index = 0; Index < InnerList.Count; Index++)
                {
                    Season = (SCSSeason)InnerList[Index];
                    Array.Copy(Season.Data, 0, Data, DataIndex, Season.Data.Length);
                    DataIndex += Season.Data.Length;
                }

                return Data;
            }
        }

        #endregion

        #region Members

        private Logger m_Logger;

        #endregion

    } // SCSSeasonCollection


    /// <summary>
    /// This class manages the block of TOU info for SCS devices. It reads it,
    /// writes it, dumps it, and provides access to each item</summary>
    /// 
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ -------------------------------------------
    /// 07/06/06 mcm 7.30.00  N/A	Created
    /// 
    public class SCSTOUInfo
    {
        #region Constants
        /// <summary>Size of this class' data</summary>
        public const ushort SIZE_OF_TOU_INFO = 35;

        private const int SIZE_OF_END_OF_CALENDAR = 2;
        private const byte TYPICAL_MONDAY_MASK = 0x03;
        private const byte TYPICAL_TUESDAY_MASK = 0x0C;
        private const byte TYPICAL_WEDNESDAY_MASK = 0x30;
        private const byte TYPICAL_THURSDAY_MASK = 0xC0;
        private const byte TYPICAL_FRIDAY_MASK = 0x03;
        private const byte TYPICAL_SATURDAY_MASK = 0x0C;
        private const byte TYPICAL_SUNDAY_MASK = 0x30;

        private const byte TYPICAL_MONDAY_SHIFT = 0;
        private const byte TYPICAL_TUESDAY_SHIFT = 2;
        private const byte TYPICAL_WEDNESDAY_SHIFT = 4;
        private const byte TYPICAL_THURSDAY_SHIFT = 6;
        private const byte TYPICAL_FRIDAY_SHIFT = 0;
        private const byte TYPICAL_SATURDAY_SHIFT = 2;
        private const byte TYPICAL_SUNDAY_SHIFT = 4;

        private const byte CLEAR_TYPICAL_MONDAY = 0xFC;
        private const byte CLEAR_TYPICAL_TUESDAY = 0xF3;
        private const byte CLEAR_TYPICAL_WEDNESDAY = 0xCF;
        private const byte CLEAR_TYPICAL_THURSDAY = 0x3F;
        private const byte CLEAR_TYPICAL_FRIDAY = 0xFC;
        private const byte CLEAR_TYPICAL_SATURDAY = 0xF3;
        private const byte CLEAR_TYPICAL_SUNDAY = 0xCF;

        #endregion

        #region Definitions

        /// <summary>Offsets of the data to be used with the Data[]</summary>
        private enum DataOffset : int
        {
            /// <summary>Offset of the 2 byte Yearly Schedule value</summary>
            START_OF_YEARLY_SCHED = 0,
            /// <summary>Offset of the 2 byte Daily Sched value. 
            /// 2 unused bytes follow this value</summary>
            START_OF_DAILY_SCHED = 2,
            /// <summary>Offset of the 4 byte BCD expiration date yyyymmdd</summary>
            EXPIR_DATE = 6,
            /// <summary>Offset of the 2 byte TOU Schedule ID value</summary>
            SCHED_ID = 10,
            /// <summary>Offset of the 2 byte Base Address value. Same as the 
            /// Start of Yearly Schedule for CENTRON, VECTRON and MT200. For  
            /// the FULCRUM, this is the start of the TOU Header info. This 
            /// value is followed by 3 unused bytes.</summary>
            TOU_BASE_ADDRESS = 12,
            /// <summary>Offset of the 2 byte address Season 0</summary>
            SEASON_0 = 17,
            /// <summary>Offset of the 2 byte address Season 1</summary>
            SEASON_1 = 19,
            /// <summary>Offset of the 2 byte address Season 2</summary>
            SEASON_2 = 21,
            /// <summary>Offset of the 2 byte address Season 3</summary>
            SEASON_3 = 23,
            /// <summary>Offset of the 2 byte address Season 4</summary>
            SEASON_4 = 25,
            /// <summary>Offset of the 2 byte address Season 5</summary>
            SEASON_5 = 27,
            /// <summary>Offset of the 2 byte address Season 6</summary>
            SEASON_6 = 29,
            /// <summary>Offset of the 2 byte address Season 7</summary>
            SEASON_7 = 31,
            /// <summary>Offset of the 2 byte typical week value</summary>
            TYPICAL_WEEK = 33,
        }

        #endregion Definitions

        #region Public Methods

        /// <summary>Constructs an empty block of SCSTOUInfo data</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A	Created
        /// 
        public SCSTOUInfo()
        {
            m_Logger = Logger.TheInstance;
            m_Data = new byte[SIZE_OF_TOU_INFO];
            m_Data.Initialize();
        }

        /// <summary>SCSTOUInfo constructor</summary>
        /// <param name="Data">TOU Info Data as read from the meter. This 
        /// should be exactly 33 bytes or I will throw up on you!</param>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A	Created
        /// 
        public SCSTOUInfo(byte[] Data)
        {
            if (SIZE_OF_TOU_INFO != Data.Length)
            {
                throw (new Exception("Can't instantiate SCSTOUInfo with " +
                    Data.Length + " bytes of data"));
            }

            m_Logger = Logger.TheInstance;
            m_Data = Data;
        }

        /// <summary>Dumps the SCS TOU info for debugging</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A	Created
        /// 
        public virtual SCSProtocolResponse Dump()
        {
            SCSProtocolResponse Result = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "Starting Dump of SCSTOUInfo");

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "StartOfYearlySchedule = 0x" + StartOfYearlySchedule.ToString("X4", CultureInfo.InvariantCulture));
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "StartOfDailySchedule = 0x" + StartOfDailySchedule.ToString("X4", CultureInfo.InvariantCulture));
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "ExpirationDate = " + ExpirationDate.ToString("d", CultureInfo.InvariantCulture));
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "ScheduleID = 0x" + ScheduleID.ToString("X4", CultureInfo.InvariantCulture));
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "TOUBaseAddress = 0x" + TOUBaseAddress.ToString("X4", CultureInfo.InvariantCulture));
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "StartOfSeason0 = 0x" + StartOfSeason0.ToString("X4", CultureInfo.InvariantCulture));
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "StartOfSeason1 = 0x" + StartOfSeason1.ToString("X4", CultureInfo.InvariantCulture));
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "StartOfSeason2 = 0x" + StartOfSeason2.ToString("X4", CultureInfo.InvariantCulture));
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "StartOfSeason3 = 0x" + StartOfSeason3.ToString("X4", CultureInfo.InvariantCulture));
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "StartOfSeason4 = 0x" + StartOfSeason4.ToString("X4", CultureInfo.InvariantCulture));
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "StartOfSeason5 = 0x" + StartOfSeason5.ToString("X4", CultureInfo.InvariantCulture));
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "StartOfSeason6 = 0x" + StartOfSeason6.ToString("X4", CultureInfo.InvariantCulture));
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "StartOfSeason7 = 0x" + StartOfSeason7.ToString("X4", CultureInfo.InvariantCulture));

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "Typical Monday daytype = " + TypicalMonday);
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "Typical Tuesday daytype = " + TypicalTuesday);
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "Typical Wednesday daytype = " + TypicalWednesday);
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "Typical Thursday daytype = " + TypicalThursday);
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "Typical Friday daytype = " + TypicalFriday);
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "Typical Saturday daytype = " + TypicalSaturday);
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "Typical Sunday daytype = " + TypicalSunday);

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "End Dump of SCSTOUInfo");

            return Result;

        } // Dump

        #endregion Public Methods

        #region Public Properties

        /// <summary>Retrurns true if Season 0 has more than just an end of
        /// calendar flag in it</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/24/06 mcm 7.30.00  N/A   Created
        ///
        public virtual bool TOUIsConfigured
        {
            get
            {
                if (SIZE_OF_END_OF_CALENDAR < SizeOfSeason0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>Provides access to set the data block</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///
        public byte[] Data
        {
            get
            {
                return m_Data;
            }
            set
            {
                if (SIZE_OF_TOU_INFO != value.Length)
                {
                    throw (new Exception("Can't instantiate SCSTOUInfo with " +
                        value.Length + " bytes of data"));
                }

                m_Data = value;
            }
        }


        /// <summary>Provides access to the address of the start of the yearly
        /// schedule</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///
        public virtual ushort StartOfYearlySchedule
        {
            get
            {
                return (ushort)(m_Data[YearlySchedOffset]
                     * 0x100 + m_Data[YearlySchedOffset + 1]);
            }
            set
            {
                m_Data[YearlySchedOffset] = (byte)(value >> 8);
                m_Data[YearlySchedOffset + 1] = (byte)value;
            }
        }


        /// <summary>Provides access to the address of the start of the daily
        /// schedule</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///
        public virtual ushort StartOfDailySchedule
        {
            get
            {
                return (ushort)(m_Data[DailySchedOffset] * 0x100
                     + m_Data[DailySchedOffset + 1]);
            }
            set
            {
                m_Data[DailySchedOffset] = (byte)(value >> 8);
                m_Data[DailySchedOffset + 1] = (byte)value;
            }
        }


        /// <summary>Provides access to the address of the expiration date of 
        /// the schedule</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///
        public virtual DateTime ExpirationDate
        {
            get
            {
                // Date is in BCD stored as yy yy mm dd
                int Year = (m_Data[ExpirDateOffset] >> 4) * 1000 +
                    (m_Data[ExpirDateOffset] & 0x0F) * 100 +
                    (m_Data[ExpirDateOffset + 1] >> 4) * 10 +
                    (m_Data[ExpirDateOffset + 1] & 0x0F);
                int Month = (m_Data[ExpirDateOffset + 2] >> 4) * 10 +
                    (m_Data[ExpirDateOffset + 2] & 0x0F);
                int Day = (m_Data[ExpirDateOffset + 3] >> 4) * 10 +
                    (m_Data[ExpirDateOffset + 3] & 0x0F);
                return new DateTime(Year, Month, Day);
            }
            set
            {
                m_Data[ExpirDateOffset] =
                    BCD.BytetoBCD((byte)(value.Year / 100));
                m_Data[ExpirDateOffset + 1] =
                    BCD.BytetoBCD((byte)(value.Year % 100));
                m_Data[ExpirDateOffset + 2] =
                    BCD.BytetoBCD((byte)value.Month);
                m_Data[ExpirDateOffset + 3] =
                    BCD.BytetoBCD((byte)value.Day);
            }
        }

        /// <summary>Provides access to the TOU Schedule ID</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///
        public virtual ushort ScheduleID
        {
            // mcm 9/7/06 TOU ID is stored in BCD
            get
            {
                return (ushort)(BCD.BCDtoByte(m_Data[SchedIDOffset]) * 0x100 +
                    BCD.BCDtoByte(m_Data[SchedIDOffset + 1]));
            }
            set
            {
                byte[] BCDValue = BCD.InttoBCD(value, 2);

                m_Data[SchedIDOffset] = BCDValue[0];
                m_Data[SchedIDOffset + 1] = BCDValue[1];
            }
        }

        /// <summary>Provides access to the TOU Base address</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///
        public virtual ushort TOUBaseAddress
        {
            get
            {
                return (ushort)(m_Data[BaseAddrOffset] * 0x100 +
                    m_Data[BaseAddrOffset + 1]);
            }
            set
            {
                m_Data[BaseAddrOffset] = (byte)(value >> 8);
                m_Data[BaseAddrOffset + 1] = (byte)value;
            }
        }


        /// <summary>Provides access to the address of Season 0</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///
        public virtual ushort StartOfSeason0
        {
            get
            {
                return (ushort)(m_Data[Season0Offset] * 0x100 +
                    m_Data[Season0Offset + 1]);
            }
            set
            {
                m_Data[Season0Offset] = (byte)(value >> 8);
                m_Data[Season0Offset + 1] = (byte)value;
            }
        }


        /// <summary>Provides access to the address of Season 1</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///
        public virtual ushort StartOfSeason1
        {
            get
            {
                return (ushort)(m_Data[Season1Offset] * 0x100 +
                    m_Data[Season1Offset + 1]);
            }
            set
            {
                m_Data[Season1Offset] = (byte)(value >> 8);
                m_Data[Season1Offset + 1] = (byte)value;
            }
        }

        /// <summary>Provides access to the address of Season 2</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///
        public virtual ushort StartOfSeason2
        {
            get
            {
                return (ushort)(m_Data[Season2Offset] * 0x100 +
                    m_Data[Season2Offset + 1]);
            }
            set
            {
                m_Data[Season2Offset] = (byte)(value >> 8);
                m_Data[Season2Offset + 1] = (byte)value;
            }
        }


        /// <summary>Provides access to the address of Season 3</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///
        public virtual ushort StartOfSeason3
        {
            get
            {
                return (ushort)(m_Data[Season3Offset] * 0x100 +
                    m_Data[Season3Offset + 1]);
            }
            set
            {
                m_Data[Season3Offset] = (byte)(value >> 8);
                m_Data[Season3Offset + 1] = (byte)value;
            }
        }


        /// <summary>Provides access to the address of Season 4</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///
        public virtual ushort StartOfSeason4
        {
            get
            {
                return (ushort)(m_Data[Season4Offset] * 0x100 +
                    m_Data[Season4Offset + 1]);
            }
            set
            {
                m_Data[Season4Offset] = (byte)(value >> 8);
                m_Data[Season4Offset + 1] = (byte)value;
            }
        }


        /// <summary>Provides access to the address of Season 5</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///
        public virtual ushort StartOfSeason5
        {
            get
            {
                return (ushort)(m_Data[Season5Offset] * 0x100 +
                    m_Data[Season5Offset + 1]);
            }
            set
            {
                m_Data[Season5Offset] = (byte)(value >> 8);
                m_Data[Season5Offset + 1] = (byte)value;
            }
        }


        /// <summary>Provides access to the address of Season 6</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///
        public virtual ushort StartOfSeason6
        {
            get
            {
                return (ushort)(m_Data[Season6Offset] * 0x100 +
                    m_Data[Season6Offset + 1]);
            }
            set
            {
                m_Data[Season6Offset] = (byte)(value >> 8);
                m_Data[Season6Offset + 1] = (byte)value;
            }
        }


        /// <summary>Provides access to the address of Season 7</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///
        public virtual ushort StartOfSeason7
        {
            get
            {
                return (ushort)(m_Data[Season7Offset] * 0x100 +
                    m_Data[Season7Offset + 1]);
            }
            set
            {
                m_Data[Season7Offset] = (byte)(value >> 8);
                m_Data[Season7Offset + 1] = (byte)value;
            }
        }


        /// <summary>Returns the size of the Yearly Schedule</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00  N/A   Created
        ///
        public int SizeOfYearlySchedule
        {
            get
            {
                return StartOfSeason0 - StartOfYearlySchedule;
            }
        }

        /// <summary>Returns the size of Season 0</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int SizeOfSeason0
        {
            get
            {
                return StartOfSeason1 - StartOfSeason0;
            }
        }

        /// <summary>Returns the size of Season 1</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int SizeOfSeason1
        {
            get
            {
                return StartOfSeason2 - StartOfSeason1;
            }
        }

        /// <summary>Returns the size of Season 2</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int SizeOfSeason2
        {
            get
            {
                return StartOfSeason3 - StartOfSeason2;
            }
        }

        /// <summary>Returns the size of Season 3</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int SizeOfSeason3
        {
            get
            {
                return StartOfSeason4 - StartOfSeason3;
            }
        }

        /// <summary>Returns the size of Season 4</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int SizeOfSeason4
        {
            get
            {
                return StartOfSeason5 - StartOfSeason4;
            }
        }

        /// <summary>Returns the size of Season 5</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int SizeOfSeason5
        {
            get
            {
                return StartOfSeason6 - StartOfSeason5;
            }
        }

        /// <summary>Returns the size of Season 6</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int SizeOfSeason6
        {
            get
            {
                return StartOfSeason7 - StartOfSeason6;
            }
        }

        // The Size of Season 7 can't be determined by this class!
        //public int SizeOfSeason7


        /// <summary>Provides access to the typical Monday daytype
        /// assignments</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/11/06 mcm 7.30.00  N/A   Created
        ///
        public byte TypicalMonday
        {
            get
            {
                return (byte)((m_Data[TypicalWeekOffset] &
                    TYPICAL_MONDAY_MASK) >> TYPICAL_MONDAY_SHIFT);
            }
            set
            {
                m_Data[TypicalWeekOffset] = (byte)
                    (m_Data[TypicalWeekOffset] &
                    CLEAR_TYPICAL_MONDAY);
                m_Data[TypicalWeekOffset] = (byte)
                    (m_Data[TypicalWeekOffset] |
                    (value << TYPICAL_MONDAY_SHIFT));
            }
        }

        /// <summary>Provides access to the typical Tuesday daytype
        /// assignments</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/11/06 mcm 7.30.00  N/A   Created
        ///
        public byte TypicalTuesday
        {
            get
            {
                return (byte)((m_Data[TypicalWeekOffset] &
                    TYPICAL_TUESDAY_MASK) >> TYPICAL_TUESDAY_SHIFT);
            }
            set
            {
                m_Data[TypicalWeekOffset] = (byte)
                    (m_Data[TypicalWeekOffset] &
                    CLEAR_TYPICAL_TUESDAY);
                m_Data[TypicalWeekOffset] = (byte)
                    (m_Data[TypicalWeekOffset] |
                    (value << TYPICAL_TUESDAY_SHIFT));
            }
        }

        /// <summary>Provides access to the typical Wednesday daytype
        /// assignments</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/11/06 mcm 7.30.00  N/A   Created
        ///
        public byte TypicalWednesday
        {
            get
            {
                return (byte)((m_Data[TypicalWeekOffset] &
                    TYPICAL_WEDNESDAY_MASK) >> TYPICAL_WEDNESDAY_SHIFT);
            }
            set
            {
                m_Data[TypicalWeekOffset] = (byte)
                    (m_Data[TypicalWeekOffset] &
                    CLEAR_TYPICAL_WEDNESDAY);
                m_Data[TypicalWeekOffset] = (byte)
                    (m_Data[TypicalWeekOffset] |
                    (value << TYPICAL_WEDNESDAY_SHIFT));
            }
        }

        /// <summary>Provides access to the typical Thursday daytype
        /// assignments</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/11/06 mcm 7.30.00  N/A   Created
        ///
        public byte TypicalThursday
        {
            get
            {
                return (byte)((m_Data[TypicalWeekOffset] &
                    TYPICAL_THURSDAY_MASK) >> TYPICAL_THURSDAY_SHIFT);
            }
            set
            {
                m_Data[TypicalWeekOffset] = (byte)
                    (m_Data[TypicalWeekOffset] &
                    CLEAR_TYPICAL_THURSDAY);
                m_Data[TypicalWeekOffset] = (byte)
                    (m_Data[TypicalWeekOffset] |
                    (value << TYPICAL_THURSDAY_SHIFT));
            }
        }

        /// <summary>Provides access to the typical Friday daytype
        /// assignments</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/11/06 mcm 7.30.00  N/A   Created
        ///
        public byte TypicalFriday
        {
            get
            {
                return (byte)((m_Data[TypicalWeekOffset + 1] &
                    TYPICAL_FRIDAY_MASK) >> TYPICAL_FRIDAY_SHIFT);
            }
            set
            {
                m_Data[TypicalWeekOffset + 1] = (byte)
                    (m_Data[TypicalWeekOffset + 1] &
                    CLEAR_TYPICAL_FRIDAY);
                m_Data[TypicalWeekOffset + 1] = (byte)
                    (m_Data[TypicalWeekOffset + 1] |
                    (value << TYPICAL_FRIDAY_SHIFT));
            }
        }

        /// <summary>Provides access to the typical Saturday daytype
        /// assignments</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/11/06 mcm 7.30.00  N/A   Created
        ///
        public byte TypicalSaturday
        {
            get
            {
                return (byte)((m_Data[TypicalWeekOffset + 1] &
                    TYPICAL_SATURDAY_MASK) >> TYPICAL_SATURDAY_SHIFT);
            }
            set
            {
                m_Data[TypicalWeekOffset + 1] = (byte)
                    (m_Data[TypicalWeekOffset + 1] &
                    CLEAR_TYPICAL_SATURDAY);
                m_Data[TypicalWeekOffset + 1] = (byte)
                    (m_Data[TypicalWeekOffset + 1] |
                    (value << TYPICAL_SATURDAY_SHIFT));
            }
        }

        /// <summary>Provides access to the typical Sunday daytype
        /// assignments</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/11/06 mcm 7.30.00  N/A   Created
        ///
        public byte TypicalSunday
        {
            get
            {
                return (byte)((m_Data[TypicalWeekOffset + 1] &
                    TYPICAL_SUNDAY_MASK) >> TYPICAL_SUNDAY_SHIFT);
            }
            set
            {
                m_Data[TypicalWeekOffset + 1] = (byte)
                    (m_Data[TypicalWeekOffset + 1] &
                    CLEAR_TYPICAL_SUNDAY);
                m_Data[TypicalWeekOffset + 1] = (byte)
                    (m_Data[TypicalWeekOffset + 1] |
                    (value << TYPICAL_SUNDAY_SHIFT));
            }
        }

        /// <summary>Returns the Fulcrum's START_OF_YEARLY_SCHED data offset
        /// </summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/21/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int YearlySchedOffset
        {
            get
            {
                return (int)DataOffset.START_OF_YEARLY_SCHED;
            }
        }

        /// <summary>Returns the Fulcrum's  data offset</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/21/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int DailySchedOffset
        {
            get
            {
                return (int)DataOffset.START_OF_DAILY_SCHED;
            }
        }

        /// <summary>Returns the Fulcrum's EXPIR_DATE data offset</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/21/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int ExpirDateOffset
        {
            get
            {
                return (int)DataOffset.EXPIR_DATE;
            }
        }

        /// <summary>Returns the Fulcrum's SCHED_ID data offset</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/21/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int SchedIDOffset
        {
            get
            {
                return (int)DataOffset.SCHED_ID;
            }
        }

        /// <summary>Returns the Fulcrum's TOU_BASE_ADDRESS data offset</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/21/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int BaseAddrOffset
        {
            get
            {
                return (int)DataOffset.TOU_BASE_ADDRESS;
            }
        }

        /// <summary>Returns the Fulcrum's SEASON_0 data offset</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/21/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int Season0Offset
        {
            get
            {
                return (int)DataOffset.SEASON_0;
            }
        }

        /// <summary>Returns the Fulcrum's SEASON_1 data offset</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/21/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int Season1Offset
        {
            get
            {
                return (int)DataOffset.SEASON_1;
            }
        }

        /// <summary>Returns the Fulcrum's SEASON_2 data offset</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/21/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int Season2Offset
        {
            get
            {
                return (int)DataOffset.SEASON_2;
            }
        }

        /// <summary>Returns the Fulcrum's SEASON_3 data offset</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/21/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int Season3Offset
        {
            get
            {
                return (int)DataOffset.SEASON_3;
            }
        }

        /// <summary>Returns the Fulcrum's SEASON_4 data offset</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/21/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int Season4Offset
        {
            get
            {
                return (int)DataOffset.SEASON_4;
            }
        }

        /// <summary>Returns the Fulcrum's SEASON_5 data offset</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/21/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int Season5Offset
        {
            get
            {
                return (int)DataOffset.SEASON_5;
            }
        }

        /// <summary>Returns the Fulcrum's SEASON_6 data offset</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/21/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int Season6Offset
        {
            get
            {
                return (int)DataOffset.SEASON_6;
            }
        }

        /// <summary>Returns the Fulcrum's SEASON_7 data offset</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/21/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int Season7Offset
        {
            get
            {
                return (int)DataOffset.SEASON_7;
            }
        }


        /// <summary>Returns the Fulcrum's Typical Week data offset</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/21/06 mcm 7.30.00  N/A   Created
        ///
        public virtual int TypicalWeekOffset
        {
            get
            {
                return (int)DataOffset.TYPICAL_WEEK;
            }
        }

        #endregion

        #region Members

        /// <summary>The debug logger</summary>
        protected Logger m_Logger;

        /// <summary>
        /// The data for the TOU Info section. Note that this is not all of the
        /// Data for Fulcrum devices. For those devices the TOU ID and 
        /// expiration date need to be handled separately.
        /// </summary>
        protected byte[] m_Data;

        #endregion

    } // SCSTOUInfo

}
