///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
// embodying substantial creative efforts and trade secrets, confidential 
// information, ideas and expressions. No part of which may be reproduced or 
// transmitted in any form or by any means electronic, mechanical, or 
// otherwise.  Including photocopying and recording or in connection with any
// information storage or retrieval system without the permission in writing 
// from Itron, Inc.
//
//                           Copyright © 2006 - 2009
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Management;
using System.Threading;
using System.Globalization;
using System.ServiceProcess;
using Itron.Metering.Zigbee;

using Itron.Metering.Utilities;

namespace Itron.Metering.ZigBeeRadioServerObjects
{
    /// <summary>
    /// The server object for the ZigBee radio manager service.
    /// </summary>

    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single, ConcurrencyMode=ConcurrencyMode.Single)]
    public class ZigBeeRadioService : IZigBeeRadioService, IDisposable
    {
        #region Constants

        private const string IA_USB_DONGLE_ID = "VID_1795&PID_600A";
        private const string IA_USB_DONGLE_ID2 = "VID_1795&PID_6006";
        private const string IA_HID_DONGLE_ID = "VID_1795&PID_6004";
        private const string BC_USB_RADIO_ID = "VID_0403+PID_6001";
        private const string TELEGESIS_RADIO_ID = "VID_10C4&PID_8293";
        private const int SCAN_PERIOD = 500;
        private static readonly int[] SCAN_CHANNELS = { 11, 15, 20, 25 };
        private const string SERVICE_NAME = "ZigBee Radio Manager";
        private const int SERVICE_TIMEOUT = 5000;
        private const uint NUMBER_OF_BEACONS = 30;

        #endregion

        #region Public Methods

        /// <summary>
        /// Default constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/08 RCG 1.00           Created

        public ZigBeeRadioService()
        {
            m_AvailableRadios = new List<ZigBeeRadioToken>();
            m_RadiosInUse = new List<ZigBeeRadioToken>();
            m_ZigBeeDevices = new List<ZigBeeDevice>();
            m_ScanningRadio = null;
            m_ScanningRadioSemaphore = new Semaphore(1, 1);
            m_ScanSubscribers = new List<OperationContext>();

            m_Logger = Logger.TheInstance;

            // To enable logging uncomment this line and specify the location you would like the log.
            // ------- Make sure that you comment this out when done. These logs can get big over time. ---------
            //m_Logger.Initialize(Logger.LoggingLevel.ZigBeeProtocol, @"C:\ZigBee Log\ZigBeeManager.log");

            m_InitiateScanTimer = new Timer(new TimerCallback(ScanForDevicesCallBack), null, Timeout.Infinite, SCAN_PERIOD);

            CreateUSBWatcher();

            UpdateRadioLists();
        }

        /// <summary>
        /// Deconstructor.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/08 RCG 1.00           Created

        ~ZigBeeRadioService()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes the object so that it can be garbage collected.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/08 RCG 1.00           Created

        public void Dispose()
        {
            if (m_USBEventWatcher != null)
            {
                // Make sure we stop the event watcher or we will get an
                // exception when the object is destructed
                m_USBEventWatcher.Stop();
                m_USBEventWatcher.Dispose();
            }

            if (m_InitiateScanTimer != null)
            {
                m_InitiateScanTimer.Change(Timeout.Infinite, Timeout.Infinite);
                m_InitiateScanTimer.Dispose();
            }

            if (m_ScanningRadioSemaphore != null)
            {
                m_ScanningRadioSemaphore.Close();
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Subscribes to device scanned events.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/14/08 RCG 1.00           Created
        public void SubscribeToScans()
        {
            OperationContext CurrentContext = OperationContext.Current;
            bool bSubscriberFound = false;
            bool bIsFirstSubscriber = m_ScanSubscribers.Count == 0;

            lock (m_ScanSubscribers)
            {
                // The comparisons don't seem to be working correctly with the 
                // OperationContext class so we need to check Session IDs manually

                foreach (OperationContext SubscriberContext in m_ScanSubscribers)
                {
                    if (SubscriberContext.SessionId.Equals(CurrentContext.SessionId, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        bSubscriberFound = true;
                        break;
                    }
                }

                if (bSubscriberFound == false)
                {
                    m_ScanSubscribers.Add(CurrentContext);

                    WriteLog(Logger.LoggingLevel.Minimal, "Client subscribed: " + CurrentContext.SessionId);
                }

                if (bIsFirstSubscriber)
                {
                    // Start the scan timer
                    m_InitiateScanTimer.Change(0, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Unsubscribe to device scanned events.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/14/08 RCG 1.00           Created

        public void UnsubscribeFromScans()
        {
            OperationContext CurrentContext = OperationContext.Current;
            List<OperationContext> ContextsToRemove = new List<OperationContext>();

            // The comparisons don't seem to be working correctly with the 
            // OperationContext class so we need to check Session IDs manually

            lock (m_ScanSubscribers)
            {
                foreach (OperationContext SubscriberContext in m_ScanSubscribers)
                {
                    if (SubscriberContext.SessionId.Equals(CurrentContext.SessionId, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        ContextsToRemove.Add(SubscriberContext);
                    }
                }

                foreach (OperationContext RemoveContext in ContextsToRemove)
                {
                    m_ScanSubscribers.Remove(RemoveContext);

                    WriteLog(Logger.LoggingLevel.Minimal, "Client unsubscribed: " + CurrentContext.SessionId);
                }

                // Stop the timer if there are no more subscribers
                if (m_ScanSubscribers.Count == 0)
                {
                    m_InitiateScanTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Requests a radio from the radio management service.
        /// </summary>
        /// <returns>A token to a ZigBee radio or null if no radios are available.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/08 RCG 1.00           Created
        //  08/25/09 AF  2.21.03 138987 Increased the timeout for the semaphore

        public ZigBeeRadioToken RequestZigBeeRadio()
        {
            ZigBeeRadioToken SelectedRadio = null;

            if (m_AvailableRadios.Count > 1)
            {
                // Get the first radio that is not scanning
                foreach (ZigBeeRadioToken RadioToken in m_AvailableRadios)
                {
                    if (m_ScanningRadio != RadioToken)
                    {
                        SelectedRadio = RadioToken;
                        break;
                    }
                }

                if (SelectedRadio != null)
                {
                    m_AvailableRadios.Remove(SelectedRadio);
                    SelectedRadio.Status = ZigBeeRadioToken.ZigBeeRadioStatus.InUse;
                    m_RadiosInUse.Add(SelectedRadio);
                }
            }
            else if(m_AvailableRadios.Count == 1)
            {
                // This must mean that the only radio available is currently scanning so we need to
                // wait for scanning to complete to continue. We need to limit the wait time so we
                // don't lock up the calling application.
                if (m_ScanningRadioSemaphore.WaitOne(200, false) == true)
                {

                    SelectedRadio = m_AvailableRadios[0];
                    m_ScanningRadio = null;

                    m_AvailableRadios.Remove(SelectedRadio);
                    SelectedRadio.Status = ZigBeeRadioToken.ZigBeeRadioStatus.InUse;
                    m_RadiosInUse.Add(SelectedRadio);

                    m_ScanningRadioSemaphore.Release();
                }
            }

            if (SelectedRadio != null)
            {
                WriteLog(Logger.LoggingLevel.Minimal, "Radio request succeeded: " + SelectedRadio.RadioIdentifier);
            }
            else if (m_AvailableRadios.Count > 0)
            {
                WriteLog(Logger.LoggingLevel.Minimal, "Radio request failed: Semaphore timeout");
            }
            else
            {
                WriteLog(Logger.LoggingLevel.Minimal, "Radio request failed: No available radios");
            }

            return SelectedRadio;
        }

        /// <summary>
        /// Releases the specified radio back to the radio management service.
        /// </summary>
        /// <param name="Radio">The radio object to be released.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/08 RCG 1.00           Created

        public void ReleaseZigBeeRadio(ZigBeeRadioToken Radio)
        {
            bool bRadioRemoved = false;

            if (Radio != null)
            {
                // Due to the nature of WCF we need to compare the identifiers to find the radio
                // references will not be the same across processes.
                foreach (ZigBeeRadioToken RadioToken in m_RadiosInUse)
                {
                    if (RadioToken.RadioType == Radio.RadioType 
                        && RadioToken.RadioIdentifier == Radio.RadioIdentifier)
                    {
                        bRadioRemoved = true;
                        m_RadiosInUse.Remove(RadioToken);
                        break;
                    }
                }

                if (bRadioRemoved)
                {
                    // We removed the radio from the list of radios in use so we can add it back to the available
                    m_AvailableRadios.Add(Radio);
                    Radio.Status = ZigBeeRadioToken.ZigBeeRadioStatus.Available;

                    WriteLog(Logger.LoggingLevel.Minimal, "Radio Released: " + Radio.RadioIdentifier);
                }
            }
        }

        /// <summary>
        /// Gets the list of devices that were seen during the last scan.
        /// </summary>
        /// <returns>The list of ZigBee devices that were seen last.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/08 RCG 1.00           Created

        public List<ZigBeeDevice> GetVisibleDevices()
        {
            return m_ZigBeeDevices;
        }

        /// <summary>
        /// Restarts the ZigBee Radio Manager Service
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/09/09 RCG 2.20.11        Created

        public static bool RestartService()
        {
            // First stop the service.
            bool bSuccess = true;

            StopService();
            // Wait a short amount of time and then restart the service.
            Thread.Sleep(1000);
            bSuccess = StartService();

            return bSuccess;
        }

        /// <summary>
        /// Starts the ZigBee Radio Manager Service
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/09/09 RCG 2.20.11        Created

        public static bool StartService()
        {
            ServiceController Service = new ServiceController(SERVICE_NAME);
            TimeSpan Timeout = TimeSpan.FromMilliseconds(SERVICE_TIMEOUT);
            bool bSuccess = true;

            try
            {
                Service.Start();
                Service.WaitForStatus(ServiceControllerStatus.Running, Timeout);
            }
            catch (Exception)
            {
                bSuccess = false;
            }

            return bSuccess;
        }

        /// <summary>
        /// Stops the ZigBee Radio Manager Service
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/09/09 RCG 2.20.11        Created

        public static bool StopService()
        {
            ServiceController Service = new ServiceController(SERVICE_NAME);
            TimeSpan Timeout = TimeSpan.FromMilliseconds(SERVICE_TIMEOUT);
            bool bSuccess = true;

            try
            {
                Service.Stop();
                Service.WaitForStatus(ServiceControllerStatus.Stopped, Timeout);
            }
            catch (Exception)
            {
                bSuccess = false;
            }

            return bSuccess;
        }

        /// <summary>
        /// Gets a list of all radios that are being managed by the service.
        /// </summary>
        /// <returns>The list of radios.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/03/08 RCG 1.00           Created

        public List<ZigBeeRadioToken> GetRadioInformation()
        {
            List<ZigBeeRadioToken> Radios = new List<ZigBeeRadioToken>();

            // Add all radios to the list.
            Radios.AddRange(m_AvailableRadios);
            Radios.AddRange(m_RadiosInUse);

            return Radios;
        }     

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the service is set to scan for ZigBee devices.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/08 RCG 1.00           Created

        public bool IsScanningDevices
        {
            get 
            {
                return m_ScanSubscribers.Count > 0;
            }
        }

        /// <summary>
        /// Gets the number of radios that are currently available.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/08 RCG 1.00           Created

        public int AvailableRadioCount
        {
            get 
            {
                return m_AvailableRadios.Count;
            }
        }        

        #endregion

        #region Private Methods

        /// <summary>
        /// Scans for available devices using an available radio. This method is meant to be used
        /// as a TimerCallBack delegate for m_InitiateScanTimer.
        /// </summary>
        /// <param name="stateInfo">The CallBack state info.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/08 RCG 1.00           Created

        private void ScanForDevicesCallBack(object stateInfo)
        {
            bool bScanSuccess = true;
            DateTime StartTime = DateTime.Now;

            m_ZigBeeDevices.Clear();

            // First check to see if there are any subscribers that would like a scan
            if (m_ScanSubscribers.Count > 0)
            {
                WriteLog(Logger.LoggingLevel.Minimal, "Scan starting");

                // Scan each of the channels individually in order to get better results.
                foreach (int iChannel in SCAN_CHANNELS)
                {

                    // Use the semaphore in case we are scanning with the only available radio.
                    if (m_AvailableRadios.Count > 0 && m_ScanningRadioSemaphore.WaitOne(0, false) == true)
                    {
                        // Use the last available radio so that it will be less likely to be requested.
                        m_ScanningRadio = m_AvailableRadios[m_AvailableRadios.Count - 1];

                        try
                        {
                            if (m_ScanningRadio != null)
                            {
                                switch (m_ScanningRadio.RadioType)
                                {
                                    case ZigBeeRadioToken.ZigBeeRadioType.USBRadio:
                                    case ZigBeeRadioToken.ZigBeeRadioType.TelegesisRadio:
                                    {
                                        WriteLog(Logger.LoggingLevel.Minimal, "\tScanning Channel " + iChannel.ToString(CultureInfo.InvariantCulture) + " using USB Radio");
                                        ScanUsingUSBBeltClipRadio(iChannel);
                                        break;
                                    }
                                    case ZigBeeRadioToken.ZigBeeRadioType.BluetoothRadio:
                                    {
                                        break;
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            WriteLog(Logger.LoggingLevel.Minimal, "Exception occurred: " + ex.Message);
                        }

                        WriteLog(Logger.LoggingLevel.Minimal, "\tChannel " + iChannel.ToString(CultureInfo.InvariantCulture) + " scan complete");

                        m_ScanningRadio = null;

                        // We are no longer using a radio to scan so set this back to null
                        m_ScanningRadioSemaphore.Release();
                    }
                    else
                    {
                        // The last radio has been taken
                        WriteLog(Logger.LoggingLevel.Minimal, "\tScan failed: No available radios");
                        bScanSuccess = false;
                        break;
                    }
                }

                if (bScanSuccess)
                {
                    NotifySubscribers();
                }

                if (m_ScanSubscribers.Count > 0)
                {
                    // Set the timer for the next scan
                    m_InitiateScanTimer.Change(SCAN_PERIOD, Timeout.Infinite);
                }
                else
                {
                    // There are no more subscribers so disable the scan
                    m_InitiateScanTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }

            WriteLog(Logger.LoggingLevel.Minimal, "Scan completion time: " + (DateTime.Now - StartTime).ToString());
        }

        /// <summary>
        /// Notifies all subscribers that a scan has completed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/25/09 RCG 2.21.03        Extracted method from ScanForDevicesCallback

        private void NotifySubscribers()
        {
            List<OperationContext> ContextsToRemove = new List<OperationContext>();

            WriteLog(Logger.LoggingLevel.Minimal, "Notifying Subscribers");

            lock (m_ScanSubscribers)
            {
                // Notify the clients
                foreach (OperationContext CurrentContext in m_ScanSubscribers)
                {
                    if (CurrentContext.Channel.State == CommunicationState.Opened)
                    {
                        try
                        {
                            WriteLog(Logger.LoggingLevel.Minimal, "\tNotifying: " + CurrentContext.SessionId);
                            IZigBeeRadioCallBack CallBack = CurrentContext.GetCallbackChannel<IZigBeeRadioCallBack>();
                            CallBack.NotifyNetworkScanned(m_ZigBeeDevices);
                        }
                        catch (Exception ex)
                        {
                            // The callback must have faulted for some reason we can just ignore it
                            // since it should be removed from the list.
                            WriteLog(Logger.LoggingLevel.Minimal, "Exception occurred: " + ex.Message);
                        }
                    }
                    else
                    {
                        // Looks like we lost a client we should remove them from the subscription
                        ContextsToRemove.Add(CurrentContext);
                    }
                }

                // Remove any dead contexts - This needs to be done outside of the previous loop
                // so we dont modify the list while going through it.
                foreach (OperationContext CurrentContext in ContextsToRemove)
                {
                    m_ScanSubscribers.Remove(CurrentContext);
                    WriteLog(Logger.LoggingLevel.Minimal, "Dead client removed: " + CurrentContext.SessionId);
                }
            }
        }

        /// <summary>
        /// Scan for devices using a USB Belt Clip Radio
        /// </summary>
        /// <param name="iChannel">The channel to scan.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/25/08 RCG 1.50.10        Created
        //  04/09/09 AF  2.20.00        Added code to distinguish between an Itron ZBCR
        //                              and a Telegesis dongle
        //  09/18/15 jrf 4.21.04 616082 Modified to send a beacon burst and encapsulated storing found networks 
        //                              in a new method.
        private void ScanUsingUSBBeltClipRadio(int iChannel)
        {
            Radio BeltClipRadio = new BeltClipRadio();
            ZigbeeNetwork[] ZigBeeNetworks = null;
            ZigbeeResult Result = ZigbeeResult.ERROR;

            if (m_ScanningRadio != null && iChannel == SCAN_CHANNELS[0])
            {
                if (ZigBeeRadioToken.ZigBeeRadioType.USBRadio == m_ScanningRadio.RadioType)
                {
                    ((BeltClipRadio)BeltClipRadio).RadioManufacturer = Itron.Metering.Zigbee.BeltClipRadio.RadioMfg.ItronZBCR;
                }
                else
                {
                    ((BeltClipRadio)BeltClipRadio).RadioManufacturer = Itron.Metering.Zigbee.BeltClipRadio.RadioMfg.TelegesisDongle;
                }

                BeltClipRadio.OpenPort(m_ScanningRadio.RadioIdentifier);                

                if (BeltClipRadio.IsOpen == true)
                {
                    uint BeaconChannels = 0;

                    WriteLog(Logger.LoggingLevel.Minimal, "\tBeacon Channels: ");
                    for (int iIndex = 0; iIndex < SCAN_CHANNELS.Length; iIndex++)
                    {
                        BeaconChannels |= (uint)(0x1 << SCAN_CHANNELS[iIndex]);
                        WriteLog(Logger.LoggingLevel.Minimal, "\t\t Channel " + (SCAN_CHANNELS[iIndex]).ToString(CultureInfo.InvariantCulture));
                    }

                    ZigBeeNetworks = BeltClipRadio.SendBeaconBurst(BeaconChannels);

                    //Store off networks found during burst.
                    StoreZigBeeNetworks(ZigBeeNetworks);

                    for (int iIndex = 0; iIndex < SCAN_CHANNELS.Length; iIndex++)
                    {
                        Result = BeltClipRadio.FindNetworks(BeaconChannels, out ZigBeeNetworks, true);

                        if (Result == ZigbeeResult.SUCCESS)
                        {
                            StoreZigBeeNetworks(ZigBeeNetworks);
                        }
                    }
                }

                BeltClipRadio.ClosePort();
            }
        }

        /// <summary>
        /// This method stores the ZigBee networks
        /// </summary>
        /// <param name="ZigBeeNetworks">Array of ZigBee Networks.</param>
        /// <param name="RSSI">Optional RSSI to use for IA Dongle scan.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/18/15 jrf 4.21.04 616082 Created.
        private void StoreZigBeeNetworks(ZigbeeNetwork[] ZigBeeNetworks, sbyte? RSSI = null)
        {
            foreach (ZigbeeNetwork CurrentNetwork in ZigBeeNetworks)
            {
                if (null == RSSI)
                {
                    RSSI = CurrentNetwork.LastHopRssi;
                }

                ZigBeeDevice CurrentDevice = new ZigBeeDevice(CurrentNetwork.ExPanID,
                    CurrentNetwork.LogicalChannel, RSSI.Value, DateTime.Now);

                if (CurrentDevice.IsItronDevice == true && (CurrentDevice.DeviceType == ZigbeeDeviceType.ELECTRIC_METER
                    || CurrentDevice.DeviceType == ZigbeeDeviceType.CELL_RELAY) && false == m_ZigBeeDevices.Contains(CurrentDevice))
                {
                    m_ZigBeeDevices.Add(CurrentDevice);
                }
            }
        }

        /// <summary>
        /// Creates a ManagementEventWatcher that will raise an event when a usb device is added.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/08 RCG 1.00           Created
        //  12/08/10 RCG 2.45.16 161914 Increasing WithinInterval time to reduce CPU Usage

        private void CreateUSBWatcher()
        {
            ManagementScope Scope = new ManagementScope("root\\CIMV2");
            WqlEventQuery Query = new WqlEventQuery();

            Scope.Options.EnablePrivileges = true;

            Query.EventClassName = "__InstanceOperationEvent";
            Query.WithinInterval = new TimeSpan(0, 0, 0, 0, 1000);
            Query.Condition = @"TargetInstance ISA 'Win32_USBControllerDevice'";

            m_USBEventWatcher = new ManagementEventWatcher(Scope, Query);
            m_USBEventWatcher.EventArrived += new EventArrivedEventHandler(m_USBEventWatcher_EventArrived);
            m_USBEventWatcher.Start();
        }

        /// <summary>
        /// Handles the USB event watcher EventArrived event.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/08 RCG 1.00           Created

        private void m_USBEventWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            UpdateRadioLists();
        }

        /// <summary>
        /// Gets the list of IA ZigBee radios that are currently connected to the computer.
        /// </summary>
        /// <returns>The list of device addresses for all connected radios.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/08 RCG 1.00           Created

        private static List<ZigBeeRadioToken> GetIADongleRadioList()
        {
            List<ZigBeeRadioToken> RadioList = new List<ZigBeeRadioToken>();

            // Set up the Query to WMI Win32_PnPEntity
            SelectQuery Query = new SelectQuery("Win32_PnPEntity");
            ManagementObjectSearcher Searcher = new ManagementObjectSearcher(Query);

            // Check the available objects to see if any IADongles are present.
            foreach (ManagementObject Object in Searcher.Get())
            {
                // The DeviceID has three parts seperated by '\' so we need to break it down to get the device address
                // ex: USB\VID_1795&PID_600A\B911000000EB0D00 or [Connection]\[VID & PID]\[Device Address]
                string[] strSeparator = { @"\" };
                string[] DeviceID = Object["DeviceID"].ToString().Split(strSeparator, StringSplitOptions.None);

                // Find the IA Dongle by checking against the VID and PID
                if (DeviceID[0] == "USB" && (DeviceID[1] == IA_USB_DONGLE_ID || DeviceID[1] == IA_HID_DONGLE_ID || DeviceID[1] == IA_USB_DONGLE_ID2))
                {
                    // The radio's connect requires that the device address to be in the format XX:XX:XX:XX:XX:XX:XX:XX so
                    // we need to convert it first
                    StringBuilder strIEEEAddress = new StringBuilder();

                    for (int iIndex = 0; iIndex < DeviceID[2].Length; iIndex += 2)
                    {
                        strIEEEAddress.Append(DeviceID[2].Substring(iIndex, 2));
                        if (iIndex + 2 < DeviceID[2].Length)
                        {
                            strIEEEAddress.Append(":");
                        }
                    }

                    // Now that we have the address in the correct format add it to list
                    RadioList.Add(new ZigBeeRadioToken(ZigBeeRadioToken.ZigBeeRadioType.IADongle, strIEEEAddress.ToString()));
                }
            }

            return RadioList;
        }

        /// <summary>
        /// Gets the list of USB Belt Clip Radios that are currently connected to the computer.
        /// </summary>
        /// <returns>The list of USB Belt Clip Radios</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  02/25/08 RCG 1.50.10           Created
        //  04/20/17 jrf 4.72.00 WR 573799 Added code to verify Beltclip radio is indeed a Beltclip radio.
        private List<ZigBeeRadioToken> GetUSBRadioList()
        {
            List<ZigBeeRadioToken> RadioList = new List<ZigBeeRadioToken>();

            // Set up the Query to WMI Win32_PnPEntity
            SelectQuery Query = new SelectQuery("Win32_PnPEntity");
            ManagementObjectSearcher Searcher = new ManagementObjectSearcher(Query);

            // Check the available objects to see if any IADongles are present.
            foreach (ManagementObject Object in Searcher.Get())
            {
                // The DeviceID has three parts separated by '\' so we need to break it down to get the device address
                string[] strSeparator = { @"\" };
                string[] DeviceID = Object["DeviceID"].ToString().Split(strSeparator, StringSplitOptions.None);

                // Find the USB Radio by checking against the VID and PID
                if (DeviceID[0] == "FTDIBUS" && DeviceID[1].Contains(BC_USB_RADIO_ID))
                {
                    // The COM Port is in the name of the entry we are looking for so we need to parse it out.
                    string strRadioName = Object["Name"].ToString();
                    int iStartIndex = strRadioName.IndexOf('(');
                    int iStopIndex = strRadioName.IndexOf(')');

                    // Make sure the indices are valid so we can get the correct port name. Without the port we can't really use the radio
                    if (iStartIndex >= 0 && iStartIndex < strRadioName.Length && iStopIndex > iStartIndex && iStopIndex < strRadioName.Length)
                    {
                        BeltClipRadio BeltClipRadio = new BeltClipRadio();
                        ZigBeeRadioToken NewUSBRadio = new ZigBeeRadioToken(ZigBeeRadioToken.ZigBeeRadioType.USBRadio, strRadioName.Substring(iStartIndex + 1, iStopIndex - iStartIndex - 1));

                        //We are having non radio devices getting flagged as beltclip radios.
                        //So let's try to connect to radio to rule out false positives.
                        try
                        {
                            BeltClipRadio.OpenPort(NewUSBRadio.RadioIdentifier);

                            if (BeltClipRadio.IsOpen == true)
                            {
                                if (BeltClipRadio.C177App.IsConnected)
                                {
                                    //We connected so we know it's good.
                                    RadioList.Add(NewUSBRadio);
                                }
                            }
                        }
                        catch { /*Ignore exception*/}
                        finally
                        {
                            BeltClipRadio.ClosePort();
                        }
                    }
                }
                else if (DeviceID[0] == "USB" && DeviceID[1].Contains(TELEGESIS_RADIO_ID))
                {
                    // The COM Port is in the name of the entry we are looking for so we need to parse it out.
                    string strRadioName = Object["Name"].ToString();
                    int iStartIndex = strRadioName.IndexOf('(');
                    int iStopIndex = strRadioName.IndexOf(')');

                    // Make sure the indices are valid so we can get the correct port name. Without the port we can't really use the radio
                    if (iStartIndex >= 0 && iStartIndex < strRadioName.Length && iStopIndex > iStartIndex && iStopIndex < strRadioName.Length)
                    {
                        RadioList.Add(new ZigBeeRadioToken(ZigBeeRadioToken.ZigBeeRadioType.TelegesisRadio, strRadioName.Substring(iStartIndex + 1, iStopIndex - iStartIndex - 1)));
                    }
                }
            }

            return RadioList;
        }

        /// <summary>
        /// Updates the list of radios that are currently connected to the computer.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/08 RCG 1.00           Created

        private void UpdateRadioLists()
        {
            List<ZigBeeRadioToken> IARadioAddresses = GetIADongleRadioList();
            List<ZigBeeRadioToken> USBRadioAdresses = GetUSBRadioList();
            List<ZigBeeRadioToken> AllRadios = new List<ZigBeeRadioToken>();

            AllRadios.AddRange(IARadioAddresses);
            AllRadios.AddRange(USBRadioAdresses);

            AddNewRadios(AllRadios);
            RemoveUnusedRadios(AllRadios);
        }

        /// <summary>
        /// Removes any radios that are no longer connected to the computer.
        /// </summary>
        /// <param name="NewRadios">The list of radios that are connected to the computer.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/08 RCG 1.00           Created

        private void RemoveUnusedRadios(List<ZigBeeRadioToken> NewRadios)
        {
            List<ZigBeeRadioToken> AvailableRadios = new List<ZigBeeRadioToken>();
            List<ZigBeeRadioToken> InUseRadios = new List<ZigBeeRadioToken>();

            // Check the available list
            foreach (ZigBeeRadioToken AvailableRadioToken in m_AvailableRadios)
            {
                bool bRadioFound = false;

                // Make sure that the radio is still in the list.
                foreach (ZigBeeRadioToken NewRadioToken in NewRadios)
                {
                    if (NewRadioToken.Equals(AvailableRadioToken) == true)
                    {
                        bRadioFound = true;
                        break;
                    }
                }

                if (bRadioFound == true)
                {
                    // We dont want to modify the list we are currently using so add it to
                    // a temporary list
                    AvailableRadios.Add(AvailableRadioToken);
                }
            }


            // Check the in use list
            foreach (ZigBeeRadioToken InUseRadioToken in m_RadiosInUse)
            {
                bool bRadioFound = false;

                // Make sure that the radio is still in the list.
                foreach (ZigBeeRadioToken NewRadioToken in NewRadios)
                {
                    if (NewRadioToken.Equals(InUseRadioToken) == true)
                    {
                        bRadioFound = true;
                        break;
                    }
                }

                if (bRadioFound == true)
                {
                    // We dont want to modify the list we are currently using so add it to
                    // a temporary list
                    InUseRadios.Add(InUseRadioToken);
                }
            }

            // Assign the new list of radios
            m_AvailableRadios = AvailableRadios;
            m_RadiosInUse = InUseRadios;
        }

        /// <summary>
        /// Adds radio tokens to the available list if they are new.
        /// </summary>
        /// <param name="NewRadios">The list of currently attached IA radios.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/08 RCG 1.00           Created

        private void AddNewRadios(List<ZigBeeRadioToken> NewRadios)
        {
            foreach (ZigBeeRadioToken NewRadioToken in NewRadios)
            {
                bool bRadioFound = false;

                // Check the list of available radios
                foreach(ZigBeeRadioToken RadioToken in m_AvailableRadios)
                {
                    if (NewRadioToken.Equals(RadioToken) == true)
                    {
                        bRadioFound = true;
                        break;
                    }
                }

                // Check the list of radios in use
                if (bRadioFound == false)
                {
                    foreach (ZigBeeRadioToken RadioToken in m_RadiosInUse)
                    {
                        if (NewRadioToken.Equals(RadioToken) == true)
                        {
                            bRadioFound = true;
                            break;
                        }
                    }
                }

                if (bRadioFound == false)
                {
                    // We found a new radio so add it to the list
                    m_AvailableRadios.Add(NewRadioToken);
                }
            }
        }

        /// <summary>
        /// Writes a log entry to the log file.
        /// </summary>
        /// <param name="Level">The level of the message you would like to write.</param>
        /// <param name="strMessage">The message you would like to write.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/24/08 RCG 1.00           Created

        private void WriteLog(Logger.LoggingLevel Level, string strMessage)
        {
            m_Logger.WriteLine(Level, DateTime.Now.ToString() + ": " + strMessage);
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// The list of radios that are currently available
        /// </summary>
        private List<ZigBeeRadioToken> m_AvailableRadios;

        /// <summary>
        /// The list of radios that are currently in use
        /// </summary>
        private List<ZigBeeRadioToken> m_RadiosInUse;

        /// <summary>
        /// The list of devices that were seen in the last scan
        /// </summary>
        private List<ZigBeeDevice> m_ZigBeeDevices;

        /// <summary>
        /// Watches for a USB device to be inserted or removed
        /// </summary>
        private ManagementEventWatcher m_USBEventWatcher;

        /// <summary>
        /// The radio that is currently being used for scanning
        /// </summary>
        private ZigBeeRadioToken m_ScanningRadio;

        /// <summary>
        /// Semaphore used to prevent giving the scanning radio away while it is in use.
        /// </summary>
        private Semaphore m_ScanningRadioSemaphore;

        /// <summary>
        /// Timer used to initiate a scan.
        /// </summary>
        private Timer m_InitiateScanTimer;

        /// <summary>
        /// The list of clients that have subscribed to the scan events
        /// </summary>
        private List<OperationContext> m_ScanSubscribers;

        /// <summary>
        /// Logger used for debug purposes.
        /// </summary>
        private Logger m_Logger;

        #endregion
    }
}
