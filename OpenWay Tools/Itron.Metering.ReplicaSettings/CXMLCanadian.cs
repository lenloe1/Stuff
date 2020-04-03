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
//                              Copyright © 2007
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using Itron.Metering.Utilities;

namespace Itron.Metering.ReplicaSettings
{
    /// <summary>
    /// Class to help determine if we have a Canadian Installation
    /// </summary>
    public class CXMLCanadian : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
    {
        #region Constants
        /// <summary>
        /// XML_NODE_IS_CANADIAN = "IsCanadian";
        /// </summary>
        protected const string XML_NODE_IS_CANADIAN = "IsCanadian";

        #endregion
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //	07/17/07 KRC 8.10.13        Need a class to determine if the installation is Canadian
        // 
        public CXMLCanadian(string strFilePath)
        {
            m_XMLSettings = new CXMLSettings(DEFAULT_SETTINGS_DIRECTORY + "Canadian.xml", "", "Canadian");

            if (null != m_XMLSettings)
            {
                m_XMLSettings.XMLFileName = strFilePath;
            }
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Determine if IsCanadian bit is set in Canadian Settings
        /// </summary>
        //Revision History
        //MM/DD/YY who Version Issue# Description
        //-------- --- ------- ------ ---------------------------------------------
        //07/17/07 KRC 8.10.13 2954   Determining if Canadian Installation
        //
        public virtual bool IsCanadian
        {
            get
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_IS_CANADIAN, true);
                return m_XMLSettings.CurrentNodeBool;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_IS_CANADIAN, true);
                m_XMLSettings.CurrentNodeBool = value;
            }
        }
        #endregion
    }
}
