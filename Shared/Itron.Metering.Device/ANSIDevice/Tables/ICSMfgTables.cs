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
//                            Copyright © 2012 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;


namespace Itron.Metering.Device
{
    #region definitions
    /// <summary>
    /// Registration status enumeration
    /// </summary>
    public enum RegistrationStatus : byte
    {
        /// <summary>
        /// not registered, MT is not currently searching a new operator to register to
        /// </summary>
        [EnumDescription("Not registered, modem is not currently searching.")]
        NotRegisteredMTNotSearching = 0,
        /// <summary>
        /// registered, home network
        /// </summary>
        [EnumDescription("Registered, home network.")]
        RegisteredHomeNetwork = 1,
        /// <summary>
        /// not registered, but MT is currently searching a new operator to register to
        /// </summary>
        [EnumDescription("Not registered, modem is currently searching.")]
        NotRegisteredMTSearching = 2,
        /// <summary>
        /// registration denied
        /// </summary>
        [EnumDescription("Registration denied.")]
        RegistrationDenied = 3,
        /// <summary>
        /// unknown
        /// </summary>
        [EnumDescription("Unknown.")]
        Unknown = 4,
        /// <summary>
        /// registered, roaming
        /// </summary>
        [EnumDescription("Registered, roaming.")]
        RegisteredRoaming = 5,
        /// <summary>
        /// registered for "SMS only", home network
        /// </summary>
        [EnumDescription("Registered for \"SMS only\", home network.")]
        RegisteredSMSOnlyHomeNetwork = 6,
        /// <summary>
        /// registered for "SMS only", roaming
        /// </summary>
        [EnumDescription("Registered for \"SMS only\", roaming.")]
        RegisteredSMSOnlyRoaming = 7,
        /// <summary>
        /// attached for emergency bearer services only
        /// </summary>
        [EnumDescription("Attached for emergency bearer services only.")]
        AttachedEmergencyBearerServicesOnly = 8,
    }

    /// <summary>
    /// Link Connection State enumeration
    /// </summary>
    public enum LinkConnectionState : byte
    {
        /// <summary>
        /// Link Down
        /// </summary>
        [EnumDescription("Link Down.")]
        LinkDown = 0,
        /// <summary>
        /// Link Up
        /// </summary>
        [EnumDescription("Link Up.")]
        LinkUp = 1,
    }

    /// <summary>
    /// Link Connection State enumeration
    /// </summary>
    public enum ICMModuleStatus : byte
    {
        /// <summary>
        /// Default state on fresh device
        /// </summary>
        [EnumDescription("Default state on fresh device.")]
        Initial = 0,
        /// <summary>
        /// Manufacturing state
        /// </summary>
        [EnumDescription("Manufacturing state.")]
        ManufacturingState = 1,
        /// <summary>
        /// Prior to contacting configuration server
        /// </summary>
        [EnumDescription("Prior to contacting configuration server.")]
        PriorToContactingConfigurationServer = 4,
        /// <summary>
        /// Auto-configured and attempting to auto-register
        /// </summary>
        [EnumDescription("Auto-configured and attempting to auto-register.")]
        AutoConfiguredAttemptingAutoRegister = 5,
        /// <summary>
        /// Auto-registered with TMS waiting to be provisioned
        /// </summary>
        [EnumDescription("Auto-registered with TMS waiting to be provisioned.")]
        AutoRegisteredWaitingToBeProvisioned = 6,
        /// <summary>
        /// Provisioned
        /// </summary>
        [EnumDescription("Provisioned.")]
        Provisioned = 7,
    }

    /// <summary>
    /// ERT Type
    /// </summary>
    public enum ERTType : byte
    {
        /// <summary>
        /// Telemerty Devices
        /// </summary>
        [EnumDescription("Telemerty Devices")]
        TelemetryDevices = 0x09,

        /// <summary>
        /// Phase 1 Telemerty Devices
        /// </summary>
        [EnumDescription("Phase 1 Telemerty Devices")]
        Phase1TelemetryDevices = 0x49,

        /// <summary>
        /// 100W
        /// </summary>
        [EnumDescription("100W")]
        OneHundredW = 0x0B,

        /// <summary>
        /// 100W + LS
        /// </summary>
        [EnumDescription("100W + LS")]
        OneHundredWPlusLS = 0x4B,

        /// <summary>
        /// 100WP, 100WE
        /// </summary>
        [EnumDescription("100WP, 100WE")]
        OneHundredWPWE = 0x6B,

        /// <summary>
        /// 100WP, 100WE + LS
        /// </summary>
        [EnumDescription("100WP, 100WE + LS")]
        OneHundredWPWEPlusLS = 0x8B,

        /// <summary>
        /// 100WP, 100WE
        /// </summary>
        [EnumDescription("100WP, 100WE")]
        OneHundredWPWEId2 = 0xAB,

        /// <summary>
        /// 100WP, 100WE + LS
        /// </summary>
        [EnumDescription("100WP, 100WE + LS")]
        OneHundredWPWEPlusLSId2 = 0xCB,

        /// <summary>
        /// 100G
        /// </summary>
        [EnumDescription("100G")]
        OnehundredG = 0x0C,

        /// <summary>
        /// 100G P2
        /// </summary>
        [EnumDescription("100G P2")]
        OnehundredGP2 = 0x1C,

        /// <summary>
        /// 100G P2 + RD
        /// </summary>
        [EnumDescription("100G P2 + RD")]
        OnehundredGP2PlusRD = 0x5C,

        /// <summary>
        /// 100G P3
        /// </summary>
        [EnumDescription("100G P3")]
        OnehundredGP3 = 0x7C,

        /// <summary>
        /// 100G P4
        /// </summary>
        [EnumDescription("100G P4")]
        OnehundredGP4 = 0x9C,

        /// <summary>
        /// MSM
        /// </summary>
        [EnumDescription("MSM")]
        MSM = 0x0E,

        /// <summary>
        /// MSM PP
        /// </summary>
        [EnumDescription("MSM PP")]
        MSMPP = 0x4E,
    }

    /// <summary>
    /// The ICS Status Filter Alarms.  Each value represents the bit
    /// position of the specified alarm.
    /// </summary>
    public enum ICSStatusAlarms : ushort
    {
        /// <summary>The Power outage alarm - Bit 7</summary>
        [EnumDescription("Prmary Power Down")]
        PowerOutage = 7,

        /// <summary>The Tamper alarm - Bit 16 - Set when either the removal or inversion tamper is triggered.</summary>
        [EnumDescription("Inversion or Removal Tamper")]
        Tamper = 16,
    }

    /// <summary>
    /// An indication of the HAN Modules Status. The HAN module populated or not based on run time detection.
    /// </summary>
    public enum HANModuleStatus : byte
    {
        /// <summary>
        /// HAN Network disabled and chip not populated.
        /// </summary>
        [EnumDescription("HAN Network disabled and chip not populated")]
        DisabledChipNotPopulated = 0,
        /// <summary>
        /// HAN Network disabled and chip populated.
        /// </summary>
        [EnumDescription("HAN Network disabled and chip populated")]
        DisabledChipPopulated = 1,
        /// <summary>
        /// HAN Network enabled and chip not populated.
        /// </summary>
        [EnumDescription("HAN Network enabled and chip not populated")]
        EnabledChipNotPopulated = 2,
        /// <summary>
        /// HAN Network enabled and chip populated.
        /// </summary>
        [EnumDescription("HAN Network enabled and chip populated")]
        EnabledChipPopulated = 4,
    }

    /// <summary>
    /// An indication of the ERT Modules Status. The HAN module populated or not based on run time detection.
    /// </summary>
    public enum ERTModuleStatus : byte
    {
        /// <summary>
        /// ERT Radio disabled and chip not populated.
        /// </summary>
        [EnumDescription("ERT Radio disabled and chip not populated")]
        DisabledChipNotPopulated = 0,
        /// <summary>
        /// ERT Radio disabled and chip populated.
        /// </summary>
        [EnumDescription("ERT Radio disabled and chip populated")]
        DisabledChipPopulated = 1,
        /// <summary>
        /// ERT Radio enabled and chip not populated.
        /// </summary>
        [EnumDescription("ERT Radio enabled and chip not populated")]
        EnabledChipNotPopulated = 2,
        /// <summary>
        /// ERT Radio enabled and chip populated.
        /// </summary>
        [EnumDescription("ERT Radio enabled and chip populated")]
        EnabledChipPopulated = 4,
    }

    #endregion

    /// <summary>
    /// MFG Table 2508 (Itron 460)
    /// ERT Data
    /// </summary>
    public class ICMMfgTable2508ERTData : AnsiTable
    {
        #region Constants
        /// <summary>
        /// payload size hardcoded in TDL def for table ... may change
        /// </summary>
        public const byte ERT_PAYLOAD_SIZE = 49;

        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">Protocol instance being used by the device.</param>
        /// <param name="sTIMEFormat">Indicates the format of the time in the meter.</param>
        /// <param name="Table2510">The actual ERT dimension table.</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  03/27/13 jkw 2.80.10    n/a    Created
        //  09/09/13 jrf 2.85.39 WR 422369 Modified to pass in mfg. table 2510.
        //
        public ICMMfgTable2508ERTData(CPSEM psem, int sTIMEFormat, ICMMfgTable2510ERTActual Table2510)
            : base(psem, 2508, DetermineTableSize(sTIMEFormat, Table2510.NumberOfDataRecords), TABLE_TIMEOUT)
        {
            InitializeMembers(sTIMEFormat, Table2510);
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        /// <param name="sTIMEFormat">Indicates the format of the time in the meter.</param>
        /// <param name="Table2510">The actual ERT dimension table.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/30/13 jrf 2.85.34 WR 418110 Created
        //  09/09/13 jrf 2.85.39 WR 422369 Modified to pass in mfg. table 2510.
        //
        public ICMMfgTable2508ERTData(PSEMBinaryReader reader, int sTIMEFormat, ICMMfgTable2510ERTActual Table2510)
            : base(2508, DetermineTableSize(sTIMEFormat, Table2510.NumberOfDataRecords))
        {
            InitializeMembers(sTIMEFormat, Table2510);

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
        // 03/27/13 jkw 2.80.10 n/a    Created
        // 08/29/13 AF  2.85.31 WR418048 Added a call to the base class read method
        // 09/09/13 jrf 2.85.39 WR 422369 Rereading mfg. table 2510 and updating this table's
        //                                size if necessary before reading.
        //
        public override PSEMResponse Read()
        {
            // If the number of data records has changed then we need to resize the table.
            m_Table2510.Read();
            if (m_Table2510.NumberOfDataRecords != m_NumberOfDataRecords)
            {
                InitializeMembers(m_sTIMEFormat, m_Table2510);
                ChangeTableSize(DetermineTableSize(m_sTIMEFormat, m_NumberOfDataRecords));
            }

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2508.Read");

            for (int ndx = 0; ndx < m_NumberOfDataRecords; ndx++)
            {
                // Read the table one record at a time
                base.Read(ndx * (int)ERTConsumptionDataRecord.Size(m_sTIMEFormat), (ushort)ERTConsumptionDataRecord.Size(m_sTIMEFormat));

                //ERTStatisticsRecord.debugERTIDIndex = ndx; //debug [jkw]
                m_ERTConsumptionDataRecords[ndx] = new ERTConsumptionDataRecord(m_sTIMEFormat);
                m_ERTConsumptionDataRecords[ndx].Parse(m_Reader);

                //DEBUG
                //m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Time Recorded: " + m_ERTConsumptionDataRecords[ndx].TimeRecorded.ToString());
                //m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "ERT ID: " + m_ERTConsumptionDataRecords[ndx].ERTPayload.ERTId.ToString());
                //m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "ERT Type: " + m_ERTConsumptionDataRecords[ndx].ERTPayload.ERTType.ToDescription());
                //m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Message Number: " + m_ERTConsumptionDataRecords[ndx].ERTPayload.MessageNumber.ToString());
                //m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Utility ID: " + m_ERTConsumptionDataRecords[ndx].ERTPayload.UtilityId.ToString());
                //m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Sequence Number: " + m_ERTConsumptionDataRecords[ndx].ERTPayload.SequenceCounter.ToString());
                //END DEBUG

            }

            m_TableState = TableState.Loaded;

            return PSEMResponse.Ok;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the read time
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        // 08/30/13 jrf 2.85.34 WR 418110 Setting to only read when table state is not loaded.
        //
        public ERTConsumptionDataRecord[] ERTConsumptionDataRecords
        {
            get
            {
                ReadUnloadedTable();

                return m_ERTConsumptionDataRecords;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method initializes all member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/30/13 jrf 2.85.34 WR 418110 Created
        //  09/09/13 jrf 2.85.39 WR 422369 Modified to pass in mfg. table 2510.
        //
        private void InitializeMembers(int sTIMEFormat, ICMMfgTable2510ERTActual Table2510)
        {
            m_Table2510 = Table2510;
            m_sTIMEFormat = sTIMEFormat;
            m_NumberOfDataRecords = Table2510.NumberOfDataRecords;
            m_ERTConsumptionDataRecords = new ERTConsumptionDataRecord[m_NumberOfDataRecords];
        }  
        
        /// <summary>
        /// Parses the data from the specified reader.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/30/13 jrf 2.85.34 WR 418110 Created
        //
        private void ParseData()
        {
            if (m_Reader != null)
            {
                for (int i = 0; i < m_NumberOfDataRecords; i++)
                {   
                    m_ERTConsumptionDataRecords[i] = new ERTConsumptionDataRecord(m_sTIMEFormat);
                    m_ERTConsumptionDataRecords[i].Parse(m_Reader);
                }                                                       
            }
        }

        /// <summary>
        /// This method determines the size of the table.
        /// </summary>
        /// <returns>The size of the table.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        private static uint DetermineTableSize(int sTIMEFormat, byte numberOfRecords)
        {
            uint uiTableSize = 0;

            uiTableSize += numberOfRecords * ERTConsumptionDataRecord.Size(sTIMEFormat);

            return uiTableSize;
        }

        #endregion

        #region Members

        ICMMfgTable2510ERTActual m_Table2510;
        private byte m_NumberOfDataRecords;
        private int m_sTIMEFormat;
        private ERTConsumptionDataRecord[] m_ERTConsumptionDataRecords;

        #endregion
    }
    
    /// <summary>
    /// MFG Table 2509 (Itron 461)
    /// ERT Configuration Table.
    /// </summary>
    public class ICMMfgTable2509ERTConfigurationTable : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 7;
        private const uint EXTENDED_TABLE_SIZE = 18;

        // ERT Meters Supported Masks
        private const byte SUPPORT_100G_MASK = 0x01;
        private const byte SUPPORT_100W_MASK = 0x02;
        private const byte SUPPORT_100WPLUS_MASK = 0x04;

        #endregion

        #region Definitions

        /// <summary>
        /// Enumeration that identifies configuration fields. Enumeration values
        /// are set to config field offsets.
        /// </summary>
        public enum ConfigFields : ushort
        {
            /// <summary>
            /// Max ERT Records
            /// </summary>
            [EnumDescription("Maximum ERT Records")]
            MaxERTRecords = 0,
            /// <summary>
            /// ERT Utility ID
            /// </summary>
            [EnumDescription("ERT Utility ID")]
            ERTUtilityID = 1,
            /// <summary>
            /// ERT Data Lifetime
            /// </summary>
            [EnumDescription("ERT Data Lifetime")]
            ERTDataLifetime = 3,
            /// <summary>
            /// ERT Radio Enabled
            /// </summary>
            [EnumDescription("ERT Radio Enabled")]
            ERTRadio = 4,
            /// <summary>
            /// ERT Resting Channel Interval
            /// </summary>
            [EnumDescription("ERT Data Lifetime")]
            ERTRestingChannelInterval = 5,            
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //
        public ICMMfgTable2509ERTConfigurationTable(CPSEM psem)
            : base(psem, 2509, TABLE_SIZE)
        {
            InitializeMembers();

            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //
        public ICMMfgTable2509ERTConfigurationTable(PSEMBinaryReader reader)
            : base(2509, TABLE_SIZE)
        {
            InitializeMembers();

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
        //  03/26/13 jkw 2.80.10 n/a    Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2509.Read");

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Writes the specified field to the meter.
        /// </summary>
        /// <returns>The result of the write.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/14/13 jrf 2.80.38 TQ???? Created
        //
        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "OpenWayMFGTable2509.Write");

            // Resynch our members to the base's data array
            m_DataStream.Position = 0;

            m_Writer.Write(m_MaxRecords);
            m_Writer.Write(m_usUtilityID);
            m_Writer.Write(m_DataLifetime);
            m_Writer.Write(m_ERTRadio);
            m_Writer.Write(m_RestingChannelInterval);
            
            return base.Write();
        }

        /// <summary>
        /// Writes the specified field to the meter.
        /// </summary>
        /// <returns>The result of the write.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/14/13 jrf 2.80.38 TQ???? Created
        //
        public PSEMResponse Write(ConfigFields Field)
        {
            ushort usFieldSize = 0;

            m_DataStream.Position = (ushort)Field;

            switch (Field)
            {
                case ConfigFields.MaxERTRecords:
                    {
                        m_Writer.Write(m_MaxRecords);
                        usFieldSize = sizeof(byte);
                        break;
                    }
                case ConfigFields.ERTUtilityID:
                    {
                        m_Writer.Write(m_usUtilityID);
                        usFieldSize = sizeof(ushort);
                        break;
                    }
                case ConfigFields.ERTDataLifetime:
                    {
                        m_Writer.Write(m_DataLifetime);
                        usFieldSize = sizeof(byte);
                        break;
                    }
                case ConfigFields.ERTRadio:
                    {
                        m_Writer.Write(m_ERTRadio);
                        usFieldSize = sizeof(byte);
                        break;
                    }
                case ConfigFields.ERTRestingChannelInterval:
                    {
                        m_Writer.Write(m_RestingChannelInterval);
                        usFieldSize = sizeof(ushort);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("The config field is not supported", "Field");
                    }
            }

            return base.Write((ushort)Field, usFieldSize);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Resting Channel Interval (in minutes).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //  06/10/13 jrf 2.80.37 TQ8278 Changing to ushort.
        //
        public ushort RestingChannelInterval
        {
            get
            {
                ReadUnloadedTable();

                return m_RestingChannelInterval;
            }
        }

        /// <summary>
        /// ERT Radio, used to turn the ERT module's transceiver on/off.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //  06/10/13 jrf 2.80.37 TQ8278 Changing to return bool.
        //  09/30/13 jrf 3.00.10 TC15862 Added set.
        //
        public bool ERTRadioEnabled
        {
            get
            {
                ReadUnloadedTable();
                bool blnEnabled = false;

                if (1 == m_ERTRadio)
                {
                    blnEnabled = true;
                }

                return blnEnabled;
            }
            set
            {
                if (value)
                {
                    m_ERTRadio = 1;
                }
                else
                {
                    m_ERTRadio = 0;
                }

                State = TableState.Dirty;
            }        
        }

        /// <summary>
        /// Data Lifetime (in hours).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //
        public byte DataLifetime
        {
            get
            {
                ReadUnloadedTable();

                return m_DataLifetime;
            }
        }

        /// <summary>
        /// Utility ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //  06/10/13 jrf 2.80.37 TQ8278 Changing utility ID to ushort.
        //  06/14/13 jrf 2.80.38 TQ???? Added set.
        //
        public ushort UtilityID
        {
            get
            {
                ReadUnloadedTable();

                return m_usUtilityID;
            }
            set
            {
                m_usUtilityID = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// The max number of data records
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //
        public byte MaxRecords
        {
            get
            {
                ReadUnloadedTable();

                return m_MaxRecords;
            }
        }

        /// <summary>
        /// Maximum managed meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? MaxManagedMeters
        {
            get
            {
                ReadUnloadedTable();

                return m_MaxManagedMeters;
            }
        }

        /// <summary>
        /// Maximum unmanaged meters
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? MaxUnmanagedMeters
        {
            get
            {
                ReadUnloadedTable();

                return m_MaxUnmanagedMeters;
            }
        }

        /// <summary>
        /// Maximum managed threshold attempts
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? MaxManagedThresholdAttempts
        {
            get
            {
                ReadUnloadedTable();

                return m_MaxManagedThresholdAttempts;
            }
        }

        /// <summary>
        /// Threshold RSSI
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public sbyte? ThresholdRSSI
        {
            get
            {
                ReadUnloadedTable();

                return m_ThresholdRSSI;
            }
        }

        /// <summary>
        /// RSSI samples
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? RSSISamples
        {
            get
            {
                ReadUnloadedTable();

                return m_RSSISamples;
            }
        }

        /// <summary>
        /// Steal threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? StealThreshold
        {
            get
            {
                ReadUnloadedTable();

                return m_StealThreshold;
            }
        }

        /// <summary>
        /// 100G meter support
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public bool? Support100GMeter
        {
            get
            {
                ReadUnloadedTable();

                return (m_SupportedERTMeterType & SUPPORT_100G_MASK) == SUPPORT_100G_MASK;
            }
        }

        /// <summary>
        /// 100W meter support
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public bool? Support100WMeter
        {
            get
            {
                ReadUnloadedTable();

                return (m_SupportedERTMeterType & SUPPORT_100W_MASK) == SUPPORT_100W_MASK;
            }
        }

        /// <summary>
        /// 100W plus meter support
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public bool? Support100WPlusMeter
        {
            get
            {
                ReadUnloadedTable();

                return (m_SupportedERTMeterType & SUPPORT_100WPLUS_MASK) == SUPPORT_100WPLUS_MASK;
            }
        }

        /// <summary>
        /// Channel hop frequency multiplier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? ChannelHopFrequencyMultiplier
        {
            get
            {
                ReadUnloadedTable();

                return m_ChannelHopFrequencyMultiplier;
            }
        }

        /// <summary>
        /// Data store multiplier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? DataStoreMultiplier
        {
            get
            {
                ReadUnloadedTable();

                return m_DataStoreMultiplier;
            }
        }

        /// <summary>
        /// Camping channel timer.   This value (in seconds) determines how long the ICM will wait
        /// on a “predicted” channel for a valid 100G packet to arrive at the radio.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/14 AF  3.50.31 WR459392 Created
        //
        public byte? CampingChannelTimer
        {
            get
            {
                ReadUnloadedTable();

                return m_CampingChannelTimer;
            }
        }

        /// <summary>
        /// Conn down time. This value determines how long the ICM will wait in hours when there is 
        /// no data connectivity before releasing the 100Gs it is managing.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  03/15/16 AF  4.50.236 WR 660484  Created
        //
        public byte? ConnDownTime
        {
            get
            {
                ReadUnloadedTable();

                return m_ConnDownTime;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method initializes all member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/30/13 jrf 2.85.34 WR 418110 Created
        //  02/07/14 AF  3.50.31 WR 459392 Added new fields
        //  03/15/16 AF  4.50.236 WR 660484 Added Conn down time
        //
        private void InitializeMembers()
        {
            m_MaxRecords = 0;
            m_usUtilityID = 0;
            m_DataLifetime = 0;
            m_ERTRadio = 0;
            m_RestingChannelInterval = 0;
            m_MaxManagedMeters = null;
            m_MaxUnmanagedMeters = null;
            m_MaxManagedThresholdAttempts = null;
            m_ThresholdRSSI = null;
            m_RSSISamples = null;
            m_StealThreshold = null;
            m_SupportedERTMeterType = null;
            m_ChannelHopFrequencyMultiplier = null;
            m_DataStoreMultiplier = null;
            m_ConnDownTime = null;
            m_CampingChannelTimer = null;
        } 
        
        /// <summary>
        /// Parses the data read from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //  06/10/13 jrf 2.80.37 TQ8278 Changing utility ID to ushort and resting 
        //                              channel interval to ushort;
        //  02/07/14 AF  3.50.31 WR 459392 Added new fields - only present in ITRJ and ITRV
        //  03/15/16 AF 4.50.236 WR 660484 Added Conn down time
        //
        private void ParseData()
        {
            m_MaxRecords = m_Reader.ReadByte();
            m_usUtilityID = m_Reader.ReadUInt16();
            m_DataLifetime = m_Reader.ReadByte();
            m_ERTRadio = m_Reader.ReadByte();
            m_RestingChannelInterval = m_Reader.ReadUInt16();

            if (m_DataStream.Length >= EXTENDED_TABLE_SIZE)
            {
                m_MaxManagedMeters = m_Reader.ReadByte();
                m_MaxUnmanagedMeters = m_Reader.ReadByte();
                m_MaxManagedThresholdAttempts = m_Reader.ReadByte();
                m_ThresholdRSSI = m_Reader.ReadSByte();
                m_RSSISamples = m_Reader.ReadByte();
                m_StealThreshold = m_Reader.ReadByte();
                m_SupportedERTMeterType = m_Reader.ReadByte();
                m_ChannelHopFrequencyMultiplier = m_Reader.ReadByte();
                m_DataStoreMultiplier = m_Reader.ReadByte();
                m_ConnDownTime = m_Reader.ReadByte();
                m_CampingChannelTimer = m_Reader.ReadByte();
            }
        }

        #endregion

        #region Members

        byte m_MaxRecords;
        ushort m_usUtilityID;
        byte m_DataLifetime;
        byte m_ERTRadio;
        ushort m_RestingChannelInterval;
        byte? m_MaxManagedMeters;
        byte? m_MaxUnmanagedMeters;
        byte? m_MaxManagedThresholdAttempts;
        sbyte? m_ThresholdRSSI;
        byte? m_RSSISamples;
        byte? m_StealThreshold;
        byte? m_SupportedERTMeterType;
        byte? m_ChannelHopFrequencyMultiplier;
        byte? m_DataStoreMultiplier;
        byte? m_ConnDownTime;
        byte? m_CampingChannelTimer;
        #endregion
    }
    
    /// <summary>
    /// MFG Table 2510 (Itron 462)
    /// ERT Actual.
    /// </summary>
    public class ICMMfgTable2510ERTActual : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 3;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //
        public ICMMfgTable2510ERTActual(CPSEM psem)
            : base(psem, 2510, TABLE_SIZE)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //
        public ICMMfgTable2510ERTActual(PSEMBinaryReader reader)
            : base(2510, TABLE_SIZE)
        {
            InitializeMembers();
            
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
        //  03/26/13 jkw 2.80.10 n/a    Created
        //  09/19/13 jrf 2.90.01 WR 422369 Corrected number of table printed to comm log.
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2510.Read");

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
        /// Record size
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //
        public byte RecordSize
        {
            get
            {
                ReadUnloadedTable();

                return m_RecordSize;
            }
        }


        /// <summary>
        /// Number of statistics records.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //  08/30/13 jrf 2.85.34 WR 418110 Renamed to indicate statistics and not status.
        //
        public byte NumberOfStatisticsRecords
        {
            get
            {
                ReadUnloadedTable();

                return m_NumberOfStatusRecords;
            }
        }

        /// <summary>
        /// The number of data records
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //
        public byte NumberOfDataRecords
        {
            get
            {
                ReadUnloadedTable();

                return m_NumberOfDataRecords;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method initializes all member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/30/13 jrf 2.85.34 WR 418110 Created
        //
        private void InitializeMembers()
        {
            m_NumberOfDataRecords = 0;
            m_NumberOfStatusRecords = 0;
            m_RecordSize = 0;
        }
        
        /// <summary>
        /// Parses the data read from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //
        private void ParseData()
        {
            m_NumberOfDataRecords = m_Reader.ReadByte();
            m_NumberOfStatusRecords = m_Reader.ReadByte();
            m_RecordSize = m_Reader.ReadByte();
        }

        #endregion

        #region Members

        byte m_NumberOfDataRecords;
        byte m_NumberOfStatusRecords;
        byte m_RecordSize;

        #endregion
    }
    
    /// <summary>
    /// MFG Table 2511 (Itron 463)
    /// ERT Statistics
    /// </summary>
    public class ICMMfgTable2511ERTStatistics : AnsiTable
    {

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">Protocol instance being used by the device.</param>
        /// <param name="Table2510">The actual ERT dimension table.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        // 09/09/13 jrf 2.85.39 WR 422369 Modified to pass in mfg. table 2510.
        //
        public ICMMfgTable2511ERTStatistics(CPSEM psem, ICMMfgTable2510ERTActual Table2510)
            : base(psem, 2511, DetermineTableSize(Table2510.NumberOfStatisticsRecords))
        {
            InitializeMembers(Table2510);
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        /// <param name="Table2510">The actual ERT dimension table.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 n/a    Created
        //  09/09/13 jrf 2.85.39 WR 422369 Modified to pass in mfg. table 2510
        //
        public ICMMfgTable2511ERTStatistics(PSEMBinaryReader reader, ICMMfgTable2510ERTActual Table2510)
            : base(2511, DetermineTableSize(Table2510.NumberOfStatisticsRecords))
        {
            InitializeMembers(Table2510);
            
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
        // 03/27/13 jkw 2.80.10 n/a    Created
        // 08/29/13 AF  2.85.31 WR418048 Added a call to the base class read method
        // 09/09/13 jrf 2.85.39 WR 422369 Rereading mfg. table 2510 and updating this table's
        //                                size if necessary before reading.
        //
        public override PSEMResponse Read()
        {
            // If the number of statistics records has changed then we need to resize the table.
            m_Table2510.Read();
            if (m_Table2510.NumberOfStatisticsRecords != m_NumberOfStatRecords)
            {
                InitializeMembers(m_Table2510);
                ChangeTableSize(DetermineTableSize(m_NumberOfStatRecords));
            }
            
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2511.Read");

            for (int ndx = 0; ndx < m_NumberOfStatRecords; ndx++)
            {
                base.Read(ndx * (int)ERTStatisticsRecord.Size(), (ushort)ERTStatisticsRecord.Size());
                m_ERTStatRecords[ndx] = new ERTStatisticsRecord();
                m_ERTStatRecords[ndx].Parse(m_Reader);
            }

            m_TableState = TableState.Loaded;

            return PSEMResponse.Ok;
        }        

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the read time
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        // 08/30/13 jrf 2.85.34 WR 418110 Setting to only read when table state is not loaded.
        //
        public ERTStatisticsRecord[] ERTStatisiticsRecords
        {
            get
            {
                ReadUnloadedTable();

                return m_ERTStatRecords;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method initializes all member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/30/13 jrf 2.85.34 WR 418110 Created
        //  09/09/13 jrf 2.85.39 WR 422369 Modified to pass in mfg. table 2510.
        //
        private void InitializeMembers(ICMMfgTable2510ERTActual Table2510)
        {
            m_Table2510 = Table2510;
            m_NumberOfStatRecords = Table2510.NumberOfStatisticsRecords;
            m_ERTStatRecords = new ERTStatisticsRecord[m_NumberOfStatRecords];
        }

        /// <summary>
        /// Parses the data from the specified reader.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/30/13 jrf 2.85.34 WR 418110 Created
        //
        private void ParseData()
        {
            if (m_Reader != null)
            {
                for (int i = 0; i < m_NumberOfStatRecords; i++)
                {
                    m_ERTStatRecords[i] = new ERTStatisticsRecord();
                    m_ERTStatRecords[i].Parse(m_Reader);
                }
            }
        }
        
        /// <summary>
        /// This method determines the size of the table.
        /// </summary>
        /// <returns>The size of the table.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        private static uint DetermineTableSize(byte numberOfRecords)
        {
            uint uiTableSize = 0;

            uiTableSize += numberOfRecords * ERTStatisticsRecord.Size();

            return uiTableSize;
        }

        #endregion

        #region Members

        ICMMfgTable2510ERTActual m_Table2510;
        private byte m_NumberOfStatRecords;
        private ERTStatisticsRecord[] m_ERTStatRecords;

        #endregion
    }

    /// <summary>
    /// The ERT statisitics data table object.
    /// </summary>
    public class ERTStatisticsRecord
    {
        //public static UInt32[] debugERTIDs = { 0x12345678, 0x87654321, 0xABCDEF01, 0x01FEDCBA, 0x01 };
        //public static int debugERTIDIndex = 0;
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public ERTStatisticsRecord()
        {
        }

        /// <summary>
        /// Parses the data from the specified binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data to parse.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        // 09/05/13 AF  2.85.36 WR418048 Changed the data type of the avg rssi to signed byte
        //
        public void Parse(PSEMBinaryReader Reader)
        {
            m_ERTId = Reader.ReadUInt32();
            m_FrequencyLast24Hours = Reader.ReadByte();
            m_AverageRSSI = Reader.ReadSByte();
        }

        /// <summary>
        /// Gets the size of a data block.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public static uint Size()
        {
            uint uiSize = 6;

            return uiSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the average RSSI
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        // 09/05/13 AF  2.85.36 WR418048 Changed the data type of the avg rssi to signed byte
        //
        public sbyte AverageRSSI
        {
            get
            {
                return m_AverageRSSI;
            }
        }

        /// <summary>
        /// Gets the frequency in the last 24 hours
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public byte FrequencyLast24Hours
        {
            get
            {
                return m_FrequencyLast24Hours;
            }
        }

        /// <summary>
        /// Gets ERT serial number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public UInt32 ERTId
        {
            get
            {
                return m_ERTId;
            }
        }

        #endregion

        #region Member Variables
        
        UInt32 m_ERTId;
        byte m_FrequencyLast24Hours;
        sbyte m_AverageRSSI;

        #endregion
    }

    /// <summary>
    /// The ERT consumption data table object.
    /// </summary>
    public class ERTConsumptionDataRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public ERTConsumptionDataRecord(int timeFormat)
        {
            m_TimeFormat = timeFormat;
        }

        /// <summary>
        /// Parses the data from the specified binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data to parse.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        // 09/05/13 AF  2.85.36 WR418048 Changed the code for reading the time - the field
        //                              contains seconds since 1970, not minutes.
        //
        public void Parse(PSEMBinaryReader Reader)
        {
            // Read the time.
            DateTime ReferenceDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan ts = Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_TimeFormat);
            m_TimeRecorded = ReferenceDate + ts;

            // Read the payload
            m_ERTPayload = new ERTPayloadDataRecord();
            m_ERTPayload.Parse(Reader);
        }

        /// <summary>
        /// Gets the size of a data block.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public static uint Size(int sTIMEFormat)
        {
            uint uiSize = 0;

            if ((Itron.Metering.Communications.PSEM.PSEMBinaryReader.TM_FORMAT)sTIMEFormat == PSEMBinaryReader.TM_FORMAT.UINT32_TIME)
            {
                // Read time
                uiSize += sizeof(UInt32);

                // payload
                uiSize += (uint)(ICMMfgTable2508ERTData.ERT_PAYLOAD_SIZE);
            }

            return uiSize;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the read time
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        // 09/05/13 AF  2.85.36 WR418048 Change the time to local time
        //
        public DateTime TimeRecorded
        {
            get
            {
                return m_TimeRecorded.ToLocalTime();
            }
        }

        /// <summary>
        /// Gets the size of the ERT payload
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public byte SizeOfERTPayload
        {
            get
            {
                return ICMMfgTable2508ERTData.ERT_PAYLOAD_SIZE;
            }
        }

        /// <summary>
        /// Gets ERT payload.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public ERTPayloadDataRecord ERTPayload
        {
            get
            {
                return m_ERTPayload;
            }
        }

        #endregion

        #region Member Variables

        private int m_TimeFormat;
        private DateTime m_TimeRecorded;
        private ERTPayloadDataRecord m_ERTPayload;

        #endregion
    }

    /// <summary>
    /// The ERT payload object.
    /// </summary>
    public class ERTPayloadDataRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public ERTPayloadDataRecord()
        {
            m_EncryptedHourlyOffsets = new byte[16];
            m_EncryptedHourlyOffsets.Initialize();
        }

        /// <summary>
        /// Parses the data from the specified binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data to parse.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        // 09/05/13 AF  2.85.36 WR418048 Reversed the byte order of fields
        //
        public void Parse(PSEMBinaryReader Reader)
        {
            m_ProtocolID = Reader.ReadByte();

            byte[] byaTemp = Reader.ReadBytes(2);
            Array.Reverse(byaTemp);
            m_LengthAndHammingCode = BitConverter.ToUInt16(byaTemp, 0);
            
            m_MessageNumber = Reader.ReadByte();
            m_ERTType = (ERTType)Reader.ReadByte();

            byaTemp = Reader.ReadBytes(4);
            Array.Reverse(byaTemp);
            m_ERTId = BitConverter.ToUInt32(byaTemp, 0);

            byaTemp = Reader.ReadBytes(4);
            Array.Reverse(byaTemp);
            m_Time = BitConverter.ToUInt32(byaTemp, 0);

            m_SecurityAttributes = Reader.ReadByte();

            byaTemp = Reader.ReadBytes(2);
            Array.Reverse(byaTemp);
            m_UtilityId = BitConverter.ToUInt16(byaTemp, 0);

            byaTemp = Reader.ReadBytes(2);
            Array.Reverse(byaTemp);
            m_NetworkConfigurationWord = BitConverter.ToUInt16(byaTemp, 0);

            byaTemp = Reader.ReadBytes(2);
            Array.Reverse(byaTemp);
            m_ExtendedTamperField = BitConverter.ToUInt16(byaTemp, 0);

            m_SequenceCounter = Reader.ReadByte();
            m_EncryptedDataType = Reader.ReadByte();
            m_EncryptedMultiplierCompensation = Reader.ReadUInt24();
            m_EncryptedCurrentConsumption = Reader.ReadUInt32();
            m_EncryptedHourlyOffsets = Reader.ReadBytes(m_EncryptedHourlyOffsets.Length);
            m_CBCMAC = Reader.ReadUInt32();
        }

        /// <summary>
        /// Gets the size of a data block.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public static uint Size(int sTIMEFormat, byte sizeOfERTPayload)
        {
            return ICMMfgTable2508ERTData.ERT_PAYLOAD_SIZE;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the ERT Type
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public ERTType ERTType
        {
            get
            {
                return m_ERTType;
            }
        }

        /// <summary>
        /// Gets the ERT Id / ERT Serial Number / Device Id
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public UInt32 ERTId
        {
            get
            {
                return m_ERTId;
            }
        }

        /// <summary>
        /// Gets the sequence counter
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public byte SequenceCounter
        {
            get
            {
                return m_SequenceCounter;
            }
        }

        /// <summary>
        /// Gets the message number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public byte MessageNumber
        {
            get
            {
                return m_MessageNumber;
            }
        }

        /// <summary>
        /// Gets the Utility ID
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/27/13 jkw 2.80.10 n/a    Created
        //
        public ushort UtilityId
        {
            get
            {
                return m_UtilityId;
            }
        }

        #endregion

        #region Member Variables

        private byte m_ProtocolID;
        private UInt16 m_LengthAndHammingCode;
        private byte m_MessageNumber;
        private ERTType m_ERTType;
        private UInt32 m_ERTId;
        private UInt32 m_Time;
        private byte m_SecurityAttributes;
        private UInt16 m_UtilityId;
        private UInt16 m_NetworkConfigurationWord;
        private UInt16 m_ExtendedTamperField;
        private byte m_SequenceCounter;

        private byte m_EncryptedDataType;
        private uint m_EncryptedMultiplierCompensation;
        private UInt32 m_EncryptedCurrentConsumption;
        private byte[] m_EncryptedHourlyOffsets;
        private UInt32 m_CBCMAC;

        #endregion
    }

    /// <summary>
    /// Destination Address. Specifies either an email, IP or URL address.
    /// </summary>
    public class DestinationAddressRecord
    {
        #region Constants

        private const byte DESTINATION_SIZE = 64;
        private const byte IP_ADDRESS_LENGTH = 4;
        private const byte IP_PORT_INDEX = 4;

        #endregion

        #region Definitions

        /// <summary>
        /// Destination address types
        /// </summary>
        public enum AddressType : byte
        {

            /// <summary>
            /// Unspecified address
            /// </summary>
            [EnumDescription("Unset")]
            Unset = 0,
            /// <summary>
            /// Reserved Address (Do Not Use)
            /// </summary>
            [EnumDescription("Reserved Address (Do Not Use)")]
            Reserved = 1,
            /// <summary>
            /// E-mail address
            /// </summary>
            [EnumDescription("E-mail")]
            Email = 2,
            /// <summary>
            /// IP address
            /// </summary>
            [EnumDescription("IP")]
            IP = 3,
            /// <summary>
            /// URL address
            /// </summary>
            [EnumDescription("URL")]
            URL = 4,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public DestinationAddressRecord()
        {
            m_byType = 0;
            m_byLength = 0;
            m_abyDestination = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public DestinationAddressRecord(AddressType AddrType, byte[] abyAddress)
        {
            Format = AddrType;
            DestinationAddress = abyAddress;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.25 TQ8284 Created
        //
        public DestinationAddressRecord(IPAddress Address, ushort usPort)
        {
            byte[] abyFullAddress = null;
            AddressType AddressFormat = AddressType.Unset;

            try
            {
                if (null != Address)
                {
                    byte[] abyIPAddress = Address.GetAddressBytes();

                    AddressFormat = AddressType.IP;
                    abyFullAddress = new byte[abyIPAddress.Length + sizeof(ushort)];

                    //Add the IP Address
                    Format = AddressType.IP;
                    Array.Copy(abyIPAddress, abyFullAddress, abyIPAddress.Length);

                    //Add the port
                    Array.Copy(BitConverter.GetBytes(usPort), 0, abyFullAddress, IP_PORT_INDEX, sizeof(ushort));                    
                }
            }
            catch
            {
                AddressFormat = AddressType.Unset;
                abyFullAddress = null;
            }

            Format = AddressFormat;
            DestinationAddress = abyFullAddress;
        }

        /// <summary>
        /// Parses the data from the specified binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data to parse.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public void Parse(PSEMBinaryReader Reader)
        {
            m_byType = Reader.ReadByte(); 
            m_byLength = Reader.ReadByte(); 
            m_abyDestination = Reader.ReadBytes(DESTINATION_SIZE); 
        }

        /// <summary>
        /// Writes the data to the specified binary writer.
        /// </summary>
        /// <param name="Writer">The binary reader that contains the data to parse.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 jrf 2.80.19 TQ8278 Created
        //
        public void Write(PSEMBinaryWriter Writer)
        {
            Writer.Write(m_byType);
            Writer.Write(m_byLength);
            Writer.Write(m_abyDestination); 
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the type of destination address.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public AddressType Format
        {
            get
            {
                AddressType AddrType = AddressType.Unset;

                if (true == Enum.IsDefined(typeof(AddressType), m_byType))
                {
                    AddrType = (AddressType)m_byType;
                }

                return AddrType;
            }
            set
            {
                m_byType = (byte)value;
            }
        }
                
        /// <summary>
        /// Gets the record's destination address formatted for display.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public string DisplayAddress
        {
            get
            {
                string strAddress = "";
                uint uiPort = 0;

                if (m_byLength > 0 && null != m_abyDestination && m_abyDestination.Length > 0)
                {
                    if ((byte)AddressType.IP == m_byType)
                    {
                        //Add IP Address
                        strAddress = m_abyDestination[0].ToString(CultureInfo.InvariantCulture);

                        for (int i = 1; i < IP_ADDRESS_LENGTH; i++)
                        {
                            strAddress += "." + m_abyDestination[i].ToString(CultureInfo.InvariantCulture);
                        }

                        strAddress += ":";

                        //Add port 
                        uiPort = BitConverter.ToUInt16(m_abyDestination, IP_PORT_INDEX);

                        strAddress += uiPort.ToString(CultureInfo.InvariantCulture);

                    }
                    else
                    {
                        for (int i = 0; i < m_abyDestination.Length; i++)
                        {
                            strAddress += (char)strAddress[i];
                        }
                    }
                }

                return strAddress;
            }
        }

        /// <summary>
        /// Sets the record's destination address. Assumes the passed in byte array's length
        /// equals the length of the destination address.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public byte[] DestinationAddress
        {
            get
            {
                byte[] abyAddress = new byte[m_byLength];

                Array.Copy(m_abyDestination, abyAddress, m_byLength);

                return abyAddress;
            }
            set
            {
                if (null != value)
                {
                    m_byLength = (byte)value.Length;

                    //Make sure incoming byte array does not exceed maximum size.
                    if (m_byLength > DESTINATION_SIZE)
                    {
                        throw new ArgumentException("The specified destination byte array for the destination address record is too long!");
                    }

                    if (null == m_abyDestination)
                    {
                        m_abyDestination = new byte[DESTINATION_SIZE];
                    }

                    //Clear the contents of the existing destination address.
                    Array.Clear(m_abyDestination, 0, m_abyDestination.Length);

                    //Store the new destination address.
                    Array.Copy(value, m_abyDestination, m_byLength);
                }
                else
                {
                    m_byLength = 0;
                    m_abyDestination = null;
                }
            }            

        }

        /// <summary>
        /// Gets the record's destination address formatted as an IP address.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/21/13 jrf 2.85.24 WR 419581 Created.
        //
        public IPAddress AsIPAddress
        {
            get
            {
                string strAddress = "";
                IPAddress AsIPAddress = null;

                try
                {

                    if (m_byLength > 0 && null != m_abyDestination && m_abyDestination.Length > 0)
                    {
                        if ((byte)AddressType.IP == m_byType)
                        {
                            //Add IP Address
                            strAddress = m_abyDestination[0].ToString(CultureInfo.InvariantCulture);

                            for (int i = 1; i < IP_ADDRESS_LENGTH; i++)
                            {
                                strAddress += "." + m_abyDestination[i].ToString(CultureInfo.InvariantCulture);
                            }

                            AsIPAddress = IPAddress.Parse(strAddress);
                        }
                    }
                }
                catch
                {
                    AsIPAddress = null;
                }

                return AsIPAddress;
            }
        }

        #endregion

        #region Member Variables

        private byte m_byType;
        private byte m_byLength;
        private byte[] m_abyDestination;

        #endregion
    }

    /// <summary>
    /// ICM Mfg Table 2512 (Itron 464) - Module Configuration.
    /// MFG Table 2512 (Itron 464)
    /// </summary>
    public class ICMMfgTable2512ModuleConfiguration : AnsiTable
    {
        #region Constants

        private const uint MINIMUM_TABLE_SIZE = 927;
        private const uint IS_ERT_POPULATED_TABLE_SIZE = 928;
        private const uint ZIGBEE_ACCESS_TABLE_SIZE = 932;
        private const byte ALARM_MASK_LENGTH = 64;
        private const byte UTILITY_KEY_SEED_LENGTH = 10;
        private const byte DESTINATION_ADDRESS_RCD_LENGTH = 66;
        private const byte DST_CONFIGURATION_LENGTH = 6;
        private const byte ALARM_GENERIC_MASK_RCD_LENGTH = 65;
        private const byte TIME_SYNCH_CONFIGURATION_RCD_LENGTH = 11;
        private const byte DEVICE_STATE_RCD_LENGTH = 3;
        private const byte UNKNOWN = 0xFF;

        #endregion

        #region Definitions

        /// <summary>
        /// Enumeration that identifies configuration fields. Enumeration values
        /// are set to config field offsets.
        /// </summary>
        public enum ConfigFields: ushort
        {
            /// <summary>
            /// Socket Idle Timeout
            /// </summary>
            [EnumDescription("Socket Idle Timeout")]
            SocketIdleTimeOut = 0,
            /// <summary>
            /// Cellular Gateway
            /// </summary>
            [EnumDescription("Cellular Gateway")]
            GatewayAddress = 342,
            /// <summary>
            /// Cellular DNS
            /// </summary>
            [EnumDescription("Cellular DNS")]
            DNSAddress = 408,
            /// <summary>
            /// Cellular NTP
            /// </summary>
            [EnumDescription("Cellular NTP")]
            NTPAddress = 474,
            /// <summary>
            /// ICS Status Filter Length
            /// </summary>
            [EnumDescription("ICS Status Filter Length")]
            ICSStatusFilterLength = 546,
            /// <summary>
            /// ICS Status Filter
            /// </summary>
            [EnumDescription("ICS Status Filter")]
            ICSStatusFilter = 547,
            /// <summary>
            /// Cellular Power Outage Recognition Time
            /// </summary>
            [EnumDescription("Cellular Power Outage Recognition Time")]
            PowerFailTime = 873,
            /// <summary>
            /// NTP Update Frequency
            /// </summary>
            [EnumDescription("NTP Update Frequency")]
            NTPUpdateFrequency = 904,
            /// <summary>
            /// NTP Valid Time
            /// </summary>
            [EnumDescription("NTP Valid Time")]
            NTPValidTime = 905,
            /// <summary>
            /// Link Failures Threshold
            /// </summary>
            [EnumDescription("Link Failures Threshold")]
            LinkFailuresThreshold = 915,
            /// <summary>
            /// Tower Changes Threshold
            /// </summary>
            [EnumDescription("Tower Changes Threshold")]
            TowerChangesThreshold = 917,
            /// <summary>
            /// Sector ID Changes Threshold
            /// </summary>
            [EnumDescription("Sector ID Changes Threshold")]
            SectorIDChangesThreshold = 919,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">the PSEM object for the device</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  04/08/13 jrf 2.80.19 TQ 8278   Created
        //  07/18/13 jrf 2.80.54 WR 417794 Allowing table read to set its own size because 
        //                                 we are not sure what the table size will be and we 
        //                                 cannot count on firmware to indicate this.
        //
        public ICMMfgTable2512ModuleConfiguration(CPSEM psem)
            : base(psem, 2512, MINIMUM_TABLE_SIZE)
        {
            InitializeMembers();

            m_blnAllowAutomaticTableResizing = true;
        }
        
        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/08/13 jrf 2.80.19 TQ8278 Created
        //
        public ICMMfgTable2512ModuleConfiguration(PSEMBinaryReader reader)
            : base(2512, MINIMUM_TABLE_SIZE)
        {
            InitializeMembers();

            m_Reader = reader;

            ParseData();

            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Full read of 2512 (Mfg 464) out of the meter
        /// </summary>
        /// <returns>The PSEM response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "ICMMfgTable2512.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                ParseData();

                m_TableState = TableState.Loaded;
            }

            return Result;
        }

        /// <summary>
        /// Writes the specified field to the meter.
        /// </summary>
        /// <returns>The result of the write.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "OpenWayMFGTable2512.Write");

            // Resynch our members to the base's data array
            m_DataStream.Position = 0;

            m_Writer.Write(m_usSocketIdleTimeOut);
            m_Writer.Write(m_abyUtilityKeySeed);
            m_ProductionDestinationAddress.Write(m_Writer);
            m_AlarmDestinationAddress.Write(m_Writer);
            m_SchedulingDestinationAddress.Write(m_Writer);
            m_RegistrationDestinationAddress.Write(m_Writer);
            m_FirmwareDestinationAddress.Write(m_Writer);
            m_GatewayAddress.Write(m_Writer);
            m_DNSAddress.Write(m_Writer);
            m_NTPAddress.Write(m_Writer);
            m_Writer.Write(m_abyDSTConfiguration);
            m_Writer.Write(m_byICSStatusFilterLength);
            m_Writer.Write(m_abyICSStatusFilter);
            m_Writer.Write(m_abStandardANSIHistoryFilter);
            m_Writer.Write(m_abyStandardANSIEventFilter);
            m_Writer.Write(m_abyStandardANSIStatusFilter);
            m_Writer.Write(m_abyManufacturerANSIStatus);
            m_Writer.Write(m_usSpreadDelay);
            m_Writer.Write(m_uiPowerFailTime);
            m_Writer.Write(m_uiPowerRestoreTime);
            m_Writer.Write(m_usMonitoringPollingFrequency);
            m_Writer.Write(m_abyTimeSynchronizationConfiguration);
            m_Writer.Write(m_byEnableDisableTimeSync);
            m_Writer.Write(m_uiTTLTime);
            m_Writer.Write(m_BARTime);
            m_Writer.Write(m_abyDeviceState);
            m_Writer.Write(m_byNTPUpdateFrequency);
            m_Writer.Write(m_byNTPValidTime);
            m_Writer.Write(m_usPowerOutageTransmissionDelay);
            m_Writer.Write(m_usPowerRestorationTransmissionDelay);
            m_Writer.Write(m_usAlarmTransmissionDelay);
            m_Writer.Write(m_byDisplayMode);
            m_Writer.Write(m_bySocketBehavior);
            m_Writer.Write(m_byNetworkRetries);
            
            return base.Write();
        }

        /// <summary>
        /// Writes the specified field to the meter.
        /// </summary>
        /// <returns>The result of the write.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ 8278   Created
        //  08/21/13 jrf 2.85.24 WR 419581 Correcting error writing NTP address.
        //
        public PSEMResponse Write(ConfigFields Field)
        {
            ushort usFieldSize = 0;
            
            m_DataStream.Position = (ushort)Field;

            switch (Field)
            {
                case ConfigFields.SocketIdleTimeOut:
                {
                    m_Writer.Write(m_usSocketIdleTimeOut);
                    usFieldSize = sizeof(ushort);
                    break;
                }
                case ConfigFields.GatewayAddress:
                {
                    m_GatewayAddress.Write(m_Writer);
                    usFieldSize = DESTINATION_ADDRESS_RCD_LENGTH;
                    break;
                }
                case ConfigFields.DNSAddress:
                {
                    m_DNSAddress.Write(m_Writer);
                    usFieldSize = DESTINATION_ADDRESS_RCD_LENGTH;
                    break;
                }
                case ConfigFields.NTPAddress:
                {
                    m_NTPAddress.Write(m_Writer);
                    usFieldSize = DESTINATION_ADDRESS_RCD_LENGTH;
                    break;
                }
                case ConfigFields.ICSStatusFilterLength:
                {
                    m_Writer.Write(m_byICSStatusFilterLength);
                    usFieldSize = sizeof(byte);
                    break;
                }
                case ConfigFields.ICSStatusFilter:
                {
                    m_Writer.Write(m_abyICSStatusFilter);
                    usFieldSize = ALARM_MASK_LENGTH;
                    break;
                }
                case ConfigFields.PowerFailTime:
                {
                    m_Writer.Write(m_uiPowerFailTime);
                    usFieldSize = sizeof(UInt32);
                    break;
                }
                case ConfigFields.NTPUpdateFrequency:
                {
                    m_Writer.Write(m_byNTPUpdateFrequency);
                    usFieldSize = sizeof(byte);
                    break;
                }
                case ConfigFields.NTPValidTime:
                {
                    m_Writer.Write(m_byNTPValidTime);
                    usFieldSize = sizeof(byte);
                    break;
                }
                default:
                {
                    throw new ArgumentException("The config field is not supported", "Field");
                }
            }

            return base.Write((ushort)Field, usFieldSize);
        }

        /// <summary>
        /// Determines if the given alarm is set in the ICS status filter.
        /// </summary>
        /// <returns>Returns whether or not the given alarm is set.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 05/06/13 jrf 2.80.26 TQ8278  Created.
        //
        public bool IsAlarmSet(ICSStatusAlarms Alarm)
        {           
            int iAlarmsBytePosition = 0;            
            int iAlarmBitPosition = 0;            
            byte byBitMask = 1;
            bool blnAlarmSet = false;
            byte[] abyICSStatusFilter = ICSStatusFilter;

            //ICSStatusAlarm is big endian, largest alarm IDs byte first. 
            //Reversing so smallest alarm IDs are at front of array.
            Array.Reverse(abyICSStatusFilter);

            //This gives us which byte in array we want.
            iAlarmsBytePosition = (ushort)Alarm / 8;

            //This gives us which bit in selected byte represents the alarm.
            iAlarmBitPosition = (ushort)Alarm % 8;

            //This creates the mask for the bit of the alarm we want
            byBitMask <<= iAlarmBitPosition;

            //Determine if alarm is set.
            if ((byBitMask & abyICSStatusFilter[iAlarmsBytePosition]) == byBitMask)
            {
                blnAlarmSet = true;
            }

            return blnAlarmSet;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Socket Idle Timeout (in seconds).
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public ushort SocketIdleTimeout
        {
            get
            {
                ReadUnloadedTable();

                return m_usSocketIdleTimeOut;
            }
            set
            {
                m_usSocketIdleTimeOut = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Gateway Address formatted for display.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //  05/17/13 jrf 2.80.32 TQ8284 Making a partial read to reduce read time when just retrieving this value.
        //
        public string DisplayableGatewayAddress
        {
            get
            {
                PSEMResponse Result = base.Read((int)ConfigFields.GatewayAddress, DESTINATION_ADDRESS_RCD_LENGTH);

                if (PSEMResponse.Ok == Result)
                {
                    m_GatewayAddress.Parse(m_Reader);
                    return m_GatewayAddress.DisplayAddress;
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the Cellular Gateway"));
                }
            }
        }

        /// <summary>
        /// Gateway Address.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public DestinationAddressRecord GatewayAddress
        {
            set
            {
                m_GatewayAddress = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// DNS Address formatted for display.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public string DisplayableDNSAddress
        {
            get
            {
                ReadUnloadedTable();

                return m_DNSAddress.DisplayAddress;
            }
        }

        /// <summary>
        /// DNS Address
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public DestinationAddressRecord DNSAddress
        {
            set
            {
                m_DNSAddress = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// NTP Address formatted for display.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public string DisplayableNTPAddress
        {
            get
            {
                ReadUnloadedTable();

                return m_NTPAddress.DisplayAddress;
            }
        }

        /// <summary>
        /// NTP Address
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public DestinationAddressRecord NTPAddress
        {
            set
            {
                m_NTPAddress = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Sets the ICS status filter. Assumes the passed in byte array's length
        /// is set to the length of the ICSStatusFilter.
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 jrf 2.80.19 TQ8278 Created
        //
        public byte[] ICSStatusFilter
        {
            get
            {
                byte[] abyICSStatusFilter = new byte[m_byICSStatusFilterLength];

                Array.Copy(m_abyICSStatusFilter, abyICSStatusFilter, m_byICSStatusFilterLength);
                
                return abyICSStatusFilter;
            }
            set
            {
                m_byICSStatusFilterLength = (byte)value.Length;

                //Make sure incoming byte array does not exceed maximum size.
                if (m_byICSStatusFilterLength > ALARM_MASK_LENGTH)
                {
                    m_byICSStatusFilterLength = ALARM_MASK_LENGTH;
                }

                if (null == m_abyICSStatusFilter)
                {
                    m_abyICSStatusFilter = new byte[ALARM_MASK_LENGTH];
                }

                //Clear the contents of the existing destination address.
                Array.Clear(m_abyICSStatusFilter, 0, m_abyICSStatusFilter.Length);

                //Store the new destination address.
                Array.Copy(value, m_abyICSStatusFilter, m_byICSStatusFilterLength);
                
                State = TableState.Dirty;
            }

        }

        /// <summary>
        /// The power fail time or the minimum outage required before the ICS module 
        /// recognizes a power outage.
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 jrf 2.80.19 TQ8278 Created
        //
        public UInt32 PowerFailTime
        {
            get
            {
                ReadUnloadedTable();

                return m_uiPowerFailTime;
            }
            set
            {
                m_uiPowerFailTime = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// The NTP update frequency (in hours) is how often the ICS module asks the 
        /// SNTP server for the time.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 jrf 2.80.19 TQ8278 Created
        //
        public byte NTPUpdateFrequency
        {
            get
            {
                ReadUnloadedTable();

                return m_byNTPUpdateFrequency;
            }
            set
            {
                m_byNTPUpdateFrequency = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// The NTP valid time (in minutes) is how long the ICS time is valid after
        /// being recieved from the SNTP server.
        /// Supported by: OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 jrf 2.80.19 TQ8278 Created
        //  05/21/13 jrf 2.80.32 TQ8278 Corrected cut/paste error.
        //
        public byte NTPValidTime
        {
            get
            {
                ReadUnloadedTable();

                return m_byNTPValidTime;
            }
            set
            {
                m_byNTPValidTime = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Link Failures Threshold.
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8278 Created
        //
        public ushort LinkFailuresThreshold
        {
            get
            {
                ReadUnloadedTable();

                return m_usLinkFailuresThreshold;
            }
            set
            {
                m_usLinkFailuresThreshold = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Tower Changes Threshold.
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8278 Created
        //
        public ushort TowerChangesThreshold
        {
            get
            {
                ReadUnloadedTable();

                return m_usTowerChangesThreshold;
            }
            set
            {
                m_usTowerChangesThreshold = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Sector ID Changes Threshold.
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8278 Created
        //
        public ushort SectorIDChangesThreshold
        {
            get
            {
                ReadUnloadedTable();

                return m_usSectorIDChangesThreshold;
            }
            set
            {
                m_usSectorIDChangesThreshold = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Link Failures Counter Reset Frequency.
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/19/13 jrf 2.80.39 TQ8278 Created
        //
        public ushort LinkFailuresCounterResetFrequency
        {
            get
            {
                ReadUnloadedTable();

                return m_usLinkFailuresCounterResetFrequency;
            }
            set
            {
                m_usLinkFailuresCounterResetFrequency = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Tower Changes Counter Reset Frequency.
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/19/13 jrf 2.80.39 TQ8278 Created
        //
        public ushort TowerChangesCounterResetFrequency
        {
            get
            {
                ReadUnloadedTable();

                return m_usTowerChangesCounterResetFrequency;
            }
            set
            {
                m_usTowerChangesCounterResetFrequency = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Sector ID Changes Counter Reset Frequency.
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/19/13 jrf 2.80.39 TQ8278 Created
        //
        public ushort SectorIDChangesCounterResetFrequency
        {
            get
            {
                ReadUnloadedTable();

                return m_usSectorIDChangesCounterResetFrequency;
            }
            set
            {
                m_usSectorIDChangesCounterResetFrequency = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Indicates wheter or not ERT tables are populated.
        /// Supported by: I-210, kV2c, OW Centron
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  07/18/13 jrf 2.80.54 WR 417794 Created
        //
        public byte IsERTPopulated
        {
            get
            {
                ReadUnloadedTable();

                return m_byIsERTPopulated;
            }
            set
            {
                m_byIsERTPopulated = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Reads the maximum APDU size
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/30/14 AF  3.50.29 WR444483 Created
        //
        public ushort MaximumAPDUSize
        {
            get
            {
                ReadUnloadedTable();

                return m_usMaximumAPDUSize;
            }
            set
            {
                m_usMaximumAPDUSize = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Indicates if ZigBee is populated
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/30/14 AF  3.50.29 WR444483 Created
        //
        public byte IsZigBeePopulated
        {
            get
            {
                ReadUnloadedTable();

                return m_byIsZigBeePopulated;
            }
            set
            {
                m_byIsZigBeePopulated = value;
                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Reads the ZigBee access bit
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/30/14 AF  3.50.29 WR444483 Created
        //
        public byte ZigBeeAccess
        {
            get
            {
                ReadUnloadedTable();

                return m_byZigBeeAccess;
            }
            set
            {
                m_byZigBeeAccess = value;
                State = TableState.Dirty;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method initializes all member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/11/13 jrf 2.80.19 TQ8278 Created
        //  06/11/13 jrf 2.80.37 TQ8278 Added new fields.
        //
        private void InitializeMembers()
        {
            m_usSocketIdleTimeOut = 0;
            m_abyUtilityKeySeed = null;
            m_ProductionDestinationAddress = new DestinationAddressRecord();
            m_AlarmDestinationAddress = new DestinationAddressRecord();
            m_SchedulingDestinationAddress = new DestinationAddressRecord();
            m_RegistrationDestinationAddress = new DestinationAddressRecord();
            m_FirmwareDestinationAddress = new DestinationAddressRecord();
            m_GatewayAddress = new DestinationAddressRecord();
            m_DNSAddress = new DestinationAddressRecord();
            m_NTPAddress = new DestinationAddressRecord();
            m_abyDSTConfiguration = null;
            m_byICSStatusFilterLength = 0;
            m_abyICSStatusFilter = null;
            m_abStandardANSIHistoryFilter = null;
            m_abyStandardANSIEventFilter = null;
            m_abyStandardANSIStatusFilter = null;
            m_abyManufacturerANSIStatus = null;
            m_usSpreadDelay = 0;
            m_uiPowerFailTime = 0;
            m_uiPowerRestoreTime = 0;
            m_usMonitoringPollingFrequency = 0;
            m_abyTimeSynchronizationConfiguration = null;
            m_byEnableDisableTimeSync = 0;
            m_uiTTLTime = 0;
            m_BARTime = 0;
            m_abyDeviceState = null;
            m_byNTPUpdateFrequency = 0;
            m_byNTPValidTime = 0;
            m_usPowerOutageTransmissionDelay = 0;
            m_usPowerRestorationTransmissionDelay = 0;
            m_usAlarmTransmissionDelay = 0;
            m_byDisplayMode = 0;
            m_bySocketBehavior = 0;
            m_byNetworkRetries = 0;
            m_usLinkFailuresThreshold = 0;
            m_usTowerChangesThreshold = 0;
            m_usSectorIDChangesThreshold = 0;
            m_byIsERTPopulated = UNKNOWN;
            m_usMaximumAPDUSize = 0;
            m_byIsZigBeePopulated = 0;
            m_byZigBeeAccess = 0;
        }        

        /// <summary>
        /// Parses the data from the specified reader.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  04/08/13 jrf 2.80.19 TQ 8278   Created
        //  06/11/13 jrf 2.80.37 TQ 8278   Added new fields.
        //  06/19/13 jrf 2.80.42 TQ 8278   Added new fields.
        //  07/18/13 jrf 2.80.54 WR 417794 Handling uncertainty of table size.
        //
        private void ParseData()
        {
            if (m_Reader != null) 
            {
                if (MINIMUM_TABLE_SIZE > m_Reader.BaseStream.Length)
                {
                    throw new Exception("Table size is less than the minimum size expected. Table size = "
                        + m_Size.ToString(CultureInfo.InvariantCulture));
                }
                else //  MINIMUM_TABLE_SIZE <= m_Size
                {                    
                    m_usSocketIdleTimeOut = m_Reader.ReadUInt16();                                      // 2    2   0

                    m_abyUtilityKeySeed = m_Reader.ReadBytes(UTILITY_KEY_SEED_LENGTH);                  // 10  12   2
                    m_ProductionDestinationAddress.Parse(m_Reader);                                     // 66  78   12
                    m_AlarmDestinationAddress.Parse(m_Reader);                                          // 66  144  78
                    m_SchedulingDestinationAddress.Parse(m_Reader);                                     // 66  210  144
                    m_RegistrationDestinationAddress.Parse(m_Reader);                                   // 66  276  210
                    m_FirmwareDestinationAddress.Parse(m_Reader);                                       // 66  342  276

                    m_GatewayAddress.Parse(m_Reader);                                                   // 66  408  342
                    m_DNSAddress.Parse(m_Reader);                                                       // 66  474  408
                    m_NTPAddress.Parse(m_Reader);                                                       // 66  540  474

                    m_abyDSTConfiguration = m_Reader.ReadBytes(DST_CONFIGURATION_LENGTH);               // 6  546   540

                    m_byICSStatusFilterLength = m_Reader.ReadByte();                                    // 1  547   546
                    m_abyICSStatusFilter = m_Reader.ReadBytes(ALARM_MASK_LENGTH);                       // 64 611   547

                    m_abStandardANSIHistoryFilter = m_Reader.ReadBytes(ALARM_GENERIC_MASK_RCD_LENGTH);  // 65 676   611
                    m_abyStandardANSIEventFilter = m_Reader.ReadBytes(ALARM_GENERIC_MASK_RCD_LENGTH);   // 65 741   676
                    m_abyStandardANSIStatusFilter = m_Reader.ReadBytes(ALARM_GENERIC_MASK_RCD_LENGTH);  // 65 806   741
                    m_abyManufacturerANSIStatus = m_Reader.ReadBytes(ALARM_GENERIC_MASK_RCD_LENGTH);    // 65 871   806
                    m_usSpreadDelay = m_Reader.ReadUInt16();                                            // 2  873   871

                    m_uiPowerFailTime = m_Reader.ReadUInt32();                                          // 4  877   873      

                    m_uiPowerRestoreTime = m_Reader.ReadUInt32();
                    m_usMonitoringPollingFrequency = m_Reader.ReadUInt16();                             // 2  883   877
                    m_abyTimeSynchronizationConfiguration = m_Reader.ReadBytes(TIME_SYNCH_CONFIGURATION_RCD_LENGTH);    // 11 894   883
                    m_byEnableDisableTimeSync = m_Reader.ReadByte();                                    // 1  895   894
                    m_uiTTLTime = m_Reader.ReadUInt32();                                                // 4  899   895
                    m_BARTime = m_Reader.ReadUInt16();                                                  // 2  901   899
                    m_abyDeviceState = m_Reader.ReadBytes(DEVICE_STATE_RCD_LENGTH);                     // 3  904   901

                    m_byNTPUpdateFrequency = m_Reader.ReadByte();                                       // 1  905   904
                    m_byNTPValidTime = m_Reader.ReadByte();                                             // 1  906   905

                    m_usPowerOutageTransmissionDelay = m_Reader.ReadUInt16();                           // 2  908   906
                    m_usPowerRestorationTransmissionDelay = m_Reader.ReadUInt16();                      // 2  910   908
                    m_usAlarmTransmissionDelay = m_Reader.ReadUInt16();                                 // 2  912   910
                    m_byDisplayMode = m_Reader.ReadByte();                                              // 1  913   912
                    m_bySocketBehavior = m_Reader.ReadByte();                                           // 1  914   913
                    m_byNetworkRetries = m_Reader.ReadByte();                                           // 1  915   914
                    m_usLinkFailuresThreshold = m_Reader.ReadUInt16();                                  // 2  917   915
                    m_usTowerChangesThreshold = m_Reader.ReadUInt16();                                  // 2  919   917
                    m_usSectorIDChangesThreshold = m_Reader.ReadUInt16();                               // 2  921   919
                    m_usLinkFailuresCounterResetFrequency = m_Reader.ReadUInt16();                      // 2  923   921
                    m_usTowerChangesCounterResetFrequency = m_Reader.ReadUInt16();                      // 2  925   923
                    m_usSectorIDChangesCounterResetFrequency = m_Reader.ReadUInt16();                   // 2  927   925
                }

                if (IS_ERT_POPULATED_TABLE_SIZE <= m_Reader.BaseStream.Length)
                {
                    m_byIsERTPopulated = m_Reader.ReadByte();                                           // 1  928   927   
                }

                if (ZIGBEE_ACCESS_TABLE_SIZE <= m_Reader.BaseStream.Length)
                {
                    m_usMaximumAPDUSize = m_Reader.ReadUInt16();
                    m_byIsZigBeePopulated = m_Reader.ReadByte();
                    m_byZigBeeAccess = m_Reader.ReadByte();
                }
            }
        }

        /// <summary>
        /// Formats a byte array into a hex string
        /// </summary>
        /// <param name="data">The data to format</param>
        /// <returns>The hex string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 N/A    Created
        //
        private string FormatBytesHex(byte[] data)
        {
            string FormattedValue = "";

            for (int iIndex = 0; iIndex < data.Length; iIndex++)
            {
                FormattedValue += data[iIndex].ToString("X2", CultureInfo.InvariantCulture);
            }

            return FormattedValue;
        }

        #endregion

        #region Members

        private ushort m_usSocketIdleTimeOut;                              //I-210, kV2c
        private byte[] m_abyUtilityKeySeed;                                //I-210, kV2c
        private DestinationAddressRecord m_ProductionDestinationAddress;   //I-210, kV2c
        private DestinationAddressRecord m_AlarmDestinationAddress;        //I-210, kV2c
        private DestinationAddressRecord m_SchedulingDestinationAddress;   //I-210, kV2c
        private DestinationAddressRecord m_RegistrationDestinationAddress; //I-210, kV2c
        private DestinationAddressRecord m_FirmwareDestinationAddress;     //I-210, kV2c
        private DestinationAddressRecord m_GatewayAddress;                 //OW Centron
        private DestinationAddressRecord m_DNSAddress;                     //OW Centron, but not supported yet.
        private DestinationAddressRecord m_NTPAddress;                     //OW Centron
        private byte[] m_abyDSTConfiguration;                              //I-210, kV2c
        private byte m_byICSStatusFilterLength;                            //I-210, kV2c
        private byte[] m_abyICSStatusFilter;                               //I-210, kV2c
        private byte[] m_abStandardANSIHistoryFilter;                      //I-210, kV2c
        private byte[] m_abyStandardANSIEventFilter;                       //I-210, kV2c
        private byte[] m_abyStandardANSIStatusFilter;                      //I-210, kV2c
        private byte[] m_abyManufacturerANSIStatus;                        //I-210, kV2c
        private ushort m_usSpreadDelay;                                    //I-210, kV2c
        private UInt32 m_uiPowerFailTime;                                  //I-210, kV2c, OW Centron
        private UInt32 m_uiPowerRestoreTime;                               //I-210, kV2c
        private ushort m_usMonitoringPollingFrequency;                     //I-210, kV2c 
        private byte[] m_abyTimeSynchronizationConfiguration;              //I-210, kV2c
        private byte m_byEnableDisableTimeSync;                            //I-210, kV2c
        private UInt32 m_uiTTLTime;                                        //I-210, kV2c
        private ushort m_BARTime;                                          //I-210, kV2c
        private byte[] m_abyDeviceState;                                   //I-210, kV2c
        private byte m_byNTPUpdateFrequency;                               //OW Centron
        private byte m_byNTPValidTime;                                     //OW Centron
        private ushort m_usPowerOutageTransmissionDelay;                   //I-210, kV2c
        private ushort m_usPowerRestorationTransmissionDelay;              //I-210, kV2c
        private ushort m_usAlarmTransmissionDelay;                         //I-210, kV2c
        private byte m_byDisplayMode;                                      //I-210, kV2c
        private byte m_bySocketBehavior;                                   //I-210, kV2c
        private byte m_byNetworkRetries;                                   //I-210, kV2c
        private ushort m_usLinkFailuresThreshold;                          //I-210, kV2c, OW Centron
        private ushort m_usTowerChangesThreshold;                          //I-210, kV2c, OW Centron
        private ushort m_usSectorIDChangesThreshold;                       //I-210, kV2c, OW Centron
        private ushort m_usLinkFailuresCounterResetFrequency;              //I-210, kV2c, OW Centron
        private ushort m_usTowerChangesCounterResetFrequency;              //I-210, kV2c, OW Centron
        private ushort m_usSectorIDChangesCounterResetFrequency;           //I-210, kV2c, OW Centron
        private byte m_byIsERTPopulated;                                   //I-210 & kV2c(eventually), OW Centron
        private ushort m_usMaximumAPDUSize;                                //I-210, kV2c
        private byte m_byIsZigBeePopulated;                                //I-210, kV2c
        private byte m_byZigBeeAccess;                                     //I-210, kV2c

        #endregion
    }
    
    /// <summary>
    /// ICM Mfg Table Module Data.
    /// </summary>
    public class ICMMfgTable2515ModuleData : AnsiTable
    {
        #region Constants

        private const uint TABLE_LENGTH_2515 = 50;
        private const int ICM_FIRMWARE_MAJOR_VERSION_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">the PSEM object for the device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.07 n/a    Created
        //  12/10/13 AF  3.50.13 TQ9508 There were an extra 2 bytes added and then taken away in Superior
        //
        public ICMMfgTable2515ModuleData(CPSEM psem)
            : base(psem, 2515, TABLE_LENGTH_2515)
        {
            m_ExtendedFirmwareVersion = new byte[16];
            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/13 jkw 2.80.10 n/a    Created
        //
        public ICMMfgTable2515ModuleData(PSEMBinaryReader reader)
            : base(2515, TABLE_LENGTH_2515)
        {
            m_ExtendedFirmwareVersion = new byte[16];

            m_Reader = reader;

            ParseData();

            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Parses the data from the specified reader.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/13 jkw 2.80.10 N/A    Created
        //
        public void ParseData()
        {
            if (m_Reader != null)
            {
                m_ICMFirmwareVersionMajor = m_Reader.ReadByte();                                    // 1    1
                m_ICMFirmwareVersionRevision = m_Reader.ReadByte();                                 // 1    2
                m_ICMFirmwareVersionMinor = m_Reader.ReadByte();                                    // 1    3
                m_ExtendedFirmwareVersion = m_Reader.ReadBytes(m_ExtendedFirmwareVersion.Length);   // 16   19
                m_HardwareVersionMajor = m_Reader.ReadByte();                                       // 1    20
                m_HardwareVersionMinor = m_Reader.ReadByte();                                       // 1    21
                m_NumberOfSuperCapacitors = m_Reader.ReadByte();                                    // 1    22
                m_ICMSerialNumberMajor = m_Reader.ReadUInt32();                                     // 4    26
                m_ICMSerialNumberBuild = m_Reader.ReadUInt32();                                     // 4    30
                m_ICMSerialNumberMinor = m_Reader.ReadUInt32();                                     // 4    34
                m_ICMCPUIDHigh = m_Reader.ReadUInt32();                                             // 4    38
                m_ICMCPUIDLow = m_Reader.ReadUInt32();                                              // 4    42
                m_BootLoaderVersionMajor = m_Reader.ReadByte();                                     // 1    43
                m_BootLoaderVersionMinor = m_Reader.ReadByte();                                     // 1    44
                m_BootLoaderVersionRevision = m_Reader.ReadByte();                                  // 1    45
                m_LastPowerFailure =
                    m_Reader.ReadLTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME).ToLocalTime();       // 5    50
            }
        }

        /// <summary>
        /// Full read of 2515 (Mfg 467) out of the meter
        /// </summary>
        /// <returns>The PSEM response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.07 n/a    Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "ICMMfgTable2515.Read");


            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                ParseData();

                m_TableState = TableState.Loaded;
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// ICM Major part of the Firmware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMFirmwareVersionMajor
        {
            get
            {
                ReadUnloadedTable();

                return m_ICMFirmwareVersionMajor;
            }
        }

        /// <summary>
        /// Uncached ICM Major part of the Firmware Version. Will be used to verify communications
        /// with the comm module.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/14 jrf 3.50.68 WR458134 Created.
        public byte? UncachedICMFirmwareVersionMajor
        {
            get
            {
                PSEMResponse Result = base.Read(ICM_FIRMWARE_MAJOR_VERSION_OFFSET, 1);
                byte? Value = null;

                if (PSEMResponse.Ok == Result)
                {
                    Value = m_Reader.ReadByte();                    
                }

                return Value;
            }
        }

        /// <summary>
        /// ICM Minor part of the Firmware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMFirmwareVersionMinor
        {
            get
            {
                ReadUnloadedTable();

                return m_ICMFirmwareVersionMinor;
            }
        }

        /// <summary>
        /// ICM Firmware Version Revision
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte ICMFirmwareVersionRevision
        {
            get
            {
                ReadUnloadedTable();

                return m_ICMFirmwareVersionRevision;
            }
        }

        /// <summary>
        /// ICM Extended Firmware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string ICMExtendedFirmwareVersion
        {
            get
            {
                ReadUnloadedTable();

                return FormatBytesString(m_ExtendedFirmwareVersion);
            }
        }

        /// <summary>
        /// Hardware Version Major
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte HardwareVersionMajor
        {
            get
            {
                ReadUnloadedTable();

                return m_HardwareVersionMajor;
            }
        }

        /// <summary>
        /// Hardware Version Minor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte HardwareVersionMinor
        {
            get
            {
                ReadUnloadedTable();

                return m_HardwareVersionMinor;
            }
        }

        /// <summary>
        /// Number of Super Capacitors
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte NumberSuperCapacitors
        {
            get
            {
                ReadUnloadedTable();

                return m_NumberOfSuperCapacitors;
            }
        }


        /// <summary>
        /// ICM Module Major part of the Serial Number 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMSerialNumberMajor
        {
            get
            {
                ReadUnloadedTable();

                return m_ICMSerialNumberMajor;
            }
        }

        /// <summary>
        /// ICM Module Build part of the Serial Number 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMSerialNumberBuild
        {
            get
            {
                ReadUnloadedTable();

                return m_ICMSerialNumberBuild;
            }
        }

        /// <summary>
        /// ICM Module Minor part of the Serial Number 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMSerialNumberMinor
        {
            get
            {
                ReadUnloadedTable();

                return m_ICMSerialNumberMinor;
            }
        }

        /// <summary>
        /// ICM CPU Identifier High 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMCPUIDHigh
        {
            get
            {
                ReadUnloadedTable();

                return m_ICMCPUIDHigh;
            }
        }

        /// <summary>
        /// ICM CPU Identifier Low
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 ICMCPUIDLow
        {
            get
            {
                ReadUnloadedTable();

                return m_ICMCPUIDLow;
            }
        }

        /// <summary>
        /// Boot Loader Major part of the version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public byte BootLoaderVersionMajor
        {
            get
            {
                ReadUnloadedTable();

                return m_BootLoaderVersionMajor;
            }
        }

        /// <summary>
        /// Boot Loader Minor part of the version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public byte BootLoaderVersionMinor
        {
            get
            {
                ReadUnloadedTable();

                return m_BootLoaderVersionMinor;
            }
        }

        /// <summary>
        /// Boot Loader Revision part of the version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public byte BootLoaderVersionRevision
        {
            get
            {
                ReadUnloadedTable();

                return m_BootLoaderVersionRevision;
            }
        }

        /// <summary>
        /// Last Power Failure
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public DateTime LastPowerFailure
        {
            get
            {
                ReadUnloadedTable();

                return m_LastPowerFailure;
            }
        }

        #endregion

        #region private methods
        /// <summary>
        /// Formats a byte array into a hex string
        /// </summary>
        /// <param name="data">The data to format</param>
        /// <returns>The hex string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 N/A    Created
        //
        private string FormatBytesHex(byte[] data)
        {
            string FormattedValue = "";

            for (int iIndex = 0; iIndex < data.Length; iIndex++)
            {
                FormattedValue += data[iIndex].ToString("X2", CultureInfo.InvariantCulture);
            }

            return FormattedValue;
        }

        #endregion

        #region Members

        private byte m_ICMFirmwareVersionMajor;
        private byte m_ICMFirmwareVersionMinor;
        private byte m_ICMFirmwareVersionRevision;
        private byte[] m_ExtendedFirmwareVersion;
        private byte m_HardwareVersionMajor;
        private byte m_HardwareVersionMinor;
        private byte m_NumberOfSuperCapacitors;
        private UInt32 m_ICMSerialNumberMajor;
        private UInt32 m_ICMSerialNumberBuild;
        private UInt32 m_ICMSerialNumberMinor;
        private UInt32 m_ICMCPUIDHigh;
        private UInt32 m_ICMCPUIDLow;
        private byte m_BootLoaderVersionMajor;
        private byte m_BootLoaderVersionMinor;
        private byte m_BootLoaderVersionRevision;
        private DateTime m_LastPowerFailure;

        #endregion
    }

    /// <summary>
    /// ICM Mfg Table Module Status.
    /// </summary>
    public class ICMMfgTable2516ModuleStatus : AnsiTable
    {
        #region Constants

        private const uint TABLE_LENGTH_2516 = 12;

        // Length of the table for ITRU and ITRV
        private const uint TABLE_LENGTH_SUPERIOR_2516 = 22;

        #endregion

        #region Definitions

        // Needed only if we display the configuration status fields
        private enum ConfigurationStatusMask : uint
        {
            ICM_CONFIGURATION_CHANGED_MASK = 0x0000,
            REGISTER_END_DEVICE_PROGRAMMED_MASK = 0x0001,
            OPTICAL_ACCESS_TO_ICM_MASK = 0x0080,
            OPTICAL_ACCESS_WITH_TABLE_WRITE_TO_ICM_MASK = 0x0100,
            OPTICAL_ACCESS_TO_REGISTER_MASK = 0x0400,
            OPTICAL_ACCESS_WITH_TABLE_WRITE_TO_REGISTER = 0x0800,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">the PSEM object for the device</param>
        /// <param name="deviceClass">the device class for the meter we are logged onto</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.07 n/a    Created
        //  12/12/13 AF  3.50.14 TQ9508 Added device class parameter to determine table length
        //
        public ICMMfgTable2516ModuleStatus(CPSEM psem, string deviceClass)
            : base(psem, 2516, GetTableLength(deviceClass))
        {
            m_deviceClass = deviceClass;
            m_tableSize = GetTableLength(deviceClass);
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        /// <param name="deviceClass">the device class for the meter we are logged onto</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/13 jkw 2.80.10 n/a    Created
        //  12/12/13 AF  3.50.14 TQ9507 The size of this table now depends on device class.
        //                              ITRU/V have extra fields that ITRJ/H do not.
        //  11/30/15 AF  4.50.216 WR635872 Make sure this table is read every time. If we run the CLI 
        //                              command to set the state to configured, we want to be able to tell that it worked.
        //  01/19/16 AF  4.50.224 WR647295 Reverted last change. It broke the reading of EDL files.
        //
        public ICMMfgTable2516ModuleStatus(PSEMBinaryReader reader, string deviceClass)
            : base(2516, GetTableLength(deviceClass))
        {
            m_Reader = reader;
            m_deviceClass = deviceClass;
            m_tableSize = GetTableLength(deviceClass);

            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Parses the data from the specified reader.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/13 jkw 2.80.10 N/A    Created
        //  12/12/13 AF  3.50.15 TQ9508 Removed the read of the fields not supported in ITRJ.
        //                              The CE doesn't yet support them
        //
        public void ParseData()
        {
            if (m_Reader != null)
            {
                m_SuperCapcitorStatus = m_Reader.ReadByte();
                m_RebootCount = m_Reader.ReadUInt32();
                m_UpTime = m_Reader.ReadUInt32();
                m_ModuleStatus = m_Reader.ReadByte();
                m_ModuleTemerature = m_Reader.ReadInt16();
            }
        }

        /// <summary>
        /// Full read of 2516 (Mfg 468) out of the meter
        /// </summary>
        /// <returns>The PSEM response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.07 n/a    Created
        //  11/30/15 AF  4.50.216 WR635872 Took out the unnecessary setting of the table state. That will be
        //                              handled in the tables base class.
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "ICMMfgTable2516.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_SuperCapcitorStatus = m_Reader.ReadByte();
                m_RebootCount = m_Reader.ReadUInt32();
                m_UpTime = m_Reader.ReadUInt32();
                m_ModuleStatus = m_Reader.ReadByte();
                m_ModuleTemerature = m_Reader.ReadInt16();
                if (m_tableSize == TABLE_LENGTH_SUPERIOR_2516)
                {
                    m_ConfigurationStatusBfld = m_Reader.ReadUInt32();
                    m_MigrationState = m_Reader.ReadByte();
                    m_MigrationStateTimestamp = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Super Capacitor Status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public byte SuperCapacitorStatus
        {
            get
            {
                ReadUnloadedTable();

                return m_SuperCapcitorStatus;
            }
        }

        /// <summary>
        /// Reboot Count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 RebootCount
        {
            get
            {
                ReadUnloadedTable();

                return m_RebootCount;
            }
        }

        /// <summary>
        /// Uptime
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 Uptime
        {
            get
            {
                ReadUnloadedTable();

                return m_UpTime;
            }
        }

        /// <summary>
        /// Module Status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //  11/30/15 AF  4.50.216 WR635872 Make sure that the table is read every time. In the firmware tests, we might have
        //                              to switch the ICM to production mode and we need to be able to verify its status
        //  01/19/16 AF  4.50.224 WR647295 Reverted last change. It broke reading EDL files
        //
        public ICMModuleStatus ModuleStatus
        {
            get
            {
                ReadUnloadedTable();

                return (ICMModuleStatus)m_ModuleStatus;
            }
        }

        /// <summary>
        /// Module status - does a fresh read of the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/19/16 AF  4.50.224 WR 647295  Created for use by the firmware testers. Makes
        //                                   sure we get an updated read of the status
        //
        public ICMModuleStatus ModuleStatusUncached
        {
            get
            {
                Read();
                return (ICMModuleStatus)m_ModuleStatus;
            }
        }

        /// <summary>
        /// Module Temperature
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public Int16 ModuleTemperature
        {
            get
            {
                ReadUnloadedTable();

                return m_ModuleTemerature;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the length of the table
        /// </summary>
        /// <param name="deviceClass">the device class for the meter we are logged onto</param>
        /// <returns>length of the table in bytes</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.Compare(System.String,System.String,System.Boolean)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.Compare(System.String,System.String,System.Boolean)")]
        static private uint GetTableLength(string deviceClass)
        {
            uint uiTableLength;

            if ((String.Compare(deviceClass, "ITRU", true) == 0) || (String.Compare(deviceClass, "ITRV", true) == 0))
            {
                uiTableLength = TABLE_LENGTH_SUPERIOR_2516;
            }
            else
            {
                uiTableLength = TABLE_LENGTH_2516;
            }

            return uiTableLength;
        }

        #endregion

        #region Members

        private byte m_SuperCapcitorStatus;
        private UInt32 m_RebootCount;
        private UInt32 m_UpTime;
        private byte m_ModuleStatus;
        private Int16 m_ModuleTemerature;
        private uint m_ConfigurationStatusBfld;
        private byte m_MigrationState;
        private DateTime m_MigrationStateTimestamp;
        private string m_deviceClass;
        private uint m_tableSize;

        #endregion
    }

    /// <summary>
    /// Generic APN record. Specifies the APN, user name and password.
    /// </summary>
    public class GenericAPNRecord
    {
        #region Constants

        private const byte APN_SIZE = 32;
        private const byte USERNAME_SIZE = 32;
        private const byte PASSWORD_SIZE = 32;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public GenericAPNRecord()
        {
            m_byAPNLength = 0;
            m_byUserNameLength = 0;
            m_byPasswordLength = 0;
            m_abyAPN = null;
            m_abyUserName = null;
            m_abyPassword = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public GenericAPNRecord(string strAPN, string strUserName, string strPassword)
        {
            UserName = strUserName;
            APN = strAPN;
            Password = strPassword;
        }

        /// <summary>
        /// Parses the data from the specified binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data to parse.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public void Parse(PSEMBinaryReader Reader)
        {
            m_byAPNLength = Reader.ReadByte();
            m_byUserNameLength = Reader.ReadByte();
            m_byPasswordLength = Reader.ReadByte();
            m_abyAPN = Reader.ReadBytes(APN_SIZE);
            m_abyUserName = Reader.ReadBytes(USERNAME_SIZE);
            m_abyPassword = Reader.ReadBytes(PASSWORD_SIZE);
        }

        /// <summary>
        /// Writes the data to the specified binary writer.
        /// </summary>
        /// <param name="Writer">The binary reader that contains the data to parse.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public void Write(PSEMBinaryWriter Writer)
        {
            Writer.Write(m_byAPNLength);
            Writer.Write(m_byUserNameLength);
            Writer.Write(m_byPasswordLength);
            Writer.Write(m_abyAPN);
            Writer.Write(m_abyUserName);
            Writer.Write(m_abyPassword);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Sets the record's APN. Assumes the passed in byte array's length
        /// equals the length of the APN.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public string APN
        {
            get
            {
                string strAPN = "";

                if (null != m_abyAPN && m_byAPNLength <= m_abyAPN.Length)
                {
                    for (int i = 0; i < m_byAPNLength; i++)
                    {
                        strAPN += (char)m_abyAPN[i];
                    }
                }

                return strAPN;
            }
            set
            {
                m_byAPNLength = (byte)value.Length;

                //Make sure incoming byte array does not exceed maximum size.
                if (m_byAPNLength > APN_SIZE)
                {
                    m_byAPNLength = APN_SIZE;
                }

                if (null == m_abyAPN)
                {
                    m_abyAPN = new byte[APN_SIZE];
                }

                //Clear the contents of the existing APN.
                Array.Clear(m_abyAPN, 0, m_abyAPN.Length);

                //Store the new APN.
                char[] achAPN = value.ToCharArray();

                for (int i = 0; i < m_byAPNLength; i++)
                {
                    m_abyAPN[i] = (byte)achAPN[i];
                }
            }
        }

        /// <summary>
        /// Sets the record's user name. Assumes the passed in byte array's length
        /// equals the length of the user name.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public string UserName
        {
            get
            {
                string strUserName = "";

                if (null != m_abyUserName && m_byUserNameLength <= m_abyUserName.Length)
                {
                    for (int i = 0; i < m_byUserNameLength; i++)
                    {
                        strUserName += (char)m_abyUserName[i];
                    }
                }

                return strUserName;
            }
            set
            {
                m_byUserNameLength = (byte)value.Length;

                //Make sure incoming byte array does not exceed maximum size.
                if (m_byUserNameLength > USERNAME_SIZE)
                {
                    m_byUserNameLength = USERNAME_SIZE;
                }

                if (null == m_abyUserName)
                {
                    m_abyUserName = new byte[USERNAME_SIZE];
                }

                //Clear the contents of the existing APN.
                Array.Clear(m_abyUserName, 0, m_abyUserName.Length);

                //Store the new APN.
                char[] achUserName = value.ToCharArray();

                for (int i = 0; i < m_byUserNameLength; i++)
                {
                    m_abyUserName[i] = (byte)achUserName[i];
                }
            }
        }

        /// <summary>
        /// Sets the record's password. Assumes the passed in byte array's length
        /// equals the length of the password.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public string Password
        {
            get
            {
                string strPassword = "";

                if (null != m_abyPassword && m_byPasswordLength <= m_abyPassword.Length)
                {
                    for (int i = 0; i < m_byPasswordLength; i++)
                    {
                        strPassword += (char)m_abyPassword[i];
                    }
                }

                return strPassword;
            }
            set
            {
                m_byPasswordLength = (byte)value.Length;

                //Make sure incoming byte array does not exceed maximum size.
                if (m_byPasswordLength > PASSWORD_SIZE)
                {
                    m_byPasswordLength = PASSWORD_SIZE;
                }

                if (null == m_abyPassword)
                {
                    m_abyPassword = new byte[PASSWORD_SIZE];
                }

                //Clear the contents of the existing APN.
                Array.Clear(m_abyPassword, 0, m_abyPassword.Length);

                //Store the new APN.
                char[] achPassword = value.ToCharArray();

                for (int i = 0; i < m_byPasswordLength; i++)
                {
                    m_abyPassword[i] = (byte)achPassword[i];
                }
            }
        }

        #endregion

        #region Member Variables

        private byte m_byAPNLength;
        private byte m_byUserNameLength;
        private byte m_byPasswordLength;
        private byte[] m_abyAPN;
        private byte[] m_abyUserName;
        private byte[] m_abyPassword;

        #endregion
    }

    /// <summary>
    /// ICM Mfg Table 2517 (Itron 469) - Cellular Configuration.
    /// MFG Table 2517 (Itron 469)
    /// </summary>
    public class ICMMfgTable2517CellularConfiguration : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 268;
        private const uint TABLE_SIZE_SUPERIOR = 269;
        private byte PHONE_NUMBER_LENGTH = 64;
        private byte APN_RECORD_LENGTH = 99;

        #endregion

        #region Definitions

        /// <summary>
        /// The units used to designate what the cellular data timout value is stored in.
        /// </summary>
        public enum TimeoutUnits: byte
        {
            /// <summary>
            /// Hours
            /// </summary>
            [EnumDescription("Hours")]
            Hours = 0,
            /// <summary>
            /// Minutes
            /// </summary>
            [EnumDescription("Minutes")]
            Minutes = 1,
        }

        /// <summary>
        /// SMS configuration options.
        /// </summary>
        public enum SMSConfiguration
        {
            /// <summary>
            /// SMS configuration has not been set.
            /// </summary>
            [EnumDescription("Unknown")]
            Unset = 0,
            /// <summary>
            /// SMS is configured with Ping enabled.
            /// </summary>
            [EnumDescription("SMS Ping")]
            SMSPingEnabled = 1,
            /// <summary>
            /// SMS is configured with TMI enabled.
            /// </summary>
            [EnumDescription("SMS TMI")]
            SMSTMIEnabled = 2,
            /// <summary>
            /// SMS is configured with both Ping and TMI enabled.
            /// </summary>
            [EnumDescription("SMS Ping and TMI")]
            SMSPingAndTMIEnabled = 3,
        }
        
        /// <summary>
        /// Enumeration that identifies configuration fields. Enumeration values
        /// are set to config field offsets.
        /// </summary>
        public enum ConfigFields : ushort
        {
            /// <summary>
            /// Radio Phone Number
            /// </summary>
            [EnumDescription("Radio Phone Number")]
            RadioPhoneNumber = 0,
            /// <summary>
            /// GSM Primary APN
            /// </summary>
            [EnumDescription("GSM Primary APN")]
            GSMPrimaryAPN = 65,
            /// <summary>
            /// GSM Secondary APN
            /// </summary>
            [EnumDescription("GSM Secondary APN")]
            GSMSecondaryAPN = 164,
            /// <summary>
            /// Cellular Data Timeout Units
            /// </summary>
            [EnumDescription("Cellular Data Timeout Units")]
            CellularDataTimeoutUnits = 263,
            /// <summary>
            /// Cellular Data Timeout
            /// </summary>
            [EnumDescription("Cellular Data Timeout")]
            CellularDataTimeout = 264,
            /// <summary>
            /// SMS Only Mode
            /// </summary>
            [EnumDescription("SMS Only Mode")]
            SMSOnlyMode = 265,
            /// <summary>
            /// SMS Configuration
            /// </summary>
            [EnumDescription("SMS Configuration")]
            SMSConfiguration = 266,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">the PSEM object for the device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //  04/24/14 AF  3.50.83 WR428714 An extra byte was added midway through Superior
        //
        public ICMMfgTable2517CellularConfiguration(CPSEM psem)
            : base(psem, 2517, TABLE_SIZE)
        {
            InitializeMembers();

            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //  04/24/14 AF  3.50.83 WR428714 An extra byte was added midway through Superior
        //
        public ICMMfgTable2517CellularConfiguration(PSEMBinaryReader reader)
            : base(2517, TABLE_SIZE)
        {
            InitializeMembers();

            m_blnAllowAutomaticTableResizing = true;

            m_Reader = reader;

            ParseData();

            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Full read of 2517 (Mfg 469) out of the meter
        /// </summary>
        /// <returns>The PSEM response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "ICMMfgTable2517.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                ParseData();

                m_TableState = TableState.Loaded;
            }

            return Result;
        }

        /// <summary>
        /// Writes the subtable to the meter.
        /// </summary>
        /// <returns>The result of the write.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "OpenWayMFGTable2517.Write");

            // Resynch our members to the base's data array
            m_DataStream.Position = 0;

            m_Writer.Write(m_byRadioPhoneNumberLength);
            m_Writer.Write(m_abyRadioPhoneNumber);
            m_GSMPrimaryAPN.Write(m_Writer);
            m_GSMSecondaryAPN.Write(m_Writer);
            m_Writer.Write(m_byCellularDataTimeoutUnits);
            m_Writer.Write(m_byCellularDataTimeout);
            m_Writer.Write(m_bySMSOnlyMode);
            m_Writer.Write(m_usSMSConfiguration);

            return base.Write();
        }

        /// <summary>
        /// Writes the subtable to the meter.
        /// </summary>
        /// <returns>The result of the write.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/13 jrf 2.80.19 TQ8278 Created
        //
        public PSEMResponse Write(ConfigFields Field)
        {
            ushort usFieldSize = 0;

            m_DataStream.Position = (ushort)Field;

            switch (Field)
	        {
		        case ConfigFields.RadioPhoneNumber:
                {
                    m_Writer.Write(m_byRadioPhoneNumberLength);
                    m_Writer.Write(m_abyRadioPhoneNumber);
                    usFieldSize = (ushort)(sizeof(byte) + PHONE_NUMBER_LENGTH);
                    break;
                }
                case ConfigFields.GSMPrimaryAPN:
                {
                    m_GSMPrimaryAPN.Write(m_Writer);
                    usFieldSize = APN_RECORD_LENGTH;
                    break;
                }
                case ConfigFields.GSMSecondaryAPN:
                {
                    m_GSMSecondaryAPN.Write(m_Writer);
                    usFieldSize = APN_RECORD_LENGTH;
                    break;
                }
                case ConfigFields.CellularDataTimeoutUnits:
                {
                    m_Writer.Write(m_byCellularDataTimeoutUnits);
                    usFieldSize = sizeof(byte);
                    break;
                }
                case ConfigFields.CellularDataTimeout:
                {
                    m_Writer.Write(m_byCellularDataTimeout);
                    usFieldSize = sizeof(byte);
                    break;
                }
                case ConfigFields.SMSOnlyMode:
                {
                    m_Writer.Write(m_bySMSOnlyMode);
                    usFieldSize = sizeof(byte);
                    break;
                }
                case ConfigFields.SMSConfiguration:
                {
                    m_Writer.Write(m_usSMSConfiguration);
                    usFieldSize = sizeof(ushort);
                    break;
                }
                default:
                {
                    throw new ArgumentException("The config field is not supported", "Field");
                }
	        }
            
            return base.Write((ushort)Field, usFieldSize);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Cellular radio's phone number.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public byte[] RadioPhoneNumber
        {
            get
            {
                ReadUnloadedTable();

                byte[] abyPhoneNumber = new byte[m_byRadioPhoneNumberLength];

                if (null != m_abyRadioPhoneNumber)
                {
                    Array.Copy(m_abyRadioPhoneNumber, abyPhoneNumber, m_byRadioPhoneNumberLength);
                }

                return abyPhoneNumber;
            }
            set
            {
                m_byRadioPhoneNumberLength = (byte)value.Length;

                //Make sure incoming byte array does not exceed maximum size.
                if (m_byRadioPhoneNumberLength > PHONE_NUMBER_LENGTH)
                {
                    m_byRadioPhoneNumberLength = PHONE_NUMBER_LENGTH;
                }

                if (null == m_abyRadioPhoneNumber)
                {
                    m_abyRadioPhoneNumber = new byte[PHONE_NUMBER_LENGTH];
                }

                //Clear the contents of the existing radio phone number.
                Array.Clear(m_abyRadioPhoneNumber, 0, m_abyRadioPhoneNumber.Length);

                //Store the new destination address.
                Array.Copy(value, m_abyRadioPhoneNumber, m_byRadioPhoneNumberLength);

                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// The GSM's primary APN record.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public GenericAPNRecord GSMPrimaryAPN
        {
            get
            {
                ReadUnloadedTable();

                return m_GSMPrimaryAPN;
            }
            set
            {
                m_GSMPrimaryAPN = value;

                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// The GSM's secondary APN record.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public GenericAPNRecord GSMSecondaryAPN
        {
            get
            {
                ReadUnloadedTable();

                return m_GSMSecondaryAPN;
            }
            set
            {
                m_GSMSecondaryAPN = value;

                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// The units (hours or minutes) that the cellular data timeout is measured in.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public TimeoutUnits CellularDataTimeoutUnits
        {
            get
            {
                ReadUnloadedTable();

                return (TimeoutUnits)m_byCellularDataTimeoutUnits;
            }
            set
            {
                m_byCellularDataTimeoutUnits = (byte)value;

                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// The timeout value for cellular data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public byte CellularDataTimeout
        {
            get
            {
                ReadUnloadedTable();

                return m_byCellularDataTimeout;
            }
            set
            {
                m_byCellularDataTimeout = value;

                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// Determines if cellular configuration is set for SMS mode only.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public bool SMSOnlyMode
        {
            get
            {
                bool blnSMSOnlyMode = false;

                if (0 != m_bySMSOnlyMode)
                {
                    blnSMSOnlyMode = true;
                }
                
                return blnSMSOnlyMode;
            }
            set
            {
                if (false == value)
                {
                    m_bySMSOnlyMode = 0;
                }
                else //true
                {
                    m_bySMSOnlyMode = 1;
                }

                State = TableState.Dirty;
            }
        }

        /// <summary>
        /// How SMS is configured.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/12/13 jrf 2.80.19 TQ8278 Created
        //
        public SMSConfiguration SMSOperation
        {
            get
            {
                ReadUnloadedTable();

                return (SMSConfiguration)m_usSMSConfiguration;
            }
            set
            {
                m_usSMSConfiguration = (ushort)value;

                State = TableState.Dirty;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method initializes all member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/15/13 jrf 2.80.19 TQ8278 Created
        //
        private void InitializeMembers()
        {
            m_byRadioPhoneNumberLength = 0;
            m_abyRadioPhoneNumber = null;                               
            m_GSMPrimaryAPN = new GenericAPNRecord();
            m_GSMSecondaryAPN = new GenericAPNRecord();
            m_byCellularDataTimeoutUnits = 0;
            m_byCellularDataTimeout = 0;
            m_bySMSOnlyMode = 0;
            m_usSMSConfiguration = 0;
        }

        /// <summary>
        /// Parses the data from the specified reader.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/15/13 jrf 2.80.19 TQ8278 Created
        //  04/24/14 AF  3.50.83 WR428714 Added code to deal with the extra byte of data added to the table
        //
        private void ParseData()
        {
            if (m_Reader != null)
            {
                m_byRadioPhoneNumberLength = m_Reader.ReadByte();                                      // 1    1   0
                m_abyRadioPhoneNumber = m_Reader.ReadBytes(PHONE_NUMBER_LENGTH);                       //64   65   1
                m_GSMPrimaryAPN.Parse(m_Reader);                                                      // 99  164   65
                m_GSMSecondaryAPN.Parse(m_Reader);                                                    // 99  263  164
                m_byCellularDataTimeoutUnits = m_Reader.ReadByte();                                   //  1  264  263
                m_byCellularDataTimeout = m_Reader.ReadByte();                                        //  1  265  264
                m_bySMSOnlyMode = m_Reader.ReadByte();                                                //  1  266  265
                m_usSMSConfiguration = m_Reader.ReadUInt16();                                         //  2  268  266

                if (m_DataStream.Length >= TABLE_SIZE_SUPERIOR)
                {
                    m_byCENoCommResetCounterValue = m_Reader.ReadByte();                               //  1 269  267
                }
            }
        }

        #endregion

        #region Members

        private byte m_byRadioPhoneNumberLength;
        private byte[] m_abyRadioPhoneNumber;                      
        private GenericAPNRecord m_GSMPrimaryAPN;
        private GenericAPNRecord m_GSMSecondaryAPN;
        private byte m_byCellularDataTimeoutUnits;
        private byte m_byCellularDataTimeout;
        private byte m_bySMSOnlyMode;
        private ushort m_usSMSConfiguration;
        private byte m_byCENoCommResetCounterValue;

        #endregion
    }

    /// <summary>
    /// ICM Mfg Table Cellular Data.
    /// </summary>
    public class ICMMfgTable2518CellularData : AnsiTable
    {
        #region Constants

        private const uint TABLE_LENGTH_2518 = 307;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">the PSEM object for the device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.07 n/a    Created
        //
        public ICMMfgTable2518CellularData(CPSEM psem)
            : base(psem, 2518, TABLE_LENGTH_2518)
        {
            m_Manufacturer = new byte[64];
            m_Model = new byte[64];
            m_HardwareVersion = new byte[64];
            m_FirmwareVersion = new byte[64];
            m_IMEI_or_ESN_or_MEID = new byte[16];
            m_SIM_ICC_ID = new byte[20];
            m_IMSI_or_MIN = new byte[15];

            m_Manufacturer.Initialize();
            m_Model.Initialize();
            m_HardwareVersion.Initialize();
            m_FirmwareVersion.Initialize();
            m_IMEI_or_ESN_or_MEID.Initialize();
            m_SIM_ICC_ID.Initialize();
            m_IMSI_or_MIN.Initialize();
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/13 jkw 2.80.10 n/a    Created
        //
        public ICMMfgTable2518CellularData(PSEMBinaryReader reader)
            : base(2518, TABLE_LENGTH_2518)
        {
            m_Manufacturer = new byte[64];
            m_Model = new byte[64];
            m_HardwareVersion = new byte[64];
            m_FirmwareVersion = new byte[64];
            m_IMEI_or_ESN_or_MEID = new byte[16];
            m_SIM_ICC_ID = new byte[20];
            m_IMSI_or_MIN = new byte[15];

            m_Reader = reader;

            ParseData();

            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Parses the data from the specified reader.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/13 jkw 2.80.10 N/A    Created
        //
        public void ParseData()
        {
            if (m_Reader != null)
            {
                m_Manufacturer = m_Reader.ReadBytes(m_Manufacturer.Length);                 //64    64
                m_Model = m_Reader.ReadBytes(m_Model.Length);                               //64    128
                m_HardwareVersion = m_Reader.ReadBytes(m_HardwareVersion.Length);           //64    192
                m_FirmwareVersion = m_Reader.ReadBytes(m_FirmwareVersion.Length);           //64    256
                m_IMEI_or_ESN_or_MEID = m_Reader.ReadBytes(m_IMEI_or_ESN_or_MEID.Length);   //16    272 
                m_SIM_ICC_ID = m_Reader.ReadBytes(m_SIM_ICC_ID.Length);                     //20    292
                m_IMSI_or_MIN = m_Reader.ReadBytes(m_IMSI_or_MIN.Length);                   //15    307
            }
        }

        /// <summary>
        /// Full read of 2518 (Mfg 470) out of the meter
        /// </summary>
        /// <returns>The PSEM response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.07 n/a    Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "ICMMfgTable2518.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_Manufacturer = m_Reader.ReadBytes(m_Manufacturer.Length);               //64    64
                m_Model = m_Reader.ReadBytes(m_Model.Length);                             //64    128
                m_HardwareVersion = m_Reader.ReadBytes(m_HardwareVersion.Length);         //64    192
                m_FirmwareVersion = m_Reader.ReadBytes(m_FirmwareVersion.Length);         //64    256
                m_IMEI_or_ESN_or_MEID = m_Reader.ReadBytes(m_IMEI_or_ESN_or_MEID.Length); //16    272 
                m_SIM_ICC_ID = m_Reader.ReadBytes(m_SIM_ICC_ID.Length);                   //20    292
                m_IMSI_or_MIN = m_Reader.ReadBytes(m_IMSI_or_MIN.Length);                 //15    307

                m_TableState = TableState.Loaded;
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Manufacturer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string Manufacturer
        {
            get
            {
                ReadUnloadedTable();

                return FormatBytesString(m_Manufacturer);
            }
        }

        /// <summary>
        /// Model
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string Model
        {
            get
            {
                ReadUnloadedTable();

                return FormatBytesString(m_Model);
            }
        }

        /// <summary>
        /// Hardware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string HardwareVersion
        {
            get
            {
                ReadUnloadedTable();

                return FormatBytesString(m_HardwareVersion);
            }
        }

        /// <summary>
        /// Firmware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string FirmwareVersion
        {
            get
            {
                ReadUnloadedTable();

                return FormatBytesString(m_FirmwareVersion);
            }
        }


        /// <summary>
        /// IMEI (International Mobile Station Equipment Identity) OR
        /// ESN (Electronic Serial Number) OR
        /// MEID (Mobile Equipment IDentifier)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string IMEI_or_ESN_or_MEID
        {
            get
            {
                ReadUnloadedTable();

                return FormatBytesString(m_IMEI_or_ESN_or_MEID);
            }
        }

        /// <summary>
        /// Subscriber identity module (SIM) integrated circuit card (ICC) identifier (ID)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string SIMICCID
        {
            get
            {
                ReadUnloadedTable();

                return FormatBytesString(m_SIM_ICC_ID);
            }
        }

        /// <summary>
        /// International Mobile Subscriber Identity or IMSI  for Global System for Mobile Communications (GSM) OR
        /// Mobile identification number (MIN) for Code division multiple access (CDMA) 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public string IMSIforGSMorMINforCDMA
        {
            get
            {
                ReadUnloadedTable();

                return FormatBytesString(m_IMSI_or_MIN);
            }
        }

        #endregion

        #region private methods
        /// <summary>
        /// Formats a byte array into a hex string
        /// </summary>
        /// <param name="data">The data to format</param>
        /// <returns>The hex string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 N/A    Created
        //
        private string FormatBytesHex(byte[] data)
        {
            string FormattedValue = "";

            for (int iIndex = 0; iIndex < data.Length; iIndex++)
            {
                FormattedValue += data[iIndex].ToString("X2", CultureInfo.InvariantCulture);
            }

            return FormattedValue;
        }

        #endregion

        #region Members

        private byte[] m_Manufacturer;
        private byte[] m_Model;
        private byte[] m_HardwareVersion;
        private byte[] m_FirmwareVersion;
        private byte[] m_IMEI_or_ESN_or_MEID;   // International Mobile Station Equipment Identity (IMEI) >OR< ESN >OR< mobile equipment identifier (MEID)
        private byte[] m_SIM_ICC_ID;            // A subscriber identity module integrated circuit card identifier
        private byte[] m_IMSI_or_MIN;           // International Mobile Subscriber Identity or IMSI (GSM) or Code division multiple access (CDMA) Mobile identification number (MIN)

        #endregion
    }
    
    /// <summary>
    /// ICM Mfg Table Cellular Status.
    /// </summary>
    public class ICMMfgTable2519CellularStatus : AnsiTable
    {
        #region Constants

        private const uint MINIMUM_TABLE_SIZE = 139;
        private const uint TABLE_SIZE_WITH_CARRIER = 171;
        private const uint TABLE_LENGTH_2519 = 139;
        private const int SIGNAL_STRENGTH_OFFSET = 0;
        private const int SIGNAL_STRENGTH_BYTES = 1;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">the PSEM object for the device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.07 n/a    Created
        //
        public ICMMfgTable2519CellularStatus(CPSEM psem)
            : base(psem, 2519, MINIMUM_TABLE_SIZE)
        {
            m_NetworkMode = new byte[16];
            m_IPAddress = new byte[16];
            m_GatewayAddress = new byte[16];
            m_MDNRadioPhoneNumber = new byte[10];
            m_Carrier = new byte[32];

            m_NetworkMode.Initialize();
            m_IPAddress.Initialize();
            m_GatewayAddress.Initialize();
            m_MDNRadioPhoneNumber.Initialize();
            m_Carrier.Initialize();

            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/13 jkw 2.80.10 n/a    Created
        //
        public ICMMfgTable2519CellularStatus(PSEMBinaryReader reader)
            : base(2519, MINIMUM_TABLE_SIZE)
        {
            m_NetworkMode = new byte[16];
            m_IPAddress = new byte[16];
            m_GatewayAddress = new byte[16];
            m_MDNRadioPhoneNumber = new byte[10];
            m_Carrier = new byte[32];

            m_NetworkMode.Initialize();
            m_IPAddress.Initialize();
            m_GatewayAddress.Initialize();
            m_MDNRadioPhoneNumber.Initialize();
            m_Carrier.Initialize();

            m_Reader = reader;

            ParseData();

            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Parses the data from the specified reader.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/25/13 jkw 2.80.10 N/A    Created
        //
        public void ParseData()
        {
            if (m_Reader != null)
            {
                if (m_Reader.BaseStream.Length < MINIMUM_TABLE_SIZE)
                {
                    throw new Exception("Table size is less than the minimum size expected. Table size = "
                        + m_Size.ToString(CultureInfo.InvariantCulture));
                }
                else //  m_Size >= MINIMUM_TABLE_SIZE
                {
                    m_SignalStrength = m_Reader.ReadSByte();                              // 1
                    m_RegistrationStatus = m_Reader.ReadByte();                           // 1
                    m_NetworkMode = m_Reader.ReadBytes(m_NetworkMode.Length);             // 16
                    m_TowerIdentifier = m_Reader.ReadUInt16();                            // 4
                    m_SectorIdentifier = m_Reader.ReadUInt16();                           // 4
                    m_NumberOfCellTowerChanges = m_Reader.ReadUInt32();                   // 4
                    m_LinkConnectionState = m_Reader.ReadByte();                          // 1
                    m_NetworkConnectionUptime = m_Reader.ReadUInt32();                    // 4
                    m_IPAddress = m_Reader.ReadBytes(m_IPAddress.Length);                 // 16
                    m_GatewayAddress = m_Reader.ReadBytes(m_GatewayAddress.Length);       // 16
                    m_CumulativeKBytesSent = m_Reader.ReadUInt32();                       // 4
                    m_CumulativeKBytesReceived = m_Reader.ReadUInt32();                   // 4
                    m_BytesSent = m_Reader.ReadUInt32();                                  // 4
                    m_BytesReceived = m_Reader.ReadUInt32();                              // 4
                    m_PacketsSent = m_Reader.ReadUInt32();                                // 4
                    m_PacketsReceived = m_Reader.ReadUInt32();                            // 4
                    m_LastSuccessfulTowerCommunication =                                  // 5
                          m_Reader.ReadLTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME).ToLocalTime();
                    m_NumberOfLinkFailures = m_Reader.ReadUInt32();                       // 4
                    m_ModemTemperature = m_Reader.ReadInt16();                            // 2
                    m_LastModemShutdownForTemperature =                                   // 5
                          m_Reader.ReadLTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME).ToLocalTime();
                    m_LastModemPowerUpAfterTemperatureShutdown =                          // 5
                          m_Reader.ReadLTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME).ToLocalTime();
                    m_MDNRadioPhoneNumber =                                               // 10
                          m_Reader.ReadBytes(m_MDNRadioPhoneNumber.Length);
                    m_NumberOfSectorIdentifierChanges = m_Reader.ReadUInt32();            // 4  
                    m_TrafficChannelsGoodCRCCount = m_Reader.ReadUInt32();                // 4
                    m_TrafficChannelsBadCRCCount = m_Reader.ReadUInt32();                 // 4
                    m_ControlChannelsGoodCRCCount = m_Reader.ReadUInt32();                // 4
                    m_ControlChannelsBadCRCCount = m_Reader.ReadUInt32();                 // 4
                    m_FigureOfMerit = m_Reader.ReadByte();                                // 1 145

                    if (m_Reader.BaseStream.Length >= TABLE_SIZE_WITH_CARRIER)
                    {
                        m_Carrier = m_Reader.ReadBytes(m_Carrier.Length);                 // 32
                    }
                }
            }
        }

        /// <summary>
        /// Full read of 2519 (Mfg 471) out of the meter
        /// </summary>
        /// <returns>The PSEM response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.07 n/a    Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "ICMMfgTable2519.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();

                m_TableState = TableState.Loaded;

            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Reads signal strength
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a      Created
        //  05/25/17 CFB 4.72.00 WR762403 Changed to read Signal Strength each times its value its requested
        public sbyte SignalStrength
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                Result = base.Read(SIGNAL_STRENGTH_OFFSET, SIGNAL_STRENGTH_BYTES);

                if (Result == PSEMResponse.Ok)
                {
                    ParseData();
                }

                return m_SignalStrength;
            }
        }

        /// <summary>
        /// Reads registration status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public RegistrationStatus RegistrationStatus
        {
            get
            {
                ReadUnloadedTable();

                return (RegistrationStatus)m_RegistrationStatus;
            }
        }

        /// <summary>
        /// Reads Network Mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string NetworkMode
        {
            get
            {
                ReadUnloadedTable();

                return FormatBytesString(m_NetworkMode);
            }
        }

        /// <summary>
        /// Reads Network Mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/12/13 jkw 2.85.16 n/a    Created
        //
        public string Carrier
        {
            get
            {
                ReadUnloadedTable();

                return FormatBytesString(m_Carrier);
            }
        }

        /// <summary>
        /// Reads the IP Address
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string IPAddress
        {
            get
            {
                ReadUnloadedTable();

                return ConvertIPAddressByteArrayToString(m_IPAddress, this.TableID);
            }
        }

        /// <summary>
        /// Reads the Gateway Address
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //
        public string GatewayAddress
        {
            get
            {
                                ReadUnloadedTable();

                return ConvertIPAddressByteArrayToString(m_GatewayAddress, this.TableID);
            }
        }

        /// <summary>
        /// Reads the Tower Identifier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt16 TowerIdentifier
        {
            get
            {
                ReadUnloadedTable();

                return m_TowerIdentifier;
            }
        }

        /// <summary>
        /// Reads the Sector Identifier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt16 SectorIdentifier
        {
            get
            {
                ReadUnloadedTable();

                return m_SectorIdentifier;
            }
        }

        /// <summary>
        /// Reads the Number of Cell Tower Changes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 NumberOfCellTowerChanges
        {
            get
            {
                ReadUnloadedTable();

                return m_NumberOfCellTowerChanges;
            }
        }

        /// <summary>
        /// Reads the Link Connection State
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public LinkConnectionState LinkConnectionState
        {
            get
            {
                ReadUnloadedTable();

                return (LinkConnectionState)m_LinkConnectionState;
            }
        }

        /// <summary>
        /// Reads the Network Connection Up Time (seconds)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 NetworkConnectionUptime
        {
            get
            {
                ReadUnloadedTable();

                return m_NetworkConnectionUptime;
            }
        }

        /// <summary>
        /// Reads the Cumulative KiloBytes Sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 CumulativeKBytesSent
        {
            get
            {
                ReadUnloadedTable();

                return m_CumulativeKBytesSent;
            }
        }

        /// <summary>
        /// Reads the Cumulative KiloBytes Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 CumulativeKBytesReceived
        {
            get
            {
                ReadUnloadedTable();

                return m_CumulativeKBytesReceived;
            }
        }

        /// <summary>
        /// Reads the Bytes Sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 BytesSent
        {
            get
            {
                ReadUnloadedTable();

                return m_BytesSent;
            }
        }

        /// <summary>
        /// Reads the Bytes Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 BytesReceived
        {
            get
            {
                ReadUnloadedTable();

                return m_BytesReceived;
            }
        }

        /// <summary>
        /// Reads the Packets Delivered
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 PacketsSent
        {
            get
            {
                ReadUnloadedTable();

                return m_PacketsSent;
            }
        }

        /// <summary>
        /// Reads the Packets Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 PacketsReceived
        {
            get
            {
                ReadUnloadedTable();

                return m_PacketsReceived;
            }
        }

        /// <summary>
        /// Reads the Last Successful Tower Communication
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public DateTime LastSuccessfulTowerCommunication
        {
            get
            {
                ReadUnloadedTable();

                return m_LastSuccessfulTowerCommunication;
            }
        }

        /// <summary>
        /// Reads the Number Of Link Failures
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public UInt32 NumberOfLinkFailures
        {
            get
            {
                ReadUnloadedTable();

                return m_NumberOfLinkFailures;
            }
        }

        /// <summary>
        /// Reads the Modem Temperature
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public Int16 ModemTemperature
        {
            get
            {
                ReadUnloadedTable();

                return m_ModemTemperature;
            }
        }

        /// <summary>
        /// Reads the Last Modem Shutdown For Temperature
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public DateTime LastModemShutdownForTemperature
        {
            get
            {
                ReadUnloadedTable();

                return m_LastModemShutdownForTemperature;
            }
        }

        /// <summary>
        /// Reads the Last Modem Power Up After Temperature Shutdown
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public DateTime LastModemPowerUpAfterTemperatureShutdown
        {
            get
            {
                ReadUnloadedTable();

                return m_LastModemPowerUpAfterTemperatureShutdown;
            }
        }

        /// <summary>
        /// Reads the MDM Radio Phone Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/13/13 jkw 2.80.08 n/a    Created
        //
        public string MDNRadioPhoneNumber
        {
            get
            {
                ReadUnloadedTable();

                return FormatBytesString(m_MDNRadioPhoneNumber);
            }
        }

        /// <summary>
        /// Reads the Number Of Sector Identifier Changes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/65 jkw 2.80.35 n/a    Created
        //
        public UInt32 NumberOfSectorIdentifierChanges
        {
            get
            {
                ReadUnloadedTable();

                return m_NumberOfSectorIdentifierChanges;
            }
        }

        /// <summary>
        /// Reads the Traffic Channels - good CRC count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/65 jkw 2.80.35 n/a    Created
        //
        public UInt32 TrafficChannelsGoodCRCCount
        {
            get
            {
                ReadUnloadedTable();

                return m_TrafficChannelsGoodCRCCount;
            }
        }

        /// <summary>
        /// Reads the Traffic Channels - bad CRC count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/65 jkw 2.80.35 n/a    Created
        //
        public UInt32 TrafficChannelsBadCRCCount
        {
            get
            {
                ReadUnloadedTable();

                return m_TrafficChannelsBadCRCCount;
            }
        }

        /// <summary>
        /// Reads the Control Channels - good CRC count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/65 jkw 2.80.35 n/a    Created
        //
        public UInt32 ControlChannelsGoodCRCCount
        {
            get
            {
                ReadUnloadedTable();

                return m_ControlChannelsGoodCRCCount;
            }
        }

        /// <summary>
        /// Reads the Control Channels - bad CRC count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/65 jkw 2.80.35 n/a    Created
        //
        public UInt32 ControlChannelsBadCRCCount
        {
            get
            {
                ReadUnloadedTable();

                return m_ControlChannelsBadCRCCount;
            }
        }

        /// <summary>
        /// Reads the Figure of Merit (% good CRC count)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/65 jkw 2.80.35 n/a    Created
        //
        public byte FigureOfMerit
        {
            get
            {
                ReadUnloadedTable();

                return m_FigureOfMerit;
            }
        }

        /// <summary>
        /// Reads the name of the cellular carrier.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/13/13 jrf 2.85.44 WR423526 Created.
        //
        public string CellularCarrier
        {
            get
            {
                ReadUnloadedTable();

                return Encoding.Default.GetString(m_Carrier).Replace("\0", "");
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// Converts the raw IP address in byte[] format to standard string 
        /// representation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/15/13 jkw 2.80.08 n/a    Created
        //  08/15/13 AF  2.85.19 WR412809 Added code to deal with an IP address that is all 0's.
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
        private string ConvertIPAddressByteArrayToString(byte[] IP, ushort tableNumber)
        {
            try
            {
                string returnValue = string.Empty;

                if (IP != null)
                {
                    for (int ndx = 0; ndx < IP.Length; ndx++)
                    {
                        if (IP[ndx] != 0)
                        {
                            returnValue += (char)IP[ndx];
                        }
                    }
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                string strRAW = "null";

                if (IP != null)
                {
                    strRAW = IP.ToHexString();
                }

                throw new Exception(string.Format("Exception caught trying to parse IP Address [{0}] in manufacturing table [{1}]. Exception text [{2}]", strRAW, this.TableID, ex.Message));
            }
        }

        /// <summary>
        /// Formats a byte array into a hex string
        /// </summary>
        /// <param name="data">The data to format</param>
        /// <returns>The hex string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 N/A    Created
        //
        private string FormatBytesHex(byte[] data)
        {
            string FormattedValue = "";

            for (int iIndex = 0; iIndex < data.Length; iIndex++)
            {
                FormattedValue += data[iIndex].ToString("X2", CultureInfo.InvariantCulture);
            }

            return FormattedValue;
        }

        #endregion

        #region Members

        private sbyte m_SignalStrength;
        private byte m_RegistrationStatus;
        private byte[] m_NetworkMode;
        private UInt16 m_TowerIdentifier;
        private UInt16 m_SectorIdentifier;
        private UInt32 m_NumberOfCellTowerChanges;
        private byte m_LinkConnectionState;
        private UInt32 m_NetworkConnectionUptime;
        private byte[] m_IPAddress;
        private byte[] m_GatewayAddress;
        private UInt32 m_CumulativeKBytesSent;
        private UInt32 m_CumulativeKBytesReceived;
        private UInt32 m_BytesSent;
        private UInt32 m_BytesReceived;
        private UInt32 m_PacketsSent;
        private UInt32 m_PacketsReceived;
        private DateTime m_LastSuccessfulTowerCommunication;
        private UInt32 m_NumberOfLinkFailures;
        private Int16 m_ModemTemperature;
        private DateTime m_LastModemShutdownForTemperature;
        private DateTime m_LastModemPowerUpAfterTemperatureShutdown;
        private byte[] m_MDNRadioPhoneNumber;
        private UInt32 m_NumberOfSectorIdentifierChanges;
        private UInt32 m_TrafficChannelsGoodCRCCount;
        private UInt32 m_TrafficChannelsBadCRCCount;
        private UInt32 m_ControlChannelsGoodCRCCount;
        private UInt32 m_ControlChannelsBadCRCCount;
        private byte m_FigureOfMerit;
        private byte[] m_Carrier;
        
        #endregion
    }

    /// <summary>
    /// ICS Mfg ACT Table Event Log Control
    /// </summary>
    public class ICSMfgTable2521 : AnsiTable
    {
        #region Constants

        private const int TABLE_2521_SIZE = 6;
        private const byte EVENT_NUMBER_FLAG_MASK = 0x01;
        private const byte HIST_DATE_TIME_FLAG_MASK = 0x02;
        private const byte HIST_SEQ_NBR_FLAG_MASK = 0x04;

        private const int LTIME_LENGTH = 5;
        private const int UINT16_LENGTH = 2;
        private const int UINT8_LENGTH = 1;
        private const int IDB_BFLD_LENGTH = 2;
        private const int EC_BFLD_LENGTH = 2;

	    #endregion

	    #region Public Methods

        /// <summary>
        /// Table 2521 - Actual Log Dimensions Constructor
        /// </summary>
        /// <remarks>Very similar to STD table 71</remarks>
        /// <param name="psem">PSEM object for this current session</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/16/13 MSC 2.80.32 N/A    Created
        // 04/04/14 jrf 3.50.61 461982 Removed passing in unneeded std version.
        public ICSMfgTable2521(CPSEM psem)
            : base(psem, 2521, TABLE_2521_SIZE)
        {
            m_PSEM = psem;
        }

        /// <summary>
        /// Constructor that uses that data stored in a Binary Reader
        /// </summary>
        /// <param name="reader">The binary reader that contains the table data</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/16/13 MSC 2.80.32 N/A    Created
        // 04/04/14 jrf 3.50.61 461982 Removed passing in unneeded std version.
        public ICSMfgTable2521(PSEMBinaryReader reader)
            : base(2521, TABLE_2521_SIZE)
        {
            m_Reader = reader;
            ParseData();
            State = TableState.Loaded;
        }

        /// <summary>
        /// Reads table ICM Mfg 2521 out of the meter.
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        // 05/16/13 MSC 2.80.32 N/A    Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "ICMMfgTable2521.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }


            return Result;
        }

        /// <summary>
        /// Provide a way to have us reread the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/23/13 AF  2.85.26 WR419631 Created
        //
        public void Refresh()
        {
            m_TableState = TableState.Expired;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get the data out of the binary reader and into the member variables.
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        // 05/16/13 MSC 2.80.32 N/A    Created
        //
        private void ParseData()
        {
            //Populate the member variable that represent the table
            m_byLogFlagBfld = m_Reader.ReadByte();
            m_byNbrStdEvents = m_Reader.ReadByte();
            m_byNbrMfgEvents = m_Reader.ReadByte();
            m_byICSDataLength = m_Reader.ReadByte();
            m_usNbrICSEntries = m_Reader.ReadUInt16();
        }

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Exposes the History Date Time Flag
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        // 05/16/13 MSC 2.80.32 N/A    Created
        // 06/21/13 AF  2.80.40 TR7586 Used ReadUnloadedTable() instead of longer code
        //
        public bool HistoryDateTimeFlag
        {
            get
            {
                bool bHistDateTimeFlag = false;

                ReadUnloadedTable();
                
                if (0 != (byte)(m_byLogFlagBfld & HIST_DATE_TIME_FLAG_MASK))
                {
                    bHistDateTimeFlag = true;
                }

                return bHistDateTimeFlag;
            }
        }

        /// <summary>
        /// Exposes the Event Number Flag
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        // 05/16/13 MSC 2.80.32 N/A    Created
        // 06/21/13 AF  2.80.40 TR7586 Added ReadUnloadedTable()
        //
        public bool EventNumberFlag
        {
            get
            {
                bool bEventNumberFlag = false;

                ReadUnloadedTable();

                if (0 != (byte)(m_byLogFlagBfld & EVENT_NUMBER_FLAG_MASK))
                {
                    bEventNumberFlag = true;
                }
                return bEventNumberFlag;
            }
        }

        /// <summary>
        /// Exposes the History Sequence Number Flag
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        // 05/16/13 MSC 2.80.32 N/A    Created
        // 06/21/13 AF  2.80.40 TR7586 Added ReadUnloadedTable()
        //
        public bool HistorySequenceNumberFlag
        {
            get
            {
                bool bHistSeqNbrFlag = false;

                ReadUnloadedTable();

                if (0 != (byte)(m_byLogFlagBfld & HIST_SEQ_NBR_FLAG_MASK))
                {
                    bHistSeqNbrFlag = true;
                }
                return bHistSeqNbrFlag;
            }
        }

        /// <summary>
        /// Exposes the ICS Data Length
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        // 05/16/13 MSC 2.80.32 N/A    Created
        // 06/21/13 AF  2.80.40 TR7586 Added ReadUnloadedTable()
        //
        public byte ICSDataLength
        {
            get
            {
                ReadUnloadedTable();

                return m_byICSDataLength;
            }
        }

        /// <summary>
        /// Exposes the Number of ICS Entries
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        // 05/16/13 MSC 2.80.32 N/A     Created
        // 06/21/13 AF  2.80.40 TR7586  Added ReadUnloadedTable() and corrected size of return type
        // 07/24/13 AF  2.85.02 WR416508 Changed the ReadUnloadedTable to Read to make sure the table
        //                               gets reread each time we ask
        // 08/23/13 AF  2.85.26 WR419631 Changed back to ReadUnloadedTable because Read called the wrong
        //                               code when reading an EDL file. Now use Refresh() after the freeze log procedure
        //
        public UInt16 NumberICSEntries
        {
            get
            {
                ReadUnloadedTable();

                return m_usNbrICSEntries;
            }
        }

        /// <summary>
        /// Property that will return the size of 74 based off the values in 71
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        // 05/16/13 MSC 2.80.32 N/A    Created
        // 06/21/13 AF  2.80.40 TR7586 Simplified the logic to avoid repeated code
        //
        public uint SizeOfTable2524
        {
            get
            {
                // Start size with the constant data: Hist Flags, Nbr valid entries
                //	Last Entry Element, Last Entry Seq Number and # unread entries
                int iTable2524Size = 11;

                //	Calculate the table size.
                iTable2524Size += (int)(SizeOfEventEntry * NumberICSEntries);

                return (uint) iTable2524Size;
            }
        }

        /// <summary>
        /// Property that will return the size of 76 based off the values in 71
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        // 05/16/13 MSC 2.80.32 N/A    Created
        // 06/21/13 AF  2.80.40 TR7586 Corrected the calculation
        //
        public uint SizeOfEventEntry
        {
            get
            {
                uint uiEntryLength = 0;

                // Now we need to calculate an Entry Length
                // Event time
                uiEntryLength += LTIME_LENGTH;
                if (EventNumberFlag)
                {
                    // Event number
                    uiEntryLength += UINT16_LENGTH;
                }
                // History sequence number
                uiEntryLength += UINT16_LENGTH;
                // User id
                uiEntryLength += UINT16_LENGTH;
                // History code
                uiEntryLength += UINT16_LENGTH;
                // Finally add in the History Arguments
                uiEntryLength += ICSDataLength;

                return (uint)uiEntryLength;
            }
        }

        /// <summary>
        /// The number of bytes needed for a bitfield of std events supported
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        // 05/16/13 MSC 2.80.32 N/A    Created
        //
        public uint NumberStandardEvents
        {
            get
            {
                ReadUnloadedTable();

                return m_byNbrStdEvents;
            }
        }

        /// <summary>
        /// The number of bytes needed for a bitfield of mfg events supported
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        // 05/16/13 MSC 2.80.32 N/A    Created
        //
        public uint NumberManufacturerEvents
        {
            get
            {
                ReadUnloadedTable();

                return m_byNbrMfgEvents;
            }
        }

        #endregion

	    #region Members

        private byte m_byLogFlagBfld;
        private byte m_byNbrStdEvents;
        private byte m_byNbrMfgEvents;
        private byte m_byICSDataLength;
        private UInt16 m_usNbrICSEntries;

	    #endregion
    }

    /// <summary>
    /// ICS Mfg Table Events ID
    /// </summary>
    public class ICSMfgTable2522 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">the PSEM object for the device</param>
        /// <param name="table2521">MFG ICM Table 2521</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //
        public ICSMfgTable2522(CPSEM psem, ICSMfgTable2521 table2521)
            : base(psem, 2522, GetTableLength(table2521), TABLE_TIMEOUT)
        {
            m_Table2521 = table2521;
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        /// <param name="table2521">MFG ICM Table 2521</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //
        public ICSMfgTable2522(PSEMBinaryReader reader, ICSMfgTable2521 table2521)
            : base(2522, GetTableLength(table2521))
        {
            m_Table2521 = table2521;
            m_Reader = reader;

            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Full read of 2522 (Mfg 474) out of the meter
        /// </summary>
        /// <returns>The PSEM response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "ICMMfgTable2522.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Each bit position in the byte array returned represents a std event.
        /// If the event is supported, the bit value is 1; otherwise, 0.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //
        public byte[] StdEventsSupported
        {
            get
            {
                ReadUnloadedTable();

                return m_abyStdEventsSupported;
            }
        }

        /// <summary>
        /// Each bit position in the byte array returned represents an mfg event.
        /// If the event is supported, the bit value is 1; otherwise, 0.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //
        public byte[] MfgEventsSupported
        {
            get
            {
                ReadUnloadedTable();

                return m_abyMfgEventsSupported;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper method to determine the length of the table.
        /// </summary>
        /// <param name="table2521">MFG ICM table 2521</param>
        /// <returns>length in bytes of table 2522</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //
        static private uint GetTableLength(ICSMfgTable2521 table2521)
        {
            return (table2521.NumberStandardEvents + table2521.NumberManufacturerEvents);
        }

        /// <summary>
        /// Parses the data from the specified reader.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //
        private void ParseData()
        {
            if (m_Reader != null)
            {
                m_abyStdEventsSupported = m_Reader.ReadBytes((int)(m_Table2521.NumberStandardEvents));
                m_abyMfgEventsSupported = m_Reader.ReadBytes((int)(m_Table2521.NumberManufacturerEvents));
            }
        }

        #endregion

        #region Members

        private ICSMfgTable2521 m_Table2521;
        private byte[] m_abyStdEventsSupported;
        private byte[] m_abyMfgEventsSupported;

        #endregion
    }

    /// <summary>
    /// ICS Mfg Table Event Log Control
    /// </summary>
    public class ICSMfgTable2523 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;

        private readonly int[] ICS_USER_EVENTS = new int[] {
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FAILURE_TO_SET_METER_TIME,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SYNCHRONIZATION_TIME_ERROR_IS_TOO_LARGE,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_COMMUNICATION_FAULT,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_COMMUNICATION_REESTABLISHED,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_OUTAGE_DETECTED,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_ERROR_GREATER_THAN_MAXIMUM_CORRECTABLE_TIME_ERROR,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_POWER_OUTAGES_RECOGNIZED_BY_SSI_MODULE,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_POWER_OUTAGES_DUE_TO_LOSS_OF_AC_POWER,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_TILT_SWITCH_SET,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_TILT_SWITCH_CLEARED,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_MODULE_INVERTED,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.EXTREME_TEMPERATURE_SHUTDOWN,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.EXTREME_TEMPERATURE_INSERVICE,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_IMAGE_CORRUPTED,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_RESTORATION_DETECTED,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.IP_ADDRESS_REPORT,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.DELAYED_RESET_SSI_MODULE_ALARM,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.EXTENSIBLE_FIRMWARE_DOWNLOAD_STATUS,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SET_ACTIVE_FIRMWARE_ALARM
                    };
        private readonly int[] ICS_NON_USER_EVENTS = new int[] {
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_COMMUNICATION_STATUS_ALARM,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.COMM_LINK_FAILURE,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CELL_TOWER_CHANGES_EXCEED_THRESHOLD,
                    (int)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SECTOR_CHANGES_EXCEED_THRESHOLD
                    };

        #endregion

        #region Definitions

        /// <summary>The Resource Project strings</summary>
        protected static readonly string RESOURCE_FILE_PROJECT_STRINGS =
                                            "Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings";
        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_1536_1551
        {
            /// <summary>Unintelligible Message Received - Index 0</summary>
            UNINTELLIGIBLE_MESSAGE_RECEIVED = 0x01,
            /// <summary>Failure to Set Meter Time - Index 1</summary>
            FAILURE_TO_SET_METER_TIME = 0x02,
            /// <summary>Synchronization Time Error is too Large - Index 2</summary>
            SYNCHRONIZATION_TIME_ERROR_IS_TOO_LARGE = 0x04,
            /// <summary>Meter Communication Fault - Index 4</summary>
            METER_COMMUNICATION_FAULT = 0x10,
            /// <summary>Meter Communication Reestablished - Index 6</summary>
            METER_COMMUNICATION_REESTABLISHED = 0x40,
            /// <summary>Power Outage Detected - Index 7</summary>
            POWER_OUTAGE_DETECTED = 0x80,
            /// <summary>Meter Error Greater than Maximum Correctable Time Error - Index 8</summary>
            METER_ERROR_GREATER_THAN_MAXIMUM_CORRECTABLE_TIME_ERROR = 0x100,
            /// <summary>Tamper Detection Password Recovery Detected- Index 9</summary>
            TAMPER_DETECTION_PASSWORD_RECOVERY_DETECTED = 0x200,
            /// <summary>Tamper Detection Total Demand Resets- Index 10</summary>
            TAMPER_DETECTION_TOTAL_DEMAND_RESETS = 0x400,
            /// <summary>Tamper Detection Optical Port Session in Progress- Index 11</summary>
            TAMPER_DETECTION_OPTICAL_PORT_SESSION_IN_PROGRESS = 0x800,
            /// <summary>Tamper Detection Reconfigured Reprogrammed- Index 12</summary>
            TAMPER_DETECTION_DEVICE_RECONFIGURED_REPROGRAMMED = 0x1000,
            /// <summary>Tamper Detection Service Error Detected- Index 13</summary>
            TAMPER_DETECTION_SERVICE_ERROR_DETECTED = 0x2000,
            /// <summary>Tamper Detection Password Failure- Index 14</summary>
            TAMPER_DETECTION_PASSWORD_FAILURE = 0x4000,
            /// <summary>Tamper Detection Power Outages Recognized by SSI Module- Index 15</summary>
            TAMPER_DETECTION_POWER_OUTAGES_RECOGNIZED_BY_SSI_MODULE = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_1552_1567
        {
            /// <summary>Tamper Detection Power Outages Due to Loss of AC Power - Index 16</summary>
            TAMPER_DETECTION_POWER_OUTAGES_DUE_TO_LOSS_OF_AC_POWER = 0x01,
            /// <summary>Tamper Detection Tilt Switch Set - Index 17</summary>
            TAMPER_DETECTION_TILT_SWITCH_SET = 0x02,
            /// <summary>Tamper Detection Tilt Switch Cleared - Index 18</summary>
            TAMPER_DETECTION_TILT_SWITCH_CLEARED = 0x04,
            /// <summary>Tamper Detection Disconnect Switch Bypass - Index 19</summary>
            TAMPER_DETECTION_REMOTE_DISCONNECT_SWITCH_BYPASS = 0x08,
            /// <summary>Tamper Detection Module Inverted - Index 20</summary>
            TAMPER_DETECTION_MODULE_INVERTED = 0x10,
            /// <summary>Extreme Temperature Shutdown - Index 21</summary>
            EXTREME_TEMPERATURE_SHUTDOWN = 0x20,
            /// <summary>Extreme Temperature In Service - Index 22</summary>
            EXTREME_TEMPERATURE_INSERVICE = 0x40,
            /// <summary>Firmware Image Corrupted - Index 23</summary>
            FIRMWARE_IMAGE_CORRUPTED = 0x80,
            /// <summary>Power Quality Detection Diagnostic 1 - Index 24</summary>
            POWER_QUALITY_DETECTION_DIAGNOSTIC_1 = 0x100,
            /// <summary>Power Quality Detection Diagnostic 2 - Index 25</summary>
            POWER_QUALITY_DETECTION_DIAGNOSTIC_2 = 0x200,
            /// <summary>Power Quality Detection Diagnostic 3 - Index 26</summary>
            POWER_QUALITY_DETECTION_DIAGNOSTIC_3 = 0x400,
            /// <summary>Power Quality Detection Diagnostic 4 - Index 27</summary>
            POWER_QUALITY_DETECTION_DIAGNOSTIC_4 = 0x800,
            /// <summary>Power Quality Detection Diagnostic 5 - Index 28</summary>
            POWER_QUALITY_DETECTION_DIAGNOSTIC_5 = 0x1000,
            /// <summary>Power Quality Detection Diagnostic 6 - Index 29</summary>
            POWER_QUALITY_DETECTION_DIAGNOSTIC_6 = 0x2000,
            /// <summary>Power Quality Detection Diagnostic 7 - Index 30</summary>
            POWER_QUALITY_DETECTION_DIAGNOSTIC_7 = 0x4000,
            /// <summary>Power Quality Detection Diagnostic 8 - Index 31</summary>
            POWER_QUALITY_DETECTION_DIAGNOSTIC_8 = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_1568_1583
        {
            /// <summary>Power Quality Detection Voltage Sag - Index 32</summary>
            POWER_QUALITY_DETECTION_VOLTAGE_SAG = 0x01,
            /// <summary>Power Quality Detection Voltage Swell - Index 33</summary>
            POWER_QUALITY_DETECTION_VOLTAGE_SWELL = 0x02,
            /// <summary>Meter Passwords Out of Sync - Index 34</summary>
            METER_PASSWORDS_OUT_OF_SYNC = 0x04,
            /// <summary>RCDC Fault RCDC Comm Error - Index 35</summary>
            RCDC_FAULT_RCDC_COMM_ERROR = 0x08,
            /// <summary>RCDC Fault Switch Controller Error - Index 36</summary>
            RCDC_FAULT_SWITCH_CONTROLLER_ERROR = 0x10,
            /// <summary>RCDC Fault Switch Failed to Close - Index 37</summary>
            RCDC_FAULT_SWITCHED_FAILED_TO_CLOSE = 0x20,
            /// <summary>RCDC Fault Alternate Source - Index 38</summary>
            RCDC_FAULT_ALTERNATE_SOURCE = 0x40,
            /// <summary>RCDC Fault Bypassed - Index 39</summary>
            RCDC_FAULT_BYPASSED = 0x80,
            /// <summary>RCDC Fault Switch Failed to Open - Index 40</summary>
            RCDC_FAULT_SWITCH_FAILED_TO_OPEN = 0x100,
            /// <summary>RCDC Fault PPM Alert - Index 41</summary>
            RCDC_FAULT_PPM_ALERT = 0x200,
            /// <summary>RCDC Fault Manual Arm Timed Out - Index 42</summary>
            RCDC_FAULT_MANUAL_ARM_TIMED_OUT = 0x400,
            /// <summary>Auto Registration - Index 43</summary>
            AUTO_REGISTRATION = 0x800,
            /// <summary>Successful Time Sync Time Change Occurred - Index 44</summary>
            SUCCESSFUL_TIME_SYNC_TIME_CHANGE_OCCURRED = 0x1000,
            /// <summary>Power Restoration Detected- Index 45</summary>
            POWER_RESTORATION_DETECTED = 0x2000,
            /// <summary>IP Address Report - Index 46</summary>
            IP_ADDRESS_REPORT = 0x4000,
            /// <summary>Delayed Reset SSI Module Alarm - Index 47</summary>
            DELAYED_RESET_SSI_MODULE_ALARM = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_1584_1599
        {
            /// <summary>Meter Communication Status Alarm - Index 48</summary>
            METER_COMMUNICATION_STATUS_ALARM = 0x01,
            /// <summary>Extensible Firmware Download Status - Index 49</summary>
            EXTENSIBLE_FIRMWARE_DOWNLOAD_STATUS = 0x02,
            /// <summary>Set Active Firmware Alarm - Index 50</summary>
            SET_ACTIVE_FIRMWARE_ALARM = 0x04,
            /// <summary>Firmware Upgrade Download Alarm - Index 51</summary>
            FIRMWARE_UPGRADE_DOWNLOAD_ALARM = 0x08,
            /// <summary>Firmware Upgrade Active Alarm - Index 52</summary>
            FIRMWARE_UPGRADE_ACTIVE_ALARM = 0x10,
            /// <summary>Firmware Download Copying File - Index 53</summary>
            FIRMWARE_DOWNLOAD_COPYING_FILE = 0x20,
            /// <summary>Firmware Download Canceling - Index 54</summary>
            FIRMWARE_DOWNLOAD_CANCELING = 0x40,
            /// <summary>Firmware Download Cancelled - Index 55</summary>
            FIRMWARE_DOWNLOAD_CANCELED = 0x80,
            /// <summary>Firmware Download Total Time - Index 56</summary>
            FIRMWARE_DOWNLOAD_TOTAL_TIME = 0x100,
            /// <summary>Firmware Download Successful - Index 57</summary>
            FIRMWARE_DOWNLOAD_SUCCESSFUL = 0x200,
            /// <summary>Firmware Download Removing Inactive Files - Index 58</summary>
            FIRMWARE_DOWNLOAD_REMOVING_INACTIVE_FILES = 0x400,
            /// <summary>Firmware Download Retries Exceeded - Index 59</summary>
            FIRMWARE_DOWNLOAD_RETRIES_EXCEEDED = 0x800,
            /// <summary>Firmware Download Failed Will Retry - Index 60</summary>
            FIRMWARE_DOWNLOAD_FAILED_WILL_RETRY = 0x1000,
            /// <summary>Firmware Download Incorrect Version - Index 61</summary>
            FIRMWARE_DOWNLOAD_INCORRECT_VERSION = 0x2000,
            /// <summary>Firmware Download File Exists - Index 62</summary>
            FIRMWARE_DOWNLOAD_FILE_EXISTS = 0x4000,
            /// <summary>Firmware Download Activating - Index 63</summary>
            FIRMWARE_DOWNLOAD_ACTIVATING = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_1600_1615
        {
            /// <summary>Firmware Download Set Active Reboot - Index 64</summary>
            FIRMWARE_DOWNLOAD_SET_ACTIVE_REBOOT = 0x01,
            /// <summary>Comm Link Failure - Index 65</summary>
            COMM_LINK_FAILURE = 0x02,
            /// <summary>ICM Reboot Modem Not Responding - Index 66</summary>
            ICM_REBOOT_MODEM_NOT_RESPONDING = 0x04,
            /// <summary>Initial Modem Provision - Index 67</summary>
            INITIAL_MODEM_PROVISION = 0x08,
            /// <summary>Modem Provision Failed - Index 68</summary>
            MODEM_PROVISION_FAILED = 0x10,
            /// <summary>Modem Provision Successful - Index 69</summary>
            MODEM_PROVISION_SUCCESSFUL = 0x20,
            /// <summary>Modem Identity Error - Index 70</summary>
            MODEM_IDENTITY_ERROR = 0x40,
            /// <summary>CDMA Subscription Error - Index 71</summary>
            CDMA_SUBSCRIPTION_ERROR = 0x80,
            /// <summary>MDN Login Error - Index 72</summary>
            MDN_LOGIN_ERROR = 0x100,
            /// <summary>Received SMS - Index 73</summary>
            RECEIVED_SMS = 0x200,
            /// <summary>Gateway Changed - Index 74</summary>
            GATEWAY_CHANGED = 0x400,
            /// <summary>Cellular Timeout Sent - Index 75</summary>
            CELLUAR_TIMEOUT_SENT = 0x800,
            /// <summary>Cellular Timeout Received - Index 76</summary>
            CELLUAR_TIMEOUT_RECEIVED = 0x1000,
            /// <summary>Unknown Meter Type - Index 77</summary>
            UNKNOWN_METER_TYPE = 0x2000,
            /// <summary>Meter Model I 210C - Index 78</summary>
            METER_MODEL_I_210C = 0x4000,
            /// <summary>Meter Model I 210 - Index 79</summary>
            METER_MODEL_I_210 = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_1616_1631
        {
            /// <summary>Meter Model kV2C - Index 80</summary>
            METER_MODEL_KV2C = 0x01,
            /// <summary>Meter Inversion Detected on Startup - Index 81</summary>
            METER_INVERSION_DETECTED_ON_STARTUP = 0x02,
            /// <summary>Module Power Fail - Index 82</summary>
            MODULE_POWER_FAIL = 0x04,
            /// <summary>Qualified Power Fail - Index 83</summary>
            QUALIFIED_POWER_FAIL = 0x08,
            /// <summary>ICM Heater Enabled - Index 84</summary>
            ICM_HEATER_ENABLED = 0x10,
            /// <summary>ICM Heater Disabled - Index 85</summary>
            ICM_HEATER_DISABLED = 0x20,
            /// <summary>CLI Password Failed - Index 86</summary>
            CLI_PASSWORD_FAILED = 0x40,
            /// <summary>ICM Entering Quiet Mode - Index 87</summary>
            ICM_ENTERING_QUIET_MODE = 0x80,
            /// <summary>ICM Leaving Quiet Mode - Index 88</summary>
            ICM_LEAVING_QUIET_MODE = 0x100,
            /// <summary>Magnetic Swipe in Manufacturing Mode - Index 89</summary>
            MAGNETIC_SWIPE_IN_MANUFACTURING_MODE = 0x200,
            /// <summary>Magnetic Swipe in Production Mode - Index 90</summary>
            MAGNETIC_SWIPE_IN_PRODUCTION_MODE = 0x400,
            /// <summary>Magnetic Swipe Ignored Not in Non-Comm - Index 91</summary>
            MAGNETIC_SWIPE_IGNORED_NOT_IN_NON_COMM = 0x800,
            /// <summary>Magnetic Swipe Ignored Condition Not Met - Index 92</summary>
            MAGNETIC_SWIPE_IGNORED_CONDITION_NOT_MET = 0x1000,
            /// <summary>Meter Login via Optical Port - Index 93</summary>
            METER_LOGIN_VIA_OPTICAL_PORT = 0x2000,
            /// <summary>Meter Logoff via Optical Port - Index 94</summary>
            METER_LOGOFF_VIA_OPTICAL_PORT = 0x4000,
            /// <summary>CLI Login via Optical Port - Index 95</summary>
            CLI_LOGIN_VIA_OPTICAL_PORT = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_1632_1647
        {
            /// <summary>CLI Login Attempted via Optical Port - Index 96</summary>
            CLI_LOGIN_ATTEMPTED_VIA_OPTICAL_PORT = 0x01,
            /// <summary>CLI Command Executed via Optical Port - Index 97</summary>
            CLI_COMMAND_EXECUTED_VIA_OPTICAL_PORT = 0x02,
            /// <summary>CLI Locked Out Too Many Login Attempts via Optical Port - Index 98</summary>
            CLI_LOCKED_OUT_TOO_MANY_LOGIN_ATTEMPTS_VIA_OPTICAL_PORT = 0x04,
            /// <summary>ZigBee Optical Passthrough Started - Index 99</summary>
            ZIGBEE_OPTICAL_PASSTHROUGH_STARTED = 0x08,
            /// <summary>Modem Optical Passthrough Started - Index 100</summary>
            MODEM_OPTICAL_PASSTHROUGH_STARTED = 0x10,
            /// <summary>Started C12.18 Session via Optical Port - Index 101</summary>
            STARTED_C1218_SESSION_VIA_OPTICAL_PORT = 0x20,
            /// <summary>ICM Configuration Change - Index 102</summary>
            ICM_CONFIGURATION_CHANGE = 0x40,
            /// <summary>ICM State Changed - Index 103</summary>
            ICM_STATE_CHANGED = 0x80,
            /// <summary>ICM Time Updated From Network - Index 104</summary>
            ICM_TIME_UPDATED_FROM_NETWORK = 0x100,
            /// <summary>ICM Time Set From Meter - Index 105</summary>
            ICM_TIME_SET_FROM_METER = 0x200,
            /// <summary>Time Synch State Changed - Index 106</summary>
            TIMESYNCH_STATE_CHANGED = 0x400,
            /// <summary>CLI is Disabled- Index 107</summary>
            CLI_IS_DISABLED = 0x800,
            /// <summary>CLI is Revert Only - Index 108</summary>
            CLI_IS_REVERT_ONLY = 0x1000,
            /// <summary>Loadside Voltage While Switch Open - Index 109</summary>
            LOADSIDE_VOLTAGE_WHILE_SWITCH_OPEN = 0x2000,
            /// <summary>Loadside Voltage While Switch Open - Index 110</summary>
            LOADSIDE_VOLTAGE_WHILE_SWITCH_OPEN_CLEAR = 0x4000,
            /// <summary>Loadside Voltage While Switch Open - Index 111</summary>
            CELL_TOWER_CHANGES_EXCEED_THRESHOLD = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_1648_1663
        {
            /// <summary>Sector Changes Exceed Threshold - Index 112</summary>
            SECTOR_CHANGES_EXCEED_THRESHOLD = 0x01,
            /// <summary>Accumulator Read Failure - Index 113</summary>
            ACCUMULATOR_READ_FAILURE = 0x02,
            /// <summary>Cellular Connection Timeout Alarm - Index 114</summary>
            CELLULAR_CONNECTION_TIMEOUT_ALARM = 0x04,
            /// <summary>SMS Wakeup Received - Index 115</summary>
            SMS_WAKEUP_RECEIVED = 0x08,
            /// <summary>Meter Model OpenWay CENTRON - Index 116</summary>
            METER_MODEL_OW_CENTRON = 0x10,
            /// <summary>ERT Successfully Initialized SPI Driver - Index 117</summary>
            ERT_SUCCESSFULLY_INITIALIZED_SPI_DRIVER = 0x20,
            /// <summary>ERT CC1121 PART NUMBER - Index 118</summary>
            ERT_CC1121_PART_NUMBER = 0x40,
            /// <summary>ERT CC1121 PART NUMBER - Index 119</summary>
            ERT_ERT_HW_SUCCESSFULLY_INITIALIZED = 0x80,
            /// <summary>ERT RADIO TURNED OFF IN ERT CFG DATA TABLE - Index 120</summary>
            ERT_ERT_RADIO_TURNED_OFF_IN_ERT_CFG_DATA_TABLE = 0x100,
            /// <summary>ERT CC1121 MANUAL CALIBRATION SUCCESSFUL - Index 121</summary>
            ERT_ERT_CC1121_MANUAL_CALIBRATION_SUCCESSFUL = 0x200,
            /// <summary>ERT MASTER LIST INITIALIZATION CREATION FAILED - Index 122</summary>
            ERT_ERT_MASTER_LIST_INITIALIZATION_CREATION_FAILED = 0x400,
            /// <summary>ICM camping channel - Index 123</summary>
            ERT_ICM_CAMPING_CHANNEL = 0x800,
            /// <summary>ERT: ADDING AN ERT METER READING TO OUR MASTER LIST - Index 124</summary>
            ERT_ADDING_AN_ERT_METER_READING_TO_OUR_MASTER_LIST = 0x1000,
            /// <summary>ERT: REACHED MAX NUMBER OF ERT METERS - Index 125</summary>
            ERT_REACHED_MAX_NUMBER_OF_ERT_METERS = 0x2000,
            /// <summary>ERT: INCOMING ERT PACKET CRC ERROR - Index 126</summary>
            ERT_INCOMING_ERT_PACKET_CRC_ERROR = 0x4000,
            /// <summary>ERT: CHANGING ERT RESTING CHANNEL - Index 127</summary>
            ERT_CHANGING_ERT_RESTING_CHANNEL = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_1664_1679
        {
            /// <summary>ERT: ERT RADIO OFF FREEZE PROC IMC REJECTED - Index 128</summary>
            ERT_ERT_RADIO_OFF_FREEZE_PROC_IMC_REJECTED = 0x01,
            /// <summary>ERT: NUMBER OF ERTS WHOSE RECORDS WERE FROZEN - Index 129</summary>
            ERT_NUMBER_OF_ERTS_WHOSE_RECORDS_WERE_FROZEN = 0x02,
            /// <summary>ERT: RECEIVED INVALID TIME FROM NTP TASK - Index 130</summary>
            ERT_RECEIVED_INVALID_TIME_FROM_NTP_TASK = 0x04,
            /// <summary>ERT: UNSUCCESSFUL PDOID READ - Index 131</summary>
            ERT_UNSUCCESSFUL_PDOID_READ = 0x08,
            /// <summary>ERT 242 TX FAILED - Index 132</summary>
            ERT_242_TX_FAILED = 0x10,
            /// <summary>Added ERT to Managed List - Index 133</summary>
            ADDED_ERT_TO_MANAGED_LIST = 0x20,
            /// <summary>ERT Time Set Failed - Index 134</summary>
            ERT_TIME_SET_FAILED = 0x40,
            /// <summary>ICM Event Log Cleared - Index 135</summary>
            ICM_EVENT_LOG_CLEARED = 0x80,
            ///// <summary>POWER QUALITY DETECTION MOMENTARY INTERRUPTION - Index 136</summary>
            //POWER_QUALITY_DETECTION_MOMENTARY_INTERRUPTION = 0x100,
            ///// <summary>POWER QUALITY DETECTION SUSTAINED INTERRUPTION - Index 137</summary>
            //POWER_QUALITY_DETECTION_SUSTAINED_INTERRUPTION = 0x200,
            /// <summary>TAMPER TILT SET ON OUTAGE (REMOVAL TAMPER) - Index 138</summary>
            TAMPER_TILT_SET_ON_OUTAGE = 0x400,
            /// <summary>Configuration Commit - Index 139</summary>
            CONFIGURATION_COMMIT = 0x800,
            /// <summary>Firmware Download: Initialization Failure - Index 140</summary>
            FIRMWARE_DOWNLOAD_INITIALIZATION_FAILURE = 0x1000,
            /// <summary>HAN Firmware Download Failure - Index 141</summary>
            HAN_FIRMWARE_DOWNLOAD_FAILURE = 0x2000,
            /// <summary>ERT 242 Command Request - Index 142</summary>
            ERT_242_COMMAND_REQUEST = 0x4000,
            /// <summary>SMS Wakeup Identity Request Sent - Index 143</summary>
            SMS_WAKEUP_IDENTITY_REQUEST_SENT = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_1680_1695
        {
            /// <summary>SMS Wakeup Identity Request Not Sent Because Not Registered - Index 144</summary>
            SMS_WAKEUP_IDENTITY_NOT_SENT_BECAUSE_NOT_REGISTERED = 0x01,
            /// <summary>SMS Wakeup Identity Request Not Sent Because Not Synchronized - Index 145</summary>
            SMS_WAKEUP_IDENTITY_REQUEST_NOT_SENT_BECAUSE_NOT_SYNCHRONIZED = 0x02,
            /// <summary>Failed Security Key Verification - Index 146</summary>
            FAILED_SECURITY_KEY_VERIFICATION = 0x04,
            /// <summary>Failed CE Configuration Verification - Index 147</summary>
            FAILED_CE_CONFIGURATION_VERIFICATION = 0x08,
            /// <summary>Migration State Change - Index 148</summary>
            MIGRATION_STATE_CHANGE = 0x10,
            /// <summary>Critical Peak Pricing Status - Index 149</summary>
            CRITICAL_PEAK_PRICING_STATUS = 0x20,
            /// <summary>Security Event - Index 150</summary>
            SECURITY_EVENT = 0x40,
            /// <summary>ERT Meter Stolen - Index 151</summary>
            ERT_METER_STOLEN = 0x80,
            /// <summary>ERT Meter Removed - Index 152</summary>
            ERT_METER_REMOVED = 0x100,
            /// <summary>ERT Connection Downtime  Time Exceeded - Index 153</summary>
            ERT_CONNECTION_DOWNTIME_TIME_EXCEEDED = 0x200,
            /// <summary>ERT Predictor List Time Modified - Index 154</summary>
            ERT_PREDICTOR_LIST_TIME_MODIFIED = 0x400,
            /// <summary>ERT CC1121 Manual Calibration Failed - Index 155</summary>
            ERT_MANUAL_CALIBRATION_FAILED = 0x800,
            /// <summary>ICM tracking 100G failed - Index 156</summary>
            ERT_100G_TRACKING_FAILED = 0x1000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_1696_1711
        {
            /// <summary>Time Source Unavailable - Index 165</summary>
            TIME_SOURCE_UNAVAILABLE = 0x20,
            /// <summary>Signing Key Update Success - Index 166</summary>
            SIGNING_KEY_UPDATE_SUCCESS = 0x40,
            /// <summary>Symmetric Key Update Success - Index 167</summary>
            SYMMETRIC_KEY_UPDATE_SUCCESS = 0x80,
            /// <summary>Key Roll Over Success - Index 168</summary>
            KEY_ROLLOVER_SUCCESS = 0x100,
        }

        ///// <summary>
        ///// Each item in the enum is a bit mask for identifying whether or
        ///// not the meter has been configured to record the event
        ///// </summary>
        //protected enum Event_1984_1999
        //{
        //    /// <summary>Unprogrammed - Index 448</summary>
        //    UNPROGRAMMED = 0x00,
        //    // 449: 0x02
        //    //450: 0x04
        //    /// <summary>RAM Failure - Index 451</summary>
        //    RAM_FAILURE = 0x08,
        //    /// <summary>ROM Failure - Index 452</summary>
        //    ROM_FAILURE = 0x10,
        //    /// <summary>NONVOLATILE MEMORY FAILURE - Index 453</summary>
        //    NONVOLATILE_MEMORY_FAILURE = 0x20,
        //    /// <summary>CLOCK ERROR - Index 454</summary>
        //    CLOCK_ERROR = 0x40,
        //    /// <summary>Measurement Error - Index 455</summary>
        //    MEASUREMENT_ERROR = 0x80,
        //    /// <summary>Low Battery Detected - Index 456</summary>
        //    LOW_BATTERY = 0x100,
        //    /// <summary>Low Loss Potential - Index 457</summary>
        //    LOW_LOSS_POTENTIAL = 0x200,
        //    /// <summary>Demand Overload - Index 458</summary>
        //    DEMAND_OVERLOAD = 0x400,
        //    /// <summary>Power Failure - Index 459</summary>
        //    POWER_FAILURE = 0x800,
        //    /// <summary>Bad Password - Index 460</summary>
        //    BAD_PASSWORD = 0x1000,
        //}

        ///// <summary>
        ///// Each item in the enum is a bit mask for identifying whether or
        ///// not the meter has been configured to record the event
        ///// </summary>
        //protected enum Event_2000_2015
        //{
        //    /// <summary>METERING_ERROR- Index 472</summary>
        //    METERING_ERROR = 0x100,
        //    /// <summary>DC DETECTED (I-210) OR TIME CHANGED (kV2c) - Index 473</summary>
        //    DC_DETECTED_OR_TIME_CHANGED = 0x200,
        //    /// <summary>System Error - Index 474</summary>
        //    SYSTEM_ERROR = 0x400,
        //    /// <summary>Received KWH - Index 475</summary>
        //    RECEIVED_KWH = 0x800,
        //    /// <summary>Leading KVARH- Index 476</summary>
        //    LEADING_KVARH = 0x1000,
        //    /// <summary>Loss of Program- Index 477</summary>
        //    LOSS_OF_PROGRAM = 0x2000,
        //     /// <summary>Time Changed Status- Index 479</summary>
        //    TIME_CHANGED_STATUS = 0x8000,
        //}

        ///// <summary>
        ///// Each item in the enum is a bit mask for identifying whether or
        ///// not the meter has been configured to record the event
        ///// </summary>
        //protected enum Event_2016_2031
        //{
        //    /// <summary>HIGH TEMP (I-210) OR FLASH CODE ERROR (kV2c)- Index 480</summary>
        //    HIGH_TEMP_OR_FLASH_CODE_ERROR = 0x01,
        //    /// <summary>Data Flash Error - Index 481</summary>
        //    FLASH_DATA_ERROR = 0x02,
        //}

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">the PSEM object for the device</param>
        /// <param name="table2522">MFG ICM table 2522 object</param>
        /// <param name="table2521">MFG ICM table 2521 object</param>
        /// <param name="CommModuleVersion">the firmware version of the ICM</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  08/01/16 AF  4.60.02 623194 Added a comm module version parameter
        //
        public ICSMfgTable2523(CPSEM psem, ICSMfgTable2522 table2522, ICSMfgTable2521 table2521, byte CommModuleVersion)
            : base(psem, 2523, GetTableLength(table2521), TABLE_TIMEOUT)
        {
            m_ICSMfgTable2521 = table2521;
            m_ICSMfgTable2522 = table2522;
            m_psem = psem;
            m_ICMFWVersion = CommModuleVersion;

            m_rmStrings = new System.Resources.ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                                   this.GetType().Assembly);
            m_lstEvents = new List<MFG2048EventItem>();
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        /// <param name="table2522">MFG ICM table 2522 object</param>
        /// <param name="table2521">MFG ICM table 2521 object</param>
        /// <param name="ICMFWVersion">the firmware version of the ICM</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  08/01/16 AF  4.60.02 623194 Added a comm module version parameter
        //
        public ICSMfgTable2523(PSEMBinaryReader reader, ICSMfgTable2522 table2522, ICSMfgTable2521 table2521, byte ICMFWVersion)
            : base(2523, GetTableLength(table2521))
        {
            m_ICSMfgTable2521 = table2521;
            m_ICSMfgTable2522 = table2522;
            m_ICMFWVersion = ICMFWVersion;

            m_Reader = reader;
            m_rmStrings = new System.Resources.ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                                   this.GetType().Assembly);
            m_lstEvents = new List<MFG2048EventItem>();

            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Full read of 2523 (Mfg 475) out of the meter
        /// </summary>
        /// <returns>The PSEM response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "ICMMfgTable2523.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns a list of supported events with their descriptions and whether 
        /// or not they are monitored.  Designed to provide the same information as 
        /// MFGTable2048.HistoryLogConfig.HistoryLogEventList
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  08/01/16 AF  4.60.02 623194 Changed the way we create the event list.
        //
        public List<MFG2048EventItem> ICSHistoryLogEventList
        {
            get
            {
                List<EventEntry> lstSupportedEvents = new List<EventEntry>();
                byte ICM_3G_FWVersion = 27;

                ReadUnloadedTable();

                m_lstEvents.Clear();


                // build the event list
                // 3G meters only
                if (m_ICMFWVersion <= ICM_3G_FWVersion)
                {
                    // Add Event_1536_1551 items
                    AddEventItem(Enum.GetName(typeof(Event_1536_1551), Event_1536_1551.UNINTELLIGIBLE_MESSAGE_RECEIVED), m_usEvent1536_1551, (UInt16)(Event_1536_1551.UNINTELLIGIBLE_MESSAGE_RECEIVED));
                    AddEventItem(Enum.GetName(typeof(Event_1536_1551), Event_1536_1551.SYNCHRONIZATION_TIME_ERROR_IS_TOO_LARGE), m_usEvent1536_1551, (UInt16)(Event_1536_1551.SYNCHRONIZATION_TIME_ERROR_IS_TOO_LARGE));
                    AddEventItem(Enum.GetName(typeof(Event_1536_1551), Event_1536_1551.TAMPER_DETECTION_PASSWORD_RECOVERY_DETECTED), m_usEvent1536_1551, (UInt16)(Event_1536_1551.TAMPER_DETECTION_PASSWORD_RECOVERY_DETECTED));
                    AddEventItem(Enum.GetName(typeof(Event_1536_1551), Event_1536_1551.TAMPER_DETECTION_TOTAL_DEMAND_RESETS), m_usEvent1536_1551, (UInt16)(Event_1536_1551.TAMPER_DETECTION_TOTAL_DEMAND_RESETS));
                    AddEventItem(Enum.GetName(typeof(Event_1536_1551), Event_1536_1551.TAMPER_DETECTION_OPTICAL_PORT_SESSION_IN_PROGRESS), m_usEvent1536_1551, (UInt16)(Event_1536_1551.TAMPER_DETECTION_OPTICAL_PORT_SESSION_IN_PROGRESS));
                    AddEventItem(Enum.GetName(typeof(Event_1536_1551), Event_1536_1551.TAMPER_DETECTION_DEVICE_RECONFIGURED_REPROGRAMMED), m_usEvent1536_1551, (UInt16)(Event_1536_1551.TAMPER_DETECTION_DEVICE_RECONFIGURED_REPROGRAMMED));
                    AddEventItem(Enum.GetName(typeof(Event_1536_1551), Event_1536_1551.TAMPER_DETECTION_SERVICE_ERROR_DETECTED), m_usEvent1536_1551, (UInt16)(Event_1536_1551.TAMPER_DETECTION_SERVICE_ERROR_DETECTED));
                    AddEventItem(Enum.GetName(typeof(Event_1536_1551), Event_1536_1551.TAMPER_DETECTION_PASSWORD_FAILURE), m_usEvent1536_1551, (UInt16)(Event_1536_1551.TAMPER_DETECTION_PASSWORD_FAILURE));
                    //Event_1552_1567
                    AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.TAMPER_DETECTION_POWER_OUTAGES_DUE_TO_LOSS_OF_AC_POWER), m_usEvent1552_1567, (UInt16)(Event_1552_1567.TAMPER_DETECTION_POWER_OUTAGES_DUE_TO_LOSS_OF_AC_POWER));
                    AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.TAMPER_DETECTION_REMOTE_DISCONNECT_SWITCH_BYPASS), m_usEvent1552_1567, (UInt16)(Event_1552_1567.TAMPER_DETECTION_REMOTE_DISCONNECT_SWITCH_BYPASS));
                    AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.FIRMWARE_IMAGE_CORRUPTED), m_usEvent1552_1567, (UInt16)(Event_1552_1567.FIRMWARE_IMAGE_CORRUPTED));
                    AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_1), m_usEvent1552_1567, (UInt16)(Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_1));
                    AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_2), m_usEvent1552_1567, (UInt16)(Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_2));
                    AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_3), m_usEvent1552_1567, (UInt16)(Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_3));
                    AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_4), m_usEvent1552_1567, (UInt16)(Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_4));
                    AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_5), m_usEvent1552_1567, (UInt16)(Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_5));
                    AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_6), m_usEvent1552_1567, (UInt16)(Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_6));
                    AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_7), m_usEvent1552_1567, (UInt16)(Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_7));
                    AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_8), m_usEvent1552_1567, (UInt16)(Event_1552_1567.POWER_QUALITY_DETECTION_DIAGNOSTIC_8));
                    //Event_1568_1583
                    AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.POWER_QUALITY_DETECTION_VOLTAGE_SAG), m_usEvent1568_1583, (UInt16)(Event_1568_1583.POWER_QUALITY_DETECTION_VOLTAGE_SAG));
                    AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.POWER_QUALITY_DETECTION_VOLTAGE_SWELL), m_usEvent1568_1583, (UInt16)(Event_1568_1583.POWER_QUALITY_DETECTION_VOLTAGE_SWELL));
                    AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.METER_PASSWORDS_OUT_OF_SYNC), m_usEvent1568_1583, (UInt16)(Event_1568_1583.METER_PASSWORDS_OUT_OF_SYNC));
                    AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.RCDC_FAULT_RCDC_COMM_ERROR), m_usEvent1568_1583, (UInt16)(Event_1568_1583.RCDC_FAULT_RCDC_COMM_ERROR));
                    AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.RCDC_FAULT_SWITCH_CONTROLLER_ERROR), m_usEvent1568_1583, (UInt16)(Event_1568_1583.RCDC_FAULT_SWITCH_CONTROLLER_ERROR));
                    AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.RCDC_FAULT_SWITCHED_FAILED_TO_CLOSE), m_usEvent1568_1583, (UInt16)(Event_1568_1583.RCDC_FAULT_SWITCHED_FAILED_TO_CLOSE));
                    AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.RCDC_FAULT_ALTERNATE_SOURCE), m_usEvent1568_1583, (UInt16)(Event_1568_1583.RCDC_FAULT_ALTERNATE_SOURCE));
                    AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.RCDC_FAULT_BYPASSED), m_usEvent1568_1583, (UInt16)(Event_1568_1583.RCDC_FAULT_BYPASSED));
                    AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.RCDC_FAULT_SWITCH_FAILED_TO_OPEN), m_usEvent1568_1583, (UInt16)(Event_1568_1583.RCDC_FAULT_SWITCH_FAILED_TO_OPEN));
                    AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.RCDC_FAULT_PPM_ALERT), m_usEvent1568_1583, (UInt16)(Event_1568_1583.RCDC_FAULT_PPM_ALERT));
                    AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.RCDC_FAULT_MANUAL_ARM_TIMED_OUT), m_usEvent1568_1583, (UInt16)(Event_1568_1583.RCDC_FAULT_MANUAL_ARM_TIMED_OUT));
                    AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.AUTO_REGISTRATION), m_usEvent1568_1583, (UInt16)(Event_1568_1583.AUTO_REGISTRATION));
                    AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.SUCCESSFUL_TIME_SYNC_TIME_CHANGE_OCCURRED), m_usEvent1568_1583, (UInt16)(Event_1568_1583.SUCCESSFUL_TIME_SYNC_TIME_CHANGE_OCCURRED));
                    AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.IP_ADDRESS_REPORT), m_usEvent1568_1583, (UInt16)(Event_1568_1583.IP_ADDRESS_REPORT));
                    //Event_1584_1599
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.EXTENSIBLE_FIRMWARE_DOWNLOAD_STATUS), m_usEvent1584_1599, (UInt16)(Event_1584_1599.EXTENSIBLE_FIRMWARE_DOWNLOAD_STATUS));
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.SET_ACTIVE_FIRMWARE_ALARM), m_usEvent1584_1599, (UInt16)(Event_1584_1599.SET_ACTIVE_FIRMWARE_ALARM));
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.FIRMWARE_UPGRADE_DOWNLOAD_ALARM), m_usEvent1584_1599, (UInt16)(Event_1584_1599.FIRMWARE_UPGRADE_DOWNLOAD_ALARM));
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.FIRMWARE_UPGRADE_ACTIVE_ALARM), m_usEvent1584_1599, (UInt16)(Event_1584_1599.FIRMWARE_UPGRADE_ACTIVE_ALARM));
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.FIRMWARE_DOWNLOAD_COPYING_FILE), m_usEvent1584_1599, (UInt16)(Event_1584_1599.FIRMWARE_DOWNLOAD_COPYING_FILE));
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.FIRMWARE_DOWNLOAD_CANCELING), m_usEvent1584_1599, (UInt16)(Event_1584_1599.FIRMWARE_DOWNLOAD_CANCELING));
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.FIRMWARE_DOWNLOAD_CANCELED), m_usEvent1584_1599, (UInt16)(Event_1584_1599.FIRMWARE_DOWNLOAD_CANCELED));
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.FIRMWARE_DOWNLOAD_TOTAL_TIME), m_usEvent1584_1599, (UInt16)(Event_1584_1599.FIRMWARE_DOWNLOAD_TOTAL_TIME));
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.FIRMWARE_DOWNLOAD_SUCCESSFUL), m_usEvent1584_1599, (UInt16)(Event_1584_1599.FIRMWARE_DOWNLOAD_SUCCESSFUL));
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.FIRMWARE_DOWNLOAD_REMOVING_INACTIVE_FILES), m_usEvent1584_1599, (UInt16)(Event_1584_1599.FIRMWARE_DOWNLOAD_REMOVING_INACTIVE_FILES));
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.FIRMWARE_DOWNLOAD_RETRIES_EXCEEDED), m_usEvent1584_1599, (UInt16)(Event_1584_1599.FIRMWARE_DOWNLOAD_RETRIES_EXCEEDED));
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.FIRMWARE_DOWNLOAD_FAILED_WILL_RETRY), m_usEvent1584_1599, (UInt16)(Event_1584_1599.FIRMWARE_DOWNLOAD_FAILED_WILL_RETRY));
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.FIRMWARE_DOWNLOAD_INCORRECT_VERSION), m_usEvent1584_1599, (UInt16)(Event_1584_1599.FIRMWARE_DOWNLOAD_INCORRECT_VERSION));
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.FIRMWARE_DOWNLOAD_FILE_EXISTS), m_usEvent1584_1599, (UInt16)(Event_1584_1599.FIRMWARE_DOWNLOAD_FILE_EXISTS));
                    AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.FIRMWARE_DOWNLOAD_ACTIVATING), m_usEvent1584_1599, (UInt16)(Event_1584_1599.FIRMWARE_DOWNLOAD_ACTIVATING));
                    //Event_1600_1615
                    AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.FIRMWARE_DOWNLOAD_SET_ACTIVE_REBOOT), m_usEvent1600_1615, (UInt16)(Event_1600_1615.FIRMWARE_DOWNLOAD_SET_ACTIVE_REBOOT));
                    AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.INITIAL_MODEM_PROVISION), m_usEvent1600_1615, (UInt16)(Event_1600_1615.INITIAL_MODEM_PROVISION));
                    AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.MODEM_PROVISION_FAILED), m_usEvent1600_1615, (UInt16)(Event_1600_1615.MODEM_PROVISION_FAILED));
                    AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.MODEM_PROVISION_SUCCESSFUL), m_usEvent1600_1615, (UInt16)(Event_1600_1615.MODEM_PROVISION_SUCCESSFUL));
                    AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.MODEM_IDENTITY_ERROR), m_usEvent1600_1615, (UInt16)(Event_1600_1615.MODEM_IDENTITY_ERROR));
                    AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.CDMA_SUBSCRIPTION_ERROR), m_usEvent1600_1615, (UInt16)(Event_1600_1615.CDMA_SUBSCRIPTION_ERROR));
                    AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.MDN_LOGIN_ERROR), m_usEvent1600_1615, (UInt16)(Event_1600_1615.MDN_LOGIN_ERROR));
                    AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.RECEIVED_SMS), m_usEvent1600_1615, (UInt16)(Event_1600_1615.RECEIVED_SMS));
                    AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.GATEWAY_CHANGED), m_usEvent1600_1615, (UInt16)(Event_1600_1615.GATEWAY_CHANGED));
                    AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.UNKNOWN_METER_TYPE), m_usEvent1600_1615, (UInt16)(Event_1600_1615.UNKNOWN_METER_TYPE));
                    AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.METER_MODEL_I_210C), m_usEvent1600_1615, (UInt16)(Event_1600_1615.METER_MODEL_I_210C));
                    AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.METER_MODEL_I_210), m_usEvent1600_1615, (UInt16)(Event_1600_1615.METER_MODEL_I_210));
                    //Event_1616_1631
                    AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.METER_MODEL_KV2C), m_usEvent1616_1631, (UInt16)(Event_1616_1631.METER_MODEL_KV2C));
                    AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.METER_INVERSION_DETECTED_ON_STARTUP), m_usEvent1616_1631, (UInt16)(Event_1616_1631.METER_INVERSION_DETECTED_ON_STARTUP));
                    AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.MODULE_POWER_FAIL), m_usEvent1616_1631, (UInt16)(Event_1616_1631.MODULE_POWER_FAIL));
                    AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.QUALIFIED_POWER_FAIL), m_usEvent1616_1631, (UInt16)(Event_1616_1631.QUALIFIED_POWER_FAIL));
                    AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.CLI_PASSWORD_FAILED), m_usEvent1616_1631, (UInt16)(Event_1616_1631.CLI_PASSWORD_FAILED));
                    AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.ICM_ENTERING_QUIET_MODE), m_usEvent1616_1631, (UInt16)(Event_1616_1631.ICM_ENTERING_QUIET_MODE));
                    AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.ICM_LEAVING_QUIET_MODE), m_usEvent1616_1631, (UInt16)(Event_1616_1631.ICM_LEAVING_QUIET_MODE));
                    AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.MAGNETIC_SWIPE_IN_MANUFACTURING_MODE), m_usEvent1616_1631, (UInt16)(Event_1616_1631.MAGNETIC_SWIPE_IN_MANUFACTURING_MODE));
                    AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.MAGNETIC_SWIPE_IN_PRODUCTION_MODE), m_usEvent1616_1631, (UInt16)(Event_1616_1631.MAGNETIC_SWIPE_IN_PRODUCTION_MODE));
                    AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.MAGNETIC_SWIPE_IGNORED_NOT_IN_NON_COMM), m_usEvent1616_1631, (UInt16)(Event_1616_1631.MAGNETIC_SWIPE_IGNORED_NOT_IN_NON_COMM));
                    AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.MAGNETIC_SWIPE_IGNORED_CONDITION_NOT_MET), m_usEvent1616_1631, (UInt16)(Event_1616_1631.MAGNETIC_SWIPE_IGNORED_CONDITION_NOT_MET));
                    AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.METER_LOGIN_VIA_OPTICAL_PORT), m_usEvent1616_1631, (UInt16)(Event_1616_1631.METER_LOGIN_VIA_OPTICAL_PORT));
                    AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.METER_LOGOFF_VIA_OPTICAL_PORT), m_usEvent1616_1631, (UInt16)(Event_1616_1631.METER_LOGOFF_VIA_OPTICAL_PORT));
                    AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.CLI_LOGIN_VIA_OPTICAL_PORT), m_usEvent1616_1631, (UInt16)(Event_1616_1631.CLI_LOGIN_VIA_OPTICAL_PORT));
                    //Event_1632_1647
                    AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.CLI_LOGIN_ATTEMPTED_VIA_OPTICAL_PORT), m_usEvent1632_1647, (UInt16)(Event_1632_1647.CLI_LOGIN_ATTEMPTED_VIA_OPTICAL_PORT));
                    AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.CLI_LOCKED_OUT_TOO_MANY_LOGIN_ATTEMPTS_VIA_OPTICAL_PORT), m_usEvent1632_1647, (UInt16)(Event_1632_1647.CLI_LOCKED_OUT_TOO_MANY_LOGIN_ATTEMPTS_VIA_OPTICAL_PORT));
                    AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.ZIGBEE_OPTICAL_PASSTHROUGH_STARTED), m_usEvent1632_1647, (UInt16)(Event_1632_1647.ZIGBEE_OPTICAL_PASSTHROUGH_STARTED));
                    AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.MODEM_OPTICAL_PASSTHROUGH_STARTED), m_usEvent1632_1647, (UInt16)(Event_1632_1647.MODEM_OPTICAL_PASSTHROUGH_STARTED));
                    AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.STARTED_C1218_SESSION_VIA_OPTICAL_PORT), m_usEvent1632_1647, (UInt16)(Event_1632_1647.STARTED_C1218_SESSION_VIA_OPTICAL_PORT));
                    AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.ICM_CONFIGURATION_CHANGE), m_usEvent1632_1647, (UInt16)(Event_1632_1647.ICM_CONFIGURATION_CHANGE));
                    AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.ICM_STATE_CHANGED), m_usEvent1632_1647, (UInt16)(Event_1632_1647.ICM_STATE_CHANGED));
                    AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.TIMESYNCH_STATE_CHANGED), m_usEvent1632_1647, (UInt16)(Event_1632_1647.TIMESYNCH_STATE_CHANGED));
                    AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.CLI_IS_DISABLED), m_usEvent1632_1647, (UInt16)(Event_1632_1647.CLI_IS_DISABLED));
                    AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.CLI_IS_REVERT_ONLY), m_usEvent1632_1647, (UInt16)(Event_1632_1647.CLI_IS_REVERT_ONLY));
                    AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.LOADSIDE_VOLTAGE_WHILE_SWITCH_OPEN), m_usEvent1632_1647, (UInt16)(Event_1632_1647.LOADSIDE_VOLTAGE_WHILE_SWITCH_OPEN));
                    AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.LOADSIDE_VOLTAGE_WHILE_SWITCH_OPEN_CLEAR), m_usEvent1632_1647, (UInt16)(Event_1632_1647.LOADSIDE_VOLTAGE_WHILE_SWITCH_OPEN_CLEAR));
                    //Event_1648_1663
                    AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.ACCUMULATOR_READ_FAILURE), m_usEvent1648_1663, (UInt16)(Event_1648_1663.ACCUMULATOR_READ_FAILURE));
                    AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.METER_MODEL_OW_CENTRON), m_usEvent1648_1663, (UInt16)(Event_1648_1663.METER_MODEL_OW_CENTRON));
                    AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.ERT_ERT_CC1121_MANUAL_CALIBRATION_SUCCESSFUL), m_usEvent1648_1663, (UInt16)(Event_1648_1663.ERT_ERT_CC1121_MANUAL_CALIBRATION_SUCCESSFUL));
                }

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // 4G (& 3G) meters 
                // Add Event_1536_1551 items
                AddEventItem(Enum.GetName(typeof(Event_1536_1551), Event_1536_1551.FAILURE_TO_SET_METER_TIME), m_usEvent1536_1551, (UInt16)(Event_1536_1551.FAILURE_TO_SET_METER_TIME));
                AddEventItem(Enum.GetName(typeof(Event_1536_1551), Event_1536_1551.METER_COMMUNICATION_FAULT), m_usEvent1536_1551, (UInt16)(Event_1536_1551.METER_COMMUNICATION_FAULT));
                AddEventItem(Enum.GetName(typeof(Event_1536_1551), Event_1536_1551.METER_COMMUNICATION_REESTABLISHED), m_usEvent1536_1551, (UInt16)(Event_1536_1551.METER_COMMUNICATION_REESTABLISHED));
                AddEventItem(Enum.GetName(typeof(Event_1536_1551), Event_1536_1551.POWER_OUTAGE_DETECTED), m_usEvent1536_1551, (UInt16)(Event_1536_1551.POWER_OUTAGE_DETECTED));
                AddEventItem(Enum.GetName(typeof(Event_1536_1551), Event_1536_1551.METER_ERROR_GREATER_THAN_MAXIMUM_CORRECTABLE_TIME_ERROR), m_usEvent1536_1551, (UInt16)(Event_1536_1551.METER_ERROR_GREATER_THAN_MAXIMUM_CORRECTABLE_TIME_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_1536_1551), Event_1536_1551.TAMPER_DETECTION_POWER_OUTAGES_RECOGNIZED_BY_SSI_MODULE), m_usEvent1536_1551, (UInt16)(Event_1536_1551.TAMPER_DETECTION_POWER_OUTAGES_RECOGNIZED_BY_SSI_MODULE));
                //Event_1552_1567
                AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.TAMPER_DETECTION_TILT_SWITCH_SET), m_usEvent1552_1567, (UInt16)(Event_1552_1567.TAMPER_DETECTION_TILT_SWITCH_SET));
                AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.TAMPER_DETECTION_TILT_SWITCH_CLEARED), m_usEvent1552_1567, (UInt16)(Event_1552_1567.TAMPER_DETECTION_TILT_SWITCH_CLEARED));
                AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.TAMPER_DETECTION_MODULE_INVERTED), m_usEvent1552_1567, (UInt16)(Event_1552_1567.TAMPER_DETECTION_MODULE_INVERTED));
                AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.EXTREME_TEMPERATURE_SHUTDOWN), m_usEvent1552_1567, (UInt16)(Event_1552_1567.EXTREME_TEMPERATURE_SHUTDOWN));
                AddEventItem(Enum.GetName(typeof(Event_1552_1567), Event_1552_1567.EXTREME_TEMPERATURE_INSERVICE), m_usEvent1552_1567, (UInt16)(Event_1552_1567.EXTREME_TEMPERATURE_INSERVICE));
                //Event_1568_1583
                AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.POWER_RESTORATION_DETECTED), m_usEvent1568_1583, (UInt16)(Event_1568_1583.POWER_RESTORATION_DETECTED));
                AddEventItem(Enum.GetName(typeof(Event_1568_1583), Event_1568_1583.DELAYED_RESET_SSI_MODULE_ALARM), m_usEvent1568_1583, (UInt16)(Event_1568_1583.DELAYED_RESET_SSI_MODULE_ALARM));
                //Event_1584_1599
                AddEventItem(Enum.GetName(typeof(Event_1584_1599), Event_1584_1599.METER_COMMUNICATION_STATUS_ALARM), m_usEvent1584_1599, (UInt16)(Event_1584_1599.METER_COMMUNICATION_STATUS_ALARM));
                //Event_1600_1615
                AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.COMM_LINK_FAILURE), m_usEvent1600_1615, (UInt16)(Event_1600_1615.COMM_LINK_FAILURE));
                AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.ICM_REBOOT_MODEM_NOT_RESPONDING), m_usEvent1600_1615, (UInt16)(Event_1600_1615.ICM_REBOOT_MODEM_NOT_RESPONDING));
                AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.CELLUAR_TIMEOUT_SENT), m_usEvent1600_1615, (UInt16)(Event_1600_1615.CELLUAR_TIMEOUT_SENT));
                AddEventItem(Enum.GetName(typeof(Event_1600_1615), Event_1600_1615.CELLUAR_TIMEOUT_RECEIVED), m_usEvent1600_1615, (UInt16)(Event_1600_1615.CELLUAR_TIMEOUT_RECEIVED));
                //Event_1616_1631
                AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.ICM_HEATER_ENABLED), m_usEvent1616_1631, (UInt16)(Event_1616_1631.ICM_HEATER_ENABLED));
                AddEventItem(Enum.GetName(typeof(Event_1616_1631), Event_1616_1631.ICM_HEATER_DISABLED), m_usEvent1616_1631, (UInt16)(Event_1616_1631.ICM_HEATER_DISABLED));
                //Event_1632_1647
                AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.CLI_COMMAND_EXECUTED_VIA_OPTICAL_PORT), m_usEvent1632_1647, (UInt16)(Event_1632_1647.CLI_COMMAND_EXECUTED_VIA_OPTICAL_PORT));
                AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.ICM_TIME_UPDATED_FROM_NETWORK), m_usEvent1632_1647, (UInt16)(Event_1632_1647.ICM_TIME_UPDATED_FROM_NETWORK));
                AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.ICM_TIME_SET_FROM_METER), m_usEvent1632_1647, (UInt16)(Event_1632_1647.ICM_TIME_SET_FROM_METER));
                AddEventItem(Enum.GetName(typeof(Event_1632_1647), Event_1632_1647.CELL_TOWER_CHANGES_EXCEED_THRESHOLD), m_usEvent1632_1647, (UInt16)(Event_1632_1647.CELL_TOWER_CHANGES_EXCEED_THRESHOLD));
                //Event_1648_1663
                AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.SECTOR_CHANGES_EXCEED_THRESHOLD), m_usEvent1648_1663, (UInt16)(Event_1648_1663.SECTOR_CHANGES_EXCEED_THRESHOLD));
                AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.CELLULAR_CONNECTION_TIMEOUT_ALARM), m_usEvent1648_1663, (UInt16)(Event_1648_1663.CELLULAR_CONNECTION_TIMEOUT_ALARM));
                AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.SMS_WAKEUP_RECEIVED), m_usEvent1648_1663, (UInt16)(Event_1648_1663.SMS_WAKEUP_RECEIVED));
                AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.ERT_SUCCESSFULLY_INITIALIZED_SPI_DRIVER), m_usEvent1648_1663, (UInt16)(Event_1648_1663.ERT_SUCCESSFULLY_INITIALIZED_SPI_DRIVER));
                AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.ERT_CC1121_PART_NUMBER), m_usEvent1648_1663, (UInt16)(Event_1648_1663.ERT_CC1121_PART_NUMBER));
                AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.ERT_ERT_HW_SUCCESSFULLY_INITIALIZED), m_usEvent1648_1663, (UInt16)(Event_1648_1663.ERT_ERT_HW_SUCCESSFULLY_INITIALIZED));
                AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.ERT_ERT_RADIO_TURNED_OFF_IN_ERT_CFG_DATA_TABLE), m_usEvent1648_1663, (UInt16)(Event_1648_1663.ERT_ERT_RADIO_TURNED_OFF_IN_ERT_CFG_DATA_TABLE));
                AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.ERT_ERT_MASTER_LIST_INITIALIZATION_CREATION_FAILED), m_usEvent1648_1663, (UInt16)(Event_1648_1663.ERT_ERT_MASTER_LIST_INITIALIZATION_CREATION_FAILED));
                AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.ERT_ICM_CAMPING_CHANNEL), m_usEvent1648_1663, (UInt16)(Event_1648_1663.ERT_ICM_CAMPING_CHANNEL));
                AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.ERT_ADDING_AN_ERT_METER_READING_TO_OUR_MASTER_LIST), m_usEvent1648_1663, (UInt16)(Event_1648_1663.ERT_ADDING_AN_ERT_METER_READING_TO_OUR_MASTER_LIST));
                AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.ERT_REACHED_MAX_NUMBER_OF_ERT_METERS), m_usEvent1648_1663, (UInt16)(Event_1648_1663.ERT_REACHED_MAX_NUMBER_OF_ERT_METERS));
                AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.ERT_INCOMING_ERT_PACKET_CRC_ERROR), m_usEvent1648_1663, (UInt16)(Event_1648_1663.ERT_INCOMING_ERT_PACKET_CRC_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_1648_1663), Event_1648_1663.ERT_CHANGING_ERT_RESTING_CHANNEL), m_usEvent1648_1663, (UInt16)(Event_1648_1663.ERT_CHANGING_ERT_RESTING_CHANNEL));
                //Event_1664_1679
                AddEventItem(Enum.GetName(typeof(Event_1664_1679), Event_1664_1679.ERT_ERT_RADIO_OFF_FREEZE_PROC_IMC_REJECTED), m_usEvent1664_1679, (UInt16)(Event_1664_1679.ERT_ERT_RADIO_OFF_FREEZE_PROC_IMC_REJECTED));
                AddEventItem(Enum.GetName(typeof(Event_1664_1679), Event_1664_1679.ERT_NUMBER_OF_ERTS_WHOSE_RECORDS_WERE_FROZEN), m_usEvent1664_1679, (UInt16)(Event_1664_1679.ERT_NUMBER_OF_ERTS_WHOSE_RECORDS_WERE_FROZEN));
                AddEventItem(Enum.GetName(typeof(Event_1664_1679), Event_1664_1679.ERT_RECEIVED_INVALID_TIME_FROM_NTP_TASK), m_usEvent1664_1679, (UInt16)(Event_1664_1679.ERT_RECEIVED_INVALID_TIME_FROM_NTP_TASK));
                AddEventItem(Enum.GetName(typeof(Event_1664_1679), Event_1664_1679.ERT_UNSUCCESSFUL_PDOID_READ), m_usEvent1664_1679, (UInt16)(Event_1664_1679.ERT_UNSUCCESSFUL_PDOID_READ));
                AddEventItem(Enum.GetName(typeof(Event_1664_1679), Event_1664_1679.ERT_242_TX_FAILED), m_usEvent1664_1679, (UInt16)(Event_1664_1679.ERT_242_TX_FAILED));
                AddEventItem(Enum.GetName(typeof(Event_1664_1679), Event_1664_1679.ADDED_ERT_TO_MANAGED_LIST), m_usEvent1664_1679, (UInt16)(Event_1664_1679.ADDED_ERT_TO_MANAGED_LIST));
                AddEventItem(Enum.GetName(typeof(Event_1664_1679), Event_1664_1679.ERT_TIME_SET_FAILED), m_usEvent1664_1679, (UInt16)(Event_1664_1679.ERT_TIME_SET_FAILED));
                AddEventItem(Enum.GetName(typeof(Event_1664_1679), Event_1664_1679.ICM_EVENT_LOG_CLEARED), m_usEvent1664_1679, (UInt16)(Event_1664_1679.ICM_EVENT_LOG_CLEARED));
                AddEventItem(Enum.GetName(typeof(Event_1664_1679), Event_1664_1679.TAMPER_TILT_SET_ON_OUTAGE), m_usEvent1664_1679, (UInt16)(Event_1664_1679.TAMPER_TILT_SET_ON_OUTAGE));
                //Event_1680_1695
                AddEventItem(Enum.GetName(typeof(Event_1680_1695), Event_1680_1695.ERT_METER_STOLEN), m_usEvent1680_1695, (UInt16)(Event_1680_1695.ERT_METER_STOLEN));
                AddEventItem(Enum.GetName(typeof(Event_1680_1695), Event_1680_1695.ERT_METER_REMOVED), m_usEvent1680_1695, (UInt16)(Event_1680_1695.ERT_METER_REMOVED));
                AddEventItem(Enum.GetName(typeof(Event_1680_1695), Event_1680_1695.ERT_CONNECTION_DOWNTIME_TIME_EXCEEDED), m_usEvent1680_1695, (UInt16)(Event_1680_1695.ERT_CONNECTION_DOWNTIME_TIME_EXCEEDED));
                AddEventItem(Enum.GetName(typeof(Event_1680_1695), Event_1680_1695.ERT_PREDICTOR_LIST_TIME_MODIFIED), m_usEvent1680_1695, (UInt16)(Event_1680_1695.ERT_PREDICTOR_LIST_TIME_MODIFIED));
                AddEventItem(Enum.GetName(typeof(Event_1680_1695), Event_1680_1695.ERT_MANUAL_CALIBRATION_FAILED), m_usEvent1680_1695, (UInt16)(Event_1680_1695.ERT_MANUAL_CALIBRATION_FAILED));
                AddEventItem(Enum.GetName(typeof(Event_1680_1695), Event_1680_1695.ERT_100G_TRACKING_FAILED), m_usEvent1680_1695, (UInt16)(Event_1680_1695.ERT_100G_TRACKING_FAILED));
                //Event_1696_1711
                AddEventItem(Enum.GetName(typeof(Event_1696_1711), Event_1696_1711.TIME_SOURCE_UNAVAILABLE), m_usEvent1696_1711 , (UInt16)(Event_1696_1711.TIME_SOURCE_UNAVAILABLE));

                return m_lstEvents;
            }
        }

        /// <summary>
        /// Returns a list of monitored events with their descriptions.  Designed to provide
        /// the same type of information as MFGTable2048.HistoryLogConfig.HistoryLogEventList
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //
        public List<MFG2048EventItem> ICSHistoryLogMonitoredEventList
        {
            get
            {
                List<EventEntry> lstMonitoredEvents = new List<EventEntry>();

                ReadUnloadedTable();

                m_lstEvents.Clear();

                AddMonitoredEvents(ref lstMonitoredEvents, m_abyStdEventsMonitored, false);
                AddMonitoredEvents(ref lstMonitoredEvents, m_abyMfgEventsMonitored, true);

                foreach (EventEntry entry in lstMonitoredEvents)
                {
                    AddEventItem(entry);
                }

                return m_lstEvents;
            }
        }

        /// <summary>
        /// Returns a list of supported user events with their descriptions and whether 
        /// or not they are monitored.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/13 jrf 2.80.37 TQ8280 Created
        //
        public IEnumerable<MFG2048EventItem> ICSHistoryLogSupportedUserEventList
        {
            get
            {
                return ICSHistoryLogEventList.Where(e => ICS_USER_EVENTS.Contains(e.ID));
            }
        }

        /// <summary>
        /// Returns a list of supported non-user events with their descriptions and whether 
        /// or not they are monitored.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/10/13 jrf 2.80.37 TQ8280 Created
        //
        public IEnumerable<MFG2048EventItem> ICSHistoryLogSupportedNonUserEventList
        {
            get
            {
                return ICSHistoryLogEventList.Where(e => ICS_NON_USER_EVENTS.Contains(e.ID));
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper method to determine the length of the table.
        /// </summary>
        /// <param name="table2521">ICM MFG table 2521</param>
        /// <returns>length in bytes of table 73</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //
        static private uint GetTableLength(ICSMfgTable2521 table2521)
        {
            uint uiTableLength = table2521.NumberStandardEvents + table2521.NumberManufacturerEvents;
            return uiTableLength;
        }

        /// <summary>
        /// Gets the data out of the binary reader and into the member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  08/01/16 AF  4.60.02 623194 Repositioned the data stream position to read the table again and
        //                              put the data into individual member variables.
        //
        private void ParseData()
        {
            m_abyStdEventsMonitored = m_Reader.ReadBytes((int)(m_ICSMfgTable2521.NumberStandardEvents));
            m_abyMfgEventsMonitored = m_Reader.ReadBytes((int)(m_ICSMfgTable2521.NumberManufacturerEvents));

            // Reposition the data stream position so that we can get the data into the other member variables
            m_Reader.BaseStream.Seek(192, SeekOrigin.Begin);

            m_usEvent1536_1551 = m_Reader.ReadUInt16();
            m_usEvent1552_1567 = m_Reader.ReadUInt16();
            m_usEvent1568_1583 = m_Reader.ReadUInt16();
            m_usEvent1584_1599 = m_Reader.ReadUInt16();
            m_usEvent1600_1615 = m_Reader.ReadUInt16();
            m_usEvent1616_1631 = m_Reader.ReadUInt16();
            m_usEvent1632_1647 = m_Reader.ReadUInt16();
            m_usEvent1648_1663 = m_Reader.ReadUInt16();
            m_usEvent1664_1679 = m_Reader.ReadUInt16();
            m_usEvent1680_1695 = m_Reader.ReadUInt16();
            m_usEvent1696_1711 = m_Reader.ReadUInt16();
            // Skip forward to the next supported event - I210 and kV2C events
            //m_DataStream.Position += 34;
            //m_usEvent1984_1999 = m_Reader.ReadUInt16();
            //m_usEvent2000_2015 = m_Reader.ReadUInt16();
        }

        /// <summary>
        /// Produces a list of supported events from the information read from standard
        /// tables 72 and 73
        /// </summary>
        /// <param name="SupportedEvents">The list being constructed</param>
        /// <param name="StdOrMfgEventsSupported">
        /// Byte array, each bit of which represents a standard or manufacturers event.
        /// The value of the bit is 1 if the event is supported; 0, if not.
        /// </param>
        /// <param name="StdOrMfgEventsMonitored">
        /// Byte array, each bit of which represents a std or mfg event.
        /// The value of the bit is 1 if the event is monitored; 0, if not.
        /// </param>
        /// <param name="bIsMfgEvent">
        /// Flag to let us know to add 2048 to the event id.
        /// </param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  07/01/13 AF  2.80.45 TR7640 Corrected the calculation of the event number
        //
        private void AddSupportedEvents(ref List<EventEntry> SupportedEvents, byte[] StdOrMfgEventsSupported, byte[] StdOrMfgEventsMonitored, bool bIsMfgEvent)
        {
            byte byBitMask;
            UInt16 iEventNumber;

            for (int iIndex = 0; iIndex < StdOrMfgEventsSupported.Length; iIndex++)
            {
                for (int iBitIndex = 0; iBitIndex < 8; iBitIndex++)
                {
                    EventEntry Event = new EventEntry();

                    iEventNumber = (UInt16)(8 * iIndex + iBitIndex);

                    byBitMask = (byte)(1 << iBitIndex);

                    if ((StdOrMfgEventsSupported[iIndex] & byBitMask) == byBitMask)
                    {
                        // event is supported - add to the event list
                        Event.HistoryCode = (ushort)(iEventNumber + 0x800);
                        // check to see if it is monitored
                        if ((StdOrMfgEventsMonitored[iIndex] & byBitMask) == byBitMask)
                        {
                            Event.Monitored = true;
                        }
                        else
                        {
                            Event.Monitored = false;
                        }

                        SupportedEvents.Add(Event);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MonitoredEvents"></param>
        /// <param name="StdOrMfgEventsMonitored"></param>
        /// <param name="bIsMfgEvent"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //
        private void AddMonitoredEvents(ref List<EventEntry> MonitoredEvents, byte[] StdOrMfgEventsMonitored, bool bIsMfgEvent)
        {
            byte byBitMask;
            UInt16 iEventNumber;

            for (int iIndex = 0; iIndex < StdOrMfgEventsMonitored.Length; iIndex++)
            {
                for (int iBitIndex = 0; iBitIndex < 8; iBitIndex++)
                {
                    iEventNumber = (UInt16)(8 * iIndex + iBitIndex);

                    if (bIsMfgEvent)
                    {
                        iEventNumber += 2048;
                    }
                    byBitMask = (byte)(1 << iBitIndex);

                    // check to see if it is monitored
                    if ((StdOrMfgEventsMonitored[iIndex] & byBitMask) == byBitMask)
                    {
                        EventEntry Event = new EventEntry();
                        Event.HistoryCode = iEventNumber;
                        Event.Monitored = true;
                        MonitoredEvents.Add(Event);
                    }
                }
            }
        }

        /// <summary>
        /// Adds an event item to the list.
        /// </summary>
        /// <param name="strResourceString">The description of the event</param>
        /// <param name="usEventField">The raw data from the meter</param>
        /// <param name="usEventMask">The mask to apply to determine whether or not
        /// the event is enabled</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  08/01/16 AF  4.60.02  WR 623194  Created
        //
        protected void AddEventItem(string strResourceString, UInt16 usEventField, UInt16 usEventMask)
        {
            MFG2048EventItem eventItem = GetEventItem(strResourceString, usEventField, usEventMask);
            m_lstEvents.Add(eventItem);
        }

        /// <summary>
        /// Gets the Event Item
        /// </summary>
        /// <param name="strResourceString"> The Description of the Event</param>
        /// <param name="usEventField">The raw data from the device</param>
        /// <param name="usEventMask">The mast to apply to determ if the event is enabled</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  08/01/16 AF  4.60.02  WR 623194  Created
        //
        protected MFG2048EventItem GetEventItem(string strResourceString, UInt16 usEventField, UInt16 usEventMask)
        {
            MFG2048EventItem eventItem = new MFG2048EventItem();
            eventItem.Description = m_rmStrings.GetString(strResourceString);
            eventItem.Enabled = false;
            if (0 != (usEventField & usEventMask))
            {
                eventItem.Enabled = true;
            }

            return eventItem;
        }

        /// <summary>
        /// Takes an event entry item, translates the event id into a text description 
        /// and adds it to the MFG2048EventItem list
        /// </summary>
        /// <param name="entry"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  06/24/13 AF  2.80.41 TR7586 Added more events
        //  07/03/13 AF  2.80.45 TR7586 Added one more event
        //  07/11/13 AF  2.80.51 WR417110 Updated with more events
        //  04/07/14 AF  3.50.62 WR466261 Updated with more events
        //  04/24/14 AF  3.50.83 WR488012 Changes to align the event descriptions with the CE UI
        //  05/16/14 jrf 3.50.95 WR 516785 Making changes to add two new events.
        //  10/27/14 AF  4.00.80 WR503425 Added key management security success events
        //  04/05/16 CFB 4.50.244 WR603380 Modified names of several 100G specific meter events
        //                                to generic names to match CE
        //  09/02/16 AF  4.70.16           Corrected merge errors
        //
        private void AddEventItem(EventEntry entry)
        {
            MFG2048EventItem eventItem = new MFG2048EventItem();
            eventItem.ID = entry.HistoryCode;
            switch (entry.HistoryCode)
            {
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.UNINTELLIGIBLE_MESSAGE_RECEIVED:
                {
                    eventItem.Description = m_rmStrings.GetString("UNINTELLIGIBLE_MESSAGE_RECEIVED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FAILURE_TO_SET_METER_TIME:
                {
                    eventItem.Description = m_rmStrings.GetString("FAILURE_TO_SET_METER_TIME");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SYNCHRONIZATION_TIME_ERROR_IS_TOO_LARGE:
                {
                    eventItem.Description = m_rmStrings.GetString("SYNCHRONIZATION_TIME_ERROR_IS_TOO_LARGE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_COMMUNICATION_FAULT:
                {
                    eventItem.Description = m_rmStrings.GetString("METER_COMMUNICATION_FAULT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_COMMUNICATION_REESTABLISHED:
                {
                    eventItem.Description = m_rmStrings.GetString("METER_COMMUNICATION_REESTABLISHED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_OUTAGE_DETECTED:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_OUTAGE_DETECTED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_ERROR_GREATER_THAN_MAXIMUM_CORRECTABLE_TIME_ERROR:
                {
                    eventItem.Description = m_rmStrings.GetString("METER_ERROR_GREATER_THAN_MAXIMUM_CORRECTABLE_TIME_ERROR");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_PASSWORD_RECOVERY_DETECTED:
                {
                    eventItem.Description = m_rmStrings.GetString("TAMPER_DETECTION_PASSWORD_RECOVERY_DETECTED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_TOTAL_DEMAND_RESETS:
                {
                    eventItem.Description = m_rmStrings.GetString("TAMPER_DETECTION_TOTAL_DEMAND_RESETS");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_OPTICAL_PORT_SESSION_IN_PROGRESS:
                {
                    eventItem.Description = m_rmStrings.GetString("TAMPER_DETECTION_OPTICAL_PORT_SESSION_IN_PROGRESS");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_DEVICE_RECONFIGURED_REPROGRAMMED:
                {
                    eventItem.Description = m_rmStrings.GetString("TAMPER_DETECTION_DEVICE_RECONFIGURED_REPROGRAMMED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_SERVICE_ERROR_DETECTED:
                {
                    eventItem.Description = m_rmStrings.GetString("TAMPER_DETECTION_SERVICE_ERROR_DETECTED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_PASSWORD_FAILURE:
                {
                    eventItem.Description = m_rmStrings.GetString("TAMPER_DETECTION_PASSWORD_FAILURE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_POWER_OUTAGES_RECOGNIZED_BY_SSI_MODULE:
                {
                    eventItem.Description = m_rmStrings.GetString("TAMPER_DETECTION_POWER_OUTAGES_RECOGNIZED_BY_SSI_MODULE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_POWER_OUTAGES_DUE_TO_LOSS_OF_AC_POWER:
                {
                    eventItem.Description = m_rmStrings.GetString("TAMPER_DETECTION_POWER_OUTAGES_DUE_TO_LOSS_OF_AC_POWER");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_TILT_SWITCH_SET:
                {
                    eventItem.Description = m_rmStrings.GetString("TAMPER_DETECTION_TILT_SWITCH_SET");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_TILT_SWITCH_CLEARED:
                {
                    eventItem.Description = m_rmStrings.GetString("TAMPER_DETECTION_TILT_SWITCH_CLEARED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_REMOTE_DISCONNECT_SWITCH_BYPASS:
                {
                    eventItem.Description = m_rmStrings.GetString("TAMPER_DETECTION_REMOTE_DISCONNECT_SWITCH_BYPASS");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_DETECTION_MODULE_INVERTED:
                {
                    eventItem.Description = m_rmStrings.GetString("TAMPER_DETECTION_MODULE_INVERTED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.EXTREME_TEMPERATURE_SHUTDOWN:
                {
                    eventItem.Description = m_rmStrings.GetString("EXTREME_TEMPERATURE_SHUTDOWN");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.EXTREME_TEMPERATURE_INSERVICE:
                {
                    eventItem.Description = m_rmStrings.GetString("EXTREME_TEMPERATURE_INSERVICE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_IMAGE_CORRUPTED:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_IMAGE_CORRUPTED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_QUALITY_DETECTION_DIAGNOSTIC_1:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_QUALITY_DETECTION_DIAGNOSTIC_1");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_QUALITY_DETECTION_DIAGNOSTIC_2:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_QUALITY_DETECTION_DIAGNOSTIC_2");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_QUALITY_DETECTION_DIAGNOSTIC_3:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_QUALITY_DETECTION_DIAGNOSTIC_3");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_QUALITY_DETECTION_DIAGNOSTIC_4:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_QUALITY_DETECTION_DIAGNOSTIC_4");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_QUALITY_DETECTION_DIAGNOSTIC_5:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_QUALITY_DETECTION_DIAGNOSTIC_5");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_QUALITY_DETECTION_DIAGNOSTIC_6:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_QUALITY_DETECTION_DIAGNOSTIC_6");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_QUALITY_DETECTION_DIAGNOSTIC_7:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_QUALITY_DETECTION_DIAGNOSTIC_7");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_QUALITY_DETECTION_DIAGNOSTIC_8:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_QUALITY_DETECTION_DIAGNOSTIC_8");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_QUALITY_DETECTION_VOLTAGE_SAG:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_QUALITY_DETECTION_VOLTAGE_SAG");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_QUALITY_DETECTION_VOLTAGE_SWELL:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_QUALITY_DETECTION_VOLTAGE_SWELL");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_PASSWORDS_OUT_OF_SYNC:
                {
                    eventItem.Description = m_rmStrings.GetString("METER_PASSWORDS_OUT_OF_SYNC");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.RCDC_FAULT_RCDC_COMM_ERROR:
                {
                    eventItem.Description = m_rmStrings.GetString("RCDC_FAULT_RCDC_COMM_ERROR");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.RCDC_FAULT_SWITCH_CONTROLLER_ERROR:
                {
                    eventItem.Description = m_rmStrings.GetString("RCDC_FAULT_SWITCH_CONTROLLER_ERROR");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.RCDC_FAULT_SWITCHED_FAILED_TO_CLOSE:
                {
                    eventItem.Description = m_rmStrings.GetString("RCDC_FAULT_SWITCHED_FAILED_TO_CLOSE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.RCDC_FAULT_ALTERNATE_SOURCE:
                {
                    eventItem.Description = m_rmStrings.GetString("RCDC_FAULT_ALTERNATE_SOURCE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.RCDC_FAULT_BYPASSED:
                {
                    eventItem.Description = m_rmStrings.GetString("RCDC_FAULT_BYPASSED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.RCDC_FAULT_SWITCH_FAILED_TO_OPEN:
                {
                    eventItem.Description = m_rmStrings.GetString("RCDC_FAULT_SWITCH_FAILED_TO_OPEN");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.RCDC_FAULT_PPM_ALERT:
                {
                    eventItem.Description = m_rmStrings.GetString("RCDC_FAULT_PPM_ALERT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.RCDC_FAULT_MANUAL_ARM_TIMED_OUT:
                {
                    eventItem.Description = m_rmStrings.GetString("RCDC_FAULT_MANUAL_ARM_TIMED_OUT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.AUTO_REGISTRATION:
                {
                    eventItem.Description = m_rmStrings.GetString("AUTO_REGISTRATION");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SUCCESSFUL_TIME_SYNC_TIME_CHANGE_OCCURRED:
                {
                    eventItem.Description = m_rmStrings.GetString("SUCCESSFUL_TIME_SYNC_TIME_CHANGE_OCCURRED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_RESTORATION_DETECTED:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_RESTORATION_DETECTED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.IP_ADDRESS_REPORT:
                {
                    eventItem.Description = m_rmStrings.GetString("IP_ADDRESS_REPORT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.DELAYED_RESET_SSI_MODULE_ALARM:
                {
                    eventItem.Description = m_rmStrings.GetString("DELAYED_RESET_SSI_MODULE_ALARM");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_COMMUNICATION_STATUS_ALARM:
                {
                    eventItem.Description = m_rmStrings.GetString("METER_COMMUNICATION_STATUS_ALARM");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.EXTENSIBLE_FIRMWARE_DOWNLOAD_STATUS:
                {
                    eventItem.Description = m_rmStrings.GetString("EXTENSIBLE_FIRMWARE_DOWNLOAD_STATUS");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SET_ACTIVE_FIRMWARE_ALARM:
                {
                    eventItem.Description = m_rmStrings.GetString("SET_ACTIVE_FIRMWARE_ALARM");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_UPGRADE_DOWNLOAD_ALARM:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_UPGRADE_DOWNLOAD_ALARM");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_UPGRADE_ACTIVE_ALARM:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_UPGRADE_ACTIVE_ALARM");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_COPYING_FILE:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_DOWNLOAD_COPYING_FILE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_CANCELING:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_DOWNLOAD_CANCELING");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_CANCELED:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_DOWNLOAD_CANCELED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_TOTAL_TIME:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_DOWNLOAD_TOTAL_TIME");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_SUCCESSFUL:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_DOWNLOAD_SUCCESSFUL");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_REMOVING_INACTIVE_FILES:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_DOWNLOAD_REMOVING_INACTIVE_FILES");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_RETRIES_EXCEEDED:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_DOWNLOAD_RETRIES_EXCEEDED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_FAILED_WILL_RETRY:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_DOWNLOAD_FAILED_WILL_RETRY");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_INCORRECT_VERSION:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_DOWNLOAD_INCORRECT_VERSION");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_FILE_EXISTS:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_DOWNLOAD_FILE_EXISTS");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_ACTIVATING:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_DOWNLOAD_ACTIVATING");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_SET_ACTIVE_REBOOT:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_DOWNLOAD_SET_ACTIVE_REBOOT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.COMM_LINK_FAILURE:
                {
                    eventItem.Description = m_rmStrings.GetString("COMM_LINK_FAILURE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ICM_REBOOT_MODEM_NOT_RESPONDING:
                {
                    eventItem.Description = m_rmStrings.GetString("ICM_REBOOT_MODEM_NOT_RESPONDING");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.INITIAL_MODEM_PROVISION:
                {
                    eventItem.Description = m_rmStrings.GetString("INITIAL_MODEM_PROVISION");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.MODEM_PROVISION_FAILED:
                {
                    eventItem.Description = m_rmStrings.GetString("MODEM_PROVISION_FAILED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.MODEM_PROVISION_SUCCESSFUL:
                {
                    eventItem.Description = m_rmStrings.GetString("MODEM_PROVISION_SUCCESSFUL");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.MODEM_IDENTITY_ERROR:
                {
                    eventItem.Description = m_rmStrings.GetString("MODEM_IDENTITY_ERROR");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CDMA_SUBSCRIPTION_ERROR:
                {
                    eventItem.Description = m_rmStrings.GetString("CDMA_SUBSCRIPTION_ERROR");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.MDN_LOGIN_ERROR:
                {
                    eventItem.Description = m_rmStrings.GetString("MDN_LOGIN_ERROR");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.RECEIVED_SMS:
                {
                    eventItem.Description = m_rmStrings.GetString("RECEIVED_SMS");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.GATEWAY_CHANGED:
                {
                    eventItem.Description = m_rmStrings.GetString("GATEWAY_CHANGED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CELLUAR_TIMEOUT_SENT:
                {
                    eventItem.Description = m_rmStrings.GetString("CELLUAR_TIMEOUT_SENT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CELLUAR_TIMEOUT_RECEIVED:
                {
                    eventItem.Description = m_rmStrings.GetString("CELLUAR_TIMEOUT_RECEIVED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.UNKNOWN_METER_TYPE:
                {
                    eventItem.Description = m_rmStrings.GetString("UNKNOWN_METER_TYPE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_MODEL_I_210C:
                {
                    eventItem.Description = m_rmStrings.GetString("METER_MODEL_I_210C");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_MODEL_I_210:
                {
                    eventItem.Description = m_rmStrings.GetString("METER_MODEL_I_210");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_MODEL_KV2C:
                {
                    eventItem.Description = m_rmStrings.GetString("METER_MODEL_KV2C");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_INVERSION_DETECTED_ON_STARTUP:
                {
                    eventItem.Description = m_rmStrings.GetString("METER_INVERSION_DETECTED_ON_STARTUP");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.MODULE_POWER_FAIL:
                {
                    eventItem.Description = m_rmStrings.GetString("MODULE_POWER_FAIL");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.QUALIFIED_POWER_FAIL:
                {
                    eventItem.Description = m_rmStrings.GetString("QUALIFIED_POWER_FAIL");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ICM_HEATER_ENABLED:
                {
                    eventItem.Description = m_rmStrings.GetString("ICM_HEATER_ENABLED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ICM_HEATER_DISABLED:
                {
                    eventItem.Description = m_rmStrings.GetString("ICM_HEATER_DISABLED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CLI_PASSWORD_FAILED:
                {
                    eventItem.Description = m_rmStrings.GetString("CLI_PASSWORD_FAILED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ICM_ENTERING_QUIET_MODE:
                {
                    eventItem.Description = m_rmStrings.GetString("ICM_ENTERING_QUIET_MODE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ICM_LEAVING_QUIET_MODE:
                {
                    eventItem.Description = m_rmStrings.GetString("ICM_LEAVING_QUIET_MODE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.MAGNETIC_SWIPE_IN_MANUFACTURING_MODE:
                {
                    eventItem.Description = m_rmStrings.GetString("MAGNETIC_SWIPE_IN_MANUFACTURING_MODE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.MAGNETIC_SWIPE_IN_PRODUCTION_MODE:
                {
                    eventItem.Description = m_rmStrings.GetString("MAGNETIC_SWIPE_IN_PRODUCTION_MODE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.MAGNETIC_SWIPE_IGNORED_NOT_IN_NON_COMM:
                {
                    eventItem.Description = m_rmStrings.GetString("MAGNETIC_SWIPE_IGNORED_NOT_IN_NON_COMM");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.MAGNETIC_SWIPE_IGNORED_CONDITION_NOT_MET:
                {
                    eventItem.Description = m_rmStrings.GetString("MAGNETIC_SWIPE_IGNORED_CONDITION_NOT_MET");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_LOGIN_VIA_OPTICAL_PORT:
                {
                    eventItem.Description = m_rmStrings.GetString("METER_LOGIN_VIA_OPTICAL_PORT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_LOGOFF_VIA_OPTICAL_PORT:
                {
                    eventItem.Description = m_rmStrings.GetString("METER_LOGOFF_VIA_OPTICAL_PORT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CLI_LOGIN_VIA_OPTICAL_PORT:
                {
                    eventItem.Description = m_rmStrings.GetString("CLI_LOGIN_VIA_OPTICAL_PORT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CLI_LOGIN_ATTEMPTED_VIA_OPTICAL_PORT:
                {
                    eventItem.Description = m_rmStrings.GetString("CLI_LOGIN_ATTEMPTED_VIA_OPTICAL_PORT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CLI_COMMAND_EXECUTED_VIA_OPTICAL_PORT:
                {
                    eventItem.Description = m_rmStrings.GetString("CLI_COMMAND_EXECUTED_VIA_OPTICAL_PORT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CLI_LOCKED_OUT_TOO_MANY_LOGIN_ATTEMPTS_VIA_OPTICAL_PORT:
                {
                    eventItem.Description = m_rmStrings.GetString("CLI_LOCKED_OUT_TOO_MANY_LOGIN_ATTEMPTS_VIA_OPTICAL_PORT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ZIGBEE_OPTICAL_PASSTHROUGH_STARTED:
                {
                    eventItem.Description = m_rmStrings.GetString("ZIGBEE_OPTICAL_PASSTHROUGH_STARTED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.MODEM_OPTICAL_PASSTHROUGH_STARTED:
                {
                    eventItem.Description = m_rmStrings.GetString("MODEM_OPTICAL_PASSTHROUGH_STARTED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.STARTED_C1218_SESSION_VIA_OPTICAL_PORT:
                {
                    eventItem.Description = m_rmStrings.GetString("STARTED_C1218_SESSION_VIA_OPTICAL_PORT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ICM_CONFIGURATION_CHANGE:
                {
                    eventItem.Description = m_rmStrings.GetString("ICM_CONFIGURATION_CHANGE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ICM_STATE_CHANGED:
                {
                    eventItem.Description = m_rmStrings.GetString("ICM_STATE_CHANGED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ICM_TIME_UPDATED_FROM_NETWORK:
                {
                    eventItem.Description = m_rmStrings.GetString("ICM_TIME_UPDATED_FROM_NETWORK");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ICM_TIME_SET_FROM_METER:
                {
                    eventItem.Description = m_rmStrings.GetString("ICM_TIME_SET_FROM_METER");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TIMESYNCH_STATE_CHANGED:
                {
                    eventItem.Description = m_rmStrings.GetString("TIMESYNCH_STATE_CHANGED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CLI_IS_DISABLED:
                {
                    eventItem.Description = m_rmStrings.GetString("CLI_IS_DISABLED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CLI_IS_REVERT_ONLY:
                {
                    eventItem.Description = m_rmStrings.GetString("CLI_IS_REVERT_ONLY");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.LOADSIDE_VOLTAGE_WHILE_SWITCH_OPEN:
                {
                    eventItem.Description = m_rmStrings.GetString("LOADSIDE_VOLTAGE_WHILE_SWITCH_OPEN");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.LOADSIDE_VOLTAGE_WHILE_SWITCH_OPEN_CLEAR:
                {
                    eventItem.Description = m_rmStrings.GetString("LOADSIDE_VOLTAGE_WHILE_SWITCH_OPEN_CLEAR");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CELL_TOWER_CHANGES_EXCEED_THRESHOLD:
                {
                    eventItem.Description = m_rmStrings.GetString("CELL_TOWER_CHANGES_EXCEED_THRESHOLD");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SECTOR_CHANGES_EXCEED_THRESHOLD:
                {
                    eventItem.Description = m_rmStrings.GetString("SECTOR_CHANGES_EXCEED_THRESHOLD");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ACCUMULATOR_READ_FAILURE:
                {
                    eventItem.Description = m_rmStrings.GetString("ACCUMULATOR_READ_FAILURE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METER_MODEL_OW_CENTRON:
                {
                    eventItem.Description = m_rmStrings.GetString("METER_MODEL_OW_CENTRON");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SMS_WAKEUP_RECEIVED:
                {
                    eventItem.Description = m_rmStrings.GetString("SMS_WAKEUP_RECEIVED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CELLULAR_CONNECTION_TIMEOUT_ALARM:
                {
                    eventItem.Description = m_rmStrings.GetString("CELLULAR_CONNECTION_TIMEOUT_ALARM");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_CC1121_PART_NUMBER:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_CC1121_PART_NUMBER");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_SUCCESSFULLY_INITIALIZED_SPI_DRIVER:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_SUCCESSFULLY_INITIALIZED_SPI_DRIVER");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_ERT_HW_SUCCESSFULLY_INITIALIZED:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_ERT_HW_SUCCESSFULLY_INITIALIZED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_ERT_RADIO_TURNED_OFF_IN_ERT_CFG_DATA_TABLE:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_ERT_RADIO_TURNED_OFF_IN_ERT_CFG_DATA_TABLE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_ERT_CC1121_MANUAL_CALIBRATION_SUCCESSFUL:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_ERT_CC1121_MANUAL_CALIBRATION_SUCCESSFUL");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_ERT_MASTER_LIST_INITIALIZATION_CREATION_FAILED:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_ERT_MASTER_LIST_INITIALIZATION_CREATION_FAILED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_ICM_CAMPING_CHANNEL:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_ICM_CAMPING_CHANNEL");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_ADDING_AN_ERT_METER_READING_TO_OUR_MASTER_LIST:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_ADDING_AN_ERT_METER_READING_TO_OUR_MASTER_LIST");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_REACHED_MAX_NUMBER_OF_ERT_METERS:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_REACHED_MAX_NUMBER_OF_ERT_METERS");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_INCOMING_ERT_PACKET_CRC_ERROR:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_INCOMING_ERT_PACKET_CRC_ERROR");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_CHANGING_ERT_RESTING_CHANNEL:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_CHANGING_ERT_RESTING_CHANNEL");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_ERT_RADIO_OFF_FREEZE_PROC_IMC_REJECTED:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_ERT_RADIO_OFF_FREEZE_PROC_IMC_REJECTED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_NUMBER_OF_ERTS_WHOSE_RECORDS_WERE_FROZEN:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_NUMBER_OF_ERTS_WHOSE_RECORDS_WERE_FROZEN");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_RECEIVED_INVALID_TIME_FROM_NTP_TASK:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_RECEIVED_INVALID_TIME_FROM_NTP_TASK");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_UNSUCCESSFUL_PDOID_READ:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_UNSUCCESSFUL_PDOID_READ");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_242_TX_FAILED:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_242_TX_FAILED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ADDED_ERT_TO_MANAGED_LIST:
                {
                    eventItem.Description = m_rmStrings.GetString("ADDED_ERT_TO_MANAGED_LIST");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_TIME_SET_FAILED:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_TIME_SET_FAILED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ICM_EVENT_LOG_CLEARED:
                {
                    eventItem.Description = m_rmStrings.GetString("ICM_EVENT_LOG_CLEARED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_QUALITY_DETECTION_MOMENTARY_INTERRUPTION:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_QUALITY_DETECTION_MOMENTARY_INTERRUPTION");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_QUALITY_DETECTION_SUSTAINED_INTERRUPTION:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_QUALITY_DETECTION_SUSTAINED_INTERRUPTION");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TAMPER_TILT_SET_ON_OUTAGE:
                {
                    eventItem.Description = m_rmStrings.GetString("TAMPER_TILT_SET_ON_OUTAGE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CONFIGURATION_COMMIT:
                {
                    eventItem.Description = m_rmStrings.GetString("CONFIGURATION_COMMIT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FIRMWARE_DOWNLOAD_INITIALIZATION_FAILURE:
                {
                    eventItem.Description = m_rmStrings.GetString("FIRMWARE_DOWNLOAD_INITIALIZATION_FAILURE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.HAN_FIRMWARE_DOWNLOAD_FAILURE:
                {
                    eventItem.Description = m_rmStrings.GetString("HAN_FIRMWARE_DOWNLOAD_FAILURE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_242_COMMAND_REQUEST:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_242_COMMAND_REQUEST");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SMS_WAKEUP_IDENTITY_REQUEST_SENT:
                {
                    eventItem.Description = m_rmStrings.GetString("SMS_WAKEUP_IDENTITY_REQUEST_SENT");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SMS_WAKEUP_IDENTITY_NOT_SENT_BECAUSE_NOT_REGISTERED:
                {
                    eventItem.Description = m_rmStrings.GetString("SMS_WAKEUP_IDENTITY_NOT_SENT_BECAUSE_NOT_REGISTERED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SMS_WAKEUP_IDENTITY_REQUEST_NOT_SENT_BECAUSE_NOT_SYNCHRONIZED:
                {
                    eventItem.Description = m_rmStrings.GetString("SMS_WAKEUP_IDENTITY_REQUEST_NOT_SENT_BECAUSE_NOT_SYNCHRONIZED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FAILED_SECURITY_KEY_VERIFICATION:
                {
                    eventItem.Description = m_rmStrings.GetString("FAILED_SECURITY_KEY_VERIFICATION");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FAILED_CE_CONFIGURATION_VERIFICATION:
                {
                    eventItem.Description = m_rmStrings.GetString("FAILED_CE_CONFIGURATION_VERIFICATION");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.MIGRATION_STATE_CHANGE:
                {
                    eventItem.Description = m_rmStrings.GetString("MIGRATION_STATE_CHANGE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CRITICAL_PEAK_PRICING_STATUS:
                {
                    eventItem.Description = m_rmStrings.GetString("CRITICAL_PEAK_PRICING_STATUS");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SECURITY_EVENT:
                {
                    eventItem.Description = m_rmStrings.GetString("SECURITY_EVENT_ICM");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_METER_STOLEN:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_METER_STOLEN");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_METER_REMOVED:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_METER_REMOVED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_CONNECTION_DOWNTIME_TIME_EXCEEDED:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_CONNECTION_DOWNTIME_TIME_EXCEEDED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_PREDICTOR_LIST_TIME_MODIFIED:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_PREDICTOR_LIST_TIME_MODIFIED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.TIME_SOURCE_UNAVAILABLE:
                {
                    eventItem.Description = m_rmStrings.GetString("TIME_SOURCE_UNAVAILABLE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SIGNING_KEY_UPDATE_SUCCESS:
                {
                    eventItem.Description = m_rmStrings.GetString("SIGNING_KEY_UPDATE_SUCCESS");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SYMMETRIC_KEY_UPDATE_SUCCESS:
                {
                    eventItem.Description = m_rmStrings.GetString("SYMMETRIC_KEY_UPDATE_SUCCESS");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.KEY_ROLLOVER_SUCCESS:
                {
                    eventItem.Description = m_rmStrings.GetString("KEY_ROLLOVER_SUCCESS");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.UNPROGRAMMED:
                {
                    eventItem.Description = m_rmStrings.GetString("UNPROGRAMMED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.RAM_FAILURE:
                {
                    eventItem.Description = m_rmStrings.GetString("RAM_FAILURE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ROM_FAILURE:
                {
                    eventItem.Description = m_rmStrings.GetString("ROM_FAILURE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.NONVOLATILE_MEMORY_FAILURE:
                {
                    eventItem.Description = m_rmStrings.GetString("NONVOLATILE_MEMORY_FAILURE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.CLOCK_ERROR:
                {
                    eventItem.Description = m_rmStrings.GetString("CLOCK_ERROR");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.MEASUREMENT_ERROR:
                {
                    eventItem.Description = m_rmStrings.GetString("MEASUREMENT_ERROR");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.LOW_BATTERY:
                {
                    eventItem.Description = m_rmStrings.GetString("LOW_BATTERY");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.LOW_LOSS_POTENTIAL:
                {
                    eventItem.Description = m_rmStrings.GetString("LOW_LOSS_POTENTIAL");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.DEMAND_OVERLOAD:
                {
                    eventItem.Description = m_rmStrings.GetString("DEMAND_OVERLOAD");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.POWER_FAILURE:
                {
                    eventItem.Description = m_rmStrings.GetString("POWER_FAILURE");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.BAD_PASSWORD:
                {
                    eventItem.Description = m_rmStrings.GetString("BAD_PASSWORD");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.METERING_ERROR:
                {
                    eventItem.Description = m_rmStrings.GetString("METERING_ERROR");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.DC_DETECTED_OR_TIME_CHANGED:
                {
                    //TODO - the kV2c will need its own event dictionary because the following event means "time changed" for that device
                    eventItem.Description = m_rmStrings.GetString("DC_DETECTED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.SYSTEM_ERROR:
                {
                    eventItem.Description = m_rmStrings.GetString("SYSTEM_ERROR");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.RECEIVED_KWH:
                {
                    eventItem.Description = m_rmStrings.GetString("RECEIVED_KWH");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.LEADING_KVARH:
                {
                    eventItem.Description = m_rmStrings.GetString("LEADING_KVARH");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.LOSS_OF_PROGRAM:
                {
                    eventItem.Description = m_rmStrings.GetString("LOSS_OF_PROGRAM");
                    break;
                }
                //TODO - the kV2c will need its own event dictionary because the following event means "flash code error" for that device
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.HIGH_TEMP_OR_FLASH_CODE_ERROR:
                {
                    eventItem.Description = m_rmStrings.GetString("HIGH_TEMP");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.FLASH_DATA_ERROR:
                {
                    eventItem.Description = m_rmStrings.GetString("FLASH_DATA_ERROR");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_MANUAL_CALIBRATION_FAILED:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_CC1121_MANUAL_CALIBRATION_FAILED");
                    break;
                }
                case (ushort)ICS_Gateway_EventDictionary.CommModuleHistoryEvents.ERT_100G_TRACKING_FAILED:
                {
                    eventItem.Description = m_rmStrings.GetString("ERT_ICM_100G_TRACKING_FAILED");
                    break;
                }
            }

            eventItem.Enabled = entry.Monitored;

            if (eventItem.Description == "")
            {
                eventItem.Description = "Unknown Event " + entry.HistoryCode.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            m_lstEvents.Add(eventItem);
        }

        #endregion

        #region Members

        private byte[] m_abyStdEventsMonitored;
        private byte[] m_abyMfgEventsMonitored;
        private CPSEM m_psem;
        private ICSMfgTable2521 m_ICSMfgTable2521;
        private ICSMfgTable2522 m_ICSMfgTable2522;
        private List<MFG2048EventItem> m_lstEvents;
        private byte m_ICMFWVersion;

        /// <summary>The Resource Manager</summary>
        protected System.Resources.ResourceManager m_rmStrings;

        //The events in table 2523 have an offset of 1536 rather than the 3584 offset for table 2524.
        /// <summary>
        /// Events 1536-1551
        /// </summary>
        protected UInt16 m_usEvent1536_1551;
        /// <summary>
        /// Events 1552-1567
        /// </summary>
        protected UInt16 m_usEvent1552_1567;
        /// <summary>
        /// Events 1568-1583
        /// </summary>
        protected UInt16 m_usEvent1568_1583;
        /// <summary>
        /// Events 1584-1599
        /// </summary>
        protected UInt16 m_usEvent1584_1599;
        /// <summary>
        /// Events 1600-1615
        /// </summary>
        protected UInt16 m_usEvent1600_1615;
        /// <summary>
        /// Events 1616-1631
        /// </summary>
        protected UInt16 m_usEvent1616_1631;
        /// <summary>
        /// Events 1632-1647
        /// </summary>
        protected UInt16 m_usEvent1632_1647;
        /// <summary>
        /// Events 1648-1663
        /// </summary>
        protected UInt16 m_usEvent1648_1663;
        /// <summary>
        /// Events 1664-1679
        /// </summary>
        protected UInt16 m_usEvent1664_1679;
        /// <summary>
        /// Events 1680-1695
        /// </summary>
        protected UInt16 m_usEvent1680_1695;
        /// <summary>
        /// Events 1696-1711
        /// </summary>
        protected UInt16 m_usEvent1696_1711;
        ///// <summary>
        ///// Events 1984-1999
        ///// </summary>
        //protected UInt16 m_usEvent1984_1999;
        ///// <summary>
        ///// Events 2000-2015
        ///// </summary>
        //protected UInt16 m_usEvent2000_2015;

        #endregion
    }

    /// <summary>
    /// ICS Mfg Table Event Log Data
    /// </summary>
    public class ICSMfgTable2524 : AnsiTable
    {
        #region Constants

        private const int LTIME_LENGTH = 5;
        private const int TABLE_TIMEOUT = 5000;
        /// <summary>
        /// We can't read more than 1400 bytes in one offset read.  The following
        /// constant was determined by trial and error
        /// </summary>
        private const int MAX_ENTRIES_IN_ONE_READ = 60;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">the PSEM object for the device</param>
        /// <param name="table2521">Table 2521 object</param>
        /// <param name="eventDictionary">The ICS Gateway Event Dictionary</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  05/14/13 MSC 2.80.29 N/A    "Cell Data" => "Comm Module Data"
        //  06/21/13 AF  2.80.40 TR7586 Replaced CommModuleHistoryEntry with HistoryEntry
        //
        public ICSMfgTable2524(CPSEM psem, ICSMfgTable2521 table2521, ICS_Gateway_EventDictionary eventDictionary)
            : base(psem, 2524, table2521.SizeOfTable2524, TABLE_TIMEOUT)
        {
            m_table2521 = table2521;
            m_collCommModuleEntries = new List<HistoryEntry>();
            m_EventDictionary = eventDictionary;
            m_TimeFormat = (PSEMBinaryReader.TM_FORMAT)psem.TimeFormat;
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object</param>
        /// <param name="table2521">Table 2521 object</param>
        /// <param name="eventDictionary">The ICS Gateway Event Dictionary</param>
        /// <param name="timeFormat">The time format used in the meter</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  06/21/13 AF  2.80.40 TR7586 Replaced CommModuleHistoryEntry with HistoryEntry
        //
        public ICSMfgTable2524(PSEMBinaryReader reader, ICSMfgTable2521 table2521, ICS_Gateway_EventDictionary eventDictionary, int timeFormat)
            : base(2524, table2521.SizeOfTable2524)
        {
            m_table2521 = table2521;
            m_collCommModuleEntries = new List<HistoryEntry>();
            m_EventDictionary = eventDictionary;
            m_TimeFormat = (PSEMBinaryReader.TM_FORMAT)timeFormat;
            m_Reader = reader;

            ParseFromReader();
            State = TableState.Loaded;
        }

        /// <summary>
        /// Full read of 2524 (Mfg 476) out of the meter
        /// </summary>
        /// <returns>The PSEM response code for the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  06/21/13 AF  2.80.40 TR7586 Corrected the code for actual table layout
        //  06/26/13 AF  2.80.43 TR7586 Divide the read into multiple offset reads if there are
        //                              more than 60 events in the table
        //  07/03/13 AF  2.80.46 TR7586 Had to reverse the history code and event number fields to
        //                              match the collection engine
        //  07/12/13 AF  2.80.51 TR7586 Undid the previous change and added a conditional for the event number
        //  08/22/13 DLG 2.85.26 419848 Added logic to re-read table 2521 so that we can update the size for
        //                              table 2524.
        //  03/14/14 AF  3.50.49 464163 Removed the logic to re-read table 2521.  That will already be done when we instantiate the table.
        //  04/03/14 AF  3.50.59 464163 I may have been over zealous in removing the logic to re-read 2521.  I've put it back
        //                              to ensure we don't break WR 419848
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "ICMMfgTable2524.Read");

            // Re-read table 2521. This will give us the new side of table 2524 which we then use to change the
            // table size.
            m_table2521.Read();

            ChangeTableSize(m_table2521.SizeOfTable2524);

            //Read the header			
            PSEMResponse Result = base.Read(0, 11);

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                //Populate the member variable that represent the table
                m_byHistFlags = m_Reader.ReadByte();
                m_uiNbrValidEntries = m_Reader.ReadUInt16();
                m_uiLastEntryElement = m_Reader.ReadUInt16();
                m_uiLastEntrySeqNbr = m_Reader.ReadUInt32();
                m_uiNbrUnreadEntries = m_Reader.ReadUInt16();

                m_collCommModuleEntries.Clear();

                //Don't do any more if there are no events
                if (m_uiNbrValidEntries > 0)
                {
                    ushort usNumberOfBytesToRead = 0;

                    // How many reads do we need to perform to get all the events in the table
                    int iNumberOfOffsetReads = m_uiNbrValidEntries / MAX_ENTRIES_IN_ONE_READ;

                    // It might not divide evenly, so get the remainder
                    if (m_uiNbrValidEntries != iNumberOfOffsetReads * MAX_ENTRIES_IN_ONE_READ)
                    {
                        iNumberOfOffsetReads++;
                    }

                    for (int iIndex = 0; iIndex < iNumberOfOffsetReads; iIndex++)
                    {
                        if (iIndex == iNumberOfOffsetReads - 1)
                        {
                            // This is the last read and where we deal with reading the remainder
                            usNumberOfBytesToRead = (ushort)((m_uiNbrValidEntries % MAX_ENTRIES_IN_ONE_READ)*m_table2521.SizeOfEventEntry);
                        }

                        if (usNumberOfBytesToRead == 0)
                        {
                            // We need to read the max
                            usNumberOfBytesToRead = (ushort)(MAX_ENTRIES_IN_ONE_READ * m_table2521.SizeOfEventEntry);
                        }

                        Result = base.Read(11 + iIndex * MAX_ENTRIES_IN_ONE_READ * (ushort)m_table2521.SizeOfEventEntry, usNumberOfBytesToRead);

                        if (PSEMResponse.Ok == Result)
                        {
                            // Now we are going to populate the collection
                            ushort usNumEntriesInThisRead = (ushort)(usNumberOfBytesToRead / m_table2521.SizeOfEventEntry);

                            for (UInt16 uiNbrHistoryEntries = 0; uiNbrHistoryEntries < usNumEntriesInThisRead; uiNbrHistoryEntries++)
                            {
                                HistoryEntry histEntry = new HistoryEntry(true,
                                                true,
                                                true,
                                                m_table2521.ICSDataLength,
                                                m_EventDictionary);

                                histEntry.HistoryTime = m_Reader.ReadLTIME(m_TimeFormat);
                                if (m_table2521.EventNumberFlag)
                                {
                                    histEntry.EventNumber = m_Reader.ReadUInt16();
                                }
                                histEntry.HistorySequenceNumber = m_Reader.ReadUInt16();
                                histEntry.UserID = m_Reader.ReadUInt16();
                                histEntry.HistoryCode = m_Reader.ReadUInt16();
                                histEntry.HistoryArgument = m_Reader.ReadBytes(m_table2521.ICSDataLength);

                                m_collCommModuleEntries.Add(histEntry);
                            }
                        }
                    }
                }
            }

            return Result;
        }

        /// <summary>
        /// Provide a way to have us reread the Events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //
        public void Refresh()
        {
            m_TableState = TableState.Expired;
            m_collCommModuleEntries.Clear();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Access to the History Log Event Collection
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  06/21/13 AF  2.80.40 TR7586 Replaced CommModuleHistoryEntry with HistoryEntry
        //
        public List<HistoryEntry> CommModuleHistoryEventEntries
        {
            get
            {
                ReadUnloadedTable();

                return m_collCommModuleEntries;
            }
        }

        /// <summary>
        /// Access to the Number of valid Entries count
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  06/21/13 AF  2.80.40 TR7586 Added read of unloaded table
        //
        public UInt16 NumberValidEntries
        {
            get
            {
                ReadUnloadedTable();

                return m_uiNbrValidEntries;
            }
        }

        /// <summary>
        /// Access to the Last Entry Element
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  06/21/13 AF  2.80.40 TR7586 Added read of unloaded table
        //
        public UInt16 LastEntryElement
        {
            get
            {
                ReadUnloadedTable();

                return m_uiLastEntryElement;
            }
        }

        /// <summary>
        /// Access to the Last Entry Sequence Number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  06/21/13 AF  2.80.40 TR7586 Added read of unloaded table
        //
        public UInt32 LastEntrySequenceNumber
        {
            get
            {
                ReadUnloadedTable();

                return m_uiLastEntrySeqNbr;
            }
        }

        /// <summary>
        /// Access to the Number of Unread Entries
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  06/21/13 AF  2.80.40 TR7586 Added read of unloaded table
        //
        public UInt16 NumberUnreadEntries
        {
            get
            {
                ReadUnloadedTable();

                return m_uiNbrUnreadEntries;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data from the binary reader. This should not be used when reading data from an actual meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  04/03/13 MSC 2.80.10 n/a    Created
        //  06/21/13 AF  2.80.40 TR7586 Corrected code for actual table layout
        //  07/01/13 AF  2.80.45 TR7640 We have to match the CE dlls which have the event number and history code 
        //                              reversed from what we expected.
        //  07/12/13 AF  2.80.51 TR7586 Undid the previous change and added a conditional for the event number
        //
        private void ParseFromReader()
        {
            m_DataStream.Position = 0;

            //Populate the member variables that represent the table
            m_byHistFlags = m_Reader.ReadByte();
            m_uiNbrValidEntries = m_Reader.ReadUInt16();
            m_uiLastEntryElement = m_Reader.ReadUInt16();
            m_uiLastEntrySeqNbr = m_Reader.ReadUInt32();
            m_uiNbrUnreadEntries = m_Reader.ReadUInt16();

            m_collCommModuleEntries.Clear();

            // Now we are going to read the collection
            for (uint uiNbrHistoryEntries = 0; uiNbrHistoryEntries < m_uiNbrValidEntries; uiNbrHistoryEntries++)
            {
                HistoryEntry histEntry = new HistoryEntry(true,
                                                          true,
                                                          true,
                                                          m_table2521.ICSDataLength,
                                                          m_EventDictionary);

                histEntry.HistoryTime = m_Reader.ReadLTIME(m_TimeFormat);
                if (m_table2521.EventNumberFlag)
                {
                    histEntry.EventNumber = m_Reader.ReadUInt16();
                }
                histEntry.HistorySequenceNumber = m_Reader.ReadUInt16();
                histEntry.UserID = m_Reader.ReadUInt16();
                histEntry.HistoryCode = m_Reader.ReadUInt16();
                histEntry.HistoryArgument = m_Reader.ReadBytes(m_table2521.ICSDataLength);

                m_collCommModuleEntries.Add(histEntry);
            }
        }

        #endregion

        #region Members

        ICSMfgTable2521 m_table2521;
        private byte m_byHistFlags;
        private UInt16 m_uiNbrValidEntries;
        private UInt16 m_uiLastEntryElement;
        private UInt32 m_uiLastEntrySeqNbr;
        private UInt16 m_uiNbrUnreadEntries;
        private List<HistoryEntry> m_collCommModuleEntries;
        private ICS_Gateway_EventDictionary m_EventDictionary;
        private PSEMBinaryReader.TM_FORMAT m_TimeFormat;

        #endregion
    }

    /// <summary>
    /// MFG Table 481 (2529) - ICS Comm LAN (ERT and Zigbee) Information.
    /// </summary>
    public class ICMMfgTable2529CommLanInfo : AnsiTable
    {
        #region Constants

        private const ushort TABLE_SIZE = 27;
        private const ushort EXTENDED_TABLE_SIZE = 30;
        private const ushort TABLE_OFFSET = 0;
        private const ushort TABLE_TIMEOUT = 3;
        private const ushort HAN_HW_VERSION_BYTE_SIZE = 1;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  09/15/16 jrf 4.70.18 WI713982 Allowing automatic table resizing.
        public ICMMfgTable2529CommLanInfo(CPSEM psem)
            : base(psem, 2529, TABLE_SIZE, TABLE_TIMEOUT)
        {
            InitializeVariables();

            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Full read of table 2529 (ICS Comm LAN) out of the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  09/16/16 jrf 4.70.18 WI713982 Making a full read.
        //  
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2529ICSCommLanInfo.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the HAN module Type.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/13/13 DLG 3.50.03 TR9505   Created.
        //  
        public string HanModuleType
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    // Read MFG Table 481 (2529) - ICS Comm LAN (ERT and Zigbee) Information
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Table 2529.  Table length may be unexpected."));
                    }
                }

                return TranslateHanModuleType(m_HanModType);
            }
        }

        /// <summary>
        /// Gets the HAN Module firmware build version.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/13/13 DLG 3.50.03 TR9505   Created.
        //  
        public string HanModuleBuild
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                string strFWVerRev = "0.0";

                if (TableState.Loaded != m_TableState)
                {
                    // Read MFG Table 481 (2529) - ICS Comm LAN (ERT and Zigbee) Information
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2529.";
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                                                 Result, strMsg));
                    }
                }

                strFWVerRev = m_HanModFwBuild.ToString("d3", CultureInfo.CurrentCulture);

                return strFWVerRev;
            }
        }

        /// <summary>
        /// Gets the Han module version and revision in the format VERSION.REVISION
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/13/13 DLG 3.50.03 TR9505   Created.
        //  
        public string HanModuleVersion
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                string strFWVerRev = "0.0";

                if (TableState.Loaded != m_TableState)
                {
                    // Read MFG Table 481 (2529) - ICS Comm LAN (ERT and Zigbee) Information
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        string strMsg = "Error reading Table 2529.";
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result, strMsg));
                    }
                }

                strFWVerRev = m_HanModFwVers.ToString(CultureInfo.InvariantCulture) + "."
                    + m_HanModFwRev.ToString("d3", CultureInfo.CurrentCulture);

                return strFWVerRev;
            }
        }

        /// <summary>
        /// Gets the Version of the HAN module firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  
        public byte HANVersionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_HanModFwVers;
            }
        }

        /// <summary>
        /// Gets the Revision of the HAN module firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  
        public byte HANRevisionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_HanModFwRev;
            }
        }

        /// <summary>
        /// Gets the Build of the HAN module firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  
        public byte HANBuildOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_HanModFwBuild;
            }
        }

        /// <summary>
        /// Gets the HAN Hardware version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  
        public string HanHardwareVersion
        {
            get
            {
                ReadUnloadedTable();

                return FormatBytesString(m_HanHwVersion);
            }
        }

        /// <summary>
        /// MAC address of the HAN client.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  
        public UInt64 HanMacAddress
        {
            get
            {
                ReadUnloadedTable();

                return m_HanMacAddr;
            }
        }

        /// <summary>
        /// Gets the ERT Module Type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  
        public byte ERTModType
        {
            get
            {
                ReadUnloadedTable();

                return m_ErtModType;
            }
        }

        /// <summary>
        /// Gets the ERT Hardware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  
        public byte ERTHwVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_ErtHwVersion;
            }
        }

        /// <summary>
        /// Gets the ERT Hardware Revision
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  
        public byte ERTHwRevision
        {
            get
            {
                ReadUnloadedTable();

                return m_ErtHwRevision;
            }
        }

        /// <summary>
        /// Gets the ERT Module Firmware Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  
        public byte ERTModFwVersion
        {
            get
            {
                ReadUnloadedTable();

                return m_ErtModFwVers;
            }
        }

        /// <summary>
        /// Gets the ERT Module Firmware Revision
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  
        public byte ERTModFwRevision
        {
            get
            {
                ReadUnloadedTable();

                return m_ErtModFwRev;
            }
        }

        /// <summary>
        /// Gets the ERT Module Firmware Build
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  
        public byte ERTModFwBuild
        {
            get
            {
                ReadUnloadedTable();

                return m_ErtModFwBuild;
            }
        }

        /// <summary>
        /// Gets the Version of the HAN application firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  09/15/16 jrf 4.70.18  WI 713982  Created.
        public byte HANAppVersionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_HANAppVersion;
            }
        }

        /// <summary>
        /// Gets the Revision of the HAN application firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  09/15/16 jrf 4.70.18  WI 713982  Created.
        public byte HANAppRevisionOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_HANAppRevision;
            }
        }

        /// <summary>
        /// Gets the Build of the HAN application firmware
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  09/15/16 jrf 4.70.18  WI 713982  Created.
        public byte HANAppBuildOnly
        {
            get
            {
                ReadUnloadedTable();

                return m_HANAppBuild;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize Variables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  09/15/16 jrf 4.70.18  WI713982 Added new members.
        private void InitializeVariables()
        {
            m_HanModType = 0;
            m_HanModFwVers = 0;
            m_HanModFwRev = 0;
            m_HanModFwBuild = 0;
            m_HanHwVersion = null;
            m_HanMacAddr = 0;
            m_ErtModType = 0;
            m_ErtHwVersion = 0;
            m_ErtHwRevision = 0;
            m_ErtModFwVers = 0;
            m_ErtModFwRev = 0;
            m_ErtModFwBuild = 0;
            m_HANAppVersion = 0;
            m_HANAppRevision = 0;
            m_HANAppBuild = 0;
        }

        /// <summary>
        /// Parses the data from the specified reader.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  09/15/16 jrf 4.70.18  WI 713982 Added reading HAN app version when available.
        private void ParseData()
        {
            if (m_Reader != null)
            {
                m_DataStream.Position = 0;

                m_HanModType = m_Reader.ReadByte();
                m_HanModFwVers = m_Reader.ReadByte();
                m_HanModFwRev = m_Reader.ReadByte();
                m_HanModFwBuild = m_Reader.ReadByte();
                m_HanHwVersion = m_Reader.ReadBytes(HAN_HW_VERSION_BYTE_SIZE);
                m_HanMacAddr = m_Reader.ReadUInt64();

                // Skipping the HAN reserved ARRAY[4] of UINT8 until it's defined.
                m_Reader.ReadBytes(4);

                m_ErtModType = m_Reader.ReadByte();
                m_ErtHwVersion = m_Reader.ReadByte();
                m_ErtHwRevision = m_Reader.ReadByte();
                m_ErtModFwVers = m_Reader.ReadByte();
                m_ErtModFwRev = m_Reader.ReadByte();
                m_ErtModFwBuild = m_Reader.ReadByte();

                // Skipping the ERT reserved ARRAY[4] of UINT8 until it's defined.
                m_Reader.ReadBytes(4);

                if (m_DataStream.Length >= EXTENDED_TABLE_SIZE)
                {
                    m_HANAppVersion = m_Reader.ReadByte();
                    m_HANAppRevision = m_Reader.ReadByte();
                    m_HANAppBuild = m_Reader.ReadByte();
                }

                    m_TableState = TableState.Loaded;
            }
        }

        #endregion

        #region Public Static Methods (Translation Methods)

        /// <summary>
        /// Translate the Han Module Type
        /// </summary>
        /// <param name="byHanModType">byte value from the device</param>
        /// <returns>string - HAN Module Type</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- -------------------------------------------
        //  11/14/13 DLG 3.50.03 TR9505   Created.
        //  
        public static string TranslateHanModuleType(byte byHanModType)
        {
            string strHanModType = "Unknown";

            if (0 == byHanModType)
            {
                strHanModType = "ZigBee";
            }

            else
            {
                strHanModType = "Unknown";
            }

            return strHanModType;
        }

        #endregion

        #region Members

        private byte m_HanModType;
        private byte m_HanModFwVers;
        private byte m_HanModFwRev;
        private byte m_HanModFwBuild;
        private byte[] m_HanHwVersion;
        private UInt64 m_HanMacAddr;
        private byte m_ErtModType;
        private byte m_ErtHwVersion;
        private byte m_ErtHwRevision;
        private byte m_ErtModFwVers;
        private byte m_ErtModFwRev;
        private byte m_ErtModFwBuild;
        private byte m_HANAppVersion;
        private byte m_HANAppRevision;
        private byte m_HANAppBuild;

        #endregion
    }

    /// <summary>
    /// ICS Mfg Table Force Time Sync Clock Table.  It mirrors the std table 52 clock table.
    /// </summary>
    public class ICSMfgTable2525 : StdTable52
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for this session</param>
        /// <param name="Table0">The table object for table 0.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ -------------------------------------------
        // 07/18/13 jrf 2.80.54 WR 411418 Created.
        //
        public ICSMfgTable2525(CPSEM psem, CTable00 Table0)
            : base(psem, Table0, 2525) 
        {
        }

        #endregion
    }

    /// <summary>
    /// Actual table for the ICM exception configuration
    /// </summary>
    public class ICMMfgTable2536ActualNetwork : AnsiTable
    {
        #region Constants

        private const ushort TABLE_SIZE = 14;

        private const byte TIME_STAMP_ENABLE_FLAG_MASK = 0x01;
        private const byte PROG_NATIVE_ADDRESS_MASK = 0x02;
        private const byte PROG_BROADCAST_ADDRESS_MASK = 0x04;
        private const byte STATIC_RELAY_MASK = 0x08;
        private const byte STATIC_APTITLE_MASK = 0x10;
        private const byte STATIC_MASTER_RELAY_MASK = 0x20;
        private const byte CLIENT_RESPONSE_CTRL_MASK = 0x40;
        private const byte COMM_MODULE_SUPP_FLAG_MASK = 0X80;

        #endregion

        #region Definitions
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        public ICMMfgTable2536ActualNetwork(CPSEM psem)
            : base(psem, 2536, TABLE_SIZE)
        {
            m_PSEM = psem;
        }

        /// <summary>
        /// Constructor that uses that data stored in a Binary Reader
        /// </summary>
        /// <param name="Reader">The current PSEM binary reader object</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        public ICMMfgTable2536ActualNetwork(PSEMBinaryReader Reader) : base(2536, TABLE_SIZE)
        {
            m_Reader = Reader;

            ParseData();

            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads ICM mfg table 2536 out of the meter
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "ICMMfgTable2536ActualNetwork.Read");

            PSEMResponse Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The number of exception hosts
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        public ushort NumberOfExceptionHosts
        {
            get
            {
                ReadUnloadedTable();

                return m_NbrExceptionHosts;
            }
        }

        /// <summary>
        /// The number of exception events
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        public ushort NumberOfExceptionEvents
        {
            get
            {
                ReadUnloadedTable();

                return m_NbrExceptionEvents;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the data out of the binary reader and into the member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        private void ParseData()
        {
            if (m_Reader != null)
            {
                m_NetworkBitfield = m_Reader.ReadByte();
                m_NbrInterfaces = m_Reader.ReadByte();
                m_NbrRegistrations = m_Reader.ReadByte();
                m_NbrFilteringRules = m_Reader.ReadUInt16();
                m_NbrExceptionHosts = m_Reader.ReadUInt16();
                m_NbrExceptionEvents = m_Reader.ReadUInt16();
                m_NbrStatistics = m_Reader.ReadUInt16();
                m_NbrMulticastAddresses = m_Reader.ReadByte();
                m_NativeAddressLength = m_Reader.ReadByte();
                m_FilteringExpLength = m_Reader.ReadByte();
            }
        }

        #endregion

        #region Members

        private byte m_NetworkBitfield;
        private byte m_NbrInterfaces;
        private byte m_NbrRegistrations;
        private ushort m_NbrFilteringRules;
        private ushort m_NbrExceptionHosts;
        private ushort m_NbrExceptionEvents;
        private ushort m_NbrStatistics;
        private byte m_NbrMulticastAddresses;
        private byte m_NativeAddressLength;
        private byte m_FilteringExpLength;

        #endregion

    }

    /// <summary>
    /// Class representing the ICM Exception Report Configuration table
    /// </summary>
    public class ICMMfgTable2537ExceptionReport : AnsiTable
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for the current session</param>
        /// <param name="Table2536">The actual table which gives us the number 
        /// of exception hosts and the number of exception events configured</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        public ICMMfgTable2537ExceptionReport(CPSEM psem, ICMMfgTable2536ActualNetwork Table2536)
            : base(psem, 2537, GetTableSize(Table2536))
        {
            m_Table2536 = Table2536;
        }

        /// <summary>
        /// Constructor that uses that data stored in a Binary Reader
        /// </summary>
        /// <param name="Reader">The current PSEM binary reader object</param>
        /// <param name="Table2536"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        public ICMMfgTable2537ExceptionReport(PSEMBinaryReader Reader, ICMMfgTable2536ActualNetwork Table2536) 
            : base(2537, GetTableSize(Table2536))
        {
            m_Table2536 = Table2536;
            m_ExceptionReports = new List<ExceptionReportRecord>();
            m_Reader = Reader;

            ParseData();
        }

        /// <summary>
        /// Reads ICM mfg table 2537 out of the meter
        /// </summary>
        /// <returns>the result of the read attempt</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "ICMMfgTable2537ExceptionReport.Read");

            PSEMResponse Result = PSEMResponse.Ok;

            if (m_Table2536.NumberOfExceptionHosts > 0)
            {
                Result = base.Read();

                if (PSEMResponse.Ok == Result)
                {
                    m_DataStream.Position = 0;

                    ParseData();
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties
        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the size of table 2537 based on values read from 2536
        /// </summary>
        /// <param name="Table2536">Table 2536 object</param>
        /// <returns>the size of the table</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        private static uint GetTableSize(ICMMfgTable2536ActualNetwork Table2536)
        {
            uint uiTableSize = 0;
            uint uiEventReportedLength = 0;

            uiEventReportedLength = (uint)(Table2536.NumberOfExceptionEvents * 2);
            uiTableSize = Table2536.NumberOfExceptionHosts * (25 + uiEventReportedLength);

            return uiTableSize;
        }

        /// <summary>
        /// Gets the data out of the binary reader and into the member variables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        private void ParseData()
        {
            ExceptionReportRecord ExceptionReport = new ExceptionReportRecord(m_Table2536);
            UInt16[] EventsReported = null;

            for (int iHost = 0; iHost < m_Table2536.NumberOfExceptionHosts; iHost++)
            {
                ExceptionReport.AptitleNotify = m_Reader.ReadBytes(20);
                ExceptionReport.MaxNumberOfRetries = m_Reader.ReadByte();
                ExceptionReport.RetryDelay = m_Reader.ReadUInt16();
                ExceptionReport.ExclusionPeriod = m_Reader.ReadUInt16();

                for (int iEvent = 0; iEvent < m_Table2536.NumberOfExceptionEvents; iEvent++)
                {
                    EventsReported[iEvent] = m_Reader.ReadUInt16();
                }
            }
        }

        #endregion

        #region Members

        private ICMMfgTable2536ActualNetwork m_Table2536;
        private List<ExceptionReportRecord> m_ExceptionReports;

        #endregion
    }

    /// <summary>
    /// Class that represents an exception report record
    /// </summary>
    public class ExceptionReportRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Table2536">Table 2536 object, which will give us the number of exception events</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        public ExceptionReportRecord(ICMMfgTable2536ActualNetwork Table2536)
        {
            m_Table2536 = Table2536;
            m_AptitleNotify = new byte[20];
            m_EventReported = new ushort[Table2536.NumberOfExceptionEvents];
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets/sets the Aptitle Notify
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        public byte[] AptitleNotify
        {
            get
            {
                return m_AptitleNotify;
            }
            set
            {
                m_AptitleNotify = value;
            }
        }

        /// <summary>
        /// Gets/sets the max number of retries
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        public byte MaxNumberOfRetries
        {
            get
            {
                return m_MaxNumberOfRetries;
            }
            set
            {
                m_MaxNumberOfRetries = value;
            }
        }

        /// <summary>
        /// Gets/sets the retry delay
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        public ushort RetryDelay
        {
            get
            {
                return m_RetryDelay;
            }
            set
            {
                m_RetryDelay = value;
            }
        }

        /// <summary>
        /// Gets/sets the exclusion period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        public ushort ExclusionPeriod
        {
            get
            {
                return m_ExclusionDelay;
            }
            set
            {
                m_ExclusionDelay = value;
            }
        }

        /// <summary>
        /// Gets/sets the event reported
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        public ushort[] EventReported
        {
            get
            {
                return m_EventReported;
            }
            set
            {
                m_EventReported = value;
            }
        }

        #endregion

        #region Members

        private ICMMfgTable2536ActualNetwork m_Table2536;
        private byte[] m_AptitleNotify;
        private byte m_MaxNumberOfRetries;
        private ushort m_RetryDelay;
        private ushort m_ExclusionDelay;
        private ushort[] m_EventReported;

        #endregion

    }

    /// <summary>
    /// ICM Mfg Table FWDL Component. It has the firmware and hardware versions 
    /// of all the microtypes
    /// </summary>
    public class ICMMfgTable2539FWDLComponent : AnsiTable
    {
        #region Constants

        private const uint MINIMUM_TABLE_SIZE = 2;
        private const int SIZE_OF_ENTRY = 10;

        #endregion        

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/23/15 AF  4.20.03  WR 577606  Created
        //
        public ICMMfgTable2539FWDLComponent(CPSEM psem)
            : base(psem, 2539, MINIMUM_TABLE_SIZE)
        {
            m_PSEM = psem;
            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Constructor that uses that data stored in a Binary Reader
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/23/15 AF  4.20.03  WR 577606  Created
        //
        public ICMMfgTable2539FWDLComponent(PSEMBinaryReader reader)
            : base(2539, MINIMUM_TABLE_SIZE)
        {
            m_Reader = reader;

            ParseData();

            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads table ICM Mfg 2539 out of the meter.
        /// </summary>
        /// <returns>the result of the read attempt</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/23/15 AF  4.20.03  WR 577606  Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "ICMMfgTable2539FWDLComponent.Read");
            
             PSEMResponse Result = base.Read(0, 2);

             if (PSEMResponse.Ok == Result)
             {
                 m_DataStream.Position = 0;

                 m_NumberOfEntries = m_Reader.ReadUInt16();

                 ChangeTableSize((uint)(m_NumberOfEntries * SIZE_OF_ENTRY + 2));

                 m_MicrotypeRecords = new MicrotypeRecord[m_NumberOfEntries];

                 for (int index = 0; index < m_NumberOfEntries; index++)
                 {
                     Result = base.Read(2 + index * SIZE_OF_ENTRY, SIZE_OF_ENTRY);
                     if (PSEMResponse.Ok == Result)
                     {
                         m_MicrotypeRecords[index] = new MicrotypeRecord();
                         m_MicrotypeRecords[index].Parse(m_Reader);
                     }
                 }

                m_TableState = TableState.Loaded;
             }

             return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of microtype records from the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/23/15 AF  4.20.03  WR 577606  Created
        //
        public MicrotypeRecord[] MicrotypeRecords
        {
            get
            {
                ReadUnloadedTable();

                return m_MicrotypeRecords;
            }
        }

        /// <summary>
        /// Gets the NXP (ERT radio firmware) version from the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/23/15 AF  4.20.03  WR 577606  Created
        //
        public byte[] NXPFWVersion
        {
            get
            {
                ReadUnloadedTable();

                byte[] firmwareVersion = new byte[3];

                for (int index = 0; index < m_MicrotypeRecords.Length; index++)
                {
                    if (m_MicrotypeRecords[index].MicrotypeFirmwareType == (byte)MicrotypeRecord.MICROTYPES.NXP_FIRMWARE)
                    {
                        firmwareVersion = m_MicrotypeRecords[index].FirmwareVersion;
                        break;
                    }
                }

                return firmwareVersion;
            }
        }

        /// <summary>
        /// Gets the modem firmware version from the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  05/08/15 jrf 4.20.06  WR 584075  Created
        //
        public byte[] ModemFWVersion
        {
            get
            {
                ReadUnloadedTable();

                byte[] firmwareVersion = new byte[3];

                for (int index = 0; index < m_MicrotypeRecords.Length; index++)
                {
                    if (IsModemFirmware(m_MicrotypeRecords[index].MicrotypeFirmwareType))
                    {
                        firmwareVersion = m_MicrotypeRecords[index].FirmwareVersion;
                        break;
                    }
                }

                return firmwareVersion;
            }
        }

        /// <summary>
        /// Gets the PIC firmware version from the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  05/08/15 jrf 4.20.06  WR 584075  Created
        public byte[] PICFWVersion
        {
            get
            {
                ReadUnloadedTable();

                byte[] firmwareVersion = new byte[3];

                for (int index = 0; index < m_MicrotypeRecords.Length; index++)
                {
                    if (m_MicrotypeRecords[index].MicrotypeFirmwareType == (byte)MicrotypeRecord.MICROTYPES.PIC_FIRMWARE)
                    {
                        firmwareVersion = m_MicrotypeRecords[index].FirmwareVersion;
                        break;
                    }
                }

                return firmwareVersion;
            }
        }

        /// <summary>
        /// Gets the ICM module firmware version from the table
        /// </summary>
        public byte[] ICMModuleFWVersion
        {
            get
            {
                ReadUnloadedTable();

                byte[] firmwareVersion = new byte[3];

                for (int index = 0; index < m_MicrotypeRecords.Length; index++)
                {
                    if (m_MicrotypeRecords[index].MicrotypeFirmwareType == (byte)MicrotypeRecord.MICROTYPES.ICM_MODULE_FIRMWARE)
                    {
                        firmwareVersion = m_MicrotypeRecords[index].FirmwareVersion;
                        break;
                    }
                }

                return firmwareVersion;
            }
        }

        /// <summary>
        /// Gets the ICM module hardware version from the table
        /// </summary>
        public byte[] ICMModuleHWVersion
        {
            get
            {
                ReadUnloadedTable();

                byte[] firmwareVersion = new byte[3];

                for (int index = 0; index < m_MicrotypeRecords.Length; index++)
                {
                    if (m_MicrotypeRecords[index].MicrotypeFirmwareType == (byte)MicrotypeRecord.MICROTYPES.ICM_MODULE_FIRMWARE)
                    {
                        firmwareVersion = m_MicrotypeRecords[index].HardwareVersion;
                        break;
                    }
                }

                return firmwareVersion;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the data out of the binary reader and into the member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/23/15 AF  4.20.03  WR 577606  Created
        //
        private void ParseData()
        {
            if (m_Reader != null)
            {
                if (m_Reader.BaseStream.Length < MINIMUM_TABLE_SIZE)
                {
                    throw new Exception("Table size is less than the minimum size expected. Table size = "
                        + m_Size.ToString(CultureInfo.InvariantCulture));
                }
                else // m_Size >= MINIMUM_TABLE_SIZE
                {
                    m_NumberOfEntries = m_Reader.ReadUInt16();
                    m_MicrotypeRecords = new MicrotypeRecord[m_NumberOfEntries];

                    for (int index = 0; index < m_NumberOfEntries; index++)
                    {
                        base.Read(2 + (index * SIZE_OF_ENTRY), SIZE_OF_ENTRY);
                        m_MicrotypeRecords[index] = new MicrotypeRecord();
                        m_MicrotypeRecords[index].Parse(m_Reader);
                    }
                }
            }
        }

        /// <summary>
        /// Method determines if the firmware type represented by the 
        /// byte paramater is a modem firmware type.
        /// </summary>
        /// <param name="fWType">The byte representing the firmware type</param>
        /// <returns>Whether or not firmware type is modem firmware</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  05/11/15 jrf 4.20.06  WR 584075  Created
        private static bool IsModemFirmware(byte fWType)
        {
            bool IsModemFW = false;

            if (Enum.IsDefined(typeof(MicrotypeRecord.MICROTYPES), fWType))
            {
                MicrotypeRecord.MICROTYPES ICMFWType = (MicrotypeRecord.MICROTYPES)fWType;

                switch (ICMFWType)
                {
                    case MicrotypeRecord.MICROTYPES.MODEM_FIRMWARE_SIERRA_VERIZON_3G:
                    case MicrotypeRecord.MICROTYPES.MODEM_FIRMWARE_SIERRA_ATT_3G:
                    case MicrotypeRecord.MICROTYPES.MODEM_FIRMWARE_SIERRA_VERIZON_LTE:
                    case MicrotypeRecord.MICROTYPES.MODEM_FIRMWARE_SIERRA_ATT_LTE:
                    case MicrotypeRecord.MICROTYPES.MODEM_FIRMWARE_SIERRA_ROGERS_LTE:
                    {
                        IsModemFW = true;
                        break;
                    }
                    default:
                        break;
                }
            }

            return IsModemFW;
        }

        #endregion

        #region Members

        private UInt16 m_NumberOfEntries;
        private MicrotypeRecord[] m_MicrotypeRecords;

        #endregion
    }

    /// <summary>
    /// Class representing one entry in the Mfg table 2539 (491) table
    /// </summary>
    public class MicrotypeRecord
    {
        #region Definitions

        /// <summary>
        /// The firmware types for all the ICM micros
        /// </summary>
        public enum MICROTYPES : byte
        {
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("ICM Module Firmware")]
            ICM_MODULE_FIRMWARE = 32,
            /// <summary>
            /// 
            /// </summary>            
            [EnumDescription("Modem Firmware Sierra Verizon 3G")]
            MODEM_FIRMWARE_SIERRA_VERIZON_3G = 33,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("GE Register")]
            GE_REGISTER = 34,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("GE MeterMate Program")]
            GE_METERMATE_PROGRAM = 35,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("Modem Firmware Sierra AT&T 3G")]
            MODEM_FIRMWARE_SIERRA_ATT_3G = 36,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("Modem Firmware Sierra Verizon LTE")]
            MODEM_FIRMWARE_SIERRA_VERIZON_LTE = 37,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("Modem Firmware Sierra AT&T LTE")]
            MODEM_FIRMWARE_SIERRA_ATT_LTE = 38,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("Modem Firmware Sierra Rogers LTE")]
            MODEM_FIRMWARE_SIERRA_ROGERS_LTE = 39,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("NXP Firmware")]
            NXP_FIRMWARE = 40,
            /// <summary>
            /// 
            /// </summary>
            [EnumDescription("PIC Firmware")]
            PIC_FIRMWARE = 41,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor - initializes the member arrays
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/23/15 AF  4.20.03  WR 577606  Created
        //
        public MicrotypeRecord()
        {
            m_byFirmwareType = 0;
            m_abyHardwareVersion = new byte[3];
            m_abyFirmwareVersion = new byte[3];
            m_abyReserved = new byte[3];
        }

        /// <summary>
        /// Gets the data out of the binary reader and into the member variables.
        /// </summary>
        /// <param name="Reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/23/15 AF  4.20.03  WR 577606  Created
        //
        public void Parse(PSEMBinaryReader Reader)
        {
            m_byFirmwareType = Reader.ReadByte();
            m_abyHardwareVersion = Reader.ReadBytes(3);
            m_abyFirmwareVersion = Reader.ReadBytes(3);
            m_abyReserved = Reader.ReadBytes(3);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The firmware type of the mfg table 491 entry
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/23/15 AF  4.20.03  WR 577606  Created
        //
        public byte MicrotypeFirmwareType
        {
            get
            {
                return m_byFirmwareType;
            }
        }

        /// <summary>
        /// The hardware version of the microtype in the form of a byte array.
        /// The members of the array will be version, revision, and reserved
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/23/15 AF  4.20.03  WR 577606  Created
        //
        public byte[] HardwareVersion
        {
            get
            {
                return m_abyHardwareVersion;
            }
        }

        /// <summary>
        /// The firmware version of the microtype in the form of a byte array.
        /// The members of the array will be version, revision, and build
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/23/15 AF  4.20.03  WR 577606  Created
        //
        public byte[] FirmwareVersion
        {
            get
            {
                return m_abyFirmwareVersion;
            }
        }

        /// <summary>
        /// Gets the firmware version out of microtype record in the
        /// form of an x.x string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  05/08/15 jrf 4.20.06  WR 584075  Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string FormattedFirmwareVersion
        {
            get
            {
                string Version = "";

                if (FirmwareVersion != null && 2 <= FirmwareVersion.Length)
                {
                    Version = FirmwareVersion[0].ToString() + "."
                        + FirmwareVersion[1].ToString("d3", CultureInfo.CurrentCulture);
                }

                return Version;
            }
        }

        /// <summary>
        /// Gets the firmware build number out of microtype record in the
        /// form of a string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  05/08/15 jrf 4.20.06  WR 584075  Created
        public string FormattedFirmwareBuild
        {
            get
            {
                string Build = "";

                if (FirmwareVersion != null && 3 == FirmwareVersion.Length)
                {
                    Build = FirmwareVersion[2].ToString("d3", CultureInfo.CurrentCulture);
                }

                return Build;
            }
        }

        /// <summary>
        /// Gets the name of the microtype based on the microtype byte.
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  05/11/15 jrf 4.20.06  WR 584075  Created
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public string MicrotypeName
        {
            get
            {
                string Name = "";

                if (Enum.IsDefined(typeof(MicrotypeRecord.MICROTYPES), MicrotypeFirmwareType))
                {
                    MicrotypeRecord.MICROTYPES ICMFWType = (MicrotypeRecord.MICROTYPES)MicrotypeFirmwareType;

                    switch (ICMFWType)
                    {
                        case MICROTYPES.GE_METERMATE_PROGRAM:
                            {
                                Name = "GE MeterMate Program";
                                break;
                            }
                        case MICROTYPES.GE_REGISTER:
                            {
                                Name = "GE Register Firmware";
                                break;
                            }
                        case MICROTYPES.ICM_MODULE_FIRMWARE:
                            {
                                Name = "Comm Module Firmware";
                                break;
                            }
                        case MICROTYPES.NXP_FIRMWARE:
                            {
                                Name = "ERT Module NXP Firmware";
                                break;
                            }
                        case MICROTYPES.PIC_FIRMWARE:
                            {
                                Name = "PIC Firmware";
                                break;
                            }
                        case MicrotypeRecord.MICROTYPES.MODEM_FIRMWARE_SIERRA_VERIZON_3G:
                        case MicrotypeRecord.MICROTYPES.MODEM_FIRMWARE_SIERRA_ATT_3G:
                        case MicrotypeRecord.MICROTYPES.MODEM_FIRMWARE_SIERRA_VERIZON_LTE:
                        case MicrotypeRecord.MICROTYPES.MODEM_FIRMWARE_SIERRA_ATT_LTE:
                        case MicrotypeRecord.MICROTYPES.MODEM_FIRMWARE_SIERRA_ROGERS_LTE:
                            {
                                Name = "Modem Firmware";
                                break;
                            }
                        default:
                            {
                                Name = "Unknown Firmware Type " + MicrotypeFirmwareType.ToString();
                                break;
                            }
                    }
                }

                return Name;
            }
        }

        #endregion

        #region Members

        private byte m_byFirmwareType;
        private byte[] m_abyHardwareVersion;
        private byte[] m_abyFirmwareVersion;
        private byte[] m_abyReserved;

        #endregion
    }

    /// <summary>
    /// MFG Table 494 - File Retrieval Actual Table
    /// </summary>
    public class ICMMfgTable2542FileRetrievalActual : AnsiTable
    {
        #region Constants

        private const int TABLE_SIZE = 3;

        #endregion

        #region Definitions
        #endregion

        #region Public Methods

        /// <summary>
        /// Table 2542 - File Retrieval Actual Table
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.01  WR 579281  Created
        //
        public ICMMfgTable2542FileRetrievalActual(CPSEM psem)
            : base(psem, 2542, TABLE_SIZE)
        {
            m_PSEM = psem;
        }

        /// <summary>
        /// Constructor that uses that data stored in a Binary Reader
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        public ICMMfgTable2542FileRetrievalActual(PSEMBinaryReader reader)
            : base(2542, TABLE_SIZE)
        {
            m_Reader = reader;
            ParseData();
            State = TableState.Loaded;
        }

        /// <summary>
        /// Reads table ICM Mfg 2542 out of the meter.
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "ICMMfgTable2542FileRetrievalActual.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get the data out of the binary reader and into the member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        private void ParseData()
        {
            m_usFileRetrievalSize = m_Reader.ReadUInt16();
            m_byFileConfigEntrySize = m_Reader.ReadByte();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Maximum length of the file retrieval data table. The actual data populated into the table
        /// might be less, but this value will be given in the return parameter for mfg proc 200, 16,
        /// and mfg proc 200, 17
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        public UInt16 FileRetrievalSize
        {
            get
            {
                ReadUnloadedTable();

                return m_usFileRetrievalSize;
            }
        }

        /// <summary>
        /// Maximum length of the arrays in mfg table 495 and 496 that hold the file
        /// and directory names. The file and directory names are ASCII null-terminated
        /// strings and thus the data is self-describing in terms of actual length
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        public byte FileConfigEntrySize
        {
            get
            {
                ReadUnloadedTable();

                return m_byFileConfigEntrySize;
            }
        }

        #endregion

        #region Members

        private UInt16 m_usFileRetrievalSize;
        private byte m_byFileConfigEntrySize;

        #endregion

    }

    /// <summary>
    /// MFG Table 495 - File Retrieval Configuration
    /// Lists the directories from which MFG table 496 will be populated
    /// </summary>
    public class ICMMfgTable2543FileRetrievalConfig : AnsiTable
    {
        #region Constants

        private const uint MINIMUM_TABLE_SIZE = 2;

        #endregion

        #region Public Methods

        /// <summary>
        /// Table 2543 - File Retrieval Configuration
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="Table2542">Table 2542 object to retrieve the size of each entry</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.01  -- ------  Created
        //
        public ICMMfgTable2543FileRetrievalConfig(CPSEM psem, ICMMfgTable2542FileRetrievalActual Table2542)
            : base(psem, 2543, MINIMUM_TABLE_SIZE)
        {
            m_PSEM = psem;
            m_blnAllowAutomaticTableResizing = true;
            m_Table2542 = Table2542;
        }

        /// <summary>
        /// Constructor that uses that data stored in a Binary Reader
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object</param>
        /// <param name="Table2542">Table 2542 object to retrieve the size of each entry</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        public ICMMfgTable2543FileRetrievalConfig(PSEMBinaryReader reader, ICMMfgTable2542FileRetrievalActual Table2542)
            : base(2543, MINIMUM_TABLE_SIZE)
        {
            m_Reader = reader;
            m_Table2542 = Table2542;

            ParseData();

            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads table ICM Mfg 2543 out of the meter.
        /// </summary>
        /// <returns>success or failure of the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "ICMMfgTable2543FileRetrievalConfig.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();

                m_TableState = TableState.Loaded;
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The number of directories, file names, or search strings in the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        public UInt16 NumberOfEntries
        {
            get
            {
                ReadUnloadedTable();

                return m_usNumberOfEntries;
            }
        }

        /// <summary>
        /// List of null-terminated ASCII strings describing the directory, file name, or search string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        public List<string> EntriesList
        {
            get
            {
                ReadUnloadedTable();

                return m_EntriesList;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get the data out of the binary reader and into the member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        private void ParseData()
        {
            if (m_Reader != null)
            {
                if (m_Reader.BaseStream.Length < MINIMUM_TABLE_SIZE)
                {
                    throw new Exception("Table size is less than the minimum size expected. Table size = "
                        + m_Size.ToString(CultureInfo.InvariantCulture));
                }
                else // m_Size >= MINIMUM_TABLE_SIZE
                {
                    m_usNumberOfEntries = m_Reader.ReadUInt16();
                    m_EntriesList = new List<string>(m_usNumberOfEntries);

                    for (int index = 0; index < m_usNumberOfEntries; index++)
                    {
                        m_EntriesList.Add(m_Reader.ReadString(m_Table2542.FileConfigEntrySize));
                    }
                }
            }
        }

        #endregion

        #region Members

        private UInt16 m_usNumberOfEntries;
        private List<string> m_EntriesList;
        private ICMMfgTable2542FileRetrievalActual m_Table2542;

        #endregion
    }

    /// <summary>
    /// MFG Table 496 - File List Table
    /// Lists the files available for retrieval
    /// </summary>
    public class ICMMfgTable2544FileListTableDefinition : AnsiTable
    {
        #region Constants

        private const uint MINIMUM_TABLE_SIZE = 2;
        private const int MAX_ENTRIES_IN_ONE_READ = 7;

        #endregion

        #region Public Methods

        /// <summary>
        /// Table 2544 - File List
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="Table2542">Table 2542 object to retrieve the size of each entry</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        public ICMMfgTable2544FileListTableDefinition(CPSEM psem, ICMMfgTable2542FileRetrievalActual Table2542)
            : base(psem, 2544, MINIMUM_TABLE_SIZE)
        {
            m_PSEM = psem;
            m_blnAllowAutomaticTableResizing = true;
            m_Table2542 = Table2542;
        }

        /// <summary>
        /// Constructor that uses that data stored in a Binary Reader
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object</param>
        /// <param name="Table2542">Table 2542 object to retrieve the size of each entry</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        public ICMMfgTable2544FileListTableDefinition(PSEMBinaryReader reader, ICMMfgTable2542FileRetrievalActual Table2542)
            : base(2544, MINIMUM_TABLE_SIZE)
        {
            m_Reader = reader;
            m_Table2542 = Table2542;

            ParseData();

            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads table ICM Mfg 2544 out of the meter.
        /// </summary>
        /// <returns>success or failure of the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "ICMMfgTable2544FileListTableDefinition.Read");

            PSEMResponse Result = base.Read(0, 2);

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_usNumberOfEntries = m_Reader.ReadUInt16();
                m_EntriesList = new List<string>(m_usNumberOfEntries);
                byte entrySize = m_Table2542.FileConfigEntrySize;
                ushort usNumberOfBytesToRead = 0;

                ChangeTableSize((uint)(m_usNumberOfEntries * entrySize + 2));

                int iNumberOfOffsetReads = m_usNumberOfEntries / MAX_ENTRIES_IN_ONE_READ;

                // It might not divide evenly, so get the remainder
                if (m_usNumberOfEntries != iNumberOfOffsetReads * MAX_ENTRIES_IN_ONE_READ)
                {
                    iNumberOfOffsetReads++;
                }

                string path = "";

                for (int iIndex = 0; iIndex < iNumberOfOffsetReads; iIndex++)
                {
                    if (iIndex == iNumberOfOffsetReads - 1)
                    {
                        // This is the last read and where we deal with reading the remainder
                        usNumberOfBytesToRead = (ushort)((m_usNumberOfEntries % MAX_ENTRIES_IN_ONE_READ) * entrySize);
                    }

                    if (usNumberOfBytesToRead == 0)
                    {
                        // We need to read the max
                        usNumberOfBytesToRead = (ushort)(MAX_ENTRIES_IN_ONE_READ * entrySize);
                    }

                    Result = base.Read(2 + iIndex * MAX_ENTRIES_IN_ONE_READ * (ushort)entrySize, usNumberOfBytesToRead);

                    if (PSEMResponse.Ok == Result)
                    {
                        for (int index = 0; index < usNumberOfBytesToRead / entrySize; index++)
                        {
                            path = m_Reader.ReadString(entrySize);
                            m_EntriesList.Add(path.Trim());
                        }
                    }
                }

                m_TableState = TableState.Loaded;
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The number of entries in the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        public UInt16 NumberOfEntries
        {
            get
            {
                ReadUnloadedTable();

                return m_usNumberOfEntries;
            }
        }

        /// <summary>
        /// The list of file names from the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        public List<string> EntriesList
        {
            get
            {
                //ReadUnloadedTable();
                PSEMResponse Result = PSEMResponse.Ok;

                if (State != TableState.Loaded)
                {
                    Result = Read();

                    if (Result != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                            Result, "Error reading table " + m_TableID.ToString(CultureInfo.CurrentCulture));
                    }
                }

                return m_EntriesList;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///  Get the data out of the binary reader and into the member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/08/15 AF  4.20.02  WR 579281  Created
        //
        private void ParseData()
        {
            if (m_Reader != null)
            {
                if (m_Reader.BaseStream.Length < MINIMUM_TABLE_SIZE)
                {
                    throw new Exception("Table size is less than the minimum size expected. Table size = "
                        + m_Size.ToString(CultureInfo.InvariantCulture));
                }
                else // m_Size >= MINIMUM_TABLE_SIZE
                {
                    m_usNumberOfEntries = m_Reader.ReadUInt16();
                    m_EntriesList = new List<string>(m_usNumberOfEntries);
                    byte entrySize = m_Table2542.FileConfigEntrySize;

                    for (int index = 0; index < m_usNumberOfEntries; index++)
                    {
                        m_EntriesList.Add(m_Reader.ReadString(entrySize));
                    }
                }
            }
        }

        #endregion

        #region Members

        private UInt16 m_usNumberOfEntries;
        private List<string> m_EntriesList;
        private ICMMfgTable2542FileRetrievalActual m_Table2542;

        #endregion
    }

    /// <summary>
    /// ICS Mfg Table Comm Module Clock Table.  It mirrors the std table 52 clock table.
    /// </summary>
    public class ICSMfgTable2751 : StdTable52
    {
        #region

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for this session</param>
        /// <param name="Table0">The table object for table 0.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ -------------------------------------------
        // 08/05/13 jrf 2.85.10 TC 12652  Created.
        //
        public ICSMfgTable2751(CPSEM psem, CTable00 Table0)
            : base(psem, Table0, 2751)
        {
        }

        #endregion
    }
}