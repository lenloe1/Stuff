using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itron.Metering.Utilities;

namespace Itron.Metering.DeviceDataTypes
{
    /// <summary>
    /// Simple Metering Units Of Measure
    /// </summary>
    public enum SimpleMeteringUnitOfMeasure : byte
    {
        /// <summary>kW and kWh in Binary Format</summary>
        [EnumDescription("kW")]
        kWBinary = 0x00,
        /// <summary>Cubic Meter and Cubic Meter per hour in Binary Format</summary>
        [EnumDescription("Cubic Meter")]
        CubicMeterBinary = 0x01,
        /// <summary>Cubic Feet and Cubic Feet per hour in Binary Format</summary>
        [EnumDescription("Cubic Feet")]
        CubicFeetBinary = 0x02,
        /// <summary>ccf and ccf/h in Binary Format</summary>
        [EnumDescription("CCF")]
        ccfBinary = 0x03,
        /// <summary>US Gallons in Binary Format</summary>
        [EnumDescription("US Gal")]
        USGalBinary = 0x04,
        /// <summary>Imperial Gallongs in Binary Format</summary>
        [EnumDescription("Imp Gal")]
        ImpGalBinary = 0x05,
        /// <summary>BTU and BTU/h in Binary Format</summary>
        [EnumDescription("BTU")]
        BTUBinary = 0x06,
        /// <summary>Liters in Binary Format</summary>
        [EnumDescription("l")]
        LitersBinary = 0x07,
        /// <summary>kPA (guage) in Binary Format</summary>
        [EnumDescription("Gauge (kPA)")]
        kPAGuageBinary = 0x08,
        /// <summary>kPA (absolute) in Binary Format</summary>
        [EnumDescription("Absolute (kPA)")]
        kPAAbsoluteBinary = 0x09,
        /// <summary>mcf and mcf/h in Binary Format</summary>
        [EnumDescription("MCF")]
        mcfBinary = 0x0A,
        /// <summary>No Units in Binary Format</summary>
        [EnumDescription("")]
        UnitlessBinary = 0x0B,
        /// <summary>MJ in Binary Format</summary>
        [EnumDescription("MJ")]
        MJBinary = 0x0C,

        /// <summary>kW and kWh in BCD format</summary>
        [EnumDescription("kW")]
        kWBCD = 0x80,
        /// <summary>Cubic Meters and Cubic Meters per hour in BCD format</summary>
        [EnumDescription("Cubic Meter")]
        CubicMeterBCD = 0x81,
        /// <summary>Cubic Feet and Cubic Feet per hour in BCD format</summary>
        [EnumDescription("Cubic Feet")]
        CubicFeetBCD = 0x82,
        /// <summary>ccf and ccf/h in BCD format</summary>
        [EnumDescription("CCF")]
        ccfBCDy = 0x83,
        /// <summary>US Gallons in BCD format</summary>
        [EnumDescription("US Gal")]
        USGalBCD = 0x84,
        /// <summary>Imperial Gallons in BCD format</summary>
        [EnumDescription("Imp Gal")]
        ImpGalBCD = 0x85,
        /// <summary>BTU and BTU/h in BCD format</summary>
        [EnumDescription("BTU")]
        BTUBCD = 0x86,
        /// <summary>Liters in BCD format</summary>
        [EnumDescription("l")]
        LitersBCD = 0x87,
        /// <summary>kPA (gauge) in BCD format</summary>
        [EnumDescription("Gauge (kPA)")]
        kPAGuageBCD = 0x88,
        /// <summary>kPA (absolute) in BCD format</summary>
        [EnumDescription("Absolute (kPA)")]
        kPAAbsoluteBCD = 0x89,
        /// <summary>mcf and mcf/h in BCD format</summary>
        [EnumDescription("MCF")]
        mcfBCD = 0x8A,
        /// <summary>No Units in BCD format</summary>
        [EnumDescription("")]
        UnitlessBCD = 0x8B,
        /// <summary>MJ in BCD format</summary>
        [EnumDescription("MJ")]
        MJBCD = 0x8C,
    }
}
