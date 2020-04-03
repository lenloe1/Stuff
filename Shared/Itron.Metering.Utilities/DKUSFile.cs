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
//                              Copyright © 2009 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Class used to represent DKUS files. This class has been adapted from code given to us by Kevin Guthrie.
    /// </summary>

    public class DKUSFile
    {
        #region Constants

        private const string DKUS_ROOT = "AMI";
        private const string DEFAULT_NAMESPACE = "http://www.itron.com/ami/2008/07/NewMeterImport";
        private const string SEC_APPL_PUB_KEYS = "SEC_APPL_PUB_KEYS";
        private const string PUB_CMD_KEYS = "PUB_CMD_KEYS";
        private const string PUB_CMD_KEY = "PUB_CMD_KEY";
        private const string ATTRIBUTE_SLOT = "slot";
        private const string PUB_REV_KEYS = "PUB_REV_KEYS";
        private const string PUB_REV_KEY = "PUB_REV_KEY";
        private const string NODE_EPS = "EPS";
        private const string NODE_ENDPOINT = "EP";
        private const string NODE_DCG = "DCG";
        private const string ATTRIBUTE_DEVICE_CLASS = "deviceClass";
        private const string NODE_ESN = "ESN";
        private const string NODE_PUB_REC_KEY = "PUB_REC_KEY";
        private const string NODE_SIGNATURE = "SIGNATURE";
        private const string ATTRIBUTE_VALUE = "value";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="strFileName">The file that contains the keys.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/09 RCG 2.10.02        Adapted from code given by Kevin Gutrhie

        public DKUSFile(string strFileName)
        {
            m_Keys = new byte[6][];
            XmlReader reader = XmlReader.Create(strFileName);

            reader.MoveToElement();

            // Check to make sure that the file has the correct root node.
            while (false == reader.EOF)
            {
                reader.Read();
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        if (reader.Name == DKUS_ROOT)
                        {
                            // Found the root node so read the rest of the file.
                            ReadDKUSRoot(reader);
                        }

                        break;
                    }
                    default:
                    {
                        // We have not found the root so keep looking.
                        break;
                    }
                }
            }

            reader.Close();

            // Check to see if we have found all of the keys.

            foreach (byte[] key in Keys)
            {
                bool bAllNulls = true;

                if(key != null)
                {
                    foreach (byte byKey in key)
                    {
                        if (byKey != 0)
                        {
                            bAllNulls = false;
                            break;
                        }
                    }
                }

                if (bAllNulls)
                {
                    // One of the keys does not exist
                    throw new ArgumentException("A key does not exist.", "strFileName");
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the keys contained in the file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/09 RCG 2.10.02        Created

        public byte[][] Keys
        {
            get
            {
                return m_Keys;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses an unsupported node.
        /// </summary>
        /// <param name="objReader">The xml reader</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/09 RCG 2.10.02        Adapted from code given by Kevin Gutrhie

        private static void ParseUnsupportedNode(XmlReader objReader)
        {
            string strName = objReader.Name;
            bool bDone = false;

            objReader.MoveToElement();

            while ((false == objReader.EOF) && (false == bDone))
            {
                objReader.Read();

                switch (objReader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        // Found another element that we have to parse through.
                        ParseUnsupportedNode(objReader);

                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        if (strName == objReader.Name)
                        {
                            // Found the end of the element so we are done.
                            bDone = true;
                        }

                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Reads the file starting from the root.
        /// </summary>
        /// <param name="reader">The xml reader</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/09 RCG 2.10.02        Adapted from code given by Kevin Gutrhie

        private void ReadDKUSRoot(XmlReader reader)
        {
            bool bDone = false;

            reader.MoveToElement();

            while ((false == reader.EOF) && (false == bDone))
            {
                reader.Read();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        if (SEC_APPL_PUB_KEYS == reader.Name)
                        {
                            // Read the Application public keys
                            ReadApplicationPublicKeys(reader);
                        }
                        else
                        {
                            // Read an usupported node.
                            ParseUnsupportedNode(reader);
                        }
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        if (DKUS_ROOT == reader.Name)
                        {
                            // Found the end element for the roor so we are done.
                            bDone = true;
                        }

                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Reads the Application Public Keys
        /// </summary>
        /// <param name="reader">The xml reader</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/09 RCG 2.10.02        Adapted from code given by Kevin Gutrhie

        private void ReadApplicationPublicKeys(XmlReader reader)
        {
            bool bDone = false;

            reader.MoveToElement();

            while ((false == reader.EOF) && (false == bDone))
            {
                reader.Read();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        if (PUB_CMD_KEYS == reader.Name)
                        {
                            // Found the command keys so read those.
                            ReadPublicCommandKeys(reader);
                        }
                        else if (PUB_REV_KEYS == reader.Name)
                        {
                            // Found the revocation key so read those.
                            ReadPublicRevocationKeys(reader);
                        }
                        else
                        {
                            // Found an unsupported node
                            ParseUnsupportedNode(reader);
                        }

                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        if (SEC_APPL_PUB_KEYS == reader.Name)
                        {
                            // Found the end of the public keys so we are done
                            bDone = true;
                        }
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Reads the public command keys.
        /// </summary>
        /// <param name="reader">The xml reader</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/09 RCG 2.10.02        Adapted from code given by Kevin Gutrhie

        private void ReadPublicCommandKeys(XmlReader reader)
        {
            bool bDone = false;

            reader.MoveToElement();

            while ((false == reader.EOF) && (false == bDone))
            {
                reader.Read();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        if (PUB_CMD_KEY == reader.Name)
                        {
                            string strSlot = reader.GetAttribute(ATTRIBUTE_SLOT);
                            int slot;

                            // We found a command key first determine which slot the key is for.
                            try
                            {
                                slot = int.Parse(strSlot, CultureInfo.InvariantCulture);

                                if ((slot >= 1) && (slot <= 4))
                                {
                                    // We found the slot so now read the key
                                    ReadKey(reader, out Keys[slot - 1]);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else
                        {
                            // Found an unsupported node so we can ignore it.
                            ParseUnsupportedNode(reader);
                        }

                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        if (PUB_CMD_KEYS == reader.Name)
                        {
                            // Found the end element of the key so we are done
                            bDone = true;
                        }

                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Reads the public revocation keys
        /// </summary>
        /// <param name="reader">The xml reader</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/09 RCG 2.10.02        Adapted from code given by Kevin Gutrhie

        private void ReadPublicRevocationKeys(XmlReader reader)
        {
            bool bDone = false;

            reader.MoveToElement();

            while ((false == reader.EOF) && (false == bDone))
            {
                reader.Read();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        if (PUB_REV_KEY == reader.Name)
                        {
                            string strSlot = reader.GetAttribute(ATTRIBUTE_SLOT);
                            int slot;

                            // We found a revocation key so first determine which slot it is in.
                            try
                            {
                                slot = int.Parse(strSlot, CultureInfo.InvariantCulture);

                                if ((slot >= 5) && (slot <= 6))
                                {
                                    // Found the slot so read the key
                                    ReadKey(reader, out Keys[slot - 1]);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else
                        {
                            // Found an unsupported node so we can ignore it.
                            ParseUnsupportedNode(reader);
                        }

                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        if (PUB_REV_KEYS == reader.Name)
                        {
                            // Found the end of the node so we are done.
                            bDone = true;
                        }

                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Reads a single key.
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <param name="key">The key that was read.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/09 RCG 2.10.02        Adapted from code given by Kevin Gutrhie

        private static void ReadKey(XmlReader reader, out byte[] key)
        {
            key = new byte[37];
            reader.Read();
            reader.ReadContentAsBase64(key, 0, key.Length);
        }

        #endregion

        #region Member Variables

        private byte[][] m_Keys;

        #endregion
    }
}
