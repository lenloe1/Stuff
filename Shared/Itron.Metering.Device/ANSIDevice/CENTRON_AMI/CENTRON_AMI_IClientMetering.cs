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
//                              Copyright © 2006 - 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Device;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class representing the CENTRON_AMI, IClientMetering interface
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 11/14/06 AF 8.00.00  N/A    Created
    // 
    public partial class CENTRON_AMI :  IClientMetering
    {
        #region Constants

        private const int SIZE_OF_CLIENT_DATA_REGION = 128;
        private const int SIZE_OF_INTERVAL_DATA = 93;
        private const int SIZE_OF_LATEST_CONSUMPTION_DATA = 24;
        private const int SIZE_OF_SECURITY_RESPONSE = 10;
        private const int SIZE_OF_TIME_RESP = 6;
        private const int SIZE_OF_ERROR_RESPONSE = 4;
        private const int SIZE_OF_CMD_RESP = 3;
        private const int SIZE_OF_DECOMM_RESP = 2;
        private const UInt64 UNASSIGNED_MAC_ADDR = 0xFFFFFFFFFFFFFFFF;
        private const int SIZE_OF_CMD_PARAM_ARRAY = 8;
        private const int SIZE_OF_INIT_CONSUMPTION_RESP = 8;

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes a GW_Set_Remote_Date_Time command to table 2100 for the 
        /// specified Zigbee client
        /// </summary>
        /// <param name="ulClientAddr">
        /// MAC address of the client to which to send the command
        /// </param>
        /// <returns>ClientMeterCmdResult</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/22/06 AF  8.00.00        Created
        //
        public virtual ClientMeterCmdResult SetClientMeterTime(UInt64 ulClientAddr)
        {
            ClientMeterCmdResult Result = ClientMeterCmdResult.UNSUPPORTED_COMMAND;
            PSEMResponse PSEMResult = new PSEMResponse();
            byte[] abyCmdParams = new byte[SIZE_OF_CMD_PARAM_ARRAY];
            int iIndex;
            

            PSEMResult = PSEMResponse.Err;
            
            // Start with a read of table 2100 to see what's already there.
            // We don't want to overwrite valid commands
            List<ClientCfgRcd> lstRcds = Table2100.HANConfigData;

            // Build the command parameter array.  For this command it really 
            // doesn't matter.  The electric meter will overwrite the field
            // when the command is sent to the Zigbee client
            for (iIndex = 0; iIndex < SIZE_OF_CMD_PARAM_ARRAY; iIndex++)
            {
                abyCmdParams[iIndex] = 0;
            }

            // Insert the command data into the existing contents of table 2100
            // if there is room for it
            bool blnFoundCmdSpace = ConstructTable2100(ulClientAddr, 
                                                       abyCmdParams, 
                                                       eZigbeeAppCmds.SetDateTime, 
                                                       ref lstRcds);

            if (false != blnFoundCmdSpace)
            {
                //Write the data back to the meter
                byte[] abyTable2100Data = ConvertDataToByteArray(lstRcds);
                PSEMResult = m_PSEM.FullWrite(2100, abyTable2100Data);

                Result = TranslatePSEMResponse(PSEMResult);
            }
            else
            {
                Result = ClientMeterCmdResult.TOO_MANY_COMMANDS;
            }
            
            return Result;
        }

        /// <summary>
        /// Configures how the Gas/Water meter is to collect its meter data
        /// </summary>
        /// <param name="ulClientAddr">
        /// MAC address of the client to which to send the command
        /// </param>
        /// <param name="byDFTHour">
        /// Daily Freeze Hour - the hour at which the daily consumption readings are read
        /// </param>
        /// <param name="byReadingType">
        /// 0x00 => Current Consumption and DFT readings, 
        /// 0x01 => Hourly interval data, 
        /// 0x02 => Daily Interval data
        /// </param>
        /// <returns>ClientMeterCmdResult</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/11/06 AF  8.00.00        Created
        //  06/30/08 AF  1.51.01        Removed the BCD parameter.  BCD format is 
        //                              assumed and the corresponding bit in the
        //                              command parameter is now reserved
        //
        public virtual ClientMeterCmdResult SetCollectionConfig(UInt64 ulClientAddr,
                                                 byte byDFTHour,
                                                 byte byReadingType)
        {
            ClientMeterCmdResult Result = ClientMeterCmdResult.UNSUPPORTED_COMMAND;
            PSEMResponse PSEMResult = new PSEMResponse();
            byte[] abyCmdParams = new byte[SIZE_OF_CMD_PARAM_ARRAY];
            int iIndex = 0;

            PSEMResult = PSEMResponse.Err;

            //Construct the command parameter
            byte byCommandParameter = (byte)((byReadingType << 6) | byDFTHour);

            // Start with a read of table 2100 to see what's already there.
            // We don't want to overwrite valid commands
            List<ClientCfgRcd> lstRcds = Table2100.HANConfigData;

            // Build the command parameter array.  For this command, the first
            // byte contains the configuration info
            abyCmdParams[0] = byCommandParameter;

            //Fill the rest of the command data fields with FF
            for (iIndex = 1; iIndex < SIZE_OF_CMD_PARAM_ARRAY; iIndex++)
            {
                abyCmdParams[iIndex] = 0xFF;
            }

            bool blnFoundCmdSpace = ConstructTable2100(ulClientAddr, abyCmdParams,
                                                       eZigbeeAppCmds.SetCollectionCfg,
                                                       ref lstRcds);

            if (false != blnFoundCmdSpace)
            {
                //Write the data into a byte array
                byte[] abyTable2100Data = ConvertDataToByteArray(lstRcds);
                PSEMResult = m_PSEM.FullWrite(2100, abyTable2100Data);

                Result = TranslatePSEMResponse(PSEMResult);
            }
            else
            {
                Result = ClientMeterCmdResult.TOO_MANY_COMMANDS;
            }

            return Result;
        }

        /// <summary>
        /// This command configures the schedule by which the Gas/Water meter
        /// will wake up and send its meter data to the electric meter
        /// </summary>
        /// <param name="ulClientAddr">
        /// MAC address of the client to which to send the command
        /// </param>
        /// <param name="byWakeUpHour">
        /// Hour for wake up and data delivery: midnight is 0, 11 pm is 23
        /// </param>
        /// <param name="byFrequency">
        /// Whether the client should wake up once or twice a day. 1 => every 12 hours;
        /// 0 => every 24 hours.
        /// </param>
        /// <returns>ClientMeterCmdResult</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/05/06 AF  8.00.00        Created
        //
        public virtual ClientMeterCmdResult SetDataDeliveryConfig(UInt64 ulClientAddr, 
                                                                   byte byWakeUpHour, 
                                                                   byte byFrequency)
        {
            ClientMeterCmdResult Result = ClientMeterCmdResult.UNSUPPORTED_COMMAND;
            PSEMResponse PSEMResult = new PSEMResponse();
            byte[] abyCmdParams = new byte[SIZE_OF_CMD_PARAM_ARRAY];
            int iIndex = 0;

            PSEMResult = PSEMResponse.Err;

            //Construct the command parameter
            byte byCommandParameter = (byte)((byFrequency << 5) | byWakeUpHour);
            
            // Start with a read of table 2100 to see what's already there.
            // We don't want to overwrite valid commands
            List<ClientCfgRcd> lstRcds = Table2100.HANConfigData;

            // Build the command parameter array.  For this command, the first
            // byte contains the configuration info
            abyCmdParams[0] = byCommandParameter;

            //Fill the rest of the command data fields with FF
            for (iIndex = 1; iIndex < SIZE_OF_CMD_PARAM_ARRAY; iIndex++)
            {
                abyCmdParams[iIndex] = 0xFF;
            }

            bool blnFoundCmdSpace = ConstructTable2100(ulClientAddr, abyCmdParams,
                                                       eZigbeeAppCmds.SetDataDeliveryCfg, 
                                                       ref lstRcds);

            if (false != blnFoundCmdSpace)
            {
                //Write the data into a byte array
                byte[] abyTable2100Data = ConvertDataToByteArray(lstRcds);
                PSEMResult = m_PSEM.FullWrite(2100, abyTable2100Data);

                Result = TranslatePSEMResponse(PSEMResult);
            }
            else
            {
                Result = ClientMeterCmdResult.TOO_MANY_COMMANDS;
            }

            return Result;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Client data retrieved from mfg table 2101
        /// </summary>
        /// <returns>
        /// list of readings along with tamper information and the customer id
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/06 AF  8.00.00        Created
        //  12/20/06 AF  8.00.03        Fixed bugs in the interpretation of
        //                              eZigbeeAppCmds.SetCollectionCfg and
        //                              eZigbeeAppCmds.SetDataDeliveryCfg
        //  04/27/07 AF  8.10.02 2751   Test data might not have the size we expect.
        //                              Check on data size before plunging ahead.
        //  04/27/07 AF  8.10.02 2753   Gas module keeps GMT time.  Labeled those
        //                              timestamps to alert the user.
        //  05/20/08 jrf 1.50.27 114449 Moved the processing of the client data into 
        //                              a list of client meters into a static method
        //                              so that it might also be used when extracting
        //                              this data from a data file.
        //
        public virtual List<ClientMeter> ClientMeters
        {
            get
            {
                return ClientMeter.ProcessClientData(Table2101.HANClientDataList);
            }
        }

        /// <summary>
        /// Essentially a dump of manufacturer's table 2100, this property will
        /// give users a way to view the queued up client meter commands.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/02/07 AF  8.00.10        Created
        //
        public virtual List<ClientCfgRcd> ClientConfigCommands
        {
            get
            {
                List<ClientCfgRcd> lstClientConfigCmds = new List<ClientCfgRcd>();
                lstClientConfigCmds = Table2100.HANConfigData;

                return lstClientConfigCmds;
            }
        }

        #endregion
        
        #region Private Methods

        /// <summary>
        /// Takes the data from table 2100 in the form of a list of records and
        /// transforms it into a byte array
        /// object
        /// </summary>
        /// <param name="lstRcds">
        /// Contents of table 2100 in the form of a list of records
        /// </param>
        /// <returns>a byte array containing the contents of table 2100</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/27/06 AF  8.00.00        Created
        //
        private byte[] ConvertDataToByteArray(List<ClientCfgRcd> lstRcds)
        {
            int iTableIndex = 0;
            byte[] abyTable2100Data = new byte[Table2100.TableLength];

            for (int iIndex = 0; iIndex < lstRcds.Count; iIndex++)
            {
                byte[] abyData = BitConverter.GetBytes(lstRcds[iIndex].ClientAddress);
                Array.Copy(abyData, 0, abyTable2100Data, iTableIndex, 8);
                iTableIndex += 8;
                abyTable2100Data[iTableIndex] = lstRcds[iIndex].NumberCfgCmds;
                iTableIndex++;
                for (int iInnerIndex = 0; iInnerIndex < lstRcds[iIndex].ClientCmdList.Count; iInnerIndex++)
                {
                    abyTable2100Data[iTableIndex] = lstRcds[iIndex].ClientCmdList[iInnerIndex].CmdID;
                    iTableIndex++;
                    abyTable2100Data[iTableIndex] = lstRcds[iIndex].ClientCmdList[iInnerIndex].PacketVer;
                    iTableIndex++;
                    abyData = BitConverter.GetBytes(lstRcds[iIndex].ClientCmdList[iInnerIndex].SequenceNum);
                    Array.Copy(abyData, 0, abyTable2100Data, iTableIndex, 2);
                    iTableIndex += 2;
                    abyData = lstRcds[iIndex].ClientCmdList[iInnerIndex].CmdData;
                    Array.Copy(abyData, 0, abyTable2100Data, iTableIndex, SIZE_OF_CMD_PARAM_ARRAY);
                    iTableIndex += SIZE_OF_CMD_PARAM_ARRAY;
                }
            }
            return abyTable2100Data;
        }

        /// <summary>
        /// Translates a PSEMResponse to a ClientMeterCmdResult response
        /// </summary>
        /// <param name="PSEMResult">the PSEMResponse to be translated</param>
        /// <returns>a ClientMeterCmdResult response code</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/01/06 AF  8.00.00        Created
        //
        private ClientMeterCmdResult TranslatePSEMResponse(PSEMResponse PSEMResult)
        {
            ClientMeterCmdResult Result = ClientMeterCmdResult.UNSUPPORTED_COMMAND;

            switch (PSEMResult)
            {
                case PSEMResponse.Ok:
                {
                    Result = ClientMeterCmdResult.SUCCESS;
                    break;
                }
                case PSEMResponse.Sns:
                case PSEMResponse.Onp:
                case PSEMResponse.Iar:
                {
                    Result = ClientMeterCmdResult.UNSUPPORTED_COMMAND;
                    break;
                }
                default:
                {
                    Result = ClientMeterCmdResult.ERROR;
                    break;
                }
            }
            return Result;
        }

        /// <summary>
        /// Takes the current table 2100 data as input and rewrites the portion
        /// that will hold the command to be sent to the client module
        /// </summary>
        /// <param name="ulClientAddr">
        /// MAC address of the client to which to send the command
        /// </param>
        /// <param name="abytCommandParams">
        /// byte array containing the command parameters for this command
        /// </param>
        /// <param name="eCmd">
        /// the command id of the command to send to the HAN (Zigbee) client
        /// </param>
        /// <param name="lstRcds">
        /// List containing the contents of table 2100.  Passed by ref so that it
        /// can be modified.
        /// </param>
        /// <returns>
        /// True if there is space in the table for the command.  False, otherwise.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/06 AF  8.00.00        Created
        //  12/16/13 DLG 3.50.15        Updated to use HANInformation object to access table 2099.
        //
        private bool ConstructTable2100(UInt64 ulClientAddr, byte[] abytCommandParams,
                                        eZigbeeAppCmds eCmd, ref List<ClientCfgRcd> lstRcds)
        {
            bool blnFoundSpace = false;

            for (int iIndex = 0; iIndex < lstRcds.Count; iIndex++)
            {
                if (lstRcds[iIndex].ClientAddress != UNASSIGNED_MAC_ADDR)
                {
                    // This section is in use.  Is it for this client?
                    if (lstRcds[iIndex].ClientAddress == ulClientAddr)
                    {
                        //We've found our client.  Is there room for another command?
                        int intNumCmds = lstRcds[iIndex].NumberCfgCmds;

                        if (intNumCmds < m_HANInfo.Table2099.NumberHANConfigCmds)
                        {
                            //Use the next available slot
                            lstRcds[iIndex].ClientCmdList[intNumCmds].CmdID = (byte)eCmd;
                            lstRcds[iIndex].ClientCmdList[intNumCmds].PacketVer = 0;
                            lstRcds[iIndex].ClientCmdList[intNumCmds].SequenceNum = 0;
                            lstRcds[iIndex].ClientCmdList[intNumCmds].CmdData = abytCommandParams;

                            lstRcds[iIndex].NumberCfgCmds += 1;
                            blnFoundSpace = true;
                            break;
                        }
                        else
                        {
                            //The table already has the max number of commands for this client
                            blnFoundSpace = false;
                            break;
                        }
                    }
                    else
                    {
                        // this is another client.  Skip forward to the next client area
                        continue;
                    }
                }
                else
                {
                    //We've found an unused client address, so use this space for our data
                    lstRcds[iIndex].NumberCfgCmds = 1;
                    lstRcds[iIndex].ClientAddress = ulClientAddr;
                    lstRcds[iIndex].ClientCmdList[0].CmdID = (byte)eCmd;
                    lstRcds[iIndex].ClientCmdList[0].PacketVer = 0;
                    lstRcds[iIndex].ClientCmdList[0].SequenceNum = 0;
                    lstRcds[iIndex].ClientCmdList[0].CmdData = abytCommandParams;

                    blnFoundSpace = true;
                    break;
                }
            }   // end for

            return blnFoundSpace;
        }

        #endregion
    }
}


