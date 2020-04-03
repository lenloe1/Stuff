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
//                              Copyright © 2015 - 2016
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

namespace Itron.Metering.Device
{

    /// <summary>
    /// Used to enumerate the meter base
    /// </summary>
    public enum MeterBase : byte
    {
        /// <summary>
        /// None
        /// </summary>
        [EnumDescription("None")]
        None = 0,
        /// <summary>
        /// S-base
        /// </summary>
        [EnumDescription("S-base (socket)")]
        S_base = 1,
        /// <summary>
        /// A-base
        /// </summary>
        [EnumDescription("A-base (ANSI bottom connected)")]
        A_base = 2,
        /// <summary>
        /// K-base
        /// </summary>
        [EnumDescription("K-base")]
        K_base = 3,
        /// <summary>
        /// IEC Asymmetric
        /// </summary>
        [EnumDescription("IEC bottom connected (Asymmetric)")]
        IEC_Asymmetric = 4,
        /// <summary>
        /// Switchboard
        /// </summary>
        [EnumDescription("Switchboard")]
        Switchboard = 5,
        /// <summary>
        /// Rackmount
        /// </summary>
        [EnumDescription("Rackmount")]
        Rackmount = 6,
        /// <summary>
        /// B-base
        /// </summary>
        [EnumDescription("B-base")]
        B_base = 7,
        /// <summary>
        /// P-base
        /// </summary>
        [EnumDescription("P-base (Canadian Standard)")]
        P_base = 8,
        /// <summary>
        /// IEC Symmetric
        /// </summary>
        [EnumDescription("IEC bottom connected (Symmetric)")]
        IEC_Symmetric = 9,
    }

    ///<summary>
    /// Meter Frequency
    ///</summary>
    public enum MeterFrequency : byte
    {
        /// <summary>
        /// 50 Hz
        /// </summary>
        [EnumDescription("50 Hz")]
        FiftyHz = 0,
        /// <summary>
        /// 60 Hz
        /// </summary>
        [EnumDescription("60 Hz")]
        SixtyHz = 1,
    }

    /// <summary>
    /// Class that describes OpenWay MFG table 2425 - Temperature Configuration
    /// </summary>
    public class OpenWayMFGTable2425 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 17;
        private const int TABLE_TIMEOUT = 500;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public OpenWayMFGTable2425(CPSEM psem)
            : base(psem, 2425, TABLE_SIZE, TABLE_TIMEOUT)
        {
            m_TemperatureConfigRcd = null;
        }

        /// <summary>
        /// Constructor used to get data from the EDL file
        /// </summary>
        /// <param name="reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public OpenWayMFGTable2425(PSEMBinaryReader reader)
            : base(2425, TABLE_SIZE)
        {
            m_TemperatureConfigRcd = null;
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the Temperature Configuration out of Mfg table 2425
        /// </summary>
        /// <returns>PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "TemperatureConfiguration.Read");

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
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.UInt16.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int16.ToString")]
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                   "Dump of Temperature Configuration.");

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Enable: " + m_TemperatureConfigRcd.Enable.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "High Temperature Threshold 1: " + m_TemperatureConfigRcd.HighTemperatureThreshold1.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "High Temperature Threshold 2: " + m_TemperatureConfigRcd.HighTemperatureThreshold2.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Hysteresis: " + m_TemperatureConfigRcd.Hysteresis.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Randomization Period Seconds: " + m_TemperatureConfigRcd.RandomizationPeriodSeconds.ToString());

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Daily Capture Time1 Hour of Day: " + m_TemperatureConfigRcd.DailyCaptureTime1[0].ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Daily Capture Time1 Minute of Hour: " + m_TemperatureConfigRcd.DailyCaptureTime1[1].ToString());

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Daily Capture Time2 Hour of Day: " + m_TemperatureConfigRcd.DailyCaptureTime2[0].ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Daily Capture Time2 Minute of Hour: " + m_TemperatureConfigRcd.DailyCaptureTime2[1].ToString());

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Daily Capture Time3 Hour of Day: " + m_TemperatureConfigRcd.DailyCaptureTime3[0].ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Daily Capture Time3 Minute of Hour: " + m_TemperatureConfigRcd.DailyCaptureTime3[1].ToString());

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Daily Capture Time4 Hour of Day: " + m_TemperatureConfigRcd.DailyCaptureTime4[0].ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Daily Capture Time4 Minute of Hour: " + m_TemperatureConfigRcd.DailyCaptureTime4[1].ToString());

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
        /// Gets the Temperature Configuration Record from table 2425
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public TemperatureConfigRcd TemperatureConfigRcd
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Temperature Configuration Record."));
                    }
                }

                return m_TemperatureConfigRcd;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the Temperature Configuration Record out of the stream.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        private void ParseData()
        {
            m_TemperatureConfigRcd = new TemperatureConfigRcd();

            m_TemperatureConfigRcd.Enable = m_Reader.ReadBoolean(); // 1
            m_TemperatureConfigRcd.HighTemperatureThreshold1 = m_Reader.ReadInt16(); // 2
            m_TemperatureConfigRcd.HighTemperatureThreshold2 = m_Reader.ReadInt16(); // 2
            m_TemperatureConfigRcd.Hysteresis = m_Reader.ReadInt16(); // 2
            m_TemperatureConfigRcd.RandomizationPeriodSeconds = m_Reader.ReadUInt16(); // 2
            m_TemperatureConfigRcd.DailyCaptureTime1[0] = m_Reader.ReadByte(); // 1
            m_TemperatureConfigRcd.DailyCaptureTime1[1] = m_Reader.ReadByte(); // 1
            m_TemperatureConfigRcd.DailyCaptureTime2[0] = m_Reader.ReadByte(); // 1
            m_TemperatureConfigRcd.DailyCaptureTime2[1] = m_Reader.ReadByte(); // 1
            m_TemperatureConfigRcd.DailyCaptureTime3[0] = m_Reader.ReadByte(); // 1
            m_TemperatureConfigRcd.DailyCaptureTime3[1] = m_Reader.ReadByte(); // 1
            m_TemperatureConfigRcd.DailyCaptureTime4[0] = m_Reader.ReadByte(); // 1
            m_TemperatureConfigRcd.DailyCaptureTime4[1] = m_Reader.ReadByte(); // 1
        }

        #endregion


        #region Members

        private TemperatureConfigRcd m_TemperatureConfigRcd;

        #endregion

    }

    /// <summary>
    /// Class that represents a Temperature Configuration Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue# Description
    //  -------- --- -------  ------ -------------------------------------------
    //  01/21/16 PGH 4.50.225 RTT556309 Created
    //
    public class TemperatureConfigRcd
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
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public TemperatureConfigRcd()
        {
            m_blnEnable = false;
            m_sHighTemperatureThreshold1 = 0;
            m_sHighTemperatureThreshold2 = 0;
            m_sHysteresis = 0;
            m_usRandomizationPeriodSeconds = 0;
            m_byaDailyCaptureTime1 = new byte[2];
            m_byaDailyCaptureTime2 = new byte[2];
            m_byaDailyCaptureTime3 = new byte[2];
            m_byaDailyCaptureTime4 = new byte[2];
        }

        #endregion

        #region Public Properties

        ///<summary>
        /// Enable
        ///</summary>
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

        /// <summary>
        /// High Temperature Threshold 1
        /// </summary>
        public short HighTemperatureThreshold1
        {
            get
            {
                return m_sHighTemperatureThreshold1;
            }
            set
            {
                m_sHighTemperatureThreshold1 = value;
            }
        }

        /// <summary>
        /// High Temperature Threshold 2
        /// </summary>
        public short HighTemperatureThreshold2
        {
            get
            {
                return m_sHighTemperatureThreshold2;
            }
            set
            {
                m_sHighTemperatureThreshold2 = value;
            }
        }

        /// <summary>
        /// Hysteresis
        /// </summary>
        public short Hysteresis
        {
            get
            {
                return m_sHysteresis;
            }
            set
            {
                m_sHysteresis = value;
            }
        }

        /// <summary>
        /// Randomization Period Seconds
        /// </summary>
        public ushort RandomizationPeriodSeconds
        {
            get
            {
                return m_usRandomizationPeriodSeconds;
            }
            set
            {
                m_usRandomizationPeriodSeconds = value;
            }
        }

        /// <summary>
        /// Daily Capture Time 1
        /// </summary>
        public byte[] DailyCaptureTime1
        {
            get
            {
                return m_byaDailyCaptureTime1;
            }
            set
            {
                m_byaDailyCaptureTime1 = value;
            }
        }

        /// <summary>
        /// Daily Capture Time 2
        /// </summary>
        public byte[] DailyCaptureTime2
        {
            get
            {
                return m_byaDailyCaptureTime2;
            }
            set
            {
                m_byaDailyCaptureTime2 = value;
            }
        }

        /// <summary>
        /// Daily Capture Time 3
        /// </summary>
        public byte[] DailyCaptureTime3
        {
            get
            {
                return m_byaDailyCaptureTime3;
            }
            set
            {
                m_byaDailyCaptureTime3 = value;
            }
        }

        /// <summary>
        /// Daily Capture Time 4
        /// </summary>
        public byte[] DailyCaptureTime4
        {
            get
            {
                return m_byaDailyCaptureTime4;
            }
            set
            {
                m_byaDailyCaptureTime4 = value;
            }
        }

        #endregion

        #region Members

        private bool m_blnEnable;
        private short m_sHighTemperatureThreshold1;
        private short m_sHighTemperatureThreshold2;
        private short m_sHysteresis;
        private ushort m_usRandomizationPeriodSeconds;
        private byte[] m_byaDailyCaptureTime1;
        private byte[] m_byaDailyCaptureTime2;
        private byte[] m_byaDailyCaptureTime3;
        private byte[] m_byaDailyCaptureTime4;

        #endregion
    }

    /// <summary>
    /// Class that describes OpenWay MFG table 2426 - Temperature Data
    /// </summary>
    public class OpenWayMFGTable2426 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 16;
        private const int TABLE_TIMEOUT = 500;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public OpenWayMFGTable2426(CPSEM psem)
            : base(psem, 2426, TABLE_SIZE, TABLE_TIMEOUT)
        {
            m_TemperatureDataRcd = null;
        }

        /// <summary>
        /// Constructor used to get data from the EDL file
        /// </summary>
        /// <param name="reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public OpenWayMFGTable2426(PSEMBinaryReader reader)
            : base(2426, TABLE_SIZE)
        {
            m_TemperatureDataRcd = null;
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the Temperature Data out of Mfg table 2426
        /// </summary>
        /// <returns>PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "TemperatureData.Read");

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
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int16.ToString")]
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                   "Dump of Temperature Data.");

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Temperature: " + m_TemperatureDataRcd.Temperature.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Average Aggregate Current: " + m_TemperatureDataRcd.AverageAggregateCurrent.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Base type: " + m_TemperatureDataRcd.BaseType.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Meter form: " + m_TemperatureDataRcd.MeterForm.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Frequency: " + m_TemperatureDataRcd.Frequency.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Number of elements: " + m_TemperatureDataRcd.NumElements.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Power supply type: " + m_TemperatureDataRcd.PowerSupplyType.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Meter class: " + m_TemperatureDataRcd.MeterClass.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Device class: " + m_TemperatureDataRcd.DeviceClass.ToString());
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Meter type bits: " + m_TemperatureDataRcd.MeterTypeBits.ToString());

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
        /// Gets the Temperature Data Record from table 2426
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //  05/20/16 PGH 4.50.270 687608 Read table instantaneously
        //  05/24/16 PGH 4.50.271 687608 The table must be in synch with the meter for the EDL viewer
        //
        public TemperatureDataRcd TemperatureDataRcd
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Temperature Data Record."));
                    }
                }

                return m_TemperatureDataRcd;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the Temperature Data Record out of the stream.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        private void ParseData()
        {
            m_TemperatureDataRcd = new TemperatureDataRcd();

            m_TemperatureDataRcd.Temperature = m_Reader.ReadInt16();
            m_TemperatureDataRcd.AverageAggregateCurrent = m_Reader.ReadInt16();
            m_TemperatureDataRcd.BaseType = (MeterBase)m_Reader.ReadByte();
            m_TemperatureDataRcd.MeterForm = (MeterForm)m_Reader.ReadByte();
            m_TemperatureDataRcd.Frequency = (MeterFrequency)m_Reader.ReadByte();
            m_TemperatureDataRcd.NumElements = m_Reader.ReadByte();
            m_TemperatureDataRcd.PowerSupplyType = m_Reader.ReadByte();
            m_TemperatureDataRcd.MeterClass = (MeterClass)m_Reader.ReadByte();
            m_TemperatureDataRcd.ServiceType = (MeterServiceType)m_Reader.ReadByte();
            m_TemperatureDataRcd.DeviceClass = new string(m_Reader.ReadChars(4));
            m_TemperatureDataRcd.MeterTypeBits = m_Reader.ReadByte();
        }

        #endregion


        #region Members

        private TemperatureDataRcd m_TemperatureDataRcd;

        #endregion

    }

    /// <summary>
    /// Class that represents a Temperature Data Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue# Description
    //  -------- --- -------  ------ -------------------------------------------
    //  01/21/16 PGH 4.50.225 RTT556309 Created
    //
    public class TemperatureDataRcd
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
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public TemperatureDataRcd()
        {
            m_sTemperature = 0;
            m_sAverageAggregateCurrent = 0;
            m_byBaseType = 0;
            m_byMeterForm = 0;
            m_byFrequency = 0;
            m_byNumElements = 0;
            m_byPowerSupplyType = 0;
            m_byMeterClass = 0;
            m_byServiceType = 0;
            m_byaDeviceClass = "";
            m_byMeterTypeBits = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Temperature
        /// </summary>
        public short Temperature
        {
            get
            {
                return m_sTemperature;
            }
            set
            {
                m_sTemperature = value;
            }
        }

        /// <summary>
        /// Average Aggregate Current
        /// </summary>
        public short AverageAggregateCurrent
        {
            get
            {
                return m_sAverageAggregateCurrent;
            }
            set
            {
                m_sAverageAggregateCurrent = value;
            }
        }

        /// <summary>
        /// Base Type
        /// </summary>
        public MeterBase BaseType
        {
            get
            {
                return m_byBaseType;
            }
            set
            {
                m_byBaseType = value;
            }
        }

        /// <summary>
        /// Meter Form
        /// </summary>
        public MeterForm MeterForm
        {
            get
            {
                return m_byMeterForm;
            }
            set
            {
                m_byMeterForm = value;
            }
        }

        /// <summary>
        /// Frequency
        /// </summary>
        public MeterFrequency Frequency
        {
            get
            {
                return m_byFrequency;
            }
            set
            {
                m_byFrequency = value;
            }
        }

        /// <summary>
        /// Number of elements
        /// </summary>
        public byte NumElements
        {
            get
            {
                return m_byNumElements;
            }
            set
            {
                m_byNumElements = value;
            }
        }

        /// <summary>
        /// Power Supply Type
        /// </summary>
        public byte PowerSupplyType
        {
            get
            {
                return m_byPowerSupplyType;
            }
            set
            {
                m_byPowerSupplyType = value;
            }
        }

        /// <summary>
        /// Meter Class
        /// </summary>
        public MeterClass MeterClass
        {
            get
            {
                return m_byMeterClass;
            }
            set
            {
                m_byMeterClass = value;
            }
        }

        /// <summary>
        /// Service Type
        /// </summary>
        public MeterServiceType ServiceType
        {
            get
            {
                return m_byServiceType;
            }
            set
            {
                m_byServiceType = value;
            }
        }

        /// <summary>
        /// Device Class
        /// </summary>
        public string DeviceClass
        {
            get
            {
                return m_byaDeviceClass;
            }
            set
            {
                m_byaDeviceClass = value;
            }
        }

        /// <summary>
        /// Meter Type Bits
        /// </summary>
        public byte MeterTypeBits
        {
            get
            {
                return m_byMeterTypeBits;
            }
            set
            {
                m_byMeterTypeBits = value;
            }
        }

        #endregion

        #region Members

        private short m_sTemperature;
        private short m_sAverageAggregateCurrent;
        private MeterBase m_byBaseType;
        private MeterForm m_byMeterForm;
        private MeterFrequency m_byFrequency;
        private byte m_byNumElements;
        private byte m_byPowerSupplyType;
        private MeterClass m_byMeterClass;
        private MeterServiceType m_byServiceType;
        private string m_byaDeviceClass;
        private byte m_byMeterTypeBits;

        #endregion
    }

    /// <summary>
    /// Class that describes OpenWay MFG table 2427 - Temperature Log
    /// </summary>
    public class OpenWayMFGTable2427 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 257;
        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public OpenWayMFGTable2427(CPSEM psem)
            : base(psem, 2427, TABLE_SIZE, TABLE_TIMEOUT)
        {
            m_TemperatureLog = null;
        }

        /// <summary>
        /// Constructor used to get data from the EDL file
        /// </summary>
        /// <param name="reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public OpenWayMFGTable2427(PSEMBinaryReader reader)
            : base(2427, TABLE_SIZE)
        {
            m_TemperatureLog = null;
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the Temperature Log out of Mfg table 2427
        /// </summary>
        /// <returns>PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "TemperatureLog.Read");

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
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.DateTime.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int16.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                   "Dump of Temperature Log.");

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Next Log Index: " + m_TemperatureLog.NextLogIndex.ToString());

                int NumberOfEntries = m_TemperatureLog.TemperatureLogEntries.Count();
                for (int Index = 0; Index < NumberOfEntries; Index++)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Entry " + (Index + 1).ToString());
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Temperature: " + m_TemperatureLog.TemperatureLogEntries[Index].Temperature.ToString());
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Average Aggregate Current: " + m_TemperatureLog.TemperatureLogEntries[Index].AverageAggregateCurrent.ToString());
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Capture DateTime: " + m_TemperatureLog.TemperatureLogEntries[Index].CaptureDateTime.ToString());
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
        /// Gets the Temperature Log from table 2427
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public TemperatureLogRcd TemperatureLog
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Temperature Log."));
                    }
                }

                return m_TemperatureLog;
            }
        }

        /// <summary>
        /// Gets the Temperature Log from table 2427. Does a fresh read to get the uncached values
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/19/16 AF  4.50.231 TC 62792   Created
        //
        public TemperatureLogRcd UncachedTemperatureLog
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                Result = Read();
                if (PSEMResponse.Ok != Result)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading Temperature Log."));
                }

                return m_TemperatureLog;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the Temperature Log out of the stream.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //  02/05/16 PGH 4.50.226 RTT556309 Updated ReadSTIME
        //
        private void ParseData()
        {
            m_TemperatureLog = new TemperatureLogRcd();

            m_TemperatureLog.NextLogIndex = m_Reader.ReadByte();

            int NumberOfEntries = m_TemperatureLog.TemperatureLogEntries.Count();
            for (int Index = 0; Index < NumberOfEntries; Index++)
            {
                m_TemperatureLog.TemperatureLogEntries[Index] = new TemperatureLogEntry();
                m_TemperatureLog.TemperatureLogEntries[Index].Temperature = m_Reader.ReadInt16();
                m_TemperatureLog.TemperatureLogEntries[Index].AverageAggregateCurrent = m_Reader.ReadInt16();
                m_TemperatureLog.TemperatureLogEntries[Index].CaptureDateTime = m_Reader.ReadSTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME).ToLocalTime();
            }
        }

        #endregion


        #region Members

        private TemperatureLogRcd m_TemperatureLog;

        #endregion

    }

    /// <summary>
    /// Class that represents a Temperature Log Entry
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue# Description
    //  -------- --- -------  ------ -------------------------------------------
    //  01/21/16 PGH 4.50.225 RTT556309 Created
    //
    public class TemperatureLogEntry
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
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public TemperatureLogEntry()
        {
            m_sTemperature = 0;
            m_sAverageAggregateCurrent = 0;
            m_dtCaptureDateTime = DateTime.MinValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Temperature
        /// </summary>
        public short Temperature
        {
            get
            {
                return m_sTemperature;
            }
            set
            {
                m_sTemperature = value;
            }
        }

        /// <summary>
        /// Average Aggregate Current
        /// </summary>
        public short AverageAggregateCurrent
        {
            get
            {
                return m_sAverageAggregateCurrent;
            }
            set
            {
                m_sAverageAggregateCurrent = value;
            }
        }

        /// <summary>
        /// Capture DateTime
        /// </summary>
        public DateTime CaptureDateTime
        {
            get
            {
                return m_dtCaptureDateTime;
            }
            set
            {
                m_dtCaptureDateTime = value;
            }
        }

        #endregion

        #region Members

        private short m_sTemperature;
        private short m_sAverageAggregateCurrent;
        private DateTime m_dtCaptureDateTime;

        #endregion
    }

    /// <summary>
    /// Class that represents a Temperature Log Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue# Description
    //  -------- --- -------  ------ -------------------------------------------
    //  01/21/16 PGH 4.50.225 RTT556309 Created
    //
    public class TemperatureLogRcd
    {
        #region Constants

        private const ushort MAXIMUM_NUMBER_OF_LOG_ENTRIES = 32;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  01/21/16 PGH 4.50.225 RTT556309 Created
        //
        public TemperatureLogRcd()
        {
            m_byNextLogIndex = 0;
            m_TemperatureLogEntries = new TemperatureLogEntry[MAXIMUM_NUMBER_OF_LOG_ENTRIES];
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Next Log Index
        /// </summary>
        public byte NextLogIndex
        {
            get
            {
                return m_byNextLogIndex;
            }
            set
            {
                m_byNextLogIndex = value;
            }
        }

        /// <summary>
        /// Temperature Log
        /// </summary>
        public TemperatureLogEntry[] TemperatureLogEntries
        {
            get
            {
                return m_TemperatureLogEntries;
            }
            set
            {
                m_TemperatureLogEntries = value;
            }
        }

        #endregion

        #region Members

        private byte m_byNextLogIndex;
        private TemperatureLogEntry[] m_TemperatureLogEntries;

        #endregion
    }

}