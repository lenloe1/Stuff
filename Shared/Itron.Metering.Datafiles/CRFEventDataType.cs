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
    /// Class representing the Event Data stored in the CRF file
    /// </summary>
    public class CRFEventDataType : CRFDataType
    {
        #region Constants

        // Node Names
        private const string ROOTNODETAG = "EventDocument";
        private const string EVENTSTAG = "Events";
        private const string EVENTTAG = "Event";
        private const string COLLECTIONSYSTEMIDTAG = "CollectionSystemID";
        private const string OBJECTIDTAG = "ObjectID";
        private const string OBJECTTYPETAG = "ObjectType";
        private const string EVENTTYPETAG = "EventType";
        private const string EVENTDATETIMETAG = "EventDateTime";
        private const string CAPTUREDATETIMETAG = "CaptureDateTime";
        private const string ISHISTORICALTAG = "IsHistorical";

        //Attribute Names
        private const string ENDPOINTIDTAG = "EndpointID";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor used when reading the Event Data out of the CRF File
        /// </summary>
        public CRFEventDataType()
            : base()
        {
        }

        /// <summary>
        /// Constructor used when writing a CRF file
        /// </summary>
        /// <param name="EventsList">Event data to be written to the CRF file</param>
        /// <param name="strExternalSystemID">The External System ID to use</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/30/09 AF  2.30.05        Created
        //
        public CRFEventDataType(List<HistoryEntry> EventsList, string strExternalSystemID)
        {
            m_EventsList = EventsList;
            m_strExternalSystemID = strExternalSystemID;
        }

        /// <summary>
        /// Method that handles the writing of the XML file.
        /// </summary>
        /// <param name="xmlDoc">XML document that the data is going in to</param>
        /// <param name="ESN">The meter's electronic serial number</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/30/09 AF  2.30.05        Created
        //  10/02/09 AF  2.30.05        Changed the way the result is handled so that we
        //                              don't falsely claim success
        //
        public override CreateCRFResult Write(XmlDocument xmlDoc, string ESN)
        {
            m_Result = base.Write(xmlDoc, ESN);

            try
            {
                if (m_EventsList.Count > 0)
                {
                    // Get the root node and write the <Events> tag to it
                    XmlNode testRoot = m_xmlDoc.SelectSingleNode(ROOTNODETAG);
                    XmlNode EventsNode = m_xmlDoc.CreateElement(EVENTSTAG);
                    testRoot.AppendChild(EventsNode);

                    foreach (HistoryEntry histEntry in m_EventsList)
                    {
                        WriteReading(EventsNode, histEntry);
                    }
                }

                // If we reach this point without an exception, we can assume success
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
        /// Writes an individual event to the CRF file
        /// </summary>
        /// <param name="EventsNode">The parent node to append to</param>
        /// <param name="Event">The event data to be written</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/30/09 AF  2.30.05        Created
        //  10/12/09 AF  2.30.09        Updated the EventType field and added 
        //                              IsHistorical attribute
        //  12/14/09 AF  2.30.28 TBD    Needed to modify the ObjectID and ObjectType fields
        //
        private void WriteReading(XmlNode EventsNode, HistoryEntry Event)
        {
            XmlNode EventNode = m_xmlDoc.CreateElement(EVENTTAG);
            EventsNode.AppendChild(EventNode);

            // Write the CollectionSystemID
            XmlNode CollSysIDNode = m_xmlDoc.CreateElement(COLLECTIONSYSTEMIDTAG);
            CollSysIDNode.InnerText = m_strExternalSystemID;
            EventNode.AppendChild(CollSysIDNode);

            // Write the ObjectID (identifies the meter)
            XmlNode ObjIDNode = m_xmlDoc.CreateElement(OBJECTIDTAG);
            ObjIDNode.InnerText = m_ESN;
            EventNode.AppendChild(ObjIDNode);

            // Write the ObjectType
            XmlNode ObjTypeNode = m_xmlDoc.CreateElement(OBJECTTYPETAG);
            ObjTypeNode.InnerText = ENDPOINTIDTAG;
            EventNode.AppendChild(ObjTypeNode);

            // Write the event code
            XmlNode EventTypeNode = m_xmlDoc.CreateElement(EVENTTYPETAG);
            EventTypeNode.InnerText = Event.TranslateEventCodeForMDM();
            EventNode.AppendChild(EventTypeNode);

            // Write the IsHistorical tag
            XmlNode IsHistoricalNode = m_xmlDoc.CreateElement(ISHISTORICALTAG);
            IsHistoricalNode.InnerText = "1";
            EventNode.AppendChild(IsHistoricalNode);

            // Write the event date
            XmlNode EventDateTimeNode = m_xmlDoc.CreateElement(EVENTDATETIMETAG);
            EventDateTimeNode.InnerText = Event.HistoryTime.ToString("s", CultureInfo.InvariantCulture);
            EventNode.AppendChild(EventDateTimeNode);

            // Write the capture date/time
            XmlNode CaptureDateTimeNode = m_xmlDoc.CreateElement(CAPTUREDATETIMETAG);
            CaptureDateTimeNode.InnerText = DateTime.Now.ToString("s", CultureInfo.InvariantCulture);
            EventNode.AppendChild(CaptureDateTimeNode);
        }

        #endregion

        #region Members

        private List<HistoryEntry> m_EventsList;
        private string m_strExternalSystemID;

        #endregion

    }
}
