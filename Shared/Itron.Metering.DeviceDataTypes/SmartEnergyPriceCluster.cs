using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Itron.Metering.Utilities;

namespace Itron.Metering.DeviceDataTypes
{

    #region Definitions

    /// <summary>
    /// Price Cluster Attribute Sets
    /// </summary>
    public enum PriceClusterAttributeSets : byte
    {
        /// <summary>Tier Label</summary>
        TierLabel = 0x00,
        /// <summary>Block Threshold</summary>
        BlockThreshold = 0x01,
        /// <summary>Block Period</summary>
        BlockPeriod = 0x02,
        /// <summary>Commodity</summary>
        Commodity = 0x03,
        /// <summary>Block Price Information</summary>
        BlockPriceInformation = 0x04,
        /// <summary>Reserved</summary>
        Reserved1 = 0x05,
        /// <summary>Reserved</summary>
        Reserved2 = 0x06,
        /// <summary>Billing Information Set</summary>
        BillingInformationSet = 0x07,
        /// <summary>Reserved</summary>
        Reserved3 = 0x08,
    }

    /// <summary>
    /// Tier Label Attribute Set
    /// </summary>
    public enum TierLabelAttributeSet : ushort
    {
        /// <summary>Tier1 Price Label</summary>
        Tier1PriceLabel = 0x00 | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Tier2 Price Label</summary>
        Tier2PriceLabel = 0x01 | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Tier3 Price Label</summary>
        Tier3PriceLabel = 0x02 | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Tier4 Price Label</summary>
        Tier4PriceLabel = 0x03 | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Tier5 Price Label</summary>
        Tier5PriceLabel = 0x04 | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Tier6 Price Label</summary>
        Tier6PriceLabel = 0x05 | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Tier7 Price Label</summary>
        Tier7PriceLabel = 0x06 | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Tier8 Price Label</summary>
        Tier8PriceLabel = 0x07 | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Tier9 Price Label</summary>
        Tier9PriceLabel = 0x08 | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Tier10 Price Label</summary>
        Tier10PriceLabel = 0x09 | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Tier11 Price Label</summary>
        Tier11PriceLabel = 0x0A | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Tier12 Price Label</summary>
        Tier12PriceLabel = 0x0B | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Tier13 Price Label</summary>
        Tier13PriceLabel = 0x0C | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Tier14 Price Label</summary>
        Tier14PriceLabel = 0x0D | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Tier15 Price Label</summary>
        Tier15PriceLabel = 0x0E | (PriceClusterAttributeSets.TierLabel << 8),
        /// <summary>Reserved</summary>
        Reserved = 0x0F | (PriceClusterAttributeSets.TierLabel << 8),
    }

    /// <summary>
    /// Block Threshold Attribute Set
    /// </summary>
    public enum BlockThresholdAttributeSet : ushort
    {
        /// <summary>Block1 Price Label</summary>
        Block1Threshold = 0x00 | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Block2 Price Label</summary>
        Block2Threshold = 0x01 | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Block3 Price Label</summary>
        Block3Threshold = 0x02 | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Block4 Price Label</summary>
        Block4Threshold = 0x03 | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Block5 Price Label</summary>
        Block5Threshold = 0x04 | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Block6 Price Label</summary>
        Block6Threshold = 0x05 | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Block7 Price Label</summary>
        Block7Threshold = 0x06 | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Block8 Price Label</summary>
        Block8Threshold = 0x07 | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Block9 Price Label</summary>
        Block9Threshold = 0x08 | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Block10 Price Label</summary>
        Block10Threshold = 0x09 | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Block11 Price Label</summary>
        Block11Threshold = 0x0A | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Block12 Price Label</summary>
        Block12Threshold = 0x0B | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Block13 Price Label</summary>
        Block13Threshold = 0x0C | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Block14 Price Label</summary>
        Block14Threshold = 0x0D | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Block15 Price Label</summary>
        Block15Threshold = 0x0E | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
        /// <summary>Reserved</summary>
        Reserved = 0x0F | (ushort)PriceClusterAttributeSets.BlockThreshold << 8,
    }

    /// <summary>
    /// Block Period Attribute Set
    /// </summary>
    public enum BlockPeriodAttributeSet : ushort
    {
        /// <summary>Start of Block Period</summary>
        StartOfBlockPeriod = 0x00 | (PriceClusterAttributeSets.BlockPeriod << 8),
        /// <summary>Block Period Duration (Minutes)</summary>
        BlockPeriodDuration = 0x01 | (PriceClusterAttributeSets.BlockPeriod << 8),
        /// <summary>Threshold Multiplier</summary>
        ThresholdMultiplier = 0x02 | (PriceClusterAttributeSets.BlockPeriod << 8),
        /// <summary>Threshold Divisor</summary>
        ThresholdDivisor = 0x03 | (PriceClusterAttributeSets.BlockPeriod << 8),
        /// <summary>Reserved</summary>
        Reserved = 0x04,
    }

    /// <summary>
    /// Commodity Attribute Set
    /// </summary>
    public enum CommodityAttributeSet : ushort
    {
        /// <summary>Commodity Type</summary>
        CommodityType = 0x00 | (PriceClusterAttributeSets.Commodity << 8),
        /// <summary>Standing Charge</summary>
        StandingCharge = 0x01 | (PriceClusterAttributeSets.Commodity << 8),
        /// <summary>Reserved</summary>
        Reserved = 0x02 | (PriceClusterAttributeSets.Commodity << 8),
    }

    /// <summary>
    /// Block Price Information Attribute Set
    /// </summary>
    public enum BlockPriceInformationAttributeSet : ushort
    {
        /// <summary>No Tier Block1 Price</summary>
        NoTierBlock1Price = 0x00 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block2 Price</summary>
        NoTierBlock2Price = 0x01 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block3 Price</summary>
        NoTierBlock3Price = 0x02 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block4 Price</summary>
        NoTierBlock4Price = 0x03 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block5 Price</summary>
        NoTierBlock5Price = 0x04 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block6 Price</summary>
        NoTierBlock6Price = 0x05 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block7 Price</summary>
        NoTierBlock7Price = 0x06 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block8 Price</summary>
        NoTierBlock8Price = 0x07 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block9 Price</summary>
        NoTierBlock9Price = 0x08 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block10 Price</summary>
        NoTierBlock10Price = 0x09 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block11 Price</summary>
        NoTierBlock11Price = 0x0A | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block12 Price</summary>
        NoTierBlock12Price = 0x0B | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block13 Price</summary>
        NoTierBlock13Price = 0x0C | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block14 Price</summary>
        NoTierBlock14Price = 0x0D | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block15 Price</summary>
        NoTierBlock15Price = 0x0E | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>No Tier Block16 Price</summary>
        NoTierBlock16Price = 0x0F | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block1 Price</summary>
        Tier1Block1Price = 0x10 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block2 Price</summary>
        Tier1Block2Price = 0x11 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block3 Price</summary>
        Tier1Block3Price = 0x12 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block4 Price</summary>
        Tier1Block4Price = 0x13 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block5 Price</summary>
        Tier1Block5Price = 0x14 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block6 Price</summary>
        Tier1Block6Price = 0x15 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block7 Price</summary>
        Tier1Block7Price = 0x16 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block8 Price</summary>
        Tier1Block8Price = 0x17 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block9 Price</summary>
        Tier1Block9Price = 0x18 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block10 Price</summary>
        Tier1Block10Price = 0x19 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block11 Price</summary>
        Tier1Block11Price = 0x1A | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block12 Price</summary>
        Tier1Block12Price = 0x1B | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block13 Price</summary>
        Tier1Block13Price = 0x1C | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block14 Price</summary>
        Tier1Block14Price = 0x1D | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block15 Price</summary>
        Tier1Block15Price = 0x1E | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier1 Block16 Price</summary>
        Tier1Block16Price = 0x1F | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block1 Price</summary>
        Tier2Block1Price = 0x20 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block2 Price</summary>
        Tier2Block2Price = 0x21 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block3 Price</summary>
        Tier2Block3Price = 0x22 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block4 Price</summary>
        Tier2Block4Price = 0x23 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block5 Price</summary>
        Tier2Block5Price = 0x24 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block6 Price</summary>
        Tier2Block6Price = 0x25 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block7 Price</summary>
        Tier2Block7Price = 0x26 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block8 Price</summary>
        Tier2Block8Price = 0x27 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block9 Price</summary>
        Tier2Block9Price = 0x28 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block10 Price</summary>
        Tier2Block10Price = 0x29 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block11 Price</summary>
        Tier2Block11Price = 0x2A | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block12 Price</summary>
        Tier2Block12Price = 0x2B | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block13 Price</summary>
        Tier2Block13Price = 0x2C | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block14 Price</summary>
        Tier2Block14Price = 0x2D | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block15 Price</summary>
        Tier2Block15Price = 0x2E | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier2 Block16 Price</summary>
        Tier2Block16Price = 0x2F | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block1 Price</summary>
        Tier3Block1Price = 0x30 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block2 Price</summary>
        Tier3Block2Price = 0x31 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block3 Price</summary>
        Tier3Block3Price = 0x32 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block4 Price</summary>
        Tier3Block4Price = 0x33 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block5 Price</summary>
        Tier3Block5Price = 0x34 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block6 Price</summary>
        Tier3Block6Price = 0x35 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block7 Price</summary>
        Tier3Block7Price = 0x36 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block8 Price</summary>
        Tier3Block8Price = 0x37 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block9 Price</summary>
        Tier3Block9Price = 0x38 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block10 Price</summary>
        Tier3Block10Price = 0x39 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block11 Price</summary>
        Tier3Block11Price = 0x3A | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block12 Price</summary>
        Tier3Block12Price = 0x3B | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block13 Price</summary>
        Tier3Block13Price = 0x3C | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block14 Price</summary>
        Tier3Block14Price = 0x3D | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block15 Price</summary>
        Tier3Block15Price = 0x3E | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier3 Block16 Price</summary>
        Tier3Block16Price = 0x3F | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block1 Price</summary>
        Tier4Block1Price = 0x40 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block2 Price</summary>
        Tier4Block2Price = 0x41 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block3 Price</summary>
        Tier4Block3Price = 0x42 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block4 Price</summary>
        Tier4Block4Price = 0x43 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block5 Price</summary>
        Tier4Block5Price = 0x44 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block6 Price</summary>
        Tier4Block6Price = 0x45 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block7 Price</summary>
        Tier4Block7Price = 0x46 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block8 Price</summary>
        Tier4Block8Price = 0x47 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block9 Price</summary>
        Tier4Block9Price = 0x48 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block10 Price</summary>
        Tier4Block10Price = 0x49 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block11 Price</summary>
        Tier4Block11Price = 0x4A | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block12 Price</summary>
        Tier4Block12Price = 0x4B | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block13 Price</summary>
        Tier4Block13Price = 0x4C | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block14 Price</summary>
        Tier4Block14Price = 0x4D | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block15 Price</summary>
        Tier4Block15Price = 0x4E | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier4 Block16 Price</summary>
        Tier4Block16Price = 0x4F | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block1 Price</summary>
        Tier5Block1Price = 0x50 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block2 Price</summary>
        Tier5Block2Price = 0x51 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block3 Price</summary>
        Tier5Block3Price = 0x52 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block4 Price</summary>
        Tier5Block4Price = 0x53 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block5 Price</summary>
        Tier5Block5Price = 0x54 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block6 Price</summary>
        Tier5Block6Price = 0x55 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block7 Price</summary>
        Tier5Block7Price = 0x56 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block8 Price</summary>
        Tier5Block8Price = 0x57 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block9 Price</summary>
        Tier5Block9Price = 0x58 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block10 Price</summary>
        Tier5Block10Price = 0x59 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block11 Price</summary>
        Tier5Block11Price = 0x5A | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block12 Price</summary>
        Tier5Block12Price = 0x5B | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block13 Price</summary>
        Tier5Block13Price = 0x5C | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block14 Price</summary>
        Tier5Block14Price = 0x5D | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block15 Price</summary>
        Tier5Block15Price = 0x5E | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier5 Block16 Price</summary>
        Tier5Block16Price = 0x5F | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block1 Price</summary>
        Tier6Block1Price = 0x60 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block2 Price</summary>
        Tier6Block2Price = 0x61 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block3 Price</summary>
        Tier6Block3Price = 0x62 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block4 Price</summary>
        Tier6Block4Price = 0x63 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block5 Price</summary>
        Tier6Block5Price = 0x64 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block6 Price</summary>
        Tier6Block6Price = 0x65 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block7 Price</summary>
        Tier6Block7Price = 0x66 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block8 Price</summary>
        Tier6Block8Price = 0x67 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block9 Price</summary>
        Tier6Block9Price = 0x68 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block10 Price</summary>
        Tier6Block10Price = 0x69 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block11 Price</summary>
        Tier6Block11Price = 0x6A | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block12 Price</summary>
        Tier6Block12Price = 0x6B | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block13 Price</summary>
        Tier6Block13Price = 0x6C | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block14 Price</summary>
        Tier6Block14Price = 0x6D | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block15 Price</summary>
        Tier6Block15Price = 0x6E | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier6 Block16 Price</summary>
        Tier6Block16Price = 0x6F | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block1 Price</summary>
        Tier7Block1Price = 0x70 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block2 Price</summary>
        Tier7Block2Price = 0x71 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block3 Price</summary>
        Tier7Block3Price = 0x72 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block4 Price</summary>
        Tier7Block4Price = 0x73 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block5 Price</summary>
        Tier7Block5Price = 0x74 | (PriceClusterAttributeSets.BlockPriceInformation << 8) | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block6 Price</summary>
        Tier7Block6Price = 0x75 | (PriceClusterAttributeSets.BlockPriceInformation << 8) | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block7 Price</summary>
        Tier7Block7Price = 0x76 | (PriceClusterAttributeSets.BlockPriceInformation << 8) | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block8 Price</summary>
        Tier7Block8Price = 0x77 | (PriceClusterAttributeSets.BlockPriceInformation << 8) | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block9 Price</summary>
        Tier7Block9Price = 0x78 | (PriceClusterAttributeSets.BlockPriceInformation << 8) | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block10 Price</summary>
        Tier7Block10Price = 0x79 | (PriceClusterAttributeSets.BlockPriceInformation << 8) | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block11 Price</summary>
        Tier7Block11Price = 0x7A | (PriceClusterAttributeSets.BlockPriceInformation << 8) | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block12 Price</summary>
        Tier7Block12Price = 0x7B | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block13 Price</summary>
        Tier7Block13Price = 0x7C | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block14 Price</summary>
        Tier7Block14Price = 0x7D | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block15 Price</summary>
        Tier7Block15Price = 0x7E | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier7 Block16 Price</summary>
        Tier7Block16Price = 0x7F | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block1 Price</summary>
        Tier8Block1Price = 0x80 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block2 Price</summary>
        Tier8Block2Price = 0x81 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block3 Price</summary>
        Tier8Block3Price = 0x82 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block4 Price</summary>
        Tier8Block4Price = 0x83 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block5 Price</summary>
        Tier8Block5Price = 0x84 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block6 Price</summary>
        Tier8Block6Price = 0x85 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block7 Price</summary>
        Tier8Block7Price = 0x86 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block8 Price</summary>
        Tier8Block8Price = 0x87 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block9 Price</summary>
        Tier8Block9Price = 0x88 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block10 Price</summary>
        Tier8Block10Price = 0x89 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block11 Price</summary>
        Tier8Block11Price = 0x8A | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block12 Price</summary>
        Tier8Block12Price = 0x8B | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block13 Price</summary>
        Tier8Block13Price = 0x8C | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block14 Price</summary>
        Tier8Block14Price = 0x8D | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block15 Price</summary>
        Tier8Block15Price = 0x8E | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier8 Block16 Price</summary>
        Tier8Block16Price = 0x8F | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block1 Price</summary>
        Tier9Block1Price = 0x90 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block2 Price</summary>
        Tier9Block2Price = 0x91 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block3 Price</summary>
        Tier9Block3Price = 0x92 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block4 Price</summary>
        Tier9Block4Price = 0x93 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block5 Price</summary>
        Tier9Block5Price = 0x94 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block6 Price</summary>
        Tier9Block6Price = 0x95 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block7 Price</summary>
        Tier9Block7Price = 0x96 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block8 Price</summary>
        Tier9Block8Price = 0x97 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block9 Price</summary>
        Tier9Block9Price = 0x98 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block10 Price</summary>
        Tier9Block10Price = 0x99 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block11 Price</summary>
        Tier9Block11Price = 0x9A | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block12 Price</summary>
        Tier9Block12Price = 0x9B | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block13 Price</summary>
        Tier9Block13Price = 0x9C | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block14 Price</summary>
        Tier9Block14Price = 0x9D | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block15 Price</summary>
        Tier9Block15Price = 0x9E | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier9 Block16 Price</summary>
        Tier9Block16Price = 0x9F | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block1 Price</summary>
        Tier10Block1Price = 0xA0 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block2 Price</summary>
        Tier10Block2Price = 0xA1 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block3 Price</summary>
        Tier10Block3Price = 0xA2 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block4 Price</summary>
        Tier10Block4Price = 0xA3 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block5 Price</summary>
        Tier10Block5Price = 0xA4 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block6 Price</summary>
        Tier10Block6Price = 0xA5 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block7 Price</summary>
        Tier10Block7Price = 0xA6 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block8 Price</summary>
        Tier10Block8Price = 0xA7 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block9 Price</summary>
        Tier10Block9Price = 0xA8 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block10 Price</summary>
        Tier10Block10Price = 0xA9 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block11 Price</summary>
        Tier10Block11Price = 0xAA | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block12 Price</summary>
        Tier10Block12Price = 0xAB | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block13 Price</summary>
        Tier10Block13Price = 0xAC | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block14 Price</summary>
        Tier10Block14Price = 0xAD | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block15 Price</summary>
        Tier10Block15Price = 0xAE | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier10 Block16 Price</summary>
        Tier10Block16Price = 0xAF | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block1 Price</summary>
        Tier11Block1Price = 0xB0 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block2 Price</summary>
        Tier11Block2Price = 0xB1 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block3 Price</summary>
        Tier11Block3Price = 0xB2 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block4 Price</summary>
        Tier11Block4Price = 0xB3 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block5 Price</summary>
        Tier11Block5Price = 0xB4 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block6 Price</summary>
        Tier11Block6Price = 0xB5 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block7 Price</summary>
        Tier11Block7Price = 0xB6 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block8 Price</summary>
        Tier11Block8Price = 0xB7 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block9 Price</summary>
        Tier11Block9Price = 0xB8 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block10 Price</summary>
        Tier11Block10Price = 0xB9 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block11 Price</summary>
        Tier11Block11Price = 0xBA | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block12 Price</summary>
        Tier11Block12Price = 0xBB | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block13 Price</summary>
        Tier11Block13Price = 0xBC | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block14 Price</summary>
        Tier11Block14Price = 0xBD | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block15 Price</summary>
        Tier11Block15Price = 0xBE | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier11 Block16 Price</summary>
        Tier11Block16Price = 0xBF | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block1 Price</summary>
        Tier12Block1Price = 0xC0 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block2 Price</summary>
        Tier12Block2Price = 0xC1 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block3 Price</summary>
        Tier12Block3Price = 0xC2 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block4 Price</summary>
        Tier12Block4Price = 0xC3 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block5 Price</summary>
        Tier12Block5Price = 0xC4 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block6 Price</summary>
        Tier12Block6Price = 0xC5 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block7 Price</summary>
        Tier12Block7Price = 0xC6 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block8 Price</summary>
        Tier12Block8Price = 0xC7 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block9 Price</summary>
        Tier12Block9Price = 0xC8 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block10 Price</summary>
        Tier12Block10Price = 0xC9 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block11 Price</summary>
        Tier12Block11Price = 0xCA | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block12 Price</summary>
        Tier12Block12Price = 0xCB | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block13 Price</summary>
        Tier12Block13Price = 0xCC | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block14 Price</summary>
        Tier12Block14Price = 0xCD | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block15 Price</summary>
        Tier12Block15Price = 0xCE | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier12 Block16 Price</summary>
        Tier12Block16Price = 0xCF | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block1 Price</summary>
        Tier13Block1Price = 0xD0 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block2 Price</summary>
        Tier13Block2Price = 0xD1 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block3 Price</summary>
        Tier13Block3Price = 0xD2 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block4 Price</summary>
        Tier13Block4Price = 0xD3 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block5 Price</summary>
        Tier13Block5Price = 0xD4 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block6 Price</summary>
        Tier13Block6Price = 0xD5 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block7 Price</summary>
        Tier13Block7Price = 0xD6 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block8 Price</summary>
        Tier13Block8Price = 0xD7 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block9 Price</summary>
        Tier13Block9Price = 0xD8 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block10 Price</summary>
        Tier13Block10Price = 0xD9 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block11 Price</summary>
        Tier13Block11Price = 0xDA | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block12 Price</summary>
        Tier13Block12Price = 0xDB | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block13 Price</summary>
        Tier13Block13Price = 0xDC | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block14 Price</summary>
        Tier13Block14Price = 0xDD | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block15 Price</summary>
        Tier13Block15Price = 0xDE | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier13 Block16 Price</summary>
        Tier13Block16Price = 0xDF | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block1 Price</summary>
        Tier14Block1Price = 0xE0 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block2 Price</summary>
        Tier14Block2Price = 0xE1 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block3 Price</summary>
        Tier14Block3Price = 0xE2 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block4 Price</summary>
        Tier14Block4Price = 0xE3 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block5 Price</summary>
        Tier14Block5Price = 0xE4 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block6 Price</summary>
        Tier14Block6Price = 0xE5 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block7 Price</summary>
        Tier14Block7Price = 0xE6 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block8 Price</summary>
        Tier14Block8Price = 0xE7 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block9 Price</summary>
        Tier14Block9Price = 0xE8 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block10 Price</summary>
        Tier14Block10Price = 0xE9 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block11 Price</summary>
        Tier14Block11Price = 0xEA | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block12 Price</summary>
        Tier14Block12Price = 0xEB | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block13 Price</summary>
        Tier14Block13Price = 0xEC | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block14 Price</summary>
        Tier14Block14Price = 0xED | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block15 Price</summary>
        Tier14Block15Price = 0xEE | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier14 Block16 Price</summary>
        Tier14Block16Price = 0xEF | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block1 Price</summary>
        Tier15Block1Price = 0xF0 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block2 Price</summary>
        Tier15Block2Price = 0xF1 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block3 Price</summary>
        Tier15Block3Price = 0xF2 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block4 Price</summary>
        Tier15Block4Price = 0xF3 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block5 Price</summary>
        Tier15Block5Price = 0xF4 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block6 Price</summary>
        Tier15Block6Price = 0xF5 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block7 Price</summary>
        Tier15Block7Price = 0xF6 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block8 Price</summary>
        Tier15Block8Price = 0xF7 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block9 Price</summary>
        Tier15Block9Price = 0xF8 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block10 Price</summary>
        Tier15Block10Price = 0xF9 | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block11 Price</summary>
        Tier15Block11Price = 0xFA | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block12 Price</summary>
        Tier15Block12Price = 0xFB | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block13 Price</summary>
        Tier15Block13Price = 0xFC | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block14 Price</summary>
        Tier15Block14Price = 0xFD | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block15 Price</summary>
        Tier15Block15Price = 0xFE | (PriceClusterAttributeSets.BlockPriceInformation << 8),
        /// <summary>Tier15 Block16 Price</summary>
        Tier15Block16Price = 0xFF | (PriceClusterAttributeSets.BlockPriceInformation << 8),
    }

    /// <summary>
    /// Billing (Period) Information Attribute Set.
    /// </summary>
    public enum BillingInformationAttributes : ushort
    {
        /// <summary>Current Billing Period Start</summary>
        CurrentBillingPeriodStart = 0x00 | (ushort)(PriceClusterAttributeSets.BillingInformationSet << 8),
        /// <summary>CurrentBillingPeriodDuration</summary>
        CurrentBillingPeriodDuration = 0x001 | (ushort)(PriceClusterAttributeSets.BillingInformationSet << 8),
    }

    /// <summary>
    /// Price Client Cluster Attributes
    /// </summary>
    public enum PriceClientClusterAttributes : ushort
    {
        /// <summary>Price Increase Randomize Minutes</summary>
        PriceIncreaseRandomizeMinutes = 0x0000,
        /// <summary>Price Decrease Randomize Minutes</summary>
        PriceDecreaseRandomizeMinutes = 0x0001,
        /// <summary>Commodity Type</summary>
        CommodityType = 0x0002,
    }

    /// <summary>
    /// Unit of Measure values as defined in the ZigBee Smart Energy 
    /// profile spec.
    /// </summary>
    public enum UnitOfMeasure : byte
    {
        /// <summary>
        /// kW &amp; kWh in pure Binary format.
        /// </summary>
        [EnumDescription("kW & kWh")]
        KiloWattsBinary = 0,
        /// <summary>
        /// m³ &amp; m³/h in pure Binary format.
        /// </summary>
        [EnumDescription(@"m³ & m³/h")]
        MetersCubedBinary = 1,
        /// <summary>
        /// ft³ &amp; ft³/h in pure Binary format.
        /// </summary>
        [EnumDescription(@"ft³ & ft³/h")]
        FeetCubedBinary = 2,
        /// <summary>
        /// ccf &amp; ccf/h in pure Binary format.
        /// </summary>
        [EnumDescription(@"ccf & ccf/h")]
        HundredCubicFeetBinary = 3,
        /// <summary>
        /// US gl &amp; US gl/h in pure Binary format.
        /// </summary>
        [EnumDescription(@"US gl & US gl/h")]
        USGallonsBinary = 4,
        /// <summary>
        /// IMP gl &amp; IMP gl/h in pure Binary format.
        /// </summary>
        [EnumDescription(@"IMP gl & IMP gl/h")]
        ImperialGallonsBinary = 5,
        /// <summary>
        /// BTUs &amp; BTU/h in pure Binary format.
        /// </summary>
        [EnumDescription(@"BTUs & BTU/h")]
        BritishThermalUnitsBinary = 6,
        /// <summary>
        /// Liters &amp; l/h in pure Binary format.
        /// </summary>
        [EnumDescription(@"Liters & l/h")]
        LitersBinary = 7,
        /// <summary>
        /// kPA(gauge) in pure Binary format.
        /// </summary>
        [EnumDescription(@"Gauge (kPA)")]
        KiloPascalsGaugeBinary = 8,
        /// <summary>
        /// kPA(absolute) in pure Binary format.
        /// </summary>
        [EnumDescription(@"Absolute (kPA)")]
        KiloPascalsAbsoluteBinary = 9,
        /// <summary>
        /// kW &amp; kWh in BCD format.
        /// </summary>
        [EnumDescription("kW & kWh")]
        KiloWattsBCD = 128,
        /// <summary>
        /// m³ &amp; m³/h in BCD format.
        /// </summary>
        [EnumDescription(@"m³ & m³/h")]
        MetersCubedBCD = 129,
        /// <summary>
        /// ft³ &amp; ft³/h in BCD format.
        /// </summary>
        [EnumDescription(@"ft³ & ft³/h")]
        FeetCubedBCD = 130,
        /// <summary>
        /// ccf &amp; ccf/h in BCD format.
        /// </summary>
        [EnumDescription(@"ccf & ccf/h")]
        HundredCubicFeetBCD = 131,
        /// <summary>
        /// US gl &amp; US gl/h in BCD format.
        /// </summary>
        [EnumDescription(@"US gl & US gl/h")]
        USGallonsBCD = 132,
        /// <summary>
        /// IMP gl &amp; IMP gl/h in BCD format.
        /// </summary>
        [EnumDescription(@"IMP gl & IMP gl/h")]
        ImperialGallonsBCD = 133,
        /// <summary>
        /// BTUs &amp; BTU/h in BCD format.
        /// </summary>
        [EnumDescription(@"BTUs & BTU/h")]
        BritishThermalUnitsBCD = 134,
        /// <summary>
        /// Liters &amp; l/h in BCD format.
        /// </summary>
        [EnumDescription(@"Liters & l/h")]
        LitersBCD = 135,
        /// <summary>
        /// kPA(gauge) in BCD format.
        /// </summary>
        [EnumDescription(@"Gauge (kPA)")]
        KiloPascalsGaugeBCD = 136,
        /// <summary>
        /// kPA(absolute) in BCD format.
        /// </summary>
        [EnumDescription(@"Absolute (kPA)")]
        KiloPascalsAbsoluteBCD = 137,
        /// <summary>
        /// Undefined.
        /// </summary>
        [EnumDescription("Undefined")]
        Undefined = 255,
    }

    /// <summary>
    /// Currencies defined in ISO 4217.
    /// </summary>
    public enum UnitOfCurrency : ushort
    {
        /// <summary>
        /// UICFranc 
        /// </summary>
        [EnumDescription("XFU")]
        UICFranc = 0,
        /// <summary>
        /// Lek 
        /// </summary>
        [EnumDescription("ALL")]
        Lek = 8,
        /// <summary>
        /// AlgerianDinar 
        /// </summary>
        [EnumDescription("DZD")]
        AlgerianDinar = 12,
        /// <summary>
        /// ArgentinePeso 
        /// </summary>
        [EnumDescription("ARS")]
        ArgentinePeso = 32,
        /// <summary>
        /// AustralianDollar 
        /// </summary>
        [EnumDescription("AUD")]
        AustralianDollar = 36,
        /// <summary>
        /// BahamianDollar 
        /// </summary>
        [EnumDescription("BSD")]
        BahamianDollar = 44,
        /// <summary>
        /// BahrainiDinar 
        /// </summary>
        [EnumDescription("BHD")]
        BahrainiDinar = 48,
        /// <summary>
        /// Taka 
        /// </summary>
        [EnumDescription("BDT")]
        Taka = 50,
        /// <summary>
        /// ArmenianDram 
        /// </summary>
        [EnumDescription("AMD")]
        ArmenianDram = 51,
        /// <summary>
        /// BarbadosDollar 
        /// </summary>
        [EnumDescription("BBD")]
        BarbadosDollar = 52,
        /// <summary>
        /// BermudianDollar 
        /// </summary>
        [EnumDescription("BMD")]
        BermudianDollar = 60,
        /// <summary>
        /// Ngultrum 
        /// </summary>
        [EnumDescription("BTN")]
        Ngultrum = 64,
        /// <summary>
        /// Boliviano 
        /// </summary>
        [EnumDescription("BOB")]
        Boliviano = 68,
        /// <summary>
        /// Pula 
        /// </summary>
        [EnumDescription("BWP")]
        Pula = 72,
        /// <summary>
        /// BelizeDollar 
        /// </summary>
        [EnumDescription("BZD")]
        BelizeDollar = 84,
        /// <summary>
        /// SolomonIslandsDollar 
        /// </summary>
        [EnumDescription("SBD")]
        SolomonIslandsDollar = 90,
        /// <summary>
        /// BruneiDollar 
        /// </summary>
        [EnumDescription("BND")]
        BruneiDollar = 96,
        /// <summary>
        /// Kyat 
        /// </summary>
        [EnumDescription("MMK")]
        Kyat = 104,
        /// <summary>
        /// BurundiFranc 
        /// </summary>
        [EnumDescription("BIF")]
        BurundiFranc = 108,
        /// <summary>
        /// Riel 
        /// </summary>
        [EnumDescription("KHR")]
        Riel = 116,
        /// <summary>
        /// CanadianDollar 
        /// </summary>
        [EnumDescription("CAD")]
        CanadianDollar = 124,
        /// <summary>
        /// CapeVerdeEscudo 
        /// </summary>
        [EnumDescription("CVE")]
        CapeVerdeEscudo = 132,
        /// <summary>
        /// CaymanIslandsDollar 
        /// </summary>
        [EnumDescription("KYD")]
        CaymanIslandsDollar = 136,
        /// <summary>
        /// SriLankaRupee 
        /// </summary>
        [EnumDescription("LKR")]
        SriLankaRupee = 144,
        /// <summary>
        /// ChileanPeso 
        /// </summary>
        [EnumDescription("CLP")]
        ChileanPeso = 152,
        /// <summary>
        /// YuanRenminbi 
        /// </summary>
        [EnumDescription("CNY")]
        YuanRenminbi = 156,
        /// <summary>
        /// ColombianPeso 
        /// </summary>
        [EnumDescription("COP")]
        ColombianPeso = 170,
        /// <summary>
        /// ComoroFranc 
        /// </summary>
        [EnumDescription("KMF")]
        ComoroFranc = 174,
        /// <summary>
        /// CostaRicanColon 
        /// </summary>
        [EnumDescription("CRC")]
        CostaRicanColon = 188,
        /// <summary>
        /// CroatianKuna 
        /// </summary>
        [EnumDescription("HRK")]
        CroatianKuna = 191,
        /// <summary>
        /// CubanPeso 
        /// </summary>
        [EnumDescription("CUP")]
        CubanPeso = 192,
        /// <summary>
        /// CzechKoruna 
        /// </summary>
        [EnumDescription("CZK")]
        CzechKoruna = 203,
        /// <summary>
        /// DanishKrone 
        /// </summary>
        [EnumDescription("DKK")]
        DanishKrone = 208,
        /// <summary>
        /// DominicanPeso 
        /// </summary>
        [EnumDescription("DOP")]
        DominicanPeso = 214,
        /// <summary>
        /// ElSalvadorColon 
        /// </summary>
        [EnumDescription("SVC")]
        ElSalvadorColon = 222,
        /// <summary>
        /// EthiopianBirr 
        /// </summary>
        [EnumDescription("ETB")]
        EthiopianBirr = 230,
        /// <summary>
        /// Nakfa 
        /// </summary>
        [EnumDescription("ERN")]
        Nakfa = 232,
        /// <summary>
        /// Kroon 
        /// </summary>
        [EnumDescription("EEK")]
        Kroon = 233,
        /// <summary>
        /// FalklandIslandsPound 
        /// </summary>
        [EnumDescription("FKP")]
        FalklandIslandsPound = 238,
        /// <summary>
        /// FijiDollar 
        /// </summary>
        [EnumDescription("FJD")]
        FijiDollar = 242,
        /// <summary>
        /// DjiboutiFranc 
        /// </summary>
        [EnumDescription("DJF")]
        DjiboutiFranc = 262,
        /// <summary>
        /// Dalasi 
        /// </summary>
        [EnumDescription("GMD")]
        Dalasi = 270,
        /// <summary>
        /// GibraltarPound 
        /// </summary>
        [EnumDescription("GIP")]
        GibraltarPound = 292,
        /// <summary>
        /// Quetzal 
        /// </summary>
        [EnumDescription("GTQ")]
        Quetzal = 320,
        /// <summary>
        /// GuineaFranc 
        /// </summary>
        [EnumDescription("GNF")]
        GuineaFranc = 324,
        /// <summary>
        /// GuyanaDollar 
        /// </summary>
        [EnumDescription("GYD")]
        GuyanaDollar = 328,
        /// <summary>
        /// Gourde 
        /// </summary>
        [EnumDescription("HTG")]
        Gourde = 332,
        /// <summary>
        /// Lempira 
        /// </summary>
        [EnumDescription("HNL")]
        Lempira = 340,
        /// <summary>
        /// HongKongDollar 
        /// </summary>
        [EnumDescription("HKD")]
        HongKongDollar = 344,
        /// <summary>
        /// Forint 
        /// </summary>
        [EnumDescription("HUF")]
        Forint = 348,
        /// <summary>
        /// IcelandKrona 
        /// </summary>
        [EnumDescription("ISK")]
        IcelandKrona = 352,
        /// <summary>
        /// IndianRupee 
        /// </summary>
        [EnumDescription("INR")]
        IndianRupee = 356,
        /// <summary>
        /// Rupiah 
        /// </summary>
        [EnumDescription("IDR")]
        Rupiah = 360,
        /// <summary>
        /// IranianRial 
        /// </summary>
        [EnumDescription("IRR")]
        IranianRial = 364,
        /// <summary>
        /// IraqiDinar 
        /// </summary>
        [EnumDescription("IQD")]
        IraqiDinar = 368,
        /// <summary>
        /// NewIsraeliSheqel 
        /// </summary>
        [EnumDescription("ILS")]
        NewIsraeliSheqel = 376,
        /// <summary>
        /// JamaicanDollar 
        /// </summary>
        [EnumDescription("JMD")]
        JamaicanDollar = 388,
        /// <summary>
        /// Yen 
        /// </summary>
        [EnumDescription("JPY")]
        Yen = 392,
        /// <summary>
        /// Tenge 
        /// </summary>
        [EnumDescription("KZT")]
        Tenge = 398,
        /// <summary>
        /// JordanianDinar 
        /// </summary>
        [EnumDescription("JOD")]
        JordanianDinar = 400,
        /// <summary>
        /// KenyanShilling 
        /// </summary>
        [EnumDescription("KES")]
        KenyanShilling = 404,
        /// <summary>
        /// NorthKoreanWon 
        /// </summary>
        [EnumDescription("KPW")]
        NorthKoreanWon = 408,
        /// <summary>
        /// Won 
        /// </summary>
        [EnumDescription("KRW")]
        Won = 410,
        /// <summary>
        /// KuwaitiDinar 
        /// </summary>
        [EnumDescription("KWD")]
        KuwaitiDinar = 414,
        /// <summary>
        /// Som 
        /// </summary>
        [EnumDescription("KGS")]
        Som = 417,
        /// <summary>
        /// Kip 
        /// </summary>
        [EnumDescription("LAK")]
        Kip = 418,
        /// <summary>
        /// LebanesePound 
        /// </summary>
        [EnumDescription("LBP")]
        LebanesePound = 422,
        /// <summary>
        /// Loti 
        /// </summary>
        [EnumDescription("LSL")]
        Loti = 426,
        /// <summary>
        /// LatvianLats 
        /// </summary>
        [EnumDescription("LVL")]
        LatvianLats = 428,
        /// <summary>
        /// LiberianDollar 
        /// </summary>
        [EnumDescription("LRD")]
        LiberianDollar = 430,
        /// <summary>
        /// LibyanDinar 
        /// </summary>
        [EnumDescription("LYD")]
        LibyanDinar = 434,
        /// <summary>
        /// LithuanianLitas 
        /// </summary>
        [EnumDescription("LTL")]
        LithuanianLitas = 440,
        /// <summary>
        /// Pataca 
        /// </summary>
        [EnumDescription("MOP")]
        Pataca = 446,
        /// <summary>
        /// Kwacha 
        /// </summary>
        [EnumDescription("MWK")]
        Kwacha = 454,
        /// <summary>
        /// MalaysianRinggit 
        /// </summary>
        [EnumDescription("MYR")]
        MalaysianRinggit = 458,
        /// <summary>
        /// Rufiyaa 
        /// </summary>
        [EnumDescription("MVR")]
        Rufiyaa = 462,
        /// <summary>
        /// Ouguiya 
        /// </summary>
        [EnumDescription("MRO")]
        Ouguiya = 478,
        /// <summary>
        /// MauritiusRupee 
        /// </summary>
        [EnumDescription("MUR")]
        MauritiusRupee = 480,
        /// <summary>
        /// MexicanPeso 
        /// </summary>
        [EnumDescription("MXN")]
        MexicanPeso = 484,
        /// <summary>
        /// Tugrik 
        /// </summary>
        [EnumDescription("MNT")]
        Tugrik = 496,
        /// <summary>
        /// MoldovanLeu 
        /// </summary>
        [EnumDescription("MDL")]
        MoldovanLeu = 498,
        /// <summary>
        /// MoroccanDirham 
        /// </summary>
        [EnumDescription("MAD")]
        MoroccanDirham = 504,
        /// <summary>
        /// RialOmani 
        /// </summary>
        [EnumDescription("OMR")]
        RialOmani = 512,
        /// <summary>
        /// NamibiaDollar 
        /// </summary>
        [EnumDescription("NAD")]
        NamibiaDollar = 516,
        /// <summary>
        /// NepaleseRupee 
        /// </summary>
        [EnumDescription("NPR")]
        NepaleseRupee = 524,
        /// <summary>
        /// NetherlandsAntillianGuilder 
        /// </summary>
        [EnumDescription("ANG")]
        NetherlandsAntillianGuilder = 532,
        /// <summary>
        /// ArubanGuilder 
        /// </summary>
        [EnumDescription("AWG")]
        ArubanGuilder = 533,
        /// <summary>
        /// Vatu 
        /// </summary>
        [EnumDescription("VUV")]
        Vatu = 548,
        /// <summary>
        /// NewZealandDollar 
        /// </summary>
        [EnumDescription("NZD")]
        NewZealandDollar = 554,
        /// <summary>
        /// CordobaOro 
        /// </summary>
        [EnumDescription("NIO")]
        CordobaOro = 558,
        /// <summary>
        /// Naira 
        /// </summary>
        [EnumDescription("NGN")]
        Naira = 566,
        /// <summary>
        /// NorwegianKrone 
        /// </summary>
        [EnumDescription("NOK")]
        NorwegianKrone = 578,
        /// <summary>
        /// PakistanRupee 
        /// </summary>
        [EnumDescription("PKR")]
        PakistanRupee = 586,
        /// <summary>
        /// Balboa 
        /// </summary>
        [EnumDescription("PAB")]
        Balboa = 590,
        /// <summary>
        /// Kina 
        /// </summary>
        [EnumDescription("PGK")]
        Kina = 598,
        /// <summary>
        /// Guarani 
        /// </summary>
        [EnumDescription("PYG")]
        Guarani = 600,
        /// <summary>
        /// NuevoSol 
        /// </summary>
        [EnumDescription("PEN")]
        NuevoSol = 604,
        /// <summary>
        /// PhilippinePeso 
        /// </summary>
        [EnumDescription("PHP")]
        PhilippinePeso = 608,
        /// <summary>
        /// GuineaBissauPeso 
        /// </summary>
        [EnumDescription("GWP")]
        GuineaBissauPeso = 624,
        /// <summary>
        /// QatariRial 
        /// </summary>
        [EnumDescription("QAR")]
        QatariRial = 634,
        /// <summary>
        /// RussianRuble 
        /// </summary>
        [EnumDescription("RUB")]
        RussianRuble = 643,
        /// <summary>
        /// RwandaFranc 
        /// </summary>
        [EnumDescription("RWF")]
        RwandaFranc = 646,
        /// <summary>
        /// SaintHelenaPound 
        /// </summary>
        [EnumDescription("SHP")]
        SaintHelenaPound = 654,
        /// <summary>
        /// Dobra 
        /// </summary>
        [EnumDescription("STD")]
        Dobra = 678,
        /// <summary>
        /// SaudiRiyal 
        /// </summary>
        [EnumDescription("SAR")]
        SaudiRiyal = 682,
        /// <summary>
        /// SeychellesRupee 
        /// </summary>
        [EnumDescription("SCR")]
        SeychellesRupee = 690,
        /// <summary>
        /// Leone 
        /// </summary>
        [EnumDescription("SLL")]
        Leone = 694,
        /// <summary>
        /// SingaporeDollar 
        /// </summary>
        [EnumDescription("SGD")]
        SingaporeDollar = 702,
        /// <summary>
        /// Dong 
        /// </summary>
        [EnumDescription("VND")]
        Dong = 704,
        /// <summary>
        /// SomaliShilling 
        /// </summary>
        [EnumDescription("SOS")]
        SomaliShilling = 706,
        /// <summary>
        /// Rand 
        /// </summary>
        [EnumDescription("ZAR")]
        Rand = 710,
        /// <summary>
        /// Lilangeni 
        /// </summary>
        [EnumDescription("SZL")]
        Lilangeni = 748,
        /// <summary>
        /// SwedishKrona 
        /// </summary>
        [EnumDescription("SEK")]
        SwedishKrona = 752,
        /// <summary>
        /// SwissFranc 
        /// </summary>
        [EnumDescription("CHF")]
        SwissFranc = 756,
        /// <summary>
        /// SyrianPound 
        /// </summary>
        [EnumDescription("SYP")]
        SyrianPound = 760,
        /// <summary>
        /// Baht 
        /// </summary>
        [EnumDescription("THB")]
        Baht = 764,
        /// <summary>
        /// Paanga 
        /// </summary>
        [EnumDescription("TOP")]
        Paanga = 776,
        /// <summary>
        /// TrinidadTobagoDollar 
        /// </summary>
        [EnumDescription("TTD")]
        TrinidadTobagoDollar = 780,
        /// <summary>
        /// UAEDirham 
        /// </summary>
        [EnumDescription("AED")]
        UAEDirham = 784,
        /// <summary>
        /// TunisianDinar 
        /// </summary>
        [EnumDescription("TND")]
        TunisianDinar = 788,
        /// <summary>
        /// UgandaShilling 
        /// </summary>
        [EnumDescription("UGX")]
        UgandaShilling = 800,
        /// <summary>
        /// Denar 
        /// </summary>
        [EnumDescription("MKD")]
        Denar = 807,
        /// <summary>
        /// EgyptianPound 
        /// </summary>
        [EnumDescription("EGP")]
        EgyptianPound = 818,
        /// <summary>
        /// PoundSterling 
        /// </summary>
        [EnumDescription("GBP")]
        PoundSterling = 826,
        /// <summary>
        /// TanzanianShilling 
        /// </summary>
        [EnumDescription("TZS")]
        TanzanianShilling = 834,
        /// <summary>
        /// USDollar 
        /// </summary>
        [EnumDescription("USD")]
        USDollar = 840,
        /// <summary>
        /// PesoUruguayo 
        /// </summary>
        [EnumDescription("UYU")]
        PesoUruguayo = 858,
        /// <summary>
        /// UzbekistanSum 
        /// </summary>
        [EnumDescription("UZS")]
        UzbekistanSum = 860,
        /// <summary>
        /// Tala 
        /// </summary>
        [EnumDescription("WST")]
        Tala = 882,
        /// <summary>
        /// YemeniRial 
        /// </summary>
        [EnumDescription("YER")]
        YemeniRial = 886,
        /// <summary>
        /// ZambianKwacha 
        /// </summary>
        [EnumDescription("ZMK")]
        ZambianKwacha = 894,
        /// <summary>
        /// NewTaiwanDollar 
        /// </summary>
        [EnumDescription("TWD")]
        NewTaiwanDollar = 901,
        /// <summary>
        /// PesoConvertible 
        /// </summary>
        [EnumDescription("CUC")]
        PesoConvertible = 931,
        /// <summary>
        /// ZimbabweDollar 
        /// </summary>
        [EnumDescription("ZWL")]
        ZimbabweDollar = 932,
        /// <summary>
        /// Manat 
        /// </summary>
        [EnumDescription("TMT")]
        Manat = 934,
        /// <summary>
        /// Cedi 
        /// </summary>
        [EnumDescription("GHS")]
        Cedi = 936,
        /// <summary>
        /// BolivarFuerte 
        /// </summary>
        [EnumDescription("VEF")]
        BolivarFuerte = 937,
        /// <summary>
        /// SudanesePound 
        /// </summary>
        [EnumDescription("SDG")]
        SudanesePound = 938,
        /// <summary>
        /// UruguayPeso 
        /// </summary>
        [EnumDescription("UYI")]
        UruguayPeso = 940,
        /// <summary>
        /// SerbianDinar 
        /// </summary>
        [EnumDescription("RSD")]
        SerbianDinar = 941,
        /// <summary>
        /// Metical 
        /// </summary>
        [EnumDescription("MZN")]
        Metical = 943,
        /// <summary>
        /// AzerbaijanianManat 
        /// </summary>
        [EnumDescription("AZN")]
        AzerbaijanianManat = 944,
        /// <summary>
        /// NewLeu 
        /// </summary>
        [EnumDescription("RON")]
        NewLeu = 946,
        /// <summary>
        /// WIREuro 
        /// </summary>
        [EnumDescription("CHE")]
        WIREuro = 947,
        /// <summary>
        /// WIRFranc 
        /// </summary>
        [EnumDescription("CHW")]
        WIRFranc = 948,
        /// <summary>
        /// TurkishLira 
        /// </summary>
        [EnumDescription("TRY")]
        TurkishLira = 949,
        /// <summary>
        /// CFAFrancBEAC 
        /// </summary>
        [EnumDescription("XAF")]
        CFAFrancBEAC = 950,
        /// <summary>
        /// EastCaribbeanDollar 
        /// </summary>
        [EnumDescription("XCD")]
        EastCaribbeanDollar = 951,
        /// <summary>
        /// CFAFrancBCEAO 
        /// </summary>
        [EnumDescription("XOF")]
        CFAFrancBCEAO = 952,
        /// <summary>
        /// CFPFranc 
        /// </summary>
        [EnumDescription("XPF")]
        CFPFranc = 953,
        /// <summary>
        /// EuropeanCompositeUnit 
        /// </summary>
        [EnumDescription("XBA")]
        EuropeanCompositeUnit = 955,
        /// <summary>
        /// EuropeanMonetaryUnit 
        /// </summary>
        [EnumDescription("XBB")]
        EuropeanMonetaryUnit = 956,
        /// <summary>
        /// EuropeanUnitOfAccount9 
        /// </summary>
        [EnumDescription("XBC")]
        EuropeanUnitOfAccount9 = 957,
        /// <summary>
        /// EuropeanUnitOfAccount17 
        /// </summary>
        [EnumDescription("XBD")]
        EuropeanUnitOfAccount17 = 958,
        /// <summary>
        /// Gold 
        /// </summary>
        [EnumDescription("XAU")]
        Gold = 959,
        /// <summary>
        /// SDR 
        /// </summary>
        [EnumDescription("XDR")]
        SDR = 960,
        /// <summary>
        /// Silver 
        /// </summary>
        [EnumDescription("XAG")]
        Silver = 961,
        /// <summary>
        /// Platinum 
        /// </summary>
        [EnumDescription("XPT")]
        Platinum = 962,
        /// <summary>
        /// TestCurrency 
        /// </summary>
        [EnumDescription("XTS")]
        TestCurrency = 963,
        /// <summary>
        /// Palladium 
        /// </summary>
        [EnumDescription("XPD")]
        Palladium = 964,
        /// <summary>
        /// SurinamDollar 
        /// </summary>
        [EnumDescription("SRD")]
        SurinamDollar = 968,
        /// <summary>
        /// MalagasyAriary 
        /// </summary>
        [EnumDescription("MGA")]
        MalagasyAriary = 969,
        /// <summary>
        /// UnidadDeValorReal 
        /// </summary>
        [EnumDescription("COU")]
        UnidadDeValorReal = 970,
        /// <summary>
        /// Afghani  
        /// </summary>
        [EnumDescription("AFN")]
        Afghani = 971,
        /// <summary>
        /// Somoni 
        /// </summary>
        [EnumDescription("TJS")]
        Somoni = 972,
        /// <summary>
        /// Kwanza 
        /// </summary>
        [EnumDescription("AOA")]
        Kwanza = 973,
        /// <summary>
        /// BelarussianRuble 
        /// </summary>
        [EnumDescription("BYR")]
        BelarussianRuble = 974,
        /// <summary>
        /// BulgarianLev 
        /// </summary>
        [EnumDescription("BGN")]
        BulgarianLev = 975,
        /// <summary>
        /// CongoleseFranc 
        /// </summary>
        [EnumDescription("CDF")]
        CongoleseFranc = 976,
        /// <summary>
        /// ConvertibleMarks 
        /// </summary>
        [EnumDescription("BAM")]
        ConvertibleMarks = 977,
        /// <summary>
        /// Euro 
        /// </summary>
        [EnumDescription("EUR")]
        Euro = 978,
        /// <summary>
        /// MexicanUnidadDeInversion 
        /// </summary>
        [EnumDescription("MXV")]
        MexicanUnidadDeInversion = 979,
        /// <summary>
        /// Hryvnia 
        /// </summary>
        [EnumDescription("UAH")]
        Hryvnia = 980,
        /// <summary>
        /// Lari 
        /// </summary>
        [EnumDescription("GEL")]
        Lari = 981,
        /// <summary>
        /// Mvdol 
        /// </summary>
        [EnumDescription("BOV")]
        Mvdol = 984,
        /// <summary>
        /// Zloty 
        /// </summary>
        [EnumDescription("PLN")]
        Zloty = 985,
        /// <summary>
        /// BrazilianReal 
        /// </summary>
        [EnumDescription("BRL")]
        BrazilianReal = 986,
        /// <summary>
        /// UnidadesDeFomento 
        /// </summary>
        [EnumDescription("CLF")]
        UnidadesDeFomento = 990,
        /// <summary>
        /// USDollarNextDay 
        /// </summary>
        [EnumDescription("USN")]
        USDollarNextDay = 997,
        /// <summary>
        /// USDollarSameDay 
        /// </summary>
        [EnumDescription("USS")]
        USDollarSameDay = 998,
        /// <summary>
        /// NoCurrency 
        /// </summary>
        [EnumDescription("XXX")]
        NoCurrency = 999,
        /// <summary>
        /// UndefinedCurrency 
        /// </summary>
        [EnumDescription("Undefined")]
        UndefinedCurrency = 65535,
    }

    #endregion

    /// <summary>
    /// Class that specifies the Smart Energy Price Cluster Publish Price Payload
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  04/02/12 PGH 2.60.13        Created
    //
    public class PublishPriceRcd
    {
        #region Constants

        private const byte PRICE_ACKNOWLEDGEMENT_REQUIRED = 0x01;

        private byte PRICE_TRAILING_DIGIT_MASK = 0xF0;
        private byte PRICE_TIER_MASK = 0x0F;
        private byte PRICE_TRAILING_DIGIT_SHIFT = 4;
        private byte NBR_PRICE_TIERS_MASK = 0xF0;
        private byte REGISTER_TIER_MASK = 0x0F;
        private byte NBR_PRICE_TIERS_SHIFT = 4;

        #endregion

        #region Definitions

        /// <summary>
        /// Tier Price Label Reference
        /// </summary>
        public enum TierPriceLabelReference : byte
        {
            /// <summary>No Tier Related</summary>
            NoTierRelated = 0x0,
            /// <summary>Reference Tier1 Price Label</summary>
            Tier1PriceLabel = 0x1,
            /// <summary>Reference Tier2 Price Label</summary>
            Tier2PriceLabel = 0x2,
            /// <summary>Reference Tier3 Price Label</summary>
            Tier3PriceLabel = 0x3,
            /// <summary>Reference Tier4 Price Label</summary>
            Tier4PriceLabel = 0x4,
            /// <summary>Reference Tier5 Price Label</summary>
            Tier5PriceLabel = 0x5,
            /// <summary>Reference Tier6 Price Label</summary>
            Tier6PriceLabel = 0x6,
            /// <summary>Reference Tier7 Price Label</summary>
            Tier7PriceLabel = 0x7,
            /// <summary>Reference Tier8 Price Label</summary>
            Tier8PriceLabel = 0x8,
            /// <summary>Reference Tier9 Price Label</summary>
            Tier9PriceLabel = 0x9,
            /// <summary>Reference Tier10 Price Label</summary>
            Tier10PriceLabel = 0x10,
            /// <summary>Reference Tier11 Price Label</summary>
            Tier11PriceLabel = 0x11,
            /// <summary>Reference Tier12 Price Label</summary>
            Tier12PriceLabel = 0x12,
            /// <summary>Reference Tier13 Price Label</summary>
            Tier13PriceLabel = 0x13,
            /// <summary>Reference Tier14 Price Label</summary>
            Tier14PriceLabel = 0x14,
            /// <summary>Reference Tier15 Price Label</summary>
            Tier15PriceLabel = 0x15,
        }

        /// <summary>
        /// Register Tier Reference
        /// </summary>
        public enum RegisterTierReference : byte
        {
            /// <summary>No Tier Related</summary>
            NoTierRelated = 0x0,
            /// <summary>Accumulating in CurrentTier1SummationDelivered attribute</summary>
            CurrentTier1SummationDelivered = 0x1,
            /// <summary>Accumulating in CurrentTier2SummationDelivered attribute</summary>
            CurrentTier2SummationDelivered = 0x2,
            /// <summary>Accumulating in CurrentTier3SummationDelivered attribute</summary>
            CurrentTier3SummationDelivered = 0x3,
            /// <summary>Accumulating in CurrentTier4SummationDelivered attribute</summary>
            CurrentTier4SummationDelivered = 0x4,
            /// <summary>Accumulating in CurrentTier5SummationDelivered attribute</summary>
            CurrentTier5SummationDelivered = 0x5,
            /// <summary>Accumulating in CurrentTier6SummationDelivered attribute</summary>
            CurrentTier6SummationDelivered = 0x6,
            /// <summary>Accumulating in CurrentTier7SummationDelivered attribute</summary>
            CurrentTier7SummationDelivered = 0x7,
            /// <summary>Accumulating in CurrentTier8SummationDelivered attribute</summary>
            CurrentTier8SummationDelivered = 0x8,
            /// <summary>Accumulating in CurrentTier9SummationDelivered attribute</summary>
            CurrentTier9SummationDelivered = 0x9,
            /// <summary>Accumulating in CurrentTier10SummationDelivered attribute</summary>
            CurrentTier10SummationDelivered = 0xA,
            /// <summary>Accumulating in CurrentTier11SummationDelivered attribute</summary>
            CurrentTier11SummationDelivered = 0xB,
            /// <summary>Accumulating in CurrentTier12SummationDelivered attribute</summary>
            CurrentTier12SummationDelivered = 0xC,
            /// <summary>Accumulating in CurrentTier13SummationDelivered attribute</summary>
            CurrentTier13SummationDelivered = 0xD,
            /// <summary>Accumulating in CurrentTier14SummationDelivered attribute</summary>
            CurrentTier14SummationDelivered = 0xE,
            /// <summary>Accumulating in CurrentTier15SummationDelivered attribute</summary>
            CurrentTier15SummationDelivered = 0xF,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        //
        public PublishPriceRcd()
        {
        }

        /// <summary>
        /// Get Most Significant Nibble
        /// </summary>
        /// <param name="most">byte</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/04/12 PGH 2.60.13        Created
        public byte GetMostSignificantNibble(byte most)
        {
            return ((byte)(most >> 4));
        }

        /// <summary>
        /// Get Least Significant Nibble
        /// </summary>
        /// <param name="least">byte</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/04/12 PGH 2.60.13        Created
        public byte GetLeastSignificantNibble(byte least)
        {
            least = (byte)(least << 4);
            return ((byte)(least >> 4));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Provider ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
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
        /// Rate Label
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public string RateLabel
        {
            get
            {
                return m_RateLabel;
            }
            set
            {
                m_RateLabel = value;
            }
        }

        /// <summary>
        /// Issuer Event ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public uint IssuerEventId
        {
            get
            {
                return m_IssuerEventId;
            }
            set
            {
                m_IssuerEventId = value;
            }
        }

        /// <summary>
        /// Current Time (UTC)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public DateTime CurrentTime
        {
            get
            {
                return m_CurrentTime;
            }
            set
            {
                m_CurrentTime = value;
            }
        }

        /// <summary>
        /// Unit of Measure
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public byte UnitOfMeasure
        {
            get
            {
                return m_UnitOfMeasure;
            }
            set
            {
                m_UnitOfMeasure = value;
            }
        }

        /// <summary>
        /// Currency
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public ushort Currency
        {
            get
            {
                return m_Currency;
            }
            set
            {
                m_Currency = value;
            }
        }

        /// <summary>
        /// Price Trailing Digit and Price Tier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public byte PriceTrailingDigitAndPriceTier
        {
            get
            {
                return m_PriceTrailingDigitAndPriceTier;
            }
            set
            {
                m_PriceTrailingDigitAndPriceTier = value;
            }
        }

        /// <summary>
        /// Number of Price Tiers and Register Tier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public byte NumberOfPriceTiersAndRegisterTier
        {
            get
            {
                return m_NumberOfPriceTiersAndRegisterTier;
            }
            set
            {
                m_NumberOfPriceTiersAndRegisterTier = value;
            }
        }

        /// <summary>
        /// Price trailing digit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/12 RCG 2.60.24        Created

        public byte PriceTrailingDigit
        {
            get
            {
                return (byte)((m_PriceTrailingDigitAndPriceTier & PRICE_TRAILING_DIGIT_MASK) >> PRICE_TRAILING_DIGIT_SHIFT);
            }
            set
            {
                m_PriceTrailingDigitAndPriceTier = (byte)((value << PRICE_TRAILING_DIGIT_SHIFT) | (~PRICE_TRAILING_DIGIT_MASK & m_PriceTrailingDigitAndPriceTier));
            }
        }

        /// <summary>
        /// Price tier.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/12 RCG 2.60.24        Created

        public byte PriceTier
        {
            get
            {
                return (byte)(m_PriceTrailingDigitAndPriceTier & PRICE_TIER_MASK);
            }
            set
            {
                m_PriceTrailingDigitAndPriceTier = (byte)((value & PRICE_TIER_MASK) | (~PRICE_TIER_MASK & m_PriceTrailingDigitAndPriceTier));
            }
        }

        /// <summary>
        /// Price trailing digit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/12 RCG 2.60.24        Created

        public byte NumberOfPriceTiers
        {
            get
            {
                return (byte)((m_NumberOfPriceTiersAndRegisterTier & NBR_PRICE_TIERS_MASK) >> NBR_PRICE_TIERS_SHIFT);
            }
            set
            {
                m_NumberOfPriceTiersAndRegisterTier = (byte)((value << NBR_PRICE_TIERS_SHIFT) | (~NBR_PRICE_TIERS_MASK & m_NumberOfPriceTiersAndRegisterTier));
            }
        }

        /// <summary>
        /// Price tier.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/12 RCG 2.60.24        Created

        public byte RegisterTier
        {
            get
            {
                return (byte)(m_NumberOfPriceTiersAndRegisterTier & REGISTER_TIER_MASK);
            }
            set
            {
                m_NumberOfPriceTiersAndRegisterTier = (byte)((value & REGISTER_TIER_MASK) | (~REGISTER_TIER_MASK & m_NumberOfPriceTiersAndRegisterTier));
            }

        }

        /// <summary>
        /// Start Time (UTC)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public DateTime StartTime
        {
            get
            {
                return m_StartTime;
            }
            set
            {
                m_StartTime = value;
            }
        }

        /// <summary>
        /// Duration (Minutes)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public TimeSpan Duration
        {
            get
            {
                return m_Duration;
            }
            set
            {
                m_Duration = value;
            }
        }

        /// <summary>
        /// Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public uint Price
        {
            get
            {
                return m_Price;
            }
            set
            {
                m_Price = value;
            }
        }

        /// <summary>
        /// Price Ratio
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public byte PriceRatio
        {
            get
            {
                return m_PriceRatio;
            }
            set
            {
                m_PriceRatio = value;
            }
        }

        /// <summary>
        /// Generation Price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public uint GenerationPrice
        {
            get
            {
                return m_GenerationPrice;
            }
            set
            {
                m_GenerationPrice = value;
            }
        }

        /// <summary>
        /// Generation Price Ratio
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public byte GenerationPriceRatio
        {
            get
            {
                return m_GenerationPriceRatio;
            }
            set
            {
                m_GenerationPriceRatio = value;
            }
        }

        /// <summary>
        /// Alternate Cost Delivered
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public uint AlternateCostDelivered
        {
            get
            {
                return m_AlternateCostDelivered;
            }
            set
            {
                m_AlternateCostDelivered = value;
            }
        }

        /// <summary>
        /// Alternate Cost Unit
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public byte AlternateCostUnit
        {
            get
            {
                return m_AlternateCostUnit;
            }
            set
            {
                m_AlternateCostUnit = value;
            }
        }

        /// <summary>
        /// Alternate Cost Trailing Digit
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public byte AlternateCostTrailingDigit
        {
            get
            {
                return m_AlternateCostTrailingDigit;
            }
            set
            {
                m_AlternateCostTrailingDigit = value;
            }
        }

        /// <summary>
        /// Number of Block Thresholds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public byte NumberOfBlockThresholds
        {
            get
            {
                return m_NumberOfBlockThresholds;
            }
            set
            {
                m_NumberOfBlockThresholds = value;
            }
        }

        /// <summary>
        /// Price Control
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public byte PriceControl
        {
            get
            {
                return m_PriceControl;
            }
            set
            {
                m_PriceControl = value;
            }
        }

        /// <summary>
        /// Acknowledgement Required
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        public bool IsAcknowledgementRequired
        {
            get
            {
                return (m_PriceControl & PRICE_ACKNOWLEDGEMENT_REQUIRED) == PRICE_ACKNOWLEDGEMENT_REQUIRED;
            }
            set
            {
                if (value)
                    m_PriceControl = (byte)(m_PriceControl | PRICE_ACKNOWLEDGEMENT_REQUIRED);
                else
                    m_PriceControl = (byte)(m_PriceControl & ~PRICE_ACKNOWLEDGEMENT_REQUIRED);

            }
        }

        #endregion

        #region Members

        private uint m_ProviderId;
        private string m_RateLabel;
        private uint m_IssuerEventId;
        private DateTime m_CurrentTime;
        private byte m_UnitOfMeasure;
        private ushort m_Currency;
        private byte m_PriceTrailingDigitAndPriceTier;
        private byte m_NumberOfPriceTiersAndRegisterTier;
        private DateTime m_StartTime;
        private TimeSpan m_Duration;
        private uint m_Price;
        private byte m_PriceRatio;
        private uint m_GenerationPrice;
        private byte m_GenerationPriceRatio;
        private uint m_AlternateCostDelivered;
        private byte m_AlternateCostUnit;
        private byte m_AlternateCostTrailingDigit;
        private byte m_NumberOfBlockThresholds;
        private byte m_PriceControl;

        #endregion
    }

    /// <summary>
    /// Class that specifies the Smart Energy Price Cluster Publish Block Period Payload
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  04/09/12 PGH 2.60.13        Created
    //
    public class PublishBlockPeriodRcd
    {
        #region Constants

        private const byte PRICE_ACKNOWLEDGEMENT_REQUIRED = 0x01;
        private const byte REPEATING_BLOCK = 0x02;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/03/12 PGH 2.60.13        Created
        //
        public PublishBlockPeriodRcd()
        {
        }

        /// <summary>
        /// Get Most Significant Nibble
        /// </summary>
        /// <param name="most">byte</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/04/12 PGH 2.60.13        Created
        public byte GetMostSignificantNibble(byte most)
        {
            return ((byte)(most >> 4));
        }

        /// <summary>
        /// Get Least Significant Nibble
        /// </summary>
        /// <param name="least">byte</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/04/12 PGH 2.60.13        Created
        public byte GetLeastSignificantNibble(byte least)
        {
            least = (byte)(least << 4);
            return ((byte)(least >> 4));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Provider ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/12 PGH 2.60.13        Created
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
        /// Issuer Event ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/12 PGH 2.60.13        Created
        public uint IssuerEventId
        {
            get
            {
                return m_IssuerEventId;
            }
            set
            {
                m_IssuerEventId = value;
            }
        }

        /// <summary>
        /// Start Time (UTC)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/12 PGH 2.60.13        Created
        public DateTime StartTime
        {
            get
            {
                return m_StartTime;
            }
            set
            {
                m_StartTime = value;
            }
        }

        /// <summary>
        /// Duration (Minutes)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/12 PGH 2.60.13        Created
        public TimeSpan Duration
        {
            get
            {
                return m_Duration;
            }
            set
            {
                m_Duration = value;
            }
        }

        /// <summary>
        /// Number of Price Tiers and number of Block Thresholds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/12 PGH 2.60.13        Created
        public byte NumberOfPriceTiersAndNumberOfBlockThresholds
        {
            get
            {
                return m_NumberOfPriceTiersAndNumberOfBlockThresholds;
            }
            set
            {
                m_NumberOfPriceTiersAndNumberOfBlockThresholds = value;
            }
        }

        /// <summary>
        /// Block Period Control
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/12 PGH 2.60.13        Created
        public byte BlockPeriodControl
        {
            get
            {
                return m_BlockPeriodControl;
            }
            set
            {
                m_BlockPeriodControl = value;
            }
        }

        /// <summary>
        /// Acknowledgement Required
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/12 PGH 2.60.13        Created
        public bool IsAcknowledgementRequired
        {
            get
            {
                return (m_BlockPeriodControl & PRICE_ACKNOWLEDGEMENT_REQUIRED) == PRICE_ACKNOWLEDGEMENT_REQUIRED;
            }
            set
            {
                if (value)
                    m_BlockPeriodControl = (byte)(m_BlockPeriodControl | PRICE_ACKNOWLEDGEMENT_REQUIRED);
                else
                    m_BlockPeriodControl = (byte)(m_BlockPeriodControl & ~PRICE_ACKNOWLEDGEMENT_REQUIRED);
            }
        }

        /// <summary>
        /// Is Repeating Block
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/12 PGH 2.60.13        Created
        public bool IsRepeatingBlock
        {
            get
            {
                return (m_BlockPeriodControl & REPEATING_BLOCK) == REPEATING_BLOCK;
            }
            set
            {
                if (value)
                    m_BlockPeriodControl = (byte)(m_BlockPeriodControl | REPEATING_BLOCK);
                else
                    m_BlockPeriodControl = (byte)(m_BlockPeriodControl & ~REPEATING_BLOCK);

            }
        }

        /// <summary>
        /// Size of Publish Block Period Duration field
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/12 PGH 2.60.13        Created
        public int SizeOfDurationField
        {
            get
            {
                return m_SizeOfDurationField;
            }
        }

        #endregion

        #region Members

        private uint m_ProviderId;
        private uint m_IssuerEventId;
        private DateTime m_StartTime;
        private TimeSpan m_Duration;
        private byte m_NumberOfPriceTiersAndNumberOfBlockThresholds;
        private byte m_BlockPeriodControl;
        private int m_SizeOfDurationField = 3;

        #endregion
    }

    
}
