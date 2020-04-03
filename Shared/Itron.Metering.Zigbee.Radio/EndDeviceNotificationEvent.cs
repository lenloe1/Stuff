///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential  
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or  
//  otherwise. Including photocopying and recording or in connection with any 
//  information storage or retrieval system without the permission in writing 
//  from Itron, Inc.
//
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Zigbee
{
    /// <summary>
    /// Handler delegate for the EndDeviceJoinedEvent.  Only the GasModule class
    /// should be handling this event. All application events will come from the 
    /// GasModule class via GasModuleEvents.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void EndDeviceJoinedEventHandler(object sender, EndDeviceJoinedEventArgs e);

    /// <summary>
    /// Event argument class for the end device joined event.
    /// </summary>
    public class EndDeviceJoinedEventArgs : EventArgs
    {
        /// <summary>
        /// Arguments for EndDeviceJoinedEvent. These arguements correspond to 
        /// IA's TC_AUTHENTICATION_indication primitive. This may need to be 
        /// modified once more devices are supported.
        /// </summary>
        /// <param name="PanID">PanID of device that joined</param>
        /// <param name="EndDevMAC">IEEE address of device that joined</param>
        /// <param name="ParentMAC">IEEE address of the coordinator of the 
        /// network the end device joined. This should always be the dongle's
        /// address.</param>
        /// <param name="SecureStatus">The security status the end device 
        /// joined with. 0=Secured Join, 1=Unsecured Join</param>
        public EndDeviceJoinedEventArgs(ushort PanID, ulong EndDevMAC,
            ulong ParentMAC, byte SecureStatus)
        {
            m_PanID = PanID;
            m_EndDeviceMAC = EndDevMAC;
            m_ParentMAC = ParentMAC;
            m_SecureStatus = SecureStatus;
        }

        /// <summary>PanID of device that joined</summary>
        public ushort PanID
        {
            get { return m_PanID; }
        }

        /// <summary>IEEE address of the coordinator of the/ network the end 
        /// device joined. This should always be the dongle's address.</summary>
        public ulong ParentMAC
        {
            get { return m_ParentMAC; }
        }

        /// <summary>IEEE address of device that joined</summary>
        public ulong EndDeviceMAC
        {
            get { return m_EndDeviceMAC; }
        }

        /// <summary>The security status the end device joined with. 
        /// 0=Secured Join, 1=Unsecured Join</summary>
        public byte SecureStatus
        {
            get { return m_SecureStatus; }
        }

        private ushort m_PanID;
        private ulong m_EndDeviceMAC;
        private ulong m_ParentMAC;

        // 0=Secured Join, 1=Unsecured Join
        private byte m_SecureStatus;
    }
}
