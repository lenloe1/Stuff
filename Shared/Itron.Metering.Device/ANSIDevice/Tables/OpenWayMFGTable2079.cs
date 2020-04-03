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
//                              Copyright © 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// MFG Table 2079 (Itron 31) 
    /// </summary>
    internal class OpenWayMFGTable2079 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 512;
        private const int TABLE_TIMEOUT = 1000;
        private const int MAGNETIC_TAMPER_OFFSET = 390;
        private const int MAGNETIC_TAMPER_BYTES = 2;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/18/12 jrf 2.70.18 TQ6657 Created
        //
        public OpenWayMFGTable2079(CPSEM psem)
            : base(psem, 2079, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Only partially reading part of table that contains 
        /// magnetic tamper counts.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/18/12 jrf 2.70.18 TQ6657 Created
        // 09/21/12 jrf 2.70.19 TQ6835 Removed setting table state to loaded so 
        //                             these values will be reread each time.
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2079.OffsetRead");

            Result = base.Read(MAGNETIC_TAMPER_OFFSET, MAGNETIC_TAMPER_BYTES);

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of magnetic tampers that have been detected.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/18/12 jrf 2.70.18 TQ6657 Created
        //
        public byte MagneticTamperDetectCount
        {
            get
            {
                ReadUnloadedTable();

                return m_bytMagneticTamperDetectCount;
            }
        }

        /// <summary>
        /// Gets the number of magnetic tampers that have been cleared.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/18/12 jrf 2.70.18 TQ6657 Created
        //
        public byte MagneticTamperClearedCount
        {
            get
            {
                ReadUnloadedTable();

                return m_byMagneticTamperClearedCount;
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
        // 09/18/12 jrf 2.70.18 TQ6657 Created
        //
        private void ParseData()
        {
            m_bytMagneticTamperDetectCount = m_Reader.ReadByte();
            m_byMagneticTamperClearedCount = m_Reader.ReadByte();
        }

        #endregion

        #region Member Variables

        private byte m_bytMagneticTamperDetectCount = 0;
        private byte m_byMagneticTamperClearedCount = 0;

        #endregion
    }
}
