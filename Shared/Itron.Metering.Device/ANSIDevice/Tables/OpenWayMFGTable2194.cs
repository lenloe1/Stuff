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
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// MFG Table 2194 (Itron 146) - Comm Module Table ID
    /// </summary>
    internal class OpenWayMFGTable2194 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 2;
        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public OpenWayMFGTable2194(CPSEM psem)
            : base(psem, 2194, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Reads the table from the meter
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2194.Read");

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Writes the table to the meter.
        /// </summary>
        /// <returns>The result of the write</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "OpenWayMFGTable2194.Write");

            // Resynch our members to the base's data array
            m_DataStream.Position = 0;

            m_Writer.Write(m_usCommTableID);

            return base.Write();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Comm Module Table ID to read in table 2195
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        public ushort CommTableID
        {
            get
            {
                ReadUnloadedTable();

                return m_usCommTableID;
            }
            set
            {
                m_usCommTableID = value;

                State = TableState.Dirty;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data read by the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        private void ParseData()
        {
            m_usCommTableID = m_Reader.ReadUInt16();
        }

        #endregion

        #region Member Variables

        private ushort m_usCommTableID;

        #endregion
    }
}
