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
//                              Copyright © 2006 - 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Resources;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Interface which needs to be implemented by a device capable of supporting
    /// HAN Clients
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  11/10/06 AF  8.00.00 N/A    Created
    //
    public interface IClientMetering
    {
        /// <summary>
        /// Client data retrieved from mfg table 2101
        /// </summary>
        /// <returns>list of readings along with tamper information and the customer id</returns>
        List<ClientMeter> ClientMeters
        {
            get;
        }

        /// <summary>
        /// Essentially a dump of manufacturer's table 2100, this property will
        /// give users a way to view the queued up client meter commands.
        /// </summary>
        List<ClientCfgRcd> ClientConfigCommands
        {
            get;
        }

        /// <summary>
        /// Tells the client to set its internal clock to the date
        /// time specified.  The time parameter will be constructed by the
        /// electric meter at the time the command is sent
        /// </summary>
        /// <param name="ulClientAddr">
        /// MAC address of the client to which to send the command
        /// </param>
        /// <returns>ClientMeterCmdResult</returns>
        ClientMeterCmdResult SetClientMeterTime(UInt64 ulClientAddr);

        /// <summary>
        /// Configures how the Gas/Water meter is to collect its meter data
        /// </summary>
        /// <param name="ulClientAddress">
        /// MAC address of the client to which to send the command
        /// </param>
        /// <param name="byDFTHour">
        /// Daily Freeze Hour - the hour at which the daily consumption readings are read
        /// </param>
        /// <param name="byReadingType">
        /// Current Consumption and DFT readings (0x00), 
        /// Hourly interval data (0x01), 
        /// Daily Interval data (0x02)
        /// </param>
        /// <returns>ClientMeterCmdResult</returns>
        ClientMeterCmdResult SetCollectionConfig(UInt64 ulClientAddress, 
                                                 byte byDFTHour, 
                                                 byte byReadingType);

        /// <summary>
        /// Configures the schedule by which the Gas/Water meter
        /// will wake up and send its meter data to the electric meter
        /// </summary>
        /// <param name="ulClientAddress">
        /// MAC address of the client to which to send the command
        /// </param>
        /// <param name="byWakeUpHour">
        /// Hour for wake up and data delivery: midnight is 0, 11 pm is 23
        /// </param>
        /// <param name="byFrequency">
        /// Whether the client should wake up once or twice a day. 1 => every 12 hours;
        /// 0 => every 24 hours.
        /// </param>
        /// <returns></returns>
        ClientMeterCmdResult SetDataDeliveryConfig(UInt64 ulClientAddress, byte byWakeUpHour, byte byFrequency);
     }

     /// <summary>
     /// Enumerates the types of Zigbee nodes.  Gas meters and, eventually, 
     /// water meters are the only types that will have client data
     /// </summary>
     internal enum eNodeType : byte
     {
         /// <summary>
         /// Electric meter
         /// </summary>
         ElectricMeter = 0,
         /// <summary>
         /// Cell relay
         /// </summary>
         CellRelay = 1,
         /// <summary>
         /// Gas meter
         /// </summary>
         GasMeter = 2,
         /// <summary>
         /// Water meter
         /// </summary>
         WaterMeter = 3,
         /// <summary>
         /// Hand Held Computer
         /// </summary>
         HHC = 4,
         /// <summary>
         /// Comverge Gateway
         /// </summary>
         ComvergeGateway = 6,
         /// <summary>
         ///Gas Range Extender
         /// </summary>
         GasRangeExtender = 7,
         /// <summary>
         /// All other types
         /// </summary>
         Other = 8,
     }

     /// <summary>
     /// Enumeration of the Zigbee Application Level Commands
     /// </summary>
     //  Revision History	
     //  MM/DD/YY Who Version Issue# Description
     //  -------- --- ------- ------ -------------------------------------------
     //  04/29/10 AF  2.40.45 140960 Added Read Firmware Version command id
     //  04/21/11 AF  2.50.33 171978 Added Get Battery Information command id
     // 
     public enum eZigbeeAppCmds : byte
     {
         /// <summary>
         /// General purpose ACK
         /// </summary>
         ACK = 0x00,
         /// <summary>
         /// Sets the date and time in a client
         /// </summary>
         SetDateTime = 0x02,
         /// <summary>
         /// Configures how the Gas/Water meter is to collect its meter data
         /// </summary>
         SetCollectionCfg = 0x03,
         /// <summary>
         /// Configures when the Gas/Water meter is to wake up and send its data
         /// to the electric meter
         /// </summary>
         SetDataDeliveryCfg = 0x04,
         /// <summary>
         /// Configures how long the Gas/Water meter will wait before assuming
         /// a communications failure with the Electric meter has occurred
         /// </summary>
         SetTimeoutDuration = 0x05,
         /// <summary>
         /// Returns the current date and time in the Gas/Water meter
         /// </summary>
         GetDateTime = 0x07,
         /// <summary>
         /// Returns the configuration for collecting the Gas/Water meter data
         /// </summary>
         GetCollectionCfg = 0x08,
         /// <summary>
         /// Returns the configuration for the Gas/Water meter wake up time.
         /// </summary>
         GetDataDeliveryCfg = 0x09,
         /// <summary>
         /// Returns the configuration of how long the Gas/Water meter will wait
         /// before assuming a communication failure with the Electric meter has
         /// occurred
         /// </summary>
         GetTimeoutDuration = 0x0A,
         /// <summary>
         /// Instructs the Gas or Water meter to leave the current Network
         /// (PAN ID) and reassociate with another Electric Meter.
         /// </summary>
         ForceDecommission = 0x0B,
         /// <summary>
         /// Provides the ability to send a block of configuration (Programming)
         /// data during the meter installation
         /// </summary>
         SetCfgBlock1 = 0x0C,
         /// <summary>
         /// Retrieves the block of configuration (Programming) data that was
         /// set in the meter.
         /// </summary>
         GetCfgBlock1 = 0x0D,
         /// <summary>
         /// Alters the upper half of the 128-bit Application Security Key
         /// </summary>
         SetHighAppSecKey = 0x0E,
         /// <summary>
         /// Alters the lower half of the 128-bit Application Security Key
         /// </summary>
         SetLowAppSecKey = 0x0F,
         /// <summary>
         /// Alters the upper half of the 128-bit Network Level Security Key
         /// </summary>
         SetHighNetSecKey = 0x10,
         /// <summary>
         /// Alters the lower half of the 128-bit Network Level Security Key
         /// </summary>
         SetLowNetSecKey = 0x11,
         /// <summary>
         /// Sets the variables used to control sensor switch handling 
         /// sensitivity
         /// </summary>
         SetHandlerCfg = 0x12,
         /// <summary>
         /// Returns the variables used to control sensor switch handling 
         /// sensitivity
         /// </summary>
         GetHandlerCfg = 0x13,
         /// <summary>
         /// Sets the variables used to control the RF sequencing under error 
         /// conditions
         /// </summary>
         SetRFRetry = 0x14,
         /// <summary>
         /// Retrieves the RF retry configuration
         /// </summary>
         GetRFRetry = 0x15,
         /// <summary>
         /// Notifies the Gas/Water meter that there are no additional commands
         /// to be processed this cycle
         /// </summary>
         KeepAlive = 0x16,
         /// <summary>
         /// Notifies the Gas/Water meter to immediately start utilizing the
         /// new Application Level Security Key
         /// </summary>
         NewAppKeyActivate = 0x17,
         /// <summary>
         /// Allows the HHC to set the initial consumption value to match 
         /// current dial readings
         /// </summary>
         SetInitConsumption = 0x18,
         /// <summary>
         /// Gets the firmware version of the gas module
         /// </summary>
         ReadFirmwareVersion = 0x41,
         /// <summary>
         /// Gets consumption data only
         /// </summary>
         GetMeterDataLatest = 0x80,
         /// <summary>
         /// Consumption data with hourly interval data
         /// </summary>
         GetMeterDataHourly = 0x81,
         /// <summary>
         /// Consumption data with daily DFT reads
         /// </summary>
         GetMeterDataDaily = 0x82,
         /// <summary>
         /// Gets the consumption data using the SE Profile
         /// </summary>
         GetSEMeterDataLatest = 0x90,
         /// <summary>
         /// Consumption data with hourly interval data (Using SE Profile)
         /// </summary>
         GetSEMeterDataHourly = 0x91,
         /// <summary>
         /// Consumption data with daily DFT reads (Using SE Profile)
         /// </summary>
         GetSEMeterDataDaily = 0x92,
         /// <summary>
         /// Gets information on the battery
         /// </summary>
         GetBatteryInformation = 0xC3,
         /// <summary>
         /// Gets Gas Range Extender status and child data
         /// </summary>
         GetGasRangeExtenderData = 0xF4,
         /// <summary>
         /// General purpose NAK
         /// </summary>
         NAK = 0xFF,
     }

    /// <summary>
    /// Represents all the data available in table 2101 (HAN Client Data Table) 
    /// for a single client
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  11/21/06 AF  8.00.00        Created
    //
     public class ClientMeter
     {
         #region Constants

         private const int SIZE_OF_LATEST_CONSUMPTION_DATA = 24;
         private const int SIZE_OF_HOURLY_CONSUMPTION_DATA = 130;
         private const int SIZE_OF_DAILY_CONSUMPTION_DATA = 161;
         private const int SIZE_OF_SE_LATEST_CONSUMPTION_DATA = 37;
         private const int SIZE_OF_TIME_RESP = 6;
         private const int SIZE_OF_ERROR_RESPONSE = 4;
         private const int SIZE_OF_CMD_RESP = 3;
         private const int SIZE_OF_DECOMM_RESP = 2;
         private const int SIZE_OF_INIT_CONSUMPTION_RESP = 8;
         private const int SIZE_OF_GAS_RANGE_EXTENDER_DATA = 9;
         private const int SIZE_OF_GAS_RANGE_EXTENDER_CHILDREN = 5;
         private static readonly DateTime SE_REFERNCE_TIME = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

         #endregion

         #region Public Methods

         /// <summary>
         /// Constructor
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  11/14/06 AF                 Created
         //
         public ClientMeter()
         {
             m_lstCmdResponses = new List<ClientCommandResponse>();

             //Get the resource manager
             m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS, this.GetType().Assembly);
         }

        /// <summary>
        /// This static method retrieves data from mfg table 2101 and processes
        /// it into Client Meter objects.
        /// </summary>
        /// <param name="lstRcds">A list of HANClientDataRcds.</param>
        /// <returns>a list of ClientMeter objects.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/20/08 jrf 1.50.27 114449 Created so the client data may be processed
        //                              while online with the meter and from a data file.
        //  06/24/08 AF  1.50.42 116686 Changed GetMeterDataLatest case.  The customer id
        //                              bytes should not be reversed.
        //  10/02/08 AF  2.00    120620 Changed the string displayed for the Set Data Delivery
        //                              command.  Also change the string for Set Collection
        //                              Config command.  The CultureInfo setting was incorrect
        // 11/24/09 AF  2.30.22 145687  Retrieve the reading units from the data.  Can no
        //                              longer assume that it will be ccf.
        // 07/25/12 jrf 2.60.48 199605  Adding in the correct decimal place to the Latest Reading and 
        //                              Daily Freeze Readings for SEP Gas Modules.
        //                              For all hourly and daily consumption readings, adding the correct
        //                              decimal place.
        // 10/08/12 PGH 2.70.28 TREQ-6671 Added response processing for command GetGasRangeExtenderData
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.DateTime.ToString")]
        public static List<ClientMeter> ProcessClientData(List<HANClientDataRcd> lstRcds)
         {
             byte[] abyBuffer;
             UInt16 usDataSize;
             byte byCmdId;
             byte byReadingFormat;
             UInt64 ulngValue;
             byte bytTemp;
             UInt16 usTemp;

             ClientMeter objClientMeter;
             List<ClientMeter> lstClientMeter = new List<ClientMeter>();

             // for each client in the list
             for (int iIndex = 0; iIndex < lstRcds.Count; iIndex++)
             {
                 if (0 < lstRcds[iIndex].ClientDataSize)
                 {
                     objClientMeter = new ClientMeter();
                     objClientMeter.MACAddress = lstRcds[iIndex].ClientAddress;
                     usDataSize = lstRcds[iIndex].ClientDataSize;

                     abyBuffer = lstRcds[iIndex].ClientData;
                     
                     objClientMeter.TimeRecorded = lstRcds[iIndex].TimeRecorded;
                     objClientMeter.NodeType = DetermineNodeType(objClientMeter.MACAddress);

                     PSEMBinaryReader DataReader = new PSEMBinaryReader(new MemoryStream(abyBuffer));

                     // for each command in the client's data
                     while (0 < usDataSize)
                     {
                         //Command id
                         byCmdId = DataReader.ReadByte();
                         //Packet version
                         bytTemp = DataReader.ReadByte();

                         ClientCommandResponse cmdResp = new ClientCommandResponse();

                         //assume success unless we see a NAK
                         cmdResp.Success = true;

                         switch (byCmdId)
                         {
                             case (byte)eZigbeeAppCmds.SetDateTime:
                             {
                                 cmdResp.CommandName =
                                     cmdResp.GetCommandName(eZigbeeAppCmds.SetDateTime);

                                 if (usDataSize >= SIZE_OF_TIME_RESP)
                                 {
                                     DateTime dtTemp = ManipulateDateTime(ref DataReader);
                                     cmdResp.Data = dtTemp.ToString() + " (GMT)";

                                     usDataSize -= SIZE_OF_TIME_RESP;
                                 }
                                 else
                                 {
                                     // Something is wrong - the data size is not
                                     // what we expected
                                     cmdResp.Success = false;
                                     cmdResp.Data = "Error reading the command data";

                                     usDataSize = 0;
                                 }
                                 objClientMeter.CommandResponses.Add(cmdResp);
                                 break;
                             }
                             case (byte)eZigbeeAppCmds.SetCollectionCfg:
                             {
                                 cmdResp.CommandName =
                                         cmdResp.GetCommandName(eZigbeeAppCmds.SetCollectionCfg);

                                 if (usDataSize >= SIZE_OF_CMD_RESP)
                                 {
                                     byte byDFTHour = DataReader.ReadByte();

                                     // Mask off bits 0 - 4, 0xthe DFT hour
                                     byDFTHour = (byte)(byDFTHour & 0x1F);

                                     if (0x1F != byDFTHour)
                                     {
                                         DateTime dtTime = new DateTime(DateTime.Now.Year,
                                                                         DateTime.Now.Month,
                                                                         DateTime.Now.Day,
                                                                         (int)byDFTHour,
                                                                         0,
                                                                         0);

                                         cmdResp.Data = "DFT: " + dtTime.ToShortTimeString() + " (GMT)";

                                     }
                                     else
                                     {
                                         // A value of 0x1F means no DFT is captured
                                         cmdResp.Data = "No DFT is captured";
                                     }

                                     usDataSize -= SIZE_OF_CMD_RESP;
                                 }
                                 else
                                 {
                                     // Something is wrong - the data size is not
                                     // what we expected
                                     cmdResp.Success = false;
                                     cmdResp.Data = "Error reading the command data";

                                     usDataSize = 0;
                                 }
                                 objClientMeter.CommandResponses.Add(cmdResp);
                                 break;
                             }
                             case (byte)eZigbeeAppCmds.SetDataDeliveryCfg:
                             {
                                 cmdResp.CommandName =
                                         cmdResp.GetCommandName(eZigbeeAppCmds.SetDataDeliveryCfg);

                                 if (usDataSize >= SIZE_OF_CMD_RESP)
                                 {
                                     byte byWakeupHour = DataReader.ReadByte();

                                     // Mask off bit 5, which sets wake up to every 12 or 24 hrs
                                     byte byFrequency = (byte)(byWakeupHour & 0x20);

                                     // Mask off bits 0 - 4, the hour for wake up and delivery
                                     byWakeupHour = (byte)(byWakeupHour & 0x1F);

                                     DateTime dtTime = new DateTime(DateTime.Now.Year,
                                                                     DateTime.Now.Month,
                                                                     DateTime.Now.Day,
                                                                     (int)byWakeupHour,
                                                                     0,
                                                                     0);

                                     cmdResp.Data = "Wake up: " + dtTime.ToShortTimeString() + " (GMT)";

                                     string strTemp;

                                     if (0 == byFrequency)
                                     {
                                         strTemp = " Every 24 hrs";
                                     }
                                     else
                                     {
                                         strTemp = " Every 12 hrs";
                                     }
                                     cmdResp.Data += strTemp;

                                     usDataSize -= SIZE_OF_CMD_RESP;
                                 }
                                 else
                                 {
                                     // Something is wrong - the data size is not
                                     // what we expected
                                     cmdResp.Success = false;
                                     cmdResp.Data = "Error reading the command data";

                                     usDataSize = 0;
                                 }
                                 objClientMeter.CommandResponses.Add(cmdResp);
                                 break;
                             }
                             case (byte)eZigbeeAppCmds.GetDateTime:
                             {
                                 cmdResp.CommandName =
                                         cmdResp.GetCommandName(eZigbeeAppCmds.GetDateTime);

                                 if (usDataSize >= SIZE_OF_TIME_RESP)
                                 {
                                     DateTime dtTemp = ManipulateDateTime(ref DataReader);
                                     cmdResp.Data = dtTemp.ToString();

                                     usDataSize -= SIZE_OF_TIME_RESP;
                                 }
                                 else
                                 {
                                     // Something is wrong - the data size is not
                                     // what we expected
                                     cmdResp.Success = false;
                                     cmdResp.Data = "Error reading the command data";
                                     usDataSize = 0;
                                 }
                                 objClientMeter.CommandResponses.Add(cmdResp);
                                 break;
                             }
                             case (byte)eZigbeeAppCmds.SetTimeoutDuration:
                             {
                                 cmdResp.CommandName =
                                         cmdResp.GetCommandName(eZigbeeAppCmds.SetTimeoutDuration);

                                 if (usDataSize >= SIZE_OF_CMD_RESP)
                                 {
                                     byte bytData = DataReader.ReadByte();
                                     // Timeout duration in seconds
                                     cmdResp.Data = bytData.ToString(CultureInfo.InvariantCulture);


                                     usDataSize -= SIZE_OF_CMD_RESP;
                                 }
                                 else
                                 {
                                     // Something is wrong - the data size is not
                                     // what we expected
                                     cmdResp.Success = false;
                                     cmdResp.Data = "Error reading the command data";

                                     usDataSize = 0;
                                 }
                                 objClientMeter.CommandResponses.Add(cmdResp);
                                 break;
                             }
                             case (byte)eZigbeeAppCmds.GetTimeoutDuration:
                             {
                                 cmdResp.CommandName =
                                         cmdResp.GetCommandName(eZigbeeAppCmds.GetTimeoutDuration);

                                 if (usDataSize >= SIZE_OF_CMD_RESP)
                                 {
                                     byte bytData = DataReader.ReadByte();
                                     // Timeout duration in seconds
                                     cmdResp.Data = bytData.ToString(CultureInfo.InvariantCulture);

                                     usDataSize -= SIZE_OF_CMD_RESP;
                                 }
                                 else
                                 {
                                     // Something is wrong - the data size is not
                                     // what we expected
                                     cmdResp.Data = "Error reading the command data";
                                     usDataSize = 0;
                                 }
                                 objClientMeter.CommandResponses.Add(cmdResp);
                                 break;
                             }
                             case (byte)eZigbeeAppCmds.ForceDecommission:
                             {
                                 cmdResp.CommandName =
                                         cmdResp.GetCommandName(eZigbeeAppCmds.ForceDecommission);

                                 if (usDataSize >= SIZE_OF_DECOMM_RESP)
                                 {
                                     // No data associated with this command
                                     cmdResp.Data = "";

                                     usDataSize -= SIZE_OF_DECOMM_RESP;
                                 }
                                 else
                                 {
                                     // Something is wrong - the data size is not
                                     // what we expected
                                     cmdResp.Success = false;
                                     cmdResp.Data = "Error reading the command data";

                                     usDataSize = 0;
                                 }
                                 objClientMeter.CommandResponses.Add(cmdResp);
                                 break;
                             }
                             case (byte)eZigbeeAppCmds.SetInitConsumption:
                             {
                                 cmdResp.CommandName =
                                     cmdResp.GetCommandName(eZigbeeAppCmds.SetInitConsumption);

                                 if (usDataSize >= SIZE_OF_INIT_CONSUMPTION_RESP)
                                 {
                                     ulngValue = InterpretBCDValue(DataReader.ReadBytes(5));
                                     cmdResp.Data = ulngValue.ToString(CultureInfo.InvariantCulture);
                                     objClientMeter.CommandResponses.Add(cmdResp);

                                     usDataSize -= SIZE_OF_INIT_CONSUMPTION_RESP;
                                 }
                                 else
                                 {
                                     // Something is wrong - the data size is not
                                     // what we expected
                                     cmdResp.Success = false;
                                     cmdResp.Data = "Error reading the command data";

                                     usDataSize = 0;
                                 }
                                 break;
                             }
                             case (byte)eZigbeeAppCmds.GetMeterDataLatest:
                             {
                                 cmdResp.CommandName =
                                     cmdResp.GetCommandName(eZigbeeAppCmds.GetMeterDataLatest);

                                 if (usDataSize >= SIZE_OF_LATEST_CONSUMPTION_DATA)
                                 {
                                     // Consumption Reading Normalization Value
                                     byReadingFormat = DataReader.ReadByte();

                                     // Tamper Info
                                     usTemp = DataReader.ReadUInt16();
                                     //objClientMeter.TamperIndicators = 

                                     // Latest Consumption
                                     ulngValue = InterpretBCDValue(DataReader.ReadBytes(5));
                                     objClientMeter.LatestConsumption = new ClientMeterReading();

                                     // Adjust the decimal point using the Consumption
                                     // Reading Normalization Value
                                     objClientMeter.LatestConsumption.Value =
                                         (double)ulngValue / Math.Pow(10.0, (byReadingFormat & 0x0F));

                                     objClientMeter.LatestConsumption.Timestamp =
                                         ManipulateDateTime(ref DataReader);

                                     objClientMeter.LatestConsumption.Units = TranslateScaleFactor((byte)(byReadingFormat & 0x0F));

                                     // DFT Hour
                                     bytTemp = DataReader.ReadByte();

                                     DateTime tempDate = objClientMeter.LatestConsumption.Timestamp;
                                     DateTime DFTDate = new DateTime(tempDate.Year,
                                         tempDate.Month,
                                         tempDate.Day,
                                         bytTemp,
                                         0,
                                         0);

                                     if (DFTDate > tempDate)
                                     {
                                         DFTDate = DFTDate.AddDays(-1);
                                     }

                                     objClientMeter.DFTConsumption = new ClientMeterReading();
                                     objClientMeter.DFTConsumption.Timestamp = DFTDate;

                                     // DFT consumption reading
                                     ulngValue = InterpretBCDValue(DataReader.ReadBytes(5));
                                     // Adjust the decimal point using the Consumption Reading Normalization Value 
                                     objClientMeter.DFTConsumption.Value =
                                             (double)ulngValue / Math.Pow(10.0, (byReadingFormat & 0x0F));

                                     objClientMeter.DFTConsumption.Units = TranslateScaleFactor((byte)(byReadingFormat & 0x0F));

                                     objClientMeter.CustomerID = DataReader.ReadUInt32();

                                     cmdResp.Data = m_rmStrings.GetString("GW_CONSUMPTION_DATA_RETRIEVED");

                                     usDataSize -= SIZE_OF_LATEST_CONSUMPTION_DATA;
                                 }
                                 else
                                 {
                                     // Something is wrong - the data size is not
                                     // what we expected
                                     cmdResp.Success = false;
                                     cmdResp.Data = "Error reading the command data";

                                     usDataSize = 0;
                                 }
                                 objClientMeter.CommandResponses.Add(cmdResp);
                                 break;
                             }
                             case (byte)eZigbeeAppCmds.GetSEMeterDataLatest:
                             case (byte)eZigbeeAppCmds.GetSEMeterDataHourly:
                             case (byte)eZigbeeAppCmds.GetSEMeterDataDaily:
                             {
                                 cmdResp.CommandName = cmdResp.GetCommandName((eZigbeeAppCmds)byCmdId);
                                 bool bHasMoreIntervalData = false;

                                 if (usDataSize >= SIZE_OF_SE_LATEST_CONSUMPTION_DATA)
                                 {
                                     // We might need to interpret this value as either BCD or Binary so lets read it both ways
                                     byte[] SummationBytes = DataReader.ReadBytes(6);
                                     DataReader.BaseStream.Seek(-6, SeekOrigin.Current);
                                     ulong ulSummationValue = DataReader.ReadUInt48();

                                     // We might need to interpret this value as either BCD or Binary so lets read it both ways
                                     byte[] DFTSummationBytes = DataReader.ReadBytes(6);
                                     DataReader.BaseStream.Seek(-6, SeekOrigin.Current);
                                     ulong ulDFTSummationValue = DataReader.ReadUInt48();

                                     byte byDailyFreezeTimeHours = DataReader.ReadByte();
                                     byte byDailyFreezeTimeMinutes = DataReader.ReadByte();
                                     uint uiReadingSnapshotTime = DataReader.ReadUInt32();
                                     byte byMeterStatus = DataReader.ReadByte();
                                     byte byUnitOfMeasure = DataReader.ReadByte();
                                     uint uiMultiplier = DataReader.ReadUInt24();
                                     uint uiDivisor = DataReader.ReadUInt24();
                                     byte bySummationFormatting = DataReader.ReadByte();
                                     //Summation Formatting (From ZigBee Smart Energy Protocol)
                                     //  Bits 0 to 2: Number of Digits to the right of the Decimal Point.
                                     //  Bits 3 to 6: Number of Digits to the left of the Decimal Point.
                                     //  Bit 7: If set, suppress leading zeros.
                                     double dblFormattingDivisor = Math.Pow(10.0, (bySummationFormatting & 0x07));
                                     byte byDeviceType = DataReader.ReadByte();
                                     byte byMaxPeriodsDelivered = DataReader.ReadByte();
                                     ushort usTamper = DataReader.ReadUInt16();
                                     uint uiUtilityData = DataReader.ReadUInt32();

                                     objClientMeter.LatestConsumption = new ClientMeterReading();

                                     if (byUnitOfMeasure < (byte)SimpleMeteringUnitOfMeasure.kWBCD)
                                     {
                                         objClientMeter.LatestConsumption.Value = ulSummationValue * (double)uiMultiplier / (double)uiDivisor;
                                     }
                                     else
                                     {
                                         objClientMeter.LatestConsumption.Value = InterpretBCDValue(SummationBytes);
                                     }

                                     objClientMeter.LatestConsumption.Value /= dblFormattingDivisor;

                                     objClientMeter.LatestConsumption.Timestamp = SE_REFERNCE_TIME.AddSeconds(uiReadingSnapshotTime);
                                     objClientMeter.LatestConsumption.Units = EnumDescriptionRetriever.RetrieveDescription((SimpleMeteringUnitOfMeasure)byUnitOfMeasure);

                                     objClientMeter.DFTConsumption = new ClientMeterReading();

                                     if (byUnitOfMeasure < (byte)SimpleMeteringUnitOfMeasure.kWBCD)
                                     {
                                         objClientMeter.DFTConsumption.Value = ulDFTSummationValue * (double)uiMultiplier / (double)uiDivisor;
                                     }
                                     else
                                     {
                                         objClientMeter.DFTConsumption.Value = InterpretBCDValue(DFTSummationBytes);
                                     }

                                     objClientMeter.DFTConsumption.Value /= dblFormattingDivisor;

                                     objClientMeter.DFTConsumption.Timestamp = objClientMeter.LatestConsumption.Timestamp.Date.AddHours(byDailyFreezeTimeHours).AddMinutes(byDailyFreezeTimeMinutes);

                                     if (objClientMeter.DFTConsumption.Timestamp > objClientMeter.LatestConsumption.Timestamp)
                                     {
                                         objClientMeter.DFTConsumption.Timestamp = objClientMeter.DFTConsumption.Timestamp.AddDays(-1);
                                     }

                                     objClientMeter.DFTConsumption.Units = EnumDescriptionRetriever.RetrieveDescription((SimpleMeteringUnitOfMeasure)byUnitOfMeasure);

                                     objClientMeter.CustomerID = uiUtilityData;

                                     cmdResp.Data = m_rmStrings.GetString("GW_CONSUMPTION_DATA_RETRIEVED");

                                     usDataSize -= SIZE_OF_SE_LATEST_CONSUMPTION_DATA;

                                     // At this point we should start seeing Interval Data responses so we should peek to make sure we have additional commands
                                     do
                                     {
                                         // Make sure that there is more data that can be read
                                         if (usDataSize > 0)
                                         {
                                             eZigbeeAppCmds CurrentCommand = (eZigbeeAppCmds)DataReader.ReadByte();

                                             if (CurrentCommand == eZigbeeAppCmds.GetSEMeterDataHourly || CurrentCommand == eZigbeeAppCmds.GetSEMeterDataDaily)
                                             {
                                                 bHasMoreIntervalData = true;

                                                 // Parse out the interval data
                                                 bytTemp = DataReader.ReadByte(); // Packet Version
                                                 DateTime IntervalTime = SE_REFERNCE_TIME.AddSeconds(DataReader.ReadUInt32());
                                                 bytTemp = DataReader.ReadByte(); // SE Status Response
                                                 SmartEnergyLoadProfilePeriod ProfilePeriod = (SmartEnergyLoadProfilePeriod)DataReader.ReadByte(); // SE Interval Period
                                                 TimeSpan IntervalLength = SmartEnergyLoadProfile.GetPeriodDuration(ProfilePeriod);
                                                 byte byNumberOfPeriods = DataReader.ReadByte();

                                                 usDataSize -= 9;

                                                 for (int iInterval = 0; iInterval < byNumberOfPeriods; iInterval++)
                                                 {
                                                     uint uiIntervalValue = DataReader.ReadUInt24();
                                                     usDataSize -= 3;

                                                     ClientMeterReading NewReading = new ClientMeterReading();

                                                     NewReading.Timestamp = IntervalTime;
                                                     NewReading.Units = objClientMeter.LatestConsumption.Units;

                                                     if (uiIntervalValue < 0xFFFFF0)
                                                     {
                                                         NewReading.Value = (double)uiIntervalValue / dblFormattingDivisor;
                                                     }
                                                     else
                                                     {
                                                         NewReading.Value = 0.0;
                                                         NewReading.Status = TranslateIntervalReadingSpecialValue(uiIntervalValue);
                                                     }

                                                     if (CurrentCommand == eZigbeeAppCmds.GetSEMeterDataHourly)
                                                     {
                                                         if (cmdResp.HourlyReadings == null)
                                                         {
                                                             cmdResp.HourlyReadings = new List<ClientMeterReading>();
                                                         }

                                                         cmdResp.HourlyReadings.Add(NewReading);
                                                     }
                                                     else
                                                     {
                                                         if (cmdResp.DailyReadings == null)
                                                         {
                                                             cmdResp.DailyReadings = new List<ClientMeterReading>();
                                                         }

                                                         cmdResp.DailyReadings.Add(NewReading);
                                                     }

                                                     IntervalTime -= IntervalLength;
                                                 }
                                                 
                                             }
                                             else
                                             {
                                                 bHasMoreIntervalData = false;
                                                 // We read one more byte than we should have so we should go back
                                                 DataReader.BaseStream.Seek(-1, SeekOrigin.Current);
                                             }
                                         }
                                         else
                                         {
                                             bHasMoreIntervalData = false;
                                         }
                                     } while (bHasMoreIntervalData == true);
                                 }
                                 else
                                 {
                                     // Something is wrong - the data size is not
                                     // what we expected
                                     cmdResp.Success = false;
                                     cmdResp.Data = "Error reading the command data";

                                     usDataSize = 0;
                                 }

                                 objClientMeter.CommandResponses.Add(cmdResp);

                                 break;
                             }
                             case (byte)eZigbeeAppCmds.GetMeterDataHourly:
                             {
                                 cmdResp.CommandName = cmdResp.GetCommandName((eZigbeeAppCmds)byCmdId);

                                 if (usDataSize >= SIZE_OF_HOURLY_CONSUMPTION_DATA)
                                 {
                                     // Consumption Reading Normalization Value
                                     byReadingFormat = DataReader.ReadByte();

                                     // Tamper Info
                                     usTemp = DataReader.ReadUInt16();

                                     // Latest Consumption
                                     ulngValue = InterpretBCDValue(DataReader.ReadBytes(5));
                                     objClientMeter.LatestConsumption = new ClientMeterReading();

                                     // Adjust the decimal point using the Consumption
                                     // Reading Normalization Value
                                     objClientMeter.LatestConsumption.Value = (double)ulngValue / Math.Pow(10.0, (byReadingFormat & 0x0F));

                                     objClientMeter.LatestConsumption.Timestamp = ManipulateDateTime(ref DataReader);

                                     objClientMeter.LatestConsumption.Units = TranslateScaleFactor((byte)(byReadingFormat & 0x0F));

                                     // DFT Hour
                                     bytTemp = DataReader.ReadByte();

                                     DateTime tempDate = objClientMeter.LatestConsumption.Timestamp;
                                     DateTime DFTDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, bytTemp, 0, 0);

                                     if (DFTDate > tempDate)
                                     {
                                         DFTDate = DFTDate.AddDays(-1);
                                     }

                                     objClientMeter.DFTConsumption = new ClientMeterReading();
                                     objClientMeter.DFTConsumption.Timestamp = DFTDate;

                                     // DFT consumption reading
                                     ulngValue = InterpretBCDValue(DataReader.ReadBytes(5));
                                     // Adjust the decimal point using the Consumption Reading Normalization Value 
                                     objClientMeter.DFTConsumption.Value = (double)ulngValue / Math.Pow(10.0, (byReadingFormat & 0x0F));

                                     objClientMeter.DFTConsumption.Units = TranslateScaleFactor((byte)(byReadingFormat & 0x0F));

                                     objClientMeter.CustomerID = DataReader.ReadUInt32();

                                     cmdResp.Data = m_rmStrings.GetString("GW_CONSUMPTION_DATA_RETRIEVED");

                                     // Get the interval length
                                     bytTemp = DataReader.ReadByte();

                                     TimeSpan IntervalLength;

                                     if (bytTemp == 0)
                                     {
                                         IntervalLength = new TimeSpan(0, 15, 0);
                                     }
                                     else if (bytTemp == 1)
                                     {
                                         IntervalLength = new TimeSpan(0, 30, 0);
                                     }
                                     else
                                     {
                                         IntervalLength = new TimeSpan(1, 0, 0);
                                     }

                                     // Read the current sequence number
                                     bytTemp = DataReader.ReadByte();
                                     DateTime IntervalTime = objClientMeter.LatestConsumption.Timestamp;
                                     IntervalTime = new DateTime(IntervalTime.Year, IntervalTime.Month, IntervalTime.Day, IntervalTime.Hour, 0, 0, DateTimeKind.Utc);

                                     // If we have an interval length greater than 1 hour we need to figure out the time closest to the last interval
                                     while (IntervalTime + IntervalLength <= objClientMeter.LatestConsumption.Timestamp)
                                     {
                                         IntervalTime += IntervalLength;
                                     }

                                     cmdResp.HourlyReadings = new List<ClientMeterReading>();

                                     // First packet has 6 interval values;
                                     for (int iInterval = 0; iInterval < 6; iInterval++)
                                     {
                                         uint IntervalValue = DataReader.ReadUInt24();

                                         ClientMeterReading NewReading = new ClientMeterReading();

                                         NewReading.Timestamp = IntervalTime;
                                         NewReading.Units = objClientMeter.DFTConsumption.Units;

                                         // Values greater than 0xFFFFF0 are error conditions
                                         if (IntervalValue < 0xFFFFF0)
                                         {
                                             NewReading.Value = (double)IntervalValue / Math.Pow(10.0, (byReadingFormat & 0x0F));
                                         }
                                         else
                                         {
                                             NewReading.Value = 0.0;
                                             NewReading.Status = TranslateIntervalReadingSpecialValue(IntervalValue);
                                         }

                                         cmdResp.HourlyReadings.Add(NewReading);

                                         IntervalTime -= IntervalLength;
                                     }

                                     // Between each packet we have the Cmd ID, packet version, and Sequence Number again
                                     bytTemp = DataReader.ReadByte(); // Cmd ID
                                     bytTemp = DataReader.ReadByte(); // Packet Version
                                     bytTemp = DataReader.ReadByte(); // Interval Length
                                     bytTemp = DataReader.ReadByte(); // Sequence Number

                                     // Second packet has 13 intervals
                                     for (int iInterval = 0; iInterval < 13; iInterval++)
                                     {
                                         uint IntervalValue = DataReader.ReadUInt24();

                                         ClientMeterReading NewReading = new ClientMeterReading();

                                         NewReading.Timestamp = IntervalTime;
                                         NewReading.Units = objClientMeter.DFTConsumption.Units;

                                         // Values greater than 0xFFFFF0 are error conditions
                                         if (IntervalValue < 0xFFFFF0)
                                         {
                                             NewReading.Value = (double)IntervalValue / Math.Pow(10.0, (byReadingFormat & 0x0F));
                                         }
                                         else
                                         {
                                             NewReading.Value = 0.0;
                                             NewReading.Status = TranslateIntervalReadingSpecialValue(IntervalValue);
                                         }

                                         cmdResp.HourlyReadings.Add(NewReading);

                                         IntervalTime -= IntervalLength;
                                     }

                                     // Between each packet we have the Cmd ID, packet version, and Sequence Number again
                                     bytTemp = DataReader.ReadByte(); // Cmd ID
                                     bytTemp = DataReader.ReadByte(); // Packet Version
                                     bytTemp = DataReader.ReadByte(); // Interval Length
                                     bytTemp = DataReader.ReadByte(); // Sequence Number

                                     // The Third Packet has 13 Intervals
                                     for (int iInterval = 0; iInterval < 13; iInterval++)
                                     {
                                         uint IntervalValue = DataReader.ReadUInt24();

                                         ClientMeterReading NewReading = new ClientMeterReading();

                                         NewReading.Timestamp = IntervalTime;
                                         NewReading.Units = objClientMeter.DFTConsumption.Units;

                                         // Values greater than 0xFFFFF0 are error conditions
                                         if (IntervalValue < 0xFFFFF0)
                                         {
                                             NewReading.Value = (double)IntervalValue / Math.Pow(10.0, (byReadingFormat & 0x0F));
                                         }
                                         else
                                         {
                                             NewReading.Value = 0.0;
                                             NewReading.Status = TranslateIntervalReadingSpecialValue(IntervalValue);
                                         }

                                         cmdResp.HourlyReadings.Add(NewReading);

                                         IntervalTime -= IntervalLength;
                                     }

                                     usDataSize -= SIZE_OF_HOURLY_CONSUMPTION_DATA;
                                 }
                                 else
                                 {
                                     // Something is wrong - the data size is not
                                     // what we expected
                                     cmdResp.Success = false;
                                     cmdResp.Data = "Error reading the command data";

                                     usDataSize = 0;
                                 }

                                 objClientMeter.CommandResponses.Add(cmdResp);

                                 break;
                             }
                             case (byte)eZigbeeAppCmds.GetMeterDataDaily:
                             {
                                 cmdResp.CommandName = cmdResp.GetCommandName((eZigbeeAppCmds)byCmdId);

                                 if (usDataSize >= SIZE_OF_HOURLY_CONSUMPTION_DATA)
                                 {
                                     // Consumption Reading Normalization Value
                                     byReadingFormat = DataReader.ReadByte();

                                     // Tamper Info
                                     usTemp = DataReader.ReadUInt16();

                                     // Latest Consumption
                                     ulngValue = InterpretBCDValue(DataReader.ReadBytes(5));
                                     objClientMeter.LatestConsumption = new ClientMeterReading();

                                     // Adjust the decimal point using the Consumption
                                     // Reading Normalization Value
                                     objClientMeter.LatestConsumption.Value = (double)ulngValue / Math.Pow(10.0, (byReadingFormat & 0x0F));

                                     objClientMeter.LatestConsumption.Timestamp = ManipulateDateTime(ref DataReader);

                                     objClientMeter.LatestConsumption.Units = TranslateScaleFactor((byte)(byReadingFormat & 0x0F));

                                     // DFT Hour
                                     bytTemp = DataReader.ReadByte();

                                     DateTime tempDate = objClientMeter.LatestConsumption.Timestamp;
                                     DateTime DFTDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, bytTemp, 0, 0);

                                     if (DFTDate > tempDate)
                                     {
                                         DFTDate = DFTDate.AddDays(-1);
                                     }

                                     objClientMeter.DFTConsumption = new ClientMeterReading();
                                     objClientMeter.DFTConsumption.Timestamp = DFTDate;

                                     // DFT consumption reading
                                     ulngValue = InterpretBCDValue(DataReader.ReadBytes(5));
                                     // Adjust the decimal point using the Consumption Reading Normalization Value 
                                     objClientMeter.DFTConsumption.Value = (double)ulngValue / Math.Pow(10.0, (byReadingFormat & 0x0F));

                                     objClientMeter.DFTConsumption.Units = TranslateScaleFactor((byte)(byReadingFormat & 0x0F));

                                     objClientMeter.CustomerID = DataReader.ReadUInt32();

                                     cmdResp.Data = m_rmStrings.GetString("GW_CONSUMPTION_DATA_RETRIEVED");

                                     // Get the interval length
                                     bytTemp = DataReader.ReadByte();

                                     TimeSpan IntervalLength = new TimeSpan(24, 0, 0);

                                     // Read the current sequence number
                                     bytTemp = DataReader.ReadByte();
                                     DateTime IntervalTime = objClientMeter.DFTConsumption.Timestamp;
                                     cmdResp.DailyReadings = new List<ClientMeterReading>();

                                     // First packet has 6 interval values;
                                     for (int iInterval = 0; iInterval < 6; iInterval++)
                                     {
                                         uint IntervalValue = DataReader.ReadUInt24();

                                         ClientMeterReading NewReading = new ClientMeterReading();

                                         NewReading.Timestamp = IntervalTime;
                                         NewReading.Units = objClientMeter.DFTConsumption.Units;

                                         // Values greater than 0xFFFFF0 are error conditions
                                         if (IntervalValue < 0xFFFFF0)
                                         {
                                             NewReading.Value = (double)IntervalValue / Math.Pow(10.0, (byReadingFormat & 0x0F));
                                         }
                                         else
                                         {
                                             NewReading.Value = 0.0;
                                             NewReading.Status = TranslateIntervalReadingSpecialValue(IntervalValue);
                                         }

                                         cmdResp.DailyReadings.Add(NewReading);

                                         IntervalTime -= IntervalLength;
                                     }

                                     // Between each packet we have the Cmd ID, packet version, and Sequence Number again
                                     bytTemp = DataReader.ReadByte(); // Cmd ID
                                     bytTemp = DataReader.ReadByte(); // Packet Version
                                     bytTemp = DataReader.ReadByte(); // Interval Length
                                     bytTemp = DataReader.ReadByte(); // Sequence Number

                                     // Second packet has 13 intervals
                                     for (int iInterval = 0; iInterval < 13; iInterval++)
                                     {
                                         uint IntervalValue = DataReader.ReadUInt24();

                                         ClientMeterReading NewReading = new ClientMeterReading();

                                         NewReading.Timestamp = IntervalTime;
                                         NewReading.Units = objClientMeter.DFTConsumption.Units;

                                         // Values greater than 0xFFFFF0 are error conditions
                                         if (IntervalValue < 0xFFFFF0)
                                         {
                                             NewReading.Value = (double)IntervalValue / Math.Pow(10.0, (byReadingFormat & 0x0F));
                                         }
                                         else
                                         {
                                             NewReading.Value = 0.0;
                                             NewReading.Status = TranslateIntervalReadingSpecialValue(IntervalValue);
                                         }

                                         cmdResp.DailyReadings.Add(NewReading);

                                         IntervalTime -= IntervalLength;
                                     }

                                     // Between each packet we have the Cmd ID, packet version, and Sequence Number again
                                     bytTemp = DataReader.ReadByte(); // Cmd ID
                                     bytTemp = DataReader.ReadByte(); // Packet Version
                                     bytTemp = DataReader.ReadByte(); // Interval Length
                                     bytTemp = DataReader.ReadByte(); // Sequence Number

                                     // The Third Packet has 13 Intervals
                                     for (int iInterval = 0; iInterval < 13; iInterval++)
                                     {
                                         uint IntervalValue = DataReader.ReadUInt24();

                                         ClientMeterReading NewReading = new ClientMeterReading();

                                         NewReading.Timestamp = IntervalTime;
                                         NewReading.Units = objClientMeter.DFTConsumption.Units;

                                         // Values greater than 0xFFFFF0 are error conditions
                                         if (IntervalValue < 0xFFFFF0)
                                         {
                                             NewReading.Value = (double)IntervalValue / Math.Pow(10.0, (byReadingFormat & 0x0F));
                                         }
                                         else
                                         {
                                             NewReading.Value = 0.0;
                                             NewReading.Status = TranslateIntervalReadingSpecialValue(IntervalValue);
                                         }

                                         cmdResp.DailyReadings.Add(NewReading);

                                         IntervalTime -= IntervalLength;
                                     }

                                     // Between each packet we have the Cmd ID, packet version, and Sequence Number again
                                     bytTemp = DataReader.ReadByte(); // Cmd ID
                                     bytTemp = DataReader.ReadByte(); // Packet Version
                                     bytTemp = DataReader.ReadByte(); // Interval Length
                                     bytTemp = DataReader.ReadByte(); // Sequence Number

                                     // The Fourth Packet has 9 Intervals
                                     for (int iInterval = 0; iInterval < 9; iInterval++)
                                     {
                                         uint IntervalValue = DataReader.ReadUInt24();

                                         ClientMeterReading NewReading = new ClientMeterReading();

                                         NewReading.Timestamp = IntervalTime;
                                         NewReading.Units = objClientMeter.DFTConsumption.Units;

                                         // Values greater than 0xFFFFF0 are error conditions
                                         if (IntervalValue < 0xFFFFF0)
                                         {
                                             NewReading.Value = (double)IntervalValue / Math.Pow(10.0, (byReadingFormat & 0x0F));
                                         }
                                         else
                                         {
                                             NewReading.Value = 0.0;
                                             NewReading.Status = TranslateIntervalReadingSpecialValue(IntervalValue);
                                         }

                                         cmdResp.DailyReadings.Add(NewReading);

                                         IntervalTime -= IntervalLength;
                                     }


                                     usDataSize -= SIZE_OF_HOURLY_CONSUMPTION_DATA;
                                 }
                                 else
                                 {
                                     // Something is wrong - the data size is not
                                     // what we expected
                                     cmdResp.Success = false;
                                     cmdResp.Data = "Error reading the command data";

                                     usDataSize = 0;
                                 }

                                 objClientMeter.CommandResponses.Add(cmdResp);

                                 break;
                             }
                             case (byte)eZigbeeAppCmds.GetGasRangeExtenderData:
                             {
                                 cmdResp.CommandName =
                                     cmdResp.GetCommandName(eZigbeeAppCmds.GetGasRangeExtenderData);

                                 if (usDataSize >= SIZE_OF_GAS_RANGE_EXTENDER_DATA)
                                 {
                                     cmdResp.Data = m_rmStrings.GetString("GW_GAS_RANGE_EXTENDER_DATA");

                                     ClientRangeExtenderRcd extender = new ClientRangeExtenderRcd();
                                     extender.AlarmFlags = (UInt16)DataReader.ReadUInt16();
                                     extender.TotalOnTime = (UInt32)DataReader.ReadUInt32();
                                     extender.NetworkAccessTamper = (UInt16)DataReader.ReadUInt16();
                                     extender.ChildCount = (byte)DataReader.ReadByte();
                                     ClientRangeExtenderChildRcd[] children = null;

                                     usDataSize -= SIZE_OF_GAS_RANGE_EXTENDER_DATA;

                                     if (usDataSize >= (SIZE_OF_GAS_RANGE_EXTENDER_CHILDREN * extender.ChildCount))
                                     {
                                         for (int ChildIndex = 0; ChildIndex < extender.ChildCount; ChildIndex++)
                                         {
                                             children[ChildIndex] = new ClientRangeExtenderChildRcd();
                                             children[ChildIndex].MACAddress = (UInt32)DataReader.ReadUInt32();
                                             children[ChildIndex].RSSI = (sbyte)DataReader.ReadSByte();
                                         }
                                     }

                                     extender.Children = children;
                                     cmdResp.ClientRangeExtenderRecord = extender;
                                     usDataSize -= (ushort)(SIZE_OF_GAS_RANGE_EXTENDER_CHILDREN * extender.ChildCount);
                                 }
                                 else
                                 {
                                     // Something is wrong - the data size is not
                                     // what we expected
                                     cmdResp.Success = false;
                                     cmdResp.Data = "Error reading the command data";

                                     usDataSize = 0;
                                 }
                                 objClientMeter.CommandResponses.Add(cmdResp);
                                 break;
                             }
                             case (byte)eZigbeeAppCmds.NAK:
                             {
                                 if (usDataSize >= SIZE_OF_ERROR_RESPONSE)
                                 {
                                     //Cmd being rejected
                                     bytTemp = DataReader.ReadByte();
                                     cmdResp.CommandName = cmdResp.GetCommandName((eZigbeeAppCmds)bytTemp);
                                     cmdResp.Success = false;
                                     cmdResp.Data = TranslateErrorCode(DataReader.ReadByte());

                                     usDataSize -= SIZE_OF_ERROR_RESPONSE;
                                 }
                                 else
                                 {
                                     // Something is wrong - the data size is not
                                     // what we expected
                                     cmdResp.Success = false;
                                     cmdResp.Data = "Error reading the command data";

                                     usDataSize = 0;
                                 }
                                 objClientMeter.CommandResponses.Add(cmdResp);
                                 break;
                             }
                             default:
                             {
                                 // We have a command response but we don't know it's size.
                                 // Best to just give up on the rest of the data.
                                 usDataSize = 0;
                                 break;
                             }
                         }

                     }   // End while (0 < usDataSize) 

                     lstClientMeter.Add(objClientMeter);

                 }   // end if data size > 0
             }   // end for each client

             return lstClientMeter;
         }

         #endregion

         #region Public Properties

         /// <summary>
         /// MAC address of the client this block of data belongs to.  The MAC
         /// address is an 8-byte identifier assigned at the factory which uniquely 
         /// identifies the client
         /// </summary>
         public UInt64 MACAddress
         {
             get
             {
                 return m_ulMACAddr;
             }
             set
             {
                 m_ulMACAddr = value;
             }
         }

         /// <summary>
         /// Translates the node type enum into a string
         /// </summary>
         public string NodeType
         {
             get
             {
                 return m_strNodeType;
             }
             set
             {
                 m_strNodeType = value;
             }
         }

         /// <summary>
         /// 4-byte customer id
         /// </summary>
         public UInt32 CustomerID
         {
             get
             {
                 return m_uiCustomerID;
             }
             set
             {
                 m_uiCustomerID = value;
             }
         }

         /// <summary>
         /// The time at which the electric meter recorded the client meter readings
         /// </summary>
         public DateTime TimeRecorded
         {
             get
             {
                 return m_dtTimeRecorded;
             }
             set
             {
                 m_dtTimeRecorded = value;
             }
         }

         /// <summary>
         /// Value and timestamp for the latest consumption reading
         /// </summary>
         public ClientMeterReading LatestConsumption
         {
             get
             {
                 return m_LatestConsumption;
             }
             set
             {
                 m_LatestConsumption = value;
             }
         }

         /// <summary>
         /// Value and timestamp for the reading that occurred at the daily
         /// freeze time.
         /// </summary>
         public ClientMeterReading DFTConsumption
         {
             get
             {
                 return m_DFTConsumption;
             }
             set
             {
                 m_DFTConsumption = value;
             }
         }

         /// <summary>
         /// 
         /// </summary>
         public List<String> TamperIndicators;

         /// <summary>
         /// List of command responses.  The list could be empty
         /// </summary>
         public List<ClientCommandResponse> CommandResponses
         {
             get
             {
                 return m_lstCmdResponses;
             }
             set
             {
                 m_lstCmdResponses = value;
             }
         }

         #endregion

         #region Private Methods

         /// <summary>
         /// Translates special IPP Interval Values to a readable status
         /// </summary>
         /// <param name="value">The value to translate</param>
         /// <returns>The readable status</returns>
         //  Revision History	
         //  MM/DD/YY Who Version Issue#  Description
         //  -------- --- ------- ------- -------------------------------------------
         //  01/21/12 RCG 2.53.42 TRQ2900 Created
         
         private static string TranslateIntervalReadingSpecialValue(uint value)
         {
             string strStatus;

             switch (value)
             {
                 case 0xFFFFF0:
                 {
                     strStatus = "Overflow";
                     break;
                 }
                 case 0xFFFFF1:
                 {
                     strStatus = "Underflow";
                     break;
                 }
                 case 0xFFFFF4:
                 {
                     strStatus = "Time Error";
                     break;
                 }
                 case 0xFFFFFF:
                 {
                     strStatus = "No Data Available";
                     break;
                 }
                 default:
                 {
                     strStatus = "Invalid Value";
                     break;
                 }
             }

             return strStatus;
         }

         /// <summary>
         /// Extracts the client node type from the MAC address passed in and
         /// translates the type into a string
         /// </summary>
         /// <param name="ulMACAddr"></param>
         /// <returns></returns>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  11/21/06 AF  8.00.00        Created
         //
         private static string DetermineNodeType(UInt64 ulMACAddr)
         {
             byte byNodeType = (byte)((ulMACAddr & 0xFF00000000) >> 32);

             switch (byNodeType)
             {
                 case (byte)eNodeType.GasMeter:
                     {
                         return "Gas Meter";
                     }
                 case (byte)eNodeType.WaterMeter:
                     {
                         return "Water Meter";
                     }
                 case (byte)eNodeType.ComvergeGateway:
                     {
                         return "ComvergeGateway";
                     }
                 case (byte)eNodeType.GasRangeExtender:
                     {
                         return "Gas Range Extender";
                     }
                 default:
                     {
                         return "Unknown Node Type";
                     }
             }
         }

         /// <summary>
         /// Translates the seconds since 1970 into a datetime value
         /// </summary>
         /// <param name="BinReader">
         /// reference to the binary reader that has access to the data
         /// </param>
         /// <returns>the datetime value calculated from the input</returns>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  11/21/06 AF  8.00.00        Created
         //
         private static DateTime ManipulateDateTime(ref PSEMBinaryReader BinReader)
         {
             UInt32 uiSecondsSince1970 = 0;
             DateTime dtTemp;

             uiSecondsSince1970 = BinReader.ReadUInt32();
             dtTemp = new DateTime(1970, 1, 1);
             return dtTemp.AddSeconds(uiSecondsSince1970);
         }

         /// <summary>
         /// Translates the array of bytes representing a BCD value into an 
         /// unsigned long
         /// </summary>
         /// <param name="abyBCDValue">BCD value contained in an array of bytes</param>
         /// <returns>unsigned long value of the BCD quantity</returns>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  11/21/06 AF  8.00.00        Created
         //
         private static UInt64 InterpretBCDValue(byte[] abyBCDValue)
         {
             UInt64 ulngValue = 0;

             //Reverse the byte order to get the original data
             byte[] abyTemp = new byte[abyBCDValue.Length];

             for (int iCounter = 0; iCounter < abyBCDValue.Length; iCounter++)
             {
                 abyTemp[iCounter] = abyBCDValue[abyBCDValue.Length - iCounter - 1];
             }

             for (int iIndex = 0; iIndex < abyBCDValue.Length; iIndex++)
             {
                 ulngValue = (ulngValue * 100) +
                            (byte)((abyTemp[iIndex] & 0x0F) + ((abyTemp[iIndex] >> 4) * 10));
             }
             return ulngValue;
         }

         /// <summary>
         /// Translates a Zigbee command error code into a string description
         /// </summary>
         /// <param name="bytErrorCode">Zigbee command error code</param>
         /// <returns>Error description</returns>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  11/21/06 AF  8.00.00        Created
         //
         private static string TranslateErrorCode(byte bytErrorCode)
         {
             string strError = "";

             switch (bytErrorCode)
             {
                 case (byte)CommandErrorResult.UNKNOWN_COMMAND:
                     {
                         strError = m_rmStrings.GetString("GW_ERR_UNKNOWN_CMD");
                         break;
                     }
                 case (byte)CommandErrorResult.INVALID_PARAM_SIZE_OR_COUNT:
                     {
                         strError = m_rmStrings.GetString("GW_ERR_INVALID_PARAM_SIZE_OR_CNT");
                         break;
                     }
                 case (byte)CommandErrorResult.INVALID_PARAM_VALUE:
                     {
                         strError = m_rmStrings.GetString("GW_ERR_INVALID_PARAM_VAL");
                         break;
                     }
                 case (byte)CommandErrorResult.INVALID_TIMESTAMP:
                     {
                         strError = m_rmStrings.GetString("GW_ERR_INVALID_TIMESTAMP");
                         break;
                     }
                 case (byte)CommandErrorResult.INVALID_SECURITY_KEY_CHANGE_SEQ:
                     {
                         strError = m_rmStrings.GetString("GW_ERR_INVALID_SECURITY_KEY_CHANGE_SEQ");
                         break;
                     }
                 case (byte)CommandErrorResult.INVALID_PACKET_VERSION:
                     {
                         strError = m_rmStrings.GetString("GW_ERR_INVALID_PKT_VER");
                         break;
                     }
                 case (byte)CommandErrorResult.UNKNOWN_ERROR:
                     {
                         strError = m_rmStrings.GetString("GW_ERR_UNKNOWN_ERROR");
                         break;
                     }
                 default:
                     {
                         strError = m_rmStrings.GetString("GW_ERR_UNKNOWN_ERROR");
                         break;
                     }
             }
             return strError;
         }

         /// <summary>
         /// The gas consumption reading payload has a field called Consumption
         /// Normalization Value, the low nibble of which gives us the digits
         /// to the right of the decimal point and is a scaling factor
         /// </summary>
         /// <param name="bytScale">the lower nibble of the Consumption Normalization Value</param>
         /// <returns></returns>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  11/25/09 AF  2.30.22 145687 Created
         //
         private static string TranslateScaleFactor(byte bytScale)
         {
             string strUnits = "";

             if (2 == bytScale)
             {
                 strUnits = "CCF";
             }
             else if (3 == bytScale)
             {
                 strUnits = "MCF";
             }
             else if (1 == bytScale)
             {
                 strUnits = "10CF/10000CF";
             }
             else
             {
                 strUnits = "Gas Quantity";
             }

             return strUnits;
         }

         #endregion

         #region Members

         private UInt64 m_ulMACAddr;
         private DateTime m_dtTimeRecorded;
         private string m_strNodeType;
         //private UInt16 m_usTamperInfo;
         private ClientMeterReading m_LatestConsumption;
         private ClientMeterReading m_DFTConsumption;
         private UInt32 m_uiCustomerID;
         private List<ClientCommandResponse> m_lstCmdResponses;
         internal static System.Resources.ResourceManager m_rmStrings;
         internal static readonly string RESOURCE_FILE_PROJECT_STRINGS =
                                     "Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings";


         #endregion
     }

     /// <summary>
     /// Class which encapsulates a single water or gas reading as a value and
     /// timestamp pair.
     /// </summary>
     //  Revision History	
     //  MM/DD/YY Who Version Issue# Description
     //  -------- --- ------- ------ -------------------------------------------
     //  11/21/06 AF  8.00.00        Created
     //
     public class ClientMeterReading
     {
         #region Public Properties

         /// <summary>
         /// Value of the reading
         /// </summary>
         public double Value
         {
             get
             {
                 return m_dblValue;
             }
             set
             {
                 m_dblValue = value;
             }
         }

         /// <summary>
         /// Time at which the reading occurred
         /// </summary>
         public DateTime Timestamp
         {
             get
             {
                 return m_dtTimestamp;
             }
             set
             {
                 m_dtTimestamp = value;
             }
         }

         /// <summary>
         /// The units to use when displaying the reading
         /// </summary>
         public string Units
         {
             get
             {
                 return m_strUnits;
             }
             set
             {
                 m_strUnits = value;
             }
         }

         /// <summary>
         /// Gets the status for the reading
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue#  Description
         //  -------- --- ------- ------- -------------------------------------------
         //  01/21/12 RCG 2.53.42 TRQ2900 Created

         public string Status
         {
             get
             {
                 return m_strStatus;
             }
             set
             {
                 m_strStatus = value;
             }
         }

         #endregion

         #region Members

         private double m_dblValue;
         private DateTime m_dtTimestamp;
         private string m_strUnits;
         private string m_strStatus;

         #endregion
     }

     /// <summary>
     /// Class that represents the information available in a command response from 
     /// a client meter
     /// </summary>
     //  Revision History	
     //  MM/DD/YY Who Version Issue# Description
     //  -------- --- ------- ------ -------------------------------------------
     //  11/21/06 AF  8.00.00        Created
     //
     public class ClientCommandResponse
     {
         #region Constants
         #endregion

         #region Definitions
         #endregion

         #region Public Methods

         /// <summary>
         /// Constructor
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  11/21/06 AF  8.00.00        Created
         //
         public ClientCommandResponse()
         {
             m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                               this.GetType().Assembly);
         }

         /// <summary>
         /// Takes a command id enum as input and returns the description of the
         /// command
         /// </summary>
         /// <param name="eCmd">eZigbeeAppCmds</param>
         /// <returns>command description</returns>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  11/21/06 AF  8.00.00        Created
         //
         public string GetCommandName(eZigbeeAppCmds eCmd)
         {
             string strName = "";

             switch ((byte)eCmd)
             {
                 case (byte)eZigbeeAppCmds.SetDateTime:
                     {
                         strName = m_rmStrings.GetString("GW_SET_REMOTE_DATE_TIME");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.SetCollectionCfg:
                     {
                         strName = m_rmStrings.GetString("GW_SET_COLLECTION_CFG");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.SetDataDeliveryCfg:
                     {
                         strName = m_rmStrings.GetString("GW_SET_DATA_DELIVERY_CFG");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.SetTimeoutDuration:
                     {
                         strName = m_rmStrings.GetString("GW_SET_TIMEOUT_DURATION");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.GetMeterDataLatest:
                 case (byte)eZigbeeAppCmds.GetSEMeterDataLatest:
                     {
                         strName = m_rmStrings.GetString("GW_GET_METER_DATA_LATEST");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.GetMeterDataHourly:
                 case (byte)eZigbeeAppCmds.GetSEMeterDataHourly:
                     {
                         strName = m_rmStrings.GetString("GW_GET_METER_DATA_HOURLY");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.GetMeterDataDaily:
                 case (byte)eZigbeeAppCmds.GetSEMeterDataDaily:
                     {
                         strName = m_rmStrings.GetString("GW_GET_METER_DATA_DAILY");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.GetDateTime:
                     {
                         strName = m_rmStrings.GetString("GW_GET_DATE_TIME");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.GetCollectionCfg:
                     {
                         strName = m_rmStrings.GetString("GW_GET_COLLECTION_CFG");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.GetDataDeliveryCfg:
                     {
                         strName = m_rmStrings.GetString("GW_GET_DATA_DELIVERY_CFG");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.GetTimeoutDuration:
                     {
                         strName = m_rmStrings.GetString("GW_GET_TIMEOUT_DURATION");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.ForceDecommission:
                     {
                         strName = m_rmStrings.GetString("GW_FORCE_DECOMMISSION");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.SetCfgBlock1:
                     {
                         strName = m_rmStrings.GetString("GW_SET_CFG_BLOCK1");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.GetCfgBlock1:
                     {
                         strName = m_rmStrings.GetString("GW_GET_CFG_BLOCK1");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.SetHighAppSecKey:
                     {
                         strName = m_rmStrings.GetString("GW_SET_HIGH_APP_SECURITY_KEY");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.SetLowAppSecKey:
                     {
                         strName = m_rmStrings.GetString("GW_SET_LOW_APP_SECURITY_KEY");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.SetHighNetSecKey:
                     {
                         strName = m_rmStrings.GetString("GW_SET_HIGH_NET_SECURITY_KEY");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.SetLowNetSecKey:
                     {
                         strName = m_rmStrings.GetString("GW_SET_LOW_NET_SECURITY_KEY");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.SetHandlerCfg:
                     {
                         strName = m_rmStrings.GetString("GW_SET_HANDLER_CFG");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.GetHandlerCfg:
                     {
                         strName = m_rmStrings.GetString("GW_GET_HANDLER_CFG");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.SetRFRetry:
                     {
                         strName = m_rmStrings.GetString("GW_SET_RF_RETRY_CFG");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.GetRFRetry:
                     {
                         strName = m_rmStrings.GetString("GW_GET_RF_RETRY_CFG");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.KeepAlive:
                     {
                         strName = m_rmStrings.GetString("GW_KEEP_ALIVE");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.NewAppKeyActivate:
                     {
                         strName = m_rmStrings.GetString("GW_NEW_APP_KEY_ACTIVE");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.SetInitConsumption:
                     {
                         strName = m_rmStrings.GetString("GW_SET_INITIAL_CONSUMPTION");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.GetGasRangeExtenderData:
                     {
                         strName = m_rmStrings.GetString("GET_GAS_RANGE_EXTENDER_DATA");
                         break;
                     }
                 case (byte)eZigbeeAppCmds.NAK:
                     {
                         strName = m_rmStrings.GetString("GW_ERROR_RESPONSE");
                         break;
                     }
                 default:
                     {
                         strName = m_rmStrings.GetString("UNKNOWN_CMD");
                         break;
                     }

             }
             return strName;
         }

         #endregion

         #region Public Properties

         /// <summary>
         /// Description of the command
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  11/21/06 AF  8.00.00        Created
         //
         public string CommandName
         {
             get
             {
                 return m_strCmdName;
             }
             set
             {
                 m_strCmdName = value;
             }
         }

         /// <summary>
         /// Boolean which tells whether or not the command succeeded
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  11/21/06 AF  8.00.00        Created
         //
         public bool Success
         {
             get
             {
                 return m_blnSuccess;
             }
             set
             {
                 m_blnSuccess = value;
             }
         }

         /// <summary>
         /// Summary of the data
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  11/21/06 AF  8.00.00        Created
         //
         public string Data
         {
             get
             {
                 return m_strData;
             }
             set
             {
                 m_strData = value;
             }
         }

         /// <summary>
         /// List of hourly readings.  This list could be empty.
         /// </summary>
         public List<ClientMeterReading> HourlyReadings
         {
             get
             {
                 //TO DO-- transform deltas into actual values
                 return m_lstHourlyDeltas;
             }
             set
             {
                 m_lstHourlyDeltas = value;
             }
         }

         /// <summary>
         /// List of daily readings.  This list could be empty.
         /// </summary>
         public List<ClientMeterReading> DailyReadings
         {
             get
             {
                 //To DO -- transform deltas into actual values
                 return m_lstDailyDeltas;
             }
             set
             {
                 m_lstDailyDeltas = value;
             }
         }

         /// <summary>
         /// Client Range Extender Record
         /// </summary>
         public ClientRangeExtenderRcd ClientRangeExtenderRecord
         {
             get
             {
                 return m_ClientRangeExtenderRcd;
             }
             set
             {
                 m_ClientRangeExtenderRcd = value;
             }
         }

         #endregion

         #region Members

         private string m_strCmdName;
         private bool m_blnSuccess;
         private string m_strData;
         private System.Resources.ResourceManager m_rmStrings;
         private static readonly string RESOURCE_FILE_PROJECT_STRINGS =
             "Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings";
         private List<ClientMeterReading> m_lstHourlyDeltas;
         private List<ClientMeterReading> m_lstDailyDeltas;
         private ClientRangeExtenderRcd m_ClientRangeExtenderRcd;

         #endregion

     }

     /// <summary>
     /// Class that represents a single Client Range Extender (24ZR) record
     /// </summary>
     //  Revision History	
     //  MM/DD/YY Who Version Issue# Description
     //  -------- --- ------- ------ -------------------------------------------
     //  09/25/12 PGH  2.70.28       Created
     //
     public class ClientRangeExtenderRcd
     {
         #region Definitions

         /// <summary>
         /// Gas Range Extender Alarm Flags
         /// </summary>
         [Flags]
         public enum Alarms : ushort
         {
             /// <summary>
             /// None selected
             /// </summary>
             None = 0x0000,
             /// <summary>
             /// Battery Alarm
             /// </summary>
             BatteryAlarm = 0x0001,
         }

         #endregion

         #region Public Methods

         /// <summary>
         /// Constructor
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  09/25/12 PGH 2.70.28        Created
         //
         public ClientRangeExtenderRcd()
         {

         }

         #endregion

         #region Public Properties

         /// <summary>
         /// Has Battery Alarm
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  10/09/12 PGH 2.70.30          Created.
         //
         public bool HasBatteryAlarm
         {
             get
             {
                 return ((Alarms)m_usAlarmFlags & Alarms.BatteryAlarm) == Alarms.BatteryAlarm;
             }
         }

         /// <summary>
         /// Alarm Flags
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  09/25/12 PGH 2.70.28        Created
         //
         public UInt16 AlarmFlags
         {
             get
             {
                 return m_usAlarmFlags;
             }
             set
             {
                 m_usAlarmFlags = value;
             }
         }

         /// <summary>
         /// Total on time
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  09/25/12 PGH 2.70.28        Created
         //
         public UInt32 TotalOnTime
         {
             get
             {
                 return m_ulTotalOnTime;
             }
             set
             {
                 m_ulTotalOnTime = value;
             }
         }

         /// <summary>
         /// Network Access Tamper
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  09/25/12 PGH 2.70.28        Created
         //
         public UInt16 NetworkAccessTamper
         {
             get
             {
                 return m_usNetworkAccessTamper;
             }
             set
             {
                 m_usNetworkAccessTamper = value;
             }
         }

         /// <summary>
         /// Child count
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  09/25/12 PGH 2.70.28        Created
         //
         public byte ChildCount
         {
             get
             {
                 return m_bytChildCount;
             }
             set
             {
                 m_bytChildCount = value;
             }
         }

         /// <summary>
         /// Children
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  09/25/12 PGH 2.70.28        Created
         //
         public ClientRangeExtenderChildRcd[] Children
         {
             get
             {
                 return m_RangeExtenderChildren;
             }
             set
             {
                 m_RangeExtenderChildren = value;
             }
         }


         #endregion

         #region Members

         private UInt16 m_usAlarmFlags;
         private UInt32 m_ulTotalOnTime;
         private UInt16 m_usNetworkAccessTamper;
         private byte m_bytChildCount;
         private ClientRangeExtenderChildRcd[] m_RangeExtenderChildren;

         #endregion
     }

     /// <summary>
     /// Class that represents a single Client Range Extender (24ZR) Child record
     /// </summary>
     //  Revision History	
     //  MM/DD/YY Who Version Issue# Description
     //  -------- --- ------- ------ -------------------------------------------
     //  09/25/12 PGH  2.70.28       Created
     //
     public class ClientRangeExtenderChildRcd
     {
         #region Public Methods

         /// <summary>
         /// Constructor
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  09/25/12 PGH 2.70.28        Created
         //
         public ClientRangeExtenderChildRcd()
         {

         }

         #endregion

         #region Public Properties

         /// <summary>
         /// MAC Address
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  09/25/12 PGH 2.70.28        Created
         //
         public UInt32 MACAddress
         {
             get
             {
                 return m_ulMACAddress;
             }
             set
             {
                 m_ulMACAddress = value;
             }
         }

         /// <summary>
         /// RSSI
         /// </summary>
         //  Revision History	
         //  MM/DD/YY Who Version Issue# Description
         //  -------- --- ------- ------ -------------------------------------------
         //  09/25/12 PGH 2.70.28        Created
         //
         public sbyte RSSI
         {
             get
             {
                 return m_sbytRSSI;
             }
             set
             {
                 m_sbytRSSI = value;
             }
         }

         #endregion

         #region Members

         private UInt32 m_ulMACAddress;
         private sbyte m_sbytRSSI;

         #endregion
     }
}
