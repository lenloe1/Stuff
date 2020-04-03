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
using System.Resources;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Clas that translates the CENTRON AMI LAN Communication Event Log Events
    /// </summary>
    public class CENTRON_AMI_CommEventDictionary : Dictionary<int, string>
    {
        #region Constants
        private const byte RX_OR_TX_MASK = 0x01;
        private const byte SUCCESS_OR_FAIL_MASK = 0x02;
        #endregion

        /// <summary>Constructs a dictionary of Centron AMI specific LAN events</summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/13/08 KRC 1.50.23
        // 06/07/13 jrf 2.80.27 TQ8279 Adding in undefined comm events.
        // 12/10/14 jrf	4.00.91 551420 added missing event definition.
        public CENTRON_AMI_CommEventDictionary()
            : base()
        {
            m_rmStrings = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                            this.GetType().Assembly);

            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_COMM_MODULE_LINK_FAILED, m_rmStrings.GetString("COMM_MODULE_LINK_FAILED"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_COMM_MODULE_LINK_RESET, m_rmStrings.GetString("COMM_MODULE_LINK_RESET"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_COMM_MODULE_LINK_UP, m_rmStrings.GetString("COMM_MODULE_LINK_UP"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_CONFIGURED_SEND_FAILURE_LIMIT_EXCEEDED, m_rmStrings.GetString("CONFIGURED_SEND_FAILURE_LIMIT_EXCEEDED"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_DEREGISTERED, m_rmStrings.GetString("DEREGISTERED"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_DEREGISTRATION_ATTEMPT, m_rmStrings.GetString("DEREGISTRATION_ATTEMPT"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_DEREGISTRATION_RESULT, m_rmStrings.GetString("DEREGISTRATION_RESULT"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_LINK_FAILURE, m_rmStrings.GetString("LINK_FAILURE"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_LINK_METRIC, m_rmStrings.GetString("LINK_METRIC"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_LINK_SWITCH, m_rmStrings.GetString("LINK_SWITCH"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_LINK_UP, m_rmStrings.GetString("LINK_UP"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_PROCESS_RCVD_MSG_TIMING, m_rmStrings.GetString("PROCESS_RCVD_MSG_TIMING"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_RECEIVED_INVALID_MESSAGE, m_rmStrings.GetString("RECEIVED_INVALID_MESSAGE"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_RECEIVED_REQUEST, m_rmStrings.GetString("RECEIVED_REQUEST"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_REGISTERED, m_rmStrings.GetString("C1222_REGISTERED"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_REGISTRATION_ATTEMPT, m_rmStrings.GetString("REGISTRATION_ATTEMPT"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_REGISTRATION_RESULT, m_rmStrings.GetString("REGISTRATION_RESULT"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_EXCEPTION_FAILED, m_rmStrings.GetString("SEND_EXCEPTION_FAILED"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_EXCEPTION_SUCCESS, m_rmStrings.GetString("SEND_EXCEPTION_SUCCESS"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_MESSAGE_SUCCEEDED, m_rmStrings.GetString("SEND_MESSAGE_SUCCEEDED"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_PERIODIC_READ_FAILED, m_rmStrings.GetString("SEND_PERIODIC_READ_FAILED"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_PERIODIC_READ_SUCCESS, m_rmStrings.GetString("SEND_PERIODIC_READ_SUCCESS"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_RESPONSE_FAILED, m_rmStrings.GetString("SEND_RESPONSE_FAILED"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_RESPONSE_SUCCESS, m_rmStrings.GetString("SEND_RESPONSE_SUCCESS"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_SENT_SEGMENT_BYTES, m_rmStrings.GetString("SENT_SEGMENT_BYTES"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_RECEIVED_SEGMENT_BYTES, m_rmStrings.GetString("RECEIVED_SEGMENT_BYTES"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_SENT_MESSAGE_FAILED, m_rmStrings.GetString("SENT_MESSAGE_FAILED"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_SENT_RESPONSE, m_rmStrings.GetString("SENT_RESPONSE"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_RECEIVED_MESSAGE_FROM, m_rmStrings.GetString("RECEIVED_MESSAGE_FROM"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_SENT_MESSAGE_TO, m_rmStrings.GetString("SENT_MESSAGE_TO"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_PR_SEND_TABLE, m_rmStrings.GetString("PR_SEND_TABLE"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_PR_SEND_TABLE_FAILED, m_rmStrings.GetString("PR_SEND_TABLE_FAILED"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_RESET, m_rmStrings.GetString("C1222_RESET"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_SENT_SIMPLE_ERROR_RESPONSE, m_rmStrings.GetString("SENT_SIMPLE_ERROR_RESONSE"));

            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_RFLAN_CELL_CHANGE, m_rmStrings.GetString("RFLAN_CELL_CHANGE"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_LEVEL_CHANGE, m_rmStrings.GetString("LEVEL_CHANGE"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_BEST_FATHER_CHANGE, m_rmStrings.GetString("BEST_FATHER_CHANGE"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_SYNCH_FATHER_CHANGE, m_rmStrings.GetString("SYNCH_FATHER_CHANGE"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_RFLAN_COLD_START, m_rmStrings.GetString("RFLAN_COLD_START"));

            Add((int)CENTRON_AMI.LANEvents.COMEVENT_INCOMING_SEGMENT_DISCARDED_IN_KEEPALIVE, m_rmStrings.GetString("INCOMING_SEGMENT_DISCARDED_IN_KEEPALIVE"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_COMM_LOG_TEST, m_rmStrings.GetString("COMM_LOG_TEST"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_COMM_LOG_ONE_HR_MAX, m_rmStrings.GetString("COMM_LOG_ONE_HR_MAX"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_PERIODIC_READ_TIMING, m_rmStrings.GetString("C1222_PERIODIC_READ_TIMING"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_FAILED_ADD_EXCEPTION_ID, m_rmStrings.GetString("C1222_FAILED_ADD_EXCEPTION_ID"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_FAILED_ADD_EXCEPTION_DETAIL, m_rmStrings.GetString("C1222_FAILED_ADD_EXCEPTION_DETAIL"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_ADDED_EXCEPTION_ID, m_rmStrings.GetString("C1222_ADDED_EXCEPTION_ID"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_EVENT_CACHE_OVERFLOWED, m_rmStrings.GetString("EVENT_CACHE_OVERFLOWED"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_IDENT_REQUEST_FAILED, m_rmStrings.GetString("C1222_SEND_IDENT_REQUEST_FAILED"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_IDENT_REQUEST, m_rmStrings.GetString("C1222_SEND_IDENT_REQUEST"));
            Add((int)CENTRON_AMI.LANEvents.COMEVENT_C1222_FIRMWARE_BLOCK_CRC_ERRORS, m_rmStrings.GetString("C1222_FIRMWARE_BLOCK_CRC_ERRORS"));

            Add((int)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_DRLC_MSG, m_rmStrings.GetString("ZIGBEE_DRLC_MSG"));
            Add((int)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_KEY_EST_MSG, m_rmStrings.GetString("ZIGBEE_KEY_EST_MSG"));
            Add((int)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_MESSAGING_MSG, m_rmStrings.GetString("ZIGBEE_MESSAGING_MSG"));
            Add((int)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_MSG, m_rmStrings.GetString("ZIGBEE_MSG"));
            Add((int)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_PREPAY_MSG, m_rmStrings.GetString("ZIGBEE_PREPAY_MSG"));
            Add((int)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_PRICE_MSG, m_rmStrings.GetString("ZIGBEE_PRICE_MSG"));
            Add((int)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_SE_TUNNELING_MSG, m_rmStrings.GetString("ZIGBEE_SE_TUNNELING_MSG"));
            Add((int)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_SIMPLE_METERING_MSG, m_rmStrings.GetString("ZIGBEE_SIMPLE_METERING_MSG")); 
        }

        /// <summary>
        /// TranslateEventData - Takes the LAN Event Data and translates it to something human readable
        /// </summary>
        /// <param name="uiCommEventCode">The Event Code of the Comm Event the Data belongs to</param>
        /// <param name="ArgumentReader">The raw data we are translating</param>
        /// <returns>The Human Readable text</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/13/08 KRC 1.50.24		    Created
        //
        public virtual string TranslatedEventData(ushort uiCommEventCode, BinaryReader ArgumentReader)
        {
            string strData = "";

            switch (uiCommEventCode)
            {
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_LINK_SWITCH:
                    {
                        strData = "New Link ID: " + ArgumentReader.ReadUInt16().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_LINK_METRIC:
                    {
                        UInt16 gpd = ArgumentReader.ReadUInt16();
                        byte syncFatherRssi = ArgumentReader.ReadByte();
                        byte bestFatherRssi = ArgumentReader.ReadByte();
                        byte fatherCount = ArgumentReader.ReadByte();

                        strData = "gpd: " + gpd.ToString(CultureInfo.InvariantCulture) + "; Synch Father RSSI: " + 
                                    syncFatherRssi.ToString(CultureInfo.InvariantCulture) +
                                    "; Best Father RSSI: " + bestFatherRssi.ToString(CultureInfo.InvariantCulture) +
                                    "; Father Count: " + fatherCount.ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_PERIODIC_READ_FAILED:
                    {
                        byte byReason = ArgumentReader.ReadByte();
                        if (byReason == 1)
                        {
                            strData = "Table Request Too Big";
                        }
                        else if (byReason == 2)
                        {
                            strData = "Message is Invalid";
                        }
                        else if (byReason == 3)
                        {
                            strData = "Send Failed";
                        }
                        else if (byReason == 4)
                        {
                            strData = "Table 8 Response could not be added to EPSEM when starting report message";
                        }
                        else if (byReason == 5)
                        {
                            strData = "Could not set user information into the message when starting periodic upload message";
                        }
                        else if (byReason == 6)
                        {
                            strData = "EPSEM validation failed when checked when finishing the periodic upload message.";
                        }
                        else if (byReason == 7)
                        {
                            strData = "Could not locate app data in message at end of periodic upload generation";
                        }
                        else if (byReason == 8)
                        {
                            strData = "Position EPSEM to end failed when padding EPSEM";
                        }
                        else if (byReason == 9)
                        {
                            strData = "Could not force terminate EPSEM after add Table 8 response at beginning of message";
                        }
                        break;
                    }
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_PERIODIC_READ_SUCCESS:
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_RECEIVED_REQUEST:
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_MESSAGE_SUCCEEDED:
                    {
                        strData = "Calling Application Invocation ID: " + ArgumentReader.ReadUInt16().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_EXCEPTION_FAILED:
                    {
                        byte byReason = ArgumentReader.ReadByte();
                        if (byReason == 1)
                        {
                            strData = "Cannot Add to EPSEM";
                        }
                        else if (byReason == 2)
                        {
                            strData = "Encryption Failed";
                        }
                        else if (byReason == 3)
                        {
                            strData = "Send Failed";
                        }
                        break;
                    }
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_EXCEPTION_SUCCESS:
                    {
                        strData = "Calling Application Invocation ID: " + ArgumentReader.ReadUInt16().ToString(CultureInfo.InvariantCulture) +
                                  "; Event ID: " + ArgumentReader.ReadUInt16().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_SEND_RESPONSE_FAILED:
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_REGISTRATION_RESULT:
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_DEREGISTRATION_RESULT:
                    {
                        byte C1222MessageFailureCode = ArgumentReader.ReadByte();

                        strData = GetC1222MessageFailureCode(C1222MessageFailureCode);
                        break;
                    }
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_SENT_MESSAGE_FAILED:
                    {
                        UInt16 CallingApInvocationID = ArgumentReader.ReadUInt16();
                        byte C1222MessageFailureCode = ArgumentReader.ReadByte();

                        strData = "Calling Application Invocation ID: " + CallingApInvocationID.ToString(CultureInfo.InvariantCulture);
                         
                        strData += "; " + GetC1222MessageFailureCode(C1222MessageFailureCode);
                        break;
                    }
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_LEVEL_CHANGE:
                    {
                        strData = "New Level: " + ArgumentReader.ReadByte().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_BEST_FATHER_CHANGE:
                    {
                        strData = "New Best Father: " + ArgumentReader.ReadUInt32().ToString("X8", CultureInfo.InvariantCulture);
                        break;
                    }
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_SYNCH_FATHER_CHANGE:
                    {
                        strData = "New Synch Father: " + ArgumentReader.ReadUInt32().ToString("X8", CultureInfo.InvariantCulture);
                        break;
                    }
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_PROCESS_RCVD_MSG_TIMING:
                    {
                        UInt32 firstSegReceived = ArgumentReader.ReadUInt32();
                        UInt32 lastSegReceived = ArgumentReader.ReadUInt32();
                        UInt32 avaiableforServer = ArgumentReader.ReadUInt32();
                        UInt32 responseBuiltPreEncryption = ArgumentReader.ReadUInt32();
                        UInt32 responseReadyToSend = ArgumentReader.ReadUInt32();

                        UInt32 uiSegReceiveTime = lastSegReceived - firstSegReceived;
                        UInt32 uiDecryptionTime = avaiableforServer - lastSegReceived;
                        UInt32 uiMeterProcessTime = responseBuiltPreEncryption - avaiableforServer;
                        UInt32 uiEncryptionTime = responseReadyToSend - responseBuiltPreEncryption;

                        strData = "Segement Receive Time: " + uiSegReceiveTime.ToString(CultureInfo.InvariantCulture) +
                                    "ms; Decryption Time: " + uiDecryptionTime.ToString(CultureInfo.InvariantCulture) +
                                  "ms; Meter Process Time: " + uiMeterProcessTime.ToString(CultureInfo.InvariantCulture) + "ms; Encryption Time: " +
                                  uiEncryptionTime.ToString(CultureInfo.InvariantCulture) + "ms";
                        break;
                    }
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_RECEIVED_INVALID_MESSAGE:
                    {
                        byte[] byRawAptitle = new byte[20];
                        byRawAptitle = ArgumentReader.ReadBytes(20);
                        string strCallingAptitle = ESNConverter.Decode(byRawAptitle);
                        UInt16 callingAppInvocationID = ArgumentReader.ReadUInt16();
                        byte C1222MessageValidationResult1 = ArgumentReader.ReadByte();
                        byte C1222MessageValidationResult2 = ArgumentReader.ReadByte();

                        strData = "Calling APTitle: " + strCallingAptitle.ToString(CultureInfo.InvariantCulture) +
                                    "; Result 1: " + C1222MessageValidationResult1.ToString(CultureInfo.InvariantCulture) +
                                    "; Result 2: " + C1222MessageValidationResult2.ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_C1222_RFLAN_CELL_CHANGE:
                    {
                        strData = "New Cell ID: " + ArgumentReader.ReadUInt16().ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_RECEIVED_MESSAGE_FROM:
                case (ushort)CENTRON_AMI.LANEvents.COMEVENT_SENT_MESSAGE_TO:
                    {
                        byte[] byRawAptitle = new byte[20];
                        byRawAptitle = ArgumentReader.ReadBytes(20);
                        if (byRawAptitle[0] == 0x80)
                        {
                            // 0x80 is for the network message, it should be 0x0D. (per Steve Bardey)
                            byRawAptitle[0] = 0x0D;
                        }

                        string strAptitle = ESNConverter.Decode(byRawAptitle);
                        if (strAptitle == "1")
                        {
                            // "1" is the default - We should tell the user this is their own device (per Steve Bardey)
                            strAptitle += " (Meter's Comm Module)";
                        }

                        UInt16 CalledSeq = ArgumentReader.ReadUInt16();
                        byte C1222MessageType = ArgumentReader.ReadByte();

                        strData = "APTitle: " + strAptitle + "; Called Sequence: " + CalledSeq.ToString(CultureInfo.InvariantCulture) + "; " + GetC1222MessageType(C1222MessageType);
                        break;
                    }
                case (ushort) CENTRON_AMI.LANEvents.COMEVENT_PR_SEND_TABLE:
                case (ushort) CENTRON_AMI.LANEvents.COMEVENT_PR_SEND_TABLE_FAILED:
                    {
                        UInt16 table8ProcedureID = ArgumentReader.ReadUInt16();
                        byte table8Sequence = ArgumentReader.ReadByte();
                        byte table8Result = ArgumentReader.ReadByte();
                        byte table8TotalWriteCount = ArgumentReader.ReadByte();
                        UInt16 tableID = ArgumentReader.ReadUInt16();
                        UInt32 offset = ArgumentReader.ReadUInt32();
                        UInt16 length = ArgumentReader.ReadUInt16();
                        UInt16 result = ArgumentReader.ReadUInt16();
                        byte tableReadResult = ArgumentReader.ReadByte();

                        strData = "Table 8 Procedure ID: " + table8ProcedureID.ToString(CultureInfo.InvariantCulture) +
                                  "; Table 8 Sequence: " + table8Sequence.ToString(CultureInfo.InvariantCulture) +
                                  "; Table 8 Result: " + table8Result.ToString(CultureInfo.InvariantCulture) +
                                  "; Table 8 Write Count: " + table8TotalWriteCount.ToString(CultureInfo.InvariantCulture) +
                                  "; Table ID: " + tableID.ToString(CultureInfo.InvariantCulture) +
                                  "; Offset: " + offset.ToString(CultureInfo.InvariantCulture) + 
                                  "; length: " + length.ToString(CultureInfo.InvariantCulture) +
                                  "; Table Send Result: " + GetC1222TableSendResult(result) +
                                  "; Table Read Result: " + tableReadResult.ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                case (ushort)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_DRLC_MSG:
                case (ushort)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_KEY_EST_MSG:
                case (ushort)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_MESSAGING_MSG:
                case (ushort)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_MSG:
                case (ushort)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_PREPAY_MSG:
                case (ushort)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_PRICE_MSG:
                case (ushort)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_SE_TUNNELING_MSG:
                case (ushort)CENTRON_AMI.HANEvents.EVENT_ZIGBEE_SIMPLE_METERING_MSG:
                    {
                        byte byMessageStatus = ArgumentReader.ReadByte();
                        byte byLQI = ArgumentReader.ReadByte();
                        byte byRSSI = ArgumentReader.ReadByte();
                        UInt16 SrcOrDest = ArgumentReader.ReadUInt16();
                        UInt16 ProfileID = ArgumentReader.ReadUInt16();
                        UInt16 ClusterID = ArgumentReader.ReadUInt16();
                        byte bySrcEndPoint = ArgumentReader.ReadByte();
                        byte byDestEndPoint = ArgumentReader.ReadByte();
                        UInt16 APSOptions = ArgumentReader.ReadUInt16();
                        UInt16 GroupID = ArgumentReader.ReadUInt16();
                        byte byAPSSeqNbr = ArgumentReader.ReadByte();
                        byte byMsgLen = ArgumentReader.ReadByte();
                        byte[] arrMsg = ArgumentReader.ReadBytes((int)byMsgLen);

                        // There is a lot of information that can be retreived for this event.  Right now we
                        //  are showing just a very small part.  As we understand how these events are used
                        //  we can come in here and show more data if necessary.
                        if ((byMessageStatus & RX_OR_TX_MASK) == 0)   // (0 == Transmitted; 1 == Received)
                        {
                            strData = "Transmitted - ";
                        }
                        else
                        {
                            strData = "Received - ";
                        }

                        if ((byMessageStatus & SUCCESS_OR_FAIL_MASK) == 0)    //(0 == Failure; 1 == Success)
                        {
                            strData += "Failed;";
                        }
                        else
                        {
                            strData += "Succeeded;";
                        }

                        break;
                    }

            }

            return strData;
        }

        /// <summary>
        /// Gets the Human Readable String for the C12.22 Table Send Result
        /// </summary>
        /// <param name="result">Numerical value of the error code</param>
        /// <returns>string - Human Readable text</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/03/08 KRC 1.51.01		    Created
        //
        private string GetC1222TableSendResult(UInt16 result)
        {
            string strData = "";

            switch (result)
            {
                case 0:
                    {
                        strData = "Success";
                        break;
                    }
                case 1:
                    {
                        strData = "Table 8 parameters changed";
                        break;
                    }
                case 2:
                    {
                        strData = "Table too big";
                        break;
                    }
                case 3:
                    {
                        strData = "Request offset greater than or equal to table length";
                        break;
                    }
                case 4:
                    {
                        strData = "Invalid table request";
                        break;
                    }
                case 5:
                    {
                        strData = "Cannot find app data in constructed data";
                        break;
                    }
                case 6:
                    {
                        strData = "Data length too big";
                        break;
                    }
                case 7:
                    {
                        strData = "Read result was not OK";
                        break;
                    }
                case 8:
                    {
                        strData = "Offset read result was not OK";
                        break;
                    }
                case 9:
                    {
                        strData = "Could not adjust message length";
                        break;
                    }
                case 10:
                    {
                        strData = "Could not get app data when padding EPSEM";
                        break;
                    }
                case 11:
                    {
                        strData = "Could not adjust (add to) user information length when padding EPSEM";
                        break;
                    }
                case 12:
                    {
                        strData = "Could not adjust (subtract from) user info length when padding epsem";
                        break;
                    }
                case 13:
                    {
                        strData = "Could not adjust (subtract from) user info length when padding epsetm (place 2)";
                        break;
                    }
                case 14:
                    {
                        strData = "Epsem Position to end failed";
                        break;
                    }
                case 15:
                    {
                        strData = "Read length of full table read changed from when checked at beginning of processing of a table to when the table was read. Table read failed.";
                        break;
                    }
                case 16:
                    {
                        strData = "Read length of offset table read changed from when checked at beginning of processing of a table to when the table was read. Table read failed.";
                        break;
                    }
            }

            return strData;
        }

        /// <summary>
        /// Gets the Human Readable String for the C12.22 Message Type 
        /// </summary>
        /// <param name="C1222MessageType">Numerical value of the error code</param>
        /// <returns>string - Human Readable text</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/13/08 KRC 1.50.24		    Created
        //
        private string GetC1222MessageType(byte C1222MessageType)
        {
            string strData = "";

            switch (C1222MessageType)
            {
                case 0:
                    {
                        strData = "Unknown Message Type";
                        break;
                    }
                case 1:
                    {
                        strData = "Invalid Message Received";
                        break;
                    }
                case 2:
                    {
                        strData = "Exception Sent by the Meter";
                        break;
                    }
                case 3:
                    {
                        strData = "Periodic Read Response Sent by the Meter";
                        break;
                    }
                case 4:
                    {
                        strData = "Registration Request Sent from the Meter";
                        break;
                    }
                case 5:
                    {
                        strData = "Deregistration Request Sent from the Meter";
                        break;
                    }
                case 6:
                    {
                        strData = "Registration Response Received by the Meter";
                        break;
                    }
                case 7:
                    {
                        strData = "Deregistration Rersponse Received by the Meter";
                        break;
                    }
                case 8:
                    {
                        strData = "Request Reccevied by the Meter";
                        break;
                    }
                case 9:
                    {
                        strData = "Response Sent from the Meter to a Received Request";
                        break;
                    }
            }

            return strData;
        }

        /// <summary>
        /// Gets the Human Readable String for the C12.22 Message Failure Code 
        /// </summary>
        /// <param name="C1222MessageFailureCode">Numerical value of the error code</param>
        /// <returns>string - Human Readable text</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/13/08 KRC 1.50.24		    Created
        //
        private string GetC1222MessageFailureCode(byte C1222MessageFailureCode)
        {
            string strData = "";

            switch (C1222MessageFailureCode)
            {
                case 0:
                    {
                        strData = "No Error";
                        break;
                    }
                case 1:
                    {
                        strData = "Send Message Failed";
                        break;
                    }
                case 2:
                    {
                        strData = "Received EPSEM Invalid";
                        break;
                    }
                case 3:
                    {
                        strData = "Received Information Element Missing";
                        break;
                    }
                case 4:
                    {
                        strData = "Send Transport Busy";
                        break;
                    }
                case 5:
                    {
                        strData = "Send Invalid Native Address";
                        break;
                    }
                case 6:
                    {
                        strData = "Send Set Unsegmented Message Failed";
                        break;
                    }
                case 7:
                    {
                        strData = "Send All Retries Failed";
                        break;
                    }
                case 8:
                    {
                        strData = "Send Get Segment Failed";
                        break;
                    }
                case 9:
                    {
                        strData = "No Request or Response in EPSEM";
                        break;
                    }
                case 10:
                    {
                        strData = "Register/Deregister APTitle Invalid";
                        break;
                    }
                case 11:
                    {
                        strData = "Deregister APTitle Mismatch";
                        break;
                    }
                case 12:
                    {
                        strData = "Register/Deregister Response Length Invalid";
                        break;
                    }
                case 13:
                    {
                        strData = "Register/Deregister Response Not OK";
                        break;
                    }
                case 14:
                    {
                        strData = "Register/Deregister EPSEM Invalid";
                        break;
                    }
                case 15:
                    {
                        strData = "Register/Deregister Information Element Missing";
                        break;
                    }
                case 16:
                    {
                        strData = "Register/Deregister APTitle Compare Failed";
                        break;
                    }
                case 17:
                    {
                        strData = "Register Not Synchronized";
                        break;
                    }
                case 18:
                    {
                        strData = "Register/Deregister Pending or Timeout";
                        break;
                    }
            }

            return strData;
        }

        /// <summary>
        /// Get the Human Readable translation of the C12.22 Message Validation Result Code
        /// </summary>
        /// <param name="C122MessageValidationResult">Numerical value of the result code</param>
        /// <returns>string - Human Readable text</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/13/08 KRC 1.50.24		    Created
        //
        private string GetC1222MessageValidationResult(byte C122MessageValidationResult)
        {
            string strData = "";

            switch (C122MessageValidationResult)
            {
                case 0:
                    {
                        strData = "No Error";
                        break;
                    }
                case 1:
                    {
                        strData = "No calling APTitle in the Message";
                        break;
                    }
                case 2:
                    {
                        strData = "No Information Element in the Message";
                        break;
                    }
                case 3:
                    {
                        strData = "Tags Out of Order";
                        break;
                    }
                case 4:
                    {
                        strData = "ACSE Context is not C12.22";
                        break;
                    }
                case 5:
                    {
                        strData = "Called APTitle Invalid";
                        break;
                    }
                case 6:
                    {
                        strData = "Calling APTitle Invalid";
                        break;
                    }
                case 7:
                    {
                        strData = "Calling AE Qualifier Invalid";
                        break;
                    }
                case 8:
                    {
                        strData = "Called AE Qualifier Invalid";
                        break;
                    }
                case 9:
                    {
                        strData = "Tag Invalid in Segmented Message";
                        break;
                    }
                case 10:
                    {
                        strData = "Called AP Invocation ID Invalid";
                        break;
                    }
                case 11:
                    {
                        strData = "Calling Ap Invocation ID Invalid";
                        break;
                    }
                case 12:
                    {
                        strData = "Authentication Value Invalid";
                        break;
                    }
                case 13:
                    {
                        strData = "Information Value Invalid";
                        break;
                    }
                case 14:
                    {
                        strData = "Encryption KeyID Missing";
                        break;
                    }
                case 15:
                    {
                        strData = "Invalid Message Structure for Decryption";
                        break;
                    }
                case 16:
                    {
                        strData = "Information Element Decryption Failed";
                        break;
                    }
                case 17:
                    {
                        strData = "Authentication Failed";
                        break;
                    }
                case 18:
                    {
                        strData = "Invalid Message Structure For Authentication";
                        break;
                    }
                case 19:
                    {
                        strData = "Message Must Not Be Plain Text";
                        break;
                    }
                case 20:
                    {
                        strData = "User Element Decryption Failed";
                        break;
                    }   
            }

            return strData;
        }

        /// <summary>String resource describing our events</summary>
        protected System.Resources.ResourceManager m_rmStrings;

        private readonly string RESOURCE_FILE_PROJECT_STRINGS =
                                    "Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings";

    }
}
