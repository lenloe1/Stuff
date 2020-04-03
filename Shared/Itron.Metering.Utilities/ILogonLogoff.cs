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
    /// Delegate for the Logon event handler
    /// </summary>
    /// <param name="sender">The object that sent the event.</param>
    /// <param name="e">The event arguments</param>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  03/07/08 AF  1.5.0          Changed 2nd param to LogonEventArgs to support
    //                              ZigBee logon
    //
    public delegate void LogonEventHandler(object sender, LogonEventArgs e);
     
    /// <summary>
    /// Delegate for finding a Cell Relay event handler
    /// </summary>
    /// <param name="sender">The object that sent the event.</param>
    /// <param name="e">The event arguments</param>
    public delegate void FindCellRelayNetworkEventHandler(object sender, EventArgs e);
     
    /// <summary>
    /// Delegate for finding a Cell Relay event handler
    /// </summary>
    /// <param name="sender">The object that sent the event.</param>
    /// <param name="e">The event arguments</param>
    public delegate void FindCellRelayEventHandler(object sender, EventArgs e);
     
    /// <summary>
    /// Delegate for the Logon to a Cell Relay event handler
    /// </summary>
    /// <param name="sender">The object that sent the event.</param>
    /// <param name="e">The event arguments</param>
    public delegate void LogonCellRelayEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Delegate for the Logoff event handler
    /// </summary>
    /// <param name="sender">The object that sent the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void LogoffEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Logon and Logoff Interface
    /// </summary>
    public interface ILogonLogoff
    {
        /// <summary>
        /// Event for logging on to the device
        /// </summary>
        event LogonEventHandler LogonEvent;

        /// <summary>
        /// Event for logging off of the device
        /// </summary>
        event LogoffEventHandler LogoffEvent;

        /// <summary>
        /// Event for finding a Cell Relay Network via Zigbee
        /// </summary>
        event FindCellRelayNetworkEventHandler FindCellRelayNetworkEvent;

        /// <summary>
        /// Event for finding a Cell Relay via Zigbee
        /// </summary>
        event FindCellRelayEventHandler FindCellRelayEvent;

        /// <summary>
        /// Event for logging on to a Cell Relay via Zigbee
        /// </summary>
        event LogonCellRelayEventHandler LogonCellRelayEvent;

    }
}
