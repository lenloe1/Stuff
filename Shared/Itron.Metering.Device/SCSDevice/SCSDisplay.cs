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
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Itron.Metering.Communications;
using Itron.Metering.Communications.SCS;
using Itron.Metering.Utilities;
using Itron.Metering.Progressable;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
  	/// <summary>
	/// The base class for all SCS devices. 
	/// </summary>
    /// <remarks>
    /// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ ---------------------------------------
	/// </remarks>
    public abstract partial class SCSDevice : ItronDevice
    {
        #region Properties

        /// <summary>
        /// Provides access to Normal Display List
        /// </summary>
        /// <returns>
        /// List of DisplayItems.  
        /// </returns> 
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  12/05/06 MAH  8.00.00			Created 
        /// </remarks>
        override public List<DisplayItem> NormalDisplayList
        {
            get
            {
                // In order to return the display values we need to do two things
                // First get the display list configuration and, second, get the current
                // values for each of the display items.  Since the display configuration
                // does not change, this can and should be cached

                if (null == m_NormalDisplayList)
                {
                    ReadDisplayConfigurations();
                }

                // This can be a particularly lengthy operation.  Be sure to show a progress indicator
                // as we read each item
                OnShowProgress(new ShowProgressEventArgs(1, m_NormalDisplayList.Count,
                                         "Retrieving normal display list items...", "Retrieving normal display list items..."));

                int nCurrentDisplayItem = 1;
                
                // Now we have to go get the values for each of the display lists
                foreach (DisplayItem displayItem in m_NormalDisplayList)
                {
                    OnStepProgress(new ProgressEventArgs("Reading item " + nCurrentDisplayItem.ToString(CultureInfo.InvariantCulture) + " of " + m_NormalDisplayList.Count.ToString(CultureInfo.InvariantCulture)));
                    nCurrentDisplayItem++;

                    if (displayItem is SCSDisplayItem)
                    {
                        ((SCSDisplayItem)displayItem).ReadCurrentValue(this);
                    }
                }

                return m_NormalDisplayList;
            }
        }

        /// <summary>
        /// Provides access to Alternate Display List
        /// </summary>
        /// <returns>
        /// List of DisplayItems.  
        /// </returns> 
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  12/08/06 MAH  8.00.00			Created 
        /// </remarks>
        override public List<DisplayItem> AlternateDisplayList
        {
            get
            {
                // In order to return the display values we need to do two things
                // First get the display list configuration and, second, get the current
                // values for each of the display items.  Since the display configuration
                // does not change, this can and should be cached

                if (null == m_AlternateDisplayList)
                {
                    ReadDisplayConfigurations();
                }

                // This can be a particularly lengthy operation.  Be sure to show a progress indicator
                // as we read each item
                OnShowProgress(new ShowProgressEventArgs(1, m_AlternateDisplayList.Count,
                                         "Retrieving alternate display list items...", "Retrieving alternate display list items..."));
 
                int nCurrentDisplayItem = 1;

                // Now we have to go get the values for each of the display lists
                foreach (DisplayItem displayItem in m_AlternateDisplayList)
                {
                    OnStepProgress(new ProgressEventArgs("Reading item " + nCurrentDisplayItem.ToString(CultureInfo.InvariantCulture) +
                                    " of " + m_AlternateDisplayList.Count.ToString(CultureInfo.InvariantCulture)));
                    nCurrentDisplayItem++;

                    if (displayItem is SCSDisplayItem)
                    {
                        ((SCSDisplayItem)displayItem).ReadCurrentValue(this);
                    }
                }

                return m_AlternateDisplayList;
            }
        }

        /// <summary>
        /// Provides access to Test mode Display List
        /// </summary>
        /// <returns>
        /// List of Display Items.  
        /// </returns> 
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  12/08/06 MAH  8.00.00			Created 
        /// </remarks>
        override public List<DisplayItem> TestDisplayList
        {
            get
            {
                // In order to return the display values we need to do two things
                // First get the display list configuration and, second, get the current
                // values for each of the display items.  Since the display configuration
                // does not change, this can and should be cached

                if (null == m_TestDisplayList)
                {
                    ReadDisplayConfigurations();
                }

                // This can be a particularly lengthy operation.  Be sure to show a progress indicator
                // as we read each item
                OnShowProgress(new ShowProgressEventArgs(1, m_TestDisplayList.Count,
                                         "Retrieving test mode display list items...", "Retrieving test mode display list items..."));

                int nCurrentDisplayItem = 1;

                // Now we have to go get the values for each of the display lists
                foreach (DisplayItem displayItem in m_TestDisplayList)
                {
                    OnStepProgress(new ProgressEventArgs("Reading item " + nCurrentDisplayItem.ToString(CultureInfo.InvariantCulture) +
                        " of " + m_TestDisplayList.Count.ToString(CultureInfo.InvariantCulture)));
                    nCurrentDisplayItem++;

                    if (displayItem is SCSDisplayItem)
                    {
                        ((SCSDisplayItem)displayItem).ReadCurrentValue(this);
                    }
                }

                return m_TestDisplayList;
            }
        }

        /// <summary>
        /// Provides access to the list of Editable Registers (Same as Normal Display for SCS)
        /// </summary>
        /// <returns>
        /// List of DisplayItems.  
        /// </returns> 
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  02/26/07 KRC  8.00.14			Created 
        /// </remarks>
        override public List<DisplayItem> EditableRegisterList
        {
            get
            {
                return NormalDisplayList;
            }
        }
        /// <summary>
        /// This property returns the display format to be used with 
        /// for all energy values
        /// </summary>
        internal SCSDisplayFormat EnergyFormat
        {
            get
            {
                if (!m_EnergyFormat.Cached)
                {
                    ReadDisplayFormats();
                }

                return m_EnergyFormat;
            }
        }

        /// <summary>
        /// This property returns the display format to be used with 
        /// for all demand values values - including present, previous, and
        /// maximum demands
        /// </summary>
        internal SCSDisplayFormat DemandFormat
        {
            get
            {
                if (!m_DemandFormat.Cached)
                {
                    ReadDisplayFormats();
                }

                return m_DemandFormat;
            }
        }

        /// <summary>
        /// This property returns the display format to be used with 
        /// for all cumulative demand values
        /// </summary>
        internal SCSDisplayFormat CumulativeFormat
        {
            get
            {
                if (!m_CumulativeFormat.Cached)
                {
                    ReadDisplayFormats();
                }

                return m_CumulativeFormat;
            }
        }

        /// <summary>
        /// This property returns that basepage address of the 
        /// display format settings for energy registers
        /// </summary>
        abstract protected int EnergyFormatAddress
        {
            get;
        }

        /// <summary>
        /// This property returns that basepage address of the 
        /// display options settings
        /// </summary>
        abstract protected int DisplayOptionsAddress
        {
            get;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// This method is responsible for retrieving the basepage address of any given
        /// meter display item - normal, alternate, or test mode.  SInce all basepage addresses
        /// are unique per meter type, this method is a pure abstract method.  Each 
        /// derived meter class will have its own implementation
        /// </summary>
        /// <param name="displayItem">The display item to look up</param>
        /// <returns>An integer representing the associated basepage address
        /// </returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        abstract internal int TranslateDisplayAddress(SCSDisplayItem displayItem);

        /// <summary>
        /// This method returns a user viewable description for a basepage address.  Since
        /// each meter type has a unique basepage layout, this method must be implemented
        /// within each device class.
        /// </summary>
        /// <param name="nBasepageAddress"></param>
        /// <returns>A string that describes the given display item
        /// </returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        abstract internal string GetDisplayItemDescription(int nBasepageAddress);

        /// <summary>
        /// This method is responsible for either retrieving or calculating the continuous
        /// cummulative demand value associated with the given display item.  Note
        /// that in some SCS devices this value is simply read from the meter yet in
        /// other cases, the value must be calculated from the cummulative and max
        /// demand values.  Since the actual implementation differs per device type, this
        /// method must be overriden for each derived device type
        /// </summary>
        /// <param name="displayItem">The display item to look up</param>
        /// <returns>A string representing the ccum value
        /// </returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/03/07 mah 8.00.00  N/A   Created
        /// </remarks>
        abstract internal String RetrieveCCumValue(SCSDisplayItem displayItem);

        /// <summary>
        /// This method returns a user viewable description for a given display item.  NOte that
        /// all derived device classes must override this method.  The base class implementation 
        /// here is only provided as a debugging tool should a derived class not be able to 
        /// successfully describe the given display item.
        /// </summary>
        /// <param name="displayItem">The display item </param>
        /// <returns>A string that describes the given display item
        /// </returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual internal string GetDisplayItemDescription(SCSDisplayItem displayItem)
        {
            String strDescription;

            // The following list are special case display items that are common to all SCS devices

            if (displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.FixedBCD && displayItem.RegisterType == 0)
            {
                strDescription = "Segment Test";
            }
            else if (displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.DateValue && displayItem.LowerAddress == 0)
            {
                strDescription = "Current Date";
            }
            else if (displayItem.RegisterClass == SCSDisplayItem.SCSDisplayClass.TimeValue && displayItem.LowerAddress == 0x06)
            {
                strDescription = "Current Time";
            }
            else
            {
                strDescription = "Class = " + displayItem.RegisterClass.ToString() +
                                    " Type = " + displayItem.RegisterType.ToString(CultureInfo.InvariantCulture) +
                                    " @ 0x" + TranslateDisplayAddress(displayItem).ToString("X", CultureInfo.InvariantCulture);
            }

            return strDescription;
        }

        /// <summary>
        /// This method a 7 byte fixed floating point BCD value from an SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the value cannot be retreived from the meter.
        /// </exception>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual internal String ReadFixedBCDValue(int nBasepageAddress, int nDecimalBytes, int nLength)
        {
            String strValue = "";
            byte[] byEnergyValue;

             SCSProtocolResponse ProtocolResponse = m_SCSProtocol.Upload(nBasepageAddress, nLength, out byEnergyValue);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                strValue = BCD.FixedBCDtoString(byEnergyValue, nDecimalBytes, nLength);
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,  ProtocolResponse,  nBasepageAddress,  "Fixed BCD Value");
                throw scsException;
            }

            return strValue;
        }

        /// <summary>
        /// This will set the given value as a Fixed BCD
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the value cannot be retreived from the meter.
        /// </exception>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/26/07 KRC 8.00.09        Adding Edit Registers
        /// </remarks>
        virtual internal ItronDeviceResult SetFixedBCDValue(int nBasepageAddress, int nDeciamalBytes, int nLength, string strValue)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            byte[] byBCDValue;
            double dblValue;

            // Get the double out of the string (This allows us to format the value as needed when doing the conversion)
            dblValue = double.Parse(strValue, CultureInfo.CurrentCulture);
            byBCDValue = BCD.DoubleToFixedBCD(dblValue, nLength, nDeciamalBytes);

            //Now that we have the byte array, send it to the meter.
            SCSProtocolResponse ProtocolResponse = m_SCSProtocol.Download(nBasepageAddress, nLength, ref byBCDValue);

            if (SCSProtocolResponse.SCS_CAN == ProtocolResponse)
            {
                Result = ItronDeviceResult.SECURITY_ERROR;
            }
            else if (SCSProtocolResponse.SCS_NAK == ProtocolResponse)
            {
                Result = ItronDeviceResult.ERROR;
            }

            return Result;
        }
        /// <summary>
        /// This method a 4 byte floating point BCD value from an SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the value cannot be retreived from the meter.
        /// </exception>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual internal String ReadFloatingBCDValue(int nBasepageAddress, int nLength)
        {
            String strValue = "";
            byte[] byBCDValue;

             SCSProtocolResponse ProtocolResponse = m_SCSProtocol.Upload(nBasepageAddress, nLength, out byBCDValue);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                strValue = BCD.FloatingBCDtoString(ref byBCDValue, nLength);
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,  ProtocolResponse,  nBasepageAddress,  "Floating BCD Value");
                throw scsException;
            }

            return strValue;
        }

        /// <summary>
        /// This method a 4 byte floating point BCD value from an SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the value cannot be retreived from the meter.
        /// </exception>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/26/07 KRC 8.00.09        Adding Edit Registers
        /// </remarks>
        virtual internal ItronDeviceResult SetFloatingBCDValue(int nBasepageAddress, int nLength, string strValue)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            byte[] byBCDValue;
            double dblValue;

            // Get the double out of the string (This allows us to format the value as needed when doing the conversion)
            dblValue = double.Parse(strValue, CultureInfo.CurrentCulture);
            byBCDValue = BCD.DoubleToFloatingBCD(dblValue, nLength);

            // Now that we have the byte array, we can send it to the meter.
            SCSProtocolResponse ProtocolResponse = m_SCSProtocol.Download(nBasepageAddress, nLength, ref byBCDValue);
            
            if (SCSProtocolResponse.SCS_CAN == ProtocolResponse)
            {
                Result = ItronDeviceResult.SECURITY_ERROR;
            }
            else if (SCSProtocolResponse.SCS_NAK == ProtocolResponse)
            {
                Result = ItronDeviceResult.ERROR;
            }


            return Result;
        }

        /// <summary>
        /// This method reads a integer BCD value from an SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the value cannot be retreived from the meter.
        /// </exception>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual internal int ReadBCDInteger(int nBasepageAddress, int nLength)
        {
            int nValue = 0;
            byte[] byBCDValue;

             SCSProtocolResponse ProtocolResponse = m_SCSProtocol.Upload(nBasepageAddress, nLength, out byBCDValue);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                nValue = BCD.BCDtoInt(ref byBCDValue, nLength);
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,  ProtocolResponse, nBasepageAddress, "Integer BCD Value");
                throw scsException;
            }

            return nValue;
        }

        /// <summary>
        /// This method reads and translates a single nibble from an SCS device
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the value cannot be retreived from the meter.
        /// </exception>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual internal int ReadNibble(int nBasepageAddress, bool boolReadMSN)
        {
            int nValue = 0;
            byte[] byValue;

            SCSProtocolResponse ProtocolResponse = m_SCSProtocol.Upload(nBasepageAddress, 1, out byValue);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                if (boolReadMSN)
                {
                    nValue = (byValue[0] >> 4);
                }
                else
                {
                    nValue = (byValue[0] & 0x0F);
                }
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,  ProtocolResponse,  nBasepageAddress, "Read nibble");
                throw scsException;
            }

            return nValue;
        }

        /// <summary>
        /// This method a 4 byte floating point BCD value from an SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the value cannot be retreived from the meter.
        /// </exception>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual internal String ReadASCIIValue(int nBasepageAddress, int nLength)
        {
            String strAsciiValue = "";
            ASCIIEncoding Encoder = new ASCIIEncoding();  // For converting byte array to string	
            byte[] byASCIIValue;

            SCSProtocolResponse ProtocolResponse = m_SCSProtocol.Upload(nBasepageAddress, nLength, out byASCIIValue);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                char[] chrNull = { '\0' }; 

                strAsciiValue = Encoder.GetString(byASCIIValue, 0, nLength);
                strAsciiValue = strAsciiValue.Trim(chrNull);
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,
                    ProtocolResponse,
                    nBasepageAddress,
                    "ASCII Value");
                throw scsException;
            }

            return strAsciiValue;
        }

        /// <summary>
        /// This method reads a 2 byte BCD time from an SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the value cannot be retreived from the meter.
        /// </exception>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual internal void ReadBCDTime(int nBasepageAddress, out int nHour, out int nMinute)
        {
            byte[] byBCDValue;

            // Upload the date string
             SCSProtocolResponse ProtocolResponse = m_SCSProtocol.Upload(nBasepageAddress, 2, out byBCDValue);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                nHour = (int)BCD.BCDtoByte(byBCDValue[0]);
                nMinute = (int)BCD.BCDtoByte(byBCDValue[1]);
            }
            else
            {
                nHour = 0;
                nMinute = 0;

                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U, ProtocolResponse, nBasepageAddress, "Display time");

                throw scsException;
            }
        }

        /// <summary>
        /// This method reads a 3 byte BCD time from an SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the value cannot be retreived from the meter.
        /// </exception>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual internal void ReadBCDTime(int nBasepageAddress, out int nHour, out int nMinute, out int nSecond)
        {
            byte[] byBCDValue;

            // Upload the date string
             SCSProtocolResponse ProtocolResponse = m_SCSProtocol.Upload(nBasepageAddress, 3, out byBCDValue);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                nHour = (int)BCD.BCDtoByte(byBCDValue[0]);
                nMinute = (int)BCD.BCDtoByte(byBCDValue[1]);
                nSecond = (int)BCD.BCDtoByte(byBCDValue[2]);
            }
            else
            {
                nHour = 0;
                nMinute = 0;
                nSecond = 0;

                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U, ProtocolResponse, nBasepageAddress, "Display time");

                throw scsException;
            }
        }

        /// <summary>
        /// This method reads a 2 byte BCD date from an SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the value cannot be retreived from the meter.
        /// </exception>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual internal void ReadBCDDate(int nBasepageAddress, out int nMonth, out int nDay)
        {
            byte[] byBCDValue;

            // Upload the date string
             SCSProtocolResponse ProtocolResponse = m_SCSProtocol.Upload(nBasepageAddress, 2, out byBCDValue);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                nMonth = (int)BCD.BCDtoByte(byBCDValue[0]);
                nDay = (int)BCD.BCDtoByte(byBCDValue[1]);
            }
            else
            {
                nMonth = 0;
                nDay = 0;

                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U, ProtocolResponse, nBasepageAddress, "Display date");

                throw scsException;
            }
        }

        /// <summary>
        /// This method reads a 3 byte BCD date from an SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the value cannot be retreived from the meter.
        /// </exception>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual internal void ReadBCDDate(int nBasepageAddress, out int nYear, out int nMonth, out int nDay)
        {
            byte[] byBCDValue;

            // Upload the date string
             SCSProtocolResponse ProtocolResponse = m_SCSProtocol.Upload(nBasepageAddress, 3, out byBCDValue);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                nYear = (int)BCD.BCDtoByte(byBCDValue[0]);
                nMonth = (int)BCD.BCDtoByte(byBCDValue[1]);
                nDay = (int)BCD.BCDtoByte(byBCDValue[2]);
            }
            else
            {
                nYear = 0;
                nMonth = 0;
                nDay = 0;

                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U, ProtocolResponse, nBasepageAddress, "Display date");

                throw scsException;
            }
        }

        /// <summary>
        /// This method reorders an array of bytes read from the VECTRON so 
        /// that they are in the right order to be converted to floats.
        /// </summary>
        /// <param name="byFloatBytes">a byte array to </param>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 jrf 7.30.00  N/A   Created
        /// 
        protected void ReorderFloats(ref byte[] byFloatBytes)
        {
            byte[] tempValue = new byte[4];

            for (int iIndex = 0; iIndex + 4 <= byFloatBytes.Length; iIndex += 4)
            {
                tempValue[0] = byFloatBytes[iIndex + 3];
                tempValue[1] = byFloatBytes[iIndex + 2];
                tempValue[2] = byFloatBytes[iIndex + 1];
                tempValue[3] = byFloatBytes[iIndex + 0];

                byFloatBytes[iIndex + 0] = tempValue[0];
                byFloatBytes[iIndex + 1] = tempValue[1];
                byFloatBytes[iIndex + 2] = tempValue[2];
                byFloatBytes[iIndex + 3] = tempValue[3];
            }

        } // End ReorderFloats()

        /// <summary>
        /// This method reads a 4 byte floating point value from an SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the value cannot be retreived from the meter.
        /// </exception>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/13/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual internal float ReadFloatingPointValue(int nBasepageAddress)
        {
            byte[] byValue;
            float fltValue = (float)0.0;

            SCSProtocolResponse ProtocolResponse = m_SCSProtocol.Upload(nBasepageAddress, 4, out byValue);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                ReorderFloats(ref byValue);
                MemoryStream TempStream = new MemoryStream(byValue);
                BinaryReader TempBReader = new BinaryReader(TempStream);

                // Interpret the toolbox data
                fltValue = TempBReader.ReadSingle();
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U, ProtocolResponse, nBasepageAddress, "Floating point value");

                throw scsException;
            }

            return fltValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/13/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual internal string ReadPresentDemandValue(SCSDisplayItem displayItem)
        {
            return ReadFloatingBCDValue(TranslateDisplayAddress(displayItem), 4);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/13/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual internal string ReadPreviousDemandValue(SCSDisplayItem displayItem)
        {
            return ReadFloatingBCDValue(TranslateDisplayAddress(displayItem), 4);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This method reads and caches all of the display 
        /// format settings.  Note that this may involve more
        /// than one basepage field.  The goal is to read all of the display
        /// formats in a single shot and cache the values so that they do
        /// not have to be re-read for each display item.  
        /// 
        /// This method must be overriden if the meter does not use the
        /// display format structures common to the MT200, CENTRON, and
        /// VECTRON meters
        /// </summary>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual protected void ReadDisplayFormats()
        {
            byte[] byDisplayFormats;
            SCSProtocolResponse ProtocolResponse;
            
            // In order to be efficient we will try to read & cache all three display formats at the same time

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading display formats");

            ProtocolResponse = m_SCSProtocol.Upload(EnergyFormatAddress, 3, out byDisplayFormats);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                m_EnergyFormat.MeterDisplayFormat = byDisplayFormats[0];
                m_DemandFormat.MeterDisplayFormat = byDisplayFormats[1];
                m_CumulativeFormat.MeterDisplayFormat = byDisplayFormats[2];
            }

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                ProtocolResponse = m_SCSProtocol.Upload(DisplayOptionsAddress, 1, out byDisplayFormats);

                if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
                {
                    if ((byDisplayFormats[0] & 0x10) != 0)
                    {
                        m_DemandFormat.Units = SCSDisplayFormat.DisplayUnits.Units;
                        m_CumulativeFormat.Units = SCSDisplayFormat.DisplayUnits.Units;
                    }
                    else
                    {
                        m_DemandFormat.Units = SCSDisplayFormat.DisplayUnits.Kilo;
                        m_CumulativeFormat.Units = SCSDisplayFormat.DisplayUnits.Kilo;
                    }

                    if ((byDisplayFormats[0] & 0x80) != 0)
                    {
                        m_EnergyFormat.Units = SCSDisplayFormat.DisplayUnits.Units;
                    }
                    else
                    {
                        m_EnergyFormat.Units = SCSDisplayFormat.DisplayUnits.Kilo;
                    }

                }
            }

            if (SCSProtocolResponse.SCS_ACK != ProtocolResponse)
            {
                SCSException scsException = new SCSException(SCSCommands.SCS_U,
                    ProtocolResponse, EnergyFormatAddress, "Display Formats");

                throw scsException;
            }
        }

        /// <summary>
        /// Returns a list of values currently shown on the meter's normal mode display
        /// </summary>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/05/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual protected void ReadDisplayConfigurations()
        {
            byte[] byDisplayItem;
            SCSProtocolResponse ProtocolResponse;

            m_NormalDisplayList = new List<DisplayItem>();
            m_AlternateDisplayList = new List<DisplayItem>();
            m_TestDisplayList = new List<DisplayItem>();

            // All SCS devices MUST have at least one item in the normal display
            // list.  Read each of the items in the list until we reach either an end 
            // of table indicator or we find the first item in the alternate display list

            int nDisplayItemAddress = DisplayTableAddress;
            Boolean boolEndOfList = false;
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading display table");

            while (!boolEndOfList)
            {
                // Read the entire display item from the meter

                ProtocolResponse = m_SCSProtocol.Upload(nDisplayItemAddress, SCS_DISPLAYITEM_LENGTH, out byDisplayItem);

                if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
                {
                    SCSDisplayItem displayItem = CreateDisplayItem(ref byDisplayItem, 0, false);
                  
                    if (!displayItem.EndOfFIle)
                    {
                        if (displayItem.DisplayType == ItronDevice.DisplayMode.NORMAL_MODE)
                        {
                            m_NormalDisplayList.Add(displayItem);
                        }
                        else
                        {
                            m_AlternateDisplayList.Add(displayItem);
                        }

                        // set the address of the next item

                        nDisplayItemAddress += SCS_DISPLAYITEM_LENGTH;
                    }
                    else
                    {
                        boolEndOfList = true;
                        nDisplayItemAddress += 1;  // The end of list is only one byte long - reset the address to point to the next item
                    }
                }
                else
                {
                    SCSException scsException = new SCSException(SCSCommands.SCS_U,
                        ProtocolResponse, nDisplayItemAddress, "Display table item");

                    throw scsException;
                }
            }

            // The test mode list immediately follows the normal and alternate mode lists and has the same 
            // format

           boolEndOfList = false;
           m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading test mode display table");

            while (!boolEndOfList)
            {
                // Read the entire display item from the meter

                ProtocolResponse = m_SCSProtocol.Upload(nDisplayItemAddress, SCS_DISPLAYITEM_LENGTH, out byDisplayItem);

                if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
                {
                    SCSDisplayItem displayItem = new SCSDisplayItem(ref byDisplayItem, 0, true);

                    if (!displayItem.EndOfFIle)
                    {
                        m_TestDisplayList.Add(displayItem);

                        // set the address of the next item

                        nDisplayItemAddress += SCS_DISPLAYITEM_LENGTH;
                    }
                    else
                    {
                        boolEndOfList = true;
                    }
                }
                else
                {
                    SCSException scsException = new SCSException(SCSCommands.SCS_U,
                        ProtocolResponse, nDisplayItemAddress, "Test Mode display item");

                    throw scsException;
                }
            }
        }
        
        /// <summary>
        /// This method will create the appropriate display item type
        ///  (It can be overridden if needed)
        /// </summary>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/31/07 KRC 8.10.00  N/A   Need to create unique display item for CENTRON to handle CCum Edit
        /// </remarks>
        virtual protected SCSDisplayItem CreateDisplayItem(ref byte[] byDisplayTable, int nTableOffset, bool boolTestMode)
        {
            return new SCSDisplayItem(ref byDisplayTable, nTableOffset, boolTestMode);
        }

        #endregion

        #region Members

        private SCSDisplayFormat m_EnergyFormat;
        private SCSDisplayFormat m_DemandFormat;
        private SCSDisplayFormat m_CumulativeFormat;

        #endregion
    }


    /// <summary>
    /// This class represents the register formatting rules configured in a meter.
    /// These rules should be used to format displayable values to be consistent
    /// with the meter's display
    /// </summary>
    /// <remarks >
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 12/07/06 mah 8.00.00  N/A   Created
    /// </remarks>
    public class SCSDisplayFormat : DataCache
    {
        #region Definitions

        /// <summary>
        /// 
        /// </summary>
        public enum DisplayUnits
        {
            /// <summary>
            /// 
            /// </summary>
            Units = 0,
            /// <summary>
            /// 
            /// </summary>
            Kilo = 1
        };

        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor
        /// </summary>
        public SCSDisplayFormat( ) : base()
        {
            m_byDisplayFormat = 0;
            m_DisplayUnits = DisplayUnits.Kilo;  // by default all units are scaled to kilo
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Used to set the display format as read directly from the meter.  Note
        /// that this property also sets the cached flag to indicate that we do not
        /// need to read this value from the meter again
        /// </summary>
        public Byte MeterDisplayFormat
        {
            set
            {
                m_boolCached = true;
                m_byDisplayFormat = value;
            }
        }

        /// <summary>
        /// If true, the decimal point can shift right to display the most
        /// significant digits of a number that exceeds the normal displayable
        /// size
        /// </summary>
        public bool FloatingDecimal
        {
            get
            {
                return ((m_byDisplayFormat & 0x80) != 0);
            }
        }

        /// <summary>
        /// The number of digits shown on the register's display
        /// </summary>
        public int Width
        {
            get
            {
                return ((m_byDisplayFormat & 0x70) >> 4);
            }
        }

        /// <summary>
        /// Boolean value to indicate whether or not leading zeros are to be displayed
        /// </summary>
        public bool LeadingZeros
        {
            get
            {
                return ((m_byDisplayFormat & 0x08) != 0);
            }
        }

        /// <summary>
        /// The number of digits to be shown to the right of the decimal point
        /// </summary>
        public int NumDecimalDIgits
        {
            get
            {
                return (m_byDisplayFormat & 0x07);
            }
        }

        /// <summary>
        /// This property indicates how values will be scaled on the display
        /// </summary>
        public DisplayUnits Units
        {
            get
            {
                return (m_DisplayUnits);
            }
            set
            {
                m_DisplayUnits = value;
            }
        }

        #endregion

        #region Members
        private Byte m_byDisplayFormat;
        private DisplayUnits m_DisplayUnits;
        #endregion
    }
}
