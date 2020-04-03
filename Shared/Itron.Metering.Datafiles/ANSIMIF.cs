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
//                              Copyright © 2008 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Itron.Metering.CustomSchedule;
using Itron.Metering.TOU;
using Itron.Metering.Utilities;
using VirtDevLib;

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// This class represents a MIF created from an ANSI device.
    /// </summary>
    public class ANSIMIF: MIF
    {
        #region Constants

        private const int BILLING_SCHEDULE_ITEM_COUNT = 13000;
        private const int BILLING_SCHEDULE_ITEMS = 13001;
        private const int TOU_SEASON_1_DAY_TYPE_MAP = 28101;
        private const int SUNDAY_MASK = 0x03;
        private const int MONDAY_MASK = 0x0C;
        private const int TUESDAY_MASK = 0x30;
        private const int WEDNESDAY_MASK = 0xC0;
        private const int THURSDAY_MASK = 0x300;
        private const int FRIDAY_MASK = 0xC00;
        private const int SATURDAY_MASK = 0xC3000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="strFileName">The full path for the MIF.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 jrf 9.50           Created.
        //
        public ANSIMIF(string strFileName)
            : base(strFileName)
        {
            m_blnCustomScheduleUsed = new CachedBool();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the TOU schedule from the MIF.
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/30/08	jrf	9.50           Created
        //
        public override CTOUSchedule TOUSchedule
        {
            get
            {
                if (null == m_TOUSchedule)
                {
                    ReverseEngineerTOUSchedule();
                    RetrieveANSITypicalWeek();
                }
                            
                return m_TOUSchedule;
            }
        }

        /// <summary>
        /// Returns an bool that indicates if a custom schedule is used.
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/08/08	jrf	9.50.05        Created
        //
        public bool CustomScheduleUsed
        {
            get
            {
                object objValue = null;

                // Check to see if we have already retrieved the value.
                if (m_blnCustomScheduleUsed.Cached == false)
                {
                    if (RetrieveItem(BILLING_SCHEDULE_ITEM_COUNT, ref objValue))
                    {
                        short sCount = (short)Convert.ChangeType(objValue, typeof(short), CultureInfo.InvariantCulture);

                        if (0 < sCount)
                        {
                            m_blnCustomScheduleUsed.Value = true;
                        }
                        else
                        {
                            m_blnCustomScheduleUsed.Value = false;
                        }
                    }
                    else
                    {
                        // An error occurred
                        m_blnCustomScheduleUsed.Value = false;
                    }
                }

                return m_blnCustomScheduleUsed.Value;
            }
        }

        /// <summary>
        /// This property gets the custom schedule from the MIF.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/10/08 jrf 9.50           Created
        //
        public CCustomSchedule CustomSchedule
        {
            get
            {
                if (null == m_CustomSchedule)
                {
                    ReverseEngineerCustomSchedule();
                }

                return m_CustomSchedule;
            }
        } 

        #endregion

        #region Private Methods

        /// <summary>
        /// This method retrieves the typical week used by the TOU schedule.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/01/08 jrf 9.50           Created.
        //
        private void RetrieveANSITypicalWeek()
        {
            short sDisplayProgress = 0;
            short sResponse = DEVICE_SERVER_SUCCESS;
            object objValue = null;

            //Get the typical week
            sResponse = VirtualDevice.GetValue(TOU_SEASON_1_DAY_TYPE_MAP, ref objValue, sDisplayProgress);

            if (DEVICE_SERVER_SUCCESS == sResponse && null != objValue)
            {
                //extract the daytype for each day of the typical week.
                int iTypicalWeek = (int)objValue;
                int iSunday = SUNDAY_MASK & iTypicalWeek;
                int iMonday = (MONDAY_MASK & iTypicalWeek) >> 2;
                int iTuesday = (TUESDAY_MASK & iTypicalWeek) >> 4;
                int iWednesday = (WEDNESDAY_MASK & iTypicalWeek) >> 6;
                int iThursday = (THURSDAY_MASK & iTypicalWeek) >> 8;
                int iFriday = (FRIDAY_MASK & iTypicalWeek) >> 10;
                int iSaturday = (SATURDAY_MASK & iTypicalWeek) >> 12;

                m_TOUSchedule.TypicalWeek[(int)eTypicalDay.SUNDAY] = m_TOUSchedule.NormalDays[iSunday];
                m_TOUSchedule.TypicalWeek[(int)eTypicalDay.MONDAY] = m_TOUSchedule.NormalDays[iMonday];
                m_TOUSchedule.TypicalWeek[(int)eTypicalDay.TUESDAY] = m_TOUSchedule.NormalDays[iTuesday];
                m_TOUSchedule.TypicalWeek[(int)eTypicalDay.WEDNESDAY] = m_TOUSchedule.NormalDays[iWednesday];
                m_TOUSchedule.TypicalWeek[(int)eTypicalDay.THURSDAY] = m_TOUSchedule.NormalDays[iThursday];
                m_TOUSchedule.TypicalWeek[(int)eTypicalDay.FRIDAY] = m_TOUSchedule.NormalDays[iFriday];
                m_TOUSchedule.TypicalWeek[(int)eTypicalDay.SATURDAY] = m_TOUSchedule.NormalDays[iSaturday];
            }
        }

        /// <summary>
        /// This method reverse engineers the active custom schedule from the given
        /// MIF file.
        /// </summary>
        /// <returns>The reverse engineered custom schedule.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/08/08 jrf 9.50           Created.
        //
        private void ReverseEngineerCustomSchedule()
        {
            object objValue = null;
            short sResponse = DEVICE_SERVER_SUCCESS;
            short sDisplayProgress = 0;
            DateTime[] adtBillingDates;
            DateCollection colBillingDates = new DateCollection();

            if (false == m_bHasHHFBeenRead)
            {
                sResponse = (short)OpenHHF();
            }

            if (DEVICE_SERVER_SUCCESS == sResponse)
            {
                //Get the array of billing dates
                sResponse = VirtualDevice.GetValue(BILLING_SCHEDULE_ITEMS, ref objValue, sDisplayProgress);
            }

            if (DEVICE_SERVER_SUCCESS == sResponse && null != objValue)
            {
                adtBillingDates = (DateTime[])Convert.ChangeType(objValue, typeof(DateTime[]), CultureInfo.InvariantCulture);

                //Populate a date collection
                foreach (DateTime dtDate in adtBillingDates)
                {
                    colBillingDates.Add(dtDate);
                }

                //Add dates to the new schedule
                m_CustomSchedule = new CCustomSchedule();

                m_CustomSchedule.AddDates(colBillingDates);
            }
        }

        #endregion

        #region Members

        private CachedBool m_blnCustomScheduleUsed;
        private CCustomSchedule m_CustomSchedule;

        #endregion
    }
}
