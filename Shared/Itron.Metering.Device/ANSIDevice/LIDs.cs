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
//                              Copyright © 2006-2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Resources;
using System.Globalization;

namespace Itron.Metering.Device
{
    /// <summary>
    /// This class describes the LID supported by the ANSI meter.  It is the
    /// base class for all LIDs.  The Sentinel and Centron meters overrides this
    /// class to add support for LIDs which are different between meters.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------
    //  05/23/06 mrj 7.30.00 N/A	Created
    //  
    public class DefinedLIDs
    {
        #region Constants

        #region "Public LIDs"
        //This is the list of public LID's.  These LIDs are the same between all
        //meters (Sentinel and Centrons).

        #region Coefficient LIDS

        /// <summary>
        /// Gets the LID for the CT ratio
        /// </summary>
        public LID CT_RATIO = new LID((uint)BaseLIDs.COEFF_CONFIG | (uint)Coefficients_Config.CT_MULT);
        /// <summary>
        /// Gets the LID for the VT Ratio
        /// </summary>
        public LID VT_RATIO = new LID((uint)BaseLIDs.COEFF_CONFIG | (uint)Coefficients_Config.VT_MULT);
        /// <summary>
        /// Gets the LID for the Register Multiplier
        /// </summary>
        public LID REGISTER_MULTIPLIER = new LID((uint)BaseLIDs.COEFF_CONFIG | (uint)Coefficients_Config.REG_MULT);
        /// <summary>
        /// Gets the LID for the Meter Multiplier
        /// </summary>
        public LID METER_MULTIPLIER = new LID((uint)BaseLIDs.COEFF_DATA | (uint)Coefficient_Data.METER_MULT);
        /// <summary>
        /// Gets the LID for the Transformer Ratio
        /// </summary>
        public LID TRANSFORMER_RATIO = new LID((uint)BaseLIDs.COEFF_DATA | (uint)Coefficient_Data.TRANSFORMER_MULT);

        #endregion

        #region Calendar Data LIDs
        /// <summary>
        /// LID to get the clock running flag
        /// </summary>
        public LID CLOCK_RUNNING = new LID((uint)BaseLIDs.CALENDAR_DATA | (uint)Calendar_Data.CLD_HAS_CLOCK);
        /// <summary>
        /// LID to get the number of TOU Rates being used
        /// </summary>
        public LID NUM_TOU_RATES = new LID((uint)BaseLIDs.CALENDAR_DATA | (uint)Calendar_Data.CLD_NUM_TOU_RATES);
        /// <summary>
        /// LID to get the current date
        /// </summary>
        public LID CURRENT_DATE = new LID((uint)BaseLIDs.CALENDAR_DATA | (uint)Calendar_Data.CLD_DATE);
        /// <summary>
        /// LID to get the current time
        /// </summary>
        public LID CURRENT_TIME = new LID((uint)BaseLIDs.CALENDAR_DATA | (uint)Calendar_Data.CLD_TIME);
        /// <summary>
        /// LID to get the minutes on battery.
        /// </summary>
        public LID MINUTES_ON_BATTERY = new LID((uint)BaseLIDs.CALENDAR_DATA | (uint)Calendar_Data.CLD_MIN_ON_BATTERY);
        /// <summary>
        /// LID to get the Days on battery.
        /// </summary>
        public LID DAYS_ON_BATTERY = new LID((uint)BaseLIDs.CALENDAR_DATA | (uint)Calendar_Data.CLD_DAYS_ON_BATTERY);
        /// <summary>
        /// LID to get the Day of the Week.
        /// </summary>
        public LID DAY_OF_WEEK = new LID((uint)BaseLIDs.CALENDAR_DATA | (uint)Calendar_Data.CLD_DAY_OF_WEEK);
        /// <summary>
        /// LID to get if DST is Configured
        /// </summary>
        public LID DST_CONFIGURED = new LID((uint)BaseLIDs.CALENDAR_DATA | (uint)Calendar_Data.CLD_DST_CONFIGURED);
        /// <summary>
        /// LID to get if the meter is in DST
        /// </summary>
        public LID METER_IN_DST = new LID((uint)BaseLIDs.CALENDAR_DATA | (uint)Calendar_Data.CLD_IN_DST);
        /// <summary>
        /// LID to get the TOU Expiration Date.
        /// </summary>
        public LID TOU_EXPIRATION_DATE = new LID((uint)BaseLIDs.CALENDAR_DATA | (uint)Calendar_Data.CLD_EXPIRE_DATE);

        #endregion

        #region Constants Config LIDs

        /// <summary>
        /// LID to get the program ID
        /// </summary>
        public LID PROGRAM_ID = new LID((uint)BaseLIDs.CONSTANT_CONFIG | (uint)Constant_Config.PROGRAM_ID);
        /// <summary>
        /// LID to get the User Data 1
        /// </summary>
        public LID USER_DATA_1 = new LID((uint)BaseLIDs.CONSTANT_CONFIG | (uint)Constant_Config.USER_DATA_1);
        /// <summary>
        /// LID to get the User Data 2
        /// </summary>
        public LID USER_DATA_2 = new LID((uint)BaseLIDs.CONSTANT_CONFIG | (uint)Constant_Config.USER_DATA_2);
        /// <summary>
        /// LID to get the User Data 3
        /// </summary>
        public LID USER_DATA_3 = new LID((uint)BaseLIDs.CONSTANT_CONFIG | (uint)Constant_Config.USER_DATA_3);

        #endregion

        #region Constants Data LIDs

        /// <summary>
        /// LID to get the meter ID
        /// </summary>
        public LID METER_ID = new LID((uint)BaseLIDs.CONSTANTS_DATA | (uint)Constant_Data.METER_ID_PART1);

        /// <summary>
        /// LID to get the meter ID 2
        /// </summary>
        public LID METER_ID_2 = new LID((uint)BaseLIDs.CONSTANTS_DATA | (uint)Constant_Data.METER_ID_PART2);

        #endregion

        #region Demand Config LIDs
        /// <summary>
        /// LID to get the number of demand sub intervals
        /// </summary>
        public LID DEMAND_NUM_SUBINTERVALS = new LID((uint)BaseLIDs.DEMAND_CONFIG | (uint)DemandConfig_Data.CONF_NBR_SUB);
        /// <summary>
        /// LID to get the number of demand test sub intervals
        /// </summary>
        public LID DEMAND_NUM_TEST_SUBINTERVAL = new LID((uint)BaseLIDs.DEMAND_CONFIG | (uint)DemandConfig_Data.CONF_TST_NBR_SUB);
        /// <summary>
        /// LID to get the register fullscale
        /// </summary>
        public LID DEMAND_REGISTER_FULLSCALE = new LID((uint)BaseLIDs.DEMAND_CONFIG | (uint)DemandConfig_Data.CONF_REG_FS);
        /// <summary>
        /// LID to get the Cold Load Pick-up time
        /// </summary>
        public LID CLPU_OUTAGE_TIME = new LID((uint)BaseLIDs.DEMAND_CONFIG | (uint)DemandConfig_Data.CONF_CLPU);

        #endregion

        #region Demand Data LIDs
        /// <summary>
        /// not programmed
        /// </summary>
        public LID DEMAND_NOT_PROGRAMMED = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                         (uint)WhichOneEnergyDemand.NOT_PROGRAMMED);
        /// <summary>
        /// max W d
        /// </summary>
        public LID DEMAND_MAX_W_DEL = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.WH_DELIVERED);
        /// <summary>
        /// max W r
        /// </summary>
        public LID DEMAND_MAX_W_REC = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.WH_RECEIVED);
        /// <summary>
        /// max W Net
        /// </summary>
        public LID DEMAND_MAX_W_NET = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.WH_NET);
        /// <summary>
        /// max W Uni-Directional
        /// </summary>
        public LID DEMAND_MAX_W_UNI = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.WH_UNI);
        /// <summary>
        /// max VA d
        /// </summary>
        public LID DEMAND_MAX_VA_DEL = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VAH_DEL_ARITH);
        /// <summary>
        /// max VA r
        /// </summary>
        public LID DEMAND_MAX_VA_REC = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VAH_REC_ARITH);
        /// <summary>
        /// max VA d Arithmetic
        /// </summary>
        public LID DEMAND_MAX_VA_DEL_ARITH = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VAH_DEL_ARITH);
        /// <summary>
        /// max VA r Arithmetic
        /// </summary>
        public LID DEMAND_MAX_VA_REC_ARITH = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VAH_REC_ARITH);
        /// <summary>
        /// max VA d Vectorial
        /// </summary>
        public LID DEMAND_MAX_VA_DEL_VECT = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VAH_DEL_VECT);
        /// <summary>
        /// max VA r Vectorial
        /// </summary>
        public LID DEMAND_MAX_VA_REC_VECT = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VAH_REC_VECT);
        /// <summary>
        /// max VA Lagging
        /// </summary>
        public LID DEMAND_MAX_VA_LAG = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VAH_LAG);
        /// <summary>
        /// max Var d
        /// </summary>
        public LID DEMAND_MAX_VAR_DEL = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VARH_DEL);
        /// <summary>
        /// max Var r
        /// </summary>
        public LID DEMAND_MAX_VAR_REC = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VARH_REC);
        /// <summary>
        /// max Var Net
        /// </summary>
        public LID DEMAND_MAX_VAR_NET = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VARH_NET);
        /// <summary>
        /// max Var Net d
        /// </summary>
        public LID DEMAND_MAX_VAR_NET_DEL = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VARH_NET_DEL);
        /// <summary>
        /// max Var Net r
        /// </summary>
        public LID DEMAND_MAX_VAR_NET_REC = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VARH_NET_REC);
        /// <summary>
        /// max Var Q1
        /// </summary>
        public LID DEMAND_MAX_VAR_Q1 = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VARH_Q1);
        /// <summary>
        /// max Var Q2
        /// </summary>
        public LID DEMAND_MAX_VAR_Q2 = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VARH_Q2);
        /// <summary>
        /// max Var Q3
        /// </summary>
        public LID DEMAND_MAX_VAR_Q3 = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VARH_Q3);
        /// <summary>
        /// max Var Q4
        /// </summary>
        public LID DEMAND_MAX_VAR_Q4 = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VARH_Q4);
        /// <summary>
        /// max A(a)
        /// </summary>
        public LID DEMAND_MAX_A_PHA = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.AH_PHA);
        /// <summary>
        /// max A(b)
        /// </summary>
        public LID DEMAND_MAX_A_PHB = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.AH_PHB);
        /// <summary>
        /// max A(c)
        /// </summary>
        public LID DEMAND_MAX_A_PHC = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.AH_PHC);
        /// <summary>
        /// max NA
        /// </summary>
        public LID DEMAND_MAX_A_NEUTRAL = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.AH_NEUTRAL);
        /// <summary>
        /// max A^2 
        /// </summary>
        public LID DEMAND_MAX_I2_AGG = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.I2H_AGG);
        /// <summary>
        /// max V(a)
        /// </summary>
        public LID DEMAND_MAX_V_PHA = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VH_PHA);
        /// <summary>
        /// max V(b)
        /// </summary>
        public LID DEMAND_MAX_V_PHB = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VH_PHB);
        /// <summary>
        /// max V(c)
        /// </summary>
        public LID DEMAND_MAX_V_PHC = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VH_PHC);
        /// <summary>
        /// max V avg
        /// </summary>
        public LID DEMAND_MAX_V_AVG = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.VH_AVG);
        /// <summary>
        /// max V^2 agg
        /// </summary>
        public LID DEMAND_MAX_V2_AGG = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.V2H_AGG);
        /// <summary>
        /// min PF Arithmetic
        /// </summary>
        public LID DEMAND_MIN_PF_INTERVAL_ARITH = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MIN_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.PF_INTERVAL_ARITH);
        /// <summary>
        /// min PF Vectorial
        /// </summary>
        public LID DEMAND_MIN_PF_INTERVAL_VECT = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MIN_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.PF_INTERVAL_VECT);
        /// <summary>
        /// max Q d
        /// </summary>
        public LID DEMAND_MAX_Q_DEL = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.QH_DEL);
        /// <summary>
        /// max Q r
        /// </summary>
        public LID DEMAND_MAX_Q_REC = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.QH_REC);

        /// <summary>
        /// max Pulse Input 1
        /// </summary>
        public LID DEMAND_MAX_PULSE_INPUT_1 = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.PULSE_INPUT_1);

        /// <summary>
        /// max Pulse Input 2
        /// </summary>
        public LID DEMAND_MAX_PULSE_INPUT_2 = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                        (uint)WhichOneEnergyDemand.PULSE_INPUT_2);

        #endregion

        #region Demand Data Misc LIDs
        /// <summary>
        /// LID to get the Time Remaining in the Demand Sub Interval
        /// </summary>
        public LID TIME_REMAINING_DEMAND_SUBINTERVAL = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MISC_DEMAND | (uint)MiscDemand.TOO_TIME_REMAINING);
        /// <summary>
        /// LID to get the last demand reset date
        /// </summary>
        public LID LAST_DEMAND_RESET_DATE = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MISC_DEMAND |
                                (uint)MiscDemand.TO_LAST_DEMAND_RES);
        /// <summary>
        /// LID to get the test mode sub interval length
        /// </summary>
        public LID DEMAND_SUBINT_LENGTH = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MISC_DEMAND | (uint)MiscDemand.SUB_INT_LEN);
        /// <summary>
        /// LID to get the test mode sub interval length
        /// </summary>
        public LID DEMAND_TEST_SUBINT_LENGTH = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MISC_DEMAND | (uint)MiscDemand.TEST_SUB_INT_LEN);
        /// <summary>
        /// LID to get the number of demand resets
        /// </summary>
        public LID NUMBER_DEMAND_RESETS = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MISC_DEMAND | (uint)MiscDemand.COUNT_DEMAND_RES);
        /// <summary>
        /// LID to get the number of days since the last demand reset
        /// </summary>
        public LID DAYS_SINCE_DR = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MISC_DEMAND | (uint)MiscDemand.DAYS_SINCE_DR);
        /// <summary>
        /// Number of Demands in meter
        /// </summary>
        public LID NUMBER_DEMANDS = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MISC_DEMAND | (uint)MiscDemand.NUM_DEMANDS);
        /// <summary>
        /// Previous max demand quantity 1
        /// </summary>
        public LID PREVIOUS_MAX_DEMAND_QTY_1 = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MISC_DEMAND | (uint)MiscDemand.LAST_MAX_DMD1);
        /// <summary>
        /// Previous max demand quantity 2
        /// </summary>
        public LID PREVIOUS_MAX_DEMAND_QTY_2 = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MISC_DEMAND | (uint)MiscDemand.LAST_MAX_DMD2);
        /// <summary>
        /// Previous max demand quantity 3
        /// </summary>
        public LID PREVIOUS_MAX_DEMAND_QTY_3 = new LID((uint)BaseLIDs.DEMAND_DATA | (uint)Demand_Data.MISC_DEMAND | (uint)MiscDemand.LAST_MAX_DMD3);

        #endregion

        #region Display Data LIDs

        /// <summary>
        /// The number of seconds the display will show a display item.
        /// </summary>
        public LID DISPLAY_TIME = new LID((uint)BaseLIDs.DISPLAY_DATA | (uint)Display_Data.DISPLAY_TIME);

        #endregion Display Data LIDs

        #region Energy Data LIDs
        /// <summary>
        /// Number of Energies in meter
        /// </summary>
        public LID NUMBER_ENERGIES = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)Energy_Data.ENERGY_MISC_DATA | (uint)Misc_Energy.NUMBER_OF_ENERGIES);
        /// <summary>
        /// not programmed
        /// </summary>
        public LID ENERGY_NOT_PROGRAMMED = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.NOT_PROGRAMMED);
        /// <summary>
        /// Wh d
        /// </summary>
        public LID ENERGY_WH_DEL = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.WH_DELIVERED);
        /// <summary>
        /// Wh r
        /// </summary>
        public LID ENERGY_WH_REC = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.WH_RECEIVED);
        /// <summary>
        /// Wh Net
        /// </summary>
        public LID ENERGY_WH_NET = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.WH_NET);
        /// <summary>
        /// Wh Uni-Directional
        /// </summary>
        public LID ENERGY_WH_UNI = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.WH_UNI);
        /// <summary>
        /// VAh d
        /// </summary>
        public LID ENERGY_VAH_DEL = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_DEL_ARITH);
        /// <summary>
        /// VAh r
        /// </summary>
        public LID ENERGY_VAH_REC = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_REC_ARITH);
        /// <summary>
        /// VAh d Arithmetic
        /// </summary>
        public LID ENERGY_VAH_DEL_ARITH = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_DEL_ARITH);
        /// <summary>
        /// VAh r Arithmetic
        /// </summary>
        public LID ENERGY_VAH_REC_ARITH = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_REC_ARITH);
        /// <summary>
        /// VAh d Vectorial
        /// </summary>
        public LID ENERGY_VAH_DEL_VECT = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_DEL_VECT);
        /// <summary>
        /// VAh r Vectorial
        /// </summary>
        public LID ENERGY_VAH_REC_VECT = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_REC_VECT);
        /// <summary>
        /// VAh lagging
        /// </summary>
        public LID ENERGY_VAH_LAG = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_LAG);
        /// <summary>
        /// Varh d
        /// </summary>
        public LID ENERGY_VARH_DEL = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_DEL);
        /// <summary>
        /// Varh r
        /// </summary>
        public LID ENERGY_VARH_REC = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_REC);
        /// <summary>
        /// Varh Net
        /// </summary>
        public LID ENERGY_VARH_NET = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_NET);
        /// <summary>
        /// Varh Net d
        /// </summary>
        public LID ENERGY_VARH_NET_DEL = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_NET_DEL);
        /// <summary>
        /// Varh Net r
        /// </summary>
        public LID ENERGY_VARH_NET_REC = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_NET_REC);
        /// <summary>
        /// Varh Q1
        /// </summary>
        public LID ENERGY_VARH_Q1 = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_Q1);
        /// <summary>
        /// Varh Q2
        /// </summary>
        public LID ENERGY_VARH_Q2 = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_Q2);
        /// <summary>
        /// Varh Q3
        /// </summary>
        public LID ENERGY_VARH_Q3 = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_Q3);
        /// <summary>
        /// Varh Q4
        /// </summary>
        public LID ENERGY_VARH_Q4 = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_Q4);
        /// <summary>
        /// Ah(a)
        /// </summary>
        public LID ENERGY_AH_PHA = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.AH_PHA);
        /// <summary>
        /// Ah(b)
        /// </summary>
        public LID ENERGY_AH_PHB = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.AH_PHB);
        /// <summary>
        /// Ah(c)
        /// </summary>
        public LID ENERGY_AH_PHC = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.AH_PHC);
        /// <summary>
        /// Ah Neutral
        /// </summary>
        public LID ENERGY_AH_NEUTRAL = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.AH_NEUTRAL);
        /// <summary>
        /// A^2h
        /// </summary>
        public LID ENERGY_I2H_AGG = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.I2H_AGG);
        /// <summary>
        /// Vh(a)
        /// </summary>
        public LID ENERGY_VH_PHA = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VH_PHA);
        /// <summary>
        /// Vh(b)
        /// </summary>
        public LID ENERGY_VH_PHB = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VH_PHB);
        /// <summary>
        /// Vh(c)
        /// </summary>
        public LID ENERGY_VH_PHC = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VH_PHC);
        /// <summary>
        /// Vh Average
        /// </summary>
        public LID ENERGY_VH_AVG = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VH_AVG);
        /// <summary>
        /// V^2h
        /// </summary>
        public LID ENERGY_V2H_AGG = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.V2H_AGG);
        /// <summary>
        /// Qh d
        /// </summary>
        public LID ENERGY_QH_DEL = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.QH_DEL);
        /// <summary>
        /// Qh r
        /// </summary>
        public LID ENERGY_QH_REC = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.QH_REC);
        /// <summary>
        /// Pulse Input 1
        /// </summary>
        public LID ENERGY_PULSE_INPUT_1 = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.PULSE_INPUT_1);
        /// <summary>
        /// Pulse Input 2
        /// </summary>
        public LID ENERGY_PULSE_INPUT_2 = new LID((uint)BaseLIDs.ENERGY_DATA | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.PULSE_INPUT_2);

        #endregion

        #region Load Profile LIDs
        /// <summary>
        /// LID to get the load profile interval length
        /// </summary>
        public LID LP_INTERVAL_LENGTH = new LID((uint)BaseLIDs.LOAD_PROFILE_CONFIG | (uint)LP_Config.LP_INT_LEN);
        /// <summary>
        /// LID to get the channel 1 quantity's LID
        /// </summary>
        public LID LP_CHAN_1_QUANTITY = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG | (uint)DefinedLIDs.LP_Config.LP_CHAN_1_QUAN);
        /// <summary>
        /// LID to get the channel 2 quantity's LID
        /// </summary>
        public LID LP_CHAN_2_QUANTITY = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG | (uint)DefinedLIDs.LP_Config.LP_CHAN_2_QUAN);
        /// <summary>
        /// LID to get the channel 3 quantity's LID
        /// </summary>
        public LID LP_CHAN_3_QUANTITY = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG | (uint)DefinedLIDs.LP_Config.LP_CHAN_3_QUAN);
        /// <summary>
        /// LID to get the channel 4 quantity's LID
        /// </summary>
        public LID LP_CHAN_4_QUANTITY = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG | (uint)DefinedLIDs.LP_Config.LP_CHAN_4_QUAN);
        /// <summary>
        /// LID to get the channel 5 quantity's LID
        /// </summary>
        public LID LP_CHAN_5_QUANTITY = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG | (uint)DefinedLIDs.LP_Config.LP_CHAN_5_QUAN);
        /// <summary>
        /// LID to get the channel 6 quantity's LID
        /// </summary>
        public LID LP_CHAN_6_QUANTITY = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG | (uint)DefinedLIDs.LP_Config.LP_CHAN_6_QUAN);
        /// <summary>
        /// LID to get the channel 7 quantity's LID
        /// </summary>
        public LID LP_CHAN_7_QUANTITY = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG | (uint)DefinedLIDs.LP_Config.LP_CHAN_7_QUAN);
        /// <summary>
        /// LID to get the channel 8 quantity's LID
        /// </summary>
        public LID LP_CHAN_8_QUANTITY = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG | (uint)DefinedLIDs.LP_Config.LP_CHAN_8_QUAN);
        /// <summary>
        /// LID to get the channel 1 pulse weight (Configuration Item)
        /// </summary>
        public LID LP_CHAN_1_INT_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG |
                                (uint)DefinedLIDs.LP_Config.LP_CHAN_1_PW);
        /// <summary>
        /// LID to get the channel 2 pulse weight (Configuration Item)
        /// </summary>
        public LID LP_CHAN_2_INT_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG |
                                (uint)DefinedLIDs.LP_Config.LP_CHAN_2_PW);
        /// <summary>
        /// LID to get the channel 3 pulse weight (Configuration Item)
        /// </summary>
        public LID LP_CHAN_3_INT_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG |
                                (uint)DefinedLIDs.LP_Config.LP_CHAN_3_PW);
        /// <summary>
        /// LID to get the channel 4 pulse weight (Configuration Item)
        /// </summary>
        public LID LP_CHAN_4_INT_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG |
                                (uint)DefinedLIDs.LP_Config.LP_CHAN_4_PW);
        /// <summary>
        /// LID to get the channel 5 pulse weight (Configuration Item)
        /// </summary>
        public LID LP_CHAN_5_INT_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG |
                                (uint)DefinedLIDs.LP_Config.LP_CHAN_5_PW);
        /// <summary>
        /// LID to get the channel 6 pulse weight (Configuration Item)
        /// </summary>
        public LID LP_CHAN_6_INT_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG |
                                (uint)DefinedLIDs.LP_Config.LP_CHAN_6_PW);
        /// <summary>
        /// LID to get the channel 7 pulse weight (Configuration Item)
        /// </summary>
        public LID LP_CHAN_7_INT_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG |
                                (uint)DefinedLIDs.LP_Config.LP_CHAN_7_PW);
        /// <summary>
        /// LID to get the channel 8 pulse weight (Configuration Item)
        /// </summary>
        public LID LP_CHAN_8_INT_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG |
                                (uint)DefinedLIDs.LP_Config.LP_CHAN_8_PW);

        /// <summary>
        /// LID to get the channel 1 pulse weight (Real Value - Not supported pre-SATURN)
        /// </summary>
        public LID LP_CHAN_1_REAL_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_DATA |
                                (uint)DefinedLIDs.LP_Data.LP_MISC_CH1_REAL_PW);
        /// <summary>
        /// LID to get the channel 2 pulse weight (Real Value - Not supported pre-SATURN)
        /// </summary>
        public LID LP_CHAN_2_REAL_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_DATA |
                                (uint)DefinedLIDs.LP_Data.LP_MISC_CH2_REAL_PW);
        /// <summary>
        /// LID to get the channel 3 pulse weight (Real Value - Not supported pre-SATURN)
        /// </summary>
        public LID LP_CHAN_3_REAL_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_DATA |
                                (uint)DefinedLIDs.LP_Data.LP_MISC_CH3_REAL_PW);
        /// <summary>
        /// LID to get the channel 4 pulse weight (Real Value - Not supported pre-SATURN)
        /// </summary>
        public LID LP_CHAN_4_REAL_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_DATA |
                                (uint)DefinedLIDs.LP_Data.LP_MISC_CH4_REAL_PW);
        /// <summary>
        /// LID to get the channel 5 pulse weight (Real Value - Not supported pre-SATURN)
        /// </summary>
        public LID LP_CHAN_5_REAL_PULSE_W = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_DATA |
                                (uint)DefinedLIDs.LP_Data.LP_MISC_CH5_REAL_PW);
        /// <summary>
        /// LID to get the channel 6 pulse weight (Real Value - Not supported pre-SATURN)
        /// </summary>
        public LID LP_CHAN_6_REAL_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_DATA |
                                (uint)DefinedLIDs.LP_Data.LP_MISC_CH6_REAL_PW);
        /// <summary>
        /// LID to get the channel 7 pulse weight (Real Value - Not supported pre-SATURN)
        /// </summary>
        public LID LP_CHAN_7_REAL_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_DATA |
                                (uint)DefinedLIDs.LP_Data.LP_MISC_CH7_REAL_PW);
        /// <summary>
        /// LID to get the channel 8 pulse weight (Real Value - Not supported pre-SATURN)
        /// </summary>
        public LID LP_CHAN_8_REAL_PULSE_WT = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_DATA |
                                (uint)DefinedLIDs.LP_Data.LP_MISC_CH8_REAL_PW);
        /// <summary>
        /// LID to get whether or not Load Profile is running
        /// </summary>
        public LID LP_RUNNING = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_DATA |
                                (uint)DefinedLIDs.LP_Data.LP_MISC_RUNNING);

        /// <summary>
        /// LID to get whether or not Extended Load Profile is running
        /// </summary>
        public LID EXT_LP_RUNNING = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_DATA |(uint)DefinedLIDs.LP_Which_Profile.LP_EXT_SET_1 |
                                (uint)DefinedLIDs.LP_Data.LP_MISC_RUNNING);

        /// <summary>
        /// LID to get whether or not Instrumentation Profile is running
        /// </summary>
        public LID IP_RUNNING = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_DATA | (uint)DefinedLIDs.LP_Which_Profile.LP_EXT_SET_2 |
                                (uint)DefinedLIDs.LP_Data.LP_MISC_RUNNING);
        /// <summary>
        /// LID to get the number of Load Profile channels in the meter
        /// </summary>
        public LID LP_NUM_CHANNELS = new LID((uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG |
                                (uint)DefinedLIDs.LP_Config.LP_NUM_CHAN);

        #endregion

        #region Metrology Config LIDs
        /// <summary>
        /// LID to get the Normal Pulse Weight
        /// </summary>
        public LID COEFF_KH = new LID((uint)BaseLIDs.METROLOGY_CONFIG | (uint)Metrology_Config.CPC_CONF_PUL_WGT_NORM);
        /// <summary>
        /// LID to get the test Pulse Weight
        /// </summary>
        public LID COEFF_KT = new LID((uint)BaseLIDs.METROLOGY_CONFIG | (uint)Metrology_Config.CPC_CONF_PUL_WGT_TEST);
        /// <summary>
        /// LID to get the Pulse Weight for the Alt values
        /// </summary>
        public LID COEFF_ALT_KH = new LID((uint)BaseLIDs.METROLOGY_CONFIG | (uint)Metrology_Config.CPC_CONF_PUL_WGT_ALT);
        /// <summary>
        /// LID to get the test Pulse Weight for the Alt values
        /// </summary>
        public LID COEFF_ALT_KT = new LID((uint)BaseLIDs.METROLOGY_CONFIG | (uint)Metrology_Config.CPC_CONF_PUL_WGT_TSTALT);
        /// <summary>
        /// LID to get the VA calculation method
        /// </summary>
        public LID VA_CALC_METHOD = new LID((uint)BaseLIDs.METROLOGY_CONFIG | (uint)Metrology_Config.CPC_CONF_PWR_CALC_METH);
        /// <summary>
        /// LID to get the Pulse Weight as a real value
        /// </summary>
        public LID REAL_COEFF_KH = new LID((uint)BaseLIDs.METROLOGY_CONFIG | (uint)Metrology_Config.CPC_CONF_REAL_PW_NORM);
        /// <summary>
        /// LID to get the test Pulse Weight as a real value
        /// </summary>
        public LID REAL_COEFF_KT = new LID((uint)BaseLIDs.METROLOGY_CONFIG | (uint)Metrology_Config.CPC_CONF_REAL_PW_TEST);
        /// <summary>
        /// LID to get the Alt pulse weight as a real value 
        /// </summary>
        public LID REAL_COEFF_ALT_KH = new LID((uint)BaseLIDs.METROLOGY_CONFIG | (uint)Metrology_Config.CPC_CONF_REAL_PW_ALT);
        /// <summary>
        ///  LID to get the test Alt Pulse Weight as a real value
        /// </summary>
        public LID REAL_COEFF_ALT_KT = new LID((uint)BaseLIDs.METROLOGY_CONFIG | (uint)Metrology_Config.CPC_CONF_REAL_PW_TST_ALT);
        #endregion

        #region Metrology Data LIDs
        /// <summary>
        /// LID to get the instantaneous secondary volts RMS, phase A
        /// </summary>
        public LID SERVICE_VOLTAGE = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_VRMS);
#pragma warning disable 1591 // Ignores the XML comment warnings
        public LID LINE_FREQUENCY = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_FREQ);

        public LID INST_PF = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_PF | (uint)MetrologyDataPhase.CPC_PHASE_AGGREG);

        public LID INST_W = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_W | (uint)MetrologyDataPhase.CPC_PHASE_AGGREG);

        public LID INST_W_PHASE_A = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_W | (uint)MetrologyDataPhase.CPC_PHASE_A);
        
        public LID INST_W_PHASE_B = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_W | (uint)MetrologyDataPhase.CPC_PHASE_B);
        
        public LID INST_W_PHASE_C = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_W | (uint)MetrologyDataPhase.CPC_PHASE_C);

        public LID INST_VAR = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_VAR | (uint)MetrologyDataPhase.CPC_PHASE_AGGREG);

        public LID INST_VAR_PHASE_A = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_VAR | (uint)MetrologyDataPhase.CPC_PHASE_A);

        public LID INST_VAR_PHASE_B = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_VAR | (uint)MetrologyDataPhase.CPC_PHASE_B);

        public LID INST_VAR_PHASE_C = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_VAR | (uint)MetrologyDataPhase.CPC_PHASE_C);

        public LID INST_VA_ARITH = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_VA |
                    (uint)MetrologyDataPhase.CPC_PHASE_AGGREG | (uint)MetrologyDataCalc.CPC_CALC_ARITH);

        public LID INST_VA_VECT = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_VA |
                    (uint)MetrologyDataPhase.CPC_PHASE_AGGREG | (uint)MetrologyDataCalc.CPC_CALC_VECT);

        public LID INST_VA_LAG = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_VA |
                    (uint)MetrologyDataPhase.CPC_PHASE_AGGREG | (uint)MetrologyDataCalc.CPC_CALC_LAG);

        public LID INST_CURRENT_PHASE_A = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_IRMS | (uint)MetrologyDataPhase.CPC_PHASE_A);

        public LID INST_CURRENT_PHASE_B = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_IRMS | (uint)MetrologyDataPhase.CPC_PHASE_B);

        public LID INST_CURRENT_PHASE_C = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_IRMS | (uint)MetrologyDataPhase.CPC_PHASE_C);

        public LID INST_VOLTAGE_PHASE_A = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_VRMS | (uint)MetrologyDataPhase.CPC_PHASE_A);

        public LID INST_VOLTAGE_PHASE_B = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_VRMS | (uint)MetrologyDataPhase.CPC_PHASE_B);

        public LID INST_VOLTAGE_PHASE_C = new LID((uint)BaseLIDs.METROLOGY_DATA | (uint)MetrologyDataType.CPC_INST_TYPE |
                    (uint)MetrologyDataFormat.CPC_FORMAT_SEC | (uint)MetrologyDataIns.CPC_INST_VRMS | (uint)MetrologyDataPhase.CPC_PHASE_C);

#pragma warning restore 1591
        #endregion

        #region Misc Data LIDs
        /// <summary>
        /// LID to get the firmware revision
        /// </summary>
        public LID FIRMWARE_REVISION = new LID((uint)BaseLIDs.MISC_DATA | (uint)Misc_Data.MISC_FW_VERS_REV);
        /// <summary>
        /// LID to get the Firmware Build
        /// </summary>
        public LID FIRMWARE_BUILD = new LID((uint)BaseLIDs.MISC_DATA | (uint)Misc_Data.MISC_FW_BUILD);
        /// <summary>
        /// LID to get the Atmega firmware version and revision
        /// </summary>
        public LID ATMEL_FIRMWARE_VER_REV = new LID((uint)BaseLIDs.MISC_DATA | (uint)Misc_Data.MISC_ATM_VERS_REV);
        /// <summary>
        /// LID to get the software revision
        /// </summary>
        public LID SOFTWARE_REVISION = new LID((uint)BaseLIDs.MISC_DATA | (uint)Misc_Data.MISC_SW_REVISION);
        /// <summary>
        /// LID to get the last outage time
        /// </summary>
        public LID OUTAGE_TIME = new LID((uint)BaseLIDs.MISC_DATA | (uint)Misc_Data.MISC_OUTAGE_TIME);
        /// <summary>
        /// LID to get the last time meter was configured
        /// </summary>
        public LID CONFIG_TIME = new LID((uint)BaseLIDs.MISC_DATA | (uint)Misc_Data.MISC_LAST_CONFIG_TIME);
        /// <summary>
        /// LID to get the last time meter was interrogated
        /// </summary>
        public LID INTERROGATION_TIME = new LID((uint)BaseLIDs.MISC_DATA | (uint)Misc_Data.MISC_INTERROGATION);
        /// <summary>
        /// LID to get the status of the sealed Canadian flag
        /// </summary>
        public LID SEALED_CANADIAN = new LID((uint)BaseLIDs.MISC_DATA | (uint)Misc_Data.MISC_SEALED_CANADIAN);
        /// <summary>
        /// LID to get the status of the Canadian flag
        /// </summary>
        public LID CANADIAN = new LID((uint)BaseLIDs.MISC_DATA | (uint)Misc_Data.MISC_CANADIAN_METER);
        /// <summary>
        /// Date and time of the last test mode
        /// </summary>
        public LID LAST_TEST_TIME = new LID((uint)BaseLIDs.MISC_DATA | (uint)Misc_Data.MISC_TEST_TIME);
        /// <summary>Version of the Firmware Loader</summary>
        public LID FIRMWARE_LOADER_VERSION = new LID((uint)BaseLIDs.MISC_DATA | (uint)Misc_Data.MISC_LDR_VERSION);
        /// <summary>Revision of the Firmware Loader</summary>
        public LID FIRMWARE_LOADER_REVISION = new LID((uint)BaseLIDs.MISC_DATA | (uint)Misc_Data.MISC_LDR_REVISION);
        /// <summary>Version.Revsion of the Firmware Loader</summary>
        public LID FIRMWARE_LOADER_VER_REV = new LID((uint)BaseLIDs.MISC_DATA | (uint)Misc_Data.MISC_LDR_VERS_REV);
        /// <summary>Segment Test</summary>
        public LID SEGMENT_TEST = new LID((uint)BaseLIDs.MISC_DATA | (uint)Misc_Data.MISC_SEGMENT_TEST);

        #endregion

        #region Mode Control LIDs
        /// <summary>
        /// LID to get the Time Remaining in Test Mode
        /// </summary>
        public LID TIME_REMAINING_TEST_MODE = new LID((uint)BaseLIDs.MODE_CNTRL_DATA | (uint)Mode_Cntrl_Data.MM_TIMEOUT_REMAIN);
        /// <summary>
        /// LID to get the configured Display Mode Timeout
        /// </summary>
        public LID DISP_MODE_TIMEOUT = new LID((uint)BaseLIDs.MODE_CNTRL_CONFIG | (uint)Mode_Cntrl_Config.MM_TIMEOUT_CONFIG);
        /// <summary>
        /// Meter Display Mode
        /// </summary>
        public LID DISP_MODE = new LID((uint)BaseLIDs.MODE_CNTRL_DATA | (uint)Mode_Cntrl_Data.MM_METER_MODE);

        #endregion

        #region Option Board LIDs
        /// <summary>
        /// LID to get the option board ID
        /// </summary>		
        public LID OPT_BRD_ID = new LID((uint)BaseLIDs.OPTION_BRD_DATA | (uint)Opt_Brd_Data.OPT_BRD_ID);
        /// <summary>
        /// LID to get the vendor field 1, ERT ID for R300
        /// </summary>
        public LID DISP_VENDOR_FIELD_1 = new LID((uint)BaseLIDs.OPTION_BRD_DATA | (uint)Opt_Brd_Data.OPT_BRD_VEND_FLD_1);
        /// <summary>
        /// LID to get the vendor field 2, ERT ID for R300
        /// </summary>
        public LID DISP_VENDOR_FIELD_2 = new LID((uint)BaseLIDs.OPTION_BRD_DATA | (uint)Opt_Brd_Data.OPT_BRD_VEND_FLD_2);
        /// <summary>
        /// LID to get the vendor field 3, ERT ID for R300
        /// </summary>
        public LID DISP_VENDOR_FIELD_3 = new LID((uint)BaseLIDs.OPTION_BRD_DATA | (uint)Opt_Brd_Data.OPT_BRD_VEND_FLD_3);

        #endregion

        #region Self Read LIDs
        /// <summary>
        /// LID to get the last self read date
        /// </summary>
        public LID SR1_FREEZE_DATE = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)SlfRd_Data.SR_BUFF_1 | (uint)ReadTimeData.SR_OTHER_DATA | (uint)OtherSRData.SR_MISC | (uint)WhichSRMisc.MISC_SR_TIME);

        /// <summary>
        /// Wh d (SR)
        /// </summary>
        public LID ENERGY_SR_WH_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.WH_DELIVERED);
        /// <summary>
        /// Wh r (SR)
        /// </summary>
        public LID ENERGY_SR_WH_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.WH_RECEIVED);
        /// <summary>
        /// Wh Net (SR)
        /// </summary>
        public LID ENERGY_SR_WH_NET = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.WH_NET);
        /// <summary>
        /// Wh Uni-Directional (SR)
        /// </summary>
        public LID ENERGY_SR_WH_UNI = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.WH_UNI);
        /// <summary>
        /// VAh d (SR)
        /// </summary>
        public LID ENERGY_SR_VAH_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_DEL_ARITH);
        /// <summary>
        /// VAh r (SR)
        /// </summary>
        public LID ENERGY_SR_VAH_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_REC_ARITH);
        /// <summary>
        /// VAh d Arithmetic (SR)
        /// </summary>
        public LID ENERGY_SR_VAH_DEL_ARITH = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_DEL_ARITH);
        /// <summary>
        /// VAh r Arithmetic (SR)
        /// </summary>
        public LID ENERGY_SR_VAH_REC_ARITH = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_REC_ARITH);
        /// <summary>
        /// VAh d Vectorial (SR)
        /// </summary>
        public LID ENERGY_SR_VAH_DEL_VECT = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_DEL_VECT);
        /// <summary>
        /// VAh r Vectorial (SR)
        /// </summary>
        public LID ENERGY_SR_VAH_REC_VECT = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_REC_VECT);
        /// <summary>
        /// VAh lagging (SR)
        /// </summary>
        public LID ENERGY_SR_VAH_LAG = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_LAG);
        /// <summary>
        /// Varh d (SR)
        /// </summary>
        public LID ENERGY_SR_VARH_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_DEL);
        /// <summary>
        /// Varh r (SR)
        /// </summary>
        public LID ENERGY_SR_VARH_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_REC);
        /// <summary>
        /// Varh Net (SR)
        /// </summary>
        public LID ENERGY_SR_VARH_NET = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_NET);
        /// <summary>
        /// Varh Net d (SR)
        /// </summary>
        public LID ENERGY_SR_VARH_NET_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_NET_DEL);
        /// <summary>
        /// Varh Net r (SR)
        /// </summary>
        public LID ENERGY_SR_VARH_NET_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_NET_REC);
        /// <summary>
        /// Varh Q1 (SR)
        /// </summary>
        public LID ENERGY_SR_VARH_Q1 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_Q1);
        /// <summary>
        /// Varh Q2 (SR)
        /// </summary>
        public LID ENERGY_SR_VARH_Q2 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_Q2);
        /// <summary>
        /// Varh Q3 (SR)
        /// </summary>
        public LID ENERGY_SR_VARH_Q3 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_Q3);
        /// <summary>
        /// Varh Q4 (SR)
        /// </summary>
        public LID ENERGY_SR_VARH_Q4 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_Q4);
        /// <summary>
        /// Ah(a) (SR)
        /// </summary>
        public LID ENERGY_SR_AH_PHA = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.AH_PHA);
        /// <summary>
        /// Ah(b) (SR)
        /// </summary>
        public LID ENERGY_SR_AH_PHB = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.AH_PHB);
        /// <summary>
        /// Ah(c) (SR)
        /// </summary>
        public LID ENERGY_SR_AH_PHC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.AH_PHC);
        /// <summary>
        /// Ah Neutral (SR)
        /// </summary>
        public LID ENERGY_SR_AH_NEUTRAL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.AH_NEUTRAL);
        /// <summary>
        /// A^2h (SR)
        /// </summary>
        public LID ENERGY_SR_I2H_AGG = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.I2H_AGG);
        /// <summary>
        /// Vh(a) (SR)
        /// </summary>
        public LID ENERGY_SR_VH_PHA = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VH_PHA);
        /// <summary>
        /// Vh(b) (SR)
        /// </summary>
        public LID ENERGY_SR_VH_PHB = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VH_PHB);
        /// <summary>
        /// Vh(c) (SR)
        /// </summary>
        public LID ENERGY_SR_VH_PHC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VH_PHC);
        /// <summary>
        /// Vh Average (SR)
        /// </summary>
        public LID ENERGY_SR_VH_AVG = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VH_AVG);
        /// <summary>
        /// V^2h (SR)
        /// </summary>
        public LID ENERGY_SR_V2H_AGG = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.V2H_AGG);
        /// <summary>
        /// Qh d (SR)
        /// </summary>
        public LID ENERGY_SR_QH_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.QH_DEL);
        /// <summary>
        /// Qh r (SR)
        /// </summary>
        public LID ENERGY_SR_QH_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.QH_REC);
        /// <summary>
        /// max W d (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_W_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.WH_DELIVERED);
        /// <summary>
        /// max W r (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_W_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.WH_RECEIVED);
        /// <summary>
        /// max W Net (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_W_NET = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.WH_NET);
        /// <summary>
        /// max W Uni-Directional (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_W_UNI = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.WH_UNI);
        /// <summary>
        /// max VA d (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VA_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VAH_DEL_ARITH);
        /// <summary>
        /// max VA r (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VA_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VAH_REC_ARITH);
        /// <summary>
        /// max VA d Arithmetic (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VA_DEL_ARITH = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VAH_DEL_ARITH);
        /// <summary>
        /// max VA r Arithmetic (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VA_REC_ARITH = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VAH_REC_ARITH);
        /// <summary>
        /// max VA d Vectorial (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VA_DEL_VECT = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VAH_DEL_VECT);
        /// <summary>
        /// max VA r Vectorial (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VA_REC_VECT = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VAH_REC_VECT);
        /// <summary>
        /// max VA Lagging (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VA_LAG = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VAH_LAG);
        /// <summary>
        /// max Var d (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VAR_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_DEL);
        /// <summary>
        /// max Var r (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VAR_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_REC);
        /// <summary>
        /// max Var Net (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VAR_NET = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_NET);
        /// <summary>
        /// max Var Net d (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VAR_NET_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_NET_DEL);
        /// <summary>
        /// max Var Net r (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VAR_NET_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_NET_REC);
        /// <summary>
        /// max Var Q1 (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VAR_Q1 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_Q1);
        /// <summary>
        /// max Var Q2 (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VAR_Q2 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_Q2);
        /// <summary>
        /// max Var Q3 (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VAR_Q3 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_Q3);
        /// <summary>
        /// max Var Q4 (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_VAR_Q4 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_Q4);
        /// <summary>
        /// max A(a) (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_A_PHA = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.AH_PHA);
        /// <summary>
        /// max A(b) (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_A_PHB = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.AH_PHB);
        /// <summary>
        /// max A(c) (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_A_PHC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.AH_PHC);
        /// <summary>
        /// max NA (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_A_NEUTRAL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.AH_NEUTRAL);
        /// <summary>
        /// max A^2 (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_I2_AGG = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.I2H_AGG);
        /// <summary>
        /// max V(a) (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_V_PHA = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VH_PHA);
        /// <summary>
        /// max V(b) (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_V_PHB = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VH_PHB);
        /// <summary>
        /// max V(c) (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_V_PHC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VH_PHC);
        /// <summary>
        /// max V avg (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_V_AVG = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VH_AVG);
        /// <summary>
        /// max V^2 agg (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_V2_AGG = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.V2H_AGG);
        /// <summary>
        /// min PF Arithmetic (SR)
        /// </summary>
        public LID DEMAND_MIN_SR_PF_INTERVAL_ARITH = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MIN_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.PF_INTERVAL_ARITH);
        /// <summary>
        /// min PF Vectorial (SR)
        /// </summary>
        public LID DEMAND_MIN_SR_PF_INTERVAL_VECT = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MIN_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.PF_INTERVAL_VECT);
        /// <summary>
        /// max Q d (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_Q_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MIN_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.QH_DEL);
        /// <summary>
        /// max Q r (SR)
        /// </summary>
        public LID DEMAND_MAX_SR_Q_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MIN_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.QH_REC);
        /// <summary>
        /// Wh d (DR)
        /// </summary>
        public LID ENERGY_DR_WH_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.WH_DELIVERED);
        /// <summary>
        /// Wh r (DR)
        /// </summary>
        public LID ENERGY_DR_WH_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.WH_RECEIVED);
        /// <summary>
        /// Wh Net (DR)
        /// </summary>
        public LID ENERGY_DR_WH_NET = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.WH_NET);
        /// <summary>
        /// Wh Uni-Directional (DR)
        /// </summary>
        public LID ENERGY_DR_WH_UNI = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.WH_UNI);
        /// <summary>
        /// VAh d (DR)
        /// </summary>
        public LID ENERGY_DR_VAH_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_DEL_ARITH);
        /// <summary>
        /// VoltHour (a) (DR)
        /// </summary>
        public LID ENERGY_DR_VH_PH_A = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                        (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VH_PHA);
        /// <summary>
        /// VAh r (DR)
        /// </summary>
        public LID ENERGY_DR_VAH_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VAH_REC_ARITH);
        /// <summary>
        /// Varh d (DR)
        /// </summary>
        public LID ENERGY_DR_VARH_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_DEL);
        /// <summary>
        /// Varh r (DR)
        /// </summary>
        public LID ENERGY_DR_VARH_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_REC);
        /// <summary>
        /// Varh Net (DR)
        /// </summary>
        public LID ENERGY_DR_VARH_NET = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_NET);
        /// <summary>
        /// Varh Net d (DR)
        /// </summary>
        public LID ENERGY_DR_VARH_NET_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_NET_DEL);
        /// <summary>
        /// Varh Net r (DR)
        /// </summary>
        public LID ENERGY_DR_VARH_NET_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_NET_REC);
        /// <summary>
        /// Varh Q1 (DR)
        /// </summary>
        public LID ENERGY_DR_VARH_Q1 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_Q1);
        /// <summary>
        /// Varh Q2 (DR)
        /// </summary>
        public LID ENERGY_DR_VARH_Q2 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_Q2);
        /// <summary>
        /// Varh Q3 (DR)
        /// </summary>
        public LID ENERGY_DR_VARH_Q3 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_Q3);
        /// <summary>
        /// Varh Q4 (DR)
        /// </summary>
        public LID ENERGY_DR_VARH_Q4 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_ENERGY | (uint)WhichEnergyFormat.SECONDARY_DATA | (uint)WhichOneEnergyDemand.VARH_Q4);

        /// <summary>
        /// max W d (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_W_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.WH_DELIVERED);
        /// <summary>
        /// max W r (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_W_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.WH_RECEIVED);
        /// <summary>
        /// max W Net (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_W_NET = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.WH_NET);
        /// <summary>
        /// max W Uni-Directional (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_W_UNI = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.WH_UNI);
        /// <summary>
        /// max Volts (a) (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_V_PH_A = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VH_PHA);
        /// <summary>
        /// max VA d (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_VA_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VAH_DEL_ARITH);
        /// <summary>
        /// max VA r (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_VA_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VAH_REC_ARITH);
        /// <summary>
        /// max Var d (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_VAR_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_DEL);
        /// <summary>
        /// max Var r (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_VAR_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_REC);
        /// <summary>
        /// max Var Net (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_VAR_NET = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_NET);
        /// <summary>
        /// max Var Net d (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_VAR_NET_DEL = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_NET_DEL);
        /// <summary>
        /// max Var Net r (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_VAR_NET_REC = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_NET_REC);
        /// <summary>
        /// max Var Q1 (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_VAR_Q1 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_Q1);
        /// <summary>
        /// max Var Q2 (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_VAR_Q2 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_Q2);
        /// <summary>
        /// max Var Q3 (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_VAR_Q3 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_Q3);
        /// <summary>
        /// max Var Q4 (DR)
        /// </summary>
        public LID DEMAND_MAX_DR_VAR_Q4 = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_BILLING_DATA | (uint)TOU_Data.TOTAL |
                                       (uint)TOU_Rate_Data.TOU_DEMAND | (uint)Demand_Data.MAX_DEMAND | (uint)WhichDemandFormat.SECONDARY_DATA |
                                       (uint)WhichOneEnergyDemand.VARH_Q4);

        /// <summary>
        /// Base LID for Time and Date of Last DR and SR
        /// </summary>
        public LID BASE_TIME_DATE_OF_PERIODIC_READ = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_OTHER_DATA | (uint)OtherSRData.SR_MISC
                                                    | (uint)WhichSRMisc.MISC_SR_TIME);
        /// <summary>
        /// Base LID to determine if there is valid data in last DR or SR
        /// </summary>
        public LID BASE_VALID_DATA_IN_PERIODIC_READ = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)ReadTimeData.SR_OTHER_DATA | (uint)OtherSRData.SR_MISC | (uint)WhichSRMisc.MISC_SR_VALID);
        /// <summary>
        /// Number of Valid Self Reads stored in the meter.
        /// </summary>
        public LID NUMBER_OF_VALID_SELF_READS = new LID((uint)BaseLIDs.SELF_READ_DATA | (uint)SlfRd_Data.SR_BUFF_DATA | (uint)SR_Buffer_Data.NUM_VALID_SELF_READS);

        #endregion

        #region SiteScan LIDs
        /// <summary>
        /// LID to get the phase a voltage
        /// </summary>
        public LID PH_A_VOLTAGE = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.PH_A_VOLTAGE);
        /// <summary>
        /// LID to get the phase a current
        /// </summary>
        public LID PH_A_CURRENT = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.PH_A_CURRENT);
        /// <summary>
        /// LID to get the phase a current angle
        /// </summary>
        public LID PH_A_CUR_ANGLE = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.PH_A_CUR_ANGLE);
        /// <summary>
        /// LID to get the phase b voltage
        /// </summary>
        public LID PH_B_VOLTAGE = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.PH_B_VOLTAGE);
        /// <summary>
        /// LID to get the phase b voltage angle
        /// </summary>
        public LID PH_B_VOLT_ANGLE = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.PH_B_VOLT_ANGLE);
        /// <summary>
        /// LID to get the phase a current
        /// </summary>
        public LID PH_B_CURRENT = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.PH_B_CURRENT);
        /// <summary>
        /// LID to get the phase b current angle
        /// </summary>
        public LID PH_B_CUR_ANGLE = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.PH_B_CUR_ANGLE);
        /// <summary>
        /// LID to get the phase b voltage
        /// </summary>
        public LID PH_C_VOLTAGE = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.PH_C_VOLTAGE);
        /// <summary>
        /// LID to get the phase c voltage angle
        /// </summary>
        public LID PH_C_VOLT_ANGLE = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.PH_C_VOLT_ANGLE);
        /// <summary>
        /// LID to get the phase c current
        /// </summary>
        public LID PH_C_CURRENT = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.PH_C_CURRENT);
        /// <summary>
        /// LID to get the phase c current angle
        /// </summary>
        public LID PH_C_CUR_ANGLE = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.PH_C_CUR_ANGLE);
        /// <summary>
        /// LID to get the serivce type
        /// </summary>
        public LID SITESCAN_SERVICE_TYPE = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.SS_SERVICE_TYPE);
        /// <summary>
        ///  LID to get the snapshot count
        /// </summary>
        public LID SS_SNAPSHOT_COUNT = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.SS_SNAPSHOT_COUNT);
        /// <summary>
        /// LID to get all the sitescan data at once
        /// </summary>
        public LID ALL_SITESCAN = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.SS_ALL_INSTANT);
        /// <summary>
        /// LID to get the displayed service type
        /// </summary>
        public LID SS_SERVICE_TYPE_DISP = new LID((uint)BaseLIDs.SITE_SCAN_DATA | (uint)SiteScan_Data.SS_SERV_TYPE_DISP);

        #endregion

        #region Voltage Monitor LIDs

        /// <summary>
        /// LID to get the RMS Below Threshold Count.
        /// </summary>
        public LID RMS_BELOW_THRESHOLD_COUNT = new LID((uint)DefinedLIDs.BaseLIDs.VOLTMON_DATA | (uint)DefinedLIDs.VoltMon_Data.RMS_BELOW_THRESH_COUNT);
        /// <summary>
        /// LID to get the RMS High Threshold Count.
        /// </summary>
        public LID RMS_HIGH_THRESHOLD_COUNT = new LID((uint)DefinedLIDs.BaseLIDs.VOLTMON_DATA | (uint)DefinedLIDs.VoltMon_Data.RMS_HIGH_THRESH_COUNT);
        /// <summary>
        /// LID to get the Vh Below Threshold Count.
        /// </summary>
        public LID VH_BELOW_THRESHOLD_COUNT = new LID((uint)DefinedLIDs.BaseLIDs.VOLTMON_DATA | (uint)DefinedLIDs.VoltMon_Data.VH_BELOW_THRESH_COUNT);
        /// <summary>
        /// LID to get the Vh High Threshold Count.
        /// </summary>
        public LID VH_HIGH_THRESHOLD_COUNT = new LID((uint)DefinedLIDs.BaseLIDs.VOLTMON_DATA | (uint)DefinedLIDs.VoltMon_Data.VH_HIGH_THRESH_COUNT);

        #endregion Voltage Monitor LIDs

        #region PQ LIDs
        /// <summary>
        /// LID to get the instantaneous THD phase A
        /// </summary>
        public LID INS_THD_V_PHASE_A = new LID((uint)BaseLIDs.PQ_DATA | (uint)VQ_Data.PQ_THD_V_A);
        /// <summary>
        /// LID to get the instantaneous THD phase B
        /// </summary>
        public LID INS_THD_V_PHASE_B = new LID((uint)BaseLIDs.PQ_DATA | (uint)VQ_Data.PQ_THD_V_B);
        /// <summary>
        /// LID to get the instantaneous THD phase C
        /// </summary>
        public LID INS_THD_V_PHASE_C = new LID((uint)BaseLIDs.PQ_DATA | (uint)VQ_Data.PQ_THD_V_C);
        /// <summary>
        /// LID to get the instantaneous TDD phase A
        /// </summary>
        public LID INS_TDD_I_PHASE_A = new LID((uint)BaseLIDs.PQ_DATA | (uint)VQ_Data.PQ_TDD_I_A);
        /// <summary>
        /// LID to get the instantaneous TDD phase B
        /// </summary>
        public LID INS_TDD_I_PHASE_B = new LID((uint)BaseLIDs.PQ_DATA | (uint)VQ_Data.PQ_TDD_I_B);
        /// <summary>
        /// LID to get the instantaneous TDD phase B
        /// </summary>
        public LID INS_TDD_I_PHASE_C = new LID((uint)BaseLIDs.PQ_DATA | (uint)VQ_Data.PQ_TDD_I_C);

        #endregion PQ LIDs

        #region State Mon LIDs
        /// <summary>
        /// The Number of Inversion tampers in the meter
        /// </summary>
        public LID NUMBER_INVERSION_TAMPERS = new LID((uint)BaseLIDs.STATEMON_DATA | (uint)StatMon_Data.STATMON_STD | (uint)StateMon_STD.INV_TAMPER_COUNT);
        /// <summary>
        /// Number of Removal tampers in the meter
        /// </summary>
        public LID NUMBER_REMOVAL_TAMPERS = new LID((uint)BaseLIDs.STATEMON_DATA | (uint)StatMon_Data.STATMON_STD | (uint)StateMon_STD.REMOV_TAMPER_COUNT);
        /// <summary>
        /// Optical port logon count
        /// </summary>
        public LID OPTICAL_PORT_LOGON_COUNT = new LID((uint)BaseLIDs.STATEMON_DATA | (uint)StatMon_Data.STATMON_STD | (uint)StateMon_STD.OPT_LOGON_COUNT);

        #endregion

        #region MeterKey LIDs
        /// <summary>
        /// The Number of Rates that the meter is capable of supporting.
        /// </summary>
        public LID METERKEY_NUMBER_TOU_RATES_SUPPORTED = new LID((uint)BaseLIDs.METER_KEY_DATA | (uint)MeterKey_Data.KEY_TOU);
        #endregion

        #region Super LIDs
        /// <summary>
        /// Will provide the LIDs of all programmed Energies
        /// </summary>
        public LID ALL_SEC_ENERGIES_TOTAL = new LID(0x140000BF);
        /// <summary>
        /// Will provide the LIDs of all programmed Demands
        /// </summary>
        public LID ALL_SEC_DEMANDS_TOTAL = new LID(0x1C03C0BE);

        #endregion

        #endregion

        #region "Meter Specific LIDs"
        //These lids are different between the meter types and must be defined in 
        //the inherited device specific LIDs classes
        /// <summary>
        /// LID to get the non-fatal errors
        /// </summary>
        public LID STATEMON_NON_FATAL_ERRORS;
        /// <summary>
        /// LID to get the second byte of non-fatal errors
        /// </summary>
        public LID STATEMON_NON_FATAL_ERRORS2;
        /// <summary>
        /// LID to get the fatal errors
        /// </summary>
        public LID STATEMON_FATAL_ERRORS;
        /// <summary>
        /// LID to get the Diag 1 count
        /// </summary>
        public LID SITESCAN_DIAG_1_COUNT;
        /// <summary>
        /// LID to get the Diag 2 count
        /// </summary>
        public LID SITESCAN_DIAG_2_COUNT;
        /// <summary>
        /// LID to get the Diag 3 count
        /// </summary>
        public LID SITESCAN_DIAG_3_COUNT;
        /// <summary>
        /// LID to get the Diag 4 count
        /// </summary>
        public LID SITESCAN_DIAG_4_COUNT;
        /// <summary>
        /// LID to get the Diag 5 count
        /// </summary>
        public LID SITESCAN_DIAG_5_COUNT;
        /// <summary>
        /// LID to get the Diag 5a count
        /// </summary>
        public LID SITESCAN_DIAG_5_COUNT_A;
        /// <summary>
        /// LID to get the Diag 5b count
        /// </summary>
        public LID SITESCAN_DIAG_5_COUNT_B;
        /// <summary>
        /// LID to get the Diag 5c count
        /// </summary>
        public LID SITESCAN_DIAG_5_COUNT_C;
        /// <summary>
        /// LID to get the Diag 6 count
        /// </summary>
        public LID SITESCAN_DIAG_6_COUNT;
        /// <summary>
        /// LID to get the active diagnostics 1-6
        /// </summary>
        public LID STATEMON_DIAG_ERRORS;
        /// <summary>
        /// LID to get the number of Power Outages
        /// </summary>
        public LID NUMBER_POWER_OUTAGES;
        /// <summary>
        /// LID to get the number of Times Programmed
        /// </summary>
        public LID NUMBER_TIMES_PROGRAMMED;
        // Base Data LIDS - Currently Centron Poly only
        /// <summary>
        /// LID to get the meter form factor
        /// </summary>
        public LID METER_FORM_FACTOR;
        /// <summary>
        /// LID to get the meter base type
        /// </summary>
        public LID METER_BASE_TYPE;
        /// <summary>
        /// LID to get the Hardware version 
        /// </summary>
        public LID METER_HARDWARE_VER_REV;
        /// <summary>
        /// LID to get the Calibration date
        /// </summary>
        public LID METER_CALIBRATION_DATE;
        /// <summary>
        ///  Billing Schedule 
        /// </summary>
        public LID BILLING_SCHEDULE_NAME;
        /// <summary>
        /// Percentage of VQ space currently used
        /// </summary>
        public LID VQ_PERCENT_LOG_FULL;
        /// <summary>
        /// Number of VQ Sag currently logged
        /// </summary>
        public LID VQ_SAG_COUNT;
        /// <summary>
        /// Number of VQ swell events currently logged
        /// </summary>
        public LID VQ_SWELL_COUNT;
        /// <summary>
        /// Number of VQ voltage imbalance events currently logged
        /// </summary>  
        public LID VQ_VOLTAGE_IMBALANCE_COUNT;
        /// <summary>
        /// Number of VQ current imbalance events currently logged
        /// </summary>
        public LID VQ_CURRENT_IMBALANCE_COUNT;
        /// <summary>
        /// Number of VQ Interruption events currently logged
        /// </summary>
        public LID VQ_INTERRUPTION_COUNT;
        /// <summary>
        /// Numerical indication of the IO board's capabilities.
        /// </summary>
        public LID IO_BOARD_CAPABILITES;

        #endregion
#pragma warning disable 1591 // Ignores the XML comment warnings

        /// <summary>
        /// Enumerations based on the LIDS.h file for the Centron Image. This enumeration is
        /// only used when building the LIDs that we will use.
        /// </summary>
        /// <remarks>
        /// These enumerations do not contain the entire list of LIDs.
        /// </remarks>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/23/06 mrj 7.30.00 N/A	Created
        //  09/13/06 KRC 7.35.00 		Adding more LIDS for AMI
        //  12/13/06 AF  8.00.00        Added Atmega f/w ver/rev for Open Way
        //  12/05/13 DLG 3.50.12 TR9480 Added Voltage Monitor Data
        //  02/12/16 PGH 4.50.229 RTT556309 Added Temperature Data
        //
        #region Base LIDs

        public enum BaseLIDs : uint
        {
            //Major Components
            BILL_SCHED_CONFIG = 0xA8000000,
            CALENDAR_DATA = 0x6C000000,
            COEFF_CONFIG = 0x00000000,
            COEFF_DATA = 0x04000000,
            COMPONENT_MASK = 0xFC000000,
            COMPONENT_MASK_OUT = 0x03FFFFFF,
            CONSTANT_CONFIG = 0x08000000,
            CONSTANTS_DATA = 0x0C000000,
            ENERGY_DATA = 0x14000000,
            DEMAND_CONFIG = 0x18000000,
            DEMAND_DATA = 0x1C000000,
            DISPLAY_DATA = 0x24000000,
            METROLOGY_CONFIG = 0x30000000,
            METROLOGY_DATA = 0x34000000,
            STATEMON_DATA = 0x3C000000,
            IO_DATA = 0x44000000,
            LOAD_PROFILE_CONFIG = 0x48000000,
            LOAD_PROFILE_DATA = 0x4C000000,
            SELF_READ_DATA = 0x5C000000,
            SITE_SCAN_DATA = 0x64000000,
            TOU_DATA = 0x74000000,
            PQ_DATA = 0x84000000,
            MODE_CNTRL_CONFIG = 0x88000000,
            MODE_CNTRL_DATA = 0x8C000000,
            MISC_DATA = 0x94000000,
            OPTION_BRD_DATA = 0xA4000000,
            METER_KEY_DATA = 0xE4000000,
            TEMPERATURE_DATA = 0xEC000000,
            VOLTMON_DATA = 0xF4000000,
        }

        #endregion Base LIDs

        #region "Base enumeration of the engery or demand quantity type"
        public enum WhichOneEnergyDemand : uint
        {
            WHICH_ONE_MASK = 0x0000003F,
            NOT_PROGRAMMED = 0x00000000,
            WH_DELIVERED = 0x00000001,
            WH_RECEIVED = 0x00000002,
            VARH_Q1 = 0x00000003,
            VARH_Q2 = 0x00000004,
            VARH_Q3 = 0x00000005,
            VARH_Q4 = 0x00000006,
            VARH_NET_DEL = 0x00000007,
            VARH_NET_REC = 0x00000008,
            VAH_DEL_ARITH = 0x00000009,
            VAH_REC_ARITH = 0x0000000A,
            VAH_DEL_VECT = 0x0000000B,
            VAH_REC_VECT = 0x0000000C,
            VAH_LAG = 0x0000000D,
            QH_DEL = 0x0000000E,
            VH_PHA = 0x0000000F,
            VH_PHB = 0x00000010,
            VH_PHC = 0x00000011,
            VH_AVG = 0x00000012,
            AH_PHA = 0x00000013,
            AH_PHB = 0x00000014,
            AH_PHC = 0x00000015,
            AH_NEUTRAL = 0x00000016,
            V2H_AGG = 0x00000017,
            I2H_AGG = 0x00000018,
            // not used						0x00000019
            PULSE_INPUT_1 = 0x0000001A,
            PULSE_INPUT_2 = 0x0000001B,
            WH_NET_PHA = 0x0000001C,
            WH_NET_PHB = 0x0000001D,
            WH_NET_PHC = 0x0000001E,
            // not used						0x0000001F
            VARH_DEL = 0x00000020,
            VARH_REC = 0x00000021,
            // not used						0x00000022
            // not used						0x00000023
            WH_NET = 0x00000024,
            VARH_NET = 0x00000025,
            QH_REC = 0x00000026,
            WH_UNI = 0x00000027,
            // not used						0x00000028
            // not used						0x00000029
            // not used						0x0000002A
            TOTALIZER1 = 0x0000002B,
            TOTALIZER2 = 0x0000002C,
            TOTALIZER3 = 0x0000002D,
            MAX_READABLE_ENERGY = TOTALIZER3,
            PF_INTERVAL_ARITH = 0x0000003C,
            PF_INTERVAL_VECT = 0x0000003D,
            NO_READ_VAH_VECT = 0x0000003E,
            ALL_CONFIGURED = 0x0000003E,
            ALL_ACTIVE = 0x0000003F,
        }
        #endregion

        #region Billing Schedule Config LIDs

        public enum BillSched_Config : uint
        {
            BILL_SCHED_NAME = 0x0000012C,
        }

        #endregion

        #region "Calendar LIDs"
        public enum Calendar_Data : uint
        {
            CLD_DATE = 0x00000000,
            CLD_TIME = 0x00000001,
            CLD_MIN_ON_BATTERY = 0x00000002,
            CLD_DAYS_ON_BATTERY = 0x00000003,
            CLD_HAS_CLOCK = 0x00000005,
            CLD_NUM_TOU_RATES = 0x00000006,
            CLD_DAY_OF_WEEK = 0x00000007,
            CLD_DST_CONFIGURED = 0x00000008,
            CLD_IN_DST = 0x00000009,
            CLD_EXPIRE_DATE = 0x0000000A,
            CLD_MASK = 0x0000000F,
        }
        #endregion

        #region Voltage Monitor Data

        public enum VoltMon_Data : uint
        {
            RMS_BELOW_THRESH_COUNT = 0x00000000,
            RMS_HIGH_THRESH_COUNT = 0x00000001,
            VH_BELOW_THRESH_COUNT = 0x00000002,
            VH_HIGH_THRESH_COUNT = 0x00000003,
        }

        #endregion Voltage Monitor Data

        #region Coefficient Config LIDs

        public enum Coefficients_Config : uint
        {
            CT_MULT = 0x00000000,
            VT_MULT = 0x00000001,
            REG_MULT = 0x00000002,
        }

        #endregion

        #region Coefficient Data LIDs

        public enum Coefficient_Data : uint
        {
            TRANSFORMER_MULT = 0x00000000,
            METER_MULT = 0x00000001,
            COEFF_MASK = 0x00000001,
        }

        #endregion

        #region "Constant Config"
        public enum Constant_Config : uint
        {
            CUST_SERIAL_NBR = 0x00000000,
            UNUSED_CONST = 0x00000001,
            LOAD_RESRCH = 0x00000002,
            NETWORK_ID = 0x00000003,
            PROGRAM_ID = 0x00000004,
            USER_DATA_1 = 0x00000005,
            USER_DATA_2 = 0x00000006,
            USER_DATA_3 = 0x00000007,
            PRI_SEC_METER = 0x00000008,
            CLOCK_SYNC = 0x00000009,
            INTERNAL_BAUD_RATE = 0x0000000A,
            TIME_ZONE_OFFSET = 0x0000000B,

            CONSTANT_CONFIG_MASK = 0x0000001F,
        }

        #endregion

        #region Constant Data

        public enum Constant_Data : uint
        {
            METER_ID_PART1 = 0x00000000,
            METER_ID_PART2 = 0x00000001,
            LOAD_RESRCH_PART1 = 0x00000002,
            LOAD_RESRCH_PART2 = 0x00000003,
            CONST_DATA_MASK = 0x0000000F,
        }

        #endregion

        #region "Demand LIDs"
        public enum Demand_Data : uint
        {
            DATA_SEG_MASK = 0x0003C000,
            DATA_SEG_MASK_OUT = 0xFFFC3FFF,
            PRES_DEMAND = 0x00000000,
            MAX_DEMAND = 0x00004000,
            MAX_DMND_2ND = 0x00008000,
            MAX_DMND_3RD = 0x0000C000,
            MAX_DMND_4TH = 0x00010000,
            MAX_DMND_5TH = 0x00014000,
            PREV_DEMAND = 0x00018000,
            PROJ_DEMAND = 0x0001C000,
            CUM_DEMAND = 0x00020000,
            CONT_CUM_DEMAND = 0x00024000,
            MIN_DEMAND = 0x00028000,
            COINC = 0x0002C000,
            MISC_DEMAND = 0x00030000,
            ALL_ACTIVE_DMND = 0x0003C000,
        }
        public enum MiscDemand : uint
        {
            MISC_DEMAND_MASK = 0x0000000F,
            TOO_TIME_REMAINING = 0x00000000,
            TO_LAST_DEMAND_RES = 0x00000001,
            COUNT_DEMAND_RES = 0x00000002,
            SUB_INT_LEN = 0x00000003,
            TEST_SUB_INT_LEN = 0x00000004,
            PF_AVG_BILL_ARITH = 0x00000005,
            PF_AVG_BILL_VECT = 0x00000006,
            DMD_THRESH_EXCEED = 0x00000007,
            DAYS_SINCE_DR = 0x00000008,
            NUM_DEMANDS = 0x00000009,
            NUM_CUMS = 0x0000000A,
            PF_AVG_BILL_AVAIL = 0x0000000B,
            LAST_MAX_DMD1 = 0x0000000C,
            LAST_MAX_DMD2 = 0x0000000D,
            LAST_MAX_DMD3 = 0x0000000E,
        }
        public enum WhichDemandFormat : uint
        {
            WHICH_FORMAT_MASK = 0x000000C0,
            WHICH_FORMAT_MASK_OUT = 0xFFFFFF3F,
            RAW_DATA = 0x00000000,
            TOO_DATA = 0x00000040,
            SECONDARY_DATA = 0x00000080,
            PRIMARY_DATA = 0x000000C0,
        }
        public enum CoincTriggers : uint
        {
            WH_DEL = 0x00000100,
            WH_REC = 0x00000200,
            VAR_Q1 = 0x00000300,
            VAR_Q2 = 0x00000400,
            VAR_Q3 = 0x00000500,
            VAR_Q4 = 0x00000600,
            VAH_DEL_ARITH = 0x00000900,
            VAH_REC_ARITH = 0x00000A00,
            VAH_DEL_VECT = 0x00000B00,
            VAH_REC_VECT = 0x00000C00,
            VAH_LAG = 0x00000D00,
            THD_V = 0x00001C00,
            THD_I = 0x00001E00,
            VAR_DEL = 0x00002000,
            VAR_REC = 0x00002100,
            VAH_NET_ARITH = 0x00002200,
            VAH_NET_VECT = 0x00002300,
            WH_NET = 0x00002400,
            WH_UNI = 0x00002700,
            VAR_NET = 0x00002500,
            WH_DEL_SLC = 0x00002E00,
            WH_REC_SLC = 0x00002F00,
            WH_NET_SLC = 0x00003000,
            VARH_Q1_SLC = 0x00003100,
            VARH_Q2_SLC = 0x00003200,
            VARH_Q3_SLC = 0x00003300,
            VARH_Q4_SLC = 0x00003400,
            VAR_DEL_SLC = 0x00003700,
            VAR_REC_SLC = 0x00003800,
            VAH_DEL_VECT_SLC = 0x00003A00,
            VAH_REC_VECT_SLC = 0x00003B00,
            PF_MIN_ARITH = 0x00003C00,
            PF_MIN_VECT = 0x00003D00,
            TRIGGER_MASK = 0x00003F00,
        }
        #endregion

        #region Demand Config LIDs

        public enum DemandConfigMasks
        {
            DMD_CONF_LID_MASK = 0x0003FF3F,
            DMD_CONF_TYPE_MASK = 0x000001E0,
            DMD_CONF_DMD_MASK = 0x0000001F,
            DMD_CONF_THR_MASK = 0x0000001E,
            DMD_CONF_SL_MASK = 0x00000001,
        }

        public enum DemandConfig_Data
        {
            CONF_DMD_TYPE = 0x00000000,
            CONF_NBR_SUB = 0x00000020,
            CONF_INT_LEN = 0x00000040,
            CONF_TST_NBR_SUB = 0x00000060,
            CONF_TST_INT_LEN = 0x00000080,
            CONF_DAY_MEAN = 0x000000A0,
            CONF_HOUR = 0x000000C0,
            CONF_MIN = 0x000000E0,
            CONF_DAY = 0x00000100,
            CONF_DMD_DEF = 0x00000120,
            CONF_THR_DEF = 0x00000140,
            CONF_REG_FS = 0x00000160,
            CONF_OB_CLPU = 0x00000180,
            CONF_CLPU = 0x000001A0,
        }

        public enum DemandConfigThresholds
        {
            CONF_THR1 = 0x00000000,
            CONF_THR2 = 0x00000002,
            CONF_THR3 = 0x00000004,
            CONF_THR4 = 0x00000006,
            CONF_MASK = 0x0000000E,
        }

        public enum DemandConfigThreshold_Data
        {
            CONF_THR_SRC = 0x00000000,
            CONF_THR_LVL = 0x00000001,
            CONF_THR_MASK = 0x00000001,
        }

        #endregion

        #region Display Data

        public enum Display_Data : uint
        {
            DISPLAY_TIME = 0x00000000,
        }

        #endregion

        #region "Energy LIDs"
        public enum Energy_Data : uint
        {
            ENERGY_DATA_MASK = 0x00000100,
            ENERGY_REG_DATA = 0x00000000,
            ENERGY_MISC_DATA = 0x00000100,
        }

        public enum WhichEnergyFormat : uint
        {
            WHICH_FORMAT_MASK = 0x000000C0,
            WHICH_FORMAT_MASK_OUT = 0xFFFFFF3F,
            RAW_DATA = 0x00000000,
            TOO_DATA = 0x00000040,
            SECONDARY_DATA = 0x00000080,
            PRIMARY_DATA = 0x000000C0,

        }

        public enum Misc_Energy : uint
        {
            NUMBER_OF_ENERGIES = 0x00000000,
        }

        #endregion

        #region "Load Profile LIDs"
        public enum LP_Config
        {
            LP_CONF_MASK = 0x0000001F,
            LP_INT_LEN = 0x00000000,
            LP_NUM_CHAN = 0x00000001,
            LP_CHAN_1_QUAN = 0x00000002,
            LP_CHAN_2_QUAN = 0x00000003,
            LP_CHAN_3_QUAN = 0x00000004,
            LP_CHAN_4_QUAN = 0x00000005,
            LP_CHAN_5_QUAN = 0x00000006,
            LP_CHAN_6_QUAN = 0x00000007,
            LP_CHAN_7_QUAN = 0x00000008,
            LP_CHAN_8_QUAN = 0x00000009,
            LP_CHAN_1_PW = 0x0000000A,
            LP_CHAN_2_PW = 0x0000000B,
            LP_CHAN_3_PW = 0x0000000C,
            LP_CHAN_4_PW = 0x0000000D,
            LP_CHAN_5_PW = 0x0000000E,
            LP_CHAN_6_PW = 0x0000000F,
            LP_CHAN_7_PW = 0x00000010,
            LP_CHAN_8_PW = 0x00000011,
            LP_SIGNED_INT = 0x00000014,
        }

        public enum LP_Data
        {
            LP_MISC_UNRD_BLCK = 0x00000000,
            LP_MISC_1ST_UNRD = 0x00000001,
            LP_MISC_LAST_INT = 0x00000002,
            LP_MISC_MAX_BLCK = 0x00000003,
            LP_MISC_VALID_BLCKS = 0x00000004,
            LP_MISC_SEQ_NUM = 0x00000005,
            LP_MISC_LAST_BLCK = 0x00000006,
            LP_MISC_CH1_REAL_PW = 0x00000007,
            LP_MISC_CH2_REAL_PW = 0x00000008,
            LP_MISC_CH3_REAL_PW = 0x00000009,
            LP_MISC_CH4_REAL_PW = 0x0000000A,
            LP_MISC_CH5_REAL_PW = 0x0000000B,
            LP_MISC_CH6_REAL_PW = 0x0000000C,
            LP_MISC_CH7_REAL_PW = 0x0000000D,
            LP_MISC_CH8_REAL_PW = 0x0000000E,
            LP_MISC_RUNNING = 0x0000000F,
            LP_MISC_MASK = 0x0000000F,

        }

        public enum LP_Which_Profile
        {
            //Standard Load Profle
            LP_STD_SET_1 = 0x00000000,
            LP_STD_SET_2 = 0x00200000,
            LP_STD_SET_3 = 0x00400000,
            LP_STD_SET_4 = 0x00600000,
            //Extended Load Profile
            LP_EXT_SET_1 = 0x00800000,
            //Instrumentation Profile
            LP_EXT_SET_2 = 0x00A00000,
            LP_EXT_SET_3 = 0x00C00000,
            LP_EXT_SET_4 = 0x00E0000,
        }
        #endregion

        #region Metrology Config LIDs

        public enum Metrology_Config : uint
        {
            CPC_CONF_TYPE_MASK = 0x0000001F,
            CPC_CONF_PUL_WGT_NORM = 0x00000000,
            CPC_CONF_PUL_OUT1_NORM = 0x00000001,
            CPC_CONF_PUL_WGT_ALT = 0x00000002,
            CPC_CONF_PUL_OUT1_ALT = 0x00000003,
            CPC_CONF_PUL_WGT_TEST = 0x00000004,
            CPC_CONF_PUL_OUT1_TEST = 0x00000005,
            CPC_CONF_PUL_WGT_TSTALT = 0x00000006,
            CPC_CONF_PUL_OUT1_TSTALT = 0x00000007,
            CPC_CONF_PWR_CALC_METH = 0x00000008,
            CPC_CONF_REAL_PW_NORM = 0x00000010,
            CPC_CONF_REAL_PW_ALT = 0x00000011,
            CPC_CONF_REAL_PW_TEST = 0x00000012,
            CPC_CONF_REAL_PW_TST_ALT = 0x00000013,
        }

        #endregion

        #region Metrology Data LIDs

        public enum MetrologyDataType : uint
        {
            CPC_ENERGY_TYPE = 0x00000000,
            CPC_INST_TYPE = 0x00001000,
            CPC_MISC_TYPE = 0x00004000,
            CPC_TYPE_MASK = 0x00007000,
        }

        public enum MetrologyDataFormat : uint
        {
            CPC_FORMAT_RAW = 0x00000000,
            CPC_FORMAT_SEC = 0x00000800,
            CPC_FORMAT_PRI = 0x00000C00,
            CPC_FORMAT_MASK = 0x00000C00,
        }

        public enum MetrologyDataIns : uint
        {
            CPC_INST_VRMS = 0x00000000,
            CPC_INST_IRMS = 0x00000040,
            CPC_INST_ANGLES = 0x00000080,
            CPC_INST_FREQ = 0x000000C0,
            CPC_INST_PF = 0x00000100,
            CPC_INST_NEURMS = 0x00000140,
            CPC_INST_W = 0x00000180,
            CPC_INST_VAR = 0x000001C0,
            CPC_INST_VA = 0x00000200,
            CPC_INST_I_DCOFFSET = 0x00000240,
            CPC_INST_MASK = 0x000003C0,
        }

        public enum MetrologyDataPhase : uint
        {
            CPC_PHASE_A = 0x00000000,
            CPC_PHASE_B = 0x00000001,
            CPC_PHASE_C = 0x00000002,
            CPC_PHASE_AGGREG = 0x00000003,
            CPC_PHASE_NEUT = 0x00000003,
            CPC_PHASE_MASK = 0x00000007,
        }

        public enum MetrologyDataCalc : uint
        {
            CPC_CALC_ARITH = 0x00000000,
            CPC_CALC_VECT = 0x00000010,
            CPC_CALC_LAG = 0x00000020,
            CPC_CALC_MASK = 0x00000030,
        }

        #endregion

        #region "Misc LIDs"
        public enum Misc_Data : uint
        {
            MISC_CONFIG_SIZE = 0x00000000,
            MISC_CONFIG_VER = 0x00000001,
            MISC_LAST_CONFIG_TIME = 0x00000002,
            MISC_TEST_TIME = 0x00000003,
            MISC_OUTAGE_TIME = 0x00000004,
            MISC_INTERROGATION = 0x00000005,
            MISC_FW_VERSION = 0x00000006,
            MISC_FW_REVISION = 0x00000007,
            MISC_FW_VERS_REV = 0x00000008,
            MISC_SW_VERSION = 0x00000009,
            MISC_SW_REVISION = 0x0000000A,
            MISC_SW_VERS_REV = 0x0000000B,
            MISC_GOOD_BATT = 0x0000000C,
            MISC_CURRENT_BATT = 0x0000000D,
            MISC_CANADIAN_METER = 0x0000000E,
            MISC_SEALED_CANADIAN = 0x0000000F,
            MISC_DAYS_SINCE_TST = 0x00000010,
            MISC_HW_VERSION = 0x00000011,
            MISC_HW_REVISION = 0x00000012,
            MISC_HW_VERS_REV = 0x00000013,
            MISC_POWERUP_CMPLT = 0x00000014,
            MISC_LDR_VERSION = 0x00000015,
            MISC_LDR_REVISION = 0x00000016,
            MISC_LDR_VERS_REV = 0x00000017,
            MISC_FW_BUILD = 0x00000018,
            MISC_HW_PRESENT = 0x00000019,
            MISC_MFG_SER_NUM = 0x0000001A,
            MISC_MFG_SPEC_NUM = 0x0000001B,
            MISC_MFG_TIME_STMP = 0x0000001C,
            MISC_MONO_FREQ = 0x0000001D,
            MISC_SEGMENT_TEST = 0x0000001F,
            MISC_ATM_VERSION = 0x00000020,
            MISC_ATM_REVISION = 0x00000021,
            MISC_ATM_VERS_REV = 0x00000022,
            MISC_DATA_MASK = 0x0000003F,
        }
        #endregion

        #region "Mode Control LIDs"
        public enum Mode_Cntrl_Config : uint
        {
            MM_TIMEOUT_CONFIG = 0x00000000,
            MM_LOCKOUT_TIME = 0x00000001,
            MM_DISABLE_SWITCH = 0x00000002,
            MM_CONFIG_MASK = 0x0000000F,
        }

        public enum Mode_Cntrl_Data : uint
        {
            MM_WHICH_ONE_MASK = 0x000000FF,
            MM_TIMEOUT_REMAIN = 0x00000000,
            MM_METER_MODE = 0x00000001,
        }
        #endregion

        #region "Temperature LIDs"
        public enum Temperature_Data : uint
        {
            TEMPERATURE = 0xEC00000E,
            AVERAGE_AGGREGATE_CURRENT = 0x34001283,
        }
        #endregion

        #region "State Monitor LIDs"
        public enum StatMon_Data : uint
        {
            STATMON_STD = 0x00000000,
            STATMON_DEBUG = 0x00000040,
        }
        public enum StateMon_STD : uint
        {
            INV_TAMPER_COUNT = 0x00000010,
            REMOV_TAMPER_COUNT = 0x00000011,
            OPT_LOGON_COUNT = 0x0000002C,
            STATEMON_STD_MASK = 0x0000003F,
        }
        #endregion

        #region MeterKey LIDS
        public enum MeterKey_Data : uint
        {
            METER_KEY_MASK = 0x0000000F,
            KEY_SOFT_VER = 0x00000000,
            KEY_SOFT_REV = 0x00000001,
            KEY_FLAVOR = 0x00000002,
            KEY_TIME = 0x00000003,
            KEY_ENERGY_1 = 0x00000004,
            KEY_DEMAND = 0x00000005,
            KEY_TOU = 0x00000006,
            KEY_LP = 0x00000007,
            KEY_PQ = 0x00000008,
            KEY_MISC = 0x00000009,
            KEY_IO = 0x0000000A,
            KEY_OPT_BRD = 0x0000000B,
            KEY_INSTANT = 0x0000000C,
            KEY_SELF_READ = 0x0000000D,
            KEY_CALENDAR = 0x0000000E,
            KEY_ENERGY_2 = 0x0000000F,
        }

        #endregion

        #region "SelfRead LIDs"
        public enum SlfRd_Data : uint
        {
            SR_FILE_MASK = 0x03800000,
            SR_LAST_SEASON = 0x00000000,
            SR_LAST_DR = 0x00800000,
            SR_2ND_LAST_DR = 0x01000000,
            SR_BUFF_1 = 0x01800000,
            SR_BUFF_2 = 0x02000000,
            SR_BUFF_3 = 0x02800000,
            SR_BUFF_4 = 0x03000000,
            SR_BUFF_DATA = 0x03800000,
            SR_DATA_CLEAR = 0xFC7FFFFF,
        }
        public enum ReadTimeData : uint
        {
            SR_DATA_SEG_MASK = 0x00400000,
            SR_BILLING_DATA = 0x00000000,
            SR_OTHER_DATA = 0x00400000,
        }
        public enum OtherSRData : uint
        {
            SR_COMP_MASK = 0x00300000,
            SR_MISC = 0x00000000,
            SR_INST = 0x00100000,
            SR_STATE_DATA = 0x00200000,
        }
        public enum WhichSRMisc : uint
        {
            SR_MISC_MASK = 0x00000003,
            MISC_SR_TIME = 0x00000000,
            MISC_SR_SEASON = 0x00000001,
            MISC_SR_VALID = 0x00000002,
        }
        public enum SR_Buffer_Data : uint
        {
            NUM_UNREAD_SELF_READS = 0x00000000,
            MAX_NUM_SELF_READS = 0x00000001,
            LAST_VALID_SELF_READ_INDEX = 0x00000002,
            NUM_VALID_SELF_READS = 0x00000003,
            LAST_SELF_READ_ENTRY_SEQ_NUM = 0x00000004,
            BUFFER_DATA_MASK = 0x0000000F,
        }
        #endregion

        #region "SiteScan LIDs"
        public enum SiteScan_Data : uint
        {
            PH_A_VOLTAGE = 0x00000000,
            PH_A_CURRENT = 0x00000001,
            PH_A_CUR_ANGLE = 0x00000002,
            PH_B_VOLTAGE = 0x00000003,
            PH_B_VOLT_ANGLE = 0x00000004,
            PH_B_CURRENT = 0x00000005,
            PH_B_CUR_ANGLE = 0x00000006,
            PH_C_VOLTAGE = 0x00000007,
            PH_C_VOLT_ANGLE = 0x00000008,
            PH_C_CURRENT = 0x00000009,
            PH_C_CUR_ANGLE = 0x0000000A,
            PH_A_DC_DETECT = 0x0000000B,
            PH_B_DC_DETECT = 0x0000000C,
            PH_C_DC_DETECT = 0x0000000D,
            SS_SERVICE_TYPE = 0x0000000E,

            SS_THD_V_A = 0x00000010,
            SS_THD_V_B = 0x00000011,
            SS_THD_V_C = 0x00000012,
            SS_TDD_I_A = 0x00000013,
            SS_TDD_I_B = 0x00000014,
            SS_TDD_I_C = 0x00000015,

            SS_SNAPSHOTS_FULL = 0x0000001A,
            SS_SNAPSHOT_COUNT = 0x0000001B,
            SS_NOMINAL_VOLTAGE = 0x0000001C,
            SS_ALL_INSTANT_2 = 0x0000001D,
            SS_SERV_TYPE_DISP = 0x0000001E,
            SS_ALL_INSTANT = 0x0000001F,
            SS_DATA_MASK = 0x0000001F,
        }
        #endregion

        #region "TOU LIDs"
        public enum TOU_Data : uint
        {
            TOU_DATA_MASK = 0x00380000,
            TOTAL = 0x00000000,
            RATE_A = 0x00080000,
            RATE_B = 0x00100000,
            RATE_C = 0x00180000,
            RATE_D = 0x00200000,
            RATE_E = 0x00280000,
            RATE_F = 0x00300000,
            RATE_G = 0x00380000,
        }
        public enum TOU_Rate_Data
        {
            TOU_RATE_MASK = 0x00040000,
            TOU_ENERGY = 0x00000000,
            TOU_DEMAND = 0x00040000,
        }
        #endregion

        #region "PQ LIDs"
        public enum VQ_Data : uint
        {
            PQ_VQ_LOG_EVT_CNT = 0x00000000,
            PQ_VQ_LOG_PCT_FULL = 0x00000001,
            PQ_SAG_CNT = 0x00000002,
            PQ_SWELL_CNT = 0x00000003,
            PQ_INTERRUPT_CNT = 0x00000004,
            PQ_IMBAL_V_CNT = 0x00000005,
            PQ_IMBAL_I_CNT = 0x00000006,
            PQ_THD_V_A = 0x00000007,
            PQ_THD_V_B = 0x00000008,
            PQ_THD_V_C = 0x00000009,
            PQ_TDD_I_A = 0x0000000A,
            PQ_TDD_I_B = 0x0000000B,
            PQ_TDD_I_C = 0x0000000C,
            PQ_ANSI_IEC = 0x0000000D,
            PQ_VQ_LOG_NEAR_FULL = 0x0000000E,
            PQ_PK_DMD_CURR = 0x0000000F,
            PQ_DATA_MASK = 0x0000000F,
        }
        #endregion "PQ LIDs"

        #region "Option Board LIDs"
        public enum Opt_Brd_Data : uint
        {
            OPT_BRD_ID = 0x00000000,
            OPT_BRD_VEND_FLD_1 = 0x00000001,
            OPT_BRD_VEND_FLD_2 = 0x00000002,
            OPT_BRD_VEND_FLD_3 = 0x00000003,
            OPT_BRD_DATA_MASK = 0x0000000F,
        }
        #endregion
#pragma warning restore 1591
        #endregion

        #region Public Methods

        /// <summary>
        /// LIDs contructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/06/06 mrj 7.30.00 N/A	Created
        //  10/30/06 KRC 7.40.00 N/A    Removed LIDs and made them static properties.
        public DefinedLIDs()
        {
            //All shared LIDs should be declated as static properties.  We will leave the device specific
            //  LIDS here for now.

            //This LIDs will be created by the device specific LID classes
            STATEMON_NON_FATAL_ERRORS = null;
            STATEMON_NON_FATAL_ERRORS2 = null;
            STATEMON_FATAL_ERRORS = null;
            SITESCAN_DIAG_1_COUNT = null;
            SITESCAN_DIAG_2_COUNT = null;
            SITESCAN_DIAG_3_COUNT = null;
            SITESCAN_DIAG_4_COUNT = null;
            SITESCAN_DIAG_5_COUNT = null;
            SITESCAN_DIAG_5_COUNT_A = null;
            SITESCAN_DIAG_5_COUNT_B = null;
            SITESCAN_DIAG_5_COUNT_C = null;
            SITESCAN_DIAG_6_COUNT = null;
            STATEMON_DIAG_ERRORS = null;
        }

        #endregion

    }//class LIDs

    /// <summary>
    /// This class is used to define the LID values for the SENTINEL meter.
    /// The LID objects from this class can then be used by the LIDRetriever
    /// class to retrieve the data.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------
    //  05/24/06 mrj 7.30.00 N/A	Created
    // 
    internal class SentinelDefinedLIDs : DefinedLIDs
    {
        #region Constants

        #region Base LIDs

        internal new enum BaseLIDs : uint
        {
            CALIBRATION_DATA = 0xEC000000,
        }

        #endregion

        #region State Monitor LIDs

        //These LID values are different from the Centron meters.
        internal new enum StateMon_STD : uint
        {
            STATMON_STD = 0x00000000,
            NON_FATAL_ERRORS = 0x00000000,
            FATAL_ERRORS = 0x00000001,
            DIAG_ERRORS = 0x00000002,
            DIAG_COUNT_1 = 0x00000003,
            DIAG_COUNT_2 = 0x00000004,
            DIAG_COUNT_3 = 0x00000005,
            DIAG_COUNT_4 = 0x00000006,
            DIAG_COUNT_5 = 0x00000007,
            PHASE_COUNT_1 = 0x00000008,
            PHASE_COUNT_2 = 0x00000009,
            PHASE_COUNT_3 = 0x0000000A,
            POWER_OUTAGES = 0x0000000B,
            TIMES_PROGRAMMED = 0x0000000C,
            EPF_COUNT = 0x0000000D,
            NON_FATAL_ERRORS2 = 0x0000000E,
            DIAG_COUNT_6 = 0x00000010,
            OPT_LOGON_COUNT = 0x0000002C,
            AUX_LOGON_COUNT = 0x0000002D,
            DIAG6_ERRORS = 0x0000002E,

        }

        #endregion

        #region "IO LIDs"

        public enum IO_Data : uint
        {
            IO_CAPABILITES = 0x00000002,
            IO_DATA_MASK = 0x00000007,
        }

        #endregion

        #region Calibration Data LIDs

        internal enum CalibDataQuantities : uint
        {
            CAL_NUM_ELE = 0x00000000,
            CAL_VOLT_QUAL = 0x00000100,
            CAL_METER_CLASS = 0x00000200,
            CAL_FREQ = 0x00000300,
            CAL_PH_ENERGY_GAIN = 0x00000400,
            CAL_PH_IRMS_GAIN = 0x00000500,
            CAL_PH_VRMS_GAIN = 0x00000600,
            CAL_PH_FILTER = 0x00000700,
            CAL_PH_INTEG_FILTER = 0x00000800,
            CAL_IRMS_SCALE = 0x00000900,
            CAL_VRMS_SCALE = 0x00000A00,
            CAL_PULSE_OUTPUT = 0x00000B00,
            CAL_I_ANTI_CREEP = 0x00000C00,
            CAL_V_ANTI_CREEP = 0x00000D00,
            CAL_ACT_ENERG_OFF = 0x00001000,
            CAL_RACT_ENERG_OFF = 0x00001100,
            CAL_METER_FORM = 0x00001200,
            CAL_METER_BASE = 0x00001300,
            CAL_PHASE_LOSS_THR = 0x00001400,
            CAL_MFG_SERIAL_NUM = 0x00001500,
            CAL_MFG_SPEC = 0x00001600,
            CAL_TIME_STAMP = 0x00001700,
            CAL_HW_VERSION = 0x00001800,
            CAL_HW_REVISION = 0x00001900,
            CAL_HW_VERS_REV = 0x00001A00,
            CAL_PWR_SUP_TYPE = 0x00001B00,
            CAL_ALL_CAL = 0x00001F00,
            CAL_QUANT_MASK = 0x00001F00,
        }

        internal enum CalibDataPhase : uint
        {
            CAL_PHASE_A = 0x00000000,
            CAL_PHASE_B = 0x00000004,
            CAL_PHASE_C = 0x00000008,
            CAL_PHASE_MASK = 0x0000000C,
        }

        internal enum CalibDataPulseOut : uint
        {
            CAL_PULSE_OUT_1 = 0x00000000,
            CAL_PULSE_OUT_2 = 0x00000004,
            CAL_PULSE_OUT_MASK = 0x0000000C,
        }

        internal enum CalibDataTime : uint
        {
            CAL_MAX_ON_TIME = 0x00000000,
            CAL_MIN_OFF_TIME = 0x00000010,
            CAL_TIME_MASK = 0x00000030,
        }

        internal enum CalibDataITAP : uint
        {
            CAL_ITAP1 = 0x00000000,
            CAL_ITAP2 = 0x00000001,
            CAL_ITAP3 = 0x00000002,
            CAL_ITAP_MASK = 0x00000003,
        }

        internal enum CalibDataFilter : uint
        {
            CAL_BELOW_2A = 0x00000000,
            CAL_ABOVE_2A = 0x00000040,
            CAL_GAIN_COEF = 0x00000080,
            CAL_FILTER_MASK = 0x000000C0,
        }

        internal enum CalibDataKTAP : uint
        {
            CAL_KTAP1 = 0x00000000,
            CAL_KTAP2 = 0x00000010,
            CAL_KTAP3 = 0x00000020,
            CAL_KTAP_MASK = 0x00000030,

        }

        internal enum CalibDataGainMult : uint
        {
            CAL_GAIN_MULT = 0x00000000,
            CAL_GAIN_GROSS_MULT = 0x00000010,
            CAL_GAIN_MULT_MASK = 0x00000010,
        }

        #endregion

        #endregion

        /// <summary>
        /// Constructor.  Overrides the LIDs which differ from the Centron
        /// meters.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/24/06 mrj 7.30.00 N/A	Created
        // 
        public SentinelDefinedLIDs()
            : base()
        {
            //Create these State Monitor LIDs
            STATEMON_NON_FATAL_ERRORS = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.NON_FATAL_ERRORS);
            STATEMON_NON_FATAL_ERRORS2 = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.NON_FATAL_ERRORS2);
            STATEMON_FATAL_ERRORS = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.FATAL_ERRORS);
            SITESCAN_DIAG_1_COUNT = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.DIAG_COUNT_1);
            SITESCAN_DIAG_2_COUNT = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.DIAG_COUNT_2);
            SITESCAN_DIAG_3_COUNT = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.DIAG_COUNT_3);
            SITESCAN_DIAG_4_COUNT = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.DIAG_COUNT_4);
            SITESCAN_DIAG_5_COUNT = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.DIAG_COUNT_5);
            SITESCAN_DIAG_5_COUNT_A = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.PHASE_COUNT_1);
            SITESCAN_DIAG_5_COUNT_B = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.PHASE_COUNT_2);
            SITESCAN_DIAG_5_COUNT_C = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.PHASE_COUNT_3);
            SITESCAN_DIAG_6_COUNT = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.DIAG_COUNT_6);
            STATEMON_DIAG_ERRORS = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.DIAG_ERRORS);
            NUMBER_POWER_OUTAGES = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.POWER_OUTAGES);
            NUMBER_TIMES_PROGRAMMED = new SentinelLID((uint)DefinedLIDs.BaseLIDs.STATEMON_DATA | (uint)SentinelDefinedLIDs.StateMon_STD.STATMON_STD | (uint)SentinelDefinedLIDs.StateMon_STD.TIMES_PROGRAMMED);
            METER_FORM_FACTOR = new SentinelLID((uint)BaseLIDs.CALIBRATION_DATA | (uint)CalibDataQuantities.CAL_METER_FORM);
            VQ_PERCENT_LOG_FULL = new SentinelLID((uint)DefinedLIDs.BaseLIDs.PQ_DATA | (uint)DefinedLIDs.VQ_Data.PQ_VQ_LOG_PCT_FULL, TypeCode.Byte);
            VQ_SAG_COUNT = new SentinelLID((uint)DefinedLIDs.BaseLIDs.PQ_DATA | (uint)DefinedLIDs.VQ_Data.PQ_SAG_CNT, TypeCode.Byte);
            VQ_SWELL_COUNT = new SentinelLID((uint)DefinedLIDs.BaseLIDs.PQ_DATA | (uint)DefinedLIDs.VQ_Data.PQ_SWELL_CNT, TypeCode.Byte);
            VQ_INTERRUPTION_COUNT = new SentinelLID((uint)DefinedLIDs.BaseLIDs.PQ_DATA | (uint)DefinedLIDs.VQ_Data.PQ_INTERRUPT_CNT, TypeCode.Byte);
            VQ_VOLTAGE_IMBALANCE_COUNT = new SentinelLID((uint)DefinedLIDs.BaseLIDs.PQ_DATA | (uint)DefinedLIDs.VQ_Data.PQ_IMBAL_V_CNT, TypeCode.Byte);
            VQ_CURRENT_IMBALANCE_COUNT = new SentinelLID((uint)DefinedLIDs.BaseLIDs.PQ_DATA | (uint)DefinedLIDs.VQ_Data.PQ_IMBAL_I_CNT, TypeCode.Byte);
            METER_CALIBRATION_DATE = new SentinelLID((uint)BaseLIDs.CALIBRATION_DATA | (uint)CalibDataQuantities.CAL_TIME_STAMP);
            IO_BOARD_CAPABILITES = new SentinelLID((uint)DefinedLIDs.BaseLIDs.IO_DATA | (uint)IO_Data.IO_CAPABILITES);
        }
    }//class SentinelLIDs

    /// <summary>
    /// This class is used to define the LID values for the Centron (C12.19) meter.
    /// The LID objects from this class can then be used by the LIDRetriever
    /// class to retrieve the data.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------
    //  05/24/06 mrj 7.30.00 N/A	Created
    // 
    public class CentronMonoDefinedLIDs : DefinedLIDs
    {
        #region Constants
        //These LID values are different from the Sentinel meters.
        internal new enum StateMon_STD : uint
        {
            STATMON_STD = 0x00000000,
            NON_FATAL_ERRORS = 0x00000000,
            FATAL_ERRORS = 0x00000001,
            DIAG_ERRORS = 0x00000002,
            NON_FATAL_ERRORS2 = 0x00000003,
            DIAG_COUNT_1 = 0x00000004,
            DIAG_COUNT_2 = 0x00000005,
            DIAG_COUNT_3 = 0x00000006,
            DIAG_COUNT_4 = 0x00000007,
            DIAG_COUNT_5 = 0x00000008,
            PHASE_COUNT_1 = 0x00000009,
            PHASE_COUNT_2 = 0x0000000A,
            PHASE_COUNT_3 = 0x0000000B,
            DIAG_COUNT_6 = 0x0000000C,
            POWER_OUTAGES = 0x0000000D,
            TIMES_PROGRAMMED = 0x0000000E,
            EPF_COUNT = 0x0000000F,
            OPT_LOGON_COUNT = 0x0000002C,
            AUX_LOGON_COUNT = 0x0000002D,
            DIAG6_ERRORS = 0x0000002E,

        }

        #endregion

        /// <summary>
        /// Constructor.  Overrides the LIDs which differ from the Sentinel
        /// meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/24/06 mrj 7.30.00 N/A	Created
        // 
        public CentronMonoDefinedLIDs()
            : base()
        {
            //Create these State Monitor LIDs 
            STATEMON_NON_FATAL_ERRORS = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.NON_FATAL_ERRORS);
            STATEMON_NON_FATAL_ERRORS2 = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.NON_FATAL_ERRORS2);
            STATEMON_FATAL_ERRORS = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.FATAL_ERRORS);
            SITESCAN_DIAG_1_COUNT = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG_COUNT_1);
            SITESCAN_DIAG_2_COUNT = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG_COUNT_2);
            SITESCAN_DIAG_3_COUNT = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG_COUNT_3);
            SITESCAN_DIAG_4_COUNT = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG_COUNT_4);
            SITESCAN_DIAG_5_COUNT = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG_COUNT_5);
            SITESCAN_DIAG_5_COUNT_A = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.PHASE_COUNT_1);
            SITESCAN_DIAG_5_COUNT_B = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.PHASE_COUNT_2);
            SITESCAN_DIAG_5_COUNT_C = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.PHASE_COUNT_3);
            SITESCAN_DIAG_6_COUNT = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG_COUNT_6);
            STATEMON_DIAG_ERRORS = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG_ERRORS);
            NUMBER_POWER_OUTAGES = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.POWER_OUTAGES);
            NUMBER_TIMES_PROGRAMMED = new CentronMonoLID((uint)BaseLIDs.STATEMON_DATA | (uint)CentronMonoDefinedLIDs.StateMon_STD.STATMON_STD | (uint)CentronMonoDefinedLIDs.StateMon_STD.TIMES_PROGRAMMED);
            BILLING_SCHEDULE_NAME = new CentronMonoLID((uint)BaseLIDs.BILL_SCHED_CONFIG | (uint)BillSched_Config.BILL_SCHED_NAME);
        }

    }//class CentronLIDs

    /// <summary>
    /// This class is used to define the LID values for the Centron Poly meter.
    /// The LID objects from this class can then be used by the LIDRetriever
    /// class to retrieve the data.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------
    //  01/16/07 RCG 8.00.06 N/A	Created

    public class CentronPolyDefinedLIDs : CentronMonoDefinedLIDs
    {
        #region Constants

        #region Base LIDs
        internal new enum BaseLIDs : uint
        {
            //Major Components
            BASE_DATA = 0xEC000000,
        }

        #endregion

        #region Base Data LIDs
        internal enum Base_Data : uint
        {
            BASE_NUM_ELE = 0x00000000,
            BASE_METER_CLASS = 0x00000001,
            BASE_FREQ = 0x00000002,
            BASE_SERVICE_TYPE = 0x00000003,
            BASE_METER_FORM = 0x00000004,
            BASE_METER_BASE = 0x00000005,
            BASE_TIME_STAMP = 0x00000006,
            BASE_HW_VERSION = 0x00000007,
            BASE_HW_REVISION = 0x00000008,
            BASE_HW_VERS_REV = 0x00000009,
            BASE_FW_VERSION = 0x0000000A,
            BASE_FW_REVISION = 0x0000000B,
            BASE_FW_VERS_REV = 0x0000000C,
            BASE_PWR_SUP_TYPE = 0x0000000D,
            BASE_ALL_BASE = 0x0000001F,
            BASE_QUANT_MASK = 0x0000001F,
        }
        #endregion

        #endregion

        /// <summary>
        /// Constructor. Adds LIDs specific to the Centron Poly
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/16/07 RCG 8.00.06 N/A	Created

        public CentronPolyDefinedLIDs()
            : base()
        {
            METER_FORM_FACTOR = new CentronPolyLID((uint)BaseLIDs.BASE_DATA | (uint)Base_Data.BASE_METER_FORM);
            METER_BASE_TYPE = new CentronPolyLID((uint)BaseLIDs.BASE_DATA | (uint)Base_Data.BASE_METER_BASE);
            METER_HARDWARE_VER_REV = new CentronPolyLID((uint)BaseLIDs.BASE_DATA | (uint)Base_Data.BASE_HW_VERSION);
            METER_CALIBRATION_DATE = new CentronPolyLID((uint)BaseLIDs.BASE_DATA | (uint)Base_Data.BASE_TIME_STAMP);
        }
    }

    /// <summary>
    /// LID object that contains all information necessary for retrieving
    /// data from the meter using a LID value.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------
    //  05/25/06 mrj 7.30.00 N/A	Created
    // 
    public class LID : IEquatable<LID>
    {
        #region Constants
        /// <summary>
        /// The unit of measure a quantity is stored in.
        /// </summary>
        public enum MeasurementUnit
        {
            /// <summary>
            /// UNIT = 0
            /// </summary>
            UNIT = 0,
            /// <summary>
            /// KILO = 1
            /// </summary>
            KILO = 1,
            /// <summary>
            /// MEGA = 2
            /// </summary>
            MEGA = 2,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// LID constructor
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/31/07 RCG 8.00.11 N/A	Updated to call the constructor with the
        //                              most parameters

        public LID(uint lid) :
            this(lid, TypeCode.Empty, 0, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        ///  LID Constructor that sets the unit type
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/31/07 RCG 8.00.11 N/A	Created

        public LID(uint lid, MeasurementUnit measUnit) :
            this(lid, TypeCode.Empty, 0, measUnit)
        {
        }

        /// <summary>
        /// LID Constructor that takes a type.
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/31/07 RCG 8.00.11 N/A	Updated to call the constructor with the
        //                              most parameters

        public LID(uint lid, TypeCode type) :
            this(lid, type, 0, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        /// LID Constructor that takes the data type and unit type
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/31/07 RCG 8.00.11 N/A	Created

        public LID(uint lid, TypeCode type, MeasurementUnit measUnit) :
            this(lid, type, 0, measUnit)
        {
        }

        /// <summary>
        /// LID Constructor that takes a type and the length (Used for Strings)
        /// </summary>
        /// <param name="lid">32-bit number value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="uiLength">The length of the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/31/07 RCG 8.00.11 N/A	Updated to call the constructor with the
        //                              most parameters

        public LID(uint lid, TypeCode type, uint uiLength) :
            this(lid, type, uiLength, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        /// LID Constructor that takes the data type, length, and unit type
        /// </summary>
        /// <param name="lid">32-bit number value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="uiLength">The length of the data</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/31/07 RCG 8.00.11 N/A	Created
        //  01/21/11 RCG 2.45.24        Fixing Ins VA and Ins Var reading issue

        public LID(uint lid, TypeCode type, uint uiLength, MeasurementUnit measUnit)
        {
            // Get the resource manager
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS, this.GetType().Assembly);

            // Set the member variables
            m_lidValue = lid;
            m_lidType = type;
            m_requestedTypeCode = type;
            m_lidLength = uiLength;
            m_lidMeasUnit = measUnit;

            // Determine the LID description and type
            DetermineLIDInformation();
        }

        /// <summary>
        /// MakeSecondary - Changes the current LID to type Secondary and repopulates the Info
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/26/07 KRC 8.00.14 N/A	Created

        public void MakeSecondary()
        {
            m_bIsPrimary = false;

            if (true == IsEnergy)
            {
                m_lidValue = m_lidValue & (uint)DefinedLIDs.WhichEnergyFormat.WHICH_FORMAT_MASK_OUT;
                m_lidValue = m_lidValue | (uint)DefinedLIDs.WhichEnergyFormat.SECONDARY_DATA;
            }
            else
            {
                m_lidValue = m_lidValue & (uint)DefinedLIDs.WhichDemandFormat.WHICH_FORMAT_MASK_OUT;
                m_lidValue = m_lidValue | (uint)DefinedLIDs.WhichDemandFormat.SECONDARY_DATA;
            }

            // Repopulate the LID Info
            DetermineLIDInformation();
        }

        /// <summary>
        /// This method determines if one LID value is equal to the current.
        /// </summary>
        /// <param name="other">The LID value to compare against the current.</param>
        /// <returns>Whether the given LID is equal to the current.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/11/10 jrf 2.45.02 N/A	Created
        public bool Equals(LID other)
        {
            if ((object)other == null) return base.Equals(other);

            if (this.lidValue == other.lidValue)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This method determines if one object is equal to the current LID value.
        /// </summary>
        /// <param name="obj">The object value to compare against the current LID.</param>
        /// <returns>Whether the given object is equal to the current LID.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/11/10 jrf 2.45.02 N/A	Created
        public override bool Equals(Object obj)
        {
            if (obj == null) return base.Equals(obj);

            if (false == (obj is LID))
                throw new InvalidCastException("The 'obj' argument is not a LID object.");
            else
                return Equals(obj as LID);
        }

        /// <summary>
        /// This method returns a hash code for this instance.
        /// </summary>
        /// <returns>The hashcode.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/11/10 jrf 2.45.02 N/A	Created
        public override int GetHashCode()
        {
            return this.lidValue.GetHashCode();
        }

        /// <summary>
        /// This method determines if two LID values are equal.
        /// </summary>
        /// <param name="LID1">First LID</param>
        /// <param name="LID2">Second LID</param>
        /// <returns>Whether the two LIDs are equal.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/11/10 jrf 2.45.02 N/A	Created
        public static bool operator ==(LID LID1, LID LID2)
        {
            bool blnEquality = false;

            if (null != (object)LID1)
            {
                blnEquality = LID1.Equals(LID2);
            }
            else if (null == (object)LID2)
            {
                blnEquality = true;
            }

            return blnEquality;
        }

        /// <summary>
        /// This method determines if two LID values are not equal.
        /// </summary>
        /// <param name="LID1">First LID</param>
        /// <param name="LID2">Second LID</param>
        /// <returns>Whether the two LIDs are not equal.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/11/10 jrf 2.45.02 N/A	Created 
        public static bool operator !=(LID LID1, LID LID2)
        {
            bool blnInequality = false;

            if (null != (object)LID1)
            {
                blnInequality = (false == LID1.Equals(LID2));
            }
            else if (null != (object)LID2)
            {
                blnInequality = true;
            }

            return blnInequality;
        }

        /// <summary>
        /// This method converts the LID to a string representation.
        /// </summary>
        /// <returns>The string description of the LID.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/05/10 jrf 2.45.09        Created
        //
        public override string ToString()
        {
            return lidDescription;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Provides Get and Set access to the value of the LID
        /// </summary>
        /// <Returns>uint</Returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/05/06 KRC 7.36.00 N/A    Created
        //
        public uint lidValue
        {
            get
            {
                return m_lidValue;
            }
            set
            {
                m_lidValue = value;
                DetermineLIDInformation();
            }
        }

        /// <summary>
        /// Provides Get and Set access to the LID Type.
        /// </summary>
        /// <returns>TypeCode</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/05/06 KRC 7.36.00 N/A    Created
        //  01/21/11 RCG 2.45.24        Fixing Ins VA and Ins Var reading issue

        public TypeCode lidType
        {
            get
            {
                return m_lidType;
            }
            set
            {
                m_lidType = value;
                m_requestedTypeCode = value;
            }
        }

        /// <summary>
        /// Provides Get and Set access to the LID Length (Only needed for String types).
        /// </summary>
        /// <returns>uint</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/05/06 KRC 7.36.00 N/A    Created
        //
        public uint lidLength
        {
            get
            {
                return m_lidLength;
            }
            set
            {
                m_lidLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the Unit type for the LID and changes
        /// the description to reflect the unit change.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/31/07 RCG 8.00.11 N/A    Created

        public MeasurementUnit lidMeasurementUnit
        {
            get
            {
                return m_lidMeasUnit;
            }
            set
            {
                m_lidMeasUnit = value;
                DetermineLIDInformation();
            }
        }

        /// <summary>
        /// Gets and sets the description of the current LID. This field
        /// is populated when the lid value or measurement unit is changed
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/31/07 RCG 8.00.11 N/A    Created
        //  04/05/07 KRC 8.00.25        Add set support
        //
        public string lidDescription
        {
            get
            {
                return m_lidDescription;
            }
            set
            {
                m_lidDescription = value;
            }
        }

        /// <summary>
        /// Gets the type of quantity of the current LID. 
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/17/10 jrf 2.45.06 N/A    Created
        //
        public DefinedLIDs.WhichOneEnergyDemand lidQuantity
        {
            get
            {
                DefinedLIDs.WhichOneEnergyDemand Quantity = DefinedLIDs.WhichOneEnergyDemand.NOT_PROGRAMMED;
                uint uiQuantityType = (uint)DefinedLIDs.WhichOneEnergyDemand.WHICH_ONE_MASK & m_lidValue;

                if (true == Enum.IsDefined(typeof(DefinedLIDs.WhichOneEnergyDemand), uiQuantityType))
                {
                    Quantity = (DefinedLIDs.WhichOneEnergyDemand)uiQuantityType;
                }

                return Quantity;
            }
        }

        /// <summary>
        /// Gets the type of quantity that triggers a coincident demand quantity. 
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/10 jrf 9.70.12 N/A    Created
        //
        public DefinedLIDs.WhichOneEnergyDemand lidCoincidentTriggerQuantity
        {
            get
            {
                DefinedLIDs.WhichOneEnergyDemand Quantity = DefinedLIDs.WhichOneEnergyDemand.NOT_PROGRAMMED;
                uint uiTiggerType = m_lidValue & (uint)DefinedLIDs.CoincTriggers.TRIGGER_MASK;

                // Add the trigger register name
                switch (uiTiggerType)
                {
                    case (uint)DefinedLIDs.CoincTriggers.WH_DEL:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.WH_DELIVERED;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.WH_REC:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.WH_RECEIVED;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.VAR_Q1:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.VARH_Q1;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.VAR_Q2:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.VARH_Q2;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.VAR_Q3:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.VARH_Q3;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.VAR_Q4:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.VARH_Q4;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.VAH_DEL_ARITH:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_ARITH;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.VAH_REC_ARITH:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.VAH_REC_ARITH;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.VAH_DEL_VECT:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_VECT;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.VAH_REC_VECT:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.VAH_REC_VECT;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.VAH_LAG:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.VAH_LAG;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.VAR_DEL:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.VARH_DEL;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.VAR_REC:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.VARH_REC;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.WH_NET:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.WH_NET;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.WH_UNI:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.WH_UNI;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.VAR_NET:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.VARH_NET;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.PF_MIN_ARITH:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.PF_INTERVAL_ARITH;
                        break;
                    }
                    case (uint)DefinedLIDs.CoincTriggers.PF_MIN_VECT:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.PF_INTERVAL_VECT;
                        break;
                    }
                    default:
                    {
                        Quantity = DefinedLIDs.WhichOneEnergyDemand.NOT_PROGRAMMED;
                        break;
                    }

                }

                return Quantity;
            }
        }

        /// <summary>
        /// Gets the Is Energy Property
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 KRC 8.00.13 N/A    Created
        //
        public bool IsEnergy
        {
            get
            {
                return m_bIsEnergy;
            }
        }

        /// <summary>
        /// Gets the Is Max Demand Property
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 KRC 8.00.13 N/A    Created
        //
        public bool IsMaxDemand
        {
            get
            {
                return m_bIsMaxDemand;
            }
        }

        /// <summary>
        /// Gets the Is Min Demand Property
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 KRC 8.00.13 N/A    Created
        //
        public bool IsMinDemand
        {
            get
            {
                return m_bIsMinDemand;
            }
        }

        /// <summary>
        /// Gets the Is Peak Demand Property (Peak 2 - 5)
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 KRC 8.00.13 N/A    Created
        //
        public bool IsPeakDemand
        {
            get
            {
                return m_bIsPeakDemand;
            }
        }

        /// <summary>
        /// Gets the Is Cum Demand Property
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 KRC 8.00.13 N/A    Created
        //
        public bool IsCumDemand
        {
            get
            {
                return m_bIsCumDemand;
            }
        }

        /// <summary>
        /// Gets the Is CCum Demand Property
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 KRC 8.00.13 N/A    Created
        //
        public bool IsCCumDemand
        {
            get
            {
                return m_bIsCCumDemand;
            }
        }

        /// <summary>
        /// Gets the Is NonRegister Item Property
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 KRC 8.00.13 N/A    Created
        //
        public bool IsNonRegisterItem
        {
            get
            {
                return m_bIsNonRegisterItem;
            }
        }

        /// <summary>
        /// Gets the Is TOO Item Property
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 KRC 8.00.13 N/A    Created
        //
        public bool IsTOO
        {
            get
            {
                return m_bIsTOO;
            }
        }

        /// <summary>
        /// Gets the Is TOU Rate Item Property (Indicates if it is Rate A-G 
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/26/07 KRC 8.00.14 N/A    Created
        //
        public bool IsTOURate
        {
            get
            {
                return m_bIsTOURate;
            }
        }

        /// <summary>
        /// Gets the Is Coincident Item Property
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 KRC 8.00.13 N/A    Created
        //
        public bool IsCoincident
        {
            get
            {
                return m_bIsCoincident;
            }
        }

        /// <summary>
        /// Gets the Is Present Item Property
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 KRC 8.00.13 N/A    Created
        //
        public bool IsPresentDemand
        {
            get
            {
                return m_bIsPresent;
            }
        }

        /// <summary>
        /// Gets the Is Projected Demand Item Property
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 KRC 8.00.13 N/A    Created
        //
        public bool IsProjectedDemand
        {
            get
            {
                return m_bIsProjected;
            }
        }

        /// <summary>
        /// Gets the Is Previous Item Property
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 KRC 8.00.13 N/A    Created
        //
        public bool IsPreviousDemand
        {
            get
            {
                return m_bIsPrevious;
            }
        }

        /// <summary>
        /// Gets the Is Primary Item Property
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/26/07 KRC 8.00.14 N/A    Created
        //
        public bool IsPrimary
        {
            get
            {
                return m_bIsPrimary;
            }
        }

        /// <summary>
        /// Gets the Is Self Read Property.  This tells us if is is a Self Read Register.
        /// This value is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 KRC 8.00.18 N/A    Created
        //
        public bool IsSelfRead
        {
            get
            {
                return m_bIsSelfRead;
            }
        }

        /// <summary>
        /// Gets the Is Snapshot Property.  This tells us if is is a Snapshot Register.
        /// This value is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 KRC 8.00.18 N/A    Created
        //
        public bool IsSnapshot
        {
            get
            {
                return m_bIsSnapShot;
            }
        }

        /// <summary>
        /// Gets the Is Last Season Property.  This tells us if is is a Last Season Register.
        /// This value is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 KRC 8.00.18 N/A    Created
        //
        public bool IsLastSeason
        {
            get
            {
                return m_bIsLastSeason;
            }
        }

        /// <summary>
        /// Gets the Is Negative Allowed Property.
        /// This value is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/29/07 KRC 8.00.11 N/A    Created
        //
        public bool IsNegativeAllowed
        {
            get
            {
                return m_bIsNegativeAllowed;
            }
        }

        /// <summary>
        /// Gets the Is Quantity Property
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/03/10 jrf 9.70.13 N/A    Created
        //
        public bool IsQuantity
        {
            get
            {
                return m_bIsQuantity;
            }
        }

        /// <summary>
        /// Gets the Is Instantaneous Property
        /// is populated during DetermineLIDInformation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/06/10 jrf 9.70.16 N/A    Created
        //
        public bool IsInstantaneous
        {
            get
            {
                return m_bIsInstantaneous;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Determine the LID Type if it is not provided in the constructor.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#       Description
        //  -------- --- -------  ------       ---------------------------------------------
        //  10/05/06 KRC 7.36.00  N/A          Created
        //  01/21/11 RCG 2.45.24               Fixing Ins VA and Ins Var reading issue
        //  10/28/11 jrf 2.53.01		       Adding description for Firmware Build LID.
        //  09/25/12 jrf 2.70.23  TQ6616       Adding code to interpret new last max demand lids.
        //  02/12/16 PGH 4.50.229 RTT556309    Added Average Aggregate Current and Temperature
        //  05/31/16 MP  4.50.275 WR687364     Changed some lid description formatting. Mostly spacing and
        //                                        capitalization.
        protected virtual void DetermineLIDInformation()
        {
            uint uiBaseComponent = (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK & m_lidValue;

            switch (uiBaseComponent)
            {
                case (uint)DefinedLIDs.BaseLIDs.BILL_SCHED_CONFIG:
                {
                    // The only LID for this Base LID is the Billing Schedule Name
                    m_lidType = TypeCode.String;
                    m_lidLength = 10;
                    m_lidDescription = "Custom Schedule";
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.COEFF_CONFIG:
                {
                    switch (m_lidValue)
                    {
                        case (uint)DefinedLIDs.Coefficients_Config.CT_MULT:
                        {
                            m_lidType = TypeCode.Single;
                            m_lidDescription = "CT Ratio";
                            break;
                        }
                        case (uint)DefinedLIDs.Coefficients_Config.VT_MULT:
                        {
                            m_lidType = TypeCode.Single;
                            m_lidDescription = "VT Ratio";
                            break;
                        }
                        case (uint)DefinedLIDs.Coefficients_Config.REG_MULT:
                        {
                            m_lidType = TypeCode.Single;
                            m_lidDescription = "Register Multiplier";
                            break;
                        }
                    }
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.COEFF_DATA:
                {
                    uint uiCoeffData = m_lidValue & (uint)DefinedLIDs.Coefficient_Data.COEFF_MASK;

                    switch (uiCoeffData)
                    {
                        case (uint)DefinedLIDs.Coefficient_Data.TRANSFORMER_MULT:
                        {
                            m_lidType = TypeCode.Single;
                            m_lidDescription = "Transformer Ratio";
                            break;
                        }
                        case (uint)DefinedLIDs.Coefficient_Data.METER_MULT:
                        {
                            m_lidType = TypeCode.Single;
                            m_lidDescription = "Meter Ratio";
                            break;
                        }
                    }

                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.CALENDAR_DATA:
                {
                    uint uiCALData = m_lidValue & (uint)DefinedLIDs.Calendar_Data.CLD_MASK;
                    switch (uiCALData)
                    {
                        case (uint)DefinedLIDs.Calendar_Data.CLD_HAS_CLOCK:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)DefinedLIDs.Calendar_Data.CLD_NUM_TOU_RATES:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)DefinedLIDs.Calendar_Data.CLD_DATE:
                        {
                            //This is the current date
                            m_lidType = TypeCode.DateTime;
                            m_lidDescription = "Current Date";
                            break;
                        }
                        case (uint)DefinedLIDs.Calendar_Data.CLD_TIME:
                        {
                            m_lidType = TypeCode.DateTime;
                            m_lidDescription = "Current Time";
                            break;
                        }
                        case (uint)DefinedLIDs.Calendar_Data.CLD_MIN_ON_BATTERY:
                        {
                            m_lidType = TypeCode.UInt32;
                            m_lidDescription = "Minutes on Battery";
                            break;
                        }
                        case (uint)DefinedLIDs.Calendar_Data.CLD_DAYS_ON_BATTERY:
                        {
                            m_lidType = TypeCode.UInt32;
                            break;
                        }
                        case (uint)DefinedLIDs.Calendar_Data.CLD_DAY_OF_WEEK:
                        {
                            m_lidType = TypeCode.Byte;
                            m_lidDescription = "Current Day of Week";
                            break;
                        }
                        case (uint)DefinedLIDs.Calendar_Data.CLD_DST_CONFIGURED:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)DefinedLIDs.Calendar_Data.CLD_IN_DST:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)DefinedLIDs.Calendar_Data.CLD_EXPIRE_DATE:
                        {
                            // This item has its own format and therfore must be handled
                            // differently than other LID values
                            m_lidType = TypeCode.DateTime;
                            m_lidDescription = "TOU Expiration Date";
                            break;
                        }
                    }
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.MODE_CNTRL_DATA:
                {
                    uint uiModeControlData = m_lidValue & (uint)DefinedLIDs.Mode_Cntrl_Data.MM_WHICH_ONE_MASK;
                    switch (uiModeControlData)
                    {
                        case (uint)DefinedLIDs.Mode_Cntrl_Data.MM_TIMEOUT_REMAIN:
                        {
                            m_lidType = TypeCode.DateTime;
                            m_lidDescription = "Time Remaining in Test Mode";
                            break;
                        }
                        case (uint)DefinedLIDs.Mode_Cntrl_Data.MM_METER_MODE:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                    }
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.CONSTANTS_DATA:
                {
                    uint uiConstData = m_lidValue & (uint)DefinedLIDs.Constant_Data.CONST_DATA_MASK;

                    switch (uiConstData)
                    {
                        case (uint)DefinedLIDs.Constant_Data.METER_ID_PART1:
                        {
                            m_lidType = TypeCode.String;
                            m_lidLength = 9;
                            m_lidDescription = "Meter ID";
                            break;
                        }
                        case (uint)DefinedLIDs.Constant_Data.METER_ID_PART2:
                        {
                            m_lidType = TypeCode.String;
                            m_lidLength = 9;
                            m_lidDescription = "Meter ID 2";
                            break;
                        }
                        case (uint)DefinedLIDs.Constant_Data.LOAD_RESRCH_PART1:
                        {
                            m_lidType = TypeCode.String;
                            m_lidLength = 8;
                            m_lidDescription = "Load Research ID";
                            break;
                        }
                        case (uint)DefinedLIDs.Constant_Data.LOAD_RESRCH_PART2:
                        {
                            m_lidType = TypeCode.String;
                            m_lidLength = 8;
                            m_lidDescription = "Load Research ID 2";
                            break;
                        }
                    }
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.DISPLAY_DATA:
                {
                    // The only item in this Base is the Display On Time
                    m_lidType = TypeCode.Single;
                    m_lidDescription = "Display On Time";
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.SELF_READ_DATA:
                {
                    uint uiSRType = m_lidValue & (uint)DefinedLIDs.SlfRd_Data.SR_FILE_MASK;

                    switch (uiSRType)
                    {
                        case (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_DATA:
                        {
                            uint uiSRBuffData = m_lidValue & (uint)DefinedLIDs.SR_Buffer_Data.BUFFER_DATA_MASK;

                            switch (uiSRBuffData)
                            {
                                case (uint)DefinedLIDs.SR_Buffer_Data.NUM_VALID_SELF_READS:
                                {
                                    m_lidType = TypeCode.Byte;
                                    break;
                                }
                            }

                            break;
                        }
                        default:
                        {
                            uint uiTimeData = m_lidValue & (uint)DefinedLIDs.ReadTimeData.SR_DATA_SEG_MASK;

                            // First determine the data types for the Self Read 
                            switch (uiTimeData)
                            {
                                case (uint)DefinedLIDs.ReadTimeData.SR_BILLING_DATA:
                                {
                                    uint uiRateData = m_lidValue & (uint)DefinedLIDs.TOU_Rate_Data.TOU_RATE_MASK;
                                    m_bIsQuantity = true;

                                    switch (uiRateData)
                                    {
                                        case (uint)DefinedLIDs.TOU_Rate_Data.TOU_ENERGY:
                                        {
                                            m_lidType = TypeCode.Double;
                                            break;
                                        }
                                        case (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND:
                                        {
                                            if ((m_lidValue & (uint)DefinedLIDs.WhichDemandFormat.WHICH_FORMAT_MASK) ==
                                                (uint)DefinedLIDs.WhichDemandFormat.TOO_DATA)
                                            {
                                                // It is a date and time
                                                m_lidType = TypeCode.DateTime;
                                            }
                                            else
                                            {
                                                uint uiDemandType = m_lidValue & (uint)DefinedLIDs.Demand_Data.DATA_SEG_MASK;

                                                // CUM and CCUM values are doubles so check for those values
                                                switch (uiDemandType)
                                                {
                                                    case (uint)DefinedLIDs.Demand_Data.CUM_DEMAND:
                                                    case (uint)DefinedLIDs.Demand_Data.CONT_CUM_DEMAND:
                                                    {
                                                        m_lidType = TypeCode.Double;
                                                        break;
                                                    }
                                                    default:
                                                    {
                                                        m_lidType = TypeCode.Single;
                                                        break;
                                                    }
                                                }
                                            }
                                            break;

                                        }
                                    }

                                    break;
                                }
                                case (uint)DefinedLIDs.ReadTimeData.SR_OTHER_DATA:
                                {
                                    uint uiOtherComponent = m_lidValue & (uint)DefinedLIDs.OtherSRData.SR_COMP_MASK;
                                    switch (uiOtherComponent)
                                    {
                                        case (uint)DefinedLIDs.OtherSRData.SR_MISC:
                                        {
                                            uint uiMiscSRComponent = m_lidValue & (uint)DefinedLIDs.WhichSRMisc.SR_MISC_MASK;

                                            switch (uiMiscSRComponent)
                                            {
                                                case (uint)DefinedLIDs.WhichSRMisc.MISC_SR_TIME:
                                                {
                                                    m_lidType = TypeCode.DateTime;
                                                    break;
                                                }
                                                case (uint)DefinedLIDs.WhichSRMisc.MISC_SR_VALID:
                                                {
                                                    m_lidType = TypeCode.Byte;
                                                    break;
                                                }
                                            }

                                            break;
                                        }
                                    }

                                    break;
                                }
                            }

                            break;
                        }
                    }

                    // Determine the description for the quantity
                    m_lidDescription = DetermineSRQuantity();
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.ENERGY_DATA:
                {
                    uint uiEnergyType = m_lidValue & (uint)DefinedLIDs.Energy_Data.ENERGY_DATA_MASK;

                    switch (uiEnergyType)
                    {
                        case (uint)DefinedLIDs.Energy_Data.ENERGY_MISC_DATA:
                        {
                            // The number of energies is the only misc energy LID
                            m_lidType = TypeCode.Byte;
                            m_lidDescription = "Number of Energies";

                            break;
                        }
                        case (uint)DefinedLIDs.Energy_Data.ENERGY_REG_DATA:
                        {
                            //This is an energy LID, which makes it a Double			
                            m_lidType = TypeCode.Double;
                            m_lidDescription = DetermineEnergy();
                            m_bIsQuantity = true;
                            break;
                        }
                    }

                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.DEMAND_DATA:
                {
                    uint uiDemandData = m_lidValue & (uint)DefinedLIDs.Demand_Data.DATA_SEG_MASK;

                    //This is an demand lid
                    switch (uiDemandData)
                    {
                        case (uint)DefinedLIDs.Demand_Data.MISC_DEMAND:
                        {
                            uint uiMiscDemand = m_lidValue & (uint)DefinedLIDs.MiscDemand.MISC_DEMAND_MASK;
                            m_bIsNonRegisterItem = true;

                            // We have a MISC demand so we need to set these up individually
                            switch (uiMiscDemand)
                            {
                                case (uint)DefinedLIDs.MiscDemand.COUNT_DEMAND_RES:
                                {
                                    m_lidType = TypeCode.UInt16;
                                    m_lidDescription = "Number of Demand Resets";
                                    break;
                                }
                                case (uint)DefinedLIDs.MiscDemand.TEST_SUB_INT_LEN:
                                {
                                    m_lidType = TypeCode.Byte;
                                    m_lidDescription = "Test Subinterval Length";
                                    break;
                                }
                                case (uint)DefinedLIDs.MiscDemand.SUB_INT_LEN:
                                {
                                    m_lidType = TypeCode.Byte;
                                    m_lidDescription = "Subinterval Length";
                                    break;
                                }
                                case (uint)DefinedLIDs.MiscDemand.DAYS_SINCE_DR:
                                {
                                    m_lidType = TypeCode.UInt16;
                                    m_lidDescription = "Days Since Demand Reset";
                                    break;
                                }
                                case (uint)DefinedLIDs.MiscDemand.TOO_TIME_REMAINING:
                                {
                                    m_lidType = TypeCode.UInt32;
                                    m_lidDescription = "Time Remaining in Demand Subinterval";
                                    break;
                                }
                                case (uint)DefinedLIDs.MiscDemand.PF_AVG_BILL_ARITH:
                                case (uint)DefinedLIDs.MiscDemand.PF_AVG_BILL_VECT:
                                {
                                    m_lidType = TypeCode.Single;
                                    m_lidDescription = "avg PF";
                                    break;
                                }
                                case (uint)DefinedLIDs.MiscDemand.TO_LAST_DEMAND_RES:
                                {
                                    m_lidType = TypeCode.DateTime;
                                    m_lidDescription = "Last Demand Reset Date and Time";
                                    break;
                                }
                                case (uint)DefinedLIDs.MiscDemand.NUM_DEMANDS:
                                {
                                    m_lidType = TypeCode.Byte;
                                    break;
                                }
                                case (uint)DefinedLIDs.MiscDemand.LAST_MAX_DMD1:
                                {
                                    m_lidType = TypeCode.Single;
                                    m_lidDescription = "Previous Max Demand Qty #1";
                                    break;
                                }
                                case (uint)DefinedLIDs.MiscDemand.LAST_MAX_DMD2:
                                {
                                    m_lidType = TypeCode.Single;
                                    m_lidDescription = "Previous Max Demand Qty #2";
                                    break;
                                }
                                case (uint)DefinedLIDs.MiscDemand.LAST_MAX_DMD3:
                                {
                                    m_lidType = TypeCode.Single;
                                    m_lidDescription = "Previous Max Demand Qty #3";
                                    break;
                                }
                            } // switch (uiMiscDemand)

                            break;
                        }
                        case (uint)DefinedLIDs.Demand_Data.COINC:
                        {
                            // All coincident values are Single
                            m_lidType = TypeCode.Single;
                            m_lidDescription = DetermineCoinc();
                            m_bIsQuantity = true;
                            break;
                        }
                        default:
                        {
                            // If it none of the things listed above, then hopefully it is a demand quantity

                            // Determine the type
                            if ((m_lidValue & (uint)DefinedLIDs.WhichDemandFormat.WHICH_FORMAT_MASK) ==
                                        (uint)DefinedLIDs.WhichDemandFormat.TOO_DATA)
                            {
                                // It is a date and time
                                m_lidType = TypeCode.DateTime;
                                m_bIsTOO = true;
                            }
                            else
                            {
                                uint uiDemandType = m_lidValue & (uint)DefinedLIDs.Demand_Data.DATA_SEG_MASK;
                                m_bIsQuantity = true;

                                // CUM and CCUM values are doubles so check for those values
                                switch (uiDemandType)
                                {
                                    case (uint)DefinedLIDs.Demand_Data.CUM_DEMAND:
                                    {
                                        m_lidType = TypeCode.Double;
                                        break;
                                    }
                                    case (uint)DefinedLIDs.Demand_Data.CONT_CUM_DEMAND:
                                    {
                                        m_lidType = TypeCode.Double;
                                        break;
                                    }
                                    default:
                                    {
                                        m_lidType = TypeCode.Single;
                                        break;
                                    }
                                }
                            }

                            // Now determine the description
                            m_lidDescription = DetermineDemand();

                            break;
                        }
                    } // switch (uiDemandData)

                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.DEMAND_CONFIG:
                {
                    uint uiConfigType = m_lidValue & (uint)DefinedLIDs.DemandConfigMasks.DMD_CONF_TYPE_MASK;
                    m_bIsNonRegisterItem = true;

                    switch (uiConfigType)
                    {
                        case (uint)DefinedLIDs.DemandConfig_Data.CONF_NBR_SUB:
                        {
                            m_lidType = TypeCode.Byte;
                            m_lidDescription = "Number of Subintervals";
                            break;
                        }
                        case (uint)DefinedLIDs.DemandConfig_Data.CONF_TST_NBR_SUB:
                        {
                            m_lidDescription = "Number of Test Subintervals";
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)DefinedLIDs.DemandConfig_Data.CONF_CLPU:
                        {
                            m_lidDescription = "CLPU Outage Time";
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)DefinedLIDs.DemandConfig_Data.CONF_REG_FS:
                        {
                            m_lidDescription = "Register Full-scale";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.DemandConfig_Data.CONF_THR_DEF:
                        {
                            DetermineThresholds();
                            break;
                        }
                    }
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.TOU_DATA:
                {
                    switch (m_lidValue & (uint)DefinedLIDs.TOU_Rate_Data.TOU_RATE_MASK)
                    {
                        case (uint)DefinedLIDs.TOU_Rate_Data.TOU_ENERGY:
                        {
                            m_lidType = TypeCode.Double;
                            m_bIsEnergy = true;
                            m_bIsQuantity = true;
                            break;
                        }
                        case (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND:
                        {
                            if ((m_lidValue & (uint)DefinedLIDs.WhichDemandFormat.WHICH_FORMAT_MASK) ==
                                (uint)DefinedLIDs.WhichDemandFormat.TOO_DATA)
                            {
                                // It is a date and time
                                m_lidType = TypeCode.DateTime;
                                m_bIsTOO = true;
                            }
                            else
                            {
                                uint uiDemandType = m_lidValue & (uint)DefinedLIDs.Demand_Data.DATA_SEG_MASK;
                                m_bIsQuantity = true;

                                // CUM and CCUM values are doubles so check for those values
                                switch (uiDemandType)
                                {
                                    case (uint)DefinedLIDs.Demand_Data.CUM_DEMAND:
                                    {
                                        m_lidType = TypeCode.Double;
                                        m_bIsCumDemand = true;
                                        break;
                                    }
                                    case (uint)DefinedLIDs.Demand_Data.CONT_CUM_DEMAND:
                                    {
                                        m_lidType = TypeCode.Double;
                                        m_bIsCCumDemand = true;
                                        break;
                                    }
                                    case (uint)DefinedLIDs.Demand_Data.MAX_DEMAND:
                                    {
                                        m_lidType = TypeCode.Single;
                                        m_bIsMaxDemand = true;
                                        break;
                                    }
                                    case (uint)DefinedLIDs.Demand_Data.MIN_DEMAND:
                                    {
                                        m_lidType = TypeCode.Single;
                                        m_bIsMinDemand = true;
                                        break;
                                    }
                                    case (uint)DefinedLIDs.Demand_Data.MAX_DMND_2ND:
                                    case (uint)DefinedLIDs.Demand_Data.MAX_DMND_3RD:
                                    case (uint)DefinedLIDs.Demand_Data.MAX_DMND_4TH:
                                    case (uint)DefinedLIDs.Demand_Data.MAX_DMND_5TH:
                                    {
                                        m_lidType = TypeCode.Single;
                                        m_bIsPeakDemand = true;
                                        break;
                                    }
                                    default:
                                    {
                                        m_lidType = TypeCode.Single;
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }

                    m_lidDescription = DetermineTOUQuantity();
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.MISC_DATA:
                {
                    m_bIsNonRegisterItem = true;

                    switch (m_lidValue & (uint)DefinedLIDs.Misc_Data.MISC_DATA_MASK)
                    {
                        case (uint)DefinedLIDs.Misc_Data.MISC_SEGMENT_TEST:
                        {
                            m_lidType = TypeCode.String;
                            m_lidLength = 6;
                            m_lidDescription = "Segment Test";
                            break;
                        }
                        case (uint)DefinedLIDs.Misc_Data.MISC_FW_VERS_REV:
                        {
                            m_lidType = TypeCode.Single;
                            m_lidDescription = "Firmware Version";
                            break;
                        }
                        case (uint)DefinedLIDs.Misc_Data.MISC_FW_BUILD:
                        {
                            m_lidType = TypeCode.Byte;
                            m_lidDescription = "Firmware Build";
                            break;
                        }
                        case (uint)DefinedLIDs.Misc_Data.MISC_LDR_VERSION:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)DefinedLIDs.Misc_Data.MISC_LDR_REVISION:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)DefinedLIDs.Misc_Data.MISC_LDR_VERS_REV:
                        {
                            m_lidType = TypeCode.Single;
                            m_lidDescription = "Loader Version";
                            break;
                        }
                        case (uint)DefinedLIDs.Misc_Data.MISC_ATM_VERS_REV:
                        {
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.Misc_Data.MISC_SW_VERS_REV:
                        {
                            m_lidType = TypeCode.Single;
                            m_lidDescription = "Software Version";
                            break;
                        }
                        case (uint)DefinedLIDs.Misc_Data.MISC_SW_REVISION:
                        {
                            m_lidType = TypeCode.Byte;
                            m_lidDescription = "Software Revision";
                            break;
                        }
                        case (uint)DefinedLIDs.Misc_Data.MISC_LAST_CONFIG_TIME:
                        {
                            m_lidType = TypeCode.DateTime;
                            m_lidDescription = "Last Program Date and Time";
                            break;
                        }
                        case (uint)DefinedLIDs.Misc_Data.MISC_OUTAGE_TIME:
                        {
                            m_lidType = TypeCode.DateTime;
                            m_lidDescription = "Last Outage Date and Time";
                            break;
                        }
                        case (uint)DefinedLIDs.Misc_Data.MISC_TEST_TIME:
                        {
                            m_lidType = TypeCode.DateTime;
                            m_lidDescription = "Last Test Date and Time";
                            break;
                        }
                        case (uint)DefinedLIDs.Misc_Data.MISC_INTERROGATION:
                        {
                            m_lidType = TypeCode.DateTime;
                            m_lidDescription = "Optical Port Last Interrogate Date and Time";
                            break;
                        }
                        case (uint)DefinedLIDs.Misc_Data.MISC_SEALED_CANADIAN:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                    }
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.SITE_SCAN_DATA:
                {
                    uint uiSiteScanData = m_lidValue & (uint)DefinedLIDs.SiteScan_Data.SS_DATA_MASK;

                    switch (uiSiteScanData)
                    {
                        case (uint)DefinedLIDs.SiteScan_Data.SS_SNAPSHOT_COUNT:
                        {
                            m_lidType = TypeCode.Byte;
                            m_lidDescription = "SiteScan Snapshot Count";
                            break;
                        }
                        case (uint)DefinedLIDs.SiteScan_Data.SS_SERV_TYPE_DISP:
                        {
                            m_lidType = TypeCode.String;
                            m_lidLength = 10;
                            m_lidDescription = "Service Type";
                            break;
                        }
                        case (uint)DefinedLIDs.SiteScan_Data.SS_SERVICE_TYPE:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)DefinedLIDs.SiteScan_Data.PH_A_VOLTAGE:
                        case (uint)DefinedLIDs.SiteScan_Data.PH_A_CURRENT:
                        case (uint)DefinedLIDs.SiteScan_Data.PH_A_CUR_ANGLE:
                        case (uint)DefinedLIDs.SiteScan_Data.PH_A_DC_DETECT:
                        case (uint)DefinedLIDs.SiteScan_Data.PH_B_VOLTAGE:
                        case (uint)DefinedLIDs.SiteScan_Data.PH_B_CURRENT:
                        case (uint)DefinedLIDs.SiteScan_Data.PH_B_CUR_ANGLE:
                        case (uint)DefinedLIDs.SiteScan_Data.PH_B_DC_DETECT:
                        case (uint)DefinedLIDs.SiteScan_Data.PH_C_VOLTAGE:
                        case (uint)DefinedLIDs.SiteScan_Data.PH_C_CURRENT:
                        case (uint)DefinedLIDs.SiteScan_Data.PH_C_CUR_ANGLE:
                        case (uint)DefinedLIDs.SiteScan_Data.PH_C_DC_DETECT:
                        {
                            m_lidType = TypeCode.Single;
                            break;
                        }
                    }
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.STATEMON_DATA:
                {
                    uint uiStateMonData = m_lidValue & (uint)DefinedLIDs.StateMon_STD.STATEMON_STD_MASK;
                    switch (uiStateMonData)
                    {
                        case (uint)DefinedLIDs.StateMon_STD.INV_TAMPER_COUNT:
                        {
                            m_lidType = TypeCode.Byte;
                            m_lidDescription = "Number of Inversion Tampers";
                            break;
                        }
                        case (uint)DefinedLIDs.StateMon_STD.REMOV_TAMPER_COUNT:
                        {
                            m_lidType = TypeCode.Byte;
                            m_lidDescription = "Number of Removal Tampers";
                            break;
                        }
                        case (uint)DefinedLIDs.StateMon_STD.OPT_LOGON_COUNT:
                        {
                            m_lidType = TypeCode.Byte;
                            m_lidDescription = "Optical Port Logon Count";
                            break;
                        }
                    }
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.METROLOGY_CONFIG:
                {
                    uint uiMetrologyConfig = m_lidValue & (uint)DefinedLIDs.Metrology_Config.CPC_CONF_TYPE_MASK;

                    switch (uiMetrologyConfig)
                    {
                        case (uint)DefinedLIDs.Metrology_Config.CPC_CONF_PUL_WGT_NORM:
                        {
                            m_lidType = TypeCode.UInt16;
                            m_lidDescription = "Normal Mode Kh";
                            break;
                        }
                        case (uint)DefinedLIDs.Metrology_Config.CPC_CONF_PUL_WGT_ALT:
                        {
                            m_lidType = TypeCode.UInt16;
                            m_lidDescription = "Normal Mode Kh #2";
                            break;
                        }
                        case (uint)DefinedLIDs.Metrology_Config.CPC_CONF_PUL_WGT_TEST:
                        {
                            m_lidType = TypeCode.UInt16;
                            m_lidDescription = "Test Mode Kh";
                            break;
                        }
                        case (uint)DefinedLIDs.Metrology_Config.CPC_CONF_PUL_WGT_TSTALT:
                        {
                            m_lidType = TypeCode.UInt16;
                            m_lidDescription = "Test Alt Mode Kh";
                            break;
                        }
                        case (uint)DefinedLIDs.Metrology_Config.CPC_CONF_PWR_CALC_METH:
                        {
                            m_lidType = TypeCode.Byte;
                            m_lidDescription = "VA Calculation Method";
                            break;
                        }
                        case (uint)DefinedLIDs.Metrology_Config.CPC_CONF_REAL_PW_NORM:
                        {
                            m_lidType = TypeCode.Single;
                            m_lidDescription = "Normal Mode Kh";
                            break;
                        }
                        case (uint)DefinedLIDs.Metrology_Config.CPC_CONF_REAL_PW_ALT:
                        {
                            m_lidType = TypeCode.Single;
                            m_lidDescription = "Normal Mode Kh #2";
                            break;
                        }
                        case (uint)DefinedLIDs.Metrology_Config.CPC_CONF_REAL_PW_TEST:
                        {
                            m_lidType = TypeCode.Single;
                            m_lidDescription = "Test Mode Kh";
                            break;
                        }
                        case (uint)DefinedLIDs.Metrology_Config.CPC_CONF_REAL_PW_TST_ALT:
                        {
                            m_lidType = TypeCode.Single;
                            m_lidDescription = "Test Alt Mode Kh";
                            break;
                        }
                    }

                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.METROLOGY_DATA:
                {
                    uint uiMetrologyData = m_lidValue & (uint)DefinedLIDs.MetrologyDataType.CPC_TYPE_MASK;
                    switch (uiMetrologyData)
                    {
                        case (uint)DefinedLIDs.MetrologyDataType.CPC_INST_TYPE:
                        {
                            switch (m_lidValue)
                            {
                                case (uint)DefinedLIDs.Temperature_Data.AVERAGE_AGGREGATE_CURRENT:
                                    {
                                        m_lidType = TypeCode.Int16;
                                        m_lidDescription = "Average Aggregate Current";
                                        break;
                                    }
                                default:
                                    {
                                        m_lidType = TypeCode.Single;
                                        m_lidDescription = DetermineMetrology();
                                        break;
                                    }
                            }
                            break;
                        }
                    }
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.TEMPERATURE_DATA:
                {
                    switch (m_lidValue)
                    {
                        case (uint)DefinedLIDs.Temperature_Data.TEMPERATURE:
                            {
                                m_lidType = TypeCode.Int16;
                                m_lidDescription = "Temperature (C)";
                                break;
                            }
                    }
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.CONSTANT_CONFIG:
                {
                    uint uiConstConfig = m_lidValue & (uint)DefinedLIDs.Constant_Config.CONSTANT_CONFIG_MASK;
                    switch (uiConstConfig)
                    {
                        case (uint)DefinedLIDs.Constant_Config.LOAD_RESRCH:
                        {
                            // The type for this item varies depending on the device
                            // The type will be set in the Device specific classes
                            m_lidDescription = "Load Research ID";
                            break;
                        }
                        case (uint)DefinedLIDs.Constant_Config.PROGRAM_ID:
                        {
                            m_lidType = TypeCode.UInt16;
                            m_lidDescription = "Program ID";
                            break;
                        }
                        case (uint)DefinedLIDs.Constant_Config.USER_DATA_1:
                        {
                            m_lidType = TypeCode.String;
                            m_lidLength = 10;
                            m_lidDescription = "User Data Field 1";
                            break;
                        }
                        case (uint)DefinedLIDs.Constant_Config.USER_DATA_2:
                        {
                            m_lidType = TypeCode.String;
                            m_lidLength = 10;
                            m_lidDescription = "User Data Field 2";
                            break;
                        }
                        case (uint)DefinedLIDs.Constant_Config.USER_DATA_3:
                        {
                            m_lidType = TypeCode.String;
                            m_lidLength = 10;
                            m_lidDescription = "User Data Field 3";
                            break;
                        }
                    }
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.OPTION_BRD_DATA:
                {
                    uint uiOptBrdData = m_lidValue & (uint)DefinedLIDs.Opt_Brd_Data.OPT_BRD_DATA_MASK;
                    switch (uiOptBrdData)
                    {
                        case (uint)DefinedLIDs.Opt_Brd_Data.OPT_BRD_ID:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)DefinedLIDs.Opt_Brd_Data.OPT_BRD_VEND_FLD_1:
                        {
                            m_lidType = TypeCode.String;
                            m_lidLength = 10;
                            m_lidDescription = "Option Board Field 1";
                            break;
                        }
                        case (uint)DefinedLIDs.Opt_Brd_Data.OPT_BRD_VEND_FLD_2:
                        {
                            m_lidType = TypeCode.String;
                            m_lidLength = 10;
                            m_lidDescription = "Option Board Field 2";
                            break;
                        }
                        case (uint)DefinedLIDs.Opt_Brd_Data.OPT_BRD_VEND_FLD_3:
                        {
                            m_lidType = TypeCode.String;
                            m_lidLength = 10;
                            m_lidDescription = "Option Board Field 3";
                            break;
                        }
                    }
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.MODE_CNTRL_CONFIG:
                {
                    uint uiModeControlConfig = m_lidValue & (uint)DefinedLIDs.Mode_Cntrl_Config.MM_CONFIG_MASK;

                    switch (uiModeControlConfig)
                    {
                        case (uint)DefinedLIDs.Mode_Cntrl_Config.MM_TIMEOUT_CONFIG:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                    }

                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_CONFIG:
                {
                    uint uiLPConfig = m_lidValue & (uint)DefinedLIDs.LP_Config.LP_CONF_MASK;

                    switch (uiLPConfig)
                    {
                        case (uint)DefinedLIDs.LP_Config.LP_INT_LEN:
                        case (uint)DefinedLIDs.LP_Config.LP_NUM_CHAN:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_1_QUAN:
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_2_QUAN:
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_3_QUAN:
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_4_QUAN:
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_5_QUAN:
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_6_QUAN:
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_7_QUAN:
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_8_QUAN:
                        {
                            m_lidType = TypeCode.UInt32;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_1_PW:
                        {
                            m_lidDescription += "Pulse Weight 1";
                            m_lidType = TypeCode.UInt16;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_2_PW:
                        {
                            m_lidDescription += "Pulse Weight 2";
                            m_lidType = TypeCode.UInt16;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_3_PW:
                        {
                            m_lidDescription += "Pulse Weight 3";
                            m_lidType = TypeCode.UInt16;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_4_PW:
                        {
                            m_lidDescription += "Pulse Weight 4";
                            m_lidType = TypeCode.UInt16;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_5_PW:
                        {
                            m_lidDescription += "Pulse Weight 5";
                            m_lidType = TypeCode.UInt16;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_6_PW:
                        {
                            m_lidDescription += "Pulse Weight 6";
                            m_lidType = TypeCode.UInt16;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_7_PW:
                        {
                            m_lidDescription += "Pulse Weight 7";
                            m_lidType = TypeCode.UInt16;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Config.LP_CHAN_8_PW:
                        {
                            m_lidDescription += "Pulse Weight 8";
                            m_lidType = TypeCode.UInt16;
                            break;
                        }
                    }

                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.LOAD_PROFILE_DATA:
                {
                    uint uiLPData = m_lidValue & (uint)DefinedLIDs.LP_Data.LP_MISC_MASK;
                    string strDesc = "Load Profile ";

                    switch (uiLPData)
                    {
                        case (uint)DefinedLIDs.LP_Data.LP_MISC_CH1_REAL_PW:
                        {
                            strDesc += "Pulse Weight 1";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Data.LP_MISC_CH2_REAL_PW:
                        {
                            strDesc += "Pulse Weight 2";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Data.LP_MISC_CH3_REAL_PW:
                        {
                            strDesc += "Pulse Weight 3";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Data.LP_MISC_CH4_REAL_PW:
                        {
                            strDesc += "Pulse Weight 4";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Data.LP_MISC_CH5_REAL_PW:
                        {
                            strDesc += "Pulse Weight 5";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Data.LP_MISC_CH6_REAL_PW:
                        {
                            strDesc += "Pulse Weight 6";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Data.LP_MISC_CH7_REAL_PW:
                        {
                            strDesc += "Pulse Weight 7";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Data.LP_MISC_CH8_REAL_PW:
                        {
                            strDesc += "Pulse Weight 8";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.LP_Data.LP_MISC_RUNNING:
                        {
                            m_lidType = TypeCode.Byte;
                            strDesc += "Running";
                            break;
                        }

                    }

                    m_lidDescription = strDesc;
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.PQ_DATA:
                {
                    uint uiPQData = m_lidValue & (uint)DefinedLIDs.VQ_Data.PQ_DATA_MASK;
                    switch (uiPQData)
                    {
                        case (uint)DefinedLIDs.VQ_Data.PQ_VQ_LOG_EVT_CNT:
                        {
                            m_lidDescription = "VQ Event Count";
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)DefinedLIDs.VQ_Data.PQ_THD_V_A:
                        {
                            m_lidDescription = "%THD V(a)";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.VQ_Data.PQ_THD_V_B:
                        {
                            m_lidDescription = "%THD V(b)";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.VQ_Data.PQ_THD_V_C:
                        {
                            m_lidDescription = "%THD V(c)";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.VQ_Data.PQ_TDD_I_A:
                        {
                            m_lidDescription = "%TDD I(a)";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.VQ_Data.PQ_TDD_I_B:
                        {
                            m_lidDescription = "%TDD I(b)";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)DefinedLIDs.VQ_Data.PQ_TDD_I_C:
                        {
                            m_lidDescription = "%TDD I(c)";
                            m_lidType = TypeCode.Single;
                            break;
                        }
                    }
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.METER_KEY_DATA:
                {
                    uint uiMeterKeyData = m_lidValue & (uint)DefinedLIDs.MeterKey_Data.METER_KEY_MASK;

                    switch (uiMeterKeyData)
                    {
                        case (uint)DefinedLIDs.MeterKey_Data.KEY_SOFT_REV:
                        case (uint)DefinedLIDs.MeterKey_Data.KEY_SOFT_VER:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)DefinedLIDs.MeterKey_Data.KEY_FLAVOR:
                        {
                            m_lidType = TypeCode.UInt16;
                            break;
                        }
                        default:
                        {
                            // All of the other items are UInt32
                            m_lidType = TypeCode.UInt32;
                            break;
                        }
                    }
                    break;
                }
                default:
                {
                    //KRC:TODO - Replace with real code after we finish debugging.
                    String strError = "We have missed a LID: " + m_lidValue.ToString(CultureInfo.InvariantCulture);
                    //MessageBox.Show(strError);
                    break;
                }
            }

            // Change the type back if the user has requested a different data type
            if (m_requestedTypeCode != TypeCode.Empty && m_requestedTypeCode != m_lidType)
            {
                m_lidType = m_requestedTypeCode;
            }
        }

        /// <summary>
        /// Determines the LID information for the Demand Threshold LIDs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/13/07 RCG 8.00.18 N/A	Created

        private void DetermineThresholds()
        {
            string strDesc = "Demand Threshold";
            uint uiThreshold = m_lidValue & (uint)DefinedLIDs.DemandConfigThresholds.CONF_MASK;

            switch (uiThreshold)
            {
                case (uint)DefinedLIDs.DemandConfigThresholds.CONF_THR1:
                {
                    // Add nothing in this case
                    break;
                }
                case (uint)DefinedLIDs.DemandConfigThresholds.CONF_THR2:
                {
                    // Add the number
                    strDesc += " 2";
                    break;
                }
                case (uint)DefinedLIDs.DemandConfigThresholds.CONF_THR3:
                {
                    strDesc += " 3";
                    break;
                }
                case (uint)DefinedLIDs.DemandConfigThresholds.CONF_THR4:
                {
                    strDesc += " 4";
                    break;
                }
            }

            // Determine the type
            uiThreshold = m_lidValue & (uint)DefinedLIDs.DemandConfigThreshold_Data.CONF_THR_MASK;

            switch (uiThreshold)
            {
                case (uint)DefinedLIDs.DemandConfigThreshold_Data.CONF_THR_SRC:
                {
                    strDesc += " LID";
                    m_lidType = TypeCode.UInt32;
                    break;
                }
                case (uint)DefinedLIDs.DemandConfigThreshold_Data.CONF_THR_LVL:
                {
                    m_lidType = TypeCode.Single;
                    break;
                }
            }

            m_lidDescription = strDesc;
        }

        /// <summary>
        /// This method determines the Coincident Demand Quantities
        /// </summary>
        /// <returns>Quantity description</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/22/07 RCG 8.00.08 N/A	Created
        //  04/29/16 jrf 4.50.258 668691 Removed Arith and Vec modifiers from VA quantity descriptions.
        protected virtual string DetermineCoinc()
        {
            string strQuantity = "";
            string strTOURate = "";
            uint uiTiggerType = m_lidValue & (uint)DefinedLIDs.CoincTriggers.TRIGGER_MASK;
            bool bIsEnergy;

            // Indicate that this is a coincident quantity.
            m_bIsCoincident = true;

            // Add the Coincident register name
            strQuantity += GetBaseMeasurement(false);

            // Add on the TOU rate
            strTOURate = GetTOURate(out bIsEnergy);
            strQuantity += strTOURate;

            // Add in the @ symbol
            strQuantity += " @ ";

            // Add the trigger register name
            switch (uiTiggerType)
            {
                case (uint)DefinedLIDs.CoincTriggers.WH_DEL:
                {
                    strQuantity += "max W d";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.WH_REC:
                {
                    strQuantity += "max W r";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.VAR_Q1:
                {
                    strQuantity += "max VAR Q1";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.VAR_Q2:
                {
                    strQuantity += "max VAR Q2";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.VAR_Q3:
                {
                    strQuantity += "max VAR Q3";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.VAR_Q4:
                {
                    strQuantity += "max VAR Q4";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.VAH_DEL_ARITH:
                {
                    strQuantity += "max VA d";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.VAH_REC_ARITH:
                {
                    strQuantity += "max VA r";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.VAH_DEL_VECT:
                {
                    strQuantity += "max VA d";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.VAH_REC_VECT:
                {
                    strQuantity += "max VA r";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.VAH_LAG:
                {
                    strQuantity += "max VA Lag";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.VAR_DEL:
                {
                    strQuantity += "max VAR d";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.VAR_REC:
                {
                    strQuantity += "max VAR r";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.WH_NET:
                {
                    strQuantity += "max W net";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.WH_UNI:
                {
                    strQuantity += "max W";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.VAR_NET:
                {
                    strQuantity += "max VAR net";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.PF_MIN_ARITH:
                {
                    strQuantity += "min PF";
                    break;
                }
                case (uint)DefinedLIDs.CoincTriggers.PF_MIN_VECT:
                {
                    strQuantity += "min PF";
                    break;
                }
                default:
                {
                    strQuantity = "";
                    break;
                }
            }

            return strQuantity;
        }

        /// <summary>
        /// This method determines the information for self read quantities
        /// </summary>
        /// <returns>Quantity description</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/09/06 mrj 7.30.00 N/A	Created
        //  09/26/06 KRC 7.35.00 N/A    Need support for Mega
        //  03/09/07 RCG 8.00.17 2623   Fixing issue where cum and ccum self read values
        //                              were not being read correctly.

        protected virtual string DetermineSRQuantity()
        {
            string strQuantity = "";
            string strType = "";
            uint uiSRDataType = (uint)DefinedLIDs.SlfRd_Data.SR_FILE_MASK & m_lidValue;

            // First determine the type of the read
            switch (uiSRDataType)
            {
                case (uint)DefinedLIDs.SlfRd_Data.SR_LAST_DR:
                {
                    //This is a snapshot 1 quantity
                    strType = " (SS1)";
                    m_bIsSnapShot = true;
                    break;
                }
                case (uint)DefinedLIDs.SlfRd_Data.SR_2ND_LAST_DR:
                {
                    strType = " (SS2)";
                    m_bIsSnapShot = true;
                    break;
                }
                case (uint)DefinedLIDs.SlfRd_Data.SR_LAST_SEASON:
                {
                    strType = " (LSSR)";
                    m_bIsLastSeason = true;
                    break;
                }
                case (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_1:
                {
                    //This is a self read 1 quantity
                    strType = " (SR1)";
                    m_bIsSelfRead = true;
                    break;
                }
                case (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_2:
                {
                    //This is a self read 2 quantity
                    strType = " (SR2)";
                    m_bIsSelfRead = true;
                    break;
                }
                case (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_3:
                {
                    //This is a self read 3 quantity
                    strType = " (SR3)";
                    m_bIsSelfRead = true;
                    break;
                }
                case (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_4:
                {
                    //This is a self read 4 quantity
                    strType = " (SR4)";
                    m_bIsSelfRead = true;
                    break;
                }

                case (uint)DefinedLIDs.SlfRd_Data.SR_BUFF_DATA:
                default:
                {
                    strType = "";
                    m_bIsSelfRead = true;
                    break;
                }
            }//switch

            // Now determine the base quantity
            if ("" != strType)
            {
                uint uiTimeData = m_lidValue & (uint)DefinedLIDs.ReadTimeData.SR_DATA_SEG_MASK;

                switch (uiTimeData)
                {
                    case (uint)DefinedLIDs.ReadTimeData.SR_BILLING_DATA:
                    {
                        uint uiRateData = m_lidValue & (uint)DefinedLIDs.TOU_Rate_Data.TOU_RATE_MASK;

                        switch (uiRateData)
                        {
                            case (uint)DefinedLIDs.TOU_Rate_Data.TOU_ENERGY:
                            {
                                m_lidType = TypeCode.Double;
                                break;
                            }
                            case (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND:
                            {
                                if ((m_lidValue & (uint)DefinedLIDs.WhichDemandFormat.WHICH_FORMAT_MASK) ==
                                    (uint)DefinedLIDs.WhichDemandFormat.TOO_DATA)
                                {
                                    // It is a date and time
                                    m_lidType = TypeCode.DateTime;
                                }
                                else
                                {
                                    uint uiDemandType = m_lidValue & (uint)DefinedLIDs.Demand_Data.DATA_SEG_MASK;

                                    // CUM and CCUM values are doubles so check for those values
                                    switch (uiDemandType)
                                    {
                                        case (uint)DefinedLIDs.Demand_Data.CUM_DEMAND:
                                        case (uint)DefinedLIDs.Demand_Data.CONT_CUM_DEMAND:
                                        {
                                            m_lidType = TypeCode.Double;
                                            break;
                                        }
                                        default:
                                        {
                                            m_lidType = TypeCode.Single;
                                            break;
                                        }
                                    }
                                }
                                break;

                            }
                        }

                        //Now get the base quantity and the TOU annunciator
                        strQuantity = DetermineTOUQuantity();

                        break;
                    }
                    case (uint)DefinedLIDs.ReadTimeData.SR_OTHER_DATA:
                    {
                        uint uiOtherComponent = m_lidValue & (uint)DefinedLIDs.OtherSRData.SR_COMP_MASK;
                        switch (uiOtherComponent)
                        {
                            case (uint)DefinedLIDs.OtherSRData.SR_MISC:
                            {
                                uint uiMiscSRComponent = m_lidValue & (uint)DefinedLIDs.WhichSRMisc.SR_MISC_MASK;

                                switch (uiMiscSRComponent)
                                {
                                    case (uint)DefinedLIDs.WhichSRMisc.MISC_SR_TIME:
                                    {
                                        m_lidType = TypeCode.DateTime;
                                        strQuantity = "Self Read Date and Time";

                                        break;
                                    }
                                }// switch (uiMiscSRComponent)

                                break;
                            }
                        } // switch (uiOtherComponent)

                        break;
                    }
                } // switch(uiTimeData)
            }

            if (strQuantity != "")
            {
                //Construct the full quantity
                strQuantity = strQuantity + strType;
            }
            else
            {
                m_lidDescription = "";
            }

            return strQuantity;
        }

        /// <summary>
        /// This method determines the information for TOU quantities
        /// </summary>
        /// <returns>Quantity description</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/09/06 mrj 7.30.00 N/A	Created
        //  09/25/06 KRC 7.35.00 N/A    Need to support Mega
        protected virtual string DetermineTOUQuantity()
        {
            string strTOURate = "";
            string strQuantity = "";
            bool bEnergy;

            //Get the TOU rate
            strTOURate = GetTOURate(out bEnergy);

            if (bEnergy)
            {
                //Get the engery quantity
                strQuantity = DetermineEnergy();
            }
            else
            {
                //Get the demand quantity
                uint uiDemandType = m_lidValue & (uint)DefinedLIDs.Demand_Data.DATA_SEG_MASK;

                if (uiDemandType == (uint)DefinedLIDs.Demand_Data.COINC)
                {
                    strQuantity = DetermineCoinc();
                }
                else if (uiDemandType == (uint)DefinedLIDs.Demand_Data.MISC_DEMAND)
                {
                    uint uiMiscDemand = m_lidValue & (uint)DefinedLIDs.MiscDemand.MISC_DEMAND_MASK;

                    switch (uiMiscDemand)
                    {
                        case (uint)DefinedLIDs.MiscDemand.PF_AVG_BILL_ARITH:
                        case (uint)DefinedLIDs.MiscDemand.PF_AVG_BILL_VECT:
                        {
                            strQuantity = "avg PF";
                            break;
                        }
                    }
                }
                else
                {
                    strQuantity = DetermineDemand();
                }
            }

            if (strQuantity != "")
            {
                //Construct the full quantity
                strQuantity = strQuantity + strTOURate;
            }

            return strQuantity;
        }

        /// <summary>
        /// This method determines the TOU rate
        /// </summary>
        /// <returns>TOU rate descriptor</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/09/06 mrj 7.30.00 N/A	Created
        // 
        protected virtual string GetTOURate(out bool bEnergy)
        {
            string strRate = "";
            uint uiRate = (uint)DefinedLIDs.TOU_Data.TOU_DATA_MASK & m_lidValue;
            m_bIsTOURate = true;    // Assume it is one of the TOU Rates unless it falls into Total

            switch (uiRate)
            {
                case (uint)DefinedLIDs.TOU_Data.RATE_A:
                {
                    strRate = " Rate A";
                    break;
                }
                case (uint)DefinedLIDs.TOU_Data.RATE_B:
                {
                    strRate = " Rate B";
                    break;
                }
                case (uint)DefinedLIDs.TOU_Data.RATE_C:
                {
                    strRate = " Rate C";
                    break;
                }
                case (uint)DefinedLIDs.TOU_Data.RATE_D:
                {
                    strRate = " Rate D";
                    break;
                }
                case (uint)DefinedLIDs.TOU_Data.RATE_E:
                {
                    strRate = " Rate E";
                    break;
                }
                case (uint)DefinedLIDs.TOU_Data.RATE_F:
                {
                    strRate = " Rate F";
                    break;
                }
                case (uint)DefinedLIDs.TOU_Data.RATE_G:
                {
                    strRate = " Rate G";
                    break;
                }
                case (uint)DefinedLIDs.TOU_Data.TOTAL:
                default:
                {
                    strRate = "";
                    m_bIsTOURate = false;
                    break;
                }
            }

            if ((uint)DefinedLIDs.TOU_Rate_Data.TOU_ENERGY == ((uint)DefinedLIDs.TOU_Rate_Data.TOU_RATE_MASK & m_lidValue))
            {
                bEnergy = true;
            }
            else
            {
                bEnergy = false;
            }

            return strRate;
        }

        /// <summary>
        /// This method determines the information for energy quantities
        /// </summary>
        /// <returns>Demand quantity descriptor</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/09/06 mrj 7.30.00 N/A	Created
        //  09/26/06 KRC 7.35.00 N/A    Need to support Mega
        //  
        protected virtual string DetermineEnergy()
        {
            string strQuantity = "";
            uint uiEnergyData = (uint)DefinedLIDs.Energy_Data.ENERGY_DATA_MASK & m_lidValue;
            uint uiEnergyFormat = (uint)DefinedLIDs.WhichEnergyFormat.WHICH_FORMAT_MASK & m_lidValue;

            if ((uint)DefinedLIDs.Energy_Data.ENERGY_REG_DATA == uiEnergyData)
            {
                m_bIsEnergy = true;

                if ((uint)DefinedLIDs.WhichEnergyFormat.SECONDARY_DATA == uiEnergyFormat)
                {
                    //Only convert secondary quantities											
                    strQuantity = GetBaseMeasurement(true);
                }
                else if ((uint)DefinedLIDs.WhichEnergyFormat.PRIMARY_DATA == uiEnergyFormat)
                {
                    strQuantity = GetBaseMeasurement(true);
                    m_bIsPrimary = true;
                }
                else
                {
                    //This is a raw quantitiy
                    strQuantity = "";

                    //Currently we only care about raw quantities when they are unassigned in the R300 configuration
                    if (DefinedLIDs.WhichOneEnergyDemand.NOT_PROGRAMMED == (DefinedLIDs.WhichOneEnergyDemand)((uint)DefinedLIDs.WhichOneEnergyDemand.WHICH_ONE_MASK & m_lidValue))
                    {

                        strQuantity = "Unassigned";
                    }

                }
            }
            else if ((uint)DefinedLIDs.Energy_Data.ENERGY_MISC_DATA == uiEnergyData)
            {
                //This is misc energy
                strQuantity = "";
            }

            return strQuantity;
        }

        /// <summary>
        /// This method determines the information for demand quantities
        /// </summary>
        /// <returns>Demand quantity descriptor</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/09/06 mrj 7.30.00 N/A	Created
        //  09/26/06 KRC 7.35.00 N/A    Need support for Mega
        //  
        protected virtual string DetermineDemand()
        {
            string strQuantity = "";
            uint uiDemandData = (uint)DefinedLIDs.Demand_Data.DATA_SEG_MASK & m_lidValue;
            uint uiDemandFormat = (uint)DefinedLIDs.WhichDemandFormat.WHICH_FORMAT_MASK & m_lidValue;

            if (uiDemandFormat == (uint)DefinedLIDs.WhichDemandFormat.TOO_DATA)
            {
                // Add the Data and Time portion
                strQuantity += "Date and Time of ";
                m_bIsTOO = true;
            }

            switch (uiDemandData)
            {
                case (uint)DefinedLIDs.Demand_Data.PRES_DEMAND:
                {
                    strQuantity += "prs ";
                    m_bIsPresent = true;
                    break;
                }
                case (uint)DefinedLIDs.Demand_Data.MAX_DEMAND:
                {
                    strQuantity += "max ";
                    m_bIsMaxDemand = true;
                    break;
                }
                case (uint)DefinedLIDs.Demand_Data.PREV_DEMAND:
                {
                    strQuantity += "prv ";
                    m_bIsPrevious = true;
                    break;
                }
                case (uint)DefinedLIDs.Demand_Data.PROJ_DEMAND:
                {
                    strQuantity += "proj ";
                    m_bIsProjected = true;
                    break;

                }
                case (uint)DefinedLIDs.Demand_Data.CUM_DEMAND:
                {
                    strQuantity += "cum ";
                    m_bIsCumDemand = true;
                    break;
                }
                case (uint)DefinedLIDs.Demand_Data.CONT_CUM_DEMAND:
                {
                    strQuantity += "ccum ";
                    m_bIsCCumDemand = true;
                    break;
                }
                case (uint)DefinedLIDs.Demand_Data.MIN_DEMAND:
                {
                    strQuantity += "min ";
                    m_bIsMinDemand = true;
                    break;
                }
                case (uint)DefinedLIDs.Demand_Data.MAX_DMND_2ND:
                {
                    strQuantity += "max 2 ";
                    m_bIsPeakDemand = true;
                    break;
                }
                case (uint)DefinedLIDs.Demand_Data.MAX_DMND_3RD:
                {
                    strQuantity += "max 3 ";
                    m_bIsPeakDemand = true;
                    break;
                }

                case (uint)DefinedLIDs.Demand_Data.MAX_DMND_4TH:
                {
                    strQuantity += "max 4 ";
                    m_bIsPeakDemand = true;
                    break;
                }
                case (uint)DefinedLIDs.Demand_Data.MAX_DMND_5TH:
                {
                    strQuantity += "max 5 ";
                    m_bIsPeakDemand = true;
                    break;
                }
                case (uint)DefinedLIDs.Demand_Data.COINC:
                {
                    strQuantity = "";
                    m_bIsCoincident = true;
                    break;
                }
                case (uint)DefinedLIDs.Demand_Data.MISC_DEMAND:
                case (uint)DefinedLIDs.Demand_Data.ALL_ACTIVE_DMND:
                default:
                {
                    strQuantity = "";
                    break;
                }
            }

            if ("" != strQuantity)
            {
                //We have the start of a quantity, now find the base quantity type
                if ((uint)DefinedLIDs.WhichDemandFormat.SECONDARY_DATA == uiDemandFormat ||
                    (uint)DefinedLIDs.WhichDemandFormat.TOO_DATA == uiDemandFormat ||
                    (uint)DefinedLIDs.WhichDemandFormat.PRIMARY_DATA == uiDemandFormat)
                {
                    string strBase = GetBaseMeasurement(false);

                    if (0 == String.Compare(strBase, "Unassigned", StringComparison.CurrentCulture))
                    {
                        // If this is Unassigned then don't append the current modifier
                        strQuantity = strBase;
                    }
                    else if ("" != strBase)
                    {
                        strQuantity = strQuantity + strBase;
                    }
                    else
                    {
                        //We could not find the base quantity so return blank
                        strQuantity = "";
                    }

                    // Mark Primary values so we can easily find them.
                    if ((uint)DefinedLIDs.WhichDemandFormat.PRIMARY_DATA == uiDemandFormat)
                    {
                        m_bIsPrimary = true;
                    }
                }
                else
                {
                    //This is a raw quantitiy
                    strQuantity = "";
                }
            }

            return strQuantity;
        }

        /// <summary>
        /// This method determines the information for the base energy or demand quantity.
        /// </summary>
        /// <param name="bEnergy">Flag indicating that this is an energy 
        /// LID</param>
        /// <returns>The base quantity descriptor (energy or demand)</returns>		
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/09/06 mrj 7.30.00 N/A	Created
        //  09/26/06 KRC 7.35.00 N/A    Need support for Mega
        //  12/01/06 jrf 8.00.00 N/A    Adding support for additional quantities
        //  12/15/11 RCG 2.53.20		Adding support for new Lithium LIDs
        //  04/29/16 jrf 4.50.258 668691 Removed Arith and Vec modifiers from VA quantity descriptions.
        protected virtual string GetBaseMeasurement(bool bEnergy)
        {
            string strQuantity = "";
            uint uiQuantityType = (uint)DefinedLIDs.WhichOneEnergyDemand.WHICH_ONE_MASK & m_lidValue;

            switch (uiQuantityType)
            {

                case (uint)DefinedLIDs.WhichOneEnergyDemand.TOTALIZER1:
                case (uint)DefinedLIDs.WhichOneEnergyDemand.TOTALIZER2:
                case (uint)DefinedLIDs.WhichOneEnergyDemand.TOTALIZER3:
                case (uint)DefinedLIDs.WhichOneEnergyDemand.PULSE_INPUT_1:
                case (uint)DefinedLIDs.WhichOneEnergyDemand.PULSE_INPUT_2:
                {
                    // Since these items are not really quantities we do not
                    // want to add the measurement unit to the description
                    break;
                }
                default:
                {
                    // Add the measurement unit to the quantity
                    if (LID.MeasurementUnit.KILO == m_lidMeasUnit)
                    {
                        strQuantity = "k";
                    }
                    else if (LID.MeasurementUnit.MEGA == m_lidMeasUnit)
                    {
                        strQuantity = "M";
                    }

                    break;
                }
            }

            switch (uiQuantityType)
            {
                case (uint)DefinedLIDs.WhichOneEnergyDemand.NOT_PROGRAMMED:
                {
                    strQuantity = "Unassigned";
                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.WH_DELIVERED:
                {
                    if (bEnergy)
                        strQuantity += "Wh d";
                    else
                        strQuantity += "W d";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.WH_RECEIVED:
                {
                    if (bEnergy)
                        strQuantity += "Wh r";
                    else
                        strQuantity += "W r";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_ARITH:
                {
                    if (bEnergy)
                        strQuantity += "VAh d";
                    else
                        strQuantity += "VA d";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VAH_REC_ARITH:
                {
                    if (bEnergy)
                        strQuantity += "VAh r";
                    else
                        strQuantity += "VA r";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_VECT:
                {
                    if (bEnergy)
                        strQuantity += "VAh d";
                    else
                        strQuantity += "VA d";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VAH_REC_VECT:
                {
                    if (bEnergy)
                        strQuantity += "VAh r";
                    else
                        strQuantity += "VA r";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VAH_LAG:
                {
                    if (bEnergy)
                        strQuantity += "VAh Lag";
                    else
                        strQuantity += "VA Lag";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VARH_DEL:
                {
                    if (bEnergy)
                        strQuantity += "VARh d";
                    else
                        strQuantity += "VAR d";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VARH_REC:
                {
                    if (bEnergy)
                        strQuantity += "VARh r";
                    else
                        strQuantity += "VAR r";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.WH_NET:
                {
                    if (bEnergy)
                        strQuantity += "Wh net";
                    else
                        strQuantity += "W net";

                    // Net values can be negative
                    m_bIsNegativeAllowed = true;

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.WH_NET_PHA:
                {
                    if (bEnergy)
                        strQuantity += "Wh net(a)";
                    else
                        strQuantity += "W net(a)";

                    // Net values can be negative
                    m_bIsNegativeAllowed = true;

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.WH_NET_PHB:
                {
                    if (bEnergy)
                        strQuantity += "Wh net(b)";
                    else
                        strQuantity += "W net(b)";

                    // Net values can be negative
                    m_bIsNegativeAllowed = true;

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.WH_NET_PHC:
                {
                    if (bEnergy)
                        strQuantity += "Wh net(c)";
                    else
                        strQuantity += "W net(c)";

                    // Net values can be negative
                    m_bIsNegativeAllowed = true;

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.WH_UNI:
                {
                    if (bEnergy)
                        strQuantity += "Wh uni";
                    else
                        strQuantity += "W uni";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VARH_Q1:
                {
                    if (bEnergy)
                        strQuantity += "VARh Q1";
                    else
                        strQuantity += "VAR Q1";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VARH_Q2:
                {
                    if (bEnergy)
                        strQuantity += "VARh Q2";
                    else
                        strQuantity += "VAR Q2";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VARH_Q3:
                {
                    if (bEnergy)
                        strQuantity += "VARh Q3";
                    else
                        strQuantity += "VAR Q3";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VARH_Q4:
                {
                    if (bEnergy)
                        strQuantity += "VARh Q4";
                    else
                        strQuantity += "VAR Q4";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VARH_NET_DEL:
                {
                    if (bEnergy)
                        strQuantity += "VARh net d";
                    else
                        strQuantity += "VAR net d";

                    // Net values can be negative
                    m_bIsNegativeAllowed = true;

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VARH_NET_REC:
                {
                    if (bEnergy)
                        strQuantity += "VARh net r";
                    else
                        strQuantity += "VAR net r";

                    // Net values can be negative
                    m_bIsNegativeAllowed = true;

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.QH_DEL:
                {
                    if (bEnergy)
                        strQuantity += "Qh d";
                    else
                        strQuantity += "Q d";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VH_PHA:
                {
                    if (bEnergy)
                        strQuantity += "Vh(a)";
                    else
                        strQuantity += "V(a)";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VH_PHB:
                {
                    if (bEnergy)
                        strQuantity += "Vh(b)";
                    else
                        strQuantity += "V(b)";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VH_PHC:
                {
                    if (bEnergy)
                        strQuantity += "Vh(c)";
                    else
                        strQuantity += "V(c)";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VH_AVG:
                {
                    if (bEnergy)
                        strQuantity += "Vh avg";
                    else
                        strQuantity += "V avg";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.AH_PHA:
                {
                    if (bEnergy)
                        strQuantity += "Ah(a)";
                    else
                        strQuantity += "A(a)";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.AH_PHB:
                {
                    if (bEnergy)
                        strQuantity += "Ah(b)";
                    else
                        strQuantity += "A(b)";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.AH_PHC:
                {
                    if (bEnergy)
                        strQuantity += "Ah(c)";
                    else
                        strQuantity += "A(c)";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.AH_NEUTRAL:
                {
                    if (bEnergy)
                        strQuantity += "NAh";
                    else
                        strQuantity += "NA";
                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.V2H_AGG:
                {
                    if (bEnergy)
                        strQuantity += "V^2h";
                    else
                        strQuantity += "V^2";
                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.I2H_AGG:
                {
                    if (bEnergy)
                        strQuantity += "A^2h";
                    else
                        strQuantity += "A^2";
                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.PULSE_INPUT_1:
                {
                    if (bEnergy)
                        strQuantity += "Analog Input #1";
                    else
                        strQuantity += "Analog Input #1";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.PULSE_INPUT_2:
                {
                    if (bEnergy)
                        strQuantity += "Analog Input #2";
                    else
                        strQuantity += "Analog Input #2";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.VARH_NET:
                {
                    if (bEnergy)
                        strQuantity += "VARh net";
                    else
                        strQuantity += "VAR net";

                    // Net values can be negative
                    m_bIsNegativeAllowed = true;

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.QH_REC:
                {
                    if (bEnergy)
                        strQuantity += "Qh r";
                    else
                        strQuantity += "Q r";

                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.TOTALIZER1:
                {
                    if (bEnergy)
                        strQuantity += "Totalized-h #1";
                    else
                        strQuantity += "Totalized #1";
                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.TOTALIZER2:
                {
                    if (bEnergy)
                        strQuantity += "Totalized-h #2";
                    else
                        strQuantity += "Totalized #2";
                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.TOTALIZER3:
                {
                    if (bEnergy)
                        strQuantity += "Totalized-h #3";
                    else
                        strQuantity += "Totalized #3";
                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.PF_INTERVAL_ARITH:
                {
                    strQuantity += "PF";
                    break;
                }
                case (uint)DefinedLIDs.WhichOneEnergyDemand.PF_INTERVAL_VECT:
                {
                    strQuantity += "PF";
                    break;
                }
                default:
                {
                    strQuantity = "";
                    break;
                }
            }

            return strQuantity;
        }

        /// <summary>
        /// DetermineMetrology - Handle the Metrology Quantities
        /// </summary>
        /// <returns></returns>
        //  Revision History
        //  MM/DD/YY Who Version    Issue#     Description
        //  -------- --- -------    ------     ---------------------------------------------
        //  10/09/06 KRC 7.36.00    N/A        Created
        //  10/25/06 AF  7.40.00    N/A        Added code to determine format and added
        //                                        strings to the resource file
        //  03/13/07 RCG 8.00.18    2586       Fixing issue where freq and NA were displaying a phase
        //  03/26/07 MAH 8.00.21    2661       Changed quantity description from Var to var
        //  04/29/16 jrf 4.50.258   668691     Removed Arith and Vec modifiers from VA quantity descriptions.
        //  06/02/16 MP  4.50.276   WR690954   Changed "Ins " to "ins "
        protected virtual string DetermineMetrology()
        {
            string strQuantity = "";
            uint uiMetType = m_lidValue & (uint)DefinedLIDs.MetrologyDataType.CPC_TYPE_MASK;
            uint uiPhase = m_lidValue & (uint)DefinedLIDs.MetrologyDataPhase.CPC_PHASE_MASK;
            bool bUsesPhase = true;

            // Determine if the value is instantaneous
            switch (uiMetType)
            {
                case (uint)DefinedLIDs.MetrologyDataType.CPC_INST_TYPE:
                {
                    strQuantity += "ins ";
                    m_bIsInstantaneous = true;
                    break;
                }
                default:
                {
                    break;
                }
            }

            // Add the measurement unit to the quantity
            if (LID.MeasurementUnit.KILO == m_lidMeasUnit)
            {
                strQuantity += "k";
            }
            else if (LID.MeasurementUnit.MEGA == m_lidMeasUnit)
            {
                strQuantity += "M";
            }

            if (m_bIsInstantaneous == true)
            {
                // Determine the Measurement Type
                uint uiIns = m_lidValue & (uint)DefinedLIDs.MetrologyDataIns.CPC_INST_MASK;

                switch (uiIns)
                {
                    case (uint)DefinedLIDs.MetrologyDataIns.CPC_INST_VRMS:
                    {
                        strQuantity += "V";
                        break;
                    }
                    case (uint)DefinedLIDs.MetrologyDataIns.CPC_INST_IRMS:
                    {
                        strQuantity += "A";
                        break;
                    }
                    case (uint)DefinedLIDs.MetrologyDataIns.CPC_INST_FREQ:
                    {
                        strQuantity += "Hz";

                        // The LID for frequency does not use phases so set it not
                        // to add the phase description
                        bUsesPhase = false;
                        break;
                    }
                    case (uint)DefinedLIDs.MetrologyDataIns.CPC_INST_PF:
                    {
                        strQuantity += "PF";
                        break;
                    }
                    case (uint)DefinedLIDs.MetrologyDataIns.CPC_INST_NEURMS:
                    {
                        strQuantity += "NA";

                        // The LID for NA does not use phases so set it not
                        // to add the phase description
                        bUsesPhase = false;
                        break;
                    }
                    case (uint)DefinedLIDs.MetrologyDataIns.CPC_INST_W:
                    {
                        strQuantity += "W";
                        break;
                    }
                    case (uint)DefinedLIDs.MetrologyDataIns.CPC_INST_VAR:
                    {
                        strQuantity += "VAR";
                        break;
                    }
                    case (uint)DefinedLIDs.MetrologyDataIns.CPC_INST_VA:
                    {
                        strQuantity += "VA";
                        break;
                    }
                }
            }

            if (bUsesPhase == true)
            {
                // Add the phase
                switch (uiPhase)
                {
                    case (uint)DefinedLIDs.MetrologyDataPhase.CPC_PHASE_A:
                    {
                        strQuantity += "(a)";
                        break;
                    }
                    case (uint)DefinedLIDs.MetrologyDataPhase.CPC_PHASE_B:
                    {
                        strQuantity += "(b)";
                        break;
                    }
                    case (uint)DefinedLIDs.MetrologyDataPhase.CPC_PHASE_C:
                    {
                        strQuantity += "(c)";
                        break;
                    }
                }
            }

            return strQuantity;
        }
        #endregion

        #region Members

        /// <summary>
        /// The value of the LID.
        /// </summary>
        protected uint m_lidValue;
        /// <summary>
        /// The type of LID.
        /// </summary>
        protected TypeCode m_lidType;
        /// <summary>
        /// The Type that was requested
        /// </summary>
        protected TypeCode m_requestedTypeCode;
        /// <summary>
        /// The length of the LID.
        /// </summary>
        protected uint m_lidLength;
        /// <summary>
        /// The unit of measure used by the LID.
        /// </summary>
        protected MeasurementUnit m_lidMeasUnit;
        /// <summary>
        /// The description of the LID.
        /// </summary>
        protected string m_lidDescription;

        // LID Properties
        /// <summary>
        /// Bool indicating whether or not the LID is an energy value.
        /// </summary>
        protected bool m_bIsEnergy = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a maximum demand value.
        /// </summary>
        protected bool m_bIsMaxDemand = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a minimum demand value.
        /// </summary>
        protected bool m_bIsMinDemand = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a peak demand value.
        /// </summary>
        protected bool m_bIsPeakDemand = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a cumulative demand value.
        /// </summary>
        protected bool m_bIsCumDemand = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a continuous cumulative demand value.
        /// </summary>
        protected bool m_bIsCCumDemand = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a non-register item.
        /// </summary>
        protected bool m_bIsNonRegisterItem = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a time of occurence value.
        /// </summary>
        protected bool m_bIsTOO = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a coincident value.
        /// </summary>
        protected bool m_bIsCoincident = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a present value.
        /// </summary>
        protected bool m_bIsPresent = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a previous value.
        /// </summary>
        protected bool m_bIsPrevious = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a projected value.
        /// </summary>
        protected bool m_bIsProjected = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a TOU rate.
        /// </summary>
        protected bool m_bIsTOURate = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a primary value.
        /// </summary>
        protected bool m_bIsPrimary = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a self read value.
        /// </summary>
        protected bool m_bIsSelfRead = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a snapshot value.
        /// </summary>
        protected bool m_bIsSnapShot = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a last season value.
        /// </summary>
        protected bool m_bIsLastSeason = false;
        /// <summary>
        /// Bool indicating whether or not the LID is a negative allowed value.
        /// </summary>
        protected bool m_bIsNegativeAllowed = false;
        /// <summary>
        /// Bool indicating whether or not the LID is an energy or demand quantity.
        /// </summary>
        protected bool m_bIsQuantity = false;
        /// <summary>
        /// Bool indicating whether or not the LID is an instantaneous value.
        /// </summary>
        protected bool m_bIsInstantaneous = false;

        /// <summary>
        /// A resource manager to retrieve strings.
        /// </summary>
        protected System.Resources.ResourceManager m_rmStrings;
        private static readonly string RESOURCE_FILE_PROJECT_STRINGS = "Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings";

        #endregion

    }

    /// <summary>
    /// LID object for the Sentinel device.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------
    //  02/07/07 RCG 8.00.11 N/A	Created

    internal class SentinelLID : LID
    {

        #region Public Methods
        /// <summary>
        /// LID constructor
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public SentinelLID(uint lid)
            : this(lid, TypeCode.Empty, 0, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        ///  LID Constructor that sets the unit type
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public SentinelLID(uint lid, MeasurementUnit measUnit)
            : this(lid, TypeCode.Empty, 0, measUnit)
        {
        }

        /// <summary>
        /// LID Constructor that takes a type.
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public SentinelLID(uint lid, TypeCode type)
            : this(lid, type, 0, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        /// LID Constructor that takes the data type and unit type
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public SentinelLID(uint lid, TypeCode type, MeasurementUnit measUnit)
            : this(lid, type, 0, measUnit)
        {
        }

        /// <summary>
        /// LID Constructor that takes a type and the length (Used for Strings)
        /// </summary>
        /// <param name="lid">32-bit number value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="uiLength">The length of the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public SentinelLID(uint lid, TypeCode type, uint uiLength)
            : this(lid, type, uiLength, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        /// LID Constructor that takes the data type, length, and unit type
        /// </summary>
        /// <param name="lid">32-bit number value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="uiLength">The length of the data</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public SentinelLID(uint lid, TypeCode type, uint uiLength, MeasurementUnit measUnit)
            : base(lid, type, uiLength, measUnit)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Determines the LID description and type for Sentinel specific LIDs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created
        //  01/21/11 RCG 2.45.24        Fixing Ins VA and Ins Var reading issue

        protected override void DetermineLIDInformation()
        {
            uint uiBaseComponent = (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK & m_lidValue;

            // First call the base method so that we can overwrite
            // values if we need to.
            base.DetermineLIDInformation();

            // Now check for the AMI specific items
            switch (uiBaseComponent)
            {
                case (uint)DefinedLIDs.BaseLIDs.CONSTANT_CONFIG:
                {
                    uint uiConstConfig = m_lidValue & (uint)DefinedLIDs.Constant_Config.CONSTANT_CONFIG_MASK;

                    switch (uiConstConfig)
                    {
                        case (uint)DefinedLIDs.Constant_Config.LOAD_RESRCH:
                        {
                            // The load research ID for the Sentinel is a byte
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                    }

                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.STATEMON_DATA:
                {
                    uint uiStateMonData = m_lidValue & (uint)DefinedLIDs.StateMon_STD.STATEMON_STD_MASK;
                    switch (uiStateMonData)
                    {
                        case (uint)SentinelDefinedLIDs.StateMon_STD.NON_FATAL_ERRORS:
                        case (uint)SentinelDefinedLIDs.StateMon_STD.FATAL_ERRORS:
                        case (uint)SentinelDefinedLIDs.StateMon_STD.DIAG_ERRORS:
                        case (uint)SentinelDefinedLIDs.StateMon_STD.DIAG_COUNT_1:
                        case (uint)SentinelDefinedLIDs.StateMon_STD.DIAG_COUNT_2:
                        case (uint)SentinelDefinedLIDs.StateMon_STD.DIAG_COUNT_3:
                        case (uint)SentinelDefinedLIDs.StateMon_STD.DIAG_COUNT_4:
                        case (uint)SentinelDefinedLIDs.StateMon_STD.DIAG_COUNT_5:
                        case (uint)SentinelDefinedLIDs.StateMon_STD.PHASE_COUNT_1:
                        case (uint)SentinelDefinedLIDs.StateMon_STD.PHASE_COUNT_2:
                        case (uint)SentinelDefinedLIDs.StateMon_STD.PHASE_COUNT_3:
                        case (uint)SentinelDefinedLIDs.StateMon_STD.NON_FATAL_ERRORS2:
                        case (uint)SentinelDefinedLIDs.StateMon_STD.DIAG_COUNT_6:
                        case (uint)SentinelDefinedLIDs.StateMon_STD.DIAG6_ERRORS:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)SentinelDefinedLIDs.StateMon_STD.POWER_OUTAGES:
                        {
                            m_lidType = TypeCode.Byte;
                            m_lidDescription = "Number of Power Outages";
                            break;
                        }
                        case (uint)SentinelDefinedLIDs.StateMon_STD.TIMES_PROGRAMMED:
                        {
                            m_lidType = TypeCode.Byte;
                            m_lidDescription = "Program Count";
                            break;
                        }

                    }
                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.IO_DATA:
                {
                    uint uiIOData = m_lidValue & (uint)SentinelDefinedLIDs.IO_Data.IO_DATA_MASK;
                    switch (uiIOData)
                    {

                        case (uint)SentinelDefinedLIDs.IO_Data.IO_CAPABILITES:
                        {
                            m_lidType = TypeCode.UInt16;
                            m_lidDescription = "I/O Capabilities";
                            break;
                        }
                    }
                    break;
                }
                case (uint)SentinelDefinedLIDs.BaseLIDs.CALIBRATION_DATA:
                {
                    uint uiCalibDataQuantity = m_lidValue & (uint)SentinelDefinedLIDs.CalibDataQuantities.CAL_QUANT_MASK;

                    switch (uiCalibDataQuantity)
                    {
                        case (uint)SentinelDefinedLIDs.CalibDataQuantities.CAL_TIME_STAMP:
                        {
                            m_lidType = TypeCode.DateTime;
                            m_lidDescription = "Calibration Date and Time";
                            break;
                        }
                        case (uint)SentinelDefinedLIDs.CalibDataQuantities.CAL_METER_FORM:
                        case (uint)SentinelDefinedLIDs.CalibDataQuantities.CAL_METER_BASE:
                        case (uint)SentinelDefinedLIDs.CalibDataQuantities.CAL_PWR_SUP_TYPE:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)SentinelDefinedLIDs.CalibDataQuantities.CAL_HW_VERS_REV:
                        {
                            m_lidType = TypeCode.Single;
                            break;
                        }
                        case (uint)SentinelDefinedLIDs.CalibDataQuantities.CAL_MFG_SERIAL_NUM:
                        {
                            m_lidType = TypeCode.String;
                            m_lidLength = 10;
                            break;
                        }
                    }
                    break;
                }
            }

            // Change the type back if the user has requested a different data type
            if (m_requestedTypeCode != TypeCode.Empty && m_requestedTypeCode != m_lidType)
            {
                m_lidType = m_requestedTypeCode;
            }
        }

        #endregion

    }

    /// <summary>
    /// LID object for the Centron Mono device.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------
    //  02/07/07 RCG 8.00.11 N/A	Created

    public class CentronMonoLID : LID
    {

        #region Public Methods
        /// <summary>
        /// LID constructor
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronMonoLID(uint lid)
            : this(lid, TypeCode.Empty, 0, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        ///  LID Constructor that sets the unit type
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronMonoLID(uint lid, MeasurementUnit measUnit)
            : this(lid, TypeCode.Empty, 0, measUnit)
        {
        }

        /// <summary>
        /// LID Constructor that takes a type.
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronMonoLID(uint lid, TypeCode type)
            : this(lid, type, 0, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        /// LID Constructor that takes the data type and unit type
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronMonoLID(uint lid, TypeCode type, MeasurementUnit measUnit)
            : this(lid, type, 0, measUnit)
        {
        }

        /// <summary>
        /// LID Constructor that takes a type and the length (Used for Strings)
        /// </summary>
        /// <param name="lid">32-bit number value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="uiLength">The length of the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronMonoLID(uint lid, TypeCode type, uint uiLength)
            :
            this(lid, type, uiLength, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        /// LID Constructor that takes the data type, length, and unit type
        /// </summary>
        /// <param name="lid">32-bit number value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="uiLength">The length of the data</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronMonoLID(uint lid, TypeCode type, uint uiLength, MeasurementUnit measUnit)
            : base(lid, type, uiLength, measUnit)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Determines the LID description and type for Centron Mono specific LIDs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created
        //  01/21/11 RCG 2.45.24        Fixing Ins VA and Ins Var reading issue

        protected override void DetermineLIDInformation()
        {
            uint uiBaseComponent = (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK & m_lidValue;

            // First call the base method so that we can overwrite
            // values if we need to.
            base.DetermineLIDInformation();

            // Now check for the AMI specific items
            switch (uiBaseComponent)
            {
                case (uint)DefinedLIDs.BaseLIDs.CONSTANT_CONFIG:
                {
                    uint uiConstConfig = m_lidValue & (uint)DefinedLIDs.Constant_Config.CONSTANT_CONFIG_MASK;

                    switch (uiConstConfig)
                    {
                        case (uint)DefinedLIDs.Constant_Config.LOAD_RESRCH:
                        {
                            // The load research ID for the Centron ANSI meters is a
                            // 16 character string
                            m_lidType = TypeCode.String;
                            m_lidLength = 16;
                            break;
                        }
                    }

                    break;
                }
                case (uint)DefinedLIDs.BaseLIDs.STATEMON_DATA:
                {
                    uint uiStateMonData = m_lidValue & (uint)DefinedLIDs.StateMon_STD.STATEMON_STD_MASK;
                    switch (uiStateMonData)
                    {
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.NON_FATAL_ERRORS:
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.FATAL_ERRORS:
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG_ERRORS:
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.NON_FATAL_ERRORS2:
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG_COUNT_1:
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG_COUNT_2:
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG_COUNT_3:
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG_COUNT_4:
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG_COUNT_5:
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.PHASE_COUNT_1:
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.PHASE_COUNT_2:
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.PHASE_COUNT_3:
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG_COUNT_6:
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.DIAG6_ERRORS:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.POWER_OUTAGES:
                        {
                            m_lidType = TypeCode.Byte;
                            m_lidDescription = "Number of Power Outages";
                            break;
                        }
                        case (uint)CentronMonoDefinedLIDs.StateMon_STD.TIMES_PROGRAMMED:
                        {
                            m_lidType = TypeCode.Byte;
                            m_lidDescription = "Program Count";
                            break;
                        }

                    }
                    break;
                }
            }

            // Change the type back if the user has requested a different data type
            if (m_requestedTypeCode != TypeCode.Empty && m_requestedTypeCode != m_lidType)
            {
                m_lidType = m_requestedTypeCode;
            }
        }

        #endregion

    }

    /// <summary>
    /// LID object for the Centron Poly device.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------
    //  02/07/07 RCG 8.00.11 N/A	Created

    public class CentronPolyLID : CentronMonoLID
    {

        #region Public Methods
        /// <summary>
        /// LID constructor
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronPolyLID(uint lid)
            : this(lid, TypeCode.Empty, 0, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        ///  LID Constructor that sets the unit type
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronPolyLID(uint lid, MeasurementUnit measUnit)
            : this(lid, TypeCode.Empty, 0, measUnit)
        {
        }

        /// <summary>
        /// LID Constructor that takes a type.
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronPolyLID(uint lid, TypeCode type)
            : this(lid, type, 0, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        /// LID Constructor that takes the data type and unit type
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronPolyLID(uint lid, TypeCode type, MeasurementUnit measUnit)
            : this(lid, type, 0, measUnit)
        {
        }

        /// <summary>
        /// LID Constructor that takes a type and the length (Used for Strings)
        /// </summary>
        /// <param name="lid">32-bit number value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="uiLength">The length of the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronPolyLID(uint lid, TypeCode type, uint uiLength)
            : this(lid, type, uiLength, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        /// LID Constructor that takes the data type, length, and unit type
        /// </summary>
        /// <param name="lid">32-bit number value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="uiLength">The length of the data</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronPolyLID(uint lid, TypeCode type, uint uiLength, MeasurementUnit measUnit)
            : base(lid, type, uiLength, measUnit)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Determines the LID description and type for Centron Mono specific LIDs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created
        //  01/21/11 RCG 2.45.24        Fixing Ins VA and Ins Var reading issue

        protected override void DetermineLIDInformation()
        {
            uint uiBaseComponent = (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK & m_lidValue;

            // First call the base method so that we can overwrite
            // values if we need to.
            base.DetermineLIDInformation();

            // Now check for the AMI specific items
            switch (uiBaseComponent)
            {
                case (uint)CentronPolyDefinedLIDs.BaseLIDs.BASE_DATA:
                {
                    uint uiBaseData = m_lidValue & (uint)CentronPolyDefinedLIDs.Base_Data.BASE_QUANT_MASK;

                    switch (uiBaseData)
                    {
                        case (uint)CentronPolyDefinedLIDs.Base_Data.BASE_TIME_STAMP:
                        {
                            m_lidType = TypeCode.DateTime;
                            m_lidDescription = "Calibration Date and Time";
                            break;
                        }
                        case (uint)CentronPolyDefinedLIDs.Base_Data.BASE_METER_FORM:
                        case (uint)CentronPolyDefinedLIDs.Base_Data.BASE_METER_BASE:
                        case (uint)CentronPolyDefinedLIDs.Base_Data.BASE_HW_VERSION:
                        {
                            m_lidType = TypeCode.Byte;
                            break;
                        }
                    }
                    break;
                }
            }

            // Change the type back if the user has requested a different data type
            if (m_requestedTypeCode != TypeCode.Empty && m_requestedTypeCode != m_lidType)
            {
                m_lidType = m_requestedTypeCode;
            }
        }

        #endregion

    }

    /// <summary>
    /// LID object for the Centron AMI device.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------
    //  02/07/07 RCG 8.00.11 N/A	Created

    public class CentronAMILID : CentronMonoLID
    {

        #region Public Methods
        /// <summary>
        /// LID constructor
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronAMILID(uint lid)
            : this(lid, TypeCode.Empty, 0, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        ///  LID Constructor that sets the unit type
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronAMILID(uint lid, MeasurementUnit measUnit)
            : this(lid, TypeCode.Empty, 0, measUnit)
        {
        }

        /// <summary>
        /// LID Constructor that takes a type.
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronAMILID(uint lid, TypeCode type)
            : this(lid, type, 0, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        /// LID Constructor that takes the data type and unit type
        /// </summary>
        /// <param name="lid">32-bit integer value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronAMILID(uint lid, TypeCode type, MeasurementUnit measUnit)
            : this(lid, type, 0, measUnit)
        {
        }

        /// <summary>
        /// LID Constructor that takes a type and the length (Used for Strings)
        /// </summary>
        /// <param name="lid">32-bit number value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="uiLength">The length of the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronAMILID(uint lid, TypeCode type, uint uiLength)
            :
            this(lid, type, uiLength, MeasurementUnit.UNIT)
        {
        }

        /// <summary>
        /// LID Constructor that takes the data type, length, and unit type
        /// </summary>
        /// <param name="lid">32-bit number value of the LID</param>
        /// <param name="type">The type of the data represented by the LID</param>
        /// <param name="uiLength">The length of the data</param>
        /// <param name="measUnit">The unit type of the LID</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created

        public CentronAMILID(uint lid, TypeCode type, uint uiLength, MeasurementUnit measUnit)
            : base(lid, type, uiLength, measUnit)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Determines the LID description and type for Centron Mono specific LIDs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 RCG 8.00.11 N/A	Created
        //  01/21/11 RCG 2.45.24        Fixing Ins VA and Ins Var reading issue

        protected override void DetermineLIDInformation()
        {
            uint uiBaseComponent = (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK & m_lidValue;

            // First call the base method so that we can overwrite
            // values if we need to.
            base.DetermineLIDInformation();

            // Now check for the AMI specific items
            switch (uiBaseComponent)
            {
                case (uint)DefinedLIDs.BaseLIDs.OPTION_BRD_DATA:
                {
                    uint uiOptBrdData = m_lidValue & (uint)DefinedLIDs.Opt_Brd_Data.OPT_BRD_DATA_MASK;

                    // For AMI the option board fields are being used for communication status information
                    // so we need to change the description to reflect this change
                    switch (uiOptBrdData)
                    {
                        case (uint)DefinedLIDs.Opt_Brd_Data.OPT_BRD_VEND_FLD_1:
                        {
                            m_lidDescription = "Comm. Status Field 1";
                            break;
                        }
                        case (uint)DefinedLIDs.Opt_Brd_Data.OPT_BRD_VEND_FLD_2:
                        {
                            m_lidDescription = "Comm. Status Field 2";
                            break;
                        }
                        case (uint)DefinedLIDs.Opt_Brd_Data.OPT_BRD_VEND_FLD_3:
                        {
                            m_lidDescription = "Comm. Status Field 3";
                            break;
                        }
                    }
                    break;
                }
            }

            // Change the type back if the user has requested a different data type
            if (m_requestedTypeCode != TypeCode.Empty && m_requestedTypeCode != m_lidType)
            {
                m_lidType = m_requestedTypeCode;
            }
        }

        #endregion

    }
}
