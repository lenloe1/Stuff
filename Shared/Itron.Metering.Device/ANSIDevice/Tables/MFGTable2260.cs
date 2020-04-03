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
    /// MFG Table 2260 - SR 3.0 TAMPER TAP Configuration and DST Schedule
    /// </summary>
    public class MFGTable2260 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;
        private const uint TABLE_SIZE = 512;
		
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/15/10 SCW 9.70.18 N/A    Created for CENTRON II

        public MFGTable2260(CPSEM psem)
            : base(psem, 2260, TABLE_SIZE, TABLE_TIMEOUT)
        {
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

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "MFGTable2260.Read");

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
        /// Gets the DST Hour
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/14/10 SCW 9.70.18 N/A    Created for CENTRON II

        public byte DST_HOUR
        {
            get
            {
                ReadUnloadedTable();
                return m_DST_Hour;
            }
        }

        /// <summary>
        /// Gets the DST Minute
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/14/10 SCW 9.70.18 N/A    Created for CENTRON II

        public byte DST_MINUTE
        {
            get
            {
                ReadUnloadedTable();
                return m_DST_Minute;
            }
        }

        /// <summary>
        /// Gets the register data for the last demand reset.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/14/10 SCW 9.70.18 N/A    Created for CENTRON II

        public byte DST_OFFSET
        {
            get
            {
                ReadUnloadedTable();
                return m_DST_Offset;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parse that data that was just read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/14/10 SCW 9.70.18 N/A    Created for CENTRON II

        private void ParseData()
        {
            // Tamper Tap Control and Fatal Error Recovery
            m_byTapControl = m_Reader.ReadByte();
            m_byInversionThreshold = m_Reader.ReadByte();
            m_byRemovalThreshold = m_Reader.ReadByte();
            m_byTapThreshold = m_Reader.ReadByte();
            m_uiWakeupDurationSecond = m_Reader.ReadUInt32();
            m_byFatalErrorRecoveryConfig = m_Reader.ReadByte();
            m_byAssetSynchConfig = m_Reader.ReadByte();
            m_usFiller1 = m_Reader.ReadUInt16();
            m_usFiller2 = m_Reader.ReadUInt16();
            m_usFiller3 = m_Reader.ReadUInt16();

            // DST Control Structure
            m_DST_Hour = m_Reader.ReadByte();
			m_DST_Minute = m_Reader.ReadByte();
			m_DST_Offset = m_Reader.ReadByte();
			
			// To do: to read DST Schedule in here
        }

        #endregion

        #region Member Variables

        // Tamper Tap Control and Fatal Error Recovery
        private byte m_byFatalErrorRecoveryConfig;
        private byte m_byTapControl;
        private byte m_byInversionThreshold;
        private byte m_byRemovalThreshold;
        private byte m_byTapThreshold;
        private UInt32 m_uiWakeupDurationSecond;
        private byte m_byAssetSynchConfig;

        private UInt16 m_usFiller1;
        private UInt16 m_usFiller2;
        private UInt16 m_usFiller3;

        // DST Control Structure
        private byte m_DST_Hour;
        private byte m_DST_Minute;
        private byte m_DST_Offset;
		
		// To do: add DST Schedule in here

        #endregion
    }

    #endregion
}	
