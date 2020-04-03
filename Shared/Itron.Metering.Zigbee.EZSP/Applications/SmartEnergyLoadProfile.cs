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
using System.Linq;
using System.Text;

namespace Itron.Metering.Zigbee
{
    #region Definitions

    /// <summary>
    /// Load Profile Status
    /// </summary>
    public enum SmartEnergyLoadProfileStatus : byte
    {
        /// <summary>The request was successful</summary>
        Success = 0x00,
        /// <summary>The requested channel is not defined</summary>
        UndefinedIntervalChannelRequested = 0x01,
        /// <summary>The requested channel is not supported</summary>
        IntervalChannelNotSupported = 0x02,
        /// <summary>The requested end time is not valid</summary>
        InvalidEndTime = 0x03,
        /// <summary>More periods were requested than could be returned by the meter.</summary>
        MorePeriodsRequestedThanCanReturn = 0x04,
        /// <summary>No intervals are available for the requested time</summary>
        NoIntervalsForRequestedTime = 0x05,
    }

    /// <summary>
    /// Load Profile Period Lengths
    /// </summary>
    public enum SmartEnergyLoadProfilePeriod : byte
    {
        /// <summary>Daily</summary>
        Daily = 0,
        /// <summary>60 minutes</summary>
        SixtyMinutes = 1,
        /// <summary>30 minutes</summary>
        ThirtyMinutes = 2,
        /// <summary>15 minutes</summary>
        FifteenMinutes = 3,
        /// <summary>10 minutes</summary>
        TenMinutes = 4,
        /// <summary>7.5 minutes</summary>
        SevenMinutesThirtySeconds = 5,
        /// <summary>5 minutes</summary>
        FiveMinutes = 6,
        /// <summary>2.5 minutes</summary>
        TwoMinutesThirtySeconds = 7,
    }

    #endregion

    /// <summary>
    /// Smart Energy Load Profile data
    /// </summary>
    public class SmartEnergyLoadProfile
    {
        #region Constants

        private static readonly DateTime UTC_REFERENCE_TIME = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/11 RCG 2.52.28        Created
        
        public SmartEnergyLoadProfile()
        {
            m_EndTime = UTC_REFERENCE_TIME;
            m_Status = SmartEnergyLoadProfileStatus.NoIntervalsForRequestedTime;
            m_PeriodDuration = SmartEnergyLoadProfilePeriod.Daily;
            m_NumberOfPeriods = 0;
            m_Intervals = new List<SmartEnergyLoadProfileInterval>();
        }

        /// <summary>
        /// Gets a TimeSpan representing the duration of a Load Profile period
        /// </summary>
        /// <param name="periodDuration">The duration of the period</param>
        /// <returns>The TimeSpan representing the duration of the period</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/11 RCG 2.52.28        Created
        
        public static TimeSpan GetPeriodDuration(SmartEnergyLoadProfilePeriod periodDuration)
        {
            TimeSpan Duration = new TimeSpan();

            switch(periodDuration)
            {
                case SmartEnergyLoadProfilePeriod.Daily:
                {
                    Duration = new TimeSpan(24, 0, 0);
                    break;
                }
                case SmartEnergyLoadProfilePeriod.SixtyMinutes:
                {
                    Duration = new TimeSpan(1, 0, 0);
                    break;
                }
                case SmartEnergyLoadProfilePeriod.ThirtyMinutes:
                {
                    Duration = new TimeSpan(0, 30, 0);
                    break;
                }
                case SmartEnergyLoadProfilePeriod.FifteenMinutes:
                {
                    Duration = new TimeSpan(0, 15, 0);
                    break;
                }
                case SmartEnergyLoadProfilePeriod.TenMinutes:
                {
                    Duration = new TimeSpan(0, 10, 0);
                    break;
                }
                case SmartEnergyLoadProfilePeriod.SevenMinutesThirtySeconds:
                {
                    Duration = new TimeSpan(0, 7, 30);
                    break;
                }
                case SmartEnergyLoadProfilePeriod.FiveMinutes:
                {
                    Duration = new TimeSpan(0, 5, 0);
                    break;
                }
                case SmartEnergyLoadProfilePeriod.TwoMinutesThirtySeconds:
                {
                    Duration = new TimeSpan(0, 2, 30);
                    break;
                }
            }

            return Duration;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the end time of the intervals
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/11 RCG 2.52.28        Created

        public DateTime EndTime
        {
            get
            {
                return m_EndTime;
            }
            internal set
            {
                m_EndTime = value;
            }
        }

        /// <summary>
        /// Gets the status of the request
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/11 RCG 2.52.28        Created
        
        public SmartEnergyLoadProfileStatus Status
        {
            get
            {
                return m_Status;
            }
            internal set
            {
                m_Status = value;
            }
        }

        /// <summary>
        /// Gets the duration of the load profile period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/11 RCG 2.52.28        Created
        
        public SmartEnergyLoadProfilePeriod Duration
        {
            get
            {
                return m_PeriodDuration;
            }
            internal set
            {
                m_PeriodDuration = value;
            }
        }

        /// <summary>
        /// Gets the number of intervals
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/11 RCG 2.52.28        Created
        
        public byte NumberOfPeriods
        {
            get
            {
                return m_NumberOfPeriods;
            }
            internal set
            {
                m_NumberOfPeriods = value;
            }
        }

        /// <summary>
        /// Gets the Load Profile Intervals
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/11 RCG 2.52.28        Created
        
        public List<SmartEnergyLoadProfileInterval> Intervals
        {
            get
            {
                return m_Intervals;
            }
            internal set
            {
                m_Intervals = value;
            }
        }

        #endregion

        #region Member Variables

        private DateTime m_EndTime;
        private SmartEnergyLoadProfileStatus m_Status;
        private SmartEnergyLoadProfilePeriod m_PeriodDuration;
        private byte m_NumberOfPeriods;
        private List<SmartEnergyLoadProfileInterval> m_Intervals;

        #endregion
    }

    /// <summary>
    /// Load Profile interval retrieved using the Smart Energy Simple Metering Cluster
    /// </summary>
    public class SmartEnergyLoadProfileInterval
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="intervalTime">The time the interval ended</param>
        /// <param name="intervalValue">The load profile value</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/11 RCG 2.52.28        Created
        
        public SmartEnergyLoadProfileInterval(DateTime intervalTime, uint intervalValue)
        {
            m_Time = intervalTime;
            m_Value = intervalValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the end time of the interval
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/11 RCG 2.52.28        Created

        public DateTime IntervalEndTime
        {
            get
            {
                return m_Time;
            }
        }

        /// <summary>
        /// Gets the value of the interval
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/18/11 RCG 2.52.28        Created

        public uint IntervalValue
        {
            get
            {
                return m_Value;
            }
        }

        #endregion

        #region Member Variables

        private DateTime m_Time;
        private uint m_Value;

        #endregion
    }
}
