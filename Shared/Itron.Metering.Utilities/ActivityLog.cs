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
//                              Copyright © 2006 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Represents a Log Item for the an Activity Log.   The class contains
    /// the elements that go into an entry of the Activity Log, which 
    /// are Date/Time, MeterType, UnitID, MeterSerialNumber, Event, and Data. 
    /// The class contains all functionality needed to add an activity Log 
    /// item.
    /// </summary>
    /// <example>
    /// The strucure of the activity log is simulated below
    /// <code>
    /// ActivityLog
    ///		Entry
    ///			DateTime--Tuesday, July 05, 2005 11:36:18 AM--/DateTime
    ///			MeterType--SENTINEL--/MeterType
    ///			UnitID--Meter ID 1--/UnitID
    ///			MeterSerialNumber--123-456-789--/MeterSerialNumber
    ///			Event--Reconfigure Failed--/Event
    ///			Data--Custom 1--/Data
    ///		/Entry
    /// /ActivityLog
    /// </code>
    /// </example>	
    public class ActivityLogEntry
    {
        #region Constants
#if(!WindowsCE)
        private const string XSL_FILE = "ItronActivityLog.xsl";
#else
		private const string XSL_FILE = "ItronCEActivityLog.xsl";
#endif
        private const string DATE_FORMAT = "F";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor - Sets the event time to the current time.  Sets the
        /// other member variables to their empty values.
        /// </summary>
        /// <example>
        /// <code>
        /// CEActivityLog x = new CEActivityLog();
        /// </code>
        /// </example>
        /// <remarks>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/16/07 mah 8.00.00 Created
        /// </remarks>
        public ActivityLogEntry(string strApplication)
        {
            //Initialize member variables
            m_dtmEventTime = DateTime.Now;
            m_strEvent = "";
            m_strData = "";
            m_strSerialNumber = "";
            m_strUnitID = "";
            m_strMeterType = "";

            m_strApplication = strApplication;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds an XmlNode to the XmlDocument for the Activity Log.  Gets the 
        /// application directory from the registry and then opens or creates 
        /// the Activity Log xml file.  Appends the new entry to the XmlDocument 
        /// An XmlException is thrown if there is a problem with loading the xml 
        /// file.  
        /// </summary>
        /// <example>
        /// <code>
        /// CCustSchedItem x = new CCustSchedItem();
        /// x.Event = "Reconfigure Failed";
        /// x.Data = "Custom 1";
        /// x.MeterType = "SENTINEL";
        /// x.UnitID = "Meter ID 1";
        /// x.SerialNumber = "123-456-789";
        ///
        /// x.AddItem();
        /// </code>
        /// </example>
        /// <remarks>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/14/05 rrr 7.13.00 N/A	Creation of class 
        /// 04/25/06 mrj 7.30.00        Updated for HH-Pro
        /// 01/15/07 mah 8.00.00 Changed registry access class from CE version
        /// </remarks>
        public void Add()
        {
            //Local variables
            XmlNode xmlnodeRoot;
            XmlNode xmlnodeTemp;
            bool blnFileCreated = false;

            //Get the data directory for the log and get the log file
            m_strDirectory = CRegistryHelper.GetDataDirectory(m_strApplication);
            blnFileCreated = GetLogFile();

            //Create the xml document and load the xml file if need be
            if (!blnFileCreated)
            {
                m_xmldomLog = new System.Xml.XmlDocument();
                m_xmldomLog.Load(m_strXmlFile);
            }

            //Create the xml node to be added
            ConstructLogItemNode();

            //get the root node to add the xml node to
            xmlnodeRoot = m_xmldomLog.LastChild;
            xmlnodeTemp = xmlnodeRoot;

            //Append the xml node to the root node
            if (null != xmlnodeRoot)
            {
                xmlnodeTemp = xmlnodeRoot.AppendChild(m_xmlnodeLogItem);
            }

            //Save the xml documents and set variables to null
            m_xmldomLog.Save(m_strXmlFile);
            m_xmldomLog = null;
            xmlnodeTemp = null;
            xmlnodeRoot = null;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Property to get and set the date and time that the event occurred.
        /// Returned as a DateTime object
        /// </summary>
        /// <example>
        /// <code>
        /// CCustSchedItem x = new CCustSchedItem();
        /// x.EventTime = DateTime.Now;
        /// DateTime y = x.EventTime;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/14/05 rrr 7.13.00 N/A	Creation of class  
        public DateTime EventTime
        {
            get
            {
                return m_dtmEventTime;
            }
            set
            {
                m_dtmEventTime = value;
            }
        }

        /// <summary>
        /// Property that gets and sets the meter type for the log item
        /// </summary>
        /// <example>
        /// <code>
        /// CCustSchedItem x = new CCustSchedItem();
        /// x.MeterType = "SENTINEL";
        /// string y = x.MeterType;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/14/05 rrr 7.13.00 N/A	Creation of class  
        public string MeterType
        {
            get
            {
                return m_strMeterType;
            }
            set
            {
                m_strMeterType = value;
            }
        }

        /// <summary>
        /// Property that gets and sets the unit id for the log item
        /// </summary>
        /// <example>
        /// <code>
        /// CCustSchedItem x = new CCustSchedItem();
        /// x.UnitID = "SENTINEL";
        /// string y = x.UnitID;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/14/05 rrr 7.13.00 N/A	Creation of class 
        public string UnitID
        {
            get
            {
                return m_strUnitID;
            }
            set
            {
                m_strUnitID = value;
            }
        }

        /// <summary>
        /// Property that gets and sets the meter serial number for the log item
        /// </summary>
        /// <example>
        /// <code>
        /// CCustSchedItem x = new CCustSchedItem();
        /// x.SerialNumber = "SENTINEL";
        /// string y = x.SerialNumber;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/14/05 rrr 7.13.00 N/A	Creation of class 
        public string SerialNumber
        {
            get
            {
                return m_strSerialNumber;
            }
            set
            {
                m_strSerialNumber = value;
            }
        }

        /// <summary>
        /// Property that gets and sets the event for the log item.  This 
        /// property should not be blank when gotten.  If the string is blank
        /// than the empty string is used and then handled by the xsl when
        /// ready to display.
        /// </summary>
        /// <example>
        /// <code>
        /// CCustSchedItem x = new CCustSchedItem();
        /// x.Event = "Logon Success";
        /// string y = x.Event;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/14/05 rrr 7.13.00 N/A	Creation of class
        /// 04/25/06 mrj 7.30.00        Updated for HH-Pro
        ///
        public string Event
        {
            get
            {
                return m_strEvent;
            }
            set
            {
                m_strEvent = value;
            }
        }

        /// <summary>
        /// Property that gets and sets the data for the log item
        /// </summary>
        /// <example>
        /// <code>
        /// CCustSchedItem x = new CCustSchedItem();
        /// x.data = "Custom Schedule: CYCLE 01";
        /// string y = x.Data;
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/14/05 rrr 7.13.00 N/A	Creation of class
        /// 04/25/06 mrj 7.30.00        Updated for HH-Pro
        ///   
        public string Data
        {
            get
            {
                return m_strData;
            }
            set
            {
                m_strData = value;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Takes the attributs of a log entry and puts them into an XmlNode.  
        /// The node is then returned or null if the node was not created.  
        /// An ArgumentException is thrown if an invalid node is created.  
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/14/05 rrr 7.13.00 N/A	Creation of class  
        private void ConstructLogItemNode()
        {
            //Local variables
            XmlNode xmlnodeItem;
            XmlNode xmlnodeTemp;

            //Create the xml node for the new Log Entry
            m_xmlnodeLogItem = m_xmldomLog.CreateNode(XmlNodeType.Element,
                ActivityLog.NODE_ENTRY, "");

            //If the xml node is not null then add it children nodes
            if (null != m_xmlnodeLogItem)
            {
                //Add the node for the time
                xmlnodeItem = m_xmldomLog.CreateNode(XmlNodeType.Element,
                                                      ActivityLog.NODE_TIME,
                                                      "");
                xmlnodeItem.InnerText = EventTime.ToString(DATE_FORMAT, CultureInfo.InvariantCulture);
                xmlnodeTemp = m_xmlnodeLogItem.AppendChild(xmlnodeItem);

                if (null != xmlnodeTemp)
                {
                    //Add the node for the meter type
                    xmlnodeItem = m_xmldomLog.CreateNode(XmlNodeType.Element,
                                                          ActivityLog.NODE_METERTYPE,
                                                          "");
                    xmlnodeItem.InnerText = m_strMeterType;
                    xmlnodeTemp = m_xmlnodeLogItem.AppendChild(xmlnodeItem);
                }
                if (null != xmlnodeTemp)
                {
                    //Add the node for the Unit ID
                    xmlnodeItem = m_xmldomLog.CreateNode(XmlNodeType.Element,
                                                          ActivityLog.NODE_UNITID,
                                                          "");
                    xmlnodeItem.InnerText = m_strUnitID;
                    xmlnodeTemp = m_xmlnodeLogItem.AppendChild(xmlnodeItem);
                }
                if (null != xmlnodeTemp)
                {
                    //Add the node for the Meter Serial Number
                    xmlnodeItem = m_xmldomLog.CreateNode(XmlNodeType.Element,
                                                          ActivityLog.NODE_SERIAL,
                                                          "");
                    xmlnodeItem.InnerText = m_strSerialNumber;
                    xmlnodeTemp = m_xmlnodeLogItem.AppendChild(xmlnodeItem);
                }
                if (null != xmlnodeTemp)
                {
                    //Add the node for the Event
                    xmlnodeItem = m_xmldomLog.CreateNode(XmlNodeType.Element,
                                                          ActivityLog.NODE_EVENT,
                                                          "");
                    xmlnodeItem.InnerText = m_strEvent;
                    xmlnodeTemp = m_xmlnodeLogItem.AppendChild(xmlnodeItem);
                }
                if (null != xmlnodeTemp)
                {
                    //Add the node for the Event Data
                    xmlnodeItem = m_xmldomLog.CreateNode(XmlNodeType.Element,
                                                          ActivityLog.NODE_DATA,
                                                          "");
                    xmlnodeItem.InnerText = m_strData;
                    xmlnodeTemp = m_xmlnodeLogItem.AppendChild(xmlnodeItem);
                }
                else
                {
                    m_xmlnodeLogItem = null;
                }
            }
        }


        /// <summary>
        /// Private method used to check if the xml and xsl files exist
        /// </summary>
        /// <returns>
        /// returns true if fill created, false if file not
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/14/05 rrr 7.13.00 N/A	Creation of class 
        private bool GetLogFile()
        {
            //Create file infor objects for the xml and xsl files
            FileInfo objFile = new FileInfo(m_strDirectory + ActivityLog.XML_FILE);
            FileInfo objXslFile = new FileInfo(m_strDirectory + XSL_FILE);

            //if the xsl file exists than set its name to the member variable
            if (objXslFile.Exists)
            {
                m_strXslFile = XSL_FILE;
            }

            //if the xml file does not exist than create it otherwise set the
            //member variable
            if (!objFile.Exists)
            {
                Create();
                return true;
            }
            else
            {
                m_strXmlFile = objFile.FullName;
                return false;
            }
        }

        /// <summary>
        /// Private method used to create the xml file for the Activity Log if it
        /// does not already exist. 
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/14/05 rrr 7.13.00 N/A	Creation of class 
        private void Create()
        {
            //Initialize lical variables
            XmlNode xmlnodeRoot = null;
            XmlNode xmlnodeTemp = null;

            m_xmldomLog = new XmlDocument();

            //Add the processing instructions to the xml file
            AddProcessingInstruction("xml", "version='1.0'");

            if (!String.IsNullOrEmpty(  m_strXslFile))
            {
                AddProcessingInstruction("xml-stylesheet", "type='text/xsl' href='" + m_strXslFile + "'");
            }
            else
            {
                AddProcessingInstruction("xml-stylesheet", "type='text/xsl'");
            }

            //Create the node to act as the root of all entries and append it
            xmlnodeRoot = m_xmldomLog.CreateNode(XmlNodeType.Element,
                                                  ActivityLog.MAIN_NODE,
                                                  "");
            xmlnodeTemp = m_xmldomLog.AppendChild(xmlnodeRoot);

            if (null != xmlnodeTemp)
            {
                m_strXmlFile = m_strDirectory + ActivityLog.XML_FILE;
                m_xmldomLog.Save(m_strXmlFile);
            }

            //Set all local variables to null
            xmlnodeRoot = null;
            xmlnodeTemp = null;
        }

        /// <summary>
        /// Add processing instructions to the xml file being created
        /// </summary>
        /// <param name="strTarget">
        /// The target to put the data into
        /// </param>
        /// <param name="strData">
        /// The date to put in the given target
        /// </param>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/14/05 rrr 7.13.00 N/A	Creation of class  
        private void AddProcessingInstruction(string strTarget, string strData)
        {
            //Local variables
            XmlProcessingInstruction xmlInstruction = null;
            XmlNode xmlnodeProcessing = null;

            //Create the xml processing instruction and append to the document
            if (null != m_xmldomLog)
            {
                xmlInstruction = m_xmldomLog.CreateProcessingInstruction(strTarget, strData);
                xmlnodeProcessing = xmlInstruction;
                m_xmldomLog.AppendChild(xmlnodeProcessing);
            }
        }

        #endregion

        #region Members
        //Private member variables for CEActivityLog
        private string m_strEvent;
        private string m_strSerialNumber;
        private string m_strUnitID;
        private string m_strMeterType;
        private string m_strData;
        private XmlNode m_xmlnodeLogItem;
        private DateTime m_dtmEventTime;
        private string m_strXmlFile;
        private string m_strXslFile;
        private string m_strDirectory;
        private string m_strApplication;
        private XmlDocument m_xmldomLog;
        #endregion
    }

    /// <summary>
    /// This class represents an activity log entry for the FieldPro activity log
    /// </summary>
    /// <remarks>
    /// Revision History
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 01/16/07 mah 8.00.00 Created
    /// </remarks>
    public class FieldProActivityLogEntry : ActivityLogEntry
    {
        /// <summary>
        /// Constructor - Sets the event time to the current time.  Sets the
        /// other member variables to their empty values.
        /// </summary>
        /// <remarks>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/16/07 mah 8.00.0000 N/A	Creation of class 
        /// </remarks>
        public FieldProActivityLogEntry() : base("FieldPro" )
        {
        }

    }

	/// <summary>
	/// This class represents an activity log entry for the HH-Pro activity log
	/// </summary>
	//  Revision History
	//  MM/DD/YY Who Version Issue# Description
	//  -------- --- ------- ------ ---------------------------------------------
	//  03/01/07 mrj 8.00.16		Created
	//  
	public class HHProActivityLogEntry : ActivityLogEntry
	{
		/// <summary>
		/// Constructor - Sets the event time to the current time.  Sets the
		/// other member variables to their empty values.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  03/01/07 mrj 8.00.16		Created
		//  
		public HHProActivityLogEntry()
			: base("HH-Pro")
		{
		}

	}

    /// <summary>
    /// This class represents the whole activity log file.  This class provides read only 
    /// access to the activity log entries.  This class should not be used to write activity
    /// log entries.
    /// </summary>
    /// <remarks>
    /// Revision History
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 01/16/07 mah 8.00.00 Created
    /// </remarks>
    public class ActivityLog
    {
        #region Constants
        //string constants to be useed within the code
        
        /// <summary>
        /// 
        /// </summary>
        public const string NODE_ENTRY = "Entry";
        /// <summary>
        /// 
        /// </summary>
        public const string NODE_TIME = "DateTime";
        /// <summary>
        /// 
        /// </summary>
        public const string NODE_METERTYPE = "MeterType";
        /// <summary>
        /// 
        /// </summary>
        public const string NODE_UNITID = "UnitID";
        /// <summary>
        /// 
        /// </summary>
        public const string NODE_SERIAL = "MeterSerialNumber";
        /// <summary>
        /// 
        /// </summary>
        public const string NODE_EVENT = "Event";
        /// <summary>
        /// 
        /// </summary>
        public const string NODE_DATA = "Data";
        /// <summary>
        /// 
        /// </summary>
#if(!WindowsCE)
        public const string MAIN_NODE = "ActivityLog";
#else
		public const string MAIN_NODE = "CEActivityLog";
#endif

        private const string DATE_FORMAT = "F";

        /// <summary>
        /// The name of activity log file
        /// </summary>
#if(!WindowsCE)
        public const string XML_FILE = "ItronActivityLog.xml";
#else
		public const string XML_FILE = "ItronCEActivityLog.xml";
#endif
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/16/07 mah 8.00.00 Created
        /// </remarks>
        public ActivityLog()
        {
        }

        /// <summary>
        /// This method reads all of the entries in the activity log and returns
        /// them as a generic list.  
        /// </summary>
        /// <param name="strApplication">This string is used to identify the file to read. 
        /// The assumption is that different applications will be able to generate activity
        /// logs and that each file can be looked up using the application name
        /// as a registry key
        /// </param>
        /// <returns>A generic list containing activity log items.  A value of NULL will be returned
        /// if the file cannot be found.  If the file does exist, but is unreadable, the returned list
        /// will be empty
        /// </returns>
        /// <remarks>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/16/07 mah 8.00.00 Created
        /// </remarks>
        public static List<ActivityLogEntry> Read(String strApplication)
        {
            //Local variables
            XmlNodeList xmlEntryNodes;
            // XmlNode xmlnodeTemp;

            //Get the data directory for the log and get the log file
            String strDirectory = CRegistryHelper.GetDataDirectory( strApplication);

            //Create file info objects for the xml file to allow us to see if the activity log exists
            FileInfo objFile = new FileInfo(strDirectory + XML_FILE);

            //if the xml file does not exist than create it otherwise set the
            //member variable
            if (!objFile.Exists)
            {
                return null;
            }
            else
            {
                List<ActivityLogEntry> logEventList = new List<ActivityLogEntry>();

                String strXmlFile = objFile.FullName;

                XmlDocument xmldomLog = new System.Xml.XmlDocument();
                xmldomLog.Load( strXmlFile);

                xmlEntryNodes = xmldomLog.GetElementsByTagName( NODE_ENTRY );

                foreach (XmlNode xmlLogEntry in xmlEntryNodes)
                {
                    ActivityLogEntry logEntry = new ActivityLogEntry(strApplication);
                    
                    XmlNode xmlEventTime = xmlLogEntry.SelectSingleNode(NODE_TIME);
                    if (null != xmlEventTime)
                    {
                        logEntry.EventTime = DateTime.Parse(xmlEventTime.InnerText, CultureInfo.InvariantCulture );
                    }

                    XmlNode xmlMeterType = xmlLogEntry.SelectSingleNode(NODE_METERTYPE);
                    if (null != xmlMeterType)
                    {
                        logEntry.MeterType = xmlMeterType.InnerText;
                    }

                    XmlNode xmlMeterID = xmlLogEntry.SelectSingleNode(NODE_UNITID);
                    if (null != xmlMeterID)
                    {
                        logEntry.UnitID = xmlMeterID.InnerText;
                    }

                    XmlNode xmlMeterSerialNumber = xmlLogEntry.SelectSingleNode(NODE_SERIAL);
                    if (null != xmlMeterSerialNumber)
                    {
                        logEntry.SerialNumber  = xmlMeterSerialNumber.InnerText;
                    }

                    XmlNode xmlEvent = xmlLogEntry.SelectSingleNode(NODE_EVENT);
                    if (null != xmlEvent)
                    {
                        logEntry.Event = xmlEvent.InnerText;
                    }

                    XmlNode xmlEventData = xmlLogEntry.SelectSingleNode(NODE_DATA);
                    if (null != xmlEventData)
                    {
                        logEntry.Data = xmlEventData.InnerText;
                    }

                    logEventList.Add(logEntry);
                }
                
                return logEventList;
            }
        }

    }
}
