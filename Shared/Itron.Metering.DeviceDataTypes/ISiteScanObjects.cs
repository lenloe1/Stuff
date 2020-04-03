using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.DeviceDataTypes
{
    /// <summary>
    /// This Structure represents all of the meter data items shown on the 'Toolbox' display.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------
    //  04/12/06 mrj 7.30.00    N/A Created
    //  02/09/07 mrj 8.00.11		Changed to double and added instantaneous
    //								quantities
    //
    public struct Toolbox
    {
        /// <summary>
        /// Voltage angle
        /// </summary>
        public double m_fVAngleA;
        /// <summary>
        /// Voltage angle
        /// </summary>
        public double m_fVAngleB;
        /// <summary>
        /// Voltage angle
        /// </summary>
        public double m_fVAngleC;
        /// <summary>
        /// Current angle
        /// </summary>
        public double m_fIAngleA;
        /// <summary>
        /// Current angle
        /// </summary>
        public double m_fIAngleB;
        /// <summary>
        /// Current angle
        /// </summary>
        public double m_fIAngleC;
        /// <summary>
        /// Voltage (Volts)
        /// </summary>
        public double m_fVoltsA;
        /// <summary>
        /// Voltage (Volts)
        /// </summary>
        public double m_fVoltsB;
        /// <summary>
        /// Voltage (Volts)
        /// </summary>
        public double m_fVoltsC;
        /// <summary>
        /// Current (Amps)
        /// </summary>
        public double m_fCurrentA;
        /// <summary>
        /// Current (Amps)
        /// </summary>
        public double m_fCurrentB;
        /// <summary>
        /// Current (Amps)
        /// </summary>
        public double m_fCurrentC;

        /// <summary>
        /// Instantaneous KW
        /// </summary>
        public double m_dInsKW;
        /// <summary>
        /// Instantaneous KVar
        /// </summary>
        public double m_dInsKVar;
        /// <summary>
        /// Instantaneous KVA
        /// </summary>
        public double m_dInsKVA;
        /// <summary>
        /// Instantaneous Arithmatic VA
        /// </summary>
        public double m_dInsKVAArith;
        /// <summary>
        /// Instantaneous Vectorial VA
        /// </summary>
        public double m_dInsKVAVect;
        /// <summary>
        /// Instantaneous PF
        /// </summary>
        public double m_dInsPF;
    }

    /// <summary>
    /// This class represents the collection of sitescan diagnostics found in Itron meters
    /// </summary>
    /// Revision History
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------------
    /// 04/27/06 mrj 7.30.00        Created for HH-Pro
    ///
    public class CDiagnostics
    {
        /// <summary>
        /// This is the enumeration of all possible sitescan diagnostics
        /// </summary>
        public enum DiagEnum
        {
            /// <summary>
            /// 
            /// </summary>
            DIAG_1 = 0,
            /// <summary>
            /// 
            /// </summary>
            DIAG_2 = 1,
            /// <summary>
            /// 
            /// </summary>
            DIAG_3 = 2,
            /// <summary>
            /// 
            /// </summary>
            DIAG_4 = 3,
            /// <summary>
            /// 
            /// </summary>
            DIAG_5A = 4,
            /// <summary>
            /// 
            /// </summary>
            DIAG_5B = 5,
            /// <summary>
            /// 
            /// </summary>
            DIAG_5C = 6,
            /// <summary>
            /// 
            /// </summary>
            DIAG_5T = 7,
            /// <summary>
            /// 
            /// </summary>
            DIAG_6 = 8,
            /// <summary>
            /// 
            /// </summary>
            NUM_DIAGS = 9
        }

        /// <summary>
        /// This structure represents a single sitescan diagnostic value.  It contains
        /// a user readable name, a flag indicating if the diagnostic is currently active, and
        /// a count of the number of times the diagnostic has triggered.
        /// </summary>
        public struct Diag
        {
            /// <summary>
            /// The count of the diagnostic
            /// </summary>
            public int Count;
            /// <summary>
            /// A flag indicating whether or not a diagnostic is active
            /// </summary>
            public bool Active;
            /// <summary>
            /// The name of the diagnostic
            /// </summary>
            public string Name;
        }

        /// <summary>
        /// The is the array of supported diagnosics
        /// </summary>
        public Diag[] m_Diags;

        private const string DIAG_1_NAME = "Diag 1";
        private const string DIAG_2_NAME = "Diag 2";
        private const string DIAG_3_NAME = "Diag 3";
        private const string DIAG_4_NAME = "Diag 4";
        private const string DIAG_5A_NAME = "Diag 5A";
        private const string DIAG_5B_NAME = "Diag 5B";
        private const string DIAG_5C_NAME = "Diag 5C";
        private const string DIAG_5T_NAME = "Diag 5T";
        private const string DIAG_6_NAME = "Diag 6";


        /// <summary>
        /// Constructor, this method sizes the diagnostics and sets
        /// their names.
        /// </summary>
        /// <param name="bDiag6Supported">Flag indicating whether or not
        /// the Diag 6 is supported</param>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 04/27/06 mrj 7.30.00        Created for HH-Pro
        ///
        public CDiagnostics(bool bDiag6Supported)
            : this(true, bDiag6Supported)
        {
        }

        /// <summary>
        /// Constructor that allows for supporting Diag 5.
        /// </summary>
        /// <param name="bDiag5Supported">Whether or not Diag 5 is supported.</param>
        /// <param name="bDiag6Supported">Whether or not Diag 6 is supported.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/23/09 RCG 2.20.02 N/A    Created

        public CDiagnostics(bool bDiag5Supported, bool bDiag6Supported)
        {
            //Create the correct sized array
            // Currently if Diag 5 is not supported Diag 6 will not be either. 
            // It will be hard to support this without rewriting the code which 
            // brings more risk since this is exposed through a public interface.
            if (false == bDiag5Supported)
            {
                m_Diags = new Diag[(int)DiagEnum.DIAG_4 + 1];
            }
            else if (false == bDiag6Supported)
            {
                m_Diags = new Diag[(int)DiagEnum.NUM_DIAGS - 1];
            }
            else
            {
                m_Diags = new Diag[(int)DiagEnum.NUM_DIAGS];
            }

            //Set the name to the diagnostics
            m_Diags[(int)DiagEnum.DIAG_1].Name = DIAG_1_NAME;
            m_Diags[(int)DiagEnum.DIAG_2].Name = DIAG_2_NAME;
            m_Diags[(int)DiagEnum.DIAG_3].Name = DIAG_3_NAME;
            m_Diags[(int)DiagEnum.DIAG_4].Name = DIAG_4_NAME;

            if (bDiag5Supported)
            {
                m_Diags[(int)DiagEnum.DIAG_5A].Name = DIAG_5A_NAME;
                m_Diags[(int)DiagEnum.DIAG_5B].Name = DIAG_5B_NAME;
                m_Diags[(int)DiagEnum.DIAG_5C].Name = DIAG_5C_NAME;
                m_Diags[(int)DiagEnum.DIAG_5T].Name = DIAG_5T_NAME;
            }

            if (bDiag6Supported)
            {
                m_Diags[(int)DiagEnum.DIAG_6].Name = DIAG_6_NAME;
            }
        }
    }

    /// <summary>
    /// This class represent a sitescan snapshot.  Each snapshot contains the a set of critical 
    /// measurements and flags to indicate the health of a service point.  Some ANSI meters can 
    /// be programmed to record snapshots when a diagnostic condition is detected.
    /// </summary>
    public class SnapshotEntry
    {
        #region Public Properties

        /// <summary>
        /// Time of the snapshot
        /// </summary>
        public DateTime SnapshotTime
        {
            get { return m_SnapshotTime; }
            set { m_SnapshotTime = value; }
        }
        /// <summary>
        /// Diagnostic that triggered the snapshot
        /// </summary>
        public int SnapshotTrigger
        {
            get { return m_SnapshotTrigger; }
            set { m_SnapshotTrigger = value; }
        }
        /// <summary>
        /// The program id
        /// </summary>
        public int ProgramID
        {
            get { return m_ProgramID; }
            set { m_ProgramID = value; }
        }
        /// <summary>
        /// The meter form
        /// </summary>
        public string MeterForm
        {
            get { return m_MeterForm; }
            set { m_MeterForm = value; }
        }
        /// <summary>
        /// The meter service
        /// </summary>
        public string MeterService
        {
            get { return m_MeterService; }
            set { m_MeterService = value; }
        }
        /// <summary>
        /// The firmware version.revision
        /// </summary>
        public float FirmwareVersion
        {
            get { return m_FirmwareVersion; }
            set { m_FirmwareVersion = value; }
        }
        /// <summary>
        /// Peak demand current
        /// </summary>
        public double PeakDemandCurrent
        {
            get { return m_PeakDemandCurrent; }
            set { m_PeakDemandCurrent = value; }
        }
        /// <summary>
        /// Instantaneous Watts
        /// </summary>
        public double InsWatt
        {
            get { return m_InsWatt; }
            set { m_InsWatt = value; }
        }
        /// <summary>
        /// Instantaneous VA
        /// </summary>
        public double InsVA
        {
            get { return m_InsVA; }
            set { m_InsVA = value; }
        }
        /// <summary>
        /// Instantaneous Vars
        /// </summary>
        public double InsVar
        {
            get { return m_InsVar; }
            set { m_InsVar = value; }
        }
        /// <summary>
        /// Instantaneous PF
        /// </summary>
        public double InsPF
        {
            get { return m_InsPF; }
            set { m_InsPF = value; }
        }
        /// <summary>
        /// Neutral Amps
        /// </summary>
        public double IRMS
        {
            get { return m_IRMS; }
            set { m_IRMS = value; }
        }
        /// <summary>
        /// Line frequency
        /// </summary>
        public double LineFrequency
        {
            get { return m_LineFrequency; }
            set { m_LineFrequency = value; }
        }
        /// <summary>
        /// Voltage
        /// </summary>
        public double VoltagePhaseA
        {
            get { return m_VoltagePhaseA; }
            set { m_VoltagePhaseA = value; }
        }
        /// <summary>
        /// Voltage angle
        /// </summary>
        public double VoltageAnglePhaseA
        {
            get { return m_VoltageAnglePhaseA; }
            set { m_VoltageAnglePhaseA = value; }
        }
        /// <summary>
        /// Current
        /// </summary>
        public double CurrentPhaseA
        {
            get { return m_CurrentPhaseA; }
            set { m_CurrentPhaseA = value; }
        }
        /// <summary>
        /// Current angle
        /// </summary>
        public double CurrentAnglePhaseA
        {
            get { return m_CurrentAnglePhaseA; }
            set { m_CurrentAnglePhaseA = value; }
        }
        /// <summary>
        /// Voltage
        /// </summary>
        public double VoltagePhaseB
        {
            get { return m_VoltagePhaseB; }
            set { m_VoltagePhaseB = value; }
        }
        /// <summary>
        /// Voltage angle
        /// </summary>
        public double VoltageAnglePhaseB
        {
            get { return m_VoltageAnglePhaseB; }
            set { m_VoltageAnglePhaseB = value; }
        }
        /// <summary>
        /// Current
        /// </summary>
        public double CurrentPhaseB
        {
            get { return m_CurrentPhaseB; }
            set { m_CurrentPhaseB = value; }
        }
        /// <summary>
        /// Current angle
        /// </summary>
        public double CurrentAnglePhaseB
        {
            get { return m_CurrentAnglePhaseB; }
            set { m_CurrentAnglePhaseB = value; }
        }
        /// <summary>
        /// Voltage
        /// </summary>
        public double VoltagePhaseC
        {
            get { return m_VoltagePhaseC; }
            set { m_VoltagePhaseC = value; }
        }
        /// <summary>
        /// Voltage angle
        /// </summary>
        public double VoltageAnglePhaseC
        {
            get { return m_VoltageAnglePhaseC; }
            set { m_VoltageAnglePhaseC = value; }
        }
        /// <summary>
        /// Current
        /// </summary>
        public double CurrentPhaseC
        {
            get { return m_CurrentPhaseC; }
            set { m_CurrentPhaseC = value; }
        }
        /// <summary>
        /// Current angle
        /// </summary>
        public double CurrentAnglePhaseC
        {
            get { return m_CurrentAnglePhaseC; }
            set { m_CurrentAnglePhaseC = value; }
        }
        /// <summary>
        /// Diagnostic counter
        /// </summary>
        public int Diag1Counter
        {
            get { return m_Diag1Counter; }
            set { m_Diag1Counter = value; }
        }
        /// <summary>
        /// Diagnostic counter
        /// </summary>
        public int Diag2Counter
        {
            get { return m_Diag2Counter; }
            set { m_Diag2Counter = value; }
        }
        /// <summary>
        /// Diagnostic counter
        /// </summary>
        public int Diag3Counter
        {
            get { return m_Diag3Counter; }
            set { m_Diag3Counter = value; }
        }
        /// <summary>
        /// Diagnostic counter
        /// </summary>
        public int Diag4Counter
        {
            get { return m_Diag4Counter; }
            set { m_Diag4Counter = value; }
        }
        /// <summary>
        /// Diagnostic counter
        /// </summary>
        public int Diag5ACounter
        {
            get { return m_Diag5ACounter; }
            set { m_Diag5ACounter = value; }
        }
        /// <summary>
        /// Diagnostic counter
        /// </summary>
        public int Diag5BCounter
        {
            get { return m_Diag5BCounter; }
            set { m_Diag5BCounter = value; }
        }
        /// <summary>
        /// Diagnostic counter
        /// </summary>
        public int Diag5CCounter
        {
            get { return m_Diag5CCounter; }
            set { m_Diag5CCounter = value; }
        }
        /// <summary>
        /// Diagnostic counter
        /// </summary>
        public int Diag5TCounter
        {
            get { return m_Diag5TCounter; }
            set { m_Diag5TCounter = value; }
        }
        /// <summary>
        /// Diagnostic counter
        /// </summary>
        public int Diag6Counter
        {
            get { return m_Diag6Counter; }
            set { m_Diag6Counter = value; }
        }
        /// <summary>
        /// Counter
        /// </summary>
        public int PowerOutageCount
        {
            get { return m_PowerOutageCount; }
            set { m_PowerOutageCount = value; }
        }
        /// <summary>
        /// Counter
        /// </summary>
        public int VQSagCounter
        {
            get { return m_VQSagCounter; }
            set { m_VQSagCounter = value; }
        }
        /// <summary>
        /// Counter
        /// </summary>
        public int VQSwellCounter
        {
            get { return m_VQSwellCounter; }
            set { m_VQSwellCounter = value; }
        }
        /// <summary>
        /// Counter
        /// </summary>
        public int VQInterruptionCounter
        {
            get { return m_VQInterruptionCounter; }
            set { m_VQInterruptionCounter = value; }
        }
        /// <summary>
        /// Counter
        /// </summary>
        public int VQImbalanceVCounter
        {
            get { return m_VQImbalanceVCounter; }
            set { m_VQImbalanceVCounter = value; }
        }
        /// <summary>
        /// Counter
        /// </summary>
        public int VQImbalanceICounter
        {
            get { return m_VQImbalanceICounter; }
            set { m_VQImbalanceICounter = value; }
        }
        /// <summary>
        /// % THD value
        /// </summary>
        public double THDPhaseA
        {
            get { return m_THDPhaseA; }
            set { m_THDPhaseA = value; }
        }
        /// <summary>
        /// % THD value
        /// </summary>
        public double THDPhaseB
        {
            get { return m_THDPhaseB; }
            set { m_THDPhaseB = value; }
        }
        /// <summary>
        /// % THD value
        /// </summary>
        public double THDPhaseC
        {
            get { return m_THDPhaseC; }
            set { m_THDPhaseC = value; }
        }
        /// <summary>
        /// % TDD value
        /// </summary>
        public double TDDPhaseA
        {
            get { return m_TDDPhaseA; }
            set { m_TDDPhaseA = value; }
        }
        /// <summary>
        /// % TDD value
        /// </summary>
        public double TDDPhaseB
        {
            get { return m_TDDPhaseB; }
            set { m_TDDPhaseB = value; }
        }
        /// <summary>
        /// % TDD value
        /// </summary>
        public double TDDPhaseC
        {
            get { return m_TDDPhaseC; }
            set { m_TDDPhaseC = value; }
        }
        /// <summary>
        /// Flag to indicate whether peak current is supported
        /// </summary>
        public bool PeakCurrentSupported
        {
            get { return m_bPeakCurrentSupported; }
            set { m_bPeakCurrentSupported = value; }
        }
        /// <summary>
        /// Flag to indicate whether I RMS is supported
        /// </summary>
        public bool IRMSSupported
        {
            get { return m_bIRMSSupported; }
            set { m_bIRMSSupported = value; }
        }
        /// <summary>
        /// Flag to indicate whether power quality items are supported
        /// </summary>
        public bool PQSupported
        {
            get { return m_bPQSupported; }
            set { m_bPQSupported = value; }
        }

        #endregion Public Properties

        #region Members

        private DateTime m_SnapshotTime;
        private int m_SnapshotTrigger;
        private int m_ProgramID;
        private string m_MeterForm;
        private string m_MeterService;
        private float m_FirmwareVersion;
        private double m_PeakDemandCurrent;
        private double m_InsWatt;
        private double m_InsVA;
        private double m_InsVar;
        private double m_InsPF;
        private double m_IRMS;
        private double m_LineFrequency;
        private double m_VoltagePhaseA;
        private double m_VoltageAnglePhaseA;
        private double m_CurrentPhaseA;
        private double m_CurrentAnglePhaseA;
        private double m_VoltagePhaseB;
        private double m_VoltageAnglePhaseB;
        private double m_CurrentPhaseB;
        private double m_CurrentAnglePhaseB;
        private double m_VoltagePhaseC;
        private double m_VoltageAnglePhaseC;
        private double m_CurrentPhaseC;
        private double m_CurrentAnglePhaseC;
        private int m_Diag1Counter;
        private int m_Diag2Counter;
        private int m_Diag3Counter;
        private int m_Diag4Counter;
        private int m_Diag5ACounter;
        private int m_Diag5BCounter;
        private int m_Diag5CCounter;
        private int m_Diag5TCounter;
        private int m_Diag6Counter;
        private int m_PowerOutageCount;

        //Only support if PQ is enabled
        private int m_VQSagCounter;
        private int m_VQSwellCounter;
        private int m_VQInterruptionCounter;
        private int m_VQImbalanceVCounter;
        private int m_VQImbalanceICounter;
        private double m_THDPhaseA;
        private double m_THDPhaseB;
        private double m_THDPhaseC;
        private double m_TDDPhaseA;
        private double m_TDDPhaseB;
        private double m_TDDPhaseC;

        private bool m_bPeakCurrentSupported;
        private bool m_bIRMSSupported;
        private bool m_bPQSupported;

        #endregion Members
    }
}
