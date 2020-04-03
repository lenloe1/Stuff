using System;
using System.Collections.Generic;
using System.Text;
#if (!WindowsCE)
using System.Runtime.Serialization;
#endif

namespace Itron.Metering.Device
{
    /// <summary>
    /// Display Item class for the OpenWay meter.
    /// </summary>
#if (!WindowsCE)
	[DataContract]
#endif
    public class OpenWayDisplayItem : ANSIDisplayItem
    {

        #region Public Methods

        /// <summary>
        /// Default Constructor.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version  Issue# Description
        // -------- --- -------- ------ ---------------------------------------
        // 04/02/08 RCG 10.00.00        Created

        public OpenWayDisplayItem()
            : this(null, "", 0, 0)
        {
        }

        /// <summary>
        /// Constructor for Display Item that can be called while reading Table 2048
        /// </summary>
        /// <param name="Lid">The LID for the given Display Item</param>
        /// <param name="strDisplayID">The Display ID for the given Display Item</param>
        /// <param name="usFormat">The Format Code for the given display item</param>
        /// <param name="byDim">The Dimension of the given display item</param>
        // Revision History	
        // MM/DD/YY who Version  Issue# Description
        // -------- --- -------- ------ ---------------------------------------
        // 04/02/08 RCG 10.00.00        Created

        public OpenWayDisplayItem(LID Lid, string strDisplayID, ushort usFormat, byte byDim)
            : base(Lid, strDisplayID, usFormat, byDim)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the format as a string for the display item.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version  Issue# Description
        // -------- --- -------- ------ ---------------------------------------
        // 04/02/08 RCG 10.00.00        Created

        public override string Format
        {
            get
            {

                ANSIDisplayItem.DisplayType Format = (ANSIDisplayItem.DisplayType)(DisplayFormat & ANSIDisplayItem.TypeMask);
                byte byTotalDigits = (byte)((DisplayDim & ANSIDisplayItem.TotalDigitsMask) >> 4);
                byte byDecimalDigits = (byte)(DisplayDim & ANSIDisplayItem.DecimalDigitMask);
                string strFormat = "";

                switch (Format)
                {
                    case ANSIDisplayItem.DisplayType.ALL_SEGMENTS:
                    case ANSIDisplayItem.DisplayType.USER_FIELD:
                        {
                            strFormat = "";
                            break;
                        }
                    case ANSIDisplayItem.DisplayType.DATE_DD_MM_YY:
                        {
                            strFormat = "DD/MM/YY";
                            break;
                        }
                    case ANSIDisplayItem.DisplayType.DATE_MM_DD_YY:
                        {
                            strFormat = "MM/DD/YY";
                            break;
                        }
                    case ANSIDisplayItem.DisplayType.DATE_YY_MM_DD:
                        {
                            strFormat = "YY/MM/DD";
                            break;
                        }
                    case ANSIDisplayItem.DisplayType.TIME_HH_MM_SS:
                        {
                            strFormat = "HH:MM:SS";
                            break;
                        }
                    case ANSIDisplayItem.DisplayType.SINGED_INTEGER:
                    case ANSIDisplayItem.DisplayType.UNSIGNED_INTEGER:
                        {
                            for (int iIndex = 0; iIndex < byTotalDigits; iIndex++)
                            {
                                strFormat += "#";
                            }

                            break;
                        }
                    case ANSIDisplayItem.DisplayType.UNSIGNED_INTEGER_LEADING_ZERO:
                        {
                            strFormat = "0";

                            for (int iIndex = 1; iIndex < byTotalDigits; iIndex++)
                            {
                                strFormat += "#";
                            }

                            break;
                        }
                    case ANSIDisplayItem.DisplayType.FLOATING_DECIMAL:
                        {
                            strFormat = "#[.][#] Floating Decimal";
                            break;
                        }
                    case ANSIDisplayItem.DisplayType.FLOATING_DECIMAL_LEADING_ZEROS:
                        {
                            strFormat = "0[.][#] Floating Decimal";
                            break;
                        }
                    case ANSIDisplayItem.DisplayType.DECIMAL:
                    case ANSIDisplayItem.DisplayType.DECIMAL_DEGREES:
                        {
                            // Add the digits to the left of the decimal
                            for (int iIndex = 0; iIndex < byTotalDigits - byDecimalDigits; iIndex++)
                            {
                                strFormat += "#";
                            }

                            strFormat += ".";

                            // Add the digits to the right of the decimal
                            for (int iIndex = 0; iIndex < byDecimalDigits; iIndex++)
                            {
                                strFormat += "#";
                            }

                            break;
                        }
                    case ANSIDisplayItem.DisplayType.DECIMAL_LEADING_ZERO:
                        {
                            strFormat = "0";

                            // Add the digits to the left of the decimal
                            for (int iIndex = 1; iIndex < byTotalDigits - byDecimalDigits; iIndex++)
                            {
                                strFormat += "#";
                            }

                            strFormat += ".";

                            // Add the digits to the right of the decimal
                            for (int iIndex = 0; iIndex < byDecimalDigits; iIndex++)
                            {
                                strFormat += "#";
                            }

                            break;
                        }
                }

                return strFormat;
            }
        }

        /// <summary>
        /// Gets the units as a string for the display item.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version  Issue# Description
        // -------- --- -------- ------ ---------------------------------------
        // 04/02/08 RCG 10.00.00        Created

        public override string Units
        {
            get
            {
                ANSIDisplayItem.UnitType Units = (ANSIDisplayItem.UnitType)((DisplayFormat & ANSIDisplayItem.UnitTypeMask) >> 4);
                string strUnits = "";

                switch (Units)
                {
                    case ANSIDisplayItem.UnitType.NONE:
                        {
                            strUnits = "None";
                            break;
                        }
                    case ANSIDisplayItem.UnitType.W:
                    case ANSIDisplayItem.UnitType.V:
                    case ANSIDisplayItem.UnitType.A:
                    case ANSIDisplayItem.UnitType.VH:
                    case ANSIDisplayItem.UnitType.AH:
                    case ANSIDisplayItem.UnitType.VA:
                    case ANSIDisplayItem.UnitType.VAR:
                    case ANSIDisplayItem.UnitType.WH:
                    case ANSIDisplayItem.UnitType.VAH:
                    case ANSIDisplayItem.UnitType.VARH:
                    case ANSIDisplayItem.UnitType.PF:
                        {
                            strUnits = "Units";
                            break;
                        }
                    case ANSIDisplayItem.UnitType.K:
                        {
                            strUnits = "Kilo - No Annunciator";
                            break;
                        }
                    case ANSIDisplayItem.UnitType.KAH:
                    case ANSIDisplayItem.UnitType.KVA:
                    case ANSIDisplayItem.UnitType.KVAH:
                    case ANSIDisplayItem.UnitType.KVAR:
                    case ANSIDisplayItem.UnitType.KVARH:
                    case ANSIDisplayItem.UnitType.KVH:
                    case ANSIDisplayItem.UnitType.KW:
                    case ANSIDisplayItem.UnitType.KWH:
                        {
                            strUnits = "Kilo";
                            break;
                        }
                    case ANSIDisplayItem.UnitType.M:
                        {
                            strUnits = "Mega - No Annunciator";
                            break;
                        }
                    case ANSIDisplayItem.UnitType.MVA:
                    case ANSIDisplayItem.UnitType.MVAH:
                    case ANSIDisplayItem.UnitType.MVAR:
                    case ANSIDisplayItem.UnitType.MVARH:
                    case ANSIDisplayItem.UnitType.MW:
                    case ANSIDisplayItem.UnitType.MWH:
                        {
                            strUnits = "Mega";
                            break;
                        }
                    default:
                        {
                            strUnits = Units.ToString();
                            break;
                        }
                }

                return strUnits;
            }
        }

        #endregion
    }
}
