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
//                              Copyright © 2012
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itron.Metering.Communications.DLMS
{
    /// <summary>
    /// Interface for DLMS Communications ports
    /// </summary>
    public interface IDLMSComm
    {
        /// <summary>
        /// Event raised when data has been received
        /// </summary>
        event APDUEventHandler APDUReceived;

        /// <summary>
        /// Opens the Communications Port
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the Communications Port
        /// </summary>
        void Close();

        /// <summary>
        /// Sends the data over the port
        /// </summary>
        /// <param name="apdu">The APDU to send</param>
        void SendAPDU(xDLMSAPDU apdu);

        /// <summary>
        /// Clears the current buffers
        /// </summary>
        void ClearBuffers();

        /// <summary>
        /// Clears any event handlers attached to the APDU Received event.
        /// </summary>
        void ClearAPDUReceivedHandlers();

        /// <summary>
        /// Gets whether or not the port is open
        /// </summary>
        bool IsOpen
        {
            get;
        }

        /// <summary>
        /// Gets or sets the Client Port number
        /// </summary>
        ushort ClientPort
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Server Port number
        /// </summary>
        ushort ServerPort
        {
            get;
            set;
        }

        /// <summary>
        /// The Maximum APDU Size
        /// </summary>
        ushort MaxAPDUSize
        {
            get;
        }

        /// <summary>
        /// Gets or sets the Global Encryption Key
        /// </summary>
        byte[] GlobalEncryptionKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Dedicated Encryption Key
        /// </summary>
        byte[] DedicatedEncryptionKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Authentication Key
        /// </summary>
        byte[] DecryptAuthenticationKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Pending Authentication Key
        /// </summary>
        byte[] PendingDecryptAuthenticationKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Server ApTitle
        /// </summary>
        byte[] ServerApTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the last frame counter received
        /// </summary>
        uint LastFrameCounterReceived
        {
            get;
        }
    }

    /// <summary>
    /// Delegate used for APDU Event Handlers
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void APDUEventHandler(object sender, APDUEventArguments e);

    /// <summary>
    /// Event arguments that include a DLMS APDU
    /// </summary>
    public class APDUEventArguments : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="apdu">The APDU for the event</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/13 RCG 2.80.32 N/A    Created
        
        public APDUEventArguments(xDLMSAPDU apdu)
            : base()
        {
            m_APDU = apdu;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the APDU associated with the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/13 RCG 2.80.32 N/A    Created

        public xDLMSAPDU APDU
        {
            get
            {
                return m_APDU;
            }
        }

        #endregion

        #region Member Variables

        private xDLMSAPDU m_APDU;

        #endregion
    }
}
