///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
// embodying substantial creative efforts and trade secrets, confidential 
// information, ideas and expressions. No part of which may be reproduced or 
// transmitted in any form or by any means electronic, mechanical, or 
// otherwise.  Including photocopying and recording or in connection with any 
// information storage or retrieval system without the permission in writing 
// from Itron, Inc.
//
//                           Copyright © 2006 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Resources;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Constructs a dictionary of common events for our ANSI devices
    /// </summary>
	public class ANSIEventDictionary : Dictionary<int, string>
    {
        #region Constants

        private const byte PHASE_A_MASK = 0x01;
        private const byte PHASE_B_MASK = 0x02;
        private const byte PHASE_C_MASK = 0x04;

        private const byte RATE_A_MASK = 0x01;
        private const byte RATE_B_MASK = 0x02;
        private const byte RATE_C_MASK = 0x03;
        private const byte RATE_D_MASK = 0x04;
        private const byte RATE_E_MASK = 0x05;
        private const byte RATE_F_MASK = 0x06;
        private const byte RATE_G_MASK = 0x07;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructs a dictionary of common events for our 
        /// ANSI devices. All ANSI devices must have events 0-76 listed in C12.19 spec.
        /// </summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version    Issue#      Description
        /// -------- --- -------    ------      ---------------------------------------
        /// 05/11/07 mcm 8.10.04  	 	        Created
        /// 07/02/09 AF  2.20.11    137349      Removed CUST_SCHED_CHANGED because this event
        ///                                      as a different meaning for OpenWay devices
        /// 10/13/09 MMD 2.30.09    141987      Changed the string representation of LOSS_VOLTAGE_A from
        ///                                         LOSS_OF_POTENTIAL to LOSS_OF_PHASE
        /// 05/11/10 jrf 2.41.01                Adding the Table Configuration event.
        /// 06/15/16 MP  4.50.284   WR680128    Promoted On Demand Periodic Read from base dictionary.
        /// 07/12/16 MP  4.70.7     WR688986    Changed how event descriptions were accessed
        /// 07/14/16 MP  4.70.7     WR688986    Removed commented code
        /// </remarks>
        public ANSIEventDictionary()
        {
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                            this.GetType().Assembly);

            int ANSIStdEventRange = 76;

            for (int StdEventID = 1; StdEventID <= ANSIStdEventRange; StdEventID++)
            {
                if (Enum.IsDefined(typeof(CANSIDevice.HistoryEvents), StdEventID))
                {
                    Add(StdEventID, m_rmStrings.GetString(Enum.GetName(typeof(CANSIDevice.HistoryEvents), StdEventID)));
                }
            }                             
        }

        /// <summary>
        /// TranslateEventData - Takes the Event Data and translates it to something human readable
        /// </summary>
        /// <param name="HistoryEvent">The Event whose argument data is being translated</param>
        /// <param name="ArgumentReader">The raw data we are translating</param>
        /// <returns>The Human Readable text</returns>
        // Revision History	
        // MM/DD/YY who Version   Issue#       Description
        // -------- --- -------   ------       ---------------------------------------
        // 05/01/08 KRC 1.50.20	  	           Created
        // 07/02/09 AF  2.20.11                Corrected the naming of SITESCAN_OR_PENDING_TABLE_CLEAR.
        //                                          This is a standard event and does not refer to SiteScan
        // 03/15/12 jrf 2.53.50   TREQ5571     Switched to pass as argument a HistoryEntry instead of just 
        //                                          the history code.
        // 10/12/12 MSC 2.70.28   TQ6684       Translating the Power Up and Power Down events.
        // 12/17/12 MSC 2.70.51   241238       Code Review Changes.
        // 02/23/15 jrf 4.10.04   566291       The power outage and restored event data was modified to 
        //                                          only show the power outage count if the first event argument is 1. 
        //                                          Anything higher than 1 is reseved.
        // 12/17/15 AF  4.23.00   559019       Adding the event argument data to the CLOCK_RESET event
        // 01/28/16 AF  4.50.224  RTT586620    Extending the event argument for the power restored event to support power up threshold
        // 04/18/16 CFB 4.50.248  WR625383     Changed fields "Power outage count" and "Power restored count" to "Outage ID" and "Power Restored ID" respectively 
        // 04/20/16 CFB 4.50.248  WR625383     Changed fields "Outage ID" and "Power Restored ID" to "Outage "Id"
        // 07/12/16 MP  4.70.7    WR688986     Changed POWER_OUTAGE and POWER_RESTORED to PRIMARY_POWER_DOWN and PRIMARY_POWER_UP
        public virtual string TranslatedEventData(HistoryEntry HistoryEvent, PSEMBinaryReader ArgumentReader)
        {
            String strData = "";

            switch (HistoryEvent.HistoryCode)
            {
                case (ushort)(CANSIDevice.HistoryEvents.CLOCK_RESET):
                {
                    // The time format must be set before this method is called
                    DateTime dtCurrentTime = ArgumentReader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)TimeFormat);
                    strData = "Current Time: " + dtCurrentTime.ToShortDateString() + " " + dtCurrentTime.ToLongTimeString();
                    break;
                }
                case (ushort)(CANSIDevice.HistoryEvents.PENDING_TABLE_ACTIVATION):
                case (ushort)(CANSIDevice.HistoryEvents.PENDING_TABLE_CLEAR):
                    {
                        strData = "Table: ";
                        strData += ArgumentReader.ReadInt16().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                case (ushort)(CANSIDevice.HistoryEvents.LOSS_OF_PHASE):
                case (ushort)(CANSIDevice.HistoryEvents.REVERSE_POWER_FLOW):
                    {
                        byte byArg = ArgumentReader.ReadByte();

                        strData = "Phase(s): ";
                        if (0 != (byArg & PHASE_A_MASK))
                        {
                            strData += " (A)";
                        }
                        if (0 != (byArg & PHASE_B_MASK))
                        {
                            strData += " (B)";
                        }
                        if (0 != (byArg & PHASE_C_MASK))
                        {
                            strData += " (C)";
                        }
                        break;
                    }

                case (ushort)(CANSIDevice.HistoryEvents.RATE_CHANGE):
                    {
                        byte byArg = ArgumentReader.ReadByte();

                        if (byArg == RATE_A_MASK)
                        {
                            strData += " (Rate A)";
                        }
                        else if (byArg == RATE_B_MASK)
                        {
                            strData += " (Rate B)";
                        }
                        else if (byArg == RATE_C_MASK)
                        {
                            strData += " (Rate C)";
                        }
                        else if (byArg == RATE_D_MASK)
                        {
                            strData += " (Rate D)";
                        }
                        else if (byArg == RATE_E_MASK)
                        {
                            strData += " (Rate E)";
                        }
                        else if (byArg == RATE_F_MASK)
                        {
                            strData += " (Rate F)";
                        }
                        else if (byArg == RATE_G_MASK)
                        {
                            strData += " (Rate G)";
                        }
                        break;
                    }
                case (ushort)(CANSIDevice.HistoryEvents.PRIMARY_POWER_DOWN):
                    {
                        byte Enabled = ArgumentReader.ReadByte();
                        if (Enabled == 1)
                        {
                            byte Counter = ArgumentReader.ReadByte();
                            strData += "Outage Id: " + Counter;
                        }
                        break;
                    }
                case (ushort)(CANSIDevice.HistoryEvents.PRIMARY_POWER_UP):
                    {
                        byte Enabled = ArgumentReader.ReadByte();
                        if (Enabled == 1)
                        {
                            byte Counter = ArgumentReader.ReadByte();
                            strData += "Outage Id: " + Counter;
                        }
                        else if (Enabled == 2)
                        {
                            byte reserved = ArgumentReader.ReadByte();
                            ushort NumberOfPowerCycles = ArgumentReader.ReadUInt16();
                            strData += "Number of Power Cycles: " + NumberOfPowerCycles.ToString(CultureInfo.CurrentCulture);
                        }
                        break;
                    }
                // KRC:TODO - Add other Events here that have data to interpret.
                default:
                    {
                        strData = "";
                        break;
                    }
            }

            return strData;
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the time format to be used for interpreting set clock event arguments
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  12/17/15 AF  4.23.00  WR 559019  Created
        //
        public int TimeFormat
        {
            get
            {
                if (m_iTimeFormat >= 0)
                {
                    return m_iTimeFormat;
                }
                else
                {
                    throw (new Exception("Time Format not set!"));
                }
            }
            set
            {
                m_iTimeFormat = value;
            }
        }

        #endregion

        private int m_iTimeFormat;

        /// <summary>String resource describing our events</summary>
        protected System.Resources.ResourceManager m_rmStrings;

        private readonly string RESOURCE_FILE_PROJECT_STRINGS =
                                    "Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings";

    }
}
