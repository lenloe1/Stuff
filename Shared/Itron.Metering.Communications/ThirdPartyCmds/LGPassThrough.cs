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
//                              Copyright © 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using System.Threading;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Globalization;
using Itron.Metering.Utilities;

namespace Itron.Metering.Communications
{
    /// <summary>
    /// This class carries out the Landis+Gyr Pass Through command for the M2 Gateway
    /// </summary>
    public class LGPassThrough
    {
        #region Constants

        private const int LG_PASS_THRU_CMD_LEN = 13;
        private const int MAX_RETRIES = 3;
        private const int PASS_THRU_WAIT_TIME = 100;
        private const int PASS_THRU_RESP_LENGTH = 1;

        #endregion

        #region Definitions

        /// <summary>
        /// Enumeration that encapsulates the L+G responses to the Pass through mode command
        /// </summary>
        public enum LGPassThruResponse : byte
        {
            /// <summary>
            /// Acknowledged - Pass Thru mode enabled
            /// </summary>
            LG_ACK = 0x31,
            /// <summary>
            /// Error - request rejected for unknown reason
            /// </summary>
            LG_ERROR = 0x32,
            /// <summary>
            /// Timer timed out - Pass thru mode terminated
            /// </summary>
            LG_TIMER_TIMED_OUT = 0x36,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Comm">The communication object that supports
        /// communication over the physical port.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/14/10 AF  2.40           Created
        //
        public LGPassThrough(ICommunications Comm)
        {
            m_CommPort = Comm;

            // For checking when commport has received data
            m_CommPort.FlagCharReceived += new CommEvent(RcvdData);

            // Create semaphore to handle data received			
            m_ReadEvent = new ManualResetEvent(false);
        }

        /// <summary>
        /// Constructs and sends out the pass through mode command and interprets 
        /// the response from the meter
        /// </summary>
        /// <returns>Response code enum</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/01/10 AF  2.40.32        Created
        //  07/01/10 AF  2.42.01        Adjusted the timeout so that meter won't be
        //                              in pass through mode after log off
        //  04/04/11 AF  2.50.20 171044 Turn off logging while sending the L&G password
        //
        public LGPassThruResponse SendPassThroughCmd()
        {
            LGPassThruResponse Response = LGPassThruResponse.LG_ERROR;

            // Construct the packet and send it out.
            // 0x53 = 'S' = start of pass thru cmd.
            // 0x36 = 6 = baud rate (range is 1200 (0x33) - 9600 (0x36), highest supported  )
            // 0x30 0x37 = 0x06 = 7 seconds timeout
            // 0x34 = 4 = Data Format = 8N1
            // 0x34 0x42 0x38 0x30 0x31 0x32 0x39 0x33 = L&G password
            byte[] byCmdBuffer = new byte[] { 0x53, 0x36, 0x30, 0x37, 0x34, 0x34, 0x42, 0x38, 0x30, 0x31, 0x32, 0x39, 0x33 };
            byte[] byRxByte = null;
            int iIndex = 0;

            //Do not show the security code in the log file
            Logger.LoggingState CurrentState = m_Logger.LoggerState;
            m_Logger.LoggerState = Logger.LoggingState.PROTOCOL_SENDS_SUSPENDED;

            while ((LGPassThruResponse.LG_ACK != Response) && (iIndex < MAX_RETRIES))
            {
                m_ReadEvent.Reset();

                m_CommPort.Send(byCmdBuffer);

                if (m_ReadEvent.WaitOne(PASS_THRU_WAIT_TIME, false))
                {
                    if (0 < m_CommPort.Read(PASS_THRU_RESP_LENGTH, 0))
                    {
                        byRxByte = new byte[m_CommPort.InputLen];
                        Array.Copy(m_CommPort.Input, 0, byRxByte, 0, PASS_THRU_RESP_LENGTH);
                    }

                    if ((byRxByte != null) &&
                        (System.Enum.IsDefined(Response.GetType(), byRxByte[0])))
                    {
                        Response = (LGPassThruResponse)byRxByte[0];
                    }
                }

                iIndex++;
            }

            //Resume logging
            m_Logger.LoggerState = CurrentState;

            return Response;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method called when the communications port character received flag
        /// is set.
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/06/10 AF  2.40.32        Created/cloned from SCS Protocol code 
        private void RcvdData()
        {
            // Signal semaphore when data is received on the port			
            m_ReadEvent.Set();
        }

        #endregion

        #region Members

        /// <summary>
        /// Communication object
        /// </summary>
        public ICommunications m_CommPort;

        private ManualResetEvent m_ReadEvent = null;
        private Logger m_Logger = Logger.TheInstance;

        #endregion

    }
}
