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
//                              Copyright © 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using Itron.Metering.DST;
using Itron.Metering.TOU;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Sub Table class for the DST Configuration in MFG Table 212 (2260)
    /// </summary>
    public class OpenWayMFGTable2260DSTCalendar : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 16;
        private const ushort TABLE_SIZE = 128;

        private const ushort EVENT_MASK = 0x000F;
        private const ushort MONTH_MASK = 0x00F0;
        private const ushort DAY_MASK = 0x1F00;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/27/11 RCG 2.50.00        Created
        
        public OpenWayMFGTable2260DSTCalendar(CPSEM psem)
            : base(psem, 2260, TABLE_OFFSET, TABLE_SIZE)
        {
        }

        /// <summary>
        /// Reads the sub table from the meter.
        /// </summary>
        /// <returns>The result of the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/27/11 RCG 2.50.00        Created

        public override PSEMResponse Read()
        {
            PSEMResponse Response = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2260DSTCalendar.Read()");

            Response = base.Read();

            if (Response == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Response;
        }

        /// <summary>
        /// Writes the table to the meter.
        /// </summary>
        /// <returns>The result of the write.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/23/11 RCG 2.50.05        Created

        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2260DSTCalendar.Write");

            // Resynch our members to the base's data array
            m_DataStream.Position = 0;

            m_Writer.Write(m_byDSTHour);
            m_Writer.Write(m_byDSTMinute);
            m_Writer.Write(m_byDSTOffset);

            for (int iIndex = 0; iIndex < 25; iIndex++)
            {
                byte Year = 0xFF;
                ushort ToDSTDayEvent = 0xFFFF;
                ushort FromDSTDayEvent = 0xFFFF;

                // If we have less than 25 DST dates we will fill the empty dates with FF's
                if (iIndex < m_DSTDates.Count)
                {
                    // Combine the To Date

                    // Month and Day are 0 based in the meter so subtract 1
                    ushort DayOfMonth = (ushort)(m_DSTDates[iIndex].ToDate.Day - 1);
                    ushort Month = (ushort)(m_DSTDates[iIndex].ToDate.Month - 1);

                    ToDSTDayEvent = (ushort)(((DayOfMonth << 8) & DAY_MASK) | ((Month << 4) & MONTH_MASK) | ((ushort)(AMICalendarEvent.AMICalendarEventType.DST_ON) & EVENT_MASK));

                    // Now Combine the From Date
                    DayOfMonth = (ushort)(m_DSTDates[iIndex].FromDate.Day - 1);
                    Month = (ushort)(m_DSTDates[iIndex].FromDate.Month - 1);

                    FromDSTDayEvent = (ushort)(((DayOfMonth << 8) & DAY_MASK) | ((Month << 4) & MONTH_MASK) | ((ushort)(AMICalendarEvent.AMICalendarEventType.DST_OFF) & EVENT_MASK));

                    // The year is an offset from 2000 in the meter
                    Year = (byte)(m_DSTDates[iIndex].ToDate.Year - 2000);
                }

                // Write the values
                m_Writer.Write(Year);
                m_Writer.Write(ToDSTDayEvent);
                m_Writer.Write(FromDSTDayEvent);
            }

            return base.Write();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the configured DST change hour
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/27/11 RCG 2.50.00        Created

        public byte DSTHour
        {
            get
            {
                ReadUnloadedTable();

                return m_byDSTHour;
            }
            set
            {
                State = TableState.Dirty;

                m_byDSTHour = value;
            }
        }

        /// <summary>
        /// Gets the configured DST change minute
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/27/11 RCG 2.50.00        Created

        public byte DSTMinute
        {
            get
            {
                ReadUnloadedTable();

                return m_byDSTMinute;
            }
            set
            {
                State = TableState.Dirty;

                m_byDSTMinute = value;
            }
        }

        /// <summary>
        /// Gets the configured DST change offset
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/27/11 RCG 2.50.00        Created

        public byte DSTOffset
        {
            get
            {
                ReadUnloadedTable();

                return m_byDSTOffset;
            }
            set
            {
                State = TableState.Dirty;

                m_byDSTOffset = value;
            }
        }

        /// <summary>
        /// Gets the list of configured DST dates
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/27/11 RCG 2.50.00        Created

        public List<CDSTDatePair> DSTDates
        {
            get
            {
                ReadUnloadedTable();

                return m_DSTDates;
            }
            set
            {
                State = TableState.Dirty;

                m_DSTDates = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the sub table from the data that was just read.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/27/11 RCG 2.50.00        Created

        private void ParseData()
        {
            m_byDSTHour = m_Reader.ReadByte();
            m_byDSTMinute = m_Reader.ReadByte();
            m_byDSTOffset = m_Reader.ReadByte();

            m_DSTDates = new List<CDSTDatePair>();

            for (int iIndex = 0; iIndex < 25; iIndex++)
            {
                byte Year = m_Reader.ReadByte();

                ushort[] DayEvents = new ushort[2];
                DayEvents[0] = m_Reader.ReadUInt16();
                DayEvents[1] = m_Reader.ReadUInt16();

                if (Year != 0xFF)
                {
                    CDSTDatePair DSTDate = new CDSTDatePair();

                    foreach (ushort CurrentDayEvent in DayEvents)
                    {
                        // The Event Type, Month, and Day values are each nibbles within the UINT16 value
                        AMICalendarEvent.AMICalendarEventType EventType = (AMICalendarEvent.AMICalendarEventType)(CurrentDayEvent & EVENT_MASK);

                        // The Month and Day values are 0 based (0 == January and 0 == 1st day of the month) so add 1
                        int Month = ((CurrentDayEvent & MONTH_MASK) >> 4) + 1;
                        int Day = ((CurrentDayEvent & DAY_MASK) >> 8) + 1;
                        int Hour = 0;
                        int Minute = 0;

                        if (m_byDSTHour < 24)
                        {
                            Hour = m_byDSTHour;
                        }

                        if (m_byDSTMinute < 60)
                        {
                            Minute = m_byDSTMinute;
                        }

                        DateTime EventDate = new DateTime(2000 + Year, Month, Day, Hour, Minute, 0);

                        if (EventType == AMICalendarEvent.AMICalendarEventType.DST_ON)
                        {
                            DSTDate.ToDate = EventDate;
                        }
                        else if (EventType == AMICalendarEvent.AMICalendarEventType.DST_OFF)
                        {
                            DSTDate.FromDate = EventDate;
                        }
                    }

                    m_DSTDates.Add(DSTDate);
                }
            }
        }

        #endregion

        #region Member Variables

        private byte m_byDSTHour;
        private byte m_byDSTMinute;
        private byte m_byDSTOffset;

        private List<CDSTDatePair> m_DSTDates;

        #endregion
    }
}
