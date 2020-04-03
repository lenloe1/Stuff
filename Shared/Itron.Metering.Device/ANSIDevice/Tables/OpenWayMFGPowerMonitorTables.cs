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
//                           Copyright © 2011 - 2012
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Threading;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Used to enumerate the phases of the meter.
    /// </summary>
    public enum Phases : byte
    {
        /// <summary>
        /// Phase A
        /// </summary>
        [EnumDescription("(a)")]
        A = 0,
        /// <summary>
        /// Phase B
        /// </summary>
        [EnumDescription("(b)")]
        B = 1,
        /// <summary>
        /// Phase C
        /// </summary>
        [EnumDescription("(c)")]
        C = 2,
    }
    
    /// <summary>
    /// MFG Table 2368 (Itron 320)
    /// </summary>
    public class OpenWayMFGTable2368 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 1;
        private const int TABLE_TIMEOUT = Timeout.Infinite;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public OpenWayMFGTable2368(CPSEM psem)
            : base(psem, 2368, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public OpenWayMFGTable2368(PSEMBinaryReader reader)
            : base(2368, TABLE_SIZE)
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
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2368.Read");

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
        /// The number of power monitor records in use.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public byte PowerMonitorRecordsUsed
        {
            get
            {
                ReadUnloadedTable();

                return m_byPMRecordsUsed;
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
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        private void ParseData()
        {
            m_byPMRecordsUsed = m_Reader.ReadByte();
        }

        #endregion

        #region Members

        byte m_byPMRecordsUsed = 0;

        #endregion
    }

    /// <summary>
    /// MFG Table 2369 (Itron 321)
    /// </summary>
    public class OpenWayMFGTable2369 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 2;
        private const int TABLE_TIMEOUT = Timeout.Infinite;
        private const byte PM_ENABLED_MASK = 0x01;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public OpenWayMFGTable2369(CPSEM psem)
            : base(psem, 2369, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public OpenWayMFGTable2369(PSEMBinaryReader reader)
            : base(2369, TABLE_SIZE)
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
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2369.Read");

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
        /// Whether or not power monitoring is enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public bool PowerMonitoringEnabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_byPMEnabled & PM_ENABLED_MASK) == PM_ENABLED_MASK;
            }
        }

        /// <summary>
        /// The amount of time after startup before power monitoring starts processing in seconds.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public byte ColdLoadTime
        {
            get
            {
                ReadUnloadedTable();

                return m_byColdLoadTime;
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
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        private void ParseData()
        {
            m_byPMEnabled = m_Reader.ReadByte();
            m_byColdLoadTime = m_Reader.ReadByte();
        }

        #endregion

        #region Members

        byte m_byPMEnabled = 0;
        byte m_byColdLoadTime = 0;

        #endregion
    }

    
    /// <summary>
    /// MFG Table 2370 (Itron 322)
    /// </summary>
    public class OpenWayMFGTable2370 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 1000;
        private const int NUMBER_OF_MONOPHASE_RECORDS = 8;
        private const int NUMBER_OF_POLYPHASE_RECORDS = 32;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object</param>
        /// <param name="Table0"></param>
        /// <param name="Table2368"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public OpenWayMFGTable2370(CPSEM psem, CTable00 Table0, OpenWayMFGTable2368 Table2368)
            : base(psem, 2370, GetTableSize(Table0, Table2368), TABLE_TIMEOUT)
        {
            m_iTimeFormat = Table0.TimeFormat;
            m_byPowerMonitorRecordsUsed = Table2368.PowerMonitorRecordsUsed;
            //Get the resource manager
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS, this.GetType().Assembly);
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="Table0"></param>
        /// <param name="Table2368"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public OpenWayMFGTable2370(PSEMBinaryReader reader, CTable00 Table0, OpenWayMFGTable2368 Table2368)
            : base(2370, GetTableSize(Table0, Table2368))
        {
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS, this.GetType().Assembly);
            m_iTimeFormat = Table0.TimeFormat;
            m_byPowerMonitorRecordsUsed = Table2368.PowerMonitorRecordsUsed;
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
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2370.Read");

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
        /// The Instantaneous Watts Delivered values.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public InstantaneousQuantity InstantaneousWattsDelivered
        {
            get
            {
                ReadUnloadedTable();

                return m_InsWattsDelivered;
            }
        }

        /// <summary>
        /// The Instantaneous Watts Received values.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public InstantaneousQuantity InstantaneousWattsReceived
        {
            get
            {
                ReadUnloadedTable();

                return m_InsWattsReceived;
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
        //  06/07/11 jrf 2.51.10 173353 Created
        //  03/20/12 AF  2.53.51 194597 Removed the "aggregate" label and changed the phase descriptions
        //                              in the enum above
        //
        private void ParseData()
        {
            if (null == m_InsWattsDelivered)
            {
                m_InsWattsDelivered = new InstantaneousQuantity("Instantaneous Watts Delivered");
            }
            else
            {
                m_InsWattsDelivered.Refresh();
            }

            if (null == m_InsWattsReceived)
            {
                m_InsWattsReceived = new InstantaneousQuantity("Instantaneous Watts Received");
            }
            else
            {
                m_InsWattsReceived.Refresh();
            }

            if (NUMBER_OF_MONOPHASE_RECORDS == m_byPowerMonitorRecordsUsed)
            {
                m_InsWattsDelivered.MostRecentMeasurement = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), "Ins W d");
                m_InsWattsDelivered.Minimum = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), "Ins min W d");
                m_InsWattsDelivered.Maximum = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), "Ins max W d");
                m_InsWattsDelivered.Average = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), "Ins avg W d");

                m_InsWattsReceived.MostRecentMeasurement = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), "Ins W r");
                m_InsWattsReceived.Minimum = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), "Ins min W r");
                m_InsWattsReceived.Maximum = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), "Ins max W r");
                m_InsWattsReceived.Average = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), "Ins avg W r");
            }
            else //Have to read the measurements for each phase.
            {
                m_InsWattsDelivered.MostRecentMeasurementsPerPhase = new List<DemandMeasurement>();
                m_InsWattsDelivered.MinimumPerPhase = new List<DemandMeasurement>();
                m_InsWattsDelivered.MaximumPerPhase = new List<DemandMeasurement>();
                m_InsWattsDelivered.AveragePerPhase = new List<DemandMeasurement>();

                m_InsWattsReceived.MostRecentMeasurementsPerPhase = new List<DemandMeasurement>();
                m_InsWattsReceived.MinimumPerPhase = new List<DemandMeasurement>();
                m_InsWattsReceived.MaximumPerPhase = new List<DemandMeasurement>();
                m_InsWattsReceived.AveragePerPhase = new List<DemandMeasurement>();

                foreach (Phases Phase in Enum.GetValues(typeof(Phases)))
                {
                    m_InsWattsDelivered.MostRecentMeasurementsPerPhase.Add(new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("INS_W_D") + " " + EnumDescriptionRetriever.RetrieveDescription(Phase)));
                    m_InsWattsDelivered.MinimumPerPhase.Add(new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("MIN_INS_W_D") + " " + EnumDescriptionRetriever.RetrieveDescription(Phase)));
                    m_InsWattsDelivered.MaximumPerPhase.Add(new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("MAX_INS_W_D") + " " + EnumDescriptionRetriever.RetrieveDescription(Phase)));
                    m_InsWattsDelivered.AveragePerPhase.Add(new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("AVG_INS_W_D") + " " + EnumDescriptionRetriever.RetrieveDescription(Phase)));
                    
                }

                m_InsWattsDelivered.MostRecentMeasurement = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("INS_W_D"));
                m_InsWattsDelivered.Minimum = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("MIN_INS_W_D"));
                m_InsWattsDelivered.Maximum = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("MAX_INS_W_D"));
                m_InsWattsDelivered.Average = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("AVG_INS_W_D"));
                
                foreach (Phases Phase in Enum.GetValues(typeof(Phases)))
                {
                    m_InsWattsReceived.MostRecentMeasurementsPerPhase.Add(new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("INS_W_R") + " " + EnumDescriptionRetriever.RetrieveDescription(Phase)));
                    m_InsWattsReceived.MinimumPerPhase.Add(new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("MIN_INS_W_R") + " " + EnumDescriptionRetriever.RetrieveDescription(Phase)));
                    m_InsWattsReceived.MaximumPerPhase.Add(new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("MAX_INS_W_R") + " " + EnumDescriptionRetriever.RetrieveDescription(Phase)));
                    m_InsWattsReceived.AveragePerPhase.Add(new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("AVG_INS_W_R") + " " + EnumDescriptionRetriever.RetrieveDescription(Phase)));
                    
                }

                m_InsWattsReceived.MostRecentMeasurement = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("INS_W_R"));
                m_InsWattsReceived.Minimum = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("MIN_INS_W_R"));
                m_InsWattsReceived.Maximum = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("MAX_INS_W_R"));
                m_InsWattsReceived.Average = new DemandMeasurement(m_Reader.ReadDouble(), m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_iTimeFormat), m_rmStrings.GetString("AVG_INS_W_R"));
            }
        }

        /// <summary>
        /// Gets the size of the table in bytes.
        /// </summary>
        /// <param name="Table0">The table 0 object for the current device</param>
        /// <param name="Table2368">The table 2368 object for the current device</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        private static uint GetTableSize(CTable00 Table0, OpenWayMFGTable2368 Table2368)
        {
            uint uiNumberRecords = Table2368.PowerMonitorRecordsUsed;

            if (NUMBER_OF_MONOPHASE_RECORDS != uiNumberRecords)
            {
                uiNumberRecords = NUMBER_OF_POLYPHASE_RECORDS;
            }

            return (8 + Table0.STIMESize) * uiNumberRecords;
        }

        #endregion

        #region Members

        private byte m_byPowerMonitorRecordsUsed;
        private InstantaneousQuantity m_InsWattsDelivered;
        private InstantaneousQuantity m_InsWattsReceived;
        private int m_iTimeFormat = 0;
        internal static System.Resources.ResourceManager m_rmStrings;
        internal static readonly string RESOURCE_FILE_PROJECT_STRINGS =
                                    "Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings";

        #endregion

    }

    
}