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
//                              Copyright © 2015
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using System.Globalization;

using System.Windows.Forms;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class that describes the DataSet Configuration of mfg table 2265
    /// </summary>
    public class MFGTable2265DataSetConfiguration : ANSISubTable
    {
        #region Constants

        private const ushort DATASET_CONFIGURATION_TBL_OFFSET = 330;
        private const ushort DATASET_CONFIGURATION_TBL_SIZE = 153;
        private const ushort NUMBER_OF_DATASET_CONFIG_RCD = 3;
        private const ushort DATASET_ID_LENGTH = 10;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public MFGTable2265DataSetConfiguration(CPSEM psem)
            : base(psem, 2265, DATASET_CONFIGURATION_TBL_OFFSET, DATASET_CONFIGURATION_TBL_SIZE)
        {
            m_DataSetConfiguration = null;
        }

        /// <summary>
        /// Constructor used to get data from the EDL file
        /// </summary>
        /// <param name="reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public MFGTable2265DataSetConfiguration(PSEMBinaryReader reader)
            : base(2265, DATASET_CONFIGURATION_TBL_SIZE)
        {
            m_DataSetConfiguration = null;
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the DataSet Configuration out of Mfg table 2265
        /// </summary>
        /// <returns>PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "DataSetConfiguration.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;
                ParseData();
            }

            return Result;
        }

        /// <summary>
        ///  Writes the data to the log file for debugging purposes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //  11/06/15 PGH 4.50.213 577471 Added VM Interval Data
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.UInt32.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                   "Dump of DataSet Configuration.");

                for (int Index = 0; Index < NUMBER_OF_DATASET_CONFIG_RCD; Index++)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "DataSet Config Record " + (Index + 1).ToString());

                    string MonitoredLids = "";

                    for (int LidIndex = 0; LidIndex < m_DataSetConfiguration[Index].MonitoredLids.Length; LidIndex++)
                    {
                        MonitoredLids += m_DataSetConfiguration[Index].MonitoredLids[LidIndex].ToString() + " ";
                    }

                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Monitored Lids: " + MonitoredLids);
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Data Set Id: " + m_DataSetConfiguration[Index].DataSetId);
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "VM Interval Data: " + m_DataSetConfiguration[Index].VMIntervalData.ToString());
                }
            }
            catch (Exception e)
            {
                try
                {
                    m_Logger.WriteException(this, e);
                }
                catch
                {
                    // No exceptions thrown from this debugging method
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the DataSet Configuration from table 2265
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //  12/22/15 PGH 4.50.222 577471 Updated
        //
        public DataSetConfigRcd[] DataSetConfiguration
        {
            get
            {
                ReadUnloadedTable();

                return m_DataSetConfiguration;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the DataSet Configuration out of the stream.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //  11/06/15 PGH 4.50.213 577471 Added VM Interval Data
        //
        private void ParseData()
        {
            m_DataSetConfiguration = new DataSetConfigRcd[NUMBER_OF_DATASET_CONFIG_RCD];

            for (int Index = 0; Index < NUMBER_OF_DATASET_CONFIG_RCD; Index++)
            {
                m_DataSetConfiguration[Index] = new DataSetConfigRcd();

                for (int LidIndex = 0; LidIndex < m_DataSetConfiguration[Index].MonitoredLids.Length; LidIndex++)
                {
                    m_DataSetConfiguration[Index].MonitoredLids[LidIndex] = m_Reader.ReadUInt32();
                }

                byte[] DataSetId = new byte[DATASET_ID_LENGTH];
                for (int i = 0; i < DATASET_ID_LENGTH; i++)
                {
                    DataSetId[i] = m_Reader.ReadByte();
                }
                m_DataSetConfiguration[Index].DataSetId = System.Text.Encoding.UTF8.GetString(DataSetId);

                m_DataSetConfiguration[Index].VMIntervalData = m_Reader.ReadBoolean();
            }
        }

        #endregion


        #region Members

        private DataSetConfigRcd[] m_DataSetConfiguration;

        #endregion

    }

    /// <summary>
    /// Class that represents a Data Set Configuration Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue# Description
    //  -------- --- -------  ------ -------------------------------------------
    //  10/23/15 PGH 4.50.208 577471 Created
    //  11/06/15 PGH 4.50.213 577471 Added VM Interval Data
    //
    public class DataSetConfigRcd
    {
        #region Constants

        private const ushort NUMBER_OF_MONITORED_LIDS = 10;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //  11/06/15 PGH 4.50.213 577471 Added VM Interval Data
        //
        public DataSetConfigRcd()
        {
            m_auiMonitoredLids = new uint[NUMBER_OF_MONITORED_LIDS];
            m_strDataSetId = "";
            m_blnVMIntervalData = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Monitored Lids
        /// </summary>
        public uint[] MonitoredLids
        {
            get
            {
                return m_auiMonitoredLids;
            }
            set
            {
                m_auiMonitoredLids = value;
            }
        }

        /// <summary>
        /// Data Set Id
        /// </summary>
        public string DataSetId
        {
            get
            {
                return m_strDataSetId;
            }
            set
            {
                m_strDataSetId = value;
            }
        }

        /// <summary>
        /// VM Interval Data
        /// </summary>
        public bool VMIntervalData
        {
            get
            {
                return m_blnVMIntervalData;
            }
            set
            {
                m_blnVMIntervalData = value;
            }
        }

        #endregion

        #region Members

        private uint[] m_auiMonitoredLids;
        private string m_strDataSetId;
        private bool m_blnVMIntervalData;

        #endregion
    }

    /// <summary>
    /// Class that represents a Push Group Configuration Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue# Description
    //  -------- --- -------  ------ -------------------------------------------
    //  10/23/15 PGH 4.50.208 577471 Created
    //
    public class PushGroupConfigRcd
    {
        #region Constants

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public PushGroupConfigRcd()
        {
            m_strGroupKey = "";
            m_dtStartPushTime = TimeSpan.Zero;
            m_dtEndPushTime = TimeSpan.Zero;
            m_byFrequencyMin = 0;
            m_byDelayMin = 0;
            m_byWindowMin = 0;
            m_byWindowSec = 0;
            m_strDataSetId = "";
            m_blnEnableGroup = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Group Key
        /// </summary>
        public string GroupKey
        {
            get
            {
                return m_strGroupKey;
            }
            set
            {
                m_strGroupKey = value;
            }
        }

        /// <summary>
        /// Start Push Time
        /// </summary>
        public TimeSpan StartPushTime
        {
            get
            {
                return m_dtStartPushTime;
            }
            set
            {
                m_dtStartPushTime = value;
            }
        }

        /// <summary>
        /// End Push Time
        /// </summary>
        public TimeSpan EndPushTime
        {
            get
            {
                return m_dtEndPushTime;
            }
            set
            {
                m_dtEndPushTime = value;
            }
        }

        /// <summary>
        /// Frequency Minutes
        /// </summary>
        public byte FrequencyMin
        {
            get
            {
                return m_byFrequencyMin;
            }
            set
            {
                m_byFrequencyMin = value;
            }
        }

        /// <summary>
        /// Delay Minutes
        /// </summary>
        public byte DelayMin
        {
            get
            {
                return m_byDelayMin;
            }
            set
            {
                m_byDelayMin = value;
            }
        }

        /// <summary>
        /// Delay Seconds
        /// </summary>
        public byte DelaySec
        {
            get
            {
                return m_byDelaySec;
            }
            set
            {
                m_byDelaySec = value;
            }
        }

        /// <summary>
        /// Window Minutes
        /// </summary>
        public byte WindowMin
        {
            get
            {
                return m_byWindowMin;
            }
            set
            {
                m_byWindowMin = value;
            }
        }

        /// <summary>
        /// Window Seconds
        /// </summary>
        public byte WindowSec
        {
            get
            {
                return m_byWindowSec;
            }
            set
            {
                m_byWindowSec = value;
            }
        }

        /// <summary>
        /// Data Set Id
        /// </summary>
        public string DataSetId
        {
            get
            {
                return m_strDataSetId;
            }
            set
            {
                m_strDataSetId = value;
            }
        }

        /// <summary>
        /// Enable Group
        /// </summary>
        public bool EnableGroup
        {
            get
            {
                return m_blnEnableGroup;
            }
            set
            {
                m_blnEnableGroup = value;
            }
        }

        #endregion

        #region Members

        private string m_strGroupKey;
        private TimeSpan m_dtStartPushTime;
        private TimeSpan m_dtEndPushTime;
        private byte m_byFrequencyMin;
        private byte m_byDelayMin;
        private byte m_byDelaySec;
        private byte m_byWindowMin;
        private byte m_byWindowSec;
        private string m_strDataSetId;
        private bool m_blnEnableGroup;

        #endregion
    }

    /// <summary>
    /// Class that represents a Push Configuration Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue# Description
    //  -------- --- -------  ------ -------------------------------------------
    //  10/23/15 PGH 4.50.208 577471 Created
    //
    public class PushConfigRcd
    {
        #region Constants

        private const ushort NUMBER_OF_MONITORED_LIDS = 10;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public PushConfigRcd()
        {
            m_blnGlobalEnable = false;
            m_uiTransmitErrors = 0;
            m_uiSuccessfulTransmits = 0;
            m_MonitoredLids = new uint[NUMBER_OF_MONITORED_LIDS];
            m_VMIntervalCapture = false;
            m_PushGroupConfigRcd = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Global Enable
        /// </summary>
        public bool GlobalEnable
        {
            get
            {
                return m_blnGlobalEnable;
            }
            set
            {
                m_blnGlobalEnable = value;
            }
        }

        /// <summary>
        /// Transmit Errors
        /// </summary>
        public uint TransmitErrors
        {
            get
            {
                return m_uiTransmitErrors;
            }
            set
            {
                m_uiTransmitErrors = value;
            }
        }

        /// <summary>
        /// Successful Transmits
        /// </summary>
        public uint SuccessfulTransmits
        {
            get
            {
                return m_uiSuccessfulTransmits;
            }
            set
            {
                m_uiSuccessfulTransmits = value;
            }
        }

        /// <summary>
        /// Monitored Lids
        /// </summary>
        public uint[] MonitoredLids
        {
            get
            {
                return m_MonitoredLids;
            }
            set
            {
                m_MonitoredLids = value;
            }
        }

        /// <summary>
        /// VM Interval Capture
        /// </summary>
        public bool VMIntervalCapture
        {
            get
            {
                return m_VMIntervalCapture;
            }
            set
            {
                m_VMIntervalCapture = value;
            }
        }

        /// <summary>
        /// Push Group Config Record
        /// </summary>
        public PushGroupConfigRcd PushGroupConfigRcd
        {
            get
            {
                return m_PushGroupConfigRcd;
            }
            set
            {
                m_PushGroupConfigRcd = value;
            }
        }

        #endregion

        #region Members

        private bool m_blnGlobalEnable;
        private uint m_uiTransmitErrors;
        private uint m_uiSuccessfulTransmits;
        private uint[] m_MonitoredLids;
        private bool m_VMIntervalCapture;
        private PushGroupConfigRcd m_PushGroupConfigRcd;

        #endregion
    }

    /// <summary>
    /// Class that describes mfg table 2185 - Bell Weather Configuration
    /// </summary>
    public class OpenWayMFGTable2185 : AnsiTable
    {
        #region Constants

        private const ushort NUMBER_OF_PUSH_CONFIG_RCD = 3;
        private const int TABLE_SIZE = 234;
        private const int TABLE_TIMEOUT = 1000;
        private const ushort GROUP_KEY_LENGTH = 10;
        private const ushort DATASET_ID_LENGTH = 10;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public OpenWayMFGTable2185(CPSEM psem)
            : base(psem, 2185, TABLE_SIZE, TABLE_TIMEOUT)
        {
            m_BellWeatherConfigRcd = null;
        }

        /// <summary>
        /// Constructor used to get Config Data from the EDL file
        /// </summary>
        /// <param name="reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public OpenWayMFGTable2185(PSEMBinaryReader reader)
            : base(2185, TABLE_SIZE)
        {
            m_BellWeatherConfigRcd = null;
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads Mfg table 2185
        /// </summary>
        /// <returns>PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "MFGTable2185.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;
                ParseData();
            }

            return Result;
        }

        /// <summary>
        ///  Writes the data to the log file for debugging purposes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.UInt32.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                   "Dump of the Bell Weather Configuration - Mfg table 2185.");

                for (int Index = 0; Index < NUMBER_OF_PUSH_CONFIG_RCD; Index++)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Push Config Record " + (Index + 1).ToString());
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Global Enable: " + m_BellWeatherConfigRcd[Index].GlobalEnable.ToString());
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Transmit Errors: " + m_BellWeatherConfigRcd[Index].TransmitErrors.ToString());
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Successful Transmits: " + m_BellWeatherConfigRcd[Index].SuccessfulTransmits.ToString());

                    string MonitoredLids = "";
                    for (int i = 0; i < m_BellWeatherConfigRcd[Index].MonitoredLids.Length; i++)
                    {
                        MonitoredLids += m_BellWeatherConfigRcd[Index].MonitoredLids[i].ToString() + " ";
                    }
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Monitored Lids: " + MonitoredLids);

                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "VM Interval Capture: " + m_BellWeatherConfigRcd[Index].VMIntervalCapture.ToString());

                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Push Group Config Record ");
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Group Key: " + m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.GroupKey);
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Start Push Time: " + m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.StartPushTime.ToString());
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "End Push Time: " + m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.EndPushTime.ToString());
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Frequency Minutes: " + m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.FrequencyMin.ToString());
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Delay Minutes: " + m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.ToString());
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Delay Seconds: " + m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.DelaySec.ToString());
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Window Minutes: " + m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.WindowMin.ToString());
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Window Seconds: " + m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.WindowSec.ToString());
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Data Set Id: " + m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.DataSetId);
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Enable Group: " + m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.EnableGroup.ToString());
                }

            }
            catch (Exception e)
            {
                try
                {
                    m_Logger.WriteException(this, e);
                }
                catch
                {
                    // No exceptions thrown from this debugging method
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Bell Weather Configuration Record - Mfg Table 2185
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //  12/22/15 PGH 4.50.222 577471 Updated
        //
        public PushConfigRcd[] BellWeatherConfigRcd
        {
            get
            {

                ReadUnloadedTable();

                return m_BellWeatherConfigRcd;
            }
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Get the size of the table.
        /// </summary>
        /// <returns>The size of the table in bytes.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        private static uint GetTableSize()
        {
            uint uiGlobalEnable = 1;
            uint uiTransmitErrors = 1;
            uint uiSuccessfulTransmits = 1;

            // Data Set Configuration
            uint uiDataSetConfiguration = (4 * 10);  // Monitored Lids : Array[10] Uint32
            uiDataSetConfiguration += 1; // VM Interval Capture : Boolean

            // Push Group Configuration Record
            uint uiPushGroupConfig = 10; // Group Key : Array[10] Uint8
            uiPushGroupConfig += 4; // Start Push Time : ANSITIME - Uint32
            uiPushGroupConfig += 4; // End Push Time : ANSITIME - Uint32
            uiPushGroupConfig += 1; // Frequency Minutes : Uint8
            uiPushGroupConfig += 1; // Delay Minutes : Uint8
            uiPushGroupConfig += 1; // Delay Seconds : Uint8
            uiPushGroupConfig += 1; // Window Minutes : Uint8
            uiPushGroupConfig += 1; // Window Seconds : Uint8
            uiPushGroupConfig += 10; // DataSet Id : Array[10] Uint8
            uiPushGroupConfig += 1; // Enable Group : Boolean

            uint uiPushConfig = uiGlobalEnable + uiTransmitErrors + uiSuccessfulTransmits + uiDataSetConfiguration + uiPushGroupConfig;

            uint uiTableSize = 3 * uiPushConfig;

            return uiTableSize;
        }

        /// <summary>
        /// Parses the Bell Weather Configuration out of the stream.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        private void ParseData()
        {
            m_BellWeatherConfigRcd = new PushConfigRcd[NUMBER_OF_PUSH_CONFIG_RCD];

            for (int Index = 0; Index < NUMBER_OF_PUSH_CONFIG_RCD; Index++)
            {
                m_BellWeatherConfigRcd[Index] = new PushConfigRcd();

                m_BellWeatherConfigRcd[Index].GlobalEnable = m_Reader.ReadBoolean();
                m_BellWeatherConfigRcd[Index].TransmitErrors = m_Reader.ReadUInt32();
                m_BellWeatherConfigRcd[Index].SuccessfulTransmits = m_Reader.ReadUInt32();

                for (int i = 0; i < m_BellWeatherConfigRcd[Index].MonitoredLids.Length; i++)
                {
                    m_BellWeatherConfigRcd[Index].MonitoredLids[i] = m_Reader.ReadUInt32();
                }

                m_BellWeatherConfigRcd[Index].VMIntervalCapture = m_Reader.ReadBoolean();

                m_BellWeatherConfigRcd[Index].PushGroupConfigRcd = new PushGroupConfigRcd();

                byte[] GroupKey = new byte[GROUP_KEY_LENGTH];
                for (int i = 0; i < GROUP_KEY_LENGTH; i++)
                {
                    GroupKey[i] = m_Reader.ReadByte();
                }
                m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.GroupKey = System.Text.Encoding.UTF8.GetString(GroupKey);

                m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.StartPushTime = m_Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.EndPushTime = m_Reader.ReadTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.FrequencyMin = m_Reader.ReadByte();
                m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.DelayMin = m_Reader.ReadByte();
                m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.DelaySec = m_Reader.ReadByte();
                m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.WindowMin = m_Reader.ReadByte();
                m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.WindowSec = m_Reader.ReadByte();

                byte[] DataSetId = new byte[DATASET_ID_LENGTH];
                for (int i = 0; i < DATASET_ID_LENGTH; i++)
                {
                    DataSetId[i] = m_Reader.ReadByte();
                }
                m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.DataSetId = System.Text.Encoding.UTF8.GetString(DataSetId);

                m_BellWeatherConfigRcd[Index].PushGroupConfigRcd.EnableGroup = m_Reader.ReadBoolean();
            }

        }


        #endregion

        #region Members

        private PushConfigRcd[] m_BellWeatherConfigRcd;

        #endregion

    }

    /// <summary>
    /// Class that represents the VM Interval Data Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue# Description
    //  -------- --- -------  ------ -------------------------------------------
    //  11/06/15 PGH 4.50.213 577471 Created
    //
    public class VMIntervalDataRcd
    {
        #region Constants

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  11/06/15 PGH 4.50.213 577471 Created
        //
        public VMIntervalDataRcd()
        {
            m_dtTimestampGMT = DateTime.MinValue;
            m_usIntervalStatus = 0;
            m_ausIntervalData = null;
            m_ausVMMinVoltage = null;
            m_ausVMMaxVoltage = null;
            m_bytValidPhases = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Timestamp GMT
        /// </summary>
        public DateTime TimestampGMT
        {
            get
            {
                return m_dtTimestampGMT;
            }
            set
            {
                m_dtTimestampGMT = value;
            }
        }

        /// <summary>
        /// Interval Status
        /// </summary>
        public ushort IntervalStatus
        {
            get
            {
                return m_usIntervalStatus;
            }
            set
            {
                m_usIntervalStatus = value;
            }
        }

        /// <summary>
        /// Interval Data
        /// </summary>
        public ushort[] IntervalData
        {
            get
            {
                return m_ausIntervalData;
            }
            set
            {
                m_ausIntervalData = value;
            }
        }

        /// <summary>
        /// VM Min Voltage
        /// </summary>
        public ushort[] VMMinVoltage
        {
            get
            {
                return m_ausVMMinVoltage;
            }
            set
            {
                m_ausVMMinVoltage = value;
            }
        }

        /// <summary>
        /// VM Max Voltage
        /// </summary>
        public ushort[] VMMaxVoltage
        {
            get
            {
                return m_ausVMMaxVoltage;
            }
            set
            {
                m_ausVMMaxVoltage = value;
            }
        }

        /// <summary>
        /// Valid Phases
        /// </summary>
        public byte ValidPhases
        {
            get
            {
                return m_bytValidPhases;
            }
            set
            {
                m_bytValidPhases = value;
            }
        }
        
        #endregion

        #region Members

        private DateTime m_dtTimestampGMT;
        private ushort m_usIntervalStatus;
        private ushort[] m_ausIntervalData;
        private ushort[] m_ausVMMinVoltage;
        private ushort[] m_ausVMMaxVoltage;
        private byte m_bytValidPhases;

        #endregion
    }

    /// <summary>
    /// Class that represents the Group Data Status Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue# Description
    //  -------- --- -------  ------ -------------------------------------------
    //  10/23/15 PGH 4.50.208 577471 Created
    //  11/06/15 PGH 4.50.213 577471 Added VM Interval Data Record
    //
    public class GroupDataStatusRcd
    {
        #region Constants

        private const ushort NUMBER_OF_LIDS = 10;
        private const ushort LID_DATA_LENGTH = 10;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public GroupDataStatusRcd()
        {
            m_dtTimestampGMT = DateTime.MinValue;
            m_strGroupKey = "";
            m_strDataSetKey = "";
            m_auiLids = new uint[NUMBER_OF_LIDS];
            m_auiLidData = new uint[LID_DATA_LENGTH];
            m_VMIntervalDataRcd = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Timestamp GMT
        /// </summary>
        public DateTime TimestampGMT
        {
            get
            {
                return m_dtTimestampGMT;
            }
            set
            {
                m_dtTimestampGMT = value;
            }
        }

        /// <summary>
        /// Group Key
        /// </summary>
        public string GroupKey
        {
            get
            {
                return m_strGroupKey;
            }
            set
            {
                m_strGroupKey = value;
            }
        }

        /// <summary>
        /// DataSet Key
        /// </summary>
        public string DataSetKey
        {
            get
            {
                return m_strDataSetKey;
            }
            set
            {
                m_strDataSetKey = value;
            }
        }

        /// <summary>
        /// Lids
        /// </summary>
        public uint[] Lids
        {
            get
            {
                return m_auiLids;
            }
            set
            {
                m_auiLids = value;
            }
        }

        /// <summary>
        /// Lid Data
        /// </summary>
        public uint[] LidData
        {
            get
            {
                return m_auiLidData;
            }
            set
            {
                m_auiLidData = value;
            }
        }

        /// <summary>
        /// VM Interval Data Record
        /// </summary>
        public VMIntervalDataRcd VMIntervalDataRecord
        {
            get
            {
                return m_VMIntervalDataRcd;
            }
            set
            {
                m_VMIntervalDataRcd = value;
            }
        }

        #endregion

        #region Members

        private DateTime m_dtTimestampGMT;
        private string m_strGroupKey;
        private string m_strDataSetKey;
        private uint[] m_auiLids;
        private uint[] m_auiLidData;
        private VMIntervalDataRcd m_VMIntervalDataRcd;

        #endregion
    }

    /// <summary>
    /// Class that describes mfg table 2186 - Group Data Status
    /// </summary>
    public class OpenWayMFGTable2186 : AnsiTable
    {
        #region Constants

        private const int TABLE_SIZE = 131;
        private const int TABLE_TIMEOUT = 1000;
        private const ushort GROUP_KEY_LENGTH = 10;
        private const ushort DATASET_KEY_LENGTH = 10;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public OpenWayMFGTable2186(CPSEM psem)
            : base(psem, 2186, TABLE_SIZE, TABLE_TIMEOUT)
        {
            m_BellWeatherGroupDataStatusRcd = null;
        }

        /// <summary>
        /// Constructor used to get Config Data from the EDL file
        /// </summary>
        /// <param name="reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public OpenWayMFGTable2186(PSEMBinaryReader reader)
            : base(2186, TABLE_SIZE)
        {
            m_BellWeatherGroupDataStatusRcd = null;
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads Mfg table 2186
        /// </summary>
        /// <returns>PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "MFGTable2186.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;
                ParseData();
            }

            return Result;
        }

        /// <summary>
        ///  Writes the data to the log file for debugging purposes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.UInt16.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.UInt32.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.DateTime.ToString")]
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                   "Dump of the Bell Weather Group Data Status Record - Mfg table 2186.");

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Timestamp GMT: " + m_BellWeatherGroupDataStatusRcd.TimestampGMT.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Group Key: " + m_BellWeatherGroupDataStatusRcd.GroupKey);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "DataSet Key: " + m_BellWeatherGroupDataStatusRcd.DataSetKey);

                string Lids = "";
                for (int Index = 0; Index < m_BellWeatherGroupDataStatusRcd.Lids.Length; Index++)
                {
                    Lids += m_BellWeatherGroupDataStatusRcd.Lids[Index].ToString() + " ";
                }
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Lids: " + Lids);

                string LidData = "";
                for (int Index = 0; Index < m_BellWeatherGroupDataStatusRcd.LidData.Length; Index++)
                {
                    LidData += m_BellWeatherGroupDataStatusRcd.LidData[Index].ToString() + " ";
                }
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Lid Data: " + LidData);

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "VM Interval Data Record");

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "TimestamepGMT: " + m_BellWeatherGroupDataStatusRcd.VMIntervalDataRecord.TimestampGMT.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Interval Status: " + m_BellWeatherGroupDataStatusRcd.VMIntervalDataRecord.IntervalStatus.ToString());
                string strIntervalValues = m_BellWeatherGroupDataStatusRcd.VMIntervalDataRecord.IntervalData[0].ToString() + " ";
                strIntervalValues += m_BellWeatherGroupDataStatusRcd.VMIntervalDataRecord.IntervalData[1].ToString() + " ";
                strIntervalValues += m_BellWeatherGroupDataStatusRcd.VMIntervalDataRecord.IntervalData[2].ToString();
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Interval Data: " + strIntervalValues);

                strIntervalValues = m_BellWeatherGroupDataStatusRcd.VMIntervalDataRecord.VMMinVoltage[0].ToString() + " ";
                strIntervalValues += m_BellWeatherGroupDataStatusRcd.VMIntervalDataRecord.VMMinVoltage[1].ToString() + " ";
                strIntervalValues += m_BellWeatherGroupDataStatusRcd.VMIntervalDataRecord.VMMinVoltage[2].ToString();
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "VM Min Voltage: " + strIntervalValues);

                strIntervalValues = m_BellWeatherGroupDataStatusRcd.VMIntervalDataRecord.VMMaxVoltage[0].ToString() + " ";
                strIntervalValues += m_BellWeatherGroupDataStatusRcd.VMIntervalDataRecord.VMMaxVoltage[1].ToString() + " ";
                strIntervalValues += m_BellWeatherGroupDataStatusRcd.VMIntervalDataRecord.VMMaxVoltage[2].ToString();
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "VM Max Voltage: " + strIntervalValues);

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Valid Phases: " + m_BellWeatherGroupDataStatusRcd.VMIntervalDataRecord.ValidPhases.ToString());

            }
            catch (Exception e)
            {
                try
                {
                    m_Logger.WriteException(this, e);
                }
                catch
                {
                    // No exceptions thrown from this debugging method
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Bell Weather Group Data Status Record from table 2186
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //  12/22/15 PGH 4.50.222 577471 Updated
        //
        public GroupDataStatusRcd BellWeatherGroupDataStatusRcd
        {
            get
            {

                ReadUnloadedTable();

                return m_BellWeatherGroupDataStatusRcd;
            }
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Get the size of the table.
        /// </summary>
        /// <returns>The size of the table in bytes.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        private static uint GetTableSize()
        {
            // VM Interval Data Record
            uint uiVMIntervalDataRecordSize = 5; // Timestamp GMT : LTIME - 4 bytes for minutes 1 byte for seconds
            uiVMIntervalDataRecordSize += 2; // Interval Status
            uiVMIntervalDataRecordSize += 6; // Interval Data - array of 3 Uint16
            uiVMIntervalDataRecordSize += 6; // VM Min Voltage - array of 3 Uint16
            uiVMIntervalDataRecordSize += 6; // VM Max Voltage - array of 3 Uint16
            uiVMIntervalDataRecordSize += 1; // Valid Phases

            // Group Data Status Record
            uint uiGroupDataStatusRecordSize = 5; // Timestamp GMT : LTIME - 4 bytes for minutes 1 byte for seconds
            uiGroupDataStatusRecordSize += 10; // Group Key : Array[10] Uint8
            uiGroupDataStatusRecordSize += 10; // DataSet Key : Array[10] Uint8
            uiGroupDataStatusRecordSize += (4 * 10); // Lids : Array[10] Uint32
            uiGroupDataStatusRecordSize += (4 * 10); // Lid Data : Array[10] Uint32
            uiGroupDataStatusRecordSize += uiVMIntervalDataRecordSize; // VM Interval Data Record

            return uiGroupDataStatusRecordSize;
        }

        /// <summary>
        /// Parses the Bell Weather Group Status Record out of the stream.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        private void ParseData()
        {
            m_BellWeatherGroupDataStatusRcd = new GroupDataStatusRcd();

            m_BellWeatherGroupDataStatusRcd.TimestampGMT = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);

            byte[] GroupKey = new byte[GROUP_KEY_LENGTH];
            for (int i = 0; i < GROUP_KEY_LENGTH; i++)
            {
                GroupKey[i] = m_Reader.ReadByte();
            }
            m_BellWeatherGroupDataStatusRcd.GroupKey = System.Text.Encoding.UTF8.GetString(GroupKey);

            byte[] DataSetKey = new byte[DATASET_KEY_LENGTH];
            for (int i = 0; i < DATASET_KEY_LENGTH; i++)
            {
                DataSetKey[i] = m_Reader.ReadByte();
            }
            m_BellWeatherGroupDataStatusRcd.DataSetKey = System.Text.Encoding.UTF8.GetString(DataSetKey);

            for (int Index = 0; Index < m_BellWeatherGroupDataStatusRcd.Lids.Length; Index++)
            {
                m_BellWeatherGroupDataStatusRcd.Lids[Index] = m_Reader.ReadUInt32();
            }

            for (int Index = 0; Index < m_BellWeatherGroupDataStatusRcd.LidData.Length; Index++)
            {
                m_BellWeatherGroupDataStatusRcd.LidData[Index] = m_Reader.ReadUInt32();
            }

            VMIntervalDataRcd VMIntervalDataRcd = new VMIntervalDataRcd();

            VMIntervalDataRcd.TimestampGMT = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
            VMIntervalDataRcd.IntervalStatus = m_Reader.ReadUInt16();
            VMIntervalDataRcd.IntervalData = new ushort[3];
            VMIntervalDataRcd.IntervalData[0] = m_Reader.ReadUInt16();
            VMIntervalDataRcd.IntervalData[1] = m_Reader.ReadUInt16();
            VMIntervalDataRcd.IntervalData[2] = m_Reader.ReadUInt16();
            VMIntervalDataRcd.VMMinVoltage = new ushort[3];
            VMIntervalDataRcd.VMMinVoltage[0] = m_Reader.ReadUInt16();
            VMIntervalDataRcd.VMMinVoltage[1] = m_Reader.ReadUInt16();
            VMIntervalDataRcd.VMMinVoltage[2] = m_Reader.ReadUInt16();
            VMIntervalDataRcd.VMMaxVoltage = new ushort[3];
            VMIntervalDataRcd.VMMaxVoltage[0] = m_Reader.ReadUInt16();
            VMIntervalDataRcd.VMMaxVoltage[1] = m_Reader.ReadUInt16();
            VMIntervalDataRcd.VMMaxVoltage[2] = m_Reader.ReadUInt16();
            VMIntervalDataRcd.ValidPhases = m_Reader.ReadByte();

            m_BellWeatherGroupDataStatusRcd.VMIntervalDataRecord = VMIntervalDataRcd;
        }


        #endregion

        #region Members

        private GroupDataStatusRcd m_BellWeatherGroupDataStatusRcd;

        #endregion

    }

    /// <summary>
    /// Class that represents the Group Enable Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue# Description
    //  -------- --- -------  ------ -------------------------------------------
    //  10/23/15 PGH 4.50.208 577471 Created
    //
    public class GroupEnableRcd
    {
        #region Constants

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public GroupEnableRcd()
        {
            m_strGroupKey = "";
            m_blnEnable = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Group Key
        /// </summary>
        public string GroupKey
        {
            get
            {
                return m_strGroupKey;
            }
            set
            {
                m_strGroupKey = value;
            }
        }

        /// <summary>
        /// Enable
        /// </summary>
        public bool Enable
        {
            get
            {
                return m_blnEnable;
            }
            set
            {
                m_blnEnable = value;
            }
        }

        #endregion

        #region Members

        private string m_strGroupKey;
        private bool m_blnEnable;

        #endregion
    }

    /// <summary>
    /// Class that represents the Bubble Up Enable Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue# Description
    //  -------- --- -------  ------ -------------------------------------------
    //  10/23/15 PGH 4.50.208 577471 Created
    //
    public class BubbleUpEnableRcd
    {
        #region Constants

        private ushort NUMBER_OF_GROUP_ENABLE_RCD = 3;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public BubbleUpEnableRcd()
        {
            m_blnGlobalEnable = false;
            m_GroupStatus = new GroupEnableRcd[NUMBER_OF_GROUP_ENABLE_RCD];
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Global Enable
        /// </summary>
        public bool GlobalEnable
        {
            get
            {
                return m_blnGlobalEnable;
            }
            set
            {
                m_blnGlobalEnable = value;
            }
        }

        /// <summary>
        /// Group Status
        /// </summary>
        public GroupEnableRcd[] GroupStatus
        {
            get
            {
                return m_GroupStatus;
            }
            set
            {
                m_GroupStatus = value;
            }
        }

        #endregion

        #region Members

        private bool m_blnGlobalEnable;
        private GroupEnableRcd[] m_GroupStatus;

        #endregion
    }

    /// <summary>
    /// Class that describes mfg table 2187 - Bell Weather Enable Record
    /// </summary>
    public class OpenWayMFGTable2187 : AnsiTable
    {
        #region Constants

        private const int TABLE_SIZE = 34;
        private const int TABLE_TIMEOUT = 1000;
        private const ushort GROUP_KEY_LENGTH = 10;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public OpenWayMFGTable2187(CPSEM psem)
            : base(psem, 2187, TABLE_SIZE, TABLE_TIMEOUT)
        {
            m_BellWeatherEnableRcd = null;
        }

        /// <summary>
        /// Constructor used to get Config Data from the EDL file
        /// </summary>
        /// <param name="reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public OpenWayMFGTable2187(PSEMBinaryReader reader)
            : base(2187, TABLE_SIZE)
        {
            m_BellWeatherEnableRcd = null;
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads Mfg table 2187
        /// </summary>
        /// <returns>PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "MFGTable2187.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;
                ParseData();
            }

            return Result;
        }

        /// <summary>
        ///  Writes the data to the log file for debugging purposes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                   "Dump of the Bell Weather Enable Record - Mfg table 2187.");

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Bell Weather Global Enable: " + m_BellWeatherEnableRcd.GlobalEnable.ToString());

                for (int Index = 0; Index < m_BellWeatherEnableRcd.GroupStatus.Length; Index++)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Group Enable Record " + (Index + 1).ToString());
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Group Key: " + m_BellWeatherEnableRcd.GroupStatus[Index].GroupKey);
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Enable: " + m_BellWeatherEnableRcd.GroupStatus[Index].Enable.ToString());
                }
            }
            catch (Exception e)
            {
                try
                {
                    m_Logger.WriteException(this, e);
                }
                catch
                {
                    // No exceptions thrown from this debugging method
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Bell Weather Enable Record - Mfg Table 2187
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //  12/22/15 PGH 4.50.222 577471 Updated
        //
        public BubbleUpEnableRcd BellWeatherEnableRcd
        {
            get
            {
                ReadUnloadedTable();

                return m_BellWeatherEnableRcd;
            }
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Get the size of the table.
        /// </summary>
        /// <returns>The size of the table in bytes.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        private static uint GetTableSize()
        {
            // BubbleUpEnableRcd is

            uint uiTableSize = 1; // Boolean

            // Plus 3 GroupEnableRcd

            uiTableSize += 10; // Group Key
            uiTableSize += 1;  // Enable

            uiTableSize += 10; // Group Key
            uiTableSize += 1;  // Enable

            uiTableSize += 10; // Group Key
            uiTableSize += 1;  // Enable

            return uiTableSize;
        }

        /// <summary>
        /// Parses the Bell Weather Enable Record out of the stream.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  10/23/15 PGH 4.50.208 577471 Created
        //
        private void ParseData()
        {
            m_BellWeatherEnableRcd = new BubbleUpEnableRcd();

            m_BellWeatherEnableRcd.GlobalEnable = m_Reader.ReadBoolean();
            
            for (int Index = 0; Index < m_BellWeatherEnableRcd.GroupStatus.Length; Index++)
            {
                byte[] GroupKey = new byte[GROUP_KEY_LENGTH];
                for (int i=0; i< GROUP_KEY_LENGTH; i++)
                {
                    GroupKey[i] = m_Reader.ReadByte();
                }
                m_BellWeatherEnableRcd.GroupStatus[Index] = new GroupEnableRcd();
                m_BellWeatherEnableRcd.GroupStatus[Index].GroupKey = System.Text.Encoding.UTF8.GetString(GroupKey);
                m_BellWeatherEnableRcd.GroupStatus[Index].Enable = m_Reader.ReadBoolean();

            }

        }

        #endregion

        #region Members

        private BubbleUpEnableRcd m_BellWeatherEnableRcd;

        #endregion

    }
}
