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
//                              Copyright © 2012
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{

    #region OpenWayMFGTable2419

    /// <summary>
    /// MFG Table 371 (2419) - Actual MFG Extended Self Read Table
    /// </summary>
    public class OpenWayMFGTable2419 : AnsiTable
    {

        #region Definitions

        private const uint TABLE_SIZE = 5;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public OpenWayMFGTable2419(CPSEM psem)
            : base(psem, 2419, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public OpenWayMFGTable2419(PSEMBinaryReader reader)
            : base(2419, TABLE_SIZE)
        {
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2419.Read");

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
        /// The number of extended energy values that have been configured for extended self reads.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public byte NumberConfiguredExtendedEnergies
        {
            get
            {
                ReadUnloadedTable();

                return m_byNumCFGNonBillingEnergies;
            }
        }

        /// <summary>
        /// The number of instantaneous values that have been configured for extended Self reads.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public byte NumberConfiguredInstantaneousValues
        {
            get
            {
                ReadUnloadedTable();

                return m_byNumCFGInstantaneousValues;
            }
        }

        /// <summary>
        /// The number of extended self read entries stored in the extended self read data table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public UInt16 NumberExtendedSelfReadEntries
        {
            get
            {
                ReadUnloadedTable();

                return m_uiNumExtSelfReadEntries;
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
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        private void ParseData()
        {
            m_byNumCFGNonBillingEnergies = m_Reader.ReadByte();
            m_byNumCFGInstantaneousValues = m_Reader.ReadByte();
            m_uiNumExtSelfReadEntries = m_Reader.ReadUInt16();
            m_byNumPeriodicTableReads = m_Reader.ReadByte();
        }

        #endregion

        #region Members

        byte m_byNumCFGNonBillingEnergies = 0;
        byte m_byNumCFGInstantaneousValues = 0;
        UInt16 m_uiNumExtSelfReadEntries = 0;
        byte m_byNumPeriodicTableReads = 0;

        #endregion

    }

    #endregion

    #region OpenWayMFGTable2421

    /// <summary>
    /// MFG Table 373 (2421) - Extended Self Read Status Table
    /// </summary>
    public class OpenWayMFGTable2421 : AnsiTable
    {

        #region Definitions

        private const uint TABLE_SIZE = 10;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public OpenWayMFGTable2421(CPSEM psem)
            : base(psem, 2421, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public OpenWayMFGTable2421(PSEMBinaryReader reader)
            : base(2421, TABLE_SIZE)
        {
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2421.Read");

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
        /// The number of phases being monitored.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public byte MonitoringPhases
        {
            get
            {
                ReadUnloadedTable();

                return m_byMonitoringPhases;
            }
        }

        /// <summary>
        /// For poly meters, it is the actual meter form.  For mono meters, the value 12 is for a 12S and the value 255 is for a non-12S form (1S, 2S, 3S, 4S).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public byte MeterForm
        {
            get
            {
                ReadUnloadedTable();

                return m_byMeterForm;
            }
        }

        /// <summary>
        /// The sequence number of the first entry in the extended self read data table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public UInt32 FirstEntrySequenceNumber
        {
            get
            {
                ReadUnloadedTable();

                return m_uiFirstEntrySeqNo;
            }
        }

        /// <summary>
        /// The sequence number of the last entry in the extended self read data table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public UInt32 LastEntrySequenceNumber
        {
            get
            {
                ReadUnloadedTable();

                return m_uiLastEntrySeqNo;
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
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        private void ParseData()
        {
            m_byMonitoringPhases = m_Reader.ReadByte();
            m_byMeterForm = m_Reader.ReadByte();
            m_uiFirstEntrySeqNo = m_Reader.ReadUInt32();
            m_uiLastEntrySeqNo = m_Reader.ReadUInt32();
        }

        #endregion

        #region Members

        byte m_byMonitoringPhases = 0;
        byte m_byMeterForm = 0;
        UInt32 m_uiFirstEntrySeqNo = 0;
        UInt32 m_uiLastEntrySeqNo = 0;

        #endregion

    }

    #endregion

    #region OpenWayMFGTable2422

    /// <summary>
    /// MFG Table 374 (2422) - Extended Energy and Instantaneous Self Read Current Data Table
    /// </summary>
    public class OpenWayMFGTable2422 : AnsiTable
    {

        #region Definitions

        private const int TABLE_TIMEOUT = 500;
        private const uint NONBILLING_CURRENT_ENTRY_RCD_SIZE = 12;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        /// <param name="Table2419">Extended self read actual table.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public OpenWayMFGTable2422(CPSEM psem, OpenWayMFGTable2419 Table2419)
            : base(psem, 2422, DetermineTableSize(Table2419), TABLE_TIMEOUT)
        {
            m_byConfiguredEnergies = Table2419.NumberConfiguredExtendedEnergies;
            m_byConfguredInstantaneousValues = Table2419.NumberConfiguredInstantaneousValues;
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        /// <param name="Table2419">Extended self read actual table.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public OpenWayMFGTable2422(PSEMBinaryReader reader, OpenWayMFGTable2419 Table2419)
            : base(2422, DetermineTableSize(Table2419))
        {
            m_Reader = reader;
            m_byConfiguredEnergies = Table2419.NumberConfiguredExtendedEnergies;
            m_byConfguredInstantaneousValues = Table2419.NumberConfiguredInstantaneousValues;

            ParseData();
            
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2422.Read");

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
        /// The number of phases being monitored.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public byte MonitoringPhases
        {
            get
            {
                ReadUnloadedTable();

                return m_byMonitoringPhases;
            }
        }

        /// <summary>
        /// The current extended energy data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public ReadOnlyCollection<ExtendedCurrentEntryRecord> CurrentExtEnergyData
        {
            get
            {
                ReadUnloadedTable();

                return m_lstConfigExtEnergies.AsReadOnly();
            }
        }

        /// <summary>
        /// The current extended instantaneous data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public ReadOnlyCollection<ExtendedCurrentEntryRecord> CurrentExtInstantaneousData
        {
            get
            {
                ReadUnloadedTable();

                return m_lstConfigInstValues.AsReadOnly();
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
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        private void ParseData()
        {
            UInt32 uiLID = 0;
            double dblValue = 0.0;
            ExtendedCurrentEntryRecord CurrentEntryRcd = null;

            m_lstConfigExtEnergies.Clear();
            m_lstConfigInstValues.Clear();

            m_byMonitoringPhases = m_Reader.ReadByte();

            //Add in the configured energies
            for (int i = 0; i < m_byConfiguredEnergies; i++)
            {
                uiLID = m_Reader.ReadUInt32();
                dblValue = m_Reader.ReadDouble();
                
                CurrentEntryRcd = new ExtendedCurrentEntryRecord(uiLID, dblValue);                
                m_lstConfigExtEnergies.Add(CurrentEntryRcd);
            }

            //Add in the configured instantaneous values
            for (int i = 0; i < m_byConfguredInstantaneousValues; i++)
            {
                uiLID = m_Reader.ReadUInt32();
                dblValue = m_Reader.ReadDouble();
                
                CurrentEntryRcd = new ExtendedCurrentEntryRecord(uiLID, dblValue);
                m_lstConfigInstValues.Add(CurrentEntryRcd);
            }
        }

        /// <summary>
        /// This method determines the size of the table.
        /// </summary>
        /// <param name="Table2419">Extended self read actual table.</param>
        /// <returns>The size of the table.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        private static uint DetermineTableSize(OpenWayMFGTable2419 Table2419)
        {
            uint uiTableSize = 1; //Monitoring Phases

            //Add in extended energies size
            uiTableSize += NONBILLING_CURRENT_ENTRY_RCD_SIZE * Table2419.NumberConfiguredExtendedEnergies;

            //Add in instantaneous values size
            uiTableSize += NONBILLING_CURRENT_ENTRY_RCD_SIZE * Table2419.NumberConfiguredInstantaneousValues;

            return uiTableSize;
        }

        #endregion

        #region Members

        byte m_byMonitoringPhases = 0;
        byte m_byConfiguredEnergies = 0;
        byte m_byConfguredInstantaneousValues = 0;
        List<ExtendedCurrentEntryRecord> m_lstConfigExtEnergies = new List<ExtendedCurrentEntryRecord>();
        List<ExtendedCurrentEntryRecord> m_lstConfigInstValues = new List<ExtendedCurrentEntryRecord>();

        #endregion

    }

    #endregion

    #region OpenWayMFGTable2423

    /// <summary>
    /// MFG Table 375 (2423) - Extended Self Read Data Table
    /// </summary>
    public class OpenWayMFGTable2423 : AnsiTable
    {

        #region Definitions

        private const int TABLE_TIMEOUT = 5000;
        private const uint EXTENDED_SELF_READ_RCD_SIZE = 17;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        /// <param name="Table2419">Extended self read actual table.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //  02/08/12 jrf 2.53.39 TC7075 Removed storing the number of extended self read entries
        //                              and now store the whole table2419.
        //
        public OpenWayMFGTable2423(CPSEM psem, OpenWayMFGTable2419 Table2419)
            : base(psem, 2423, DetermineTableSize(Table2419), TABLE_TIMEOUT)
        {
            m_TimeFormat = (PSEMBinaryReader.TM_FORMAT)psem.TimeFormat;
            m_Table2419 = Table2419;
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        /// <param name="Table2419">Extended self read actual table.</param>
        /// <param name="iTimeFormat">The time format used in the meter.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //  02/08/12 jrf 2.53.39 TC7075 Removed storing the number of extended self read entries
        //                              and now store the whole table2419.
        //
        public OpenWayMFGTable2423(PSEMBinaryReader reader, OpenWayMFGTable2419 Table2419, int iTimeFormat)
            : base(2423, DetermineTableSize(Table2419))
        {
            m_Reader = reader;
            m_TimeFormat = (PSEMBinaryReader.TM_FORMAT)iTimeFormat;
            m_Table2419 = Table2419;

            ParseData();
            
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2423.Read");

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
        /// The extended self read data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //  02/08/12 jrf 2.53.39 TC7075 Resizing table if necessary.
        //
        public ReadOnlyCollection<ExtendedSelfReadRecord> ExtendedSelfReadData
        {
            get
            {

                uint uiCurrentTableSize = DetermineTableSize(m_Table2419);

                if (uiCurrentTableSize != m_Size)
                {
                    ChangeTableSize(uiCurrentTableSize);
                }

                ReadUnloadedTable();

                return m_lstExtSREntries.AsReadOnly();
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
        //  01/06/12 jrf 2.53.27 TREQ2904 Created
        //  02/08/12 jrf 2.53.39 TC7075 Now pulling number of extended self read entries from table.
        //
        private void ParseData()
        {
            UInt32 uiLID = 0;
            double dblValue = 0.0;
            DateTime dtTOO = DateTime.MinValue;
            ExtendedSelfReadRecord ExtSRRcd = null;

            m_lstExtSREntries.Clear();


            //Add in the extended self read entries
            for (int i = 0; i < m_Table2419.NumberExtendedSelfReadEntries; i++)
            {
                dtTOO = m_Reader.ReadLTIME(m_TimeFormat);
                uiLID = m_Reader.ReadUInt32();
                dblValue = m_Reader.ReadDouble();
                                
                ExtSRRcd = new ExtendedSelfReadRecord(dtTOO, uiLID, dblValue);
                m_lstExtSREntries.Add(ExtSRRcd);
            }

        }

        /// <summary>
        /// This method determines the size of the table.
        /// </summary>
        /// <param name="Table2419">Extended self read actual table.</param>
        /// <returns>The size of the table.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/21/11 jrf 2.53.10 TC5321 Created
        //
        private static uint DetermineTableSize(OpenWayMFGTable2419 Table2419)
        {
            uint uiTableSize = 0; 

            //Add in approprate number of extended self read entries
            uiTableSize += EXTENDED_SELF_READ_RCD_SIZE * Table2419.NumberExtendedSelfReadEntries;

            return uiTableSize;
        }

        #endregion

        #region Members

        private List<ExtendedSelfReadRecord> m_lstExtSREntries = new List<ExtendedSelfReadRecord>();
        private PSEMBinaryReader.TM_FORMAT m_TimeFormat;
        private OpenWayMFGTable2419 m_Table2419 = null;

        #endregion

    }

    #endregion

    #region Data Record Classes

    /// <summary>
    /// Record used to store current extended energy and instantaneous values.
    /// </summary>
    public class ExtendedCurrentEntryRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uiLID">The LID of measure.</param>
        /// <param name="dblValue">The value measured.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/09/12 jrf 2.53.27 TREQ2904 Created
        //
        public ExtendedCurrentEntryRecord(UInt32 uiLID, double dblValue)
        {
            m_LID = new LID(uiLID);
            m_dlbValue = dblValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The LID of the quantity being measured.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/09/12 jrf 2.53.27 TREQ2904 Created
        //
        public LID QuantityID
        {
            get
            {
                return m_LID;
            }
        }

        /// <summary>
        /// The value of the quantity being measured.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/09/12 jrf 2.53.27 TREQ2904 Created
        //
        public double Measurement
        {
            get
            {
                return m_dlbValue;
            }
        }

        #endregion

        #region Members

        private LID m_LID = null;
        private double m_dlbValue = 0.0;

        #endregion
    }

    /// <summary>
    /// Record used to store extended self read entries.
    /// </summary>
    public class ExtendedSelfReadRecord: ExtendedCurrentEntryRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dtTOO">The time of occurence of the self read.</param>
        /// <param name="uiLID">The LID of measure.</param>
        /// <param name="dblValue">The value measured.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/09/12 jrf 2.53.27 TREQ2904 Created
        //
        public ExtendedSelfReadRecord(DateTime dtTOO, UInt32 uiLID, double dblValue)
        :base(uiLID, dblValue)  
        {
            m_dtTOO = dtTOO;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The self read entry's time of occurence.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/09/12 jrf 2.53.27 TREQ2904 Created
        //
        public DateTime TimeOfOccurence
        {
            get
            {
                return m_dtTOO;
            }
        }

        #endregion

        #region Members

        private DateTime m_dtTOO;

        #endregion
    }

    /// <summary>
    /// Extended Self Read Configuration Qualifier
    /// </summary>
    public enum ExtendedSelfReadQualifier : byte
    {
        /// <summary>
        /// The Quantity is always included in the Extended Self Read data
        /// </summary>
        [EnumDescription("Always Read")]
        AlwaysListed = 0,
        /// <summary>
        /// The Quantity is only included when the corresponding phase is present in the meter
        /// </summary>
        [EnumDescription("Read When Present")]
        OnlyListedWhenPresent = 1,
    }

    /// <summary>
    /// Extended Self Read Configuration Record
    /// </summary>
    public class ExtendedSelfReadConfigRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="quantity">The configure quantity</param>
        /// <param name="qualifier">The qualifier used to determine if the quantity should be listed</param>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/12 RCG 2.53.35 TRQ3448 Created 
        
        public ExtendedSelfReadConfigRecord(LID quantity, ExtendedSelfReadQualifier qualifier)
        {
            m_Quantity = quantity;
            m_Qualifier = qualifier;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the LID for the Quantity
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/12 RCG 2.53.35 TRQ3448 Created 
        
        public LID Quantity
        {
            get
            {
                return m_Quantity;
            }
        }

        /// <summary>
        /// Gets the qualifier for the quantity
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------- -------------------------------------------
        // 01/27/12 RCG 2.53.35 TRQ3448 Created 
        
        public ExtendedSelfReadQualifier Qualifier
        {
            get
            {
                return m_Qualifier;
            }
        }

        #endregion

        #region Member Variables

        private LID m_Quantity;
        private ExtendedSelfReadQualifier m_Qualifier;

        #endregion
    }

    #endregion

}


