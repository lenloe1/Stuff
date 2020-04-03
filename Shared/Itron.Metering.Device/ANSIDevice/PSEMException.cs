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
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class which handles PSEM exceptions.
    /// </summary>
    /// <example>
    /// <code>
    /// try{...}
    /// catch(PSEMException e){...}
    /// </code>
    /// </example>
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 09/11/06 KRC 7.35.00 N/A	Created
    /// 12/05/06 MAH 8.00.00 Moved from Itron.Metering namespace to Itron.Metering.Device
    /// 
    public class PSEMException : ApplicationException
    {

    #region Constants
	#endregion

	#region Definitions
        /// <summary>
        /// PSEMCommand Enumeration
        /// </summary>
        public enum PSEMCommands
        {
            /// <summary>
            /// A PSEM Read of some sort was done
            /// </summary>
            [EnumDescription("Read")]
            PSEM_READ = 0,
            /// <summary>
            /// a PSEN Write of some sort was done.
            /// </summary>
            [EnumDescription("Write")]
            PSEM_WRITE = 1,
        }

	#endregion

	#region Public Methods
        /// <summary>
        /// Constructor to create a SCSException.
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="ProtocolResponse"></param>
        /// <param name="strItemName"></param>
        /// <example>
        /// <code>
        /// PSEMException e = new PSEMException(    
        ///		SCSCommands.SCS_D, 
        ///		PSEMProtocolResponse.NoResponse,    
        ///		0x2156, 
        ///		"Unit ID");
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// 09/11/06 KRC 7.30.00 N/A	Created
        public PSEMException(PSEMCommands Command, PSEMResponse ProtocolResponse,
            string strItemName)
            :
            base("PSEM Protocol error occurred")
        {
            CommandType = Command;
            PSEMResponse = ProtocolResponse;
            FieldName = strItemName;

            switch (ProtocolResponse)
            {
                case PSEMResponse.Isc:
                    Description = "Security Error (ISC)";
                    break;
                case PSEMResponse.Iar:
                    Description = "Inappropriate Action Request (IAR)";
                    break;
                case PSEMResponse.Bsy:
                    Description = "Busy (BSY)";
                    break;
                case PSEMResponse.Isss:
                    Description = "Invalid Service Sequence State (ISSS)";
                    break;
                case PSEMResponse.Err:
                    Description = "Service Request Rejected (ERR)";
                    break;
                case PSEMResponse.Sns:
                    Description = "Service Not Supported (SNS)";
                    break;
                case PSEMResponse.Onp:
                    Description = "Operation Not Possible (ONP)";
                    break;
                case PSEMResponse.Dnr:
                    Description = "Data Not Ready (DNR)";
                    break;
                case PSEMResponse.Dlk:
                    Description = "Data Locked (DLK)";
                    break;
                case PSEMResponse.Rno:
                    Description = "Renegotiate Request (RNO)";
                    break;
                default:
                    Description = "Unknown (" + ProtocolResponse.ToString() + ")";
                    break;
            }
            Description += ": Item = " + FieldName + ": Command = " + CommandType.ToString();
        }

        /// <summary>This property gets or sets the field name of the item which 
        /// caused the exception.</summary>
        /// <returns>
        /// An string representing the field name.
        /// </returns>
        /// <example>
        /// <code>
        /// try{...} catch(PSEMException e) {
        ///		string strFieldName = e.FieldName;
        ///	}
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/11/06 KRC 7.30.00 N/A	Created
        /// 
        public string FieldName
        {
            get { return m_strFieldName; }
            set { m_strFieldName = value; }
        }

        /// <summary>This property gets or sets the PSEM protocol response 
        /// where the exception occured.</summary>
        /// <returns>
        /// An PSEMResponse object representing the PSEM protocol 
        /// response.
        /// </returns>
        /// <example>
        /// <code>
        /// try{...}
        /// catch(PSEMException e)
        /// {
        ///		PSEMResponse objPSEMResponse = e.PSEMResponse;
        ///	}
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/11/06 KRC 7.30.00 N/A	Created
        /// 
        public PSEMResponse PSEMResponse
        {
            get { return m_PSEMResponse; }
            set { m_PSEMResponse = value; }
        }

        /// <summary>This property gets or sets the PSEM command where the 
        /// exception occured.</summary>
        /// <returns>
        /// An SCSCommands object representing the PSEM command.
        /// </returns>
        /// <example>
        /// <code>
        /// try{...}
        /// catch(SCSException e)
        /// {
        ///		PSEMCommands objPSEMCommand = e.CommandType;
        ///	}
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/11/06 KRC 7.30.00 N/A	Created
        /// 
        public PSEMCommands CommandType
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
        /// catch(PSEMException e)
        /// {
        ///		string strDescription = e.Description;
        ///	}
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/11/06 KRC 7.30.00 N/A	Created
        /// 
        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
         
    #endregion

	#region Public Properties
	#endregion

	#region Events
	#endregion

    #region Internal Methods
    #endregion

	#region Internal Properties
	#endregion

    #region Protected Methods
    #endregion

	#region Protected Properties
	#endregion

    #region Private Methods
    #endregion

	#region Private Properties
	#endregion

	#region Members
        
        private string m_strDescription;
        private PSEMCommands m_CommandType;
        private PSEMResponse m_PSEMResponse;
        private string m_strFieldName;

	#endregion

    } // End PSEMException
}
