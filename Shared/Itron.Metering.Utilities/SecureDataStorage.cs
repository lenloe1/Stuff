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
//                           Copyright © 2009 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.IO;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Stores data in a secure format
    /// </summary>
    public class SecureDataStorage
    {
        #region Constants

        private const string RESERVED_ID = "0";

        /// <summary>ID for the Signed Authorization Encryption Key</summary>
        public const string SIGNED_AUTH_KEY_ID = "1";
        /// <summary>ID for the Signed Authorization Encryption IV</summary>
        public const string SIGNED_AUTH_IV_ID = "2";
        /// <summary>ID for the Signed Authorization Encryption Key</summary>
        public const string RMA_SIGNED_AUTH_KEY_ID = "3";
        /// <summary>ID for the Signed Authorization Encryption IV</summary>
        public const string RMA_SIGNED_AUTH_IV_ID = "4";
        /// <summary>ID for the ZigBee Encryption Key</summary>
        public const string ZIGBEE_KEY_ID = "5";
        /// <summary>ID for the ZigBee Encryption IV</summary>
        public const string ZIGBEE_IV_ID = "6";
        /// <summary>ID for the Replica File Encryption Key</summary>
        public const string REPLICA_KEY_ID = "7";
        /// <summary>ID for the Replica File Encryption IV</summary>
        public const string REPLICA_IV_ID = "8";
        /// <summary>ID for the Replica File Encryption IV</summary>
        public const string OPTICAL_PASSWORD = "9";
        /// <summary>ID for the Replica File Encryption IV</summary>
        public const string SIGNED_AUTHORIZATION_KEY = "10";
        /// <summary>The default file location</summary>
        public static readonly string DEFAULT_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Itron\Metering\OpenWay Shared Data\OpenWayData.xml";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath">The path to the file used to store the data</param>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        public SecureDataStorage(string filePath)
        {
            FileInfo FilePathInfo = new FileInfo(filePath);
            m_FilePath = filePath;

            if (FilePathInfo.Exists == false)
            {
                if (FilePathInfo.Directory.Exists == false)
                {
                    FilePathInfo.Directory.Create();
                }

                // Create the file on the disk
                m_SecureDataFile = new SecureData();
                m_SecureDataFile.SaveAs(m_FilePath);

                //Make sure everyone has access to this file. Existing permissions were forcing user to run FP as administrator.
                DirectorySecurity ds = Directory.GetAccessControl(m_FilePath);
                ds.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                Directory.SetAccessControl(m_FilePath, ds);

                StoreEntropyData();
            }
            else
            {
                //Make sure everyone has access to this file. Existing permissions were forcing user to run FP as administrator.
                DirectorySecurity ds = Directory.GetAccessControl(m_FilePath);
                ds.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                Directory.SetAccessControl(m_FilePath, ds);

                // Load the existing file
                m_SecureDataFile = new SecureData();
                m_SecureDataFile.Load(filePath);
            }
        }

        /// <summary>
        /// Encrypts and stores the specified data in the file
        /// </summary>
        /// <param name="dataID">The ID of the data to store</param>
        /// <param name="secureData">The secure data to store</param>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        public void StoreSecureData(string dataID, byte[] secureData)
        {
            // We don't want them to be able to overwrite our Entropy Data
            if (RESERVED_ID.Equals(dataID) == false)
            {
                if (m_SecureDataFile != null && m_SecureDataFile.SecureItems.Where(d => d.DataID.Equals(dataID)).Count() > 0)
                {
                    m_SecureDataFile.SecureItems.RemoveAll(d => d.DataID.Equals(dataID));
                }

                m_SecureDataFile.SecureItems.Add(new SecureDataItem(dataID, ProtectedData.Protect(secureData, RetrieveEntropyData(), DataProtectionScope.LocalMachine)));
                m_SecureDataFile.Save();
            }
            else
            {
                throw new ArgumentException("The specified ID may not be used", "dataID");
            }
        }

        /// <summary>
        /// Retrieve the Secure Data with the specified ID from the file
        /// </summary>
        /// <param name="dataID">The ID of the secure data to retrieve</param>
        /// <returns>The unencrypted secure data</returns>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        public byte[] RetrieveSecureData(string dataID)
        {
            byte[] Data = null;

            // We don't want them trying to retrieve the Entropy Data
            if (RESERVED_ID.Equals(dataID) == false)
            {
                if (m_SecureDataFile.SecureItems.Where(d => d.DataID.Equals(dataID)).Count() > 0)
                {
                    Data = ProtectedData.Unprotect(m_SecureDataFile.SecureItems.First(d => d.DataID.Equals(dataID)).BinaryData, RetrieveEntropyData(), DataProtectionScope.LocalMachine);
                }
            }

            return Data;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Stores the Entropy Data to use for encrypting the rest of the data stored in the file
        /// </summary>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        private void StoreEntropyData()
        {
            byte[] Entropy = new byte[8];
            byte[] SecureEntropy;
            RNGCryptoServiceProvider EntropyGenerator = new RNGCryptoServiceProvider();
            EntropyGenerator.GetBytes(Entropy);
            
            if (m_SecureDataFile != null && m_SecureDataFile.SecureItems.Where(d => d.DataID.Equals(RESERVED_ID)).Count() > 0)
            {
                m_SecureDataFile.SecureItems.RemoveAll(d => d.DataID.Equals(RESERVED_ID));
            }

            // We are going to store the Entropy data using the basic Local Machine key
            // and everything else will use the secure entropy to protect the data
            SecureEntropy = ProtectedData.Protect(Entropy, null, DataProtectionScope.LocalMachine);

            m_SecureDataFile.SecureItems.Add(new SecureDataItem(RESERVED_ID, SecureEntropy));
            m_SecureDataFile.Save();
        }

        /// <summary>
        /// Retrieves the store Entropy data from the secure file.
        /// </summary>
        /// <returns>The Entropy data from the file</returns>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        private byte[] RetrieveEntropyData()
        {
            byte[] Entropy = null;

            if (m_SecureDataFile.SecureItems.Where(d => d.DataID.Equals(RESERVED_ID)).Count() > 0)
            {
                Entropy = ProtectedData.Unprotect(m_SecureDataFile.SecureItems.First(d => d.DataID.Equals(RESERVED_ID)).BinaryData, null, DataProtectionScope.LocalMachine);
            }

            return Entropy;
        }

        #endregion

        #region Member Variables

        private string m_FilePath;
        private SecureData m_SecureDataFile;

        #endregion
    }

    /// <summary>
    /// Handles the reading and writing of the Secure Data File
    /// </summary>
    internal class SecureData
    {
        #region Constants

        private const string ELEMENT_NAME = "SecureData"; 

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        public SecureData()
        {
            m_FileName = null;
            m_SecureItems = new List<SecureDataItem>();
        }

        /// <summary>
        /// Loads the Secured Data from the specified file
        /// </summary>
        /// <param name="filePath">The path to the file to load.</param>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        public void Load(string filePath)
        {
            m_FileName = filePath;
            XDocument NewDocument = XDocument.Load(m_FileName);

            m_SecureItems.Clear();

            if (NewDocument.Root != null && NewDocument.Root.Name.LocalName == ELEMENT_NAME)
            {
                m_SecureItems = new List<SecureDataItem>();
                XElement SecureDataElement = NewDocument.Element(SecureData.ELEMENT_NAME);

                foreach (XElement CurrentElement in SecureDataElement.Elements(SecureDataItem.ELEMENT_NAME))
                {
                    m_SecureItems.Add(new SecureDataItem(CurrentElement));
                }
            }
            else
            {
                throw new ArgumentException("Not a valid Test Results file", "filePath");
            }
        }

        /// <summary>
        /// Saves the Secured Data to the current file path
        /// </summary>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        public void Save()
        {
            XElement NewElement = new XElement(ELEMENT_NAME);

            foreach (SecureDataItem CurrentItem in m_SecureItems)
            {
                NewElement.Add(CurrentItem.XElement);
            }

            NewElement.Save(m_FileName);
        }

        /// <summary>
        /// Saves the Secured Data to the specified file
        /// </summary>
        /// <param name="filePath">The path to the file to save.</param>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        public void SaveAs(string filePath)
        {
            m_FileName = filePath;
            Save();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of Secure Items stored in the file
        /// </summary>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        public List<SecureDataItem> SecureItems
        {
            get
            {
                return m_SecureItems;
            }
        }

        #endregion

        #region MemberVariables

        private List<SecureDataItem> m_SecureItems;
        private string m_FileName;

        #endregion
    }

    /// <summary>
    /// An individual secure item
    /// </summary>
    internal class SecureDataItem
    {
        #region Constants

        public const string ELEMENT_NAME = "SecureDataItem";

        private const string ATTRIB_ID = "ID";
        private const string ATTRIB_BINARYDATA = "BinaryData";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataID">The ID of the new Secure Data Item</param>
        /// <param name="binaryData">The secure binary data</param>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        public SecureDataItem(string dataID, byte[] binaryData)
        {
            DataID = dataID;
            BinaryData = binaryData;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The XElement object storing the necessary data</param>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        public SecureDataItem(XElement element)
        {
            m_DataID = null;
            m_BinaryData = null;

            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (element.Attribute(ATTRIB_ID) != null)
                {
                    DataID = element.Attribute(ATTRIB_ID).Value;
                }

                if (element.Attribute(ATTRIB_BINARYDATA) != null)
                {
                    string BinaryString = element.Attribute(ATTRIB_BINARYDATA).Value;
                    int Discarded;

                    BinaryData = HexEncoding.GetBytes(BinaryString, out Discarded);
                }
            }
            else
            {
                throw new ArgumentException("The specified element is not valid", "element");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Data ID for the item
        /// </summary>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        public string DataID
        {
            get
            {
                return m_DataID;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value", "DataID may not be null or an empty string");
                }
                else
                {
                    m_DataID = value;
                }
            }
        }

        /// <summary>
        /// Gets the Binary Data for the item
        /// </summary>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        public byte[] BinaryData
        {
            get
            {
                return m_BinaryData;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "BinaryData may not be null");
                }
                else
                {
                    m_BinaryData = value;
                }
            }
        }

        /// <summary>
        /// Gets the XElement object for the secure data item
        /// </summary>
        // Revision History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/15/12 RCG 2.53.50 188198 Created
        
        public XElement XElement
        {
            get
            {
                XElement NewElement = new XElement(ELEMENT_NAME);

                if (String.IsNullOrEmpty(m_DataID) || m_BinaryData == null)
                {
                    // This is an invalid element so we should return null
                    NewElement = null;
                }
                else
                {
                    string BinaryString = "";

                    NewElement.Add(new XAttribute(ATTRIB_ID, m_DataID));

                    foreach (byte CurrentByte in m_BinaryData)
                    {
                        BinaryString += CurrentByte.ToString("X2", CultureInfo.InvariantCulture);
                    }

                    NewElement.Add(new XAttribute(ATTRIB_BINARYDATA, BinaryString));
                }

                return NewElement;
            }
        }

        #endregion

        #region Member Variables

        private string m_DataID;
        private byte[] m_BinaryData;

        #endregion
    }
}
