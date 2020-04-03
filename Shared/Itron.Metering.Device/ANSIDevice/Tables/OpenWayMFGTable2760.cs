///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//  storage or retrieval system without the permission in writing from Itron, Inc.
//
//                           Copyright © 2016 - 2017
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    /// <summary>
    /// MFG Table 712 - Tamper Summary Table
    /// </summary>
    public class OpenWayMFGTable2760 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 24;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">Protocol instance being used by the device</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/17/16 AF  4.50.231 WR 419822  Created
        //
        public OpenWayMFGTable2760(CPSEM psem)
            : base(psem, 2760, TABLE_SIZE)
        {
            m_PSEM = psem;
            m_TimeFormat = (PSEMBinaryReader.TM_FORMAT)psem.TimeFormat;
        }


        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object</param>
        /// <param name="timeFormat">he time format used in the meter</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/17/16 AF  4.50.231 WR 419822  Created
        //
        public OpenWayMFGTable2760(PSEMBinaryReader reader, int timeFormat)
            : base(2760, TABLE_SIZE)
        {
            m_Reader = reader;
            m_TimeFormat = (PSEMBinaryReader.TM_FORMAT)timeFormat;

            ParseData();

            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Full read of 2760 from the meter
        /// </summary>
        /// <returns>The PSEM response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/17/16 AF  4.50.231 WR 419822  Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                // Populate the member variables that represent the table
                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The number of inversion tampers that have occurred
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/17/16 AF  4.50.231 WR 419822  Created
        //  11/27/17 AF  4.73.00  Task469275 Make sure we get a fresh read of the data, in case
        //                                   the tamper counts have been cleared
        //
        public byte InversionTamperCount
        {
            get
            {
                Read();

                return m_InversionCount;
            }
        }

        /// <summary>
        /// The timestamp of the latest inversion tamper
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/17/16 AF  4.50.231 WR 419822  Created
        //
        public DateTime LastInversionDateTime
        {
            get
            {
                ReadUnloadedTable();

                return m_LastInversionDateTime;
            }
        }

        /// <summary>
        /// The number of removal tampers that have occurred
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/17/16 AF  4.50.231 WR 419822  Created
        //  03/27/18 CFB 4.72.06  N/A        Replace Read unloaded table with Read to ensure we catch
        //                                   a change in removal tamper count during a programs lifetime.
        //
        public byte RemovalTamperCount
        {
            get
            {
                Read();

                return m_RemovalCount;
            }
        }

        /// <summary>
        /// The timestamp of the latest removal tamper
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/17/16 AF  4.50.231 WR 419822  Created
        //
        public DateTime LastRemovalDateTime
        {
            get
            {
                ReadUnloadedTable();

                return m_LastRemovalDateTime;
            }
        }

        /// <summary>
        /// The number of magnetic tampers that have occurred
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/17/16 AF  4.50.231 WR 419822  Created
        //
        public byte MagneticTamperCount
        {
            get
            {
                ReadUnloadedTable();

                return m_MagneticTamperCount;
            }
        }

        /// <summary>
        /// The timestamp of the latest magnetic tamper
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/17/16 AF  4.50.231 WR 419822  Created
        //
        public DateTime LastMagneticTamperDateTime
        {
            get
            {
                ReadUnloadedTable();

                return m_LastMagneticTamperDateTime;
            }
        }

        /// <summary>
        /// The number of magnetic tamper cleared events that have occurred
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/17/16 AF  4.50.231 WR 419822  Created
        //
        public byte MagneticTamperClearedCount
        {
            get
            {
                ReadUnloadedTable();

                return m_MagneticTamperClearedCount;
            }
        }

        /// <summary>
        /// The timestamp of the latest magnetic tamper cleared
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/17/16 AF  4.50.231 WR 419822  Created
        //
        public DateTime LastMagneticTamperClearedDateTime
        {
            get
            {
                ReadUnloadedTable();

                return m_LastMagneticTamperClearedDateTime;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data from the binary reader.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/17/16 AF  4.50.231 WR 419822  Created
        //
        private void ParseData()
        {
            if (m_Reader != null)
            {
                m_InversionCount = m_Reader.ReadByte();
                m_LastInversionDateTime = m_Reader.ReadLTIME(m_TimeFormat);
                m_RemovalCount = m_Reader.ReadByte();
                m_LastRemovalDateTime = m_Reader.ReadLTIME(m_TimeFormat);
                m_MagneticTamperCount = m_Reader.ReadByte();
                m_LastMagneticTamperDateTime = m_Reader.ReadLTIME(m_TimeFormat);
                m_MagneticTamperClearedCount = m_Reader.ReadByte();
                m_LastMagneticTamperClearedDateTime = m_Reader.ReadLTIME(m_TimeFormat);
            }
        }
        #endregion


        #region Members

        private byte m_InversionCount;
        private DateTime m_LastInversionDateTime;
        private byte m_RemovalCount;
        private DateTime m_LastRemovalDateTime;
        private byte m_MagneticTamperCount;
        private DateTime m_LastMagneticTamperDateTime;
        private byte m_MagneticTamperClearedCount;
        private DateTime m_LastMagneticTamperClearedDateTime;
        private PSEMBinaryReader.TM_FORMAT m_TimeFormat;

        #endregion

    }
}
