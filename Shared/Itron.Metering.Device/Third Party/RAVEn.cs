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
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Itron.Metering.Communications.RAVEn;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Device object for the RAVEn ZigBee module
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class RAVEn
    {
        #region Constants

        private static readonly string XML_DEVICEINFO = "DeviceInfo";
        private static readonly string XML_SCHEDULEINFO = "ScheduleInfo";
        private static readonly string XML_TIMECLUSTER = "TimeCluster";
        private static readonly string XML_MESSAGECLUSTER = "MessageCluster";
        private static readonly string XML_PRICECLUSTER = "PriceCluster";
        private static readonly string XML_METERINGCLUSTER = "SimpleMeteringCluster";
        private static readonly string XML_INFORMATION = "Information";
        private static readonly string XML_ERROR = "Error";

        private static readonly string XML_COMMAND = "Command";
        private static readonly string XML_NAME = "Name";
        private static readonly string XML_CMDID = "CmdID";
        private static readonly string XML_ID = "Id";
        private static readonly string XML_FREQUENCY = "Frequency";
        private static readonly string XML_ENABLED = "Enabled";
        private static readonly string XML_DURATION = "Duration";
        private static readonly string XML_REFRESH = "Refresh";
        private static readonly string XML_MULTIPLIER = "Multiplier";
        private static readonly string XML_DIVISOR = "Divisor";
        private static readonly string XML_CURRENTSUMMATION = "CurrentSummationDelivered";
        private static readonly string XML_INSTDEMAND = "InstantaneousDemand";
        private static readonly string XML_CURRENTPERIOD = "CurrentPeriodDelivered";
        private static readonly string XML_LASTPERIOD = "LastPeriodDelivered";
        private static readonly string XML_STARTDATE = "StartDate";
        private static readonly string XML_ENDDATE = "EndDate";
        private static readonly string XML_LOCALTIME = "LocalTime";
        private static readonly string XML_UTCTIME = "UTCTime";
        private static readonly string XML_TEXT = "Text";

        private static readonly DateTime REFERENCE_LOCAL_TIME = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local);
        private static readonly DateTime REFERENCE_UTC_TIME = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Public Events

        /// <summary>
        /// Event that occurs when the billing data is updated
        /// </summary>
        public event EventHandler DataUpdated;
        /// <summary>
        /// Event that occurs when a message has been received
        /// </summary>
        public event EventHandler MessageReceived;
        /// <summary>
        /// Event that occurs when the device information has been updated
        /// </summary>
        public event EventHandler DeviceInfoUpdated;
        /// <summary>
        /// Event that occurs when the price information has been updated
        /// </summary>
        public event EventHandler PriceUpdated;
        /// <summary>
        /// Event that occurs when the device time has been updated
        /// </summary>
        public event EventHandler DeviceTimeUpdated;
        /// <summary>
        /// Event that occurs when an Information update has occurred
        /// </summary>
        public event StatusEventHandler InformationReceived;
        /// <summary>
        /// Event that occurs when an Error is received
        /// </summary>
        public event StatusEventHandler ErrorReceived;
        /// <summary>
        /// Event that occurs when an Instantaneous Power value is received
        /// </summary>
        public event EventHandler InstantPowerReceived;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commPort">The COM port that the device is on</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public RAVEn(string commPort)
        {
            m_PortName = commPort;
            m_Comm = new RAVEnSerialCommunications();

            m_DataReceivedHandler = new EventHandler(m_Comm_DataReceived);
        }

        /// <summary>
        /// Connect to the RAVEn device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void Connect()
        {
            if (m_Comm != null && m_Comm.IsOpen == false)
            {
                m_Comm.OpenPort(m_PortName);
                m_Comm.DataReceived += m_DataReceivedHandler;
                m_Comm.Initialize();

                m_CurrentCmdID = 0;
                m_CurrentElements = new List<XElement>();
                m_DeviceInfo = null;
                m_ScheduleInfo = new RAVEnScheduleInformation();
                m_Messages = new List<RAVEnMessage>();
                m_Price = new RAVEnPrice();

                m_CurrrentSummation = 0;
                m_InstantaneousDemand = 0;
                m_CurrentPeriodDelivered = 0;
                m_LastPeriodDelivered = 0;

                m_DeviceTime = REFERENCE_UTC_TIME;
                m_CurrentPeriodStartDate = REFERENCE_UTC_TIME;
                m_LastPeriodEndDate = REFERENCE_UTC_TIME;
                m_LastPeriodStartDate = REFERENCE_UTC_TIME;

                m_LastErrorMessage = "";
                m_LastInformationMessage = "";
            }
        }

        /// <summary>
        /// Disconnects from the RAVEn device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void Disconnect()
        {
            if (m_Comm != null && m_Comm.IsOpen)
            {
                m_Comm.DataReceived -= m_DataReceivedHandler;
                m_Comm.ClosePort();
            }
        }

        /// <summary>
        /// Restarts the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void Restart()
        {
            XElement NewCommand = CreateCommandElement("restart", false);

            if (m_Comm != null && m_Comm.IsOpen)
            {
                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Performs a Factory Reset which clears any currently joined networks 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void FactoryReset()
        {
            XElement NewCommand = CreateCommandElement("factory_reset", false);

            if (m_Comm != null && m_Comm.IsOpen)
            {
                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Gets the device info
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void GetDeviceInfo()
        {
            XElement NewCommand = CreateCommandElement("get_device_info", false);

            if (m_Comm != null && m_Comm.IsOpen)
            {
                // Clear the device info since it is being requested again
                m_DeviceInfo = null;
                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Gets the schedule information
        /// </summary>
        /// <param name="scheduleID">The schedule being requested</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void GetScheduleInfo(RAVEnScheduleInformation.ScheduleID scheduleID)
        {
            if (m_Comm != null && m_Comm.IsOpen)
            {
                XElement NewCommand = CreateCommandElement("get_schedule", false);

                if (scheduleID != RAVEnScheduleInformation.ScheduleID.All)
                {
                    XElement IDElement = new XElement(XML_ID);
                    IDElement.Value = "0x" + ((byte)scheduleID).ToString("X2", CultureInfo.InvariantCulture);

                    NewCommand.Add(IDElement);
                }

                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Sets the schedule information
        /// </summary>
        /// <param name="scheduleID">The schedule to set</param>
        /// <param name="enabled">Whether or not auto updating of the item should be enabled</param>
        /// <param name="frequency">The frequency to update the item</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void SetScheduleInfo(RAVEnScheduleInformation.ScheduleID scheduleID, bool enabled, uint frequency)
        {
            if (m_Comm != null && m_Comm.IsOpen)
            {
                XElement NewCommand = CreateCommandElement("set_schedule", false);
                XElement IDElement = new XElement(XML_ID);
                XElement FrequencyElement = new XElement(XML_FREQUENCY);
                XElement EnabledElement = new XElement(XML_ENABLED);

                IDElement.Value = "0x" + ((byte)scheduleID).ToString("X2", CultureInfo.InvariantCulture);
                NewCommand.Add(IDElement);

                FrequencyElement.Value = "0x" + frequency.ToString("X8", CultureInfo.InvariantCulture);
                NewCommand.Add(FrequencyElement);

                if (enabled)
                {
                    EnabledElement.Value = "Y";
                }
                else
                {
                    EnabledElement.Value = "N";
                }

                NewCommand.Add(EnabledElement);

                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Sets the specified schedule to the default value
        /// </summary>
        /// <param name="scheduleID">The schedule to reset</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void SetScheduleDefault(RAVEnScheduleInformation.ScheduleID scheduleID)
        {
            if (m_Comm != null && m_Comm.IsOpen)
            {
                XElement NewCommand = CreateCommandElement("set_schedule_default", false);

                if (scheduleID != RAVEnScheduleInformation.ScheduleID.All)
                {
                    XElement IDElement = new XElement(XML_ID);
                    IDElement.Value = "0x" + ((byte)scheduleID).ToString("X2", CultureInfo.InvariantCulture);

                    NewCommand.Add(IDElement);
                }

                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Gets the time from the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void GetTime()
        {
            if (m_Comm != null && m_Comm.IsOpen)
            {
                XElement NewCommand = CreateCommandElement("get_time", false);
                XElement RefreshElement = new XElement(XML_REFRESH);

                RefreshElement.Value = "Y";
                NewCommand.Add(RefreshElement);

                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Gets the messages from the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void GetMessage()
        {
            if (m_Comm != null && m_Comm.IsOpen)
            {
                XElement NewCommand = CreateCommandElement("get_message", false);
                XElement RefreshElement = new XElement(XML_REFRESH);

                RefreshElement.Value = "Y";
                NewCommand.Add(RefreshElement);

                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Confirms the specified message from the device
        /// </summary>
        /// <param name="messageID">The message to confirm</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void ConfirmMessage(uint messageID)
        {
            if (m_Comm != null && m_Comm.IsOpen)
            {
                XElement NewCommand = CreateCommandElement("confirm_message", false);
                XElement IDElement = new XElement(XML_ID);

                IDElement.Value = "0x" + messageID.ToString("X8", CultureInfo.InvariantCulture);
                NewCommand.Add(IDElement);

                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Gets the current price information
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void GetCurrentPrice()
        {
            if (m_Comm != null && m_Comm.IsOpen)
            {
                XElement NewCommand = CreateCommandElement("get_current_price", false);
                XElement RefreshElement = new XElement(XML_REFRESH);

                RefreshElement.Value = "Y";
                NewCommand.Add(RefreshElement);

                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Gets the Instantaneous Demand
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void GetInstantaneousDemand()
        {
            if (m_Comm != null && m_Comm.IsOpen)
            {
                XElement NewCommand = CreateCommandElement("get_instantaneous_demand", false);
                XElement RefreshElement = new XElement(XML_REFRESH);

                RefreshElement.Value = "Y";
                NewCommand.Add(RefreshElement);

                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Gets the current summation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void GetCurrentSummationDelivered()
        {
            if (m_Comm != null && m_Comm.IsOpen)
            {
                XElement NewCommand = CreateCommandElement("get_current_summation_delivered", false);
                XElement RefreshElement = new XElement(XML_REFRESH);

                RefreshElement.Value = "Y";
                NewCommand.Add(RefreshElement);

                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Gets the current period demand
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void GetCurrentPeriodDelivered()
        {
            if (m_Comm != null && m_Comm.IsOpen)
            {
                XElement NewCommand = CreateCommandElement("get_current_period_delivered", false);
                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Gets the last period demand
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void GetLastPeriodDelivered()
        {
            if (m_Comm != null && m_Comm.IsOpen)
            {
                XElement NewCommand = CreateCommandElement("get_last_period_delivered", false);
                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Closes the current period in the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void CloseCurrentPeriodDelivered()
        {
            if (m_Comm != null && m_Comm.IsOpen)
            {
                XElement NewCommand = CreateCommandElement("close_current_period_delivered", false);
                m_Comm.SendFragment(NewCommand);
            }
        }

        /// <summary>
        /// Sets the RAVEn to fast poll mode
        /// </summary>
        /// <param name="frequency">The frequency to poll</param>
        /// <param name="duration">The duration of fast poll mode</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void SetFastPoll(ushort frequency, ushort duration)
        {
            if (m_Comm != null && m_Comm.IsOpen)
            {
                XElement NewCommand = CreateCommandElement("set_fast_poll", false);
                XElement FrequencyElement = new XElement(XML_FREQUENCY);
                XElement DurationElement = new XElement(XML_DURATION);

                FrequencyElement.Value = "0x" + frequency.ToString("X4", CultureInfo.InvariantCulture);
                DurationElement.Value = "0x" + duration.ToString("X4", CultureInfo.InvariantCulture);

                NewCommand.Add(FrequencyElement);
                NewCommand.Add(DurationElement);

                m_Comm.SendFragment(NewCommand);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Port Name for the RAVEn module
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/16/11 RCG 2.51.12        Created

        public string Port
        {
            get
            {
                return m_PortName;
            }
        }

        /// <summary>
        /// Gets the list of current unhandled elements
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public List<XElement> CurrentElements
        {
            get
            {
                return m_CurrentElements;
            }
        }

        /// <summary>
        /// Gets the last received Device Information
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public RAVEnDeviceInformation DeviceInformation
        {
            get
            {
                return m_DeviceInfo;
            }
        }

        /// <summary>
        /// Gets the last received Schedule Information
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public RAVEnScheduleInformation ScheduleInformation
        {
            get
            {
                return m_ScheduleInfo;
            }
        }

        /// <summary>
        /// Gets the last received Device Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public DateTime DeviceTime
        {
            get
            {
                return m_DeviceTime;
            }
        }

        /// <summary>
        /// Gets the list of messages received from the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public List<RAVEnMessage> Messages
        {
            get
            {
                return m_Messages;
            }
        }

        /// <summary>
        /// Gets the pricing information last received from the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public RAVEnPrice Price
        {
            get
            {
                return m_Price;
            }
        }

        /// <summary>
        /// Gets the Instantaneous Demand
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public double InstantaneousDemand
        {
            get
            {
                return m_InstantaneousDemand;
            }
        }

        /// <summary>
        /// Gets the Current Summation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public double CurrentSummationDelivered
        {
            get
            {
                return m_CurrrentSummation;
            }
        }

        /// <summary>
        /// Gets the value for the Current Period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public double CurrentPeriodDelivered
        {
            get
            {
                return m_CurrentPeriodDelivered;
            }
        }

        /// <summary>
        /// Gets the value for the last period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public double LastPeriodDelivered
        {
            get
            {
                return m_LastPeriodDelivered;
            }
        }

        /// <summary>
        /// Gets the start date for the current period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public DateTime CurrentPeriodStartDate
        {
            get
            {
                return m_CurrentPeriodStartDate;
            }
        }

        /// <summary>
        /// Gets the start date for the last period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public DateTime LastPeriodStartDate
        {
            get
            {
                return m_LastPeriodStartDate;
            }
        }

        /// <summary>
        /// Gets the end date for the last period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public DateTime LastPeriodEndDate
        {
            get
            {
                return m_LastPeriodEndDate;
            }
        }

        /// <summary>
        /// Gets the last Information message received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public string LastInformationMessage
        {
            get
            {
                return m_LastInformationMessage;
            }
        }

        /// <summary>
        /// Gets the last error message received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public string LastErrorMessage
        {
            get
            {
                return m_LastErrorMessage;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Waits for a response for the specified command
        /// </summary>
        /// <param name="usCmdID">The command to wait for</param>
        /// <returns>The response to the command</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private XElement WaitForResponse(ushort usCmdID)
        {
            int RetryCount = 0;
            XElement ResponseElement = null;

            while (RetryCount < 10)
            {
                foreach (XElement CurrentElement in m_CurrentElements)
                {
                    if (CurrentElement.Elements().Where(n => n.Name.LocalName.Equals(XML_CMDID)).Count() > 0)
                    {
                        XElement CmdIDElement = CurrentElement.Elements().First(n => n.Name.LocalName.Equals(XML_CMDID));

                        if (CmdIDElement != null)
                        {
                            if (CmdIDElement.Value.Equals("0x" + usCmdID.ToString("X4", CultureInfo.InvariantCulture)))
                            {
                                ResponseElement = CurrentElement;
                                break;
                            }
                        }
                    }
                }

                if (ResponseElement != null)
                {
                    break;
                }

                Thread.Sleep(500);
                RetryCount++;
            }

            return ResponseElement;
        }

        /// <summary>
        /// Creates a basic Command element used for sending commands to the RAVEn
        /// </summary>
        /// <param name="command">The command to send</param>
        /// <param name="useCommandID">Whether or not a Command ID should be sent with the command</param>
        /// <returns>The generated Command</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private XElement CreateCommandElement(string command, bool useCommandID)
        {
            XElement NewCommand = new XElement(XML_COMMAND);
            XElement NewName = new XElement(XML_NAME);

            NewName.Value = command;

            NewCommand.Add(NewName);

            if (useCommandID)
            {
                XElement NewCmdID = new XElement(XML_CMDID);

                NewCmdID.Value = "0x" + m_CurrentCmdID.ToString("X4", CultureInfo.InvariantCulture);
                NewCommand.Add(NewCmdID);
            }

            m_CurrentCmdID++;

            return NewCommand;
        }

        /// <summary>
        /// Handles the Data Received event from the Communications object
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void m_Comm_DataReceived(object sender, EventArgs e)
        {
            m_CurrentElements = m_Comm.ReceivedElements;

            CheckForSpecialResponse();
        }

        /// <summary>
        /// Checks the list of XML fragments received for special fragments and reads out the relevant information
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void CheckForSpecialResponse()
        {
            CheckForDeviceInfoMessages();
            CheckForScheduleInfoMessages();
            CheckForTimeClusterMessages();
            CheckForMessageClusterMessages();
            CheckForPriceClusterMessages();
            CheckForSimpleMeteringClusterMessages();
            CheckForInformationMessages();
            CheckForErrorMessages();
        }

        /// <summary>
        /// Checks the list of XML fragments received for Error Messages
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void CheckForErrorMessages()
        {
            // Check for Schedule Info elements
            IEnumerable<XElement> Elements = m_CurrentElements.Where(e => e.Name.LocalName.Equals(XML_ERROR));

            foreach (XElement CurrentElement in Elements)
            {
                if (CurrentElement.Elements(XML_TEXT).Count() > 0)
                {
                    m_LastErrorMessage = CurrentElement.Elements(XML_TEXT).First().Value;
                    OnErrorReceived(m_LastErrorMessage);
                }
            }

            m_CurrentElements.RemoveAll(e => e.Name.LocalName.Equals(XML_ERROR));
        }

        /// <summary>
        /// Checks for Information messages
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created
        
        private void CheckForInformationMessages()
        {
            // Check for Schedule Info elements
            IEnumerable<XElement> Elements = m_CurrentElements.Where(e => e.Name.LocalName.Equals(XML_INFORMATION));

            foreach (XElement CurrentElement in Elements)
            {
                if (CurrentElement.Elements(XML_TEXT).Count() > 0)
                {
                    m_LastInformationMessage = CurrentElement.Elements(XML_TEXT).First().Value;
                    OnInformationReceived(m_LastInformationMessage);
                }
            }

            m_CurrentElements.RemoveAll(e => e.Name.LocalName.Equals(XML_INFORMATION));
        }

        /// <summary>
        /// Checks the list of received messages for SimpleMeteringCluster messages
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created
        //  07/29/11 MSC 2.51.30        Instant Power triggers it's own event now.

        private void CheckForSimpleMeteringClusterMessages()
        {
            IEnumerable<XElement> Elements = m_CurrentElements.Where(e => e.Name.LocalName.Equals(XML_METERINGCLUSTER));

            if (Elements.Count() > 0)
            {
                foreach (XElement CurrentElement in Elements)
                {
                    uint uiMultiplier = 1;
                    uint uiDivisor = 1;
                    uint uiValue = 0;

                    if (Elements.Elements(XML_MULTIPLIER).Count() > 0)
                    {
                        uiMultiplier = uint.Parse(Elements.Elements(XML_MULTIPLIER).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    }

                    if (Elements.Elements(XML_DIVISOR).Count() > 0)
                    {
                        uiDivisor = uint.Parse(Elements.Elements(XML_DIVISOR).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    }

                    // Summation
                    if (Elements.Elements(XML_CURRENTSUMMATION).Count() > 0)
                    {
                        uiValue = uint.Parse(Elements.Elements(XML_CURRENTSUMMATION).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                        m_CurrrentSummation = (double)uiValue * (double)uiMultiplier / (double)uiDivisor;

                        OnDataUpdated();
                    }

                    
                    // Instantaneous
                    if (Elements.Elements(XML_INSTDEMAND).Count() > 0)
                    {
                        uiValue = uint.Parse(Elements.Elements(XML_INSTDEMAND).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                        m_InstantaneousDemand = (double)uiValue * (double)uiMultiplier / (double)uiDivisor;

                        OnPowerUpdated();
                    }

                    // Current Period
                    if (Elements.Elements(XML_CURRENTPERIOD).Count() > 0)
                    {
                        uiValue = uint.Parse(Elements.Elements(XML_CURRENTPERIOD).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                        m_CurrentPeriodDelivered = (double)uiValue * (double)uiMultiplier / (double)uiDivisor;

                        if (Elements.Elements(XML_STARTDATE).Count() > 0)
                        {
                            uiValue = uint.Parse(Elements.Elements(XML_STARTDATE).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                            m_CurrentPeriodStartDate = REFERENCE_UTC_TIME.AddSeconds(uiValue);
                        }

                        OnDataUpdated();
                    }

                    // Last Period
                    if (Elements.Elements(XML_LASTPERIOD).Count() > 0)
                    {
                        uiValue = uint.Parse(Elements.Elements(XML_LASTPERIOD).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                        m_LastPeriodDelivered = (double)uiValue * (double)uiMultiplier / (double)uiDivisor;

                        if (Elements.Elements(XML_STARTDATE).Count() > 0)
                        {
                            uiValue = uint.Parse(Elements.Elements(XML_STARTDATE).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                            m_LastPeriodStartDate = REFERENCE_UTC_TIME.AddSeconds(uiValue);
                        }

                        if (Elements.Elements(XML_ENDDATE).Count() > 0)
                        {
                            uiValue = uint.Parse(Elements.Elements(XML_ENDDATE).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                            m_LastPeriodEndDate = REFERENCE_UTC_TIME.AddSeconds(uiValue);
                        }

                        OnDataUpdated();
                    }
                }

            }

            m_CurrentElements.RemoveAll(e => e.Name.LocalName.Equals(XML_METERINGCLUSTER));
        }

        /// <summary>
        /// Checks the list of received messages for ScheduleInfo messages
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void CheckForScheduleInfoMessages()
        {
            // Check for Schedule Info elements
            IEnumerable<XElement> Elements = m_CurrentElements.Where(e => e.Name.LocalName.Equals(XML_SCHEDULEINFO));

            foreach (XElement CurrentElement in Elements)
            {
                m_ScheduleInfo.UpdateScheduleInfo(CurrentElement);
            }

            m_CurrentElements.RemoveAll(e => e.Name.LocalName.Equals(XML_SCHEDULEINFO));
        }

        /// <summary>
        /// Checks the list of received messages for DeviceInfo messages
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void CheckForDeviceInfoMessages()
        {
            // Check for Device Info elements
            IEnumerable<XElement> Elements = m_CurrentElements.Where(e => e.Name.LocalName.Equals(XML_DEVICEINFO));

            if (Elements.Count() > 0)
            {
                m_DeviceInfo = new RAVEnDeviceInformation(Elements.Last());

                OnDeviceInfoUpdated();
            }

            m_CurrentElements.RemoveAll(e => e.Name.LocalName.Equals(XML_DEVICEINFO));
        }

        /// <summary>
        /// Checks the list of received messages for TimeCluster messages
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void CheckForTimeClusterMessages()
        {
            // Check for TimeCluster elements
            IEnumerable<XElement> Elements = m_CurrentElements.Where(e => e.Name.LocalName.Equals(XML_TIMECLUSTER));

            if (Elements.Count() > 0)
            {
                // The most recent time should be the last one
                XElement RecentTime = Elements.Last();

                if (RecentTime.Elements(XML_LOCALTIME).Count() > 0)
                {
                    uint uiLocalSeconds = uint.Parse(RecentTime.Elements(XML_LOCALTIME).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    m_DeviceTime = REFERENCE_LOCAL_TIME.AddSeconds(uiLocalSeconds);
                }

                if (RecentTime.Elements(XML_UTCTIME).Count() > 0)
                {
                    uint uiUTCSeconds = uint.Parse(RecentTime.Elements(XML_UTCTIME).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    m_DeviceTime = REFERENCE_UTC_TIME.AddSeconds(uiUTCSeconds);
                }

                OnDeviceTimeUpdated();
            }

            m_CurrentElements.RemoveAll(e => e.Name.LocalName.Equals(XML_TIMECLUSTER));
        }

        /// <summary>
        /// Checks the list of received messages for MessageCluster messages
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void CheckForMessageClusterMessages()
        {
            // Check for MessageCluster elements
            IEnumerable<XElement> Elements = m_CurrentElements.Where(e => e.Name.LocalName.Equals(XML_MESSAGECLUSTER));

            if (Elements.Count() > 0)
            {
                foreach (XElement CurrentMessageElement in Elements)
                {
                    try
                    {
                        RAVEnMessage NewMessage = new RAVEnMessage();
                        NewMessage.ParseElement(CurrentMessageElement);

                        // Remove any messages with the same ID that may already exist because we want the latest message only
                        m_Messages.RemoveAll(m => m.Equals(NewMessage));

                        m_Messages.Add(NewMessage);
                    }
                    catch (ArgumentNullException)
                    {
                        //This is currently used to catch the unexplained empty MessagingCluster XML fragment coming
                        //from the RAVEn
                    }
                }

                OnMessageReceived();
            }

            m_CurrentElements.RemoveAll(e => e.Name.LocalName.Equals(XML_MESSAGECLUSTER));
        }

        /// <summary>
        /// Checks the list of received messages for PriceCluster messages
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void CheckForPriceClusterMessages()
        {
            // Check for PriceCluster elements
            IEnumerable<XElement> Elements = m_CurrentElements.Where(e => e.Name.LocalName.Equals(XML_PRICECLUSTER));

            if (Elements.Count() > 0)
            {
                RAVEnPrice NewPrice = new RAVEnPrice();
                NewPrice.ParseElement(Elements.Last());

                m_Price = NewPrice;
                OnPriceUpdated();
            }

            m_CurrentElements.RemoveAll(e => e.Name.LocalName.Equals(XML_PRICECLUSTER));
        }

        /// <summary>
        /// Raises the Information Received event
        /// </summary>
        /// <param name="status">The Information text</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created
        
        private void OnInformationReceived(string status)
        {
            if (InformationReceived != null)
            {
                InformationReceived(this, new StatusEventArgs(status));
            }
        }

        /// <summary>
        /// Raises the error received event
        /// </summary>
        /// <param name="status">The error text</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created
        
        private void OnErrorReceived(string status)
        {
            if (ErrorReceived != null)
            {
                ErrorReceived(this, new StatusEventArgs(status));
            }
        }

        /// <summary>
        /// Raises the Device Time Updated event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created
        
        private void OnDeviceTimeUpdated()
        {
            if (DeviceTimeUpdated != null)
            {
                DeviceTimeUpdated(this, new EventArgs());
            }
        }

        /// <summary>
        /// Raises the Device Info updated event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void OnDeviceInfoUpdated()
        {
            if (DeviceInfoUpdated != null)
            {
                DeviceInfoUpdated(this, new EventArgs());
            }
        }

        /// <summary>
        /// Raises the Message Received event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void OnMessageReceived()
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, new EventArgs());
            }
        }

        /// <summary>
        /// Raises the Data Updated event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void OnDataUpdated()
        {
            if (DataUpdated != null)
            {
                DataUpdated(this, new EventArgs());
            }
        }

        /// <summary>
        /// Raises the PriceUpdated event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void OnPriceUpdated()
        {
            if (PriceUpdated != null)
            {
                PriceUpdated(this, new EventArgs());
            }
        }

        /// <summary>
        /// Raises the Instantaneous event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/29/11 MSC 2.51.30        Created

        private void OnPowerUpdated()
        {
            if (InstantPowerReceived != null)
            {
                InstantPowerReceived(this, new EventArgs());
            }
        }

        #endregion

        #region Member Variables

        private RAVEnSerialCommunications m_Comm;
        private string m_PortName;
        private ushort m_CurrentCmdID;
        private List<XElement> m_CurrentElements;

        private RAVEnDeviceInformation m_DeviceInfo;
        private RAVEnScheduleInformation m_ScheduleInfo;
        private List<RAVEnMessage> m_Messages;
        private RAVEnPrice m_Price;
        private DateTime m_DeviceTime;

        private double m_InstantaneousDemand;
        private double m_CurrrentSummation;
        private double m_CurrentPeriodDelivered;
        private double m_LastPeriodDelivered;

        private DateTime m_CurrentPeriodStartDate;
        private DateTime m_LastPeriodStartDate;
        private DateTime m_LastPeriodEndDate;

        private EventHandler m_DataReceivedHandler;

        private string m_LastInformationMessage;
        private string m_LastErrorMessage;

        #endregion
    }

    /// <summary>
    /// Device Information object for the RAVEn ZigBee module
    /// </summary>
    public class RAVEnDeviceInformation
    {
        #region Constants

        private static readonly string XML_DEVICEINFO = "DeviceInfo";
        private static readonly string XML_MACID = "MacId";
        private static readonly string XML_INSTALLCODE = "InstallCode";
        private static readonly string XML_FWVERSION = "FWVersion";
        private static readonly string XML_HWVERSION = "HWVersion";
        private static readonly string XML_MANUFACTURER = "Manufacturer";
        private static readonly string XML_MODELID = "ModelId";
        private static readonly string XML_DATECODE = "DateCode";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="xmlElement">The element containing the Device Information</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public RAVEnDeviceInformation(XElement xmlElement)
        {
            m_MACAddress = "";
            m_InstallCode = "";
            m_FWVersion = "";
            m_HWVersion = "";
            m_Manufacturer = "";
            m_ModelID = "";
            m_DateCode = "";

            ParseElement(xmlElement);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the MAC Address
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public string MACAddress
        {
            get
            {
                return m_MACAddress;
            }
        }

        /// <summary>
        /// Gets the Install Code
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public string InstallCode
        {
            get
            {
                return m_InstallCode;
            }
        }

        /// <summary>
        /// Gets the FW Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public string FWVersion
        {
            get
            {
                return m_FWVersion;
            }
        }

        /// <summary>
        /// Gets the HW Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public string HWVersion
        {
            get
            {
                return m_HWVersion;
            }
        }

        /// <summary>
        /// Gets the Manufacturer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public string Manufacturer
        {
            get
            {
                return m_Manufacturer;
            }
        }

        /// <summary>
        /// Gets the Model ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public string ModelID
        {
            get
            {
                return m_ModelID;
            }
        }

        /// <summary>
        /// Gets the Date Code
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public string DateCode
        {
            get
            {
                return m_DateCode;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the Device Info from the XML element
        /// </summary>
        /// <param name="xmlElement">The element to parse from</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        private void ParseElement(XElement xmlElement)
        {
            if (xmlElement != null && xmlElement.Name.LocalName.Equals(XML_DEVICEINFO))
            {
                if (xmlElement.Elements(XML_MACID).Count() > 0)
                {
                    m_MACAddress = xmlElement.Elements(XML_MACID).First().Value;
                }

                if (xmlElement.Elements(XML_INSTALLCODE).Count() > 0)
                {
                    m_InstallCode = xmlElement.Elements(XML_INSTALLCODE).First().Value;
                }

                if (xmlElement.Elements(XML_FWVERSION).Count() > 0)
                {
                    m_FWVersion = xmlElement.Elements(XML_FWVERSION).First().Value;
                }

                if (xmlElement.Elements(XML_HWVERSION).Count() > 0)
                {
                    m_HWVersion = xmlElement.Elements(XML_HWVERSION).First().Value;
                }

                if (xmlElement.Elements(XML_MANUFACTURER).Count() > 0)
                {
                    m_Manufacturer = xmlElement.Elements(XML_MANUFACTURER).First().Value;
                }

                if (xmlElement.Elements(XML_MODELID).Count() > 0)
                {
                    m_ModelID = xmlElement.Elements(XML_MODELID).First().Value;
                }

                if (xmlElement.Elements(XML_DATECODE).Count() > 0)
                {
                    m_DateCode = xmlElement.Elements(XML_DATECODE).First().Value;
                }
            } 
        }

        #endregion

        #region Member Variables

        private string m_MACAddress;
        private string m_InstallCode;
        private string m_FWVersion;
        private string m_HWVersion;
        private string m_Manufacturer;
        private string m_ModelID;
        private string m_DateCode;

        #endregion

    }

    /// <summary>
    /// Schedule Information object for the RAVEn ZigBee module
    /// </summary>
    public class RAVEnScheduleInformation
    {
        #region Constants

        private static readonly string XML_ELEMENT_NAME = "ScheduleInfo";
        private static readonly string XML_ID = "Id";
        private static readonly string XML_FREQUENCY = "Frequency";
        private static readonly string XML_ENABLED = "Enabled";

        /// <summary>
        /// Schedule types
        /// </summary>
        public enum ScheduleID : byte
        {
            /// <summary>
            /// Time update schedule
            /// </summary>
            Time = 0x00,
            /// <summary>
            /// Message update schedule
            /// </summary>
            Message = 0x01,
            /// <summary>
            /// Price update schedule
            /// </summary>
            Price = 0x02,
            /// <summary>
            /// Summation update schedule
            /// </summary>
            Summation = 0x03,
            /// <summary>
            /// Demand update schedule
            /// </summary>
            Demand = 0x04,
            /// <summary>
            /// None update schedule
            /// </summary>
            None = 0x05,
            /// <summary>
            /// All schedules
            /// </summary>
            All = 0xFF,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public RAVEnScheduleInformation()
        {
            m_UpdateTimeEnabled = false;
            m_UpdateMessageEnabled = false;
            m_UpdatePriceEnabled = false;
            m_UpdateSummationEnabled = false;
            m_UpdateDemandEnabled = false;
            m_UpdateNoneEnabled = false;

            m_TimeUpdateFrequency = 0;
            m_MessageUpdateFrequency = 0;
            m_PriceUpdateFrequency = 0;
            m_SummationUpdateFrequency = 0;
            m_DemandUpdateFrequency = 0;
            m_NoneUpdateFrequency = 0;
        }

        /// <summary>
        /// Updates the schedule info from the specified ScheduleInfo element
        /// </summary>
        /// <param name="element">The element to update from</param>
        public void UpdateScheduleInfo(XElement element)
        {
            if (element != null && element.Name.LocalName.Equals(XML_ELEMENT_NAME))
            {
                if (element.Elements(XML_ID).Count() > 0 && element.Elements(XML_FREQUENCY).Count() > 0 && element.Elements(XML_ENABLED).Count() > 0)
                {
                    ScheduleID ID = (ScheduleID)Byte.Parse(element.Elements(XML_ID).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    uint Frequency = UInt32.Parse(element.Elements(XML_FREQUENCY).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    bool Enabled = element.Elements(XML_ENABLED).First().Value.Equals("Y");

                    switch (ID)
                    {
                        case ScheduleID.Time:
                        {
                            m_TimeUpdateFrequency = Frequency;
                            m_UpdateTimeEnabled = Enabled;
                            break;
                        }
                        case ScheduleID.Message:
                        {
                            m_MessageUpdateFrequency = Frequency;
                            m_UpdateMessageEnabled = Enabled;
                            break;
                        }
                        case ScheduleID.Price:
                        {
                            m_PriceUpdateFrequency = Frequency;
                            m_UpdatePriceEnabled = Enabled;
                            break;
                        }
                        case ScheduleID.Summation:
                        {
                            m_SummationUpdateFrequency = Frequency;
                            m_UpdateSummationEnabled = Enabled;
                            break;
                        }
                        case ScheduleID.Demand:
                        {
                            m_DemandUpdateFrequency = Frequency;
                            m_UpdateDemandEnabled = Enabled;
                            break;
                        }
                        case ScheduleID.None:
                        {
                            m_NoneUpdateFrequency = Frequency;
                            m_UpdateNoneEnabled = Enabled;
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or Sets whether or not Time should be automatically updated
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created
        
        public bool UpdateTimeEnabled
        {
            get
            {
                return m_UpdateTimeEnabled;
            }
            set
            {
                m_UpdateTimeEnabled = value;
            }
        }

        /// <summary>
        /// Gets or Sets whether or not Messages should be automatically updated
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created
        
        public bool UpdateMessageEnabled
        {
            get
            {
                return m_UpdateMessageEnabled;
            }
            set
            {
                m_UpdateMessageEnabled = value;
            }
        }

        /// <summary>
        /// Gets or Sets whether or not Prices should be automatically updated
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created
        
        public bool UpdatePriceEnabled
        {
            get
            {
                return m_UpdatePriceEnabled;
            }
            set
            {
                m_UpdatePriceEnabled = value;
            }
        }

        /// <summary>
        /// Gets or Sets whether or not Summations should be automatically updated
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public bool UpdateSummationEnabled
        {
            get
            {
                return m_UpdateSummationEnabled;
            }
            set
            {
                m_UpdateSummationEnabled = value;
            }
        }

        /// <summary>
        /// Gets or Sets whether or not Demands should be automatically updated
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public bool UpdateDemandEnabled
        {
            get
            {
                return m_UpdateDemandEnabled;
            }
            set
            {
                m_UpdateDemandEnabled = value;
            }
        }

        /// <summary>
        /// Gets or Sets whether or not None should be automatically updated
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public bool UpdateNoneEnabled
        {
            get
            {
                return m_UpdateNoneEnabled;
            }
            set
            {
                m_UpdateNoneEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the Time Update Frequency
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public uint TimeUpdateFrequency
        {
            get
            {
                return m_TimeUpdateFrequency;
            }
            set
            {
                m_TimeUpdateFrequency = value;
            }
        }

        /// <summary>
        /// Gets or sets the Message Update Frequency
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public uint MessageUpdateFrequency
        {
            get
            {
                return m_MessageUpdateFrequency;
            }
            set
            {
                m_MessageUpdateFrequency = value;
            }
        }

        /// <summary>
        /// Gets or sets the Price Update Frequency
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public uint PriceUpdateFrequency
        {
            get
            {
                return m_PriceUpdateFrequency;
            }
            set
            {
                m_PriceUpdateFrequency = value;
            }
        }

        /// <summary>
        /// Gets or sets the Summation Update Frequency
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public uint SummationUpdateFrequency
        {
            get
            {
                return m_SummationUpdateFrequency;
            }
            set
            {
                m_SummationUpdateFrequency = value;
            }
        }

        /// <summary>
        /// Gets or sets the Demand Update Frequency
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public uint DemandUpdateFrequency
        {
            get
            {
                return m_DemandUpdateFrequency;
            }
            set
            {
                m_DemandUpdateFrequency = value;
            }
        }

        /// <summary>
        /// Gets or sets the None Update Frequency
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public uint NoneUpdateFrequency
        {
            get
            {
                return m_NoneUpdateFrequency;
            }
            set
            {
                m_NoneUpdateFrequency = value;
            }
        }

        #endregion

        #region Member Variables

        private bool m_UpdateTimeEnabled;
        private bool m_UpdateMessageEnabled;
        private bool m_UpdatePriceEnabled;
        private bool m_UpdateSummationEnabled;
        private bool m_UpdateDemandEnabled;
        private bool m_UpdateNoneEnabled;

        private uint m_TimeUpdateFrequency;
        private uint m_MessageUpdateFrequency;
        private uint m_PriceUpdateFrequency;
        private uint m_SummationUpdateFrequency;
        private uint m_DemandUpdateFrequency;
        private uint m_NoneUpdateFrequency;

        #endregion
    }

    /// <summary>
    /// Message object for the RAVEn ZigBee module
    /// </summary>
    public class RAVEnMessage : IEquatable<RAVEnMessage>
    {
        #region Constants

        private static readonly string XML_CLUSTER = "MessageCluster";
        private static readonly string XML_ID = "ID";
        private static readonly string XML_TEXT = "Text";
        private static readonly string XML_CONFIRMATION_REQUIRED = "ConfirmationRequired";
        private static readonly string XML_CONFIRMED = "Confirmed";
        private static readonly string XML_QUEUE = "Queue";

        #endregion

        #region Definitions

        /// <summary>
        /// Which message queue the message belongs to
        /// </summary>
        public enum QueueType
        {
            /// <summary>
            /// The Queue was not specified in the message
            /// </summary>
            [EnumDescription("Not Specified")]
            NotSpecified,
            /// <summary>
            /// Message is in the Active Queue
            /// </summary>
            [EnumDescription("Active")]
            Active,
            /// <summary>
            /// Message is in the Cancel Pending Queue
            /// </summary>
            [EnumDescription("Cancel Pending")]
            CancelPending,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public RAVEnMessage()
        {
            m_MessageID = 0;
            m_Message = "";
            m_ConfirmationRequired = false;
            m_Confirmed = false;
            m_QueueType = QueueType.NotSpecified;
        }

        /// <summary>
        /// Parses the message from the specified element
        /// </summary>
        /// <param name="element">The element to parse from</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public void ParseElement(XElement element)
        {
            if (element != null && element.Name.LocalName.Equals(XML_CLUSTER))
            {
                IEnumerable<XElement> ItemElements = element.Elements(XML_ID);
                //This variable is used to ensure that no Parse() calls will happen on a null or empty string.
                string ElementToParse = "";
                if (ItemElements.Count() > 0)
                {
                    ElementToParse = ItemElements.First().Value.Replace("0x", "");
                    if (!String.IsNullOrEmpty(ElementToParse))
                    {
                        m_MessageID = uint.Parse(ItemElements.First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        //Prevent an empty Message from being put in the MessageList
                        throw new ArgumentNullException("MessageID");
                    }
                }

                ItemElements = element.Elements(XML_TEXT);

                if (ItemElements.Count() > 0)
                {
                    m_Message = ItemElements.First().Value;
                }

                ItemElements = element.Elements(XML_CONFIRMATION_REQUIRED);

                if (ItemElements.Count() > 0)
                {
                    m_ConfirmationRequired = ItemElements.First().Value.Equals("Y");
                }

                ItemElements = element.Elements(XML_CONFIRMED);

                if (ItemElements.Count() > 0)
                {
                    m_Confirmed = ItemElements.First().Value.Equals("Y");
                }

                ItemElements = element.Elements(XML_QUEUE);

                if (ItemElements.Count() > 0)
                {
                    m_QueueType = EnumDescriptionRetriever.ParseToEnum<QueueType>(ItemElements.First().Value);
                }
            }
        }

        /// <summary>
        /// Determines if the Messages are equal
        /// </summary>
        /// <param name="other">The message to compare to.</param>
        /// <returns>True if the messages are equal. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public bool Equals(RAVEnMessage other)
        {
            bool bEqual = false;

            if (other != null)
            {
                bEqual = MessageID == other.MessageID;
            }

            return bEqual;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Message ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public uint MessageID
        {
            get
            {
                return m_MessageID;
            }
        }

        /// <summary>
        /// Gets the Message text
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public string Message
        {
            get
            {
                return m_Message;
            }
        }

        /// <summary>
        /// Gets whether or not the message requires confirmation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public bool ConfirmationRequired
        {
            get
            {
                return m_ConfirmationRequired;
            }
        }

        /// <summary>
        /// Gets whether or not the message has been confirmed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public bool Confirmed
        {
            get
            {
                return m_Confirmed;
            }
        }

        #endregion

        #region Member Variables

        private uint m_MessageID;
        private string m_Message;
        private bool m_ConfirmationRequired;
        private bool m_Confirmed;
        private QueueType m_QueueType;

        #endregion
    }

    /// <summary>
    /// Price object for the RAVEn ZigBee module
    /// </summary>
    public class RAVEnPrice : IEquatable<RAVEnPrice>
    {
        #region Constants

        private static readonly string XML_PRICECLUSTER = "PriceCluster";
        private static readonly string XML_PRICE = "Price";
        private static readonly string XML_CURRENCY = "Currency";
        private static readonly string XML_TRAILINGDIGITS = "TrailingDigits";
        private static readonly string XML_TIER = "Tier";
        private static readonly string XML_TIERLABEL = "TierLabel";
        private static readonly string XML_RATELABEL = "RateLabel";

        #endregion

        #region Definitions

        /// <summary>
        /// ISO 4217 Currency Codes
        /// </summary>
        public enum CurrencyType
        {
            /// <summary>
            /// Unknown/Not Used Currency
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// Australian Dollar
            /// </summary>
            AUD = 36,
            /// <summary>
            /// Canadian Dollar
            /// </summary>
            CAD = 124,
            /// <summary>
            /// Chinese Yen
            /// </summary>
            CNY = 156,
            /// <summary>
            /// Indian Rupee
            /// </summary>
            INR = 356,
            /// <summary>
            /// Japanese Yen
            /// </summary>
            JPY = 392,
            /// <summary>
            /// Mexican Peso
            /// </summary>
            MXN = 484,
            /// <summary>
            /// Qatari Rial
            /// </summary>
            QAR = 634,
            /// <summary>
            /// Pound Sterling
            /// </summary>
            GBP = 826,
            /// <summary>
            /// US Dollar
            /// </summary>
            USD = 840,
            /// <summary>
            /// Euro
            /// </summary>
            EUR = 978,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public RAVEnPrice()
        {
        }

        /// <summary>
        /// Determines whether or not the two RAVEn Price objects are equal
        /// </summary>
        /// <param name="other">The RAVEn price to compare against</param>
        /// <returns>True if the prices are equal. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/06/12 RCG 2.70.02        Created

        public bool Equals(RAVEnPrice other)
        {
            bool IsEqual = false;

            if (other != null)
            {
                if (Currency == other.Currency)
                {
                    if (RateLabel == other.RateLabel)
                    {
                        if (TierLabel == other.TierLabel)
                        {
                            if (Price == other.Price)
                            {
                                if (Tier == other.Tier)
                                {
                                    IsEqual = true;
                                }
                            }
                        }
                    }
                }
            }

            return IsEqual;
        }

        /// <summary>
        /// Parses the Price info from the specified element
        /// </summary>
        /// <param name="element">The element to parse from</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created
        //  07/06/11 WW  2.51.20        Fixed a bug in price calculating involving TrailingDigit from payload (byDigits in RAVEn code)

        public void ParseElement(XElement element)
        {
            if (element != null && element.Name.LocalName.Equals(XML_PRICECLUSTER))
            {
                uint uiPrice = 0;
                byte byDigits = 0;

                // Get the price information
                if (element.Elements(XML_PRICE).Count() > 0)
                {
                    uiPrice = uint.Parse(element.Elements(XML_PRICE).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                }

                if (element.Elements(XML_TRAILINGDIGITS).Count() > 0)
                {
                    byDigits = byte.Parse(element.Elements(XML_TRAILINGDIGITS).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                }

                // The actual price is the raw value adjust for the number of trailing digits
                m_Price = (double)uiPrice / (double)(Math.Pow(10.0, (double)byDigits));

                if (element.Elements(XML_CURRENCY).Count() > 0)
                {
                    m_Currency = (CurrencyType)ushort.Parse(element.Elements(XML_CURRENCY).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                }

                if (element.Elements(XML_TIER).Count() > 0)
                {
                    m_Tier = byte.Parse(element.Elements(XML_TIER).First().Value.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                }

                if(element.Elements(XML_TIERLABEL).Count() > 0)
                {
                    m_TierLabel = element.Elements(XML_TIERLABEL).First().Value;
                }

                if (element.Elements(XML_RATELABEL).Count() > 0)
                {
                    m_RateLabel = element.Elements(XML_RATELABEL).First().Value;
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public double Price
        {
            get
            {
                return m_Price;
            }
        }

        /// <summary>
        /// Gets the currency type for the price
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public CurrencyType Currency
        {
            get
            {
                return m_Currency;
            }
        }

        /// <summary>
        /// Gets the current tier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public byte Tier
        {
            get
            {
                return m_Tier;
            }
        }

        /// <summary>
        /// Gets the label for the current tier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public string TierLabel
        {
            get
            {
                return m_TierLabel;
            }
        }

        /// <summary>
        /// Gets the label for the current rate
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public string RateLabel
        {
            get
            {
                return m_RateLabel;
            }
        }

        #endregion

        #region Member Variables

        private double m_Price;
        private CurrencyType m_Currency;
        private byte m_Tier;
        private string m_TierLabel;
        private string m_RateLabel;

        #endregion
    }

    /// <summary>
    /// Status Event Handle delegate
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void StatusEventHandler(object sender, StatusEventArgs e);

    /// <summary>
    /// Status event arguments
    /// </summary>
    public class StatusEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status">The events status</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public StatusEventArgs(string status)
        {
            m_Status = status;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/25/11 RCG 2.51.00        Created

        public string Status
        {
            get
            {
                return m_Status;
            }
        }

        #endregion

        #region Member Variables

        private string m_Status;

        #endregion
    }
}
