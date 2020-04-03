///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//  storage or retrieval system without the permission in writing from Itron, Inc.
//
//                           Copyright © 2007 - 2009
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;
using Itron.Common.C1219Tables.CentronII;
using Itron.Common.C1219Tables.ANSIStandardII;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Progressable;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    partial class CENTRON2_MONO : IValidateProgram
    {
        #region Constants

        /// <summary>
        /// Constant Describing the Firmware Version for SR 2.0 SP5
        /// </summary>
        public const float VERSION_1 = 1.000F;

        private const uint ENERGY_LID_BASE = 0x14000080;

        #endregion

        #region IValidateProgram Members

        /// <summary>
        /// Returns a list of items that are not consistent between the configuration
        /// of the program and the device.
        /// </summary>
        /// <param name="strProgramName">The name of the program to validate against.</param>
        /// <returns>
        /// A list of items that failed the validation. Returns an empty list if
        /// all items match.
        /// </returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created

        public virtual List<ProgramValidationItem> ValidateProgram(string strProgramName)
        {
            PSEMResponse Response = PSEMResponse.Ok;

            List<ProgramValidationItem> InvalidItemsList = new List<ProgramValidationItem>();
            List<CENTRONIIEDLValidationItem> ItemsToValidate = GetValidationList();
            List<ushort> ValidationTablesToRead = GetValidationTablesToRead();

            FileStream EDLFileStream = new FileStream(strProgramName, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlTextReader EDLReader = new XmlTextReader(EDLFileStream);

            CentronTables ProgramTables = new CentronTables();
            CentronTables MeterTables = new CentronTables();

            DateTime dtCurrentSeasonStart;
            DateTime dtNextSeasonStart;
            bool bDemandReset;
            bool bSelfRead;

			OnShowProgress(new ShowProgressEventArgs(1, ValidationTablesToRead.Count));

            // Read the data from the meter.
            // NOTE: ReadTable is defined in CENTRON_AMI_ICreateEDL so this will not compile if
            //       that file is not included. We may want to move this method eventually, but 
            //       we are keeping Interfaces seperate in case we wish to support OpenWay in HH-Pro
            foreach (ushort TableID in ValidationTablesToRead)
            {
                OnStepProgress(new ProgressEventArgs());

                if (Response == PSEMResponse.Ok)
                {
                    if (MeterTables.IsTableKnown(TableID) && Table00.IsTableUsed(TableID) && (TableID == 0 || MeterTables.GetTableLength(TableID) > 0))
                    {
                        Response = ReadTable(TableID, ref MeterTables);
                    }
                }
            }

            if (Response != PSEMResponse.Ok)
            {
                throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response, "Error reading device for validation.");
            }
            else
            {
                // Load the EDL file.
                ProgramTables.LoadEDLFile(EDLReader);

                // Update the TOU if this program is using the new TOU schedules
                try
                {
                    ProgramTables.UpdateTOUSeasonFromStandardTables(DeviceTime, 0,
                        out dtCurrentSeasonStart, out bDemandReset, out bSelfRead, out dtNextSeasonStart);
                }
                catch (Exception)
                {
                    // We don't really care whether or not this command fails as we will see the results
                    // when validating
                }

                // Compare the values
                foreach (CENTRONIIEDLValidationItem Item in ItemsToValidate)
                {
                    ProgramValidationItem InvalidItem;

                    // Only compare items where the meter's FW version is greater than or equal to 
                    // the minimum required for that item. 
                    if (VersionChecker.CompareTo(FWRevision, Item.MinFWVersion) >= 0)
                    {
                        // We need to handle the display items differently than the rest of the items since
                        // there can be a different number of Normal and Test items.
                        if (RequiresSpecialHandling(Item.Item) == false)
                        {
                            InvalidItem = ValidateItem(Item, MeterTables, ProgramTables);

                            // Only add the item if it does not match.
                            if (null != InvalidItem)
                            {
                                if (!ValidationException(InvalidItem))
                                {
                                    // If the meter is not a Validation Exception then add it
                                    InvalidItemsList.Add(InvalidItem);
                                }
                            }
                        }
                        else
                        {
                            InvalidItemsList.AddRange(HandleSpecialCases(Item.Item, MeterTables, ProgramTables));
                        }
                    }
                }
            }

            return InvalidItemsList;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Determines whether or not an item requires special handling.
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>True if the item requires special handling. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/17/09 RCG	2.20.08	136274 Created

        protected virtual bool RequiresSpecialHandling(long item)
        {
            bool bRequired = false;

            switch(item)
            {
                case (long)CentronTblEnum.MFGTBL0_DISPLAY_ITEMS:
                {
                    bRequired = true;
                    break;
                }
                case (long)StdTableEnum.STDTBL123_EXCEPTION_REPORT:
                {
                    bRequired = true;
                    break;
                }
            }

            return bRequired;
        }

        /// <summary>
        /// Handles and special cases.
        /// </summary>
        /// <param name="item">The item to handle</param>
        /// <param name="meterTables">The table structure for the meter.</param>
        /// <param name="programTables">The table structure for the program.</param>
        /// <returns>The list of invalid items for the special case.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/17/09 RCG	2.20.08	136274 Created

        protected virtual List<ProgramValidationItem> HandleSpecialCases(long item, CentronTables meterTables, CentronTables programTables)
        {
            List<ProgramValidationItem> InvalidItems = new List<ProgramValidationItem>();

            switch (item)
            {
                case (long)CentronTblEnum.MFGTBL0_DISPLAY_ITEMS:
                {
                    InvalidItems = GetInvalidDisplayItems(meterTables, programTables);
                    break;
                }
                case (long)StdTableEnum.STDTBL123_EXCEPTION_REPORT:
                {
                    InvalidItems = GetInvalidExceptionItems(meterTables, programTables);
                    break;
                }
            }

            return InvalidItems;
        }

        /// <summary>
        /// This method determines if the Invalid Item is an Exceptional Item
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        protected virtual bool ValidationException(ProgramValidationItem Item)
        {
            bool bResult = false;

            if (0 == String.Compare(Item.Name, "Require Enhanced Security", StringComparison.CurrentCulture))
            {
                if (FWRevision < 2.0)
                {
                    if (null == Item.ProgramValue || 0 == String.Compare(Item.ProgramValue, "False", StringComparison.CurrentCulture))
                    {
                        // We are talking to a meter prior to SR 2.0, which means it does not support Enhanced Security.
                        //  Since the value in the program is False we do not care that the meter does not support it.
                        //  If the value in the program were True, we still throw a validation error, because they
                        //  were expecting to use Enhanced Security, but the meter does not support it.
                        bResult = true;
                    }
                }
            }

            return bResult;
        }

        /// <summary>
        /// Gets a list of invalid exception items.
        /// </summary>
        /// <param name="meterTables">The table structure for the meter.</param>
        /// <param name="programTables">The table structure for the program.</param>
        /// <returns>The list of invalid items.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/17/09 RCG	2.20.08	136274 Created
        // 03/26/10 jrf 2.40.28        Only validating table 123 if firmware is SP5.1 or higher.

        protected virtual List<ProgramValidationItem> GetInvalidExceptionItems(CentronTables meterTables, CentronTables programTables)
        {
            List<ProgramValidationItem> InvalidItems = new List<ProgramValidationItem>();
            ProgramValidationItem ValidationItem = null;

            object objMeterValue;
            object objProgramValue;

            ushort usMeterHosts;
            ushort usProgramHosts;
            ushort usMeterEvents;
            ushort usProgramEvents;

            ushort usMaxHosts;
            ushort usMaxEvents;

            // We shouldn't validate this if Table 123 is not cached in the program.
            if (programTables.IsAllCached(123) && VersionChecker.CompareTo(FWRevision, VERSION_1) > 0)
            {
                // Get the number of exception hosts
                meterTables.GetValue(StdTableEnum.STDTBL121_NBR_EXCEPTION_HOSTS, null, out objMeterValue);
                programTables.GetValue(StdTableEnum.STDTBL121_NBR_EXCEPTION_HOSTS, null, out objProgramValue);

                usMeterHosts = (ushort)objMeterValue;
                usProgramHosts = (ushort)objProgramValue;

                // Get the number of exception events
                meterTables.GetValue(StdTableEnum.STDTBL121_NBR_EXCEPTION_EVENTS, null, out objMeterValue);
                programTables.GetValue(StdTableEnum.STDTBL121_NBR_EXCEPTION_EVENTS, null, out objProgramValue);

                usMeterEvents = (ushort)objMeterValue;
                usProgramEvents = (ushort)objProgramValue;

                // Determine the maximum values.
                if (usMeterHosts >= usProgramHosts)
                {
                    usMaxHosts = usMeterHosts;
                }
                else
                {
                    usMaxHosts = usProgramHosts;
                }

                if (usMeterEvents >= usProgramEvents)
                {
                    usMaxEvents = usMeterEvents;
                }
                else
                {
                    usMaxEvents = usProgramEvents;
                }

                // Verify each of the hosts
                for (int iHost = 0; iHost < usMaxHosts; iHost++)
                {
                    bool meterValidHost = iHost < usMeterHosts;
                    bool programValidHost = iHost < usProgramHosts;                    

                    ushort[] meterEvents = null;
                    ushort[] programEvents = null;

                    int[] iIndexer = { iHost };
                    string strCategory = "Exception Host " + (iHost + 1).ToString(CultureInfo.CurrentCulture);

                    ValidationItem = ValidateItem(new CENTRONIIEDLValidationItem((long)StdTableEnum.STDTBL123_APTITLE_NOTIFY, iIndexer, "Exception Host Aptitle", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    ValidationItem = ValidateItem(new CENTRONIIEDLValidationItem((long)StdTableEnum.STDTBL123_MAX_NUMBER_OF_RETRIES, iIndexer, "Max Number of Retries", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    ValidationItem = ValidateItem(new CENTRONIIEDLValidationItem((long)StdTableEnum.STDTBL123_RETRY_DELAY, iIndexer, "Retry Delay", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    ValidationItem = ValidateItem(new CENTRONIIEDLValidationItem((long)StdTableEnum.STDTBL123_EXCLUSION_PERIOD, iIndexer, "Exclusion Period", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    if (meterValidHost == true)
                    {
                        meterEvents = new ushort[usMeterEvents];

                        for (int iEvent = 0; iEvent < usMeterEvents; iEvent++)
                        {
                            meterTables.GetValue(StdTableEnum.STDTBL123_EVENT_REPORTED, new int[] {iHost, iEvent}, out objMeterValue);
                            meterEvents[iEvent] = (ushort)objMeterValue;
                        }
                    }

                    if (programValidHost == true)
                    {
                        programEvents = new ushort[usProgramEvents];

                        for (int iEvent = 0; iEvent < usProgramEvents; iEvent++)
                        {
                            programTables.GetValue(StdTableEnum.STDTBL123_EVENT_REPORTED, new int[] { iHost, iEvent }, out objProgramValue);
                            programEvents[iEvent] = (ushort)objProgramValue;
                        }
                    }

                    for (int iEvent = 0; iEvent < usMaxEvents; iEvent++)
                    {
                        string strMeterException = "";
                        string strProgramException = "";

                        if (iEvent < usMeterEvents)
                        {
                            if (EventDescriptions.TryGetValue((int)meterEvents[iEvent], out strMeterException) == false)
                            {
                                // The TryGetValue failed so say it is an unknown event.
                                strMeterException = "Unknown Event " + meterEvents[iEvent].ToString(CultureInfo.InvariantCulture);
                            }
                        }

                        if (iEvent < usProgramEvents)
                        {
                            if (EventDescriptions.TryGetValue((int)programEvents[iEvent], out strProgramException) == false)
                            {
                                // The TryGetValue failed so say it is an unknown event.
                                strProgramException = "Unknown Event " + programEvents[iEvent].ToString(CultureInfo.InvariantCulture);
                            }
                        }

                        if (strMeterException.Equals(strProgramException) == false)
                        {
                            InvalidItems.Add(new ProgramValidationItem(strCategory, "Event " + (iEvent + 1).ToString(CultureInfo.CurrentCulture), 
                                strProgramException, strMeterException));
                        }
                    }
                }
            }

            return InvalidItems;
        }

        /// <summary>
        /// Creates a list of invalid Display Items.
        /// </summary>
        /// <param name="meterTables">The table structure for the meter.</param>
        /// <param name="programTables">The table structure for the program.</param>
        /// <returns>The list of items that are invalid.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/23/07 RCG	8.10.05		   Created

        protected virtual List<ProgramValidationItem> GetInvalidDisplayItems(CentronTables meterTables, CentronTables programTables)
        {
            List<ProgramValidationItem> InvalidDisplayItems = new List<ProgramValidationItem>();

            object objMeterValue;
            object objProgramValue;

            byte byMeterNormalStart;
            byte byMeterNormalEnd;
            byte byMeterTestStart;
            byte byMeterTestEnd;

            byte byProgramNormalStart;
            byte byProgramNormalEnd;
            byte byProgramTestStart;
            byte byProgramTestEnd;

            int iMaxNormalItems;
            int iMaxTestItems;

            int iNumberOfMeterNormalItems;
            int iNumberOfProgramNormalItems;

            int iNumberOfMeterTestItems;
            int iNumberOfProgramTestItems;

            // First get the limits on the Normal and Test display items for the meter.
            meterTables.GetValue(CentronTblEnum.MFGTBL0_NORMAL_LIST_START, null, out objMeterValue);
            byMeterNormalStart = (byte)objMeterValue;

            meterTables.GetValue(CentronTblEnum.MFGTBL0_NORMAL_LIST_STOP, null, out objMeterValue);
            byMeterNormalEnd = (byte)objMeterValue;

            meterTables.GetValue(CentronTblEnum.MFGTBL0_TEST_LIST_START, null, out objMeterValue);
            byMeterTestStart = (byte)objMeterValue;

            meterTables.GetValue(CentronTblEnum.MFGTBL0_TEST_LIST_STOP, null, out objMeterValue);
            byMeterTestEnd = (byte)objMeterValue;

            // Now get the limits from the program
            programTables.GetValue(CentronTblEnum.MFGTBL0_NORMAL_LIST_START, null, out objProgramValue);
            byProgramNormalStart = (byte)objProgramValue;

            programTables.GetValue(CentronTblEnum.MFGTBL0_NORMAL_LIST_STOP, null, out objProgramValue);
            byProgramNormalEnd = (byte)objProgramValue;

            programTables.GetValue(CentronTblEnum.MFGTBL0_TEST_LIST_START, null, out objProgramValue);
            byProgramTestStart = (byte)objProgramValue;

            programTables.GetValue(CentronTblEnum.MFGTBL0_TEST_LIST_STOP, null, out objProgramValue);
            byProgramTestEnd = (byte)objProgramValue;

            // Determine the maximum number of Display items for Normal and Test displays
            iNumberOfMeterNormalItems = byMeterNormalEnd - byMeterNormalStart + 1;
            iNumberOfProgramNormalItems = byProgramNormalEnd - byProgramNormalStart + 1;
            if (iNumberOfMeterNormalItems >= iNumberOfProgramNormalItems)
            {
                iMaxNormalItems = iNumberOfMeterNormalItems;
            }
            else
            {
                iMaxNormalItems = iNumberOfProgramNormalItems;
            }

            iNumberOfMeterTestItems = byMeterTestEnd - byMeterTestStart + 1;
            iNumberOfProgramTestItems = byProgramTestEnd - byProgramTestStart + 1;
            if (iNumberOfMeterTestItems >= iNumberOfProgramTestItems)
            {
                iMaxTestItems = iNumberOfMeterTestItems;
            }
            else
            {
                iMaxTestItems = iNumberOfProgramTestItems;
            }

            // Check the Normal Display Items
            for (int iIndex = 0; iIndex < iMaxNormalItems; iIndex++)
            {
                // Get the values
                int iMeterIndex = byMeterNormalStart + iIndex;
                int iProgramIndex = byProgramNormalStart + iIndex;

                ANSIDisplayItem MeterDisplayItem;
                ANSIDisplayItem ProgramDisplayItem;

                uint uiDisplayLID;
                string strDisplayID;
                ushort usDisplayFormat;
                byte byDisplayDim;

                string strMeterFormat;
                string strProgramFormat;
                string strMeterUnits;
                string strProgramUnits;

                // Create the display item for the meter
                if(iMeterIndex <= byMeterNormalEnd)
                {
                    int[] iIndexArray = {iMeterIndex};

                    meterTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_LID, iIndexArray, out objMeterValue);
                    uiDisplayLID = (uint)objMeterValue;
                    meterTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_USERID, iIndexArray, out objMeterValue);
                    strDisplayID = (string)objMeterValue;
                    meterTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_FORMAT, iIndexArray, out objMeterValue);
                    usDisplayFormat = (ushort)objMeterValue;
                    meterTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_DIMENSION, iIndexArray, out objMeterValue);
                    byDisplayDim = (byte)objMeterValue;

                    MeterDisplayItem = CreateDisplayItem(CreateLID(uiDisplayLID), strDisplayID, usDisplayFormat, byDisplayDim); 
                }
                else
                {
                    MeterDisplayItem = null;
                }

                // Create the display item for the program
                if (iProgramIndex <= byProgramNormalEnd)
                {
                    int[] iIndexArray = { iProgramIndex };

                    programTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_LID, iIndexArray, out objProgramValue);
                    uiDisplayLID = (uint)objProgramValue;
                    programTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_USERID, iIndexArray, out objProgramValue);
                    strDisplayID = (string)objProgramValue;
                    programTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_FORMAT, iIndexArray, out objProgramValue);
                    usDisplayFormat = (ushort)objProgramValue;
                    programTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_DIMENSION, iIndexArray, out objProgramValue);
                    byDisplayDim = (byte)objProgramValue;

                    ProgramDisplayItem = CreateDisplayItem(CreateLID(uiDisplayLID), strDisplayID, usDisplayFormat, byDisplayDim);
                }
                else
                {
                    ProgramDisplayItem = null;
                }

                if (MeterDisplayItem == null && ProgramDisplayItem != null)
                {
                    InvalidDisplayItems.Add(new ProgramValidationItem("Normal Display Items",
                                                    "Display Item " + (iIndex + 1).ToString(CultureInfo.InvariantCulture) + " Description",
                                                    ProgramDisplayItem.Description,
                                                    "None"));
                }
                else if (MeterDisplayItem != null && ProgramDisplayItem == null)
                {
                    InvalidDisplayItems.Add(new ProgramValidationItem("Normal Display Items",
                                                    "Display Item " + (iIndex + 1).ToString(CultureInfo.InvariantCulture) + " Description",
                                                    "None",
                                                    MeterDisplayItem.Description));
                }
                else if (MeterDisplayItem != null && ProgramDisplayItem != null)
                {
                    // Check the quantity
                    if (MeterDisplayItem.Description != ProgramDisplayItem.Description)
                    {
                        InvalidDisplayItems.Add(new ProgramValidationItem("Normal Display Items",
                                                        "Display Item " + (iIndex + 1).ToString(CultureInfo.InvariantCulture) + " Description",
                                                        ProgramDisplayItem.Description,
                                                        MeterDisplayItem.Description));
                    }

                    // Check the Display ID
                    if (MeterDisplayItem.DisplayID.Trim() != ProgramDisplayItem.DisplayID.Trim())
                    {
                        InvalidDisplayItems.Add(new ProgramValidationItem("Normal Display Items",
                                                        "Display Item " + (iIndex + 1).ToString(CultureInfo.InvariantCulture) + " ID",
                                                        ProgramDisplayItem.DisplayID.Trim(),
                                                        MeterDisplayItem.DisplayID.Trim()));
                    }

                    // Check the format
                    strMeterFormat = MeterDisplayItem.Format;
                    strProgramFormat = ProgramDisplayItem.Format;

                    if (strMeterFormat != strProgramFormat)
                    {
                        InvalidDisplayItems.Add(new ProgramValidationItem("Normal Display Items",
                                "Display Item " + (iIndex + 1).ToString(CultureInfo.InvariantCulture) + " Format",
                                strProgramFormat,
                                strMeterFormat));
                    }

                    // Check the units
                    strMeterUnits = MeterDisplayItem.Units;
                    strProgramUnits = ProgramDisplayItem.Units;

                    if (strMeterUnits != strProgramUnits)
                    {
                        InvalidDisplayItems.Add(new ProgramValidationItem("Normal Display Items",
                                "Display Item " + (iIndex + 1).ToString(CultureInfo.InvariantCulture) + " Units",
                                strProgramUnits,
                                strMeterUnits));
                    }
                }
            }

            // Check the Test Display Items
            for (int iIndex = 0; iIndex < iMaxTestItems; iIndex++)
            {
                // Get the values
                int iMeterIndex = byMeterTestStart + iIndex;
                int iProgramIndex = byProgramTestStart + iIndex;

                ANSIDisplayItem MeterDisplayItem;
                ANSIDisplayItem ProgramDisplayItem;

                uint uiDisplayLID;
                string strDisplayID;
                ushort usDisplayFormat;
                byte byDisplayDim;

                string strMeterFormat;
                string strProgramFormat;
                string strMeterUnits;
                string strProgramUnits;

                // Create the display item for the meter
                if (iMeterIndex <= byMeterTestEnd)
                {
                    int[] iIndexArray = { iMeterIndex };

                    meterTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_LID, iIndexArray, out objMeterValue);
                    uiDisplayLID = (uint)objMeterValue;
                    meterTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_USERID, iIndexArray, out objMeterValue);
                    strDisplayID = (string)objMeterValue;
                    meterTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_FORMAT, iIndexArray, out objMeterValue);
                    usDisplayFormat = (ushort)objMeterValue;
                    meterTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_DIMENSION, iIndexArray, out objMeterValue);
                    byDisplayDim = (byte)objMeterValue;

                    MeterDisplayItem = CreateDisplayItem(CreateLID(uiDisplayLID), strDisplayID, usDisplayFormat, byDisplayDim);
                }
                else
                {
                    MeterDisplayItem = null;
                }

                // Create the display item for the program
                if (iProgramIndex <= byProgramTestEnd)
                {
                    int[] iIndexArray = { iProgramIndex };

                    programTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_LID, iIndexArray, out objProgramValue);
                    uiDisplayLID = (uint)objProgramValue;
                    programTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_USERID, iIndexArray, out objProgramValue);
                    strDisplayID = (string)objProgramValue;
                    programTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_FORMAT, iIndexArray, out objProgramValue);
                    usDisplayFormat = (ushort)objProgramValue;
                    programTables.GetValue(CentronTblEnum.MFGTBL0_DISPLAY_DIMENSION, iIndexArray, out objProgramValue);
                    byDisplayDim = (byte)objProgramValue;

                    ProgramDisplayItem = CreateDisplayItem(CreateLID(uiDisplayLID), strDisplayID, usDisplayFormat, byDisplayDim);
                }
                else
                {
                    ProgramDisplayItem = null;
                }

                if (MeterDisplayItem == null && ProgramDisplayItem != null)
                {
                    InvalidDisplayItems.Add(new ProgramValidationItem("Test Display Items",
                                                    "Display Item " + (iIndex + 1).ToString(CultureInfo.InvariantCulture) + " Quantity",
                                                    ProgramDisplayItem.Description,
                                                    "None"));
                }
                else if (MeterDisplayItem != null && ProgramDisplayItem == null)
                {
                    InvalidDisplayItems.Add(new ProgramValidationItem("Test Display Items",
                                                    "Display Item " + (iIndex + 1).ToString(CultureInfo.InvariantCulture) + " Quantity",
                                                    "None",
                                                    MeterDisplayItem.Description));
                }
                else if (MeterDisplayItem != null && ProgramDisplayItem != null)
                {
                    // Check the quantity
                    if (MeterDisplayItem.Description != ProgramDisplayItem.Description)
                    {
                        InvalidDisplayItems.Add(new ProgramValidationItem("Test Display Items",
                                                        "Display Item " + (iIndex + 1).ToString(CultureInfo.InvariantCulture) + " Quantity",
                                                        ProgramDisplayItem.Description,
                                                        MeterDisplayItem.Description));
                    }

                    // Check the Display ID
                    if (MeterDisplayItem.DisplayID.Trim() != ProgramDisplayItem.DisplayID.Trim())
                    {
                        InvalidDisplayItems.Add(new ProgramValidationItem("Test Display Items",
                                                        "Display Item " + (iIndex + 1).ToString(CultureInfo.InvariantCulture) + " ID",
                                                        ProgramDisplayItem.DisplayID.Trim(),
                                                        MeterDisplayItem.DisplayID.Trim()));
                    }

                    // Check the format
                    strMeterFormat = MeterDisplayItem.Format;
                    strProgramFormat = ProgramDisplayItem.Format;

                    if (strMeterFormat != strProgramFormat)
                    {
                        InvalidDisplayItems.Add(new ProgramValidationItem("Test Display Items",
                                "Display Item " + (iIndex + 1).ToString(CultureInfo.InvariantCulture) + " Format",
                                strProgramFormat,
                                strMeterFormat));
                    }

                    // Check the units
                    strMeterUnits = MeterDisplayItem.Units;
                    strProgramUnits = ProgramDisplayItem.Units;

                    if (strMeterUnits != strProgramUnits)
                    {
                        InvalidDisplayItems.Add(new ProgramValidationItem("Test Display Items",
                                "Display Item " + (iIndex + 1).ToString(CultureInfo.InvariantCulture) + " Units",
                                strProgramUnits,
                                strMeterUnits));
                    }
                }
            }

            return InvalidDisplayItems;
        }

        /// <summary>
        /// Checks to see if the item matches and then creates a ProgramValidationItem if it does not.
        /// </summary>
        /// <param name="item">The item to validate</param>
        /// <param name="meterTables">The table structure for the meter.</param>
        /// <param name="programTables">The table structure for the program.</param>
        /// <returns>Returns the ProgramValidationItem for the value if the items do not match, and null if the values match.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/23/07 RCG	8.10.05		   Created
        // 09/22/09 jrf 2.30.02        Adding special case for require enhanced security to 
        //                             ensure null is returned for the program value if 
        //                             the item cannot be found in the program.
        // 01/12/10 jrf 2.40.04 148923 Correcting error reading MfgTbl58SecurityMode
        //                             (HAN Security Profile).
        //
        protected virtual ProgramValidationItem ValidateItem(CENTRONIIEDLValidationItem item, CentronTables meterTables, CentronTables programTables)
        {
            bool bItemsMatch = false;

            string strDisplayMeterValue = "";
            string strDisplayProgramValue = "";

            object objMeterValue;
            object objProgramValue;

            ProgramValidationItem InvalidItem = null;

            // Get the values
            try
            {
                meterTables.GetValue(item.Item, item.Index, out objMeterValue);
            }
            catch (Exception)
            {
                // We failed to get the value so set it to null
                objMeterValue = null;
            }

            try
            {
                programTables.GetValue(item.Item, item.Index, out objProgramValue);
            }
            catch (Exception)
            {
                // We failed to get the value so set it to null
                objProgramValue = null;
            }

            switch (item.Item)
            {
                case (long)StdTableEnum.STDTBL123_APTITLE_NOTIFY:
                {
                    if (objMeterValue != null)
                    {
                        strDisplayMeterValue = ESNConverter.Decode((byte[])objMeterValue);
                    }

                    if (objProgramValue != null)
                    {
                        strDisplayProgramValue = ESNConverter.Decode((byte[])objProgramValue);
                    }

                    if (strDisplayMeterValue.Equals(strDisplayProgramValue))
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL0_LOCK_LOW_BATTERY:
                {
                    object ScrollMeterValue;
                    object ScrollProgramValue;

                    // Get the scroll values
                    meterTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_LOW_BATTERY,
                        item.Index, out ScrollMeterValue);
                    programTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_LOW_BATTERY,
                        item.Index, out ScrollProgramValue);

                    strDisplayMeterValue = ConvertDisplayableErrors((bool)objMeterValue, (bool)ScrollMeterValue);
                    strDisplayProgramValue = ConvertDisplayableErrors((bool)objProgramValue, (bool)ScrollProgramValue);

                    if (strDisplayMeterValue == strDisplayProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL0_LOCK_LOSS_PHASE:
                {
                    object ScrollMeterValue;
                    object ScrollProgramValue;

                    // Get the scroll values
                    meterTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_LOSS_PHASE,
                        item.Index, out ScrollMeterValue);
                    programTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_LOSS_PHASE,
                        item.Index, out ScrollProgramValue);

                    strDisplayMeterValue = ConvertDisplayableErrors((bool)objMeterValue, (bool)ScrollMeterValue);
                    strDisplayProgramValue = ConvertDisplayableErrors((bool)objProgramValue, (bool)ScrollProgramValue);

                    if (strDisplayMeterValue == strDisplayProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL0_LOCK_TOU_SCHEDULE_ERROR:
                {
                    object ScrollMeterValue;
                    object ScrollProgramValue;

                    // Get the scroll values
                    meterTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_TOU_SCHEDULE_ERROR,
                        item.Index, out ScrollMeterValue);
                    programTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_TOU_SCHEDULE_ERROR,
                        item.Index, out ScrollProgramValue);

                    strDisplayMeterValue = ConvertDisplayableErrors((bool)objMeterValue, (bool)ScrollMeterValue);
                    strDisplayProgramValue = ConvertDisplayableErrors((bool)objProgramValue, (bool)ScrollProgramValue);

                    if (strDisplayMeterValue == strDisplayProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL0_LOCK_REVERSE_POWER_FLOW:
                {
                    object ScrollMeterValue;
                    object ScrollProgramValue;

                    // Get the scroll values
                    meterTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_REVERSE_POWER_FLOW,
                        item.Index, out ScrollMeterValue);
                    programTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_REVERSE_POWER_FLOW,
                        item.Index, out ScrollProgramValue);

                    strDisplayMeterValue = ConvertDisplayableErrors((bool)objMeterValue, (bool)ScrollMeterValue);
                    strDisplayProgramValue = ConvertDisplayableErrors((bool)objProgramValue, (bool)ScrollProgramValue);

                    if (strDisplayMeterValue == strDisplayProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL0_LOCK_MASS_MEMORY:
                {
                    object ScrollMeterValue;
                    object ScrollProgramValue;

                    // Get the scroll values
                    meterTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_MASS_MEMORY,
                        item.Index, out ScrollMeterValue);
                    programTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_MASS_MEMORY,
                        item.Index, out ScrollProgramValue);

                    strDisplayMeterValue = ConvertDisplayableErrors((bool)objMeterValue, (bool)ScrollMeterValue);
                    strDisplayProgramValue = ConvertDisplayableErrors((bool)objProgramValue, (bool)ScrollProgramValue);

                    if (strDisplayMeterValue == strDisplayProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL0_LOCK_REGISTER_FULL_SCALE:
                {
                    object ScrollMeterValue;
                    object ScrollProgramValue;

                    // Get the scroll values
                    meterTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_REGISTER_FULL_SCALE,
                        item.Index, out ScrollMeterValue);
                    programTables.GetValue(CentronTblEnum.MFGTBL0_SCROLL_REGISTER_FULL_SCALE,
                        item.Index, out ScrollProgramValue);

                    strDisplayMeterValue = ConvertDisplayableErrors((bool)objMeterValue, (bool)ScrollMeterValue);
                    strDisplayProgramValue = ConvertDisplayableErrors((bool)objProgramValue, (bool)ScrollProgramValue);

                    if (strDisplayMeterValue == strDisplayProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL0_ENERGY_LID:
                {
                    byte byMeterValue = 0;
                    byte byProgramValue = 0;

                    if (objMeterValue != null)
                    {
                        byMeterValue = (byte)objMeterValue;
                    }

                    if (objProgramValue != null)
                    {
                        byProgramValue = (byte)objProgramValue;
                    }

                    if (byMeterValue == byProgramValue)
                    {
                        bItemsMatch = true;
                    }
                    else
                    {
                        // Translate the values so that the user will understand the difference
                        uint uiMeterLID = ENERGY_LID_BASE + byMeterValue;
                        uint uiProgramLID = ENERGY_LID_BASE + byProgramValue;

                        if (byMeterValue == 0)
                        {
                            strDisplayMeterValue = "None";
                        }
                        else
                        {
                            LID LidValue = CreateLID(uiMeterLID);
                            strDisplayMeterValue = LidValue.lidDescription;
                        }

                        if (byProgramValue == 0)
                        {
                            strDisplayProgramValue = "None";
                        }
                        else
                        {
                            LID LidValue = CreateLID(uiProgramLID);
                            strDisplayProgramValue = LidValue.lidDescription;
                        }
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL0_DISPLAY_LID:
                case (long)CentronTblEnum.MFGTBL0_DEMAND_DEFINITION:
                case (long)CentronTblEnum.MFGTBL0_LP_LID:
                {
                    uint uiMeterValue = 0;
                    uint uiProgramValue = 0;

                    if (objMeterValue != null)
                    {
                        uiMeterValue = (uint)objMeterValue;
                    }

                    if (objProgramValue != null)
                    {
                        uiProgramValue = (uint)objProgramValue;
                    }

                    // Compare the values
                    if (uiMeterValue == uiProgramValue)
                    {
                        bItemsMatch = true;
                    }
                    else
                    {
                        // Translate the values to the LID description
                        if (uiMeterValue == 0)
                        {
                            strDisplayMeterValue = "None";
                        }
                        else
                        {
                            LID LidValue = CreateLID(uiMeterValue);
                            strDisplayMeterValue = LidValue.lidDescription;
                        }

                        if (uiProgramValue == 0)
                        {
                            strDisplayProgramValue = "None";
                        }
                        else
                        {
                            LID LidValue = CreateLID(uiProgramValue);
                            strDisplayProgramValue = LidValue.lidDescription;
                        }
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT:
                {
                    if ((ushort)objMeterValue == (ushort)objProgramValue)
                    {
                        bItemsMatch = true;
                    }
                    else
                    {
                        // The Pulse Weights are stored as integers so we need to convert them to a float value
                        double dMeterValue = (ushort)objMeterValue / 100.0;
                        double dProgramValue = (ushort)objProgramValue / 100.0;

                        strDisplayMeterValue = dMeterValue.ToString("F2", CultureInfo.InvariantCulture);
                        strDisplayProgramValue = dProgramValue.ToString("F2", CultureInfo.InvariantCulture);
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL0_CLOCK_SYNC:
                {
                    byte byMeterValue = 0;
                    byte byProgramValue = 0;

                    if (objMeterValue != null)
                    {
                        byMeterValue = (byte)objMeterValue;
                    }

                    if (objProgramValue != null)
                    {
                        byProgramValue = (byte)objProgramValue;
                    }

                    // Compare the values
                    if (byMeterValue == byProgramValue)
                    {
                        bItemsMatch = true;
                    }
                    else
                    {
                        // Translate the values to something the user will understand
                        if (byMeterValue == 0)
                        {
                            strDisplayMeterValue = "Clock";
                        }
                        else
                        {
                            strDisplayMeterValue = "Line";
                        }

                        if (byProgramValue == 0)
                        {
                            strDisplayProgramValue = "Clock";
                        }
                        else
                        {
                            strDisplayProgramValue = "Line";
                        }
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL42_DST_HOUR:
                {
                    byte byProgramValue = 0;
                    byte byMeterValue = 0;

                    // EDL files with the old TOU information may have this information
                    // stored in MFGTBL0 so we should check there if the program value is null
                    if (objProgramValue == null)
                    {
                        programTables.GetValue(CentronTblEnum.MFGTBL0_DST_HOUR, item.Index, out objProgramValue);
                    }

                    if (objProgramValue != null)
                    {
                        byProgramValue = (byte)objProgramValue;
                    }

                    // Due to different implementation in CENTRON II meter, Re-read the value from MFG Table 2260
                    byMeterValue = Table2260DSTConfig.DSTHour;

                    if(byProgramValue == byMeterValue)
                    {
                        bItemsMatch = true;
                    }
                    else
                    {
                        strDisplayProgramValue = byProgramValue.ToString(CultureInfo.InvariantCulture);
                        strDisplayMeterValue = byMeterValue.ToString(CultureInfo.InvariantCulture);
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL42_DST_MINUTE:
                {
                    byte byProgramValue = 0;
                    byte byMeterValue = 0;

                    // EDL files with the old TOU information may have this information
                    // stored in MFGTBL0 so we should check there if the program value is null
                    if (objProgramValue == null)
                    {
                        programTables.GetValue(CentronTblEnum.MFGTBL0_DST_MINUTE, item.Index, out objProgramValue);
                    }

                    if (objProgramValue != null)
                    {
                        byProgramValue = (byte)objProgramValue;
                    }

                    // Due to different implementation in CENTRON II meter, Re-read the value from MFG Table 2260
                    byMeterValue = Table2260DSTConfig.DSTMinute;

                    if (byProgramValue == byMeterValue)
                    {
                        bItemsMatch = true;
                    }
                    else
                    {
                        strDisplayProgramValue = byProgramValue.ToString(CultureInfo.InvariantCulture);
                        strDisplayMeterValue = byMeterValue.ToString(CultureInfo.InvariantCulture);
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL42_DST_OFFSET:
                {
                    byte byProgramValue = 0;
                    byte byMeterValue = 0;

                    // EDL files with the old TOU information may have this information
                    // stored in MFGTBL0 so we should check there if the program value is null
                    if (objProgramValue == null)
                    {
                        programTables.GetValue(CentronTblEnum.MFGTBL0_DST_OFFSET, item.Index, out objProgramValue);
                    }

                    if (objProgramValue != null)
                    {
                        byProgramValue = (byte)objProgramValue;
                    }

                    // Due to different implementation in CENTRON II meter, Re-read the value from MFG Table 2260
                    byMeterValue = Table2260DSTConfig.DSTOffset;

                    if (byProgramValue == byMeterValue)
                    {
                        bItemsMatch = true;
                    }
                    else
                    {
                        strDisplayProgramValue = byProgramValue.ToString(CultureInfo.InvariantCulture);
                        strDisplayMeterValue = byMeterValue.ToString(CultureInfo.InvariantCulture);
                    }

                    break;
                }
                case (long)StdTableEnum.STDTBL53_TIME_ZONE_OFFSET:
                {
                    TimeSpan tsOffset;
                    short sOffset;

                    if (objMeterValue != null)
                    {
                        sOffset = (short)objMeterValue;
                        tsOffset = TimeSpan.FromMinutes((double)sOffset);
                        strDisplayMeterValue = "GMT " + tsOffset.Hours + ":00";
                    }

                    if (objProgramValue != null)
                    {
                        sOffset = (short)objProgramValue;
                        tsOffset = TimeSpan.FromMinutes((double)sOffset);
                        strDisplayProgramValue = "GMT " + tsOffset.Hours + ":00";
                    }

                    // Compare the values
                    if (strDisplayMeterValue == strDisplayProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL101_NBR_PHASES:
                {
                    if (objMeterValue != null)
                    {
                        switch ((byte)objMeterValue)
                        {
                            case 1:
                                {
                                    strDisplayMeterValue = "Phase A";
                                    break;
                                }
                            case 2:
                                {
                                    strDisplayMeterValue = "Phase A, B";
                                    break;
                                }
                            case 3:
                                {
                                    strDisplayMeterValue = "Phase A, B, C";
                                    break;
                                }
                            default:
                                {
                                    strDisplayMeterValue = "";
                                    break;
                                }
                        }
                    }

                    if (objProgramValue != null)
                    {
                        switch ((byte)objProgramValue)
                        {
                            case 1:
                                {
                                    strDisplayProgramValue = "Phase A";
                                    break;
                                }
                            case 2:
                                {
                                    strDisplayProgramValue = "Phase A, B";
                                    break;
                                }
                            case 3:
                                {
                                    strDisplayProgramValue = "Phase A, B, C";
                                    break;
                                }
                            default:
                                {
                                    strDisplayProgramValue = "";
                                    break;
                                }
                        }
                    }

                    // Compare the values
                    if (strDisplayMeterValue == strDisplayProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD:
                case (long)CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD:
                {
                    ushort usDivisor;
                    object objDivisor;
                    float fThreshold;

                    if (objMeterValue != null)
                    {
                        meterTables.GetValue(CentronTblEnum.MFGTBL103_DIVISOR, null, out objDivisor);
                        usDivisor = (ushort)objDivisor;

                        fThreshold = (ushort)objMeterValue / (float)usDivisor;
                        strDisplayMeterValue = fThreshold.ToString("F2", CultureInfo.InvariantCulture);
                    }

                    if (objProgramValue != null)
                    {
                        programTables.GetValue(CentronTblEnum.MFGTBL103_DIVISOR, null, out objDivisor);
                        usDivisor = (ushort)objDivisor;

                        fThreshold = (ushort)objProgramValue / (float)usDivisor;
                        strDisplayProgramValue = fThreshold.ToString("F2", CultureInfo.InvariantCulture);
                    }

                    // Compare the values
                    if (strDisplayMeterValue == strDisplayProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL93_MAX_SWITCH_PERIOD:
                case (long)CentronTblEnum.MFGTBL93_OPEN_TIME:
                case (long)CentronTblEnum.MFGTBL93_RANDOMIZATION_ALARM:
                case (long)CentronTblEnum.MFGTBL93_RESTORATION_RANDOM_DELAY:
                case (long)CentronTblEnum.MFGTBL93_RESTORATION_START_DELAY:
                {
                    DateTime dtValue;

                    // These values are returned as a DateTime but they should really be a TimeSpan
                    if (objMeterValue != null)
                    {
                        dtValue = (DateTime)objMeterValue;
                        strDisplayMeterValue = dtValue.TimeOfDay.ToString();
                    }

                    if (objProgramValue != null)
                    {
                        dtValue = (DateTime)objProgramValue;
                        strDisplayProgramValue = dtValue.TimeOfDay.ToString();
                    }

                    // Compare the values
                    if (strDisplayMeterValue == strDisplayProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL93_QUANTITY:
                {
                    object objValue;
                    byte byIndex;
                    LID DemandLid;

                    if (objMeterValue != null)
                    {
                        byIndex = (byte)objMeterValue;
                        if (byIndex != 255)
                        {
                            meterTables.GetValue(CentronTblEnum.MFGTBL0_DEMAND_DEFINITION, new int[] { byIndex }, out objValue);
                            DemandLid = CreateLID((uint)objValue);
                            strDisplayMeterValue = DemandLid.lidDescription;
                        }
                        else
                        {
                            strDisplayMeterValue = "None";
                        }
                    }

                    if (objProgramValue != null)
                    {
                        byIndex = (byte)objProgramValue;
                        if (byIndex != 255)
                        {
                            meterTables.GetValue(CentronTblEnum.MFGTBL0_DEMAND_DEFINITION, new int[] { byIndex }, out objValue);
                            DemandLid = CreateLID((uint)objValue);
                            strDisplayProgramValue = DemandLid.lidDescription;
                        }
                        else
                        {
                            strDisplayProgramValue = "None";
                        }
                    }

                    // Compare the values
                    if (strDisplayMeterValue == strDisplayProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME:
                {
                    byte byMeterValue = 0;
                    byte byProgramValue = 0;

                    if (objMeterValue != null)
                    {
                        byMeterValue = (byte)objMeterValue;
                    }

                    strDisplayMeterValue = CENTRON2_MONO.CENTRONIIDetermineDailySelfRead(byMeterValue);

                    if (objProgramValue != null)
                    {
                        byProgramValue = (byte)objProgramValue;
                    }

                    strDisplayProgramValue = CENTRON2_MONO.CENTRONIIDetermineDailySelfRead(byProgramValue);

                    if (byMeterValue == byProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL0_ITEM_DISPLAY_TIME:
                {
                    int iMeterValue = 0;
                    int iProgramValue = 0;

                    // This value is stored in 1/4 seconds so we need to divied by 4 to get
                    // the actual number of seconds.
                    if (objMeterValue != null)
                    {
                        iMeterValue = (byte)objMeterValue / 4;
                        strDisplayMeterValue = iMeterValue.ToString(CultureInfo.InvariantCulture);
                    }

                    if (objProgramValue != null)
                    {
                        iProgramValue = (byte)objProgramValue / 4;
                        strDisplayProgramValue = iProgramValue.ToString(CultureInfo.InvariantCulture);
                    }

                    if (iMeterValue == iProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
                case (long)CentronTblEnum.MFGTBL145_EXCEPTION_SECURITY_MODEL:
                {
                    OpenWayMFGTable2193.SecurityFormat MeterValue = OpenWayMFGTable2193.SecurityFormat.None;
                    OpenWayMFGTable2193.SecurityFormat ProgramValue = OpenWayMFGTable2193.SecurityFormat.None;

                    if (objMeterValue != null)
                    {
                        MeterValue = (OpenWayMFGTable2193.SecurityFormat)(byte)objMeterValue;
                        strDisplayMeterValue = OpenWayMFGTable2193.GetSecurityFormatString(MeterValue);
                    }

                    if (objProgramValue != null)
                    {
                        ProgramValue = (OpenWayMFGTable2193.SecurityFormat)(byte)objProgramValue;
                        strDisplayProgramValue = OpenWayMFGTable2193.GetSecurityFormatString(ProgramValue);
                    }

                    bItemsMatch = ProgramValue == MeterValue;

                    break;
                }
                case (long)CentronTblEnum.MFGTBL0_REGISTER_FULL_SCALE:
                {
                    double dMeterValue = 0.0;
                    double dProgramValue = 0.0;

                    if (objMeterValue != null)
                    {
                        dMeterValue = (double)objMeterValue / 1000.0;
                        strDisplayMeterValue = dMeterValue.ToString("F3", CultureInfo.CurrentCulture);
                    }

                    if (objProgramValue != null)
                    {
                        dProgramValue = (double)objProgramValue / 1000.0;
                        strDisplayProgramValue = dProgramValue.ToString("F3", CultureInfo.CurrentCulture);
                    }

                    bItemsMatch = dMeterValue == dProgramValue;
                    break;
                }
                case (long)CentronTblEnum.MfgTbl145C1218OverZigBee:
                {
                    bool bMeterValue = false;
                    bool bProgramValue = false;

                    strDisplayProgramValue = null;

                    if (VersionChecker.CompareTo(FWRevision, VERSION_1) > 0
                        || (VersionChecker.CompareTo(FWRevision, VERSION_1) == 0 && FirmwareBuild >= 56))
                    {
                        if (objMeterValue != null)
                        {
                            // We need to use the value of the bit
                            bMeterValue = (bool)objMeterValue;
                        }
                    }
                    else
                    {
                        // C12.18 over ZigBee was always enabled for meters before 2.5.56
                        bMeterValue = true;
                    }

                    strDisplayMeterValue = bMeterValue.ToString(CultureInfo.CurrentCulture);

                    if (objProgramValue != null)
                    {
                        bProgramValue = (bool)objProgramValue;
                        strDisplayProgramValue = bProgramValue.ToString(CultureInfo.CurrentCulture);
                    }

                    bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);
                    break;
                }
                case (long)CentronTblEnum.MFGTBL145_REQUIRE_ENHANCED_SECURITY:
                {
                    strDisplayProgramValue = null;

                    if (objMeterValue != null)
                    {
                        strDisplayMeterValue = ((bool)objMeterValue).ToString(CultureInfo.CurrentCulture);
                    }

                    if (objProgramValue != null)
                    {
                        strDisplayProgramValue = ((bool)objProgramValue).ToString(CultureInfo.CurrentCulture);
                    }

                    bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);

                    break;
                }
                case (long)CentronTblEnum.MfgTbl58SecurityMode:
                {
                    byte bySecurityMode;
                    byte byDeviceAuthMode;
                    byte byCBKEMode;

                    // Get the Meter value
                    if (objMeterValue != null && meterTables.IsCached((long)CentronTblEnum.MfgTbl58DeviceAuthMode, null)
                        && meterTables.IsCached((long)CentronTblEnum.MfgTbl58CbkeMode, null))
                    {
                        // We have already retrieved the Security Mode
                        bySecurityMode = (byte)objMeterValue;

                        // Get the other two modes
                        meterTables.GetValue(CentronTblEnum.MfgTbl58DeviceAuthMode, null, out objMeterValue);
                        byDeviceAuthMode = (byte)objMeterValue;

                        meterTables.GetValue(CentronTblEnum.MfgTbl58CbkeMode, null, out objMeterValue);
                        byCBKEMode = (byte)objMeterValue;

                        // Get the HAN Profile Name
                        strDisplayMeterValue = CHANMfgTable2106.GetHANSecurityProfile(bySecurityMode, byDeviceAuthMode, byCBKEMode);
                    }

                    // Get the Program value
                    if (objProgramValue != null && programTables.IsCached((long)CentronTblEnum.MfgTbl58DeviceAuthMode, null)
                        && programTables.IsCached((long)CentronTblEnum.MfgTbl58CbkeMode, null))
                    {
                        // We have already retrieved the Security Mode
                        bySecurityMode = (byte)objProgramValue;

                        // Get the other two modes
                        programTables.GetValue(CentronTblEnum.MfgTbl58DeviceAuthMode, null, out objProgramValue);
                        byDeviceAuthMode = (byte)objProgramValue;

                        programTables.GetValue(CentronTblEnum.MfgTbl58CbkeMode, null, out objProgramValue);
                        byCBKEMode = (byte)objProgramValue;

                        // Get the HAN Profile Name
                        strDisplayProgramValue = CHANMfgTable2106.GetHANSecurityProfile(bySecurityMode, byDeviceAuthMode, byCBKEMode);
                    }

                    bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);
                    break;
                }
                case (long)CentronTblEnum.MfgTbl58InterPanMode:
                {
                    // Get the Meter value
                    if (objMeterValue != null)
                    {
                        // Get the Inter PAN Mode description
                        strDisplayMeterValue = CHANMfgTable2106.GetInterPANMode((byte)objMeterValue);
                    }

                    // Get the Program value
                    if (objProgramValue != null)
                    {
                        // Get the Inter PAN Mode description
                        strDisplayProgramValue = CHANMfgTable2106.GetInterPANMode((byte)objProgramValue);
                    }

                    bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);

                    break;
                }
                case (long)CentronTblEnum.MfgTbl212FatalErrorRecoveryEnabled:
                {
                    object objValue;
                    bool bProgramValue = false;
                    bool bMeterValue = false;

                    // We need to check the default
                    if (objMeterValue != null)
                    {
                        meterTables.GetValue(CentronTblEnum.MfgTbl212FatalErrorRecoveryUseMeterDefaults, null, out objValue);

                        if ((bool)objValue == true)
                        {
                            // Use the default value which is false
                            bMeterValue = false;
                        }
                        else
                        {
                            // Use what the meter has
                            bMeterValue = (bool)objMeterValue;
                        }

                        strDisplayMeterValue = bMeterValue.ToString(CultureInfo.CurrentCulture);
                    }

                    if (objProgramValue != null)
                    {
                        programTables.GetValue(CentronTblEnum.MfgTbl212FatalErrorRecoveryUseMeterDefaults, null, out objValue);

                        if ((bool)objValue == true)
                        {
                            // Use the default value which is false
                            bProgramValue = false;
                        }
                        else
                        {
                            // Use what the meter has
                            bProgramValue = (bool)objProgramValue;
                        }

                        strDisplayProgramValue = bProgramValue.ToString(CultureInfo.CurrentCulture);
                    }

                    // Compare the strings in case one of them is null.
                    bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);

                    break;
                }
                case (long)CentronTblEnum.MfgTbl212AssetSynchronizationEnabled:
                {
                    object objValue;
                    bool bProgramValue = false;
                    bool bMeterValue = false;

                    // We need to check the default
                    if (objMeterValue != null)
                    {
                        meterTables.GetValue(CentronTblEnum.MfgTbl212AssetSynchronizationUseMeterDefaults, null, out objValue);

                        if ((bool)objValue == true)
                        {
                            // Use the default value which is false
                            bMeterValue = false;
                        }
                        else
                        {
                            // Use what the meter has
                            bMeterValue = (bool)objMeterValue;
                        }

                        strDisplayMeterValue = bMeterValue.ToString(CultureInfo.CurrentCulture);
                    }

                    if (objProgramValue != null)
                    {
                        programTables.GetValue(CentronTblEnum.MfgTbl212AssetSynchronizationUseMeterDefaults, null, out objValue);

                        if ((bool)objValue == true)
                        {
                            // Use the default value which is false
                            bProgramValue = false;
                        }
                        else
                        {
                            // Use what the meter has
                            bProgramValue = (bool)objProgramValue;
                        }

                        strDisplayProgramValue = bProgramValue.ToString(CultureInfo.CurrentCulture);
                    }

                    // Compare the strings in case one of them is null.
                    bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);

                    break;
                }
                default:
                {
                    // The CentronTables object may return null so make sure we don't
                    // cause an exception first.
                    if (objMeterValue != null)
                    {
                        // Trim spaces and null characters so that they will display and validate correctly
                        strDisplayMeterValue = objMeterValue.ToString().Trim(new char[] { ' ', '\0' });
                    }

                    if (objProgramValue != null)
                    {
                        // Trim spaces and null characters so that they will display and validate correctly
                        strDisplayProgramValue = objProgramValue.ToString().Trim(new char[] { ' ', '\0' });
                    }

                    // Compare the values
                    if (strDisplayMeterValue == strDisplayProgramValue)
                    {
                        bItemsMatch = true;
                    }

                    break;
                }
            }

            if (bItemsMatch == false)
            {
                // There is a mismatch so add the item.
                InvalidItem = new ProgramValidationItem(item.Category, item.Name, strDisplayProgramValue, strDisplayMeterValue);
            }

            return InvalidItem;
        }

        /// <summary>
        /// Converts the Lock and Scroll bitfield values into a string
        /// </summary>
        /// <param name="bLock">The Lock bitfield value.</param>
        /// <param name="bScroll">The Scroll bitfield value.</param>
        /// <returns>The string representation of the Displayable Error value</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/07 RCG	8.10.05		   Created

        protected string ConvertDisplayableErrors(bool bLock, bool bScroll)
        {
            string strResult = "";

            if (bLock == true && bScroll == false)
            {
                strResult = "Lock";
            }
            else if (bLock == false && bScroll == true)
            {
                strResult = "Scroll";
            }
            else if (bLock == false && bScroll == false)
            {
                strResult = "Ignore";
            }
            else
            {
                // This should never happen
                strResult = "Invalid Value";
            }

            return strResult;
        }

        /// <summary>
        /// Gets the list of tables to read from the meter.
        /// </summary>
        /// <returns>The list of Table IDs</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created

        protected virtual List<ushort> GetValidationTablesToRead()
        {
            List<ushort> TableList = new List<ushort>();

            TableList.Add(0);
            TableList.Add(1);
            TableList.Add(6);
            TableList.Add(53);
            TableList.Add(121);
            TableList.Add(123);
            TableList.Add(2048);
            TableList.Add(2090);
            TableList.Add(2106);
            TableList.Add(2139);
            TableList.Add(2141);
            TableList.Add(2142);
            TableList.Add(2143);
            TableList.Add(2149);
            TableList.Add(2150);
            TableList.Add(2151);
            TableList.Add(2159);   
            TableList.Add(2161);   
            TableList.Add(2163);   
            TableList.Add(2190);
            TableList.Add(2193);
            TableList.Add(2206);
            TableList.Add(2260);

            return TableList;
        }

        /// <summary>
        /// Creates the list of validation items.
        /// </summary>
        /// <returns>A list of items that will be validated.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created
        // 05/20/09 AF  2.20.05         Corrected a typo in a string

        protected virtual List<CENTRONIIEDLValidationItem> GetValidationList()
        {
            List<CENTRONIIEDLValidationItem> ValidationList = new List<CENTRONIIEDLValidationItem>();
            CENTRON_AMI_EventDictionary EventDictionary = new CENTRON_AMI_EventDictionary();
            CENTRON_AMI_CommEventDictionary CommEventDictionary = new CENTRON_AMI_CommEventDictionary();

            // TOU/Time configuration
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)StdTableEnum.STDTBL6_TARIFF_ID,
                                        null,
                                        "TOU Schedule ID",
                                        "TOU/Time"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)StdTableEnum.STDTBL53_TIME_ZONE_OFFSET,
                                        null,
                                        "Time Zone",
                                        "TOU/Time"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL42_DST_HOUR,
                                        null,
                                        "DST Switch Time Hour",
                                        "TOU/Time"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL42_DST_MINUTE,
                                        null,
                                        "DST Switch Time Minute",
                                        "TOU/Time"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL42_DST_OFFSET,
                                        null,
                                        "DST Switch Length",
                                        "TOU/Time"));

            // Security
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL145_REQUIRE_ENHANCED_SECURITY,
                                        null,
                                        "Require Enhanced Security",
                                        "Security"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL145_EXCEPTION_SECURITY_MODEL,
                                        null,
                                        "Exception Security Model",
                                        "Security"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MfgTbl58SecurityMode,
                                        null,
                                        "HAN Security Profile",
                                        "Security",
                                        VERSION_1));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MfgTbl58InterPanMode,
                                        null,
                                        "InterPAN Mode",
                                        "Security",
                                        VERSION_1));
            // Disable the validation of MfgTbl145RequireSignedAuthorization for now
            // ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MfgTbl145RequireSignedAuthorization, /*MfgTbl145RequireSignedAuthentication*/
            //                            null,
            //                            "Require Signed Authentication",
            //                            "Security",
            //                            VERSION_1));


            // Quantities
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_ENERGY_LID,
                                        new int[] {0},
                                        "Energy 1 Quantity",
                                        "Quantities"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_ENERGY_LID,
                                        new int[] { 1 },
                                        "Energy 2 Quantity",
                                        "Quantities"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_ENERGY_LID,
                                        new int[] { 2 },
                                        "Energy 3 Quantity",
                                        "Quantities"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_ENERGY_LID,
                                        new int[] { 3 },
                                        "Energy 4 Quantity",
                                        "Quantities"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_DEMAND_DEFINITION,
                                        new int[] { 0 },
                                        "Demand Quantity",
                                        "Quantities"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_DEMAND_THRESHOLDS,
                                        new int[] { 0 },
                                        "Load Control Threshold",
                                        "Quantities"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_DEMAND_CONTROL,
                                        null,
                                        "Reconnect Method",
                                        "Quantities"));

            // Register Operations
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_DEMAND_INTERVAL_LENGTH,
                                        null,
                                        "Demand Interval Length (minutes)",
                                        "Register Operations"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_NUM_SUB_INTERVALS,
                                        null,
                                        "Number of Subintervals",
                                        "Register Operations"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_COLD_LOAD_PICKUP,
                                        null,
                                        "Cold Load Pickup Time (minutes)",
                                        "Register Operations"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_OUTAGE_LENGTH_BEFORE_CLPU,
                                        null,
                                        "Power Outage Recognition Time (seconds)",
                                        "Register Operations"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_TEST_MODE_INTERVAL_LENGTH,
                                        null,
                                        "Test Mode Demand Interval Length (minutes)",
                                        "Register Operations"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_NUM_TEST_MODE_SUBINTERVALS,
                                        null,
                                        "Number of Test Mode Subintervals",
                                        "Register Operations"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_CLOCK_SYNC,
                                        null,
                                        "Clock Synchronization",
                                        "Register Operations"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME,
                                        null,
                                        "Daily Self Read Time",
                                        "Register Operations"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MfgTbl212FatalErrorRecoveryEnabled,
                                        null,
                                        "Enable Fatal Error Recovery",
                                        "Register Operations"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MfgTbl212AssetSynchronizationEnabled,
                                        null,
                                        "Enable Asset Synchronization",
                                        "Register Operations"));

            // Device Multipliers
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_CT_MULTIPLIER,
                                        null,
                                        "CT Multiplier",
                                        "Device Multipliers"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_VT_MULTIPLIER,
                                        null,
                                        "VT Multiplier",
                                        "Device Multipliers"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_REGISTER_MULTIPLIER,
                                        null,
                                        "Register Multiplier",
                                        "Device Multipliers"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_REGISTER_FULL_SCALE,
                                        null,
                                        "Register Fullscale",
                                        "Device Multipliers"));

            // Load Profile
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_LID,
                                        new int[] { 0 },
                                        "Quantity 1",
                                        "Load Profile"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT,
                                        new int[] { 0 },
                                        "Pulse Weight 1",
                                        "Load Profile"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_LID,
                                        new int[] { 1 },
                                        "Quantity 2",
                                        "Load Profile"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT,
                                        new int[] { 1 },
                                        "Pulse Weight 2",
                                        "Load Profile"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_LID,
                                        new int[] { 2 },
                                        "Quantity 3",
                                        "Load Profile"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT,
                                        new int[] { 2 },
                                        "Pulse Weight 3",
                                        "Load Profile"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_LID,
                                        new int[] { 3 },
                                        "Quantity 4",
                                        "Load Profile"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT,
                                        new int[] { 3 },
                                        "Pulse Weight 4",
                                        "Load Profile"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_INTERVAL_LENGTH,
                                        null,
                                        "Interval Length",
                                        "Load Profile"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_MIN_POWER_OUTAGE,
                                        null,
                                        "Outage Length",
                                        "Load Profile"));

            // Voltage Monitor

            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL102_ENABLE_FLAG,
                                        null,
                                        "Enable Voltage Monitor",
                                        "Voltage Monitor"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL101_NBR_PHASES,
                                        null,
                                        "Phase Selection",
                                        "Voltage Monitor"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL101_VM_INT_LEN,
                                        null,
                                        "Interval Length",
                                        "Voltage Monitor"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD,
                                        null,
                                        "VoltHour Low Threshold",
                                        "Voltage Monitor"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD,
                                        null,
                                        "VoltHour High Threshold",
                                        "Voltage Monitor"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL102_RMS_VOLT_LOW_THRESHOLD,
                                        null,
                                        "RMS Volt Low Threshold",
                                        "Voltage Monitor"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL102_RMS_VOLT_HIGH_THRESHOLD,
                                        null,
                                        "RMS Volt High Threshold",
                                        "Voltage Monitor"));

            // User Data
            for (int iIndex = 0; iIndex < 3; iIndex++)
            {
                ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_USER_DEFINED_FIELDS,
                                            new int[] { iIndex },
                                            "User Data #" + (iIndex + 1).ToString(CultureInfo.InvariantCulture),
                                            "User Data"));
            }

            // Display
            
            // Add the Display Items

            // Display is a bit tricky in that it is possible that the meter and program may have different
            // numbers of Normal and Test items, which means that the display items may not be aligned correctly.
            // Therefore we can not handle the display in the same manner as the other items. We will just add 
            // the item for the display and add a check to make sure not to read that item since it will cause an exception
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_DISPLAY_ITEMS,
                                        null,
                                        "Display Items",
                                        "Display Items"));


            // Add the Display Options
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_ITEM_DISPLAY_TIME,
                                        null,
                                        "Display Scroll On Time (seconds)",
                                        "Display Options"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_DISPLAY_EOI,
                                        null,
                                        "Enable End of Interval (EOI) Indicator",
                                        "Display Options"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_WATT_LOAD_INDICATOR,
                                        null,
                                        "Enable Watt Load Indicator",
                                        "Display Options"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_DISPLAY_REMOTE_DISCONNECT_MESSAGE_FLAG,
                                        null,
                                        "Enable Remote Disconnect OFF message",
                                        "Display Options"));


            // Add the Displayable Errors. Since these values are stored in seperate bit fields
            // for Scroll and Lock we will only add the Lock items and then check the scroll values
            // to determine what is displayed
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LOCK_LOW_BATTERY,
                                        null,
                                        "Non-Fatal Error #1 - Low Battery",
                                        "Displayable Errors"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LOCK_LOSS_PHASE,
                                        null,
                                        "Non-Fatal Error #2 - Loss of Phase",
                                        "Displayable Errors"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LOCK_TOU_SCHEDULE_ERROR,
                                        null,
                                        "Non-Fatal Error #3 - Clock, TOU Error",
                                        "Displayable Errors"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LOCK_REVERSE_POWER_FLOW,
                                        null,
                                        "Non-Fatal Error #4 - Reverse Power Flow",
                                        "Displayable Errors"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LOCK_MASS_MEMORY,
                                        null,
                                        "Non-Fatal Error #5 - Load Profile Error",
                                        "Displayable Errors"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_LOCK_REGISTER_FULL_SCALE,
                                        null,
                                        "Non-Fatal Error #6 - Full Scale Overflow",
                                        "Displayable Errors"));

            // Events
            for (int iIndex = 0; iIndex < 256; iIndex++)
            {
                // Get the description from the EventDictionary
                string strDescription;
                bool bValidString;

                bValidString = EventDictionary.TryGetValue(iIndex, out strDescription);

                if (bValidString == false)
                {
                    // It could be a MFG event since an event number is either STD or MFG but not both
                    bValidString = EventDictionary.TryGetValue(2048 + iIndex, out strDescription);
                }

                if (bValidString == false)
                {
                    // We don't have a description for the item.
                    strDescription = "Unknown Event " + iIndex.ToString(CultureInfo.InvariantCulture);
                }

                ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL0_HISTORY_LOG_EVENTS,
                                                new int[] { iIndex },
                                                strDescription,
                                                "Events"));
            }

            // Exceptions
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)StdTableEnum.STDTBL123_EXCEPTION_REPORT,
                                                      null,
                                                      "Exceptions",
                                                      "Exceptions"));

            // Comm Log - Standard LAN Events
            // TODO: Implement Stnadard LAN Events when they exist

            // Comm Log - Mfg LAN Events
            // for (int iIndex = (int)CENTRON_AMI.LANEvents.BEGIN_MFG_LAN_EVENTS; iIndex < (int)CENTRON_AMI.LANEvents.END_MFG_LAN_EVENTS; iIndex++)
            // {
            //    // Get the description from the EventDictionary
            //    string strDescription;
            //    bool bValidString;
            //
            //    bValidString = CommEventDictionary.TryGetValue(iIndex, out strDescription);
            //
            //    if (bValidString == false)
            //    {
            //        // We don't have a description for the item.
            //        strDescription = "Unknown Event " + iIndex.ToString(CultureInfo.InvariantCulture);
            //    }
            //
            //    // Items in the MFG_EVENTS_MONITORED_FLAGS field do not include the MFG Bit.  (Must subtract off 2048)
            //    ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL113_MFG_EVENTS_MONITORED_FLAGS,
            //                                    new int[] { iIndex - 2048 },
            //                                    strDescription,
            //                                    "Mfg LAN Events"));
            // }

            // Comm Log - Standard HAN Events
            // TODO: Implement Standard HAN Events when they exist

            // Comm Log - Mfg HAN Events
            //  for (int iIndex = (int)CENTRON_AMI.HANEvents.BEGIN_MFG_HAN_EVENTS; iIndex < (int)CENTRON_AMI.HANEvents.END_MFG_HAN_EVENTS; iIndex++)
            // {
            //    // Get the description from the EventDictionary
            //    string strDescription;
            //    bool bValidString;
            //
            //    bValidString = CommEventDictionary.TryGetValue(iIndex, out strDescription);
            //
            //    if (bValidString == false)
            //    {
            //        // We don't have a description for the item.
            //        strDescription = "Unknown Event " + iIndex.ToString(CultureInfo.InvariantCulture);
            //    }
            //
            //    // Items in the MFG_EVENTS_MONITORED_FLAGS field do not include the MFG Bit.  (Must subtract off 2048)
            //    ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL115_MFG_EVENTS_MONITORED_FLAGS,
            //                                    new int[] { iIndex - 2048 },
            //                                    strDescription,
            //                                    "Mfg HAN Events"));
            // }

            // Service Limiting
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL94_OVERRIDE_FLAG,
                                        null,
                                        "Override Connect/Disconnect Switch",
                                        "Service Limiting"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL93_CONNECT_WITH_USER_INTERVENTION_FLAG,
                                        null,
                                        "User Intervention Required After Disconnect",
                                        "Service Limiting"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL93_MAX_SWITCH_COUNT,
                                        null,
                                        "Max Number of Disconnect Switches per Period",
                                        "Service Limiting"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL93_RANDOMIZATION_ALARM,
                                        null,
                                        "Randomization Period to Send Alarms",
                                        "Service Limiting"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL93_RESTORATION_START_DELAY,
                                        null,
                                        "Reconnect Switch Delay",
                                        "Service Limiting"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL93_RESTORATION_RANDOM_DELAY,
                                        null,
                                        "Randomization Period After Reconnect Delay",
                                        "Service Limiting"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL93_OPEN_TIME,
                                        null,
                                        "Switch Open Time",
                                        "Service Limiting"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL93_RETRY_ATTEMPTS,
                                        null,
                                        "Retry Attempts",
                                        "Service Limiting"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MfgTbl95FailsafeDuration,
                                        null,
                                        "Failsafe Duration",
                                        "Service Limiting"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL93_QUANTITY,
                                        new int[] { 0 },
                                        "Normal Mode Threshold Demand",
                                        "Service Limiting"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL93_THRESHOLD,
                                        new int[] { 0 },
                                        "Normal Mode Threshold",
                                        "Service Limiting"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL93_QUANTITY,
                                        new int[] { 1 },
                                        "Critical Mode Threshold Demand",
                                        "Service Limiting"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL93_THRESHOLD,
                                        new int[] { 1 },
                                        "Critical Mode Threshold",
                                        "Service Limiting"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL93_QUANTITY,
                                        new int[] { 2 },
                                        "Emergency Mode Threshold Demand",
                                        "Service Limiting"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL93_THRESHOLD,
                                        new int[] { 2 },
                                        "Emergency Mode Threshold",
                                        "Service Limiting"));

            // Communications
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_OPTICAL,
                                        null,
                                        "Lockout: login attempts, optical",
                                        "Communications"));
            ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_OPTICAL,
                                        null,
                                        "Lockout: lockout minutes, optical",
                                        "Communications"));
            // ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_LAN,
            //                            null,
            //                            "Lockout: login attemtps, lan",
            //                            "Communications"));
            // ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_LAN,
            //                            null,
            //                            "Lockout: lockout minutes, lan",
            //                            "Communications"));
            // ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL142_FAILURES_BEFORE_FAILURE_EVENT,
            //                            null,
            //                            "LAN Send message failure limit",
            //                            "Communications"));
            // ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MFGTBL142_LAN_LINK_METRIC_PERIOD_SECONDS,
            //                            null,
            //                            "LAN Link metric (quality) period",
            //                            "Communications"));
            // ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MfgTbl145C1218OverZigBee,
            //                            null,
            //                            "ANSI C12.18 support over ZigBee Enabled",
            //                            "Communications",
            //                            VERSION_1));
            // ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MfgTbl145DisableZigBeeRadio,
            //                            null,
            //                            "Disable ZigBee Radio",
            //                            "Communications",
            //                            VERSION_1));
            // ValidationList.Add(new CENTRONIIEDLValidationItem((long)CentronTblEnum.MfgTbl145DisableZigBeePrivateProfile,
            //                            null,
            //                            "Disable ZigBee Private Profile",
            //                            "Communications",
            //                            VERSION_1));

            return ValidationList;
        }
        #endregion
    }

    /// <summary>
    /// Object that stores the information necessary for retrieving an item from the
    /// CentronTables object.
    /// </summary>
    public class CENTRONIIEDLValidationItem
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item">The CentronTblEnum value for the item.</param>
        /// <param name="index">The index for the item.</param>
        /// <param name="strName">The name of the item.</param>
        /// <param name="strCategory">The category for the item.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created

        public CENTRONIIEDLValidationItem(long item, int[] index, string strName, string strCategory)
        {
            m_Item = item;
            m_Index = index;
            m_strName = strName;
            m_strCategory = strCategory;
            m_fltMinFWVersion = 0.0f;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item">The CentronTblEnum value for the item.</param>
        /// <param name="index">The index for the item.</param>
        /// <param name="strName">The name of the item.</param>
        /// <param name="strCategory">The category for the item.</param>
        /// <param name="fltMinFWVersion">The minimum firmware version needed to validate this item.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/30/09 jrf	2.30.22		   Created
        //
        public CENTRONIIEDLValidationItem(long item, int[] index, string strName, string strCategory, float fltMinFWVersion)
        {
            m_Item = item;
            m_Index = index;
            m_strName = strName;
            m_strCategory = strCategory;
            m_fltMinFWVersion = fltMinFWVersion;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the CentronTblEnum value for the item.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created

        public long Item
        {
            get
            {
                return m_Item;
            }
        }

        /// <summary>
        /// Gets the index of the item.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created

        public int[] Index
        {
            get
            {
                return m_Index;
            }
        }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created

        public string Name
        {
            get
            {
                return m_strName;
            }
        }

        /// <summary>
        /// Gets the category of the item.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created

        public string Category
        {
            get
            {
                return m_strCategory;
            }
        }

        /// <summary>
        /// Gets the category of the item.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/30/09 jrf	2.30.22		   Created
        //
        public float MinFWVersion
        {
            get
            {
                return m_fltMinFWVersion;
            }
        }

        #endregion

        #region Member Variables

        private long m_Item;
        private int[] m_Index;
        private string m_strName;
        private string m_strCategory;
        private float m_fltMinFWVersion;

        #endregion
    }
}
