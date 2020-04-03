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
//                           Copyright © 2007 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using Itron.Common.C1219Tables.Centron;
using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Progressable;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    partial class CENTRON_AMI : IValidateProgram
    {
        #region Constants

        private const uint ENERGY_LID_BASE = 0x14000080;
        private const string VA_SSQ_NAME = "VA";
        private const string VAR_SSQ_NAME = "VAR";
        private const string VA_SSQ_VALUE = "1";
        private const string VAR_SSQ_VALUE = "2";
        private const string VA_ARITH_NAME = "Arithmetic";
        private const string VA_VECT_NAME = "Vectorial";
        private const string VA_ARITH_VALUE = "0";
        private const string VA_VECT_VALUE = "1";
        private const string BASE_QUANTITIES_CATEGORY = "Base Quantities";
        private const string VA_BASE_ENERGY_NAME = "VAh";
        private const string VAR_BASE_ENERGY_NAME = "VARh";
        private const string WATT_BASE_ENERGY_NAME = "Wh";
        private const string VA_BASE_DEMAND_NAME = "VA";
        private const string VAR_BASE_DEMAND_NAME = "VAR";
        private const string WATT_BASE_DEMAND_NAME = "W";
        private const string NOT_PROGRAMMED_QTY_NAME = "Not Programmed";


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
        // 02/19/13 AF  2.70.69 322427 Added ability to filter out obsolete items based on f/w version 
        // 12/18/13 jrf 3.50.16 TQ9560 Refactored update of TOU data in ProgramTables.
        //
        public virtual List<ProgramValidationItem> ValidateProgram(string strProgramName)
        {
            PSEMResponse Response = PSEMResponse.Ok;

            List<ProgramValidationItem> InvalidItemsList = new List<ProgramValidationItem>();
            List<EDLValidationItem> ItemsToValidate = GetValidationList();
            List<ushort> ValidationTablesToRead = GetValidationTablesToRead();

            FileStream EDLFileStream = new FileStream(strProgramName, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlTextReader EDLReader = new XmlTextReader(EDLFileStream);

            CentronTables ProgramTables = new CentronTables();
            CentronTables MeterTables = new CentronTables();


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

                //Update the TOU in the program file.
                UpdateTOU(ProgramTables);

                // Compare the values
                foreach (EDLValidationItem Item in ItemsToValidate)
                {
                    ProgramValidationItem InvalidItem;

                    // Only compare items where the meter's FW version is greater than or equal to 
                    // the minimum required for that item and the FW version is less than
                    // the version in which it became obsolete.  Max set to high for active items. 
                    if ((VersionChecker.CompareTo(FWRevision, Item.MinFWVersion) >= 0) &&
                        (VersionChecker.CompareTo(FWRevision, Item.MaxFWVersion) < 0))
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
        // 04/29/15 AF  4.20.03 577895 Added table 2537
        //
        protected virtual bool RequiresSpecialHandling(long item)
        {
            bool bRequired = false;

            switch (item)
            {
                case (long)CentronTblEnum.MFGTBL0_DISPLAY_ITEMS:
                case (long)StdTableEnum.STDTBL123_EXCEPTION_REPORT:
                case (long)CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifier:
                case (long)CentronTblEnum.MFGTBL489_EXCEPTION_REPORT:
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
        // 04/29/15 AF  4.20.03 577895 Added a case for the table 2537 items
        //
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
                case (long)CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifier:
                    {
                        InvalidItems = GetInvalidExtendedSelfReadItems(meterTables, programTables);
                        break;
                    }
                case (long)CentronTblEnum.MFGTBL489_EXCEPTION_REPORT:
                    {
                        InvalidItems = GetInvalidICMExceptionItems(meterTables, programTables);
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
        /// Gets a list of invalid Extended Self Read items.
        /// </summary>
        /// <param name="meterTables">The table structure for the meter.</param>
        /// <param name="programTables">The table structure for the program.</param>
        /// <returns>The list of invalid items.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#    Description
        // -------- --- ------- -------   ---------------------------------------
        // 02/08/12 RCG	2.53.39	TRQ3445   Created
        // 03/22/13 jrf 2.80.10           Adding support for ITRJ meter.
        // 07/19/16 MP  4.70.8  WR701234  Changed lid descriptions to match changes to quantity names. Added more quantites
        private List<ProgramValidationItem> GetInvalidExtendedSelfReadItems(CentronTables meterTables, CentronTables programTables)
        {
            List<ProgramValidationItem> InvalidItems = new List<ProgramValidationItem>();
            List<ExtendedSelfReadConfigRecord> MeterRecords = new List<ExtendedSelfReadConfigRecord>();
            List<ExtendedSelfReadConfigRecord> ProgramRecords = new List<ExtendedSelfReadConfigRecord>();
            ProgramValidationItem CurrentItem = null;

            // Get the configured LIDs from the meter and program.

            for (int iIndex = 0; iIndex < 16; iIndex++)
            {
                int[] Indexer = { iIndex };
                object objValue;
                uint LIDValue;
                byte QualifierValue;

                // Add the Meter Value
                if (meterTables.IsCached((long)CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifier, Indexer)
                    && meterTables.IsCached((long)CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifierQualifier, Indexer))
                {
                    meterTables.GetValue(CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifier, Indexer, out objValue);
                    LIDValue = (uint)objValue;

                    meterTables.GetValue(CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifierQualifier, Indexer, out objValue);
                    QualifierValue = (byte)objValue;

                    if (LIDValue != 0 && LIDValue != uint.MaxValue)
                    {
                        MeterRecords.Add(new ExtendedSelfReadConfigRecord(CreateLID(LIDValue), (ExtendedSelfReadQualifier)QualifierValue));
                    }
                }

                // Add the Program Value
                if (programTables.IsCached((long)CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifier, Indexer)
                    && programTables.IsCached((long)CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifierQualifier, Indexer))
                {
                    programTables.GetValue(CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifier, Indexer, out objValue);
                    LIDValue = (uint)objValue;

                    programTables.GetValue(CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifierQualifier, Indexer, out objValue);
                    QualifierValue = (byte)objValue;

                    if (LIDValue != 0 && LIDValue != uint.MaxValue)
                    {
                        ProgramRecords.Add(new ExtendedSelfReadConfigRecord(CreateLID(LIDValue), (ExtendedSelfReadQualifier)QualifierValue));
                    }
                }
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins W", "Instantaneous Watts Total");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins W(a)", "Instantaneous Watts Phase A");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            if (DeviceClass != ITR1_DEVICE_CLASS && DeviceClass != ITRD_DEVICE_CLASS && DeviceClass != ITRJ_DEVICE_CLASS)
            {
                CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins W(b)", "Instantaneous Watts Phase B");

                if (CurrentItem != null)
                {
                    InvalidItems.Add(CurrentItem);
                }
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins W(c)", "Instantaneous Watts Phase C");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins VAR", "Instantaneous Vars Total");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins VAR(a)", "Instantaneous Vars Phase A");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            if (DeviceClass != ITR1_DEVICE_CLASS && DeviceClass != ITRD_DEVICE_CLASS && DeviceClass != ITRJ_DEVICE_CLASS)
            {

                CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins VAR(b)", "Instantaneous Vars Phase B");

                if (CurrentItem != null)
                {
                    InvalidItems.Add(CurrentItem);
                }
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins VAR(c)", "Instantaneous Vars Phase C");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins PF", "Instantaneous Power Factor Total");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins PF(a)", "Instantaneous Power Factor Phase A");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins PF(b)", "Instantaneous Power Factor Phase B");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins PF(c)", "Instantaneous Power Factor Phase C");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins V(a)", "Instantaneous Volts Phase A");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            if (DeviceClass != ITR1_DEVICE_CLASS && DeviceClass != ITRD_DEVICE_CLASS && DeviceClass != ITRJ_DEVICE_CLASS)
            {

                CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins V(b)", "Instantaneous Volts Phase B");

                if (CurrentItem != null)
                {
                    InvalidItems.Add(CurrentItem);
                }
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins V(c)", "Instantaneous Volts Phase C");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins VA", "Instantaneous VA");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins VA Arith", "Instantaneous VA Arithmetic");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            CurrentItem = CheckExtendedSelfReadItemValidity(MeterRecords, ProgramRecords, "ins VA Vec", "Instantaneous VA Vectorial");

            if (CurrentItem != null)
            {
                InvalidItems.Add(CurrentItem);
            }

            return InvalidItems;
        }

        /// <summary>
        /// Checks to see if the Extended Self Read Item is valid
        /// </summary>
        /// <param name="meterRecords">The Extended Self Read configuration items for the meter</param>
        /// <param name="programRecords">The Extended Self Read configuration items for the program</param>
        /// <param name="lidDescription">The LID description of the item to validate</param>
        /// <param name="itemDescription">The description of the item being validated</param>
        /// <returns>Null if the item is valid. The Validation Item if the item is invalid.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------- ---------------------------------------
        // 02/08/12 RCG	2.53.39	TRQ3445 Created

        private ProgramValidationItem CheckExtendedSelfReadItemValidity(List<ExtendedSelfReadConfigRecord> meterRecords, List<ExtendedSelfReadConfigRecord> programRecords, string lidDescription, string itemDescription)
        {
            ProgramValidationItem InvalidItem = null;
            List<ExtendedSelfReadConfigRecord> SearchRecords = null;

            string strProgramValue;
            string strMeterValue;


            // Search for each of the options in the configuration list. If they are not present then our value is "Ignore" otherwise the value
            // is dependant on the qualifier.

            SearchRecords = meterRecords.FindAll(r => r.Quantity.lidDescription.Equals(lidDescription));

            if (SearchRecords.Count > 0)
            {
                strMeterValue = EnumDescriptionRetriever.RetrieveDescription(SearchRecords[0].Qualifier);
            }
            else
            {
                strMeterValue = "Ignore";
            }

            SearchRecords = programRecords.FindAll(r => r.Quantity.lidDescription.Equals(lidDescription));

            if (SearchRecords.Count > 0)
            {
                strProgramValue = EnumDescriptionRetriever.RetrieveDescription(SearchRecords[0].Qualifier);
            }
            else
            {
                strProgramValue = "Ignore";
            }

            if (strMeterValue.Equals(strProgramValue) == false)
            {
                InvalidItem = new ProgramValidationItem("Extended Self Read", itemDescription, strProgramValue, strMeterValue);
            }

            return InvalidItem;
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
        // 03/06/12 AF  2.53.48 189964 We can no longer assume that the meter and program items will
        //                             be in the same order. We have to search both lists for mismatches

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
            if (programTables.IsAllCached(123) && VersionChecker.CompareTo(FWRevision, VERSION_2_SP5) > 0)
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

                    List<ushort> meterEventsList = new List<ushort>();
                    List<ushort> programEventsList = new List<ushort>();
                    List<ushort> invalidMeterItemsList = new List<ushort>();
                    List<ushort> invalidProgramItemsList = new List<ushort>();
                    // the event number is just an index used for labeling the invalid item
                    int iEventNumber = 1;

                    int[] iIndexer = { iHost };
                    string strCategory = "Exception Host " + (iHost + 1).ToString(CultureInfo.CurrentCulture);

                    ValidationItem = ValidateItem(new EDLValidationItem((long)StdTableEnum.STDTBL123_APTITLE_NOTIFY, iIndexer, "Exception Host Aptitle", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    ValidationItem = ValidateItem(new EDLValidationItem((long)StdTableEnum.STDTBL123_MAX_NUMBER_OF_RETRIES, iIndexer, "Max Number of Retries", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    ValidationItem = ValidateItem(new EDLValidationItem((long)StdTableEnum.STDTBL123_RETRY_DELAY, iIndexer, "Retry Delay", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    ValidationItem = ValidateItem(new EDLValidationItem((long)StdTableEnum.STDTBL123_EXCLUSION_PERIOD, iIndexer, "Exclusion Period", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    if (meterValidHost == true)
                    {
                        for (int iEvent = 0; iEvent < usMeterEvents; iEvent++)
                        {
                            meterTables.GetValue(StdTableEnum.STDTBL123_EVENT_REPORTED, new int[] { iHost, iEvent }, out objMeterValue);
                            meterEventsList.Add((ushort)objMeterValue);
                        }
                    }

                    if (programValidHost == true)
                    {
                        for (int iEvent = 0; iEvent < usProgramEvents; iEvent++)
                        {
                            programTables.GetValue(StdTableEnum.STDTBL123_EVENT_REPORTED, new int[] { iHost, iEvent }, out objProgramValue);
                            programEventsList.Add((ushort)objProgramValue);
                        }
                    }

                    foreach (ushort item in meterEventsList)
                    {
                        if (!programEventsList.Contains(item))
                        {
                            invalidMeterItemsList.Add(item);
                        }
                    }

                    foreach (ushort item in invalidMeterItemsList)
                    {
                        string strMeterException = "";

                        if (EventDescriptions.TryGetValue((int)item, out strMeterException) == false)
                        {
                            // The TryGetValue failed so say it is an unknown event.
                            strMeterException = "Unknown Event " + item.ToString(CultureInfo.InvariantCulture);
                        }

                        // We don't know which program item this belongs with so just make the program item blank
                        InvalidItems.Add(new ProgramValidationItem(strCategory, "Event " + iEventNumber.ToString(CultureInfo.CurrentCulture),
                            "", strMeterException));

                        iEventNumber++;
                    }

                    foreach (ushort item in programEventsList)
                    {
                        if (!meterEventsList.Contains(item))
                        {
                            invalidProgramItemsList.Add(item);
                        }
                    }

                    foreach (ushort item in invalidProgramItemsList)
                    {
                        string strProgramException = "";

                        if (EventDescriptions.TryGetValue((int)item, out strProgramException) == false)
                        {
                            // The TryGetValue failed so say it is an unknown event.
                            strProgramException = "Unknown Event " + item.ToString(CultureInfo.InvariantCulture);
                        }

                        // We don't know which meter item this belongs with so just make the meter item blank
                        InvalidItems.Add(new ProgramValidationItem(strCategory, "Event " + iEventNumber.ToString(CultureInfo.CurrentCulture),
                            strProgramException, ""));

                        iEventNumber++;
                    }
                }
            }

            return InvalidItems;
        }

        /// <summary>
        /// Gets a list of invalid ICM exception items.
        /// </summary>
        /// <param name="meterTables">The table structure for the meter</param>
        /// <param name="programTables">The table structure for the program</param>
        /// <returns>The list of invalid items</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/29/15 AF  4.20.03  WR 577895  Created
        //
        protected virtual List<ProgramValidationItem> GetInvalidICMExceptionItems(CentronTables meterTables, CentronTables programTables)
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

            // We shouldn't validate this if Table 2537 is not cached in the program.
            //if (programTables.IsAllCached(2536) && programTables.IsAllCached(2537))
            if (programTables.IsAllCached(2537))
            {
                // Get the number of exception hosts
                meterTables.GetValue(CentronTblEnum.MFGTBL488_NBR_EXCEPTION_HOSTS, null, out objMeterValue);
                programTables.GetValue(CentronTblEnum.MFGTBL488_NBR_EXCEPTION_HOSTS, null, out objProgramValue);

                usMeterHosts = (ushort)objMeterValue;
                usProgramHosts = (ushort)objProgramValue;

                // Get the number of exception events
                meterTables.GetValue(CentronTblEnum.MFGTBL488_NBR_EXCEPTION_EVENTS, null, out objMeterValue);
                programTables.GetValue(CentronTblEnum.MFGTBL488_NBR_EXCEPTION_EVENTS, null, out objProgramValue);

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

                    List<ushort> meterEventsList = new List<ushort>();
                    List<ushort> programEventsList = new List<ushort>();
                    List<ushort> invalidMeterItemsList = new List<ushort>();
                    List<ushort> invalidProgramItemsList = new List<ushort>();
                    // the event number is just an index used for labeling the invalid item
                    int iEventNumber = 1;

                    int[] iIndexer = { iHost };
                    string strCategory = "ICM Exception Host " + (iHost + 1).ToString(CultureInfo.CurrentCulture);

                    ValidationItem = ValidateItem(new EDLValidationItem((long)CentronTblEnum.MFGTBL489_APTITLE_NOTIFY, iIndexer, "Called Aptitle", strCategory),
                                                       meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    ValidationItem = ValidateItem(new EDLValidationItem((long)CentronTblEnum.MFGTBL489_MAX_NUMBER_OF_RETRIES, iIndexer, "Max Number of Retries", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    ValidationItem = ValidateItem(new EDLValidationItem((long)CentronTblEnum.MFGTBL489_RETRY_DELAY, iIndexer, "Retry Delay", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    ValidationItem = ValidateItem(new EDLValidationItem((long)CentronTblEnum.MFGTBL489_EXCLUSION_PERIOD, iIndexer, "Exclusion Period", strCategory),
                                                    meterTables, programTables);

                    if (ValidationItem != null)
                    {
                        InvalidItems.Add(ValidationItem);
                    }

                    if (meterValidHost == true)
                    {
                        for (int iEvent = 0; iEvent < usMeterEvents; iEvent++)
                        {
                            meterTables.GetValue(CentronTblEnum.MFGTBL489_EVENT_REPORTED, new int[] { iHost, iEvent }, out objMeterValue);
                            meterEventsList.Add((ushort)objMeterValue);
                        }
                    }

                    if (programValidHost == true)
                    {
                        for (int iEvent = 0; iEvent < usProgramEvents; iEvent++)
                        {
                            programTables.GetValue(CentronTblEnum.MFGTBL489_EVENT_REPORTED, new int[] { iHost, iEvent }, out objProgramValue);
                            programEventsList.Add((ushort)objProgramValue);
                        }
                    }

                    foreach (ushort item in meterEventsList)
                    {
                        if (!programEventsList.Contains(item))
                        {
                            invalidMeterItemsList.Add(item);
                        }
                    }

                    ANSIEventDictionary ICMEventDescriptions = (ANSIEventDictionary)(new ICS_Gateway_EventDictionary());

                    foreach (ushort item in invalidMeterItemsList)
                    {
                        string strMeterException = "";


                        if (ICMEventDescriptions.TryGetValue((int)item, out strMeterException) == false)
                        {
                            // The TryGetValue failed so say it is an unknown event.
                            strMeterException = "Unknown Event " + item.ToString(CultureInfo.InvariantCulture);
                        }

                        // We don't know which program item this belongs with so just make the program item blank
                        InvalidItems.Add(new ProgramValidationItem(strCategory, "Event " + iEventNumber.ToString(CultureInfo.CurrentCulture),
                            "", strMeterException));

                        iEventNumber++;
                    }

                    foreach (ushort item in programEventsList)
                    {
                        if (!meterEventsList.Contains(item))
                        {
                            invalidProgramItemsList.Add(item);
                        }
                    }

                    foreach (ushort item in invalidProgramItemsList)
                    {
                        string strProgramException = "";

                        if (ICMEventDescriptions.TryGetValue((int)item, out strProgramException) == false)
                        {
                            // The TryGetValue failed so say it is an unknown event.
                            strProgramException = "Unknown Event " + item.ToString(CultureInfo.InvariantCulture);
                        }

                        // We don't know which meter item this belongs with so just make the meter item blank
                        InvalidItems.Add(new ProgramValidationItem(strCategory, "Event " + iEventNumber.ToString(CultureInfo.CurrentCulture),
                            strProgramException, ""));

                        iEventNumber++;
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
                if (iMeterIndex <= byMeterNormalEnd)
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
        // 06/17/11 jrf 2.51.13 175979 Adding validation of power monitoring enabled.
        // 02/23/12 jrf 2.53.43 194414 For case where LP and IP lid values are checked and case where
        //                             pulse weights are checked, treating a max value in meter as unconfigured.
        // 03/27/12 RCG 2.53.52 195665 Changing validation of Tariff ID to also look at the Calendar ID
        // 04/05/12 jrf 2.53.54 195670 Skipping validation of ZigBee Power Level and Inter PAN Mode if the program
        //                             value is null and the program came from a Lithium or greater CE.
        // 04/10/12 jrf 2.53.54 195674 Refactored check of CE version for certain items into its own method.  
        //                             Modified to only check legacy voltage monitoring items when config is pre-Lithium
        //                             and only check extended voltage monitoring items when config is Lithium.
        // 04/12/12 jrf 2.53.55 195670 Correcting cut and paste error from previous checkin in validation of 
        //                             MfgTbl58PowerLevel item.
        // 08/12/13 jrf 2.85.16 TQ7657 Adding special cases for new Michigan validation items.
        // 08/14/13 jrf 2.85.16 TQ7657 Modified special case for ERT Radio Disabled since we have to reverse the 
        //                             value we retrieve since it tells us the opposite (ERT Radio is Enabled).
        // 12/18/13 jrf 3.50.16 TQ9560 Refactored retreival of a CentronTables value into its own method.
        // 12/05/14 AF  4.00.90 550100 For TOU validation, the check on std table 6 and mfg table 42 in the meter was replaced
        //                             with a check on the calendar id in 2048.  I would have liked to have done the same
        //                             for the program, but the CE dll does not cache the table 2048 calendar id from the 
        //                             program (even though it is there).
        // 05/19/16 PGH 4.50.270 687369 At Beryllium firmware, temperature config items and the flicker config item have default values in the meter
        //                              so a meter value will always return. Therefore, you must check that the program value counterpart exists before comparing them.
        // 05/20/16 PGH 4.50.270 687369 Allow the meter value object to be null for the temperature and flicker items
        // 05/24/16 PGH 4.50.271 687369 For temperature and flicker items, show errors when the program value object for the item is null but the configuration is enabled.
        // 05/24/16 PGH 4.50.271 687369 Default value for power up threshold (flicker) is 65535 not 0 (zero).
        // 06/09/16 jrf 4.50.281 633121 Added validation items to validate conifguration secondary saved quantity against meter secondary saved quantity
        //                              and also added validation of configuration energy/demand quantities against meter's secondary saved quantity.
        // 06/14/16 PGH 4.50.283 693234 Added CTE and register operation items
        // 06/21/16 jrf 4.50.289 695777 Added case for validating push data set quantities.
        // 10/17/16 AF  4.70.26  700198 Added a case for CentronTblEnum.MfgTbl217DataPushSetVoltageMonitoringIntervalData to prevent validation
        //                              mismatches when program does not contain data push items and meter is not configured for it.
        // 11/02/16 AF  4.70.29  726204 Further tweaks to the data push items.  If the meter does not support data push or it has never been configured,
        //                              the fields fall within the unused area and will have default values of all 0xFFs. We shouldn't show mismatches in
        //                              in that case with a program that does not not data push configured.
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.Compare(System.String,System.String)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.Compare(System.String,System.String,System.Boolean)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.Compare(System.String,System.String,System.Boolean)")]
        protected virtual ProgramValidationItem ValidateItem(EDLValidationItem item, CentronTables meterTables, CentronTables programTables)
        {
            bool bItemsMatch = false;

            string strDisplayMeterValue = "";
            string strDisplayProgramValue = "";

            object objMeterValue;
            object objProgramValue;

            ProgramValidationItem InvalidItem = null;

            // Get the values
            objMeterValue = GetTableValue(item, meterTables);

            objProgramValue = GetTableValue(item, programTables);

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
                case (long)StdTableEnum.STDTBL6_TARIFF_ID:
                    {
                        object objMeterCalendarIDValue = null;
                        object objProgramCalendarIDValue;
                        ushort usMeterCalendarID = 0;
                        ushort usProgramCalendarID = 0;
                        string strProgramTariffID = null;

                        meterTables.GetValue(CentronTblEnum.MFGTBL0_CALENDAR_ID, null, out objMeterCalendarIDValue);
                        programTables.GetValue(CentronTblEnum.MFGTBL42_CALENDAR_ID, null, out objProgramCalendarIDValue);

                        if (objMeterCalendarIDValue != null)
                        {
                            usMeterCalendarID = (ushort)objMeterCalendarIDValue;
                        }

                        if (objProgramCalendarIDValue != null)
                        {
                            usProgramCalendarID = (ushort)objProgramCalendarIDValue;
                        }

                        strProgramTariffID = objProgramValue as string;

                        if (usMeterCalendarID > 0)
                        {
                            strDisplayMeterValue = "Enabled";
                        }
                        else
                        {
                            strDisplayMeterValue = "Disabled";
                        }

                        if ((strProgramTariffID != null && strProgramTariffID.Length > 0) || usProgramCalendarID > 0)
                        {
                            strDisplayProgramValue = "Enabled";
                        }
                        else
                        {
                            strDisplayProgramValue = "Disabled";
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
                        //This item will be listed twice. When it is in the
                        //base quantities category we will validate it against 
                        //energy stored in base.
                        if (item.Category == BASE_QUANTITIES_CATEGORY)
                        {

                            if (SecondaryQuantity == BaseEnergies.VarhArithmetic
                                || SecondaryQuantity == BaseEnergies.VarhVectorial)
                            {
                                strDisplayMeterValue = VAR_BASE_ENERGY_NAME;
                            }
                            else if (SecondaryQuantity == BaseEnergies.VAhArithmetic
                                || SecondaryQuantity == BaseEnergies.VAhVectorial)
                            {
                                strDisplayMeterValue = VA_BASE_ENERGY_NAME;
                            }

                            if (objProgramValue != null)
                            {
                                byte byProgramValue = (byte)objProgramValue;


                                // If the value is 0xFF then that means that it has likely never been configured
                                if (byProgramValue == 0xFF)
                                {
                                    byProgramValue = 0;
                                }

                                uint uiProgramLID = ENERGY_LID_BASE + byProgramValue;
                                LID LidValue = CreateLID(uiProgramLID);

                                switch (LidValue.lidQuantity)
                                {
                                    case DefinedLIDs.WhichOneEnergyDemand.WH_DELIVERED:
                                    case DefinedLIDs.WhichOneEnergyDemand.WH_RECEIVED:
                                    case DefinedLIDs.WhichOneEnergyDemand.WH_NET_PHA:
                                    case DefinedLIDs.WhichOneEnergyDemand.WH_NET_PHB:
                                    case DefinedLIDs.WhichOneEnergyDemand.WH_NET_PHC:
                                    case DefinedLIDs.WhichOneEnergyDemand.WH_NET:
                                    case DefinedLIDs.WhichOneEnergyDemand.WH_UNI:
                                        strDisplayProgramValue = WATT_BASE_ENERGY_NAME;
                                        strDisplayMeterValue = WATT_BASE_ENERGY_NAME;//Meter always supports Wh
                                        break;
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_DEL:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_REC:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_NET:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_Q1:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_Q2:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_Q3:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_Q4:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_NET_DEL:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_NET_REC:
                                        strDisplayProgramValue = VAR_BASE_ENERGY_NAME;
                                        break;
                                    case DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_ARITH:
                                    case DefinedLIDs.WhichOneEnergyDemand.VAH_REC_ARITH:
                                    case DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_VECT:
                                    case DefinedLIDs.WhichOneEnergyDemand.VAH_REC_VECT:
                                    case DefinedLIDs.WhichOneEnergyDemand.VAH_LAG:
                                        strDisplayProgramValue = VA_BASE_ENERGY_NAME;
                                        break;
                                    case DefinedLIDs.WhichOneEnergyDemand.NOT_PROGRAMMED:
                                        strDisplayProgramValue = NOT_PROGRAMMED_QTY_NAME;
                                        break;
                                    default:
                                        strDisplayProgramValue = "Unsupported ("
                                            + LidValue.lidQuantity.ToDescription() + ")";
                                        break;
                                }

                            }

                            if (null == objProgramValue ||
                                NOT_PROGRAMMED_QTY_NAME == strDisplayProgramValue ||
                                strDisplayProgramValue == strDisplayMeterValue)
                            {
                                bItemsMatch = true;
                            }

                        }
                        else
                        {
                            byte byMeterValue = 0;
                            byte byProgramValue = 0;

                            if (objMeterValue != null)
                            {
                                byMeterValue = (byte)objMeterValue;

                                // If the value is 0xFF then that means that it has likely never been configured
                                if (byMeterValue == 0xFF)
                                {
                                    byMeterValue = 0;
                                }
                            }

                            if (objProgramValue != null)
                            {
                                byProgramValue = (byte)objProgramValue;

                                // If the value is 0xFF then that means that it has likely never been configured
                                if (byProgramValue == 0xFF)
                                {
                                    byProgramValue = 0;
                                }
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
                        }

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl217NonBillableEnergyId:
                    {
                        byte byMeterValue = 0;
                        byte byProgramValue = 0;

                        if (objMeterValue != null)
                        {
                            byMeterValue = (byte)objMeterValue;

                            // If the value is 0xFF then that means that it has likely never been configured
                            if (byMeterValue == 0xFF)
                            {
                                byMeterValue = 0;
                            }
                        }

                        if (objProgramValue != null)
                        {
                            byProgramValue = (byte)objProgramValue;

                            // If the value is 0xFF then that means that it has likely never been configured
                            if (byProgramValue == 0xFF)
                            {
                                byProgramValue = 0;
                            }
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
                case (long)CentronTblEnum.MFGTBL0_DEMAND_DEFINITION:
                    {
                        //This item will be listed twice. When it is in the
                        //base quantities category we will validate it against 
                        //energy stored in base.
                        if (item.Category == BASE_QUANTITIES_CATEGORY)
                        {

                            if (SecondaryQuantity == BaseEnergies.VarhArithmetic
                                || SecondaryQuantity == BaseEnergies.VarhVectorial)
                            {
                                strDisplayMeterValue = VAR_BASE_DEMAND_NAME;
                            }
                            else if (SecondaryQuantity == BaseEnergies.VAhArithmetic
                                || SecondaryQuantity == BaseEnergies.VAhVectorial)
                            {
                                strDisplayMeterValue = VA_BASE_DEMAND_NAME;
                            }

                            if (objProgramValue != null)
                            {
                                uint uiProgramValue = (uint)objProgramValue;


                                // If the value is 0xFF then that means that it has likely never been configured
                                if (uiProgramValue == 0xFF)
                                {
                                    uiProgramValue = 0;
                                }

                                LID LidValue = CreateLID(uiProgramValue);

                                switch (LidValue.lidQuantity)
                                {
                                    case DefinedLIDs.WhichOneEnergyDemand.WH_DELIVERED:
                                    case DefinedLIDs.WhichOneEnergyDemand.WH_RECEIVED:
                                    case DefinedLIDs.WhichOneEnergyDemand.WH_NET_PHA:
                                    case DefinedLIDs.WhichOneEnergyDemand.WH_NET_PHB:
                                    case DefinedLIDs.WhichOneEnergyDemand.WH_NET_PHC:
                                    case DefinedLIDs.WhichOneEnergyDemand.WH_NET:
                                    case DefinedLIDs.WhichOneEnergyDemand.WH_UNI:
                                        strDisplayProgramValue = WATT_BASE_DEMAND_NAME;
                                        strDisplayMeterValue = WATT_BASE_DEMAND_NAME;//Meter always supports W
                                        break;
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_DEL:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_REC:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_NET:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_Q1:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_Q2:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_Q3:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_Q4:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_NET_DEL:
                                    case DefinedLIDs.WhichOneEnergyDemand.VARH_NET_REC:
                                        strDisplayProgramValue = VAR_BASE_DEMAND_NAME;
                                        break;
                                    case DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_ARITH:
                                    case DefinedLIDs.WhichOneEnergyDemand.VAH_REC_ARITH:
                                    case DefinedLIDs.WhichOneEnergyDemand.VAH_DEL_VECT:
                                    case DefinedLIDs.WhichOneEnergyDemand.VAH_REC_VECT:
                                    case DefinedLIDs.WhichOneEnergyDemand.VAH_LAG:
                                        strDisplayProgramValue = VA_BASE_DEMAND_NAME;
                                        break;
                                    case DefinedLIDs.WhichOneEnergyDemand.NOT_PROGRAMMED:
                                        strDisplayProgramValue = NOT_PROGRAMMED_QTY_NAME;
                                        break;
                                    default:
                                        strDisplayProgramValue = "Unsupported ("
                                            + LidValue.lidQuantity.ToDescription() + ")";
                                        break;
                                }
                            }

                            if (null == objProgramValue ||
                                NOT_PROGRAMMED_QTY_NAME == strDisplayProgramValue ||
                                strDisplayProgramValue == strDisplayMeterValue)
                            {
                                bItemsMatch = true;
                            }

                        }
                        else
                        {
                            uint uiMeterValue = 0;
                            uint uiProgramValue = 0;

                            //null or a max value means it is unconfigured.  Leave value at 0.
                            if (objMeterValue != null && UInt32.MaxValue != (uint)objMeterValue)
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
                        }

                        break;
                    }
                case (long)CentronTblEnum.MFGTBL0_DISPLAY_LID:
                case (long)CentronTblEnum.MFGTBL0_LP_LID:
                case (long)CentronTblEnum.MfgTbl217InstrumentationProfileChannelLogicalIdentifier:
                case (long)CentronTblEnum.MfgTbl217NonBillingLoadProfileChannelLogicalIdentifier:
                    {
                        uint uiMeterValue = 0;
                        uint uiProgramValue = 0;

                        //null or a max value means it is unconfigured.  Leave value at 0.
                        if (objMeterValue != null && UInt32.MaxValue != (uint)objMeterValue)
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
                case (long)CentronTblEnum.MfgTbl217InstrumentationProfilePulseWeight:
                case (long)CentronTblEnum.MfgTbl217NonBillingLoadProfilePulseWeight:
                    {
                        ushort usMeterValue = 0;
                        ushort usProgramValue = 0;

                        //null or a max value means it is unconfigured. Leave value at 0.
                        if (objMeterValue != null && UInt16.MaxValue != (ushort)objMeterValue)
                        {
                            usMeterValue = (ushort)objMeterValue;
                        }

                        if (objProgramValue != null)
                        {
                            usProgramValue = (ushort)objProgramValue;
                        }

                        if (usMeterValue == usProgramValue)
                        {
                            bItemsMatch = true;
                        }
                        else
                        {
                            // The Pulse Weights are stored as integers so we need to convert them to a float value
                            double dMeterValue = usMeterValue / 100.0;
                            double dProgramValue = usProgramValue / 100.0;

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
                        //System.Console.WriteLine("Checking MFGTBL42_DST_HOUR");
                        if (objProgramValue == null)
                        {
                            programTables.GetValue(CentronTblEnum.MFGTBL0_DST_HOUR, item.Index, out objProgramValue);
                        }

                        if (objProgramValue != null)
                        {
                            byProgramValue = (byte)objProgramValue;
                        }

                        if (objMeterValue != null)
                        {
                            byMeterValue = (byte)objMeterValue;
                        }

                        if (byProgramValue == byMeterValue)
                        {
                            bItemsMatch = true;
                        }
                        else
                        {
                            // also check MfgTbl212
                            //System.Console.WriteLine("Checking MfgTbl212TimeConfigHour");
                            try
                            {
                                // required to create new Item, otherwise there is a failure in PSEM Logoff and a Failure response is not returned to caller.
                                item = new EDLValidationItem((long)CentronTblEnum.MfgTbl212TimeConfigHour,
                                                            null, "DST Switch Time Hour", "TOU/Time");
                                meterTables.GetValue(item.Item, item.Index, out objMeterValue);
                            }
                            catch (Exception)
                            {
                                // We failed to get the value so set it to null
                                objMeterValue = null;
                            }

                            if (objMeterValue != null)
                            {
                                byMeterValue = (byte)objMeterValue;
                            }

                            if (byProgramValue == byMeterValue)
                            {
                                //System.Console.WriteLine("Found MfgTbl212TimeConfigHour");
                                bItemsMatch = true;
                            }
                            else
                            {
                                strDisplayProgramValue = byProgramValue.ToString(CultureInfo.InvariantCulture);
                                strDisplayMeterValue = byMeterValue.ToString(CultureInfo.InvariantCulture);
                            }
                        }

                        break;
                    }
                case (long)CentronTblEnum.MFGTBL42_DST_MINUTE:
                    {
                        byte byProgramValue = 0;
                        byte byMeterValue = 0;

                        // EDL files with the old TOU information may have this information
                        // stored in MFGTBL0 so we should check there if the program value is null
                        //System.Console.WriteLine("Checking MFGTBL42_DST_MINUTE");
                        if (objProgramValue == null)
                        {
                            programTables.GetValue(CentronTblEnum.MFGTBL0_DST_MINUTE, item.Index, out objProgramValue);
                        }

                        if (objProgramValue != null)
                        {
                            byProgramValue = (byte)objProgramValue;
                        }

                        if (objMeterValue != null)
                        {
                            byMeterValue = (byte)objMeterValue;
                        }

                        if (byProgramValue == byMeterValue)
                        {
                            bItemsMatch = true;
                        }
                        else
                        {
                            // also check MfgTbl212
                            //System.Console.WriteLine("Checking MfgTbl212TimeConfigMinute");
                            try
                            {
                                // required to create new Item, otherwise there is a failure in PSEM Logoff and a Failure response is not returned to caller.
                                item = new EDLValidationItem((long)CentronTblEnum.MfgTbl212TimeConfigMinute,
                                                            null, "DST Switch Time Minute", "TOU/Time");
                                meterTables.GetValue(item.Item, item.Index, out objMeterValue);
                            }
                            catch (Exception)
                            {
                                // We failed to get the value so set it to null
                                objMeterValue = null;
                            }

                            if (objMeterValue != null)
                            {
                                byMeterValue = (byte)objMeterValue;
                            }

                            if (byProgramValue == byMeterValue)
                            {
                                //System.Console.WriteLine("Found MfgTbl212TimeConfigMinute");
                                bItemsMatch = true;
                            }
                            else
                            {
                                strDisplayProgramValue = byProgramValue.ToString(CultureInfo.InvariantCulture);
                                strDisplayMeterValue = byMeterValue.ToString(CultureInfo.InvariantCulture);
                            }
                        }

                        break;
                    }
                case (long)CentronTblEnum.MFGTBL42_DST_OFFSET:
                    {
                        byte byProgramValue = 0;
                        byte byMeterValue = 0;

                        // EDL files with the old TOU information may have this information
                        // stored in MFGTBL0 so we should check there if the program value is null
                        //System.Console.WriteLine("Checking MFGTBL42_DST_OFFSET");
                        if (objProgramValue == null)
                        {
                            programTables.GetValue(CentronTblEnum.MFGTBL0_DST_OFFSET, item.Index, out objProgramValue);
                        }

                        if (objProgramValue != null)
                        {
                            byProgramValue = (byte)objProgramValue;
                        }

                        if (objMeterValue != null)
                        {
                            byMeterValue = (byte)objMeterValue;
                        }

                        if (byProgramValue == byMeterValue)
                        {
                            bItemsMatch = true;
                        }
                        else
                        {
                            // also check MfgTbl212
                            //System.Console.WriteLine("Checking MfgTbl212TimeConfigOffset");
                            try
                            {
                                // required to create new Item, otherwise there is a failure in PSEM Logoff and a Failure response is not returned to caller.
                                item = new EDLValidationItem((long)CentronTblEnum.MfgTbl212TimeConfigOffset,
                                                            null, "DST Switch Length", "TOU/Time");
                                meterTables.GetValue(item.Item, item.Index, out objMeterValue);
                            }
                            catch (Exception)
                            {
                                // We failed to get the value so set it to null
                                objMeterValue = null;
                            }

                            if (objMeterValue != null)
                            {
                                byMeterValue = (byte)objMeterValue;
                            }

                            if (byProgramValue == byMeterValue)
                            {
                                //System.Console.WriteLine("Found MfgTbl212TimeConfigOffset");
                                bItemsMatch = true;
                            }
                            else
                            {
                                strDisplayProgramValue = byProgramValue.ToString(CultureInfo.InvariantCulture);
                                strDisplayMeterValue = byMeterValue.ToString(CultureInfo.InvariantCulture);
                            }
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
                case (long)CentronTblEnum.MFGTBL102_ENABLE_FLAG:
                case (long)CentronTblEnum.MFGTBL101_VM_INT_LEN:
                case (long)CentronTblEnum.MFGTBL102_RMS_VOLT_LOW_THRESHOLD:
                case (long)CentronTblEnum.MFGTBL102_RMS_VOLT_HIGH_THRESHOLD:
                    {
                        if (true == IsConfigVersionEqualOrGreaterThan(CE_VERSION_LITHIUM_3_9, programTables))
                        {
                            //Ignoring comparison if program's CE version is Lithium or greater
                            bItemsMatch = true;
                        }
                        else
                        {
                            bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
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
                        else if (true == IsConfigVersionEqualOrGreaterThan(CE_VERSION_LITHIUM_3_9, programTables))
                        {
                            //Ignoring comparison if values do not match and program's CE version is Lithium or greater
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
                        else if (true == IsConfigVersionEqualOrGreaterThan(CE_VERSION_LITHIUM_3_9, programTables))
                        {
                            //Ignoring comparison if values do not match and program's CE version is Lithium or greater
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

                        strDisplayMeterValue = CENTRON_AMI.DetermineDailySelfRead(byMeterValue);

                        if (objProgramValue != null)
                        {
                            byProgramValue = (byte)objProgramValue;
                        }

                        strDisplayProgramValue = CENTRON_AMI.DetermineDailySelfRead(byProgramValue);

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

                        if (VersionChecker.CompareTo(FWRevision, VERSION_2_SP5) > 0
                            || (VersionChecker.CompareTo(FWRevision, VERSION_2_SP5) == 0 && FirmwareBuild >= 56))
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
                        bool blnSkipComparison = false;

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
                        else if (true == IsConfigVersionEqualOrGreaterThan(CE_VERSION_LITHIUM_3_9, programTables))
                        {
                            //Only skipping comparison if program value is null and program's CE version is Lithium or greater
                            blnSkipComparison = true;
                        }

                        if (true == blnSkipComparison)
                        {
                            bItemsMatch = true;
                        }
                        else
                        {
                            bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);
                        }

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl58PowerLevel:
                    {
                        bool blnSkipComparison = false;

                        // Get the Meter value
                        if (objMeterValue != null)
                        {
                            // Trim spaces and null characters so that they will display and validate correctly
                            strDisplayMeterValue = objMeterValue.ToString().Trim(new char[] { ' ', '\0' });
                        }

                        // Get the Program value
                        if (objProgramValue != null)
                        {
                            // Trim spaces and null characters so that they will display and validate correctly
                            strDisplayProgramValue = objProgramValue.ToString().Trim(new char[] { ' ', '\0' });
                        }
                        else if (true == IsConfigVersionEqualOrGreaterThan(CE_VERSION_LITHIUM_3_9, programTables))
                        {
                            //Only skipping comparison if program value is null and program's CE version is Lithium or greater
                            blnSkipComparison = true;
                        }

                        if (true == blnSkipComparison)
                        {
                            bItemsMatch = true;
                        }
                        else
                        {
                            bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);
                        }

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
                case (long)CentronTblEnum.MfgTbl321EnablePowerMonitor:
                    {
                        bool bProgramValue = false;
                        bool bMeterValue = false;

                        // We need to check the default
                        if (objMeterValue != null)
                        {
                            // Use what the meter has
                            bMeterValue = Convert.ToBoolean(objMeterValue, CultureInfo.InvariantCulture);

                            strDisplayMeterValue = bMeterValue.ToString(CultureInfo.CurrentCulture);
                        }

                        if (objProgramValue != null)
                        {
                            // Use what the program has
                            bProgramValue = Convert.ToBoolean(objProgramValue, CultureInfo.InvariantCulture);

                            strDisplayProgramValue = bProgramValue.ToString(CultureInfo.CurrentCulture);
                        }

                        // Compare the strings in case one of them is null.
                        bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl461ERTRadio:
                    {
                        bool bProgramValue = false;
                        bool bMeterValue = false;

                        // We need to check the default
                        if (objMeterValue != null)
                        {
                            // Use what the meter has
                            bMeterValue = Convert.ToBoolean(objMeterValue, CultureInfo.InvariantCulture);

                            //Field displayed is ERT Radio Disabled so we have to reverse value we retrieve 
                            //since it tells us the opposite (ERT Radio is Enabled).
                            strDisplayMeterValue = (!bMeterValue).ToString(CultureInfo.CurrentCulture);
                        }

                        if (objProgramValue != null)
                        {
                            // Use what the program has
                            bProgramValue = Convert.ToBoolean(objProgramValue, CultureInfo.InvariantCulture);

                            //Field displayed is ERT Radio Disabled so we have to reverse value we retrieve 
                            //since it tells us the opposite (ERT Radio is Enabled).
                            strDisplayProgramValue = (!bProgramValue).ToString(CultureInfo.CurrentCulture);
                        }

                        // Compare the strings in case one of them is null.
                        bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);

                        break;
                    }
                case (long)CentronTblEnum.MFGTBL106_ENABLE_VM:
                    {
                        if (false == IsConfigVersionEqualOrGreaterThan(CE_VERSION_LITHIUM_3_9, programTables))
                        {
                            //Ignoring comparison if program's CE version is pre-Lithium
                            bItemsMatch = true;
                        }
                        else
                        {
                            bool bProgramValue = false;
                            bool bMeterValue = false;

                            // We need to check the default
                            if (objMeterValue != null && (byte)objMeterValue != 0xFF)
                            {
                                // Use what the meter has
                                bMeterValue = Convert.ToBoolean(objMeterValue, CultureInfo.InvariantCulture);

                                strDisplayMeterValue = bMeterValue.ToString(CultureInfo.CurrentCulture);
                            }

                            if (objProgramValue != null && (byte)objProgramValue != 0xFF)
                            {
                                // Use what the program has
                                bProgramValue = Convert.ToBoolean(objProgramValue, CultureInfo.InvariantCulture);

                                strDisplayProgramValue = bProgramValue.ToString(CultureInfo.CurrentCulture);
                            }

                            // Compare the strings in case one of them is null.
                            bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);
                        }


                        break;
                    }
                case (long)CentronTblEnum.MFGTBL105_VM_INT_LEN:
                case (long)CentronTblEnum.MFGTBL106_VH_LOW_THRESHOLD_PERCENT:
                case (long)CentronTblEnum.MFGTBL106_VH_HIGH_THRESHOLD_PERCENT:
                case (long)CentronTblEnum.MFGTBL106_RMS_VOLT_LOW_THRESHOLD_PERCENT:
                case (long)CentronTblEnum.MFGTBL106_RMS_VOLT_HIGH_THRESHOLD_PERCENT:
                case (long)CentronTblEnum.MFGTBL105_VRMS_ALARM_MIN_SEC:
                    {
                        if (false == IsConfigVersionEqualOrGreaterThan(CE_VERSION_LITHIUM_3_9, programTables))
                        {
                            //Ignoring comparison if program's CE version is pre-Lithium
                            bItemsMatch = true;
                        }
                        else
                        {
                            bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                        }

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl464NtpAddress:
                    {
                        DestinationAddressRecord NTPAddress = null;
                        byte[] abyAddress = null;

                        if (null != objMeterValue)
                        {
                            abyAddress = (byte[])objMeterValue;
                            NTPAddress = new DestinationAddressRecord(DestinationAddressRecord.AddressType.IP, abyAddress);
                            strDisplayMeterValue = NTPAddress.DisplayAddress;
                        }

                        if (null != objProgramValue)
                        {
                            abyAddress = (byte[])objProgramValue;
                            NTPAddress = new DestinationAddressRecord(DestinationAddressRecord.AddressType.IP, abyAddress);
                            strDisplayProgramValue = NTPAddress.DisplayAddress;
                        }

                        // Compare the strings in case one of them is null.
                        bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl469CellularDataTimeoutUnits:
                    {
                        if (null != objMeterValue)
                        {
                            strDisplayMeterValue = DecodeCellularDataTimeoutUnits((byte)objMeterValue);
                        }

                        if (null != objProgramValue)
                        {
                            strDisplayProgramValue = DecodeCellularDataTimeoutUnits((byte)objProgramValue);
                        }

                        // Compare the strings in case one of them is null.
                        bItemsMatch = strDisplayMeterValue.Equals(strDisplayProgramValue);

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl377TemperatureControlBitField:
                    {
                        bItemsMatch = true;
                        break;
                    }
                case (long)CentronTblEnum.MfgTbl377TemperatureControlEnableTemperatureMonitoring:
                case (long)CentronTblEnum.MfgTbl377TemperatureControlHighTemperatureThreshold1:
                case (long)CentronTblEnum.MfgTbl377TemperatureControlHighTemperatureThreshold2:
                case (long)CentronTblEnum.MfgTbl377TemperatureControlHysteresis:
                case (long)CentronTblEnum.MfgTbl377TemperatureControlRandomizationPeriodSeconds:
                case (long)CentronTblEnum.MfgTbl377TemperatureControlDailyCaptureTime1:
                    {

                        if (null != objProgramValue)
                        {
                            bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                        }
                        else
                        {
                            object objValue;
                            meterTables.GetValue(CentronTblEnum.MfgTbl377TemperatureControlEnableTemperatureMonitoring, null, out objValue);
                            if (objValue != null && String.Compare(objValue.ToString(), "true", true) == 0)
                            {

                                bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                            }
                            else
                            {
                                bItemsMatch = true;
                            }
                        }

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl377TemperatureControlDailyCaptureTime2:
                    {

                        if (null != objProgramValue)
                        {
                            bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                        }
                        else
                        {
                            object objValue;
                            meterTables.GetValue(CentronTblEnum.MfgTbl377TemperatureControlEnableTemperatureMonitoring, null, out objValue);
                            if (objValue != null && String.Compare(objValue.ToString(), "true", true) == 0)
                            {
                                meterTables.GetValue(CentronTblEnum.MfgTbl377TemperatureControlDailyCaptureTime4, null, out objValue);
                                if (objValue != null)
                                {
                                    if (String.Compare(objValue.ToString(), "65535") != 0)
                                    {
                                        bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                                    }
                                    else
                                    {
                                        bItemsMatch = true;
                                    }
                                }
                                else
                                {
                                    bItemsMatch = true;
                                }
                            }
                            else
                            {
                                bItemsMatch = true;
                            }
                        }

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl377TemperatureControlDailyCaptureTime3:
                    {

                        if (null != objProgramValue)
                        {
                            bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                        }
                        else
                        {
                            object objValue;
                            meterTables.GetValue(CentronTblEnum.MfgTbl377TemperatureControlEnableTemperatureMonitoring, null, out objValue);
                            if (objValue != null && String.Compare(objValue.ToString(), "true", true) == 0)
                            {
                                meterTables.GetValue(CentronTblEnum.MfgTbl377TemperatureControlDailyCaptureTime4, null, out objValue);
                                if (objValue != null)
                                {
                                    if (String.Compare(objValue.ToString(), "65535") != 0)
                                    {
                                        bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                                    }
                                    else
                                    {
                                        bItemsMatch = true;
                                    }
                                }
                                else
                                {
                                    bItemsMatch = true;
                                }
                            }
                            else
                            {
                                bItemsMatch = true;
                            }
                        }

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl377TemperatureControlDailyCaptureTime4:
                    {

                        if (null != objProgramValue)
                        {
                            bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                        }
                        else
                        {
                            object objValue;
                            meterTables.GetValue(CentronTblEnum.MfgTbl377TemperatureControlEnableTemperatureMonitoring, null, out objValue);
                            if (objValue != null && String.Compare(objValue.ToString(), "true", true) == 0)
                            {
                                meterTables.GetValue(CentronTblEnum.MfgTbl377TemperatureControlDailyCaptureTime4, null, out objValue);
                                if (objValue != null)
                                {
                                    if (String.Compare(objValue.ToString(), "65535") != 0)
                                    {
                                        bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                                    }
                                    else
                                    {
                                        bItemsMatch = true;
                                    }
                                }
                                else
                                {
                                    bItemsMatch = true;
                                }
                            }
                            else
                            {
                                bItemsMatch = true;
                            }
                        }

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl212PowerUpThreshold:
                    {
                        if (null != objProgramValue)
                        {
                            bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                        }
                        else
                        {
                            object objValue;
                            meterTables.GetValue(CentronTblEnum.MfgTbl212PowerUpThreshold, null, out objValue);
                            if (objValue != null && String.Compare(objValue.ToString(), "0") != 0 && String.Compare(objValue.ToString(), "65535") != 0)
                            {
                                bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                            }
                            else
                            {
                                bItemsMatch = true;
                            }
                        }

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl217CurrentThresholdExceededEnable:
                case (long)CentronTblEnum.MfgTbl217CurrentThresholdExceededThreshold:
                case (long)CentronTblEnum.MfgTbl217CurrentThresholdExceededHysteresis:
                case (long)CentronTblEnum.MfgTbl217CurrentThresholdExceededDebounce:
                case (long)CentronTblEnum.MfgTbl217CurrentThresholdExceededActiveLockoutDuration:
                    {

                        if (null != objProgramValue)
                        {
                            bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                        }
                        else
                        {
                            object objValue;
                            meterTables.GetValue(CentronTblEnum.MfgTbl217CurrentThresholdExceededEnable, null, out objValue);
                            if (objValue != null && String.Compare(objValue.ToString(), "true", true) == 0)
                            {

                                bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                            }
                            else
                            {
                                bItemsMatch = true;
                            }
                        }

                        break;
                    }
                case (long)CentronTblEnum.MFGTBL143_MIN_DELTA_SECONDS:
                    {
                        if (null != objProgramValue)
                        {
                            bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                        }
                        else
                        {
                            object objValue;
                            meterTables.GetValue(CentronTblEnum.MFGTBL143_MIN_DELTA_SECONDS, null, out objValue);
                            if (objValue != null && String.Compare(objValue.ToString(), "20") != 0)
                            {

                                bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                            }
                            else
                            {
                                bItemsMatch = true;
                            }
                        }


                        break;
                    }
                case (long)CentronTblEnum.MFGTBL143_MAX_DELTA_SECONDS:
                    {
                        if (null != objProgramValue)
                        {
                            bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                        }
                        else
                        {
                            object objValue;
                            meterTables.GetValue(CentronTblEnum.MFGTBL143_MAX_DELTA_SECONDS, null, out objValue);
                            if (objValue != null && String.Compare(objValue.ToString(), "120") != 0)
                            {

                                bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);
                            }
                            else
                            {
                                bItemsMatch = true;
                            }
                        }

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl2044VAVARSelection:
                    {
                        //Get Meter Value
                        if (SecondaryQuantity == BaseEnergies.VarhArithmetic
                            || SecondaryQuantity == BaseEnergies.VarhVectorial)
                        {
                            strDisplayMeterValue = VAR_SSQ_NAME;
                        }
                        else if (SecondaryQuantity == BaseEnergies.VAhArithmetic
                            || SecondaryQuantity == BaseEnergies.VAhVectorial)
                        {
                            strDisplayMeterValue = VA_SSQ_NAME;
                        }

                        if (objProgramValue != null)
                        {
                            // Trim spaces and null characters so that they will display and validate correctly
                            strDisplayProgramValue = objProgramValue.ToString().Trim(new char[] { ' ', '\0' });

                            //Show a name instead of a number so when a discrepancy occurs it is obvious 
                            //what is what.
                            if (strDisplayProgramValue == VAR_SSQ_VALUE)
                            {
                                strDisplayProgramValue = VAR_SSQ_NAME;
                            }
                            else if (strDisplayProgramValue == VA_SSQ_VALUE)
                            {
                                strDisplayProgramValue = VA_SSQ_NAME;
                            }
                        }

                        // Compare the values
                        if (null == objProgramValue/*Some older configs don't have it*/
                            || strDisplayMeterValue == strDisplayProgramValue)
                        {
                            bItemsMatch = true;
                        }

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl2044UserInterfaceVoltAmperesSelection:
                    {
                        //Get Meter Value
                        if (SecondaryQuantity == BaseEnergies.VAhArithmetic)
                        {
                            strDisplayMeterValue = VA_ARITH_NAME;
                        }
                        else if (SecondaryQuantity == BaseEnergies.VAhVectorial)
                        {
                            strDisplayMeterValue = VA_VECT_NAME;
                        }


                        if (objProgramValue != null)
                        {
                            // Trim spaces and null characters so that they will display and validate correctly
                            strDisplayProgramValue = objProgramValue.ToString().Trim(new char[] { ' ', '\0' });

                            //Show a name instead of a number so when a discrepancy occurs it is obvious 
                            //what is what.
                            if (strDisplayProgramValue == VA_ARITH_VALUE)
                            {
                                strDisplayProgramValue = VA_ARITH_NAME;
                            }
                            else if (strDisplayProgramValue == VA_VECT_VALUE)
                            {
                                strDisplayProgramValue = VA_VECT_NAME;
                            }
                        }

                        // Compare the values
                        if (null == objProgramValue/*Some older configs don't have it*/
                            || strDisplayMeterValue == strDisplayProgramValue)
                        {
                            bItemsMatch = true;
                        }

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids:
                {
                        uint MeterValue = 0;
                        uint ProgramValue = 0;

                        if (objMeterValue != null)
                        {
                            MeterValue = (uint)objMeterValue;

                            // If the value is 0xFF then that means that it has likely never been configured
                            if (MeterValue == 0xFFFFFFFF)
                            {
                                MeterValue = 0;
                            }
                        }

                        if (objProgramValue != null)
                        {
                            ProgramValue = (uint)objProgramValue;

                            // If the value is 0xFF then that means that it has likely never been configured
                            if (ProgramValue == 0xFFFFFFFF)
                            {
                                ProgramValue = 0;
                            }
                        }

                        if (MeterValue == ProgramValue)
                        {
                            bItemsMatch = true;
                        }
                        else
                        {
                            // Translate the values so that the user will understand the difference
                            if (MeterValue == 0)
                            {
                                strDisplayMeterValue = "None";
                            }
                            else
                            {
                                LID LidValue = CreateLID(MeterValue);
                                strDisplayMeterValue = LidValue.lidDescription;
                            }

                            if (ProgramValue == 0)
                            {
                                strDisplayProgramValue = "None";
                            }
                            else
                            {
                                LID LidValue = CreateLID(ProgramValue);
                                strDisplayProgramValue = LidValue.lidDescription;
                            }
                        }

                        break;
                    }
                case (long)CentronTblEnum.MfgTbl217DataPushSetVoltageMonitoringIntervalData:
                {
                    {
                        bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);

                        // If the voltage monitoring interval data is not in the program and is false in the meter, we don't have a mismatch
                        if (string.IsNullOrEmpty(strDisplayProgramValue))
                        {
                            if (string.Compare(strDisplayMeterValue, "False", true) == 0)
                            {
                                bItemsMatch = true;
                            }
                            else
                            {
                                // If Push Data has never been configured, the value from the table will be 0xFF, which
                                // the CE dll interprets as true. We need a further check
                                object objMeterValue2;

                                if (meterTables.IsCached((long)CentronTblEnum.MfgTbl217DataPushSetId, new int[] { 0 }))
                                {
                                    meterTables.GetValue((long)CentronTblEnum.MfgTbl217DataPushSetId, new int[] { 0 }, out objMeterValue2);

                                    if (objMeterValue2 != null)
                                    {
                                        string strDisplayMeterValue2 = objMeterValue2.ToString().Trim(new char[] { ' ', '\0' });
                                        // Remove any non-ASCII characters from the meter value (if never configured, the value will be all 0xFFs
                                        strDisplayMeterValue2 = Regex.Replace(strDisplayMeterValue2, @"[^\u0000-\u007F]+", string.Empty);

                                        if (string.IsNullOrEmpty(strDisplayMeterValue2))
                                        {
                                            bItemsMatch = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    break;
                }
                case (long)CentronTblEnum.MfgTbl217DataPushSetId:
                {
                    bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);

                    // Remove any non-ASCII characters from the meter value (if never configured, the value will be all 0xFFs
                    strDisplayMeterValue = Regex.Replace(strDisplayMeterValue, @"[^\u0000-\u007F]+", string.Empty);
                    if ((string.IsNullOrEmpty(strDisplayProgramValue)) && (string.IsNullOrEmpty(strDisplayMeterValue)))
                    {
                        bItemsMatch = true;
                    }
                    break;
                }
                default:
                    {
                        bItemsMatch = PerformDefaultValidation(objMeterValue, objProgramValue, ref strDisplayMeterValue, ref strDisplayProgramValue);

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
        /// This method performs a default validation of the meter and program vales for the validation item.
        /// </summary>
        /// <param name="objMeterValue">The meter value of the validation item.</param>
        /// <param name="objProgramValue">The program value of the validation item.</param>
        /// <param name="strDisplayMeterValue">The string version of the meter value.</param>
        /// <param name="strDisplayProgramValue">The string version of the program value.</param>
        /// <returns>Whether the given program and meter values match.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/10/12 jrf 2.53.54 195674 Refactored this code so it could be called multiple times.
        //
        protected bool PerformDefaultValidation(object objMeterValue, object objProgramValue, ref string strDisplayMeterValue, ref string strDisplayProgramValue)
        {
            bool bItemsMatch = false;

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

            return bItemsMatch;
        }

        /// <summary>
        /// This method decodes the give byte value for the cellular data timeout unit
        /// to a string.
        /// </summary>
        /// <param name="byUnit">cellular data timeout unit</param>
        /// <returns>Decoded value.</returns>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 08/09/13 jrf 2.85.14 TQ 7657   Created
        //
        protected string DecodeCellularDataTimeoutUnits(byte byUnit)
        {
            string strReturn = "";

            switch (byUnit)
            {
                case (byte)ICMMfgTable2517CellularConfiguration.TimeoutUnits.Hours:
                    {
                        strReturn = m_rmStrings.GetString("HOURS");
                        break;
                    }
                case (byte)ICMMfgTable2517CellularConfiguration.TimeoutUnits.Minutes:
                    {
                        strReturn = m_rmStrings.GetString("MINUTES");
                        break;
                    }
                default:
                    {
                        strReturn = m_rmStrings.GetString("UNKNOWN");
                        break;
                    }
            }

            return strReturn;
        }

        /// <summary>
        /// This method determines if the current configuration's CE version is equal to or greater than 
        /// a given target version.
        /// </summary>
        /// <param name="fltTargetCEVersion">The target CE version.</param>
        /// <param name="programTables">The program tables for the current configuration.</param>
        /// <returns>Whether the config's version is >= to the target.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/10/12 jrf 2.53.54 195674 Refactored this code so it could be called multiple times.
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Convert.ToSingle(System.String)")]
        protected bool IsConfigVersionEqualOrGreaterThan(float fltTargetCEVersion, CentronTables programTables)
        {
            bool blnEqualOrGreater = false;

            if (programTables.IsCached((long)CentronTblEnum.MFGTBL2045_CE_VERSION_NUMBER, null))
            {
                object objValue = null;
                programTables.GetValue(CentronTblEnum.MFGTBL2045_CE_VERSION_NUMBER, null, out objValue);

                string strValue = objValue as string;
                string[] astrCEVersion = strValue.Split(new char[] { ' ', '.', '-' });
                float fltCurrentCEVersion = Convert.ToSingle(astrCEVersion[0] + "." + astrCEVersion[1]);

                if (0 <= VersionChecker.CompareTo(fltCurrentCEVersion, fltTargetCEVersion))
                {
                    blnEqualOrGreater = true;
                }
            }

            return blnEqualOrGreater;
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
        // 06/17/11 jrf 2.51.13 175979 Adding validation of power monitoring enabled.
        // 08/09/13 jrf 2.85.14 TQ7657 Adding tables for new Michigan validation items.
        // 08/14/13 jrf 2.85.16 TQ7657 Added new method to determine if ICS ERT data 
        //                             is populated.
        // 02/08/16 PGH 4.50.226 577471, RTT556309  Added tables for Push Data and Temperature Data
        // 05/17/16 PGH 4.50.269 685893 Removed temperature data tables unrelated to configuring the meter.
        //
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
            TableList.Add(2153);
            TableList.Add(2154);
            TableList.Add(2159);
            TableList.Add(2161);
            TableList.Add(2163);
            TableList.Add(2185);
            TableList.Add(2190);
            TableList.Add(2191);
            TableList.Add(2193);
            TableList.Add(2206);
            TableList.Add(2425);
            TableList.Add(2260);
            TableList.Add(2265);
            TableList.Add(2369);

            if (true == IsICSERTDataPopulated())
            {
                // only add the ERT configuration table if it is populated.
                TableList.Add(2509);
            }

            TableList.Add(2512);
            TableList.Add(2517);

            return TableList;
        }

        /// <summary>
        /// This method determines if ICS ERT data is populated in the ICS comm module.
        /// </summary>
        /// <returns>True/False indicating whether ICS ERT data is populated.</returns>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 08/09/13 jrf 2.85.14 TQ 7657   Created.
        //
        protected bool IsICSERTDataPopulated()
        {
            bool blnPopulated = false;

            ICSCommModule ICSModule = CommModule as ICSCommModule;

            if (null != ICSModule && null != ICSModule.IsERTPopulated && true == ICSModule.IsERTPopulated)
            {
                blnPopulated = true;
            }

            return blnPopulated;
        }

        /// <summary>
        /// Creates the list of validation items.
        /// </summary>
        /// <returns>A list of items that will be validated.</returns>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		      Created
        // 05/20/09 AF  2.20.05           Corrected a typo in a string
        // 06/17/11 jrf 2.51.13    175979 Adding validation of power monitoring enabled.
        // 08/08/11 jrf 2.52.00    177455 Adding validation of ZigBee power level.
        // 09/16/11 jrf 2.52.17    180287 Adding a minimum firmware version for enable asset 
        //                                synchronization, enable fatal error recovery
        //                                and enable power monitoring validation items.
        // 09/19/11 jrf 2.52.18    180287 Correcting the minimum firmware version for enable 
        //                                fatal error recovery.
        // 02/23/12 jrf 2.53.43    194414 When HAN LOG CTRL table is not present in the meter,
        //                                validation of HAN events configured there in the 
        //                                program is being removed.
        // 03/27/12 AF  2.53.52    196102 Added the HAN 2 events to the list
        // 02/15/13 jrf 2.70.69    243289 Adding appropriate code to exclude items added in Lithium
        //                                (extended voltage monitoring, instrumentation profile) when 
        //                                they are not appropriate.  Removed extended energies from validation
        //                                since only appropriate to ITRE & ITRF.
        // 02/20/13 AF  2.70.69    322427 Set a max f/w version for InterPAN Mode which became obsolete in Lithium
        // 08/09/13 jrf 2.85.14 TQ 7657   Adding validation of items new to Michigan.
        // 12/09/13 jrf 3.50.16 TQ 9560   Refactored retrieval of items into unique methods.
        // 02/08/16 PGH 4.50.226 577471, RTT556309  Added methods for Push Data and Temperature Data
        // 08/02/16 AF  4.60.02 WR 704376 Separated the event list into its own method so that the ICM events can be concatenated
        //
        protected virtual List<EDLValidationItem> GetValidationList()
        {
            List<EDLValidationItem> ValidationList = new List<EDLValidationItem>();

            // TOU/Time configuration
            GetTOUTimeValidationItems(ValidationList);

            // Security
            GetSecurityValidationItems(ValidationList);

            // Quantities
            GetQuantityValidationItems(ValidationList);

            // Register Operations
            GetRegisterOperationValidationItems(ValidationList);

            // Device Multipliers
            GetDeviceMultiplierValidationItems(ValidationList);

            // Load Profile
            GetLoadProfileValidationItems(ValidationList);

            // Instrumentation Profile
            GetInstrumentaionProfileValidationItems(ValidationList);

            // Voltage Monitor
            GetVoltageMonitoringValidationItems(ValidationList);

            // Enhanced Voltage Monitoring
            GetEnhancedVoltageMonitoringValidationItems(ValidationList);

            // User Data
            GetUserDataValidationList(ValidationList);

            // Display
            GetDisplayValidationItems(ValidationList);

            // Events
            GetEventValidationItems(ValidationList);
            GetEventExceptionValidationItems(ValidationList);

            // Service Limiting
            GetServiceLimitingValidationItems(ValidationList);

            // Communications
            GetCommunicationsValidationItems(ValidationList);

            // Push Set
            GetPushSetValidationItems(ValidationList);

            // Push Group
            GetPushGroupValidationItems(ValidationList);

            // Temperature
            GetTemperatureValidationItems(ValidationList);

            return ValidationList;
        }

        /// <summary>
        /// Adds TOU/Time validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetTOUTimeValidationItems(List<EDLValidationItem> ValidationList)
        {

            ValidationList.Add(new EDLValidationItem((long)StdTableEnum.STDTBL6_TARIFF_ID,
                                        null,
                                        "TOU Schedule ID",
                                        "TOU/Time"));
            ValidationList.Add(new EDLValidationItem((long)StdTableEnum.STDTBL53_TIME_ZONE_OFFSET,
                                        null,
                                        "Time Zone",
                                        "TOU/Time"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL42_DST_HOUR,
                                        null,
                                        "DST Switch Time Hour",
                                        "TOU/Time"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL42_DST_MINUTE,
                                        null,
                                        "DST Switch Time Minute",
                                        "TOU/Time"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL42_DST_OFFSET,
                                        null,
                                        "DST Switch Length",
                                        "TOU/Time"));
        }

        /// <summary>
        /// Adds security validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetSecurityValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL145_REQUIRE_ENHANCED_SECURITY,
                                        null,
                                        "Require Enhanced Security",
                                        "Security"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL145_EXCEPTION_SECURITY_MODEL,
                                        null,
                                        "Exception Security Model",
                                        "Security"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl58SecurityMode,
                                        null,
                                        "HAN Security Profile",
                                        "Security",
                                        VERSION_2_SP5_1));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl58InterPanMode,
                                        null,
                                        "InterPAN Mode",
                                        "Security",
                                        VERSION_2_SP5_1,
                                        VERSION_3_12_LITHIUM));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl145RequireSignedAuthorization, /*MfgTbl145RequireSignedAuthentication*/
                                        null,
                                        "Require Signed Authentication",
                                        "Security",
                                        VERSION_2_SP5_1));
        }

        /// <summary>
        /// Adds quantity validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version  ID Number Description
        // -------- --- -------  -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16  TQ 9560   Created.
        // 06/09/16 jrf 4.50.281 WR 633121 Addding validation of base quantities
        // 06/23/16 jrf 4.50.290 WR 696508 Exclude non-metering devices from base quantity validation
        protected virtual void GetQuantityValidationItems(List<EDLValidationItem> ValidationList)
        {
            if (true == IsSinglePhaseMeter)
            {
                //This makes sure this method is only called for a singlephase metering device. 
                //It will exclude pole top cell relay and transparent devices (range extenders) that 
                //are not used for metering.  
                GetBaseQuantityValidationItems(ValidationList);
            }

            GetEnergyQuantityValidationItems(ValidationList);
            GetDemandQuantityValidationItems(ValidationList);
        }

        /// <summary>
        /// Adds energy quantity validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version  ID Number Description
        // -------- --- -------  -- ------ ---------------------------------------
        // 06/09/16 jrf 4.50.280 WR 633121 Created.
        // 11/02/16 jrf 4.60.12  WR 658430 Setting miniumum firmware version for 
        //                                 validation of base quantites to versions
        //                                 greater than Bridge Phase I (3.32)
        protected virtual void GetBaseQuantityValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl2044VAVARSelection,
                                        null,
                                        "VA/VAR Selection",
                                        "Base Quantities",
                                        VERSION_GREATER_THAN_BRIDGE_PHASE1_4_00));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl2044UserInterfaceVoltAmperesSelection,
                                        null,
                                        "VA Calculation",
                                        "Base Quantities",
                                        VERSION_GREATER_THAN_BRIDGE_PHASE1_4_00));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_ENERGY_LID,
                                        new int[] { 0 },
                                        "Energy 1 Base Quantity",
                                        "Base Quantities",
                                        VERSION_GREATER_THAN_BRIDGE_PHASE1_4_00));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_ENERGY_LID,
                                        new int[] { 1 },
                                        "Energy 2 Base Quantity",
                                        "Base Quantities",
                                        VERSION_GREATER_THAN_BRIDGE_PHASE1_4_00));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_ENERGY_LID,
                                        new int[] { 2 },
                                        "Energy 3 Base Quantity",
                                        "Base Quantities",
                                        VERSION_GREATER_THAN_BRIDGE_PHASE1_4_00));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_ENERGY_LID,
                                        new int[] { 3 },
                                        "Energy 4 Base Quantity",
                                        "Base Quantities",
                                        VERSION_GREATER_THAN_BRIDGE_PHASE1_4_00));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_DEMAND_DEFINITION,
                                        new int[] { 0 },
                                        "Demand Base Quantity",
                                        "Base Quantities",
                                        VERSION_GREATER_THAN_BRIDGE_PHASE1_4_00));
        }

        /// <summary>
        /// Adds energy quantity validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetEnergyQuantityValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_ENERGY_LID,
                                        new int[] { 0 },
                                        "Energy 1 Quantity",
                                        "Quantities"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_ENERGY_LID,
                                        new int[] { 1 },
                                        "Energy 2 Quantity",
                                        "Quantities"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_ENERGY_LID,
                                        new int[] { 2 },
                                        "Energy 3 Quantity",
                                        "Quantities"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_ENERGY_LID,
                                        new int[] { 3 },
                                        "Energy 4 Quantity",
                                        "Quantities"));
        }

        /// <summary>
        /// Adds demand quantity validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetDemandQuantityValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_DEMAND_DEFINITION,
                                        new int[] { 0 },
                                        "Demand Quantity",
                                        "Quantities"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_DEMAND_THRESHOLDS,
                                        new int[] { 0 },
                                        "Load Control Threshold",
                                        "Quantities"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_DEMAND_CONTROL,
                                        null,
                                        "Reconnect Method",
                                        "Quantities"));
        }

        /// <summary>
        /// Adds register operation validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        // 01/27/16 AF  4.50.224 RTT 586620 Added Power Up Threshold
        //
        protected virtual void GetRegisterOperationValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_DEMAND_INTERVAL_LENGTH,
                                        null,
                                        "Demand Interval Length (minutes)",
                                        "Register Operations"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_NUM_SUB_INTERVALS,
                                        null,
                                        "Number of Subintervals",
                                        "Register Operations"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_COLD_LOAD_PICKUP,
                                        null,
                                        "Cold Load Pickup Time (minutes)",
                                        "Register Operations"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_OUTAGE_LENGTH_BEFORE_CLPU,
                                        null,
                                        "Power Outage Recognition Time (seconds)",
                                        "Register Operations"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_TEST_MODE_INTERVAL_LENGTH,
                                        null,
                                        "Test Mode Demand Interval Length (minutes)",
                                        "Register Operations"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_NUM_TEST_MODE_SUBINTERVALS,
                                        null,
                                        "Number of Test Mode Subintervals",
                                        "Register Operations"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_CLOCK_SYNC,
                                        null,
                                        "Clock Synchronization",
                                        "Register Operations"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_DAILY_SELF_READ_TIME,
                                        null,
                                        "Daily Self Read Time",
                                        "Register Operations"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl212FatalErrorRecoveryEnabled,
                                        null,
                                        "Enable Fatal Error Recovery",
                                        "Register Operations",
                                        VERSION_3));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl212AssetSynchronizationEnabled,
                                        null,
                                        "Enable Asset Synchronization",
                                        "Register Operations",
                                        VERSION_3));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl321EnablePowerMonitor,
                                       null,
                                       "Enable Power Monitoring",
                                       "Register Operations",
                                       VERSION_HYDROGEN_3_8));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL143_MIN_DELTA_SECONDS,
                                       null,
                                       "Comm. Card Time Drift Adjustment Threshold",
                                       "Register Operations",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL143_MAX_DELTA_SECONDS,
                                       null,
                                       "Comm. Card Time Drift Error Threshold",
                                       "Register Operations",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_DR_LOCKOUT_TIME,
                                       null,
                                       "Demand Reset Lockout Time",
                                       "Register Operations",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217CurrentThresholdExceededEnable,
                                       null,
                                       "Enable Current Threshold Exceeded",
                                       "Register Operations",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217CurrentThresholdExceededThreshold,
                                       null,
                                       "Current Threshold Exceeded - Threshold",
                                       "Register Operations",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217CurrentThresholdExceededHysteresis,
                                       null,
                                       "Current Threshold Exceeded - Hysteresis",
                                       "Register Operations",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217CurrentThresholdExceededDebounce,
                                       null,
                                       "Current Threshold Exceeded - Debounce",
                                       "Register Operations",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217CurrentThresholdExceededActiveLockoutDuration,
                                       null,
                                       "Current Threshold Exceeded - Active Lockout Duration",
                                       "Register Operations",
                                       VERSION_MICHIGAN));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl212PowerUpThreshold,
                                        null,
                                        "Power Up Threshold (seconds)",
                                        "Register Operations",
                                        VERSION_BERYLLIUM));

        }

        /// <summary>
        /// Adds device multiplier validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetDeviceMultiplierValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_CT_MULTIPLIER,
                                        null,
                                        "CT Multiplier",
                                        "Device Multipliers"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_VT_MULTIPLIER,
                                        null,
                                        "VT Multiplier",
                                        "Device Multipliers"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_REGISTER_MULTIPLIER,
                                        null,
                                        "Register Multiplier",
                                        "Device Multipliers"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_REGISTER_FULL_SCALE,
                                        null,
                                        "Register Fullscale",
                                        "Device Multipliers"));
        }

        /// <summary>
        /// Adds load profile validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetLoadProfileValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_LID,
                                        new int[] { 0 },
                                        "Quantity 1",
                                        "Load Profile"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT,
                                        new int[] { 0 },
                                        "Pulse Weight 1",
                                        "Load Profile"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_LID,
                                        new int[] { 1 },
                                        "Quantity 2",
                                        "Load Profile"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT,
                                        new int[] { 1 },
                                        "Pulse Weight 2",
                                        "Load Profile"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_LID,
                                        new int[] { 2 },
                                        "Quantity 3",
                                        "Load Profile"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT,
                                        new int[] { 2 },
                                        "Pulse Weight 3",
                                        "Load Profile"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_LID,
                                        new int[] { 3 },
                                        "Quantity 4",
                                        "Load Profile"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_PULSE_WEIGHT,
                                        new int[] { 3 },
                                        "Pulse Weight 4",
                                        "Load Profile"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_INTERVAL_LENGTH,
                                        null,
                                        "Interval Length",
                                        "Load Profile"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LP_MIN_POWER_OUTAGE,
                                        null,
                                        "Outage Length",
                                        "Load Profile"));
        }

        /// <summary>
        /// Adds instrumentation profile validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetInstrumentaionProfileValidationItems(List<EDLValidationItem> ValidationList)
        {
            if (InstrumentationProfileSupported)
            {
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfileChannelLogicalIdentifier,
                                            new int[] { 0 },
                                            "Quantity 1",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfilePulseWeight,
                                            new int[] { 0 },
                                            "Pulse Weight 1",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfileChannelLogicalIdentifier,
                                            new int[] { 1 },
                                            "Quantity 2",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfilePulseWeight,
                                            new int[] { 1 },
                                            "Pulse Weight 2",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfileChannelLogicalIdentifier,
                                            new int[] { 2 },
                                            "Quantity 3",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfilePulseWeight,
                                            new int[] { 2 },
                                            "Pulse Weight 3",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfileChannelLogicalIdentifier,
                                            new int[] { 3 },
                                            "Quantity 4",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfilePulseWeight,
                                            new int[] { 3 },
                                            "Pulse Weight 4",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfileChannelLogicalIdentifier,
                                            new int[] { 4 },
                                            "Quantity 5",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfilePulseWeight,
                                            new int[] { 4 },
                                            "Pulse Weight 5",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfileChannelLogicalIdentifier,
                                            new int[] { 5 },
                                            "Quantity 6",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfilePulseWeight,
                                            new int[] { 5 },
                                            "Pulse Weight 6",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfileChannelLogicalIdentifier,
                                            new int[] { 6 },
                                            "Quantity 7",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfilePulseWeight,
                                            new int[] { 6 },
                                            "Pulse Weight 7",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfileChannelLogicalIdentifier,
                                            new int[] { 7 },
                                            "Quantity 8",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfilePulseWeight,
                                            new int[] { 7 },
                                            "Pulse Weight 8",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfileChannelLogicalIdentifier,
                                            new int[] { 8 },
                                            "Quantity 9",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217InstrumentationProfilePulseWeight,
                                            new int[] { 8 },
                                            "Pulse Weight 9",
                                            "Instrumentation Profile",
                                            VERSION_LITHIUM_3_12));
            }
        }

        /// <summary>
        /// Adds voltage monitoring validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetVoltageMonitoringValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL102_ENABLE_FLAG,
                                        null,
                                        "Enable Voltage Monitor",
                                        "Voltage Monitor"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL101_NBR_PHASES,
                                        null,
                                        "Phase Selection",
                                        "Voltage Monitor"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL101_VM_INT_LEN,
                                        null,
                                        "Interval Length",
                                        "Voltage Monitor"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL102_VH_LOW_THRESHOLD,
                                        null,
                                        "Volt Hour Low Threshold",
                                        "Voltage Monitor"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL102_VH_HIGH_THRESHOLD,
                                        null,
                                        "Volt Hour High Threshold",
                                        "Voltage Monitor"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL102_RMS_VOLT_LOW_THRESHOLD,
                                        null,
                                        "RMS Volt Low Threshold",
                                        "Voltage Monitor"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL102_RMS_VOLT_HIGH_THRESHOLD,
                                        null,
                                        "RMS Volt High Threshold",
                                        "Voltage Monitor"));
        }

        /// <summary>
        /// Adds enhanced voltage monitoring validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetEnhancedVoltageMonitoringValidationItems(List<EDLValidationItem> ValidationList)
        {
            if (ExtVoltageMonitoringSupported)
            {
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL106_ENABLE_VM,
                                            null,
                                            "Enable Voltage Monitor",
                                            "Enhanced Voltage Monitor",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL105_VM_INT_LEN,
                                            null,
                                            "Interval Length",
                                            "Enhanced Voltage Monitor",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL106_VH_LOW_THRESHOLD_PERCENT,
                                            null,
                                            "Volt Hour Low Threshold Percentage",
                                            "Enhanced Voltage Monitor",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL106_VH_HIGH_THRESHOLD_PERCENT,
                                            null,
                                            "Volt Hour High Threshold Percentage",
                                            "Enhanced Voltage Monitor",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL106_RMS_VOLT_LOW_THRESHOLD_PERCENT,
                                            null,
                                            "RMS Volt Low Threshold Percentage",
                                            "Enhanced Voltage Monitor",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL106_RMS_VOLT_HIGH_THRESHOLD_PERCENT,
                                            null,
                                            "RMS Volt High Threshold Percentage",
                                            "Enhanced Voltage Monitor",
                                            VERSION_LITHIUM_3_12));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL105_VRMS_ALARM_MIN_SEC,
                                            null,
                                            "Instantaneous Voltage High/Low Alarm Latency",
                                            "Enhanced Voltage Monitor",
                                            VERSION_LITHIUM_3_12));
            }
        }

        /// <summary>
        /// Adds enhanced voltage monitoring validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetExtendedSelfReadValidationItems(List<EDLValidationItem> ValidationList)
        {
            if (ExtSelfReadSupported)
            {
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217SelfReadTwoLogicalIdentifier,
                                            null,
                                            "Extended Self Read",
                                            "Extended Self Read",
                                                VERSION_LITHIUM_3_12));
            }
        }

        /// <summary>
        /// Adds user data validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetUserDataValidationList(List<EDLValidationItem> ValidationList)
        {
            for (int iIndex = 0; iIndex < 3; iIndex++)
            {
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_USER_DEFINED_FIELDS,
                                            new int[] { iIndex },
                                            "User Data #" + (iIndex + 1).ToString(CultureInfo.InvariantCulture),
                                            "User Data"));
            }
        }

        /// <summary>
        /// Adds display validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetDisplayValidationItems(List<EDLValidationItem> ValidationList)
        {
            // Add the Display Items

            // Display is a bit tricky in that it is possible that the meter and program may have different
            // numbers of Normal and Test items, which means that the display items may not be aligned correctly.
            // Therefore we can not handle the display in the same manner as the other items. We will just add 
            // the item for the display and add a check to make sure not to read that item since it will cause an exception
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_DISPLAY_ITEMS,
                                        null,
                                        "Display Items",
                                        "Display Items"));


            // Add the Display Options
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_ITEM_DISPLAY_TIME,
                                        null,
                                        "Display Scroll On Time (seconds)",
                                        "Display Options"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_DISPLAY_EOI,
                                        null,
                                        "Enable End of Interval (EOI) Indicator",
                                        "Display Options"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_WATT_LOAD_INDICATOR,
                                        null,
                                        "Enable Watt Load Indicator",
                                        "Display Options"));

            GetAdditionalDisplayOptionsValidationItems(ValidationList);

            //Add the Displayable Errors
            GetDisplayErrorsValidationItems(ValidationList);
        }

        /// <summary>
        /// Adds display options validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetAdditionalDisplayOptionsValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_DISPLAY_REMOTE_DISCONNECT_MESSAGE_FLAG,
                                        null,
                                        "Enable Remote Disconnect OFF message",
                                        "Display Options"));
        }

        /// <summary>
        /// Adds display errors validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetDisplayErrorsValidationItems(List<EDLValidationItem> ValidationList)
        {
            // Add the Displayable Errors. Since these values are stored in seperate bit fields
            // for Scroll and Lock we will only add the Lock items and then check the scroll values
            // to determine what is displayed
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LOCK_LOW_BATTERY,
                                        null,
                                        "Non-Fatal Error #1 - Low Battery",
                                        "Displayable Errors"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LOCK_LOSS_PHASE,
                                        null,
                                        "Non-Fatal Error #2 - Loss of Phase",
                                        "Displayable Errors"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LOCK_TOU_SCHEDULE_ERROR,
                                        null,
                                        "Non-Fatal Error #3 - Clock, TOU Error",
                                        "Displayable Errors"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LOCK_REVERSE_POWER_FLOW,
                                        null,
                                        "Non-Fatal Error #4 - Reverse Power Flow",
                                        "Displayable Errors"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LOCK_MASS_MEMORY,
                                        null,
                                        "Non-Fatal Error #5 - Load Profile Error",
                                        "Displayable Errors"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_LOCK_REGISTER_FULL_SCALE,
                                        null,
                                        "Non-Fatal Error #6 - Full Scale Overflow",
                                        "Displayable Errors"));
        }


        /// <summary>
        /// Adds register event validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  08/03/16 AF  4.60.02  WR 704376  Separated out from GetEventExceptionValidationItems so that
        //                                   ICM events will be contiguous
        //
        protected virtual void GetEventValidationItems(List<EDLValidationItem> ValidationList)
        {
            CENTRON_AMI_EventDictionary EventDictionary = new CENTRON_AMI_EventDictionary();

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

                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL0_HISTORY_LOG_EVENTS,
                                                new int[] { iIndex },
                                                strDescription,
                                                "Events"));
            }
        }

        /// <summary>
        /// Adds event/exception validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        // 08/03/16 AF  4.60.02 WR 704376 Split off the register events into a separate method so that ICM events can be concatenated.
        //
        protected virtual void GetEventExceptionValidationItems(List<EDLValidationItem> ValidationList)
        {
            CENTRON_AMI_CommEventDictionary CommEventDictionary = new CENTRON_AMI_CommEventDictionary();

            // Exceptions
            ValidationList.Add(new EDLValidationItem((long)StdTableEnum.STDTBL123_EXCEPTION_REPORT,
                                                      null,
                                                      "Exceptions",
                                                      "Exceptions"));

            // Comm Log - Standard LAN Events
            // TODO: Implement Stnadard LAN Events when they exist

            // Comm Log - Mfg LAN Events
            for (int iIndex = (int)LANEvents.BEGIN_MFG_LAN_EVENTS; iIndex < (int)LANEvents.END_MFG_LAN_EVENTS; iIndex++)
            {
                // Get the description from the EventDictionary
                string strDescription;
                bool bValidString;

                bValidString = CommEventDictionary.TryGetValue(iIndex, out strDescription);

                if (bValidString == false)
                {
                    // We don't have a description for the item.
                    strDescription = "Unknown Event " + iIndex.ToString(CultureInfo.InvariantCulture);
                }

                // Items in the MFG_EVENTS_MONITORED_FLAGS field do not include the MFG Bit.  (Must subtract off 2048)
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL113_MFG_EVENTS_MONITORED_FLAGS,
                                                new int[] { iIndex - 2048 },
                                                strDescription,
                                                "Mfg LAN Events"));
            }

            // Comm Log - Standard HAN Events
            // TODO: Implement Standard HAN Events when they exist

            //If table is not supported in the meter then it doesn't make sense to validate these items.
            if (true == Table00.IsTableUsed(115))
            {
                // Comm Log - Mfg HAN Events
                for (int iIndex = (int)HANEvents.BEGIN_MFG_HAN_EVENTS; iIndex < (int)HANEvents.END_MFG_HAN_EVENTS; iIndex++)
                {
                    // Get the description from the EventDictionary
                    string strDescription;
                    bool bValidString;

                    bValidString = CommEventDictionary.TryGetValue(iIndex, out strDescription);

                    if (bValidString == false)
                    {
                        // We don't have a description for the item.
                        strDescription = "Unknown Event " + iIndex.ToString(CultureInfo.InvariantCulture);
                    }

                    // Items in the MFG_EVENTS_MONITORED_FLAGS field do not include the MFG Bit.  (Must subtract off 2048)
                    ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL115_MFG_EVENTS_MONITORED_FLAGS,
                                                    new int[] { iIndex - 2048 },
                                                    strDescription,
                                                    "Mfg HAN Events"));
                }
            }

            // HAN 2 events
            CENTRON_AMI_DownstreamHANEventDictionary DownstreamEvtDictionary = new CENTRON_AMI_DownstreamHANEventDictionary();
            for (int iIndex = 2305; iIndex < 2431; iIndex++)
            {
                string strDescription;
                bool bValidString;

                bValidString = DownstreamEvtDictionary.TryGetValue((ushort)iIndex, out strDescription);

                // We won't worry about unknown events for now
                if (bValidString)
                {
                    ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl212Han2LoggerControl,
                                            new int[] { iIndex - 2048 - 256 },
                                            strDescription,
                                            "HAN Events",
                                            VERSION_HYDROGEN_3_7));
                }
            }

            CENTRON_AMI_UpstreamHANEventDictionary UpstreamEvtDictionary = new CENTRON_AMI_UpstreamHANEventDictionary();
            for (int iIndex = 2432; iIndex < 2559; iIndex++)
            {
                string strDescription;
                bool bValidString;

                bValidString = UpstreamEvtDictionary.TryGetValue((ushort)iIndex, out strDescription);

                // We won't worry about unknown events for now
                if (bValidString)
                {
                    ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl212Han2LoggerControl,
                                            new int[] { iIndex - 2048 - 256 },
                                            strDescription,
                                            "HAN Events",
                                            VERSION_HYDROGEN_3_7));
                }
            }
        }

        /// <summary>
        /// Adds service limiting validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetServiceLimitingValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL94_OVERRIDE_FLAG,
                                        null,
                                        "Override Connect/Disconnect Switch",
                                        "Service Limiting"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL93_CONNECT_WITH_USER_INTERVENTION_FLAG,
                                        null,
                                        "User Intervention Required After Disconnect",
                                        "Service Limiting"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL93_MAX_SWITCH_COUNT,
                                        null,
                                        "Max Number of Disconnect Switches per Period",
                                        "Service Limiting"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL93_RANDOMIZATION_ALARM,
                                        null,
                                        "Randomization Period to Send Alarms",
                                        "Service Limiting"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL93_RESTORATION_START_DELAY,
                                        null,
                                        "Reconnect Switch Delay",
                                        "Service Limiting"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL93_RESTORATION_RANDOM_DELAY,
                                        null,
                                        "Randomization Period After Reconnect Delay",
                                        "Service Limiting"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL93_OPEN_TIME,
                                        null,
                                        "Switch Open Time",
                                        "Service Limiting"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL93_RETRY_ATTEMPTS,
                                        null,
                                        "Retry Attempts",
                                        "Service Limiting"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl95FailsafeDuration,
                                        null,
                                        "Failsafe Duration",
                                        "Service Limiting"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL93_QUANTITY,
                                        new int[] { 0 },
                                        "Normal Mode Threshold Demand",
                                        "Service Limiting"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL93_THRESHOLD,
                                        new int[] { 0 },
                                        "Normal Mode Threshold",
                                        "Service Limiting"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL93_QUANTITY,
                                        new int[] { 1 },
                                        "Critical Mode Threshold Demand",
                                        "Service Limiting"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL93_THRESHOLD,
                                        new int[] { 1 },
                                        "Critical Mode Threshold",
                                        "Service Limiting"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL93_QUANTITY,
                                        new int[] { 2 },
                                        "Emergency Mode Threshold Demand",
                                        "Service Limiting"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL93_THRESHOLD,
                                        new int[] { 2 },
                                        "Emergency Mode Threshold",
                                        "Service Limiting"));
        }

        /// <summary>
        /// Adds communications validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void GetCommunicationsValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_OPTICAL,
                                        null,
                                        "Lockout: login attempts, optical",
                                        "Communications"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_OPTICAL,
                                        null,
                                        "Lockout: lockout minutes, optical",
                                        "Communications"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL142_NBR_LOGIN_ATTEMPTS_LAN,
                                        null,
                                        "Lockout: login attemtps, lan",
                                        "Communications"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL142_NBR_LOCKOUT_MINUTES_LAN,
                                        null,
                                        "Lockout: lockout minutes, lan",
                                        "Communications"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL142_FAILURES_BEFORE_FAILURE_EVENT,
                                        null,
                                        "LAN Send message failure limit",
                                        "Communications"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MFGTBL142_LAN_LINK_METRIC_PERIOD_SECONDS,
                                        null,
                                        "LAN Link metric (quality) period",
                                        "Communications"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl145C1218OverZigBee,
                                        null,
                                        "ANSI C12.18 support over ZigBee Enabled",
                                        "Communications",
                                        VERSION_2_SP5));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl145DisableZigBeeRadio,
                                        null,
                                        "Disable ZigBee Radio",
                                        "Communications",
                                        VERSION_2_SP5_1));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl145DisableZigBeePrivateProfile,
                                        null,
                                        "Disable ZigBee Private Profile",
                                        "Communications",
                                        VERSION_2_SP5_1));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl58PowerLevel,
                                        null,
                                        "ZigBee Output Power Level",
                                        "Communications"));
        }

        /// <summary>
        /// Adds Push Set validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version  ID Number Description
        // -------- --- -------  -- ------ ---------------------------------------
        // 02/08/16 PGH 4.50.226    577471   Created.
        // 06/21/16 jrf 4.50.289 WR 695777 Corrected validation items.
        protected virtual void GetPushSetValidationItems(List<EDLValidationItem> ValidationList)
        {
            //Set 1
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 0, 0 },
                                        "Data Push Set #1 Quantity #1",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 0, 1 },
                                        "Data Push Set #1 Quantity #2",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 0, 2 },
                                        "Data Push Set #1 Quantity #3",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 0, 3 },
                                        "Data Push Set #1 Quantity #4",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 0, 4 },
                                        "Data Push Set #1 Quantity #5",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 0, 5 },
                                        "Data Push Set #1 Quantity #6",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 0, 6 },
                                        "Data Push Set #1 Quantity #7",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 0, 7 },
                                        "Data Push Set #1 Quantity #8",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 0, 8 },
                                        "Data Push Set #1 Quantity #9",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 0, 9 },
                                        "Data Push Set #1 Quantity #10",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetId,
                                        new int[] { 0 },
                                        "Data Push Set #1 Name",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetVoltageMonitoringIntervalData,
                                        new int[] { 0 },
                                        "Data Push Set #1 Voltage Monitoring Last Interval",
                                        "Push Set"));

            //Set 2
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 1, 0 },
                                        "Data Push Set #2 Quantity #1",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 1, 1 },
                                        "Data Push Set #2 Quantity #2",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 1, 2 },
                                        "Data Push Set #2 Quantity #3",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 1, 3 },
                                        "Data Push Set #2 Quantity #4",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 1, 4 },
                                        "Data Push Set #2 Quantity #5",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 1, 5 },
                                        "Data Push Set #2 Quantity #6",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 1, 6 },
                                        "Data Push Set #2 Quantity #7",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 1, 7 },
                                        "Data Push Set #2 Quantity #8",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 1, 8 },
                                        "Data Push Set #2 Quantity #9",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 1, 9 },
                                        "Data Push Set #2 Quantity #10",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetId,
                                        new int[] { 1 },
                                        "Data Push Set #2 Name",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetVoltageMonitoringIntervalData,
                                        new int[] { 1 },
                                        "Data Push Set #2 Voltage Monitoring Last Interval",
                                        "Push Set"));

            //Set 3
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 2, 0 },
                                        "Data Push Set #3 Quantity #1",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 2, 1 },
                                        "Data Push Set #3 Quantity #2",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 2, 2 },
                                        "Data Push Set #3 Quantity #3",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 2, 3 },
                                        "Data Push Set #3 Quantity #4",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 2, 4 },
                                        "Data Push Set #3 Quantity #5",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 2, 5 },
                                        "Data Push Set #3 Quantity #6",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 2, 6 },
                                        "Data Push Set #3 Quantity #7",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 2, 7 },
                                        "Data Push Set #3 Quantity #8",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 2, 8 },
                                        "Data Push Set #3 Quantity #9",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetMonitoredLids,
                                        new int[] { 2, 9 },
                                        "Data Push Set #3 Quantity #10",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetId,
                                        new int[] { 2 },
                                        "Data Push Set #3 Name",
                                        "Push Set"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl217DataPushSetVoltageMonitoringIntervalData,
                                        new int[] { 2 },
                                        "Data Push Set #3 Voltage Monitoring Last Interval",
                                        "Push Set"));
        }

        /// <summary>
        /// Adds Push Group validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 02/08/16 PGH 4.50.226 577471   Created.
        //
        protected virtual void GetPushGroupValidationItems(List<EDLValidationItem> ValidationList)
        {
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137PushGroupConfigurationRecord,
                                        null,
                                        "Push Group Configuration Record",
                                        "Push Group"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137MonitoredLids,
                                        null,
                                        "Monitored Lids",
                                        "Push Group"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137DataSetId,
                                        null,
                                        "Data Set Id",
                                        "Push Group"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137VMIntervalCapture,
                                        null,
                                        "VM Interval Capture",
                                        "Push Group"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137VoltageMonitoringIntervalData,
                                        null,
                                        "Voltage Monitoring Interval Data",
                                        "Push Group"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137GroupKey,
                                        null,
                                        "Group Key",
                                        "Push Group"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137StartPushTime,
                                        null,
                                        "Start Push Time",
                                        "Push Group"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137EndPushTime,
                                        null,
                                        "End Push Time",
                                        "Push Group"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137FrequencyMin,
                                        null,
                                        "Frequency Min",
                                        "Push Group"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137DelayMin,
                                        null,
                                        "Delay Min",
                                        "Push Group"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137DelaySec,
                                        null,
                                        "Delay Sec",
                                        "Push Group"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137WindowMin,
                                        null,
                                        "Window Min",
                                        "Push Group"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137WindowSec,
                                        null,
                                        "Window Sec",
                                        "Push Group"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137EnableGroupBitField,
                                        null,
                                        "Enable Group Bit Field",
                                        "Push Group"));
            ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl137EnableGroup,
                                        null,
                                        "Enable Group",
                                        "Push Group"));
        }

        /// <summary>
        /// Adds Temperature validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 02/08/16 PGH 4.50.226 RTT556309 Created.
        // 06/17/16 AF  4.50.287 695344    Only validate the temperature items for Beryllium and above meters.
        //
        protected virtual void GetTemperatureValidationItems(List<EDLValidationItem> ValidationList)
        {
            if (FWRevision >= VERSION_BERYLLIUM)
            {
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl377DataElements,
                                            null,
                                            "Data Elements",
                                            "Temperature Configuration"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl377TemperatureControlBitField,
                                            null,
                                            "Bit Field",
                                            "Temperature Configuration"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl377TemperatureControlEnableTemperatureMonitoring,
                                            null,
                                            "Enable Temperature Monitoring",
                                            "Temperature Configuration"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl377TemperatureControlHighTemperatureThreshold1,
                                            null,
                                            "High Temperature Threshold 1",
                                            "Temperature Configuration"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl377TemperatureControlHighTemperatureThreshold2,
                                            null,
                                            "High Temperature Threshold 2",
                                            "Temperature Configuration"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl377TemperatureControlHysteresis,
                                            null,
                                            "Hysteresis",
                                            "Temperature Configuration"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl377TemperatureControlRandomizationPeriodSeconds,
                                           null,
                                           "Randomization Period Seconds",
                                           "Temperature Configuration"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl377TemperatureControlDailyCaptureTime1,
                                           null,
                                           "Daily Capture Time 1",
                                           "Temperature Configuration"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl377TemperatureControlDailyCaptureTime2,
                                           null,
                                           "Daily Capture Time 2",
                                           "Temperature Configuration"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl377TemperatureControlDailyCaptureTime3,
                                           null,
                                           "Daily Capture Time 3",
                                           "Temperature Configuration"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl377TemperatureControlDailyCaptureTime4,
                                           null,
                                           "Daily Capture Time 4",
                                           "Temperature Configuration"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl378DataElements,
                                           null,
                                           "Data Elements",
                                           "Temperature Data"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl378Temperature,
                                           null,
                                           "Temperature",
                                           "Temperature Data"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl378AverageAggregateCurrent,
                                           null,
                                           "Average Aggregate Current",
                                           "Temperature Data"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl378BaseType,
                                           null,
                                           "Base Type",
                                           "Temperature Data"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl378MeterForm,
                                          null,
                                          "Meter Form",
                                          "Temperature Data"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl378Frequency,
                                          null,
                                          "Frequency",
                                          "Temperature Data"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl378NumberOfElements,
                                          null,
                                          "Number of Elements",
                                          "Temperature Data"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl378PowerSupplyType,
                                          null,
                                          "Power Supply Type",
                                          "Temperature Data"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl378MeterClass,
                                          null,
                                          "Meter Class",
                                          "Temperature Data"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl378ServiceType,
                                         null,
                                         "Service Type",
                                         "Temperature Data"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl378DeviceClass,
                                         null,
                                         "Device Class",
                                         "Temperature Data"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl378MeterTypeBits,
                                         null,
                                         "Meter Type Bits",
                                         "Temperature Data"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl379DataElements,
                                         null,
                                         "Data Elements",
                                         "Temperature Log"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl379NextLogIndex,
                                         null,
                                         "Next Log Index",
                                         "Temperature Log"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl379TemperatureLogEntryRecord,
                                         null,
                                         "Temperature Log Entry Record",
                                         "Temperature Log"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl379TemperatureLogTemperature,
                                         null,
                                         "Temperature",
                                         "Temperature Log"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl379TemperatureLogAverageAggregateCurrent,
                                        null,
                                        "Average Aggregate Current",
                                        "Temperature Log"));
                ValidationList.Add(new EDLValidationItem((long)CentronTblEnum.MfgTbl379TemperatureLogCaptureTimeDate,
                                        null,
                                        "Capture Date",
                                        "Temperature Log"));
            }
        }

        /// <summary>
        /// Updates the TOU for the program file.
        /// </summary>
        /// <param name="ProgramTables">Program data.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/17/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected virtual void UpdateTOU(CentronTables ProgramTables)
        {
            DateTime dtCurrentSeasonStart;
            DateTime dtNextSeasonStart;
            bool bDemandReset;
            bool bSelfRead;

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
        }

        /// <summary>
        /// Retrieves the given validation item from the given table data.
        /// </summary>
        /// <param name="item">The validation item to retrieve.</param>
        /// <param name="Tables">The table data.</param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/17/13 jrf 3.50.16 TQ 9560   Created.
        //
        internal static object GetTableValue(EDLValidationItem item, TableSet Tables)
        {
            object objMeterValue;

            try
            {
                Tables.GetValue(item.Item, item.Index, out objMeterValue);
            }
            catch (Exception)
            {
                // We failed to get the value so set it to null
                objMeterValue = null;
            }

            return objMeterValue;
        }

        #endregion
    }

    /// <summary>
    /// Object that stores the information necessary for retrieving an item from the
    /// CentronTables object.
    /// </summary>
    public class EDLValidationItem
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

        public EDLValidationItem(long item, int[] index, string strName, string strCategory)
        {
            m_Item = item;
            m_Index = index;
            m_strName = strName;
            m_strCategory = strCategory;
            m_fltMinFWVersion = 0.0f;
            m_fltMaxFWVersion = 99999.0f;
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
        public EDLValidationItem(long item, int[] index, string strName, string strCategory, float fltMinFWVersion)
        {
            m_Item = item;
            m_Index = index;
            m_strName = strName;
            m_strCategory = strCategory;
            m_fltMinFWVersion = fltMinFWVersion;
            m_fltMaxFWVersion = 99999.0f;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item">The CentronTblEnum value for the item.</param>
        /// <param name="index">The index for the item.</param>
        /// <param name="strName">The name of the item.</param>
        /// <param name="strCategory">The category for the item.</param>
        /// <param name="fltMinFWVersion">The minimum firmware version needed to validate this item.</param>
        /// <param name="fltMaxFWVersion">The maximum firmwrae version that supported this item</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/19/13 AF  2.70.69 322427 Created
        //
        public EDLValidationItem(long item, int[] index, string strName, string strCategory, float fltMinFWVersion, float fltMaxFWVersion)
        {
            m_Item = item;
            m_Index = index;
            m_strName = strName;
            m_strCategory = strCategory;
            m_fltMinFWVersion = fltMinFWVersion;
            m_fltMaxFWVersion = fltMaxFWVersion;
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
        // 12/18/13 jrf 3.50.16 TQ9560 Added set.

        public long Item
        {
            get
            {
                return m_Item;
            }
            set
            {
                m_Item = value;
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

        /// <summary>
        /// Gets the maximum firmware version that supports an item
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/19/13 AF  2.70.69 322427 Created
        //
        public float MaxFWVersion
        {
            get
            {
                return m_fltMaxFWVersion;
            }
        }

        #endregion

        #region Member Variables

        private long m_Item;
        private int[] m_Index;
        private string m_strName;
        private string m_strCategory;
        private float m_fltMinFWVersion;
        private float m_fltMaxFWVersion;

        #endregion
    }
}
