///////////////////////////////////////////////////////////////////////////////
//
//                   PROPRIETARY RIGHTS NOTICE:
//
// ALL RIGHTS RESERVED.  THIS MATERIAL CONTAINS THE VALUABLE
// PROPERTIES AND TRADE SECRETS OF
//
//                           ITRON INC.
//                      WEST UNION,SC, USA,
//
//             EMBODYING SUBSTANTIAL CREATIVE EFFORTS
// AND TRADE SECRETS, CONFIDENTIAL INFORMATION, IDEAS AND EXPRESSIONS, NO PART OF
// WHICH MAY BE REPRODUCED OR TRANSMITTED IN ANY FORM OR BY ANY MEANS
// ELECTRONIC, MECHANICAL, OR OTHERWISE, INCLUDING PHOTOCOPYING AND
// RECORDING OR IN CONNECTION WITH ANY INFORMATION STORAGE OR
// RETRIEVAL SYSTEM WITHOUT THE PERMISSION IN WRITING FROM ITRON
//                       Copyright 2008 - 2016
//                               ITRON
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;                    //For the sleep function 
using System.Timers;
using System.Runtime.InteropServices;       //For the DLL
using System.Globalization;
using System.Windows.Forms;
using Itron.Metering.Utilities;

// This class represents the Itron BeltClip Radio. This is a  full function
// USB/BlueTooth Zigbee radio.
namespace Itron.Metering.Zigbee
{
    /// <summary>
    /// This class represents the Itron BeltClip Zigbee Radio.
    /// </summary>
    public partial class BeltClipRadio : Radio
    {
        /// <summary>Notification event for an end device joining the radio's 
        /// network when the radio is configured as a Trust Center. Only the
        /// GasModule class should be handling this event. All application 
        /// events will come from the GasModule class via GasModuleEvents.
        /// </summary>
        public override event EndDeviceJoinedEventHandler EndDeviceJoinedEvent;

        #region Constants

        private const byte MAX_TX_POWER = 3;
        private byte[] PROGRAMMER_EPID = { 0x00, 0x07, 0x81, 0x04, 0x00, 0x00, 0x00, 0x01 };

#if DEBUG
        private const uint CHANNEL_16 = 0x000010000;
        private const uint CHANNEL_17 = 0x000020000;
#endif

        private const string ENCRYPTED_LINK_KEY_NAME = "GlobalLinkKey";

        private const int BEACON_PERIOD = 500;

        private static readonly int[] SCAN_CHANNELS = { 11, 15, 20, 25 };

        /// <summary>
        /// PSoC command ids
        /// </summary>
        public enum PSoCCommands
        {
            /// <summary>
            /// Command id for command to retrieve the firmware version of the PSoC
            /// </summary>
            FIRMWARE_VERSION = 0x01,
            /// <summary>
            /// Command id for command to get the battery voltage in millivolts
            /// </summary>
            BATTERY_VOLTAGE = 0x02,
            /// <summary>
            /// Command id for command to get the battery level
            /// </summary>
            BATTERY_LEVEL = 0x03,
            /// <summary>
            /// Command id for command to get the board temperature in Celsius
            /// </summary>
            BATTERY_TEMPERATURE = 0x04,
            /// <summary>
            /// Command id for command that will return the PSoC version and the
            /// battery fuel gauge information
            /// </summary>
            PSOC_STATUS = 0x50,
        }

        /// <summary>
        /// Radio Manufacturer
        /// </summary>
        public enum RadioMfg
        {
            /// <summary>
            /// Itron made Belt Clip Radio
            /// </summary>
            ItronZBCR = 0,
            /// <summary>
            /// Telegesis made dongle
            /// </summary>
            TelegesisDongle = 1,
        }

        /// <summary>
        /// Join method (association or secure rejoin)
        /// </summary>
        public enum JoinMethod : byte
        {
            /// <summary>
            /// Join by MAC association (joining must be turned on)
            /// </summary>
            MAC_ASSOCIATION = 0,
            /// <summary>
            /// Join by Secure Rejoin (joining can be turned off)
            /// </summary>
            SECURE_REJOIN = 2,
        }

        #endregion Constants

        #region Public Methods

        /// <summary>
        /// Instantiates the IAZigbee and attempts to connect to the radio. The
        /// connect attempt will throw a ZigbeeException if a radio is not 
        /// available.  Callers of this constructor should catch this exception
        /// to determine if a radio is available.  After connecting to the 
        /// radio, this method resets it (necessary to get disconnect to work)
        /// and disconnects, so radio is off but ready to go.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/31/08 mcm 1.0.x   Initial Release
        // 04/09/09 AF  2.20.00        Added code to distinguish between an 
        //                             Itron ZBCR and a Telegesis dongle
        // 10/26/12 PGH 2.70.36        Added C177App instantiation
        // 09/17/15 jrf 4.21.04 616082 Added beacon timer.
        // 09/06/16 AF  4.60.05 682273 Switched the timer to a System.Timers timer for thread safety
        //
        public BeltClipRadio()
        {
            EndDeviceJoinedEvent += null;
            RadioManufacturer = RadioMfg.ItronZBCR;
            
            C177App = new ZigBeeC177Application();
            C177App.IPPDataResponseReceived += new IPPDataResponseEventHandler(C177App_IPPDataResponseReceived);

            //Initial timer is off
            if (null == m_BeaconTimer)
            {
                m_BeaconTimer = new System.Timers.Timer();
                m_BeaconTimer.Elapsed += new ElapsedEventHandler(HandleBeaconing);
                m_BeaconTimer.Enabled = false;
            }
        }

        /// <summary>
        /// Stops the radio, frees the libraries
        /// </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/31/08 mcm 1.0.x          Initial Release
        // 09/17/15 jrf 4.21.04 616082 Making sure timer gets disposed of. 
        ~BeltClipRadio()
        {
            try
            {
                if (IsOpen)
                {
                    ClosePort();
                }

                if (m_gchEventHandler.IsAllocated)
                {
                    m_gchEventHandler.Free();
                }

                DestroyBeaconTimer();
            }
            catch { }
            finally
            {
            }
        }

        /// <summary>
        /// Starts EZSP level logging
        /// </summary>
        /// <param name="filePath">The path to the log file</param>
        /// <param name="logLevel">The logging level</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/13/13 RCG 2.70.72 327410 Created

        public void StartLogging(string filePath, EZSPLogLevels logLevel)
        {
            C177App.StartLogging(filePath, logLevel);
        }

        /// <summary>
        /// Connects to radio hardware. This method does not start the radio.
        /// This method should be used to verify the hardware exists and is
        /// available for use.  Does mo harm to call this method while 
        /// connected to the radio.
        /// </summary>
        /// <returns>true if the hardware is available</returns>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/31/08 mcm 1.0.x   
        ///</remarks>
        public override bool Connect(IntPtr hWnd)
        {
            return true;
        } // Connect

        /// <summary>
        /// Connects to radio hardware with the specified device address.
        /// </summary>
        /// <param name="hWnd">The handle to use for the connection.</param>
        /// <param name="DevAddr">The address of the device to use.</param>
        /// <returns>true if the connection was successful.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/08 RCG 1.00           Created

        public override bool Connect(IntPtr hWnd, string DevAddr)
        {
            return true;
        }

        /// <summary>
        /// Disconnects from radio hardware. This method will stop the radio.
        /// </summary>
        /// <returns>true if the hardware is available</returns>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/31/08 mcm 1.0.x   
        ///</remarks>
        public override void Disconnect()
        {
            try
            {
            }
            catch { }
            finally
            {
            }
        }

        /// <summary>Starts the Radio. Returns a bool indicating success.
        /// </summary>
        /// <param name="MAC">MAC address for the radio</param>
        /// <param name="LogicalType">The type of device to configure.  When
        /// joined to a cell relay, this should be a router, otherwise this
        /// should be a coordinator</param>
        /// <param name="ScanChannels">Packed bits representing the channels
        /// to search.  Only channels 15-26 are valid, so only bits 15 (0x800)
        /// through bit 26 (0x4000000).  Note that bits are 0 indexed, so 
        /// bit 0 = 0x01.</param>
        /// <param name="ExPanID">The 8 byte extended Pan ID you want to start 
        /// with.  This value can be 0, which will cause the radio to either 
        /// assign one at random or join the first suitable network it finds 
        /// depending on the LogicalType.</param>
        /// <returns>True if the radio exists and was successfully started</returns>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/31/08 mcm 1.0.x   Initial Release
        // 06/17/08 AF  1.50.37         Changed to read the link key from the registry
        // 09/15/08 AF                  Replaced C177 with EZSP because
        //                              the interface names changed in the new version
        //                              of the EZSP dlls.
        // 10/07/08 AF          121097  Modified the filter on channel.  We should allow
        //                              any valid channel
        // 01/26/11 AF  2.45.26 158436  Integration of new ezsp library to support secure rejoin.
        //
        // 10/17/12 PGH 2.70.36        Replaced EZSP with C177App
        // 09/17/15 jrf 4.21.04 616082 Sending a burst of beacons before joining and 
        //                             starting intermittent beacons if method is successful.
        public override ZigbeeResult Start(ulong MAC, ulong ExPanID,
            ZigbeeLogicalType LogicalType, uint ScanChannels)
        {
            ZigbeeResult Result = ZigbeeResult.SUCCESS;

            if (!m_bConnected)
            {
                Result = ZigbeeResult.NOT_CONNECTED;
            }
            else
            {
                try
                {
                    if (C177App.IsJoined)
                    {
                        C177App.LeaveNetwork();
                    }

                    byte[] NetworkKey = GetSecurityKey(false); 
                    
                    //Need to wake up troublesome devices before we attempt to join.
                    SendBeaconBurst((uint)(0x1 << (int)ScanChannels));


                    C177App.Rejoin(ConvertLogicalTypeToEmberType(LogicalType), ExPanID, (byte)ScanChannels, NetworkKey);

                    if (!C177App.IsJoined)
                    {
                        Result = ZigbeeResult.ERROR;
                    }

                }
                catch (Exception e)
                {
                    m_Logger.WriteDetail(this, "BeltClipRadio.Start exception");
                    m_Logger.WriteException(this, e);
                    Result = ZigbeeResult.ERROR;
                    throw e;
                }

            }

            if (ZigbeeResult.SUCCESS == Result)
            {
                //This will keep troublesome devices communicative.
                StartIntermittentBeacons();
            }

            return Result;

        } // Start

        /// <summary>Stops the Radio. </summary>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/31/08 mcm 1.0.x          Initial Release
        // 09/26/08 AF                 Added implementation
        // 10/06/08 AF  2.00           Commented out the WaitEvent() and increased
        //                             the amount of Sleep time to increase the
        //                             success rate.
        //
        // 10/17/12 PGH 2.70.36        Replaced EZSP with C177App
        // 
        public override void Stop()
        {
            if (C177App.IsJoined)
            {
                C177App.LeaveNetwork();
            }            

        } // Stop

        /// <summary>
        /// Find the Networks around the radio
        /// </summary>
        /// <param name="ScanChannels">Packed bits representing the channels
        /// to search.  Only channels 15-26 are valid, so only bits 15 (0x800)
        /// through bit 26 (0x4000000).  Note that bits are 0 indexed, so 
        /// channel 0 = 0x01.</param>
        /// <param name="Networks">Returned array of found networks</param>
        /// <param name="Fast">Indicates whether or not the duration period 
        /// used during scan should be set to a small number.</param>
        /// <returns>ZigbeeResult indicating success of search</returns>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/31/08 mcm 1.0.x   Initial Release
        // 09/08/08 AF                  Increased the Sleep() to give the scan
        //                              enough time to complete.  In HH-Pro we
        //                              weren't getting many channel 25 networks
        // 08/24/09 AF  2.21.03 138987  Replaced the Sleep() with a WaitEvent()
        //                              so that we will wait only as long as necessary
        //
        // 10/17/12 PGH 2.70.36        Replaced EZSP with C177App
        // 09/17/15 jrf 4.21.04 616082 Added a fast parameter to cause scan duration to 
        //                             be set to a lower number of periods (speeds scan up)
        //                             and also added in a monitor in order to synchronize 
        //                             communication with the beacon timer.
        // 11/03/15 jrf 4.22.00 629782 Adding null referenced checks for m_NetworkList
        public override ZigbeeResult FindNetworks(uint ScanChannels,
            out ZigbeeNetwork[] Networks, bool Fast = false)
        {
            ZigbeeResult Result = ZigbeeResult.SUCCESS;
            EZSPScanDuration ScanDuration = EZSPScanDuration.ScanPeriodX33;

            PauseIntermittentBeacons();

            Monitor.Enter(C177App);
            try
            {

                if (Fast)
                {
                    ScanDuration = EZSPScanDuration.ScanPeriodX2;
                }

                Networks = new ZigbeeNetwork[0];

                m_Logger.WriteDetail(this, "Searching for Zigbee Networks");

                if (null != m_NetworkList)
                {
                    m_NetworkList.Clear();
                }

                try
                {
                    m_NetworkList = C177App.ScanForDevices((ZigBeeChannels)ScanChannels, ScanDuration);

                    if (null != m_NetworkList)
                    {
                        m_NetworkList.Sort();
                        Networks = m_NetworkList.ToArray();
                    }
                }
                catch (Exception e)
                {
                    m_Logger.WriteException(this, e);
                    Result = ZigbeeResult.ERROR;
                }
            }
            finally
            {
                Monitor.Exit(C177App);
            }

            ResumeIntermittentBeacons();

            return Result;

        } // FindNeworks

        /// <summary>Sends a Request to given cluster and returns any data that 
        /// comes back. This method is usually used for a table read.</summary>
        /// <param name="TargetAddress">Short address of device to send the message to</param>
        /// <param name="Msg"></param>
        /// <param name="Response"></param>
        /// <returns></returns>
        /// <remarks>
        ///Revision History	
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------
        ///01/31/08 mcm 1.0.x   Initial Release
        ///</remarks>
        public override ZigbeeResult SendUnencryptedOTA(ushort TargetAddress,
            byte[] Msg, out byte[] Response)
        {
            ZigbeeResult Result = ZigbeeResult.SUCCESS;

            Response = null;

            //Add in Monitor if implement this method. See Send Data Request.

            return Result;

        } // SendUnencryptedOTA

        /// <summary>Sends a Request to given cluster and returns any data that 
        /// comes back. This method is usually used for a table read.</summary>
        /// <param name="TargetAddress">Short address of device to send the message to</param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/31/08 mcm 1.0.x   Initial Release
        // 09/15/08 AF                  Replaced ItronC177 with ItronEZSP because
        //                              the interface names changed in the new version
        //                              of the EZSP dlls.
        // 10/26/12 PGH                 Replaced EZSP with C177App
        // 09/17/15 jrf 4.21.04 616082 Added in a monitor in order to synchronize communication 
        //                             with the beacon timer. 
        public override ZigbeeResult SendDataRequest(ushort TargetAddress,
            byte[] Msg)
        {
            ZigbeeResult Result = ZigbeeResult.SUCCESS;

            PauseIntermittentBeacons();

            Monitor.Enter(C177App);

            try
            {
                if (null != Msg)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Sending data request to address " + TargetAddress.ToString("X4", CultureInfo.InvariantCulture));

                    try
                    {
                        C177App.SendDataRequest(Msg);
                    }
                    catch (Exception e)
                    {
                        m_Logger.WriteException(this, e);
                        Result = ZigbeeResult.ERROR;
                    }                    
                }
            }
            finally
            {
                Monitor.Exit(C177App);
            }

            ResumeIntermittentBeacons();

            return Result;

        } // SendDataRequest

        /// <summary>
        /// When the radio is running as a coordinator and Trust Center for
        /// Gas and Water Modules and one of those end devices joins the 
        /// network, the radio class will raise a EndDeviceJoinedEvent. The
        /// client application must handle that event, decide whether to allow
        /// the device to join, and authenticate the device before the radio
        /// times out.  Call this methods to authenticate the device.
        /// </summary>
        /// <param name="AllowToJoin">True to allow the end device to join the
        /// network.  False to remove it from the network.</param>
        /// <param name="MAC">The MAC (IEEE) address of the device to 
        /// authenticate. This value is passed to the client in the 
        /// EndDeviceJoinedEvent's arguement.</param>
        /// <param name="ParentMAC">The MAC address of the parent the end
        /// device joined. This should always be this radio's MAC.  This value
        /// is passed to the client in the EndDeviceJoinedEvent's arguement.
        /// </param>
        /// <param name="SecureStatus">The security status the end device
        /// joined with This value is passed to the client in the 
        /// EndDeviceJoinedEvent's arguement.</param>
        public override void Authenticate(bool AllowToJoin, ulong MAC,
            ulong ParentMAC, byte SecureStatus)
        {
            //Add in Monitor if implement this method. See Send Data Request.
        }

        /// <summary>
        /// Queries the Belt Clip Radio's PSoC layer for hardware specific information.
        /// This method assumes that the port has been opened.
        /// </summary>
        /// <param name="PSoCVersion">Firmware version of the PSoC</param>
        /// <param name="BatteryVoltage">Battery voltage in millivolts</param>
        /// <param name="BatteryTemperature">Board temperature in degrees Celsius</param>
        /// <param name="BatteryLevel">Battery level as a percent of battery capacity left</param>
        /// <param name="BatteryTimeToEmpty">
        /// Battery time to empty.  Note: A battery time to empty value of 0xFFFF (65535)
        /// indicates that the battery is not being discharged
        /// </param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 AF  1.50.26        Created
        //  08/21/08 AF  1.52.00        Code cleanup -- GetPSoCStatus is no longer
        //                              a Radio interface since it is not supported
        //                              by the dongle.
        //  09/15/08 AF                 Replaced C177 with EZSP because the interface
        //                              names changed in the new version of the EZSP dlls.
        // 12/21/10 AF  2.45.21 158436  Integration of new ezsp library to support secure rejoin.
        //
        // 10/26/12 PGH 2.70.36         Disabled EZSP
        public void GetPSoCStatus(out string PSoCVersion, out double BatteryVoltage,
                                  out byte BatteryTemperature, out byte BatteryLevel,
                                  out int BatteryTimeToEmpty)
        {
            PSoCVersion = "";
            BatteryVoltage = 0.0;
            BatteryTemperature = 0;
            BatteryLevel = 0;
            BatteryTimeToEmpty = 0;
        }

        /// <summary>
        /// This method turns on the beacon timer.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/15 jrf 4.21.04 616082 Created
        public void StartIntermittentBeacons()
        {
            //The folowing two statements must be in this order
            m_BeaconTimerRunning = true;
            ResumeIntermittentBeacons();
        }

        /// <summary>
        /// This method turns off the beacon timer.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/15 jrf 4.21.04 616082 Created
        public void StopIntermittentBeacons()
        {
            //The folowing two statements must be in this order
            PauseIntermittentBeacons();
            m_BeaconTimerRunning = false;
        }


        /// <summary>
        /// This method is called by timer to send beacon.
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/16/15 jrf 4.21.04 616082 Created
        //  09/06/16 AF  4.60.05 682273 Added DoEvents to make sure the beacon thread doesn't
        //                              hog the processor
        //
        public void HandleBeaconing(Object myObject, EventArgs myEventArgs)
        {

            //Stopping since operation may take longer than timer wait time. 
            PauseIntermittentBeacons();

            uint BeaconChannels = 0;
            ZigbeeNetwork[] ZigBeeNetworks = null;

            for (int iChannel = 0; iChannel < SCAN_CHANNELS.Length; iChannel++)
            {
                BeaconChannels |= (uint)(0x1 << SCAN_CHANNELS[iChannel]);
            }

            if (Monitor.TryEnter(C177App))
            {
                try
                {
                    //Send  beacon to make sure troublesome devices remain communicative.
                    FindNetworks(BeaconChannels, out ZigBeeNetworks, true);
                }
                finally
                {
                    Monitor.Exit(C177App);
                }
            }

            Application.DoEvents();
            ResumeIntermittentBeacons();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the manufacturer of the Belt Clip type radio, Itron or Telegesis
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/09/09 AF  2.20.00        Created
        //
        public RadioMfg RadioManufacturer
        {
            get
            {
                return m_RadioMfg;
            }
            set
            {
                m_RadioMfg = value;
            }
        }

        /// <summary>
        /// Itron's 0xC177 Private Profile Application
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/16/12 PGH 2.70.36        Created
        //
        public ZigBeeC177Application C177App
        {
            get
            {
                return m_C177App;
            }
            set
            {
                m_C177App = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Converts a ZigBee Logical Type value to an Ember Node Type value
        /// </summary>
        /// <param name="type">The Logical Type</param>
        /// <returns>The equivalent Ember Node Type</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/24/13 RCG 2.71.00        Created

        private EmberNodeType ConvertLogicalTypeToEmberType(ZigbeeLogicalType type)
        {
            EmberNodeType EmberType = EmberNodeType.EndDevice;

            switch (type)
            {
                case ZigbeeLogicalType.COORDINATOR:
                {
                    EmberType = EmberNodeType.Coordinator;
                    break;
                }
                case ZigbeeLogicalType.ENDDEVICE:
                {
                    EmberType = EmberNodeType.EndDevice;
                    break;
                }
                case ZigbeeLogicalType.ROUTER:
                {
                    EmberType = EmberNodeType.Router;
                    break;
                }
            }

            return EmberType;
        }

        /// <summary>
        /// Handles ZigBee C177 Data Response Cluster received message events
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/26/12 PGH 2.70.36        Created

        private void C177App_IPPDataResponseReceived(object sender, IPPDataResponseEventArgs e)
        {
            AddToRxBuffer(e.Message.MessageContents);
            m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Processing incoming Data Response");
        }

        /// <summary>
        /// Throws the End Device Joined Event
        /// </summary>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/11/10 RCG 2.45.12        Created

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        private void OnEndDeviceJoinedEvent(EndDeviceJoinedEventArgs e)
        {
            Console.WriteLine("OnEndDeviceJoinedEvent called");

            if (EndDeviceJoinedEvent != null)
            {
                EndDeviceJoinedEvent(this, e);
            }
        }

        /// <summary>
        /// This method causes the beacon timer to pause its operation.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/15 jrf 4.21.04 616082 Created
        //  09/06/16 AF  4.60.05 682273 Switched to a Systems.Timers timer for thread safety
        //
        private void PauseIntermittentBeacons()
        {
            if (true == m_BeaconTimerRunning && null != m_BeaconTimer)
            {
                m_BeaconTimer.Enabled = false;
            }
        }

        /// <summary>
        /// This method causes beacon timer to resume its operation.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/15 jrf 4.21.04 616082 Created
        //  09/06/16 AF  4.60.05 682273 Switched to a Systems.Timers timer for thread safety
        //
        private void ResumeIntermittentBeacons()
        {
            if (true == m_BeaconTimerRunning && null != m_BeaconTimer)
            {
                m_BeaconTimer.Interval = BEACON_PERIOD;
                m_BeaconTimer.Enabled = true;
            }
        }

        /// <summary>
        /// This method disposes of the timer.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/17/15 jrf 4.21.04 616082 Created
        //  09/06/16 AF  4.60.05 682273 Switched to a Systems.Timers timer for thread safety
        //
        private void DestroyBeaconTimer()
        {
            if (null != m_BeaconTimer)
            {
                StopIntermittentBeacons();
                m_BeaconTimer = null;
            }
        }

        #endregion

        #region Members

        private bool m_bConnected = false;

        private GCHandle m_gchEventHandler;       //Garbage collector handle
        private List<ZigbeeNetwork> m_NetworkList = new List<ZigbeeNetwork>();
        private RadioMfg m_RadioMfg;

        private ZigBeeC177Application m_C177App;

        #endregion Members

    }

    /// <summary>
    /// ZigBee C177 Application Layer
    /// </summary>
    public class  ZigBeeC177Application : ZigBeeApplication
    {

        #region Constants

        private const byte DEFAULT_SE_ENDPOINT = 0x0A;
        private const ushort SE_PROFILE_ID = 0x0109;
        private const byte DEFAULT_SE_DEVICE_VERSION = 2;
        private const ushort DEFAULT_SE_DEVICE_ID = 0x0502;
        
        private const byte DEFAULT_C177_ENDPOINT = 0x07;
        private const byte DEFAULT_C177_DEVICE_VERSION = 0;
        private const ushort DEFAULT_C177_DEVICE_ID = (ushort)ZigbeeDeviceType.HHC;

        private const string RESULTS_PATH = "C:\\OpenWayTest_V3\\Results\\";
        private const string COMM_PATH = RESULTS_PATH + "Comm Logs\\";

        #endregion

        #region Definitions

        private enum EncryptionKeyIdentifiers : byte
        {
            Gas = 0x1C,
            Water = 0x1D,
            ThirdParty = 0x1E,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/17/12 PGH 2.70.36        Created

        public ZigBeeC177Application()
            : base()
        {
        }

        /// <summary>
        /// Connects to the radio through the specified COM port
        /// </summary>
        /// <param name="portName">The name of the COM port the radio is on</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/17/12 PGH 2.70.36        Created

        public override void Connect(string portName)
        {
            m_KeyEstablishmentState = KeyEstablishmentState.NotEstablished;
            m_PartnerCertificate = null;
            m_LocalEphemeralKey = null;
            m_PartnerEphermeralKey = null;

            base.Connect(portName);
        }

        /// <summary>
        /// Rejoins the meter
        /// </summary>
        /// <param name="nodeType">The type of device to join as.</param>
        /// <param name="extendedPanID">The extended PAN ID of the network to join</param>
        /// <param name="channel">The channel that the network is on</param>
        /// <param name="networkKey">The network key to use during the rejoin</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/17/12 PGH 2.70.36        Created

        public override void Rejoin(EmberNodeType nodeType, ulong extendedPanID, byte channel, byte[] networkKey)
        {

            base.Rejoin(nodeType, extendedPanID, channel, networkKey);

            // We are currently joined with the meter we need to continue with the key establishment process
            if (IsJoined)
            {
                PerformKeyEstablishment(TRUST_CENTER_NODE_ID, false, KeyEstablishmentState.Established);

                if (m_KeyEstablishmentState != KeyEstablishmentState.Established)
                {
                    IsJoined = false;
                    m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Key establishment process failed. Result: " + m_KeyEstablishmentState.ToString());
                }
            }
        }

        /// <summary>
        /// Send Data Request to the meter
        /// </summary>
        /// <param name="Message">the message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/26/12 PGH 2.70.36  Created

        public void SendDataRequest(byte[] Message)
        {
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Sending Data Request:");

            SendC177DataRequest(TRUST_CENTER_NODE_ID, Message);
        }

        /// <summary>
        /// Send Heartbeat Request to the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/26/12 PGH 2.70.36  Created

        public void SendHeartbeatRequest()
        {
            m_Logger.WriteLine(EZSPLogLevels.ApplicationLayer, "Sending Heartbeat Request:");

            SendC177HeartbeatRequest(TRUST_CENTER_NODE_ID);
        }        


        #endregion

        #region Protected Methods

        /// <summary>
        /// Sets up any clusters that the device is hosting
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/19/12 PGH 2.70.36        Created
        protected override void SetUpClusterLists()
        {

            base.SetUpClusterLists();

            ZigBeeEndpointInfo NewEndpoint = new ZigBeeEndpointInfo();

            NewEndpoint.Endpoint = DEFAULT_SE_ENDPOINT;
            NewEndpoint.ProfileID = (ushort)ZigBeeProfileIDs.SmartEnergy;
            NewEndpoint.AppFlags = DEFAULT_SE_DEVICE_VERSION;
            NewEndpoint.DeviceID = DEFAULT_SE_DEVICE_ID;

            // Set up the Client Side List
            NewEndpoint.ClientClusterList.Add((ushort)GeneralClusters.Basic);
            NewEndpoint.ClientClusterList.Add((ushort)SmartEnergyClusters.KeyEstablishment);

            // Set up the Server Side List
            NewEndpoint.ServerClusterList.Add((ushort)GeneralClusters.Basic);
            NewEndpoint.ServerClusterList.Add((ushort)SmartEnergyClusters.KeyEstablishment);

            m_Endpoints.Add(NewEndpoint);

            // Itron Private Profile
            NewEndpoint = new ZigBeeEndpointInfo();

            NewEndpoint.Endpoint = DEFAULT_C177_ENDPOINT;
            NewEndpoint.ProfileID = (ushort)ZigBeeProfileIDs.ItronPrivateProfile;
            NewEndpoint.AppFlags = DEFAULT_C177_DEVICE_VERSION;
            NewEndpoint.DeviceID = DEFAULT_C177_DEVICE_ID;

            // Set up the Client Side List
            NewEndpoint.ClientClusterList.Add((ushort)ItronClusters.DATA_REQUEST);
            NewEndpoint.ClientClusterList.Add((ushort)ItronClusters.DATA_RESPONSE);
            NewEndpoint.ClientClusterList.Add((ushort)ItronClusters.HEARTBEAT_REQUEST);
            NewEndpoint.ClientClusterList.Add((ushort)ItronClusters.HEARTBEAT_RESPONSE);

            // Set up the Server Side List
            NewEndpoint.ServerClusterList.Add((ushort)ItronClusters.DATA_REQUEST);
            NewEndpoint.ServerClusterList.Add((ushort)ItronClusters.DATA_RESPONSE);
            NewEndpoint.ServerClusterList.Add((ushort)ItronClusters.HEARTBEAT_REQUEST);
            NewEndpoint.ServerClusterList.Add((ushort)ItronClusters.HEARTBEAT_RESPONSE);

            m_Endpoints.Add(NewEndpoint);

        }

        /// <summary>
        /// Handles ZCL Received Messages
        /// </summary>
        /// <param name="receivedMessage">The message that was received</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/26/12 PGH 2.70.36        Created
        protected override void HandleZCLMessage(IncomingMessage receivedMessage)
        {
            switch (receivedMessage.APSFrame.ProfileID)
            {
                case (ushort)ZigBeeProfileIDs.SmartEnergy:
                {
                    HandleSmartEnergyMessage(receivedMessage);
                    break;
                }
                default:
                {
                    base.HandleZCLMessage(receivedMessage);
                    break;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles incoming Smart Energy Messages
        /// </summary>
        /// <param name="receivedMessage">The incoming message</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/26/12 PGH 2.70.36        Created
        private void HandleSmartEnergyMessage(IncomingMessage receivedMessage)
        {
            switch (receivedMessage.APSFrame.ClusterID)
            {
                case (ushort)SmartEnergyClusters.KeyEstablishment:
                {
                    HandleKeyEstablishmentMessage(receivedMessage);
                    break;
                }
                default:
                {
                    m_UnhandledMessages.Add(receivedMessage);
                    break;
                }
            }
        }

        /// <summary>
        /// Sends message to ZigBee C177 Data Request Cluster
        /// </summary>
        /// <param name="destination">The destination node ID to request from</param>
        /// <param name="serialPayload">serial payload</param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue#    Description
        //  -------- --- -------  ------    -------------------------------------------
        //  10/26/12 PGH 2.70.36            Created
        //  06/09/15 PGH 4.50.140 RTT556279 Added encryption option when constructing EmberApsFrame
        //  07/29/15 PGH 4.50.177           Removed APS encryption option from EmberApsFrame
        //                                  since firmware after LithiumPlus do not understand it
        //  03/09/16 PGH 4.50.235           Added encryption option to APSFrame for firmware team testing

        private void SendC177DataRequest(ushort destination, byte[] serialPayload)
        {
            // Get source endpoint
            ZigBeeEndpointInfo C177EndPoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.ItronPrivateProfile);

            // Set up the APS Frame for the message
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.ItronPrivateProfile;
            ApsFrame.DestinationEndpoint = C177EndPoint.FindMatchingClientEndpoint(destination, (ushort)ItronClusters.DATA_REQUEST);
            ApsFrame.SourceEndpoint = C177EndPoint.Endpoint;
            ApsFrame.ClusterID = (ushort)ItronClusters.DATA_REQUEST;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;
                //| EmberApsOptions.Encryption;

            byte[] Message = new byte[2 + 1 + serialPayload.Length];
            MemoryStream MessageStream = new MemoryStream(Message);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);

            // Create the Message
            MessageWriter.Write(NodeID);
            MessageWriter.Write(DEFAULT_C177_ENDPOINT);
            MessageWriter.Write(serialPayload);

            // Send Unicast Message down to the meter
            SendUnicastMessage(destination, ApsFrame, Message);
        }

        /// <summary>
        /// Sends message to ZigBee C177 Heartbeat Request Cluster
        /// </summary>
        /// <param name="destination">The destination node ID to request from</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/26/12 PGH 2.70.36       Created

        private void SendC177HeartbeatRequest(ushort destination)
        {
            // Get source endpoint
            ZigBeeEndpointInfo C177EndPoint = m_Endpoints.First(e => e.ProfileID == (ushort)ZigBeeProfileIDs.ItronPrivateProfile);

            // Set up the APS Frame for the message
            EmberApsFrame ApsFrame = new EmberApsFrame();
            ApsFrame.ProfileID = (ushort)ZigBeeProfileIDs.ItronPrivateProfile;
            ApsFrame.DestinationEndpoint = C177EndPoint.FindMatchingClientEndpoint(destination, (ushort)ItronClusters.HEARTBEAT_REQUEST);
            ApsFrame.SourceEndpoint = C177EndPoint.Endpoint;
            ApsFrame.ClusterID = (ushort)ItronClusters.HEARTBEAT_REQUEST;
            ApsFrame.Options = EmberApsOptions.EnableAddressDiscovery
                | EmberApsOptions.EnableRouteDiscovery
                | EmberApsOptions.Retry;
                //| EmberApsOptions.Encryption;

            byte[] Message = new byte[8 + 2 + 1 + 1];
            MemoryStream MessageStream = new MemoryStream(Message);
            BinaryWriter MessageWriter = new BinaryWriter(MessageStream);

            // Create the Message
            MessageWriter.Write(MACAddress);
            MessageWriter.Write(NodeID);
            MessageWriter.Write(DEFAULT_C177_ENDPOINT);

            // Send Unicast Message down to the meter
            SendUnicastMessage(destination, ApsFrame, Message);
        }

        #endregion

    }

}
