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
//                           Copyright © 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    internal class OpenWayMFGTable2123 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 100;
        private const int NUM_EVENTS = 12;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table0">The table 0 object for the current device</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/25/11 RCG 2.50.34        Created

        public OpenWayMFGTable2123(CPSEM psem, CTable00 table0)
            : base(psem, 2123, GetTableSize(table0), TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Reads the data from the meter
        /// </summary>
        /// <returns>The PSEM Response from the read</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/25/11 RCG 2.50.34        Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMfgTable2123.Read");

            PSEMResponse Result = base.Read();

            //Populate the member variables that represent the table
            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of scheduled events
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/25/11 RCG 2.50.34        Created

        public List<ScheduledEvent> ScheduledEvents
        {
            get
            {
                ReadUnloadedTable();

                return m_Events;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the size of the table in bytes
        /// </summary>
        /// <param name="table0">The table 0 object for the current device</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/25/11 RCG 2.50.34        Created

        private static uint GetTableSize(CTable00 table0)
        {
            return (1 + table0.STIMESize) * NUM_EVENTS;
        }

        /// <summary>
        /// Parses the data that was just read from the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/25/11 RCG 2.50.34        Created

        private void ParseData()
        {
            m_Events = new List<ScheduledEvent>();

            for (int iIndex = 0; iIndex < NUM_EVENTS; iIndex++)
            {
                byte byEvent = m_Reader.ReadByte();
                DateTime dtDate = m_Reader.ReadSTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);

                m_Events.Add(new ScheduledEvent((ScheduledEventTypes)byEvent, dtDate));
            }
        }

        #endregion

        #region Member Variables

        private List<ScheduledEvent> m_Events;

        #endregion
    }

    #region ScheduledEventTypes Enum

    /// <summary>
    /// Enumeration for the scheduled events type
    /// </summary>
    public enum ScheduledEventTypes : byte
    {
#pragma warning disable 1591 // Ignores the XML comment warnings
        NoEvent = 0,
        PowerDown = 1,
        PowerUp = 2,
        ClearBillingData = 3,
        BillingScheduleExpiration = 4,
        DSTTimeAdjust = 5,
        TimeChange = 6,
        DemandThresholdExceeded = 7,
        DemandThresholdRestore = 8,
        SelfReadPointersUpdated = 9,
        ANSILogon = 10,
        NewBattery = 11,
        ANSISecuritySuccess = 12,
        ANSISecurityFailed = 13,
        LoadProfileCleared = 14,
        LoadProfilePointersUpdated = 15,
        HistoryLogCleared = 16,
        HistoryLogPointersUpdated = 17,
        EventLogCleared = 18,
        EventLogPointersUpdated = 19,
        DemandReset = 20,
        SelfRead = 21,
        InputChannelHigh = 22,
        InputChannelLow = 23,
        SeasonChange = 24,
        RateChange = 25,
        ExternalEvent = 26,
        SiteScanError = 27,
        PendingTableActivation = 28,
        PendingTableClear = 29,
        PowerQualityPointersUpdated = 30,
        VoltageQualityLogNearFull = 31,
        TestModeEnter = 32,
        TestModeExit = 33,
        ABCPhaseRotationActive = 34,
        CBAPhaseRotationActive = 35,
        MeterReconfigure = 36,
        ConfigError = 37,
        CPCCommunicationError = 38,
        ReversePowerFlowRestore = 39,
        ClearVoltageQualtiyLog = 40,
        TOUScheduleError = 41,
        MassMemoryError = 42,
        LossOfPhaseRestore = 43,
        LowBatteryError = 44,
        LossOfPhaseError = 45,
        RegisterFullScaleEXError = 46,
        TamperAttemptDetected = 47,
        ReversePowerFlowError = 48,
        SiteScanDiag1Active = 49,
        SiteScanDiag2Active = 50,
        SiteScanDiag3Active = 51,
        SiteScanDiag4Active = 52,
        SiteScanDiag5Active = 53,
        SiteScanDiag1Inactive = 54,
        SiteScanDiag2Inactive = 55,
        SiteScanDiag3Inactive = 56,
        SiteScanDiag4Inactive = 57,
        SiteScanDiag5Inactive = 58,
        SiteScanDiag6Active = 59,
        SiteScanDiag6Inactive = 60,
        SelfReadCleared = 61,
        InversionTamper = 62,
        RemovalTamper = 63,
        EOIEnergyUpdate = 64,
        FirmwareDownloadDataSave = 65,
        RegisterDownloadFailed = 66,
        RegisterDownloadSuccess = 67,
        RFLANDownloadSuccess = 68,
        ZigBeeDownloadSuccess = 69,
        FactoryProgram = 70,
        FirstConfig = 71,
        AuxIDTimeout = 72,
        AuxBreakDetected = 73,
        SiteScanSnapshotsFull = 74,
        SpecialReconfig = 75,
        RollbackConfig = 76,
        VoltageQualityEventOccurred = 77,
        GetVoltageQualityData = 78,
        StartTurboTest = 79,
        TurboTestPoll = 80,
        ZigBeeDownloadFailed = 81,
        RFLANDownloadFailed = 82,
        NewServiceType = 83,
        SiteScanErrorCleared = 84,
        LoadFirmware = 85,
        BackupData = 86,
        ForceScrollLock = 87,
        OneSecondTick = 88,
        OneMinuteTick = 89,
        ThreeButtonReset = 90,
        CPCUpdated = 91,
        ValidateConfig = 92,
        StartBatteryTest = 93,
        NormalDisplayMode = 94,
        AlternateDisplayMode = 95,
        ToolDisplayMode = 96,
        TestToTestAlt = 97,
        LoadProfileEOI = 98,
        DemandEOI = 99,
        EditEnergyValue = 100,
        ResetCounters = 101,
        TOUScheduleOK = 102,
        SetInterrogateTime = 103,
        CommitHistoryLog = 104,
        IOIndependantOutputOn = 105,
        IOIndependantOutputOff = 106,
        ScheduledSelfRead = 107,
        ScheduledOptionBoardEvent = 108,
        RecurringDate0 = 109,
        RecurringDate1 = 110,
        RecurringDate2 = 111,
        RecurringDate3 = 112,
        NonRecurringDate0 = 113,
        NonRecurringDate1 = 114,
        NonRecurringDate2 = 115,
        NonRecurringDate3 = 116,
        WeeklyScheduleCall0 = 117,
        WeeklyScheduleCall1 = 118,
        WeeklyScheduleCall2 = 119,
        WeeklyScheduleCall3 = 120,
        FatalError = 121,
        RealTimePricing = 122,
        WriteLEDConfig = 123,
        InitCPC = 124,
        PeriodicReadTransmitReport0 = 125,
        ServiceLimitingActiveTierChanged = 126,
        ServiceLimitingConnectSwitch = 127,
        RndRcdExceptions = 128,
        ServiceLimitingSwitchPeriod = 129,
        PeriodicReadDemandReset = 132,
        PeriodicReadSelfRead = 133,
        PendingReconfigure = 134,
        RNDPowerUp = 143,
        VoltageMonitoringEOI = 145
#pragma warning restore 1591
    }

    #endregion

    /// <summary>
    /// A Scheduled Event
    /// </summary>
    public class ScheduledEvent
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">The event type of the event</param>
        /// <param name="date">The date and time the event is scheduled</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/25/11 RCG 2.50.34        Created

        public ScheduledEvent(ScheduledEventTypes type, DateTime date)
        {
            m_EventType = type;
            m_ScheduledDate = date;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the event that is scheduled
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/25/11 RCG 2.50.34        Created

        public ScheduledEventTypes Event
        {
            get
            {
                return m_EventType;
            }
        }

        /// <summary>
        /// Gets the date and time the event is scheduled
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/25/11 RCG 2.50.34        Created

        public DateTime ScheduledDate
        {
            get
            {
                return m_ScheduledDate;
            }
        }

        #endregion

        #region Member Variables

        private ScheduledEventTypes m_EventType;
        private DateTime m_ScheduledDate;

        #endregion
    }
}
