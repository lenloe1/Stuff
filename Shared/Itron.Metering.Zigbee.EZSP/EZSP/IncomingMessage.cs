///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//  storage or retrieval system without the permission in writing from Itron, Inc.
//
//                              Copyright © 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itron.Metering.Zigbee
{
    /// <summary>
    /// Incoming Message object
    /// </summary>
    public class IncomingMessage
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="apsFrame">The APS frame for the message</param>
        /// <param name="lastHopLqi">The last hop LQI</param>
        /// <param name="lastHopRssi">The last hop RSSI</param>
        /// <param name="senderNodeID">The node ID of the sender</param>
        /// <param name="bindingIndex">The index of the sender in the binding table</param>
        /// <param name="addressIndex">The index of the sender in the address table</param>
        /// <param name="messageLength">The length of the message</param>
        /// <param name="messageContents">The contents of the message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public IncomingMessage(EmberIncomingMessageType messageType, EmberApsFrame apsFrame, byte lastHopLqi, byte lastHopRssi,
            ushort senderNodeID, byte bindingIndex, byte addressIndex, byte messageLength, byte[] messageContents)
        {
            m_MessageType = messageType;
            m_APSFrame = apsFrame;
            m_LastHopLqi = lastHopLqi;
            m_LastHopRssi = lastHopRssi;
            m_SenderNodeID = senderNodeID;
            m_BindingIndex = bindingIndex;
            m_AddressIndex = addressIndex;
            m_MessageLength = messageLength;
            m_MessageContents = messageContents;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the message type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public EmberIncomingMessageType MessageType
        {
            get
            {
                return m_MessageType;
            }
        }

        /// <summary>
        /// Gets the APS frame for the message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public EmberApsFrame APSFrame
        {
            get
            {
                return m_APSFrame;
            }
        }

        /// <summary>
        /// Gets the Last Hop LQI of the message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte LastHopLqi
        {
            get
            {
                return m_LastHopLqi;
            }
        }

        /// <summary>
        /// Gets the last hop RSSI of the message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte LastHopRssi
        {
            get
            {
                return m_LastHopRssi;
            }
        }

        /// <summary>
        /// Gets the Node ID of the sender
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ushort SenderNodeID
        {
            get
            {
                return m_SenderNodeID;
            }
        }

        /// <summary>
        /// Gets the index in the binding table of the sender
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte BindingIndex
        {
            get
            {
                return m_BindingIndex;
            }
        }

        /// <summary>
        /// Gets the index in the address table of the sender
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte AddressIndex
        {
            get
            {
                return m_AddressIndex;
            }
        }

        /// <summary>
        /// Gets the length of the message in bytes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte MessageLength
        {
            get
            {
                return m_MessageLength;
            }
        }

        /// <summary>
        /// Gets the contents of the message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte[] MessageContents
        {
            get
            {
                return m_MessageContents;
            }
        }

        #endregion

        #region Member Variables

        private EmberIncomingMessageType m_MessageType;
        private EmberApsFrame m_APSFrame;
        private byte m_LastHopLqi;
        private byte m_LastHopRssi;
        private ushort m_SenderNodeID;
        private byte m_BindingIndex;
        private byte m_AddressIndex;
        private byte m_MessageLength;
        private byte[] m_MessageContents;

        #endregion
    }
}
