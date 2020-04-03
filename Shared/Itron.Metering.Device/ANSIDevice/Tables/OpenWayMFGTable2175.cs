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
//                           Copyright © 2006 - 2016
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
    /// MFG Table 2175 (Itron 127)
    /// 12 Max Demands Log Table.
    /// </summary>
    public class OpenWayMFGTable2175 : AnsiTable
    {
        #region Constants

        private const int SIZE_OF_LTIME_DATE = 5;
        private const int SIZE_OF_FLOAT = 4;
        private const int TOTAL_MAX_READS = 12;
        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object.</param>
        /// <param name="Table0">The Table 0 object for the current device.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/07/11 MSC  2.53.13  N/A    Created
        public OpenWayMFGTable2175(CPSEM psem, CTable00 Table0)
            : base(psem, 2175, GetTableSize(), TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/12 jrf  2.53.54 196345 Created to read table from EDL file.
        //
        public OpenWayMFGTable2175(PSEMBinaryReader reader)
            : base(2175, GetTableSize())
        {
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Determines the size of the table.
        /// </summary>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/07/11 MSC  2.53.13  N/A    Created
        //  04/09/12 jrf  2.53.54 196345 Removing unnecessary parameter for Table0.  
        //
        public static uint GetTableSize()
        {
            uint uiTableSize = 0;

            uiTableSize += SIZE_OF_LTIME_DATE;
            uiTableSize += SIZE_OF_FLOAT;
            uiTableSize *= TOTAL_MAX_READS;

            return uiTableSize;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/08/11 MSC  2.53.14  N/A    Created
        //  04/09/12 jrf  2.53.54 196345 Refactored to call ParseData(). 
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2175.Read");

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
        /// Retrieves the 12 Max Demand records.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/12 jrf  2.53.54 196345 Changing internal storage of max demand records from array to list. 
        //  05/11/16 AF   4.50.266 236165 Make sure that the table data is not cached so that the list is always up to date.
        //  05/18/16 AF   4.50.269 236165 Previous change broke EDL viewer so restored original code
        //
        public AMIMDERCD[] AMIMDERCDs
        {
            get
            {
                ReadUnloadedTable();

                return m_lstMaxDemandEntries.ToArray();
            }
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data read from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/12 jrf 2.53.54 196345 Created to support reading data from EDL file.
        //
        private void ParseData()
        {
            DateTime dtDefaultDate = new DateTime(1970, 1, 1, 0, 0, 0);
            
            m_DataStream.Position = 0;

            m_lstMaxDemandEntries.Clear();

            for (int index = 0; index < TOTAL_MAX_READS; index++)
            {
                AMIMDERCD MaxDemandEntry = new AMIMDERCD();

                MaxDemandEntry.DateOfDemandReset = m_Reader.ReadLTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME);

                MaxDemandEntry.MaxWattsReceived = m_Reader.ReadSingle();

                //Empty entries will have a default date.  No need to store or return them.
                if (dtDefaultDate != MaxDemandEntry.DateOfDemandReset)
                {
                    m_lstMaxDemandEntries.Add(MaxDemandEntry);
                }
            }
        }

        #endregion

        #region Member Variables

        private List<AMIMDERCD> m_lstMaxDemandEntries = new List<AMIMDERCD>();

        #endregion
    }

    /// <summary>
    /// Class that represents a single AMI Max Demand Entry
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  12/08/11 MSC  2.53.14  N/A    Created
    public class AMIMDERCD
    {
        #region Public Methods
        
        /// <summary>
        /// 
        /// </summary>
        public AMIMDERCD()
        {
            m_MaxWattsReceived = 0.0f;
            m_DateOfDemandReset = new DateTime(2000, 1, 1);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public float MaxWattsReceived
        {
            get
            {
                return m_MaxWattsReceived;
            }
            set
            {
                m_MaxWattsReceived = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DateOfDemandReset
        {
            get
            {
                return m_DateOfDemandReset;
            }
            set
            {
                m_DateOfDemandReset = value;
            }
        }

        #endregion

        #region Members

        private DateTime m_DateOfDemandReset;
        private float m_MaxWattsReceived;

        #endregion
    }
}
