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
    /// MFG Table 2361
    /// </summary>
    internal class OpenWayMFGTable2361 : AnsiTable
    {
        #region Constants

        private const int TABLE_2361_SIZE = 16;

        #endregion

        #region Definitions

        /// <summary>
        /// Contains the bit masks for the DST Mode
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 06/20/13 DLG 2.80.41 TC15467 Created. 
        //
        private enum DSTModeBitMask : byte
        {
            DST_MODE = 0x01,
        }

        /// <summary>	
        /// Contains the bit masks for the BadTime
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  06/21/13 DLG 2.80.41 TC15467  Created.
        //
        private enum BadTimeBitMask : byte
        {
            BAD_TIME = 0x10,
        }

        #endregion

        #region Public Methods.

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/12/13 DLG 2.80.41 TC15467 Created.
        //
        public OpenWayMFGTable2361(CPSEM psem)
            : base(psem, 2361, TABLE_2361_SIZE)
        {

        }

        /// <summary>
        /// Full read of 2361 (MFG 313) from the meter
        /// </summary>
        /// <returns>The PSEM response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/12/13 DLG 2.80.41 TC15467 Created.
        //

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2361.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_LocalTime = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                m_GMTTime = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                m_DSTMode = m_Reader.ReadByte();
                m_TimeZoneOffset = m_Reader.ReadInt16();
                m_DSTAdjustment = m_Reader.ReadByte();
                m_BadTime = m_Reader.ReadByte();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The local time.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/13/13 DLG 2.80.41 TC15467 Created.
        //
        public DateTime LocalTime
        {
            get
            {
                Read();

                return m_LocalTime;
            }
        }

        /// <summary>
        /// The GMT time.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/13/13 DLG 2.80.41 TC15467 Created.
        //
        public DateTime GMTTime
        {
            get
            {
                Read();

                return m_GMTTime;
            }
        }

        /// <summary>
        /// DST Mode on or off.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/13/13 DLG 2.80.41 TC15467 Created.
        //
        public bool UseDSTMode
        {
            get
            {
                Read();

                // Mask off the bit that we need.
                byte byteValue = (byte)(m_DSTMode & (byte)DSTModeBitMask.DST_MODE);

                return (byteValue == (byte)DSTModeBitMask.DST_MODE);
            }
        }

        /// <summary>
        /// The Time Zone Offset.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/13/13 DLG 2.80.41 TC15467 Created.
        //
        public Int16 TimeZoneOffset
        {
            get
            {
                Read();

                return m_TimeZoneOffset;
            }
        }

        /// <summary>
        /// The DST Adjustment.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/13/13 DLG 2.80.41 TC15467 Created.
        //
        public UInt16 DSTAdjustment
        {
            get
            {
                Read();

                return m_DSTAdjustment;
            }
        }

        /// <summary>
        /// Returns true of false if the time is bad.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- -------------------------------------------
        //  06/13/13 DLG 2.80.41 TC15467 Created.
        //
        public bool BadTime
        {
            get
            {
                Read();

                // Mask off the bit that we need.
                byte byteValue = (byte)(m_BadTime & (byte)BadTimeBitMask.BAD_TIME);

                return (byteValue == (byte)BadTimeBitMask.BAD_TIME);
            }
        }

        #endregion

        #region Members

        private DateTime m_LocalTime;       // Example: 06/12/2013 16:35:18 
        private DateTime m_GMTTime;         // Example: 06/12/2013 20:35:18
        private byte m_DSTMode;             // Example: True 
        private Int16 m_TimeZoneOffset;     // Example: -300
        private byte m_DSTAdjustment;       // Example: 60
        private byte m_BadTime;             // Example: False

        #endregion
    }
}