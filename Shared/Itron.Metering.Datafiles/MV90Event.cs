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
using Itron.Metering.Datafiles.Properties;

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// This class represents an MV-90 event
    /// </summary>
    public class MV90Event
    {

        #region Constants

        private const string UNKNOWN_EVENT_CODE = "74";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf 1.00.00        Created
        public MV90Event()
        {
            m_strEventCode = UNKNOWN_EVENT_CODE;
            m_dtEventTime = new DateTime();
            m_strExtraData = "";
            m_dicEventDescriptions = new MV90EventDictionary();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/08 jrf 1.00.00        Created
        public MV90Event(string strEventCode, DateTime dtEventTime, string strExtraData)
        {
            m_strEventCode = strEventCode;
            m_dtEventTime = dtEventTime;
            m_strExtraData = strExtraData;
            m_dicEventDescriptions = new MV90EventDictionary();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets/sets a string that represents the MV-90 event code.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	1.00.00	   	   Created
        public string Code
        {
            get
            {
                return m_strEventCode;
            }

            set
            {
                m_strEventCode = value;
            }
        }

        /// <summary>
        /// Gets/sets a string that represents the MV-90 event specific data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	1.00.00	   	   Created
        public string ExtraData
        {
            get
            {
                return m_strExtraData;
            }

            set
            {
                m_strExtraData = value;
            }
        }

        /// <summary>
        /// Gets a string that represents the MV-90 event's description.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	1.00.00	   	   Created
        public string Description
        {
            get
            {
                String strEventDescription;
                if (m_dicEventDescriptions.ContainsKey(m_strEventCode))
                {
                    strEventDescription = m_dicEventDescriptions[m_strEventCode];
                }
                else
                {
                    strEventDescription = "Unknown Event: " + m_strEventCode;
                }

                return strEventDescription;
            }

        }

        /// <summary>
        /// Gets/sets a string that represents the MV-90 event's description.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	1.00.00	   	   Created
        public DateTime Time
        {
            get
            {
                return m_dtEventTime;
            }

            set
            {
                m_dtEventTime = value;
            }
        }

        #endregion

        #region Members

        private string m_strEventCode;
        private DateTime m_dtEventTime;
        private string m_strExtraData;
        private MV90EventDictionary m_dicEventDescriptions;

        #endregion
    }

    /// <summary>
    /// This class represents dictionary of common MV-90 events
    /// </summary>
    [Serializable]
    public class MV90EventDictionary : Dictionary<string, string>
    {

        #region Constants

        private const string BATT_FAIL_CODE = "01";
        private const string RAM_ERR_DETECTED_CODE = "06";
        private const string DATA_RAM_ERR_DETECTED_CODE = "08";
        private const string OPERATING_PARAM_CHANGED = "11";
        private const string CONFIGURATION_CHANGED_CODE = "12";
        private const string RECORDER_CLOCK_SET_NEW_TIME_CODE = "16";
        private const string RECORDER_CLOCK_MALFUNCTION_CODE = "17";
        private const string FALL_DST_CHANGE_CODE = "18";
        private const string SPRING_DST_CHANGE_CODE = "19";
        private const string LP_DATA_CORRUPT_CODE = "40";
        private const string INTELLIGENT_REG_COMM_FAILURE_CODE = "41";
        private const string BILLING_RESET_CODE = "45";
        private const string FREEZE_METER_READINGS_CODE = "47";
        private const string TAMPER_INDICATION_CODE = "50";
        private const string START_TEST_MODE_CODE = "51";
        private const string END_TEST_MODE_CODE = "52";
        private const string TEST_MODE_DATA_CODE = "59";
        private const string UNKNOWN_EVENT_CODE = "74";
        private const string CUST_1_ALARM_CODE = "75";
        private const string CUST_2_ALARM_CODE = "76";
        private const string AUTO_TIME_SET_CODE = "85";
        private const string ALARM_ACTIVATED = "89";
        private const string SCHED_CALL_IN = "95";
        private const string WATCHDOG_TIMEOUT = "0C";
        private const string POWER_DOWN_CODE = "0D";
        private const string POWER_UP_CODE = "0E";
        private const string DEM_THRESH_CONDITION_CLEARED_CODE = "0I";
        private const string UNIT_ACCESSED_CODE = "1A";
        private const string UNIT_ACCESSED_REMOTE_CODE = "1B";
        private const string UNIT_ACCESSED_READER_CODE = "1C";
        private const string UNIT_ACCESSED_INVALID_PW_CODE = "1E";
        private const string METER_OVER_THRESH_CODE = "2C";
        private const string ZERO_INT_VAL_CODE = "2D";
        private const string DST_CHANGE_NEW_TIME_CODE = "4B";
        private const string CONFIGURATION_ERROR_CODE = "5I";
        private const string VOLTAGE_CUT_CODE = "5U";
        private const string DEM_THRESH_CAUTION_CODE = "5W";
        private const string TIME_DATE_CHANGED_CODE = "6B";
        private const string STATUS_INPUT_CLOSING_CODE = "6D";
        private const string STATUS_INPUT_OPENING_CODE = "6E";
        private const string DEMAND_RESET_CODE = "6S";
        private const string INPUT_HIGH_CODE = "6T";
        private const string INPUT_LOW_CODE = "6U";
        private const string RATE_CHANGE_CODE = "6V";
        private const string INTELLIGENT_REG_COMM_RESTORED_CODE = "6Z";
        private const string TOU_SCHEDULE_ERROR_CODE = "7G";
        private const string REV_RUNNNING_CEASED_CODE = "7H";
        private const string DIAG_1_ACTIVE_CODE = "7I";
        private const string DIAG_2_ACTIVE_CODE = "7J";
        private const string DIAG_3_ACTIVE_CODE = "7K";
        private const string DIAG_4_ACTIVE_CODE = "7L";
        private const string DIAG_5_ACTIVE_CODE = "7M";
        private const string DIAG_1_INACTIVE_CODE = "7N";
        private const string DIAG_2_INACTIVE_CODE = "7O";
        private const string DIAG_3_INACTIVE_CODE = "7P";
        private const string DIAG_4_INACTIVE_CODE = "7Q";
        private const string DIAG_5_INACTIVE_CODE = "7R";
        private const string VQ_LOG_NEAR_FULL_CODE = "7W";
        private const string END_OF_CALENDAR_CODE = "8K";
        private const string DIAG_6_ACTIVE_CODE = "8S";
        private const string DIAG_6_INACTIVE_CODE = "8T";
        private const string INTERNAL_OP_BOARD_EVENT_CODE = "8U";
        private const string EEPROM_FAILURE_CODE = "9D";
        private const string PHASE_VOLT_LESS_80_CODE = "B4";
        private const string REGISTER_FULLSCALE_EXCEEDED_CODE = "B5";
        private const string DIAG_FAILURE_CODE = "BB";
        private const string PULSE_OVERFLOW_CODE = "BD";
        private const string CUST_1_ALARM_OFF_CODE = "C8";
        private const string EVENT_LOG_RESET_CODE = "C9";
        private const string LOW_BATTERY_CODE = "CA";
        private const string REVERSE_RUNNING_DETECTED_CODE = "CB";
        private const string VOLT_IMBALANCE_CODE = "D5";
        private const string PHASE_FAILURE_CODE = "D6";
        private const string PHASE_DROPOUT_CODE = "DB";
        private const string PHASE_RESTORATION_CODE = "DF";
        private const string VOLT_SAG_CODE = "E6";
        private const string VOLT_SWELL_CODE = "E7";
        private const string SEASON_CHANGE_CODE = "EF";
        private const string SELF_READ_CODE = "F8";      

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Constructs a dictionary of common MV-90 events.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf 1.00.00        Created
        public MV90EventDictionary()
        {
            //Add Events
            Add(BATT_FAIL_CODE, Resources.BATT_FAIL);
            Add(RAM_ERR_DETECTED_CODE, Resources.RAM_ERR);
            Add(DATA_RAM_ERR_DETECTED_CODE, Resources.DATA_RAM_ERR);
            Add(OPERATING_PARAM_CHANGED, Resources.OP_PARAM_CHANGED);
            Add(CONFIGURATION_CHANGED_CODE, Resources.CONFIG_CHANGED);
            Add(RECORDER_CLOCK_SET_NEW_TIME_CODE, Resources.RECORDER_CLOCK_SET_NEW);
            Add(RECORDER_CLOCK_MALFUNCTION_CODE, Resources.RECORDER_CLOCK_MALFUNCTION);
            Add(FALL_DST_CHANGE_CODE, Resources.FALL_DST_CHANGE);
            Add(SPRING_DST_CHANGE_CODE, Resources.SPRING_DST_CHANGE);
            Add(LP_DATA_CORRUPT_CODE, Resources.LP_DATA_CORRUPT);
            Add(INTELLIGENT_REG_COMM_FAILURE_CODE, Resources.INTELLIGENT_REG_COMM_FAIL);
            Add(BILLING_RESET_CODE, Resources.BILLING_RESET);
            Add(FREEZE_METER_READINGS_CODE, Resources.FREEZE_METER_READINGS);
            Add(TAMPER_INDICATION_CODE, Resources.TAMPER_INDICATION);
            Add(START_TEST_MODE_CODE, Resources.START_TEST_MODE);
            Add(END_TEST_MODE_CODE, Resources.END_TEST_MODE);
            Add(TEST_MODE_DATA_CODE, Resources.TEST_MODE_DATA);
            Add(UNKNOWN_EVENT_CODE, Resources.UNRECOGNIZED_RECORDER_EVENT);
            Add(CUST_1_ALARM_CODE, Resources.CUST_1_ALARM);
            Add(CUST_2_ALARM_CODE, Resources.CUST_2_ALARM);
            Add(AUTO_TIME_SET_CODE, Resources.AUTO_TIMESET);
            Add(ALARM_ACTIVATED, Resources.ALARM_ACTIVATED);
            Add(SCHED_CALL_IN, Resources.SCHED_CALLIN);
            Add(WATCHDOG_TIMEOUT, Resources.WATCHDOG_TIMEOUT);
            Add(POWER_DOWN_CODE, Resources.POWER_DOWN);
            Add(POWER_UP_CODE, Resources.POWER_UP);
            Add(DEM_THRESH_CONDITION_CLEARED_CODE, Resources.DEM_THRESH_COND_CLEARED);
            Add(UNIT_ACCESSED_CODE, Resources.UNIT_ACCESSED);
            Add(UNIT_ACCESSED_REMOTE_CODE, Resources.UNIT_ACCESSED_REMOTE);
            Add(UNIT_ACCESSED_READER_CODE, Resources.UNIT_ACCESSED_READER);
            Add(UNIT_ACCESSED_INVALID_PW_CODE, Resources.UNIT_ACCESSED_INVALID_PW);
            Add(METER_OVER_THRESH_CODE, Resources.METER_OVER_THRESH);
            Add(ZERO_INT_VAL_CODE, Resources.ZERO_INT_VAL);
            Add(DST_CHANGE_NEW_TIME_CODE, Resources.DST_CHANGE_NEW);
            Add(CONFIGURATION_ERROR_CODE, Resources.CONFIG_ERROR);
            Add(VOLTAGE_CUT_CODE, Resources.VOLT_CUT);
            Add(DEM_THRESH_CAUTION_CODE, Resources.DEM_THRESH_CAUTION);
            Add(TIME_DATE_CHANGED_CODE, Resources.TIME_DATE_CHANGED);
            Add(STATUS_INPUT_CLOSING_CODE, Resources.STATUS_INPUT_CLOSING);
            Add(STATUS_INPUT_OPENING_CODE, Resources.STATUS_INPUT_OPENING);
            Add(DEMAND_RESET_CODE, Resources.DEM_RESET);
            Add(INPUT_HIGH_CODE, Resources.INPUT_HIGH);
            Add(INPUT_LOW_CODE, Resources.INPUT_LOW);
            Add(RATE_CHANGE_CODE, Resources.RATE_CHANGE);
            Add(INTELLIGENT_REG_COMM_RESTORED_CODE, Resources.INTELLIGENT_REG_COMM_RESTORED);
            Add(TOU_SCHEDULE_ERROR_CODE, Resources.TOU_SCHED_ERR);
            Add(REV_RUNNNING_CEASED_CODE, Resources.REV_RUNNING_CEASED);
            Add(DIAG_1_ACTIVE_CODE, Resources.DIAG_1_ACTIVE);
            Add(DIAG_2_ACTIVE_CODE, Resources.DIAG_2_ACTIVE);
            Add(DIAG_3_ACTIVE_CODE, Resources.DIAG_3_ACTIVE);
            Add(DIAG_4_ACTIVE_CODE, Resources.DIAG_4_ACTIVE);
            Add(DIAG_5_ACTIVE_CODE, Resources.DIAG_5_ACTIVE);
            Add(DIAG_1_INACTIVE_CODE, Resources.DIAG_1_INACTIVE);
            Add(DIAG_2_INACTIVE_CODE, Resources.DIAG_2_INACTIVE);
            Add(DIAG_3_INACTIVE_CODE, Resources.DIAG_3_INACTIVE);
            Add(DIAG_4_INACTIVE_CODE, Resources.DIAG_4_INACTIVE);
            Add(DIAG_5_INACTIVE_CODE, Resources.DIAG_5_INACTIVE);
            Add(VQ_LOG_NEAR_FULL_CODE, Resources.VQ_LOG_NEAR_FULL);
            Add(END_OF_CALENDAR_CODE, Resources.END_OF_CAL_WARN);
            Add(DIAG_6_ACTIVE_CODE, Resources.DIAG_6_ACTIVE);
            Add(DIAG_6_INACTIVE_CODE, Resources.DIAG_6_INACTIVE);
            Add(INTERNAL_OP_BOARD_EVENT_CODE, Resources.INTERNAL_OP_BOARD_EVENT);
            Add(EEPROM_FAILURE_CODE, Resources.EEPROM_FAIL);
            Add(PHASE_VOLT_LESS_80_CODE, Resources.PHASE_VOLT_LESS_80);
            Add(REGISTER_FULLSCALE_EXCEEDED_CODE, Resources.REG_FULLSCALE_EXCEEDED_EVENT);
            Add(DIAG_FAILURE_CODE, Resources.DIAG_FAIL);
            Add(PULSE_OVERFLOW_CODE, Resources.PULSE_OVERFLOW);
            Add(CUST_1_ALARM_OFF_CODE, Resources.CUST_1_ALARM_OFF);
            Add(EVENT_LOG_RESET_CODE, Resources.EVENT_LOG_RESET);
            Add(LOW_BATTERY_CODE, Resources.LOW_BATT);
            Add(REVERSE_RUNNING_DETECTED_CODE, Resources.REV_RUNNING_DETECTED);
            Add(VOLT_IMBALANCE_CODE, Resources.VOLT_IMBALANCE);
            Add(PHASE_FAILURE_CODE, Resources.PHASE_FAIL);
            Add(PHASE_DROPOUT_CODE, Resources.PHASE_DROPOUT);
            Add(PHASE_RESTORATION_CODE, Resources.PHASE_RESTORATION);
            Add(VOLT_SAG_CODE, Resources.VOLT_SAG);
            Add(VOLT_SWELL_CODE, Resources.VOLT_SWELL);
            Add(SEASON_CHANGE_CODE, Resources.SEASON_CHANGE);
            Add(SELF_READ_CODE, Resources.SELF_READ);  
        }

        #endregion

    }



}