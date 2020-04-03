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

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// Class representing the LP Data stored in the CRF file
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/30/09 AF  2.20.10 136279 Changed "CIM" to "CRF"
    //
    public class CRFLPDataType : CRFDataType
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor used when reading the LP Data out of the CRF File
        /// </summary>
        public CRFLPDataType()
            : base()
        {
        }

        /// <summary>
        /// Constructor used when writing a CRF file
        /// </summary>
        /// <param name="LPData">Load Profile Data to write to CRF file</param>
        public CRFLPDataType(LoadProfileData LPData)
            : base()
        {
            m_LPData = LPData;
        }

        /// <summary>
        /// Method that handles the writing of the XML file.
        /// </summary>
        /// <param name="xmlDoc">The XmlDocument that this data is going into</param>
        /// <param name="ESN">Electronic Serial Number</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/20/09 jrf 2.20.14 137977 Moved readings in pulse to be an attribute
        //                             of channel node not channels.
        // 10/02/09 AF  2.30.05        Changed the way the result is handled so that we
        //                             don't falsely claim success
        // 11/06/09 AF  2.30.16 144703 Add the LP interval length to the channel attributes
        // 03/27/12 jrf 2.53.52 TREQ2891 Only creating channels node if it does not exist. 
        //                             Switched to use the base class method ModifyDescription() 
        //                             instead of ModifyChannelName().
        //
        public override CreateCRFResult Write(XmlDocument xmlDoc, string ESN)
        {
            m_Result = base.Write(xmlDoc, ESN);

            try
            {
                XmlAttribute newAttribute = null;

                // We must have at least one channel to enter this section of code.
                if (LPData.NumberIntervals > 0)
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

                    foreach (LPChannel Channel in LPData.Channels)
                    {
                        // Write the <Channel> tag
                        XmlNode ChannelNode = m_xmlDoc.CreateElement(CHANNELTAG);
                        newAttribute = m_xmlDoc.CreateAttribute(READINGSINPULSETAG);
                        newAttribute.Value = "true";
                        ChannelNode.Attributes.Append(newAttribute);

                        newAttribute = m_xmlDoc.CreateAttribute(INTERVALLENGTHTAG);
                        newAttribute.Value = LPData.IntervalDuration.ToString(CultureInfo.InvariantCulture);
                        ChannelNode.Attributes.Append(newAttribute);

                        ChannelsNode.AppendChild(ChannelNode);

                        // Write the <ChannelID> tag
                        XmlNode ChannelID = m_xmlDoc.CreateElement(CHANNELIDTAG);
                        newAttribute = m_xmlDoc.CreateAttribute(CHANNELIDTYPETAG);
                        newAttribute.Value = m_ESN + ":" + ModifyDescription(Channel.ChannelName);
                        ChannelID.Attributes.Append(newAttribute);
                        ChannelNode.AppendChild(ChannelID);

                        // Now write the Readings
                        WriteReadings(ChannelNode, Channel);
                    }
                }

                m_Result = CreateCRFResult.SUCCESS;
            }
            catch(Exception)
            {
                m_Result = CreateCRFResult.PROTOCOL_ERROR;
            }

            return m_Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Property to get the LPData object
        /// </summary>
        public LoadProfileData LPData
        {
            get
            {
                return m_LPData;
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
        /// This method writes the readings data to the CRF file.
        /// </summary>
        /// <param name="ChannelNode">The channel node to add this data to.</param>
        /// <param name="Channel">The load profile channel data to write to the file.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/20/09 jrf 2.20.14 137977 Moved readings element to be a child of
        //                             contiguous data set instead of time period.
        // 08/04/09 jrf 2.20.20 137977 Corrected formatting of statuses, changed to use 
        //                             correct status names, added reporting of channel statuses
        //                             and modified start time of readings to be the interval's
        //                             start time and not the end time.
        // 05/04/16 PGH 4.50.261 680037 Added DST status
        //
        private void WriteReadings(XmlNode ChannelNode, LPChannel Channel)
        {
            XmlAttribute newAttribute = null;
            List<LPInterval> lpIntervals = m_LPData.Intervals;

            // Write the <ContiguousIntervalSets> tag
            XmlNode ContiguousDataSets = m_xmlDoc.CreateElement(CONTIGUOUSSETSTAG);
            ChannelNode.AppendChild(ContiguousDataSets);

            // Write the <ContiguousIntervalSet> tag
            XmlNode ContiguousDataSet = m_xmlDoc.CreateElement(CONTIGUOUSSETTAG);
            newAttribute = m_xmlDoc.CreateAttribute(NUMBEROFREADINGSTAG);
            newAttribute.Value = LPData.NumberIntervals.ToString(CultureInfo.InvariantCulture);
            ContiguousDataSet.Attributes.Append(newAttribute);
            ContiguousDataSets.AppendChild(ContiguousDataSet);

            // Write the <TimePerid> tag
            XmlNode TimePeriod = m_xmlDoc.CreateElement(TIMEPERIODTAG);
            newAttribute = m_xmlDoc.CreateAttribute(ENDTIMETAG);
            newAttribute.Value = m_LPData.EndTime.ToString("s", CultureInfo.InvariantCulture);
            TimePeriod.Attributes.Append(newAttribute);
            newAttribute = m_xmlDoc.CreateAttribute(STARTTIMETAG);
            DateTime dtLPStartTime = m_LPData.StartTime.AddMinutes((double)(m_LPData.IntervalDuration * -1.0));
            newAttribute.Value = dtLPStartTime.ToString("s", CultureInfo.InvariantCulture);
            TimePeriod.Attributes.Append(newAttribute);
            ContiguousDataSet.AppendChild(TimePeriod);

            // Write the <Readings> tag
            XmlNode Readings = m_xmlDoc.CreateElement(READINGSTAG);
            ContiguousDataSet.AppendChild(Readings);

            XmlNode Reading = null;
            XmlNode ReadingStatus = null;
            XmlNode UnencodedStatus = null;
            XmlNode StatusCodes = null;
            XmlNode Code = null;

            // Now we will go through the Load Profile to write the <Reading> tags
            for (int iNumIntervals = 0; iNumIntervals < LPData.NumberIntervals; iNumIntervals++)
            {
                // Write the <Reading> tag
                Reading = m_xmlDoc.CreateElement(READINGTAG);
                newAttribute = m_xmlDoc.CreateAttribute(VALUETAG);
                newAttribute.Value = lpIntervals[iNumIntervals].Data[Channel.ChannelNumber].ToString(CultureInfo.InvariantCulture);
                Reading.Attributes.Append(newAttribute);
                Readings.AppendChild(Reading);

                if (lpIntervals[iNumIntervals].HasIntervalStatus || lpIntervals[iNumIntervals].HasChannelStatuses[Channel.ChannelNumber])
                {
                    string strStatuses = lpIntervals[iNumIntervals].IntervalStatus
                        + lpIntervals[iNumIntervals].ChannelStatuses[Channel.ChannelNumber];
                    strStatuses = strStatuses.ToUpper(CultureInfo.InvariantCulture);

                    // Write the <ReadingStatus> tag
                    ReadingStatus = m_xmlDoc.CreateElement(READINGSTATUSTAG);
                    Reading.AppendChild(ReadingStatus);

                    // Write the <Unencoded Status> tag
                    UnencodedStatus = m_xmlDoc.CreateElement(UNENCODEDSTATUSTAG);
                    newAttribute = m_xmlDoc.CreateAttribute(SOURCEVALIDATIONTAG);
                    newAttribute.Value = NOVALIDATION;
                    UnencodedStatus.Attributes.Append(newAttribute);
                    ReadingStatus.AppendChild(UnencodedStatus);
                    
                    // Write the <StatusCodes> tag
                    StatusCodes = m_xmlDoc.CreateElement(STATUSCODESTAG);
                    UnencodedStatus.AppendChild(StatusCodes);

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
        }
        #endregion

        #region Members

        private LoadProfileData m_LPData = null;

        #endregion
    }
}
