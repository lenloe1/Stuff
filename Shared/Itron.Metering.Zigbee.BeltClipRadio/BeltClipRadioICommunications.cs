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
//                           Copyright © 2008 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using Itron.Metering.Communications;
using Itron.Metering.Utilities;
#if (WindowsCE)
using MDAPICSH;
#endif

namespace Itron.Metering.Zigbee
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public partial class BeltClipRadio : Radio
    {
        #region public ICommunication methods

        /// <summary>
        /// Opens the port passed in as a parameter.
        /// </summary>
        /// <param name="portName">
        /// The communication port to open.
        /// </param>
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/31/08 mcm 1.0.x   N/A	Created
        /// 05/15/08 AF  1.50           Added call to EnableDecryptedDebugOuput
        ///                             for debug tool CommOps.exe.  This does not
        ///                             affect performance unless EthDebugInfo.txt
        ///                             is present in the Debug folder
        /// 09/15/08 AF                 Replaced C177 with EZSP because the interface
        ///                             names changed in the new version of the EZSP dlls.
        /// 04/09/09 AF  2.20.00        Added code to distinguish between an Itron ZBCR
        ///                             and a Telegesis dongle 
        /// 01/07/11 AF  2.45.22        Added text for Telegesis dongle
        /// 
        /// 10/17/12 PGH 2.70.36        Replaced EZSP with C177App
        ///                             
        public override void OpenPort(string portName)
        {            
            // If we're already connected, we don't need to do anything.
            if (!m_bConnected)
            {
                try
                {
                    if (!C177App.IsConnected)
                    {
                        C177App.Connect(portName);
                        m_bConnected = true;
                        m_PortName = portName;
                    }
                    else
                    {
                        m_Logger.WriteDetail(Logger.LoggingLevel.ZigBeeProtocol,
                            "Failed to connect to the BeltClip radio/Telegesis dongle.");
                        m_PortName = "";
                    }
                }
                catch(Exception e)
                {
                    m_Logger.WriteDetail(Logger.LoggingLevel.ZigBeeProtocol,
                            "Failed to connect to the BeltClip radio/Telegesis dongle");
                    m_Logger.WriteException(this,e);
                        m_bConnected = false;
                    m_PortName = "";
                }
            }
        }

        /// <summary>
        /// Closes the communication port. 
        /// </summary>
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// comm.ClosePort();
        /// comm.Dispose();
        /// </code>
        /// </example>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/31/08 mcm 1.0.x   N/A	Created
        // 09/04/08 AF                 Added compiler directive to disable Bluetooth
        //                             stuff if not CE.
        // 09/15/08 AF                 Replaced C177 with EZSP because the interface
        //                             names changed in the new version of the EZSP dlls.
        // 10/17/12 PGH 2.70.36        Replaced EZSP with C177App
        // 09/17/15 jrf 4.21.04 616082 Stops the beaconing when port is closed.                      
        public override void ClosePort()
        {
            //Port is closing, so we should stop beacons.
            StopIntermittentBeacons();
            
            C177App.LeaveNetwork();
            if (C177App != null && C177App.IsConnected)
            {
                C177App.Disconnect();
                C177App.StopLogging();
            }

#if (WindowsCE)
            if (false != m_blnUsingBluetooth)
            {
                BToothCe.BT_Deinit();
            }
#endif
            m_bConnected = false;
        }

#if (WindowsCE)
        /// <summary>
        /// Uses the MDAPI interfaces to find the belt clip radio, join devices,
        /// and determine the virtual port to use.
        /// </summary>
        /// <param name="strPortName">the name of the port we read from the registry</param>
        /// <param name="strZBCName">the name of the belt clip radio</param>
        /// <returns>true if we got a virtual port; false, otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#     Description
        //  -------- --- ------- ---------- -------------------------------------------
        //  08/29/08 AF                     Created
        //  09/04/08 AF                     Removed call to BT_SetDeviceInfoFolder().
        //
        public bool SetUpBluetooth(ref string strPortName, string strZBCName)
        {
            BT_ERROR result = BT_ERROR.SUCCESS;
            bool blnContinue = true;
            ushort usCount = 0;
            bool blnResult = false;

            //Initialize API - must be called prior to any other function call
            if (BToothCe.BT_Init())
            {
                //Set pin so that input dialog box won't be displayed
                BToothCe.BT_SetPin("0000");

                while (blnContinue && (2 > usCount))
                {
                    blnContinue = false;

                    // In a normal operation, we always try to map the device first
                    // assuming it's been paired.
                    result = BToothCe.BT_MapSerialPortEx(ref strPortName, BT_DEVICE.ZB);

                    if (result == BT_ERROR.SUCCESS || result == BT_ERROR.PORT_ALREADY_MAPPED)
                    {
                        blnResult = true;
                        m_Logger.WriteDetail(Logger.LoggingLevel.ZigBeeProtocol,
                            "Bluetooth Setup: Port successfully mapped");
                    }
                    else if (result == BT_ERROR.SERVICE_NOT_SUPPORTED)
                    {
                        // critical - should never happen
                        m_Logger.WriteDetail(Logger.LoggingLevel.ZigBeeProtocol,
                            "Bluetooth service NOT supported");
                    }
                    else if (result == BT_ERROR.OUT_OF_PORTS)
                    {
                        // critical - reboot the device (PDA) and try again
                        m_Logger.WriteDetail(Logger.LoggingLevel.ZigBeeProtocol,
                            "Bluetooth Setup: No available ports found. Reboot the handheld device");
                    }
                    else if (result == BT_ERROR.FAILED_CREATE_PORT)
                    {
                        // unknown error - should never happen (but I've seen it)
                        m_Logger.WriteDetail(Logger.LoggingLevel.ZigBeeProtocol,
                            "Bluetooth Setup: Failed to create port");
                    }
                    else if (result == BT_ERROR.DEVICE_NOT_PAIRED || result == BT_ERROR.DEVICE_NOT_FOUND)
                    {
                        MyBTDEV[] devs = new MyBTDEV[1];
                        devs[0] = new MyBTDEV();
                        byte[] abyDevs = new byte[MyBTDEV.Length];
                        int numElements = 1;

                        // Scan for available BT devices.
                        // NOTE: A time consuming function; so running in the main thread 
                        // can lock up the interface until the function returns
                        BToothCe.BT_GetAvailableBTDevices(abyDevs, ref numElements, strZBCName);
                        if (1 == numElements)
                        {
                            //Retrieve the device information so that we can pair
                            devs = MyBTDEV.ToMyBTDEVArray(abyDevs, numElements);

                            uint uiLowAddress = (uint)(devs[0].addr);
                            uint uiHiAddress = (uint)(devs[0].addr >> 32);
                            // Try to pair the device
                            if (BT_ERROR.SUCCESS == BToothCe.BT_PairDeviceEx(uiHiAddress, uiLowAddress))
                            {
                                blnContinue = true;
                                m_Logger.WriteDetail(Logger.LoggingLevel.ZigBeeProtocol,
                                    "Bluetooth Setup: Devices successfully paired");
                            }

                            usCount++;
                        }
                        else
                        {
                            m_Logger.WriteDetail(Logger.LoggingLevel.ZigBeeProtocol, 
                                "Could not discover the selected Belt Clip Radio");
                        }
                    }
                }

                m_blnUsingBluetooth = true;
            }
            else
            {
                BToothCe.BT_Deinit();
                m_Logger.WriteDetail(Logger.LoggingLevel.ZigBeeProtocol, 
                    "Bluetooth initialization failed!");
            }

            return blnResult;
        }
#endif

        #endregion public ICommunication methods

        #region public ICommunication properties

        /// <summary>
        /// Whether or not the communication port is open.
        /// </summary>
        /// <returns>
        /// Boolean indicating whether or not the communication port is open.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// if ( false == comm.IsOpen() )
        /// {
        ///		comm.OpenPort("COM4:");
        ///	}
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/31/08 mcm 1.0.x   N/A	Created
        public override bool IsOpen
        {
            get { return m_bConnected; }
        }

        /// <summary>
        /// NOT MEANINGFUL, at least not yet.
        /// Property that gets or sets the baud rate.  The baud rate can only be
        /// set to a port that is not opened.
        /// </summary>
        /// <returns>
        /// The baud rate (uint).
        /// </returns>
        /// <exception cref="CommPortException">
        /// Thrown if the port is already open.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// if ( false == comm.IsOpen() )
        /// {
        ///		comm.BaudRate = 9600;
        ///		comm.OpenPort("COM4:");
        ///	}
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        ///	01/31/08 mcm 1.0.x   N/A    Created
        /// 
        public override uint BaudRate
        {
            get{ return m_BaudRate; }
            set{ m_BaudRate = value; }
        }

        /// <summary>
        /// Property that gets the current port name
        /// </summary>
        /// <returns>
        /// The current port name
        /// </returns>        
        /// Revision History
        /// MM/DD/YY who Version ID Issue# Description
        /// -------- --- ------- -- ------ ---------------------------------------
        ///	01/31/08 mcm 1.0.x      N/A    Created
        ///	10/15/14 AF  4.00.73 WR 230745 Removed the extra text so that this just returns the comm port name 
        /// 
        public override string PortName
        {
            get { return m_PortName; }
        }

        ///// <summary>
        ///// The Target Short Address is the address on the network that you
        ///// want future data to be sent to when calling Send().  By default
        ///// everything will be sent to the coordinator of the network.
        ///// </summary>
        ///// <returns>
        ///// The current port name
        ///// </returns>        
        ///// Revision History
        ///// MM/DD/YY who Version Issue# Description
        ///// -------- --- ------- ------ ---------------------------------------
        /////	01/31/08 mcm 1.0.x   Support for comms with other nodes on the network
        ///// 
        //public ushort TargetShortAddress
        //{
        //    get { return m_TargetShortAddress; }
        //    set { m_TargetShortAddress = value; }
        //}


        #endregion public ICommunication properties


        private uint m_BaudRate;
        private string m_PortName;
        private System.Timers.Timer m_BeaconTimer;
        private bool m_BeaconTimerRunning = false;

#if (WindowsCE)
        private bool m_blnUsingBluetooth = false;
#endif
    }
}

