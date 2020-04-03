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
//                              Copyright © 2006 - 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Itron.Metering.Communications.SCS;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class which handles SCS exceptions.
    /// </summary>
    /// <example>
    /// <code>
    /// try{...}
    /// catch(SCSException e){...}
    /// </code>
    /// </example>
    /// <remarks>
    /// MM/DD/YY who Version Issue# Description 
    /// -------- --- ------- ------ ---------------------------------------
    /// 05/22/06 mrj 7.30.00 N/A    Created 
    /// 05/23/06 jrf 7.30.00 N/A Modified
    /// </remarks>
    public class SCSException : ApplicationException
    {
        #region Public Methods

        /// <summary>
        /// Constructor to create a SCSException.
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="ProtocolResponse"></param>
        /// <param name="iAddress"></param>
        /// <param name="strItemName"></param>
        /// <example>
        /// <code>
        /// SCSException e = new SCSException(
        ///		SCSCommands.SCS_D, 
        ///		SCSProtocolResponse.NoResponse, 
        ///		0x2156, 
        ///		"Unit ID");
        /// </code>
        /// </example>
        /// <remarks>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// 05/22/06 mrj 7.30.00 N/A	Created
        /// 05/23/06 jrf 7.30.00 N/A	Modified
        /// </remarks>
        public SCSException(SCSCommands Command, SCSProtocolResponse ProtocolResponse,
            int iAddress, string strItemName)
            :
            base("SCS Protocol error occurred")
        {
            CommandType = Command;
            SCSResponse = ProtocolResponse;
            FieldName = strItemName;
            OffendingAddress = iAddress;

            switch (ProtocolResponse)
            {
                case SCSProtocolResponse.SCS_CAN:
                    Description = "SecurityError (CAN)";
                    break;
                case SCSProtocolResponse.SCS_NAK:
                    Description = "InvalidRequest (NAK)";
                    break;
                case SCSProtocolResponse.NoResponse:
                    Description = "No Response";
                    break;
                default:
                    Description = "Unknown (" + ProtocolResponse.ToString() + ")";
                    break;
            }
            Description += ": Item = " + FieldName + ": Address = 0x" +
                OffendingAddress.ToString("X", CultureInfo.InvariantCulture) + ": Command = " + CommandType.ToString();
        }

        #endregion

        #region Public Properties

        /// <summary>This property gets or sets the field name of the item which 
        /// caused the exception.</summary>
        /// <returns>
        /// An string representing the field name.
        /// </returns>
        /// <example>
        /// <code>
        /// try{...}
        /// catch(SCSException e)
        /// {
        ///		string strFieldName = e.FieldName;
        ///	}
        /// </code>
        /// </example>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A	Modified
        /// </remarks>
        /// 
        public string FieldName
        {
            get { return m_strFieldName; }
            set { m_strFieldName = value; }
        }

        /// <summary>This property gets or sets the SCS protocol response where 
        /// the exception occured.</summary>
        /// <returns>
        /// An SCSProtocolResponse object representing the SCS protocol response.
        /// </returns>
        /// <example>
        /// <code>
        /// try{...}
        /// catch(SCSException e)
        /// {
        ///		SCSProtocolResponse objSCSResponse = e.SCSResponse;
        ///	}
        /// </code>
        /// </example>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A	Modified
        /// </remarks>
        /// 
        public SCSProtocolResponse SCSResponse
        {
            get { return m_SCSResponse; }
            set { m_SCSResponse = value; }
        }

        /// <summary>This property gets or sets the SCS command where the 
        /// exception occured.</summary>
        /// <returns>
        /// An SCSCommands object representing the SCS command.
        /// </returns>
        /// <example>
        /// <code>
        /// try{...}
        /// catch(SCSException e)
        /// {
        ///		SCSCommands objSCSCommand = e.CommandType;
        ///	}
        /// </code>
        /// </example>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A	Modified
        /// </remarks>
        /// 
        public SCSCommands CommandType
        {
            get { return m_CommandType; }
            set { m_CommandType = value; }
        }

        /// <summary>This property gets or sets a description 
        /// of the exception.</summary>
        /// <returns>
        /// A string representing the description.
        /// </returns>
        /// <example>
        /// <code>
        /// try{...}
        /// catch(SCSException e)
        /// {
        ///		string strDescription = e.Description;
        ///	}
        /// </code>
        /// </example>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A	Modified
        /// </remarks>
        /// 
        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        /// <summary>This property gets or sets the offending address which 
        /// led to the exception.</summary>
        /// <returns>
        /// A int representing the address.
        /// </returns>
        /// <example>
        /// <code>
        /// try{...}
        /// catch(SCSException e)
        /// {
        ///		int iAddress = e.OffendingAddress;
        ///	}
        /// </code>
        /// </example>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A	Modified
        /// </remarks>
        /// 
        public int OffendingAddress
        {
            get { return m_iAddress; }
            set { m_iAddress = value; }
        }

        #endregion

        #region Members

        private string m_strDescription;
        private SCSCommands m_CommandType;
        private SCSProtocolResponse m_SCSResponse;
        private string m_strFieldName;
        private int m_iAddress;

        #endregion
    } // End SCSException

}
