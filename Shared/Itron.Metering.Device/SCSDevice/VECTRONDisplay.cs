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

using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using Itron.Metering.Communications;
using Itron.Metering.Communications.SCS;

namespace Itron.Metering.Device
{
 	
    public partial class VECTRON : SCSDevice
    {
        #region Internal Methods
       
        /// <summary>
        /// This method is responsible for retrieving the basepage address of any given
        /// meter display item - normal, alternate, or test mode
        /// </summary>
        /// <param name="displayItem">The display item to look up</param>
        /// <returns>An integer representing the associated basepage address
        /// </returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        override internal int TranslateDisplayAddress(SCSDisplayItem displayItem)
        {
            int nAddress;

            switch (displayItem.UpperAddress)
            {
                case 0x01:  // RAM Bank 1

                    switch (displayItem.LowerAddress)
                    {
                        case 0x00: nAddress = 0x20F9; break;  // Current Date
                        case 0x06: nAddress = 0x20FC; break;  // Current Time
                        case 0x0C: nAddress = 0x20FF; break;  // Current Day of Week
                        case 0x2F: nAddress = 0x1B81; break;  // Instantaneous kVA
                        case 0x30: nAddress = 0x1B7D; break;  // present VA Total
                        case 0x31: nAddress = 0x1B79; break;  // present VA Lag
                        case 0x32: nAddress = 0x1B75; break;  // present Var
                        case 0x33: nAddress = 0x1B69; break;  // Test mode VAh lag
                        case 0x35: nAddress = 0x1B54; break;  // Test mode Max kW
                        case 0x36: nAddress = 0x1B61; break;  // Test mode Max var lag
                        case 0x37: nAddress = 0x1B50; break;  // Test mode kWh
                        case 0x39: nAddress = 0x1B65; break;  // Test mode varh Lead
                        case 0x3B: nAddress = 0x1B0B; break;  // present Watts
                        case 0x3C: nAddress = 0x1B07; break;  // Instantaneous kW
                        case 0x3D: nAddress = 0x1B05; break;  // Motorola FW Version
                        case 0x3E: nAddress = 0x1D28; break;  // Register Multiplier
                        case 0x3F: nAddress = 0x1D24; break;  // Demand Threshold
                        case 0x40: nAddress = 0x1B5D; break;  // Test mode varh lag
                        case 0x41: nAddress = 0x1B6D; break;  // Test mode Max VA lag
                        case 0x42: nAddress = 0x1B71; break;  // Test mode Max VA Total
                        case 0x44: nAddress = 0x2170; break;  // Time until test mode timeout
                        case 0x48: nAddress = 0x0000; break;  // Time remaining in subinterval
                        case 0xA6: nAddress = 0x0000; break;  // # input pulses
                        case 0xAC: nAddress = 0x0000; break;  // previous interval # input pulses
                        default:
                            nAddress = (0x1B00 | displayItem.LowerAddress);
                            break;
                    }
                    break;
                case 0x08: nAddress = (0x2100 | displayItem.LowerAddress);
                    break;
                case 0x09: nAddress = (0x2200 | displayItem.LowerAddress);
                    break;
                case 0x0A: nAddress = (0x2300 | displayItem.LowerAddress);
                    break;
                case 0x0B: nAddress = (0x2400 | displayItem.LowerAddress);
                    break;
                case 0x0C: nAddress = (0x2600 | displayItem.LowerAddress);
                    break;
                case 0x0D: nAddress = (0x2700 | displayItem.LowerAddress);
                    break;
                case 0x0E: nAddress = (0x2800 | displayItem.LowerAddress);
                    break;
                case 0x0F: nAddress = (0x2900 | displayItem.LowerAddress);
                    break;
                default:
                    nAddress = displayItem.LowerAddress;
                    break;
            }

            return nAddress;
        }

        /// <summary>
        /// This method returns a user viewable description for a given display item.
        /// </summary>
        /// <param name="nBasepageAddress"></param>
        /// <returns>A string that describes the given display item
        /// </returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        override internal string GetDisplayItemDescription(int nBasepageAddress)
        {
            String strDescription;

            // The VECTRON has a set of 4 general purpose registers that the user can program 
            // to measure a number of different quantities.  In order to understand which quantity is
            // mapped to which register we need to read the register map but (and this is important!)
            // we only need to read it once!  The register map can only change when the meter is
            // initialized so it would be a colossial waste of time to read it for each display item

            if (!m_blnRetreivedRegMapping)
            {
                ReadRegisterMappings();
            }

            // Now that we know which register is which, we can use the item's basepage address to
            // tell us what each display item represents.

            switch (nBasepageAddress)
            {
                case (int)VECAddresses.PROCESSOR_REVISION: strDescription = "Processor Revision"; break;
                case (int)VECAddresses.INS_KW: strDescription = "ins kW"; break;
                case (int)VECAddresses.PRESENT_KW: strDescription = "prs kW"; break;
                case (int)VECAddresses.TEST_MODE_KWH: strDescription = "kWh"; break;
                case (int)VECAddresses.TEST_MODE_KW: strDescription = "max kW"; break;
                case (int)VECAddresses.TEST_MODE_KVARH_LAG: strDescription = "kvarh lag"; break;
                case (int)VECAddresses.TEST_MODE_KVAR_LAG: strDescription = "max kvar lag"; break;
                case (int)VECAddresses.TEST_MODE_KVARH_LEAD: strDescription = "kvarh lead"; break;
                case (int)VECAddresses.TEST_MODE_KVAH_LAG: strDescription = "kVAh lag"; break;
                case (int)VECAddresses.TEST_MODE_KVA_LAG: strDescription = "max kVA lag"; break;
                case (int)VECAddresses.TEST_MODE_KVA_TOTAL: strDescription = "max kVA"; break;
                case (int)VECAddresses.PRESENT_KVAR_LAG: strDescription = "prs kvar lag"; break;
                case (int)VECAddresses.PRESENT_KVA_LAG: strDescription = "prs kVA lag"; break;
                case (int)VECAddresses.PRESENT_KVA_TOTAL: strDescription = "prs kVA total"; break;
                case (int)VECAddresses.DEMAND_THRESHOLD: strDescription = "Demand Threshold"; break;
                case (int)VECAddresses.REGISTER_MULTIPLIER: strDescription = "Register Multiplier"; break;
                case (int)VECAddresses.DAY_OF_WEEK: strDescription = "Current Day of Week"; break;
                case (int)VECAddresses.REG_2_RATE_E_MAX: strDescription = "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]); break;
                case 0x211C: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]); break;
                case 0x211E: strDescription = "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]); break;

                case 0x2119: strDescription = GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]); break;
                case (int)VECAddresses.REG_1_RATE_E_CUM: strDescription = "cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]); break;
                case (int)VECAddresses.REG_1_RATE_E_MAX: strDescription = "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]); break;
                case 0x2128: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]); break;
                case 0x212A: strDescription = "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]); break;

                case (int)VECAddresses.LAST_RESET_DATE: strDescription = "Last Reset Date"; break;
                case (int)VECAddresses.LAST_RESET_TIME: strDescription = "Last Reset Time"; break;
                case (int)VECAddresses.RESET_COUNT: strDescription = "Demand Reset Count"; break;
                case (int)VECAddresses.OUTAGE_COUNT: strDescription = "Outage Count"; break;

                case (int)VECAddresses.REG_3_RATE_E_CUM: strDescription = "cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]); break;

                case (int)VECAddresses.REG_1_RATE_A_MAX: strDescription = "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_A; break;
                case 0x213C: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_A; break;
                case 0x213E: strDescription = "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_A; break;

                case (int)VECAddresses.REG_4_RATE_E_CUM: strDescription = "cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]); break;

                case (int)VECAddresses.REG_1_RATE_B_MAX: strDescription = "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_B; break;
                case 0x2148: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_B; break;
                case 0x214A: strDescription = "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_B; break;

                case (int)VECAddresses.DAYS_SINCE_LAST_RESET: strDescription = "Days Since Demand Reset"; break;
                case (int)VECAddresses.TRANSFORMER_RATIO: strDescription = "Transformer Ratio"; break;
                case (int)VECAddresses.DISPLAY_OPTIONS: strDescription = "Display On Time"; break;

                case 0x2150: strDescription = "prv " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]); break;
                case (int)VECAddresses.REG_1_RATE_C_MAX: strDescription = "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_C; break;
                case 0x2158: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_C; break;
                case 0x215A: strDescription = "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_C; break;

                case 0x2160: strDescription = "prv " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]); break;
                case (int)VECAddresses.REG_1_RATE_D_MAX: strDescription = "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_D; break;
                case 0x2168: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_D; break;
                case 0x216A: strDescription = "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_D; break;

                case (int)VECAddresses.REGISTER_FULL_SCALE: strDescription = "Register full-scale"; break;
                case (int)VECAddresses.TEST_MODE_TIMEOUT: strDescription = "Time remaining in Test Mode"; break;

                case (int)VECAddresses.REG_2_RATE_A_MAX:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) == VECRegisterType.DEMAND)
                    {
                        strDescription = "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_A;
                    }
                    else
                    {
                        strDescription = GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_A;
                    }
                    break;

                case 0x2176: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_A; break;
                case 0x2178: strDescription = "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_A; break;
                case (int)VECAddresses.REG_2_RATE_E_CUM: strDescription = "cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]); break;

                case 0x2179: strDescription = GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_B; break;

                case (int)VECAddresses.REG_2_RATE_B_MAX:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) == VECRegisterType.DEMAND)
                    {
                        strDescription = "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_B;
                    }
                    else
                    {
                        strDescription = GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_C;
                    }
                    break;

                case 0x2184: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_B; break;
                case 0x2186: strDescription = "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_B; break;

                case 0x2187: strDescription = GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_D; break;

                case (int)VECAddresses.DEMAND_CONFIGURATION: strDescription = "Number of Subintervals"; break;
                case (int)VECAddresses.SUBINT_LENGTH: strDescription = "Subinterval Length"; break;
                case (int)VECAddresses.TEST_SUBINT_LENGTH: strDescription = "Test Subinterval Length"; break;
                case (int)VECAddresses.MINUTES_ON_BATTERY: strDescription = "Minutes On Battery"; break;
                case (int)VECAddresses.PROGRAM_COUNT: strDescription = "Program Count"; break;
                case (int)VECAddresses.LAST_PROGRAMMED_DATE: strDescription = "Last Program Date"; break;
                case (int)VECAddresses.LAST_PROGRAMMED_TIME: strDescription = "Last Program Time"; break;

                case (int)VECAddresses.REG_2_RATE_C_MAX: strDescription = "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_C; break;
                case 0x21B4: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_C; break;
                case 0x21B6: strDescription = "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_C; break;
                case (int)VECAddresses.REG_2_RATE_D_MAX: strDescription = "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_D; break;
                case 0x21BC: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_D; break;
                case 0x21BE: strDescription = "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_D; break;

                case (int)VECAddresses.NORMAL_KH: strDescription = "Normal Kh"; break;
                case (int)VECAddresses.KYZ_1_PULSE_WEIGHT: strDescription = "KYZ1 Pulse weight"; break;
                case (int)VECAddresses.TEST_MODE_KH: strDescription = "Test Kh"; break;

                case (int)VECAddresses.REG_3_RATE_E_MAX:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]) == VECRegisterType.DEMAND)
                    {
                        strDescription = "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]);
                    }
                    else
                    {
                        strDescription = GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]);
                    }
                    break;
                case 0x21D4: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]); break;
                case 0x21D6: strDescription = "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]); break;

                case (int)VECAddresses.REG_4_RATE_E_MAX:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]) == VECRegisterType.DEMAND)
                    {
                        strDescription = "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]);
                    }
                    else
                    {
                        strDescription = GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]);
                    }
                    break;
                case 0x21DC: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]); break;
                case 0x21DE: strDescription = "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]); break;

                case (int)VECAddresses.SOFTWARE_REVISION: strDescription = "Software Revision"; break;
                case (int)VECAddresses.FIRMWARE_REVISION: strDescription = "Firmware Revision"; break;
                case (int)VECAddresses.USERDEFINED_FIELD1: strDescription = "User Data 01"; break;
                case (int)VECAddresses.USERDEFINED_FIELD2: strDescription = "User Data 02"; break;
                case (int)VECAddresses.USERDEFINED_FIELD3: strDescription = "User Data 03"; break;
                case (int)VECAddresses.PROGRAM_ID: strDescription = "Program ID"; break;
                case (int)VECAddresses.SERIAL_NUMBER: strDescription = "Meter ID 1"; break;
                case (int)VECAddresses.METER_ID_2: strDescription = "Meter ID 2"; break;
                case (int)VECAddresses.KYZ_2_PULSE_WEIGHT: strDescription = "KYZ2 Pulse weight"; break;

                case (int)VECAddresses.LAST_SEASON_REG_3_RATE_E_MAX:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]) == VECRegisterType.DEMAND)
                    {
                        strDescription = "Last Season Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]);
                    }
                    else
                    {
                        strDescription = "Last Season " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]);
                    }
                    break;
                case 0x2464: strDescription = "Last Season " + "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]); break;
                case 0x2466: strDescription = "Last Season " + "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]); break;
                case 0x2468: strDescription = "Last Season " + "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]); break;
                case 0x246A: strDescription = "Last Season " + "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]); break;
                case 0x246C: strDescription = "Last Season " + "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_A; break;
                case 0x246E: strDescription = "Last Season " + "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_A; break;
                case 0x2470: strDescription = "Last Season " + "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_B; break;
                case 0x2472: strDescription = "Last Season " + "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_B; break;
                case 0x2474: strDescription = "Last Season " + "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_C; break;
                case 0x2476: strDescription = "Last Season " + "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_C; break;
                case 0x2478: strDescription = "Last Season " + "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_D; break;
                case 0x247A: strDescription = "Last Season " + "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_D; break;
                case 0x247C: strDescription = "Last Season " + "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]); break;
                case 0x247E: strDescription = "Last Season " + "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]); break;

                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_E_MAX:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]) == VECRegisterType.DEMAND)
                    {
                        strDescription = "Last Season Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]);
                    }
                    else
                    {
                        strDescription = "Last Season " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]);
                    }
                    break;
                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_E_MAX: strDescription = "Last Season Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]); break;

                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_A_MAX:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) == VECRegisterType.DEMAND)
                    {
                        strDescription = "Last Season Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_A;
                    }
                    else
                    {
                        strDescription = "Last Season " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_A;
                    }
                    break;
                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_A_MAX: strDescription = "Last Season Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]) + " " + VEC_RATE_A; break;

                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_B_MAX:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) == VECRegisterType.DEMAND)
                    {
                        strDescription = "Last Season Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_B;
                    }
                    else
                    {
                        strDescription = "Last Season " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_B;
                    }
                    break;
                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_B_MAX: strDescription = "Last Season Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]) + " " + VEC_RATE_B; break;

                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_C_MAX:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) == VECRegisterType.DEMAND)
                    {
                        strDescription = "Last Season Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_C;
                    }
                    else
                    {
                        strDescription = "Last Season " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_C;
                    }
                    break;
                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_C_MAX: strDescription = "Last Season Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]) + " " + VEC_RATE_C; break;

                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_D_MAX:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) == VECRegisterType.DEMAND)
                    {
                        strDescription = "Last Season Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_D;
                    }
                    else
                    {
                        strDescription = "Last Season " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_D;
                    }
                    break;
                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_D_MAX: strDescription = "Last Season Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]) + " " + VEC_RATE_D; break;

                case 0x24A0: strDescription = "Last Season " + "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]); break;
                case 0x24A2: strDescription = "Last Season " + "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]); break;
                case 0x24A4: strDescription = "Last Season " + "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_A; break;
                case 0x24A6: strDescription = "Last Season " + "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_A; break;
                case 0x24A8: strDescription = "Last Season " + "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_B; break;
                case 0x24AA: strDescription = "Last Season " + "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_B; break;
                case 0x24AC: strDescription = "Last Season " + "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_C; break;
                case 0x24AE: strDescription = "Last Season " + "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_C; break;

                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_E_CUM: strDescription = "Last Season cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]); break;
                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_E_CUM: strDescription = "Last Season cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]); break;
                case (int)VECAddresses.LAST_SEASON_REG_3_RATE_E_CUM: strDescription = "Last Season cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]); break;
                case (int)VECAddresses.LAST_SEASON_REG_4_RATE_E_CUM: strDescription = "Last Season cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]); break;

                case 0x24C1: strDescription = "Last Season " + "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_D; break;
                case 0x24C3: strDescription = "Last Season " + "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_D; break;

                case (int)VECAddresses.TOU_EXPIRATION_DATE: strDescription = "TOU Expiration Date"; break;
                case (int)VECAddresses.TOU_SCHEDULE_ID: strDescription = "TOU Schedule ID"; break;

                case 0x27A0: strDescription = GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]) + " " + VEC_PEAK_2; break;
                case 0x27A4: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]) + " " + VEC_PEAK_2; break;
                case 0x27A6: strDescription = "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]) + " " + VEC_PEAK_2; break;
                case 0x27A8: strDescription = GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]) + " " + VEC_PEAK_2; break;
                case 0x27AC: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]) + " " + VEC_PEAK_2; break;
                case 0x27AE: strDescription = "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]) + " " + VEC_PEAK_2; break;
                case 0x27B0: strDescription = GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]) + " " + VEC_PEAK_2; break;
                case 0x27B4: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]) + " " + VEC_PEAK_2; break;
                case 0x27B6: strDescription = "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]) + " " + VEC_PEAK_2; break;
                case 0x27B8: strDescription = GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]) + " " + VEC_PEAK_2; break;
                case 0x27BC: strDescription = "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]) + " " + VEC_PEAK_2; break;
                case 0x27BE: strDescription = "TIme of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]) + " " + VEC_PEAK_2; break;

                case (int)VECAddresses.REG_1_RATE_A_CUM: strDescription = "cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_A; break;
                case (int)VECAddresses.REG_1_RATE_B_CUM: strDescription = "cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_B; break;
                case (int)VECAddresses.REG_1_RATE_C_CUM: strDescription = "cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_C; break;
                case (int)VECAddresses.REG_1_RATE_D_CUM: strDescription = "cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_D; break;

                case (int)VECAddresses.REG_2_RATE_A_CUM: strDescription = "cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_A; break;
                case (int)VECAddresses.REG_2_RATE_B_CUM: strDescription = "cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_B; break;
                case (int)VECAddresses.REG_2_RATE_C_CUM: strDescription = "cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_C; break;
                case (int)VECAddresses.REG_2_RATE_D_CUM: strDescription = "cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_D; break;

                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_A_CUM: strDescription = "Last Season cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_A; break;
                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_B_CUM: strDescription = "Last Season cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_B; break;
                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_C_CUM: strDescription = "Last Season cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_C; break;
                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_D_CUM: strDescription = "Last Season cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " " + VEC_RATE_D; break;

                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_A_CUM: strDescription = "Last Season cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_A; break;
                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_B_CUM: strDescription = "Last Season cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_B; break;
                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_C_CUM: strDescription = "Last Season cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_C; break;
                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_D_CUM: strDescription = "Last Season cum " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " " + VEC_RATE_D; break;

                default:
                    if (nBasepageAddress >= (int)VECAddresses.SR_DATA_BLOCK_1 && nBasepageAddress < (int)VECAddresses.SR_DATA_BLOCK_2)
                    {
                        strDescription = GetSelfReadQuantityDescription(nBasepageAddress - (int)VECAddresses.SR_DATA_BLOCK_1, 1);
                    }
                    else if (nBasepageAddress >= (int)VECAddresses.SR_DATA_BLOCK_2 && nBasepageAddress < (int)VECAddresses.SR_DATA_BLOCK_3)
                    {
                        strDescription = GetSelfReadQuantityDescription(nBasepageAddress - (int)VECAddresses.SR_DATA_BLOCK_2, 2);
                    }
                    else if (nBasepageAddress >= (int)VECAddresses.SR_DATA_BLOCK_3 && nBasepageAddress < (int)VECAddresses.SR_DATA_BLOCK_4)
                    {
                        strDescription = GetSelfReadQuantityDescription(nBasepageAddress - (int)VECAddresses.SR_DATA_BLOCK_3, 3);
                    }
                    else if (nBasepageAddress >= (int)VECAddresses.SR_DATA_BLOCK_4 && nBasepageAddress < 0x29D0)
                    {
                        strDescription = GetSelfReadQuantityDescription(nBasepageAddress - (int)VECAddresses.SR_DATA_BLOCK_4, 4);
                    }
                    else
                    {
                        strDescription = "";
                    }
                    break;
            }

            return strDescription;
        }
        
        /// <summary>
        /// This method returns a user viewable description for a given display item.  This method
        /// is overriden to return the descriptions that are unique to the VECTRON
        /// </summary>
        /// <param name="displayItem"></param>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        override internal string GetDisplayItemDescription(SCSDisplayItem displayItem)
        {
            String strDescription;
            int nDisplayItemAddress = TranslateDisplayAddress(displayItem);

            if (nDisplayItemAddress != 0x00)
            {
                strDescription = GetDisplayItemDescription(nDisplayItemAddress);

                if (displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.TOUContinuousCumulativeValue)
                {
                    // All ccum values are calculated from the max demand and the cummulative demand values - the ccum
                    // values displayed are not stored in the meter.  The addresses found in the meter's display list are actually
                    // the addresses of the cum demand value and, therefore, the default description returned at this point is also
                    // for the cum demand value not the ccum demand value.  So we need to go ahead and fix it here.

                    strDescription = strDescription.Insert(strDescription.IndexOf("cum", StringComparison.Ordinal), "c");  // all we need to do is change cum to ccum
                }
                else if (displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.TotalContinuousCumulativeValue)
                {
                    if (displayItem.RegisterType != 7) // This is NOT a last season register
                    {
                        int nCumDemandRegisterAddress = 0x2100 + (displayItem.ItemDefinition[2] & 0x7F);

                        strDescription = GetDisplayItemDescription(nCumDemandRegisterAddress);
                    }

                    strDescription = strDescription.Insert(strDescription.IndexOf("cum", StringComparison.Ordinal), "c");  // all we need to do is change cum to ccum
                }
            }
            else
            {
                // there are a few special items that do not have equivalent basepage items - we need
                // to set these descriptions based on the display item's type and class

                if ( displayItem.UpperAddress == 0x01 && displayItem.LowerAddress == 0x48 )
                {
                    strDescription = "Time remaining in demand subinterval";
                }
                else if ( displayItem.UpperAddress == 0x01 && displayItem.LowerAddress == 0xA6 )
                {
                    strDescription = "# Pulses Rcv'd in Test Mode";
                }
                else if ( displayItem.UpperAddress == 0x01 && displayItem.LowerAddress == 0xAC )
                {
                    strDescription = "Prev # Pulses Rcv'd in Test Mode";
                }
                else
                {
                    strDescription = "";
                }
            }

            // Lastly check to see if we actually found a description.  If not, call the base class to display some debugging information

            if (strDescription.Length == 0)
            {
                strDescription = base.GetDisplayItemDescription(displayItem);
            }

            return strDescription;
        }

        /// <summary>
        /// This method reads and returns a present demand value from a VECTRON meter.  This override
        /// is needed since the present demand value in a VECTRON is stored as a floating point value
        /// while most other SCS devices use a floating BCD value
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/13/06 mah 8.00.00  N/A   Created
        /// </remarks>
        override internal string ReadPresentDemandValue(SCSDisplayItem displayItem)
        {
            return ReadFloatingPointValue(TranslateDisplayAddress(displayItem)).ToString("######0.000", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// This method reads and returns a previous demand value from a VECTRON meter.  This override
        /// is needed since the previous demand value in a VECTRON is stored as a floating point value
        /// while most other SCS devices use a floating BCD value
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/13/06 mah 8.00.00  N/A   Created
        /// </remarks>
        override internal string ReadPreviousDemandValue(SCSDisplayItem displayItem)
        {
            return ReadFloatingPointValue(TranslateDisplayAddress(displayItem)).ToString("######0.000", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// This method a 4 byte floating point BCD value from an SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the value cannot be retreived from the meter.
        /// </exception>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        override internal String ReadFloatingBCDValue(int nBasepageAddress, int nLength)
        {
            // The VECTRON is deceptive.  It uses the Floating point BCD class for non BCD values
            // as well - tricky, tricky.  So we need to treat all RAM values as floating point and all
            // EEPROM values as BCD

            if (nBasepageAddress < 0x2100)
            {
                return ReadFloatingPointValue(nBasepageAddress).ToString("#####0.000", CultureInfo.CurrentCulture);
            }
            else
            {
                return base.ReadFloatingBCDValue(nBasepageAddress, nLength);
            }
        }

        /// <summary>
        /// This method is responsible for either retrieving or calculating the continuous
        /// cummulative demand value associated with the given display item.  Note
        /// that this method is not currently implemented and will throw an exception
        /// </summary>
        /// <param name="displayItem">The display item to look up</param>
        /// <returns>A string representing the ccum value
        /// </returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/03/07 mah 8.00.00  N/A   Created
        /// </remarks>
        override internal String RetrieveCCumValue(SCSDisplayItem displayItem)
        {
            double dblCumDemand = 0.0;
            double dblMaxDemand = 0.0;
            int nCumDemandRegisterAddress;
            int nMaxDemandRegisterAddress;
            int nBCDLength;

            // Before we can calculate ccum demand, we need to determine where the cum demand value and 
            // max demand values live in the meter's basepage.  Unfortunately the display item structures are somewhat 
            // inconsistent.  Please read the VECTRON basepage carefully!  The addresses for both the max and cum
            // values are embedded in the display item data for current RATE E ccum items only.  For all other ccum 
            // values, we need to look up the max demand values ourselves

            if ((displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.TotalContinuousCumulativeValue) &&
                (displayItem.RegisterType != 7))
            {
                nMaxDemandRegisterAddress = 0x2100 + displayItem.ItemDefinition[3];
                nCumDemandRegisterAddress = 0x2100 + (displayItem.ItemDefinition[2] & 0x7F);
            }
            else
            {
                nCumDemandRegisterAddress = TranslateDisplayAddress(displayItem);
                nMaxDemandRegisterAddress = LookUpMaxDemandAddress(nCumDemandRegisterAddress);
            }

            // Before reading the registers we have to take note of what type of register we are looking up
            // because last season registers are shorter than the current season registers.

            if (displayItem.RegisterType == 7) // a last season register
            {
                nBCDLength = 3; // All last season registers are 3 bytes long
            }
            else
            {
                nBCDLength = 4; // All current registers are 4 bytes long
            }

            // Now that we have the addresses of both the cummulative demand register and the maximum demand
            // register we can calculate continuous cummulative demand

            if (nCumDemandRegisterAddress != 0)
            {
                dblCumDemand = double.Parse(ReadFloatingBCDValue(nCumDemandRegisterAddress, nBCDLength), CultureInfo.CurrentCulture);
            }

            if (nMaxDemandRegisterAddress != 0)
            {
                dblMaxDemand = double.Parse(ReadFloatingBCDValue(nMaxDemandRegisterAddress, nBCDLength), CultureInfo.CurrentCulture);
            } 

            double dblCCumDemand = dblCumDemand + dblMaxDemand;

            return dblCCumDemand.ToString(CultureInfo.InvariantCulture);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// This method returns a textual description for the given VECTRON
        /// register
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        private String GetQuantityDescription(VECRegQuantities register)
        {
            String strQuantity = "";
   
            switch (register)
            {
                case VECRegQuantities.VA_HOUR_LAG: 
                    if (EnergyFormat.Units == SCSDisplayFormat.DisplayUnits.Units)
                        strQuantity = "VAh lag";
                    else
                        strQuantity = "kVAh lag";
                    break;

                case VECRegQuantities.VA_LAG: 
                    if (DemandFormat.Units == SCSDisplayFormat.DisplayUnits.Units)
                        strQuantity = "VA lag";
                    else
                        strQuantity = "kVA lag";
                    break;

                case VECRegQuantities.VA_TOTAL: 
                    if (DemandFormat.Units == SCSDisplayFormat.DisplayUnits.Units)
                        strQuantity = "VA";
                    else
                        strQuantity = "kVA";
                    break;
 
                case VECRegQuantities.VAR_HOUR_LAG: 
                    if (EnergyFormat.Units == SCSDisplayFormat.DisplayUnits.Units)
                        strQuantity = "varh lag";
                    else
                        strQuantity = "kvarh lag";
                    break;
                
                case VECRegQuantities.VAR_HOUR_LEAD: 
                    if (EnergyFormat.Units == SCSDisplayFormat.DisplayUnits.Units)
                        strQuantity = "varh lead";
                    else
                        strQuantity = "kvarh lead";
                    break;
              
                case VECRegQuantities.VAR_LAG: 
                    if (DemandFormat.Units == SCSDisplayFormat.DisplayUnits.Units)
                        strQuantity = "var lag";
                    else
                        strQuantity = "kvar lag";
                    break;
                
                case VECRegQuantities.WATT: 
                case VECRegQuantities.WATT_TOU: 
                    if (DemandFormat.Units == SCSDisplayFormat.DisplayUnits.Units)
                        strQuantity = "W";
                    else
                        strQuantity = "kW";
                    break;

                case VECRegQuantities.WATT_HOUR:
                    if (EnergyFormat.Units == SCSDisplayFormat.DisplayUnits.Units)
                        strQuantity = "Wh";
                    else
                        strQuantity = "kWh";
                    break;
            }

            return strQuantity;
        }

        /// <summary>
        /// This method returns the quantity type for the given VECTRON register
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        private VECRegisterType GetQuantityType(VECRegQuantities register)
        {
            if (( register == VECRegQuantities.VA_HOUR_LAG ) ||
                ( register ==  VECRegQuantities.VAR_HOUR_LAG ) ||
                ( register ==  VECRegQuantities.VAR_HOUR_LEAD ) ||
                ( register ==  VECRegQuantities.WATT_HOUR ))
            {
                return VECRegisterType.ENERGY;
            }
            else
            {
                return VECRegisterType.DEMAND;
            }
        }
        
        /// <summary>
        /// This method returns a text description for each value contained within a 
        /// self read block
        /// </summary>
        /// <param name="nSelfReadOffset"></param>
        /// <param name="nSelfReadNumber"></param>
        /// <returns></returns>
        private String GetSelfReadQuantityDescription(int nSelfReadOffset, int nSelfReadNumber)
        {
            String strSelfReadDescription = "Self Read " + nSelfReadNumber.ToString(CultureInfo.InvariantCulture) + " ";

            switch ( nSelfReadOffset )
            {
                case 0x00: strSelfReadDescription += "Date"; break;
                case 0x02: strSelfReadDescription += "Time"; break;
                case 0x04: strSelfReadDescription += "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]); break;
                case 0x07: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]); break;
                case 0x09: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]); break;

                case 0x0B: 
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]) == VECRegisterType.DEMAND)
                    {
                        strSelfReadDescription += "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]);
                    }
                    else
                    {
                        strSelfReadDescription += GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]);
                    }
                    break;

                case 0x0E: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]); break;
                case 0x10: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]); break;

                case 0x12:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]) == VECRegisterType.DEMAND)
                    {
                        strSelfReadDescription += "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]);
                    }
                    else
                    {
                        strSelfReadDescription += GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]);
                    }
                    break;

                case 0x15: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]); break;
                case 0x17: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]); break;

                case 0x19:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]) == VECRegisterType.DEMAND)
                    {
                        strSelfReadDescription += "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]);
                    }
                    else
                    {
                        strSelfReadDescription += GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]);
                    }
                    break;

                case 0x1C: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]); break;
                case 0x1E: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]); break;

                case 0x20: strSelfReadDescription += GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]) + " Peak 2"; break;
                case 0x23: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]) + " Peak 2"; break;
                case 0x25: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_RATE_E]) + " Peak 2"; break;
                case 0x27: strSelfReadDescription += GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]) + " Peak 2"; break;
                case 0x2A: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]) + " Peak 2"; break;
                case 0x2C: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_RATE_E]) + " Peak 2"; break;
                case 0x2E: strSelfReadDescription += GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]) + " Peak 2"; break;
                case 0x31: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]) + " Peak 2"; break;
                case 0x33: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_3_RATE_E]) + " Peak 2"; break;
                case 0x35: strSelfReadDescription += GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]) + " Peak 2"; break;
                case 0x38: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]) + " Peak 2"; break;
                case 0x3A: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_4_RATE_E]) + " Peak 2"; break;

                case 0x3C: strSelfReadDescription += "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " Rate A"; break;
                case 0x3F: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " Rate A"; break;
                case 0x41: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " Rate A"; break;
                case 0x43: strSelfReadDescription += "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " Rate B"; break;
                case 0x46: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " Rate B"; break;
                case 0x48: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " Rate B"; break;
                case 0x4A: strSelfReadDescription += "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " Rate C"; break;
                case 0x4D: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " Rate C"; break;
                case 0x4F: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " Rate C"; break;
                case 0x51: strSelfReadDescription += "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " Rate D"; break;
                case 0x54: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " Rate D"; break;
                case 0x56: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_1_TOU]) + " Rate D"; break;

                case 0x58:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) == VECRegisterType.DEMAND)
                    {
                        strSelfReadDescription += "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate A";
                    }
                    else
                    {
                        strSelfReadDescription += GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate A";
                    }
                    break;

                case 0x5B: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate A"; break;
                case 0x5D: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate A"; break;

                case 0x5F:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) == VECRegisterType.DEMAND)
                    {
                        strSelfReadDescription += "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate B";
                    }
                    else
                    {
                        strSelfReadDescription += GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate B";
                    }
                    break;

                case 0x62: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate B"; break;
                case 0x64: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate B"; break;

                case 0x66:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) == VECRegisterType.DEMAND)
                    {
                        strSelfReadDescription += "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate C";
                    }
                    else
                    {
                        strSelfReadDescription += GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate C";
                    }
                    break;

                case 0x69: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate C"; break;
                case 0x6B: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate C"; break;

                case 0x6D:
                    if (GetQuantityType(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) == VECRegisterType.DEMAND)
                    {
                        strSelfReadDescription += "Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate D";
                    }
                    else
                    {
                        strSelfReadDescription += GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate D";
                    }
                    break;

                case 0x70: strSelfReadDescription += "Date of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate D"; break;
                case 0x72: strSelfReadDescription += "Time of Max " + GetQuantityDescription(m_eRegQuantities[(int)VECRegisters.REG_2_TOU]) + " Rate D"; break;

                default:
                    strSelfReadDescription += "Offset 0x" + nSelfReadOffset.ToString("X", CultureInfo.InvariantCulture);
                    break;
            }

            return strSelfReadDescription;
        }

        /// <summary>
        /// This method looks up the address of the maximum demand register address that is associated with
        /// the given cummulative demand register address.  This method is needed to calculate ccum demand values
        /// </summary>
        /// <param name="nCumDemandRegisterAddress"></param>
        /// <returns>
        /// The address of the associated maximum demand register
        /// </returns>
        private static int LookUpMaxDemandAddress(int nCumDemandRegisterAddress )
        {
            int nMaxDemandRegisterAddress = 0;

            switch (nCumDemandRegisterAddress)
            {
                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_E_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.LAST_SEASON_REG_1_RATE_E_MAX;
                    break;

                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_E_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.LAST_SEASON_REG_2_RATE_E_MAX;
                    break;

                case (int)VECAddresses.LAST_SEASON_REG_3_RATE_E_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.LAST_SEASON_REG_3_RATE_E_MAX;
                    break;

                case (int)VECAddresses.LAST_SEASON_REG_4_RATE_E_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.LAST_SEASON_REG_4_RATE_E_MAX;
                    break;

                case (int)VECAddresses.REG_1_RATE_A_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.REG_1_RATE_A_MAX;
                    break;

                case (int)VECAddresses.REG_1_RATE_B_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.REG_1_RATE_B_MAX;
                    break;

                case (int)VECAddresses.REG_1_RATE_C_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.REG_1_RATE_C_MAX;
                    break;

                case (int)VECAddresses.REG_1_RATE_D_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.REG_1_RATE_D_MAX;
                    break;

                case (int)VECAddresses.REG_2_RATE_A_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.REG_2_RATE_A_MAX;
                    break;

                case (int)VECAddresses.REG_2_RATE_B_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.REG_2_RATE_B_MAX;
                    break;

                case (int)VECAddresses.REG_2_RATE_C_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.REG_2_RATE_C_MAX;
                    break;

                case (int)VECAddresses.REG_2_RATE_D_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.REG_2_RATE_D_MAX;
                    break;

                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_A_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.LAST_SEASON_REG_1_RATE_A_MAX;
                    break;

                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_B_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.LAST_SEASON_REG_1_RATE_B_MAX;
                    break;

                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_C_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.LAST_SEASON_REG_1_RATE_C_MAX;
                    break;

                case (int)VECAddresses.LAST_SEASON_REG_1_RATE_D_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.LAST_SEASON_REG_1_RATE_D_MAX;
                    break;

                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_A_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.LAST_SEASON_REG_2_RATE_A_MAX;
                    break;

                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_B_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.LAST_SEASON_REG_2_RATE_B_MAX;
                    break;

                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_C_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.LAST_SEASON_REG_2_RATE_C_MAX;
                    break;

                case (int)VECAddresses.LAST_SEASON_REG_2_RATE_D_CUM:
                    nMaxDemandRegisterAddress = (int)VECAddresses.LAST_SEASON_REG_2_RATE_D_MAX;
                    break;
            }
            return nMaxDemandRegisterAddress;
        }

        #endregion
    }
}
