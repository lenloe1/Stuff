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
using System.Text;

namespace Itron.Metering.DataCollections
{

    /// <summary>
    /// Basic information about a protocol file
    /// </summary>
    public class ProtocolFile
    {

        #region Definitions

        /// <summary>
        /// The types for protocols
        /// </summary>
        public enum eProtocolType
        {
            /// <summary>
            /// DNP_3 protocol
            /// </summary>
            DNP_3,
            /// <summary>
            /// IEC_60870_5_102 protocol
            /// </summary>
            IEC_60870_5_102,
            /// <summary>
            /// IEC_60870_5_102_Plus protocol
            /// </summary>
            IEC_60870_5_102_Plus,
            /// <summary>
            /// Modbus protocol
            /// </summary>
            Modbus,
            /// <summary>
            /// PDS protocol
            /// </summary>
            PDS
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strFileName">file name of the protocol file</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created 
        public ProtocolFile(string strFileName)
        {
            m_strFullPath = strFileName;
        }//end ProtocolFile

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strFileName">file name of the protocol file</param>
        /// <param name="dtLastModified">time the file was last modified</param>
        /// <param name="eType">protocol type</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created 
        public ProtocolFile(
            string strFileName,
            DateTime dtLastModified,
            eProtocolType eType)
        {
            m_strFullPath = strFileName;
            m_dtLastModified = dtLastModified;
            m_eType = eType;
        }//end ProtocolFile

        /// <summary>
        /// Given a protocol type enum, this method returns a string representation
        /// of that protocol type.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created 
        public static string GetProtocolTypeString(eProtocolType e)
        {
            string strType;

            switch (e)
            {
                case eProtocolType.DNP_3:
                    strType = "DNP 3.0";
                    break;
                case eProtocolType.PDS:
                    strType = "PDS Export Config File";
                    break;
                case eProtocolType.Modbus:
                    strType = "Modbus";
                    break;
                case eProtocolType.IEC_60870_5_102:
                    strType = "IEC 60870-5-102";
                    break;
                case eProtocolType.IEC_60870_5_102_Plus:
                    strType = "IEC 60870-5-102 Plus";
                    break;
                default:
                    strType = "";
                    break;
            }

            return strType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// gets or sets the full file path
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created 
        public string FullPath
        {
            get
            {
                return m_strFullPath;
            }
            set
            {
                m_strFullPath = value;
            }
        }//end FileName

        /// <summary>
        /// gets or sets the time the file was last modified
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created 
        public DateTime LastModified
        {
            get
            {
                return m_dtLastModified;
            }
            set
            {
                m_dtLastModified = value;
            }
        }//end LastModified

        /// <summary>
        /// gets or sets the file name
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created 
        public string Name
        {
            get
            {
                return m_strName;
            }
            set
            {
                m_strName = value;
            }
        }//end Name

        /// <summary>
        /// gets or sets the protocol type
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created 
        public eProtocolType Type
        {
            get
            {
                return m_eType;
            }
            set
            {
                m_eType = value;
            }
        }//end Type

        #endregion

        #region Private Members

        private DateTime m_dtLastModified;

        private eProtocolType m_eType;

        private string m_strFullPath;

        private string m_strName;

        #endregion

    }

}
