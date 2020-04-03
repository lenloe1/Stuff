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
	/// <summary>
	/// Class representing the MT200 meter.
	/// </summary>
    /// <remarks>
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ ---------------------------------------
	/// 04/27/06 mrj 7.30.00  N/A	Created
	/// 05/30/06 jrf 7.30.00  N/A	Modified
    /// 11/22/06 mah 8.00.00 N/A Added properties to retrieve register readings
    /// </remarks>
    public partial class MT200 : SCSDevice
    {
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
 		/// 03/26/07 mah 8.00.21  2769  Added support for time remaining in demand subinterval - this is not supported
		///											   by the 200 series but could be in the display list
        /// </remarks>
        override internal int TranslateDisplayAddress(SCSDisplayItem displayItem)
        {
            int nAddress;

			if (displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.InstantaneousValue)
			{
				nAddress = 0; // Instantaneous values apply to CENTRONs only - don't try to retrieve them
			}
			else if (displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.TimeValue && 0x48 == displayItem.LowerAddress) // Time remaining in sub interval
			{
				nAddress = 0x00;  // This is a CENTRON only item - don't try to retrieve it either
			}
			else
			{
				switch (displayItem.UpperAddress)
				{
					case 0x01:
						if (displayItem.LowerAddress == 0x0C)
						{
							nAddress = 0x00FF; // Current day of week
						}
						else
						{
							nAddress = displayItem.LowerAddress;
						}
						break;
					case 0x08: nAddress = (0x0100 | displayItem.LowerAddress);
						break;
					case 0x09: nAddress = (0x0200 | displayItem.LowerAddress);
						break;
					case 0x0A: nAddress = (0x0300 | displayItem.LowerAddress);
						break;
					case 0x0B: nAddress = (0x0400 | displayItem.LowerAddress);
						break;
					default:
						nAddress = displayItem.LowerAddress;
						break;
				}
			}

            return nAddress;
        }

        /// <summary>
        /// This method returns a user viewable description for a given display item.
        /// </summary>
        /// <param name="displayItem">The display item </param>
        /// <returns>A string that describes the given display item
        /// </returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
		/// 03/09/07 mah 8.00.17           Added support for prs kW and prv kW even though they do not exist in a MT200
		/// 03/26/07 mah 8.00.21  2769  Added support for time remaining in demand subinterval - again this is not supported
		///											   by the 200 series but could be in the display list
        /// </remarks>
        override internal string GetDisplayItemDescription(SCSDisplayItem displayItem)
        {
			String strDescription = "";

			if (displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.DemandValue)
			{
				if (displayItem.RegisterType == 1)
				{
					strDescription = "prv kW";
				}
				else
				{
					strDescription = "prs kW";
				}
			}
			else if (displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.TotalContinuousCumulativeValue)
			{
				if (displayItem.RegisterType == 0x07)
				{
					strDescription = "Last Season ";
				}

				strDescription += "ccum " + DemandQuantity;

				// Add the TOU rate indeicator

				switch (displayItem.TOURate)
				{
					case 1: strDescription += " Rate A"; break;
					case 2: strDescription += " Rate B"; break;
					case 3: strDescription += " Rate C"; break;
					case 4: strDescription += " Rate D"; break;
					default: break;
				}
			}
			else if (displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.TimeValue && 0x48 == displayItem.LowerAddress ) // Time remaining in sub interval
			{
				strDescription = "Time Rem in Demand Subint";
			}
			else
			{
				strDescription = GetDisplayItemDescription(TranslateDisplayAddress(displayItem));
			}

            if ( strDescription.Length == 0 )
            {
                strDescription = base.GetDisplayItemDescription( displayItem );
            }

            return strDescription;
        }

        /// <summary>
        /// This property either 'W' or 'kW' to describe the demand values currently being measured
        /// based on the meter's display configuration.  THis property is useful when building up
        /// quantity descriptions
        /// </summary>
        protected String DemandQuantity
        {
            get
            {
                if (DemandFormat.Units == SCSDisplayFormat.DisplayUnits.Units)
                    return "W";
                else
                    return "kW";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nBasepageAddress"></param>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        override internal string GetDisplayItemDescription(int nBasepageAddress)
        {
            String strDescription;

            switch ( nBasepageAddress )
            {
                case 0x00FF: strDescription = "Current Day of Week"; break;
                case (int)MT2Addresses.RATEE_KWH: strDescription = "kWh d"; break;
                case (int)MT2Addresses.RATEE_CUM_KW: strDescription = "cum " + DemandQuantity; break;
                case 0x0124: strDescription = "Max " + DemandQuantity; break;
                case 0x0128: strDescription = "Date of Max " + DemandQuantity; break;
                case 0x012A: strDescription = "Time of Max " + DemandQuantity; break;
                case (int)MT2Addresses.LAST_RESET_DATE: strDescription = "Last Reset Date"; break;
                case 0x012E: strDescription = "Last Reset Time"; break;
                case (int)MT2Addresses.RESET_COUNT: strDescription = "Demand Reset Count"; break;
                case (int)MT2Addresses.OUTAGE_COUNT: strDescription = "Outage Count"; break;
                case (int)MT2Addresses.RATEA_CUM_KW: strDescription = "cum " + DemandQuantity + " Rate A"; break;
                case (int)MT2Addresses.RATEA_KW: strDescription = "Max " + DemandQuantity + " Rate A"; break;
                case 0x013C: strDescription = "Date of Max " + DemandQuantity + " Rate A"; break;
                case 0x013E: strDescription = "Time of Max " + DemandQuantity + " Rate A"; break;
                case (int)MT2Addresses.RATEB_CUM_KW: strDescription = "cum " + DemandQuantity + " Rate B"; break;
                case (int)MT2Addresses.RATEB_KW: strDescription = "Max " + DemandQuantity + " Rate B"; break;
                case 0x0148: strDescription = "Date of Max " + DemandQuantity + " Rate B"; break;
                case 0x014A: strDescription = "Time of Max " + DemandQuantity + " Rate B"; break;
                case 0x014C: strDescription = "Days Since Demand Reset"; break;
                case 0x014D: strDescription = "Transformer Ratio"; break;
                case (int)MT2Addresses.RATEC_CUM_KW: strDescription = "cum " + DemandQuantity + " Rate C"; break;
                case (int)MT2Addresses.RATEC_KW: strDescription = "Max " + DemandQuantity + " Rate C"; break;
                case 0x0158: strDescription = "Date of Max " + DemandQuantity + " Rate C"; break;
                case 0x015A: strDescription = "Time of Max " + DemandQuantity + " Rate C"; break;
                case 0x015C: strDescription = "Demand Threshold"; break;
                case (int)MT2Addresses.RATED_CUM_KW: strDescription = "cum " + DemandQuantity + " Rate D"; break;
                case (int)MT2Addresses.RATED_KW: strDescription = "Max " + DemandQuantity + " Rate D"; break;
                case 0x0168: strDescription = "Date of Max " + DemandQuantity + " Rate D"; break;
                case 0x016A: strDescription = "Time of Max " + DemandQuantity + " Rate D"; break;
                case 0x016C: strDescription = "Register Full-scale"; break;
                case (int)MT2Addresses.RATEA_KWH: strDescription = "kWh d Rate A"; break;
                case 0x0179: strDescription = "kWh d Rate B"; break;
                case 0x0180: strDescription = "kWh d Rate C"; break;
                case 0x0187: strDescription = "kWh d Rate D"; break;
                case 0x018F: strDescription = "Subinterval Length"; break;
                case 0x0190: strDescription = "Test Mode Subinterval Length"; break;
                case (int)MT2Addresses.MINUTES_ON_BATTERY: strDescription = "Minutes On Battery"; break;
                case (int)MT2Addresses.PROGRAM_COUNT: strDescription = "Program Count"; break;
                case (int)MT2Addresses.LAST_PROGRAMMED_DATE: strDescription = "Last Program Date"; break;
                case 0x019E: strDescription = "Last Program Time"; break;
                case (int)MT2Addresses.SELF_READ_RATEE_KWH: strDescription = "Self Read kWh"; break;
                case (int)MT2Addresses.SELF_READ_RATEE_KW: strDescription = "Self Read Max " + DemandQuantity; break;
                case (int)MT2Addresses.SELF_READ_RATEA_KWH: strDescription = "Self Read kWh Rate A"; break;
                case (int)MT2Addresses.SELF_READ_RATEA_KW: strDescription = "Self Read Max " + DemandQuantity + " Rate A"; break;
                case (int)MT2Addresses.SELF_READ_RATEB_KWH: strDescription = "Self Read kWh Rate B"; break;
                case (int)MT2Addresses.SELF_READ_RATEB_KW: strDescription = "Self Read Max " + DemandQuantity + " Rate B"; break;
                case (int)MT2Addresses.SELF_READ_RATEC_KWH: strDescription = "Self Read kWh Rate C"; break;
                case (int)MT2Addresses.SELF_READ_RATEC_KW: strDescription = "Self Read Max " + DemandQuantity + " Rate C"; break;
                case (int)MT2Addresses.SELF_READ_RATED_KWH: strDescription = "Self Read kWh Rate D"; break;
                case (int)MT2Addresses.SELF_READ_RATED_KW: strDescription = "Self Read Max " + DemandQuantity + " Rate D"; break;
				case (int)MT2Addresses.OUTPUT_PER_DISK_REV	: strDescription = "KYZ1 Output P/DR"; break;
                case 0x01C1: strDescription = "Normal Kh"; break;
                case (int)MT2Addresses.SOFTWARE_REVISION: strDescription = "Software Revision"; break;
                case (int)MT2Addresses.FIRMWARE_REVISION: strDescription = "Firmware Revision"; break;
                case (int)MT2Addresses.USERDEFINED_FIELD1: strDescription = "User Data 01"; break;
                case 0x020E: strDescription = "User Data 02"; break;
                case 0x0217: strDescription = "User Data 03"; break;
                case (int)MT2Addresses.PROGRAM_ID: strDescription = "Program ID"; break;
                case (int)MT2Addresses.SERIAL_NUMBER: strDescription = "Meter ID"; break;
                case 0x022B: strDescription = "Meter ID 2"; break;
                case 0x0481: strDescription = "Last Season kWh"; break;
                case 0x0484: strDescription = "Last Season Max " + DemandQuantity; break;
                case 0x0487: strDescription = "Last Season kWh Rate A"; break;
                case 0x048A: strDescription = "Last Season Max " + DemandQuantity + " Rate A"; break;
                case 0x048D: strDescription = "Last Season kWh Rate B"; break;
                case 0x0490: strDescription = "Last Season Max " + DemandQuantity + " Rate B"; break;
                case 0x0493: strDescription = "Last Season kWh Rate C"; break;
                case 0x0496: strDescription = "Last Season Max " + DemandQuantity + " Rate C"; break;
                case 0x0499: strDescription = "Last Season kWh Rate D"; break;
                case 0x049C: strDescription = "Last Season Max " + DemandQuantity + " Rate D"; break;

                case 0x04A0: strDescription = "Last Season Date of Max " + DemandQuantity; break;
                case 0x04A2: strDescription = "Last Season Time of Max " + DemandQuantity; break;
                case 0x04A4: strDescription = "Last Season Date of Max " + DemandQuantity + " Rate A"; break;
                case 0x04A6: strDescription = "Last Season Time of Max " + DemandQuantity + " Rate A"; break;
                case 0x04A8: strDescription = "Last Season Date of Max " + DemandQuantity + " Rate B"; break;
                case 0x04AA: strDescription = "Last Season Time of Max " + DemandQuantity + " Rate B"; break;
                case 0x04AC: strDescription = "Last Season Date of Max " + DemandQuantity + " Rate C"; break;
                case 0x04AE: strDescription = "Last Season Time of Max " + DemandQuantity + " Rate C"; break;

                case (int)MT2Addresses.LAST_SEASON_CUM_RATEE: strDescription = "Last Season cum " + DemandQuantity; break;
                case (int)MT2Addresses.LAST_SEASON_CUM_RATEA: strDescription = "Last Season cum " + DemandQuantity + " Rate A"; break;
                case (int)MT2Addresses.LAST_SEASON_CUM_RATEB: strDescription = "Last Season cum " + DemandQuantity + " Rate B"; break;
                case (int)MT2Addresses.LAST_SEASON_CUM_RATEC: strDescription = "Last Season cum " + DemandQuantity + " Rate C"; break;
                case (int)MT2Addresses.LAST_SEASON_CUM_RATED: strDescription = "Last Season cum " + DemandQuantity + " Rate D"; break;

                case 0x04C1: strDescription = "Last Season Date of Max " + DemandQuantity + " Rate D"; break;
                case 0x04C3: strDescription = "Last Season Time of Max " + DemandQuantity + " Rate D"; break;

                case (int)MT2Addresses.TOU_SCHEDULE_ID: strDescription = "TOU Schedule ID"; break;

                default: strDescription = "";
                    break;
            }

            return strDescription;
        }
		/// <summary>
		/// The MT200 does not have any present demand values - however the CENTRON does and it
		/// is possible to program an MT200 with a CENTRON's display list.  This method must be overriden
		/// to prevent attempts to read memory locations that simply are not present in the 200.
		/// </summary>
		/// <returns></returns>
		/// <remarks >
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/09/07 mah 8.00.00  N/A   Created
		/// </remarks>
		override internal string ReadPresentDemandValue(SCSDisplayItem displayItem)
		{
			return "";
		}

		/// <summary>
		/// The MT200 does not have any previous demand values - however the CENTRON does and it
		/// is possible to program an MT200 with a CENTRON's display list.  This method must be overriden
		/// to prevent attempts to read memory locations that simply are not present in the 200.
		/// </summary>
		/// <returns></returns>
		/// <remarks >
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/09/07 mah 8.00.00  N/A   Created
		/// </remarks>
		override internal string ReadPreviousDemandValue(SCSDisplayItem displayItem)
		{
			return "";
		}



        /// <summary>
        /// This method is responsible for calculating the continuous
        /// cummulative demand value associated with the given display item.  Note
		/// that the MT200 does not store this value so it must be calculated manually
        /// </summary>
        /// <param name="displayItem">The display item to look up</param>
        /// <returns>A string representing the ccum value
        /// </returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/03/07 mah 8.00.00  N/A   Created
		/// 03/07/07 mah 8.00.17  Implemented the method
        /// </remarks>
        override internal String RetrieveCCumValue(SCSDisplayItem displayItem)
        {
            // We need to calculate the ccum value ourselves and return it.  

            int nBCDLength;
			double dblCumDemand = 0.0;
            double dblMaxDemand = 0.0;
            int nCumDemandRegisterAddress;
            int nMaxDemandRegisterAddress;

			// Before reading the registers we have to take note of what type of register we are looking up
			// because last season registers are shorter than the current season registers - that's in addition
			// to being stored in completely different basepage addresses

			if (displayItem.RegisterType == 0x00)  
			{
				switch (displayItem.TOURate)
				{
					case 1: nMaxDemandRegisterAddress = (int)MT2Addresses.RATEA_KW;
						nCumDemandRegisterAddress = (int)MT2Addresses.RATEA_CUM_KW;
						break;
					case 2: nMaxDemandRegisterAddress = (int)MT2Addresses.RATEB_KW;
						nCumDemandRegisterAddress = (int)MT2Addresses.RATEB_CUM_KW;
						break;
					case 3: nMaxDemandRegisterAddress = (int)MT2Addresses.RATEC_KW;
						nCumDemandRegisterAddress = (int)MT2Addresses.RATEC_CUM_KW;
						break;
					case 4: nMaxDemandRegisterAddress = (int)MT2Addresses.RATED_KW;
						nCumDemandRegisterAddress = (int)MT2Addresses.RATED_CUM_KW;
						break;
					default: nMaxDemandRegisterAddress = (int)MT2Addresses.RATEE_KW;
						nCumDemandRegisterAddress = (int)MT2Addresses.RATEE_CUM_KW;
						break;
				}

				nBCDLength = 4; // All current registers are 4 bytes long
			}
			else
			{
				switch (displayItem.TOURate)
				{
					case 1: nMaxDemandRegisterAddress = (int)MT2Addresses.LAST_SEASON_RATEA_KW;
						nCumDemandRegisterAddress = (int)MT2Addresses.LAST_SEASON_CUM_RATEA;
						break;
					case 2: nMaxDemandRegisterAddress = (int)MT2Addresses.LAST_SEASON_RATEB_KW;
						nCumDemandRegisterAddress = (int)MT2Addresses.LAST_SEASON_CUM_RATEB;
						break;
					case 3: nMaxDemandRegisterAddress = (int)MT2Addresses.LAST_SEASON_RATEC_KW;
						nCumDemandRegisterAddress = (int)MT2Addresses.LAST_SEASON_CUM_RATEC;
						break;
					case 4: nMaxDemandRegisterAddress = (int)MT2Addresses.LAST_SEASON_RATED_KW;
						nCumDemandRegisterAddress = (int)MT2Addresses.LAST_SEASON_CUM_RATED;
						break;
					default: nMaxDemandRegisterAddress = (int)MT2Addresses.LAST_SEASON_RATEE_KW;
						nCumDemandRegisterAddress = (int)MT2Addresses.LAST_SEASON_CUM_RATEE;
						break;
				}

				nBCDLength = 3; // All last season registers are 3 bytes long
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

            return dblCCumDemand.ToString(CultureInfo.CurrentCulture);
        }
    }
}
