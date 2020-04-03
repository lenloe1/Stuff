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
//                        Copyright © 2008 - 2015
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Globalization;
using Itron.Metering.DeviceDataTypes;
using System.Windows.Forms;

namespace Itron.Metering.Datafiles
{

    /// <summary>
    /// Class representing the Ancillary Data stored in the CRF file
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue#    Description
    //  -------- --- -------  ------    -------------------------------------------
    //  05/26/15 PGH 4.50.125 RTT556298 Created
    //
    public class CRFAncillaryrDataType : CRFDataType
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
        /// Constructor used when reading the Ancillary Data out of the CRF File
        /// </summary>
        public CRFAncillaryrDataType()
            : base()
        {
        }

        /// <summary>
        /// Constructor used when writing a CRF file
        /// </summary>
        /// <param name="Records">Counter Data to write to CRF file</param>
        public CRFAncillaryrDataType(List<AncillaryDataRecord> Records)
            : base()
        {
            m_Records = Records;
        }

        #endregion

        /// <summary>
        /// Method that handles the writing of the Counter Data to the XML file.
        /// </summary>
        /// <param name="xmlDoc">The XmlDocument that this data is going into</param>
        /// <param name="ESN">Electronic Serial Number of the meter</param>
        /// <returns>The outcome of writing the current register data to the file</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue#    Description
        //  -------- --- -------  ------    -------------------------------------------
        //  05/27/15 PGH 4.50.125 RTT556298 Created

        public override CreateCRFResult Write(XmlDocument xmlDoc, string ESN)
        {
            XmlNode ChannelsNode;
            m_Result = base.Write(xmlDoc, ESN);

            try
            {
                if (m_Records.Count > 0)
                {

                    ChannelsNode = BuildChannelsNode();

                    // Process the records one at a time
                    foreach (AncillaryDataRecord Record in m_Records)
                    {
                        WriteReading(ChannelsNode, Record.Description, Record.Value, Record.ReadingTime);
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

        #region Private Methods

        /// <summary>
        /// Retrieves the Channels node from the CRF file and creates it if it
        /// is not there
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue#     Description
        //  -------- --- -------  ------    -------------------------------------------
        //  05/27/15 PGH 4.50.125 RTT556298  Taken from CRFRegDataType
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

        /// <summary>
        /// Writes the readings data to the CRF file
        /// </summary>
        /// <param name="ChannelsNode">Serves as the root node to attach the child nodes to</param>
        /// <param name="strDescription">The descripton of the reading</param>
        /// <param name="strValue">The value of the reading</param>
        /// <param name="TimeOfReading">The time that the self read occurred</param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue#    Description
        //  -------- --- -------  ------    -------------------------------------------
        //  05/27/15 PGH 4.50.125 RTT556298 Created
        
        private void WriteReading(XmlNode ChannelsNode, string strDescription, string strValue, DateTime TimeOfReading)
        {
            XmlAttribute newAttribute = null;

            // Write the <Channel> tag
            XmlNode ChannelNode = m_xmlDoc.CreateElement(CHANNELTAG);

            newAttribute = m_xmlDoc.CreateAttribute(READINGSINPULSETAG);
            newAttribute.Value = FALSE;
            ChannelNode.Attributes.Append(newAttribute);

            newAttribute = m_xmlDoc.CreateAttribute(ISREGISTERTAG);
            newAttribute.Value = FALSE;
            ChannelNode.Attributes.Append(newAttribute);

            ChannelsNode.AppendChild(ChannelNode);

            // Write the <ChannelID> tag
            XmlNode ChannelID = m_xmlDoc.CreateElement(CHANNELIDTAG);
            newAttribute = m_xmlDoc.CreateAttribute(CHANNELIDTYPETAG);
            newAttribute.Value = m_ESN + ":" + ModifyDescription(strDescription);
            ChannelID.Attributes.Append(newAttribute);
            ChannelNode.AppendChild(ChannelID);

            // Add the reading
            XmlNode Readings = m_xmlDoc.CreateElement(READINGSTAG);
            ChannelNode.AppendChild(Readings);

            XmlNode Reading = m_xmlDoc.CreateElement(READINGTAG);
            newAttribute = m_xmlDoc.CreateAttribute(VALUETAG);

            newAttribute.Value = strValue.ToString(CultureInfo.CurrentCulture);

            Reading.Attributes.Append(newAttribute);

            // Write the time of reading attribute
            newAttribute = m_xmlDoc.CreateAttribute(READINGTIMETAG);

            newAttribute.Value = TimeOfReading.ToString("s", CultureInfo.InvariantCulture);
            Reading.Attributes.Append(newAttribute);

            Readings.AppendChild(Reading);
        }

        #endregion


        #region Members

        private List<AncillaryDataRecord> m_Records;

        #endregion

    }

     /// <summary>
    /// Ancillary Data Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue#    Description
    //  -------- --- -------  ------    -------------------------------------------
    //  05/26/15 PGH 4.50.125 RTT556298 Created
    //
    public class AncillaryDataRecord
    {

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public AncillaryDataRecord()
        {
            m_ReadingTime = DateTime.MinValue;
            m_strValue = "";
            m_strDescription = "";
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Time when value was read
        /// </summary>
        public DateTime ReadingTime
        {
            get
            {
                return m_ReadingTime;
            }
            set
            {
                m_ReadingTime = value;
            }
        }

        /// <summary>
        /// The value read
        /// </summary>
        public string Value
        {
            get
            {
                return m_strValue;
            }
            set
            {
                m_strValue = value;
            }
        }

        /// <summary>
        /// Description of the value read
        /// </summary>
        public string Description
        {
            get
            {
                return m_strDescription;
            }
            set
            {
                m_strDescription = value;
            }
        }

        #endregion


        #region Members

        private DateTime m_ReadingTime;
        private string m_strValue;
        private string m_strDescription;

        #endregion
    }
}
