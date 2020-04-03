///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and trade
//                                secrets of
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
//                           Copyright © 2006 - 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    internal class OpenWayMFGTable2168 : AnsiTable
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object.</param>
        /// <param name="Table0">The Table 0 object for the current device.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created

        public OpenWayMFGTable2168(CPSEM psem, CTable00 Table0) 
            : base (psem, 2168, OpenWayMFGTable2168.GetTableSize(Table0))
        {
        }

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <param name="Table0">Table 0 object for the current device.</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created

        public static uint GetTableSize(CTable00 Table0)
        {
            uint uiTableSize = 0;

            // We are going to assume that ID Format is always Char rather than BCD
            // METER_SN is ARRAY[16] OF CHAR
            uiTableSize += 16;
            // CUSTOMER_SN is ARRAY[20] OF CHAR
            uiTableSize += 20;
            // WATT_HOURS is NI_FMAT1 (FLOAT64)
            uiTableSize += 8;
            // WATT_DEMAND is NI_FMAT2 (FLOAT32)
            uiTableSize += 4;
            // TIME_STAMP is STIME_DATE
            uiTableSize += Table0.STIMESize;

            return uiTableSize;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2168.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                try
                {
                    m_strMeterSerialNumber = m_Reader.ReadString(16);
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_strMeterSerialNumber = "";
                }

                try
                {
                    m_strCustomerSerialNumber = m_Reader.ReadString(20);
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_strCustomerSerialNumber = "";
                }

                try
                {
                    m_dWattHours = m_Reader.ReadDouble();
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_dWattHours = 0;
                }

                try
                {
                    m_fWattDemand = m_Reader.ReadSingle();
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_fWattDemand = 0;
                }

                try
                {
                    m_dtTimeStamp = m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_dtTimeStamp = m_PSEM.ReferenceTime;
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the meter swap out Meter Serial Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created

        public string MeterSerialNumber
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Meter Swap Out Data"));
                    }
                }

                return m_strMeterSerialNumber;
            }
        }

        /// <summary>
        /// Gets the meter swap out Customer Serial Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created

        public string CustomerSerialNumber
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Meter Swap Out Data"));
                    }
                }

                return m_strCustomerSerialNumber;
            }
        }

        /// <summary>
        /// Gets the value of the meter swap out Wh energy value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created

        public double WattHours
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Meter Swap Out Data"));
                    }
                }

                return m_dWattHours;
            }
        }

        /// <summary>
        /// Gets the meter swap out W demand value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created

        public float WattDemand
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Meter Swap Out Data"));
                    }
                }

                return m_fWattDemand;
            }
        }

        /// <summary>
        /// Gets the date and time the meter was swapped out.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/05/08 RCG 1.50.22 N/A    Created

        public DateTime SwapOutTime
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Meter Swap Out Data"));
                    }
                }

                return m_dtTimeStamp;
            }
        }

        #endregion

        #region Member Variables

        private string m_strMeterSerialNumber;
        private string m_strCustomerSerialNumber;
        private double m_dWattHours;
        private float m_fWattDemand;
        private DateTime m_dtTimeStamp;

        #endregion

    }
}
