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
//                           Copyright © 2013 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device.DLMSDevice
{
    /// <summary>
    /// COSEM Request Types
    /// </summary>
    public enum COSEMExceptionRequestType
    {
        /// <summary>Not Specified</summary>
        [EnumDescription("Not Specified")]
        NotSpecified = 0,
        /// <summary>Not Specified</summary>
        [EnumDescription("Get")]
        Get = 1,
        /// <summary>Not Specified</summary>
        [EnumDescription("Set")]
        Set = 2,
        /// <summary>Not Specified</summary>
        [EnumDescription("Action")]
        Action = 3,
    }

    /// <summary>
    /// COSEM Exception
    /// </summary>
    public class COSEMException : Exception
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/16/13 RCG 2.80.53 N/A    Created
        
        public COSEMException()
            :base()
        {
            m_LogicalName = null;
            m_ID = 0;
            m_Message = "";
            m_RequestType = COSEMExceptionRequestType.NotSpecified;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The Exception Message</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/16/13 RCG 2.80.53 N/A    Created
        
        public COSEMException(string message)
            :this()
        {
            m_Message = message;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the request</param>
        /// <param name="id">The Attribute or Method ID of the request</param>
        /// <param name="type">The type of request</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/16/13 RCG 2.80.53 N/A    Created
        
        public COSEMException(byte[] logicalName, sbyte id, COSEMExceptionRequestType type)
            : this(logicalName, id, type, "")
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the request</param>
        /// <param name="id">The Attribute or Method ID of the request</param>
        /// <param name="type">The type of request</param>
        /// <param name="message">The Exception Message</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/16/13 RCG 2.80.53 N/A    Created
        
        public COSEMException(byte[] logicalName, sbyte id, COSEMExceptionRequestType type, string message)
            : this(message)
        {
            m_LogicalName = logicalName;
            m_ID = id;
            m_RequestType = type;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the Exception Message.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/16/13 RCG 2.80.53 N/A    Created
        
        public override string Message
        {
            get
            {
                StringBuilder MessageBuilder = new StringBuilder();

                if (m_LogicalName != null)
                {
                    MessageBuilder.Append(COSEMLogicalNamesDictionary.LogicalNameString(m_LogicalName));

                    if (m_RequestType == COSEMExceptionRequestType.Set || m_RequestType == COSEMExceptionRequestType.Get)
                    {
                        MessageBuilder.Append(" Attribute: ");
                        MessageBuilder.Append(m_ID.ToString(CultureInfo.InvariantCulture));
                    }
                    else if (m_RequestType == COSEMExceptionRequestType.Action)
                    {
                        MessageBuilder.Append(" Method: ");
                        MessageBuilder.Append(m_ID.ToString(CultureInfo.InvariantCulture));
                    }

                    if (m_RequestType != COSEMExceptionRequestType.NotSpecified)
                    {
                        MessageBuilder.Append(" ");
                        MessageBuilder.Append(m_RequestType.ToDescription());
                    }

                    if (String.IsNullOrEmpty(m_Message) == false)
                    {
                        MessageBuilder.Append(" - ");
                    }
                }

                if (String.IsNullOrEmpty(m_Message) == false)
                {
                    MessageBuilder.Append(m_Message);
                }

                return MessageBuilder.ToString();
            }
        }

        /// <summary>
        /// Gets the Logical Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/16/13 RCG 2.80.53 N/A    Created
        
        public byte[] LogicalName
        {
            get
            {
                return m_LogicalName;
            }
        }

        /// <summary>
        /// Gets the ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/16/13 RCG 2.80.53 N/A    Created
        
        public sbyte ID
        {
            get
            {
                return m_ID;
            }
        }

        /// <summary>
        /// Gets the request type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/16/13 RCG 2.80.53 N/A    Created
        
        public COSEMExceptionRequestType RequestType
        {
            get
            {
                return m_RequestType;
            }
        }

        #endregion

        #region Member Variables

        private byte[] m_LogicalName;
        private sbyte m_ID;
        private COSEMExceptionRequestType m_RequestType;
        private string m_Message;

        #endregion
    }
}
