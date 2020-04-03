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
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using Itron.Metering.Device;

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// Class used for reading and writing an RMA File
    /// </summary>
    public class RMAFile
    {
        #region Constants

        private const string XML_ROOT = "OpenWayRMA";
        private const string XML_SERIAL_NUM = "MFGSerialNumber";
        private const string XML_AUTH_KEY = "AuthorizationKey";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/12/09 RCG 2.30.16        Created

        public RMAFile()
        {
            m_strSerialNumber = null;
            m_AuthorizationKey = null;
            m_strFileName = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strFileName">The path of the file to load.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/12/09 RCG 2.30.16        Created

        public RMAFile(string strFileName)
        {
            Load(strFileName);
        }

        /// <summary>
        /// Saves the RMA file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/12/09 RCG 2.30.16        Created

        public void Save()
        {
            XElement Root = new XElement(XML_ROOT);

            Root.Add(new XAttribute(XML_SERIAL_NUM, m_strSerialNumber));
            Root.Add(new XElement(XML_AUTH_KEY, Convert.ToBase64String(m_AuthorizationKey.EncryptedData)));

            Root.Save(m_strFileName);
        }

        /// <summary>
        /// Saves the RMA File to the specified location.
        /// </summary>
        /// <param name="strFileName">The location to save the file.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/12/09 RCG 2.30.16        Created

        public void SaveAs(string strFileName)
        {
            m_strFileName = strFileName;
            Save();
        }

        /// <summary>
        /// Loads the specified file.
        /// </summary>
        /// <param name="strFileName">The path to the file to load</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/12/09 RCG 2.30.16        Created

        public void Load(string strFileName)
        {
            XDocument CurrentDoc = null;

            if (File.Exists(strFileName))
            {
                CurrentDoc = XDocument.Load(strFileName);

                if (CurrentDoc.Root.Name == XML_ROOT)
                {
                    m_strFileName = strFileName;

                    // Get the Serial Number
                    if (CurrentDoc.Root.Attribute(XML_SERIAL_NUM) != null)
                    {
                        m_strSerialNumber = CurrentDoc.Root.Attribute(XML_SERIAL_NUM).Value;
                    }

                    // Get the Authorization Key
                    if (CurrentDoc.Root.Element(XML_AUTH_KEY) != null)
                    {
                        string strValue = CurrentDoc.Root.Element(XML_AUTH_KEY).Value;
                        m_AuthorizationKey = new RMASignedAuthorizationKey();

                        // The data is encoded in the file so we need to get the bytes by decoding it
                        m_AuthorizationKey.EncryptedData = Convert.FromBase64String(strValue);
                    }
                }
                else
                {
                    throw new ArgumentException("Specified file is not a valid RMA File", "strFileName");
                }
            }
            else
            {
                throw new FileNotFoundException("Could not find the file " + strFileName);
            }
        }

        /// <summary>
        /// Determines whether or not the specified file is a valid RMA file
        /// </summary>
        /// <param name="strFileName">The file to check</param>
        /// <returns>True if the the file is a valid RMA File. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/16/09 RCG 2.30.17        Created

        public static bool IsValidRMAFile(string strFileName)
        {
            bool bIsValid = false;

            if (File.Exists(strFileName))
            {
                try
                {
                    XDocument RMADocument = XDocument.Load(strFileName);

                    if (RMADocument.Root.Name == XML_ROOT)
                    {
                        bIsValid = true;
                    }
                }
                catch (Exception)
                {
                    // If we get an exception then its not a valid file
                    bIsValid = false;
                }
            }

            return bIsValid;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Serial Number for the RMA'd meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/12/09 RCG 2.30.16        Created

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
        /// Gets the Authorization Key for the RMA'd meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/12/09 RCG 2.30.16        Created

        public RMASignedAuthorizationKey AuthorizationKey
        {
            get
            {
                return m_AuthorizationKey;
            }
            set
            {
                m_AuthorizationKey = value;
            }
        }

        #endregion

        #region Member Variables

        private string m_strSerialNumber;
        private string m_strFileName;
        private RMASignedAuthorizationKey m_AuthorizationKey;

        #endregion
    }
}
