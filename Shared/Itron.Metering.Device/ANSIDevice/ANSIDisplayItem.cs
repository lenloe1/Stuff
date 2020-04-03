///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//   embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//storage or retrieval system without the permission in writing from Itron, Inc.
//
//                              Copyright © 2006-2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;
using System.Globalization;
#if (!WindowsCE)
using System.Runtime.Serialization;
#endif

namespace Itron.Metering.Device
{
    /// <summary>
    /// This class provides access to all of the information required for a display item
    /// for the ANSI meters.
    /// </summary>
#if (!WindowsCE)
	[DataContract]
#endif
	public class ANSIDisplayItem : DisplayItem
    {
        #region Constants

        internal const ushort UnitTypeMask = 0x01F0;
        internal const ushort TypeMask = 0x000F;
        internal const byte TotalDigitsMask = 0xF0;
        internal const byte DecimalDigitMask = 0x0F;
        internal const ushort MAX_DIGITS = 6;

        #endregion

        #region Definitions

        /// <summary>
        /// Definition of the display item's unit type
        /// </summary>
        public enum UnitType : ushort
        {
            /// <summary>
            /// No units
            /// </summary>
            [EnumDescription("None")]
            NONE = 0,
            /// <summary>
            /// Watts
            /// </summary>
            [EnumDescription("W")]
            W = 1,
            /// <summary>
            /// Kilo Watts
            /// </summary>
            [EnumDescription("KW")]
            KW = 2,
            /// <summary>
            /// Var
            /// </summary>
            [EnumDescription("VAR")]
            VAR = 3,
            /// <summary>
            /// Kilo Var
            /// </summary>
            [EnumDescription("KVAR")]
            KVAR = 4,
            /// <summary>
            /// VA
            /// </summary>
            [EnumDescription("VA")]
            VA = 5,
            /// <summary>
            /// Kilo VA
            /// </summary>
            [EnumDescription("KVA")]
            KVA = 6,
            /// <summary>
            /// Volts
            /// </summary>
            [EnumDescription("V")]
            V = 7,
            /// <summary>
            /// Amps
            /// </summary>
            [EnumDescription("A")]
            A = 8,
            /// <summary>
            /// Watt hours
            /// </summary>
            [EnumDescription("Wh")]
            WH = 9,
            /// <summary>
            /// Kilo Watt hours
            /// </summary>
            [EnumDescription("KWh")]
            KWH = 10,
            /// <summary>
            /// Var hours
            /// </summary>
            [EnumDescription("VARh")]
            VARH = 11,
            /// <summary>
            /// Kilo Var hours
            /// </summary>
            [EnumDescription("KVARh")]
            KVARH = 12,
            /// <summary>
            /// VA hours
            /// </summary>
            [EnumDescription("VAh")]
            VAH = 13,
            /// <summary>
            /// Kilo VA hours
            /// </summary>
            [EnumDescription("KVAh")]
            KVAH = 14,
            /// <summary>
            /// Volt hours
            /// </summary>
            [EnumDescription("Vh")]
            VH = 15,
            /// <summary>
            /// Kilo Volt hours
            /// </summary>
            [EnumDescription("KVh")]
            KVH = 16,
            /// <summary>
            /// Amp hours
            /// </summary>
            [EnumDescription("Ah")]
            AH = 17,
            /// <summary>
            /// Kilo Amp hours
            /// </summary>
            [EnumDescription("KAh")]
            KAH = 18,
            /// <summary>
            /// Mega Watts
            /// </summary>
            [EnumDescription("MW")]
            MW = 19,
            /// <summary>
            /// Mega Vars
            /// </summary>
            [EnumDescription("MVAR")]
            MVAR = 20,
            /// <summary>
            /// Mega VA
            /// </summary>
            [EnumDescription("MVA")]
            MVA = 21,
            /// <summary>
            /// Mega Watt hours
            /// </summary>
            [EnumDescription("MWh")]
            MWH = 22,
            /// <summary>
            /// Mega Var hours
            /// </summary>
            [EnumDescription("MVARh")]
            MVARH = 23,
            /// <summary>
            /// Mega VA hours
            /// </summary>
            [EnumDescription("MVAh")]
            MVAH = 24,
            /// <summary>
            /// Kilo
            /// </summary>
            [EnumDescription("K")]
            K = 25,
            /// <summary>
            /// Power Factor
            /// </summary>
            [EnumDescription("PF")]
            PF = 26,
            /// <summary>
            /// Mega
            /// </summary>
            [EnumDescription("M")]
            M = 27,
        }

        /// <summary>
        /// Definition of the display type values
        /// </summary>
        public enum DisplayType : ushort
        {
            /// <summary>
            /// Unsigned integer
            /// </summary>
            [EnumDescription("Unsigned Integer")]
            UNSIGNED_INTEGER = 0,
            /// <summary>
            /// Unsigned integer with leading zeroes
            /// </summary>
            [EnumDescription("Unsigned Integer with leading zeroes")]
            UNSIGNED_INTEGER_LEADING_ZERO = 1,
            /// <summary>
            /// Signed integer
            /// </summary>
            [EnumDescription("Signed Integer")]
            SINGED_INTEGER = 2,
            /// <summary>
            /// Decimal
            /// </summary>
            [EnumDescription("With Decimal Point")]
            DECIMAL = 3,
            /// <summary>
            /// Decimal with leading zeroes
            /// </summary>
            [EnumDescription("With Decimal Point and leading zeroes")]
            DECIMAL_LEADING_ZERO = 4,
            /// <summary>
            /// Decimal degrees
            /// </summary>
            [EnumDescription("With Decimal Point and degrees")]
            DECIMAL_DEGREES = 6,
            /// <summary>
            /// Floating decimal point
            /// </summary>
            [EnumDescription("Floating Decimal Point")]
            FLOATING_DECIMAL = 7,
            /// <summary>
            /// Floating decmial point with leading zeroes
            /// </summary>
            [EnumDescription("Floating Decimal Point with leading zeroes")]
            FLOATING_DECIMAL_LEADING_ZEROS = 8,
            /// <summary>
            /// Date (Year, Month, Day)
            /// </summary>
            [EnumDescription("yy:mm:dd")]
            DATE_YY_MM_DD = 9,
            /// <summary>
            /// Date (Month, Day, Year)
            /// </summary>
            [EnumDescription("mm:dd:yy")]
            DATE_MM_DD_YY = 10,
            /// <summary>
            /// Date (Day, Month, Year)
            /// </summary>
            [EnumDescription("dd:mm:yy")]
            DATE_DD_MM_YY = 11,
            /// <summary>
            /// Time (Hour, Minute, Second)
            /// </summary>
            [EnumDescription("Time in Hour:Minute:Second format")]
            TIME_HH_MM_SS = 12,
            /// <summary>
            /// User text field
            /// </summary>
            [EnumDescription("User Text Field")]
            USER_FIELD = 13,
            /// <summary>
            /// All segments
            /// </summary>
            [EnumDescription("All Segments")]
            ALL_SEGMENTS = 14,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/02/06 KRC 7.35.00 N/A    Created
        //
        public ANSIDisplayItem()
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
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/02/06 KRC 7.35.00 N/A    Created
        //
        public ANSIDisplayItem(LID Lid, string strDisplayID, ushort usFormat, byte byDim)
            : base()
        {
            m_strDisplayID = strDisplayID;
            m_usFormat = usFormat;
            m_byDim = byDim;
            //Always set the LID last so it gets created with all of the Format information.
            DisplayLID = Lid;
        }
            
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets whether or not the display item can be edited.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 KRC 8.00.13 N/A    Created
        //  03/14/07 KRC 8.00.18 2519   Remove SS, SR and LS values from being editable.
        public override bool Editable
        {
            get
            {
                bool bResult = false;

                if (true == m_LID.IsEnergy || 
                    true == m_LID.IsMaxDemand ||
                    true == m_LID.IsMinDemand )
                {
                    // Make sure it is not one of the types we don't support.
                    if (false == m_LID.IsTOO && false == m_LID.IsSelfRead &&
                        false == m_LID.IsSnapshot && false == m_LID.IsLastSeason)
                    {
                        bResult = true;
                    }
                }

                return bResult;
            }
        }

        /// <summary>
        /// Determines if a Negative is allowed
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/29/07 KRC 8.00.22 N/A    Adding support for negative and non-negative registers
        public override bool NegativeAllowed
        {
            get
            {
                return m_LID.IsNegativeAllowed;
            }
        }

        /// <summary>
        /// Gets the format of the display item as a string
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/02/08 RCG	1.50.14		   Created

        public virtual string Format
        {
            get
            {
                // TODO: Add support for the rest of the ANSI devices
                throw new NotImplementedException("Format has only been implemented for OpenWay");
            }
        }

        /// <summary>
        /// Gets the units of the display item as a string
        /// </summary>
        /// <summary>
        /// Gets the format of the display item as a string
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/02/08 RCG	1.50.14		   Created

        public virtual string Units
        {
            get
            {
                // TODO: Add support for the rest of the ANSI devices
                throw new NotImplementedException("Units has only been implemented for OpenWay");
            }
        }

        /// <summary>
        /// Gets the unit enumeration value of the display item.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 11/17/10 jrf 2.45.13        Created.
        //
        public UnitType Unit
        {
            get
            {
                return (UnitType)((DisplayFormat & UnitTypeMask) >> 4);
            }
        }
        
        /// <summary>
        /// Gets the display type enumeration value of the display item.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 11/17/10 jrf 2.45.13        Created.
        //
        public DisplayType Type
        {
            get
            {
                return (ANSIDisplayItem.DisplayType)(DisplayFormat & ANSIDisplayItem.TypeMask);
            }
        }

        /// <summary>
        /// Gets the total number of digits to display for the display item.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 11/17/10 jrf 2.45.13        Created.
        //
        public int TotalDigits
        {
            get
            {
                int iTotalDigits = (int)((DisplayDim & TotalDigitsMask) >> 4);
                return iTotalDigits;
            }
        }

        /// <summary>
        /// Gets the number of decimal digits to display for the display item.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 11/17/10 jrf 2.45.13        Created.
        //
        public int DecimalDigits
        {
            get
            {
                int iDecimalDigits = (int)(DisplayDim & DecimalDigitMask);
                return iDecimalDigits;
            }
        }

        /// <summary>
        /// Determines if the Display Item is a Floating Decimal Point value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/06 KRC 7.36.00
        //  02/08/07 RCG 8.00.11        Moved from MFGTable2048 and converted to property
        //  11/17/10 jrf 2.45.13        Made public
        //
        public bool IsFloatingPoint
        {
            get
            {
                bool bResult = false;
                ushort usType = (ushort)(DisplayFormat & TypeMask);

                switch (usType)
                {
                    case (ushort)DisplayType.FLOATING_DECIMAL:
                    case (ushort)DisplayType.FLOATING_DECIMAL_LEADING_ZEROS:
                        {
                            bResult = true;
                            break;
                        }
                }

                return bResult;
            }
        }

        /// <summary>
        /// Gets the LID object for the item on the display.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 11/17/10 jrf 2.45.13        Made public
        //
        public LID DisplayLID
        {
            get
            {
                return m_LID;
            }
            set
            {
                m_LID = value;

                if (m_LID != null)
                {
                    // Make sure that the LID number always matches the LID
                    // object
                    m_uiLIDNumber = m_LID.lidValue;

                    // Change the measurement unit of the LID to make sure we
                    // get the correct description
                    m_LID.lidMeasurementUnit = MeasurementUnit;

                    // Now set the Description based on the LID object
                    Description = m_LID.lidDescription;
                }


            }
        }   

        #endregion

        #region Internal Methods

        /// <summary>
        /// Writes a new values to the meter for registers that appear on the display.
        /// </summary>
        /// <param name="psem">PSEM Object</param>
        /// <returns>ItronDeviceResult</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 KRC 8.00.13 N/A    Created

        internal ItronDeviceResult WriteNewValue(CPSEM psem)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            PSEMResponse PSEMResult = PSEMResponse.Ok;

            LIDWriter Writer = new LIDWriter(psem);

            if (TypeCode.Double == m_LID.lidType)
            {
                Double dblValue = Double.Parse(m_strValue, CultureInfo.InvariantCulture);

                // Adjust the value to be written to the meter if the display
                //  unit is Kilo or Mega.
                if (m_LID.lidMeasurementUnit == LID.MeasurementUnit.KILO)
                {
                    dblValue = dblValue * 1000;
                }
                else if (m_LID.lidMeasurementUnit == LID.MeasurementUnit.MEGA)
                {
                    dblValue = dblValue * 1000000;
                }

                // Handles Energies, Cum and CCUM
                PSEMResult = Writer.WriteLID(m_LID, dblValue);
            }
            else if (TypeCode.Single == m_LID.lidType)
            {
                float fltValue = float.Parse(m_strValue, CultureInfo.InvariantCulture);

                // Adjust the value to be written to the meter if the display
                //  unit is Kilo or Mega.
                if (m_LID.lidMeasurementUnit == LID.MeasurementUnit.KILO)
                {
                    fltValue = fltValue * 1000;
                }
                else if (m_LID.lidMeasurementUnit == LID.MeasurementUnit.MEGA)
                {
                    fltValue = fltValue * 1000000;
                }
                // Handles all other demand registers
                PSEMResult = Writer.WriteLID(m_LID, fltValue);
            }

            // The operation is done, so tranlate the error
            if (PSEMResponse.Ok == PSEMResult)
            {
                Result = ItronDeviceResult.SUCCESS;
            }
            else if (PSEMResponse.Isc == PSEMResult)
            {
                Result = ItronDeviceResult.SECURITY_ERROR;
            }
            else
            {
                Result = ItronDeviceResult.ERROR;
            }
            return Result;
        }

        /// <summary>
        /// Formats the given data to match what is on the display
        /// </summary>
        /// <param name="objData">The object to format</param>
        /// <returns>The item formatted to match the display</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/18/06 KRC 7.36.00
        //

        internal virtual void FormatData(object objData)
        {
            double dbValue = 0.0;
            float fValue = 0.0F;
            bool bIsDouble = false;

            if (true == IsUnsignedInteger)
            {
                UInt32 uiValue = Convert.ToUInt32(objData, CultureInfo.InvariantCulture);
                FormatUnsignedInteger(uiValue);
            }
            else if (true == IsSignedInteger)
            {
                Int32 iValue = Convert.ToInt32(objData, CultureInfo.InvariantCulture);
                FormatSignedInteger(iValue);
            }
            else if (true == IsUserField)
            {
                // This is just a string, so show what is retrieved.
                Value = objData.ToString();
            }
            else if (true == IsFloatingPoint ||
                true == IsFixedPoint)
            {
                // First determine if the value is going to be a double 
                // or a single floating point value

                if (DisplayLID.lidType == TypeCode.Double)
                {
                    bIsDouble = true;
                }
                else
                {
                    bIsDouble = false;
                }

                // Now get the value into the proper scale
                if (LID.MeasurementUnit.UNIT == MeasurementUnit)
                {
                    if (bIsDouble == true)
                    {
                        dbValue = (double)objData;
                    }
                    else if (DisplayLID.lidType == TypeCode.UInt32)
                    {
                        // Some values may come back as UINT32 so handle this case
                        // Such as Ins VA and Var
                        fValue = (float)(uint)objData; 
                    }
                    else
                    {
                        // fValue = (float)objData;
                        fValue = (float)Convert.ToSingle(objData, CultureInfo.InvariantCulture);
                    }
                }
                else if (LID.MeasurementUnit.KILO == MeasurementUnit)
                {
                    if (bIsDouble == true)
                    {
                        dbValue = (double)objData / 1000;
                    }
                    else if (DisplayLID.lidType == TypeCode.UInt32)
                    {
                        // Some values may come back as UINT32 so handle this case
                        // Such as Ins VA and Var
                        fValue = (uint)objData / 1000.0f;
                    }
                    else
                    {
                        fValue = (float)objData / 1000;
                    }
                }
                else if (LID.MeasurementUnit.MEGA == MeasurementUnit)
                {
                    if (bIsDouble == true)
                    {
                        dbValue = (double)objData / 1000000;
                    }
                    else if (DisplayLID.lidType == TypeCode.UInt32)
                    {
                        // Some values may come back as UINT32 so handle this case
                        // Such as Ins VA and Var
                        fValue = (uint)objData / 1000000.0f;
                    }
                    else
                    {
                        fValue = (float)objData / 1000000;
                    }

                }

                if (true == IsFloatingPoint)
                {
                    if (bIsDouble == true)
                    {
                        FormatFloatingPoint(dbValue);
                    }
                    else
                    {
                        FormatFloatingPoint(fValue);
                    }
                }
                else if (true == IsFixedPoint)
                {
                    if (bIsDouble == true)
                    {
                        FormatFixedPoint(dbValue);
                    }
                    else
                    {
                        FormatFixedPoint(fValue);
                    } 
                }
            }
            else if (true == IsDate)
            {
                // We must assume that the data passed in is already
                // a DateTime for dates
                FormatDateTime((DateTime)objData);
            }
            else if (true == IsTime)
            {
                FormatTime(Convert.ToInt32(objData, CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Formats the a time stored as an Int32 to match the meter display
        /// </summary>
        /// <param name="iValue">The value to format</param>
        /// <returns>A string for the formatted time.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/27/06 KRC 7.36.00
        //  01/31/13 AF  2.70.62 240682 Changed TotalHours to Days + Hours.  For time spans
        //                              greater than 30 minutes but less than 1 hour, the ToString() method
        //                              rounded TotalHours up to 1.
        //
        internal virtual void FormatTime(Int32 iValue)
        {
            string strData = "";

            TimeSpan tsTime = new TimeSpan(0, 0, iValue);
            strData = ((tsTime.Days * 24) + tsTime.Hours).ToString("00", CultureInfo.InvariantCulture) + ":" +
                        tsTime.Minutes.ToString("00", CultureInfo.InvariantCulture) + ":" +
                        tsTime.Seconds.ToString("00", CultureInfo.InvariantCulture);

            Value = strData;
        }

        /// <summary>
        /// Formats the Unsigned Int Items
        /// </summary>
        /// <param name="uiData">The data to format</param>
        /// <returns>A string that represents the formatted Uint</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/27/06 KRC 7.36.00
        //
        internal virtual void FormatUnsignedInteger(UInt32 uiData)
        {
            string strData = "";
            ushort usType = (ushort)(DisplayFormat & TypeMask);

            strData = uiData.ToString(CultureInfo.InvariantCulture);
            // Now trim so we will have the right most MAX_DIGITS bytes of data
            if (strData.Length > MAX_DIGITS)
            {
                // We have over the MAX_DIGITS, so trim so we will have the right most MAX_DIGITS
                // This case works for Leading and non-leading zero since we will end up with MAX_DIGITS
                strData = strData.Substring(strData.Length - MAX_DIGITS, MAX_DIGITS);
            }
            else if ((ushort)DisplayType.UNSIGNED_INTEGER_LEADING_ZERO == usType)
            {
                // We need Leading zeros and we have not reached MAX_DIGITS so fill them up.
                for (int iCount = strData.Length; iCount < MAX_DIGITS; iCount++)
                {
                    strData = "0" + strData;
                }
            }

            Value = strData;
        }

        /// <summary>
        /// Formats the Signed Int Items
        /// </summary>
        /// <param name="iData">The integer data to format</param>
        /// <returns>A string of the formatted integer</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/27/06 KRC 7.36.00
        //
        internal virtual void FormatSignedInteger(Int32 iData)
        {
            string strData = "";

            strData = iData.ToString(CultureInfo.InvariantCulture);
            // Now trim so we will have the right most MAX_DIGITS bytes of data
            if (strData.Length > MAX_DIGITS)
            {
                // We have over the MAX_DIGITS, so trim so we will have the right most MAX_DIGITS
                // This case works for Leading and non-leading zero since we will end up with MAX_DIGITS
                strData = strData.Substring(strData.Length - MAX_DIGITS, MAX_DIGITS);
            }

            Value = strData;
        }

        /// <summary>
        /// Formats the Date/Time Items
        /// </summary>
        /// <param name="dtDateTime">The DateTime object to format</param>
        /// <returns>A string of the formatted DateTime</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/27/06 KRC 7.36.00
        //  03/14/07 RCG 8.00.18 2569   Changing formatting to match the display

        internal virtual void FormatDateTime(DateTime dtDateTime)
        {
            string strData = "";
            ushort usType = (ushort)(DisplayFormat & TypeMask);

            switch (usType)
            {
                case (ushort)DisplayType.DATE_DD_MM_YY:
                {
                    strData = dtDateTime.ToString("dd-MM-yy", CultureInfo.InvariantCulture);
                    break;
                }
                case (ushort)DisplayType.DATE_MM_DD_YY:
                {
                    strData = dtDateTime.ToString("MM-dd-yy", CultureInfo.InvariantCulture);
                    break;
                }
                case (ushort)DisplayType.DATE_YY_MM_DD:
                {
                    strData = dtDateTime.ToString("yy-MM-dd", CultureInfo.InvariantCulture);
                    break;
                }
                case (ushort)DisplayType.TIME_HH_MM_SS:
                {
                    strData = dtDateTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                    break;
                }
                default:
                {
                    strData = dtDateTime.ToString(CultureInfo.InvariantCulture);
                    break;
                }
            }

            Value = strData;
        }

        /// <summary>
        /// Formats a Floating Point Decimal Number
        /// </summary>
        /// <param name="dblValue">The floatig point value to format</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/06 KRC 7.36.00
        //  03/12/07 KRC 8.00.18 2614   Fixing formating issue with small number getting converted to scientific
        //
        internal virtual void FormatFloatingPoint(double dblValue)
        {
            string sZeros = "000000";
            string sValue = "";
            int nDecLocation;
            int nDecimals;

            // Start by converting to a string - The string passed in ensures we don't go to scientific and we keep enough digits
            sValue = dblValue.ToString("0.######", CultureInfo.InvariantCulture);

            // Make sure we have plenty of decimals
            nDecLocation = sValue.IndexOf('.');
            if (0 > nDecLocation)
            {
                sValue = sValue + '.' + sZeros;
                nDecimals = sZeros.Length;
            }
            else
            {
                sValue = sValue + sZeros;
                nDecimals = sValue.Length - nDecLocation - 1;
            }

            // Cases 1 & 2
            if ((0 <= dblValue) && (10 > dblValue))
            {
                // 5 digits used, 4 decimal places. 1.2345676 >> 1.2345
                sValue = sValue.Substring(0, sValue.Length - (nDecimals - 4));

                if (false != IsLeadingZeros)
                {
                    // 6 digits used, 4 decimal places. 1.2345676 >> 01.2345
                    sValue = "0" + sValue;
                }
            }

            // Case 3
            else if ((10 <= dblValue) && (100000 > dblValue))
            {
                // 6 digits used, decimal places depends on size of value, leading zeroes 
                // has no effect. 123.45676 >> 123.456

                // Truncate whatever won't fit.
                sValue = sValue.Substring(0, MAX_DIGITS + 1);
            }

            // Case 4
            else if (100000 <= dblValue)
            {
                // 6 digits used, no decimal places, right most digits displayed, leading 
                // zeroes has no effect. 123456789 >> 456789
                sValue = sValue.Substring(0, sValue.Length - (nDecimals + 1));

                // Truncate whatever won't fit.
                sValue = sValue.Substring((sValue.Length - MAX_DIGITS), MAX_DIGITS);
            }

            //Value must be negative if it hasn't been handled yet.
            // Case 5 
            else if (-10000 < dblValue)
            {
                // Same as Case 3, 6 digits used, decimal places depends on size of
                // value, leading zeroes has no effect.  -12.345678 >> -12.345

                // Truncate whatever won't fit.
                sValue = sValue.Substring(0, MAX_DIGITS + 1);
            }

            // Case 6: Value <= -100000
            else
            {
                // 6 digits used, no decimal places, right most digits displayed,
                // leading zeroes has no effect. -123456789 >> -56789
                sValue = sValue.Substring(0, sValue.Length - (nDecimals + 1));

                // Truncate whatever won't fit.
                sValue = "-" + sValue.Substring((sValue.Length - MAX_DIGITS), MAX_DIGITS - 1);
            }

            Value = sValue;
        }

        /// <summary>
        /// Formats a Floating Point Decimal Number
        /// </summary>
        /// <param name="fValue">The floatig point value to format</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/12/07 RCG 8.00.11        Created
        //  03/12/07 KRC 8.00.18 2614   Fixing formating issue with small number getting converted to scientific
        //
        internal virtual void FormatFloatingPoint(float fValue)
        {
            string sZeros = "000000";
            string sValue = "";
            int nDecLocation;
            int nDecimals;

            // Start by converting to a string - The string passed in ensures we don't go to scientific and we keep enough digits
            sValue = fValue.ToString("0.######", CultureInfo.InvariantCulture);

            // Make sure we have plenty of decimals
            nDecLocation = sValue.IndexOf('.');
            if (0 > nDecLocation)
            {
                sValue = sValue + '.' + sZeros;
                nDecimals = sZeros.Length;
            }
            else
            {
                sValue = sValue + sZeros;
                nDecimals = sValue.Length - nDecLocation - 1;
            }

            // Cases 1 & 2
            if ((0 <= fValue) && (10 > fValue))
            {
                // 5 digits used, 4 decimal places. 1.2345676 >> 1.2345
                sValue = sValue.Substring(0, sValue.Length - (nDecimals - 4));

                if (false != IsLeadingZeros)
                {
                    // 6 digits used, 4 decimal places. 1.2345676 >> 01.2345
                    sValue = "0" + sValue;
                }
            }

            // Case 3
            else if ((10 <= fValue) && (100000 > fValue))
            {
                // 6 digits used, decimal places depends on size of value, leading zeroes 
                // has no effect. 123.45676 >> 123.456

                // Truncate whatever won't fit.
                sValue = sValue.Substring(0, MAX_DIGITS + 1);
            }

            // Case 4
            else if (100000 <= fValue)
            {
                // 6 digits used, no decimal places, right most digits displayed, leading 
                // zeroes has no effect. 123456789 >> 456789
                sValue = sValue.Substring(0, sValue.Length - (nDecimals + 1));

                // Truncate whatever won't fit.
                sValue = sValue.Substring((sValue.Length - MAX_DIGITS), MAX_DIGITS);
            }

            //Value must be negative if it hasn't been handled yet.
            // Case 5 
            else if (-10000 < fValue)
            {
                // Same as Case 3, 6 digits used, decimal places depends on size of
                // value, leading zeroes has no effect.  -12.345678 >> -12.345

                // Truncate whatever won't fit.
                sValue = sValue.Substring(0, MAX_DIGITS + 1);
            }

            // Case 6: Value <= -100000
            else
            {
                // 6 digits used, no decimal places, right most digits displayed,
                // leading zeroes has no effect. -123456789 >> -56789
                sValue = sValue.Substring(0, sValue.Length - (nDecimals + 1));

                // Truncate whatever won't fit.
                sValue = "-" + sValue.Substring((sValue.Length - MAX_DIGITS), MAX_DIGITS - 1);
            }

            Value = sValue;
        }

        /// <summary>
        /// Formats a Fixed Point Decimal Number
        /// </summary>
        /// <param name="dbValue">The fixed point number to format</param>
        /// <returns>A string of the formatted fixed point number</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/06 KRC 7.36.00
        //  03/12/07 KRC 8.00.18 2614   Fixing formating issue with small number getting converted to scientific
        //
        internal virtual void FormatFixedPoint(double dbValue)
        {
            string sValue = "";
            string sZeros = "000000";
            int nMaxStringLength;
            int nDecLocation;
            int nDecimals;
            int iTotalDigits = (int)((DisplayDim & TotalDigitsMask) >> 4);
            int iDecimalDigits = (int)(DisplayDim & DecimalDigitMask);

            // Start by converting the number to a string - The string passed in ensures we don't go to scientific and we keep enough digits
            sValue = dbValue.ToString("0.######", CultureInfo.InvariantCulture);

            // Make sure we have plenty of decimals 
            nDecLocation = sValue.IndexOf('.');
            if (0 > nDecLocation)
            {
                sValue = sValue + '.' + sZeros;
                nDecimals = sZeros.Length;
            }
            else
            {
                sValue = sValue + sZeros;
                nDecimals = sValue.Length - nDecLocation - 1;
            }

            // Truncate to the desired decimals.  Rounding is bad.
            if (0 == iDecimalDigits)
            {
                sValue = sValue.Substring(0, (sValue.Length - (nDecimals + 1)));
            }
            else
            {
                sValue = sValue.Substring(0, (sValue.Length - (nDecimals - iDecimalDigits)));
            }

            nMaxStringLength = iTotalDigits;
            if (0 < iDecimalDigits)
            {
                // The decimal point is free.  It doesn't use up a digit's place.
                nMaxStringLength = iTotalDigits + 1;
            }
            if ((0 > dbValue) && (MAX_DIGITS > iTotalDigits))
            {
                // If we're not using all of the digits, the negative sign is free too. 
                // If we're maxing out the display, we have to count the negative sign  
                // as a digit.
                nMaxStringLength = nMaxStringLength + 1;
            }

            // Do we have to adjust the string value?
            if ((false != IsLeadingZeros) && (nMaxStringLength > sValue.Length))
            {
                // We need to pad the value with 0's.  If it's negative, it will look 
                // better if those values come after the minus sign...
                if (0 > dbValue)
                {
                    sValue = "-" + sZeros.Substring(0, (nMaxStringLength - sValue.Length))
                             + sValue.Substring(1, sValue.Length - 1);
                }
                else
                {
                    sValue = sZeros.Substring(0, (nMaxStringLength - sValue.Length)) +
                             sValue;
                }
            }
            else if (nMaxStringLength < sValue.Length)
            {
                // Again we'll need to know if it's negative, so we can handle the sign
                if (0 > dbValue)
                {
                    sValue = "-" + sValue.Substring((sValue.Length - nMaxStringLength - 1), nMaxStringLength - 1);
                }
                else
                {
                    sValue = sValue.Substring((sValue.Length - nMaxStringLength), nMaxStringLength);
                }
            }

            Value = sValue;
        }

        /// <summary>
        /// Formats a Fixed Point Decimal Number
        /// </summary>
        /// <param name="fValue">The fixed point number to format</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/12/07 RCG 8.00.11        Created
        //  03/12/07 KRC 8.00.18 2614   Fixing formating issue with small number getting converted to scientific
        //
        internal virtual void FormatFixedPoint(float fValue)
        {
            string sValue = "";
            string sZeros = "000000";
            int nMaxStringLength;
            int nDecLocation;
            int nDecimals;
            int iTotalDigits = (int)((DisplayDim & TotalDigitsMask) >> 4);
            int iDecimalDigits = (int)(DisplayDim & DecimalDigitMask);

            // Start by converting the number to a string - The string passed in ensures we don't go to scientific and we keep enough digits
            sValue = fValue.ToString("0.######", CultureInfo.InvariantCulture);

            // Make sure we plenty of decimals 
            nDecLocation = sValue.IndexOf('.');
            if (0 > nDecLocation)
            {
                sValue = sValue + '.' + sZeros;
                nDecimals = sZeros.Length;
            }
            else
            {
                sValue = sValue + sZeros;
                nDecimals = sValue.Length - nDecLocation - 1;
            }

            // Truncate to the desired decimals.  Rounding is bad.
            if (0 == iDecimalDigits)
            {
                sValue = sValue.Substring(0, (sValue.Length - (nDecimals + 1)));
            }
            else
            {
                sValue = sValue.Substring(0, (sValue.Length - (nDecimals - iDecimalDigits)));
            }

            nMaxStringLength = iTotalDigits;
            if (0 < iDecimalDigits)
            {
                // The decimal point is free.  It doesn't use up a digit's place.
                nMaxStringLength = iTotalDigits + 1;
            }
            if ((0 > fValue) && (MAX_DIGITS > iTotalDigits))
            {
                // If we're not using all of the digits, the negative sign is free too. 
                // If we're maxing out the display, we have to count the negative sign  
                // as a digit.
                nMaxStringLength = nMaxStringLength + 1;
            }

            // Do we have to adjust the string value?
            if ((false != IsLeadingZeros) && (nMaxStringLength > sValue.Length))
            {
                // We need to pad the value with 0's.  If it's negative, it will look 
                // better if those values come after the minus sign...
                if (0 > fValue)
                {
                    sValue = "-" + sZeros.Substring(0, (nMaxStringLength - sValue.Length))
                             + sValue.Substring(1, sValue.Length - 1);
                }
                else
                {
                    sValue = sZeros.Substring(0, (nMaxStringLength - sValue.Length)) +
                             sValue;
                }
            }
            else if (nMaxStringLength < sValue.Length)
            {
                // Again we'll need to know if it's negative, so we can handle the sign
                if (0 > fValue)
                {
                    sValue = "-" + sValue.Substring((sValue.Length - nMaxStringLength - 1), nMaxStringLength - 1);
                }
                else
                {
                    sValue = sValue.Substring((sValue.Length - nMaxStringLength), nMaxStringLength);
                }
            }

            Value = sValue;
        }


        #endregion

        #region Internal Property

        /// <summary>
        /// Gets the format for the display item
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/02/06 KRC 7.35.00 N/A    Created
        //
        internal ushort DisplayFormat
        {
            get
            {
                return m_usFormat;
            }
            set
            {
                m_usFormat = value;
            }
        }

        /// <summary>
        /// Gets the Dimension of the Display Item
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/02/06 KRC 7.35.00 N/A    Created
        //
        internal byte DisplayDim
        {
            get
            {
                return m_byDim;
            }
            set
            {
                m_byDim = value;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets whether or not the display item uses Leading Zeros
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/06 KRC 7.36.00
        //  02/08/07 RCG 8.00.11        Moved from MFGTable2048 and converted to property

        protected bool IsLeadingZeros
        {
            get
            {
                bool bResult = false;
                ushort usType = (ushort)(DisplayFormat & TypeMask);

                switch (usType)
                {
                    case (ushort)DisplayType.UNSIGNED_INTEGER_LEADING_ZERO:
                    case (ushort)DisplayType.DECIMAL_LEADING_ZERO:
                    case (ushort)DisplayType.FLOATING_DECIMAL_LEADING_ZEROS:
                    {
                        bResult = true;
                        break;
                    }
                }

                return bResult;
            }
        }

        /// <summary>
        /// Gets the Measurement Unit of the display item
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/18/06 KRC 7.36.00
        //  02/08/07 RCG 8.00.11        Moved from MFGTable2048 and converted to property

        protected LID.MeasurementUnit MeasurementUnit
        {
            get
            {
                LID.MeasurementUnit eMeasUnit = LID.MeasurementUnit.UNIT;
                ushort usMeasCode = 0;

                usMeasCode = (ushort)((DisplayFormat & UnitTypeMask) >> 4);

                switch (usMeasCode)
                {
                    case (ushort)UnitType.NONE:
                    case (ushort)UnitType.W:
                    case (ushort)UnitType.V:
                    case (ushort)UnitType.A:
                    case (ushort)UnitType.VH:
                    case (ushort)UnitType.AH:
                    case (ushort)UnitType.VA:
                    case (ushort)UnitType.VAR:
                    case (ushort)UnitType.WH:
                    case (ushort)UnitType.VAH:
                    case (ushort)UnitType.VARH:
                    case (ushort)UnitType.PF:
                    {
                        eMeasUnit = LID.MeasurementUnit.UNIT;
                        break;
                    }
                    case (ushort)UnitType.K:
                    case (ushort)UnitType.KAH:
                    case (ushort)UnitType.KVA:
                    case (ushort)UnitType.KVAH:
                    case (ushort)UnitType.KVAR:
                    case (ushort)UnitType.KVARH:
                    case (ushort)UnitType.KVH:
                    case (ushort)UnitType.KW:
                    case (ushort)UnitType.KWH:
                    {
                        eMeasUnit = LID.MeasurementUnit.KILO;
                        break;
                    }
                    case (ushort)UnitType.M:
                    case (ushort)UnitType.MVA:
                    case (ushort)UnitType.MVAH:
                    case (ushort)UnitType.MVAR:
                    case (ushort)UnitType.MVARH:
                    case (ushort)UnitType.MW:
                    case (ushort)UnitType.MWH:
                    {
                        eMeasUnit = LID.MeasurementUnit.MEGA;
                        break;
                    }
                }
                return eMeasUnit;
            }
        }

        /// <summary>
        /// Determines if the Display Item is a Fixed Decimal Point value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/27/06 KRC 7.36.00
        //  02/08/07 RCG 8.00.11        Moved from MFGTable2048 and converted to property

        protected bool IsFixedPoint
        {
            get
            {
                bool bResult = false;
                ushort usType = (ushort)(DisplayFormat & TypeMask);

                switch (usType)
                {
                    case (ushort)DisplayType.DECIMAL:
                    case (ushort)DisplayType.DECIMAL_DEGREES:
                    case (ushort)DisplayType.DECIMAL_LEADING_ZERO:
                    {
                        bResult = true;
                        break;
                    }
                }

                return bResult;
            }
        }

        /// <summary>
        /// Determines if the Display Item is an unsigned integer value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/27/06 KRC 7.36.00
        //  02/08/07 RCG 8.00.11        Moved from MFGTable2048 and converted to property

        protected virtual bool IsUnsignedInteger
        {
            get
            {
                bool bResult = false;
                ushort usType = (ushort)(DisplayFormat & TypeMask);

                switch (usType)
                {
                    case (ushort)DisplayType.UNSIGNED_INTEGER:
                    case (ushort)DisplayType.UNSIGNED_INTEGER_LEADING_ZERO:
                    {
                        bResult = true;
                        break;
                    }
                }

                return bResult;
            }
        }

        /// <summary>
        /// Determines if the Display Item is a signed integer value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/27/06 KRC 7.36.00
        //  02/08/07 RCG 8.00.11        Moved from MFGTable2048 and converted to property

        protected bool IsSignedInteger
        {
            get
            {
                bool bResult = false;
                ushort usType = (ushort)(DisplayFormat & TypeMask);

                switch (usType)
                {
                    case (ushort)DisplayType.SINGED_INTEGER:
                    {
                        bResult = true;
                        break;
                    }
                }

                return bResult;
            }
        }

        /// <summary>
        /// Determines if the Display Item is a Date value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/27/06 KRC 7.36.00
        //  02/08/07 RCG 8.00.11        Moved from MFGTable2048 and converted to property

        protected bool IsDate
        {
            get
            {
                bool bResult = false;
                ushort usType = (ushort)(DisplayFormat & TypeMask);

                switch (usType)
                {
                    case (ushort)DisplayType.DATE_DD_MM_YY:
                    case (ushort)DisplayType.DATE_MM_DD_YY:
                    case (ushort)DisplayType.DATE_YY_MM_DD:
                    {
                        bResult = true;
                        break;
                    }
                }

                return bResult;
            }
        }

        /// <summary>
        /// Determines if the Display Item is a Time value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/16/06 KRC 8.00.00
        //  02/08/07 RCG 8.00.11        Moved from MFGTable2048 and converted to property

        protected bool IsTime
        {
            get
            {
                bool bResult = false;
                ushort usType = (ushort)(DisplayFormat & TypeMask);

                switch (usType)
                {
                    case (ushort)DisplayType.TIME_HH_MM_SS:
                    {
                        bResult = true;
                        break;
                    }
                }

                return bResult;
            }
        }

        /// <summary>
        /// Determines if the Display Item is a user field (String) value
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/27/06 KRC 7.36.00
        //  02/08/07 RCG 8.00.11        Moved from MFGTable2048 and converted to property

        protected virtual bool IsUserField
        {
            get
            {
                bool bResult = false;
                ushort usType = (ushort)(DisplayFormat & TypeMask);

                switch (usType)
                {
                    case (ushort)DisplayType.USER_FIELD:
                    {
                        bResult = true;
                        break;
                    }
                }

                return bResult;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// The LID for the current display item
        /// </summary>
        protected LID m_LID;
        private ushort m_usFormat;
        private byte m_byDim;
        private uint m_uiLIDNumber;

        #endregion

    }
}
