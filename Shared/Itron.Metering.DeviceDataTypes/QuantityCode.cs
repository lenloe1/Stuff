using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.DeviceDataTypes
{
    /// <summary>
    /// Base Quantity Code class.
    /// </summary>
    public class QuantityCode
    {

        #region Constants

        private const uint QUANTITY_TYPE_MASK = 0x0000000F;

        #endregion

        #region Definitions

        /// <summary>
        /// Enumeration for the various quantity types.
        /// </summary>
        public enum QuantityTypes : uint
        {
#pragma warning disable 1591 // Ignores the XML comment warnings
            Undefined = 0x00000000,
            UserDefined = 0x00000001,
            Electrical = 0x00000002,
            NonRegister = 0x00000003,
            Event = 0x00000004,
            Coincident = 0x00000005,
            Harmonics = 0x00000006,
            IO = 0x00000007,
            Totalized = 0x00000008,
            VQ = 0x00000009,
            Extrema = 0x0000000A,
            Volume = 0x0000000B,
            TempPressureAndVolume = 0x0000000B,
            RatesExtrema = 0x0000000C,
            ExtendedCoincident = 0x0000000D,
#pragma warning restore 1591
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the quantity code base upon the quantity type.
        /// </summary>
        /// <param name="code">The raw quantity code.</param>
        /// <returns>The quantity code object.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public static QuantityCode Create(uint code)
        {
            QuantityCode CurrentCode = new QuantityCode(code);

            switch(CurrentCode.QuantityType)
            {
                case QuantityTypes.Coincident:
                {
                    CurrentCode = new CoincidentQuantityCode(code);
                    break;
                }
                case QuantityTypes.Electrical:
                {
                    CurrentCode = new ElectricalQuantityCode(code);
                    break;
                }
                case QuantityTypes.Event:
                {
                    CurrentCode = new EventQuantityCode(code);
                    break;
                }
                case QuantityTypes.ExtendedCoincident:
                {
                    CurrentCode = new ExtendedCoincidentQuantityCode(code);
                    break;
                }
                case QuantityTypes.Extrema:
                {
                    CurrentCode = new ExtremaQuantityCode(code);
                    break;
                }
                case QuantityTypes.Harmonics:
                {
                    CurrentCode = new HarmonicQuantityCode(code);
                    break;
                }
                case QuantityTypes.IO:
                {
                    CurrentCode = new IOQuantityCode(code);
                    break;
                }
                case QuantityTypes.RatesExtrema:
                {
                    CurrentCode = new ExtremaRatesQuantityCode(code);
                    break;
                }
                case QuantityTypes.Totalized:
                {
                    CurrentCode = new TotalizedQuantityCode(code);
                    break;
                }
                case QuantityTypes.VQ:
                {
                    CurrentCode = new VQQuantityCode(code);
                    break;
                }
                default:
                {
                    // We do not have a class for these items so just use the QuantityCode object
                    break;
                }
            }

            return CurrentCode;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the type of the quantity code.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public QuantityTypes QuantityType
        {
            get
            {
                return (QuantityTypes)(m_QuantityCode & QUANTITY_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the actual code of the quantity 
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public uint Code
        {
            get
            {
                return m_QuantityCode;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is bidirectional
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public virtual bool IsBidirectional
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is leading
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public virtual bool IsLeading
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is lagging
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public virtual bool IsLagging
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Q quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public virtual bool IsQQuantity
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a VA quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public virtual bool IsVAQuantity
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Var quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public virtual bool IsVarQuantity
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Watt quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created
        
        public virtual bool IsWattQuantity
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a totalized value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public virtual bool IsTotalizedQuantity
        {
            get
            {
                return QuantityType == QuantityTypes.Totalized;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a power quality quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public virtual bool IsPowerQualityQuantity
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Protected constructor that will only be used by Create and the inherited classes.
        /// </summary>
        /// <param name="code">The actual quantity code.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        protected QuantityCode(uint code)
        {
            m_QuantityCode = code;
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// Member variable for the quantity code.
        /// </summary>
        protected uint m_QuantityCode;

        #endregion
    }

    /// <summary>
    /// Quantity code class for electrical quantities
    /// </summary>
    public class ElectricalQuantityCode : QuantityCode
    {
        #region Constants

        // Electrical Quantity Masks
        private const uint ELEC_UNIT_MASK = 0x000001F0;
        private const uint MEAS_TYPE_MASK = 0x00003E00;
        private const uint SUB_TYPE_MASK = 0x0000C000;
        private const uint DIRECTION_MASK = 0x000F0000;
        private const uint SCALE_MASK = 0x00300000;
        private const uint TOU_RATE_MASK = 0x0FC00000;
        private const uint PHASE_MASK = 0xF0000000;

        #endregion

        #region Definitions

        /// <summary>
        /// Electrical units.
        /// </summary>
        public enum ElectricalUnit : uint
        {
#pragma warning disable 1591
            WattHour = 0x00000000,
            VarHour = 0x00000010,
            VAHour = 0x00000020,
            VoltHour = 0x00000030,
            AmpHour = 0x00000040,
            NeutralAmpHour = 0x00000050,
            VSquaredHour = 0x00000060,
            QHour = 0x00000070,
            Watt = 0x00000080,
            Var = 0x00000090,
            VA = 0x000000A0,
            Volt = 0x000000B0,
            Amp = 0x000000C0,
            VSquared = 0x000000D0,
            AmpSquared = 0x000000E0,
            NeutralAmp = 0x000000F0,
            PF = 0x00000100,
            Q = 0x00000110,
            PFHour = 0x00000120,
            Hertz = 0x00000130,
            HertzHour = 0x00000140,
            AmpSquaredHour = 0x00000150,
            Seconds = 0x00000160,
            CumulatedSeconds = 0x00000170,
            ExcessCount = 0x00000180,
            Ufer = 0x00000190,
            DMCR = 0x000001A0,
#pragma warning restore 1591
        }

        /// <summary>
        /// Electrical Measurement Types
        /// </summary>
        public enum ElectricalMeasurementType : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            ReverseDemand = 0x00000200,
            ThermalDemand = 0x00000400,
            MaxDemand = 0x00000600,
            ThermalMaxDemand = 0x00000800,
            MinDemand = 0x00000A00,
            MinValue = 0x00000C00,
            AverageValue = 0x00000E00,
            InstDemand = 0x00001000,
            InstValue = 0x00001200,
            InstPeakDemand = 0x00001400,
            PresentDemand = 0x00001600,
            ThermalPresDemand = 0x00001800,
            PreviousDemand = 0x00001A00,
            PreviousValue = 0x00001C00,
            CumDemand = 0x00001E00,
            CCumDemand = 0x00002000,
            ThermalCumDemand = 0x00002200,
            ThermalCCumDemand = 0x00002400,
            ProjectedDemand = 0x00002600,
            InstPeakValue = 0x00002800,
            PresentValue = 0x00002A00,
            InstMinDemand = 0x00002C00,
            ExcessDemand = 0x00002E00,
            ExcessValue = 0x00002E00,
            TotalizedValue = 0x00003000,
            ThermalMinDemand = 0x00003200,
            TOOMinDemand = 0x00003400,
            TOOMaxDemand = 0x00003600,
            TOOMinThermalDemand = 0x00003800,
            TOOMaxThermalDemand = 0x00003A00,
            TOOMinInstDemand = 0x00003C00,
            TOOMaxInstDemand = 0x00003E00,
#pragma warning restore 1591
        }

        /// <summary>
        /// Electrical sub types
        /// </summary>
        public enum ElectricalSubType : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Arithmetic = 0x00004000,
            Vectoral = 0x00008000,
            Fundamental = 0x0000C000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Electrical direction
        /// </summary>
        public enum ElectricalDirection : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Delivered = 0x00010000,
            Received = 0x00020000,
            Leading = 0x00030000,
            Lagging = 0x00040000,
            Q1 = 0x00050000,
            Q2 = 0x00060000,
            Q3 = 0x00070000,
            Q4 = 0x00080000,
            Net = 0x00090000,
            Unbalance = 0x000A0000,
            Distortion = 0x000B0000,
            ForPF = 0x000C0000,
            NetQ1Q3 = 0x000D0000,
            NetQ4Q2 = 0x000E0000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Electrical scale
        /// </summary>
        public enum ElectricalScale : uint
        {
#pragma warning disable 1591
            Units = 0x00000000,
            Kilo = 0x00100000,
            Mega = 0x00200000,
            Giga = 0x00300000,
            OccurrenceDate = 0x00100000,
            OccurrenceTime = 0x00200000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Electrical TOU Rate
        /// </summary>
        public enum ElectricalTOURate : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Rate1 = 0x00400000,
            Rate2 = 0x00800000,
            Rate3 = 0x00C00000,
            Rate4 = 0x01000000,
            Rate5 = 0x01400000,
            Rate6 = 0x01800000,
            Rate7 = 0x01C00000,
            Rate8 = 0x02000000,
            Rate9 = 0x02400000,
            Rate10 = 0x02800000,
            Rate11 = 0x02C00000,
            Rate12 = 0x03000000,
            Rate13 = 0x03400000,
            Rate14 = 0x03800000,
            Rate15 = 0x03C00000,
            Rate16 = 0x04000000,
            Rate17 = 0x04400000,
            Rate18 = 0x04800000,
            Rate19 = 0x04C00000,
            Rate20 = 0x05000000,
            Rate21 = 0x05400000,
            Rate22 = 0x05800000,
            Rate23 = 0x05C00000,
            Rate24 = 0x06000000,
            Rate25 = 0x06400000,
            Rate26 = 0x06800000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Electrical phase
        /// </summary>
        public enum ElectricalPhase : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Aggregate = 0x00000000,
            PhaseA = 0x10000000,
            PhaseB = 0x20000000,
            PhaseC = 0x30000000,
            Average = 0x40000000,
            AggregateSLC = 0x50000000,
            AggregateTLC = 0x60000000,
#pragma warning restore 1591
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unit of the quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ElectricalUnit Unit
        {
            get
            {
                return (ElectricalUnit)(m_QuantityCode & ELEC_UNIT_MASK);
            }
        }

        /// <summary>
        /// Gets the measurement type of the quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ElectricalMeasurementType MeasurementType
        {
            get
            {
                return (ElectricalMeasurementType)(m_QuantityCode & MEAS_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the sub type of the quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ElectricalSubType SubType
        {
            get
            {
                return (ElectricalSubType)(m_QuantityCode & SUB_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the direction of the quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ElectricalDirection Direction
        {
            get
            {
                return (ElectricalDirection)(m_QuantityCode & DIRECTION_MASK);
            }
        }

        /// <summary>
        /// Gets the scale of the quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ElectricalScale Scale
        {
            get
            {
                return (ElectricalScale)(m_QuantityCode & SCALE_MASK);
            }
            set
            {
                //Flip the bits on the scale mask to clear out scale in
                //the quantity code.
                uint uiClearScaleMask = SCALE_MASK ^ 0xFFFFFFFF;
                m_QuantityCode &= uiClearScaleMask;
                m_QuantityCode |= (uint)value;
            }
        }

        /// <summary>
        /// Gets the TOU rate of the quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ElectricalTOURate TOURate
        {
            get
            {
                return (ElectricalTOURate)(m_QuantityCode & TOU_RATE_MASK);
            }
        }

        /// <summary>
        /// Gets the phase of the quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ElectricalPhase Phase
        {
            get
            {
                return (ElectricalPhase)(m_QuantityCode & PHASE_MASK);
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is bidirectional
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public override bool IsBidirectional
        {
            get
            {
                return Direction == ElectricalDirection.Received
                    || Direction == ElectricalDirection.Net
                    || Direction == ElectricalDirection.NetQ1Q3
                    || Direction == ElectricalDirection.NetQ4Q2;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is leading.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public override bool IsLeading
        {
            get
            {
                return Direction == ElectricalDirection.Leading;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is lagging.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public override bool IsLagging
        {
            get
            {
                return Direction == ElectricalDirection.Lagging;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a power quality quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public override bool IsPowerQualityQuantity
        {
            get
            {
                return Unit == ElectricalUnit.Amp
                    || Unit == ElectricalUnit.AmpHour
                    || Unit == ElectricalUnit.AmpSquared
                    || Unit == ElectricalUnit.AmpSquaredHour
                    || Unit == ElectricalUnit.NeutralAmp
                    || Unit == ElectricalUnit.NeutralAmpHour
                    || Unit == ElectricalUnit.Volt
                    || Unit == ElectricalUnit.VoltHour
                    || Unit == ElectricalUnit.VSquared
                    || Unit == ElectricalUnit.VSquaredHour;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Q quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public override bool IsQQuantity
        {
            get
            {
                return Unit == ElectricalUnit.Q
                    || Unit == ElectricalUnit.QHour;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a VA quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public override bool IsVAQuantity
        {
            get
            {
                return Unit == ElectricalUnit.VA
                    || Unit == ElectricalUnit.VAHour;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Var quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public override bool IsVarQuantity
        {
            get
            {
                return Unit == ElectricalUnit.Var
                    || Unit == ElectricalUnit.VarHour;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Watt quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created
        
        public override bool IsWattQuantity
        {
            get
            {
                return Unit == ElectricalUnit.Watt
                    || Unit == ElectricalUnit.WattHour;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is an energy quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/08 jrf 9.50           Created
        public bool IsEnergyQuantity
        {
            get
            {
                return Unit == ElectricalUnit.AmpHour
                    || Unit == ElectricalUnit.AmpSquaredHour
                    || Unit == ElectricalUnit.HertzHour
                    || Unit == ElectricalUnit.NeutralAmpHour
                    || Unit == ElectricalUnit.PFHour
                    || Unit == ElectricalUnit.QHour
                    || Unit == ElectricalUnit.VAHour
                    || Unit == ElectricalUnit.VarHour
                    || Unit == ElectricalUnit.VoltHour
                    || Unit == ElectricalUnit.VSquaredHour
                    || Unit == ElectricalUnit.WattHour;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is an demand quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/08 jrf 9.50           Created
        public bool IsDemandQuantity
        {
            get
            {
                return (false == IsEnergyQuantity);
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is an cumulative quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/08 jrf 9.50           Created
        public bool IsCumulativeQuantity
        {
            get
            {
                return MeasurementType == ElectricalMeasurementType.CCumDemand
                    || MeasurementType == ElectricalMeasurementType.CumDemand
                    || MeasurementType == ElectricalMeasurementType.ThermalCCumDemand
                    || MeasurementType == ElectricalMeasurementType.ThermalCumDemand;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be used by QuantityCode.Create
        /// </summary>
        /// <param name="code">The actual quantity code.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        internal ElectricalQuantityCode(uint code)
            : base(code)
        {
        }

        #endregion
    }

    /// <summary>
    /// Quantity code class for information quantities
    /// </summary>
    public class InformationQuantityCode : QuantityCode
    {
        #region Constants

        // Information Quantity Masks
        private const uint INFO_UNIT_MASK = 0x0000FFF0;

        #endregion

        #region Definitions

        /// <summary>
        /// Information units
        /// </summary>
        public enum InformationUnit : uint
        {
#pragma warning disable 1591
            NonFatalError = 0x00000000,
            DiagnosticErrorCode = 0x00000010,
            SegmentTest = 0x00000020,
            CurrentDate = 0x00000030,
            CurrentTime = 0x00000040,
            CurrentDayOfWeek = 0x00000050,
            MeterID = 0x00000060,
            MeterID2 = 0x00000070,
            DaysOnBattery = 0x00000080,
            MinutesOnBattery = 0x00000090,
            ProgramID = 0x000000A0,
            SerialNumber = 0x000000B0,
            TimeInSubinterval = 0x000000C0,
            IntervalLength = 0x000000D0,
            SubintervalLength = 0x000000E0,
            NumSubintervals = 0x000000F0,
            NormalKh = 0x00000100,
            TestIntervalLength = 0x00000110,
            TestSubintervalLength = 0x00000120,
            NumTestSubintervals = 0x00000130,
            TestKh = 0x00000140,
            KYZ1Pulseweight = 0x00000150,
            KYZ2Pulseweight = 0x00000160,
            KYZ3Pulseweight = 0x00000170,
            KYZ4Pulseweight = 0x00000180,
            FirmwareVersion = 0x00000190,
            ProcessorVersion = 0x000001A0,
            SoftwareVersion = 0x000001B0,
            InstMultiplier = 0x000001C0,
            CTRatio = 0x000001D0,
            VTRatio = 0x000001E0,
            TransformerRatio = 0x000001F0,
            RegMultiplier = 0x00000200,
            RegisterFullscale = 0x00000210,
            CPLUTime = 0x00000220,
            TOUExpirationDate = 0x00000230,
            TOUScheduleID = 0x00000240,
            CalibrationDate = 0x00000250,
            CalibrationTime = 0x00000260,
            LastProgramDate = 0x00000270,
            LastProgramTime = 0x00000280,
            DaysSinceProgram = 0x00000290,
            LastInterrogatedDate = 0x000002A0,
            LastInterrogatedTime = 0x000002B0,
            LastResetDate = 0x000002C0,
            LastResetTime = 0x000002D0,
            LastTestDate = 0x000002E0,
            LastTestTime = 0x000002F0,
            LastOutageDate = 0x00000300,
            LastOutageTime = 0x00000310,
            DaysNoLoad = 0x00000320,
            ProgramCount = 0x00000330,
            InterrogationCount = 0x00000340,
            OutageCount = 0x00000350,
            PotentialLossCount = 0x00000360,
            DemandResetCount = 0x00000370,
            DaysSinceReset = 0x00000380,
            TestCount = 0x00000390,
            TestModeLength = 0x000003A0,
            DisplayOnTime = 0x000003B0,
            UserData1 = 0x000003C0,
            UserData2 = 0x000003D0,
            UserData3 = 0x000003E0,
            AngleIaToVa = 0x000003F0,
            AngleIbToVa = 0x00000400,
            AngleIcToVa = 0x00000410,
            AngleVbToVa = 0x00000420,
            AngleVcToVa = 0x00000430,
            DemandThreshold = 0x00000440,
            TimeRemInTestMode = 0x00000450,
            TestNumPulses = 0x00000460,
            TestPrevNumPulses = 0x00000470,
            PrevNumPulses = 0x00000480,
            UserData4 = 0x00000490,
            UserData5 = 0x000004A0,
            UserData6 = 0x000004B0,
            UserData7 = 0x000004C0,
            UserData8 = 0x000004D0,
            UserData9 = 0x000004E0,
            UserData10 = 0x000004F0,
            UserData11 = 0x00000500,
            UserData12 = 0x00000510,
            UserData13 = 0x00000520,
            UserData14 = 0x00000530,
            UserData15 = 0x00000540,
            UserData16 = 0x00000550,
            UserData17 = 0x00000560,
            UserData18 = 0x00000570,
            UserData19 = 0x00000580,
            UserData20 = 0x00000590,
            UserData21 = 0x000005A0,
            UserData22 = 0x000005B0,
            UserData23 = 0x000005C0,
            UserData24 = 0x000005D0,
            CustomRatio = 0x000005E0,
            AltSwitchInTest = 0x000005F0,
            TestSwitchInTest = 0x00000600,
            ScrollSwitchInTest = 0x00000610,
            ResetSwitchInTest = 0x00000620,
            LineFrequency = 0x00000630,
            DateFormat = 0x00000640,
            CurrentFrequency = 0x00000650,
            ResetReason = 0x00000660,
            CurrentTOURate = 0x00000670,
            TOURateInfo = 0x00000680,
            TOUCalendarInfo = 0x00000690,
            RealTimePricingRate = 0x000006A0,
            ThermIntervalLength = 0x000006B0,
            ThermSubintervalLength = 0x000006C0,
            AltSwitchInNormal = 0x000006D0,
            TestSwitchInNormal = 0x000006E0,
            ScrollSwitchInNormal = 0x000006F0,
            ResetSwitchInNormal = 0x00000700,
            OpticalPortSetup1 = 0x00000710,
            OpticalPortSetup2 = 0x00000720,
            OpticalPortSetup3 = 0x00000730,
            Ser1PortSetup1 = 0x00000740,
            Ser1PortSetup2 = 0x00000750,
            Ser1PortSetup3 = 0x00000760,
            Ser2PortSetup1 = 0x00000770,
            Ser2PortSetup2 = 0x00000780,
            Ser2PortSetup3 = 0x00000790,
            MeterAddressOnOpt = 0x000007A0,
            MeterAddressOnSer1 = 0x000007B0,
            MeterAddressOnSer2 = 0x000007C0,
            GroupAddressOnOpt = 0x000007D0,
            GroupAddressOnSer1 = 0x000007E0,
            GroupAddressOnSer2 = 0x000007F0,
            ProtocolOnOpt = 0x00000800,
            ProtocolOnSer1 = 0x00000810,
            ProtocolOnSer2 = 0x00000820,
            BaudRateOnOpt = 0x00000830,
            BaudRateOnSer1 = 0x00000840,
            BaudRateOnSer2 = 0x00000850,
            OnlineOnOpt = 0x00000860,
            OnlineOnSer1 = 0x00000870,
            OnlineOnSer2 = 0x00000880,
            CompleteInitCount = 0x00000890,
            KYZ5Pulseweight = 0x000008B0,
            KYZ6Pulseweight = 0x000008C0,
            KYZ7Pulseweight = 0x000008D0,
            KYZ8Pulseweight = 0x000008E0,
            LineSynched = 0x000008F0,
            CalibCount = 0x00000900,
            DemandThreshold2 = 0x00000910,
            PFThreshold1 = 0x00000920,
            PFThreshold2 = 0x00000930,
            TimeInDemandInt = 0x00000940,
            ThermTestInLength = 0x00000950,
            KYZInWeight1 = 0x00000960,
            KYZInWeight2 = 0x00000970,
            KYZInWeight3 = 0x00000980,
            KYZInWeight4 = 0x00000990,
            KYZInWeight5 = 0x000009A0,
            KYZInWeight6 = 0x000009B0,
            KYZInWeight7 = 0x000009C0,
            KYZInWeight8 = 0x000009D0,
            KYZInWeight9 = 0x000009E0,
            KYZInWeight10 = 0x000009F0,
            KYZInWeight11 = 0x00000A00,
            KYZInWeight12 = 0x00000A10,
            KYZInWeight13 = 0x00000A20,
            KYZInWeight14 = 0x00000A30,
            KYZInWeight15 = 0x00000A40,
            KYZInWeight16 = 0x00000A50,
            MeterStatus = 0x00000A60,
            KYZ1PDR = 0x00000A70,
            DRLoTime = 0x00000A80,
            SetupInfo = 0x00000A90,
            URAMTestStatus = 0x00000AA0,
            CurrentYear = 0x00000AB0,
            CLPUOutageTime = 0x00000AC0,
            ErrorStatus = 0x00000AD0,
            IOStatus = 0x00000AE0,
            TextStatus = 0x00000AF0,
            BatteryVoltage = 0x00000B00,
            SecondsOnBattery = 0x00000B10,
            NextSelfReadDate = 0x00000B20,
            NextSelfReadTime = 0x00000B30,
            NextSelfReadAction = 0x00000B40,
            LastSelfReadDate = 0x00000B50,
            LastSelfReadTime = 0x00000B60,
            LastSelfReadReason = 0x00000B70,
            DateTimeAlarmStatus = 0x00000B80,
            RegAlarmStatus1 = 0x00000B90,
            RegAlarmStatus2 = 0x00000BA0,
            DigitalStateOut = 0x00000BB0,
            DigitalStateIn = 0x00000BC0,
            IOStatusStateOutput = 0x00000BD0,
            IOStatusStateInput = 0x00000BE0,
            IOStatusPulseOutput = 0x00000BF0,
            IOStatusPulseInput = 0x00000C00,
            IOStatusAnalogOutput = 0x00000C10,
            IOStatusAnalogInput = 0x00000C20,
            BlockEncoder1 = 0x00000C30,
            BlockEncoder2 = 0x00000C40,
            BlockEncoder3 = 0x00000C50,
            BlockEncoder4 = 0x00000C60,
            CurrentEncoder1 = 0x00000C70,
            CurrentEncoder2 = 0x00000C80,
            CurrentEncoder3 = 0x00000C90,
            CurrentEncoder4 = 0x00000CA0,
            TimeRemInCLPU = 0x00000CB0,
            Ser1LastIntTime = 0x00000CC0,
            Ser1LastIntDate = 0x00000CD0,
            Ser2LastIntTime = 0x00000CE0,
            Ser2LastIntDate = 0x00000CF0,
            NormalKh2 = 0x00000D00,
            TestKh2 = 0x00000D10,
            Ser1IntCount = 0x00000D20,
            Ser2IntCount = 0x00000D30,
            ProfileResearchID = 0x00000D40,
            ProfileResearchID2 = 0x00000D50,
            SerialNumber2 = 0x00000D60,
            PotentialIndicators = 0x00000D70,
            LowExtremeVoltageDuration = 0x00000D80,
            NormExtremeVoltageDuration = 0x00000D90,
            HighExtremeVoltageDuration = 0x00000DA0,
            PulseOutputConfig = 0x00000DB0,
            BlankDisplay = 0x00000DC0,
            DemandThreshold3 = 0x00000DD0,
            DemandThreshold4 = 0x00000DE0,
            DemandThreshold5 = 0x00000DF0,
            DemandThreshold6 = 0x00000E00,
            DemandThreshold7 = 0x00000E10,
            DemandThreshold8 = 0x00000E20,
            DemandThreshold9 = 0x00000E30,
            TariffType = 0x00000E40,
            Reserved1 = 0x00000E50,
            Reserved2 = 0x00000E60,
            Reserved3 = 0x00000E70,
            Reserved4 = 0x00000E80,
            ProtocolFatalErrors = 0x00000E90,
            ProtocolSessionErrors = 0x00000EA0,
            OutputType = 0x00000EB0,
            KDCRatio = 0x00000EC0,
            KDCDRatio = 0x00000ED0,
            JoulesLossRatio = 0x00000EE0,
            KVarhRatio = 0x00000EF0,
            IronLossRatio = 0x00000F00,
            SlowVariationLowThreshold = 0x00000F10,
            SlowVariationHighThreshold = 0x00000F20,
            NominalVoltage = 0x00000F30,
            BillingAction = 0x00000F40,
            StartBillingPeriod = 0x00000F50,
            EndBillingPeriod = 0x00000F60,
            DemandIntervalLength = 0x00000F70,
            DemandThreshold10 = 0x00000F80,
            DemandThreshold11 = 0x00000F90,
            DemandThreshold12 = 0x00000FA0,
            DemandThreshold13 = 0x00000FB0,
            DemandThreshold14 = 0x00000FC0,
            Holiday1 = 0x00000FD0,
            Holiday2 = 0x00000FE0,
            Holiday3 = 0x00000FF0,
            Holiday4 = 0x00001000,
            Holiday5 = 0x00001010,
            Holiday6 = 0x00001020,
            Holiday7 = 0x00001030,
            Holiday8 = 0x00001040,
            Holiday9 = 0x00001050,
            Holiday10 = 0x00001060,
            Holiday11 = 0x00001070,
            Holiday12 = 0x00001080,
            Holiday13 = 0x00001090,
            Holiday14 = 0x000010A0,
            Holiday15 = 0x000010B0,
            Holiday16 = 0x000010C0,
            Holiday17 = 0x000010D0,
            Holiday18 = 0x000010E0,
            Holiday19 = 0x000010F0,
            Holiday20 = 0x00001100,
            TanPhi = 0x00001110,
            ExcessEnergy1 = 0x00001120,
            NonStandardMode = 0x00001130,
            PotentialLoss = 0x00001140,
            CurrentLoss = 0x00001150,
            PresenceEJP = 0x00001160,
            NumberEJPPeriodsExceeded = 0x00001170,
            MaxPowerExceedsPercentageRateThreshold = 0x00001180,
            MaxPowerLessThanPercentagePreviousMaxPower = 0x00001190,
            RatioOfCompressibility = 0x000011A0,
            OverallCorrectionFactor = 0x000011B0,
            PulseWeight = 0x000011C0,
            AtmosphericPressure = 0x000011D0,
            SpecificGravity = 0x000011E0,
            PercentN2 = 0x000011F0,
            PercentCO2 = 0x00001200,
            FallbackPressure = 0x00001210,
            FallbackTemp = 0x00001220,
            NumPulses = 0x00001230,
            OptionStatus1 = 0x00001240,
            OptionStatus2 = 0x00001250,
            OptionStatus3 = 0x00001260,
            IOStatusStateOutput2 = 0x00001270,
            IOStatusStateInput2 = 0x00001280,
            DailyPatternID = 0x00001290,
            HolidayID = 0x000012A0,
            TOUScheduleName = 0x000012B0,
            TOULatentStartDate = 0x000012C0,
            TOULatentStartTime = 0x000012D0,
            TOULatentScheduleID = 0x000012E0,
            TOULatentScheduleName = 0x000012F0,
            TOURatesAToGActive = 0x00001300,
            TOURateAActive = 0x00001310,
            TOURateBActive = 0x00001320,
            TOURateCActive = 0x00001330,
            TOURateDActive = 0x00001340,
            TOURateEActive = 0x00001350,
            TOURateFActive = 0x00001360,
            TOURateGActive = 0x00001370,
            ServiceType = 0x00001380,
            VendorField1 = 0x00001390,
            VendorField2 = 0x000013A0,
            VendorField3 = 0x000013B0,
            TestKhA = 0x000013C0,
            TestKhB = 0x000013D0,
            TestKhC = 0x000013E0,
            TestKhD = 0x000013F0,
            TestKhE = 0x00001400,
            TestKhF = 0x00001410,
            TestKhG = 0x00001420,
            MainBoardRev = 0x00001430,
            VQPercentLogFull = 0x00001440,
            VQEventCount = 0x00001450,
            ModemStatus = 0x00001460,
            SiteScanDiagCount = 0x00001470,
            UnknownQuantity = 0x00001480,
            EndOfTable = 0x00001490,
            TimeDateQualifier = 0x000014A0,
            LPCh1Pulseweight = 0x000014B0,
            LPCh2Pulseweight = 0x000014C0,
            LPCh3Pulseweight = 0x000014D0,
            LPCh4Pulseweight = 0x000014E0,
            LPCh5Pulseweight = 0x000014F0,
            LPCh6Pulseweight = 0x00001400,
            LPCh7Pulseweight = 0x00001410,
            LPCh8Pulseweight = 0x00001420,
#pragma warning restore 1591
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the information unit
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public InformationUnit Unit
        {
            get
            {
                return (InformationUnit)(m_QuantityCode & INFO_UNIT_MASK);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be used by QuantityCode.Create
        /// </summary>
        /// <param name="code">The actual quantity code</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        internal InformationQuantityCode(uint code)
            : base(code)
        {
        }

        #endregion
    }

    /// <summary>
    /// Quantity code class for coincident quantities
    /// </summary>
    public class CoincidentQuantityCode : QuantityCode
    {
        #region Constants

        // Coincident Quantity Masks
        private const uint PEAK_UNITS_MASK = 0x000003F0;
        private const uint PEAK_TYPE_MASK = 0x00000400;
        private const uint PEAK_PHASE_MASK = 0x00003800;
        private const uint PEAK_SCALE_MASK = 0x0000C000;
        private const uint COIN_UNITS_MASK = 0x003F0000;
        private const uint COIN_TYPE_MASK = 0x07C00000;
        private const uint COIN_PHASE_MASK = 0x38000000;
        private const uint COIN_SCALE_MASK = 0xC0000000;

        #endregion

        #region Definitions

        /// <summary>
        /// Peak units
        /// </summary>
        public enum PeakUnits : uint
        {
#pragma warning disable 1591
            Undefined = 0x00000000,
            WattDelivered = 0x00000010,
            WattReceived = 0x00000020,
            WattNet = 0x00000030,
            VarDelivered = 0x00000004,
            VarReceived = 0x00000050,
            VarNet = 0x00000060,
            VarQ1 = 0x00000070,
            VarQ2 = 0x00000080,
            VarQ3 = 0x00000090,
            VarQ4 = 0x000000A0,
            VarDeliveredVect = 0x000000B0,
            VarReceivedVect = 0x000000C0,
            VarNetVect = 0x000000D0,
            VarQ1Vect = 0x000000E0,
            VarQ2Vect = 0x000000F0,
            VarQ3Vect = 0x00000100,
            VarQ4Vect = 0x00000110,
            VADelivered = 0x00000120,
            VAReceived = 0x00000130,
            VANet = 0x00000140,
            VAQ1 = 0x00000150,
            VAQ2 = 0x00000160,
            VAQ3 = 0x00000170,
            VAQ4 = 0x00000180,
            VADeliveredVect = 0x00000190,
            VAReceivedVect = 0x000001A0,
            VANetVect = 0x000001B0,
            VAQ1Vect = 0x000001C0,
            VAQ2Vect = 0x000001D0,
            VAQ3Vect = 0x000001E0,
            VAQ4Vect = 0x000001F0,
            CoincidentVolt = 0x00000200,
            CoincidentAmp = 0x00000210,
            CoincidentVSq = 0x00000220,
            CoincidentAmpSq = 0x00000230,
            CoincidentNeutralAmp = 0x00000240,
            PowerFactorVectorial = 0x00000250,
            PowerFactorArithmetic = 0x00000260,
            CoincidentQ = 0x00000270,
            DigitalInput = 0x00000280,
            AnalogInput = 0x00000290,
            TotalizedQty = 0x000002A0,
            PercentTHDVolts = 0x000002B0,
            PercentTHDAmps = 0x000002C0,
            CoincidentPF = 0x000002D0,
            VALeading = 0x000002E0,
            VALagging = 0x000002F0,
            VarLeading = 0x00000300,
            VarLagging = 0x00000310,
            VALeadingNoType = 0x00000320,
            VALaggingNoType = 0x00000330,
            VADeliveredNoType = 0x00000340,
            VAReceivedNoType = 0x00000350,
            VANetNoType = 0x00000360,
            VAQ1NoType = 0x00000370,
            VAQ2NoType = 0x00000380,
            VAQ3NoType = 0x00000390,
            VAQ4NoType = 0x000003A0,
            WattNoDirection = 0x000003B0,
            QNet = 0x000003C0,
            WattNoDirectionLegacy = 0x000003D0,
            Totalized2 = 0x000003E0,
            Totalized3 = 0x000003F0,
#pragma warning restore 1591
        }

        /// <summary>
        /// Peak types
        /// </summary>
        public enum PeakTypes : uint
        {
#pragma warning disable 1591
            Minimum = 0x00000000,
            Maximum = 0x00000400,
#pragma warning restore 1591
        }

        /// <summary>
        /// Peak phases
        /// </summary>
        public enum PeakPhases : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            PhaseA = 0x00000800,
            PhaseB = 0x00001000,
            PhaseC = 0x00001800,
            AggregateSLC = 0x00002000,
            PhaseAverage = 0x00002800,
            AggregateTLC = 0x00003000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Peak scales
        /// </summary>
        public enum PeakScales : uint
        {
#pragma warning disable 1591
            Units = 0x00000000,
            Kilo = 0x00004000,
            Mega = 0x00008000,
            Giga = 0x0000C000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Coincident Units
        /// </summary>
        public enum CoincidentUnits : uint
        {
#pragma warning disable 1591
            Undefined = 0x00000000,
            WattDelivered = 0x00010000,
            WattReceived = 0x00020000,
            WattNet = 0x00030000,
            VarDelivered = 0x0004000,
            VarReceived = 0x00050000,
            VarNet = 0x00060000,
            VarQ1 = 0x00070000,
            VarQ2 = 0x00080000,
            VarQ3 = 0x00090000,
            VarQ4 = 0x000A0000,
            VarDeliveredVect = 0x000B0000,
            VarReceivedVect = 0x000C0000,
            VarNetVect = 0x000D0000,
            VarQ1Vect = 0x000E0000,
            VarQ2Vect = 0x000F0000,
            VarQ3Vect = 0x00100000,
            VarQ4Vect = 0x00110000,
            VADelivered = 0x00120000,
            VAReceived = 0x00130000,
            VANet = 0x00140000,
            VAQ1 = 0x00150000,
            VAQ2 = 0x00160000,
            VAQ3 = 0x00170000,
            VAQ4 = 0x00180000,
            VADeliveredVect = 0x00190000,
            VAReceivedVect = 0x001A0000,
            VANetVect = 0x001B0000,
            VAQ1Vect = 0x001C0000,
            VAQ2Vect = 0x001D0000,
            VAQ3Vect = 0x001E0000,
            VAQ4Vect = 0x001F0000,
            CoincidentVolt = 0x00200000,
            CoincidentAmp = 0x00210000,
            CoincidentVSq = 0x00220000,
            CoincidentAmpSq = 0x00230000,
            CoincidentNeutralAmp = 0x00240000,
            PowerFactorVectorial = 0x00250000,
            PowerFactorArithmetic = 0x00260000,
            CoincidentQ = 0x00270000,
            DigitalInput = 0x00280000,
            AnalogInput = 0x00290000,
            TotalizedQty = 0x002A0000,
            PercentTHDVolts = 0x002B0000,
            PercentTHDAmps = 0x002C0000,
            CoincidentPF = 0x002D0000,
            VALeading = 0x002E0000,
            VALagging = 0x002F0000,
            VarLeading = 0x00300000,
            VarLagging = 0x00310000,
            VALeadingNoType = 0x00320000,
            VALaggingNoType = 0x00330000,
            VADeliveredNoType = 0x00340000,
            VAReceivedNoType = 0x00350000,
            VANetNoType = 0x00360000,
            VAQ1NoType = 0x00370000,
            VAQ2NoType = 0x00380000,
            VAQ3NoType = 0x00390000,
            VAQ4NoType = 0x003A0000,
            WattNoDirection = 0x003B0000,
            QNet = 0x003C0000,
            WattNoDirectionLegacy = 0x003D0000,
            Totalized2 = 0x003E0000,
            Totalized3 = 0x003F0000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Coincident types
        /// </summary>
        public enum CoincidentTypes : uint
        {
#pragma warning disable 1591
            Instantaneous = 0x00000000,
            Minimum = 0x00400000,
            Average = 0x00800000,
            Maximum = 0x00C00000,
            Present = 0x01000000,
            Previous = 0x01400000,
            Cum = 0x01800000,
            CCum = 0x01C00000,
            Projected = 0x02000000,
            InstantaneousThermal = 0x02400000,
            MinimumThermal = 0x02800000,
            MaximumThermal = 0x02C00000,
            PresentThermal = 0x03000000,
            PreviousThermal = 0x03400000,
            CumThermal = 0x03800000,
            CCumThermal = 0x04000000,
            NormalDemand = 0x04400000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Coincident phases
        /// </summary>
        public enum CoincidentPhases : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            PhaseA = 0x08000000,
            PhaseB = 0x10000000,
            PhaseC = 0x18000000,
            AggregateSLC = 0x20000000,
            PhaseAverage = 0x28000000,
            AggregateTLC = 0x30000000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Coincident Scales
        /// </summary>
        public enum CoincidentScales : uint
        {
#pragma warning disable 1591
            Units = 0x00000000,
            Kilo = 0x40000000,
            Mega = 0x80000000,
            Giga = 0xC0000000,
#pragma warning restore 1591
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the units for a peak value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public PeakUnits PeakUnit
        {
            get
            {
                return (PeakUnits)(m_QuantityCode & PEAK_UNITS_MASK);
            }
        }

        /// <summary>
        /// Gets the type of a peak value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public PeakTypes PeakType
        {
            get
            {
                return (PeakTypes)(m_QuantityCode & PEAK_TYPE_MASK);
            }

        }

        /// <summary>
        /// Gets the phase of a peak value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public PeakPhases PeakPhase
        {
            get
            {
                return (PeakPhases)(m_QuantityCode & PEAK_PHASE_MASK);
            }
        }

        /// <summary>
        /// Gets the scale of a peak value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public PeakScales PeakScale
        {
            get
            {
                return (PeakScales)(m_QuantityCode & PEAK_SCALE_MASK);
            }
        }

        /// <summary>
        /// Gets the unit of a coincident value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public CoincidentUnits CoincidentUnit
        {
            get
            {
                return (CoincidentUnits)(m_QuantityCode & COIN_UNITS_MASK);
            }
        }

        /// <summary>
        /// Gets the type of a coincident value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public CoincidentTypes CoincidentType
        {
            get
            {
                return (CoincidentTypes)(m_QuantityCode & COIN_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the phase of a coincident value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public CoincidentPhases CoincidentPhase
        {
            get
            {
                return (CoincidentPhases)(m_QuantityCode & COIN_PHASE_MASK);
            }
        }

        /// <summary>
        /// Gets the scale of a coincident value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public CoincidentScales CoincidentScale
        {
            get
            {
                return (CoincidentScales)(m_QuantityCode & COIN_SCALE_MASK);
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is bidirectional
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsBidirectional
        {
            get
            {
                return PeakUnit == PeakUnits.QNet
                    || PeakUnit == PeakUnits.VANet
                    || PeakUnit == PeakUnits.VANetNoType
                    || PeakUnit == PeakUnits.VANetVect
                    || PeakUnit == PeakUnits.VAReceived
                    || PeakUnit == PeakUnits.VAReceivedNoType
                    || PeakUnit == PeakUnits.VAReceivedVect
                    || PeakUnit == PeakUnits.VarNet
                    || PeakUnit == PeakUnits.VarNetVect
                    || PeakUnit == PeakUnits.VarReceived
                    || PeakUnit == PeakUnits.VarReceivedVect
                    || PeakUnit == PeakUnits.WattNet
                    || PeakUnit == PeakUnits.WattReceived
                    || CoincidentUnit == CoincidentUnits.QNet
                    || CoincidentUnit == CoincidentUnits.VANet
                    || CoincidentUnit == CoincidentUnits.VANetNoType
                    || CoincidentUnit == CoincidentUnits.VANetVect
                    || CoincidentUnit == CoincidentUnits.VAReceived
                    || CoincidentUnit == CoincidentUnits.VAReceivedNoType
                    || CoincidentUnit == CoincidentUnits.VAReceivedVect
                    || CoincidentUnit == CoincidentUnits.VarNet
                    || CoincidentUnit == CoincidentUnits.VarNetVect
                    || CoincidentUnit == CoincidentUnits.VarReceived
                    || CoincidentUnit == CoincidentUnits.VarReceivedVect
                    || CoincidentUnit == CoincidentUnits.WattNet
                    || CoincidentUnit == CoincidentUnits.WattReceived;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is leading
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public override bool IsLeading
        {
            get
            {
                return PeakUnit == PeakUnits.VALeading
                    || PeakUnit == PeakUnits.VALeadingNoType
                    || PeakUnit == PeakUnits.VarLeading
                    || CoincidentUnit == CoincidentUnits.VALeading
                    || CoincidentUnit == CoincidentUnits.VALeadingNoType
                    || CoincidentUnit == CoincidentUnits.VarLeading;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is lagging
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public override bool IsLagging
        {
            get
            {
                return PeakUnit == PeakUnits.VALagging
                    || PeakUnit == PeakUnits.VALaggingNoType
                    || PeakUnit == PeakUnits.VarLagging
                    || CoincidentUnit == CoincidentUnits.VALagging
                    || CoincidentUnit == CoincidentUnits.VALaggingNoType
                    || CoincidentUnit == CoincidentUnits.VarLagging;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a power quality quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsPowerQualityQuantity
        {
            get
            {
                return PeakUnit == PeakUnits.CoincidentAmp
                    || PeakUnit == PeakUnits.CoincidentAmpSq
                    || PeakUnit == PeakUnits.CoincidentNeutralAmp
                    || PeakUnit == PeakUnits.CoincidentVolt
                    || PeakUnit == PeakUnits.CoincidentVSq
                    || CoincidentUnit == CoincidentUnits.CoincidentAmp
                    || CoincidentUnit == CoincidentUnits.CoincidentAmpSq
                    || CoincidentUnit == CoincidentUnits.CoincidentNeutralAmp
                    || CoincidentUnit == CoincidentUnits.CoincidentVolt
                    || CoincidentUnit == CoincidentUnits.CoincidentVSq;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Q quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsQQuantity
        {
            get
            {
                return PeakUnit == PeakUnits.CoincidentQ
                    || PeakUnit == PeakUnits.QNet
                    || CoincidentUnit == CoincidentUnits.CoincidentQ
                    || CoincidentUnit == CoincidentUnits.QNet;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a VA quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsVAQuantity
        {
            get
            {
                return PeakUnit == PeakUnits.VADelivered
                    || PeakUnit == PeakUnits.VADeliveredNoType
                    || PeakUnit == PeakUnits.VADeliveredVect
                    || PeakUnit == PeakUnits.VALagging
                    || PeakUnit == PeakUnits.VALaggingNoType
                    || PeakUnit == PeakUnits.VALeading
                    || PeakUnit == PeakUnits.VALeadingNoType
                    || PeakUnit == PeakUnits.VANet
                    || PeakUnit == PeakUnits.VANetNoType
                    || PeakUnit == PeakUnits.VANetVect
                    || PeakUnit == PeakUnits.VAQ1
                    || PeakUnit == PeakUnits.VAQ1NoType
                    || PeakUnit == PeakUnits.VAQ1Vect
                    || PeakUnit == PeakUnits.VAQ2
                    || PeakUnit == PeakUnits.VAQ2NoType
                    || PeakUnit == PeakUnits.VAQ2Vect
                    || PeakUnit == PeakUnits.VAQ3
                    || PeakUnit == PeakUnits.VAQ3NoType
                    || PeakUnit == PeakUnits.VAQ3Vect
                    || PeakUnit == PeakUnits.VAQ4
                    || PeakUnit == PeakUnits.VAQ4NoType
                    || PeakUnit == PeakUnits.VAQ4Vect
                    || PeakUnit == PeakUnits.VAReceived
                    || PeakUnit == PeakUnits.VAReceivedNoType
                    || PeakUnit == PeakUnits.VAReceivedVect
                    || CoincidentUnit == CoincidentUnits.VADelivered
                    || CoincidentUnit == CoincidentUnits.VADeliveredNoType
                    || CoincidentUnit == CoincidentUnits.VADeliveredVect
                    || CoincidentUnit == CoincidentUnits.VALagging
                    || CoincidentUnit == CoincidentUnits.VALaggingNoType
                    || CoincidentUnit == CoincidentUnits.VALeading
                    || CoincidentUnit == CoincidentUnits.VALeadingNoType
                    || CoincidentUnit == CoincidentUnits.VANet
                    || CoincidentUnit == CoincidentUnits.VANetNoType
                    || CoincidentUnit == CoincidentUnits.VANetVect
                    || CoincidentUnit == CoincidentUnits.VAQ1
                    || CoincidentUnit == CoincidentUnits.VAQ1NoType
                    || CoincidentUnit == CoincidentUnits.VAQ1Vect
                    || CoincidentUnit == CoincidentUnits.VAQ2
                    || CoincidentUnit == CoincidentUnits.VAQ2NoType
                    || CoincidentUnit == CoincidentUnits.VAQ2Vect
                    || CoincidentUnit == CoincidentUnits.VAQ3
                    || CoincidentUnit == CoincidentUnits.VAQ3NoType
                    || CoincidentUnit == CoincidentUnits.VAQ3Vect
                    || CoincidentUnit == CoincidentUnits.VAQ4
                    || CoincidentUnit == CoincidentUnits.VAQ4NoType
                    || CoincidentUnit == CoincidentUnits.VAQ4Vect
                    || CoincidentUnit == CoincidentUnits.VAReceived
                    || CoincidentUnit == CoincidentUnits.VAReceivedNoType
                    || CoincidentUnit == CoincidentUnits.VAReceivedVect;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Var quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsVarQuantity
        {
            get
            {
                return PeakUnit == PeakUnits.VarDelivered
                    || PeakUnit == PeakUnits.VarDeliveredVect
                    || PeakUnit == PeakUnits.VarLagging
                    || PeakUnit == PeakUnits.VarLeading
                    || PeakUnit == PeakUnits.VarNet
                    || PeakUnit == PeakUnits.VarNetVect
                    || PeakUnit == PeakUnits.VarQ1
                    || PeakUnit == PeakUnits.VarQ1Vect
                    || PeakUnit == PeakUnits.VarQ2
                    || PeakUnit == PeakUnits.VarQ2Vect
                    || PeakUnit == PeakUnits.VarQ3
                    || PeakUnit == PeakUnits.VarQ3Vect
                    || PeakUnit == PeakUnits.VarQ4
                    || PeakUnit == PeakUnits.VarQ4Vect
                    || PeakUnit == PeakUnits.VarReceived
                    || PeakUnit == PeakUnits.VarReceivedVect
                    || CoincidentUnit == CoincidentUnits.VarDelivered
                    || CoincidentUnit == CoincidentUnits.VarDeliveredVect
                    || CoincidentUnit == CoincidentUnits.VarLagging
                    || CoincidentUnit == CoincidentUnits.VarLeading
                    || CoincidentUnit == CoincidentUnits.VarNet
                    || CoincidentUnit == CoincidentUnits.VarNetVect
                    || CoincidentUnit == CoincidentUnits.VarQ1
                    || CoincidentUnit == CoincidentUnits.VarQ1Vect
                    || CoincidentUnit == CoincidentUnits.VarQ2
                    || CoincidentUnit == CoincidentUnits.VarQ2Vect
                    || CoincidentUnit == CoincidentUnits.VarQ3
                    || CoincidentUnit == CoincidentUnits.VarQ3Vect
                    || CoincidentUnit == CoincidentUnits.VarQ4
                    || CoincidentUnit == CoincidentUnits.VarQ4Vect
                    || CoincidentUnit == CoincidentUnits.VarReceived
                    || CoincidentUnit == CoincidentUnits.VarReceivedVect;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Watt quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public override bool IsWattQuantity
        {
            get
            {
                return PeakUnit == PeakUnits.WattDelivered
                    || PeakUnit == PeakUnits.WattNet
                    || PeakUnit == PeakUnits.WattNoDirection
                    || PeakUnit == PeakUnits.WattNoDirectionLegacy
                    || PeakUnit == PeakUnits.WattReceived
                    || CoincidentUnit == CoincidentUnits.WattDelivered
                    || CoincidentUnit == CoincidentUnits.WattNet
                    || CoincidentUnit == CoincidentUnits.WattNoDirection
                    || CoincidentUnit == CoincidentUnits.WattNoDirectionLegacy
                    || CoincidentUnit == CoincidentUnits.WattReceived;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be used by QuantityCode.Create
        /// </summary>
        /// <param name="code">The actual quantity code.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        internal CoincidentQuantityCode(uint code)
            : base(code)
        {
        }

        #endregion
    }

    /// <summary>
    /// Quantity code class for extrema quantities
    /// </summary>
    public class ExtremaQuantityCode : QuantityCode
    {
        #region Constants

        // Extrema Quantity Masks
        private const uint UNIT_MASK = 0x000001F0;
        private const uint MEAS_TYPE_MASK = 0x00003E00;
        private const uint SUBTYPE_MASK = 0x0000C000;
        private const uint DIRECTION_MASK = 0x000F0000;
        private const uint SCALE_MASK = 0x00300000;
        private const uint EXTREMA_MEAS_TYPE_MASK = 0x00C00000;
        private const uint EXTREMA_NUMBER_MASK = 0x0F000000;
        private const uint PHASE_MASK = 0x70000000;
        private const uint EXTREMA_TYPE_MASK = 0x80000000;

        #endregion

        #region Definitions

        /// <summary>
        /// Extrema Units
        /// </summary>
        public enum ExtremaUnit : uint
        {
#pragma warning disable 1591
            WattHour = 0x00000000,
            VarHour = 0x00000010,
            VAHour = 0x00000020,
            VoltHour = 0x00000030,
            AmpHour = 0x00000040,
            NeutralAmpHour = 0x00000050,
            VSquaredHour = 0x00000060,
            QHour = 0x00000070,
            Watt = 0x00000080,
            Var = 0x00000090,
            VA = 0x000000A0,
            Volt = 0x000000B0,
            Amp = 0x000000C0,
            VSquared = 0x000000D0,
            AmpSquared = 0x000000E0,
            NeutralAmp = 0x000000F0,
            PowerFactor = 0x00000100,
            Q = 0x00000110,
            PowerFactorHour = 0x00000120,
            Hertz = 0x00000130,
            HertzHour = 0x00000140,
            AmpsSquaredHour = 0x00000150,
            Totalized1 = 0x00000160,
            Totalized2 = 0x00000170,
            Totalized3 = 0x00000180,
#pragma warning restore 1591
        }

        /// <summary>
        /// Measurement types
        /// </summary>
        public enum MeasurementType : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            ReverseDemand = 0x00000200,
            ThermalDemand = 0x00000400,
            MaxDemand = 0x00000600,
            ThermalMaxDemand = 0x00000800,
            MinDemand = 0x00000A00,
            MinValue = 0x00000C00,
            AverageValue = 0x00000E00,
            InstDemand = 0x00001000,
            InstValue = 0x00001200,
            InstPeakDemand = 0x00001400,
            PresentDemand = 0x00001600,
            ThermalPresDemand = 0x00001800,
            PreviousDemand = 0x00001A00,
            PreviousValue = 0x00001C00,
            CumDemand = 0x00001E00,
            CCumDemand = 0x00002000,
            ThermalCumDemand = 0x00002200,
            ThermalCCumDemand = 0x00002400,
            ProjectedDemand = 0x00002600,
            InstPeakValue = 0x00002800,
            PresentValue = 0x00002A00,
            InstMinDemand = 0x00002C00,
            ExcessDemand = 0x00002E00,
            ExcessValue = 0x00002E00,
            TotalizedValue = 0x00003000,
            ThermalMinDemand = 0x00003200,
            TOOMinDemand = 0x00003400,
            TOOMaxDemand = 0x00003600,
            TOOMinThermalDemand = 0x00003800,
            TOOMaxThermalDemand = 0x00003A00,
            TOOMinInstDemand = 0x00003C00,
            TOOMaxInstDemand = 0x00003E00,
#pragma warning restore 1591
        }

        /// <summary>
        /// Extrema sub types
        /// </summary>
        public enum ExtremaSubType : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Arithmetic = 0x00004000,
            Vectoral = 0x00008000,
            Fundamental = 0x0000C000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Extrema direction
        /// </summary>
        public enum ExtremaDirection : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Delivered = 0x00010000,
            Received = 0x00020000,
            Leading = 0x00030000,
            Lagging = 0x00040000,
            Q1 = 0x00050000,
            Q2 = 0x00060000,
            Q3 = 0x00070000,
            Q4 = 0x00080000,
            Net = 0x00090000,
            Unbalance = 0x000A0000,
            Distortion = 0x000B0000,
            ForPF = 0x000C0000,
            NetQ1Q3 = 0x000D0000,
            NetQ4Q2 = 0x000E0000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Extrema scale
        /// </summary>
        public enum ExtremaScale : uint
        {
#pragma warning disable 1591
            Units = 0x00000000,
            Kilo = 0x00100000,
            Mega = 0x00200000,
            Giga = 0x00300000,
            OccurrenceDate = 0x00100000,
            OccurrenceTime = 0x00200000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Extrema measurement type
        /// </summary>
        public enum ExtremaMeasurementType : uint
        {
#pragma warning disable 1591
            Date = 0x00000000,
            Time = 0x00400000,
            Extrema = 0x00800000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Extrema number
        /// </summary>
        public enum ExtremaNumber : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Number1 = 0x01000000,
            Number2 = 0x02000000,
            Number3 = 0x03000000,
            Number4 = 0x04000000,
            Number5 = 0x05000000,
            Number6 = 0x06000000,
            Number7 = 0x07000000,
            Number8 = 0x08000000,
            Number9 = 0x09000000,
            Number10 = 0xA0000000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Extrema phase
        /// </summary>
        public enum ExtremaPhase : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Aggregate = 0x00000000,
            PhaseA = 0x10000000,
            PhaseB = 0x20000000,
            PhaseC = 0x30000000,
            Average = 0x40000000,
            AggregateSLC = 0x50000000,
            AggregateTLC = 0x60000000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Extrema type
        /// </summary>
        public enum ExtremaType : uint
        {
#pragma warning disable 1591
            Peak = 0x00000000,
            Minimum = 0x80000000,
#pragma warning restore 1591
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unit
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaUnit Unit
        {
            get
            {
                return (ExtremaUnit)(m_QuantityCode & UNIT_MASK);
            }
        }

        /// <summary>
        /// Gets the measurement
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public MeasurementType Measurement
        {
            get
            {
                return (MeasurementType)(m_QuantityCode & MEAS_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the sub type
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaSubType SubType
        {
            get
            {
                return (ExtremaSubType)(m_QuantityCode & SUBTYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the direction
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaDirection Direction
        {
            get
            {
                return (ExtremaDirection)(m_QuantityCode & DIRECTION_MASK);
            }
        }

        /// <summary>
        /// Gets the scale
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaScale Scale
        {
            get
            {
                return (ExtremaScale)(m_QuantityCode & SCALE_MASK);
            }
        }

        /// <summary>
        /// Gets the Extrema measurement
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaMeasurementType ExtremaMeasurement
        {
            get
            {
                return (ExtremaMeasurementType)(m_QuantityCode & EXTREMA_MEAS_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the number
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaNumber Number
        {
            get
            {
                return (ExtremaNumber)(m_QuantityCode & EXTREMA_NUMBER_MASK);
            }
        }

        /// <summary>
        /// Gets the phase
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaPhase Phase
        {
            get
            {
                return (ExtremaPhase)(m_QuantityCode & PHASE_MASK);
            }
        }

        /// <summary>
        /// Gets the type
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaType Type
        {
            get
            {
                return (ExtremaType)(m_QuantityCode & EXTREMA_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is bidirectional
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsBidirectional
        {
            get
            {
                return Direction == ExtremaDirection.Net
                    || Direction == ExtremaDirection.NetQ1Q3
                    || Direction == ExtremaDirection.NetQ4Q2
                    || Direction == ExtremaDirection.Received;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is leading
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public override bool IsLeading
        {
            get
            {
                return Direction == ExtremaDirection.Leading;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is lagging
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public override bool IsLagging
        {
            get
            {
                return Direction == ExtremaDirection.Lagging;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a power quality quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsPowerQualityQuantity
        {
            get
            {
                return Unit == ExtremaUnit.Amp
                    || Unit == ExtremaUnit.AmpHour
                    || Unit == ExtremaUnit.AmpSquared
                    || Unit == ExtremaUnit.AmpsSquaredHour
                    || Unit == ExtremaUnit.NeutralAmp
                    || Unit == ExtremaUnit.NeutralAmpHour
                    || Unit == ExtremaUnit.Volt
                    || Unit == ExtremaUnit.VoltHour
                    || Unit == ExtremaUnit.VSquared
                    || Unit == ExtremaUnit.VSquaredHour;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Q quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsQQuantity
        {
            get
            {
                return Unit == ExtremaUnit.Q
                    || Unit == ExtremaUnit.QHour;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a VA quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsVAQuantity
        {
            get
            {
                return Unit == ExtremaUnit.VA
                    || Unit == ExtremaUnit.VAHour;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Var quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsVarQuantity
        {
            get
            {
                return Unit == ExtremaUnit.Var
                    || Unit == ExtremaUnit.VarHour;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Watt quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public override bool IsWattQuantity
        {
            get
            {
                return Unit == ExtremaUnit.Watt
                    || Unit == ExtremaUnit.WattHour;
            }
        }


        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be called by QuantityCode.Create
        /// </summary>
        /// <param name="code">The actual quantity code.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        internal ExtremaQuantityCode(uint code)
            : base(code)
        {
        }

        #endregion
    }

    /// <summary>
    /// Quantity Code class for Events
    /// </summary>
    public class EventQuantityCode : QuantityCode
    {
        #region Constants

        private const uint UNIT_MASK = 0x0000FFF0;
        private const int UNIT_SHIFT = 4;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the raw event unit
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public uint Unit
        {
            get
            {
                return (m_QuantityCode & UNIT_MASK) >> UNIT_SHIFT;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be called by QuantityCode.Create
        /// </summary>
        /// <param name="code">The actual quantity code</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        internal EventQuantityCode(uint code)
            : base(code)
        {
        }

        #endregion
    }

    /// <summary>
    /// QuantityCode class for totalized quantities
    /// </summary>
    public class TotalizedQuantityCode : QuantityCode
    {
        #region Constants

        private const uint UNIT_MASK = 0x000001F0;
        private const uint TOTALIZED_TYPE_MASK = 0x00003E00;
        private const uint TOTALIZED_NUMBER_MASK = 0x003F0000;
        private const uint TOU_RATE_MASK = 0x0FC00000;
        private const uint PHASE_MASK = 0xF0000000;

        #endregion

        #region Definitions

        /// <summary>
        /// The units for totalized quantities
        /// </summary>
        public enum TotalizedUnit : uint
        {
#pragma warning disable 1591
            Totalized = 0x00000000,
            TotalizedHour = 0x00000010,
#pragma warning restore 1591
        }

        /// <summary>
        /// The types for totalized quantities
        /// </summary>
        public enum TotalizedType : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            ReverseDemand = 0x00000200,
            ThermalDemand = 0x00000400,
            MaxDemand = 0x00000600,
            ThermalMaxDemand = 0x00000800,
            MinDemand = 0x00000A00,
            MinValue = 0x00000C00,
            AverageValue = 0x00000E00,
            InstantaneousDemand = 0x00001000,
            InstantaneousValue = 0x00001200,
            InstantaneousPeakDemand = 0x00001400,
            PresentDemand = 0x00001600,
            ThermalPresentDemand = 0x00001800,
            PreviousDemand = 0x00001A00,
            PreviousValue = 0x00001C00,
            CumDemand = 0x00001E00,
            CCumDemand = 0x00002000,
            ThermalCumDemand = 0x00002200,
            ThermalCCumDemand = 0x00002400,
            ProjectedDemand = 0x00002600,
            InstMinDemand = 0x00002C00,
            ExcessEnergy = 0x00002E00,
            TOOMaxDemandDate = 0x00003000,
            TOOMaxDemandTime = 0x00003200,
#pragma warning restore 1591
        }

        /// <summary>
        /// The totalized quantity number
        /// </summary>
        public enum TotalizedNumber : uint
        {
#pragma warning disable 1591
            Number1 = 0x00000000,
            Number2 = 0x00010000,
            Number3 = 0x00020000,
            Number4 = 0x00030000,
            Number5 = 0x00040000,
            Number6 = 0x00050000,
            Number7 = 0x00060000,
            Number8 = 0x00070000,
            Number9 = 0x00080000,
            Number10 = 0x00090000,
            Number11 = 0x000A0000,
            Number12 = 0x000B0000,
            Number13 = 0x000C0000,
            Number14 = 0x000D0000,
            Number15 = 0x000E0000,
            Number16 = 0x000F0000,
            Number17 = 0x00100000,
            Number18 = 0x00110000,
            Number19 = 0x00120000,
            Number20 = 0x00130000,
            Number21 = 0x00140000,
            Number22 = 0x00150000,
            Number23 = 0x00160000,
            Number24 = 0x00170000,
            Number25 = 0x00180000,
            Number26 = 0x00190000,
            Number27 = 0x001A0000,
            Number28 = 0x001B0000,
            Number29 = 0x001C0000,
            Number30 = 0x001D0000,
            Number31 = 0x001E0000,
            Number32 = 0x001F0000,
#pragma warning restore 1591
        }

        /// <summary>
        /// The TOU rate for the totalized quantities
        /// </summary>
        public enum TOURate : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Rate1 = 0x00400000,
            Rate2 = 0x00800000,
            Rate3 = 0x00C00000,
            Rate4 = 0x01000000,
            Rate5 = 0x01400000,
            Rate6 = 0x01800000,
            Rate7 = 0x01C00000,
            Rate8 = 0x02000000,
            Rate9 = 0x02400000,
            Rate10 = 0x02800000,
            Rate11 = 0x02C00000,
            Rate12 = 0x03000000,
            Rate13 = 0x03400000,
            Rate14 = 0x03800000,
            Rate15 = 0x03C00000,
            Rate16 = 0x04000000,
            Rate17 = 0x04400000,
            Rate18 = 0x04800000,
            Rate19 = 0x04C00000,
            Rate20 = 0x05000000,
            Rate21 = 0x05400000,
            Rate22 = 0x05800000,
            Rate23 = 0x05C00000,
            Rate24 = 0x06000000,
            Rate25 = 0x06400000,
            Rate26 = 0x06800000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Phases for totalized quantities
        /// </summary>
        public enum TotalizedPhases : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Aggregate = 0x00000000,
            PhaseA = 0x10000000,
            PhaseB = 0x20000000,
            PhaseC = 0x30000000,
            Average = 0x40000000,
            AggregateSLC = 0x50000000,
            AggregateTLC = 0x60000000,
#pragma warning restore 1591
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unit
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public TotalizedUnit Unit
        {
            get
            {
                return (TotalizedUnit)(m_QuantityCode & UNIT_MASK);
            }
        }

        /// <summary>
        /// Gets the type
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public TotalizedType Type
        {
            get
            {
                return (TotalizedType)(m_QuantityCode & TOTALIZED_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the totalization quantity number
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public TotalizedNumber Number
        {
            get
            {
                return (TotalizedNumber)(m_QuantityCode & TOTALIZED_NUMBER_MASK);
            }
        }

        /// <summary>
        /// Gets the TOU rate
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public TOURate Rate
        {
            get
            {
                return (TOURate)(m_QuantityCode & TOU_RATE_MASK);
            }
        }

        /// <summary>
        /// Gets the phase
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public TotalizedPhases Phase
        {
            get
            {
                return (TotalizedPhases)(m_QuantityCode & PHASE_MASK);
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a totalized quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsTotalizedQuantity
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be called by QuantityCode.Create
        /// </summary>
        /// <param name="code">The actual quantity code</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        internal TotalizedQuantityCode(uint code)
            : base(code)
        {
        }

        #endregion
    }

    /// <summary>
    /// Quantity code class for VQ quantities
    /// </summary>
    public class VQQuantityCode : QuantityCode
    {
        #region Constants

        private const uint UNIT_MASK = 0x000001F0;
        private const uint VQ_NUMBER_MASK = 0x00003E00;
        private const uint OCCURRENCE_TYPE_MASK = 0x0000C000;
        private const uint EVENT_MASK = 0x0FF00000;
        private const uint PHASE_MASK = 0xF0000000;

        #endregion

        #region Definitions

        /// <summary>
        /// VQ Units
        /// </summary>
        public enum VQUnit : uint
        {
#pragma warning disable 1591
            Interruption = 0x00000000,
            Sag = 0x00000010,
            Swell = 0x00000020,
            Imbalance = 0x00000030,
            VoltageIsolation = 0x00000040,
            CurrentUnbalance = 0x00000050,
            CurrentReversal = 0x00000060,
#pragma warning restore 1591
        }

        /// <summary>
        /// VQ number
        /// </summary>
        public enum VQNumber : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Number1 = 0x00000200,
            Number2 = 0x00000400,
            Number3 = 0x00000600,
            Number4 = 0x00000800,
            Number5 = 0x00000A00,
            Number6 = 0x00000C00,
            Number7 = 0x00000E00,
            Number8 = 0x00001000,
            Number9 = 0x00001200,
            Number10 = 0x00001400,
            Number11 = 0x00001600,
            Number12 = 0x00001800,
            Number13 = 0x00001A00,
            Number14 = 0x00001C00,
            Number15 = 0x00001E00,
#pragma warning restore 1591
        }

        /// <summary>
        /// VQ occurence type
        /// </summary>
        public enum OccurrenceType : uint
        {
#pragma warning disable 1591
            NoOccurrence = 0x00000000,
            Last = 0x00004000,
            Pending = 0x00008000,
#pragma warning restore 1591
        }

        /// <summary>
        /// VQ events
        /// </summary>
        public enum VQEvent : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Count = 0x00100000,
            StartDate = 0x00200000,
            StartTime = 0x00300000,
            EndDate = 0x00400000,
            EndTime = 0x00500000,
            Duration = 0x00600000,
            Reason = 0x00700000,
            VoltageExtreme = 0x00800000,
            AverageCurrent = 0x00900000,
            AveragePercentage = 0x00A00000,
#pragma warning restore 1591
        }

        /// <summary>
        /// VQ phases
        /// </summary>
        public enum VQPhase : uint
        {
#pragma warning disable 1591
            NoPhase = 0x00000000,
            PhaseA = 0x10000000,
            PhaseB = 0x20000000,
            PhaseC = 0x30000000,
#pragma warning restore 1591
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the VQ units
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public VQUnit Unit
        {
            get
            {
                return (VQUnit)(m_QuantityCode & UNIT_MASK);
            }
        }

        /// <summary>
        /// Gets the VQ number
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public VQNumber Number
        {
            get
            {
                return (VQNumber)(m_QuantityCode & VQ_NUMBER_MASK);
            }
        }

        /// <summary>
        /// Gets the VQ type
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public OccurrenceType Type
        {
            get
            {
                return (OccurrenceType)(m_QuantityCode & OCCURRENCE_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the VQ event
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public VQEvent Event
        {
            get
            {
                return (VQEvent)(m_QuantityCode & EVENT_MASK);
            }
        }

        /// <summary>
        /// Gets the VQ phase
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public VQPhase Phase
        {
            get
            {
                return (VQPhase)(m_QuantityCode & PHASE_MASK);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be called by QuantityCode.Create
        /// </summary>
        /// <param name="code">The actual quantity code.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        internal VQQuantityCode(uint code)
            : base(code)
        {
        }

        #endregion
    }

    /// <summary>
    /// Quantity code class for harmonic quantities
    /// </summary>
    public class HarmonicQuantityCode : QuantityCode
    {
        #region Constants

        private const uint UNIT_MASK = 0x000001F0;
        private const uint HARMONICS_TYPE_MASK = 0x00003E00;
        private const uint CALCULATION_MASK = 0x0000C000;
        private const uint HARMONIC_NUMBER_MASK = 0x003F0000;
        private const uint TOU_RATE_MASK = 0x0FC00000;
        private const uint PHASE_MASK = 0xF0000000;

        #endregion

        #region Definitions

        /// <summary>
        /// Harmonic units
        /// </summary>
        public enum HarmonicUnit : uint
        {
#pragma warning disable 1591
            VoltAmplitude = 0x00000000,
            VoltPhase = 0x00000010,
            CurrentAmplitude = 0x00000020,
            CurrentPhase = 0x00000030,
            THDVolt = 0x00000040,
            THDAmp = 0x00000050,
            DispPowerFactor = 0x00000060,
            FundPower = 0x00000070,
            THDVoltHour = 0x00000080,
            THDAmpHour = 0x00000090,
            VoltAmplitudePFund = 0x000000A0,
            CurrentAmplitudePFund = 0x000000B0,
            TDDAmp = 0x000000C0,
#pragma warning restore 1591
        }

        /// <summary>
        /// harmonic type
        /// </summary>
        public enum HarmonicType : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            ReverseDemand = 0x00000200,
            ThermalDemand = 0x00000400,
            MaxDemand = 0x00000600,
            ThermalMaxDemand = 0x00000800,
            MinDemand = 0x00000A00,
            MinValue = 0x00000C00,
            AvgValue = 0x00000E00,
            InstDemand = 0x00001000,
            InstValue = 0x00001200,
            IndtPeakDemand = 0x00001400,
            PresDemand = 0x00001600,
            ThermalPresDemand = 0x00001800,
            PrevDemand = 0x00001A00,
            PrevValue = 0x00001C00,
            CumDemand = 0x00001E00,
            CCumDemand = 0x00002000,
            ThermalCumDemand = 0x00002200,
            ThermalCCumDemand = 0x00002400,
            ProjDemand = 0x00002600,
            InstMinDemand = 0x00002800,
#pragma warning restore 1591
        }

        /// <summary>
        /// Harmonic calculation
        /// </summary>
        public enum HarmonicCalculation : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            UnitedStates = 0x00004000,
            Europe = 0x00008000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Harmonic numbers
        /// </summary>
        public enum HarmonicNumber : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Fundamental = 0x00010000,
            Harmonic2 = 0x00020000,
            Harmonic3 = 0x00030000,
            Harmonic4 = 0x00040000,
            Harmonic5 = 0x00050000,
            Harmonic6 = 0x00060000,
            Harmonic7 = 0x00070000,
            Harmonic8 = 0x00080000,
            Harmonic9 = 0x00090000,
            Harmonic10 = 0x000A0000,
            Harmonic11 = 0x000B0000,
            Harmonic12 = 0x000C0000,
            Harmonic13 = 0x000D0000,
            Harmonic14 = 0x000E0000,
            Harmonic15 = 0x000F0000,
            Harmonic16 = 0x00100000,
            Harmonic17 = 0x00110000,
            Harmonic18 = 0x00120000,
            Harmonic19 = 0x00130000,
            Harmonic20 = 0x00140000,
            Harmonic21 = 0x00150000,
            Harmonic22 = 0x00160000,
            Harmonic23 = 0x00170000,
            Harmonic24 = 0x00180000,
            Harmonic25 = 0x00190000,
            Harmonic26 = 0x001A0000,
            Harmonic27 = 0x001B0000,
            Harmonic28 = 0x001C0000,
            Harmonic29 = 0x001D0000,
            Harmonic30 = 0x001E0000,
            Harmonic31 = 0x001F0000,
            Harmonic32 = 0x00200000,
#pragma warning restore 1591
        }

        /// <summary>
        /// TOU rates
        /// </summary>
        public enum TOURate : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Rate1 = 0x00400000,
            Rate2 = 0x00800000,
            Rate3 = 0x00C00000,
            Rate4 = 0x01000000,
            Rate5 = 0x01400000,
            Rate6 = 0x01800000,
            Rate7 = 0x01C00000,
            Rate8 = 0x02000000,
            Rate9 = 0x02400000,
            Rate10 = 0x02800000,
            Rate11 = 0x02C00000,
            Rate12 = 0x03000000,
            Rate13 = 0x03400000,
            Rate14 = 0x03800000,
            Rate15 = 0x03C00000,
            Rate16 = 0x04000000,
            Rate17 = 0x04400000,
            Rate18 = 0x04800000,
            Rate19 = 0x04C00000,
            Rate20 = 0x05000000,
            Rate21 = 0x05400000,
            Rate22 = 0x05800000,
            Rate23 = 0x05C00000,
            Rate24 = 0x06000000,
            Rate25 = 0x06400000,
            Rate26 = 0x06800000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Harmonic phase
        /// </summary>
        public enum HarmonicPhase : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Aggregate = 0x00000000,
            PhaseA = 0x10000000,
            PhaseB = 0x20000000,
            PhaseC = 0x30000000,
            Average = 0x40000000,
            AggregateSLC = 0x50000000,
            AggregateTLC = 0x60000000,
#pragma warning restore 1591
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the units
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public HarmonicUnit Unit
        {
            get
            {
                return (HarmonicUnit)(m_QuantityCode & UNIT_MASK);
            }
        }

        /// <summary>
        /// Gets the harmonic type
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public HarmonicType Type
        {
            get
            {
                return (HarmonicType)(m_QuantityCode & HARMONICS_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the harmonic calculation
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public HarmonicCalculation Calculation
        {
            get
            {
                return (HarmonicCalculation)(m_QuantityCode & CALCULATION_MASK);
            }
        }

        /// <summary>
        /// Gets the harmonic number
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public HarmonicNumber Number
        {
            get
            {
                return (HarmonicNumber)(m_QuantityCode & HARMONIC_NUMBER_MASK);
            }
        }

        /// <summary>
        /// Gets the TOU rate
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public TOURate Rate
        {
            get
            {
                return (TOURate)(m_QuantityCode & TOU_RATE_MASK);
            }
        }

        /// <summary>
        /// Gets the phase
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public HarmonicPhase Phase
        {
            get
            {
                return (HarmonicPhase)(m_QuantityCode & PHASE_MASK);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be called by QuantityCode.Create
        /// </summary>
        /// <param name="code">The actual quantity code</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        internal HarmonicQuantityCode(uint code)
            : base(code)
        {
        }

        #endregion
    }

    /// <summary>
    /// Quantity code class for IO quantities
    /// </summary>
    public class IOQuantityCode : QuantityCode
    {
        #region Constants

        private const uint UNIT_MASK = 0x000001F0;
        private const uint IO_TYPE_MASK = 0x00003E00;
        private const uint IO_NUMBER_MASK = 0x003F0000;
        private const uint TOU_RATE_MASK = 0x0FC00000;
        private const uint PHASE_MASK = 0xF0000000;

        #endregion

        #region Definitions

        /// <summary>
        /// IO units
        /// </summary>
        public enum IOUnit : uint
        {
#pragma warning disable 1591
            AnalogInput = 0x00000000,
            DigitalInput = 0x00000010,
            DigitalStateInput = 0x00000020,
            AnalogInputHour = 0x00000030,
            DigitalInputHour = 0x00000040,
            DigitalStateInputHour = 0x00000050,
#pragma warning restore 1591
        }

        /// <summary>
        /// IO type
        /// </summary>
        public enum IOType : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            ReverseDemand = 0x00000200,
            ThermalDemand = 0x00000400,
            MaxDemand = 0x00000600,
            ThermalMaxDemand = 0x00000800,
            MinDemand = 0x00000A00,
            MinValue = 0x00000C00,
            AvgValue = 0x00000E00,
            InstDemand = 0x00001000,
            InstValue = 0x00001200,
            IndtPeakDemand = 0x00001400,
            PresDemand = 0x00001600,
            ThermalPresDemand = 0x00001800,
            PrevDemand = 0x00001A00,
            PrevValue = 0x00001C00,
            CumDemand = 0x00001E00,
            CCumDemand = 0x00002000,
            ThermalCumDemand = 0x00002200,
            ThermalCCumDemand = 0x00002400,
            ProjDemand = 0x00002600,
            InstMinDemand = 0x00002800,
#pragma warning restore 1591
        }

        /// <summary>
        /// IO number
        /// </summary>
        public enum IONumber : uint
        {
#pragma warning disable 1591
            Number1 = 0x00000000,
            Number2 = 0x00010000,
            Number3 = 0x00020000,
            Number4 = 0x00030000,
            Number5 = 0x00040000,
            Number6 = 0x00050000,
            Number7 = 0x00060000,
            Number8 = 0x00070000,
            Number9 = 0x00080000,
            Number10 = 0x00090000,
            Number11 = 0x000A0000,
            Number12 = 0x000B0000,
            Number13 = 0x000C0000,
            Number14 = 0x000D0000,
            Number15 = 0x000E0000,
            Number16 = 0x000F0000,
            Number17 = 0x00100000,
            Number18 = 0x00110000,
            Number19 = 0x00120000,
            Number20 = 0x00130000,
            Number21 = 0x00140000,
            Number22 = 0x00150000,
            Number23 = 0x00160000,
            Number24 = 0x00170000,
            Number25 = 0x00180000,
            Number26 = 0x00190000,
            Number27 = 0x001A0000,
            Number28 = 0x001B0000,
            Number29 = 0x001C0000,
            Number30 = 0x001D0000,
            Number31 = 0x001E0000,
            Number32 = 0x001F0000,
#pragma warning restore 1591
        }

        /// <summary>
        /// TOU rates
        /// </summary>
        public enum TOURate : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Rate1 = 0x00400000,
            Rate2 = 0x00800000,
            Rate3 = 0x00C00000,
            Rate4 = 0x01000000,
            Rate5 = 0x01400000,
            Rate6 = 0x01800000,
            Rate7 = 0x01C00000,
            Rate8 = 0x02000000,
            Rate9 = 0x02400000,
            Rate10 = 0x02800000,
            Rate11 = 0x02C00000,
            Rate12 = 0x03000000,
            Rate13 = 0x03400000,
            Rate14 = 0x03800000,
            Rate15 = 0x03C00000,
            Rate16 = 0x04000000,
            Rate17 = 0x04400000,
            Rate18 = 0x04800000,
            Rate19 = 0x04C00000,
            Rate20 = 0x05000000,
            Rate21 = 0x05400000,
            Rate22 = 0x05800000,
            Rate23 = 0x05C00000,
            Rate24 = 0x06000000,
            Rate25 = 0x06400000,
            Rate26 = 0x06800000,
#pragma warning restore 1591
        }

        /// <summary>
        /// IO phases
        /// </summary>
        public enum IOPhase : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Aggregate = 0x00000000,
            PhaseA = 0x10000000,
            PhaseB = 0x20000000,
            PhaseC = 0x30000000,
            Average = 0x40000000,
            AggregateSLC = 0x50000000,
            AggregateTLC = 0x60000000,
#pragma warning restore 1591
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unit
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public IOUnit Unit
        {
            get
            {
                return (IOUnit)(m_QuantityCode & UNIT_MASK);
            }
        }

        /// <summary>
        /// Gets the type
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public IOType Type
        {
            get
            {
                return (IOType)(m_QuantityCode & IO_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the IO number
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public IONumber Number
        {
            get
            {
                return (IONumber)(m_QuantityCode & IO_NUMBER_MASK);
            }
        }

        /// <summary>
        /// Gets the TOU rate
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public TOURate Rate
        {
            get
            {
                return (TOURate)(m_QuantityCode & TOU_RATE_MASK);
            }
        }

        /// <summary>
        /// Gets the phase
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public IOPhase Phase
        {
            get
            {
                return (IOPhase)(m_QuantityCode & PHASE_MASK);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be called by QuantityCode.Create
        /// </summary>
        /// <param name="code">The actual quantity code.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        internal IOQuantityCode(uint code)
            : base(code)
        {
        }

        #endregion
    }

    /// <summary>
    /// Quantity code class for extrema rate quantities
    /// </summary>
    public class ExtremaRatesQuantityCode : QuantityCode
    {
        #region Constants

        private const uint UNIT_MASK = 0x000001F0;
        private const uint RATE_MASK = 0x00003E00;
        private const uint SUB_TYPE_MASK = 0x0000C000;
        private const uint DIRECTION_MASK = 0x000F0000;
        private const uint SCALE_MASK = 0x00300000;
        private const uint EXTREMA_MEAS_TYPE_MASK = 0x00C00000;
        private const uint EXTREMA_NUMBER_MASK = 0x0F000000;
        private const uint PHASE_MASK = 0x70000000;
        private const uint EXTREMA_TYPE_MASK = 0x80000000;

        #endregion

        #region Definitions

        /// <summary>
        /// Extrema units
        /// </summary>
        public enum ExtremaUnit : uint
        {
#pragma warning disable 1591
            WattHour = 0x00000000,
            VarHour = 0x00000010,
            VAHour = 0x00000020,
            VoltHour = 0x00000030,
            AmpHour = 0x00000040,
            NeutralAmpHour = 0x00000050,
            VSquaredHour = 0x00000060,
            QHour = 0x00000070,
            Watt = 0x00000080,
            Var = 0x00000090,
            VA = 0x000000A0,
            Volt = 0x000000B0,
            Amp = 0x000000C0,
            VSquared = 0x000000D0,
            AmpSquared = 0x000000E0,
            NeutralAmp = 0x000000F0,
            PowerFactor = 0x00000100,
            Q = 0x00000110,
            PowerFactorHour = 0x00000120,
            Hertz = 0x00000130,
            HertzHour = 0x00000140,
            AmpsSquaredHour = 0x00000150,
            Totalized1 = 0x00000160,
            Totalized2 = 0x00000170,
            Totalized3 = 0x00000180,
#pragma warning restore 1591
        }

        /// <summary>
        /// TOU rates
        /// </summary>
        public enum TOURate : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            RateA = 0x00000200,
            RateB = 0x00000400,
            RateC = 0x00000600,
            RateD = 0x00000800,
            RateE = 0x00000A00,
            RateF = 0x00000C00,
            RateG = 0x00000E00,
            RateH = 0x00001000,
            RateI = 0x00001200,
            RateJ = 0x00001400,
            RateK = 0x00001600,
            RateL = 0x00001800,
            RateM = 0x00001A00,
            RateN = 0x00001C00,
            RateO = 0x00001E00,
            RateP = 0x00002000,
            RateQ = 0x00002200,
            RateR = 0x00002400,
            RateS = 0x00002600,
            RateT = 0x00002800,
            RateU = 0x00002A00,
            RateV = 0x00002C00,
            RateW = 0x00002E00,
            RateX = 0x00003000,
            RateY = 0x00003200,
            RateZ = 0x00003400,
#pragma warning restore 1591
        }

        /// <summary>
        /// Extrema sub types
        /// </summary>
        public enum ExtremaSubType : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Arithmetic = 0x00004000,
            Vectoral = 0x00008000,
            Fundamental = 0x0000C000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Extrema direction
        /// </summary>
        public enum ExtremaDirection : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Delivered = 0x00010000,
            Received = 0x00020000,
            Leading = 0x00030000,
            Lagging = 0x00040000,
            Q1 = 0x00050000,
            Q2 = 0x00060000,
            Q3 = 0x00070000,
            Q4 = 0x00080000,
            Net = 0x00090000,
            Unbalance = 0x000A0000,
            Distortion = 0x000B0000,
            ForPF = 0x000C0000,
            NetQ1Q3 = 0x000D0000,
            NetQ4Q2 = 0x000E0000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Extrema scales
        /// </summary>
        public enum ExtremaScale : uint
        {
#pragma warning disable 1591
            Units = 0x00000000,
            Kilo = 0x00100000,
            Mega = 0x00200000,
            Giga = 0x00300000,
            OccurrenceDate = 0x00100000,
            OccurrenceTime = 0x00200000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Extrema measurement types
        /// </summary>
        public enum ExtremaMeasurementType : uint
        {
#pragma warning disable 1591
            Date = 0x00000000,
            Time = 0x00400000,
            Extrema = 0x00800000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Extrema numbers
        /// </summary>
        public enum ExtremaNumber : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Number1 = 0x01000000,
            Number2 = 0x02000000,
            Number3 = 0x03000000,
            Number4 = 0x04000000,
            Number5 = 0x05000000,
            Number6 = 0x06000000,
            Number7 = 0x07000000,
            Number8 = 0x08000000,
            Number9 = 0x09000000,
            Number10 = 0xA0000000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Extrema phases
        /// </summary>
        public enum ExtremaPhase : uint
        {
#pragma warning disable 1591
            None = 0x00000000,
            Aggregate = 0x00000000,
            PhaseA = 0x10000000,
            PhaseB = 0x20000000,
            PhaseC = 0x30000000,
            Average = 0x40000000,
            AggregateSLC = 0x50000000,
            AggregateTLC = 0x60000000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Extrema types
        /// </summary>
        public enum ExtremaType : uint
        {
#pragma warning disable 1591
            Peak = 0x00000000,
            Minimum = 0x80000000,
#pragma warning restore 1591
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the units
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaUnit Unit
        {
            get
            {
                return (ExtremaUnit)(m_QuantityCode & UNIT_MASK);
            }
        }

        /// <summary>
        /// Gets the TOU rates
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public TOURate Rate
        {
            get
            {
                return (TOURate)(m_QuantityCode & RATE_MASK);
            }

        }

        /// <summary>
        /// Gets the sub types
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaSubType SubType
        {
            get
            {
                return (ExtremaSubType)(m_QuantityCode & SUB_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the direction
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaDirection Direction
        {
            get
            {
                return (ExtremaDirection)(m_QuantityCode & DIRECTION_MASK);
            }
        }

        /// <summary>
        /// Gets the scale
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaScale Scale
        {
            get
            {
                return (ExtremaScale)(m_QuantityCode & SCALE_MASK);
            }
        }

        /// <summary>
        /// Gets the extrema measurement type
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaMeasurementType ExtremaMeasurement
        {
            get
            {
                return (ExtremaMeasurementType)(m_QuantityCode & EXTREMA_MEAS_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the extrema number
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaNumber Number
        {
            get
            {
                return (ExtremaNumber)(m_QuantityCode & EXTREMA_NUMBER_MASK);
            }
        }

        /// <summary>
        /// Gets the phase
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaPhase Phase
        {
            get
            {
                return (ExtremaPhase)(m_QuantityCode & PHASE_MASK);
            }
        }

        /// <summary>
        /// Gets the type
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public ExtremaType Type
        {
            get
            {
                return (ExtremaType)(m_QuantityCode & EXTREMA_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is bidirectional
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsBidirectional
        {
            get
            {
                return Direction == ExtremaDirection.Net
                    || Direction == ExtremaDirection.NetQ1Q3
                    || Direction == ExtremaDirection.NetQ4Q2
                    || Direction == ExtremaDirection.Received;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is leading
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public override bool IsLeading
        {
            get
            {
                return Direction == ExtremaDirection.Leading;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is leading
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public override bool IsLagging
        {
            get
            {
                return Direction == ExtremaDirection.Lagging;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a power quality quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsPowerQualityQuantity
        {
            get
            {
                return Unit == ExtremaUnit.Amp
                    || Unit == ExtremaUnit.AmpHour
                    || Unit == ExtremaUnit.AmpSquared
                    || Unit == ExtremaUnit.AmpsSquaredHour
                    || Unit == ExtremaUnit.NeutralAmp
                    || Unit == ExtremaUnit.NeutralAmpHour
                    || Unit == ExtremaUnit.Volt
                    || Unit == ExtremaUnit.VoltHour
                    || Unit == ExtremaUnit.VSquared
                    || Unit == ExtremaUnit.VSquaredHour;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Q quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsQQuantity
        {
            get
            {
                return Unit == ExtremaUnit.Q
                    || Unit == ExtremaUnit.QHour;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a VA quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsVAQuantity
        {
            get
            {
                return Unit == ExtremaUnit.VA
                    || Unit == ExtremaUnit.VAHour;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Var quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsVarQuantity
        {
            get
            {
                return Unit == ExtremaUnit.Var
                    || Unit == ExtremaUnit.VarHour;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Watt quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public override bool IsWattQuantity
        {
            get
            {
                return Unit == ExtremaUnit.Watt
                    || Unit == ExtremaUnit.WattHour;
            }
        }


        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be called by QuantityCode.Create
        /// </summary>
        /// <param name="code"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        internal ExtremaRatesQuantityCode(uint code)
            : base(code)
        {
        }

        #endregion
    }

    /// <summary>
    /// Quantity code class for extended coincident quantities
    /// </summary>
    public class ExtendedCoincidentQuantityCode : QuantityCode
    {
        #region Constants

        private const uint PEAK_UNITS_MASK =      0x000003F0;
        private const uint PEAK_TYPE_MASK =       0x00000400;
        private const uint COIN_PHASE_RATE_MASK = 0x0000F800;
        private const uint PEAK_SCALE_MASK =      0x00030000;
        private const uint COIN_UNITS_MASK =      0x00FC0000;
        private const uint COIN_TYPE_MASK =       0x1F000000;
        private const uint COIN_SCALE_MASK =      0x60000000;
        private const uint PEAK_PHASE_RATE_MASK = 0x80000000;

        #endregion

        #region Definitions

        /// <summary>
        /// Peak units
        /// </summary>
        public enum PeakUnits : uint
        {
#pragma warning disable 1591
            Undefined = 0x00000000,
            WattDelivered = 0x00000010,
            WattReceived = 0x00000020,
            WattNet = 0x00000030,
            VarDelivered = 0x00000004,
            VarReceived = 0x00000050,
            VarNet = 0x00000060,
            VarQ1 = 0x00000070,
            VarQ2 = 0x00000080,
            VarQ3 = 0x00000090,
            VarQ4 = 0x000000A0,
            VarDeliveredVect = 0x000000B0,
            VarReceivedVect = 0x000000C0,
            VarNetVect = 0x000000D0,
            VarQ1Vect = 0x000000E0,
            VarQ2Vect = 0x000000F0,
            VarQ3Vect = 0x00000100,
            VarQ4Vect = 0x00000110,
            VADelivered = 0x00000120,
            VAReceived = 0x00000130,
            VANet = 0x00000140,
            VAQ1 = 0x00000150,
            VAQ2 = 0x00000160,
            VAQ3 = 0x00000170,
            VAQ4 = 0x00000180,
            VADeliveredVect = 0x00000190,
            VAReceivedVect = 0x000001A0,
            VANetVect = 0x000001B0,
            VAQ1Vect = 0x000001C0,
            VAQ2Vect = 0x000001D0,
            VAQ3Vect = 0x000001E0,
            VAQ4Vect = 0x000001F0,
            CoincidentVolt = 0x00000200,
            CoincidentAmp = 0x00000210,
            CoincidentVSq = 0x00000220,
            CoincidentAmpSq = 0x00000230,
            CoincidentNeutralAmp = 0x00000240,
            PowerFactorVectorial = 0x00000250,
            PowerFactorArithmetic = 0x00000260,
            CoincidentQ = 0x00000270,
            DigitalInput = 0x00000280,
            AnalogInput = 0x00000290,
            TotalizedQty = 0x000002A0,
            PercentTHDVolts = 0x000002B0,
            PercentTHDAmps = 0x000002C0,
            CoincidentPF = 0x000002D0,
            VALeading = 0x000002E0,
            VALagging = 0x000002F0,
            VarLeading = 0x00000300,
            VarLagging = 0x00000310,
            VALeadingNoType = 0x00000320,
            VALaggingNoType = 0x00000330,
            VADeliveredNoType = 0x00000340,
            VAReceivedNoType = 0x00000350,
            VANetNoType = 0x00000360,
            VAQ1NoType = 0x00000370,
            VAQ2NoType = 0x00000380,
            VAQ3NoType = 0x00000390,
            VAQ4NoType = 0x000003A0,
            WattNoDirection = 0x000003B0,
            QNet = 0x000003C0,
            WattNoDirectionLegacy = 0x000003D0,
            Totalized2 = 0x000003E0,
            Totalized3 = 0x000003F0,
#pragma warning restore 1591
        }

        /// <summary>
        /// Peak types
        /// </summary>
        public enum PeakTypes : uint
        {
#pragma warning disable 1591
            Minimum = 0x00000000,
            Maximum = 0x00000400,
#pragma warning restore 1591
        }

        /// <summary>
        /// Coincident phase rates
        /// </summary>
        public enum CoincidentPhaseRates : uint
        {
#pragma warning disable 1591
            NoRate = 0x00000000,
            AggregatePhase = 0x00000000,
            RateA = 0x00000800,
            RateB = 0x00001000,
            RateC = 0x00001800,
            RateD = 0x00002000,
            RateE = 0x00002800,
            RateF = 0x00003000,
            RateG = 0x00003800,
            RateH = 0x00004000,
            RateI = 0x00004800,
            RateJ = 0x00005000,
            RateK = 0x00005800,
            RateL = 0x00006000,
            RateM = 0x00006800,
            RateN = 0x00007000,
            RateO = 0x00007800,
            RateP = 0x00008000,
            RateQ = 0x00008800,
            RateR = 0x00009000,
            RateS = 0x00009800,
            RateT = 0x0000A000,
            RateU = 0x0000A800,
            RateV = 0x0000B000,
            RateW = 0x0000B800,
            RateX = 0x0000C000,
            RateY = 0x0000C800,
            CoincidentPhaseSLC = 0x0000D000,
            CoincidentPhaseA = 0x0000D800,
            CoincidentPhaseB = 0x0000E000,
            CoincidentPhaseC = 0x0000E800,
            CoincidentPhaseAverage = 0x0000F000,
            CoicidentPhaseTLC = 0x0000F800,
#pragma warning restore 1591
        }

        /// <summary>
        /// Peak scales
        /// </summary>
        public enum PeakScales : uint
        {
#pragma warning disable 1591
            Units = 0x00000000,
            Kilo = 0x00010000,
            Mega = 0x00020000,
            Giga = 0x00030000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Coincident units
        /// </summary>
        public enum CoincidentUnits : uint
        {
#pragma warning disable 1591
            Undefined = 0x00000000,
            WattDelivered = 0x00040000,
            WattReceived =  0x00080000,
            WattNet = 0x000C0000,
            VarDelivered = 0x00100000,
            VarReceived = 0x00140000,
            VarNet = 0x00180000,
            VarQ1 =  0x001C0000,
            VarQ2 =  0x00200000,
            VarQ3 =  0x00240000,
            VarQ4 =  0x00280000,
            VarDeliveredVect = 0x002C0000,
            VarReceivedVect = 0x00300000,
            VarNetVect = 0x00340000,
            VarQ1Vect = 0x00380000,
            VarQ2Vect = 0x003C0000,
            VarQ3Vect = 0x00400000,
            VarQ4Vect = 0x00440000,
            VADelivered = 0x00480000,
            VAReceived = 0x004C0000,
            VANet = 0x00500000,
            VAQ1 = 0x00540000,
            VAQ2 = 0x00580000,
            VAQ3 = 0x005C0000,
            VAQ4 = 0x00600000,
            VADeliveredVect = 0x00640000,
            VAReceivedVect = 0x00680000,
            VANetVect = 0x006C0000,
            VAQ1Vect = 0x00700000,
            VAQ2Vect = 0x00740000,
            VAQ3Vect = 0x00780000,
            VAQ4Vect = 0x007C0000,
            CoincidentVolt = 0x00800000,
            CoincidentAmp = 0x00840000,
            CoincidentVSq = 0x00880000,
            CoincidentAmpSq = 0x008C0000,
            CoincidentNeutralAmp = 0x00900000,
            PowerFactorVectorial = 0x00940000,
            PowerFactorArithmetic = 0x00980000,
            CoincidentQ = 0x009C0000,
            DigitalInput = 0x00A00000,
            AnalogInput = 0x00A40000,
            TotalizedQty = 0x00A80000,
            PercentTHDVolts = 0x00AC0000,
            PercentTHDAmps = 0x00B00000,
            CoincidentPF = 0x00B40000,
            VALeading = 0x00B80000,
            VALagging = 0x00BC0000,
            VarLeading = 0x00C00000,
            VarLagging = 0x00C40000,
            VALeadingNoType = 0x00C80000,
            VALaggingNoType = 0x00CC0000,
            VADeliveredNoType = 0x00D00000,
            VAReceivedNoType = 0x00D40000,
            VANetNoType = 0x00D80000,
            VAQ1NoType = 0x00DC0000,
            VAQ2NoType = 0x00E00000,
            VAQ3NoType = 0x00E40000,
            VAQ4NoType = 0x00E80000,
            WattNoDirection = 0x00EC0000,
            QNet = 0x00F00000,
            WattNoDirectionLegacy = 0x00F40000,
            Totalized2 = 0x00F80000,
            Totalized3 = 0x00FC0000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Coincident types
        /// </summary>
        public enum CoincidentTypes : uint
        {
#pragma warning disable 1591
            Instantaneous = 0x00000000,
            Minimum = 0x01000000,
            Average = 0x02000000,
            Maximum = 0x03000000,
            Present = 0x04000000,
            Previous = 0x05000000,
            Cum = 0x06000000,
            CCum = 0x07000000,
            Projected = 0x08000000,
            InstantaneousThermal = 0x09000000,
            MinimumThermal = 0x0A000000,
            MaximumThermal = 0x0B000000,
            PresentThermal = 0x0C000000,
            PreviousThermal = 0x0D000000,
            CumThermal = 0x0E000000,
            CCumThermal = 0x0F000000,
            NormalDemand = 0x10000000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Coincident scales
        /// </summary>
        public enum CoincidentScales : uint
        {
#pragma warning disable 1591
            Units = 0x00000000,
            Kilo = 0x20000000,
            Mega = 0x40000000,
            Giga = 0x60000000,
#pragma warning restore 1591
        }

        /// <summary>
        /// Peak phase rates
        /// </summary>
        public enum PeakPhaseRates : uint
        {
            /// <summary>No Phase Rate</summary>
            NoPhaseRate = 0x00000000,
            /// <summary>Same As Peak Phase Rate</summary>
            SameAsPeakPhaseRate = 0x80000000,
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the units for peak values
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public PeakUnits PeakUnit
        {
            get
            {
                return (PeakUnits)(m_QuantityCode & PEAK_UNITS_MASK);
            }
        }

        /// <summary>
        /// Gets the type for peak values
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public PeakTypes PeakType
        {
            get
            {
                return (PeakTypes)(m_QuantityCode & PEAK_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the phase or rate for coincident values
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public CoincidentPhaseRates CoincidentPhaseRate
        {
            get
            {
                return (CoincidentPhaseRates)(m_QuantityCode & COIN_PHASE_RATE_MASK);
            }
        }

        /// <summary>
        /// Gets the scale for peak values
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public PeakScales PeakScale
        {
            get
            {
                return (PeakScales)(m_QuantityCode & PEAK_SCALE_MASK);
            }
        }

        /// <summary>
        /// Gets the units for coincident values
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public CoincidentUnits CoincidentUnit
        {
            get
            {
                return (CoincidentUnits)(m_QuantityCode & COIN_UNITS_MASK);
            }
        }

        /// <summary>
        /// Gets the type for coincident values
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public CoincidentTypes CoincidentType
        {
            get
            {
                return (CoincidentTypes)(m_QuantityCode & COIN_TYPE_MASK);
            }
        }

        /// <summary>
        /// Gets the scale for coincident values
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public CoincidentScales CoincidentScale
        {
            get
            {
                return (CoincidentScales)(m_QuantityCode & COIN_SCALE_MASK);
            }
        }

        /// <summary>
        /// Gets the phase or rate for peak values
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        public PeakPhaseRates PeakPhaseRate
        {
            get
            {
                return (PeakPhaseRates)(m_QuantityCode & PEAK_PHASE_RATE_MASK);
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is bidirectional
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsBidirectional
        {
            get
            {
                return PeakUnit == PeakUnits.QNet
                    || PeakUnit == PeakUnits.VANet
                    || PeakUnit == PeakUnits.VANetNoType
                    || PeakUnit == PeakUnits.VANetVect
                    || PeakUnit == PeakUnits.VAReceived
                    || PeakUnit == PeakUnits.VAReceivedNoType
                    || PeakUnit == PeakUnits.VAReceivedVect
                    || PeakUnit == PeakUnits.VarNet
                    || PeakUnit == PeakUnits.VarNetVect
                    || PeakUnit == PeakUnits.VarReceived
                    || PeakUnit == PeakUnits.VarReceivedVect
                    || PeakUnit == PeakUnits.WattNet
                    || PeakUnit == PeakUnits.WattReceived
                    || CoincidentUnit == CoincidentUnits.QNet
                    || CoincidentUnit == CoincidentUnits.VANet
                    || CoincidentUnit == CoincidentUnits.VANetNoType
                    || CoincidentUnit == CoincidentUnits.VANetVect
                    || CoincidentUnit == CoincidentUnits.VAReceived
                    || CoincidentUnit == CoincidentUnits.VAReceivedNoType
                    || CoincidentUnit == CoincidentUnits.VAReceivedVect
                    || CoincidentUnit == CoincidentUnits.VarNet
                    || CoincidentUnit == CoincidentUnits.VarNetVect
                    || CoincidentUnit == CoincidentUnits.VarReceived
                    || CoincidentUnit == CoincidentUnits.VarReceivedVect
                    || CoincidentUnit == CoincidentUnits.WattNet
                    || CoincidentUnit == CoincidentUnits.WattReceived;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is leading
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public override bool IsLeading
        {
            get
            {
                return PeakUnit == PeakUnits.VALeading
                    || PeakUnit == PeakUnits.VALeadingNoType
                    || PeakUnit == PeakUnits.VarLeading
                    || CoincidentUnit == CoincidentUnits.VALeading
                    || CoincidentUnit == CoincidentUnits.VALeadingNoType
                    || CoincidentUnit == CoincidentUnits.VarLeading;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is lagging
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public override bool IsLagging
        {
            get
            {
                return PeakUnit == PeakUnits.VALagging
                    || PeakUnit == PeakUnits.VALaggingNoType
                    || PeakUnit == PeakUnits.VarLagging
                    || CoincidentUnit == CoincidentUnits.VALagging
                    || CoincidentUnit == CoincidentUnits.VALaggingNoType
                    || CoincidentUnit == CoincidentUnits.VarLagging;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a power quality quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsPowerQualityQuantity
        {
            get
            {
                return PeakUnit == PeakUnits.CoincidentAmp
                    || PeakUnit == PeakUnits.CoincidentAmpSq
                    || PeakUnit == PeakUnits.CoincidentNeutralAmp
                    || PeakUnit == PeakUnits.CoincidentVolt
                    || PeakUnit == PeakUnits.CoincidentVSq
                    || CoincidentUnit == CoincidentUnits.CoincidentAmp
                    || CoincidentUnit == CoincidentUnits.CoincidentAmpSq
                    || CoincidentUnit == CoincidentUnits.CoincidentNeutralAmp
                    || CoincidentUnit == CoincidentUnits.CoincidentVolt
                    || CoincidentUnit == CoincidentUnits.CoincidentVSq;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Q quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsQQuantity
        {
            get
            {
                return PeakUnit == PeakUnits.CoincidentQ
                    || PeakUnit == PeakUnits.QNet
                    || CoincidentUnit == CoincidentUnits.CoincidentQ
                    || CoincidentUnit == CoincidentUnits.QNet;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a VA quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsVAQuantity
        {
            get
            {
                return PeakUnit == PeakUnits.VADelivered
                    || PeakUnit == PeakUnits.VADeliveredNoType
                    || PeakUnit == PeakUnits.VADeliveredVect
                    || PeakUnit == PeakUnits.VALagging
                    || PeakUnit == PeakUnits.VALaggingNoType
                    || PeakUnit == PeakUnits.VALeading
                    || PeakUnit == PeakUnits.VALeadingNoType
                    || PeakUnit == PeakUnits.VANet
                    || PeakUnit == PeakUnits.VANetNoType
                    || PeakUnit == PeakUnits.VANetVect
                    || PeakUnit == PeakUnits.VAQ1
                    || PeakUnit == PeakUnits.VAQ1NoType
                    || PeakUnit == PeakUnits.VAQ1Vect
                    || PeakUnit == PeakUnits.VAQ2
                    || PeakUnit == PeakUnits.VAQ2NoType
                    || PeakUnit == PeakUnits.VAQ2Vect
                    || PeakUnit == PeakUnits.VAQ3
                    || PeakUnit == PeakUnits.VAQ3NoType
                    || PeakUnit == PeakUnits.VAQ3Vect
                    || PeakUnit == PeakUnits.VAQ4
                    || PeakUnit == PeakUnits.VAQ4NoType
                    || PeakUnit == PeakUnits.VAQ4Vect
                    || PeakUnit == PeakUnits.VAReceived
                    || PeakUnit == PeakUnits.VAReceivedNoType
                    || PeakUnit == PeakUnits.VAReceivedVect
                    || CoincidentUnit == CoincidentUnits.VADelivered
                    || CoincidentUnit == CoincidentUnits.VADeliveredNoType
                    || CoincidentUnit == CoincidentUnits.VADeliveredVect
                    || CoincidentUnit == CoincidentUnits.VALagging
                    || CoincidentUnit == CoincidentUnits.VALaggingNoType
                    || CoincidentUnit == CoincidentUnits.VALeading
                    || CoincidentUnit == CoincidentUnits.VALeadingNoType
                    || CoincidentUnit == CoincidentUnits.VANet
                    || CoincidentUnit == CoincidentUnits.VANetNoType
                    || CoincidentUnit == CoincidentUnits.VANetVect
                    || CoincidentUnit == CoincidentUnits.VAQ1
                    || CoincidentUnit == CoincidentUnits.VAQ1NoType
                    || CoincidentUnit == CoincidentUnits.VAQ1Vect
                    || CoincidentUnit == CoincidentUnits.VAQ2
                    || CoincidentUnit == CoincidentUnits.VAQ2NoType
                    || CoincidentUnit == CoincidentUnits.VAQ2Vect
                    || CoincidentUnit == CoincidentUnits.VAQ3
                    || CoincidentUnit == CoincidentUnits.VAQ3NoType
                    || CoincidentUnit == CoincidentUnits.VAQ3Vect
                    || CoincidentUnit == CoincidentUnits.VAQ4
                    || CoincidentUnit == CoincidentUnits.VAQ4NoType
                    || CoincidentUnit == CoincidentUnits.VAQ4Vect
                    || CoincidentUnit == CoincidentUnits.VAReceived
                    || CoincidentUnit == CoincidentUnits.VAReceivedNoType
                    || CoincidentUnit == CoincidentUnits.VAReceivedVect;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Var quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created

        public override bool IsVarQuantity
        {
            get
            {
                return PeakUnit == PeakUnits.VarDelivered
                    || PeakUnit == PeakUnits.VarDeliveredVect
                    || PeakUnit == PeakUnits.VarLagging
                    || PeakUnit == PeakUnits.VarLeading
                    || PeakUnit == PeakUnits.VarNet
                    || PeakUnit == PeakUnits.VarNetVect
                    || PeakUnit == PeakUnits.VarQ1
                    || PeakUnit == PeakUnits.VarQ1Vect
                    || PeakUnit == PeakUnits.VarQ2
                    || PeakUnit == PeakUnits.VarQ2Vect
                    || PeakUnit == PeakUnits.VarQ3
                    || PeakUnit == PeakUnits.VarQ3Vect
                    || PeakUnit == PeakUnits.VarQ4
                    || PeakUnit == PeakUnits.VarQ4Vect
                    || PeakUnit == PeakUnits.VarReceived
                    || PeakUnit == PeakUnits.VarReceivedVect
                    || CoincidentUnit == CoincidentUnits.VarDelivered
                    || CoincidentUnit == CoincidentUnits.VarDeliveredVect
                    || CoincidentUnit == CoincidentUnits.VarLagging
                    || CoincidentUnit == CoincidentUnits.VarLeading
                    || CoincidentUnit == CoincidentUnits.VarNet
                    || CoincidentUnit == CoincidentUnits.VarNetVect
                    || CoincidentUnit == CoincidentUnits.VarQ1
                    || CoincidentUnit == CoincidentUnits.VarQ1Vect
                    || CoincidentUnit == CoincidentUnits.VarQ2
                    || CoincidentUnit == CoincidentUnits.VarQ2Vect
                    || CoincidentUnit == CoincidentUnits.VarQ3
                    || CoincidentUnit == CoincidentUnits.VarQ3Vect
                    || CoincidentUnit == CoincidentUnits.VarQ4
                    || CoincidentUnit == CoincidentUnits.VarQ4Vect
                    || CoincidentUnit == CoincidentUnits.VarReceived
                    || CoincidentUnit == CoincidentUnits.VarReceivedVect;
            }
        }

        /// <summary>
        /// Gets whether or not the quantity is a Watt quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/17/08 jrf 9.50           Created

        public override bool IsWattQuantity
        {
            get
            {
                return PeakUnit == PeakUnits.WattDelivered
                    || PeakUnit == PeakUnits.WattNet
                    || PeakUnit == PeakUnits.WattNoDirection
                    || PeakUnit == PeakUnits.WattNoDirectionLegacy
                    || PeakUnit == PeakUnits.WattReceived
                    || CoincidentUnit == CoincidentUnits.WattDelivered
                    || CoincidentUnit == CoincidentUnits.WattNet
                    || CoincidentUnit == CoincidentUnits.WattNoDirection
                    || CoincidentUnit == CoincidentUnits.WattNoDirectionLegacy
                    || CoincidentUnit == CoincidentUnits.WattReceived;
            }
        }


        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructor that should only be called by QuantityCode.Create
        /// </summary>
        /// <param name="code">The actual quantity code</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 RCG 9.50           Created
        
        internal ExtendedCoincidentQuantityCode(uint code)
            : base(code)
        {
        }

        #endregion
    }
}
