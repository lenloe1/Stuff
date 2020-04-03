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
//                              Copyright © 2004 - 2008
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Xml;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using Itron.Metering.Utilities;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Class for XML encrypted settings files.
	/// </summary>
	// Revision History
	// MM/DD/YY who Version Issue# Description
	// -------- --- ------- ------ ---------------------------------------------
	// 05/27/08 jrf 1.50.28        Created.
	//
    public class CXMLEncryptedSettings : CXMLSettings
    {

        #region Public Methods

        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="strFile">Default File Name for the XML file.</param>
		/// <param name="strRoot">Root Node in the XML file.</param>
		/// <param name="strMain">Main Node in the XML file for the settings.</param>
		// Revision History
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------------
		// 05/23/08 jrf 1.50.28        Created
		//
		public CXMLEncryptedSettings( string strFile, string strRoot, string strMain )
            :base(strFile, strRoot, strMain)
		{
		}

        /// <summary>
		/// Saves the current XML data to the specified file. All XML data in the file will be overwritten.
		/// </summary>
		/// <param name="strFilePath"> FilePath to save to. If "" is passed in for strFilePath, then m_strXMLFileName
		///  will be used for the path. Otherwise the default file name will be appended to the strFilePath</param>
		/// <returns>Whether or not the save was successful.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/27/08 jrf 1.50.28        Created
        //
		public override bool SaveSettings( string strFilePath )
		{
			bool bReturn = false;
            string strTemp = strFilePath;
			XmlDocument xmldocTemp = new XmlDocument();
            MemoryStream DecryptedStream = new MemoryStream();
            FileStream EncryptedStream = null;
            Rijndael EncryptionAlgorithm = null;

            try
            {
                if (1 > strFilePath.Length)
                {
                    strTemp = m_strXMLFileName;
                }
                else
                {
                    strTemp = strFilePath;
                }

                EncryptedStream = new FileStream(strTemp, FileMode.Create);
                EncryptionAlgorithm = Rijndael.Create();

                SecureDataStorage DataStorage = new SecureDataStorage(SecureDataStorage.DEFAULT_LOCATION);

                //Set the key and IV properties
                EncryptionAlgorithm.Key = DataStorage.RetrieveSecureData(SecureDataStorage.REPLICA_KEY_ID);
                EncryptionAlgorithm.IV = DataStorage.RetrieveSecureData(SecureDataStorage.REPLICA_IV_ID);

                Save(DecryptedStream);

                //Need to rewind stream before encrypting
                DecryptedStream.Position = 0;

                //Encrypt the data and write to the file
                Encryption.EncryptData(EncryptionAlgorithm, DecryptedStream, EncryptedStream);

                if (null != xmldocTemp)
                {
                    //Need to rewind stream before loading
                    DecryptedStream.Position = 0;

                    xmldocTemp.Load(DecryptedStream);

                    if (null != xmldocTemp.SelectSingleNode(DEFAULT_XML_ROOT_NODE))
                    {
                        bReturn = true;
                    }

                    xmldocTemp = null;
                }
            }
            catch
            {
                bReturn = false;
            }
            finally
            {
                if (null != EncryptionAlgorithm)
                {
                    EncryptionAlgorithm.Dispose();
                }

                if (null != DecryptedStream)
                {
                    DecryptedStream.Close();
                }

                if (null != EncryptedStream)
                {
                    EncryptedStream.Close();
                }
            }

			return bReturn;
        }

        /// <summary>
        /// Sets the current node to the childnode of the current node named strChildName. If it does not
        /// exist and bCreate is set to true then it will be created.
        /// </summary>
        /// <param name="strChildNode">Name of child node to select</param>
        /// <param name="bCreate">bool</param>
        /// <returns></returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/27/08 jrf 1.50.28        Created
        //
        public bool SelectNodeFromRoot(string strChildNode, bool bCreate)
        {
            m_xmlnodeCurrent = m_xmlnodeRoot;

            return SelectNode(strChildNode, bCreate);
        }

        /// <summary>
        /// Sets the current node to the childnode of the current node named strChildName. If it does not
        /// exist and bCreate is set to true then it will be created.
        /// </summary>
        /// <param name="strChildNode">Name of child node to select</param>
        /// <param name="bCreate">bool</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/27/08 jrf 1.50.28        Created
        //
        public bool SelectNodeFromAnchor(string strChildNode, bool bCreate)
        {
            m_xmlnodeCurrent = m_xmlnodeAnchor;

            return SelectNode(strChildNode, bCreate);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns or Sets the m_strXMLFileName. If XMLFileName is set to "" then the 
        /// default XMLFileName will be used. When the filename is set, then the 
        /// DOM Document is reloaded.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/23/08 jrf 1.50.28        Created
        //
        public override string XMLFileName
        {
            get
            {
                return m_strXMLFileName;
            }
            set
            {
                FileStream EncryptedStream = null;
                MemoryStream DecryptedStream = null;
                Rijndael EncryptionAlgorithm = null;

                try
                {
                    DetermineFileName(value);

                    if (File.Exists(m_strXMLFileName))
                    {
                        SetNormalAttributes();

                        EncryptedStream = new FileStream(m_strXMLFileName, FileMode.Open);
                        DecryptedStream = new MemoryStream();
                        EncryptionAlgorithm = Rijndael.Create();
                        SecureDataStorage DataStorage = new SecureDataStorage(SecureDataStorage.DEFAULT_LOCATION);

                        //assign key and IV properties
                        EncryptionAlgorithm.Key = DataStorage.RetrieveSecureData(SecureDataStorage.REPLICA_KEY_ID);
                        EncryptionAlgorithm.IV = DataStorage.RetrieveSecureData(SecureDataStorage.REPLICA_IV_ID);

                        Encryption.DecryptData(EncryptionAlgorithm, EncryptedStream, DecryptedStream);

                        //We must rewind the stream
                        DecryptedStream.Position = 0;

                        Load(DecryptedStream);

                        m_xmlnodeRoot = SelectSingleNode(DEFAULT_XML_ROOT_NODE);
                    }
                    else
                    {
                        m_xmlnodeRoot = null;
                    }

                    ProcessNodes();
                }
                catch
                {
                    //Do nothing.
                }
                finally
                {
                    if (null != EncryptedStream)
                    {
                        EncryptedStream.Close();
                    }

                    if (null != DecryptedStream)
                    {
                        DecryptedStream.Close();
                    }

                    if (null != EncryptionAlgorithm)
                    {
                        EncryptionAlgorithm.Dispose();
                    }
                }
            }
        }

        #endregion
    }
}
