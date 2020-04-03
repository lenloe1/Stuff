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
//                           Copyright © 2013 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Itron.Metering.Device.DLMSDevice
{
    /// <summary>
    /// Translates a logical name to a text description
    /// </summary>
    public class COSEMLogicalNamesDictionary : Dictionary<string, string>
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/21/12 RCG 2.80.01 N/A    Created

        public COSEMLogicalNamesDictionary()
        {
            // Add the Logical Names and their corresponding string representations
            Add("0-0:1.0.0*255", COSEMResourceStrings.Clock);
            Add("0-0:10.0.0*255", COSEMResourceStrings.GlobalMeterResetScripts);
            Add("0-0:10.0.106*255", COSEMResourceStrings.RemoteConnectDiscconectScript);
            Add("0-0:10.0.107*255", COSEMResourceStrings.FirmwareLoadScriptTable);
            Add("0-0:10.0.108*255", COSEMResourceStrings.AlarmScripts);
            Add("0-0:10.0.128*255", COSEMResourceStrings.ConfigurationScripts);
            Add("0-0:10.0.129*255", COSEMResourceStrings.NetworkTimeSyncScripts);
            Add("0-0:10.0.130*255", COSEMResourceStrings.SelfReadScripts);
            Add("0-0:10.0.131*255", COSEMResourceStrings.InterrogationCleanupSystemScripts);
            Add("0-0:10.0.132*255", COSEMResourceStrings.ImageActivationCompleteScripts);
            Add("0-0:10.0.133*255", COSEMResourceStrings.AlarmScripts);
            Add("0-0:10.0.134*255", COSEMResourceStrings.NetworkActivitySuspensionScripts);
            Add("0-0:15.0.2*255", COSEMResourceStrings.FirmwareLoadSingleActionSchedule);
            Add("0-0:15.0.130*255", COSEMResourceStrings.SelfReadSingleActionSchedule);
            Add("0-0:16.0.128*255", COSEMResourceStrings.ImageActivationMonitor);
            Add("0-0:16.0.129*255", COSEMResourceStrings.NetworkActivitySuspensionMonitor);
            Add("0-0:16.0.130*255", COSEMResourceStrings.ItronAlarmRegisterMonitor);
            Add("0-0:16.1.0*255", COSEMResourceStrings.AlarmMonitor1);
            Add("0-0:22.0.0*255", COSEMResourceStrings.HLDCSettings);
            Add("0-0:40.0.0*255", COSEMResourceStrings.CurrentAssociation);
            Add("0-0:40.0.1*255", COSEMResourceStrings.VerificationClientAssociation);
            Add("0-0:40.0.2*255", COSEMResourceStrings.HTClientAssociation);
            Add("0-0:40.0.3*255", COSEMResourceStrings.CommunicationDeviceAssociation);
            Add("0-0:42.0.0*255", COSEMResourceStrings.COSEMLogicalDeviceName);
            Add("0-0:43.0.0*255", COSEMResourceStrings.SecuritySetup);
            Add("0-0:43.0.1*255", COSEMResourceStrings.SecuritySetupEncryptAndAuthenticate);
            Add("0-0:44.0.0*255", COSEMResourceStrings.ImageTransfer);
            Add("0-65:43.0.0*255", COSEMResourceStrings.LockoutCount);
            Add("0-65:43.0.1*255", COSEMResourceStrings.LockoutTime);
            Add("0-0:96.1.0*255", COSEMResourceStrings.DeviceID1);
            Add("0-0:96.1.1*255", COSEMResourceStrings.DeviceID2);
            Add("0-0:96.1.2*255", COSEMResourceStrings.DeviceID3);
            Add("0-0:96.1.3*255", COSEMResourceStrings.DeviceID4);
            Add("0-0:96.1.4*255", COSEMResourceStrings.DeviceID5);
            Add("0-0:96.3.10*255", COSEMResourceStrings.DisconnectControl);
            Add("0-0:96.6.0*255", COSEMResourceStrings.TotalTimeOnBattery);
            Add("0-0:96.6.3*255", COSEMResourceStrings.LastBatteryReading);
            Add("0-0:96.7.0*255", COSEMResourceStrings.TotalOutageCount);
            Add("0-0:96.11.0*255", COSEMResourceStrings.EventCode);
            Add("0-0:96.15.0*255", COSEMResourceStrings.EventDataRecordNumber);
            Add("0-0:96.15.1*255", COSEMResourceStrings.LPSet1Counter);
            Add("0-0:96.15.2*255", COSEMResourceStrings.LPSet2Counter);
            Add("0-0:96.15.3*255", COSEMResourceStrings.SwitchingOperationHistoryRecordNumber);
            Add("0-0:97.98.0*255", COSEMResourceStrings.AlarmRegister1);
            Add("0-0:97.98.1*255", COSEMResourceStrings.AlarmRegister2);
            Add("0-0:97.98.10*255", COSEMResourceStrings.AlarmFilter1);
            Add("0-0:97.98.11*255", COSEMResourceStrings.AlarmFilter2);
            Add("0-0:97.98.20*255", COSEMResourceStrings.AlarmDescriptor1);
            Add("0-0:97.98.21*255", COSEMResourceStrings.AlarmDescriptor2);
            Add("0-0:97.98.255*255", COSEMResourceStrings.AlarmRegistersProfile);
            Add("0-0:98.1.0*255", COSEMResourceStrings.BillingPeriod1Data);
            Add("0-0:99.98.0*255", COSEMResourceStrings.EventData);
            Add("0-0:99.98.2*255", COSEMResourceStrings.SwitchOperationHistory);
            Add("0-0:99.98.10*255", COSEMResourceStrings.BillingSelfReadData);
            Add("0-0:99.98.11*255", COSEMResourceStrings.NonBillingSelfReadData);
            Add("0-0:99.98.12*255", COSEMResourceStrings.BillingDailySelfReadData);
            Add("0-0:99.98.13*255", COSEMResourceStrings.NonBillingDailySelfReadData);
            Add("0-1:0.2.1*255", COSEMResourceStrings.HumanReadableFirmwareVersion);
            Add("0-4:25.9.0*255", COSEMResourceStrings.AlarmPushSetup);
            Add("0-7:25.9.0*255", COSEMResourceStrings.RegistrationPushSetup);
            Add("0-128:25.9.0*255", COSEMResourceStrings.ImageActivationPushSetup);
            Add("0-128:96.0.0*255", COSEMResourceStrings.ConfigurationTag);
            Add("0-128:96.0.1*255", COSEMResourceStrings.ConfigurationXML);
            Add("0-128:96.0.2*255", COSEMResourceStrings.ReconfigureResult);
            Add("0-128:96.0.3*255", COSEMResourceStrings.MinimumTimeSynchDelta);
            Add("0-128:96.1.0*255", COSEMResourceStrings.MulticastServerApTitles);
            Add("0-128:96.1.1*255", COSEMResourceStrings.RegistrationsInUse);
            Add("0-128:96.1.2*255", COSEMResourceStrings.TimeUntilRegistration);
            Add("0-128:96.1.3*255", COSEMResourceStrings.RangeForRegistrationAttempts);
            Add("0-128:96.1.4*255", COSEMResourceStrings.PushDestinationOverride);
            Add("0-128:96.1.5*255", COSEMResourceStrings.RegistrationStatus);
            Add("0-128:96.1.6*255", COSEMResourceStrings.MulticastServerApTitlesNoConfig);
            Add("0-128:96.2.0*255", COSEMResourceStrings.ItronEventCode);
            Add("0-128:96.2.1*255", COSEMResourceStrings.ItronEventDetail);
            Add("0-128:96.2.2*255", COSEMResourceStrings.ItronEventLog);
            Add("0-128:96.2.3*255", COSEMResourceStrings.ItronAlarmIndicator);
            Add("0-128:96.3.0*255", COSEMResourceStrings.InterrogationBuilder);
            Add("0-128:96.3.1*255", COSEMResourceStrings.ImageProcessStatus);
            Add("0-128.96.3.2*255", COSEMResourceStrings.ImProvStatus);
            Add("0-128:96.4.0*255", COSEMResourceStrings.NetworkActivitySuspensionDuration);
            Add("0-128:96.6.0*255", COSEMResourceStrings.DaylightSavingsSchedule);
            Add("0-128:96.7.0*255", COSEMResourceStrings.DbusCommandExecute);
            Add("0-128:96.7.1*255", COSEMResourceStrings.RetrieveLogFiles);
            Add("0-128:96.7.2*255", COSEMResourceStrings.LineSyncExpectedFrequency);
            Add("0-128:96.10.0*255", COSEMResourceStrings.NGCInSpeed);
            Add("0-128:96.10.1*255", COSEMResourceStrings.NGCOutSpeed);
            Add("0-128:96.10.2*255", COSEMResourceStrings.NGCAdminStatus);
            Add("0-128:96.10.3*255", COSEMResourceStrings.NGCOutputStatus);
            Add("0-128:96.10.4*255", COSEMResourceStrings.NGCInOctets);
            Add("0-128:96.10.5*255", COSEMResourceStrings.NGCOutOctets);
            Add("0-128:96.10.6*255", COSEMResourceStrings.NGCInDiscards);
            Add("0-128:96.10.7*255", COSEMResourceStrings.NGCInErrors);
            Add("0-128:96.10.8*255", COSEMResourceStrings.NGCOutDiscards);
            Add("0-128:96.10.9*255", COSEMResourceStrings.NGCOutErrors);
            Add("0-128:96.10.10*255", COSEMResourceStrings.NGCInUnicastPackets);
            Add("0-128:96.10.11*255", COSEMResourceStrings.NGCInBroadcastPackets);
            Add("0-128:96.10.12*255", COSEMResourceStrings.NGCInMulticastPackets);
            Add("0-128:96.10.13*255", COSEMResourceStrings.NGCInUnknownProtocolPackets);
            Add("0-128:96.10.14*255", COSEMResourceStrings.NGCOutUnicastPackets);
            Add("0-128:96.10.15*255", COSEMResourceStrings.NGCOutBroadcastPackets);
            Add("0-128:96.10.16*255", COSEMResourceStrings.NGCOutMulticastPackets);
            Add("0-128:96.10.17*255", COSEMResourceStrings.NGCOutQueueLength);
            Add("0-128:96.10.18*255", COSEMResourceStrings.NGCMTUSize);
            Add("0-128:96.10.19*255", COSEMResourceStrings.NGCPhysicalAddressMAC);
            Add("0-128:96.10.20*255", COSEMResourceStrings.NGCInUDPv6Datagrams);
            Add("0-128:96.10.21*255", COSEMResourceStrings.NGCInUDPv6NoPort);
            Add("0-128:96.10.22*255", COSEMResourceStrings.NGCInUDPv6Errors);
            Add("0-128:96.10.23*255", COSEMResourceStrings.NGCOutUDPv6Datagrams);
            Add("0-128:96.10.24*255", COSEMResourceStrings.NGCRPLInstanceID);
            Add("0-128:96.10.25*255", COSEMResourceStrings.NGCRPLDoDagID);
            Add("0-128:96.10.26*255", COSEMResourceStrings.NGCRPLDoDagVersion);
            Add("0-128:96.10.27*255", COSEMResourceStrings.NGCRPLDoDagLastChanged);
            Add("0-128:96.10.28*255", COSEMResourceStrings.NGCRPLRank);
            Add("0-128:96.10.29*255", COSEMResourceStrings.NGCRPLNumberOfParents);
            Add("0-128:96.10.30*255", COSEMResourceStrings.NGCRPLNumberOfBestParents);
            Add("0-128:96.10.31*255", COSEMResourceStrings.NGCRPLBestParentsList);
            Add("0-128:96.10.32*255", COSEMResourceStrings.NGCRPLNumberOfNeighbors);
            Add("0-128:96.10.33*255", COSEMResourceStrings.NGCRPLNumberOfPANs);
            Add("0-128:96.10.34*255", COSEMResourceStrings.NGCRPLPANID);
            Add("0-128:96.10.35*255", COSEMResourceStrings.NGCRPLGlobalETT);
            Add("0-128:96.10.36*255", COSEMResourceStrings.NGCRPLGlobalLQL);
            Add("0-128:96.10.37*255", COSEMResourceStrings.NGCRPLDoDagSize);
            Add("0-128:96.10.38*255", COSEMResourceStrings.NGCRPLNumberOfMalformedMessages);
            Add("0-128:96.10.39*255", COSEMResourceStrings.NGCRPLNumberOfLocallyRepairedMessages);
            Add("0-128:96.10.40*255", COSEMResourceStrings.NGCRPLNumberOfGloballyRepairedMessages);
            Add("0-128:96.10.41*255", COSEMResourceStrings.NGCRPLNumberOfMigrations);
            Add("0-128:96.10.42*255", COSEMResourceStrings.NGCRPLDIOMessagesReceived);
            Add("0-128:96.10.43*255", COSEMResourceStrings.NGCRPLDIOMessagesTransmitted);
            Add("0-128:96.10.44*255", COSEMResourceStrings.NGCRPLDISMessagesReceived);
            Add("0-128:96.10.45*255", COSEMResourceStrings.NGCRPLDISMessagesTransmitted);
            Add("0-128:96.10.46*255", COSEMResourceStrings.NGCRPLDAOMessagesReceived);
            Add("0-128:96.10.47*255", COSEMResourceStrings.NGCRPLDAOMessagesTransmitted);
            Add("0-128:96.10.48*255", COSEMResourceStrings.NGCRPLDAOAckMessagesReceived);
            Add("0-128:96.10.49*255", COSEMResourceStrings.NGCRPLDAOAckMessagesTransmitted);
            Add("0-128:96.10.50*255", COSEMResourceStrings.NGCRPLUpwardDirectedMessagesReceived);
            Add("0-128:96.10.51*255", COSEMResourceStrings.NGCRPLUpwardDirectedMessagesTransmitted);
            Add("0-128:96.10.52*255", COSEMResourceStrings.NGCRPLDownwardDirectedMessagesReceived);
            Add("0-128:96.10.53*255", COSEMResourceStrings.NGCRPLDownwardDirectedMessagesTransmitted);
            Add("0-128:96.10.54*255", COSEMResourceStrings.NGCRPLIPv6Address);
            Add("0-128:96.11.0*255", COSEMResourceStrings.NGCDebugInterfaceMetrics);
            Add("0-128:96.11.1*255", COSEMResourceStrings.NGCDebugDetailedMetrics);
            Add("0-128:96.11.2*255", COSEMResourceStrings.NGCDebugDescription);
            Add("0-128:96.11.3*255", COSEMResourceStrings.NGCDebugUDPMetrics);
            Add("0-128:96.11.4*255", COSEMResourceStrings.NGCDebugRPLInstance);
            Add("0-128:96.11.5*255", COSEMResourceStrings.NGCDebugRPLConfig);
            Add("0-128:96.11.6*255", COSEMResourceStrings.NGCDebugRPLWarmStart);
            Add("0-128:96.11.7*255", COSEMResourceStrings.NGCDebugRPLStatistics);
            Add("0-128:96.11.8*255", COSEMResourceStrings.NGCDebugNANDriver);
            Add("0-128:98.0.0*255", COSEMResourceStrings.NetworkStatisticsList);
            Add("0-129:25.9.0*255", COSEMResourceStrings.ManualReconnectPushSetup);
            Add("0-130:25.9.0*255", "");
            Add("0-131:25.9.0*255", COSEMResourceStrings.DeregisterPushSetup);
            Add("0-157:0.189.203*255", COSEMResourceStrings.BootCountUnlock);
            Add("1-0:0.0.0*255", COSEMResourceStrings.InstrumentModelType);
            Add("1-0:0.2.4*255", COSEMResourceStrings.PhaseWireSystemType);
            Add("1-0:0.4.2*255", COSEMResourceStrings.CTRatioNumerator);
            Add("1-0:0.4.3*255", COSEMResourceStrings.VTRatioNumerator);
            Add("1-0:0.4.4*255", COSEMResourceStrings.TransformerRatioNumerator);
            Add("1-0:0.4.5*255", COSEMResourceStrings.CTRatioDenominator);
            Add("1-0:0.4.6*255", COSEMResourceStrings.VTRatioDenominator);
            Add("1-0:0.4.7*255", COSEMResourceStrings.TransformerRatioDenominator);
            Add("1-0:0.6.0*255", COSEMResourceStrings.RatedVoltage);
            Add("1-0:0.6.1*255", COSEMResourceStrings.RatedCurrent);
            Add("1-0:0.9.1*255", COSEMResourceStrings.CurrentTime);
            Add("1-0:0.9.2*255", COSEMResourceStrings.CurrentDate);
            Add("1-0:1.8.0*255", COSEMResourceStrings.WhDelivered);
            Add("1-0:1.25.0*255", COSEMResourceStrings.WDelivered);
            Add("1-0:2.8.0*255", COSEMResourceStrings.WhReceived);
            Add("1-0:2.25.0*255", COSEMResourceStrings.WReceived);
            Add("1-0:3.8.0*255", COSEMResourceStrings.varhDelivered);
            Add("1-0:4.8.0*255", COSEMResourceStrings.varhReceived);
            Add("1-0:9.8.0*255", COSEMResourceStrings.VAhDelivered);
            Add("1-0:10.8.0*255", COSEMResourceStrings.VAhReceived);
            Add("1-0:13.7.0*255", COSEMResourceStrings.InstantaneousPowerFactor);
            Add("1-0:13.24.0*255", COSEMResourceStrings.AveragePF);
            Add("1-0:13.25.0*255", COSEMResourceStrings.LastAveragePF);
            Add("1-0:21.8.0*255", COSEMResourceStrings.WhDeliveredPhaseA);
            Add("1-0:22.8.0*255", COSEMResourceStrings.WhReceivedPhaseA);
            Add("1-0:23.8.0*255", COSEMResourceStrings.varhDeliveredPhaseA);
            Add("1-0:31.7.0*255", COSEMResourceStrings.InstantaneousCurrentPhaseA);
            Add("1-0:31.24.0*255", COSEMResourceStrings.AverageCurrentPhaseA);
            Add("1-0:31.25.0*255", COSEMResourceStrings.LastAverageIPhaseA);
            Add("1-0:32.4.0*255", COSEMResourceStrings.ThirtyMinAverageCurrentPhaseA);
            Add("1-0:32.5.0*255", COSEMResourceStrings.ThirtyMinAverageVoltagePhaseA);
            Add("1-0:32.7.0*255", COSEMResourceStrings.InstantaneousVoltagePhaseA);
            Add("1-0:32.24.0*255", COSEMResourceStrings.AverageVoltagePhaseA);
            Add("1-0:32.25.0*255", COSEMResourceStrings.LastAverageVPhaseA);
            Add("1-0:32.128.0*255", COSEMResourceStrings.LoadSideVoltage);
            Add("1-0:41.8.0*255", COSEMResourceStrings.WhDeliveredPhaseB);
            Add("1-0:42.8.0*255", COSEMResourceStrings.WhReceivedPhaseB);
            Add("1-0:43.8.0*255", COSEMResourceStrings.varhDeliveredPhaseB);
            Add("1-0:51.7.0*255", COSEMResourceStrings.InstantaneousCurrentPhaseB);
            Add("1-0:51.24.0*255", COSEMResourceStrings.AverageCurrentPhaseB);
            Add("1-0:51.25.0*255", COSEMResourceStrings.LastAverageIPhaseB);
            Add("1-0:52.4.0*255", COSEMResourceStrings.ThirtyMinAverageCurrentPhaseB);
            Add("1-0:52.5.0*255", COSEMResourceStrings.ThirtyMinAverageVoltagePhaseB);
            Add("1-0:52.7.0*255", COSEMResourceStrings.InstantaneousVoltagePhaseB);
            Add("1-0:52.24.0*255", COSEMResourceStrings.AverageVoltagePhaseB);
            Add("1-0:52.25.0*255", COSEMResourceStrings.LastAverageVPhaseB);
            Add("1-0:61.8.0*255", COSEMResourceStrings.WhDeliveredPhaseC);
            Add("1-0:62.8.0*255", COSEMResourceStrings.WhReceivedPhaseC);
            Add("1-0:63.8.0*255", COSEMResourceStrings.varhDeliveredPhaseC);
            Add("1-0:71.7.0*255", COSEMResourceStrings.InstantaneousCurrentPhaseC);
            Add("1-0:71.24.0*255", COSEMResourceStrings.AverageCurrentPhaseC);
            Add("1-0:71.25.0*255", COSEMResourceStrings.LastAverageIPhaseC);
            Add("1-0:72.4.0*255", COSEMResourceStrings.ThirtyMinAverageCurrentPhaseC);
            Add("1-0:72.5.0*255", COSEMResourceStrings.ThirtyMinAverageVoltagePhaseC);
            Add("1-0:72.7.0*255", COSEMResourceStrings.InstantaneousVoltagePhaseC);
            Add("1-0:72.24.0*255", COSEMResourceStrings.AverageVoltagePhaseC);
            Add("1-0:72.25.0*255", COSEMResourceStrings.LastAverageVPhaseC);
            Add("1-0:90.25.0*255", COSEMResourceStrings.AverageCurrentAllPhases);
            Add("1-0:99.1.0*255", COSEMResourceStrings.LPSet1);
            Add("1-0:99.1.1*255", COSEMResourceStrings.LPSet2);
            Add("1-1:0.2.0*255", COSEMResourceStrings.ActiveFirmwareVersion);
            Add("1-2:0.2.0*255", COSEMResourceStrings.CommModuleFirmwareVersion);
            Add("1-3:0.2.0*255", COSEMResourceStrings.PICFirmwareVersion);
            Add("1-65:0.0.0*255", COSEMResourceStrings.NumberOfIntrumentDigits);
            Add("1-65:0.0.1*255", COSEMResourceStrings.InstrumentFunction);
            Add("1-65:0.16.0*255", COSEMResourceStrings.MultiplyingFactor);
            Add("1-65:0.16.1*255", COSEMResourceStrings.MultiplyingFactorMethod);
            Add("1-65:0.17.0*255", COSEMResourceStrings.EnableEventRecording);
            Add("1-65:0.142.0*255", COSEMResourceStrings.EnergizationStartTime);
            Add("1-65:0.142.1*255", COSEMResourceStrings.IndividualEnergizationConfirmation);
            Add("1-65:0.142.2*255", COSEMResourceStrings.MultiStageEnergizationSetting);
            Add("1-65:0.32.0*255", COSEMResourceStrings.DisplayTimeDelivered);
            Add("1-65:0.32.1*255", COSEMResourceStrings.DisplayTimeReceived);
            Add("1-65:0.33.0*255", COSEMResourceStrings.DisableOtherDisplays);
            Add("1-65:0.33.1*255", COSEMResourceStrings.OtherDisplayValues);
            Add("1-65:0.34.0*255", COSEMResourceStrings.EnableScreenFlicker);
            Add("1-65:0.34.1*255", COSEMResourceStrings.FlickerFlag);
            Add("1-65:0.128.0*255", COSEMResourceStrings.SwitchingClass);
            Add("1-65:0.129.0*255", COSEMResourceStrings.LoadControlBasicSettings);
            Add("1-65:0.129.1*255", COSEMResourceStrings.LoadControlTemporarySettings);
            Add("1-65:0.129.2*255", COSEMResourceStrings.LoadControlOperationValue);
            Add("1-65:0.130.0*255", COSEMResourceStrings.LoadLimitReserve);
            Add("1-65:0.131.0*255", COSEMResourceStrings.NumberOfSwitchingOperations);
            Add("1-65:98.99.0*255", COSEMResourceStrings.MeterSpecifications);
            Add("1-65:98.99.1*255", COSEMResourceStrings.BatchConfirmationOfSettingValues);
            Add("1-65:98.99.2*255", COSEMResourceStrings.MeterConditionConfirmation);
            Add("1-65:98.99.3*255", COSEMResourceStrings.LoadControlConfirmation);
            Add("1-65:98.99.48*255", COSEMResourceStrings.EnergizationStartTimeConfirmation);
            Add("1-65:98.99.49*255", COSEMResourceStrings.IndividualEnergizationConfirmation);
            Add("1-65:99.99.0*255", COSEMResourceStrings.MeterReadingOfCurrentValue);
            Add("1-65:99.99.1*255", COSEMResourceStrings.CurrentValueConfirmation);
            Add("1-65:99.99.2*255", COSEMResourceStrings.SwitchingConditionConfirmation);
            Add("1-128:99.0.0*255", COSEMResourceStrings.InstantaneousProfileData);
            Add("1-128:99.0.1*255", COSEMResourceStrings.RegisterScalarData);
        }

        /// <summary>
        /// Gets the string representation of a logical name
        /// </summary>
        /// <param name="ln">The logical name to get</param>
        /// <returns>The string representation of the LN</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        public static string LogicalNameString(byte[] ln)
        {
            string StringValue = "";

            if (ln != null)
            {
                if (ln.Length == 6)
                {
                    StringValue = ln[0].ToString(CultureInfo.InvariantCulture) + "-"  // A
                        + ln[1].ToString(CultureInfo.InvariantCulture) + ":"          // B
                        + ln[2].ToString(CultureInfo.InvariantCulture) + "."          // C
                        + ln[3].ToString(CultureInfo.InvariantCulture) + "."          // D
                        + ln[4].ToString(CultureInfo.InvariantCulture) + "*"          // E
                        + ln[5].ToString(CultureInfo.InvariantCulture);               // F
                }
                else
                {
                    throw new ArgumentException("The Logical Name must have a length of 6", "logicalName");
                }
            }
            else
            {
                throw new ArgumentNullException("logicalName", "The Logical Name may not be null");
            }

            return StringValue;
        }

        /// <summary>
        /// Parses the logical name from it's string format
        /// </summary>
        /// <param name="logicalName">The logical name in string format</param>
        /// <returns>The logical name as a byte array</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/21/12 RCG 2.80.01 N/A    Created
        
        public static byte[] ParseLogicalName(string logicalName)
        {
            string[] Values = logicalName.Split('-', ':', '.', '*');
            byte[] LNValue = new byte[6];

            if (Values.Length == 6)
            {
                for (int iIndex = 0; iIndex < Values.Length; iIndex++)
                {
                    LNValue[iIndex] = Byte.Parse(Values[iIndex]);
                }
            }
            else
            {
                throw new ArgumentException("The specified string is not a logical name", "logicalName");
            }

            return LNValue;
        }

        #endregion
    }
}
