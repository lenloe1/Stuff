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
//                        Copyright © 2008 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using Itron.Metering.DeviceDataTypes;
using System.Windows.Forms;

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// Class representing the Voltage Monitoring Profile Data stored in the CRF file
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue#   Description
    //  -------- --- -------  ------   -------------------------------------------
    //  04/28/15 PGH 4.50.109 SREQ7642 Created
    //
    public class CRFVMDataType : CRFDataType
    {
        #region Constants

        // Node Names
        private const string ROOTNODETAG = "MeterReadingDocument";
        private const string CHANNELSTAG = "Channels";
        private const string CHANNELTAG = "Channel";
        private const string CHANNELIDTAG = "ChannelID";
        private const string CONTIGUOUSSETSTAG = "ContiguousIntervalSets";
        private const string CONTIGUOUSSETTAG = "ContiguousIntervalSet";
        private const string TIMEPERIODTAG = "TimePeriod";
        private const string READINGSTAG = "Readings";
        private const string READINGTAG = "Reading";
        private const string READINGSTATUSTAG = "ReadingStatus";
        private const string UNENCODEDSTATUSTAG = "UnencodedStatus";
        private const string STATUSCODESTAG = "StatusCodes";
        private const string CODETAG = "Code";

        //Attribute Names
        private const string READINGSINPULSETAG = "ReadingsInPulse";
        private const string CHANNELIDTYPETAG = "EndPointUOMID";
        private const string NUMBEROFREADINGSTAG = "NumberOfReadings";
        private const string ENDTIMETAG = "EndTime";
        private const string STARTTIMETAG = "StartTime";
        private const string VALUETAG = "Value";
        private const string SOURCEVALIDATIONTAG = "SourceValidation";
        private const string INTERVALLENGTHTAG = "IntervalLength";

        //Attribute Values
        private const string NOVALIDATION = "NV";

        //Voltage Phases
        private enum VMPhases
        {
            PhaseA = 0,
            PhaseB = 1,
            PhaseC = 2,
        };

        private const int TWO_PHASES = 2;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor used when reading the Voltage Monitoring Profile Data out of the CRF File
        /// </summary>
        public CRFVMDataType()
            : base()
        {
        }

        /// <summary>
        /// Constructor used when writing a CRF file
        /// </summary>
        /// <param name="VMData">Voltage Monitoring Profile Data to write to CRF file</param>
        public CRFVMDataType(VMData VMData)
            : base()
        {
            m_VMData = VMData;
        }

        /// <summary>
        /// Method that handles the writing of the XML file.
        /// </summary>
        /// <param name="xmlDoc">The XmlDocument that this data is going into</param>
        /// <param name="ESN">Electronic Serial Number</param>
        // Revision History	
        // MM/DD/YY who Version  Issue#   Description
        // -------- --- -------  ------   ---------------------------------------
        // 04/28/15 PGH 4.50.109 SREQ7642 Created
        // 05/18/16 PGH 4.50.269 686474   Use ToString when appending phases to the xml channel ID attribute value
        // 12/07/17 AF  4.73.00 Bug528301 Corrected the display for 2 phase meters
        //
        public override CreateCRFResult Write(XmlDocument xmlDoc, string ESN)
        {
            m_Result = base.Write(xmlDoc, ESN);

            try
            {
                XmlAttribute newAttribute = null;

                // We must have at least one interval to enter this section of code.
                if (VMData.Intervals.Count > 0)
                {
                    // Get the Root node and write the <Channels> tag to it.
                    XmlNode testRoot = m_xmlDoc.SelectSingleNode(ROOTNODETAG);
                    XmlNode ChannelsNode = testRoot.SelectSingleNode(CHANNELSTAG);

                    // If it's not there, create it
                    if (null == ChannelsNode)
                    {
                        ChannelsNode = m_xmlDoc.CreateElement(CHANNELSTAG);
                        testRoot.AppendChild(ChannelsNode);
                    }

                    // VhData channel
                    for (int i = 0; i < VMData.Intervals[0].VhData.Count; i++)
                    {
                        XmlNode ChannelNode = m_xmlDoc.CreateElement(CHANNELTAG);
                        newAttribute = m_xmlDoc.CreateAttribute(READINGSINPULSETAG);
                        newAttribute.Value = "true";
                        ChannelNode.Attributes.Append(newAttribute);

                        newAttribute = m_xmlDoc.CreateAttribute(INTERVALLENGTHTAG);
                        newAttribute.Value = VMData.IntervalLength.Duration().TotalMinutes.ToString(CultureInfo.InvariantCulture);
                        ChannelNode.Attributes.Append(newAttribute);

                        ChannelsNode.AppendChild(ChannelNode);

                        // Write the <ChannelID> tag
                        XmlNode ChannelID = m_xmlDoc.CreateElement(CHANNELIDTAG);
                        newAttribute = m_xmlDoc.CreateAttribute(CHANNELIDTYPETAG);
                        if (VMData.Intervals[0].VhData.Count == TWO_PHASES)
                        {
                            newAttribute.Value = m_ESN + ":" + "Vh" + "(" + (i == 0 ? "a" : "c") + ")";
                        }
                        else
                        {
                            newAttribute.Value = m_ESN + ":" + "Vh" + "(" + (i == 0 ? "a" : (i == 1) ? "b" : "c") + ")";
                        }
                        ChannelID.Attributes.Append(newAttribute);
                        ChannelNode.AppendChild(ChannelID);

                        // Now write the Readings
                        WriteReadingsVhData(ChannelNode, VMData.Intervals, i);
                    }

                    // VData channel
                    for (int i = 0; i < VMData.Intervals[0].VhData.Count; i++)
                    {
                        XmlNode ChannelNode = m_xmlDoc.CreateElement(CHANNELTAG);
                        newAttribute = m_xmlDoc.CreateAttribute(READINGSINPULSETAG);
                        newAttribute.Value = "true";
                        ChannelNode.Attributes.Append(newAttribute);

                        newAttribute = m_xmlDoc.CreateAttribute(INTERVALLENGTHTAG);
                        newAttribute.Value = VMData.IntervalLength.Duration().TotalMinutes.ToString(CultureInfo.InvariantCulture);
                        ChannelNode.Attributes.Append(newAttribute);

                        ChannelsNode.AppendChild(ChannelNode);

                        // Write the <ChannelID> tag
                        XmlNode ChannelID = m_xmlDoc.CreateElement(CHANNELIDTAG);
                        newAttribute = m_xmlDoc.CreateAttribute(CHANNELIDTYPETAG);
                        if (VMData.Intervals[0].VhData.Count == TWO_PHASES)
                        {
                            newAttribute.Value = m_ESN + ":" + "V" + "(" + (i == 0 ? "a" : "c") + ")";
                        }
                        else
                        {
                            newAttribute.Value = m_ESN + ":" + "V" + "(" + (i == 0 ? "a" : (i == 1) ? "b" : "c") + ")";
                        }
                        ChannelID.Attributes.Append(newAttribute);
                        ChannelNode.AppendChild(ChannelID);

                        // Now write the Readings
                        WriteReadingsVData(ChannelNode, VMData.Intervals, VMData.IntervalLength, i);
                    }


                    // VminData channel
                    for (int i = 0; i < VMData.Intervals[0].VhData.Count; i++)
                    {
                        XmlNode ChannelNode = m_xmlDoc.CreateElement(CHANNELTAG);
                        newAttribute = m_xmlDoc.CreateAttribute(READINGSINPULSETAG);
                        newAttribute.Value = "true";
                        ChannelNode.Attributes.Append(newAttribute);

                        newAttribute = m_xmlDoc.CreateAttribute(INTERVALLENGTHTAG);
                        newAttribute.Value = VMData.IntervalLength.Duration().TotalMinutes.ToString(CultureInfo.InvariantCulture);
                        ChannelNode.Attributes.Append(newAttribute);

                        ChannelsNode.AppendChild(ChannelNode);

                        // Write the <ChannelID> tag
                        XmlNode ChannelID = m_xmlDoc.CreateElement(CHANNELIDTAG);
                        newAttribute = m_xmlDoc.CreateAttribute(CHANNELIDTYPETAG);
                        if (VMData.Intervals[0].VhData.Count == TWO_PHASES)
                        {
                            newAttribute.Value = m_ESN + ":" + "min V" + "(" + (i == 0 ? "a" : "c") + ")";
                        }
                        else
                        {
                            newAttribute.Value = m_ESN + ":" + "min V" + "(" + (i == 0 ? "a" : (i == 1) ? "b" : "c") + ")";
                        }
                        ChannelID.Attributes.Append(newAttribute);
                        ChannelNode.AppendChild(ChannelID);

                        // Now write the Readings
                        WriteReadingsVmin(ChannelNode, VMData.Intervals, i);
                    }

                    // VmaxData channel
                    for (int i = 0; i < VMData.Intervals[0].VhData.Count; i++)
                    {
                        XmlNode ChannelNode = m_xmlDoc.CreateElement(CHANNELTAG);
                        newAttribute = m_xmlDoc.CreateAttribute(READINGSINPULSETAG);
                        newAttribute.Value = "true";
                        ChannelNode.Attributes.Append(newAttribute);

                        newAttribute = m_xmlDoc.CreateAttribute(INTERVALLENGTHTAG);
                        newAttribute.Value = VMData.IntervalLength.Duration().TotalMinutes.ToString(CultureInfo.InvariantCulture);
                        ChannelNode.Attributes.Append(newAttribute);

                        ChannelsNode.AppendChild(ChannelNode);

                        // Write the <ChannelID> tag
                        XmlNode ChannelID = m_xmlDoc.CreateElement(CHANNELIDTAG);
                        newAttribute = m_xmlDoc.CreateAttribute(CHANNELIDTYPETAG);
                        if (VMData.Intervals[0].VhData.Count == TWO_PHASES)
                        {
                            newAttribute.Value = m_ESN + ":" + "max V" + "(" + (i == 0 ? "a" : "c") + ")";
                        }
                        else
                        {
                            newAttribute.Value = m_ESN + ":" + "max V" + "(" + (i == 0 ? "a" : (i == 1) ? "b" : "c") + ")";
                        }
                        ChannelID.Attributes.Append(newAttribute);
                        ChannelNode.AppendChild(ChannelID);

                        // Now write the Readings
                        WriteReadingsVmax(ChannelNode, VMData.Intervals, i);
                    }
                }

                m_Result = CreateCRFResult.SUCCESS;
            }
            catch (Exception)
            {
                m_Result = CreateCRFResult.PROTOCOL_ERROR;
            }

            return m_Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Property to get the VMData object
        /// </summary>
        public VMData VMData
        {
            get
            {
                return m_VMData;
            }
        }

        /// <summary>
        /// Property to get the Electronic Serial Number string
        /// </summary>
        public string ESN
        {
            get
            {
                return m_ESN;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method writes the Voltage hours readings data to the CRF file.
        /// </summary>
        /// <param name="channelNode">The channel node to add this data to</param>
        /// <param name="vmIntervals">The voltage monitoring profile interval data</param>
        /// <param name="phase">The phase value to write</param>
        // Revision History	
        // MM/DD/YY who Version  Issue#   Description
        // -------- --- -------  ------   ---------------------------------------
        // 04/29/15 PGH 4.50.109 SREQ7642 Created
        // 05/04/16 PGH 4.50.261 680037   Added DST status
        //
        private void WriteReadingsVhData(XmlNode channelNode, List<VMInterval>vmIntervals, int phase)
        {
            XmlAttribute NewAttribute = null;

            // Write the <ContiguousIntervalSets> tag
            XmlNode ContiguousDataSets = m_xmlDoc.CreateElement(CONTIGUOUSSETSTAG);
            channelNode.AppendChild(ContiguousDataSets);

            // Write the <ContiguousIntervalSet> tag
            XmlNode ContiguousDataSet = m_xmlDoc.CreateElement(CONTIGUOUSSETTAG);
            NewAttribute = m_xmlDoc.CreateAttribute(NUMBEROFREADINGSTAG);
            NewAttribute.Value = vmIntervals.Count.ToString(CultureInfo.InvariantCulture);
            ContiguousDataSet.Attributes.Append(NewAttribute);
            ContiguousDataSets.AppendChild(ContiguousDataSet);

            // Write the <TimePerid> tag
            XmlNode TimePeriod = m_xmlDoc.CreateElement(TIMEPERIODTAG);
            NewAttribute = m_xmlDoc.CreateAttribute(ENDTIMETAG);
            NewAttribute.Value = vmIntervals[vmIntervals.Count-1].IntervalEndTime.ToString("s", CultureInfo.InvariantCulture);
            TimePeriod.Attributes.Append(NewAttribute);
            NewAttribute = m_xmlDoc.CreateAttribute(STARTTIMETAG);
            NewAttribute.Value = vmIntervals[0].IntervalEndTime.ToString("s", CultureInfo.InvariantCulture);
            TimePeriod.Attributes.Append(NewAttribute);
            ContiguousDataSet.AppendChild(TimePeriod);

            // Write the <Readings> tag
            XmlNode Readings = m_xmlDoc.CreateElement(READINGSTAG);
            ContiguousDataSet.AppendChild(Readings);

            XmlNode Reading = null;
            XmlNode ReadingStatus = null;
            XmlNode UnencodedStatus = null;
            XmlNode StatusCodes = null;
            XmlNode Code = null;

            // Now we will go through the Voltage Profile to write the <Reading> tags
            for (int iNumIntervals = 0; iNumIntervals < vmIntervals.Count; iNumIntervals++)
            {
                // Write the <Reading> tag
                Reading = m_xmlDoc.CreateElement(READINGTAG);
                NewAttribute = m_xmlDoc.CreateAttribute(VALUETAG);
                NewAttribute.Value = vmIntervals[iNumIntervals].VhData[phase].ToString(CultureInfo.InvariantCulture);
                Reading.Attributes.Append(NewAttribute);
                Readings.AppendChild(Reading);

                // Write the <ReadingStatus> tag
                ReadingStatus = m_xmlDoc.CreateElement(READINGSTATUSTAG);
                Reading.AppendChild(ReadingStatus);

                // Write the <Unencoded Status> tag
                UnencodedStatus = m_xmlDoc.CreateElement(UNENCODEDSTATUSTAG);
                NewAttribute = m_xmlDoc.CreateAttribute(SOURCEVALIDATIONTAG);
                NewAttribute.Value = NOVALIDATION;
                UnencodedStatus.Attributes.Append(NewAttribute);
                ReadingStatus.AppendChild(UnencodedStatus);

                // Write the <StatusCodes> tag
                StatusCodes = m_xmlDoc.CreateElement(STATUSCODESTAG);
                UnencodedStatus.AppendChild(StatusCodes);

                string strStatuses = vmIntervals[iNumIntervals].IntervalStatusString.ToString(CultureInfo.InvariantCulture);

                for (int NumStatus = 0; NumStatus < strStatuses.Length; NumStatus++)
                {
                    Code = m_xmlDoc.CreateElement(CODETAG);

                    switch (strStatuses.Substring(NumStatus, 1))
                    {
                        case "A":
                            {
                                Code.InnerText = "TADJUSTED";
                                break;
                            }
                        case "L":
                        case "S":
                            {
                                Code.InnerText = "IRREGULAR";
                                break;
                            }
                        case "K":
                            {
                                Code.InnerText = "MEASURE";
                                break;
                            }
                        case "T":
                            {
                                Code.InnerText = "TEST";
                                break;
                            }
                        case "V":
                            {
                                Code.InnerText = "PULSE";
                                break;
                            }
                        case "O":
                            {
                                Code.InnerText = "POWER";
                                break;
                            }
                        case "R":
                            {
                                Code.InnerText = "PR";
                                break;
                            }
                        case "D":
                            {
                                Code.InnerText = "DST";
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }

                    if (String.IsNullOrEmpty(Code.InnerText) == false)
                    {
                        // We shouldn't add the status node if there is nothing to add
                        StatusCodes.AppendChild(Code);
                    }
                }
                
            }
        }

        /// <summary>
        /// This method writes the Voltage readings data to the CRF file.
        /// </summary>
        /// <param name="channelNode">The channel node to add this data to</param>
        /// <param name="vmIntervals">The voltage monitoring profile interval data</param>
        /// <param name="iLength">The interval length</param>
        /// <param name="phase">The phase value to write</param>
        // Revision History	
        // MM/DD/YY who Version  Issue#   Description
        // -------- --- -------  ------   ---------------------------------------
        // 05/07/15 PGH 4.50.121 SREQ7642 Created
        // 05/04/16 PGH 4.50.261 680037   Added DST status
        //
        private void WriteReadingsVData(XmlNode channelNode, List<VMInterval> vmIntervals, TimeSpan iLength, int phase)
        {
            XmlAttribute NewAttribute = null;

            // Write the <ContiguousIntervalSets> tag
            XmlNode ContiguousDataSets = m_xmlDoc.CreateElement(CONTIGUOUSSETSTAG);
            channelNode.AppendChild(ContiguousDataSets);

            // Write the <ContiguousIntervalSet> tag
            XmlNode ContiguousDataSet = m_xmlDoc.CreateElement(CONTIGUOUSSETTAG);
            NewAttribute = m_xmlDoc.CreateAttribute(NUMBEROFREADINGSTAG);
            NewAttribute.Value = vmIntervals.Count.ToString(CultureInfo.InvariantCulture);
            ContiguousDataSet.Attributes.Append(NewAttribute);
            ContiguousDataSets.AppendChild(ContiguousDataSet);

            // Write the <TimePerid> tag
            XmlNode TimePeriod = m_xmlDoc.CreateElement(TIMEPERIODTAG);
            NewAttribute = m_xmlDoc.CreateAttribute(ENDTIMETAG);
            NewAttribute.Value = vmIntervals[vmIntervals.Count - 1].IntervalEndTime.ToString("s", CultureInfo.InvariantCulture);
            TimePeriod.Attributes.Append(NewAttribute);
            NewAttribute = m_xmlDoc.CreateAttribute(STARTTIMETAG);
            NewAttribute.Value = vmIntervals[0].IntervalEndTime.ToString("s", CultureInfo.InvariantCulture);
            TimePeriod.Attributes.Append(NewAttribute);
            ContiguousDataSet.AppendChild(TimePeriod);

            // Write the <Readings> tag
            XmlNode Readings = m_xmlDoc.CreateElement(READINGSTAG);
            ContiguousDataSet.AppendChild(Readings);

            XmlNode Reading = null;
            XmlNode ReadingStatus = null;
            XmlNode UnencodedStatus = null;
            XmlNode StatusCodes = null;
            XmlNode Code = null;

            // Now we will go through the Voltage Profile to write the <Reading> tags
            for (int iNumIntervals = 0; iNumIntervals < vmIntervals.Count; iNumIntervals++)
            {
                // Write the <Reading> tag
                Reading = m_xmlDoc.CreateElement(READINGTAG);
                NewAttribute = m_xmlDoc.CreateAttribute(VALUETAG);
                double VData = vmIntervals[iNumIntervals].VhData[phase] / iLength.TotalHours;
                NewAttribute.Value = VData.ToString("F2", CultureInfo.InvariantCulture);
                Reading.Attributes.Append(NewAttribute);
                Readings.AppendChild(Reading);

                // Write the <ReadingStatus> tag
                ReadingStatus = m_xmlDoc.CreateElement(READINGSTATUSTAG);
                Reading.AppendChild(ReadingStatus);

                // Write the <Unencoded Status> tag
                UnencodedStatus = m_xmlDoc.CreateElement(UNENCODEDSTATUSTAG);
                NewAttribute = m_xmlDoc.CreateAttribute(SOURCEVALIDATIONTAG);
                NewAttribute.Value = NOVALIDATION;
                UnencodedStatus.Attributes.Append(NewAttribute);
                ReadingStatus.AppendChild(UnencodedStatus);

                // Write the <StatusCodes> tag
                StatusCodes = m_xmlDoc.CreateElement(STATUSCODESTAG);
                UnencodedStatus.AppendChild(StatusCodes);

                string strStatuses = vmIntervals[iNumIntervals].IntervalStatusString.ToString(CultureInfo.InvariantCulture);

                for (int NumStatus = 0; NumStatus < strStatuses.Length; NumStatus++)
                {
                    Code = m_xmlDoc.CreateElement(CODETAG);

                    switch (strStatuses.Substring(NumStatus, 1))
                    {
                        case "A":
                            {
                                Code.InnerText = "TADJUSTED";
                                break;
                            }
                        case "L":
                        case "S":
                            {
                                Code.InnerText = "IRREGULAR";
                                break;
                            }
                        case "K":
                            {
                                Code.InnerText = "MEASURE";
                                break;
                            }
                        case "T":
                            {
                                Code.InnerText = "TEST";
                                break;
                            }
                        case "V":
                            {
                                Code.InnerText = "PULSE";
                                break;
                            }
                        case "O":
                            {
                                Code.InnerText = "POWER";
                                break;
                            }
                        case "R":
                            {
                                Code.InnerText = "PR";
                                break;
                            }
                        case "D":
                            {
                                Code.InnerText = "DST";
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }

                    if (String.IsNullOrEmpty(Code.InnerText) == false)
                    {
                        // We shouldn't add the status node if there is nothing to add
                        StatusCodes.AppendChild(Code);
                    }
                }

            }
        }

        /// <summary>
        /// This method writes the Voltage minimum readings data to the CRF file.
        /// </summary>
        /// <param name="channelNode">The channel node to add this data to</param>
        /// <param name="vmIntervals">The voltage monitoring profile interval data</param>
        /// <param name="phase">The phase value to write</param>
        // Revision History	
        // MM/DD/YY who Version  Issue#   Description
        // -------- --- -------  ------   ---------------------------------------
        // 04/29/15 PGH 4.50.109 SREQ7642 Created
        // 05/04/16 PGH 4.50.261 680037   Added DST status
        //
        private void WriteReadingsVmin(XmlNode channelNode, List<VMInterval> vmIntervals, int phase)
        {
            XmlAttribute NewAttribute = null;

            // Write the <ContiguousIntervalSets> tag
            XmlNode ContiguousDataSets = m_xmlDoc.CreateElement(CONTIGUOUSSETSTAG);
            channelNode.AppendChild(ContiguousDataSets);

            // Write the <ContiguousIntervalSet> tag
            XmlNode ContiguousDataSet = m_xmlDoc.CreateElement(CONTIGUOUSSETTAG);
            NewAttribute = m_xmlDoc.CreateAttribute(NUMBEROFREADINGSTAG);
            NewAttribute.Value = vmIntervals.Count.ToString(CultureInfo.InvariantCulture);
            ContiguousDataSet.Attributes.Append(NewAttribute);
            ContiguousDataSets.AppendChild(ContiguousDataSet);

            // Write the <TimePerid> tag
            XmlNode TimePeriod = m_xmlDoc.CreateElement(TIMEPERIODTAG);
            NewAttribute = m_xmlDoc.CreateAttribute(ENDTIMETAG);
            NewAttribute.Value = vmIntervals[vmIntervals.Count - 1].IntervalEndTime.ToString("s", CultureInfo.InvariantCulture);
            TimePeriod.Attributes.Append(NewAttribute);
            NewAttribute = m_xmlDoc.CreateAttribute(STARTTIMETAG);
            NewAttribute.Value = vmIntervals[0].IntervalEndTime.ToString("s", CultureInfo.InvariantCulture);
            TimePeriod.Attributes.Append(NewAttribute);
            ContiguousDataSet.AppendChild(TimePeriod);

            // Write the <Readings> tag
            XmlNode Readings = m_xmlDoc.CreateElement(READINGSTAG);
            ContiguousDataSet.AppendChild(Readings);

            XmlNode Reading = null;
            XmlNode ReadingStatus = null;
            XmlNode UnencodedStatus = null;
            XmlNode StatusCodes = null;
            XmlNode Code = null;

            // Now we will go through the Voltage Profile to write the <Reading> tags
            for (int iNumIntervals = 0; iNumIntervals < vmIntervals.Count; iNumIntervals++)
            {
                // Write the <Reading> tag
                Reading = m_xmlDoc.CreateElement(READINGTAG);
                NewAttribute = m_xmlDoc.CreateAttribute(VALUETAG);
                NewAttribute.Value = vmIntervals[iNumIntervals].VminData[phase].ToString(CultureInfo.InvariantCulture);
                Reading.Attributes.Append(NewAttribute);
                Readings.AppendChild(Reading);

                // Write the <ReadingStatus> tag
                ReadingStatus = m_xmlDoc.CreateElement(READINGSTATUSTAG);
                Reading.AppendChild(ReadingStatus);

                // Write the <Unencoded Status> tag
                UnencodedStatus = m_xmlDoc.CreateElement(UNENCODEDSTATUSTAG);
                NewAttribute = m_xmlDoc.CreateAttribute(SOURCEVALIDATIONTAG);
                NewAttribute.Value = NOVALIDATION;
                UnencodedStatus.Attributes.Append(NewAttribute);
                ReadingStatus.AppendChild(UnencodedStatus);

                // Write the <StatusCodes> tag
                StatusCodes = m_xmlDoc.CreateElement(STATUSCODESTAG);
                UnencodedStatus.AppendChild(StatusCodes);

                string strStatuses = vmIntervals[iNumIntervals].IntervalStatusString.ToString(CultureInfo.InvariantCulture);

                for (int NumStatus = 0; NumStatus < strStatuses.Length; NumStatus++)
                {
                    Code = m_xmlDoc.CreateElement(CODETAG);

                    switch (strStatuses.Substring(NumStatus, 1))
                    {
                        case "A":
                            {
                                Code.InnerText = "TADJUSTED";
                                break;
                            }
                        case "L":
                        case "S":
                            {
                                Code.InnerText = "IRREGULAR";
                                break;
                            }
                        case "K":
                            {
                                Code.InnerText = "MEASURE";
                                break;
                            }
                        case "T":
                            {
                                Code.InnerText = "TEST";
                                break;
                            }
                        case "V":
                            {
                                Code.InnerText = "PULSE";
                                break;
                            }
                        case "O":
                            {
                                Code.InnerText = "POWER";
                                break;
                            }
                        case "R":
                            {
                                Code.InnerText = "PR";
                                break;
                            }
                        case "D":
                            {
                                Code.InnerText = "DST";
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }

                    if (String.IsNullOrEmpty(Code.InnerText) == false)
                    {
                        // We shouldn't add the status node if there is nothing to add
                        StatusCodes.AppendChild(Code);
                    }
                }

            }
        }

        /// <summary>
        /// This method writes the Voltage maximum readings data to the CRF file.
        /// </summary>
        /// <param name="channelNode">The channel node to add this data to</param>
        /// <param name="vmIntervals">The voltage monitoring profile interval data</param>
        /// <param name="phase">The phase value to write</param>
        // Revision History	
        // MM/DD/YY who Version  Issue#   Description
        // -------- --- -------  ------   ---------------------------------------
        // 04/29/15 PGH 4.50.109 SREQ7642 Created
        // 05/04/16 PGH 4.50.261 680037   Added DST status
        //
        private void WriteReadingsVmax(XmlNode channelNode, List<VMInterval> vmIntervals, int phase)
        {
            XmlAttribute NewAttribute = null;

            // Write the <ContiguousIntervalSets> tag
            XmlNode ContiguousDataSets = m_xmlDoc.CreateElement(CONTIGUOUSSETSTAG);
            channelNode.AppendChild(ContiguousDataSets);

            // Write the <ContiguousIntervalSet> tag
            XmlNode ContiguousDataSet = m_xmlDoc.CreateElement(CONTIGUOUSSETTAG);
            NewAttribute = m_xmlDoc.CreateAttribute(NUMBEROFREADINGSTAG);
            NewAttribute.Value = vmIntervals.Count.ToString(CultureInfo.InvariantCulture);
            ContiguousDataSet.Attributes.Append(NewAttribute);
            ContiguousDataSets.AppendChild(ContiguousDataSet);

            // Write the <TimePerid> tag
            XmlNode TimePeriod = m_xmlDoc.CreateElement(TIMEPERIODTAG);
            NewAttribute = m_xmlDoc.CreateAttribute(ENDTIMETAG);
            NewAttribute.Value = vmIntervals[vmIntervals.Count - 1].IntervalEndTime.ToString("s", CultureInfo.InvariantCulture);
            TimePeriod.Attributes.Append(NewAttribute);
            NewAttribute = m_xmlDoc.CreateAttribute(STARTTIMETAG);
            NewAttribute.Value = vmIntervals[0].IntervalEndTime.ToString("s", CultureInfo.InvariantCulture);
            TimePeriod.Attributes.Append(NewAttribute);
            ContiguousDataSet.AppendChild(TimePeriod);

            // Write the <Readings> tag
            XmlNode Readings = m_xmlDoc.CreateElement(READINGSTAG);
            ContiguousDataSet.AppendChild(Readings);

            XmlNode Reading = null;
            XmlNode ReadingStatus = null;
            XmlNode UnencodedStatus = null;
            XmlNode StatusCodes = null;
            XmlNode Code = null;

            // Now we will go through the Voltage Profile to write the <Reading> tags
            for (int iNumIntervals = 0; iNumIntervals < vmIntervals.Count; iNumIntervals++)
            {
                // Write the <Reading> tag
                Reading = m_xmlDoc.CreateElement(READINGTAG);
                NewAttribute = m_xmlDoc.CreateAttribute(VALUETAG);
                NewAttribute.Value = vmIntervals[iNumIntervals].VmaxData[phase].ToString(CultureInfo.InvariantCulture);
                Reading.Attributes.Append(NewAttribute);
                Readings.AppendChild(Reading);

                // Write the <ReadingStatus> tag
                ReadingStatus = m_xmlDoc.CreateElement(READINGSTATUSTAG);
                Reading.AppendChild(ReadingStatus);

                // Write the <Unencoded Status> tag
                UnencodedStatus = m_xmlDoc.CreateElement(UNENCODEDSTATUSTAG);
                NewAttribute = m_xmlDoc.CreateAttribute(SOURCEVALIDATIONTAG);
                NewAttribute.Value = NOVALIDATION;
                UnencodedStatus.Attributes.Append(NewAttribute);
                ReadingStatus.AppendChild(UnencodedStatus);

                // Write the <StatusCodes> tag
                StatusCodes = m_xmlDoc.CreateElement(STATUSCODESTAG);
                UnencodedStatus.AppendChild(StatusCodes);

                string strStatuses = vmIntervals[iNumIntervals].IntervalStatusString.ToString(CultureInfo.InvariantCulture);

                for (int NumStatus = 0; NumStatus < strStatuses.Length; NumStatus++)
                {
                    Code = m_xmlDoc.CreateElement(CODETAG);

                    switch (strStatuses.Substring(NumStatus, 1))
                    {
                        case "A":
                            {
                                Code.InnerText = "TADJUSTED";
                                break;
                            }
                        case "L":
                        case "S":
                            {
                                Code.InnerText = "IRREGULAR";
                                break;
                            }
                        case "K":
                            {
                                Code.InnerText = "MEASURE";
                                break;
                            }
                        case "T":
                            {
                                Code.InnerText = "TEST";
                                break;
                            }
                        case "V":
                            {
                                Code.InnerText = "PULSE";
                                break;
                            }
                        case "O":
                            {
                                Code.InnerText = "POWER";
                                break;
                            }
                        case "R":
                            {
                                Code.InnerText = "PR";
                                break;
                            }
                        case "D":
                            {
                                Code.InnerText = "DST";
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }

                    if (String.IsNullOrEmpty(Code.InnerText) == false)
                    {
                        // We shouldn't add the status node if there is nothing to add
                        StatusCodes.AppendChild(Code);
                    }
                }

            }
        }

        #endregion

        #region Members

        private VMData m_VMData = null;

        #endregion
    }
}
