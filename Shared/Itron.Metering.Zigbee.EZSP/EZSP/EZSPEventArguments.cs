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
    #region Event Delegates

    /// <summary>
    /// Delegate for the EZSP Timer Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void TimerEventHandler (object sender, TimerEventArgs e);
 
    /// <summary>
    /// Delegate for the Stack Status Updated Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void StackStatusUpdatedHandler (object sender, StackStatusUpdatedEventArgs e);

    /// <summary>
    /// Delegate for the Child Joined Event Handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ChildJoinedHandler(object sender, ChildJoinedEventArgs e);

    /// <summary>
    /// Delegate for the Binding Set Remotely Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void BindingSetRemotelyHandler(object sender, BindingSetRemotelyEventArgs e);

    /// <summary>
    /// Delegate for the Binding Deleted Remotely Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void BindingDeletedRemotelyHandler(object sender, BindingDeletedRemotelyEventArgs e);

    /// <summary>
    /// Delegate for the Message Sent Event Handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void MessageSentHandler(object sender, MessageSentEventArgs e);

    /// <summary>
    /// Delegate for a Poll Complete event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void PollCompleteHandler(object sender, EmberStatusEventArgs e);

    /// <summary>
    /// Delegate for a Poll Received event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void PollReceivedHandler(object sender, NodeIDEventArgs e);

    /// <summary>
    /// Delegate for a SenderEUIReceived event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void SenderEUIReceivedHandler(object sender, EUIEventArgs e);

    /// <summary>
    /// Delegate for a Message Received event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void MessageReceivedHandler(object sender, MessageReceivedEventArgs e);

    /// <summary>
    /// Delegate for a Route Record Received event
    /// </summary>
    /// <param name="sender">The object that send the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void RouteRecordReceivedHandler(object sender, RouteRecordReceivedEventArgs e);

    /// <summary>
    /// Delegate for a Many To One Route Available event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void ManyToOneRouteAvailableHandler(object sender, ManyToOneRouteAvailableEventArgs e);

    /// <summary>
    /// Delegate for a Route Error event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void RouteErrorOccurredHandler(object sender, RouteErrorOccurredEventArgs e);

    /// <summary>
    /// Delegate for an ID Conlfict Detected event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void IDConflictDetectedHandler(object sender, NodeIDEventArgs e);

    /// <summary>
    /// Delegate for a MAC passthrough message received event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void MacPassthroughMessageReceivedHandler(object sender, MacPassthroughMessageReceivedEventArgs e);

    /// <summary>
    /// Delegate for Mac Filter Match passthrough message received events
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void MacFilterMatchMessageReceivedHandler(object sender, MacFilterMatchMessageReceivedEventArgs e);

    /// <summary>
    /// Delegate for the Raw Message Sent event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void RawMessageSentHandler(object sender, EmberStatusEventArgs e);

    /// <summary>
    /// Delegate for the Network Key Switched event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void NetworkKeySwitchedHandler(object sender, NetworkKeySwitchedEventArgs e);

    /// <summary>
    /// Delegate for the ZigBee Key Established event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void ZigBeeKeyEstablisedHandler(object sender, ZigBeeKeyEstablishedEventArgs e);

    /// <summary>
    /// Delegate for the Trust Center Joined event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void TrustCenterJoinedHandler(object sender, TrustCenterJoinedEventArgs e);

    /// <summary>
    /// Delegate for the CBKE Key Generated event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void CBKEKeyGeneratedHandler(object sender, CBKEKeyGeneratedEventArgs e);

    /// <summary>
    /// Delegate for the SMACS Calculated event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void SmacsCalculatedHandler(object sender, SmacsCalculatedEventArgs e);

    /// <summary>
    /// Delegate for the DSA Signed event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void DsaSignedHandler(object sender, DsaSignedEventArgs e);

    /// <summary>
    /// Delegate for the DSA verified event
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void DsaVerifiedHandler(object sender, EmberStatusEventArgs e);

    #endregion

    /// <summary>
    /// Event arguments for the EZSP timer event
    /// </summary>
    public class TimerEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timerID">The ID of the timer that caused the event</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public TimerEventArgs(byte timerID)
            : base()
        {
            m_TimerID = timerID;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the ID of the timer that caused the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte TimerID
        {
            get
            {
                return m_TimerID;
            }
        }

        #endregion

        #region Member Variables

        private byte m_TimerID;

        #endregion
    }

    /// <summary>
    /// Event arguments for the Stack Status Updated event
    /// </summary>
    public class StackStatusUpdatedEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status">The new status</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public StackStatusUpdatedEventArgs(EmberStatus status)
        {
            m_Status = status;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the updated status of the stack
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public EmberStatus Status
        {
            get
            {
                return m_Status;
            }
        }

        #endregion

        #region Member Variables

        private EmberStatus m_Status;

        #endregion
    }

    /// <summary>
    /// Event arguments for the Child Joined event
    /// </summary>
    public class ChildJoinedEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">The index of the child</param>
        /// <param name="joining">Whether the child is joining or leaving</param>
        /// <param name="childNodeID">The Node ID of the child</param>
        /// <param name="childEUI">The EUI of the child</param>
        /// <param name="childType">The Node type of the child</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ChildJoinedEventArgs(byte index, bool joining, ushort childNodeID, ulong childEUI, EmberNodeType childType)
        {
            m_Index = index;
            m_Joining = joining;
            m_ChildNodeID = childNodeID;
            m_ChildEUI = childEUI;
            m_ChildType = childType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the index of the child
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte Index
        {
            get
            {
                return m_Index;
            }
        }

        /// <summary>
        /// Gets whether the child is joining or leaving
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool IsJoining
        {
            get
            {
                return m_Joining;
            }
        }

        /// <summary>
        /// Gets the Node ID of the child
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort ChildNodeID
        {
            get
            {
                return m_ChildNodeID;
            }
        }

        /// <summary>
        /// Gets the EUI of the child
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ulong ChildEUI
        {
            get
            {
                return m_ChildEUI;
            }
        }

        /// <summary>
        /// Gets the type of the child
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberNodeType ChildType
        {
            get
            {
                return m_ChildType;
            }
        }

        #endregion

        #region Member Variables

        private byte m_Index;
        private bool m_Joining;
        private ushort m_ChildNodeID;
        private ulong m_ChildEUI;
        private EmberNodeType m_ChildType;

        #endregion
    }

    /// <summary>
    /// Event arguments for the Binding Set Remotely event
    /// </summary>
    public class BindingSetRemotelyEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bindingTableEntry">The new binding table entry</param>
        /// <param name="index">The index of the new entry</param>
        /// <param name="status">The status of the set</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public BindingSetRemotelyEventArgs(EmberBindingTableEntry bindingTableEntry, byte index, EmberStatus status)
        {
            m_BindingTableEntry = bindingTableEntry;
            m_Index = index;
            m_Status = status;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the binding table entry that was set
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberBindingTableEntry BindingTableEntry
        {
            get
            {
                return m_BindingTableEntry;
            }
        }

        /// <summary>
        /// Gets the index of the table entry
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte Index
        {
            get
            {
                return m_Index;
            }
        }

        /// <summary>
        /// Gets the Status of the Binding table set
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberStatus Status
        {
            get
            {
                return m_Status;
            }
        }

        #endregion

        #region Member Variables

        private EmberBindingTableEntry m_BindingTableEntry;
        private byte m_Index;
        private EmberStatus m_Status;

        #endregion
    }

    /// <summary>
    /// Event arguments for the Binding Deleted Remotely event
    /// </summary>
    public class BindingDeletedRemotelyEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">The index of the </param>
        /// <param name="status">The status of the deletion</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public BindingDeletedRemotelyEventArgs(byte index, EmberStatus status)
        {
            m_Index = index;
            m_Status = status;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the index of the binding entry deleted
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte Index
        {
            get
            {
                return m_Index;
            }
        }

        /// <summary>
        /// Gets the status of the deletion
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public EmberStatus Status
        {
            get
            {
                return m_Status;
            }
        }

        #endregion

        #region Member Variables

        private byte m_Index;
        private EmberStatus m_Status;

        #endregion
    }

    /// <summary>
    /// Event arguments for the Message Sent event
    /// </summary>
    public class MessageSentEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageType">The type of message sent</param>
        /// <param name="indexOrDestination">The table index or Node ID for the destination</param>
        /// <param name="apsFrame">The APS frame for the message</param>
        /// <param name="messageTag">The message tag</param>
        /// <param name="status">The ack status of the message</param>
        /// <param name="messageLength">The length of the message contents</param>
        /// <param name="messageContents">The contents of the message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public MessageSentEventArgs(EmberOutgoingMessageType messageType, ushort indexOrDestination, EmberApsFrame apsFrame, byte messageTag, EmberStatus status, byte messageLength, byte[] messageContents)
        {
            m_MessageType = messageType;
            m_IndexOrDestination = indexOrDestination;
            m_APSFrame = apsFrame;
            m_MessageTag = messageTag;
            m_Status = status;
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
        
        public EmberOutgoingMessageType MessageType
        {
            get
            {
                return m_MessageType;
            }
        }

        /// <summary>
        /// Gets the index of the table or the node ID of the destination
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort IndexOrDestination
        {
            get
            {
                return m_IndexOrDestination;
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
        /// Gets the message tag
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte MessageTag
        {
            get
            {
                return m_MessageTag;
            }
        }

        /// <summary>
        /// Gets the destination ack status of the message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberStatus Status
        {
            get
            {
                return m_Status;
            }
        }

        /// <summary>
        /// Gets the length of the message
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

        private EmberOutgoingMessageType m_MessageType;
        private ushort m_IndexOrDestination;
        private EmberApsFrame m_APSFrame;
        private byte m_MessageTag;
        private EmberStatus m_Status;
        private byte m_MessageLength;
        private byte[] m_MessageContents;

        #endregion
    }

    /// <summary>
    /// Event arguments for events that only return an EmberStatus value
    /// </summary>
    public class EmberStatusEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status">The status of the call back</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberStatusEventArgs(EmberStatus status)
        {
            m_Status = status;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the status of the callback
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public EmberStatus Status
        {
            get
            {
                return m_Status;
            }
        }

        #endregion

        #region Member Variables

        private EmberStatus m_Status;

        #endregion
    }

    /// <summary>
    /// Event arguments for callbacks that only return the Node ID
    /// </summary>
    public class NodeIDEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeID">The node ID</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public NodeIDEventArgs(ushort nodeID)
        {
            m_NodeID = nodeID;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the node ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ushort NodeID
        {
            get
            {
                return m_NodeID;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_NodeID;

        #endregion

    }

    /// <summary>
    /// Event arguments for callbacks that include the EUI
    /// </summary>
    public class EUIEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eui">The EUI</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public EUIEventArgs(ulong eui)
        {
            m_EUI = eui;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the EUI
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ulong EUI
        {
            get
            {
                return m_EUI;
            }
        }

        #endregion

        #region Member Variables

        private ulong m_EUI;

        #endregion
    }

    /// <summary>
    /// Event arguments for the Message Received event
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The incoming message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public MessageReceivedEventArgs(IncomingMessage message)
        {
            m_Message = message;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the incoming Message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public IncomingMessage Message
        {
            get
            {
                return m_Message;
            }
        }

        #endregion

        #region Member Variables

        private IncomingMessage m_Message;

        #endregion
    }

    /// <summary>
    /// Event arguments for the Route Record Received event
    /// </summary>
    public class RouteRecordReceivedEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceNodeID">The Node ID of the source of the route record</param>
        /// <param name="sourceEUI">The EUI of the source of the route record</param>
        /// <param name="lastHopLqi">The last hop LQI of the message</param>
        /// <param name="lastHopRssi">The last hop RSSI of the message</param>
        /// <param name="relayCount">The number of relay</param>
        /// <param name="relayList">The list of Node IDs for the route</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public RouteRecordReceivedEventArgs(ushort sourceNodeID, ulong sourceEUI, byte lastHopLqi, byte lastHopRssi, byte relayCount, ushort[] relayList)
        {
            m_SourceNodeID = sourceNodeID;
            m_SourceEUI = sourceEUI;
            m_LastHopLqi = lastHopLqi;
            m_LastHopRssi = lastHopRssi;
            m_RelayCount = relayCount;
            m_RelayList = relayList;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Node ID of the source
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort SourceNodeID
        {
            get
            {
                return m_SourceNodeID;
            }
        }

        /// <summary>
        /// Gets the EUI of the source
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ulong SourceEUI
        {
            get
            {
                return m_SourceEUI;
            }
        }

        /// <summary>
        /// Gets the Last Hop LQI
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
        /// Gets the Last Hop RSSI
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
        /// Gets the number of Relays in the Relay List
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte RelayCount
        {
            get
            {
                return m_RelayCount;
            }
        }

        /// <summary>
        /// Gets the list of Node IDs that make up the route
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort[] RelayList
        {
            get
            {
                return m_RelayList;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_SourceNodeID;
        private ulong m_SourceEUI;
        private byte m_LastHopLqi;
        private byte m_LastHopRssi;
        private byte m_RelayCount;
        private ushort[] m_RelayList;

        #endregion
    }

    /// <summary>
    /// Many To One Route Available event arguments
    /// </summary>
    public class ManyToOneRouteAvailableEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceNodeID">The node ID of the concentrator</param>
        /// <param name="sourceEUI">The EUI of the concentrator</param>
        /// <param name="cost">The path cost to the concentrator</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ManyToOneRouteAvailableEventArgs(ushort sourceNodeID, ulong sourceEUI, byte cost)
        {
            m_SourceNodeID = sourceNodeID;
            m_SourceEUI = sourceEUI;
            m_Cost = cost;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Node ID of the concentrator
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort SourceNodeID
        {
            get
            {
                return m_SourceNodeID;
            }
        }

        /// <summary>
        /// Gets the EUI of the concentrator
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ulong SourceEUI
        {
            get
            {
                return m_SourceEUI;
            }
        }

        /// <summary>
        /// Gets the cost of the path
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte Cost
        {
            get
            {
                return m_Cost;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_SourceNodeID;
        private ulong m_SourceEUI;
        private byte m_Cost;

        #endregion
    }

    /// <summary>
    /// Route Error event arguments
    /// </summary>
    public class RouteErrorOccurredEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status">The error status</param>
        /// <param name="targetNodeID">The Node ID of the target</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public RouteErrorOccurredEventArgs(EmberStatus status, ushort targetNodeID)
        {
            m_Status = status;
            m_TargetNodeID = targetNodeID;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the error status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberStatus Status
        {
            get
            {
                return m_Status;
            }
        }

        /// <summary>
        /// Gets the Node ID of the target
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort TargetNodeID
        {
            get
            {
                return m_TargetNodeID;
            }
        }
        
        #endregion

        #region Member Variables

        private EmberStatus m_Status;
        private ushort m_TargetNodeID;

        #endregion
    }

    /// <summary>
    /// Event arguments for the Message Received event arguments
    /// </summary>
    public class MacPassthroughMessageReceivedEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="lastHopLqi">The last hop LQI</param>
        /// <param name="lastHopRssi">The last hop RSSI</param>
        /// <param name="messageLength">The length of the message</param>
        /// <param name="messageContents">The contents of the message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public MacPassthroughMessageReceivedEventArgs(EmberMacPassthroughType messageType, byte lastHopLqi, byte lastHopRssi, byte messageLength, byte[] messageContents)
        {
            m_MessageType = messageType;
            m_LastHopLqi = lastHopLqi;
            m_LastHopRssi = lastHopRssi;
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

        public EmberMacPassthroughType MessageType
        {
            get
            {
                return m_MessageType;
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

        private EmberMacPassthroughType m_MessageType;
        private byte m_LastHopLqi;
        private byte m_LastHopRssi;
        private byte m_MessageLength;
        private byte[] m_MessageContents;

        #endregion
    }

    /// <summary>
    /// MAC Passthrough Filtered Message Received event arguments
    /// </summary>
    public class MacFilterMatchMessageReceivedEventArgs : MacPassthroughMessageReceivedEventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filterIndexMatch">The index of the matched filter</param>
        /// <param name="messageType">The message type</param>
        /// <param name="lastHopLqi">The last hop LQI</param>
        /// <param name="lastHopRssi">The last hop RSSI</param>
        /// <param name="messageLength">The length of the message</param>
        /// <param name="messageContents">The contents of the message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public MacFilterMatchMessageReceivedEventArgs(byte filterIndexMatch, EmberMacPassthroughType messageType,
            byte lastHopLqi, byte lastHopRssi, byte messageLength, byte[] messageContents)
            : base(messageType, lastHopLqi, lastHopRssi, messageLength, messageContents)
        {
            m_FilterIndexMatch = filterIndexMatch;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the index of the matched filter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte FilterIndexMatch
        {
            get
            {
                return m_FilterIndexMatch;
            }
        }

        #endregion

        #region Member Variables

        private byte m_FilterIndexMatch;

        #endregion
    }

    /// <summary>
    /// Network key switched event arguments
    /// </summary>
    public class NetworkKeySwitchedEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sequenceNumber">The new network key's sequence number</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public NetworkKeySwitchedEventArgs(byte sequenceNumber)
        {
            m_SequenceNumber = sequenceNumber;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Network Key sequence number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public byte SequenceNumber
        {
            get
            {
                return m_SequenceNumber;
            }
        }

        #endregion

        #region Member Variables

        private byte m_SequenceNumber;

        #endregion
    }

    /// <summary>
    /// ZigBee Key Established Event Arguments
    /// </summary>
    public class ZigBeeKeyEstablishedEventArgs : EUIEventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eui">The EUI of the partner the key is established with</param>
        /// <param name="status">The status of the establishment</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ZigBeeKeyEstablishedEventArgs(ulong eui, EmberKeyStatus status)
            : base(eui)
        {
            m_Status = status;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the status of the establishment
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public EmberKeyStatus Status
        {
            get
            {
                return m_Status;
            }
        }

        #endregion

        #region Member Variables

        private EmberKeyStatus m_Status;

        #endregion
    }

    /// <summary>
    /// Event arguments for a Trust Center Join
    /// </summary>
    public class TrustCenterJoinedEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="newNodeID">The Node ID of the new device</param>
        /// <param name="newEUI">The EUI of the new device</param>
        /// <param name="status">The device status</param>
        /// <param name="policyDecision">The policy decision for the device join</param>
        /// <param name="parentNodeID">The Parent Node ID for the device</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public TrustCenterJoinedEventArgs(ushort newNodeID, ulong newEUI, EmberDeviceUpdate status, EmberJoinDecision policyDecision, ushort parentNodeID)
        {
            m_NewNodeID = newNodeID;
            m_NewEUI = newEUI;
            m_Status = status;
            m_PolicyDecision = policyDecision;
            m_ParentNodeID = parentNodeID;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Node ID of the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort NewNodeID
        {
            get
            {
                return m_NewNodeID;
            }
        }

        /// <summary>
        /// Gets the EUI of the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ulong NewEUI
        {
            get
            {
                return m_NewEUI;
            }
        }

        /// <summary>
        /// Gets the device status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberDeviceUpdate Status
        {
            get
            {
                return m_Status;
            }
        }

        /// <summary>
        /// Gets the policy decision of the join
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public EmberJoinDecision PolicyDecision
        {
            get
            {
                return m_PolicyDecision;
            }
        }

        /// <summary>
        /// Gets the Node ID of the device's parent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ushort ParentNodeID
        {
            get
            {
                return m_ParentNodeID;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_NewNodeID;
        private ulong m_NewEUI;
        private EmberDeviceUpdate m_Status;
        private EmberJoinDecision m_PolicyDecision;
        private ushort m_ParentNodeID;

        #endregion
    }

    /// <summary>
    /// CBKE Key generated event arguments
    /// </summary>
    public class CBKEKeyGeneratedEventArgs : EmberStatusEventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status">The result of the CBKE operation</param>
        /// <param name="publicKey">The generated public key</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public CBKEKeyGeneratedEventArgs(EmberStatus status, byte[] publicKey)
            : base(status)
        {
            m_PublicKey = publicKey;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Public Key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte[] PublicKey
        {
            get
            {
                return m_PublicKey;
            }
        }

        #endregion

        #region Member Variables

        private byte[] m_PublicKey;

        #endregion
    }

    /// <summary>
    /// SMAC Calculated event arguments
    /// </summary>
    public class SmacsCalculatedEventArgs : EmberStatusEventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status">The status of the SMAC calculation</param>
        /// <param name="initiatorSmac">The initiator SMAC</param>
        /// <param name="responderSmac">The responder SMAC</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public SmacsCalculatedEventArgs(EmberStatus status, byte[] initiatorSmac, byte[] responderSmac)
            : base(status)
        {
            m_InitiatorSmac = initiatorSmac;
            m_ResponderSmac = responderSmac;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the initiator's SMAC
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte[] InitiatorSmac
        {
            get
            {
                return m_InitiatorSmac;
            }
        }

        /// <summary>
        /// Gets the responder's SMAC
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte[] ResponderSmac
        {
            get
            {
                return m_ResponderSmac;
            }
        }

        #endregion

        #region Member Variables

        private byte[] m_InitiatorSmac;
        private byte[] m_ResponderSmac;

        #endregion
    }

    /// <summary>
    /// DSA signed event arguments
    /// </summary>
    public class DsaSignedEventArgs : EmberStatusEventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status">The status of the DSA sign</param>
        /// <param name="messageLength">The length of the message</param>
        /// <param name="messageContents">The message contents</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public DsaSignedEventArgs(EmberStatus status, byte messageLength, byte[] messageContents)
            : base(status)
        {
            m_MessageLength = messageLength;
            m_MessageContents = messageContents;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the length of the message
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
        /// Gets the message contents
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

        private byte m_MessageLength;
        private byte[] m_MessageContents;

        #endregion
    }
}
