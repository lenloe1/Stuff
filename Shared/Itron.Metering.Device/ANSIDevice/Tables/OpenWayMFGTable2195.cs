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
using System.Threading;

namespace Itron.Metering.Device
{
    /// <summary>
    /// MFG Table 2195 (Itron 147) - Comm Module Table Data
    /// </summary>
    internal class OpenWayMFGTable2195 : AnsiTable
    {
        #region Constants

        private const int TABLE_READ_DELAY = 200;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="tableSize">The size of the Comm Module table</param>
        /// <param name="tableTimeout">The table timeout</param>
        /// <param name="commTableID">The Communication Module table to read.</param>
        /// <param name="table2194">The table 2194 object for the current meter.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        protected OpenWayMFGTable2195(CPSEM psem, uint tableSize, int tableTimeout, ushort commTableID, OpenWayMFGTable2194 table2194)
            : base(psem, 2195, tableSize, tableTimeout)
        {
            m_usCommTableID = commTableID;
            m_Table2194 = table2194;
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

            if (m_Table2194 != null)
            {
                // First we need to write the Table ID to Table 146 in order to populate 147 correctly.
                // We need to set this every time we do a read in case one of the other Comm Tables were
                // read from the meter.
                m_Table2194.CommTableID = m_usCommTableID;

                Result = m_Table2194.Write();

                if (Result == PSEMResponse.Ok)
                {
                    // Give the meter a couple of seconds to populate 2195
                    Thread.Sleep(TABLE_READ_DELAY);

                    // Now read 2195
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2195.Read");

                    Result = base.Read();

                    if (Result == PSEMResponse.Ok)
                    {
                        ParseData();
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Could not write to table 2194 since the object is null.");
            }

            return Result;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the data that was just read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/10 RCG 2.40.12 N/A    Created

        protected virtual void ParseData()
        {
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// The table ID for the Comm Module table to read.
        /// </summary>
        protected ushort m_usCommTableID;
        /// <summary>
        /// The Table 2194 object for the current meter.
        /// </summary>
        protected OpenWayMFGTable2194 m_Table2194;

        #endregion
    }
}
