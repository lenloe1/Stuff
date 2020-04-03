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
    internal class OpenWayMFGTable2198 : AnsiTable
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM Communications object for the current session</param>
        /// <param name="Table0">The Table 0 object for the current meter</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/10 RCG 2.44.04 N/A    Created

        public OpenWayMFGTable2198(CPSEM psem, CTable00 Table0)
            : base(psem, 2198, GetTableSize(Table0), 300)
        {
        }

        /// <summary>
        /// Gets the size of the table in bytes
        /// </summary>
        /// <param name="Table0">The table 0 object for the current meter</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/10 RCG 2.44.04 N/A    Created

        public static uint GetTableSize(CTable00 Table0)
        {
            uint uiTableSize = 6;

            uiTableSize += Table0.LTIMESize;

            return uiTableSize;
        }

        /// <summary>
        /// Reads the table from the meter
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/10 RCG 2.44.04 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2198.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_byPinState = m_Reader.ReadByte();
                m_byOtherState = m_Reader.ReadByte();
                m_usEPFCounter = m_Reader.ReadUInt16();
                m_usPDNRequestCounter = m_Reader.ReadUInt16();
                m_dtTimeStamp = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
            }

            return Result;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the time stored in the display
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/10 RCG 2.44.04 N/A    Created

        public DateTime DisplayTime
        {
            get
            {
                ReadUnloadedTable();

                return m_dtTimeStamp;
            }
        }

        #endregion

        #region Member Variables

        private byte m_byPinState;
        private byte m_byOtherState;
        private ushort m_usEPFCounter;
        private ushort m_usPDNRequestCounter;
        private DateTime m_dtTimeStamp;

        #endregion
    }
}
