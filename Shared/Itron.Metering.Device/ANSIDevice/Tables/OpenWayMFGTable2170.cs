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
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;
using Itron.Metering.TOU;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Power Calculation Method
    /// </summary>
    public enum PowerCalculationType : byte
    {
        /// <summary>
        /// Arithmatic
        /// </summary>
        Arithmatic = 0,
        /// <summary>
        /// Vectorial
        /// </summary>
        Vectorial = 1,
    }

    internal class OpenWayMFGTable2170 : AnsiTable
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
        //  06/17/09 MMD           N/A    Created

        public OpenWayMFGTable2170(CPSEM psem, CTable00 Table0)
            : base(psem, 2170, OpenWayMFGTable2170.GetTableSize(Table0))
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
        //  06/17/09 MMD         N/A    Created

        public static uint GetTableSize(CTable00 Table0)
        {
            uint uiTableSize = 0;

            //Pulse_Weight_Normal
            uiTableSize +=2;
            //Pulse_Output_1_Quantity_Normal 
            uiTableSize +=4;
            //Pulse_Weight_Alt
            uiTableSize += 2;
            //Pulse_Output_1_Quantity_Alt 
            uiTableSize += 4;
            //Pulse_Weight_Test
            uiTableSize += 2;
            //Pulse_Output_1_Quantity_Test 
            uiTableSize += 4;
            //Pulse_Weight_Test_Alt
            uiTableSize += 2;
            //Pulse_Output_1_Quantity_Test_Alt 
            uiTableSize += 4;
            //appPowerCalcMethod
            uiTableSize += 1;
            //Dummy
            uiTableSize += 1;

            return uiTableSize;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/09 MMD         N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2170.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                try
                {
                    m_usPulseWeightNormal = m_Reader.ReadUInt16();
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_usPulseWeightNormal = 0;
                }

                try
                {
                    m_uiPulseQuantityNormal = m_Reader.ReadUInt32();
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_uiPulseQuantityNormal = 0;
                }

                try
                {
                    m_usPulseWeightAlt = m_Reader.ReadUInt16();
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_usPulseWeightAlt = 0;
                }
                try
                {
                    m_uiPulseQuantityAlt = m_Reader.ReadUInt32();
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_uiPulseQuantityAlt = 0;
                }

                try
                {
                    m_usPulseWeightTest = m_Reader.ReadUInt16();
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_usPulseWeightTest = 0;
                }

                try
                {
                    m_uiPulseQuantityTest = m_Reader.ReadUInt32();
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_uiPulseQuantityTest = 0;
                }

                try
                {
                    m_usPulseWeightTestAlt = m_Reader.ReadUInt16();
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_usPulseWeightTestAlt = 0;
                }

                try
                {
                    m_uiPulseQuantityTestAlt = m_Reader.ReadUInt32();
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_uiPulseQuantityTestAlt = 0;
                }

                try
                {
                    m_byappPowerCalcMethod = m_Reader.ReadByte();
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_byappPowerCalcMethod = 0;
                }

                try
                {
                    m_byDummy = m_Reader.ReadByte();
                }
                catch (Exception)
                {
                    // If an exception occurs populate the default value
                    m_byDummy = 0;
                }


            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the meter Test Quantity ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/09 MMD                Created

        public uint PulseQuantityTestID
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
                            "Error reading Meter CPC Data"));
                    }
                }

                return m_uiPulseQuantityTest;
            }
        }

        /// <summary>
        /// Gets the meter Test Quantity 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/09 MMD                Created

        public LEDQuantity PulseQuantityTest
        {
            get
            {

                return new LEDQuantity(PulseQuantityTestID);
            }
        }

        /// <summary>
        /// Gets the meter Test Kh
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/17/09 MMD                Created

        public ushort PulseWeightTest
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
                            "Error reading Meter CPC Data"));
                    }
                }

                return m_usPulseWeightTest;
            }
        }

        /// <summary>
        /// Gets the meter Normal Kh
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/23/09 MMD                Created

        public ushort PulseWeightNormal
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
                            "Error reading Meter CPC Data"));
                    }
                }

                return m_usPulseWeightNormal;
            }
        }

        /// <summary>
        /// Gets the current power calculation method
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/04/11 RCG 2.50.06        Created

        public PowerCalculationType PowerCalculationMethod
        {
            get
            {
                ReadUnloadedTable();

                return (PowerCalculationType)m_byappPowerCalcMethod;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_usPulseWeightNormal;
        private ushort m_usPulseWeightAlt;
        private ushort m_usPulseWeightTest;
        private ushort m_usPulseWeightTestAlt;
        private uint m_uiPulseQuantityNormal;
        private uint m_uiPulseQuantityAlt;
        private uint m_uiPulseQuantityTest;
        private uint m_uiPulseQuantityTestAlt;
        private byte m_byappPowerCalcMethod;
        private byte m_byDummy;
         

        #endregion

    }
}
