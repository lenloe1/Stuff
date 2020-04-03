///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
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
//                           Copyright © 2006 - 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{

    #region Definitions

    /// <summary>
    /// Cpp Status
    /// </summary>
    public enum CppStatus : byte
    {
        /// <summary>
        /// Invalid
        /// </summary>
        [EnumDescription("Invalid")]
        Cpp_Zero_Invalid = 0,
        /// <summary>
        /// Unknown
        /// </summary>
        [EnumDescription("Configured")]
        Cpp_Configured = 1,
        /// <summary>
        /// Pending
        /// </summary>
        [EnumDescription("Pending")]
        Cpp_Pending = 2,
        /// <summary>
        /// Waiting for EOI Sync
        /// </summary>
        [EnumDescription("Waiting for EOI Sync")]
        Cpp_WaitingForEoiSync = 3,
        /// <summary>
        /// Active
        /// </summary>
        [EnumDescription("Active")]
        Cpp_Active = 4,
        /// <summary>
        /// Done on EOI
        /// </summary>
        [EnumDescription("Done on EOI")]
        Cpp_DoneOnEoi = 5,
        /// <summary>
        /// Done
        /// </summary>
        [EnumDescription("Done")]
        Cpp_Done = 6,
        /// <summary>
        /// Not Configured
        /// </summary>
        [EnumDescription("Not Configured")]
        Cpp_NotConfigured = 7
    }

    #endregion

    /// <summary>
    /// MFG Table 2360
    /// </summary>
    public class OpenWayMFGTable2360: AnsiTable
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
        //  12/10/10 JB          N/A    Created

        public OpenWayMFGTable2360(CPSEM psem, CTable00 Table0)
            : base(psem, 2360, GetTableSize(Table0), 300)
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
        //  12/10/10 JB          N/A    Created

        public static uint GetTableSize(CTable00 Table0)
        {
            uint uiTableSize = 4;

            uiTableSize += Table0.STIMESize;

            return uiTableSize;
        }

        /// <summary>
        /// Reads the table from the meter
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/10/10 JB          N/A    Created
        //  08/14/12 jrf 2.70.04 201852 Adjusting value returned for the CPP rate so the Rate 
        //                              enumeration can be used.
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2360.Read");

            PSEMResponse Result = base.Read();
            byte bytRate = 0;

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;
                                
                m_StartTimeGmt = m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                m_duration = m_Reader.ReadUInt16();
                m_status = (CppStatus)m_Reader.ReadByte();
                bytRate = (byte)(m_Reader.ReadByte() - 1); //Subtracting 1 so we can use the Rate enumeration

                if (true == Enum.IsDefined(typeof(Rate), bytRate))
                {
                    m_rate = (Rate)bytRate;
                }
                else
                {
                    m_rate = Rate.None;
                }                 
            }

            return Result;
        }
        
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the CPP Status
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/10/10 JB          N/A    Created
        public CppStatus Status
        {
            get
            {
                ReadUnloadedTable();
                return m_status;
            }
        }

        /// <summary>
        /// Gets the CPP Start time (GMT)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/10/10 JB          N/A    Created
        public DateTime StartTimeGmt
        {
            get
            {
                ReadUnloadedTable();
                return m_StartTimeGmt;
            }
        }

        /// <summary>
        /// Gets the CPP duration
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/10/10 JB          N/A    Created
        public UInt16 Duration
        {
            get
            {
                ReadUnloadedTable();
                return m_duration;
            }
        }

        /// <summary>
        /// Gets the CPP Rate
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/10/10 JB          N/A    Created
        public Rate Rate
        {
            get
            {
                ReadUnloadedTable();
                return m_rate;
            }
        }

        #endregion

        #region Member Variables

        private CppStatus m_status;
        private DateTime m_StartTimeGmt;
        private UInt16 m_duration;
        private Rate m_rate;

        #endregion
    }
}
