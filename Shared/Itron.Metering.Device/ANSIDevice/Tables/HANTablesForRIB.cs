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
//                           Copyright © 2012 - 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;



// Reference "OpenWay Tables and Procedures.docx" 
//

namespace Itron.Metering.Device
{
    /// <summary>
    /// HAN2LimitRcd - for Table 2439 and 2440.
    /// Supports HAN RIB (Residential Inclining Block) pricing. 
    /// </summary>

    public class HAN2LimitRcd
    {
        #region Constants
        internal const uint SCHEDULE_ID_LEN = 30;
        internal const uint RATE_LABEL_LEN = 12;

        internal const uint RESERVED_READ_ONLY_BYTES = 128;
        internal const uint SIZEOF = (3 + RESERVED_READ_ONLY_BYTES + 3);
        internal const uint OFFSETOF_NEXT_NBR_BILLING_RECORDS = 3 + RESERVED_READ_ONLY_BYTES;

        #endregion

        #region Public Methods
        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/?? ??  ?.??.?? N/A    Created
        //        
        public HAN2LimitRcd()
        {
            m_ActiveNbrBillingPeriods = 0;
            m_ActiveNbrBlockPeriods = 0;
            m_ActiveNbrBlocks = 0;
            m_Reserved = new Byte[RESERVED_READ_ONLY_BYTES]; // fix magic number
            m_NextNbrBillingPeriods = 0;
            m_NextNbrBlockPeriods = 0;
            m_NextNbrBlocks = 0;
        }

        /// <summary>
        /// Parses the data from the specified reader.
        /// </summary>
        /// <param name="reader">The reader used to parse the data</param>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/01/12 RCG 2.60.18 N/A    Created
        
        public void ParseData(PSEMBinaryReader reader)
        {
            if (reader != null)
            {
                m_ActiveNbrBillingPeriods = reader.ReadByte();
                m_ActiveNbrBlockPeriods = reader.ReadByte();
                m_ActiveNbrBlocks = reader.ReadByte();
                m_Reserved = reader.ReadBytes((int)(RESERVED_READ_ONLY_BYTES));
                m_NextNbrBillingPeriods = reader.ReadByte();
                m_NextNbrBlockPeriods = reader.ReadByte();
                m_NextNbrBlocks = reader.ReadByte();
            }
        }

        /// <summary>
        /// Writes the writable data in this record to the specified writer.
        /// </summary>
        /// <param name="Writer">The PSEMBinaryWriter used to write the data.</param>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/12 jrf 2.60.23 TREQ5995 Created
        //
        public void WriteData(PSEMBinaryWriter Writer)
        {
            if (Writer != null)
            {
                Writer.Write(m_NextNbrBillingPeriods);
                Writer.Write(m_NextNbrBlockPeriods);
                Writer.Write(m_NextNbrBlocks);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Number of Billing Periods in the active schedule
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/01/12 RCG 2.60.18 N/A    Created
        
        public byte ActiveNumberOfBillingPeriods
        {
            get
            {
                return m_ActiveNbrBillingPeriods;
            }
        }

        /// <summary>
        /// Gets the number of block periods in the active schedule
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/01/12 RCG 2.60.18 N/A    Created
        
        public byte ActiveNumberOfBlockPeriods
        {
            get
            {
                return m_ActiveNbrBlockPeriods;
            }
        }

        /// <summary>
        /// Gets the number of blocks in the active schedule
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/01/12 RCG 2.60.18 N/A    Created
        
        public byte ActiveNumberOfBlocks
        {
            get
            {
                return m_ActiveNbrBlocks;
            }
        }

        /// <summary>
        /// Gets or sets the number of billing periods in the next schedule
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/01/12 RCG 2.60.18 N/A    Created
        
        public byte NextNumberOfBillingPeriods
        {
            get
            {
                return m_NextNbrBillingPeriods;
            }
            set
            {
                m_NextNbrBillingPeriods = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of block periods in the next schedule
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/01/12 RCG 2.60.18 N/A    Created
        
        public byte NextNumberOfBlockPeriods
        {
            get
            {
                return m_NextNbrBlockPeriods;
            }
            set
            {
                m_NextNbrBlockPeriods = value;
            }
        }

        /// <summary>
        /// Gets the number of blocks in the next schedule
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/01/12 RCG 2.60.18 N/A    Created
        
        public byte NextNumberOfBlocks
        {
            get
            {
                return m_NextNbrBlocks;
            }
            set
            {
                m_NextNbrBlocks = value;
            }
        }

        #endregion

        #region Members

        // Meter read-only
        private byte m_ActiveNbrBillingPeriods;
        private byte m_ActiveNbrBlockPeriods;
        private byte m_ActiveNbrBlocks;
        private byte[] m_Reserved;

        // Meter read/write
        private byte m_NextNbrBillingPeriods;
        private byte m_NextNbrBlockPeriods;
        private byte m_NextNbrBlocks;

        #endregion
    }

    /// <summary>
    /// Table 2438 (ITR1 390)  - Dimension HAN2 Limiting Table.
    /// Supports HAN RIB (Residential Inclining Block) pricing. 
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   This table is used by CHANMfgTable2440and CHANMfgTable2441
    ///   This read-only table is supported only by OpenWay meters.     
    ///   </para>
    /// </remarks>

    public class CHANMfgTable2438 : AnsiTable
    {
        #region Constants
        private const int TABLE_TIMEOUT = 5000;
        private const uint SIZEOF = sizeof(UInt16) + HAN2LimitRcd.SIZEOF;
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        //
        public CHANMfgTable2438(CPSEM psem)
            : base(psem, 2438, SIZEOF, TABLE_TIMEOUT)
        {
            m_Version = 0;
            m_HAN2Limits = new HAN2LimitRcd();
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/12 jrf 2.60.20 TREQ5995 Created
        //
        public CHANMfgTable2438(PSEMBinaryReader reader)
            : base(2438, SIZEOF)
        {
            m_Reader = reader;
            m_Version = 0;
            m_HAN2Limits = new HAN2LimitRcd();
            m_Reader = reader;

            ParseData();
            
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Full read of table 2438
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/23/12 DC  ?.??.?? N/A    Created
        //
        public override PSEMResponse Read()
        {
            if (null != m_Logger)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2438.Read");
            }

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result && null != m_DataStream)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data out of the reader and into the member variables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        //
        private void ParseData()
        {
            m_Version = m_Reader.ReadUInt16();

            if (null != m_HAN2Limits)
            {
                m_HAN2Limits.ParseData(m_Reader);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Get Version.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public UInt16 Version
        {
            get
            {
                ReadUnloadedTable();
                return m_Version;
            }
        }

        /// <summary>
        /// Get Number of Billing Periods.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public Byte ActiveNbrBillingPeriods
        {
            get
            {
                Byte byCount = 0;

                ReadUnloadedTable();

                if (null != m_HAN2Limits)
                {
                    byCount = m_HAN2Limits.ActiveNumberOfBillingPeriods;
                }

                return byCount;
            }
        }

        /// <summary>
        /// Get Number of Block Periods
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public Byte ActiveNbrBlockPeriods
        {
            get
            {
                Byte byCount = 0;

                ReadUnloadedTable();

                if (null != m_HAN2Limits)
                {
                    byCount = m_HAN2Limits.ActiveNumberOfBlockPeriods;
                }

                return byCount;
            }
        }

        /// <summary>
        /// Get Number of Blocks.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public Byte ActiveNbrBlocks
        {
            get
            {
                Byte byCount = 0;

                ReadUnloadedTable();

                if (null != m_HAN2Limits)
                {
                    byCount = m_HAN2Limits.ActiveNumberOfBlocks;
                }

                return byCount;
            }
        }


        /// <summary>
        /// Get Next Number of Billing Periods.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public Byte NextNbrBillingPeriods
        {
            get
            {
                Byte byCount = 0;

                ReadUnloadedTable();

                if (null != m_HAN2Limits)
                {
                    byCount = m_HAN2Limits.NextNumberOfBillingPeriods;
                }

                return byCount;
            }
        }

        /// <summary>
        /// Get Next Number of Block Periods.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public Byte NextNbrBlockPeriods
        {
            get
            {
                Byte byCount = 0;

                ReadUnloadedTable();

                if (null != m_HAN2Limits)
                {
                    byCount = m_HAN2Limits.NextNumberOfBlockPeriods;
                }

                return byCount;
            }
        }

        /// <summary>
        /// Gets Next Number of Blocks.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public Byte NextNbrBlocks
        {
            get
            {
                Byte byCount = 0;

                ReadUnloadedTable();

                if (null != m_HAN2Limits)
                {
                    byCount = m_HAN2Limits.NextNumberOfBlocks;
                }

                return byCount;
            }
        }

        #endregion

        #region Members

        private ushort m_Version;
        private HAN2LimitRcd m_HAN2Limits;

        #endregion
    }

    /// <summary>
    /// Table 2439 - Actual HAN2 Limiting Table.
    /// Supports HAN RIB (Residential Inclining Block) pricing. 
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   This table is used by CHANMfgTable2440 and CHANMfgTable2441
    ///   This read/write table is supported only by OpenWay meters.     
    ///   </para>
    /// </remarks>

    public class CHANMfgTable2439 : AnsiTable
    {
        #region Constants
        private const int TABLE_TIMEOUT = 5000;
        private const uint SIZEOF = HAN2LimitRcd.SIZEOF;
        #endregion

        #region Public Methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        //
        public CHANMfgTable2439(CPSEM psem)
            : base(psem, 2439, SIZEOF, TABLE_TIMEOUT)
        {
            m_HAN2Limits = new HAN2LimitRcd();
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/12 jrf 2.60.22 TREQ5995 Created
        //
        public CHANMfgTable2439(PSEMBinaryReader reader)
            : base(2439, SIZEOF)
        {
            m_HAN2Limits = new HAN2LimitRcd();
            m_Reader = reader;

            ParseData();
            
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Full read of table 2439
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/23/12 DC  ?.??.?? N/A    Created
        //
        public override PSEMResponse Read()
        {
            if (null != m_Logger)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2439.Read");
            }

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result && null != m_DataStream)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Writes the table to the meter.
        /// </summary>
        /// <returns>The result of the write.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/?? ??  ?.??.?? N/A    Created
        //
        public PSEMResponse WriteNextScheduleData()
        {
            ushort offset = (ushort)HAN2LimitRcd.OFFSETOF_NEXT_NBR_BILLING_RECORDS;
            ushort count = (ushort)(HAN2LimitRcd.SIZEOF - HAN2LimitRcd.OFFSETOF_NEXT_NBR_BILLING_RECORDS);
            PSEMResponse WriteResponse = PSEMResponse.Err;

            if (null != m_Logger && null != m_DataStream && null != m_HAN2Limits)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2439.Write");
                m_DataStream.Position = offset;
                m_HAN2Limits.WriteData(m_Writer);

                WriteResponse = base.Write(offset, count);
            }

            return WriteResponse;
        }


        /// <summary>
        /// Parses the data out of the reader and into the member variables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        //
        private void ParseData()
        {
            if (null != m_HAN2Limits)
            {
                m_HAN2Limits.ParseData(m_Reader);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Active Number of Billing Periods (in Table 2440).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public Byte ActiveNbrBillingPeriods
        {
            get
            {
                Byte byCount = 0;

                ReadUnloadedTable();

                if (null != m_HAN2Limits)
                {
                    byCount = m_HAN2Limits.ActiveNumberOfBillingPeriods;
                }

                return byCount;
            }
        }

        /// <summary>
        /// Gets the Active Number of Block Periods (in Table 2440).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        public Byte ActiveNbrBlockPeriods
        {
            get
            {
                Byte byCount = 0;

                ReadUnloadedTable();

                if (null != m_HAN2Limits)
                {
                    byCount = m_HAN2Limits.ActiveNumberOfBlockPeriods;
                }

                return byCount;
            }
        }

        /// <summary>
        /// Gets the Active Number of Blocks (in Table 2440).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        public Byte ActiveNbrBlocks
        {
            get
            {
                Byte byCount = 0;

                ReadUnloadedTable();

                if (null != m_HAN2Limits)
                {
                    byCount = m_HAN2Limits.ActiveNumberOfBlocks;
                }

                return byCount;
            }
        }

        /// <summary>
        /// Gets or sets the Next Number of Billing Periods (in Table 2441).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        public Byte NextNbrBillingPeriods
        {
            get
            {
                Byte byCount = 0;

                ReadUnloadedTable();

                if (null != m_HAN2Limits)
                {
                    byCount = m_HAN2Limits.NextNumberOfBillingPeriods;
                }

                return byCount;
            }
            set
            {
                if (null != m_HAN2Limits)
                {
                    m_HAN2Limits.NextNumberOfBillingPeriods = value;
                    State = TableState.Dirty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Next Number of Block Periods (in Table 2441).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        public Byte NextNbrBlockPeriods
        {
            get
            {
                Byte byCount = 0;

                ReadUnloadedTable();

                if (null != m_HAN2Limits)
                {
                    byCount = m_HAN2Limits.NextNumberOfBlockPeriods;
                }

                return byCount;
            }
            set
            {
                if (null != m_HAN2Limits)
                {
                    m_HAN2Limits.NextNumberOfBlockPeriods = value;
                    State = TableState.Dirty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Next Number of Blocks (in Table 2441).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        public Byte NextNbrBlocks
        {
            get
            {
                Byte byCount = 0;

                ReadUnloadedTable();

                if (null != m_HAN2Limits)
                {
                    byCount = m_HAN2Limits.NextNumberOfBlocks;
                }

                return byCount;
            }
            set
            {
                if (null != m_HAN2Limits)
                {
                    m_HAN2Limits.NextNumberOfBlocks = value;
                    State = TableState.Dirty;
                }
            }
        }

        #endregion

        #region Members

        private HAN2LimitRcd m_HAN2Limits;

        #endregion
    }

    /// <summary>
    /// ConfigBitFieldRcd - for Table 2440 and 2441.
    /// </summary>

    public class ConfigBitFieldRcd
    {
        #region Constants
        internal const uint SIZEOF = 2;

        /// <summary>
        /// The bit field definition
        /// </summary>
        [Flags]
        public enum ConfigBitField : ushort
        {
            /// <summary>No bits are set</summary>
            None = 0x0000,
            /// <summary>Block Pricing is enabled</summary>
            BlockPricingEnable = 0x0001,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.18 N/A    Created

        public ConfigBitFieldRcd()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Set/get Block Price Enable
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public bool BlockPricingEnable
        {
            get
            {
                return (m_BitField & ConfigBitField.BlockPricingEnable) == ConfigBitField.BlockPricingEnable;
            }
            set
            {
                if (value)
                {
                    m_BitField = m_BitField | ConfigBitField.BlockPricingEnable;
                }
                else
                {
                    m_BitField = m_BitField & ~ConfigBitField.BlockPricingEnable;
                }
            }
        }

        /// <summary>
        /// Gets or sets the raw bit field value.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.18 N/A    Created

        public ConfigBitField BitField
        {
            get
            {
                return m_BitField;
            }
            set
            {
                m_BitField = value;
            }
        }

        #endregion

        #region Members

        private ConfigBitField m_BitField;       // B0 - BlockPricingEnabled,  B15-B1 - Reserved

        #endregion
    }

    /// <summary>
    /// PublishPriceDataEntryRcd - for Table 2440 and 2441.
    /// </summary>

    public class PublishPriceDataEntryRcd
    {

        #region Constants

        private const uint RATE_LABEL_RCD_LEN = 13;
        private const int RATE_LABEL_LEN = 12;
        private const byte TRAILING_DIGITS_MASK = 0xF0;
        private const byte TRAILING_DIGITS_BIT_SHIFT = 4;

        #endregion

        #region Definitions
        /// <summary>
        /// The bit field definition
        /// </summary>
        [Flags]
        public enum PriceControlBitField : byte
        {
            /// <summary>No bits are set</summary>
            None = 0x0000,
            /// <summary>Block Pricing is enabled</summary>
            PriceAcknowledgement = 0x0001,
        }



        #endregion      

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        //        
        public PublishPriceDataEntryRcd()
        {
            m_ProviderId = 0;
            m_usCurrency = 0;
            m_PriceTrail = 0;
            m_byPriceControl = 0;
            m_lstRateLabels = new List<string>();
        }

        /// <summary>
        /// This method parses the data of a publish price data entry record.
        /// </summary>
        /// <param name="Reader">PSEM binary reader containing the record data.</param>
        /// <param name="uiNumberOfRateLabels">The number of rate labels.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.23 N/A    Created
        //
        public void ParseData(PSEMBinaryReader Reader, uint uiNumberOfRateLabels)
        {
            if (null != Reader)
            {
                m_ProviderId = Reader.ReadUInt32();
                m_usCurrency = Reader.ReadUInt16();
                m_PriceTrail = Reader.ReadByte();
                PriceControl = Reader.ReadByte();

                byte byRateLabelLength = 0;
                string strRateLabel = "";
                Byte[] labelBytes = new Byte[RATE_LABEL_LEN];

                if (null != m_lstRateLabels)
                {
                    for (int i = 0; i < uiNumberOfRateLabels; i++)
                    {
                        byRateLabelLength = Math.Min(Reader.ReadByte(), (Byte)(RATE_LABEL_LEN));
                        labelBytes = Reader.ReadBytes(RATE_LABEL_LEN);
                        strRateLabel = System.Text.Encoding.UTF8.GetString(labelBytes, 0, byRateLabelLength);

                        m_lstRateLabels.Add(strRateLabel);
                    }
                }
            }
        }

        /// <summary>
        /// This method writes the data of a publish price data entry record to a PSEMBinaryWriter.
        /// </summary>
        /// <param name="Writer">PSEM binary Writer to write the record data.</param>
        /// <param name="uiNumberOfRateLabels">The number of rate labels.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.23 N/A    Created
        //
        public void WriteData(PSEMBinaryWriter Writer, uint uiNumberOfRateLabels)
        {
            byte byRateLabelLen = 0;

            if (null != Writer)
            {
                Writer.Write(m_ProviderId);
                Writer.Write(m_usCurrency);
                Writer.Write(m_PriceTrail);
                Writer.Write(m_byPriceControl);

                for (int i = 0; i < uiNumberOfRateLabels; i++)
                {
                    //We should have a count of rate labels that matches the number of rate labels,
                    //but just in case...
                    if (null != m_lstRateLabels && i < m_lstRateLabels.Count)
                    {
                        byRateLabelLen = (byte)m_lstRateLabels[i].Length;
                        Writer.Write(byRateLabelLen);
                        Writer.Write(m_lstRateLabels[i], RATE_LABEL_LEN);
                    }
                    else
                    {
                        // We need to write something so write a blank label
                        Writer.Write((byte)0);
                        Writer.Write("", RATE_LABEL_LEN);
                    }
                }
            }
        }

        /// <summary>
        /// This static method returns the size of a publish price data entry record.
        /// </summary>
        /// <param name="uiNumberOfRateLabels">The number of rate labels.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.22 N/A    Created
        //
        public static uint GetSize(uint uiNumberOfRateLabels)
        {
            uint uiPublishPriceDataEntryRecordSize = 0;

            //Provider ID + currency + price trail + price control
            uiPublishPriceDataEntryRecordSize += (sizeof(uint) + sizeof(UInt16) + sizeof(byte) + sizeof(byte));

            //Rate labels
            uiPublishPriceDataEntryRecordSize += (uiNumberOfRateLabels * RATE_LABEL_RCD_LEN);

            return uiPublishPriceDataEntryRecordSize;
        }

        #endregion

        #region Public Properites

        /// <summary>
        /// Gets or sets the Provider ID.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public uint ProviderId
        {
            get
            {
                return m_ProviderId;
            }
            set
            {
                m_ProviderId = value;
            }
        }

        /// <summary>
        /// Gets or sets the price's currency.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public UnitOfCurrency Currency
        {
            get
            {
                UnitOfCurrency Currency = UnitOfCurrency.UndefinedCurrency;

                if (true == Enum.IsDefined(typeof(UnitOfCurrency), m_usCurrency))
                {
                    Currency = (UnitOfCurrency)m_usCurrency;
                }

                return Currency;
            }
            set
            {
                m_usCurrency = (ushort)value;
            }
        }

        /// <summary>
        /// Gets the symbol for the currency in use.  
        /// </summary>
        /// <returns>The symbol for the currency or an empty string if none is defined.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/22/12 jrf 2.60.23 TREQ5995 Created
        //  07/19/12 jrf 2.60.46 199803 Updated to return correct currency symbol for Canada.
        //
        public string CurrencySymbol
        {
            get
            {
                string strSymbol = "";

                switch (Currency)
                {
                    //Just worried about the dollar symbol for now.
                    case UnitOfCurrency.USDollar:
                    case UnitOfCurrency.CanadianDollar:
                    {
                        strSymbol = "$";
                        break;
                    }
                    default:
                    {
                        strSymbol = "";
                        break;
                    }
                }

                return strSymbol;
            }
        }

        /// <summary>
        /// The text representing the symbol for the local unit of currency 
        /// used in the price field.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/19/12 jrf 2.60.46 199803 Created.
        //
        public string CurrencyName
        {
            get
            {
                string strUOCText = "";

                switch (Currency)
                {
                    case UnitOfCurrency.UICFranc:
                        {
                            strUOCText = "UIC Franc ";
                            break;
                        }
                    case UnitOfCurrency.Lek:
                        {
                            strUOCText = "Lek ";
                            break;
                        }
                    case UnitOfCurrency.AlgerianDinar:
                        {
                            strUOCText = "Algerian Dinar ";
                            break;
                        }
                    case UnitOfCurrency.ArgentinePeso:
                        {
                            strUOCText = "Argentine Peso ";
                            break;
                        }
                    case UnitOfCurrency.AustralianDollar:
                        {
                            strUOCText = "Australian Dollar ";
                            break;
                        }
                    case UnitOfCurrency.BahamianDollar:
                        {
                            strUOCText = "Bahamian Dollar ";
                            break;
                        }
                    case UnitOfCurrency.BahrainiDinar:
                        {
                            strUOCText = "Bahraini Dinar ";
                            break;
                        }
                    case UnitOfCurrency.Taka:
                        {
                            strUOCText = "Taka ";
                            break;
                        }
                    case UnitOfCurrency.ArmenianDram:
                        {
                            strUOCText = "Armenian Dram ";
                            break;
                        }
                    case UnitOfCurrency.BarbadosDollar:
                        {
                            strUOCText = "Barbados Dollar ";
                            break;
                        }
                    case UnitOfCurrency.BermudianDollar:
                        {
                            strUOCText = "Bermudian Dollar ";
                            break;
                        }
                    case UnitOfCurrency.Ngultrum:
                        {
                            strUOCText = "Ngultrum ";
                            break;
                        }
                    case UnitOfCurrency.Boliviano:
                        {
                            strUOCText = "Boliviano ";
                            break;
                        }
                    case UnitOfCurrency.Pula:
                        {
                            strUOCText = "Pula ";
                            break;
                        }
                    case UnitOfCurrency.BelizeDollar:
                        {
                            strUOCText = "Belize Dollar ";
                            break;
                        }
                    case UnitOfCurrency.SolomonIslandsDollar:
                        {
                            strUOCText = "Solomon Islands Dollar ";
                            break;
                        }
                    case UnitOfCurrency.BruneiDollar:
                        {
                            strUOCText = "Brunei Dollar ";
                            break;
                        }
                    case UnitOfCurrency.Kyat:
                        {
                            strUOCText = "Kyat ";
                            break;
                        }
                    case UnitOfCurrency.BurundiFranc:
                        {
                            strUOCText = "Burundi Franc ";
                            break;
                        }
                    case UnitOfCurrency.Riel:
                        {
                            strUOCText = "Riel ";
                            break;
                        }
                    case UnitOfCurrency.CanadianDollar:
                        {
                            strUOCText = "Canadian Dollar ";
                            break;
                        }
                    case UnitOfCurrency.CapeVerdeEscudo:
                        {
                            strUOCText = "Cape Verde Escudo ";
                            break;
                        }
                    case UnitOfCurrency.CaymanIslandsDollar:
                        {
                            strUOCText = "Cayman Islands Dollar ";
                            break;
                        }
                    case UnitOfCurrency.SriLankaRupee:
                        {
                            strUOCText = "Sri Lanka Rupee ";
                            break;
                        }
                    case UnitOfCurrency.ChileanPeso:
                        {
                            strUOCText = "Chilean Peso ";
                            break;
                        }
                    case UnitOfCurrency.YuanRenminbi:
                        {
                            strUOCText = "Yuan Renminbi ";
                            break;
                        }
                    case UnitOfCurrency.ColombianPeso:
                        {
                            strUOCText = "Colombian Peso ";
                            break;
                        }
                    case UnitOfCurrency.ComoroFranc:
                        {
                            strUOCText = "Comoro Franc ";
                            break;
                        }
                    case UnitOfCurrency.CostaRicanColon:
                        {
                            strUOCText = "Costa Rican Colon ";
                            break;
                        }
                    case UnitOfCurrency.CroatianKuna:
                        {
                            strUOCText = "Croatian Kuna ";
                            break;
                        }
                    case UnitOfCurrency.CubanPeso:
                        {
                            strUOCText = "Cuban Peso ";
                            break;
                        }
                    case UnitOfCurrency.CzechKoruna:
                        {
                            strUOCText = "Czech Koruna ";
                            break;
                        }
                    case UnitOfCurrency.DanishKrone:
                        {
                            strUOCText = "Danish Krone ";
                            break;
                        }
                    case UnitOfCurrency.DominicanPeso:
                        {
                            strUOCText = "Dominican Peso ";
                            break;
                        }
                    case UnitOfCurrency.ElSalvadorColon:
                        {
                            strUOCText = "El Salvador Colon ";
                            break;
                        }
                    case UnitOfCurrency.EthiopianBirr:
                        {
                            strUOCText = "Ethiopian Birr ";
                            break;
                        }
                    case UnitOfCurrency.Nakfa:
                        {
                            strUOCText = "Nakfa ";
                            break;
                        }
                    case UnitOfCurrency.Kroon:
                        {
                            strUOCText = "Kroon ";
                            break;
                        }
                    case UnitOfCurrency.FalklandIslandsPound:
                        {
                            strUOCText = "Falkland Islands Pound ";
                            break;
                        }
                    case UnitOfCurrency.FijiDollar:
                        {
                            strUOCText = "Fiji Dollar ";
                            break;
                        }
                    case UnitOfCurrency.DjiboutiFranc:
                        {
                            strUOCText = "Djibouti Franc ";
                            break;
                        }
                    case UnitOfCurrency.Dalasi:
                        {
                            strUOCText = "Dalasi ";
                            break;
                        }
                    case UnitOfCurrency.GibraltarPound:
                        {
                            strUOCText = "Gibraltar Pound ";
                            break;
                        }
                    case UnitOfCurrency.Quetzal:
                        {
                            strUOCText = "Quetzal ";
                            break;
                        }
                    case UnitOfCurrency.GuineaFranc:
                        {
                            strUOCText = "Guinea Franc ";
                            break;
                        }
                    case UnitOfCurrency.GuyanaDollar:
                        {
                            strUOCText = "Guyana Dollar ";
                            break;
                        }
                    case UnitOfCurrency.Gourde:
                        {
                            strUOCText = "Gourde ";
                            break;
                        }
                    case UnitOfCurrency.Lempira:
                        {
                            strUOCText = "Lempira ";
                            break;
                        }
                    case UnitOfCurrency.HongKongDollar:
                        {
                            strUOCText = "Hong Kong Dollar ";
                            break;
                        }
                    case UnitOfCurrency.Forint:
                        {
                            strUOCText = "Forint ";
                            break;
                        }
                    case UnitOfCurrency.IcelandKrona:
                        {
                            strUOCText = "Iceland Krona ";
                            break;
                        }
                    case UnitOfCurrency.IndianRupee:
                        {
                            strUOCText = "Indian Rupee ";
                            break;
                        }
                    case UnitOfCurrency.Rupiah:
                        {
                            strUOCText = "Rupiah ";
                            break;
                        }
                    case UnitOfCurrency.IranianRial:
                        {
                            strUOCText = "Iranian Rial ";
                            break;
                        }
                    case UnitOfCurrency.IraqiDinar:
                        {
                            strUOCText = "Iraqi Dinar ";
                            break;
                        }
                    case UnitOfCurrency.NewIsraeliSheqel:
                        {
                            strUOCText = "New Israeli Sheqel ";
                            break;
                        }
                    case UnitOfCurrency.JamaicanDollar:
                        {
                            strUOCText = "Jamaican Dollar ";
                            break;
                        }
                    case UnitOfCurrency.Yen:
                        {
                            strUOCText = "Yen ";
                            break;
                        }
                    case UnitOfCurrency.Tenge:
                        {
                            strUOCText = "Tenge ";
                            break;
                        }
                    case UnitOfCurrency.JordanianDinar:
                        {
                            strUOCText = "Jordanian Dinar ";
                            break;
                        }
                    case UnitOfCurrency.KenyanShilling:
                        {
                            strUOCText = "Kenyan Shilling ";
                            break;
                        }
                    case UnitOfCurrency.NorthKoreanWon:
                        {
                            strUOCText = "North Korean Won ";
                            break;
                        }
                    case UnitOfCurrency.Won:
                        {
                            strUOCText = "Won ";
                            break;
                        }
                    case UnitOfCurrency.KuwaitiDinar:
                        {
                            strUOCText = "Kuwaiti Dinar ";
                            break;
                        }
                    case UnitOfCurrency.Som:
                        {
                            strUOCText = "Som ";
                            break;
                        }
                    case UnitOfCurrency.Kip:
                        {
                            strUOCText = "Kip ";
                            break;
                        }
                    case UnitOfCurrency.LebanesePound:
                        {
                            strUOCText = "Lebanese Pound ";
                            break;
                        }
                    case UnitOfCurrency.Loti:
                        {
                            strUOCText = "Loti ";
                            break;
                        }
                    case UnitOfCurrency.LatvianLats:
                        {
                            strUOCText = "Latvian Lats ";
                            break;
                        }
                    case UnitOfCurrency.LiberianDollar:
                        {
                            strUOCText = "Liberian Dollar ";
                            break;
                        }
                    case UnitOfCurrency.LibyanDinar:
                        {
                            strUOCText = "Libyan Dinar ";
                            break;
                        }
                    case UnitOfCurrency.LithuanianLitas:
                        {
                            strUOCText = "Lithuanian Litas ";
                            break;
                        }
                    case UnitOfCurrency.Pataca:
                        {
                            strUOCText = "Pataca ";
                            break;
                        }
                    case UnitOfCurrency.Kwacha:
                        {
                            strUOCText = "Kwacha ";
                            break;
                        }
                    case UnitOfCurrency.MalaysianRinggit:
                        {
                            strUOCText = "Malaysian Ringgit ";
                            break;
                        }
                    case UnitOfCurrency.Rufiyaa:
                        {
                            strUOCText = "Rufiyaa ";
                            break;
                        }
                    case UnitOfCurrency.Ouguiya:
                        {
                            strUOCText = "Ouguiya ";
                            break;
                        }
                    case UnitOfCurrency.MauritiusRupee:
                        {
                            strUOCText = "Mauritius Rupee ";
                            break;
                        }
                    case UnitOfCurrency.MexicanPeso:
                        {
                            strUOCText = "Mexican Peso ";
                            break;
                        }
                    case UnitOfCurrency.Tugrik:
                        {
                            strUOCText = "Tugrik ";
                            break;
                        }
                    case UnitOfCurrency.MoldovanLeu:
                        {
                            strUOCText = "Moldovan Leu ";
                            break;
                        }
                    case UnitOfCurrency.MoroccanDirham:
                        {
                            strUOCText = "Moroccan Dirham ";
                            break;
                        }
                    case UnitOfCurrency.RialOmani:
                        {
                            strUOCText = "Rial Omani ";
                            break;
                        }
                    case UnitOfCurrency.NamibiaDollar:
                        {
                            strUOCText = "Namibia Dollar ";
                            break;
                        }
                    case UnitOfCurrency.NepaleseRupee:
                        {
                            strUOCText = "Nepalese Rupee ";
                            break;
                        }
                    case UnitOfCurrency.NetherlandsAntillianGuilder:
                        {
                            strUOCText = "Netherlands Antillian Guilder ";
                            break;
                        }
                    case UnitOfCurrency.ArubanGuilder:
                        {
                            strUOCText = "Aruban Guilder ";
                            break;
                        }
                    case UnitOfCurrency.Vatu:
                        {
                            strUOCText = "Vatu ";
                            break;
                        }
                    case UnitOfCurrency.NewZealandDollar:
                        {
                            strUOCText = "New Zealand Dollar ";
                            break;
                        }
                    case UnitOfCurrency.CordobaOro:
                        {
                            strUOCText = "Cordoba Oro ";
                            break;
                        }
                    case UnitOfCurrency.Naira:
                        {
                            strUOCText = "Naira ";
                            break;
                        }
                    case UnitOfCurrency.NorwegianKrone:
                        {
                            strUOCText = "Norwegian Krone ";
                            break;
                        }
                    case UnitOfCurrency.PakistanRupee:
                        {
                            strUOCText = "Pakistan Rupee ";
                            break;
                        }
                    case UnitOfCurrency.Balboa:
                        {
                            strUOCText = "Balboa ";
                            break;
                        }
                    case UnitOfCurrency.Kina:
                        {
                            strUOCText = "Kina ";
                            break;
                        }
                    case UnitOfCurrency.Guarani:
                        {
                            strUOCText = "Guarani ";
                            break;
                        }
                    case UnitOfCurrency.NuevoSol:
                        {
                            strUOCText = "Nuevo Sol ";
                            break;
                        }
                    case UnitOfCurrency.PhilippinePeso:
                        {
                            strUOCText = "Philippine Peso ";
                            break;
                        }
                    case UnitOfCurrency.GuineaBissauPeso:
                        {
                            strUOCText = "Guinea Bissau Peso ";
                            break;
                        }
                    case UnitOfCurrency.QatariRial:
                        {
                            strUOCText = "Qatari Rial ";
                            break;
                        }
                    case UnitOfCurrency.RussianRuble:
                        {
                            strUOCText = "Russian Ruble ";
                            break;
                        }
                    case UnitOfCurrency.RwandaFranc:
                        {
                            strUOCText = "Rwanda Franc ";
                            break;
                        }
                    case UnitOfCurrency.SaintHelenaPound:
                        {
                            strUOCText = "Saint Helena Pound ";
                            break;
                        }
                    case UnitOfCurrency.Dobra:
                        {
                            strUOCText = "Dobra ";
                            break;
                        }
                    case UnitOfCurrency.SaudiRiyal:
                        {
                            strUOCText = "Saudi Riyal ";
                            break;
                        }
                    case UnitOfCurrency.SeychellesRupee:
                        {
                            strUOCText = "Seychelles Rupee ";
                            break;
                        }
                    case UnitOfCurrency.Leone:
                        {
                            strUOCText = "Leone ";
                            break;
                        }
                    case UnitOfCurrency.SingaporeDollar:
                        {
                            strUOCText = "Singapore Dollar ";
                            break;
                        }
                    case UnitOfCurrency.Dong:
                        {
                            strUOCText = "Dong ";
                            break;
                        }
                    case UnitOfCurrency.SomaliShilling:
                        {
                            strUOCText = "Somali Shilling ";
                            break;
                        }
                    case UnitOfCurrency.Rand:
                        {
                            strUOCText = "Rand ";
                            break;
                        }
                    case UnitOfCurrency.Lilangeni:
                        {
                            strUOCText = "Lilangeni ";
                            break;
                        }
                    case UnitOfCurrency.SwedishKrona:
                        {
                            strUOCText = "Swedish Krona ";
                            break;
                        }
                    case UnitOfCurrency.SwissFranc:
                        {
                            strUOCText = "Swiss Franc ";
                            break;
                        }
                    case UnitOfCurrency.SyrianPound:
                        {
                            strUOCText = "Syrian Pound ";
                            break;
                        }
                    case UnitOfCurrency.Baht:
                        {
                            strUOCText = "Baht ";
                            break;
                        }
                    case UnitOfCurrency.Paanga:
                        {
                            strUOCText = "Paanga ";
                            break;
                        }
                    case UnitOfCurrency.TrinidadTobagoDollar:
                        {
                            strUOCText = "Trinidad Tobago Dollar ";
                            break;
                        }
                    case UnitOfCurrency.UAEDirham:
                        {
                            strUOCText = "UAE Dirham ";
                            break;
                        }
                    case UnitOfCurrency.TunisianDinar:
                        {
                            strUOCText = "Tunisian Dinar ";
                            break;
                        }
                    case UnitOfCurrency.UgandaShilling:
                        {
                            strUOCText = "Uganda Shilling ";
                            break;
                        }
                    case UnitOfCurrency.Denar:
                        {
                            strUOCText = "Denar ";
                            break;
                        }
                    case UnitOfCurrency.EgyptianPound:
                        {
                            strUOCText = "Egyptian Pound ";
                            break;
                        }
                    case UnitOfCurrency.PoundSterling:
                        {
                            strUOCText = "Pound Sterling ";
                            break;
                        }
                    case UnitOfCurrency.TanzanianShilling:
                        {
                            strUOCText = "Tanzanian Shilling ";
                            break;
                        }
                    case UnitOfCurrency.USDollar:
                        {
                            strUOCText = "US Dollar ";
                            break;
                        }
                    case UnitOfCurrency.PesoUruguayo:
                        {
                            strUOCText = "Peso Uruguayo ";
                            break;
                        }
                    case UnitOfCurrency.UzbekistanSum:
                        {
                            strUOCText = "Uzbekistan Sum ";
                            break;
                        }
                    case UnitOfCurrency.Tala:
                        {
                            strUOCText = "Tala ";
                            break;
                        }
                    case UnitOfCurrency.YemeniRial:
                        {
                            strUOCText = "Yemeni Rial ";
                            break;
                        }
                    case UnitOfCurrency.ZambianKwacha:
                        {
                            strUOCText = "Zambian Kwacha ";
                            break;
                        }
                    case UnitOfCurrency.NewTaiwanDollar:
                        {
                            strUOCText = "New Taiwan Dollar ";
                            break;
                        }
                    case UnitOfCurrency.PesoConvertible:
                        {
                            strUOCText = "Peso Convertible ";
                            break;
                        }
                    case UnitOfCurrency.ZimbabweDollar:
                        {
                            strUOCText = "Zimbabwe Dollar ";
                            break;
                        }
                    case UnitOfCurrency.Manat:
                        {
                            strUOCText = "Manat ";
                            break;
                        }
                    case UnitOfCurrency.Cedi:
                        {
                            strUOCText = "Cedi ";
                            break;
                        }
                    case UnitOfCurrency.BolivarFuerte:
                        {
                            strUOCText = "Bolivar Fuerte ";
                            break;
                        }
                    case UnitOfCurrency.SudanesePound:
                        {
                            strUOCText = "Sudanese Pound ";
                            break;
                        }
                    case UnitOfCurrency.UruguayPeso:
                        {
                            strUOCText = "Uruguay Peso ";
                            break;
                        }
                    case UnitOfCurrency.SerbianDinar:
                        {
                            strUOCText = "Serbian Dinar ";
                            break;
                        }
                    case UnitOfCurrency.Metical:
                        {
                            strUOCText = "Metical ";
                            break;
                        }
                    case UnitOfCurrency.AzerbaijanianManat:
                        {
                            strUOCText = "Azerbaijanian Manat ";
                            break;
                        }
                    case UnitOfCurrency.NewLeu:
                        {
                            strUOCText = "New Leu ";
                            break;
                        }
                    case UnitOfCurrency.WIREuro:
                        {
                            strUOCText = "WIR Euro ";
                            break;
                        }
                    case UnitOfCurrency.WIRFranc:
                        {
                            strUOCText = "WIR Franc ";
                            break;
                        }
                    case UnitOfCurrency.TurkishLira:
                        {
                            strUOCText = "Turkish Lira ";
                            break;
                        }
                    case UnitOfCurrency.CFAFrancBEAC:
                        {
                            strUOCText = "CFA Franc BEAC ";
                            break;
                        }
                    case UnitOfCurrency.EastCaribbeanDollar:
                        {
                            strUOCText = "East Caribbean Dollar ";
                            break;
                        }
                    case UnitOfCurrency.CFAFrancBCEAO:
                        {
                            strUOCText = "CFA Franc BCEAO ";
                            break;
                        }
                    case UnitOfCurrency.CFPFranc:
                        {
                            strUOCText = "CFP Franc ";
                            break;
                        }
                    case UnitOfCurrency.EuropeanCompositeUnit:
                        {
                            strUOCText = "European Composite Unit ";
                            break;
                        }
                    case UnitOfCurrency.EuropeanMonetaryUnit:
                        {
                            strUOCText = "European Monetary Unit ";
                            break;
                        }
                    case UnitOfCurrency.EuropeanUnitOfAccount9:
                        {
                            strUOCText = "European Unit Of Account9 ";
                            break;
                        }
                    case UnitOfCurrency.EuropeanUnitOfAccount17:
                        {
                            strUOCText = "European Unit Of Account17 ";
                            break;
                        }
                    case UnitOfCurrency.Gold:
                        {
                            strUOCText = "Gold ";
                            break;
                        }
                    case UnitOfCurrency.SDR:
                        {
                            strUOCText = "SDR ";
                            break;
                        }
                    case UnitOfCurrency.Silver:
                        {
                            strUOCText = "Silver ";
                            break;
                        }
                    case UnitOfCurrency.Platinum:
                        {
                            strUOCText = "Platinum ";
                            break;
                        }
                    case UnitOfCurrency.TestCurrency:
                        {
                            strUOCText = "Test Currency ";
                            break;
                        }
                    case UnitOfCurrency.Palladium:
                        {
                            strUOCText = "Palladium ";
                            break;
                        }
                    case UnitOfCurrency.SurinamDollar:
                        {
                            strUOCText = "Surinam Dollar ";
                            break;
                        }
                    case UnitOfCurrency.MalagasyAriary:
                        {
                            strUOCText = "Malagasy Ariary ";
                            break;
                        }
                    case UnitOfCurrency.UnidadDeValorReal:
                        {
                            strUOCText = "Unidad De Valor Real ";
                            break;
                        }
                    case UnitOfCurrency.Afghani:
                        {
                            strUOCText = "Afghani  ";
                            break;
                        }
                    case UnitOfCurrency.Somoni:
                        {
                            strUOCText = "Somoni ";
                            break;
                        }
                    case UnitOfCurrency.Kwanza:
                        {
                            strUOCText = "Kwanza ";
                            break;
                        }
                    case UnitOfCurrency.BelarussianRuble:
                        {
                            strUOCText = "Belarussian Ruble ";
                            break;
                        }
                    case UnitOfCurrency.BulgarianLev:
                        {
                            strUOCText = "Bulgarian Lev ";
                            break;
                        }
                    case UnitOfCurrency.CongoleseFranc:
                        {
                            strUOCText = "Congolese Franc ";
                            break;
                        }
                    case UnitOfCurrency.ConvertibleMarks:
                        {
                            strUOCText = "Convertible Marks ";
                            break;
                        }
                    case UnitOfCurrency.Euro:
                        {
                            strUOCText = "Euro ";
                            break;
                        }
                    case UnitOfCurrency.MexicanUnidadDeInversion:
                        {
                            strUOCText = "Mexican Unidad De Inversion ";
                            break;
                        }
                    case UnitOfCurrency.Hryvnia:
                        {
                            strUOCText = "Hryvnia ";
                            break;
                        }
                    case UnitOfCurrency.Lari:
                        {
                            strUOCText = "Lari ";
                            break;
                        }
                    case UnitOfCurrency.Mvdol:
                        {
                            strUOCText = "Mvdol ";
                            break;
                        }
                    case UnitOfCurrency.Zloty:
                        {
                            strUOCText = "Zloty ";
                            break;
                        }
                    case UnitOfCurrency.BrazilianReal:
                        {
                            strUOCText = "Brazilian Real ";
                            break;
                        }
                    case UnitOfCurrency.UnidadesDeFomento:
                        {
                            strUOCText = "Unidades De Fomento ";
                            break;
                        }
                    case UnitOfCurrency.USDollarNextDay:
                        {
                            strUOCText = "US Dollar Next Day ";
                            break;
                        }
                    case UnitOfCurrency.USDollarSameDay:
                        {
                            strUOCText = "US Dollar Same Day ";
                            break;
                        }
                    case UnitOfCurrency.NoCurrency:
                        {
                            strUOCText = "No Currency ";
                            break;
                        }
                    case UnitOfCurrency.UndefinedCurrency:
                        {
                            strUOCText = "Undefined Currency ";
                            break;
                        }
                    default:
                        {
                            strUOCText = "";
                            break;
                        }
                }

                return strUOCText;
            }
        }


        /// <summary>
        /// Gets or sets the price's decimal trailing digits.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public byte PriceTrailingDigits
        {
            get
            {
                byte byTrailingDigits = (byte)(TRAILING_DIGITS_MASK & m_PriceTrail);

                byTrailingDigits = (byte)(byTrailingDigits >> TRAILING_DIGITS_BIT_SHIFT);

                return byTrailingDigits;
            }
            set
            {
                m_PriceTrail = (byte)((m_PriceTrail & ~TRAILING_DIGITS_MASK) | (value << TRAILING_DIGITS_BIT_SHIFT));
            }
        }

        /// <summary>
        /// Gets or sets the price control byte.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public byte PriceControl
        {
            get
            {
                return m_byPriceControl;
            }
            set
            {
                m_byPriceControl = value;
            }
        }

        /// <summary>
        /// Gets whether price acknowledgement is reqired.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 TREQ5995 Created
        //
        public bool PriceAcknowledgementRequired
        {
            get
            {
                return (m_byPriceControl & (byte)PriceControlBitField.PriceAcknowledgement) == (byte)PriceControlBitField.PriceAcknowledgement;
            }
            set
            {
                if (value)
                {
                    m_byPriceControl = (byte)((byte)m_byPriceControl | (byte)PriceControlBitField.PriceAcknowledgement);
                }
                else
                {
                    m_byPriceControl = (byte)((byte)m_byPriceControl & ~((byte)PriceControlBitField.PriceAcknowledgement));
                }
            }
        }

        /// <summary>
        /// Gets or sets the Rate Labels.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public List<string> RateLabels
        {
            get
            {
                List<string> RateLabels = null;

                if (null != m_lstRateLabels)
                {
                    RateLabels = m_lstRateLabels;
                }
                else
                {
                    RateLabels = new List<string>();
                }

                return RateLabels;
            }
            set
            {
                m_lstRateLabels = value;
            }
        }

        #endregion

        #region Members

        private uint m_ProviderId;
        private ushort m_usCurrency;
        private byte m_PriceTrail;
        private byte m_byPriceControl;
        private List<string> m_lstRateLabels;

        #endregion
    }

    /// <summary>
    /// BlockPriceEntryRcd - for Table 2440 and 2441.
    /// </summary>

    public class BlockPriceEntryRcd
    {
        #region Constants

        private static int ZERO = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public BlockPriceEntryRcd()
        {
            m_BlockPrice = 0;
            m_BlockThreshold = 0;
        }

        /// <summary>
        /// This method parses the data of a block price entry record.
        /// </summary>
        /// <param name="Reader">PSEM binary reader containing the record data.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.23 N/A    Created
        //
        public void ParseData(PSEMBinaryReader Reader)
        {
            if (null != Reader)
            {
                m_BlockPrice = Reader.ReadUInt32();
                m_BlockThreshold = (UInt48)Reader.ReadUInt48();
            }
        }

        /// <summary>
        /// This method writes the data of a block price entry record to a PSEMBinaryWriter.
        /// </summary>
        /// <param name="Writer">PSEM binary Writer to write the record data.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.23 N/A    Created
        //
        public void WriteData(PSEMBinaryWriter Writer)
        {
            if (null != Writer)
            {
                Writer.Write(m_BlockPrice);
                Writer.WriteUInt48(m_BlockThreshold);
            }
        }

        /// <summary>
        /// This method takes the raw threshold, multiplier and divisor and 
        /// formats them into a displayable string.
        /// </summary>
        /// <param name="uiMultiplier">The currency symbol.</param>
        /// <param name="uiDivisor">The number of trailing decimal digits.</param>
        /// <param name="strUnits">The units to use to display threshold.</param>
        /// <returns>A displayable threshold string.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 N/A    Created
        //  05/29/12 jrf 2.60.26 TREQ5995,5996,6032 Adding display of units for the threshold.
        //
        public string FormatThreshold(uint uiMultiplier, uint uiDivisor, string strUnits)
        {
            string strThreshold = "Not Used";

            // If the value is all FF's it's not used. Otherwise determine the value.
            if (BlockThreshold != UInt48.MaxValue)
            {
                float fltFormattedThreshold = (float)(BlockThreshold * uiMultiplier) / (float)uiDivisor;
                strThreshold = fltFormattedThreshold.ToString("f2", CultureInfo.CurrentCulture) + " " + strUnits;
            }

            return strThreshold;
        }

        /// <summary>
        /// This method takes the raw block price, unit of currency and number of trailing digits and 
        /// formats them into a displayable string.
        /// </summary>
        /// <param name="strCurrencySymbol">The currency symbol.</param>
        /// <param name="byTrailingDigits">The number of trailing decimal digits.</param>
        /// <returns>A displayable price string.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 N/A    Created
        //
        public string FormatPrice(string strCurrencySymbol, byte byTrailingDigits)
        {
            return FormatPrice(BlockPrice, strCurrencySymbol, byTrailingDigits);
        }


        /// <summary>
        /// This method takes a raw price, unit of currency and number of trailing digits and 
        /// formats them into a displayable string.
        /// </summary>
        /// <param name="uiPrice">The price.</param>
        /// <param name="strCurrencySymbol">The currency symbol.</param>
        /// <param name="byTrailingDigits">The number of trailing decimal digits.</param>
        /// <returns>A displayable price string.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/17/12 jrf 2.60.23 N/A    Created
        //
        public static string FormatPrice(UInt32 uiPrice, string strCurrencySymbol, byte byTrailingDigits)
        {
            string strPrice = "Not Used";

            // If the value is all FF's it's not used. Otherwise determine the value.
            if (uiPrice != uint.MaxValue)
            {
                strPrice = uiPrice.ToString(CultureInfo.InvariantCulture);

                int iDecimalIndex = strPrice.Length - byTrailingDigits;
                int iZerosToAdd = byTrailingDigits - strPrice.Length;

                if (0 < byTrailingDigits)
                {
                    // The decimal should be placed at the very beginning of the price
                    if (0 == iDecimalIndex)
                    {
                        strPrice = "0." + strPrice;
                    }
                    // The decimal occurs within the body of the price
                    else if (0 < iDecimalIndex)
                    {
                        strPrice = strPrice.Insert(iDecimalIndex, ".");
                    }
                    // More zeros should be added to show the correct number of decimal places
                    // in the price.
                    else
                    {
                        strPrice = "0." + ZERO.ToString("D" + iZerosToAdd.ToString("D", CultureInfo.CurrentCulture), CultureInfo.CurrentCulture)
                            + strPrice;
                    }
                }

                //ex.                      $ + 0.14
                strPrice = strCurrencySymbol + strPrice;
            }

            return strPrice;
        }

        /// <summary>
        /// This static method returns the size of a block price entry record.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.22 N/A    Created
        //
        public static uint GetSize()
        {
            uint uiBlockEntryRcdSize = 0;

            uiBlockEntryRcdSize += (sizeof(uint) + UInt48.SizeOf);

            return uiBlockEntryRcdSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Get/Set Block Price
        /// </summary>
        public uint BlockPrice
        {
            get
            {
                return m_BlockPrice;
            }
            set
            {
                m_BlockPrice = value;
            }
        }

        /// <summary>
        /// Get/Set Block Threshold (48-bit value)
        /// </summary>
        public UInt48 BlockThreshold
        {
            get
            {
                return m_BlockThreshold;
            }
            set
            {
                m_BlockThreshold = value;
            }
        }

        #endregion

        #region Members
        private uint m_BlockPrice;
        private UInt48 m_BlockThreshold;
        #endregion
    }

    /// <summary>
    /// BlockPriceRcd - for Table 2440 and 2441.
    /// </summary>

    public class BlockPriceRcd
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        //
        public BlockPriceRcd()
        {
            m_lstBlockPrices = new List<BlockPriceEntryRcd>();
        }

        /// <summary>
        /// This method parses the data of a block price record.
        /// </summary>
        /// <param name="Reader">PSEM binary reader containing the record data.</param>
        /// <param name="uiNumberOfNextBlocks">Number of blocks in the next schedule.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.23 N/A    Created
        //
        public void ParseData(PSEMBinaryReader Reader, uint uiNumberOfNextBlocks)
        {
            BlockPriceEntryRcd BlockPriceEntry = null;

            if (uiNumberOfNextBlocks < 255 && null != m_lstBlockPrices)
            {
                for (int i = 0; i < uiNumberOfNextBlocks; i++)
                {
                    BlockPriceEntry = new BlockPriceEntryRcd();
                    BlockPriceEntry.ParseData(Reader);
                    m_lstBlockPrices.Add(BlockPriceEntry);
                }
            }
        }

        /// <summary>
        /// This method writes the data of a block price record to a PSEMBinaryWriter.
        /// </summary>
        /// <param name="Writer">PSEM binary Writer to write the record data.</param>
        /// <param name="uiNumberOfNextBlocks">Number of blocks in the next schedule.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.23 N/A    Created
        //
        public void WriteData(PSEMBinaryWriter Writer, uint uiNumberOfNextBlocks)
        {
            for (int i = 0; i < uiNumberOfNextBlocks; i++)
            {
                //We should have a count of block price entry records that matches the number of next blocks,
                //but just in case...
                if (null != m_lstBlockPrices && i < m_lstBlockPrices.Count)
                {
                    m_lstBlockPrices[i].WriteData(Writer);
                }
            }
        }

        /// <summary>
        /// This static method returns the size of a block price record.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.22 N/A    Created
        //
        public static uint GetSize(uint uiNumberOfNextBlocks)
        {
            uint uiBlockPriceSize = 0;

            //Billing period start + billing period duration + standing charge
            uiBlockPriceSize += (uiNumberOfNextBlocks * BlockPriceEntryRcd.GetSize());

            return uiBlockPriceSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Block Price
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        //
        public List<BlockPriceEntryRcd> BlockPrice
        {
            get
            {
                List<BlockPriceEntryRcd> lstBlockPrices = null;

                if (null != m_lstBlockPrices)
                {
                    lstBlockPrices = m_lstBlockPrices;
                }
                else
                {
                    lstBlockPrices = new List<BlockPriceEntryRcd>();
                }

                return lstBlockPrices;
            }
            set
            {
                m_lstBlockPrices = value;
            }
        }

        #endregion

        #region Members

        private List<BlockPriceEntryRcd> m_lstBlockPrices;

        #endregion
    }

    /// <summary>
    /// BlockPeriodRcd - for Table 2440 and 2441.
    /// </summary>

    public class BlockPeriodRcd
    {
        #region Constants

        private readonly DateTime UTC_REFERENCE = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        //
        public BlockPeriodRcd()
        {
            m_StartOfBlockPeriod = 0;
            m_BlockPeriodDuration = UInt24.MinValue;
            m_StandingCharge = 0;
            m_lstBlocks = new List<BlockPriceEntryRcd>();
            m_lstBlockEventIDs = new List<uint>();
        }

        /// <summary>
        /// This method parses the data of a block period record.
        /// </summary>
        /// <param name="Reader">PSEM binary reader containing the record data.</param>
        /// <param name="uiNumberOfBlocks">Number of active blocks</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.23 N/A    Created
        //
        public void ParseData(PSEMBinaryReader Reader, uint uiNumberOfBlocks)
        {
            if (null != Reader)
            {
                m_StartOfBlockPeriod = Reader.ReadUInt32();
                m_BlockPeriodDuration = (UInt24)Reader.ReadUInt24();
                m_StandingCharge = Reader.ReadUInt32();

                BlockPriceEntryRcd BlockRcd = null;

                if (null != m_lstBlocks && 255 > uiNumberOfBlocks)
                {
                    for (int i = 0; i < uiNumberOfBlocks; i++)
                    {
                        BlockRcd = new BlockPriceEntryRcd();
                        BlockRcd.ParseData(Reader);
                        m_lstBlocks.Add(BlockRcd);
                    }
                }

                if (null != m_lstBlockEventIDs)
                {
                    for (int i = 0; i < uiNumberOfBlocks; i++)
                    {
                        m_lstBlockEventIDs.Add(Reader.ReadUInt32());
                    }
                }
            }
        }

        /// <summary>
        /// This method unit of currency and number of trailing digits and 
        /// formats the standing charge into a displayable string.
        /// </summary>
        /// <param name="strCurrencySymbol">The currency symbol.</param>
        /// <param name="byTrailingDigits">The number of trailing decimal digits.</param>
        /// <returns>A displayable price string.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.23 N/A    Created
        //
        public string FormatStandingCharge(string strCurrencySymbol, byte byTrailingDigits)
        {
            return BlockPriceEntryRcd.FormatPrice(StandingCharge, strCurrencySymbol, byTrailingDigits);
        }

        /// <summary>
        /// This static method returns the size of a block period record.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.22 N/A    Created
        //
        public static uint GetSize(uint NumberOfActiveBlocks)
        {
            uint uiBlockPeriodSize = 0;

            //Start of block period + period duration + standing charge
            uiBlockPeriodSize += (sizeof(uint) + UInt24.SizeOf + sizeof(uint));

            //Blocks
            uiBlockPeriodSize += (NumberOfActiveBlocks * BlockPriceEntryRcd.GetSize());
            
            //Event Ids
            uiBlockPeriodSize += (NumberOfActiveBlocks * sizeof(uint));

            return uiBlockPeriodSize;
        }
        

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the start of the block period.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public DateTime StartTime
        {
            get
            {
                return UTC_REFERENCE.AddSeconds(m_StartOfBlockPeriod);
            }
        }

        /// <summary>
        /// Gets or sets the block period duration (24-bit value)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public UInt24 BlockPeriodDuration
        {
            get
            {
                return m_BlockPeriodDuration;
            }
            set
            {
                m_BlockPeriodDuration = value;
            }
        }

        /// <summary>
        /// Gets or sets the standing charge
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public uint StandingCharge
        {
            get
            {
                return m_StandingCharge;
            }
            set
            {
                m_StandingCharge = value;
            }
        }

        /// <summary>
        /// Gets or sets the blocks
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public ReadOnlyCollection<BlockPriceEntryRcd> Blocks
        {
            get
            {
                List<BlockPriceEntryRcd> lstBlocks = null;

                if (null != m_lstBlocks)
                {
                    lstBlocks = m_lstBlocks;
                }
                else
                {
                    lstBlocks = new List<BlockPriceEntryRcd>();
                }

                return lstBlocks.AsReadOnly();
            }
            set
            {
                if (null != m_lstBlocks)
                {
                    m_lstBlocks.Clear();
                    m_lstBlocks.AddRange(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the block event ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public ReadOnlyCollection<uint> BlockEventIDs
        {
            get
            {
                List<uint> lstBlockEventIDs = null;

                if (null != m_lstBlockEventIDs)
                {
                    lstBlockEventIDs = m_lstBlockEventIDs;
                }
                else
                {
                    lstBlockEventIDs = new List<uint>();
                }

                return lstBlockEventIDs.AsReadOnly();
            }
            set
            {
                if (null != m_lstBlockEventIDs)
                {
                    m_lstBlockEventIDs.Clear();
                    m_lstBlockEventIDs.AddRange(value);
                }
            }
        }

        #endregion

        #region Members

        private uint m_StartOfBlockPeriod;
        private UInt24 m_BlockPeriodDuration;
        private uint m_StandingCharge;
        private List<BlockPriceEntryRcd> m_lstBlocks;
        private List<uint> m_lstBlockEventIDs;

        #endregion
    }

    /// <summary>
    /// BillingPeriodRcd - for Table 2440 and 2441.
    /// </summary>

    public class BillingPeriodRcd
    {
        #region Constants

        private readonly DateTime UTC_REFERENCE = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/12 ??  ?.??.?? N/A    Created
        //
        public BillingPeriodRcd()
        {
            m_BillingPeriodStart = 0;
            m_BillingPeriodDuration = UInt24.MinValue;
        }

        /// <summary>
        /// This method parses the data of a billing period record.
        /// </summary>
        /// <param name="Reader">PSEM binary reader containing the record data.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.23 N/A    Created
        //
        public void ParseData(PSEMBinaryReader Reader)
        {
            if (null != Reader)
            {
                m_BillingPeriodStart = Reader.ReadUInt32();
                m_BillingPeriodDuration = (UInt24)Reader.ReadUInt24();
            }
        }

        /// <summary>
        /// This method writes the data of a billing period record to a PSEMBinaryWriter.
        /// </summary>
        /// <param name="Writer">PSEM binary Writer containing the record data.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.23 N/A    Created
        //
        public void WriteData(PSEMBinaryWriter Writer)
        {
            if (null != Writer)
            {
                Writer.Write(m_BillingPeriodStart);
                Writer.WriteUInt24(m_BillingPeriodDuration);
            }
        }

        /// <summary>
        /// This static method returns the size of a billing period record.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.22 N/A    Created
        //
        public static uint GetSize()
        {
            uint uiBillingPeriodSize = 0;

            //Billing period start + billing period duration
            uiBillingPeriodSize += (sizeof(uint) + UInt24.SizeOf);

            return uiBillingPeriodSize;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the BillingPeriodStart.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/12 ??? ?.??.??        Created
        public DateTime BillingPeriodStart
        {
            get
            {
                return UTC_REFERENCE.AddSeconds(m_BillingPeriodStart);
            }
            set
            {
                m_BillingPeriodStart = (uint)((value - UTC_REFERENCE).TotalSeconds);
            }
        }

        /// <summary>
        /// Gets or sets the BillingPeriodDuration
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/12 ??? ?.??.??        Created
        public UInt24 BillingPeriodDuration
        {
            get
            {
                return m_BillingPeriodDuration;
            }
            set
            {
                m_BillingPeriodDuration = value;
            }
        }

        #endregion

        #region Members

        private uint m_BillingPeriodStart;
        private UInt24 m_BillingPeriodDuration;

        #endregion
        
    }

    /// <summary>
    /// NextBlockPeriodRcd - for Table 2440 and 2441.
    /// </summary>

    public class NextBlockPeriodRcd
    {
        #region Constants

        private readonly DateTime UTC_REFERENCE = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Public Methods
        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public NextBlockPeriodRcd()
        {
            m_StartOfBlockPeriod = 0;
            m_BlockPeriodDuration = UInt24.MinValue;
            m_StandingCharge = 0;
        }

        /// <summary>
        /// This static method returns the size of a next block period record.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.22 N/A    Created
        //
        public static uint GetSize()
        {
            uint uiBlockPeriodSize = 0;

            //Billing period start + billing period duration + standing charge
            uiBlockPeriodSize += (uint)(sizeof(uint) + UInt24.SizeOf + sizeof(uint));

            return uiBlockPeriodSize;
        }

        /// <summary>
        /// This method parses the data of a next block period record.
        /// </summary>
        /// <param name="Reader">PSEM binary reader containing the record data.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.23 N/A    Created
        //
        public void ParseData(PSEMBinaryReader Reader)
        {
            if (null != Reader)
            {
                m_StartOfBlockPeriod = Reader.ReadUInt32();
                m_BlockPeriodDuration = (UInt24)Reader.ReadUInt24();
                m_StandingCharge = Reader.ReadUInt32();
            }
        }

        /// <summary>
        /// This method writes the data of a next block period record to a PSEMBinaryWriter.
        /// </summary>
        /// <param name="Writer">PSEM binary writer.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/15/12 jrf 2.60.23 N/A    Created
        //
        public void WriteData(PSEMBinaryWriter Writer)
        {
            Writer.Write(m_StartOfBlockPeriod);
            Writer.WriteUInt24(m_BlockPeriodDuration);
            Writer.Write(m_StandingCharge);
        }
        
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets start of the block period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public DateTime StartOfBlockPeriod
        {
            get
            {
                return UTC_REFERENCE.AddSeconds(m_StartOfBlockPeriod);
            }
            set
            {
                m_StartOfBlockPeriod = (uint)(value - UTC_REFERENCE).TotalSeconds;
            }
        }

        /// <summary>
        /// Gets or sets the duration of the Block Period (24-bit value)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public UInt24 BlockPeriodDuration 
        {
            get
            {
                return m_BlockPeriodDuration;  
            }
            set
            {
                m_BlockPeriodDuration = value;
            }
        }

        /// <summary>
        /// Gets or sets the standing charge
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public uint StandingCharge
        {
            get
            {
                return m_StandingCharge;
            }
            set
            {
                m_StandingCharge = value;
            }
        }

        #endregion

        #region Members

        private uint m_StartOfBlockPeriod;
        private UInt24 m_BlockPeriodDuration;
        private uint m_StandingCharge;

        #endregion
    }

    /// <summary>
    /// Table 2440 - Active Block Price Schedule TABLE.
    /// Supports HAN RIB (Residential Inclining Block) pricing.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   This readonly table is supported only by OpenWay meters.     
    ///   </para>
    /// </remarks>

    public class CHANMfgTable2440 : AnsiTable
    {
        #region Constants
        private const int TABLE_TIMEOUT = 1000;
        private const int SCHEDULE_ID_LEN = 30;
        private const int RATE_LABEL_LEN = 12;

        private readonly DateTime UTC_REFERENCE = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="table2439">Dimension Table</param>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        //  02/06/13 AF  2.70.64 288082 Added a parameter to GetTableLength to avoid causing a psem table read of 2439
        //
        public CHANMfgTable2440(CPSEM psem, CHANMfgTable2439 table2439)
            : base(psem, 2440, GetTableLength(table2439, true), TABLE_TIMEOUT)
        {
            m_Configuration = new ConfigBitFieldRcd();
            m_ScheduleId = "";
            m_UTCTime = 0;
            m_CurrentBlockPeriodConsumptionDelivered = 0;
            m_PreviousBlockPeriodConsumptionDelivered = 0;
            m_ActiveBlockPeriod = null;
            m_ActiveBillingPeriod = null;
            m_PublishPriceData = null;
            m_Multiplier = UInt24.MinValue;
            m_Divisor = UInt24.MinValue;
            m_Table2439 = table2439;
            m_lstBillingPeriods = new List<BillingPeriodRcd>();
            m_lstBlockPeriods = new List<BlockPeriodRcd>();
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        /// <param name="table2439">Dimension Table</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/08/12 jrf 2.60.22 TREQ5995 Created
        //  02/06/13 AF  2.70.64 288082 Added a parameter to GetTableLength to avoid causing a psem table read of 2439
        //
        public CHANMfgTable2440(PSEMBinaryReader reader, CHANMfgTable2439 table2439)
            : base(2439, GetTableLength(table2439, false))
        {
            
            m_Configuration = new ConfigBitFieldRcd();
            m_ScheduleId = "";
            m_UTCTime = 0;
            m_CurrentBlockPeriodConsumptionDelivered = 0;
            m_PreviousBlockPeriodConsumptionDelivered = 0;
            m_ActiveBlockPeriod = null;
            m_ActiveBillingPeriod = null;
            m_PublishPriceData = null;
            m_Multiplier = UInt24.MinValue;
            m_Divisor = UInt24.MinValue;
            m_Table2439 = table2439;
            m_lstBillingPeriods = new List<BillingPeriodRcd>();
            m_lstBlockPeriods = new List<BlockPeriodRcd>();
            m_Reader = reader;

            ParseData(m_Table2439);
            
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Full read of table 2440
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/23/12 DC  ?.??.?? N/A    Created
        //  11/20/12 RCG 2.70.40 243543 Changing read to resize the table as needed
        //  02/06/13 AF  2.70.64 288082 Added a parameter to GetTableLength to avoid a table read
        //                              for EDL files
        //
        public override PSEMResponse Read()
        {
            uint CurrentTableSize = GetTableLength(m_Table2439, true);

            if (null != m_Logger)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2440Read");
            }

            // Check to see if we need to resize the table
            if (CurrentTableSize != m_Size)
            {
                ChangeTableSize(CurrentTableSize);
            }

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result && null != m_DataStream)
            {
                m_DataStream.Position = 0;

                ParseData(m_Table2439);
            }

            return Result;
        }

        /// <summary>
        /// Provide a string for pretty printout
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/23/12 DC  ?.??.?? N/A    Created
        
        public override string ToString()
        {
            ReadUnloadedTable();

            string strTableData = base.ToString() + Environment.NewLine;

            if (null != m_Configuration)
            {
                strTableData += "Configuration=" + m_Configuration.BlockPricingEnable + Environment.NewLine;
            }
            strTableData += "ScheduleId=" + m_ScheduleId + Environment.NewLine;
            strTableData += "UTCTime=" + m_UTCTime + Environment.NewLine;
            strTableData += "CurrentBlockPeriodConsumptionDelivered=" + m_CurrentBlockPeriodConsumptionDelivered + Environment.NewLine;
            strTableData += "PreviousBlockPeriodConsumptionDelivered=" + m_PreviousBlockPeriodConsumptionDelivered + Environment.NewLine;

            // tostring ActiveBlockPeriod
            if (null != m_ActiveBlockPeriod)
            {
                strTableData += "ActiveBlockPeriod" + Environment.NewLine;
                strTableData += "{" + Environment.NewLine;
                strTableData += "  StartOfBlockPeriod=" + m_ActiveBlockPeriod.StartTime + Environment.NewLine;

                strTableData += "  BlockPeriodDuration=" + m_ActiveBlockPeriod.BlockPeriodDuration + Environment.NewLine;
                strTableData += "  StandingCharge=" + m_ActiveBlockPeriod.StandingCharge + Environment.NewLine;

                
                if (null != m_Table2439 && m_Table2439.ActiveNbrBlocks > 0 && m_Table2439.ActiveNbrBlocks < 255)
                {
                    if (null != m_ActiveBlockPeriod.Blocks)
                    {
                        for (int Block = 0; Block < m_Table2439.ActiveNbrBlocks; Block++)
                        {
                            strTableData += "    BlockPrice=" + m_ActiveBlockPeriod.Blocks[Block].BlockPrice + Environment.NewLine;
                            strTableData += "    BlockThreshold=" + m_ActiveBlockPeriod.Blocks[Block].BlockThreshold + Environment.NewLine;
                        }
                    }

                    if (null != m_ActiveBlockPeriod.BlockEventIDs)
                    {
                        for (int Block = 0; Block < m_Table2439.ActiveNbrBlocks; Block++)
                        {
                            strTableData += "    BlockEventId=" + m_ActiveBlockPeriod.BlockEventIDs[Block] + Environment.NewLine;
                        }
                    }
                }
                strTableData += "}" + Environment.NewLine;
            }

            // tostring ActiveBillingPeriod
            if (null != m_ActiveBillingPeriod)
            {
                strTableData += "ActiveBillingPeriod" + Environment.NewLine;
                strTableData += "{" + Environment.NewLine;
                strTableData += "  BillingPeriodStart=" + m_ActiveBillingPeriod.BillingPeriodStart + Environment.NewLine;
                strTableData += "  BillingPeriodDuration=" + m_ActiveBillingPeriod.BillingPeriodDuration + Environment.NewLine;
                strTableData += "}" + Environment.NewLine;
            }

            // tostring PublishPriceData
            if (null != m_PublishPriceData)
            {
                strTableData += "PublishPriceData" + Environment.NewLine;
                strTableData += "{" + Environment.NewLine;
                strTableData += "  ProviderId" + m_PublishPriceData.ProviderId + Environment.NewLine;
                strTableData += "  Currency=" + m_PublishPriceData.Currency + Environment.NewLine;
                strTableData += "  PriceTrail=" + m_PublishPriceData.PriceTrailingDigits + Environment.NewLine;
                strTableData += "  PriceControl" + m_PublishPriceData.PriceControl + Environment.NewLine;

                if (null != m_Table2439 && m_Table2439.ActiveNbrBlocks > 0 && m_Table2439.ActiveNbrBlocks < 255)
                {
                    for (int CurrentBlock = 0; CurrentBlock < m_Table2439.ActiveNbrBlocks; CurrentBlock++)
                    {
                        strTableData += "    RateLabel=" + m_PublishPriceData.RateLabels[CurrentBlock] + Environment.NewLine;
                    }
                }
                strTableData += "}" + Environment.NewLine;
            }

            // tostring Multiplier and Divisor
            strTableData += "Multiplier=" + m_Multiplier + Environment.NewLine;
            strTableData += "Divisor=" + m_Divisor + Environment.NewLine;

            // get BillingPeriods
            if (null != m_lstBillingPeriods)
            {
                strTableData += "BillingPeriods" + Environment.NewLine;
                strTableData += "{" + Environment.NewLine;
                if (null != m_Table2439 && m_Table2439.ActiveNbrBillingPeriods > 0 && m_Table2439.ActiveNbrBillingPeriods < 255)
                {
                    for (int CurrentPeriod = 0; CurrentPeriod < m_Table2439.ActiveNbrBillingPeriods; CurrentPeriod++)
                    {
                        strTableData += "  BillingPeriodStart=" + m_lstBillingPeriods[CurrentPeriod].BillingPeriodStart + Environment.NewLine;
                        strTableData += "  BillingPeriodDuration=" + m_lstBillingPeriods[CurrentPeriod].BillingPeriodDuration + Environment.NewLine;
                    }
                }
                strTableData += "}" + Environment.NewLine;
            }

            // get BlockPeriodData
            if (null != m_lstBlockPeriods)
            {
                strTableData += "BlockPeriodData" + Environment.NewLine;
                strTableData += "{" + Environment.NewLine;
                if (null != m_Table2439 && m_Table2439.ActiveNbrBlockPeriods > 0 && m_Table2439.ActiveNbrBlockPeriods < 255)
                {
                    for (int BlockPeriod = 0; BlockPeriod < m_Table2439.ActiveNbrBlockPeriods; BlockPeriod++)
                    {
                        strTableData += "  StartOfBlockPeriod=" + m_lstBlockPeriods[BlockPeriod].StartTime + Environment.NewLine;
                        strTableData += "  BlockPeriodDuration=" + m_lstBlockPeriods[BlockPeriod].BlockPeriodDuration + Environment.NewLine;
                        strTableData += "  StandingCharge=" + m_lstBlockPeriods[BlockPeriod].StandingCharge + Environment.NewLine;

                        if (m_Table2439.ActiveNbrBlocks > 0 && m_Table2439.ActiveNbrBlocks < 255)
                        {
                            for (int Block = 0; Block < m_Table2439.ActiveNbrBlocks; Block++)
                            {
                                strTableData += "    BlockPrice=" + m_lstBlockPeriods[BlockPeriod].Blocks[Block].BlockPrice + Environment.NewLine;
                                strTableData += "    BlockThreshold=" + m_lstBlockPeriods[BlockPeriod].Blocks[Block].BlockThreshold + Environment.NewLine;
                            }

                            for (int Block = 0; Block < m_Table2439.ActiveNbrBlocks; Block++)
                            {
                                strTableData += "    BlockEventId=" + m_lstBlockPeriods[BlockPeriod].BlockEventIDs[Block] + Environment.NewLine;
                            }
                        }
                    }

                }
                strTableData += "}" + Environment.NewLine;
            }

            return strTableData;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper method to determine the length of the table.  The length depends on number of elements in the table as 
        /// defined in the Actual Limiting Table.
        /// </summary>
        /// <param name="table2439">Mfg Table 2439 object</param>
        /// <param name="ShouldRead2439">Allows us to prevent a read of 2439 when dealing with EDL files</param>
        /// <returns>the length in bytes of table</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/23/12 DC  ?.??.?? N/A    Created
        //  01/21/13 jrf 2.70.60 287484 Adding in a full table read of table 2439 to make sure we have the 
        //                              most recent data before deteriming table 2440's size.
        //  02/06/13 AF  2.70.64 288082 Added a parameter to avoid causing a psem table read of 2439
        //
        private static uint GetTableLength(CHANMfgTable2439 table2439, bool ShouldRead2439)
        {
            uint Size = ConfigBitFieldRcd.SIZEOF;       // m_Configuration
            Size += SCHEDULE_ID_LEN;                    // m_ScheduleId 
            Size += sizeof(UInt32);                     // m_UTCTime
            Size += UInt48.SizeOf;                      // m_CurrentBlockPeriodConsumptionDelivered
            Size += UInt48.SizeOf;                      // m_PreviousBlockPeriodConsumptionDelivered

            if (null != table2439)
            {
                if (ShouldRead2439)
                {
                    //Make sure we have the most recent data from mfg. table 2439 before we determine table size.
                    table2439.Read();
                }

                // size ActiveBlockPeriod
                Size += BlockPeriodRcd.GetSize(table2439.ActiveNbrBlocks);

                //size of active billing period
                Size += BillingPeriodRcd.GetSize();

                //size of publish price data
                Size += PublishPriceDataEntryRcd.GetSize(table2439.ActiveNbrBlocks);

                Size += UInt24.SizeOf;                      //multiplier
                Size += UInt24.SizeOf;                      //divisor

                //size of billing periods
                Size += (table2439.ActiveNbrBillingPeriods * BillingPeriodRcd.GetSize());

                // size ActiveBlockPeriod
                Size += (table2439.ActiveNbrBlockPeriods * BlockPeriodRcd.GetSize(table2439.ActiveNbrBlocks));
            }
                        
            return Size;
        }

        /// <summary>
        /// Parses the data out of the reader and into the member variables
        /// </summary>
        /// <param name="table2439">Mfg Table 2439 object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/26/12 DC  ?.??.??        Created
        //
        private void ParseData(CHANMfgTable2439 table2439)
        {
            if (null != m_Configuration)
            {
                m_Configuration.BitField = (ConfigBitFieldRcd.ConfigBitField)m_Reader.ReadUInt16();
            }
            m_ScheduleId = m_Reader.ReadString(SCHEDULE_ID_LEN);
            m_UTCTime = m_Reader.ReadUInt32();
            m_CurrentBlockPeriodConsumptionDelivered = (UInt48)m_Reader.ReadUInt48();
            m_PreviousBlockPeriodConsumptionDelivered = (UInt48)m_Reader.ReadUInt48();

            // get ActiveBlockPeriod
            m_ActiveBlockPeriod = new BlockPeriodRcd();
            m_ActiveBlockPeriod.ParseData(m_Reader, table2439.ActiveNbrBlocks);
            
            // get ActiveBillingPeriod
            m_ActiveBillingPeriod = new BillingPeriodRcd();
            m_ActiveBillingPeriod.ParseData(m_Reader);

            // get PublishPriceData
            m_PublishPriceData = new PublishPriceDataEntryRcd();
            m_PublishPriceData.ParseData(m_Reader, table2439.ActiveNbrBlocks);
            

            // get Multiplier and Divisor
            m_Multiplier = (UInt24)m_Reader.ReadUInt24();
            m_Divisor = (UInt24)m_Reader.ReadUInt24();

            // get BillingPeriods
            if (null != table2439 && null != m_lstBillingPeriods && table2439.ActiveNbrBillingPeriods > 0 && table2439.ActiveNbrBillingPeriods < 255)
            {
                BillingPeriodRcd BillingPeriod = null;
                m_lstBillingPeriods.Clear();

                for (int CurrentPeriod = 0; CurrentPeriod < table2439.ActiveNbrBillingPeriods; CurrentPeriod++)
                {
                    BillingPeriod = new BillingPeriodRcd();
                    BillingPeriod.ParseData(m_Reader);
                    m_lstBillingPeriods.Add(BillingPeriod);
                }
            }

            // get BlockPeriodData
            if (null != table2439 && null != m_lstBlockPeriods && table2439.ActiveNbrBlockPeriods > 0 && table2439.ActiveNbrBlockPeriods < 255)
            {
                BlockPeriodRcd BlockPeriod = null;
                m_lstBlockPeriods.Clear();

                for (int iBlockPeriod = 0; iBlockPeriod < table2439.ActiveNbrBlockPeriods; iBlockPeriod++)
                {
                    BlockPeriod = new BlockPeriodRcd();
                    BlockPeriod.ParseData(m_Reader, table2439.ActiveNbrBlocks);
                    m_lstBlockPeriods.Add(BlockPeriod);
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not block pricing is currently enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public bool BlockPricingEnabled
        {
            get
            {
                ReadUnloadedTable();
                bool blnEnabled = false;

                if (null != m_Configuration)
                {
                    blnEnabled = m_Configuration.BlockPricingEnable;
                }

                return blnEnabled;
            }
        }

        /// <summary>
        /// Get/Set Schedule ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/23/12 DC  ?.??.?? N/A    Created
        
        public String ScheduleId
        {
            get
            {
                ReadUnloadedTable();
                return m_ScheduleId;
            }
        }

        /// <summary>
        /// Gets the current HAN time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public DateTime CurrentHANTime
        {
            get
            {
                ReadUnloadedTable();
                return UTC_REFERENCE.AddSeconds(m_UTCTime);
            }
        }

        /// <summary>
        /// Gets the current block period's consumption delivered value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public UInt48 CurrentBlockPeriodConsumptionDelivered
        {
            get
            {
                ReadUnloadedTable();
                return m_CurrentBlockPeriodConsumptionDelivered;
            }
        }

        /// <summary>
        /// Gets the previous block period's consumption delivered value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created

        public UInt48 PreviousBlockPeriodConsumptionDelivered
        {
            get
            {
                ReadUnloadedTable();
                return m_PreviousBlockPeriodConsumptionDelivered;
            }
        }

        /// <summary>
        /// Gets the active block period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public BlockPeriodRcd ActiveBlockPeriod
        {
            get
            {
                ReadUnloadedTable();
                return m_ActiveBlockPeriod;
            }
        }

        /// <summary>
        /// Gets the active billing period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public BillingPeriodRcd ActiveBillingPeriod
        {
            get
            {
                ReadUnloadedTable();
                return m_ActiveBillingPeriod;
            }
        }

        /// <summary>
        /// Gets the publish price data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public PublishPriceDataEntryRcd PublishPriceData
        {
            get
            {
                ReadUnloadedTable();
                return m_PublishPriceData;
            }
        }

        /// <summary>
        /// Gets the multiplier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public UInt24 Multiplier
        {
            get
            {
                ReadUnloadedTable();
                return m_Multiplier;
            }
        }

        /// <summary>
        /// Gets the divisor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public UInt24 Divisor
        {
            get
            {
                ReadUnloadedTable();
                return m_Divisor;
            }
        }

        /// <summary>
        /// Gets the billing periods
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public ReadOnlyCollection<BillingPeriodRcd> BillingPeriods
        {
            get
            {
                ReadUnloadedTable();
                List<BillingPeriodRcd> lstBillingPeriods = null;

                if (null != m_lstBillingPeriods)
                {
                    lstBillingPeriods = m_lstBillingPeriods;
                }
                else
                {
                    lstBillingPeriods = new List<BillingPeriodRcd>();
                }

                return lstBillingPeriods.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the block periods
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/30/12 RCG 2.60.19 N/A    Created
        
        public ReadOnlyCollection<BlockPeriodRcd> BlockPeriods
        {
            get
            {
                ReadUnloadedTable();
                List<BlockPeriodRcd> lstBlockPeriods = null;

                if (null != m_lstBlockPeriods)
                {
                    lstBlockPeriods = m_lstBlockPeriods;
                }
                else
                {
                    lstBlockPeriods = new List<BlockPeriodRcd>();
                }

                return lstBlockPeriods.AsReadOnly();
            }
        }

        #endregion

        #region Members

        private ConfigBitFieldRcd m_Configuration;
        private string m_ScheduleId;                            // String Length defined as ACT_HAN2_LIM_TBL.SCHEDULE_ID_LEN
        private uint m_UTCTime;
        private UInt48 m_CurrentBlockPeriodConsumptionDelivered;   // UInt48
        private UInt48 m_PreviousBlockPeriodConsumptionDelivered;  // UInt48
        private BlockPeriodRcd m_ActiveBlockPeriod;
        private BillingPeriodRcd m_ActiveBillingPeriod;
        private PublishPriceDataEntryRcd m_PublishPriceData;
        private UInt24 m_Multiplier;  // UInt24
        private UInt24 m_Divisor;     // UInt24
        private List<BillingPeriodRcd> m_lstBillingPeriods;
        private List<BlockPeriodRcd> m_lstBlockPeriods;
        private CHANMfgTable2439 m_Table2439;

        #endregion
    }

    /// <summary>
    /// Table 2441 - Next Block Price Schedule TABLE.
    /// Supports HAN RIB (Residential Inclining Block) pricing.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   This read/write table is supported only by OpenWay meters.     
    ///   </para>
    /// </remarks>

    public class CHANMfgTable2441 : AnsiTable
    {
        #region Constants
        private const int TABLE_TIMEOUT = 5000;

        private const int SCHEDULE_ID_LEN = 30;
        private const int RATE_LABEL_LEN = 12;

        /// <summary>
        /// Activation Mode for Table 2441.
        /// </summary>
        public enum NextBlockPriceScheduleStatus
        {
            /// <summary>Tables have been Written, Ready for Commit</summary>
            [EnumDescription("Waiting for Commit")]
            WAITING_FOR_COMMIT = 0,
            /// <summary>Tables have been committed</summary>
            [EnumDescription("Committed")]
            COMMITTED = 1,
            /// <summary>No Data, need to write to tables</summary>
            [EnumDescription("Waiting for Valid Schedule")]
            WAITING_FOR_VALID_SCHEDULE = 0xFF,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="table2439">Dimension table</param>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        //
        public CHANMfgTable2441(CPSEM psem, CHANMfgTable2439 table2439)
            : base(psem, 2441, GetTableLength(table2439), TABLE_TIMEOUT)
        {
            m_Configuration = new ConfigBitFieldRcd();
            m_ScheduleId = "";
            m_PublishPriceData = new PublishPriceDataEntryRcd();
            m_Multiplier = UInt24.MinValue;
            m_Divisor = UInt24.MinValue;

            m_Table2439 = table2439;
            m_lstBillingPeriods = new List<BillingPeriodRcd>();
            m_lstBlockPeriodData = new List<NextBlockPeriodRcd>();

            m_lstBlockPrices = new List<BlockPriceRcd>();
            m_Status = NextBlockPriceScheduleStatus.WAITING_FOR_VALID_SCHEDULE;
        }

        /// <summary>
        /// Full read of table 2441
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/23/12 DC  ?.??.?? N/A    Created
        //
        public override PSEMResponse Read()
        {
            if (null != m_Logger)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2441Read");
            }

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result && null != m_DataStream)
            {
                m_DataStream.Position = 0;

                ParseData(m_Table2439);
            }

            return Result;
        }

        /// <summary>
        /// Write table to the Meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/12 ??  ?.??.??        Created
        //
        public override PSEMResponse Write()
        {
            PSEMResponse WriteResposne = PSEMResponse.Err;
            if (null != m_Logger)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CHANMfgTable2441.Write");
            }

            if (null != m_DataStream)
            {
                m_DataStream.Position = 0;
            }

            if (null != m_Writer && null != m_Table2439)
            {
                m_Writer.Write((ushort)m_Configuration.BitField);

                m_Writer.Write(m_ScheduleId, SCHEDULE_ID_LEN);
                // write PublishPriceData
                m_PublishPriceData.WriteData(m_Writer, m_Table2439.NextNbrBlocks);

                // Write Multiplier and Divisor as UInt24
                m_Writer.WriteUInt24(m_Multiplier);
                m_Writer.WriteUInt24(m_Divisor);

                // Write BillingPeriods
                if (m_Table2439.NextNbrBillingPeriods > 0 && m_Table2439.NextNbrBillingPeriods < 255)
                {
                    for (int CurrentPeriod = 0; CurrentPeriod < m_Table2439.NextNbrBillingPeriods; CurrentPeriod++)
                    {
                        if (CurrentPeriod < m_lstBillingPeriods.Count)
                        {
                            m_lstBillingPeriods[CurrentPeriod].WriteData(m_Writer);
                        }
                    }
                }

                // Write BlockPeriodData
                if (m_Table2439.NextNbrBlockPeriods > 0 && m_Table2439.NextNbrBlockPeriods < 255)
                {
                    for (int CurrentBlock = 0; CurrentBlock < m_Table2439.NextNbrBlockPeriods; CurrentBlock++)
                    {
                        if (CurrentBlock < m_lstBlockPeriodData.Count)
                        {
                            m_lstBlockPeriodData[CurrentBlock].WriteData(m_Writer);
                        }
                    }
                }

                // Write BlockPeriodPrices
                if (m_Table2439.NextNbrBlockPeriods > 0 && m_Table2439.NextNbrBlockPeriods < 255)
                {
                    for (int CurrentPrice = 0; CurrentPrice < m_Table2439.NextNbrBlockPeriods; CurrentPrice++)
                    {
                        if (CurrentPrice < m_lstBlockPrices.Count)
                        {
                            m_lstBlockPrices[CurrentPrice].WriteData(m_Writer, m_Table2439.NextNbrBlocks);
                        }
                    }
                }

                m_Writer.Write((byte)m_Status);

                WriteResposne = base.Write();
            }

            return WriteResposne;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper method to determine the length of the table.  The length depends on number of elements in the table as 
        /// defined in the Actual Limiting Table.
        /// </summary>
        /// <param name="table2439">Mfg Table 2439 object</param>
        /// <returns>the length in bytes of table</returns>
        private static uint GetTableLength(CHANMfgTable2439 table2439)
        {
            uint Size = ConfigBitFieldRcd.SIZEOF;           // m_Configuration
            Size += SCHEDULE_ID_LEN;                        // m_ScheduleId

            if (null != table2439)
            {
                //size of publish price data
                Size += PublishPriceDataEntryRcd.GetSize(table2439.NextNbrBlocks);

                // size Multiplier and Divisor
                Size += UInt24.SizeOf;              // m_Multiplier
                Size += UInt24.SizeOf;              // m_Divisor

                // size BillingPeriods
                Size += (table2439.NextNbrBillingPeriods * BillingPeriodRcd.GetSize());

                // size BlockPeriodData
                Size += (table2439.NextNbrBlockPeriods * NextBlockPeriodRcd.GetSize());

                // size BlockPrices
                Size += (table2439.NextNbrBlockPeriods * BlockPriceRcd.GetSize(table2439.NextNbrBlocks));
            }

            Size += sizeof(Byte);                   // m_Status

            return Size;
        }

        /// <summary>
        /// Parses the data out of the reader and into the member variables
        /// </summary>
        /// <param name="table2439">Mfg Table 2439 object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  ??/??/12 ??  ?.??.??        Created
        //
        private void ParseData(CHANMfgTable2439 table2439)
        {
            if (null != table2439)
            {
                if (null != m_Configuration)
                {
                    m_Configuration.BitField = (ConfigBitFieldRcd.ConfigBitField)m_Reader.ReadUInt16();
                }

                m_ScheduleId = m_Reader.ReadString(SCHEDULE_ID_LEN);

                if (null != m_PublishPriceData)
                {
                    // get PublishPriceData
                    m_PublishPriceData = new PublishPriceDataEntryRcd();
                    m_PublishPriceData.ParseData(m_Reader, table2439.NextNbrBlocks);
                }

                // get Multiplier and Divisor
                m_Multiplier = (UInt24)m_Reader.ReadUInt24();
                m_Divisor = (UInt24)m_Reader.ReadUInt24();

                // get BillingPeriods
                BillingPeriodRcd NextBillingPeriod = null;
                if (null != m_lstBillingPeriods && table2439.NextNbrBillingPeriods > 0 && table2439.NextNbrBillingPeriods < 255)
                {
                    for (int CurrentPeriod = 0; CurrentPeriod < table2439.NextNbrBillingPeriods; CurrentPeriod++)
                    {
                        NextBillingPeriod = new BillingPeriodRcd();
                        NextBillingPeriod.ParseData(m_Reader);
                        m_lstBillingPeriods.Add(NextBillingPeriod);
                    }
                }

                // get BlockPeriodData
                NextBlockPeriodRcd NextBlockPeriod = null;
                if (null != m_lstBlockPeriodData && table2439.NextNbrBlockPeriods > 0 && table2439.NextNbrBlockPeriods < 255)
                {
                    for (int CurrentBlock = 0; CurrentBlock < table2439.NextNbrBlockPeriods; CurrentBlock++)
                    {
                        NextBlockPeriod = new NextBlockPeriodRcd();
                        NextBlockPeriod.ParseData(m_Reader);
                        m_lstBlockPeriodData.Add(NextBlockPeriod);
                    }
                }
                
                // get BlockPeriodPrices
                BlockPriceRcd NextBlockPrice = null;
                if (null != m_lstBlockPeriodData && table2439.NextNbrBlockPeriods > 0 && table2439.NextNbrBlockPeriods < 255)
                {
                    for (int CurrentPrice = 0; CurrentPrice < table2439.NextNbrBlockPeriods; CurrentPrice++)
                    {
                        NextBlockPrice = new BlockPriceRcd();
                        NextBlockPrice.ParseData(m_Reader, table2439.NextNbrBlocks);
                        m_lstBlockPrices.Add(NextBlockPrice);
                    }
                }
                
                m_Status = (NextBlockPriceScheduleStatus)m_Reader.ReadByte();
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Get/Set Configuration
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public ConfigBitFieldRcd Configuration
        {
            get
            {
                ReadUnloadedTable();
                return m_Configuration;
            }
            set
            {
                State = TableState.Dirty;
                m_Configuration = value;
            }
        }

        /// <summary>
        /// Get/Set Schedule Id
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        // 
        public string ScheduleId
        {
            get
            {
                ReadUnloadedTable();
                return m_ScheduleId;
            }
            set
            {
                State = TableState.Dirty;
                m_ScheduleId = value;
            }
        }

        /// <summary>
        /// Get/Set
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public PublishPriceDataEntryRcd PublishPriceData
        {
            get
            {
                ReadUnloadedTable();
                return m_PublishPriceData;
            }
            set
            {
                State = TableState.Dirty;
                m_PublishPriceData = value;
            }
        }

        /// <summary>
        /// Get/Set
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public UInt24 Multiplier
        {
            get
            {
                ReadUnloadedTable();
                return m_Multiplier;
            }
            set
            {
                State = TableState.Dirty;
                m_Multiplier = value;
            }
        }

        /// <summary>
        /// Get/Set
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public UInt24 Divisor
        {
            get
            {
                ReadUnloadedTable();
                return m_Divisor;
            }
            set
            {
                State = TableState.Dirty;
                m_Divisor = value;
            }
        }

        /// <summary>
        /// Gets or sets the Billing Periods
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/12 RCG 2.60.18 N/A    Created

        public List<BillingPeriodRcd> BillingPeriods
        {
            get
            {
                List<BillingPeriodRcd> lstBillingPeriods = null;

                ReadUnloadedTable();

                if (null != m_lstBillingPeriods)
                {
                    lstBillingPeriods = m_lstBillingPeriods;
                }
                else
                {
                    lstBillingPeriods = new List<BillingPeriodRcd>();
                }

                return lstBillingPeriods;
            }
            set
            {
                m_lstBillingPeriods = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the Block Period Data
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/12 RCG 2.60.18 N/A    Created

        public List<NextBlockPeriodRcd> BlockPeriodData
        {
            get
            {
                List<NextBlockPeriodRcd> lstBlockPeriodData = null;

                ReadUnloadedTable();

                if (null != m_lstBlockPeriodData)
                {
                    lstBlockPeriodData = m_lstBlockPeriodData;
                }
                else
                {
                    lstBlockPeriodData = new List<NextBlockPeriodRcd>();
                }

                return lstBlockPeriodData;
            }
            set
            {
                m_lstBlockPeriodData = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gets or sets the block prices
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/12 RCG 2.60.18 N/A    Created

        public List<BlockPriceRcd> BlockPrices
        {
            get
            {
                List<BlockPriceRcd> lstBlockPrices = null;

                ReadUnloadedTable();

                if (null != m_lstBlockPrices)
                {
                    lstBlockPrices = m_lstBlockPrices;
                }
                else
                {
                    lstBlockPrices = new List<BlockPriceRcd>();
                }

                return lstBlockPrices;
            }
            set
            {
                m_lstBlockPrices = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Get/Set
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created

        public NextBlockPriceScheduleStatus NextTableState
        {
            get
            {
                ReadUnloadedTable();
                return m_Status;
            }
        }

        /// <summary>
        /// Get/Set
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/12 DC  ?.??.?? N/A    Created
        //
        public uint TableLength
        {
            get
            {
                return GetTableLength(m_Table2439);
            }
        }

        #endregion

        #region Members

        private ConfigBitFieldRcd m_Configuration;
        private string m_ScheduleId;
        private PublishPriceDataEntryRcd m_PublishPriceData;
        private UInt24 m_Multiplier;    //UInt24
        private UInt24 m_Divisor;       //UInt24

        private List<BillingPeriodRcd> m_lstBillingPeriods;                // dimension is ACT_HAN2_LIB_TBL.NEXT_NBR_BILLING_PERIODS
        private List<NextBlockPeriodRcd> m_lstBlockPeriodData;             // dimension is ACT_HAN2_LIB_TBL.NEXT_NBR_BLOCK_PERIODS
        private List<BlockPriceRcd> m_lstBlockPrices;                      // dimension is ACT_HAN2_LIB_TBL.NEXT_NBR_BLOCKS
        private NextBlockPriceScheduleStatus m_Status;

        private CHANMfgTable2439 m_Table2439;

        #endregion
    }
}



