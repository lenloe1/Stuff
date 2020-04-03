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
//                              Copyright © 2006 - 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Itron.Metering.Utilities;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// This class is derived from DisplayItem and represents the unique features
    /// a display item as implemented in an SCS device.  
    /// </summary>
    public class SCSDisplayItem : DisplayItem
    {
        #region Constants

        private const int ID_OFFSET = 1;
        private const int DISPLAYMODE_OFFSET = 2;
        private const int TOURATE_OFFSET = 2;
        private const int ADDRESSBANK_OFFSET = 2;
        private const int ADDRESS_OFFSET = 3;
        private const int DISPLAYITEM_LENGTH = 4;
        private const byte ALTMODE_MASK = 0x80;
        private const byte END_OF_TABLE = 0xFF;

        internal const byte DISP_RATE_E = 0x0;
        internal const byte DISP_RATE_A = 0x1;
        internal const byte DISP_RATE_B = 0x2;
        internal const byte DISP_RATE_C = 0x3;
        internal const byte DISP_RATE_D = 0x4;

        #endregion

        #region Definitions
        /// <summary>
        /// This enumeration represents the register class value found in the
        /// lower nibble of the first byte of an SCS display entry record.  The register 
        /// class is used to describe the general type of value to be displayed
        /// </summary>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/15/06 mah 8.00.00  N/A   Created
        /// </remarks>
        internal enum SCSDisplayClass
        {
            /// <summary>
            /// 
            /// </summary>
            EnergyValue = 0,
            /// <summary>
            /// 
            /// </summary>
            DemandValue = 1,
            /// <summary>
            /// 
            /// </summary>
            MaxDemandValue = 2,
            /// <summary>
            /// 
            /// </summary>
            InstantaneousValue = 3,
            /// <summary>
            /// 
            /// </summary>
            CumulativeValue = 4,
            /// <summary>
            /// 
            /// </summary>
            TotalContinuousCumulativeValue = 5,
            /// <summary>
            /// 
            /// </summary>
            FloatingPointBCD = 6,
            /// <summary>
            /// 
            /// </summary>
            IntegerBCD = 7,
            /// <summary>
            /// 
            /// </summary>
            DateValue = 8,
            /// <summary>
            /// 
            /// </summary>
            TimeValue = 9,
            /// <summary>
            /// 
            /// </summary>
            ASCIIValue = 10,
            /// <summary>
            /// 
            /// </summary>
            FixedBCD = 11,
            /// <summary>
            /// 
            /// </summary>
            BinaryValue = 12,
            /// <summary>
            /// 
            /// </summary>
            TOUContinuousCumulativeValue = 13,
            /// <summary>
            /// 
            /// </summary>
            ExtendedBCDValue = 14
        };

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an SCS display item from the bit mapped, 4 byte SCS display item
        /// record.  
        /// </summary>
        /// <param name="byDisplayTable"></param>
        /// <param name="nTableOffset"></param>
        /// <param name="boolTestMode"></param>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/15/06 mah 8.00.00  N/A   Created
        /// </remarks>
        public SCSDisplayItem(ref byte[] byDisplayTable, int nTableOffset, bool boolTestMode)
            : base()
        {
            // If the first byte contains an FF then this is an end of file marker

            m_boolEndOfFile = (byDisplayTable[nTableOffset] == END_OF_TABLE);

            if (!m_boolEndOfFile)
            {
                // make a copy of the unprocessed display item data for later interpretation if needed
                m_abyItemData = new byte[DISPLAYITEM_LENGTH];
                Array.Copy(byDisplayTable, nTableOffset, m_abyItemData, 0, DISPLAYITEM_LENGTH);

                // Decode the register type and class in order to determine what meaning of the
                // display table 

                m_byRegisterType = (byte)((byDisplayTable[nTableOffset] & 0x70) >> 4);
                m_byRegisterClass = (SCSDisplayClass)(byDisplayTable[nTableOffset] & 0x0F);

                // Extract the ID code from the second byte of the display table

                m_byDisplayID = BCD.BCDtoByte(byDisplayTable[nTableOffset + ID_OFFSET]);

                // Note that an ID code of zero indicates that no ID was programmed.  Therefore 
                // return an empty string rather than display a 0
                if (m_byDisplayID == 0)
                {
                    m_strDisplayID = "";
                }
                else
                {
                    m_strDisplayID = m_byDisplayID.ToString(CultureInfo.InvariantCulture);
                }

                if (boolTestMode)
                {
                    m_DisplayType = ItronDevice.DisplayMode.TEST_MODE;
                }
                else if ((byDisplayTable[nTableOffset + DISPLAYMODE_OFFSET] & ALTMODE_MASK) != 0)
                {
                    m_DisplayType = ItronDevice.DisplayMode.ALT_MODE;
                }
                else
                {
                    m_DisplayType = ItronDevice.DisplayMode.NORMAL_MODE;
                }

                m_byTOURate = (byte)((byDisplayTable[nTableOffset + TOURATE_OFFSET] & 0x70) >> 4);

                m_byUpperAddress = BCD.BCDtoByte((byte)(byDisplayTable[nTableOffset + ADDRESSBANK_OFFSET] & 0x0F));
                m_byLowerAddress = byDisplayTable[nTableOffset + ADDRESS_OFFSET];
            }
            else
            {
                m_strDescription = "End of Table";
                m_DisplayType = ItronDevice.DisplayMode.NORMAL_MODE;
            }

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method is used to populate the display item's description and 
        /// value fields.  Note that to populate the value field, the meter will be 
        /// interrogated to retrieve the current value of the item being displayed.
        /// </summary>
        /// <param name="device"></param>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/15/06 mah 8.00.00  N/A   Created
        /// </remarks>
        public void ReadCurrentValue(SCSDevice device)
        {
            if (Description.Length == 0)
            {
                Description = device.GetDisplayItemDescription(this);
            }

            // Next go ahead and retrieve the current value for the display
            // item from the meter

            GetDisplayItemValue(ref device);
        }

        /// <summary>
        /// Writes a new values to the meter for registers that appear on the display.
        /// </summary>
        /// <param name="device"></param>
        /// <returns>ItronDeviceResult</returns>
        public ItronDeviceResult WriteNewValue(SCSDevice device)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;

            if (true == Editable)
            {
                // The value is editable so there may be a value.  If the value is null or empty then don't bother
                if (null != Value && "" != Value)
                {
                    
                    Result = SetDisplayItemValue(ref device);
                }
            }

            return Result;
        }

        /// <summary>
        /// SetDisplayvalue - Determines the type of register this display item is
        ///                     and writes the updated value to the meter.
        /// </summary>
        /// <param name="device"></param>
        /// <returns>ItronDeviceResult</returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/30/07 KRC 8.09.00  N/A   Supporting Edit Registers
        /// </remarks>
        private ItronDeviceResult SetDisplayItemValue(ref SCSDevice device)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;

            switch (RegisterClass)
            {
                case SCSDisplayClass.EnergyValue:
                {
                    if (device.EnergyFormat.Units == SCSDisplayFormat.DisplayUnits.Units)
                    {
                        m_strValue = MoveDecimalToTheLeft(m_strValue, 3, device.EnergyFormat.NumDecimalDIgits);
                    }
                    Result = SetEnergyValue(ref device, m_strValue);

                    break;
                }
                case SCSDisplayClass.MaxDemandValue:
                {
                    if (device.DemandFormat.Units == SCSDisplayFormat.DisplayUnits.Units)
                    {
                        m_strValue = MoveDecimalToTheLeft(m_strValue, 3, device.DemandFormat.NumDecimalDIgits);
                    }
                    Result = SetMaxDemandValue(ref device, m_strValue);

                    break;
                }
                case SCSDisplayClass.CumulativeValue:
                case SCSDisplayClass.TotalContinuousCumulativeValue:
                case SCSDisplayClass.TOUContinuousCumulativeValue:
                {
                    if (device.CumulativeFormat.Units == SCSDisplayFormat.DisplayUnits.Units)
                    {
                        m_strValue = MoveDecimalToTheLeft(m_strValue, 3, device.CumulativeFormat.NumDecimalDIgits);
                    }
                    Result = SetCumulativeValue(ref device, m_strValue);
                    break;
                }

                default:
                // We need to investigate why this got called.
                Result = ItronDeviceResult.UNSUPPORTED_OPERATION;
                break;
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Provides access to the Editable property of the Display Item
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/22/07 KRC 8.00.08 N/A    Created
        //
        public override bool Editable
        {
            get
            {
                if (RegisterClass == SCSDisplayClass.EnergyValue || RegisterClass == SCSDisplayClass.MaxDemandValue || RegisterClass == SCSDisplayClass.CumulativeValue)
                {
                    m_blnEditable = (RegisterType != 0x07);
                }
                else
                {
                    m_blnEditable = false;
                }

                return m_blnEditable;
            }
        }

        #endregion


        #region Internal Properties

        internal Boolean EndOfFIle
        {
            get
            {
                return m_boolEndOfFile;
            }
        }

        internal ItronDevice.DisplayMode DisplayType
        {
            get
            {
                return m_DisplayType;
            }
        }

        internal SCSDisplayClass RegisterClass
        {
            get
            {
                return (SCSDisplayClass)m_byRegisterClass;
            }
        }

        internal byte UpperAddress
        {
            get
            {
                return m_byUpperAddress;
            }
        }

        internal byte LowerAddress
        {
            get
            {
                return m_byLowerAddress;
            }
        }

        internal int RegisterType
        {
            get
            {
                return (int)m_byRegisterType;
            }
        }

        internal int TOURate
        {
            get
            {
                return (int)m_byTOURate;
            }
        }

        internal byte[] ItemDefinition
        {
            get { return m_abyItemData; }
        }

        #endregion

        #region protected Methods

        /// <summary>
        /// This method is used to retrieve the current value of the display item.  Note that
        /// this method does not cache any values.  Therefore each time it is called, the meter
        /// will be interrogated to get the actual value that would be shown on the meter's display
        /// </summary>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/15/06 mah 8.00.00  N/A   Created
        /// </remarks>
        protected void GetDisplayItemValue(ref SCSDevice device)
        {
            String strValue;

            switch (RegisterClass)
            {
                case SCSDisplayClass.EnergyValue: strValue = GetEnergyValue(ref device);
                    break;

                case SCSDisplayClass.InstantaneousValue:

					int nAddress = device.TranslateDisplayAddress(this);

					if (nAddress != 0)
					{
                        strValue = device.ReadFloatingPointValue(nAddress).ToString("######0.000", CultureInfo.InvariantCulture);

						if (strValue.Length > 0)
						{
							// Now that we have a value that we can display we need to format it to match
							// the display format currently configured in the meter

							strValue = FormatDisplayValue(strValue, device.DemandFormat);
						}
					}
					else
					{
						strValue = "";
					}

                    break;

                case SCSDisplayClass.DemandValue: strValue = GetDemandValue(ref device);
                    break;

                case SCSDisplayClass.MaxDemandValue: strValue = GetMaxDemandValue(ref device);
                    break;

                case SCSDisplayClass.CumulativeValue: strValue = GetCumulativeValue(ref device);
                    break;

                case SCSDisplayClass.IntegerBCD: strValue = GetBCDValue(ref device);
                    break;

                case SCSDisplayClass.DateValue: strValue = GetDateValue(ref device);
                    break;

                case SCSDisplayClass.TimeValue: strValue = GetTimeValue(ref device);
                    break;

                case SCSDisplayClass.ASCIIValue: strValue = GetAsciiValue(ref device);
                    break;

                case SCSDisplayClass.FixedBCD: strValue = GetFixedBCDValue(ref device);
                    break;

                case SCSDisplayClass.FloatingPointBCD: strValue = device.ReadFloatingBCDValue(device.TranslateDisplayAddress(this), 4);
                    break;

                case SCSDisplayClass.TotalContinuousCumulativeValue: strValue = GetCCumValue(ref device);
                    break;

                case SCSDisplayClass.BinaryValue: strValue = GetBinaryValue(ref device);
                    break;

                case SCSDisplayClass.ExtendedBCDValue: strValue = GetFixedBCDValue(ref device);
                    break;

                case SCSDisplayClass.TOUContinuousCumulativeValue: strValue = GetCCumValue(ref device);
                    break;

                default:
                    strValue = "";
                    break;
            }

            Value = strValue;
        }

        /// <summary>
        /// This method retrieves an energy value from the meter and formats it according
        /// to the meter's formatting rules established in the meter's program
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        protected string GetEnergyValue(ref SCSDevice device)
        {
            String strValue;

            if (RegisterType == 0x00 || RegisterType == 0x02 || RegisterType == 0x03 )
            {
                strValue = device.ReadFixedBCDValue(device.TranslateDisplayAddress(this), 4, 7);
            }
            else if (RegisterType == 0x07)
            {
                strValue = device.ReadFloatingBCDValue(device.TranslateDisplayAddress(this), 3);
            }
            else
            {
                strValue = "";
            }

            if (strValue.Length > 0)
            {
                // Now that we have a value that we can display we need to format it to match
                // the display format currently configured in the meter

                strValue = FormatDisplayValue(strValue, device.EnergyFormat);
            }

            return strValue;
        }

        /// <summary>
        /// Writes an Energy Value to the meter
        /// </summary>
        /// <param name="device">Device we are talking to</param>
        /// <param name="strValue">The value to set into the meter</param>
        /// <returns>ItronDeviceResult</returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/30/07 KRC 8.09.00  N/A   Getting Edit Registers working
        /// </remarks>
        protected ItronDeviceResult SetEnergyValue(ref SCSDevice device, string strValue)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;

            if (RegisterType == 0x00 || RegisterType == 0x02 || RegisterType == 0x03)
            {
               Result = device.SetFixedBCDValue(device.TranslateDisplayAddress(this), 4, 7, strValue);
            }
            else
            {
                Result = ItronDeviceResult.ERROR;
            }

            return Result;
        }

        /// <summary>
        /// This method retrieves a demand value from the meter and formats it according
        /// to the meter's formatting rules established in the meter's program
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        protected string GetDemandValue(ref SCSDevice device)
        {
            String strValue;

            if (RegisterType == 1)
            {
                strValue = device.ReadPresentDemandValue(this);
            }
            else
            {
                strValue = device.ReadPreviousDemandValue(this);
            }

            if (strValue.Length > 0)
            {
                // Now that we have a value that we can display we need to format it to match
                // the display format currently configured in the meter

                strValue = FormatDisplayValue(strValue, device.DemandFormat);
            }

            return strValue;
        }

        /// <summary>
        /// This method retrieves a maximum demand value from the meter and formats it according
        /// to the meter's formatting rules established in the meter's program
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        protected string GetMaxDemandValue(ref SCSDevice device)
        {
            String strValue;

            if (RegisterType == 0x00 || RegisterType == 0x02 || RegisterType == 0x03)
            {
                strValue = device.ReadFloatingBCDValue(device.TranslateDisplayAddress(this), 4);
            }
            else if (RegisterType == 0x07)
            {
                strValue = device.ReadFloatingBCDValue(device.TranslateDisplayAddress(this), 3);
            }
            else
            {
                strValue = "";
            }

            if (strValue.Length > 0)
            {
                // Now that we have a value that we can display we need to format it to match
                // the display format currently configured in the meter

                strValue = FormatDisplayValue(strValue, device.DemandFormat);
            }

            return strValue;
        }

        /// <summary>
        /// Writes a Max Demand Value to the meter
        /// </summary>
        /// <param name="device">Device we are talking to</param>
        /// <param name="strValue">The value to set into the meter</param>
        /// <returns>ItronDeviceResult</returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/30/07 KRC 8.09.00  N/A   Getting Edit Registers working
        /// </remarks>
        protected ItronDeviceResult SetMaxDemandValue(ref SCSDevice device, string strValue)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;

            if (RegisterType == 0x00 || RegisterType == 0x02 || RegisterType == 0x03)
            {
                Result = device.SetFloatingBCDValue(device.TranslateDisplayAddress(this), 4, strValue);
            }
            else
            {
                Result = ItronDeviceResult.ERROR;
            }

            return Result;
        }

        /// <summary>
        /// This method retrieves a cummulative demand value from the meter and formats it according
        /// to the meter's formatting rules established in the meter's program
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// 01/30/07 KRC 8.00.09        Fix to handle type differences 
        ///</remarks>
        protected string GetCumulativeValue(ref SCSDevice device)
        {
            String strValue;

            if (RegisterType == 0x00 || RegisterType == 0x02 || RegisterType == 0x03)
            {
                strValue = device.ReadFloatingBCDValue(device.TranslateDisplayAddress(this), 4);
            }
            else if (RegisterType == 0x07)
            {
                strValue = device.ReadFloatingBCDValue(device.TranslateDisplayAddress(this), 3);
            }
            else
            {
                strValue = "";
            }

            if (strValue.Length > 0)
            {
                // Now that we have a value that we can display we need to format it to match
                // the display format currently configured in the meter

                strValue = FormatDisplayValue(strValue, device.CumulativeFormat);
            }

            return strValue;
        }

        /// <summary>
        /// Writes a Cum Value to the meter
        /// </summary>
        /// <param name="device">Device we are talking to</param>
        /// <param name="strValue">The value to set into the meter</param>
        /// <returns>ItronDeviceResult</returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/30/07 KRC 8.09.00  N/A   Getting Edit Registers working
        /// </remarks>
        protected ItronDeviceResult SetCumulativeValue(ref SCSDevice device, string strValue)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;

            if (RegisterType == 0x00 || RegisterType == 0x02 || RegisterType == 0x03)
            {
                Result = device.SetFloatingBCDValue(device.TranslateDisplayAddress(this), 4, strValue);
            }
            else if (RegisterType == 0x07)
            {
                Result = device.SetFloatingBCDValue(device.TranslateDisplayAddress(this), 3, strValue);
            }

            return Result;
        }

        /// <summary>
        /// This method retrieves a continuous cummulative demand value from the meter 
        /// and formats it according to the meter's formatting rules established 
        /// in the meter's program
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        protected string GetCCumValue(ref SCSDevice device)
        {
            String strValue = device.RetrieveCCumValue( this );

            if (strValue.Length > 0)
            {
                // Now that we have a value that we can display we need to format it to match
                // the display format currently configured in the meter

                strValue = FormatDisplayValue(strValue, device.CumulativeFormat);
            }

            return strValue;
        }

        /// <summary>
        /// This method sets a continuous cummulative demand value to the meter 
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/31/07 KRC 8.00.10  N/A   Support for Edit Registers
        /// </remarks>
        virtual protected ItronDeviceResult SetCCumValue(ref SCSDevice device, string strValue)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            return Result;
        }

        /// <summary>
        /// This method retrieves an ASCII value from the meter.  No additional formatting
        /// is performed
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        protected string GetAsciiValue(ref SCSDevice device)
        {
            String strValue = ""; ;

            switch (RegisterType)
            {
                case 0: // 4 byte field
                    strValue = device.ReadASCIIValue(device.TranslateDisplayAddress(this), 4);
                    break;
                case 1: // 8 byte field
                    strValue = device.ReadASCIIValue(device.TranslateDisplayAddress(this), 8);
                    break;
                case 2: // 9 byte field
                    strValue = device.ReadASCIIValue(device.TranslateDisplayAddress(this), 9);
                    break;
            }

            return strValue;
        }

        /// <summary>
        /// This method retrieves a BCD integer from the meter and returns it as a string value
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        protected string GetBCDValue(ref SCSDevice device)
        {
            string strBCDValue;
            int nBCDValue = 0;
            int nAddress = device.TranslateDisplayAddress(this);

            if (nAddress != 0x0)
            {
                switch (RegisterType)
                {
                    case 0: // 1 byte field
                        nBCDValue = device.ReadBCDInteger(nAddress, 1);
                        break;
                    case 1: // 2 byte field
                        nBCDValue = device.ReadBCDInteger(nAddress, 2);
                        break;
                    case 2: // 3 byte field
                        nBCDValue = device.ReadBCDInteger(nAddress, 3);
                        break;
                    case 3: // MSN 
                        nBCDValue = device.ReadBCDInteger(nAddress, 1);
                        break;
                    case 4: // LSN
                        nBCDValue = device.ReadBCDInteger(nAddress, 1);
                        break;
                }
                strBCDValue = nBCDValue.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                strBCDValue = "";
            }

            return strBCDValue;
        }

        /// <summary>
        /// This method retrieves a date from the meter and formats it according
        /// to the display'ss formatting rules established in the meter's program.  NOte that
        /// the value is not formatted according regional settings or preferences.  The
        /// value will be formatted precisely as it would be displayed by the meter.  Also
        /// note that the value may or may not contain a year
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        protected string GetDateValue(ref SCSDevice device)
        {
            int nYear = 0;
            int nMonth;
            int nDay;
            String strValue = "";

            DateTime dateValue;

            // The register type indicates the format while the address indicates which 
            // value is to be displayed

            if (LowerAddress == 0) // this is a special case - the meter's current date
            {
                dateValue = device.DeviceTime;

                nYear = dateValue.Year % 100;  // note that the devices only store 2 digits for the year
                nMonth = dateValue.Month;
                nDay = dateValue.Day;
            }
            else
            {
                // We need to determine if we should upload 2 bytes for the month and the day
                // verses 3 bytes for month day year.  If register type calls for a year to be displayed
                // the value must be a 3 byte date that includes the year.  If the register type only
                // calls for month and day, then we have to assume that at 2 byte date is being used

                if (RegisterType == 3 || RegisterType == 4)
                {
                    device.ReadBCDDate(device.TranslateDisplayAddress(this), out nMonth, out nDay);
                }
                else
                {
                    device.ReadBCDDate(device.TranslateDisplayAddress(this), out nYear, out nMonth, out nDay);
                }
            }

            // Now that we have a valid date, we can go ahead and format it as it was shown
            // on the display

            switch (RegisterType)
            {
                case 0: // date as MM-DD-YY
                    strValue = nMonth.ToString("00", CultureInfo.InvariantCulture) + "-" + nDay.ToString("00", CultureInfo.InvariantCulture) + "-" + nYear.ToString("00", CultureInfo.InvariantCulture);
                    break;
                case 1: // date as DD-MM-YY
                    strValue = nDay.ToString("00", CultureInfo.InvariantCulture) + "-" + nMonth.ToString("00", CultureInfo.InvariantCulture) + "-" + nYear.ToString("00", CultureInfo.InvariantCulture);
                    break;
                case 2: // date as YY-MM-DD
                    strValue = nYear.ToString("00", CultureInfo.InvariantCulture) + "-" + nMonth.ToString("00", CultureInfo.InvariantCulture) + "-" + nDay.ToString("00", CultureInfo.InvariantCulture);
                    break;
                case 3: // date as MM-DD
                    strValue = nMonth.ToString("00", CultureInfo.InvariantCulture) + "-" + nDay.ToString("00", CultureInfo.InvariantCulture);
                    break;
                case 4: // date as DD-MM
                    strValue = nDay.ToString("00", CultureInfo.InvariantCulture) + "-" + nMonth.ToString("00", CultureInfo.InvariantCulture);
                    break;
            }

            return strValue;
        }

        /// <summary>
        /// This method retrieves a time from the meter and formats it according
        /// to the display'ss formatting rules established in the meter's program.  NOte that
        /// the value is not formatted according regional settings or preferences.  The
        /// value will be formatted precisely as it would be displayed by the meter. 
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        protected string GetTimeValue(ref SCSDevice device)
        {
            String strValue = "";

            int nBasepageAddress = device.TranslateDisplayAddress(this);

            // NOte that there are certain dates & times that are simply not available.  They are 
            // indicated by a basepage address of zero.  If we encounter one of these simply
            // return an empty string

            if (nBasepageAddress != 0x0)
            {
                int nHour;
                int nMinute;
                int nSecond = 0;
                DateTime dateValue;

                // The register type indicates the format while the address indicates which 
                // value is to be displayed
                if (LowerAddress == 0x06) // this is a special case - the meter's current date
                {
                    dateValue = device.DeviceTime;

                    nHour = dateValue.Hour;
                    nMinute = dateValue.Minute;
                    nSecond = dateValue.Second;
                }
                else
                {
                    if (RegisterType == 0)
                    {
                        device.ReadBCDTime(nBasepageAddress, out nHour, out nMinute, out nSecond);
                    }
                    else
                    {
                        device.ReadBCDDate(nBasepageAddress, out nHour, out nMinute);
                    }
                }

                // Now that we have a valid date, we can go ahead and format it as it was shown
                // on the display

                switch (RegisterType)
                {
                    case 0: // time as HH-MM:SS
                        strValue = nHour.ToString("00", CultureInfo.InvariantCulture) + ":" + nMinute.ToString("00", CultureInfo.InvariantCulture) + ":" + nSecond.ToString("00", CultureInfo.InvariantCulture);
                        break;
                    case 1: // date as HH:MM or MM:SS
                        strValue = nHour.ToString("00", CultureInfo.InvariantCulture) + ":" + nMinute.ToString("00", CultureInfo.InvariantCulture);
                        break;
                }
            }

            return strValue;
        }

        /// <summary>
        /// This method retrieves a BCD floating point from the meter.  Note that the
        /// value is returned as a string in order to prevent any rounding or client formatting
        /// issues.
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
		/// 03/12/07 mah 8.00.18           Removed leading zeros per SCR #2459
        /// </remarks>
        protected string GetFixedBCDValue(ref SCSDevice device)
        {
            String strValue = ""; ;

            int nBasepageAddress = device.TranslateDisplayAddress(this);

            // A basepage address of zero indicates that the item is either undefined
            // or not displayable - simply return an empty string in this case
            if (nBasepageAddress != 0x00)
            {
                if (RegisterClass == SCSDisplayClass.ExtendedBCDValue)
                {
                    // The extended BCD format is XXX.XXX - we need to treat this a 3 byte BCD integer 
                    // and manually insert the decimal point

                    int nBCDValue = device.ReadBCDInteger(device.TranslateDisplayAddress(this), 3);
                    double fValue = (nBCDValue / 1000.0);

                    strValue = fValue.ToString("##0.000", CultureInfo.InvariantCulture);
                }
                else if (RegisterType == 0x01)
                {
                    strValue = device.ReadFixedBCDValue(device.TranslateDisplayAddress(this), 1, 2);
                }
                else if (RegisterType == 0x02)
                {
                    strValue = device.ReadFixedBCDValue(device.TranslateDisplayAddress(this), 1, 3);
                }
            }

			// Remove leading zeros - addresses SCR #2459
			strValue = strValue.TrimStart('0');

			// But did we get trim too much?
			int nDecimalLocation = strValue.IndexOf( '.');

			// if so, add the initial zero back
			if (nDecimalLocation < 1)
			{
				strValue = strValue.Insert(0, "0");
			}
				
            return strValue;
        }

        /// <summary>
        /// This method returns and formats a single nibble from the meter
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        protected string GetBinaryValue(ref SCSDevice device)
        {
            String strValue;

            if (RegisterType == 0)
            {
                strValue = device.ReadNibble(device.TranslateDisplayAddress(this), true).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                strValue = device.ReadNibble(device.TranslateDisplayAddress(this), false).ToString(CultureInfo.InvariantCulture);
            }

            return strValue;
        }

        /// <summary>
        /// This method parses and formats the given string according to the given formatting 
        /// rules.  The formatting rules should be obtained directly from the meter.
        /// </summary>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// 02/02/07 KRC 8.00.10        Fixing format issues after code changes made for edit
        /// </remarks>
        protected String FormatDisplayValue(String strDataValue, SCSDisplayFormat targetFormat)
        {
            // Start by scaling the value if needed - Assume the values are currently in kilo units
            int nDecimalLocation = strDataValue.IndexOf('.');
            int nTargetStringLength = targetFormat.Width + 1;   // Take the decimal point into account.

            if (targetFormat.Units == SCSDisplayFormat.DisplayUnits.Units)
            {
                // We need to move the decimal point three places to the right
                strDataValue = MoveDecimalToTheRight(strDataValue, 3, targetFormat.NumDecimalDIgits);
            }

			// set the appropriate number of decimal digits.  Note that we may lose some of these digits
			// in the coming steps but we should format the value appropriately
			strDataValue = FormatDecimalDigits(strDataValue, targetFormat.NumDecimalDIgits);

			// Before we establish the correct length of the display item, strip off all leading zeros.  Again
			// we may need to add some of these back on but this step simplifies the overall process
			strDataValue = strDataValue.TrimStart('0');

			// If we trimmed off too much add a single leading zero back on
			if (strDataValue.Length == 0)
			{
				strDataValue = "0.";
			}
			else if (strDataValue[0] == '.')
			{
				strDataValue = '0' + strDataValue;
			}

            // Set the display size.  Note that we have to compensate for the decimal point 
            // in the string value.  The target width does not take this into account
            if (strDataValue.Length > nTargetStringLength && targetFormat.FloatingDecimal)
            {
                // We need to see if we can remove any decimal digits - Note that we
				// have to be careful that we do not remove too many digits
                if (targetFormat.NumDecimalDIgits > 0 )
                {
					int nExcessDigits = strDataValue.Length - nTargetStringLength;

					if (nExcessDigits > targetFormat.NumDecimalDIgits)
					{
						strDataValue = strDataValue.Substring(0, strDataValue.Length - targetFormat.NumDecimalDIgits);
					}
					else
					{
						strDataValue = strDataValue.Substring(0, nTargetStringLength);
					}
                }
			}
			
			// Is the string still too long? If so, we need to remove the most significant digits to mimic
			// the device's display
            if (strDataValue.Length > nTargetStringLength)
            {
				int nStartingIndex = strDataValue.Length - nTargetStringLength;

				strDataValue = strDataValue.Substring( nStartingIndex );
			}

            // Do a final check to see if the current data value is smaller than the target display width
			if (strDataValue.Length < nTargetStringLength)
            {
                // If we don't have enough digits that is OK unless they requested leading zeros, in which case we need to pad
                if (targetFormat.LeadingZeros)
                {
                    // Add leading zeros until the string matches the target width
                    strDataValue = strDataValue.PadLeft(nTargetStringLength, '0');
                }
            }

            return strDataValue;
        }

        #endregion

        #region Members

        /// <summary>
        /// The original display item data is maintained for derived classes to intpret
        /// if necessary
        /// </summary>
        private byte[] m_abyItemData;

        private byte m_byUpperAddress;
        private byte m_byLowerAddress;
        private Boolean m_boolEndOfFile;
        private ItronDevice.DisplayMode m_DisplayType;
        private byte m_byDisplayID;
        private byte m_byRegisterType;
        private SCSDisplayClass m_byRegisterClass;
        private byte m_byTOURate;

        #endregion
    }
}
