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
//                              Copyright © 2007 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Utilities
{

	/// <summary>
	/// Class used to get the device type list and string names.
	/// </summary>
    public sealed class DeviceType
    {

        #region Definitions

		/// <summary>
		/// Enumeration of device types
		/// </summary>
        public enum eDeviceTypes
        {
			/// <summary>
			/// CENTRON
			/// </summary>
            CENTRON,
			/// <summary>
			/// VECTRON
			/// </summary>
            VECTRON,
			/// <summary>
			/// SENTINEL
			/// </summary>
            SENTINEL,
			/// <summary>
			/// Q1000
			/// </summary>
            Q1000,
			/// <summary>
			/// QUANTUM
			/// </summary>
            QUANTUM,
			/// <summary>
			/// FULCRUM
			/// </summary>
            FULCRUM,
			/// <summary>
			/// SQ400
			/// </summary>
            SQ400,
			/// <summary>
			/// CENTRON_C12_19
			/// </summary>
            CENTRON_C12_19,
			/// <summary>
			/// CENTRON_V_AND_I
			/// </summary>
            CENTRON_V_AND_I,
			/// <summary>
			/// TWO_HUNDRED_SERIES
			/// </summary>
            TWO_HUNDRED_SERIES,
            /// <summary>
            /// CENTRONII_C12_19
            /// </summary>
            CENTRONII_C12_19,
            /// <summary>
			/// UNKNOWN
			/// </summary>
            UNKNOWN
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Given an eDeviceType enum, this method returns a string representation
        /// of the device type.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/13/07 RDB         N/A	   Created 
        public static string GetDeviceTypeString(eDeviceTypes e)
        {

            string strDeviceTypeString;

            switch (e)
            {
                case eDeviceTypes.TWO_HUNDRED_SERIES:
                    {
                        strDeviceTypeString = "200 Series";
                        break;
                    }
                case eDeviceTypes.CENTRON:
                    {
                        strDeviceTypeString = "CENTRON";
                        break;
                    }
                case eDeviceTypes.CENTRON_C12_19:
                    {
                        strDeviceTypeString = "CENTRON (C12.19)";
                        break;
                    }
                case eDeviceTypes.CENTRON_V_AND_I:
                    {
                        strDeviceTypeString = "CENTRON (V&I)";
                        break;
                    }
                case eDeviceTypes.FULCRUM:
                    {
                        strDeviceTypeString = "FULCRUM";
                        break;
                    }
                case eDeviceTypes.Q1000:
                    {
                        strDeviceTypeString = "Q1000";
                        break;
                    }
                case eDeviceTypes.QUANTUM:
                    {
                        strDeviceTypeString = "QUANTUM";
                        break;
                    }
                case eDeviceTypes.SENTINEL:
                    {
                        strDeviceTypeString = "SENTINEL";
                        break;
                    }
                case eDeviceTypes.SQ400:
                    {
                        strDeviceTypeString = "SQ400";
                        break;
                    }
                case eDeviceTypes.VECTRON:
                    {
                        strDeviceTypeString = "VECTRON";
                        break;
                    }
                case eDeviceTypes.UNKNOWN:
                    {
                        strDeviceTypeString = "Unknown";
                        break;
                    }
                default:
                    {
                        strDeviceTypeString = "";
                        break;
                    }
            }

            return strDeviceTypeString;

        }//end GetDeviceTypeString

        /// <summary>
        /// Given a string, this method returns a device type enum.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/02/07 RDB         N/A	   Created 
        public static eDeviceTypes GetDeviceTypeEnum(string str)
        {

            eDeviceTypes e;

            switch(str)
            {
                case "200 Series":
                    {
                        e = eDeviceTypes.TWO_HUNDRED_SERIES;
                        break;
                    }
                case "CENTRON":
                    {
                        e = eDeviceTypes.CENTRON;
                        break;
                    }
                case "CENTRON (C12.19)":
                    {
                        e = eDeviceTypes.CENTRON_C12_19;
                        break;
                    }
                case "CENTRON (V&I)":
                    {
                        e = eDeviceTypes.CENTRON_V_AND_I;
                        break;
                    }
                case "FULCRUM":
                    {
                        e = eDeviceTypes.FULCRUM;
                        break;
                    }
                case "Q1000":
                    {
                        e = eDeviceTypes.Q1000;
                        break;
                    }
                case "QUANTUM":
                    {
                        e = eDeviceTypes.QUANTUM;
                        break;
                    }
                case "SENTINEL":
                    {
                        e = eDeviceTypes.SENTINEL;
                        break;
                    }
                case "SQ400":
                    {
                        e = eDeviceTypes.SQ400;
                        break;
                    }
                case "VECTRON":
                    {
                        e = eDeviceTypes.VECTRON;
                        break;
                    }
                case "Unknown":
                    {
                        e = eDeviceTypes.UNKNOWN;
                        break;
                    }
                default:
                    {
                        e = eDeviceTypes.UNKNOWN;
                        break;
                    }
            }

            return e;

        }//end GetDeviceTypeEnum

		/// <summary>
		///     
		/// </summary>
		/// <param name="eType" type="Itron.Metering.Utilities.DeviceType.eDeviceTypes">
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		static public String GetDeviceTypeDescription( eDeviceTypes eType )
		{
			String strTypeDescription = "";

			switch (eType)
			{
				case DeviceType.eDeviceTypes.CENTRON:
					strTypeDescription = "The CENTRON® meter is a solid-state, single-phase residential electricity meter that provides utilities with unparalleled digital accuracy, reliability, serviceability and cost-effectiveness."  +
						"\\n" +
						"The CENTRON meter is one of the most adaptable meters on the residential market, providing an array of communications and application options to meet current and future business needs.";
					break;
				case DeviceType.eDeviceTypes.CENTRON_C12_19:
					strTypeDescription = "The CENTRON® meter is a solid-state, single-phase residential electricity meter that provides utilities with unparalleled digital accuracy, reliability, serviceability and cost-effectiveness. The CENTRON meter is one of the most adaptable meters on the residential market, providing an array of communications and application options to meet current and future business needs.";
					break;
				case DeviceType.eDeviceTypes.CENTRON_V_AND_I:
					strTypeDescription = "The CENTRON Polyphase is a solid-state, polyphase electricity meter for the commercial and industrial (C&I) market. The CENTRON Polyphase provides utilities with unparalleled digital accuracy and reliability as well as a flexible platform that allows utilities to integrate their low- and mid-tier C&I customers more easily into large-scale AMR systems.";
					break;
				case DeviceType.eDeviceTypes.FULCRUM:
					strTypeDescription = "Fulcrum description...";
					break;
				case DeviceType.eDeviceTypes.Q1000: 
					strTypeDescription = "The QUANTUM® Q1000 meter is designed to be used for revenue billing, power quality, information collection and retrieval, and system measurement applications. The meter's capability for complex measurements is ideal for large commercial, industrial, and utility power exchange points.";
					break;
				case DeviceType.eDeviceTypes.QUANTUM:
					strTypeDescription = "Quantum meters are...";
					break;
				case DeviceType.eDeviceTypes.SENTINEL:
					strTypeDescription = "The SENTINEL® is a solid-state, polyphase meter of exceptional accuracy. This self-contained or transformer-rated meter is designed for use with your high-end commercial and industrial (C&I) customers, including large industrial sites and substations.";
					break;
				case DeviceType.eDeviceTypes.SQ400:
					strTypeDescription = "The FULCRUM® SQ400 electronic multifunction meters are used for measuring polyphase energy and demand consumption at distribution substations and feeders. The SQ400 meters are transformer-rated and are available to meter 3-wire delta and 4-wire wye (2.5 and 3-element) services.";
					break;
				case DeviceType.eDeviceTypes.TWO_HUNDRED_SERIES:
					strTypeDescription = "200 Series devices are....";
					break;
				case DeviceType.eDeviceTypes.VECTRON: 
					strTypeDescription = "The VECTRON meter is an electronic device incorporating digital sampling technology to accurately measure power quantities. The meter is available in three versions: Demand, Time-of-use, and Extended Function.";
					break;

				default: // unknown device type
					break;
			}

			return strTypeDescription;
		}


        #endregion

    }//end DeviceType

}
