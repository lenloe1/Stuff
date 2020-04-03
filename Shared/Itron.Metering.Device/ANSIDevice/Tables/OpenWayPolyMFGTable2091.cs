using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Available Meter Forms
    /// </summary>
    public enum MeterForms : byte
    {
        /// <summary>
        /// Form 1S
        /// </summary>
        [EnumDescription("1S")]
        Form1 = 1,
        /// <summary>
        /// Form 2S
        /// </summary>
        [EnumDescription("2S")]
        Form2 = 2,
        /// <summary>
        /// Form 3S
        /// </summary>
        [EnumDescription("3S")]
        Form3 = 3,
        /// <summary>
        /// Form 4S
        /// </summary>
        [EnumDescription("4S")]
        Form4 = 4,
        /// <summary>
        /// Form 9S
        /// </summary>
        [EnumDescription("9S")]
        Form9 = 9,
        /// <summary>
        /// Form 10S
        /// </summary>
        [EnumDescription("10S")]
        Form10 = 10,
        /// <summary>
        /// Form 12S
        /// </summary>
        [EnumDescription("12S")]
        Form12 = 12,
        /// <summary>
        /// Form 13S
        /// </summary>
        [EnumDescription("13S")]
        Form13 = 13,
        /// <summary>
        /// Form 16S
        /// </summary>
        [EnumDescription("16S")]
        Form16 = 16,
        /// <summary>
        /// Form 25S
        /// </summary>
        [EnumDescription("25S")]
        Form25 = 25,
        /// <summary>
        /// Form 29S
        /// </summary>
        [EnumDescription("29S")]
        Form29 = 29,
        /// <summary>
        /// Form 36S
        /// </summary>
        [EnumDescription("36S")]
        Form36 = 36,
        /// <summary>
        /// Form 45S
        /// </summary>
        [EnumDescription("45S")]
        Form45 = 45,
        /// <summary>
        /// Form 46S
        /// </summary>
        [EnumDescription("46S")]
        Form46 = 46,
        /// <summary>
        /// Form 48S
        /// </summary>
        [EnumDescription("48S")]
        Form48 = 48,
        /// <summary>
        /// Form 56S
        /// </summary>
        [EnumDescription("56S")]
        Form56 = 56,
        /// <summary>
        /// Form 66S
        /// </summary>
        [EnumDescription("66S")]
        Form66 = 66,
        /// <summary>
        /// Form 96S
        /// </summary>
        [EnumDescription("9S or 36S")]
        Form96 = 96,
        /// <summary>
        /// Uknown Form
        /// </summary>
        [EnumDescription("Unknown")]
        Unknown = 255,
    }

    /// <summary>
    /// SiteScan ToolBox Table for OpenWay Poly meters.
    /// </summary>

    public class OpenWayPolyMFGTable2091 : AnsiTable
    {
        #region Constants

        private const uint INITIAL_TABLE_SIZE = 73;
        private const uint BERYLLIUM_ADJUSTED_TABLE_SIZE = 78;
        private const int TABLE_TIMEOUT = 1000;

        // Active Diagnostic Masks
        private const byte DIAG1_ACTIVE_MASK = 0x01;
        private const byte DIAG2_ACTIVE_MASK = 0x02;
        private const byte DIAG3_ACTIVE_MASK = 0x04;
        private const byte DIAG4_ACTIVE_MASK = 0x08;
        private const byte DIAG5_ACTIVE_MASK = 0x10;
        private const byte DIAG6_ACTIVE_MASK = 0x20;

        // Diagnostic Status Masks
        private const byte SERIAL_SERVICE_MASK = 0x01;
        private const byte TEST_MODE_MASK = 0x02;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current device.</param>
        /// <param name="fltRegFWVersion">The register FW version for the current device.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created
        //  10/24/12 jrf 2.70.33 238238 Adding passing in FW version to determine table size.
        //
        public OpenWayPolyMFGTable2091(CPSEM psem, float fltRegFWVersion)
            : base(psem, 2091, GetTableSize(fltRegFWVersion), TABLE_TIMEOUT)
        {
            m_fltRegFWVersion = fltRegFWVersion;
        }

        /// <summary>
        /// Constructor used by EDL file.
        /// </summary>
        /// <param name="BinaryReader">The binary reader that contains the table data.</param>
        /// <param name="fltRegFWVersion">The register FW version for the current meter</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/13/09 RCG 2.20.04        Created
        //  10/24/12 jrf 2.70.33 238238 Adding passing in FW version to determine table size.
        //
        public OpenWayPolyMFGTable2091(PSEMBinaryReader BinaryReader, float fltRegFWVersion)
            : base(2091, GetTableSize(fltRegFWVersion))
        {
            m_fltRegFWVersion = fltRegFWVersion;
            State = TableState.Loaded;
            m_Reader = BinaryReader;
            ParseData();
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayPolyMFGTable2091.Read");

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
        /// Gets the form number of the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public MeterForms MeterForm
        {
            get
            {
                ReadUnloadedTable();

                return (MeterForms)m_byForm;
            }
        }

        /// <summary>
        /// Gets the current Service Type of the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public ServiceTypes ServiceType
        {
            get
            {
                ReadUnloadedTable();

                return (ServiceTypes)m_byServiceType;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Volts on Phase A
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float InsVoltsPhaseA
        {
            get
            {
                ReadUnloadedTable();

                return m_fInsVoltsPhaseA;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Volts on Phase B
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float InsVoltsPhaseB
        {
            get
            {
                ReadUnloadedTable();

                return m_fInsVoltsPhaseB;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Volts on Phase C
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float InsVoltsPhaseC
        {
            get
            {
                ReadUnloadedTable();

                return m_fInsVoltsPhaseC;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Amps on Phase A
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float InsAmpsPhaseA
        {
            get
            {
                ReadUnloadedTable();

                return m_fInsAmpsPhaseA;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Amps on Phase B
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float InsAmpsPhaseB
        {
            get
            {
                ReadUnloadedTable();

                return m_fInsAmpsPhaseB;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Amps on Phase C
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float InsAmpsPhaseC
        {
            get
            {
                ReadUnloadedTable();

                return m_fInsAmpsPhaseC;
            }
        }

        /// <summary>
        /// Gets the Phase B angle for Volts
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float VoltsPhaseBAngle
        {
            get
            {
                ReadUnloadedTable();

                return m_fVoltsPhaseBAngle;
            }
        }

        /// <summary>
        /// Gets the Phase C angle for Volts
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float VoltsPhaseCAngle
        {
            get
            {
                ReadUnloadedTable();

                return m_fVoltsPhaseCAngle;
            }
        }

        /// <summary>
        /// Gets the Phase A angle for Amps
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float AmpsPhaseAAngle
        {
            get
            {
                ReadUnloadedTable();

                return m_fAmpsPhaseAAngle;
            }
        }

        /// <summary>
        /// Gets the Phase B angle for Amps
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float AmpsPhaseBAngle
        {
            get
            {
                ReadUnloadedTable();

                return m_fAmpsPhaseBAngle;
            }
        }

        /// <summary>
        /// Gets the Phase C angle for Amps
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float AmpsPhaseCAngle
        {
            get
            {
                ReadUnloadedTable();

                return m_fAmpsPhaseCAngle;
            }
        }

        /// <summary>
        /// Gets the Instantaneous W
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float InsW
        {
            get
            {
                ReadUnloadedTable();

                return m_fInsW;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Var
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float InsVar
        {
            get
            {
                ReadUnloadedTable();

                return m_fInsVar;
            }
        }

        /// <summary>
        /// Gets the Instantaneous VA
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float InsVA
        {
            get
            {
                ReadUnloadedTable();

                return m_fInsVA;
            }
        }

        /// <summary>
        /// Gets the Instantaneous PF
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public float InsPF
        {
            get
            {
                ReadUnloadedTable();

                return m_fInsPF;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 1 is currently active
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public bool IsDiag1Active
        {
            get
            {
                ReadUnloadedTable();

                return (m_byActiveDiagnostics & DIAG1_ACTIVE_MASK) == DIAG1_ACTIVE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 2 is currently active
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public bool IsDiag2Active
        {
            get
            {
                ReadUnloadedTable();

                return (m_byActiveDiagnostics & DIAG2_ACTIVE_MASK) == DIAG2_ACTIVE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 3 is currently active
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public bool IsDiag3Active
        {
            get
            {
                ReadUnloadedTable();

                return (m_byActiveDiagnostics & DIAG3_ACTIVE_MASK) == DIAG3_ACTIVE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 4 is currently active
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public bool IsDiag4Active
        {
            get
            {
                ReadUnloadedTable();

                return (m_byActiveDiagnostics & DIAG4_ACTIVE_MASK) == DIAG4_ACTIVE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 5 is currently active
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public bool IsDiag5Active
        {
            get
            {
                ReadUnloadedTable();

                return (m_byActiveDiagnostics & DIAG5_ACTIVE_MASK) == DIAG5_ACTIVE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not Diagnostic 6 is currently active
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public bool IsDiag6Active
        {
            get
            {
                ReadUnloadedTable();

                return (m_byActiveDiagnostics & DIAG6_ACTIVE_MASK) == DIAG6_ACTIVE_MASK;
            }
        }

        /// <summary>
        /// Gets the count for Diagnostic 1
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public byte Diag1Count
        {
            get
            {
                ReadUnloadedTable();

                return m_byDiag1Count;
            }
        }

        /// <summary>
        /// Gets the count for Diagnostic 2
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public byte Diag2Count
        {
            get
            {
                ReadUnloadedTable();

                return m_byDiag2Count;
            }
        }

        /// <summary>
        /// Gets the count for Diagnostic 3
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public byte Diag3Count
        {
            get
            {
                ReadUnloadedTable();

                return m_byDiag3Count;
            }
        }

        /// <summary>
        /// Gets the count for Diagnostic 4
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public byte Diag4Count
        {
            get
            {
                ReadUnloadedTable();

                return m_byDiag4Count;
            }
        }

        /// <summary>
        /// Gets the total count for Diagnostic 5 
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public byte Diag5TotalCount
        {
            get
            {
                ReadUnloadedTable();

                return m_byDiag5TotalCount;
            }
        }

        /// <summary>
        /// Gets the phase A count for Diagnostic 5
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public byte Diag5ACount
        {
            get
            {
                ReadUnloadedTable();

                return m_byDiag5ACount;
            }
        }

        /// <summary>
        /// Gets the phase B count for Diagnostic 5
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public byte Diag5BCount
        {
            get
            {
                ReadUnloadedTable();

                return m_byDiag5BCount;
            }
        }

        /// <summary>
        /// Gets the phase C count for Diagnostic 5
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public byte Diag5CCount
        {
            get
            {
                ReadUnloadedTable();

                return m_byDiag5CCount;
            }
        }

        /// <summary>
        /// Gets the count for Diagnostic 6
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public byte Diag6Count
        {
            get
            {
                ReadUnloadedTable();

                return m_byDiag6Count;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan has been disabled due to the meter being in
        /// a serial service configuration (all phase angles at 0 degrees)
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public bool IsSiteScanDisabledDoToSerialService
        {
            get
            {
                ReadUnloadedTable();

                return (m_byDiagnosticStatus & SERIAL_SERVICE_MASK) == SERIAL_SERVICE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not SiteScan has been disabled due to the meter being in
        /// test mode.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created

        public bool IsSiteScanDisabledDoToTestMode
        {
            get
            {
                ReadUnloadedTable();

                return (m_byDiagnosticStatus & TEST_MODE_MASK) == TEST_MODE_MASK;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parse the data from the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/30/09 RCG 2.20.03        Created
        //  10/24/12 jrf 2.70.33 238238 Adding reading date last updated when FW is Beryllium.
        //
        private void ParseData()
        {
            m_byForm = m_Reader.ReadByte();
            m_byServiceType = m_Reader.ReadByte();
            m_fInsVoltsPhaseA = m_Reader.ReadSingle();
            m_fInsVoltsPhaseB = m_Reader.ReadSingle();
            m_fInsVoltsPhaseC = m_Reader.ReadSingle();
            m_fInsAmpsPhaseA = m_Reader.ReadSingle();
            m_fInsAmpsPhaseB = m_Reader.ReadSingle();
            m_fInsAmpsPhaseC = m_Reader.ReadSingle();
            m_fVoltsPhaseBAngle = m_Reader.ReadSingle();
            m_fVoltsPhaseCAngle = m_Reader.ReadSingle();
            m_fAmpsPhaseAAngle = m_Reader.ReadSingle();
            m_fAmpsPhaseBAngle = m_Reader.ReadSingle();
            m_fAmpsPhaseCAngle = m_Reader.ReadSingle();
            m_fInsW = m_Reader.ReadSingle();
            m_fInsVar = m_Reader.ReadSingle();
            m_fInsVA = m_Reader.ReadSingle();
            m_fInsPF = m_Reader.ReadSingle();
            m_byActiveDiagnostics = m_Reader.ReadByte();
            m_byDiag1Count = m_Reader.ReadByte();
            m_byDiag2Count = m_Reader.ReadByte();
            m_byDiag3Count = m_Reader.ReadByte();
            m_byDiag4Count = m_Reader.ReadByte();
            m_byDiag5TotalCount = m_Reader.ReadByte();
            m_byDiag5ACount = m_Reader.ReadByte();
            m_byDiag5BCount = m_Reader.ReadByte();
            m_byDiag5CCount = m_Reader.ReadByte();
            m_byDiag6Count = m_Reader.ReadByte();
            m_byDiagnosticStatus = m_Reader.ReadByte();

            if (VersionChecker.CompareTo(m_fltRegFWVersion, CENTRON_AMI.VERSION_MICHIGAN) >= 0)
            {
                //Beryllium added time data in table was last updated.
                m_dtTimeOfLastUpdate = m_Reader.ReadLTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME);
            }
        }

        /// <summary>
        /// Gets the size of the table
        /// </summary>
        /// <param name="fltRegFWRevision">The register firmware version for the current meter.</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/24/12 jrf 2.70.33 238238 Created
        //
        private static uint GetTableSize(float fltRegFWRevision)
        {
            uint TableSize = INITIAL_TABLE_SIZE;

            if (VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_MICHIGAN) >= 0)
            {
                TableSize = BERYLLIUM_ADJUSTED_TABLE_SIZE;
            }

            return TableSize;
        }

        #endregion

        #region Member Variables

        private byte m_byForm;
        private byte m_byServiceType;
        private float m_fInsVoltsPhaseA;
        private float m_fInsVoltsPhaseB;
        private float m_fInsVoltsPhaseC;
        private float m_fInsAmpsPhaseA;
        private float m_fInsAmpsPhaseB;
        private float m_fInsAmpsPhaseC;
        private float m_fVoltsPhaseBAngle;
        private float m_fVoltsPhaseCAngle;
        private float m_fAmpsPhaseAAngle;
        private float m_fAmpsPhaseBAngle;
        private float m_fAmpsPhaseCAngle;
        private float m_fInsW;
        private float m_fInsVar;
        private float m_fInsVA;
        private float m_fInsPF;
        private byte m_byActiveDiagnostics;
        private byte m_byDiag1Count;
        private byte m_byDiag2Count;
        private byte m_byDiag3Count;
        private byte m_byDiag4Count;
        private byte m_byDiag5TotalCount;
        private byte m_byDiag5ACount;
        private byte m_byDiag5BCount;
        private byte m_byDiag5CCount;
        private byte m_byDiag6Count;
        private byte m_byDiagnosticStatus;
        private DateTime? m_dtTimeOfLastUpdate = null;
        private float m_fltRegFWVersion;

        #endregion
    }
}
