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
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Itron.Metering.Communications;
using Itron.Metering.Communications.SCS;

namespace Itron.Metering.Device
{
    public partial class CENTRON : MT200
    {
        #region Internal Methods
        /// <summary>
        /// This method is responsible for retrieving the basepage address of any given
        /// meter display item - normal, alternate, or test mode
        /// </summary>
        /// <param name="displayItem"></param>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        override internal int TranslateDisplayAddress(SCSDisplayItem displayItem)
        {
            int nAddress = 0;

            // Handle present and previous demands separately

            if (displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.EnergyValue && displayItem.DisplayType == DisplayMode.TEST_MODE)
            {
                nAddress = 0x0119;
            }
            else if (displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.MaxDemandValue && displayItem.DisplayType == DisplayMode.TEST_MODE)
            {
                nAddress = 0x0124;
            }
            else if (displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.DemandValue)
            {
                if (displayItem.RegisterType == 0) // present demand
                {
                    nAddress = 0x0561;
                }
                else // Previous demand
                {
                    nAddress = 0x0565;
                }
            }
            else if (displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.TotalContinuousCumulativeValue)
            {
                if (displayItem.RegisterType == 0) // current season
                {
                    switch (displayItem.TOURate)
                    {
                        case SCSDisplayItem.DISP_RATE_E: nAddress = 0x558; break;
                        case SCSDisplayItem.DISP_RATE_A: nAddress = 0x533; break;
                        case SCSDisplayItem.DISP_RATE_B: nAddress = 0x537; break;
                        case SCSDisplayItem.DISP_RATE_C: nAddress = 0x53B; break;
                        case SCSDisplayItem.DISP_RATE_D: nAddress = 0x53F; break;
                    }
                }
                else // last season
                {
                    switch (displayItem.TOURate)
                    {
                        case SCSDisplayItem.DISP_RATE_E: nAddress = 0x55C; break;
                        case SCSDisplayItem.DISP_RATE_A: nAddress = 0x543; break;
                        case SCSDisplayItem.DISP_RATE_B: nAddress = 0x546; break;
                        case SCSDisplayItem.DISP_RATE_C: nAddress = 0x549; break;
                        case SCSDisplayItem.DISP_RATE_D: nAddress = 0x54C; break;
                    }
                }
            }
            else if (0x0C == displayItem.UpperAddress)
            {
                nAddress = (0x0500 | displayItem.LowerAddress);
            }

            // If we were unable to determine the address at this point, the display item
            // must represent an item that is common to both the MT200 and the CENTRON
            // registers - call the base class to get its address

            if (0 == nAddress)
            {
                nAddress = base.TranslateDisplayAddress(displayItem);
            }

            return nAddress;
        }

        /// <summary>
        /// This method returns a user viewable description for a given display item.
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

            switch (nBasepageAddress)
            {
                case 0x0533: strDescription = "ccum " + DemandQuantity + " Rate A"; break;
                case 0x0537: strDescription = "ccum " + DemandQuantity + " Rate B"; break;
                case 0x053B: strDescription = "ccum " + DemandQuantity + " Rate C"; break;
                case 0x053F: strDescription = "ccum " + DemandQuantity + " Rate D"; break;
                case 0x0543: strDescription = "Last Season ccum " + DemandQuantity + " Rate A"; break;
                case 0x0546: strDescription = "Last Season ccum " + DemandQuantity + " Rate B"; break;
                case 0x0549: strDescription = "Last Season ccum " + DemandQuantity + " Rate C"; break;
                case 0x054C: strDescription = "Last Season ccum " + DemandQuantity + " Rate D"; break;
                case 0x0551: strDescription = "kWh r"; break;
                case 0x0558: strDescription = "ccum " + DemandQuantity; break;
                case 0x055C: strDescription = "Last Season ccum " + DemandQuantity; break;
                case 0x055F: strDescription = "Time Rem in Demand Subint"; break;
                case 0x0561: strDescription = "Present " + DemandQuantity; break;
                case 0x0565: strDescription = "Previous " + DemandQuantity; break;
                case 0x056A: strDescription = "Time Remaining in Test Mode"; break;
                case 0x056C: strDescription = "# Pulses Rcv'd in Test Mode"; break;
                case 0x056F: strDescription = "Prv # Pulses Rcv'd"; break;

                default: strDescription = base.GetDisplayItemDescription( nBasepageAddress );
                    break;
            }

            return strDescription;
        }

        /// <summary>
        /// This method is responsible for either retrieving or calculating the continuous
        /// cummulative demand value associated with the given display item.  Note
        /// that in the CENTRON this value is simply read from the meter.
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
            String strCCum = "";

            if (displayItem.RegisterType == 0) // current season
            {
                strCCum = ReadFloatingBCDValue(TranslateDisplayAddress(displayItem), 4);
            }
            else if (displayItem.RegisterType == 7) // last season
            {
                strCCum = ReadFloatingBCDValue(TranslateDisplayAddress(displayItem), 3);
            }

            return strCCum;
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// This method will create the appropriate display item type
        ///  (It can be overridden if needed)
        /// </summary>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/31/07 KRC 8.10.00  N/A   Need to create unique display item for CENTRON to handle CCum Edit
        /// </remarks>
        override protected SCSDisplayItem CreateDisplayItem(ref byte[] byDisplayTable, int nTableOffset, bool boolTestMode)
        {
            return new CENTRONDisplayItem(ref byDisplayTable, nTableOffset, boolTestMode);
        }

        #endregion

    }
}
