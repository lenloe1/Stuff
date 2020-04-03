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
//                              Copyright © 2008 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Display;
using Itron.Metering.Progressable;
using Itron.Metering.TOU;

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// This class represents a MIF for an SCS device.
    /// </summary>
    public class SCSMIF: MIF
    {
        #region Constants

        private const int DISPLAY_NORMAL_COUNT = 408;
        private const int DISPLAY_ALTERNATE_COUNT = 409;
        private const int DISPLAY_TEST_COUNT = 410;
        private const int DISPLAY_ENERGY_FLOAT_DECIMAL_POINT = 17202;
        private const int DISPLAY_ENERGY_LEADING_ZEROS = 17203;
        private const int DISPLAY_ENERGY_WIDTH = 17204;
        private const int DISPLAY_ENERGY_RIGHT_OF_DECIMAL = 17205;
        private const int DISPLAY_DEMAND_FLOAT_DECIMAL_POINT = 17206;
        private const int DISPLAY_DEMAND_LEADING_ZEROS = 17207;
        private const int DISPLAY_DEMAND_WIDTH = 17208;
        private const int DISPLAY_DEMAND_RIGHT_OF_DECIMAL = 17209;
        private const int DISPLAY_CUM_FLOAT_DECIMAL_POINT = 17210;
        private const int DISPLAY_CUM_LEADING_ZEROS = 17211;
        private const int DISPLAY_CUM_WIDTH = 17212;
        private const int DISPLAY_CUM_RIGHT_OF_DECIMAL = 17213;
        private const int DISPLAY_REGISTER_ICONS = 422;
        private const int RECORD_SIZE = 256;
        private const int END_OF_DISPLAY = 0xFF;
        private const int DISP_SETTINGS_TYPE_COUNT = 3;
        private const int REG_TYPE_CLASS_MASK = 0x0F;
        private const int REG_TYPE_SHIFT = 4;
        private const int DATE_CLASS = 8;
        private const int NEXT_DISPLAY_ITEM_OFFSET = 4;
        private const int REV_ENG_DISP_PROGRESS_STEPS = 4;
        private const int ENERGY = 0;
        private const int DEMAND = 1;
        private const int CUMULATIVE = 2;
        private const string HEAD_RECORD_ID = "HEAD";
        private const int TOU_MONDAY_SCHED = 1807;
        private const int TOU_TUESDAY_SCHED = 1808;
        private const int TOU_WEDNESDAY_SCHED = 1809;
        private const int TOU_THURSDAY_SCHED = 1810;
        private const int TOU_FRIDAY_SCHED = 1811;
        private const int TOU_SATURDAY_SCHED = 1812;
        private const int TOU_SUNDAY_SCHED = 1813;
        

        #endregion

        #region Definitions

        /// <summary>
        /// The SCS display types.
        /// </summary>
        private enum DisplayType
        {
            Normal = 0,
            Alternate = 1,
            Test = 2
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="strFileName">The full path for the MIF.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 jrf 9.50           Created.
        //
        public SCSMIF(string strFileName)
            : base(strFileName)
        {
            m_DisplaySchedule = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets the display schedule from the MIF.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 jrf 9.50           Created.
        //
        public virtual Display.Display DisplaySchedule
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Returns the TOU schedule from the MIF.
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/30/08	jrf	9.50           Created
        //
        public override CTOUSchedule TOUSchedule
        {
            get
            {
                if (null == m_TOUSchedule)
                {
                    ReverseEngineerTOUSchedule();
                    RetrieveSCSTypicalWeek();
                }

                return m_TOUSchedule;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This method reverse engineers an SCS display schedule.
        /// </summary>
        /// <param name="iBaseAddress">The base address for the SCS device.</param>
        /// <param name="iDisplayAddress">The address of the display schedule.</param>
        /// <returns>The reverse engineered display schedule.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/10/08 jrf 9.50           Created.
        //
        protected void ReverseEngineerDisplaySchedule(int iBaseAddress, int iDisplayAddress)
        {
            int iDisplayCount = 0;
            object objValue = null;
            
            OnShowProgress(new ShowProgressEventArgs(1, REV_ENG_DISP_PROGRESS_STEPS, "", "Reverse Engineering Display..."));

            m_DisplaySchedule = new Itron.Metering.Display.Display();

            m_DisplaySchedule.Description = m_rmStrings.GetString("REVERSE_ENGINEERED_FROM") + Path.GetFileName(FileName);
            
            //Get the general formatting values for display items.
            RetreiveDisplayFormatting();

            //Determine the date format
            FindDisplayDateFormat(iBaseAddress, iDisplayAddress);
            OnStepProgress(new ProgressEventArgs());

            if (true == RetrieveItem(DISPLAY_NORMAL_COUNT, ref objValue))
            {
                iDisplayCount = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);

                GetDisplayItems(DisplayType.Normal, iDisplayCount);
                OnStepProgress(new ProgressEventArgs());
            }

            if (true == RetrieveItem(DISPLAY_ALTERNATE_COUNT, ref objValue))
            {
                iDisplayCount = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);

                GetDisplayItems(DisplayType.Alternate, iDisplayCount);
                OnStepProgress(new ProgressEventArgs());
            }

            if (true == RetrieveItem(DISPLAY_TEST_COUNT, ref objValue))
            {
                iDisplayCount = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);

                GetDisplayItems(DisplayType.Test, iDisplayCount);
                OnStepProgress(new ProgressEventArgs());
            }

            m_DisplaySchedule.Type = Utilities.DeviceType.GetDeviceTypeString(DeviceType);

            OnHideProgress(new EventArgs());
   
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method retrieves general formatting values for the display items.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 jrf 9.50           Created.
        //
        private void RetreiveDisplayFormatting()
        {
            object objValue = null;

            if (true == RetrieveItem(DISPLAY_REGISTER_ICONS, ref objValue))
            {
                m_blnDisplayAnnunciatorEnabled = (bool)Convert.ChangeType(objValue, typeof(bool), CultureInfo.InvariantCulture);
            }

            if (true == RetrieveItem(DISPLAY_ENERGY_LEADING_ZEROS, ref objValue))
            {
                m_ablnDisplayLeadingZeroes[ENERGY] = (bool)Convert.ChangeType(objValue, typeof(bool), CultureInfo.InvariantCulture);
            }

            if (true == RetrieveItem(DISPLAY_ENERGY_FLOAT_DECIMAL_POINT, ref objValue))
            {
                m_ablnDisplayFloatingDecimal[ENERGY] = (bool)Convert.ChangeType(objValue, typeof(bool), CultureInfo.InvariantCulture);
            }

            if (true == RetrieveItem(DISPLAY_ENERGY_WIDTH, ref objValue))
            {
                m_aiDisplayDigitCount[ENERGY] = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);
            }

            if (true == RetrieveItem(DISPLAY_ENERGY_RIGHT_OF_DECIMAL, ref objValue))
            {
                m_aiDisplayDecimalDigitCount[ENERGY] = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);
            }

            if (true == RetrieveItem(DISPLAY_DEMAND_LEADING_ZEROS, ref objValue))
            {
                m_ablnDisplayLeadingZeroes[DEMAND] = (bool)Convert.ChangeType(objValue, typeof(bool), CultureInfo.InvariantCulture);
            }

            if (true == RetrieveItem(DISPLAY_DEMAND_FLOAT_DECIMAL_POINT, ref objValue))
            {
                m_ablnDisplayFloatingDecimal[DEMAND] = (bool)Convert.ChangeType(objValue, typeof(bool), CultureInfo.InvariantCulture);
            }

            if (true == RetrieveItem(DISPLAY_DEMAND_WIDTH, ref objValue))
            {
                m_aiDisplayDigitCount[DEMAND] = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);
            }

            if (true == RetrieveItem(DISPLAY_DEMAND_RIGHT_OF_DECIMAL, ref objValue))
            {
                m_aiDisplayDecimalDigitCount[DEMAND] = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);
            }

            if (true == RetrieveItem(DISPLAY_CUM_LEADING_ZEROS, ref objValue))
            {
                m_ablnDisplayLeadingZeroes[CUMULATIVE] = (bool)Convert.ChangeType(objValue, typeof(bool), CultureInfo.InvariantCulture);
            }

            if (true == RetrieveItem(DISPLAY_CUM_FLOAT_DECIMAL_POINT, ref objValue))
            {
                m_ablnDisplayFloatingDecimal[CUMULATIVE] = (bool)Convert.ChangeType(objValue, typeof(bool), CultureInfo.InvariantCulture);
            }

            if (true == RetrieveItem(DISPLAY_CUM_WIDTH, ref objValue))
            {
                m_aiDisplayDigitCount[CUMULATIVE] = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);
            }

            if (true == RetrieveItem(DISPLAY_CUM_RIGHT_OF_DECIMAL, ref objValue))
            {
                m_aiDisplayDecimalDigitCount[CUMULATIVE] = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);
            }
        }
        
        /// <summary>
        /// This method extracts the format used to display dates from the MIF file.
        /// </summary>
        /// <param name="iBaseAddress">The base address for the SCS device.</param>
        /// <param name="iDisplayAddress">The address of the display schedule.</param>
        /// <returns>The date format that was extracted.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/10/08 jrf 9.50           Created.
        //
        private void FindDisplayDateFormat(int iBaseAddress, int iDisplayAddress)
        {
            int iDataOffset = 0;
            int iCurrentPosition = 0;
            byte bytRegTypeClass = 0;
            int iRegType = 0;
            int iRegClass = 0;
            byte[] abytRecordIdentifier = null;
            //We have to go in and retrieve the date format from the file itself.  It is not retrievable with the 
            //device server.
            FileStream MIFStream = new FileStream(FileName, System.IO.FileMode.Open,
                System.IO.FileAccess.Read, System.IO.FileShare.Read);
            string strRecordID = HEAD_RECORD_ID;

            //Find the offset of the first data record.
            while (HEAD_RECORD_ID == strRecordID)
            {
                iDataOffset += RECORD_SIZE;
                abytRecordIdentifier = new byte[HEAD_RECORD_ID.Length];
                MIFStream.Seek(iDataOffset, System.IO.SeekOrigin.Begin);
                MIFStream.Read(abytRecordIdentifier, 0, HEAD_RECORD_ID.Length);
                strRecordID = new ASCIIEncoding().GetString(abytRecordIdentifier);

            }

            //Set the current file position to the display area
            iCurrentPosition = iDataOffset + iDisplayAddress - iBaseAddress;

            //Search through the display area until we find an item with 
            //a date format.
            do
            {
                MIFStream.Seek(iCurrentPosition, System.IO.SeekOrigin.Begin);
                bytRegTypeClass = (byte)MIFStream.ReadByte();

                if (END_OF_DISPLAY != bytRegTypeClass)
                {
                    iRegClass = REG_TYPE_CLASS_MASK & bytRegTypeClass;
                    iRegType = REG_TYPE_CLASS_MASK & (bytRegTypeClass >> REG_TYPE_SHIFT);

                    if (DATE_CLASS == iRegClass)
                    {
                        m_eDisplayDateFormat = (DateFormat)iRegType;
                        break;
                    }
                }

                iCurrentPosition += NEXT_DISPLAY_ITEM_OFFSET;

            } while (END_OF_DISPLAY != bytRegTypeClass);

            MIFStream.Close();
        }

        /// <summary>
        /// This method retrieves a list of diplay items from the device server.
        /// </summary>
        /// <param name="eDisplayType">The type of display items to retrieve.</param>
        /// <param name="iDisplayCount">The number of display items to retrieve.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/10/08 jrf 9.50           Created.
        //
        private void GetDisplayItems(DisplayType eDisplayType, int iDisplayCount)
        {
            object objQuantity = null;
            object objOccurenceType = null;
            object objPeakNum = null;
            object objTimeOfOccurence = null;
            object objIDCode = null;
            object objValue = null;
            OccurrenceType eOccurenceType = OccurrenceType.Current;
            DisplayItemSettings ItemSettings;
            DisplayScale QuantityScale = DisplayScale.Units;
            string strValue = null;
            short sMaxStringLength = 80;
            short sResponse = DEVICE_SERVER_SUCCESS;
            short sDisplayProgress = 0;
            short sPeakNumber = 0;
            short sIDCode = 0;
            int iQuantity = 0;
            int iOccurenceType = 0;
            int iIndex = 0;
            Collection<DisplayItemSettings> colDisplayItemSettings = null;

            switch (eDisplayType)
            {
                case DisplayType.Normal:
                    {
                        colDisplayItemSettings = m_DisplaySchedule.NormalDisplay;
                        break;
                    }
                case DisplayType.Alternate:
                    {
                        colDisplayItemSettings = m_DisplaySchedule.AlternateDisplay;
                        break;
                    }
                default:
                    {
                        colDisplayItemSettings = m_DisplaySchedule.TestDisplay;
                        break;
                    }
            }

            //Find each display item's settings.
            for (int i = 1; i <= iDisplayCount; i++)
            {
                sResponse = VirtualDevice.GetDisplayItem((short)eDisplayType, (short)i, ref objQuantity,
                    ref objOccurenceType, ref objValue, ref strValue, sMaxStringLength,
                    ref objPeakNum, ref objTimeOfOccurence, ref objIDCode, sDisplayProgress);

                if (DEVICE_SERVER_SUCCESS == sResponse && null != objQuantity
                    && null != objOccurenceType && null != objValue && null != strValue
                    && null != objPeakNum && null != objTimeOfOccurence && null != objIDCode)
                {
                    iQuantity = (int)Convert.ChangeType(objQuantity, typeof(int), CultureInfo.InvariantCulture);
                    iOccurenceType = (int)Convert.ChangeType(objOccurenceType, typeof(int), CultureInfo.InvariantCulture);
                    eOccurenceType = (OccurrenceType)iOccurenceType;
                    sPeakNumber = (short)Convert.ChangeType(objPeakNum, typeof(short), CultureInfo.InvariantCulture);
                    sIDCode = (short)Convert.ChangeType(objIDCode, typeof(short), CultureInfo.InvariantCulture);

                    //Extract the scale from the quantity code.
                    QuantityCode Quantity = QuantityCode.Create((uint)iQuantity);
                    ElectricalQuantityCode ElectricalQuantity = Quantity as ElectricalQuantityCode;

                    if (null != ElectricalQuantity)
                    {
                        if (ElectricalQuantityCode.ElectricalScale.Kilo == ElectricalQuantity.Scale)
                        {
                            QuantityScale = DisplayScale.Kilo;
                        }

                        //Now that we have the scale store the quantity in it's units value for the display schedule.
                        ElectricalQuantity.Scale = ElectricalQuantityCode.ElectricalScale.Units;
                        iQuantity = (int)ElectricalQuantity.Code;
                    }

                    ItemSettings = new DisplayItemSettings((uint)iQuantity, eOccurenceType);

                    ItemSettings.DisplayOrder = (uint)i;
                    
                    ItemSettings.IDCode = (uint)sIDCode;
                    ItemSettings.PeakNumber = (uint)sPeakNumber;

                    if (null != ElectricalQuantity && ElectricalQuantity.IsEnergyQuantity)
                    {
                        iIndex = ENERGY;
                    }
                    else if (null != ElectricalQuantity && ElectricalQuantity.IsDemandQuantity
                        && false == ElectricalQuantity.IsCumulativeQuantity)
                    {
                        iIndex = DEMAND;
                    }
                    else if (null != ElectricalQuantity && ElectricalQuantity.IsCumulativeQuantity)
                    {
                        iIndex = CUMULATIVE;
                    }

                    ItemSettings.LeadingZeroesDisplayed = m_ablnDisplayLeadingZeroes[iIndex];
                    ItemSettings.FloatingDecimalEnabled = m_ablnDisplayFloatingDecimal[iIndex];
                    ItemSettings.DigitCount = (uint)m_aiDisplayDigitCount[iIndex];
                    ItemSettings.DecimalDigitCount = (uint)m_aiDisplayDecimalDigitCount[iIndex];

                    //Only set the following values if we have an
                    //energy or demand quantity
                    if (null != ElectricalQuantity)
                    {
                        //Only set the scale if we have an electrical quantity.
                        ItemSettings.Scale = QuantityScale;

                        if (strValue.Contains("-"))
                        {
                            //Since we have a date reset the scale and set the date.
                            //Scale and date are stored in the same location in the 
                            //display schedule.  Resetting this value allows us to 
                            //know which one to use.
                            ItemSettings.TOODateDisplayed = true;
                            ItemSettings.Scale = DisplayScale.Units;
                            ItemSettings.DateDisplay = m_eDisplayDateFormat;
                        }

                        if (strValue.Contains(":"))
                        {
                            ItemSettings.TOOTimeDisplayed = true;
                        }

                        ItemSettings.AnnunciatorEnabled = m_blnDisplayAnnunciatorEnabled;
                    }
                    else
                    {
                        //Only set the date if we don't have an electrical quantity
                        ItemSettings.DateDisplay = m_eDisplayDateFormat;
                    }

                    colDisplayItemSettings.Add(ItemSettings);
                }
            }
        }

        
        /// <summary>
        /// This method retrieves the typical week used by the TOU schedule.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/01/08 jrf 9.50           Created.
        //
        private void RetrieveSCSTypicalWeek()
        {
            short sDisplayProgress = 0;
            short sResponse = DEVICE_SERVER_SUCCESS;
            object objValue = null;
            int iSunday = 0;
            int iMonday = 0;
            int iTuesday = 0;
            int iWednesday = 0;
            int iThursday = 0;
            int iFriday = 0;
            int iSaturday = 0;

            //Get the typical week
            sResponse = VirtualDevice.GetValue(TOU_SUNDAY_SCHED, ref objValue, sDisplayProgress);
            if (DEVICE_SERVER_SUCCESS == sResponse && null != objValue)
            {
                iSunday = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);
            }

            sResponse = VirtualDevice.GetValue(TOU_MONDAY_SCHED, ref objValue, sDisplayProgress);
            if (DEVICE_SERVER_SUCCESS == sResponse && null != objValue)
            {
                iMonday = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);
            }

            sResponse = VirtualDevice.GetValue(TOU_TUESDAY_SCHED, ref objValue, sDisplayProgress);
            if (DEVICE_SERVER_SUCCESS == sResponse && null != objValue)
            {
                iTuesday = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);
            }

            sResponse = VirtualDevice.GetValue(TOU_WEDNESDAY_SCHED, ref objValue, sDisplayProgress);
            if (DEVICE_SERVER_SUCCESS == sResponse && null != objValue)
            {
                iWednesday = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);
            }

            sResponse = VirtualDevice.GetValue(TOU_THURSDAY_SCHED, ref objValue, sDisplayProgress);
            if (DEVICE_SERVER_SUCCESS == sResponse && null != objValue)
            {
                iThursday = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);
            }

            sResponse = VirtualDevice.GetValue(TOU_FRIDAY_SCHED, ref objValue, sDisplayProgress);
            if (DEVICE_SERVER_SUCCESS == sResponse && null != objValue)
            {
                iFriday = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);
            }

            sResponse = VirtualDevice.GetValue(TOU_SATURDAY_SCHED, ref objValue, sDisplayProgress);
            if (DEVICE_SERVER_SUCCESS == sResponse && null != objValue)
            {
                iSaturday = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);
            }

            m_TOUSchedule.TypicalWeek[(int)eTypicalDay.SUNDAY] = m_TOUSchedule.NormalDays[iSunday];
            m_TOUSchedule.TypicalWeek[(int)eTypicalDay.MONDAY] = m_TOUSchedule.NormalDays[iMonday];
            m_TOUSchedule.TypicalWeek[(int)eTypicalDay.TUESDAY] = m_TOUSchedule.NormalDays[iTuesday];
            m_TOUSchedule.TypicalWeek[(int)eTypicalDay.WEDNESDAY] = m_TOUSchedule.NormalDays[iWednesday];
            m_TOUSchedule.TypicalWeek[(int)eTypicalDay.THURSDAY] = m_TOUSchedule.NormalDays[iThursday];
            m_TOUSchedule.TypicalWeek[(int)eTypicalDay.FRIDAY] = m_TOUSchedule.NormalDays[iFriday];
            m_TOUSchedule.TypicalWeek[(int)eTypicalDay.SATURDAY] = m_TOUSchedule.NormalDays[iSaturday];
        }

        #endregion

        #region Members

        /// <summary>
        /// The display schedule used in the MIF.
        /// </summary>
        protected Display.Display m_DisplaySchedule;

        private int[] m_aiDisplayDigitCount = new int[DISP_SETTINGS_TYPE_COUNT];
        private int[] m_aiDisplayDecimalDigitCount = new int[DISP_SETTINGS_TYPE_COUNT];
        private bool[] m_ablnDisplayLeadingZeroes = new bool[DISP_SETTINGS_TYPE_COUNT];
        private bool[] m_ablnDisplayFloatingDecimal = new bool[DISP_SETTINGS_TYPE_COUNT];
        private bool m_blnDisplayAnnunciatorEnabled = false;
        private DateFormat m_eDisplayDateFormat = DateFormat.MMDDYY;

        #endregion
    }
}
