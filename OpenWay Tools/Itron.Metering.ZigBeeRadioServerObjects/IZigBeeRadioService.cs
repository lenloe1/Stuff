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
//                              Copyright © 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace Itron.Metering.ZigBeeRadioServerObjects
{
    /// <summary>
    /// Specifies the service contract for the WCF service that will manage the ZigBee radios for
    /// all Itron applications.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IZigBeeRadioCallBack), SessionMode = SessionMode.Required)]
    public interface IZigBeeRadioService
    {
        /// <summary>
        /// Gets whether or not the service is currently set to scan for devices when a radio is 
        /// available for use.
        /// </summary>
        bool IsScanningDevices
        {
            [OperationContract(IsOneWay = false, IsInitiating = true, IsTerminating = false)]
            get;
        }

        /// <summary>
        /// Subscribes to device scanned events.
        /// </summary>
        [OperationContract(IsOneWay = true, IsInitiating = true, IsTerminating = false)]
        void SubscribeToScans();

        /// <summary>
        /// Unsubscribe to device scanned events.
        /// </summary>
        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        void UnsubscribeFromScans();

        /// <summary>
        /// Gets the number of radios that are currently available for use.
        /// </summary>
        int AvailableRadioCount
        {
            [OperationContract(IsOneWay = false, IsInitiating = true, IsTerminating = false)]
            get;
        }

        /// <summary>
        /// Request a ZigBee radio from the service.
        /// </summary>
        /// <returns>A token for the radio that has been given or null if no radio is available.</returns>
        [OperationContract(IsOneWay = false, IsInitiating = true, IsTerminating = false)]
        ZigBeeRadioToken RequestZigBeeRadio();

        /// <summary>
        /// Releases the specified radio token back to the radio management service.
        /// </summary>
        /// <param name="Radio">The radio token that is to be released.</param>
        [OperationContract(IsOneWay = false, IsInitiating = true, IsTerminating = false)]
        void ReleaseZigBeeRadio(ZigBeeRadioToken Radio);

        /// <summary>
        /// Gets a list of the devices that seen during the last scan of the ZigBee network.
        /// </summary>
        /// <returns>The list of devices that were seen.</returns>
        [OperationContract(IsOneWay = false, IsInitiating = true, IsTerminating = false)]
        List<ZigBeeDevice> GetVisibleDevices();

        /// <summary>
        /// Gets a list of all radios that are being managed by the service.
        /// </summary>
        /// <returns>The list of radios.</returns>
        [OperationContract(IsOneWay = false, IsInitiating = true, IsTerminating = false)]
        List<ZigBeeRadioToken> GetRadioInformation();
    }

    /// <summary>
    /// Call back interface for the ZigBee radio management service.
    /// </summary>
    public interface IZigBeeRadioCallBack
    {
        /// <summary>
        /// Notifies the client that a new scan is available.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void NotifyNetworkScanned(List<ZigBeeDevice> Devices);
    }
}
