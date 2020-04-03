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
//                              Copyright © 2006 - 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Resources;

using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// The CTable00 class handles the reading of ANSI Table 00.  The reading of
    /// this table in the meter will be implicit. (read-only)
    /// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    // 08/01/06 AF  7.35.00 N/A    Created
    // 
    public class CTable00 : AnsiTable
    {
        #region Constants
        internal const string STD_REV_2 = "2.0";
        private const uint LTIME_NIL_SIZE = 0;
        private const uint LTIME_BCD_SIZE = 3;
        private const uint LTIME_UINT8_SIZE = 6;
        private const uint LTIME_UINT32_SIZE = 5;

        private const uint TIME_NIL_SIZE = 0;
        private const uint TIME_BCD_SIZE = 2;
        private const uint TIME_UINT8_SIZE = 3;
        private const uint TIME_UINT32_SIZE = 4;

        private const uint STIME_NIL_SIZE = 0;
        private const uint STIME_BCD_SIZE = 2;
        private const uint STIME_UINT8_SIZE = 2;
        private const uint STIME_UINT32_SIZE = 4;

        /// <summary>
        /// The length of the Table 0 header.
        /// </summary>
        protected const int TABLE_00_HEADER_LENGTH = 19;
        private const ushort MFG_ID_START = 2048;
        private const byte TM_FORMAT_MASK = 0x03;

        #endregion

        #region public methods

        /// <summary>
        /// Constructor, initializes the table
        /// </summary>
        /// <param name="psem">PSEM object for the current session</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// CTable00 Table0 = new CTable00( PSEM ); 
        /// </code>
        /// </example> 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/01/06 AF  7.40.00		    Created
        // 09/29/06 RCG 7.40.00         Updated to allow dynamic resizing
        // 
        public CTable00(CPSEM psem)
            : base(psem, 0, TABLE_00_HEADER_LENGTH)
        {
            // Set the Time format in the PSEMBinaryReader to be the value
            //	read from Table 00.
            psem.TimeFormat = TimeFormat;
        }

        /// <summary>
        /// Constructor to be used by Comm Module table 0 class
        /// </summary>
        /// <param name="psem">The PSEM object</param>
        /// <param name="TableID">The table ID</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/02/09 RCG 2.40.00		    Created

        protected CTable00(CPSEM psem, ushort TableID)
            : base(psem, TableID, TABLE_00_HEADER_LENGTH)
        {
        }

        /// <summary>
        /// Constructor used by EDL File
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        /// <param name="length">The length of the table</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        public CTable00(PSEMBinaryReader reader, uint length)
            : base(0, length)
        {
            State = TableState.Loaded;
            m_Reader = reader;
            ParseHeaderInfo();
            ParseTableInfo();
        }

        /// <summary>
        /// Constructor used by the Comm Module table 0 class for EDL files
        /// </summary>
        /// <param name="reader">The binary reader to use</param>
        /// <param name="TableID">The table ID</param>
        /// <param name="length">The length of the table</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/02/09 RCG 2.40.00		    Created

        protected CTable00(PSEMBinaryReader reader, ushort TableID, uint length)
            : base(TableID, length)
        {
            State = TableState.Loaded;
            m_Reader = reader;
            ParseHeaderInfo();
            ParseTableInfo();
        }

        /// <summary>
        /// Calls the base class method to perform a full read.
        /// </summary>
        /// <returns></returns>
        public PSEMResponse FullRead()
        {
            //Need automatic sizing because we don't know in advance how big this table will be.
            m_blnAllowAutomaticTableResizing = true;
            return base.Read();
        }


        /// <summary>
        /// Reads table 00 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/29/06 RCG 7.40.00 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Table " + m_TableID.ToString(CultureInfo.CurrentCulture));

            // Read the table - Since Read does a full read and checks the length
            // which should never be the same size as the whole table we need to do
            // an offset read here
            PSEMResponse Result = base.Read(0, (ushort)m_Size);


            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                // Save the Header Information
                ParseHeaderInfo();

                if (m_Size == TABLE_00_HEADER_LENGTH)
                {
                    // We have only read the header information so we need to 
                    // resize the table based on the Dim values and read the
                    // remaining data from the meter

                    // The first two values are used for both the TblsUsed and Write arrays
                    m_Size += ((uint)m_byDimStdTblsUsed * 2) + ((uint)m_byDimMfgTblsUsed * 2)
                        + (uint)m_byDimStdProcUsed + (uint)m_byDimMfgProcUsed;

                    // Resize the data array
                    byte[] ResizedData = new byte[m_Size];
                    Array.Copy(m_Data, 0, ResizedData, 0, m_Data.Length);
                    m_Data = ResizedData;

                    m_DataStream = new MemoryStream(m_Data);
                    m_Reader = new PSEMBinaryReader(m_DataStream);
                    m_Writer = new PSEMBinaryWriter(m_DataStream);

                    // Read the remainder of the data
                    Result = Read((ushort)(TABLE_00_HEADER_LENGTH), (ushort)(m_Size - TABLE_00_HEADER_LENGTH));

                    m_DataStream.Position = TABLE_00_HEADER_LENGTH;
                }

                // Save the remaining data from table 00
                if (Result == PSEMResponse.Ok)
                {
                    ParseTableInfo();

                    m_TableState = TableState.Loaded;
                }
            }

            return Result;
        }

        /// <summary>
        /// Determines whether or not the specified table is supported by the meter.
        /// </summary>
        /// <param name="TableID">The table number to check.</param>
        /// <returns>True if the table is supported. False if it is not supported</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/29/06 RCG 7.40.00 N/A    Created
        // 05/10/13 AF  2.80.29 391423 Adjusted the check on the table id to prevent array index out of bounds exceptions
        //
        public bool IsTableUsed(ushort TableID)
        {
            bool bResult = false;
            int iIndex;
            byte byBitMask;

            if (TableID < MFG_ID_START)
            {
                // Check to see if the standard table is used
                if (TableID < (8 * m_byDimStdTblsUsed))
                {
                    // There is enough data in the meter so we can check
                    // to see if it is used

                    // Determine which index the table is in
                    iIndex = TableID / 8;

                    // Determine which bit the index is in and create a mask for the location
                    byBitMask = (byte)(1 << (TableID % 8));

                    bResult = (byBitMask == (m_abyStdTblsUsed[iIndex] & byBitMask));
                }
            }
            else
            {
                // Get the MFG table number
                ushort MFGTableID = (ushort)(TableID - MFG_ID_START);

                // Check to see if the manufacturer table is used
                if (MFGTableID < (8 * m_byDimMfgTblsUsed))
                {
                    // There is enough data in the meter so we can check
                    // to see if it is used

                    // Determine which index the table is in
                    iIndex = MFGTableID / 8;

                    // Determine which bit the index is in and create a mask for the location
                    byBitMask = (byte)(1 << (MFGTableID % 8));

                    bResult = (byBitMask == (m_abyMfgTblsUsed[iIndex] & byBitMask));
                }
            }

            return bResult;
        }

        /// <summary>
        /// Determines whether or not the specified table supports write access
        /// </summary>
        /// <param name="TableID">The table number to check.</param>
        /// <returns>True if the table supports write access. False if it does not support write access</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/18/13 MAH 2.70.69 N/A    Created
        // 05/10/13 AF  2.80.29 391423 Adjusted the check on the table id to prevent array index out of bounds exceptions
        //
        public bool IsTableWriteable(ushort TableID)
        {
            bool bResult = false;
            int iIndex;
            byte byBitMask;

            if (TableID < MFG_ID_START)
            {
                // Check to see if the standard table is used
                if (TableID < (8 * m_byDimStdTblsUsed))
                {
                    // There is enough data in the meter so we can check
                    // to see if it is used

                    // Determine which index the table is in
                    iIndex = TableID / 8;

                    // Determine which bit the index is in and create a mask for the location
                    byBitMask = (byte)(1 << (TableID % 8));

                    bResult = (byBitMask == (m_abyStdTblsWrite[iIndex] & byBitMask));
                }
            }
            else
            {
                // Get the MFG table number
                ushort MFGTableID = (ushort)(TableID - MFG_ID_START);

                // Check to see if the manufacturer table is used
                if (MFGTableID < (8 * m_byDimMfgTblsUsed))
                {
                    // There is enough data in the meter so we can check
                    // to see if it is used

                    // Determine which index the table is in
                    iIndex = MFGTableID / 8;

                    // Determine which bit the index is in and create a mask for the location
                    byBitMask = (byte)(1 << (MFGTableID % 8));

                    bResult = (byBitMask == (m_abyMfgTblsWrite[iIndex] & byBitMask));
                }
            }

            return bResult;
        }


        /// <summary>
        /// Determines whether or not the procedure table is supported by the meter.
        /// </summary>
        /// <param name="procedureID">The procedure number to check.</param>
        /// <returns>True if the procedure is supported. False if it is not supported</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/12 RCG 2.53.41 N/A    Created
        // 05/01/13 AF  2.80.24 391423 Adjusted the check on the procedure id to prevent array index out of bounds exceptions
        //
        public bool IsProcedureUsed(ushort procedureID)
        {
            bool bResult = false;
            int iIndex;
            byte byBitMask;

            if (procedureID < MFG_ID_START)
            {
                // Check to see if the standard procedure is used
                if (procedureID < (8 * m_byDimStdProcUsed))
                {
                    // There is enough data in the meter so we can check
                    // to see if it is used

                    // Determine which index the table is in
                    iIndex = procedureID / 8;

                    // Determine which bit the index is in and create a mask for the location
                    byBitMask = (byte)(1 << (procedureID % 8));

                    bResult = (byBitMask == (m_abyStdProcUsed[iIndex] & byBitMask));
                }
            }
            else
            {
                // Get the MFG procedure number
                ushort MFGProcedureID = (ushort)(procedureID - MFG_ID_START);

                // Check to see if the manufacturer procedure is used
                if (MFGProcedureID < (8 * m_byDimMfgProcUsed))
                {
                    // There is enough data in the meter so we can check
                    // to see if it is used

                    // Determine which index the table is in
                    iIndex = MFGProcedureID / 8;

                    // Determine which bit the index is in and create a mask for the location
                    byBitMask = (byte)(1 << (MFGProcedureID % 8));

                    bResult = (byBitMask == (m_abyMfgProcUsed[iIndex] & byBitMask));
                }
            }

            return bResult;
        }

        /// <summary>
        /// Determines the size of an LTIME
        /// </summary>
        /// <param name="timeFormat">The time format used in the meter.</param>
        /// <returns>The size of the LTIME in bytes</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 01/14/10 RCG 2.40.06 N/A    Created

        public static uint DetermineLTIMESize(int timeFormat)
        {
            uint uiSize = 0;

            switch (timeFormat)
            {
                case 0: // Nil
                    {
                        uiSize = LTIME_NIL_SIZE;
                        break;
                    }
                case 1: // BCD
                    {
                        uiSize = LTIME_BCD_SIZE;
                        break;
                    }
                case 2: // UINT8
                    {
                        uiSize = LTIME_UINT8_SIZE;
                        break;
                    }
                case 3: // UINT32
                    {
                        uiSize = LTIME_UINT32_SIZE;
                        break;
                    }
                default:
                    {
                        throw new Exception("Time Format is not valid");
                    }
            }

            return uiSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Retrieves the Device Class of the meter (Used to be called Manufacturer)
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/07/08 KRC 1.51.02        Adding Filter based on Device Class
        //  02/08/10 AF  2.40.12        Made virtual so that it can be overridden
        //                              for table 2064
        //  
        public virtual string DeviceClass
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (m_TableState == TableState.Unloaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                    "Error reading Device Class"));
                    }
                }

                return m_strManufacturer;
            }
        }

        /// <summary>
        /// Retrieves the version of the standard tables in use in the meter
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/06 AF   7.??			Created
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //  
        public string StdVersion
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                    "Error reading Standard Version"));
                    }
                }

                return m_byStdVersionNo.ToString(CultureInfo.InvariantCulture) + "." + m_byStdRevisionNo.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// The number of bytes needed for a bitfield of the standard tables
        /// supported in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/06/06 AF   7.40			Created
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //  
        public byte DimStdTablesUsed
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                    "Error reading Dim Std Tables Used"));
                    }
                }

                return m_byDimStdTblsUsed;
            }
        }

        /// <summary>
        /// The number of bytes needed for a bitfield of the manufacturers tables
        /// supported in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/06/06 AF   7.40			Created
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //
        public byte DimMfgTablesUsed
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                    "Error reading Dim Mfg Tables Used"));
                    }
                }

                return m_byDimMfgTblsUsed;
            }
        }

        /// <summary>
        /// The number of bytes needed for a bitfield of the standard 
        /// procedures supported in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/10 AF  2.41.01        Created
        //
        public byte DimStdProceduresUsed
        {
            get
            {
                ReadUnloadedTable();

                return m_byDimStdProcUsed;
            }
        }

        /// <summary>
        /// The number of bytes needed for a bitfield of the manufacturers 
        /// procedures supported in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/10 AF  2.41.01        Created
        //
        public byte DimMfgProceduresUsed
        {
            get
            {
                ReadUnloadedTable();

                return m_byDimMfgProcUsed;
            }
        }

        /// <summary>
        /// The number of bytes needed for a bitfield of the manufacturers 
        /// statuses supported in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  07/27/15 jrf 4.20.18 WR 599965 Created
        public byte DimMfgStatusesUsed
        {
            get
            {
                ReadUnloadedTable();

                return m_byDimMfgStatusUsed;
            }
        }

        /// <summary>
        /// The number of pending tables supported in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/06/06 AF   7.40			Created
        //  04/27/09 AF   2.20.02       Changed the conditional before the read to
        //                              check that the table is not loaded rather than
        //                              checking that it is unloaded.  We probably want
        //                              to re-read a dirty or expired table.
        //
        public byte NumberPending
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                    "Error reading Dim Std Tables Used"));
                    }
                }

                return m_byNbrPending;
            }
        }

        /// <summary>
        /// Gets the size of an LTIME_DATE depending on the time format specified by the
        /// meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created
        // 01/14/10 RCG 2.40.06 N/A    Moved code to static method

        public uint LTIMESize
        {
            get
            {
                return DetermineLTIMESize(TimeFormat);
            }
        }

        /// <summary>
        /// Gets the size of a TIME type depending on the time format specified by the
        /// meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public uint TIMESize
        {
            get
            {
                uint uiSize = 0;

                switch (TimeFormat)
                {
                    case 0: // Nil
                    {
                        uiSize = TIME_NIL_SIZE;
                        break;
                    }
                    case 1: // BCD
                    {
                        uiSize = TIME_BCD_SIZE;
                        break;
                    }
                    case 2: // UINT8
                    {
                        uiSize = TIME_UINT8_SIZE;
                        break;
                    }
                    case 3: // UINT32
                    {
                        uiSize = TIME_UINT32_SIZE;
                        break;
                    }
                    default:
                    {
                        throw new Exception("Time Format is not valid");
                    }
                }

                return uiSize;
            }
        }

        /// <summary>
        /// Gets the size of an STIME type depending on the time format specified
        /// in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 02/14/07 RCG 8.00.12 N/A    Created

        public uint STIMESize
        {
            get
            {
                uint uiSize = 0;

                switch (TimeFormat)
                {
                    case 0: // Nil
                    {
                        uiSize = STIME_NIL_SIZE;
                        break;
                    }
                    case 1: // BCD
                    {
                        uiSize = STIME_BCD_SIZE;
                        break;
                    }
                    case 2: // UINT8
                    {
                        uiSize = STIME_UINT8_SIZE;
                        break;
                    }
                    case 3: // UINT32
                    {
                        uiSize = STIME_UINT32_SIZE;
                        break;
                    }
                    default:
                    {
                        throw new Exception("Time Format is not valid");
                    }
                }

                return uiSize;
            }
        }

        /// <summary>
        /// Gets the Time Format for this meter
        /// </summary>
        public int TimeFormat
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte byTimeFormat = 0;

                if (m_TableState == TableState.Unloaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                                 "Error Reading Format Control 2"));
                    }
                }

                byTimeFormat = (byte)(m_byFormatControl2 & TM_FORMAT_MASK);

                return (int)byTimeFormat;
            }
        }

        /// <summary>
        /// Produces a boolean array - the index corresponds to the procedure id
        /// and the value tells whether or not it is supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/23/10 AF  2.45.13        Created
        //
        [Obsolete("Using this property to determine whether or not a procedure is used can cause an IndexOutOfRangeException. Please use IsProcedureUsed(ushort procedureID)")]
        public bool[] StdProceduresUsed
        {
            get
            {
                byte byBitMask;
                int iProcId = 0;
                bool[] ablnStdProcSupported = new bool[m_byDimStdProcUsed * 8];

                ReadUnloadedTable();

                for (int iIndex = 0; iIndex < m_byDimStdProcUsed; iIndex++)
                {
                    for (int iBitIndex = 0; iBitIndex < 8; iBitIndex++)
                    {
                        byBitMask = (byte)(1 << iBitIndex);
                        if ((m_abyStdProcUsed[iIndex] & byBitMask) == byBitMask)
                        {
                            ablnStdProcSupported[iProcId] = true;
                        }
                        else
                        {
                            ablnStdProcSupported[iProcId] = false;
                        }

                        iProcId++;
                    }
                }

                return ablnStdProcSupported;
            }
        }

        /// <summary>
        /// Produces a boolean array - the index corresponds to the procedure id
        /// and the value tells whether or not it is supported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/23/10 AF  2.45.13        Created
        //

        [Obsolete("Using this property to determine whether or not a procedure is used can cause an IndexOutOfRangeException. Please use IsProcedureUsed(ushort procedureID)")]
        public bool[] MfgProceduresUsed
        {
            get
            {
                byte byBitMask;
                int iProcId = 0;
                bool[] ablnMfgProcSupported = new bool[m_byDimMfgProcUsed * 8];

                ReadUnloadedTable();

                for (int iIndex = 0; iIndex < m_byDimMfgProcUsed; iIndex++)
                {
                    for (int iBitIndex = 0; iBitIndex < 8; iBitIndex++)
                    {
                        byBitMask = (byte)(1 << iBitIndex);
                        if ((m_abyMfgProcUsed[iIndex] & byBitMask) == byBitMask)
                        {
                            ablnMfgProcSupported[iProcId] = true;
                        }
                        else
                        {
                            ablnMfgProcSupported[iProcId] = false;
                        }

                        iProcId++;
                    }
                }

                return ablnMfgProcSupported;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the header information
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        protected void ParseHeaderInfo()
        {
            m_byFormatControl1 = m_Reader.ReadByte();
            m_byFormatControl2 = m_Reader.ReadByte();
            m_byFormatControl3 = m_Reader.ReadByte();
            m_strManufacturer = m_Reader.ReadString(4);
            m_byNamePlateType = m_Reader.ReadByte();
            m_byDefaultSetUsed = m_Reader.ReadByte();
            m_byMaxProcParmLength = m_Reader.ReadByte();
            m_byMaxRespDataLen = m_Reader.ReadByte();
            m_byStdVersionNo = m_Reader.ReadByte();
            m_byStdRevisionNo = m_Reader.ReadByte();
            m_byDimStdTblsUsed = m_Reader.ReadByte();
            m_byDimMfgTblsUsed = m_Reader.ReadByte();
            m_byDimStdProcUsed = m_Reader.ReadByte();
            m_byDimMfgProcUsed = m_Reader.ReadByte();
            m_byDimMfgStatusUsed = m_Reader.ReadByte();
            m_byNbrPending = m_Reader.ReadByte();
        }

        /// <summary>
        /// Parses the table information
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/30/09 RCG 2.20.19 N/A    Created

        protected void ParseTableInfo()
        {
            m_abyStdTblsUsed = m_Reader.ReadBytes((int)m_byDimStdTblsUsed);
            m_abyMfgTblsUsed = m_Reader.ReadBytes((int)m_byDimMfgTblsUsed);
            m_abyStdProcUsed = m_Reader.ReadBytes((int)m_byDimStdProcUsed);
            m_abyMfgProcUsed = m_Reader.ReadBytes((int)m_byDimMfgProcUsed);
            m_abyStdTblsWrite = m_Reader.ReadBytes((int)m_byDimStdTblsUsed);
            m_abyMfgTblsWrite = m_Reader.ReadBytes((int)m_byDimMfgTblsUsed);
        }

        #endregion

        #region Member Variables

        //Table fields
        private byte m_byFormatControl1;
        private byte m_byFormatControl2;
        private byte m_byFormatControl3;
        private string m_strManufacturer;
        private byte m_byNamePlateType;
        private byte m_byDefaultSetUsed;
        private byte m_byMaxProcParmLength;
        private byte m_byMaxRespDataLen;
        private byte m_byStdVersionNo;
        private byte m_byStdRevisionNo;
        private byte m_byDimStdTblsUsed;
        private byte m_byDimMfgTblsUsed;
        private byte m_byDimStdProcUsed;
        private byte m_byDimMfgProcUsed;
        private byte m_byDimMfgStatusUsed;
        private byte m_byNbrPending;
        private byte[] m_abyStdTblsUsed;
        private byte[] m_abyMfgTblsUsed;
        private byte[] m_abyStdProcUsed;
        private byte[] m_abyMfgProcUsed;
        private byte[] m_abyStdTblsWrite;
        private byte[] m_abyMfgTblsWrite;
        //private bool[] m_ablnStdProcSupported;
        //private bool[] m_ablnMfgProcSupported;

        #endregion
    }

    /// <summary>
    /// The CTable1 class handles the reading of ANSI Table 1. The reading of 
    /// this table in the meter will be implicit.  (read-only)
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    // 07/13/05 mrj 7.13.00 N/A    Created
    //
    public class CTable01 : AnsiTable
    {
        #region public methods

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="psem">PSEM object for the current session</param>
        /// <param name="revision">Revision of the standard in use in the meter</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// CTable00 Table0 = new CTable00( PSEM );
        /// CTable01 Table1 = new CTable01( PSEM, Table0.StdVersion ); 
        /// </code>
        /// </example> 
        // Revision History	
        // MM/DD/YY who Version Issue#          Description
        // -------- --- ------- ------          ---------------------------------------
        // 07/13/05 mrj 7.13.00 N/A             Created
        // 08/07/06 AF  7.35.00 N/A	            Added revision param and call to GetTableLength()
        // 03/12/13 AF  2.80.08 TR7578, 7582    GE meters have std rev 2 but table size is not
        //                                      std rev 2 size, so allow size mismatch
        //
        public CTable01(CPSEM psem, string revision)
            : base(psem, 1, GetTableLength(revision))
        {
            m_strManufacturer = "";
            m_strModel = "";
            m_byHardwareVersion = 0;
            m_byHardwareRevision = 0;
            m_byFirmwareVersion = 0;
            m_byFirmwareRevision = 0;
            m_strSerialNumber = "";

            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Reads table 01 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/13/05 mrj 7.13.00 N/A    Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CTable01.Read");

            //Read the table			
            PSEMResponse Result = base.Read();


            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                //Populate the member variables that represent the table
                m_strManufacturer = m_Reader.ReadString(4);
                m_strModel = m_Reader.ReadString(8);
                m_byHardwareVersion = m_Reader.ReadByte();
                m_byHardwareRevision = m_Reader.ReadByte();
                m_byFirmwareVersion = m_Reader.ReadByte();
                m_byFirmwareRevision = m_Reader.ReadByte();
                m_strSerialNumber = m_Reader.ReadString(16);
            }


            return Result;
        }

        #endregion

        #region private methods

        /// <summary>
        /// Returns the size of Table 01 
        /// </summary>
        /// <param name="revision"></param>
        /// <returns></returns>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/07/06 AF          N/A    Created
        // 08/15/06 AF          N/A    rewrote to have just one return
        //
        static private uint GetTableLength(string revision)
        {
            uint uiTableLength = 0;

            if (0 > String.Compare(revision, CTable00.STD_REV_2, StringComparison.CurrentCulture))
            {
                uiTableLength = TABLE_01_LENGTH;
            }
            else
            {
                uiTableLength = (TABLE_01_LENGTH + 2);
            }
            return uiTableLength;
        }
        #endregion

        #region properties

        /// <summary>
        /// Gets the Manufacturer of the meter
        /// </summary>		
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// CTable00 Table0 = new CTable00( PSEM );
        /// CTable01 Table1 = new CTable01( PSEM, Table0.StdVersion );
        /// string strManufacturer = Table1.Manufacturer;
        /// </code>
        /// </example> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/13/05 mrj 7.13.00 N/A    Created
        //		
        public string Manufacturer
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                    "Error reading Manufacturer"));
                    }
                }

                return m_strManufacturer;
            }
        }

        /// <summary>
        /// Gets the model of the meter
        /// </summary>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// CTable00 Table0 = new CTable00( PSEM );
        /// CTable01 Table1 = new CTable01( PSEM, Table0.StdVersion );
        /// string strModel = Table1.Model;
        /// </code>
        /// </example>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/13/05 mrj 7.13.00 N/A    Created
        //		
        public string Model
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error Reading Table 01: Mode01"));
                    }
                }

                return m_strModel;
            }
        }

        /// <summary>
        /// Gets the firmware Revision of the meter
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/09/06 mcm 7.30.00 N/A    Created
        //		
        public float FW_Rev
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error Reading Table 01: Firmare Revision"));
                    }
                }

                return (m_byFirmwareVersion + m_byFirmwareRevision / 1000.0F);
            }
        }

        /// <summary>
        /// Gets the firmware version only
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  12/18/15 AF  4.50.222 RTT 587962 Created
        //
        public byte FWVersionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_byFirmwareVersion;
            }
        }

        /// <summary>
        /// Gets the firmware revision only
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  12/18/15 AF  4.50.222 RTT 587962 Created
        //
        public byte FWRevisionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_byFirmwareRevision;
            }
        }

        /// <summary>
        /// Gets the hardware Revision of the meter
        /// </summary>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/02/06 AF 7.35.00 N/A    Created
        //		
        public float HW_Rev
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Read())
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error Reading Table01: Hardware Revision"));
                    }
                }

                return (m_byHardwareVersion + m_byHardwareRevision / 1000.0F);
            }
        }

        /// <summary>
        /// Gets the Version of the hardware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/12/10 RCG 2.40.04 N/A    Created

        public byte HWVersionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_byHardwareVersion;
            }
        }

        /// <summary>
        /// Gets the Revision of the hardware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/12/10 RCG 2.40.04 N/A    Created

        public byte HWRevisionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_byHardwareRevision;
            }
        }

        /// <summary>
        /// Gets the serial number of the meter
        /// </summary>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// CTable00 Table0 = new CTable00( PSEM );
        /// CTable01 Table1 = new CTable01( PSEM, Table0.StdVersion );
        /// string strSerialNumber = Table1.SerialNumber;
        /// </code>
        /// </example>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/13/05 mrj 7.13.00 N/A    Created
        //		
        public String SerialNumber
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error Reading Table01: Serial Number"));
                    }
                }

                return m_strSerialNumber;
            }
        }

        #endregion

        #region public definitions

        #endregion public definitions

        #region private definitions

        private const int TABLE_01_LENGTH = 32;

        #endregion

        #region variable declarations

        //The table's member variables which represent the ANSI table 
        private string m_strManufacturer;
        private string m_strModel;
        private byte m_byHardwareVersion;
        private byte m_byHardwareRevision;
        private byte m_byFirmwareVersion;
        private byte m_byFirmwareRevision;
        private string m_strSerialNumber;

        #endregion
    }

    /// <summary>
    /// The CTable3 class handles the reading of ANIS Table 03. The reading of
    /// this table in the meter will be implicit. (read-only)
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  01/26/09 jrf 2.10.02 N/A    Created
    //
    public class CTable03 : AnsiTable
    {
        #region Constants

        private const int TABLE_03_FIXED_LENGTH = 4;
        private const int ED_MFG_STATUS_BYTE_1 = 0;
        private const int ED_MFG_STATUS_BYTE_2 = 1;
        private const int ED_MFG_STATUS_BYTE_3 = 2;
        private const int ED_MFG_STATUS_BYTE_4 = 3;
        private const int ED_MFG_STATUS_BYTE_5 = 4;
        private const string RESOURCE_FILE_PROJECT_STRINGS =
                                    "Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings";
        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Definitions

        /// <summary>
        /// Standard status bit field 1 values.
        /// </summary>
        private enum EDStdStatus1: ushort
        {
            CustomProgrammed = 0x01,
            ConfigurationError = 0x02,
            SelfTestError = 0x04,
            FatalError2 = 0x08,
            FatalError1 = 0x10,
            FatalError3 = 0x20,
            ClockError = 0x40,
            MeasurementElementError = 0x80,
            NonFatalError1 = 0x100,
            DiagnosticError2 = 0x200,
            DemandThresholdOverload = 0x400,
            PowerFailure = 0x800,
            TamperActivity = 0x1000,
            NonFatalError4 = 0x2000,
        }

        /// <summary>
        /// Manufacturing status bit field 1 values.
        /// </summary>
        private enum EDMFGStatus1 : byte
        {
            FatalError4 = 0x01,
            NonFatalError2 = 0x02,
            NonFatalError3 = 0x04,
            NonFatalError5 = 0x08,
            NonFatalError6 = 0x10,
            DiagnosticError1 = 0x20,
            DiagnosticError3 = 0x40,
            DiagnosticError4 = 0x80,

        }

        /// <summary>
        /// Manufacturing status bit field 2 values.
        /// </summary>
        private enum EDMFGStatus2: byte
        {
            DiagnosticError5 = 0x01,
            AlternateModeEntry = 0x02,
            NonFatalError9 = 0x08,
            DiagnosticError6 = 0x10,
            MeterReconfigure = 0x20,
            ANSISecurityFailed = 0x40,
            DemandReset = 0x80,
        }

        /// <summary>
        /// Manufacturing status bit field 3 values.
        /// </summary>
        private enum EDMFGStatus3 : byte
        {
            SeasonChange = 0x01,
            DemandThreshold1Exceeded = 0x02,
            DemandThreshold2Exceeded = 0x04,
            DemandThreshold3Exceeded = 0x08,
            DemandThreshold4Exceeded = 0x10,
            DemandThreshold1Restored = 0x20,
            DemandThreshold2Restored = 0x40,
            DemandThreshold3Restored = 0x80,
        }

        /// <summary>
        /// Manufacturing status bit field 4 values.
        /// </summary>
        private enum EDMFGStatus4 : byte
        {
            DemandThreshold4Restored = 0x01,
            InputChannel1High = 0x02,
            InputChannel2High = 0x04,
            InputChannel1Low = 0x08,
            InputChannel2Low = 0x10,
            VQEventOccurred = 0x20,
            VQLogNearlyFull = 0x40,
            DiagnosticLogFull = 0x80,
        }

        /// <summary>
        /// Manufacturing status bit field 5 values.
        /// </summary>
        private enum EDMFGStatus5 : byte
        {
            FatalError6 = 0x01,
            FatalError7 = 0x02,
            BillingSchedExpired = 0x04,
            FatalRecoveryEnabled = 0x08,
            InFatalRecoveryMode = 0x10,
        }

        /// <summary>
        /// Mode bit field values.
        /// </summary>
        private enum EDMode : byte
        {
            Metering = 0x01,
            Test = 0x02,
            MeterShop = 0x04,
            Factory = 0x08,
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="table00">ANSI Table 0.</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CTable03 Table3 = new CTable03( PSEM ); 
        /// </code>
        /// </example>
        //  Revision History	
        //  MM/DD/YY who Version ID Issue# Description
        //  -------- --- ------- -- ------ ---------------------------------------
        //  01/26/09 jrf 2.10.02 N/A    Created
        //  07/27/15 jrf 4.20.18 WR 599965 Passing in parameter to determine how 
        //                                 many manufacturer status bytes there are
        //                                 since this is not constant.
        // 07/30/15 jrf 4.50.178 WR 599965 Per code review passing in table 0.
        public CTable03(CPSEM psem, CTable00 table00)
            : base(psem, 3, GetTableLength(table00), TABLE_TIMEOUT)
        {
            InitMemberVariables(table00);
        }

        /// <summary>
        /// Constructor used when parsing an EDL file
        /// </summary>
        /// <param name="BinaryReader">Binary Reader for this current session.</param>
        /// <param name="table00">ANSI Table 0.</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CTable03 Table3 = new CTable03( PSEM ); 
        /// </code>
        /// </example>
        //  Revision History	
        //  MM/DD/YY who Version ID Issue# Description
        //  -------- --- ------- -- ------ ---------------------------------------
        //  01/26/09 jrf 2.10.02    N/A    Created
        //  07/27/15 jrf 4.20.18 WR 599965 Passing in parameter to determine how 
        //                                 many manufacturer status bytes there are
        //                                 since this is not constant.
        // 07/30/15 jrf 4.50.178 WR 599965 Per code review passing in table 0.
        public CTable03(PSEMBinaryReader BinaryReader, CTable00 table00)
            : base(3, GetTableLength(table00))
        {
            InitMemberVariables(table00);
            m_TableState = TableState.Loaded;
            m_Reader = BinaryReader;
            ParseData();
            GetErrors();
        }

        /// <summary>
        /// Reads table 03 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/26/09 jrf 2.10.02 N/A    Created
        // 
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CTable03.Read");

            //Read the table			
            PSEMResponse Result = base.Read();


            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            //Get the errors out of the meter after each read
            GetErrors();

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the errors list.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/26/09 jrf 2.10.02 N/A    Created.
        //  02/20/13 jrf 2.70.68 288152 Switched to return ReadOnlyCollection.
        // 	
        public ReadOnlyCollection<string> ErrorsList
        {
            get
            {
                ReadUnloadedTable();

                List<string> lstErrors = new List<string>();

                lstErrors.AddRange(m_lstNonFatalErrors);
                lstErrors.AddRange(m_lstFatalErrors);

                return lstErrors.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the list of non-fatal errors
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/19/13 jrf 2.70.68 288152 Created
        //
        public ReadOnlyCollection<string> NonFatalErrorsList
        {
            get
            {
                ReadUnloadedTable();

                return m_lstNonFatalErrors.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the list of fatal errors
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/31/09 RCG 2.21.05 132472 Created
        //  02/20/13 jrf 2.70.68 288152 Switched to return ReadOnlyCollection.
        public ReadOnlyCollection<string> FatalErrorsList
        {
            get
            {
                ReadUnloadedTable();

                return m_lstFatalErrors.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets an enumeration indicating fatal errors.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/19/13 jrf 2.70.68 288152 Created
        //
        public FatalErrors FatalErrorsSet
        {
            get
            {
                ReadUnloadedTable();

                return m_FatalErrors;
            }
        }

        /// <summary>
        /// Gets whether the meter is in test mode.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/05/09 jrf 2.20.08 135495 Created
        // 
        public bool InTestMode
        {
            get
            {
                ReadUnloadedTable();

                return((byte)EDMode.Test == (byte)(m_bytEDModeBfld & (byte)EDMode.Test));
            }
        }

        /// <summary>
        /// Gets whether or not Fatal Error Recovery is enabled in the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/06/10 RCG 2.40.02	    Created

        public bool IsFatalErrorRecoveryEnabled
        {
            get
            {
                ReadUnloadedTable();

                return (byte)EDMFGStatus5.FatalRecoveryEnabled == (byte)(m_abytEDMfgStatusBfld[ED_MFG_STATUS_BYTE_5]
                    & (byte)EDMFGStatus5.FatalRecoveryEnabled);
            }
        }

        /// <summary>
        /// Gets whether or not the meter is in Fatal Error Recovery Mode.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/06/10 RCG 2.40.02	    Created

        public bool IsInFatalErrorRecoveryMode
        {
            get
            {
                ReadUnloadedTable();

                return (byte)EDMFGStatus5.InFatalRecoveryMode == (byte)(m_abytEDMfgStatusBfld[ED_MFG_STATUS_BYTE_5]
                    & (byte)EDMFGStatus5.InFatalRecoveryMode);
            }
        }

        /// <summary>
        /// Gets whether or not the meter currently has a Full Core Dump available.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/06/10 RCG 2.40.02	    Created

        public bool IsFullCoreDumpAvailable
        {
            get
            {
                ReadUnloadedTable();

                return (byte)EDMFGStatus5.FatalError7 == (byte)(m_abytEDMfgStatusBfld[ED_MFG_STATUS_BYTE_5]
                    & (byte)EDMFGStatus5.FatalError7);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initalize all memeber variables.
        /// </summary>
        /// <param name="table00">ANSI Table 0.</param>
        //  Revision History	
        //  MM/DD/YY who Version ID Issue# Description
        //  -------- --- ------- -- ------ ---------------------------------------
        //  01/26/09 jrf 2.10.02    N/A    Created
        //  02/20/13 jrf 2.70.68 CQ 288152 Initializing new member variables.
        //  07/27/15 jrf 4.20.18 WR 599965 Passing in parameter to determine how 
        //                                 many manufacturer status bytes there are
        //                                 since this is not constant.
        // 07/30/15 jrf 4.50.178 WR 599965 Per code review passing in table 0.
        private void InitMemberVariables(CTable00 table00)
        {
          
            m_bytEDModeBfld = 0;
            m_uiEDStatus1Bfld = 0;
            m_bytEDStatus2Bfld = 0;
            m_abytEDMfgStatusBfld = new byte[table00.DimMfgStatusesUsed];
            m_abytEDMfgStatusBfld.Initialize();
            m_lstNonFatalErrors = new List<string>();
            m_lstFatalErrors = new List<string>();
            m_FatalErrors = FatalErrors.None;
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS, this.GetType().Assembly);
        }

        /// <summary>
        /// Retrieves the fatal and non-fatal errors that were read from the table.
        /// </summary>
        /// <remarks>All non-fatal and fatal errros will be retrieved except
        /// fatal error 5.  It's status is not listed in this table.</remarks>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/26/09 jrf 2.10.02 N/A    Created
        //  02/20/13 jrf 2.70.68 288152 Modified to create separate variables to track
        //                              non-fatal and fatal errors.  Added special logic 
        //                              for determining fatal 7.
        //  02/20/13 jrf 2.70.68 288152 Added clearing of new error variables.
        // 
        private void GetErrors()
        {
            //Clear the list in case this method is called more than once
            m_lstNonFatalErrors.Clear();
            m_lstFatalErrors.Clear();
            m_FatalErrors = FatalErrors.None;

            //Non-Fatal Errors
            if ((ushort)EDStdStatus1.NonFatalError1 == (ushort)(m_uiEDStatus1Bfld & (ushort)EDStdStatus1.NonFatalError1))
            {
                //Low battery
                m_lstNonFatalErrors.Add(m_rmStrings.GetString("NON_FATAL_1"));
            }

            if ((byte)EDMFGStatus1.NonFatalError2 == 
                (byte)(m_abytEDMfgStatusBfld[ED_MFG_STATUS_BYTE_1] & (byte)EDMFGStatus1.NonFatalError2))
            {
                //Loss of phase
                m_lstNonFatalErrors.Add(m_rmStrings.GetString("NON_FATAL_2"));
            }

            if ((byte)EDMFGStatus1.NonFatalError3 ==
                (byte)(m_abytEDMfgStatusBfld[ED_MFG_STATUS_BYTE_1] & (byte)EDMFGStatus1.NonFatalError3))
            {
                //Clock, TOU error
                m_lstNonFatalErrors.Add(m_rmStrings.GetString("NON_FATAL_3"));
            }

            if ((ushort)EDStdStatus1.NonFatalError4 == (ushort)(m_uiEDStatus1Bfld & (ushort)EDStdStatus1.NonFatalError4))
            {
                //Reverse power flow
                m_lstNonFatalErrors.Add(m_rmStrings.GetString("NON_FATAL_4"));
            }

            if ((byte)EDMFGStatus1.NonFatalError5 ==
                (byte)(m_abytEDMfgStatusBfld[ED_MFG_STATUS_BYTE_1] & (byte)EDMFGStatus1.NonFatalError5))
            {
                //Load profile error
                m_lstNonFatalErrors.Add(m_rmStrings.GetString("NON_FATAL_5"));
            }

            if ((byte)EDMFGStatus1.NonFatalError6 ==
                (byte)(m_abytEDMfgStatusBfld[ED_MFG_STATUS_BYTE_1] & (byte)EDMFGStatus1.NonFatalError6))
            {
                //Full scale overflow
                m_lstNonFatalErrors.Add(m_rmStrings.GetString("NON_FATAL_6"));
            }

            if ((byte)EDMFGStatus2.NonFatalError9 ==
                (byte)(m_abytEDMfgStatusBfld[ED_MFG_STATUS_BYTE_2] & (byte)EDMFGStatus2.NonFatalError9))
            {
                //SiteScan Error
                m_lstNonFatalErrors.Add(m_rmStrings.GetString("NON_FATAL_9"));
            }

            //Fatal Errors
            if ((ushort)EDStdStatus1.FatalError1 == (ushort)(m_uiEDStatus1Bfld & (ushort)EDStdStatus1.FatalError1))
            {
                //Processor Flash Error
                m_lstFatalErrors.Add(m_rmStrings.GetString("FATAL_1"));
                m_FatalErrors |= FatalErrors.FatalError1;
            }

            if ((ushort)EDStdStatus1.FatalError2 == (ushort)(m_uiEDStatus1Bfld & (ushort)EDStdStatus1.FatalError2))
            {
                //Processor RAM Error
                m_lstFatalErrors.Add(m_rmStrings.GetString("FATAL_2"));
                m_FatalErrors |= FatalErrors.FatalError2;
            }

            if ((ushort)EDStdStatus1.FatalError3 == (ushort)(m_uiEDStatus1Bfld & (ushort)EDStdStatus1.FatalError3))
            {
                //Data Flash Error
                //This could mean that fatal error 3 and/or 5 is set. What to do?
                //We don't know so we will just go with Fatal 3.
                m_lstFatalErrors.Add(m_rmStrings.GetString("FATAL_3"));
                m_FatalErrors |= FatalErrors.FatalError3;
            }

            if ((byte)EDMFGStatus1.FatalError4 ==
                (byte)(m_abytEDMfgStatusBfld[ED_MFG_STATUS_BYTE_1] & (byte)EDMFGStatus1.FatalError4))
            {
                //Metrology Communications Error
                m_lstFatalErrors.Add(m_rmStrings.GetString("FATAL_4"));
                m_FatalErrors |= FatalErrors.FatalError4;
            }

            //Hey! What about fatal error 5?  Check comments for fatal error 3.
            
            if ((byte)EDMFGStatus5.FatalError6 ==
                (byte)(m_abytEDMfgStatusBfld[ED_MFG_STATUS_BYTE_5] & (byte)EDMFGStatus5.FatalError6))
            {
                //File System Error
                m_lstFatalErrors.Add(m_rmStrings.GetString("FATAL_6"));
                m_FatalErrors |= FatalErrors.FatalError6;
            }

            //Table 3's Fatal 7 flag really just indicates core dump is available. 
            //But if it is the only fatal flag that has been set then there must be a fatal 7
            if ((byte)EDMFGStatus5.FatalError7 ==
                (byte)(m_abytEDMfgStatusBfld[ED_MFG_STATUS_BYTE_5] & (byte)EDMFGStatus5.FatalError7)
                && FatalErrors.None == m_FatalErrors)
            {
                //Operating System Error
                m_lstFatalErrors.Add(m_rmStrings.GetString("FATAL_7"));
                m_FatalErrors |= FatalErrors.FatalError7;
            }
        }
        
        /// <summary>
        /// Parses the data out of the reader and into the member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version ID Issue# Description
        //  -------- --- ------- -- ------ ---------------------------------------
        //  01/26/09 jrf 2.10.02    N/A    Created
        //  07/27/15 jrf 4.20.18 WR 599965 Using length of m_abytEDMfgStatusBfld to determine
        //                                 how many bytes to read since this is not 
        //                                 constant.
        private void ParseData()
        {
            // Populate the member variables that represent the table
           
            m_bytEDModeBfld = m_Reader.ReadByte();
            m_uiEDStatus1Bfld = m_Reader.ReadUInt16();
            m_bytEDStatus2Bfld = m_Reader.ReadByte();
            m_abytEDMfgStatusBfld = m_Reader.ReadBytes(m_abytEDMfgStatusBfld.Length);
        }

        /// <summary>
        /// Returns the size of Table 03 
        /// </summary>
        /// <param name="table00">ANSI table 0.</param>
        /// <returns></returns>
        // MM/DD/YY who Version ID Issue# Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 07/27/15 jrf 4.20.18 WR 599965 Created
        // 07/30/15 jrf 4.50.178 WR 599965 Per code review passing in table 0.
        static private uint GetTableLength(CTable00 table00)
        {
            return (uint)(TABLE_03_FIXED_LENGTH + table00.DimMfgStatusesUsed);
        }

        #endregion

        #region Members
       
        private byte m_bytEDModeBfld;
        private UInt16 m_uiEDStatus1Bfld;
        private byte m_bytEDStatus2Bfld;
        private byte[] m_abytEDMfgStatusBfld;
        private List<string> m_lstNonFatalErrors;
        private List<string> m_lstFatalErrors;
        private FatalErrors m_FatalErrors;
        private System.Resources.ResourceManager m_rmStrings;

        #endregion
    }

    /// <summary>
    /// The CTable04 class handles the reading of ANSI Table 04.  The reading of
    /// this table in the meter will be implicit. (read-only)
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 09/22/06 AF  7.40.00 N/A    Created
    //
    internal class CTable04 : AnsiTable
    {
        #region Constants

        private const int SIZE_OF_ENTRY_ACTIVATION_RCD = 8;
        private const int SIZE_OF_STIME_DATE = 4;
        private const int SIZE_OF_NBR_PENDING_ACTIVATION = 1;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor, initializes the table
        /// </summary>
        /// <param name="psem">PSEM object for the current session</param>
        /// <param name="bytDimMfgTblsUsed"></param>
        /// <param name="bytDimStdTblsUsed"></param>
        /// <param name="bytNumPending"></param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// CTable04 Table04 = new CTable04( PSEM ); 
        /// </code>
        /// </example> 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/25/06 AF  7.40.00 N/A    Created
        //  10/26/06 AF  7.40.00        Made cached data types dynamic
        //
        public CTable04(CPSEM psem, byte bytDimStdTblsUsed, byte bytDimMfgTblsUsed, byte bytNumPending)
            : base(psem, 4, GetTableLength(bytDimStdTblsUsed, bytDimMfgTblsUsed, bytNumPending), 250)
        {
            m_abytSetStdPending = new byte[bytDimStdTblsUsed];
            m_abytSetMfgPending = new byte[bytDimMfgTblsUsed];
            m_dtLastActivationDate = new DateTime(1970, 1, 1, 0, 0, 0);
            m_bytNumberPendingActivation = 0;
            m_EntryActivationRcds = new List<PendingEventActivationRecord>();
            m_bytNumPendingSupported = bytNumPending;
        }

        /// <summary>
        /// Reads table 04 out of the meter
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/25/06 AF  7.40.00 N/A    Created
        //  10/05/06 AF  7.40.00 N/A    Added code to determine the pending flag
        //
        public override PSEMResponse Read()
        {
            int intIndex;
            int intTemp;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable04.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;
                m_abytSetStdPending = m_Reader.ReadBytes(m_abytSetStdPending.Length);
                m_abytSetMfgPending = m_Reader.ReadBytes(m_abytSetMfgPending.Length);
                m_dtLastActivationDate = m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                m_bytNumberPendingActivation = m_Reader.ReadByte();
                m_EntryActivationRcds.Clear();

                for (intIndex = 0; intIndex < m_bytNumPendingSupported; intIndex++)
                {
                    PendingEventActivationRecord evtRcd = new PendingEventActivationRecord();
                    evtRcd.Event.EventSelector = m_Reader.ReadByte();
                    evtRcd.Event.EventStorage = m_Reader.ReadBytes(5);
                    evtRcd.TableID = m_Reader.ReadUInt16();

                    // strip out the pending flag
                    intTemp = evtRcd.TableID & 0x1000;
                    if (0 != intTemp)
                    {
                        evtRcd.StillPending = true;
                    }
                    else
                    {
                        evtRcd.StillPending = false;
                    }

                    // mask off the pending flag
                    evtRcd.TableID = (ushort)(evtRcd.TableID & 0xFFF);

                    m_EntryActivationRcds.Add(evtRcd);
                }

            }
            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the date of the last pending table activation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/25/06 AF  7.40.00 N/A    Created
        //
        public DateTime LastActivationDate
        {
            get
            {
                ReadUnloadedTable();

                return m_dtLastActivationDate;
            }
        }

        /// <summary>
        /// Gets the number of tables currently pending activation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/25/06 AF  7.40.00 N/A    Created
        //  10/26/06 AF  7.40.00        Removed check on table state so that it
        //                              will always be reread.  We can assume that
        //                              a request for this field means that the table
        //                              should be reread.
        //
        public byte NumberPendingTables
        {
            get
            {
                ReadUnloadedTable();

                return m_bytNumberPendingActivation;
            }
        }

        /// <summary>
        /// Maximum number of pending tables supported in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/06/06 AF  7.40.00 N/A    Created
        //
        public byte NumberPendingSupported
        {
            get
            {
                ReadUnloadedTable();

                return m_bytNumPendingSupported;
            }
        }

        /// <summary>
        /// Returns a collection object representing the raw table 04 activation record data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/25/06 AF  7.40.00 N/A    Created
        //
        public List<PendingEventActivationRecord> PendingTableEntries
        {
            get
            {
                ReadUnloadedTable();

                return m_EntryActivationRcds;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of Table 04 based on data read from Table 00
        /// </summary>
        /// <param name="bytDimStdTblsUsed">Number of bytes needed for a bit field
        /// designating all the supported standard tables</param>
        /// <param name="bytDimMfgTblsUsed">Number of bytes needed for a bit field
        /// designating all the supported manufacturers tables</param>
        /// <param name="bytNumPendingSupported">Number of pending tables supported</param>
        /// <returns>Length of table 04</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/06/06 AF  7.40.00 N/A    Created
        //
        static private uint GetTableLength(byte bytDimStdTblsUsed,
                                           byte bytDimMfgTblsUsed,
                                           byte bytNumPendingSupported)
        {
            uint uintTableLength = (uint)(bytDimStdTblsUsed + bytDimMfgTblsUsed +
                                   (SIZE_OF_ENTRY_ACTIVATION_RCD * bytNumPendingSupported) +
                                   SIZE_OF_STIME_DATE + SIZE_OF_NBR_PENDING_ACTIVATION);
            return uintTableLength;
        }

        #endregion

        #region Members

        /// <summary>
        /// Date and time of the last pending table activated
        /// </summary>
        private DateTime m_dtLastActivationDate;
        /// <summary>
        /// Number of activation events that have yet to be activated
        /// </summary>
        private byte m_bytNumberPendingActivation;
        /// <summary>
        /// List of pending tables and associated activation triggers
        /// </summary>
        private List<PendingEventActivationRecord> m_EntryActivationRcds;
        /// <summary>
        /// Set which indicates which of the standard tables are capable of being
        /// written with a pending status
        /// </summary>
        private byte[] m_abytSetStdPending;
        /// <summary>
        /// Set which indicates which of the manufacturers tables are capable of
        /// being written with a pending status
        /// </summary>
        private byte[] m_abytSetMfgPending;
        /// <summary>
        /// Number of pending status sets supported
        /// </summary>
        private byte m_bytNumPendingSupported;

        #endregion

    }

    /// <summary>
    /// The CTable5 class handles the reading of ANIS Table 05. The reading of
    /// this table in the meter will be implicit. (read-only)
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  07/13/05 mrj 7.13.00 N/A    Created
    //
    internal class CTable05 : AnsiTable
    {
        #region public methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CTable05 Table5 = new CTable05( PSEM ); 
        /// </code>
        /// </example>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 
        public CTable05(CPSEM psem)
            : base(psem, 5, TABLE_05_LENGTH)
        {
            m_strMeterID = "";
        }

        /// <summary>
        /// Reads table 05 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CTable05.Read");

            //Read the table			
            PSEMResponse Result = base.Read();


            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                //Populate the member variable that represent the table
                m_strMeterID = m_Reader.ReadString(20);
            }


            return Result;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the Meter ID of the meter
        /// </summary>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security("");
        /// CTable05 Table5 = new CTable05( PSEM );
        /// 
        /// string strMeterID = Table5.MeterID;
        /// </code>
        /// </example>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 		
        public string MeterID
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Meter ID"));
                    }
                }

                return m_strMeterID;
            }
        }

        #endregion

        #region public definitions

        #endregion public definitions

        #region private definitions

        private const int TABLE_05_LENGTH = 20;

        #endregion

        #region variable declarations

        //The table's member variable which represent the ANSI table 
        private string m_strMeterID;

        #endregion
    }

    /// <summary>
    /// The CTable06 class handles the reading of ANIS Table 06. The reading of
    /// this table in the meter will be implicit. (read-only)
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  10/09/06 RCG 7.40.00 N/A    Created
  
    internal class CTable06 : AnsiTable
    {
        #region Definitions
        private const uint STD_REV_1_TABLE_SIZE = 230;
        private const uint STD_REV_2_TABLE_SIZE = 358;
        private const uint MISC_ID_OFFSET = 200;
        private const uint MISC_ID_LENGTH = 30;

        #endregion

        #region Public Methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for this session.</param>
        /// <param name="revision">The revision number for the current device.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public CTable06(CPSEM psem, string revision)
            : base(psem, 6, CTable06.GetTableLength(revision))
        {
            m_strRevision = revision;
        }

        /// <summary>
        /// Reads table 00 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CTable06.Read");

            //Read the table			
            //PSEMResponse Result = base.Read();

            // For now we are going to have to do an offset read since
            // the EDL URI is not guaranteed to be in the meter in the 
            // version of firmware SCE has. Doing the full read could
            // prevent them from being able to log on in order to upgrade
            // to a version that does have the EDL URI.
            PSEMResponse Result = base.Read(0, (ushort)STD_REV_1_TABLE_SIZE);

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_strOwnerName = m_Reader.ReadString(20);
                m_strUtilityDiv = m_Reader.ReadString(20);
                m_strServicePointID = m_Reader.ReadString(20);
                m_strElectronicAddress = m_Reader.ReadString(20);
                m_strMeterID = m_Reader.ReadString(20);
                m_strUtilitySerialNumber = m_Reader.ReadString(20);
                m_strCustomerID = m_Reader.ReadString(20);
                m_abyCoordinate1 = m_Reader.ReadBytes(10);
                m_abyCoordinate2 = m_Reader.ReadBytes(10);
                m_abyCoordinate3 = m_Reader.ReadBytes(10);
                m_strTarriffID = m_Reader.ReadString(8);
                m_strSWVendor = m_Reader.ReadString(4);
                m_byEX1SWVersion = m_Reader.ReadByte();
                m_byEX1SWRevision = m_Reader.ReadByte();
                m_byEX2SWVendor = m_Reader.ReadString(4);
                m_byEX2SWVersion = m_Reader.ReadByte();
                m_byEX2SWRevision = m_Reader.ReadByte();
                m_strProgrammerName = m_Reader.ReadString(10);
                m_strMiscID = m_Reader.ReadString(30);

                // The EDL URI was not implemented in the AMI meter until
                // a late 80's build of firmware so we can not support this
                // until changes are made to the base class to allow handling
                // unexpected size tables

                //if (0 == String.Compare(m_strRevision, CTable00.STD_REV_2))
                //{
                //    m_strEDLURI = m_Reader.ReadString(128);
                //}

                m_TableState = TableState.Loaded;

            }

            return Result;
        }

        /// <summary>
        /// This method will reconfigure the misc ID to a new value. Original intention is to 
        /// update the value so CE will flag change and update the meter's configuration.
        /// </summary>
        /// <param name="miscID">the misc id to set</param>
        /// <returns></returns>
        public PSEMResponse ConfigureMiscID(string miscID)
        {
            PSEMResponse Result = PSEMResponse.Err;            

            m_DataStream.Position = MISC_ID_OFFSET;
            m_Writer.Write(miscID, (int)MISC_ID_LENGTH);

            Result = base.Write((ushort)MISC_ID_OFFSET, (ushort)MISC_ID_LENGTH);

            return Result;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the Owner Name from Table 06
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM error occurs during communications.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public string OwnerName
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Owner Name"));
                    }
                }

                return m_strOwnerName;
            }
        }

        /// <summary>
        /// Gets the Utility Division from Table 06
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM error occurs during communications.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public string UtilityDiv
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Utility Division"));
                    }
                }

                return m_strUtilityDiv;
            }
        }

        /// <summary>
        /// Gets the Service Point ID from Table 06
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM error occurs during communications.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public string ServicePointID
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Service Point ID"));
                    }
                }

                return m_strServicePointID;
            }
        }

        /// <summary>
        /// Gets the Electronic Address from Table 06
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM error occurs during communications.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public string ElectronicAddress
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Electronic Address"));
                    }
                }

                return m_strElectronicAddress;
            }
        }

        /// <summary>
        /// Gets the Meter ID from Table 06
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM error occurs during communications.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public string MeterID
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Meter ID"));
                    }
                }

                return m_strMeterID;
            }
        }

        /// <summary>
        /// Gets the Utility Serial Number from Table 06
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM error occurs during communications.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public string UtilitySerialNumber
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Utility Serial Number"));
                    }
                }

                return m_strUtilitySerialNumber;
            }
        }

        /// <summary>
        /// Gets the Customer ID from Table 06
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM error occurs during communications.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public string CustomerID
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Customer ID"));
                    }
                }

                return m_strCustomerID;
            }
        }

        /// <summary>
        /// Gets the Tarrif ID from Table 06
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM error occurs during communications.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public string TarriffID
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Tarriff ID"));
                    }
                }

                return m_strTarriffID;
            }
        }

        /// <summary>
        /// Gets the Software Vendor ID from Table 06
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM error occurs during communications.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public string SWVendor
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Software Vendor ID"));
                    }
                }

                return m_strSWVendor;
            }
        }

        /// <summary>
        /// Gets the Software Version number from Table 06
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM error occurs during communications.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public string SWVersion
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Software Version"));
                    }
                }

                return ((int)m_byEX1SWVersion).ToString(CultureInfo.InvariantCulture) + "." + ((int)m_byEX1SWRevision).ToString(CultureInfo.InvariantCulture);
            }
        }


        /// <summary>
        /// Gets the Programmer Name from Table 06
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM error occurs during communications.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public string ProgrammerName
        {
            get 
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Programmer Name"));
                    }
                }

                return m_strProgrammerName;
            }
        }

        /// <summary>
        /// Gets the Misc ID from Table 06
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM error occurs during communications.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public string MiscID
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                //Read Table
                Result = Read();
                if (PSEMResponse.Ok != Result)
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Software Misc ID"));
                }


                return m_strMiscID;
            }
        }

        /// <summary>
        /// Gets the EDL URI from Table 06
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Thrown if the meter does not support the EDL URI field.
        /// </exception>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM error occurs during communications.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public string EDLURI
        {
            get
            {
// This code has been temporarily removed since we can not guarantee that all AMI meters
// in use will have the EDL URI in table 6
#if false
                PSEMResponse Result = PSEMResponse.Ok;

                // Make sure that the device supports the EDL URI
                if (0 != String.Compare(m_strRevision, CTable00.STD_REV_2))
                {
                    throw new NotSupportedException("EDL URI is only supported in Standard Revision " + 
                        CTable00.STD_REV_2 + " devices.");
                }

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Software EDL URI"));
                    }
                }

                return m_strEDLURI;
#else
                throw new NotSupportedException("EDL URI is currently not supported");
#endif

            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Determines the length of Table 06 based on the revision number of the device
        /// </summary>
        /// <param name="revision">The revision number for the device.</param>
        /// <returns>The size of the table in number of bytes.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        private static uint GetTableLength(string revision)
        {
            uint uiTableLength = 0;

            if (0 == String.Compare(revision, CTable00.STD_REV_2, StringComparison.CurrentCulture))
            {
                uiTableLength = STD_REV_2_TABLE_SIZE;
            }
            else
            {
                uiTableLength = STD_REV_1_TABLE_SIZE;
            }

            return uiTableLength;
        }

        #endregion

        #region Member Variables
        private string m_strRevision;

        // The following members contain the data stored in the table
        private string m_strOwnerName;
        private string m_strUtilityDiv;
        private string m_strServicePointID;
        private string m_strElectronicAddress;
        private string m_strMeterID;
        private string m_strUtilitySerialNumber;
        private string m_strCustomerID;
        private byte[] m_abyCoordinate1;
        private byte[] m_abyCoordinate2;
        private byte[] m_abyCoordinate3;
        private string m_strTarriffID;
        private string m_strSWVendor;
        private byte m_byEX1SWVersion;
        private byte m_byEX1SWRevision;
        private string m_byEX2SWVendor;
        private byte m_byEX2SWVersion;
        private byte m_byEX2SWRevision;
        private string m_strProgrammerName;
        private string m_strMiscID;
        //private string m_strEDLURI;


        #endregion
    }

    /// <summary>
    /// The CTable07 class handles the writing of procedures to ANSI table 07.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  08/02/05 mrj 7.13.00 N/A    Created
    //  09/08/10 AF  2.43.06        Added a property for storing the procedure id
    //                              as a ushort for automated testing
    // 
    public class CTable07 : AnsiTable
    {
        #region public methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CTable07 Table7 = new CTable07( PSEM ); 
        /// </code>
        /// </example>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/02/05 mrj 7.13.00 N/A    Created
        ///
        public CTable07(CPSEM psem)
            : base(psem, 7, PROCEDURE_OVERHEAD)
        {
            m_bySequenceNum = 0;
        }

        /// <summary>
        /// Overrides the base class read method and throws an exception
        /// </summary>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mrj 7.30.00 N/A    Created
        ///
        public override PSEMResponse Read()
        {
            throw (new NotSupportedException("Read Not Supported"));
        }

        /// <summary>
        /// Overrides the base class read method and throws an exception
        /// </summary>
        /// <param name="Offset">byte offset to start reading from</param>
        /// <param name="Count">number bytes to read</param>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mrj 7.30.00 N/A    Created
        ///
        public override PSEMResponse Read(int Offset, ushort Count)
        {
            throw (new NotSupportedException("Read Not Supported"));
        }

        /// <summary>
        /// Overrides the base class write method to handle writing a procedure
        /// to table 07.
        /// </summary>		
        /// <returns>protocol response</returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mrj 7.30.00 N/A    Created
        /// 
        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CTable07.Write");


            //Resynch our members to the base's data array
            m_DataStream.Position = 0;
            m_Writer.Write((ushort)Procedure);
            m_Writer.Write(SequenceNumber);
            m_Writer.Write(ParameterData);

            return base.Write();
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Table 7 does not
        /// allow offset writes.
        /// </summary>		
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mrj 7.30.00 N/A    Created
        /// 
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported"));
        }

        #endregion

        #region properties

        /// <summary>
        /// Property for the sequence number
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mrj 7.30.00		Created
        ///
        public byte SequenceNumber
        {
            get
            {
                return m_bySequenceNum;
            }
            set
            {
                m_bySequenceNum = value;
            }
        }

        /// <summary>
        /// Property for the procedure
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mrj 7.30.00		Created
        ///
        public ushort Procedure
        {
            get
            {
                return m_Procedure;
            }
            set
            {
                m_Procedure = value;
            }
        }

        /// <summary>
        /// Property for the procedure
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mrj 7.30.00		Created
        ///
        public byte[] ParameterData
        {
            get
            {
                return m_byParameterData;
            }
            set
            {
                //Re-size the table
                m_Size = (uint)(value.Length + PROCEDURE_OVERHEAD);
                m_Data = new byte[m_Size];
                m_Data.Initialize();
                m_DataStream = new MemoryStream(m_Data);
                m_Writer = new PSEMBinaryWriter(m_DataStream);

                m_byParameterData = null;
                m_byParameterData = new byte[value.Length];
                Array.Copy(value, 0, m_byParameterData, 0, value.Length);
            }
        }

        #endregion

        #region public definitions

        #endregion public definitions

        #region private definitions

        private const int PROCEDURE_OVERHEAD = 3;

        #endregion

        #region variable declarations

        //The table fields
        private byte m_bySequenceNum;
        private ushort m_Procedure;
        private byte[] m_byParameterData;

        #endregion
    }

    /// <summary>
    /// The CTable08 class handles the reading of procedure responses from ANSI 
    /// table 08.  This table should be created once for every procedure that is
    /// to be read.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------------
    //  08/02/05 mrj 7.13.00 N/A    Created
    //  09/10/13 jrf 2.85.40 WR422369 Class made public so ATIApp project would compile.
    // 
    public class CTable08 : AnsiTable
    {
        #region public methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CTable07 Table7 = new CTable07( PSEM ); 
        /// CTable08 Table8 = new CTable08( PSEM ); 
        /// </code>
        /// </example>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/02/05 mrj 7.13.00 N/A    Created
        /// 10/07/05 mrj 7.20.20 1883	Added support for the result data
        ///
        public CTable08(CPSEM psem)
            : base(psem, 8, RESULT_OVERHEAD)
        {
            m_bySequenceNum = 0;
            m_byResultCode = (byte)ProcedureResultCodes.NOT_FULLY_COMPLETED;
        }

        /// <summary>
        /// This method reads the response in table 08 for a procedure
        /// that was executed by writing to the table 07.  It overrides the
        /// base class read method.
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).  It will return OK if the read is a
        /// success and we get a response.  The client will need to interpret
        /// the response code and response data.
        /// </returns>	
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mrj 7.30.00 N/A    Created
        /// 05/09/11 jrf 2.50.43        Adding debug info when table 8 read response does not return
        ///                             enough table data.
        ///
        public override PSEMResponse Read()
        {
            PSEMResponse Result;
            m_Data = null;
            m_byResultData = null;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CTable08.Read");


            //Read the table
            Result = m_PSEM.FullRead(m_TableID, out m_Data);

            if (PSEMResponse.Ok == Result)
            {
                //Re-sync the members since this tables size is dynamic
                m_Size = (uint)m_Data.Length;

                if (RESULT_OVERHEAD <= m_Data.Length)
                {
                    m_DataStream = new MemoryStream(m_Data);
                    m_Reader = new PSEMBinaryReader(m_DataStream);

                    //Get the table fields
                    m_Procedure = (Procedures)m_Reader.ReadUInt16();
                    m_bySequenceNum = m_Reader.ReadByte();
                    m_byResultCode = m_Reader.ReadByte();

                    //Get the result data (whatever is left in the data buffer)
                    m_byResultData = new byte[m_Data.Length - RESULT_OVERHEAD];
                    for (int iIndex = 0; iIndex < m_byResultData.Length; iIndex++)
                    {
                        m_byResultData[iIndex] = m_Reader.ReadByte();
                    }

                    m_TableState = TableState.Loaded;
                }
                else
                {
                    //Table 8 response did not return enough data.
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CTable08.Read Response missing data");

                    if (0 == m_Data.Length)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "   -missing procedure number");
                        m_Procedure = Procedures.UNKNOWN;
                    }
                    else if (1 >= m_Data.Length)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "   -missing 1 out of 2 bytes of procedure number");
                        m_Procedure = Procedures.UNKNOWN;
                    }

                    if (2 >= m_Data.Length)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "   -missing sequence number");
                        m_bySequenceNum = 0xFF;
                    }

                    if (3 >= m_Data.Length)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "   -missing procedure result code");
                        m_byResultCode = 0xFF;
                    }

                    m_byResultData = new byte[0];
                }
            }


            return Result;
        }

        /// <summary>
        /// Overrides the base class read method and throws an exception.
        /// </summary>
        /// <param name="Offset">byte offset to start reading from</param>
        /// <param name="Count">number bytes to read</param>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mrj 7.30.00 N/A    Created
        ///
        public override PSEMResponse Read(int Offset, ushort Count)
        {
            throw (new NotSupportedException("The table does not support offset reads."));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Table 8 does not
        /// allow writes.
        /// </summary>		
        /// <returns>protocol response</returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mrj 7.30.00 N/A    Created
        /// 
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("The table does not support writing. It is a read-only table."));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Table 8 does not
        /// allow writes.
        /// </summary>		
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mrj 7.30.00 N/A    Created
        /// 
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("The table does not support writing. It is a read-only table."));
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the sequence number
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 08/02/05 mrj 7.13.00 N/A    Created
        ///
        public byte Sequencenumber
        {
            get
            {
                return m_bySequenceNum;
            }
        }

        /// <summary>
        /// Gets the result code from table 08 read.  This is only valid after a write
        /// to table 07 and a read from table 08 has been completed.
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 08/02/05 mrj 7.13.00 N/A    Created
        /// 10/07/05 mrj 7.20.20 1883   The result code was not being returned
        ///
        public ProcedureResultCodes ResultCode
        {
            get
            {
                return (ProcedureResultCodes)m_byResultCode;
            }
        }

        /// <summary>
        /// Property for the procedure
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mrj 7.30.00		Created
        ///
        public Procedures Procedure
        {
            get
            {
                return m_Procedure;
            }
        }

        /// <summary>
        /// Property for the result data
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mrj 7.30.00		Created
        ///
        public byte[] ResultData
        {
            get
            {
                return m_byResultData;
            }
        }

        #endregion

        #region public definitions

        #endregion public definitions

        #region private definitions

        private const int RESULT_OVERHEAD = 4;

        #endregion

        #region variable declarations

        //Table fields
        private byte m_bySequenceNum;
        private byte m_byResultCode;
        private Procedures m_Procedure;
        private byte[] m_byResultData;

        #endregion
    }

    /// <summary>
    /// A class to represent the pending table information for std table 04
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------
    //  09/25/06 AF  7.40.00 N/A    Created
    //  06/07/07 RCG 8.10.06        Reorganized to use the PendingEventRecord object
    //
    public class PendingEventActivationRecord
    {

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/25/06 AF  7.40.00 N/A    Created
        //
        public PendingEventActivationRecord()
        {
            m_sTableID = 0;
            m_blnPending = false;
            Event = new PendingEventRecord();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Table bit field - contains the table id
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/25/06 AF  7.40.00 N/A    Created
        //
        public UInt16 TableID
        {
            get
            {
                return m_sTableID;
            }
            set
            {
                m_sTableID = value;
            }
        }

        /// <summary>
        /// Portion of table bit field that tells whether or not the table is still pending
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/05/06 AF  7.40.00 N/A    Created
        //
        public bool StillPending
        {
            get
            {
                return m_blnPending;
            }
            set
            {
                m_blnPending = value;
            }
        }

        /// <summary>
        /// Gets or sets the Event record for the pending event.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/07 RCG 8.10.07        Created

        public PendingEventRecord Event
        {
            get
            {
                return m_EventRecord;
            }
            set
            {
                m_EventRecord = value;
            }
        }

        /// <summary>
        /// Gets or sets the status of the Pending Table.
        /// This should be set by the device.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/07 RCG 8.10.07        Created

        public string Status
        {
            get
            {
                return m_strStatus;
            }
            set
            {
                m_strStatus = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the Pending Table.
        /// This should be set by the device.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/07 RCG 8.10.07        Created

        public string TableName
        {
            get
            {
                return m_strTableName;
            }
            set
            {
                m_strTableName = value;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// Table ID of the pending table
        /// </summary>
        private UInt16 m_sTableID;
        /// <summary>
        /// Taken from the table ida bitfield, it is a flag telling whether or not
        /// the table is still pending
        /// </summary>
        private bool m_blnPending;
        /// <summary>
        /// Event Record object that contains the event selector and event storage
        /// </summary>
        private PendingEventRecord m_EventRecord;
        /// <summary>
        /// The current status of the pending table
        /// </summary>
        private string m_strStatus;
        /// <summary>
        /// The name of the pending table.
        /// </summary>
        private string m_strTableName;

        #endregion

    }

    /// <summary>
    /// Class that represents the Event Record for a Pending Table event.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  05/07/07 RCG 8.10.07        Created

    public class PendingEventRecord
    {
        #region Constants

        private const int EVENT_STORAGE_LENGTH = 5;

        // Event Selector masks
        private const byte EVENT_CODE_MASK = 0x07;
        private const byte SELF_READ_MASK = 0x08;
        private const byte DEMAND_RESET_MASK = 0x10;

        #endregion

        #region Definitions

        /// <summary>
        /// Event code enumeration used for determining how the event storage
        /// data should be interpreted.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/07 RCG 8.10.07        Created

        public enum PendingEventCode : byte
        {
            /// <summary>
            /// Pending event is activated by an absolute time. The event storage
            /// data is stored as an STIME_DATE.
            /// </summary>
            AbsoluteTimeTrigger = 0,
            /// <summary>
            /// Pending event is activated by a relative time. The event storage
            /// data is stored as an STIME_DATE
            /// </summary>
            RelativeTimeTrigger = 1,
            /// <summary>
            /// Pending event is activated by a non time related trigger. The event
            /// storage data is stored as a 5 character string
            /// </summary>
            NonTimeTrigger = 2,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/07 RCG 8.10.07        Created

        public PendingEventRecord()
        {
            m_TimeFormat = PSEMBinaryReader.TM_FORMAT.UINT32_TIME;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="byEventCode">The event code bitfield for the event.</param>
        /// <param name="byEventStorage">The event storage data for the event.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/07 RCG 8.10.07        Created

        public PendingEventRecord(byte byEventCode, byte[] byEventStorage)
        {
            EventSelector = byEventCode;
            EventStorage = byEventStorage;
        }

        /// <summary>
        /// Constructor used for creating a pending event record using an Absolute Time Trigger
        /// </summary>
        /// <param name="activationDate">The date and time that the event should activate.</param>
        /// <param name="timeFormat">Time Format used by the meter</param>
        /// <param name="performSelfRead">Whether or not a Self Read should be performed</param>
        /// <param name="performDemandReset">Whether or not a Demand Reset should be performed</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 RCG 2.53.12        Created
        
        public PendingEventRecord(DateTime activationDate, PSEMBinaryReader.TM_FORMAT timeFormat, bool performSelfRead, bool performDemandReset)
        {
            EventCode = PendingEventCode.AbsoluteTimeTrigger;
            m_TimeFormat = timeFormat;            
            PerformSelfRead = performSelfRead;
            PerformDemandReset = performDemandReset;
            ActivationDate = activationDate;
        }

        /// <summary>
        /// Constructor used for creating a pending event record using a Relative Time Trigger
        /// </summary>
        /// <param name="activationTime">The amount of time to wait before activating the pending table</param>
        /// <param name="performSelfRead">Whether or not a Self Read should be performed</param>
        /// <param name="performDemandReset">Whether or not a Demand Reset should be performed</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 RCG 2.53.12        Created
        
        public PendingEventRecord(TimeSpan activationTime, bool performSelfRead, bool performDemandReset)
        {
            EventCode = PendingEventCode.RelativeTimeTrigger;
            PerformSelfRead = performSelfRead;
            PerformDemandReset = performDemandReset;
            ActivationTime = activationTime;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the event selector bitfield
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/07 RCG 8.10.07        Created

        public byte EventSelector
        {
            get
            {
                return m_byEventSelector;
            }
            set
            {
                m_byEventSelector = value;
            }
        }

        /// <summary>
        /// Gets or sets the Event Storage data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/07 RCG 8.10.07        Created

        public byte[] EventStorage
        {
            get
            {
                return m_byEventStorage;
            }
            set
            {
                if (value != null && value.Length != EVENT_STORAGE_LENGTH)
                {
                    throw new ArgumentException("Event Storage must be " + EVENT_STORAGE_LENGTH + " bytes");
                }
                else
                {
                    m_byEventStorage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the event code from the event selector bitfield
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/07 RCG 8.10.07        Created

        public PendingEventCode EventCode
        {
            get
            {
                return (PendingEventCode)(m_byEventSelector & EVENT_CODE_MASK);
            }
            set
            {
                // To set these bits we need to clear the current event code and then add
                // the new event code back all using bitwise operators
                m_byEventSelector = (byte)((m_byEventSelector & (~EVENT_CODE_MASK)) | ((byte)value & EVENT_CODE_MASK));
            }
        }

        /// <summary>
        /// Gets or sets whether or not a self read will be performed upon activation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/07 RCG 8.10.07        Created

        public bool PerformSelfRead
        {
            get
            {
                return (m_byEventSelector & SELF_READ_MASK) == SELF_READ_MASK;
            }
            set
            {
                if (value == true)
                {
                    // Set the Self Read bit
                    m_byEventSelector = (byte)(m_byEventSelector | SELF_READ_MASK);
                }
                else
                {
                    // Clear the Self Read bit
                    m_byEventSelector = (byte)(m_byEventSelector & ~SELF_READ_MASK);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether or not a demand reset will be performed upon activation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/07 RCG 8.10.07        Created

        public bool PerformDemandReset
        {
            get
            {
                return (m_byEventSelector & DEMAND_RESET_MASK) == DEMAND_RESET_MASK;
            }
            set
            {
                if (value == true)
                {
                    // Set the Demand Reset bit
                    m_byEventSelector = (byte)(m_byEventSelector | DEMAND_RESET_MASK);
                }
                else
                {
                    // Clear the Demand Reset bit
                    m_byEventSelector = (byte)(m_byEventSelector & ~DEMAND_RESET_MASK);
                }
            }
        }

        /// <summary>
        /// Gets a byte array of the entire record
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/07 RCG 8.10.07        Created

        public byte[] EntireRecord
        {
            get
            {
                byte[] byRecord = null;

                if (EventStorage != null)
                {
                    byRecord = new byte[EventStorage.Length + 1];

                    // Copy the data to the 
                    byRecord[0] = EventSelector;
                    EventStorage.CopyTo(byRecord, 1);
                }

                return byRecord;
            }
        }

        /// <summary>
        /// Gets or set the Activate Date used for Absolute Time Trigger Pending Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 RCG 2.53.12        Created

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "EventCode")]
        public DateTime? ActivationDate
        {
            get
            {
                DateTime? Date = null;

                if (EventCode == PendingEventCode.AbsoluteTimeTrigger)
                {
                    MemoryStream DataStream = new MemoryStream(EventStorage);
                    PSEMBinaryReader DataReader = new PSEMBinaryReader(DataStream);

                    Date = DataReader.ReadSTIME(m_TimeFormat);
                }

                return Date;
            }
            set
            {
                if (EventCode == PendingEventCode.AbsoluteTimeTrigger)
                {
                    EventStorage = new byte[EVENT_STORAGE_LENGTH];

                    if (value != null)
                    {
                        MemoryStream DataStream = new MemoryStream(EventStorage);
                        PSEMBinaryWriter DataWriter = new PSEMBinaryWriter(DataStream);

                        DataWriter.WriteSTIME(value.Value, m_TimeFormat);
                    }
                }
                else
                {
                    throw new InvalidOperationException("The Activation Date may only be set when the EventCode is set to Absolute Time Trigger");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Activation Time used for Relative Time Trigger Pending Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 RCG 2.53.12        Created

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "EventCode")]
        public TimeSpan? ActivationTime
        {
            get
            {
                TimeSpan? Time = null;

                if (EventCode == PendingEventCode.RelativeTimeTrigger)
                {
                    // The Event Storage for this breaks down to Index 0: Weeks, 1: Days, 2: Hours, 3: Minutes, 4: Seconds
                    Time = new TimeSpan(EventStorage[0] * 7 + EventStorage[1], EventStorage[2], EventStorage[3], EventStorage[4]);
                }

                return Time;
            }
            set
            {
                if (EventCode == PendingEventCode.RelativeTimeTrigger)
                {
                    EventStorage = new byte[EVENT_STORAGE_LENGTH];

                    if (value != null)
                    {
                        TimeSpan Time = value.Value;

                        if (Time.Days / 7 <= 255)
                        {
                            EventStorage[0] = (byte)(Time.Days / 7); // Weeks
                            EventStorage[1] = (byte)(Time.Days % 7); // Days
                            EventStorage[2] = (byte)Time.Hours; // Hours;
                            EventStorage[3] = (byte)Time.Minutes; // Minutes
                            EventStorage[4] = (byte)Time.Seconds; // Seconds
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("The Relative Time Trigger may not be more than 255 weeks");
                        }

                    }
                }
                else
                {
                    throw new InvalidOperationException("The Activation Time may only be set when the EventCode is set to Relative Time Trigger");
                }
            }
        }

        #endregion

        #region Member Variables

        private byte m_byEventSelector;
        private byte[] m_byEventStorage;
        private PSEMBinaryReader.TM_FORMAT m_TimeFormat;

        #endregion
    }
}
