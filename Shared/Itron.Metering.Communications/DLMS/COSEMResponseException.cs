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
//                           Copyright © 2012 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itron.Metering.Utilities;

namespace Itron.Metering.Communications.DLMS
{
    /// <summary>
    /// Exception that is raised when an Exception Response is received from the meter.
    /// </summary>
    public class DLMSResponseException : Exception
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/12 RCG 2.70.64 N/A    Created
        
        public DLMSResponseException()
            : this("Exception Response APDU received.")
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/12 RCG 2.70.64 N/A    Created
        
        public DLMSResponseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="response">The exception response</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/12 RCG 2.70.64 N/A    Created
        
        public DLMSResponseException(string message, ExceptionResponseAPDU response)
            : base(message)
        {
            m_ExceptionResponse = response;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="response">The response APDU</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/19/13 RCG 2.80.55 N/A    Created
        
        public DLMSResponseException(string message, ConfirmedServiceErrorAPDU response)
            : base(message)
        {
            m_ConfirmedServiceError = response;
        }

        /// <summary>
        /// Returns a string representation of the exception
        /// </summary>
        /// <returns>The exception as a string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/12 RCG 2.70.64 N/A    Created
        
        public override string ToString()
        {
            string ExceptionString = this.GetType().ToString() + ": ";

            if (Message != null)
            {
                ExceptionString += Message + " ";
            }

            if (m_ExceptionResponse != null)
            {
                ExceptionString += "Exception Response. Service Error: " + EnumDescriptionRetriever.RetrieveDescription(m_ExceptionResponse.ServiceError)
                    + " Stat Error: " + EnumDescriptionRetriever.RetrieveDescription(m_ExceptionResponse.StateError) + "\r\n";
            }

            if (m_ConfirmedServiceError != null)
            {
                ExceptionString += "Confirmed Service Error: " + m_ConfirmedServiceError.ServiceError.ToDescription();
            }

            if (InnerException != null)
            {
                ExceptionString += "Inner Exception: " + InnerException.ToString() + "\r\n";
            }

            ExceptionString += "Stack Trace: " + StackTrace;

            return ExceptionString;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Exception Response
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/12 RCG 2.70.64 N/A    Created
        
        public ExceptionResponseAPDU ExceptionResponse
        {
            get
            {
                return m_ExceptionResponse;
            }
            set
            {
                m_ExceptionResponse = value;
            }
        }

        #endregion

        #region Member Variables

        private ExceptionResponseAPDU m_ExceptionResponse;
        private ConfirmedServiceErrorAPDU m_ConfirmedServiceError;

        #endregion
    }
}
