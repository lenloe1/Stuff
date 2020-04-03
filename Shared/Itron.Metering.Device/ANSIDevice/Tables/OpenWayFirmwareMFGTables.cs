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
//                              Copyright © 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using System.Globalization;


namespace Itron.Metering.Device
{
    /// <summary>
    /// The Fatal Error Recovery Status flags
    /// </summary>
    public enum StatusFormat : byte
    {
        /// <summary>
        /// Blocks are missing.
        /// </summary>
        MissingBlocks = 0,
        /// <summary>
        /// Bitfield.
        /// </summary>
        Bitfield = 1,
        /// <summary>
        /// Not in firmware download mode.
        /// </summary>
        NotInDownloadMode = 255,
    }

    /// <summary>
    /// Current CRC State
    /// </summary>
    public enum CRC_STATE : byte
    {
        /// <summary>
        /// Invalid
        /// </summary>
        [EnumDescription("Invalid")]
        Invalid = 0,
        /// <summary>
        /// Waiting
        /// </summary>
        [EnumDescription("Waiting")]
        Waiting = 1,
        /// <summary>
        /// Downloading
        /// </summary>
        [EnumDescription("Downloading")]
        Downloading = 2,
        /// <summary>
        /// Calculating
        /// </summary>
        [EnumDescription("Calculating")]
        Calculating = 3,
        /// <summary>
        /// CRC Pass
        /// </summary>
        [EnumDescription("CRC Pass")]
        CRCPass = 4,
        /// <summary>
        /// CRC Fail
        /// </summary>
        [EnumDescription("CRC Fail")]
        CRCFail = 5,
        /// <summary>
        /// All Blocks Received
        /// </summary>
        [EnumDescription("All Blocks Received")]
        AllBlocksReceived = 6,
        /// <summary>
        /// Calculating Hash
        /// </summary>
        [EnumDescription("Calculating Hash")]
        CalculatingHash = 7,
    }

    /// <summary>
    /// Activation State
    /// </summary>
    public enum ACTIVATION_STATE : byte
    {
        /// <summary>
        /// Invalid
        /// </summary>
        [EnumDescription("Invalid")]
        Invalid = 0,
        /// <summary>
        /// Waiting
        /// </summary>
        [EnumDescription("Waiting")]
        Waiting = 1,
        /// <summary>
        /// Scheduled
        /// </summary>
        [EnumDescription("Scheduled")]
        Scheduled = 2,
        /// <summary>
        /// Canceled
        /// </summary>
        [EnumDescription("Canceled")]
        Canceled = 3,
    }

    /// <summary>
    /// Actual Firmware Download Status Table.  Mfg. Table 131.
    /// </summary>
    internal class OpenWayMFGTable2179 : AnsiTable
    {
        #region Constants

        private const ushort TABLE_LENGTH = 2;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12         Created
        //
        public OpenWayMFGTable2179(CPSEM psem)
            : base(psem, 2179, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads some fwdl info.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12         Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_bytStatusFormat = m_Reader.ReadByte();
                m_bytSize = m_Reader.ReadByte();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12         Created
        //
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12         Created
        //
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12         Created
        //
        public StatusFormat FormatOfStatus
        {
            get
            {
                StatusFormat enumStatusFormat = StatusFormat.NotInDownloadMode;

                ReadUnloadedTable();

                if (0 == m_bytStatusFormat)
                {
                    enumStatusFormat = StatusFormat.MissingBlocks;
                }
                else if (1 == m_bytStatusFormat)
                {
                    enumStatusFormat = StatusFormat.Bitfield;
                }
                else
                {
                    enumStatusFormat = StatusFormat.NotInDownloadMode;
                }

                return enumStatusFormat;
            }
        }

        /// <summary>
        /// This property gets 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12         Created
        //
        public byte Size
        {
            get
            {
                ReadUnloadedTable();

                return m_bytSize;
            }
        }

        #endregion

        #region Member Variables

        private byte m_bytStatusFormat;
        private byte m_bytSize;

        #endregion
    }

    /// <summary>
    /// Firmware Status Table.  Mfg. Table 132.
    /// </summary>
    internal class OpenWayMFGTable2180 : AnsiTable
    {
        #region Constants

        private const ushort TABLE_LENGTH = 2;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        /// <param name="bytSize"></param>
        /// <param name="enumStatusFormat"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12         Created
        //
        public OpenWayMFGTable2180(CPSEM psem, StatusFormat enumStatusFormat, byte bytSize)
            : base(psem, 2180, GetTableSize(enumStatusFormat, bytSize))
        {
            //MissingBlocks
            if (StatusFormat.MissingBlocks == enumStatusFormat)
            {
                m_ausBlocksMissing = new ushort[bytSize];
            }
            //Bitfield
            else if (StatusFormat.MissingBlocks == enumStatusFormat)
            {
                m_abytBlocksMissing = new byte[bytSize];
            }
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12         Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_usCRC = m_Reader.ReadUInt16();
                m_usBlocksReceived = m_Reader.ReadUInt16();
                m_bytStatusFormat = m_Reader.ReadByte();

                //MissingBlocks
                if ((byte)StatusFormat.MissingBlocks == m_bytStatusFormat)
                {
                    m_bytNumberBlocksReport = m_Reader.ReadByte();

                    for (int iIndex = 0; iIndex < m_ausBlocksMissing.Length; iIndex++)
                    {
                        m_ausBlocksMissing[iIndex] = m_Reader.ReadUInt16();
                    }
                }
                //Bitfield
                else if ((byte)StatusFormat.Bitfield == m_bytStatusFormat)
                {
                    m_usBitfieldOffset = m_Reader.ReadUInt16();
                    m_bytBitBlockRatio = m_Reader.ReadByte();
                    m_bytNumberOctetsReport = m_Reader.ReadByte();
                    
                    for (int iIndex = 0; iIndex < m_abytBlocksMissing.Length; iIndex++)
                    {
                        m_abytBlocksMissing[iIndex] = m_Reader.ReadByte();
                    }
                }
                
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12         Created
        //
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12         Created
        //
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets the format of the status information.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12         Created
        //
        public StatusFormat FormatOfStatus
        {
            get
            {
                StatusFormat enumStatusFormat = StatusFormat.NotInDownloadMode;

                ReadUnloadedTable();

                if (0 == m_bytStatusFormat)
                {
                    enumStatusFormat = StatusFormat.MissingBlocks;
                }
                else if (1 == m_bytStatusFormat)
                {
                    enumStatusFormat = StatusFormat.Bitfield;
                }
                else
                {
                    enumStatusFormat = StatusFormat.NotInDownloadMode;
                }

                return enumStatusFormat;
            }
        }

        /// <summary>
        /// This property gets the number of firmware download blocks received.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12         Created
        //
        public ushort BlocksReceived
        {
            get
            {
                ReadUnloadedTable();

                return m_usBlocksReceived;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of the table
        /// </summary>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/05/10 jrf 2.40.12         Created
        //
        private static ushort GetTableSize(StatusFormat enumStatusFormat, byte bytSize)
        {
            ushort usTableSize = 5;

            if (StatusFormat.MissingBlocks == enumStatusFormat)
            {
                usTableSize += (ushort)(1 + 2 * bytSize);
            }
            else if (StatusFormat.Bitfield == enumStatusFormat)
            {
                usTableSize += (ushort)(4 + bytSize);
            }

            return usTableSize;
        }

        #endregion

        #region Member Variables

        private byte m_bytStatusFormat;
        private ushort m_usCRC;
        private ushort m_usBlocksReceived;
        //MissingBlocks
        private byte m_bytNumberBlocksReport;
        private ushort[] m_ausBlocksMissing;
        //Bitfield
        private ushort m_usBitfieldOffset;
        private byte m_bytBitBlockRatio;
        private byte m_bytNumberOctetsReport;
        private byte[] m_abytBlocksMissing;

        #endregion
    }

    /// <summary>
    ///Table 2182: Extended FW DL Status Table
    ///
    ///A read-only MFG table for storing the CRC and activation status of the
    ///downloaded FW after running the table 7 procedures 176 (Cancel Scheduled
    ///Firmware Activation).
    /// </summary>
    internal class OpenWayMFGTable2182 : AnsiTable
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
        //  09/27/12 JKW 2.70.xx N/A    Created

        public OpenWayMFGTable2182(CPSEM psem, CTable00 Table0)
            : base(psem, 2182, GetTableSize(Table0), 300)
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
        //  09/27/12 JKW 2.70.xx N/A    Created

        public static uint GetTableSize(CTable00 Table0)
        {
            //TYPE EXTENDED_FW_DL_STATUS_RCD = PACKED RECORD
            //  CRC_Status                : TEXT;
            //    Current_CRC_State       : UINT8 INDENT(4), ENUM(CURRENT_CRC_STATE_ENUM);
            //    Supplied_CRC_Value      : UINT16 INDENT(4);
            //    Calculated_CRC_Value    : UINT16 INDENT(4);

            //  Activation_Status         : TEXT NL_BEFORE;
            //    Activation_State        : UINT8 INDENT(4), ENUM(ACTIVATION_STATE_ENUM);    
            //    Activation_Time         : STIME_DATE INDENT(4);

            //  Hash_Data                 : TEXT NL_BEFORE;
            //    Calculated_Hash_Code    : ARRAY[32] OF BYTE INDENT(4);

            //  For_future_needs          : TEXT NL_BEFORE;
            //    Unused                  : ARRAY[6] OF UINT8 INDENT(4);
            //END;

            uint uiTableSize = 6;

            uiTableSize += Table0.STIMESize;

            uiTableSize += 32;

            uiTableSize += 6;

            return uiTableSize;
        }

        /// <summary>
        /// Reads the table from the meter
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/27/12 JKW 2.70.xx N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2182.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_CurrentCRCState = (CRC_STATE)m_Reader.ReadByte();
                m_usSuppliedCRCValue = m_Reader.ReadUInt16();
                m_usCalculatedCRCValue = m_Reader.ReadUInt16();
                m_ActivationState = (ACTIVATION_STATE)m_Reader.ReadByte();
                m_dtActivationTime = m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                m_CalculatedHashCode = m_Reader.ReadBytes(32);
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/27/12 JKW 2.70.xx N/A    Created
        //
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/27/12 JKW 2.70.xx N/A    Created
        //
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the current CRC state
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/27/12 JKW 2.70.xx N/A    Created

        public CRC_STATE CurrentCRCState
        {
            get
            {
                ReadUnloadedTable();

                return m_CurrentCRCState;
            }
        }

        /// <summary>
        /// Gets the supplied CRC value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/27/12 JKW 2.70.xx N/A    Created

        public ushort SuppliedCRCValue
        {
            get
            {
                ReadUnloadedTable();

                return m_usSuppliedCRCValue;
            }
        }

        /// <summary>
        /// Gets the calculated CRC value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/27/12 JKW 2.70.xx N/A    Created

        public ushort CalculatedCRCValue
        {
            get
            {
                ReadUnloadedTable();

                return m_usCalculatedCRCValue;
            }
        }

        /// <summary>
        /// Gets the pending table activation state
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/27/12 JKW 2.70.xx N/A    Created

        public ACTIVATION_STATE ActivationState
        {
            get
            {
                ReadUnloadedTable();

                return m_ActivationState;
            }
        }

        /// <summary>
        /// Gets the pending table activation time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/27/12 JKW 2.70.xx N/A    Created

        public DateTime ActivationTime
        {
            get
            {
                ReadUnloadedTable();

                return m_dtActivationTime;
            }
        }

        /// <summary>
        /// Gets the calculated hash code
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/12 JKW 2.70.xx N/A    Created

        public byte[] CalculatedHashCode
        {
            get
            {
                ReadUnloadedTable();

                return m_CalculatedHashCode;
            }
        }

        #endregion

        #region Member Variables

        private CRC_STATE m_CurrentCRCState;
        private ushort m_usSuppliedCRCValue;
        private ushort m_usCalculatedCRCValue;
        private ACTIVATION_STATE m_ActivationState;
        private DateTime m_dtActivationTime;
        private byte[] m_CalculatedHashCode;

        #endregion
    }
}