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
//                              Copyright © 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// MFG Table 72 (2220) - Factory Data Information
    /// </summary>
    public class OpenWayMFGTable2220 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 3;
        private const int TABLE_TIMEOUT = 500;

        private const byte LED_SEND_VAR_MASK = 0x10;
        private const byte LED_SEND_VA_MASK = 0x80;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/16/10 RCG 2.40.25		   Created

        public OpenWayMFGTable2220(CPSEM psem)
            : base(psem, 2220, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Constructor used by EDL file.
        /// </summary>
        /// <param name="binaryReader">The binary reader containing the table data.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/11/11 jrf 2.52.02        Created
        //
        public OpenWayMFGTable2220(PSEMBinaryReader binaryReader)
            : base(2220, TABLE_SIZE)
        {
            m_TableState = TableState.Loaded;
            m_Reader = binaryReader;
            ParseData();
        }

        /// <summary>
        /// Reads the sub table from the meter
        /// </summary>
        /// <returns>The response of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/16/10 RCG 2.40.25		   Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2220.Read");

            PSEMResponse Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the meter is Canadian
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/16/10 RCG 2.40.25		   Created

        public bool IsCanadian
        {
            get
            {
                ReadUnloadedTable();

                return m_bIsCanadian;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is sealed for Canada
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/16/10 RCG 2.40.25		   Created

        public bool IsSealedCanadian
        {
            get
            {
                ReadUnloadedTable();

                return m_bIsCanadianSealed;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is sealed for Canada directly.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/17 jrf 4.71.06 WR 742568 Created.
        public bool IsSealedCanadianFromOffsetRead
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] abytData;

                m_bIsCanadianSealed = false;

                PSEMResult = m_PSEM.OffsetRead(2220, 2, 1, out abytData);

                if (PSEMResponse.Ok != PSEMResult || null == abytData || 0 == abytData.Length)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading Is Sealed Canadian value"));
                }
                else
                {
                    if (0 == abytData[0])
                    {
                        m_bIsCanadianSealed = false;
                    }
                    else
                    {
                        m_bIsCanadianSealed = true;
                    }
                }

                return m_bIsCanadianSealed;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is currently pulsing Var on the LED
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/16/10 RCG 2.40.25		   Created

        public bool IsLEDPulsingVar
        {
            get
            {
                ReadUnloadedTable();

                return (m_byLEDStatus & LED_SEND_VAR_MASK) == LED_SEND_VAR_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the meter is currently pulsing VA on the LED
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/16/10 RCG 2.40.25		   Created

        public bool IsLEDPulsingVA
        {
            get
            {
                ReadUnloadedTable();

                return (m_byLEDStatus & LED_SEND_VA_MASK) == LED_SEND_VA_MASK;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data that has just been read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/16/10 RCG 2.40.25		   Created

        private void ParseData()
        {
            m_byLEDStatus = m_Reader.ReadByte();
            m_bIsCanadian = m_Reader.ReadBoolean();
            m_bIsCanadianSealed = m_Reader.ReadBoolean();
        }

        #endregion

        #region Member Variables

        private byte m_byLEDStatus;
        private bool m_bIsCanadian;
        private bool m_bIsCanadianSealed;

        #endregion
    }
}
