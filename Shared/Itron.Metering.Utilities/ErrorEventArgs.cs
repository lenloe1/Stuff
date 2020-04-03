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

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Delegate for a fatal error event handler
    /// </summary>
    /// <param name="sender">The object that sent the error.</param>
    /// <param name="e">The error event earguments.</param>
    public delegate void FatalErrorEventHandler(object sender, ItronErrorEventArgs e);

    /// <summary>
    /// Delegate for a non-fatal error event handler
    /// </summary>
    /// <param name="sender">The control that sent the error.</param>
    /// <param name="e">The error event arguments.</param>
    public delegate void NonFatalErrorEventHandler(object sender, ItronErrorEventArgs e);

    /// <summary>
    /// Error event arguments class
    /// </summary>
    public class ItronErrorEventArgs : EventArgs
    {
        /// <summary>
        /// The message that will be displayed upon an error event
        /// </summary>
        protected string m_strMessage;

        /// <summary>
        /// A placeholder for the exception that caused the error event
        /// </summary>
        protected Exception m_OriginatingException;

        /// <summary>
        /// Constructor that sets the error message.
        /// </summary>
        /// <param name="strMessage">The error message.</param>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 08/31/06 RCG 7.35.00 N/A	Created
        public ItronErrorEventArgs(string strMessage)
        {
            m_strMessage  = strMessage;
            m_OriginatingException = null;
        }

        /// <summary>
        /// Constructor that sets the error message and originating exception
        /// </summary>
        /// <param name="strMessage">The error message.</param>
        /// <param name="eError">The original exception</param>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ -------------------------------------------
        /// 10/06/06 MAH 7.35.00 N/A	Created
        /// </remarks>
        public ItronErrorEventArgs(string strMessage, Exception eError)
        {
            m_strMessage = strMessage;
            OriginatingException = eError;
        }


        /// <summary>
        /// Gets or sets the message that will be displayed when an error occurs.
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 08/31/06 RCG 7.35.00 N/A	Created
        public string Message
        {
            get
            {
                return m_strMessage;
            }
            set
            {
                m_strMessage = value;
            }
        }


        /// <summary>
        /// This property represents the exception that was originally raised
        /// to indicate that an error occurred. This property could be null if 
        /// an exception is not the root cause of the error.
        /// </summary>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ -------------------------------------------
        /// 10/06/06 MAH 7.35.00 N/A	Created
        /// </remarks>
        public Exception OriginatingException
        {
            get { return m_OriginatingException; }
            set { m_OriginatingException = value; }
        }
	
    }
}
