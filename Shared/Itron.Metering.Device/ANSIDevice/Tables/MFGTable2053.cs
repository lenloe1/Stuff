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
//                              Copyright © 2006 - 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;
using Itron.Metering.TOU;
using System.Globalization;

namespace Itron.Metering.Device
{
	#region MFGTable 2053

    /// <summary>
    /// MFG Table 2053 - Previous Demand Reset Data - 2ND TO LAST
    /// </summary>
    public class MFGTable2053 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/14/10 SCW 9.70.18 N/A    Created for CENTRON II

        public MFGTable2053(CPSEM psem, CTable00 Table0, StdTable21 Table21)
            : base(psem, 2053, DetermineTableSize(Table0, Table21), TABLE_TIMEOUT)
        {
            m_Table0 = Table0;
            m_Table21 = Table21;

            m_DemandResetRegisterData = null;
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="table0">The table 0 object</param>
        /// <param name="table21">The table 21 object</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/14/10 SCW 9.70.18 N/A    Created for CENTRON II

        public MFGTable2053(PSEMBinaryReader reader, CTable00 table0, StdTable21 table21)
            : base(25, DetermineTableSize(table0, table21))
        {
            State = TableState.Loaded;
            m_Reader = reader;
            m_Table21 = table21;
            m_Table0 = table0;
            ParseData();
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/14/10 SCW 9.70.18 N/A    Created for CENTRON II

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "MFGTable2053.Read");

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the end date and time of the last demand reset.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/14/10 SCW 9.70.18 N/A    Created for CENTRON II

        public DateTime DemandResetDate
        {
            get
            {
                ReadUnloadedTable();
                return m_EndDateTime;
            }
        }

        /// <summary>
        /// Gets the season index for the last demand reset.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/14/10 SCW 9.70.18 N/A    Created for CENTRON II

        public byte DemandResetSeasonIndex
        {
            get
            {
                ReadUnloadedTable();
                return m_SeasonIndex;
            }
        }

        /// <summary>
        /// Gets the register data for the last demand reset.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/14/10 SCW 9.70.18 N/A    Created for CENTRON II

        public RegisterDataRecord DemandResetRegisterData
        {
            get
            {
                ReadUnloadedTable();
                return m_DemandResetRegisterData;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines that size of the table.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device.</param>
        /// <param name="Table21">The table 21 object for the current device.</param>
        /// <returns>The size of the table in bytes.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/14/10 SCW 9.70.18 N/A    Created for CENTRON II
		
        private static uint DetermineTableSize(CTable00 Table0, StdTable21 Table21)
        {
            uint uiTableSize = 0;

            // End Time
            if (Table21.IncludeDateTime)
            {
                uiTableSize += Table0.STIMESize;
            }

            // Season index
            if (Table21.IncludeSeasonInfo)
            {
                uiTableSize += 1;
            }

            // Demand Reset Data
            uiTableSize += RegisterDataRecord.Size(Table0, Table21);

            return uiTableSize;
        }

        /// <summary>
        /// Parse that data that was just read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/14/10 SCW 9.70.18 N/A    Created for CENTRON II

        private void ParseData()
        {
            if (m_Table21.IncludeDateTime)
            {
                m_EndDateTime = m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_Table0.TimeFormat);
            }

            if (m_Table21.IncludeSeasonInfo)
            {
                m_SeasonIndex = m_Reader.ReadByte();
            }

            m_DemandResetRegisterData = new RegisterDataRecord(m_Table0, m_Table21);

            m_DemandResetRegisterData.Parse(m_Reader);
        }

        #endregion

        #region Member Variables

        private CTable00 m_Table0;
        private StdTable21 m_Table21;

        private DateTime m_EndDateTime;
        private byte m_SeasonIndex;
        private RegisterDataRecord m_DemandResetRegisterData;

        #endregion
    }

    #endregion
}	
