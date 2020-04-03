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
//                              Copyright © 2009 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using Itron.Metering.Device;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// Class representing the Register Data in the CRF file
    /// </summary>
    public class CRFRegDataType : CRFDataType
    {
        #region Constants

        // Node Names
        private const string ROOTNODETAG = "MeterReadingDocument";
        private const string CHANNELSTAG = "Channels";
        private const string CHANNELTAG = "Channel";
        private const string CHANNELIDTAG = "ChannelID";
        private const string READINGTIMETAG = "ReadingTime";
        private const string READINGSTAG = "Readings";
        private const string READINGTAG = "Reading";
        private const string READINGSTATUSTAG = "ReadingStatus";
        private const string UNENCODEDSTATUSTAG = "UnencodedStatus";

        //Attribute Names
        private const string READINGSINPULSETAG = "ReadingsInPulse";
        private const string ISREGISTERTAG = "IsRegister";
        private const string CHANNELIDTYPETAG = "EndPointUOMID";
        private const string VALUETAG = "Value";
        private const string SOURCEVALIDATIONTAG = "SourceValidation";

        //Attribute Values
        private const string NOVALIDATION = "NV";
        private const string TRUE = "true";
        private const string FALSE = "false";

        #endregion

        #region Public Methods

        /// <summary>
        /// Default Constructor for reading the Current Register data out of the CRF file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/09 AF  2.30.04        Created
        //
        public CRFRegDataType()
            : base()
        {
        }

        /// <summary>
        /// Constructor used for writing the Current Register data to the CRF file.
        /// </summary>
        /// <param name="CurrentRegisters">Current Registers</param>
        /// <param name="TimeOfReading">Current time in the meter</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/09 AF  2.30.04        Created
        //  10/06/09 AF  2.30.07        Added TimeOfReading parameter because we need
        //                              meter time, not PC time
        //  03/27/12 jrf 2.53.52 TREQ3440/3447 Adding support for storing extended energy
        //                              registers and extended self read registers.
        //  12/21/15 PGH 4.50.222 RTT556298 Added support for Instantaneous Current
        //  04/05/16 PGH 4.50.241 RTT588001 Added support for Max Demands
        //
        public CRFRegDataType(List<Quantity> CurrentRegisters, DateTime TimeOfReading)
        {
            m_CurrentRegisters = CurrentRegisters;
            m_ExtendedRegisters = new List<ExtendedCurrentEntryRecord>();
            m_SelfReadRegisters = new List<QuantityCollection>();
            m_ExtendedSelfReadRegisters = new List<ExtendedSelfReadRecord>();
            m_InstantaneousCurrentDataRecords = new List<InstantaneousCurrentDataRecord>();
            m_MaxDemandRecords = new List<AMIMDERCD>();
            // meter current time
            m_TimeOfReading = TimeOfReading;
        }

        /// <summary>
        /// Constructor used for writing the Extended Register data to the CRF file.
        /// </summary>
        /// <param name="ExtendedRegisters">Extended Registers</param>
        /// <param name="TimeOfReading">Current time in the meter</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/09 AF  2.30.04        Created
        //  10/06/09 AF  2.30.07        Added TimeOfReading parameter because we need
        //                              meter time, not PC time
        //  03/27/12 jrf 2.53.52 TREQ3440/3447 Adding support for storing extended energy
        //                              registers and extended self read registers.
        //  12/21/15 PGH 4.50.222 RTT556298 Added support for Instantaneous Current
        //  04/05/16 PGH 4.50.241 RTT588001 Added support for Max Demands
        //
        public CRFRegDataType(List<ExtendedCurrentEntryRecord> ExtendedRegisters, DateTime TimeOfReading)
        {
            m_ExtendedRegisters = ExtendedRegisters;
            m_CurrentRegisters = new List<Quantity>();
            m_SelfReadRegisters = new List<QuantityCollection>();
            m_ExtendedSelfReadRegisters = new List<ExtendedSelfReadRecord>();
            m_InstantaneousCurrentDataRecords = new List<InstantaneousCurrentDataRecord>();
            m_MaxDemandRecords = new List<AMIMDERCD>();
            // meter current time
            m_TimeOfReading = TimeOfReading;
        }

        /// <summary>
        /// Constructor used for writing the Self Read data to the CRF file.
        /// </summary>
        /// <param name="SelfReadRegisters">Self Read Registers</param>
        /// 
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/06/09 AF  2.30.07        Combined the self read reg data class with
        //                              the current reg data class
        //  03/27/12 jrf 2.53.52 TREQ3440/3447 Adding support for storing extended energy
        //                              registers and extended self read registers.
        //  12/21/15 PGH 4.50.222 RTT556298 Added support for Instantaneous Current
        //  04/05/16 PGH 4.50.241 RTT588001 Added support for Max Demands
        //
        public CRFRegDataType(List<QuantityCollection> SelfReadRegisters)
        {
            m_SelfReadRegisters = SelfReadRegisters;
            m_ExtendedSelfReadRegisters = new List<ExtendedSelfReadRecord>();
            m_CurrentRegisters = new List<Quantity>();
            m_ExtendedRegisters = new List<ExtendedCurrentEntryRecord>();
            m_InstantaneousCurrentDataRecords = new List<InstantaneousCurrentDataRecord>();
            m_MaxDemandRecords = new List<AMIMDERCD>();
        }

        /// <summary>
        /// Constructor used for writing the Extended Self Read data to the CRF file.
        /// </summary>
        /// <param name="ExtendedSelfReadRegisters">Extended Self Read Registers</param>
        /// 
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/06/09 AF  2.30.07        Combined the self read reg data class with
        //                              the current reg data class
        //  03/27/12 jrf 2.53.52 TREQ3440/3447 Adding support for storing extended energy
        //                              registers and extended self read registers.
        //  12/21/15 PGH 4.50.222 RTT556298 Added support for Instantaneous Current
        //  04/05/16 PGH 4.50.241 RTT588001 Added support for Max Demands
        //
        public CRFRegDataType(List<ExtendedSelfReadRecord> ExtendedSelfReadRegisters)
        {
            m_ExtendedSelfReadRegisters = ExtendedSelfReadRegisters;
            m_SelfReadRegisters = new List<QuantityCollection>();
            m_CurrentRegisters = new List<Quantity>();
            m_ExtendedRegisters = new List<ExtendedCurrentEntryRecord>();
            m_InstantaneousCurrentDataRecords = new List<InstantaneousCurrentDataRecord>();
            m_MaxDemandRecords = new List<AMIMDERCD>();
        }

        /// <summary>
        /// Constructor used for writing the Instantaneous Current data to the CRF file.
        /// </summary>
        /// <param name="InstantaneousCurrentDataRecords">Instantaneous Current Data Records</param>
        /// 
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/21/15 PGH 4.50.222 RTT556298 Created
        //  04/05/16 PGH 4.50.241 RTT588001 Added support for Max Demands
        //
        public CRFRegDataType(List<InstantaneousCurrentDataRecord> InstantaneousCurrentDataRecords)
        {
            m_InstantaneousCurrentDataRecords = InstantaneousCurrentDataRecords;
            m_ExtendedSelfReadRegisters = new List<ExtendedSelfReadRecord>();
            m_SelfReadRegisters = new List<QuantityCollection>();
            m_CurrentRegisters = new List<Quantity>();
            m_ExtendedRegisters = new List<ExtendedCurrentEntryRecord>();
            m_MaxDemandRecords = new List<AMIMDERCD>();
        }

        /// <summary>
        /// Constructor used for writing the Max Demand Records to the CRF file.
        /// </summary>
        /// <param name="MaxDemandRecords">Max Demand Records</param>
        /// 
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/05/16 PGH 4.50.241 RTT588001 Created
        //
        public CRFRegDataType(List<AMIMDERCD> MaxDemandRecords)
        {
            m_MaxDemandRecords = MaxDemandRecords;
            m_ExtendedSelfReadRegisters = new List<ExtendedSelfReadRecord>();
            m_SelfReadRegisters = new List<QuantityCollection>();
            m_CurrentRegisters = new List<Quantity>();
            m_ExtendedRegisters = new List<ExtendedCurrentEntryRecord>();
            m_InstantaneousCurrentDataRecords = new List<InstantaneousCurrentDataRecord>();
        }

        /// <summary>
        /// Method that handles the writing of the Register Data to the XML file.
        /// </summary>
        /// <param name="xmlDoc">The XmlDocument that this data is going into</param>
        /// <param name="ESN">Electronic Serial Number of the meter</param>
        /// <returns>The outcome of writing the current register data to the file</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/09 AF  2.30.04        Created
        //  10/02/09 AF  2.30.05        Changed the way the result is handled so that we
        //                              don't falsely claim success
        //  10/06/09 AF  2.30.07        Restructured after code review
        //  03/27/12 jrf 2.53.52 TREQ3440/3447 Adding support for writing extended energy
        //                              registers and extended self read registers to the 
        //                              CRF file.
        //
        public override CreateCRFResult Write(XmlDocument xmlDoc, string ESN)
        {
            XmlNode ChannelsNode;
            m_Result = base.Write(xmlDoc, ESN);

            try
            {
                if (m_SelfReadRegisters.Count > 0)
                {
                    ChannelsNode = BuildChannelsNode();

                    // Process the self reads one by one
                    foreach (QuantityCollection SRQtyCollection in m_SelfReadRegisters)
                    {
                        m_TimeOfReading = SRQtyCollection.DateTimeOfReading;

                        foreach (Quantity SRReg in SRQtyCollection.Quantities)
                        {
                            WriteQuantity(ChannelsNode, SRReg);
                        }
                    }
                }
                if (m_ExtendedSelfReadRegisters.Count > 0)
                {
                    ChannelsNode = BuildChannelsNode();

                    // Process the self reads one by one
                    foreach (ExtendedSelfReadRecord ExtSRReg in m_ExtendedSelfReadRegisters)
                    {
                        WriteReading(ChannelsNode, ExtSRReg.QuantityID.lidDescription, ExtSRReg.Measurement, ExtSRReg.TimeOfOccurence);
                    }
                }
                else if (m_CurrentRegisters.Count > 0)
                {
                    ChannelsNode = BuildChannelsNode();

                    foreach (Quantity CurReg in m_CurrentRegisters)
                    {
                        WriteQuantity(ChannelsNode, CurReg);
                    }
                }
                else if (m_ExtendedRegisters.Count > 0)
                {
                    ChannelsNode = BuildChannelsNode();

                    foreach (ExtendedCurrentEntryRecord ExtReg in m_ExtendedRegisters)
                    {
                        WriteReading(ChannelsNode, ExtReg.QuantityID.lidDescription, ExtReg.Measurement, m_TimeOfReading);
                    }
                }

                if (m_InstantaneousCurrentDataRecords.Count > 0)
                {
                    ChannelsNode = BuildChannelsNode();

                    foreach (InstantaneousCurrentDataRecord InsCurr in m_InstantaneousCurrentDataRecords)
                    {
                        WriteReading(ChannelsNode, InsCurr.Description, InsCurr.Value, InsCurr.ReadingTime);
                    }
                }

                if (m_MaxDemandRecords.Count > 0)
                {
                    int iNoMD = 0;
                    DefinedLIDs LIDs = new DefinedLIDs();

                    ChannelsNode = BuildChannelsNode();

                    string StrMaxDemand = "Max Demand ";
                    string UnitDescription = "(" + LIDs.DEMAND_MAX_W_DEL.lidDescription + ")";

                    foreach (AMIMDERCD MaxDemadRecord in m_MaxDemandRecords)
                    {
                        iNoMD++;

                        string Description = StrMaxDemand + iNoMD.ToString(CultureInfo.InvariantCulture) + UnitDescription;
                        WriteReading(ChannelsNode, Description, MaxDemadRecord.MaxWattsReceived, MaxDemadRecord.DateOfDemandReset);
                    }
                }

                // If we reach this point, we can assume success
                m_Result = CreateCRFResult.SUCCESS;
            }
            catch (Exception)
            {
                m_Result = CreateCRFResult.PROTOCOL_ERROR;
            }

            return m_Result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles writing an individual quantity of whatever measurement type
        /// to the CRF file
        /// </summary>
        /// <param name="ChannelsNode">The root node to write to</param>
        /// <param name="Quantity">The quantity to be written</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/06/09 AF  2.40.07        This code was pulled out of the Write() method
        //                              so that it can be used by both current registers
        //                              and self read registers
        //
        private void WriteQuantity(XmlNode ChannelsNode, Quantity Quantity)
        {
            if (null != Quantity.TotalEnergy)
            {
                WriteReading(ChannelsNode, Quantity.TotalEnergy, m_TimeOfReading);
            }

            if (null != Quantity.TotalMaxDemand)
            {
                DateTime dtToo = Quantity.TotalMaxDemand.TimeOfOccurrence;
                WriteReading(ChannelsNode, Quantity.TotalMaxDemand, dtToo);
            }

            if (null != Quantity.CummulativeDemand)
            {
                WriteReading(ChannelsNode, Quantity.CummulativeDemand, m_TimeOfReading);
            }

            if (null != Quantity.ContinuousCummulativeDemand)
            {
                WriteReading(ChannelsNode, Quantity.ContinuousCummulativeDemand, m_TimeOfReading);
            }

            // If Quantity.TOUEnergy is not null, then this is not a coincident value
            if (null != Quantity.TOUEnergy)
            {
                WriteTOUReadings(ChannelsNode, Quantity.TOUEnergy);

                if (null != Quantity.TOUMaxDemand)
                {
                    WriteTOUDemandReadings(ChannelsNode, Quantity.TOUMaxDemand);
                }

                if (null != Quantity.TOUCummulativeDemand)
                {
                    WriteTOUReadings(ChannelsNode, Quantity.TOUCummulativeDemand);
                }

                if (null != Quantity.TOUCCummulativeDemand)
                {
                    WriteTOUReadings(ChannelsNode, Quantity.TOUCCummulativeDemand);
                }
            }
            else if (null != Quantity.TOUMaxDemand)
            {
                // Quantity.TOUEnergy == null, Quantity.TOUMaxDemand != null means 
                // this is probably a coincident value so we should populate just the demand values
                WriteTOUDemandReadings(ChannelsNode, Quantity.TOUMaxDemand);
            }
        }
        
        /// <summary>
        /// Writes the readings data to the CRF file
        /// </summary>
        /// <param name="ChannelsNode">Serves as the root node to attach the child nodes to</param>
        /// <param name="MeasurementType">The measurement type of the quantity</param>
        /// <param name="TimeOfReading">The time that the self read occurred</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/25/09 AF  2.30.04        Created
        //  10/06/09 AF  2.30.07        Restructured after code review
        //  10/15/09 AF  2.30.10        Commented out ReadingStatus fields - they might not be needed
        //  03/27/12 jrf 2.53.52 TREQ3440/3447 Refactored almost all of this method into a non Measurment
        //                              class specific method.
        //
        private void WriteReading(XmlNode ChannelsNode, Measurement MeasurementType, DateTime TimeOfReading)
        {
            WriteReading(ChannelsNode, MeasurementType.Description, MeasurementType.Value, TimeOfReading);
        }

        /// <summary>
        /// Writes the readings data to the CRF file
        /// </summary>
        /// <param name="ChannelsNode">Serves as the root node to attach the child nodes to</param>
        /// <param name="strDescription">The descripton of the reading</param>
        /// <param name="dblValue">The value of the reading</param>
        /// <param name="TimeOfReading">The time that the self read occurred</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/25/09 AF  2.30.04        Created
        //  10/06/09 AF  2.30.07        Restructured after code review
        //  10/15/09 AF  2.30.10        Commented out ReadingStatus fields - they might not be needed
        //  03/27/12 jrf 2.53.52 TREQ3440/3447 Refactored this method from another method of the same name 
        //                              to make it more generic.
        //
        private void WriteReading(XmlNode ChannelsNode, string strDescription, double dblValue, DateTime TimeOfReading)
        {
            XmlAttribute newAttribute = null;

            // Write the <Channel> tag
            XmlNode ChannelNode = m_xmlDoc.CreateElement(CHANNELTAG);

            newAttribute = m_xmlDoc.CreateAttribute(READINGSINPULSETAG);
            newAttribute.Value = FALSE;
            ChannelNode.Attributes.Append(newAttribute);

            newAttribute = m_xmlDoc.CreateAttribute(ISREGISTERTAG);
            newAttribute.Value = TRUE;
            ChannelNode.Attributes.Append(newAttribute);

            ChannelsNode.AppendChild(ChannelNode);

            // Write the <ChannelID> tag
            XmlNode ChannelID = m_xmlDoc.CreateElement(CHANNELIDTAG);
            newAttribute = m_xmlDoc.CreateAttribute(CHANNELIDTYPETAG);
            newAttribute.Value = m_ESN + ":" + ModifyDescription(strDescription);
            ChannelID.Attributes.Append(newAttribute);
            ChannelNode.AppendChild(ChannelID);

            // Add the register reading
            XmlNode Readings = m_xmlDoc.CreateElement(READINGSTAG);
            ChannelNode.AppendChild(Readings);

            XmlNode Reading = m_xmlDoc.CreateElement(READINGTAG);
            newAttribute = m_xmlDoc.CreateAttribute(VALUETAG);

            newAttribute.Value = dblValue.ToString("#######0.000", CultureInfo.CurrentCulture);

            Reading.Attributes.Append(newAttribute);

            // Write the time of reading attribute
            newAttribute = m_xmlDoc.CreateAttribute(READINGTIMETAG);

            newAttribute.Value = TimeOfReading.ToString("s", CultureInfo.InvariantCulture);
            Reading.Attributes.Append(newAttribute);

            Readings.AppendChild(Reading);

            //// Write the <ReadingStatus> tag
            //XmlNode ReadingStatus = m_xmlDoc.CreateElement(READINGSTATUSTAG);
            //Reading.AppendChild(ReadingStatus);

            //// Write the <Unencoded Status> tag
            //XmlNode UnencodedStatus = m_xmlDoc.CreateElement(UNENCODEDSTATUSTAG);
            //newAttribute = m_xmlDoc.CreateAttribute(SOURCEVALIDATIONTAG);
            //newAttribute.Value = NOVALIDATION;
            //UnencodedStatus.Attributes.Append(newAttribute);
            //ReadingStatus.AppendChild(UnencodedStatus);
        }

        /// <summary>
        /// Writes the TOU readings data to the CRF file
        /// </summary>
        /// <param name="ChannelsNode">Represents the root node to which to attach the child nodes</param>
        /// <param name="CurRegMeasList">The list of measurements associated with this quantity</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/25/09 AF  2.30.04        Created
        //  10/06/09 AF  2.30.07        Restructured after code review
        //  10/15/09 AF  2.30.10        Commented out ReadingStatus fields - they might not be needed
        //
        private void WriteTOUReadings(XmlNode ChannelsNode, List<Measurement> CurRegMeasList)
        {
            XmlAttribute newAttribute = null;

            for (int nRateIndex = 0; nRateIndex < CurRegMeasList.Count; nRateIndex++)
            {
                if (CurRegMeasList[nRateIndex] != null)
                {
                    //Write the <Channel> tag
                    XmlNode ChannelNode = m_xmlDoc.CreateElement(CHANNELTAG);
                    newAttribute = m_xmlDoc.CreateAttribute(READINGSINPULSETAG);
                    newAttribute.Value = FALSE;
                    ChannelNode.Attributes.Append(newAttribute);

                    newAttribute = m_xmlDoc.CreateAttribute(ISREGISTERTAG);
                    newAttribute.Value = TRUE;
                    ChannelNode.Attributes.Append(newAttribute);

                    ChannelsNode.AppendChild(ChannelNode);

                    // Write the <ChannelID> tag
                    XmlNode ChannelID = m_xmlDoc.CreateElement(CHANNELIDTAG);
                    newAttribute = m_xmlDoc.CreateAttribute(CHANNELIDTYPETAG);
                    newAttribute.Value = m_ESN + ":" + ModifyDescription(CurRegMeasList[nRateIndex].Description);

                    ChannelID.Attributes.Append(newAttribute);
                    ChannelNode.AppendChild(ChannelID);

                    // Add the register reading
                    XmlNode Readings = m_xmlDoc.CreateElement(READINGSTAG);
                    ChannelNode.AppendChild(Readings);

                    XmlNode Reading = m_xmlDoc.CreateElement(READINGTAG);
                    newAttribute = m_xmlDoc.CreateAttribute(VALUETAG);

                    newAttribute.Value = CurRegMeasList[nRateIndex].Value.ToString("#######0.000", CultureInfo.CurrentCulture);

                    Reading.Attributes.Append(newAttribute);

                    // Write the time of reading attribute
                    newAttribute = m_xmlDoc.CreateAttribute(READINGTIMETAG);
                    newAttribute.Value = m_TimeOfReading.ToString("s", CultureInfo.InvariantCulture);
                    Reading.Attributes.Append(newAttribute);

                    Readings.AppendChild(Reading);

                    //// Write the <ReadingStatus> tag
                    //XmlNode ReadingStatus = m_xmlDoc.CreateElement(READINGSTATUSTAG);
                    //Reading.AppendChild(ReadingStatus);

                    //// Write the <Unencoded Status> tag
                    //XmlNode UnencodedStatus = m_xmlDoc.CreateElement(UNENCODEDSTATUSTAG);
                    //newAttribute = m_xmlDoc.CreateAttribute(SOURCEVALIDATIONTAG);
                    //newAttribute.Value = NOVALIDATION;
                    //UnencodedStatus.Attributes.Append(newAttribute);
                    //ReadingStatus.AppendChild(UnencodedStatus);
                }
            }
        }

        /// <summary>
        /// Writes the TOU demand readings data to the CRF file.
        /// </summary>
        /// <param name="ChannelsNode">The root node to which to attach the child nodes</param>
        /// <param name="CurRegMeasList">The list of demand measurements associated with this quantity</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/25/09 AF  2.30.04        Created
        //  10/06/09 AF  2.30.07        Restructured after code review
        //  10/15/09 AF  2.30.10        Commented out ReadingStatus fields - they might not be needed
        //
        private void WriteTOUDemandReadings(XmlNode ChannelsNode, List<DemandMeasurement> CurRegMeasList)
        {
            XmlAttribute newAttribute = null;

            for (int nRateIndex = 0; nRateIndex < CurRegMeasList.Count; nRateIndex++)
            {
                if (CurRegMeasList[nRateIndex] != null)
                {
                    //Write the <Channel> tag
                    XmlNode ChannelNode = m_xmlDoc.CreateElement(CHANNELTAG);
                    newAttribute = m_xmlDoc.CreateAttribute(READINGSINPULSETAG);
                    newAttribute.Value = FALSE;
                    ChannelNode.Attributes.Append(newAttribute);

                    newAttribute = m_xmlDoc.CreateAttribute(ISREGISTERTAG);
                    newAttribute.Value = TRUE;
                    ChannelNode.Attributes.Append(newAttribute);

                    ChannelsNode.AppendChild(ChannelNode);

                    // Write the <ChannelID> tag
                    XmlNode ChannelID = m_xmlDoc.CreateElement(CHANNELIDTAG);
                    newAttribute = m_xmlDoc.CreateAttribute(CHANNELIDTYPETAG);

                    newAttribute.Value = m_ESN + ":" + ModifyDescription(CurRegMeasList[nRateIndex].Description);
                    ChannelID.Attributes.Append(newAttribute);
                    ChannelNode.AppendChild(ChannelID);

                    // Add the register reading
                    XmlNode Readings = m_xmlDoc.CreateElement(READINGSTAG);
                    ChannelNode.AppendChild(Readings);

                    XmlNode Reading = m_xmlDoc.CreateElement(READINGTAG);
                    newAttribute = m_xmlDoc.CreateAttribute(VALUETAG);

                    newAttribute.Value = CurRegMeasList[nRateIndex].Value.ToString("#######0.000", CultureInfo.CurrentCulture);

                    Reading.Attributes.Append(newAttribute);

                    // Write the time of reading attribute
                    newAttribute = m_xmlDoc.CreateAttribute(READINGTIMETAG);
                    newAttribute.Value = CurRegMeasList[nRateIndex].TimeOfOccurrence.ToString("s", CultureInfo.InvariantCulture);
                    Reading.Attributes.Append(newAttribute);

                    Readings.AppendChild(Reading);

                    //// Write the <ReadingStatus> tag
                    //XmlNode ReadingStatus = m_xmlDoc.CreateElement(READINGSTATUSTAG);
                    //Reading.AppendChild(ReadingStatus);

                    //// Write the <Unencoded Status> tag
                    //XmlNode UnencodedStatus = m_xmlDoc.CreateElement(UNENCODEDSTATUSTAG);
                    //newAttribute = m_xmlDoc.CreateAttribute(SOURCEVALIDATIONTAG);
                    //newAttribute.Value = NOVALIDATION;
                    //UnencodedStatus.Attributes.Append(newAttribute);
                    //ReadingStatus.AppendChild(UnencodedStatus);
                }
            }
        }

        /// <summary>
        /// Retrieves the Channels node from the CRF file and creates it if it
        /// is not there
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/06/09 AF  2.30.07        Created
        //
        private XmlNode BuildChannelsNode()
        {
            //Find the <Channels> tag if it exists
            XmlNode testRoot = m_xmlDoc.SelectSingleNode(ROOTNODETAG);
            XmlNode ChannelsNode = testRoot.SelectSingleNode(CHANNELSTAG);

            // If it's not there, create it
            if (null == ChannelsNode)
            {
                ChannelsNode = m_xmlDoc.CreateElement(CHANNELSTAG);
                testRoot.AppendChild(ChannelsNode);
            }

            return ChannelsNode;
        }

        #endregion

        #region Members

        private List<Quantity> m_CurrentRegisters;
        private List<ExtendedCurrentEntryRecord> m_ExtendedRegisters;
        private List<QuantityCollection> m_SelfReadRegisters;
        private List<ExtendedSelfReadRecord> m_ExtendedSelfReadRegisters;
        private List<InstantaneousCurrentDataRecord> m_InstantaneousCurrentDataRecords;
        private List<AMIMDERCD> m_MaxDemandRecords;
        private DateTime m_TimeOfReading;

        #endregion

    }
}
