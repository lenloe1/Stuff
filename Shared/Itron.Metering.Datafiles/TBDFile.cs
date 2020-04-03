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
//                              Copyright © 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// The type of TBD File
    /// </summary>
    public enum TBDType
    {
        /// <summary>
        /// ANSI C12.19 standard defined
        /// </summary>
        Standard,
        /// <summary>
        /// Manufacturer defined
        /// </summary>
        Manufacturer
    }

    /// <summary>
    /// Class to manage information about an OpenWay Tools specific TBD file.
    /// </summary>
    public class OpenWayToolsTBDFile
    {
        #region Definitions

        private const string OWT_VERSION = "OWTVersion";
        private const string DATE_CREATED = "DateCreated";
        private const string TBD_TYPE = "TBDType";
        private const string TABLE_DEFINITIONS = "TableDefinitions";
        private const string DEVICE_CLASS = "DeviceClass";
        private const string SIGNATURE = "Signature";
        private const string STANDARD = "STD";
        private const string MANUFACTURER = "MFG";

        #endregion


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filePath">The full path of the TBD file.</param>
        public OpenWayToolsTBDFile(string filePath)
        {
            m_Path = filePath;

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            m_TBDRoot = doc.DocumentElement;


        }

        /// <summary>
        /// Determines whether or not this file is a TBD file for use with OpenWay Tools.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsOpenWayToolsTBDFile(string filePath) 
        {
            bool IsOWTTBD = true;

            try
            {
                string[] Nodes = new string[] { OWT_VERSION, DATE_CREATED, TBD_TYPE, TABLE_DEFINITIONS};
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);

                XmlNode root = doc.DocumentElement;

                foreach (string Node in Nodes)
                {
                    if (null == root.SelectSingleNode(Node))
                    {
                        IsOWTTBD = false;
                        break;
                    }
                }
            }
            catch
            {
                IsOWTTBD = false;
            }

            return IsOWTTBD; 
        }

        /// <summary>
        /// The name of the file.
        /// </summary>
        public string FileName 
        { 
            get 
            {
                string Name = "";

                try
                {
                    FileInfo TBDInfo = new FileInfo(m_Path);

                    if (null != TBDInfo)
                    {
                        Name = TBDInfo.Name;
                    }
                }
                catch { /*Continue*/}

                return Name; 
            } 
        }

        /// <summary>
        /// Gets the OpenWay Tools file version.
        /// </summary>
        public ushort FileVersion 
        { 
            get 
            { 
                ushort Version = 0;
                XmlNode VersionNode = null;

                try
                {
                    if (null != m_TBDRoot)
                    {
                        VersionNode = m_TBDRoot.SelectSingleNode(OWT_VERSION);
                    }

                    if (null != VersionNode && null != VersionNode.InnerText)
                    {
                        Version = ushort.Parse(VersionNode.InnerText, CultureInfo.InvariantCulture);
                    }
                }
                catch { /*Continue*/}

                return Version; 
            } 
        }

        /// <summary>
        /// Gets the date this file was created.
        /// </summary>
        public DateTime DateCreated
        {
            get
            {
                DateTime Date = DateTime.MinValue;
                XmlNode DateNode = null;

                try
                {
                    if (null != m_TBDRoot)
                    {
                        DateNode = m_TBDRoot.SelectSingleNode(DATE_CREATED);
                    }

                    if (null != DateNode && null != DateNode.InnerText)
                    {
                        Date = DateTime.Parse(DateNode.InnerText, CultureInfo.InvariantCulture);
                    }
                }
                catch { /*Continue*/}

                return Date;
            }
        }

        /// <summary>
        /// Gets the type of TBD file { STD | MFG }.
        /// </summary>
        public TBDType Type
        {
            get
            {
                TBDType Type = TBDType.Standard;
                XmlNode TypeNode = null;

                try
                {
                    if (null != m_TBDRoot)
                    {
                        TypeNode = m_TBDRoot.SelectSingleNode(TBD_TYPE);
                    }

                    if (null != TypeNode && null != TypeNode.InnerText)
                    {
                        if (MANUFACTURER == TypeNode.InnerText)
                        {
                            Type = TBDType.Manufacturer;
                        }
                    }
                }
                catch { /*Continue*/}

                return Type;
            }
        }

        /// <summary>
        /// Gets the device class this file is valid for.
        /// </summary>
        public string DeviceClass
        {
            get
            {
                string DeviceClass = "";
                XmlNode DeviceClassNode = null;

                try
                {
                    if (null != m_TBDRoot)
                    {
                        DeviceClassNode = m_TBDRoot.SelectSingleNode(DEVICE_CLASS);
                    }

                    if (null != DeviceClassNode && null != DeviceClassNode.InnerText)
                    {
                        DeviceClass = DeviceClassNode.InnerText;
                    }
                }
                catch { /*Continue*/}

                return DeviceClass;
            }
        }

        /// <summary>
        /// Gets all lines of table definitions.
        /// </summary>
        public IEnumerable<string> TableDefinitions
        {
            get
            {
                IEnumerable<string> DefinitionLines = null;
                string Definitions = "";
                XmlNode DefinitionsNode = null;

                try
                {
                    if (null != m_TBDRoot)
                    {
                        DefinitionsNode = m_TBDRoot.SelectSingleNode(TABLE_DEFINITIONS);
                    }

                    if (null != DefinitionsNode && null != DefinitionsNode.InnerText)
                    {
                        Definitions = DefinitionsNode.InnerText;
                    }

                    DefinitionLines = Definitions.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                }
                catch { /*Continue*/}

                return DefinitionLines;
            }
        }


        /// <summary>
        /// Determines whether or not this file's signature is valid.
        /// </summary>
        public bool HasValidSignature { get { return true; } }


        #region Members

        private string m_Path = "";
        private XmlNode m_TBDRoot = null;

        #endregion


    }
}
