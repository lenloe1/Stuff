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
//                              Copyright © 2009
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Device;

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// Class representing the Gas Consumption Data in the CRF file
    /// </summary>
    public class CRFGasDataType : CRFDataType
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
        private const string MARKETTYPETAG = "MarketType";
        private const string CHANNELIDTYPETAG = "EndPointUOMID";
        private const string VALUETAG = "Value";
        private const string SOURCEVALIDATIONTAG = "SourceValidation";

        //Attribute Values
        private const string NOVALIDATION = "NV";
        private const string GAS = "Gas";
        private const string CCF = "Gas Quantity (CCF)";

        #endregion

        #region Public Methods

        /// <summary>
        /// Default Constructor for reading the Gas Consumption data out of the CRF file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/09 AF  2.30.04        Created
        //
        public CRFGasDataType()
            : base()
        {
        }

        /// <summary>
        /// Constructor used for writing the Gas Consumption data to the CRF file.
        /// </summary>
        /// <param name="ClientModules">List of gas modules joined to this meter's network</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/09 AF  2.30.04        Created
        //
        public CRFGasDataType(List<ClientMeter> ClientModules)
        {
            m_ClientMeters = ClientModules;
        }

        /// <summary>
        /// Method that handles the writing of the Gas Consumption Data to the XML file.
        /// </summary>
        /// <param name="xmlDoc">The XmlDocument that this data is going into</param>
        /// <param name="ESN">Electronic Serial Number of the meter</param>
        /// <returns>The outcome of writing the data to the file</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/09 AF  2.30.04        Created
        //  10/02/09 AF  2.30.05        Changed the way the result is handled so that we
        //                              don't falsely claim success
        //  10/06/09 AF  2.30.07        Changed the parameter list for WriteReading()
        //  10/21/09 AF  2.30.12        Moved setting the result to success so that
        //                              having no clients will not cause a failure message
        //                              to appear
        //
        public override CreateCRFResult Write(XmlDocument xmlDoc, string ESN)
        {
            m_Result = base.Write(xmlDoc, ESN);

            try
            {
                // Don't write anything if there are no gas modules joined to this meter
                if (m_ClientMeters.Count > 0)
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

                    for (int iClientIndex = 0; iClientIndex < m_ClientMeters.Count; iClientIndex++)
                    {
                        WriteReading(ChannelsNode, m_ClientMeters[iClientIndex]);
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
        /// Writes the readings data to the CRF file
        /// </summary>
        /// <param name="ChannelsNode">Serves as the root node to attach the child nodes to</param>
        /// <param name="GasModule">The gas module whose consumption data we are writing to the file</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/09 AF  2.30.04        Created
        //  10/06/09 AF  2.30.07        Changed the parameter list - no longer need
        //                              the client number. Also changed the format
        //                              of the time of reading
        //  10/15/09 AF  2.30.10        Commented out ReadingStatus fields - they might not be needed
        //
        private void WriteReading(XmlNode ChannelsNode, ClientMeter GasModule)
        {
            XmlAttribute newAttribute = null;

            // Write the <Channel> tag
            XmlNode ChannelNode = m_xmlDoc.CreateElement(CHANNELTAG);

            newAttribute = m_xmlDoc.CreateAttribute(READINGSINPULSETAG);
            newAttribute.Value = "false";
            ChannelNode.Attributes.Append(newAttribute);

            newAttribute = m_xmlDoc.CreateAttribute(ISREGISTERTAG);
            newAttribute.Value = "true";
            ChannelNode.Attributes.Append(newAttribute);

            newAttribute = m_xmlDoc.CreateAttribute(MARKETTYPETAG);
            newAttribute.Value = GAS;
            ChannelNode.Attributes.Append(newAttribute);

            ChannelsNode.AppendChild(ChannelNode);

            // Write the <ChannelID> tag
            XmlNode ChannelID = m_xmlDoc.CreateElement(CHANNELIDTAG);
            newAttribute = m_xmlDoc.CreateAttribute(CHANNELIDTYPETAG);
            newAttribute.Value = GasModule.MACAddress.ToString("X", CultureInfo.CurrentCulture) 
                                    + ":" + CCF;
            ChannelID.Attributes.Append(newAttribute);
            ChannelNode.AppendChild(ChannelID);

            // Write the consumption reading
            XmlNode Readings = m_xmlDoc.CreateElement(READINGSTAG);
            ChannelNode.AppendChild(Readings);

            XmlNode Reading = m_xmlDoc.CreateElement(READINGTAG);
            newAttribute = m_xmlDoc.CreateAttribute(VALUETAG);
            newAttribute.Value = GasModule.LatestConsumption.Value.ToString("#######0.000", CultureInfo.CurrentCulture);
            Reading.Attributes.Append(newAttribute);

            // Write the time of reading attribute
            newAttribute = m_xmlDoc.CreateAttribute(READINGTIMETAG);
            newAttribute.Value = GasModule.LatestConsumption.Timestamp.ToString("s", CultureInfo.InvariantCulture);

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
        #endregion

        #region Members

        private List<ClientMeter> m_ClientMeters;

        #endregion

    }
}
