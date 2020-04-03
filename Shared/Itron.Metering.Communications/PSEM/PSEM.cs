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
//                              Copyright © 2006 - 2016
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using Itron.Metering.Communications;
using Itron.Metering.Utilities;
using System.Windows.Forms;

namespace Itron.Metering.Communications.PSEM
{
	
	/// <summary>
	/// CPSEM supports the PSEM services for ANSI devices.
	/// </summary>
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ ---------------------------------------
	/// 08/01/05 bdm 7.13.00 N/A	Created
	public class CPSEM
    {
        #region Constants        
        /// <summary>
        /// The max time the protocol allows between packets, 255 seconds.
        /// Use this constant with the Wait() request to keep the device from
        /// timing out.
        /// </summary>
        public const byte MAX_WAIT_TIME = 0xFF;
        private const int DEFAULT_C1218_INTERMESSAGE_TIMEOUT = 1500; //Previously 5000, attempting to resolve timeout issues
        
        /// <summary>
        /// Communication object
        /// </summary>
        public ICommunications m_CommPort;

		private Logger m_hLogFile;

        /// <summary>
        /// Default max packet size for Itron meters (128)
        /// </summary>
        public const ushort DEFAULT_MAX_PACKET_LEGNTH = 0x80;
        /// <summary>
        /// Default max number packets for Itron meters (254)
        /// </summary>
        public const byte DEFAULT_MAX_NUMBER_OF_PACKETS = 0xFE;
        /// <summary>
        /// Default max number of packets for Landis+Gyr M2 Gateway meter (1)
        /// </summary>
        public const byte DEFAULT_MAX_NUMBER_OF_PACKETS_LG = 0x01;
        /// <summary>
        /// Default baud rate
        /// </summary>
        public const uint DEFAULT_BAUD_RATE = 9600;
        /// <summary>
        /// Default Canadian user ID for HH-Pro
        /// </summary>
        public const int DEFAULT_HH_PRO_USER_ID = 101;

        #endregion

        #region Definitions
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="comm">
        /// The communication object that supports
        /// communication over the physical layer.
        /// </param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        /// 06/14/06 mrj 7.30.00        Added ability to ignore wait commands
        /// 08/29/06 mrj 7.35.00        Store off the comm port to expose to
        ///                             the ANSI device, and the packet info.
        /// 04/16/10 AF  2.40.38        Added a max wait variable for M2 Gateway
        /// 04/26/10 AF  2.40.43        Replaced hard coded wait time with constant
        /// 
        /// 
        public CPSEM(ICommunications comm)
        {
            //The only communication supported is the port - which 
            //implies c12.18.
            //When other communication types are supported the layer 7 protocol
            //such as C12.18 or C12.21) will need to be determined, likely from 
            //the type of communication being supported (port, modem, ...).
            m_ANSIL7 = new CC1218L7(comm);
            m_CommPort = comm;
			m_hLogFile = Logger.TheInstance;

            m_byNbrPackets = DEFAULT_MAX_NUMBER_OF_PACKETS;
            m_lUserID = DEFAULT_HH_PRO_USER_ID;
            m_iTimeFormat = -1;
            m_MaxWaitTime = MAX_WAIT_TIME;
        }

        /// <summary>
        /// Performs the security service.
        /// </summary>
        /// <param name="userPassword">
        /// The user password to use when performing the security
        /// service.
        /// </param>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// PSEM.Security("userpassword");
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
		/// 03/14/07 mrj 8.00.18		Added wait if security failed.
		/// 
        public PSEMResponse Security(string userPassword)
        {
            PSEMResponse Result;

            Result = m_ANSIL7.Security(userPassword);

            return Result;
        }

        /// <summary>
        /// Performs the security service.
        /// </summary>
        /// <param name="byUserPassword">
        /// The user password to use when performing the security
        /// service.
        /// </param>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// PSEM.Security("userpassword");
        /// </code>
        /// </example>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/18/07 RCG 8.10.14 N/A	Created

        public PSEMResponse Security(byte[] byUserPassword)
        {
            PSEMResponse Result;

            Result = m_ANSIL7.Security(byUserPassword);

            return Result;
        }

        /// <summary>
        /// Performs a logon to the device.  It sends Identify, negotiate, and
        /// then logon.
        /// </summary>
        /// <param name="strUser">
        /// The logon user name.
        /// </param>
        /// <param name="lngUserID">
        /// The logon user ID.
        /// </param>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the user length exceeds the acceptable length.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// PSEM.Security("userpassword");
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        /// 08/16/06 KRC 7.35.00 N/A    Need real Logon support
        /// 08/31/06 mrj 7.35.00        Save off the user id
        /// 
        public PSEMResponse Logon(string strUser, long lngUserID)
        {
            PSEMResponse Result = PSEMResponse.Err;

            Result = m_ANSIL7.Logon(strUser, lngUserID);
            //Do not logoff if logon failed.  A logoff may
            //throw an exception which would mask the logon
            //error.

            if (PSEMResponse.Ok == Result)
            {
                m_lUserID = lngUserID;
                MaintainCommunicationsSession();
            }

            return Result;
        }

        /// <summary>Performs the Identify service.</summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Identify();		
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/12/06 mrj 7.30.00    N/A Created
        /// 08/16/06 KRC 7.35.00    N/A Adding Desktop support
        public PSEMResponse Identify()
        {
            PSEMResponse Result = PSEMResponse.Err;

            Result = m_ANSIL7.Identify();

            return Result;
        }

        /// <summary>Performs the Negotiate service.</summary>
        /// <param name="usPacketSize">Maximum packet size supported, in bytes. 
        /// This alue shall not be greater than 8192 bytes.</param>
        /// <param name="byNbrPackets">Maximum number of packets this layer is 
        /// able to reassemble into an upper layer data structure at one 
        /// time.</param>
        /// <param name="uiBaudRate">The baud rate to negotiate to.</param>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Identify();
        /// PSEM.Negotiate(128, 254, 9600 );		
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/12/06 mrj 7.30.00    N/A Created
        /// 08/31/06 mrj 7.35.00        Save off negotiated packet info
        /// 
        public PSEMResponse Negotiate(ushort usPacketSize, byte byNbrPackets, uint uiBaudRate)
        {
            PSEMResponse Result = PSEMResponse.Err;

            if (m_CommPort.MaxSupportedPacketSize < usPacketSize)
            {
                usPacketSize = m_CommPort.MaxSupportedPacketSize;
            }

            Result = m_ANSIL7.Negotiate(usPacketSize, byNbrPackets, uiBaudRate);

            return Result;
        }

        /// <summary>Performs the Terminate service.</summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Identify();
        /// PSEM.Negotiate(128, 254, 9600 );
        /// PSEM.Logon("user");
        /// PSEM.Security("");
        /// PSEM.Terminate();		
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/12/06 mrj 7.30.00    N/A Created
        /// 
        public PSEMResponse Terminate()
        {
            PSEMResponse Result = PSEMResponse.Err;

            DisableSessionMaintenance();

            Result = m_ANSIL7.Terminate();

            return Result;
        }

        /// <summary>
        /// Performs a logoff from the device. 
        /// </summary>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// PSEM.Security("userpassword");
        /// PSEM.Logoff();
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public PSEMResponse Logoff()
        {
            PSEMResponse Result = PSEMResponse.Err;
            DisableSessionMaintenance();

            Result = m_ANSIL7.Logoff();

            return Result;
        }

        /// <summary>
        /// Performs the wait service.
        /// </summary>
        /// <param name="seconds">
        /// Number of seconds to send when performing the wait service.
        /// </param>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// PSEM.Security("userpassword");
        /// PSEM.Wait(0xFF);
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        /// 06/14/06 mrj 7.30.00        Added ability to ignore wait commands
		/// 03/14/07 mrj 8.00.18		Removed ignore waits
        /// 
        public PSEMResponse Wait(byte seconds)
        {		
			PSEMResponse Result = PSEMResponse.Err;

            PauseSessionTimer();

            Result = m_ANSIL7.Wait(seconds);

            if (Result == PSEMResponse.Ok)
            {
                ResetSessionTimerToMax();
            }                

            return Result;
        }


        /// <summary>Performs the full write service.</summary>
        /// <param name="data">The data to write to the table.</param>
        /// <param name="table">
        /// The table for which to peform a full write.
        /// </param>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// byte[] abytData = new byte[3]{0x01, 0x02, 0x03};
        /// PSEM.Logon("username");
        /// PSEM.Security("userpassword");
        /// PSEM.FullWrite(5, abytData);
        /// </code>
        /// </example>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  08/01/05 bdm 7.13.00 N/A	Created
		//	04/12/07 mrj 8.00.29 2879	Changed to always reset the session timer, even
		//								if we get a protocol error.
		//
        public PSEMResponse FullWrite(ushort table, byte[] data)
        {
            PSEMResponse Result;

            PauseSessionTimer();

            Result = m_ANSIL7.FullWrite(table, data);

            ResetSessionTimer();

            return Result;
        }

        /// <summary>Performs the offset write service.</summary>
        /// <param name="data">The data to write to the table.</param>
        /// <param name="table">
        /// The table for which to peform an offset write.
        /// </param>
        /// <param name="offset">Offset into the table.</param>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// byte[] abytData = new byte[3]{0x01, 0x02, 0x03};
        /// PSEM.Logon("username");
        /// PSEM.Security("userpassword");
        /// PSEM.FullWrite(2048, 400, abytData);
        /// </code>
        /// </example>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  08/01/05 bdm 7.13.00 N/A	Created
		//	04/12/07 mrj 8.00.29 2879	Changed to always reset the session timer, even
		//								if we get a protocol error.
		//	
        public PSEMResponse OffsetWrite(ushort table, int offset, byte[] data)
        {
            PSEMResponse Result;

            PauseSessionTimer();

            Result = m_ANSIL7.OffsetWrite(table, offset, data);

            ResetSessionTimer();

            return Result;
        }


        /// <summary>Performs the index write service.</summary>
        /// <param name="data">The data to write to the table.</param>
        /// <param name="table">
        /// The table for which to peform an index write.
        /// </param>
        /// <param name="index">Index into the table.</param>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// byte[] abytData = new byte[3]{0x01, 0x02, 0x03};
        /// PSEM.Logon("username");
        /// PSEM.Security("userpassword");
        /// PSEM.IndexWrite(2049, 2, abytData); 
        /// </code>
        /// </example>
        /// <exception cref="NotImplementedException">
        /// IndexWrite not yet implemented.
        /// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  08/01/05 bdm 7.13.00 N/A	Created
		//	04/12/07 mrj 8.00.29 2879	Changed to always reset the session timer, even
		//								if we get a protocol error.
		//	
        public virtual PSEMResponse IndexWrite(ushort table, ushort index, byte[] data)
        {
            PSEMResponse Result;

            PauseSessionTimer();

            Result = m_ANSIL7.IndexWrite(table, index, data);

            ResetSessionTimer();

            return Result;
        }

        /// <summary>
        /// Performs the full read service.
        /// </summary>
        /// <remarks>
        /// When an error occurs, the byte array of data will be null.
        /// </remarks>
        /// <param name="data">
        /// The table's data.
        /// </param>
        /// <param name="table">
        /// The table for which to peform a full read.
        /// </param>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// byte[] abytData = null;
        /// PSEM.Logon("username");
        /// PSEM.Security("userpassword");
        /// PSEM.FullRead(5, out abytData);
        /// </code>
        /// </example>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/01/05 bdm 7.13.00 N/A	Created
		//	04/12/07 mrj 8.00.29 2879	Changed to always reset the session timer, even
		//								if we get a protocol error.
		//	
        public PSEMResponse FullRead(ushort table, out byte[] data)
        {
            PSEMResponse Result;

            PauseSessionTimer();

            Result = m_ANSIL7.FullRead(table, out data);
							
            ResetSessionTimer();                

            return Result;
        }

        /// <summary>Performs the offset read service.</summary>
        /// <remarks>
        /// When an error occurs, the byte array of data will be null.
        /// </remarks>
        /// <param name="data">The table's data.</param>
        /// <param name="table">
        /// The table for which to peform a full read.
        /// </param>
        /// <param name="count">Number of bytes to read.</param>
        /// <param name="offset">Offset into the table from where the
        /// read should begin.</param>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// byte[] abytData = null;
        /// PSEM.Logon("username");
        /// PSEM.Security("userpassword");
        /// PSEM.OffsetRead.OffsetRead(2049, 4261, 54, out abytData); 
        /// </code>
        /// </example>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  08/01/05 bdm 7.13.00 N/A	Created
		//	04/12/07 mrj 8.00.29 2879	Changed to always reset the session timer, even
		//								if we get a protocol error.
		//	
        public PSEMResponse OffsetRead(ushort table, int offset, ushort count, out byte[] data)
        {
            PSEMResponse Result;

            PauseSessionTimer();

            Result = m_ANSIL7.OffsetRead(table, offset, count, out data);

            ResetSessionTimer();

            return Result;
        }

        /// <summary>Performs the index read service.</summary>
        /// <remarks>
        /// When an error occurs, the byte array of data will be null.
        /// </remarks>
        /// <param name="data">The table's data.</param>
        /// <param name="table">
        /// The table for which to peform a full read.
        /// </param>
        /// <param name="count">Number of bytes to read.</param>
        /// <param name="index">Index into the table from where the
        /// read should begin.</param>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// byte[] abytData = null;
        /// PSEM.Logon("username");
        /// PSEM.Security("userpassword");
        /// PSEM.IndexRead(2049, 2, 54, out abytData); 
        /// </code>
        /// </example>
        /// <exception cref="NotImplementedException">
        /// IndexRead not yet implemented.
        /// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  08/01/05 bdm 7.13.00 N/A	Created
		//	04/12/07 mrj 8.00.29 2879	Changed to always reset the session timer, even
		//								if we get a protocol error.
		//	
        public PSEMResponse IndexRead(ushort table, ushort index, ushort count, out byte[] data)
        {
            PSEMResponse Result;

            PauseSessionTimer();

            Result = m_ANSIL7.IndexRead(table, index, count, out data);

            ResetSessionTimer();

            return Result;
        }

        /// <summary>Changes the timing parameters.  Calling this service will
        /// cause our C12.18 meters to switch to C12.21.</summary>
        /// <remarks>
        /// </remarks>
        /// <param name="byCTO">Channel Traffic Timeout in seconds</param>
        /// <param name="byITO">Inter-character Timeout in seconds</param>
        /// <param name="byRTO">Response Timeout in seconds</param>
        /// <param name="byRetries">Number of retries. If the default of 3 doesn't
        /// work, 45 probably won't work either.</param>
        /// <returns>
        /// A PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request.
        /// </returns>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <exception cref="NotImplementedException">
        /// IndexRead not yet implemented.
        /// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/24/08 mcm 1.0.x   Support for ZigBee communications
		//	
        public PSEMResponse TimingSetup(byte byCTO, byte byITO, byte byRTO, byte byRetries)
        {
            return m_ANSIL7.TimingSetup(byCTO, byITO, byRTO, byRetries);
        }

        #endregion

        #region Public Properties
        
        /// <summary>
        /// Returns the negotiated packet size
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 08/31/06 mrj 7.35.00 N/A    Created
        ///
        public ushort PacketSize
        {
            get
            {
                return m_ANSIL7.MaxPacketSize;
            }
        }

        /// <summary>
        /// Returns the negotiated number of packets
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 08/31/06 mrj 7.35.00 N/A    Created
        ///
        public byte NumberPackets
        {
            get
            {
                return m_byNbrPackets;
            }
        }

        /// <summary>
        /// Returns the negotiated baud rate.
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 02/07/14 jrf 3.50.32 419257 Created
        /// 05/16/16 AF  4.50.268 622562  Added the ability to set the baud rate for times
        ///                               that the meter has timed out and we want to return to
        ///                               base state for a retry.
        ///
        public uint BaudRate
        {
            get
            {
                return m_ANSIL7.BaudRate;
            }
            set
            {
                m_ANSIL7.BaudRate = value;
            }
        }

        /// <summary>
        /// Returns the current user id for this session
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 08/31/06 mrj 7.35.00 N/A    Created
        ///
        public long UserID
        {
            get
            {
                return m_lUserID;
            }
        }

        /// <summary>
        /// Access to the ANSI Time Format. The time format comes from standard
        /// table 00.  Be sure it has been read before accessing this property.
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 09/19/06 KRC 7.35.00 N/A    Created
        ///
        public int TimeFormat
        {
            get
            {
                if (m_iTimeFormat >= 0)
                {
                    return m_iTimeFormat;
                }
                else
                {
                    throw (new Exception("Time Format not set!"));
                }
            }
            set
            {
                m_iTimeFormat = value;
            }
        }

        /// <summary>
        /// Access to the ANSI Reference Time - Value differs depending on which ANSI meter you
        /// //  are talking to.
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 03/16/07 KRC 8.00.19 N/A    Created
        ///
        public DateTime ReferenceTime
        {
            get
            {
                return m_dtReferenceTime;
            }
            set
            {
                m_dtReferenceTime = value;
            }
        }

        /// <summary>
        /// The max time the protocol allows between packets is 255 seconds.
        /// Use this constant with the Wait() request to keep the device from
        /// timing out.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/16/10 AF  2.40.38        Created
        //
        public byte MaxWaitTime
        {
            get
            {
                return m_MaxWaitTime;
            }
            set
            {
                m_MaxWaitTime = value;
            }
        }

        /// <summary>
        /// Supports retrieving and setting the identity byte (reserved byte).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#         Description
        //  -------- --- ------- ------------   -------------------------------------------
        //  03/12/13 AF  2.80.08 TR7578, 7582   Created
        //
        public byte IdentityByte
        {
            get
            {
                return m_ANSIL7.IdentityByte;
            }
            set
            {
                m_ANSIL7.IdentityByte = value;
            }
        }

        /// <summary>
        /// Returns whether or not the next PSEM packet has the toggle bit set.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 03/26/14 jrf 3.50.55 465932 Created.
        public bool NextPacketToggleSet
        {
            get
            {
                return m_ANSIL7.NextPacketToggleSet;
            }
        }

        /// <summary>
        /// Supports retrieving and setting the CTO (Channel Traffic Timeout).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#         Description
        //  -------- --- ------- ------------   -------------------------------------------
        //  09/22/16 jrf 4.70.19 715635         Created
        //
        public uint CTO
        {
            get
            {
                return m_ANSIL7.CTO;
            }
            set
            {
                m_ANSIL7.CTO = value;
            }
        }



        #endregion

        #region Members

        //ANSI application layer 7.
        private CANSIL7 m_ANSIL7;
		      
        //mrj 8/31/06, save off the negotiated packet information
        private byte m_byNbrPackets;

        private long m_lUserID;
        private int m_iTimeFormat;
        private DateTime m_dtReferenceTime;

        private System.Windows.Forms.Timer m_SessionTimer = null;
        private byte m_MaxWaitTime;

        #endregion

        #region Private Methods

        /// <summary>
        ///  This method is used to maintain continuous communications with a metering device by
        ///  periodically issuing wait commands.  The time between wait commands is determined
        ///  by the protocol's default timeout values and will be reset each commands are issued
        ///  to the meter.
        /// 
        ///  Note that this method is a private method and all exception handling is the responsibility
        ///  of the calling routine
        /// </summary>
        /// <remarks>
        ///  Note that Windows.Forms Timers are used instead of system timers.  Windows.Form timers
        ///  are intended for single threaded, UI oriented applications while system timers are intended for 
        ///  thread safe applications.  Since serial communications is inherently single threaded and this protocol
        ///  class is not intended for concurrent use, Windows.Forms.Timers are the better choice
        ///
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/20/06 mah 8.00.00 N/A	Created
		/// 03/23/07 mrj 8.00.21		Do not allow multiple session timers
		///								to get created.
        /// </remarks>
        private void MaintainCommunicationsSession()
        {
			if (null == m_SessionTimer)
			{
				m_SessionTimer = new System.Windows.Forms.Timer();

				m_SessionTimer.Interval = DEFAULT_C1218_INTERMESSAGE_TIMEOUT;
				m_SessionTimer.Tick += new EventHandler(KeepSessionAliveHandler);

				m_SessionTimer.Enabled = true;
			}
        }

        /// <summary>
        /// This method is to be called when logging off a meter.  It disables
        /// periodic calls to the meter and releases the timer coordinating the calls
        /// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/20/06 mah 8.00.00 N/A	Created
        /// </remarks>
        private void DisableSessionMaintenance()
        {
            if (m_SessionTimer != null)
            {                
				m_SessionTimer.Enabled = false;				
                m_SessionTimer = null;				
            }
        }

        /// <summary>
        /// This method is called when the maintenance timer is fired.  It is responsible
        /// 
        /// for issueing a command to the meter to keep it alive and/or cleaning up if 
        /// we are no longer on line with the meter
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
        //  11/20/06 mah 8.00.00 N/A	Created
		//  03/12/07 mrj 8.00.18 2539	Stop the timer if the meter has timed out.
        //  04/19/10 AF  2.40           Replaced max wait time constant with member
        //                              variable
		//  
        private void KeepSessionAliveHandler(Object myObject, EventArgs myEventArgs)
        {
			try
			{
				m_hLogFile.WriteLine(Logger.LoggingLevel.Protocol, "Keep Alive");

				if (m_CommPort.IsOpen)				
				{					
					Wait(m_MaxWaitTime);					
				}
				else if (m_SessionTimer != null)
				{
					m_SessionTimer.Enabled = false;					
				}
			}
			catch
			{
				//If we had a timeout exception or any other exception we must catch
				//it here.  Disable the keep alive since we timed out.
				DisableSessionMaintenance();				
			}
        }

        /// <summary>
        /// This method should be called after each successful communication with the meter.
        /// It resets the maintenance timer for the communication session and prevents 
        /// unnecessary wakeup commands from being issued and slowing the 
        /// application down unnecessarily
        /// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/20/06 mah 8.00.00 N/A	Created
        /// </remarks>
        private void ResetSessionTimer()
        {
            if (m_SessionTimer != null)
            {
                m_SessionTimer.Interval = DEFAULT_C1218_INTERMESSAGE_TIMEOUT;
                m_SessionTimer.Enabled = true;
            }			
        }

        /// <summary>
        /// This method is called after a wait is successfully sent to the meter.  It resets
        /// the maintenance timer for the communication session to 245 seconds.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //								Created
        //  04/19/10 AF  2.40           Replaced max wait time constant with member
        //                              variable
        //  07/02/10 AF  2.42.01        M2 Gateway needs keep alives more frequently than
        //                              every 10 seconds
        //  09/17/10 AF  2.44.02        Send the Gateway keep alives a little more frequently
        //                              than the PSEM wait time so that the meter does not return
        //                              to base state.
        //  10/07/10 AF  2.45.02 161489 Now using a max wait time of 7 seconds for the M2 Gateway
        //                              but they still want the wait to be sent every 4 seconds.
        //  09/06/16 AF  4.60.05 682273 Send the keep alive every minute. We were experiencing timeouts especially
        //                              during ZigBee beaconing.
        //  
        private void ResetSessionTimerToMax()
        {
            if (m_SessionTimer != null)
            {
                // Note that we do not want to set the timeout to the maximum.  If we did the
                // meter would most likely timeout before we were able to send a message to it.
                // Therefore set the timeout to a value slightly smaller than the maximum

                if (m_MaxWaitTime > 10)
                {
                    m_SessionTimer.Interval = (m_MaxWaitTime - 195) * 1000;
                }
                else
                {
                    // The M2 Gateway has a much shorter timeout than other meters due to
                    // the pass-through mode.  The wait time is 7 seconds, but we send a
                    // new wait request every 4 seconds
                    m_SessionTimer.Interval = (m_MaxWaitTime - 3) * 1000;
                }

				m_SessionTimer.Enabled = true;				
            }			
        }

        /// <summary>
        /// This method should be called immediately before communicating with the meter.
        /// It stops the maintenance timer from firing and prevents the application from
        /// queuing up a wait command during the execution of another meter 
        /// operation
        /// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/20/06 mah 8.00.00 N/A	Created
        /// </remarks>
        private void PauseSessionTimer()
        {
            if (m_SessionTimer != null)
            {
                m_SessionTimer.Enabled = false;
            }			
        }

        #endregion

    }
}

