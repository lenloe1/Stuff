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
//                           Copyright © 2005 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Resources;
using Itron.Metering.Communications;
using Itron.Metering.Utilities;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Itron.Metering.Communications.PSEM
{
	/// <summary>
	/// CANSIL7 supports the ANSI application layer 7 communication with
	/// a device. 
	/// </summary>
	/// <remarks>
	/// CANSIL7 is internal abstract which implies it is not visible 
	/// outside the assembly and must be inherited.
	/// </remarks>
	/// <example>
	/// <code>
	/// internal class C1218L7 : CANSIL7
	/// </code>
	/// </example>
	/// Revision History
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ ---------------------------------------
	/// 08/01/05 bdm 7.13.00 N/A	Created
	internal abstract class CANSIL7
	{
		#region Enumerations
		[Flags]
		//ANSI layer 7 application services
		internal enum ServiceCodes : byte
		{
			AP_CMD_IDENTIFY	=				0x20,
			AP_CMD_TERMINATE =				0x21,
			AP_CMD_DISCONNECT =				0x22,
			AP_PREAD_FULL =					0x30,
			AP_PREAD_INDEX =				0x31,
			AP_PREAD_DEFAULT =				0x3E,
			AP_PREAD_OFFSET =				0x3F,
			AP_PWRITE_FULL =				0x40,
			AP_PWRITE_INDEX =				0x41,
			AP_PWRITE_OFFSET =				0x4F,
			AP_CMD_LOGON =					0x50,
			AP_CMD_SECURITY	=				0x51,
			AP_CMD_LOGOFF =					0x52,
			AP_CMD_AUTHENTICATE =			0x53,
			AP_CMD_NEGOTIATE_NO_BAUD =		0x60,
			AP_CMD_WAIT =					0x70,
			AP_TIMING_SETUP =				0x71,
		}

		//Baud rates for negotiating
        internal enum BaudRates : byte
        {
            BR_300 = 0x01,
            BR_600 = 0x02,
            BR_1200 = 0x03,
            BR_2400 = 0x04,
            BR_4800 = 0x05,
            BR_9600 = 0x06,
            BR_14400 = 0x07,
            BR_19200 = 0x08,
            BR_28800 = 0x09,
            BR_38400 = 0x0A
        }

		#endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ansiL2">A CANSIL2 object for which CANSIL7
        /// can use to send and receive packets.
        /// </param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		public CANSIL7(CANSIL2 ansiL2)
		{
			m_ANSIL2 = ansiL2;
			m_rmStrings = new ResourceManager( RESOURCE_FILE_PROJECT_STRINGS, 
				                               this.GetType().Assembly );
			
			if ( null == m_rmStrings )
			{
				throw( new Exception("Error creating CANSIL7 object.") );
			}
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		~CANSIL7()
		{
			m_rmStrings.ReleaseAllResources();
			m_rmStrings = null;
		}

		/// <summary>
		/// Performs the identification service.
		/// </summary>
		/// <returns>
		/// A PSEMResponse encapsulating the layer 7 response
		/// to the layer 7 request.
		/// </returns>
		/// <example>
		/// <code>
		/// PSEMResponse Result = m_ANSIL7.Identify();
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        public virtual PSEMResponse Identify()
        {
            PSEMResponse Result = PSEMResponse.Err;
            int intIndex = 0;
            m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "PSEM Identify");

            m_abytTxPkt = new byte[1] { (byte)ServiceCodes.AP_CMD_IDENTIFY };
            m_ANSIL2.Send(m_abytTxPkt);
            m_ANSIL2.Receive(out m_abytRxPkt);

            if (0 < m_abytRxPkt.Length)
            {
                intIndex = 0;
                Result = (PSEMResponse)m_abytRxPkt[intIndex++];
                if (PSEMResponse.Ok == Result)
                {
                    //parse it
                    m_bytRefStd = m_abytRxPkt[intIndex++];
                    m_bytVer = m_abytRxPkt[intIndex++];
                    m_bytRev = m_abytRxPkt[intIndex++];
                }
            }

            return Result;
        }

		/// <summary>
		/// Performs the negotiate service.
		/// </summary>
        /// <param name="usPacketSize">Maximum packet size supported, in bytes. 
        /// This value shall not be greater than 8192 bytes.</param>
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
		/// PSEMResponse Result = m_ANSIL7.Identify();
		/// if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Negotiate();
		///	}
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        /// 08/17/06 KRC 7.35.00 N/A    Adding real negotiate support
        /// 06/17/10 AF  2.41.11        Added baud rate 38400 (used only by the M2 Gateway)
        /// 
        public virtual PSEMResponse Negotiate(ushort usPacketSize, byte byNbrPackets, uint uiBaudRate)
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "PSEM Negotiate");

            List<byte> bytBauds = new List<byte>();

            int intIndex = 0;
            int intBaudIndex = 0;

            PSEMResponse Result = PSEMResponse.Err;

            // Build up Baud Rate List
            // Add the usable baud rates in order
            bytBauds.Add((byte)BaudRates.BR_9600);
            bytBauds.Add((byte)BaudRates.BR_14400);
            bytBauds.Add((byte)BaudRates.BR_19200);
            bytBauds.Add((byte)BaudRates.BR_28800);
            bytBauds.Add((byte)BaudRates.BR_38400);

            // Remove any items that are greater than the baud rate requested
            intIndex = bytBauds.IndexOf((byte)TranslateToBaudRates(uiBaudRate));

            if (intIndex + 1 < bytBauds.Count)
            {
                bytBauds.RemoveRange(intIndex + 1, bytBauds.Count - (intIndex + 1));
            }

            // Make sure that we can fit the requested baud rates in the Negotiate request
            if (bytBauds.Count > 11)
            {
                throw new Exception("ANSI protocol does not support more than 11 rates in a Negotiate request.");
            }

            // Create the packet
            intIndex = 0;

            m_abytTxPkt = new byte[4 + bytBauds.Count];
            m_abytTxPkt[intIndex++] = (byte)((byte)(ServiceCodes.AP_CMD_NEGOTIATE_NO_BAUD) + (byte)bytBauds.Count);
            m_abytTxPkt[intIndex++] = (byte)(usPacketSize >> 8); //<packet size>
            m_abytTxPkt[intIndex++] = (byte)(usPacketSize & 0x00FF);
            m_abytTxPkt[intIndex++] = byNbrPackets; //<nbr_packet>

            for (intBaudIndex = 0; intBaudIndex < bytBauds.Count; intBaudIndex++)
            {
                m_abytTxPkt[intIndex++] = bytBauds[intBaudIndex];
            }

            m_ANSIL2.Send(m_abytTxPkt);
            m_ANSIL2.Receive(out m_abytRxPkt);

            // After adding the changes to negotiate the baud rate we noticed that the
            // baud rate would sometimes change before the ACK was sent to the meter. This
            // causes the meter to never change it's baud rate. This Sleep is a bit of a  
            // hack but prevents this from occurring.
            System.Threading.Thread.Sleep(20);

            if (0 < m_abytRxPkt.Length)
            {
                intIndex = 0;
                Result = (PSEMResponse)m_abytRxPkt[intIndex++];

                if (PSEMResponse.Ok == Result)
                {
                    //parse it
                    m_ANSIL2.MaxPacketSize = (ushort)((m_abytRxPkt[intIndex++] << 8) | m_abytRxPkt[intIndex++]);

                    // Supporting the negotiated <nbr_packet> not yet implemented.
                    intIndex++;

                    // Change the Com port to use the new baud rate
                    SetBaudRate((BaudRates)m_abytRxPkt[intIndex]);

                    m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Negotiation Success. Max Packet Size: " + m_ANSIL2.MaxPacketSize.ToString(CultureInfo.InvariantCulture)
                        + " Baud Rate: " + m_ANSIL2.BaudRate.ToString(CultureInfo.InvariantCulture));
                }
            }

            return Result;
        }

		/// <summary>
		/// Performs the logoff service.
		/// </summary>
		/// <returns>
		/// A PSEMResponse encapsulating the layer 7 response
		/// to the layer 7 request.
		/// </returns>
		/// <example>
		/// <code>
		/// PSEMResponse Result = m_ANSIL7.Identify();
		/// if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Negotiate();
		///	}
		///	if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Logon("username");
		///	}
		///	if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Security("userpassword");
		///	}
		///	if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Logoff();
		///	}
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        public virtual PSEMResponse Logoff()
        {
            PSEMResponse Result = PSEMResponse.Err;
            int intIndex = 0;
            m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "PSEM Logoff");

            m_abytTxPkt = new byte[1] { (byte)ServiceCodes.AP_CMD_LOGOFF };
            m_ANSIL2.Send(m_abytTxPkt);
            m_ANSIL2.Receive(out m_abytRxPkt);

            if (0 < m_abytRxPkt.Length)
            {
                intIndex = 0;
                Result = (PSEMResponse)m_abytRxPkt[intIndex++];
            }

            return Result;
        }


		/// <summary>
		/// Performs the logon service.
		/// </summary>
		/// <param name="strUser">
		/// The logon user name.
        /// </param>
        /// <param name="lngUserID">
        /// The user ID
		/// </param>
		/// <remarks>
		/// This routine does not currently support a Canadian logon
		/// due to the implementation of the user id.
		/// </remarks>
		/// <returns>
		/// A PSEMResponse encapsulating the layer 7 response
		/// to the layer 7 request.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the user length exceeds the acceptable length.
		/// </exception>
		/// <example>
		/// <code>
		/// PSEMResponse Result = m_ANSIL7.Identify();
		/// if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Negotiate();
		///	}
		///	if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Logon("username");
		///	}
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        /// 08/17/06 KRC 7.35.00 N/A    Adding real logon support
        public virtual PSEMResponse Logon(string strUser, long lngUserID)
        {
            int intIndex = 0;
            PSEMResponse Result = PSEMResponse.Err;
            byte[] bytUser = new byte[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            string strErrDesc;
            m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "PSEM Logon");

            if (null == strUser)
            {
                strUser = "";
            }

            if (10 < strUser.Length)
            {
                strErrDesc = m_rmStrings.GetString("PARAM_LESS_THAN");
                strErrDesc = string.Format(CultureInfo.CurrentCulture, strErrDesc, bytUser.Length);
                throw (new ArgumentOutOfRangeException(strErrDesc, "user"));
            }

            //build the user bytes
            byte[] bytTmp = StringToByteArray(strUser);

            if (null != bytTmp)
            {
                System.Array.Copy(bytTmp, 0, bytUser, 0, bytTmp.Length);
            }

            //put it all together
            m_abytTxPkt = new byte[13];
            m_abytTxPkt[intIndex++] = (byte)ServiceCodes.AP_CMD_LOGON;
            m_abytTxPkt[intIndex++] = (byte)(lngUserID >> 8); //<userid>
            m_abytTxPkt[intIndex++] = (byte)(lngUserID & 0x00FF);

            Array.Copy(bytUser, 0, m_abytTxPkt, intIndex, bytUser.Length);
            intIndex += bytUser.Length;

            m_ANSIL2.Send(m_abytTxPkt);
            m_ANSIL2.Receive(out m_abytRxPkt);

            if (0 < m_abytRxPkt.Length)
            {
                intIndex = 0;
                Result = (PSEMResponse)m_abytRxPkt[intIndex++];
            }

            return Result;
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
		/// PSEMResponse Result = m_ANSIL7.Identify();
		/// if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Negotiate();
		///	}
		///	if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Logon("username");
		///	}
		///	if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Security("userpassword");
		///	}
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        public virtual PSEMResponse Security(string userPassword)
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "PSEM Security");

            int intIndex = 0;
            byte[] bytUserPassword = new byte[20] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            PSEMResponse Result = PSEMResponse.Err;

            if (null == userPassword)
            {
                userPassword = "";
            }

            if (20 < userPassword.Length)
            {
                string strErrDesc = m_rmStrings.GetString("PARAM_LESS_THAN");
                strErrDesc = string.Format(CultureInfo.CurrentCulture, strErrDesc, bytUserPassword.Length);
                throw (new ArgumentOutOfRangeException(strErrDesc, "userPassword"));
            }

            //build the user bytes
            byte[] bytTmp = StringToByteArray(userPassword);

            if (null != bytTmp)
            {
                System.Array.Copy(bytTmp, 0, bytUserPassword, 0, bytTmp.Length);
            }

            m_abytTxPkt = new byte[21];
            m_abytTxPkt[intIndex++] = (byte)ServiceCodes.AP_CMD_SECURITY;

            Array.Copy(bytUserPassword, 0, m_abytTxPkt, intIndex, bytUserPassword.Length);
            intIndex += bytUserPassword.Length;

            m_ANSIL2.Send(m_abytTxPkt);
            m_ANSIL2.Receive(out m_abytRxPkt);

            if (0 < m_abytRxPkt.Length)
            {
                intIndex = 0;
                Result = (PSEMResponse)m_abytRxPkt[intIndex++];
            }

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
        /// PSEMResponse Result = m_ANSIL7.Identify();
        /// if (PSEMResponse.OK == Result)
        /// {
        ///		Result = m_ANSIL7.Negotiate();
        ///	}
        ///	if (PSEMResponse.OK == Result)
        /// {
        ///		Result = m_ANSIL7.Logon("username");
        ///	}
        ///	if (PSEMResponse.OK == Result)
        /// {
        ///		Result = m_ANSIL7.Security("userpassword");
        ///	}
        /// </code>
        /// </example>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/18/07 RCG 8.10.14 N/A	Created

        public virtual PSEMResponse Security(byte[] byUserPassword)
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "PSEM Security");

            int intIndex = 0;
            PSEMResponse Result = PSEMResponse.Err;

            if (null == byUserPassword)
            {
                throw new ArgumentNullException("byUserPassword");
            }

            if (20 < byUserPassword.Length)
            {
                string strErrDesc = m_rmStrings.GetString("PARAM_LESS_THAN");
                strErrDesc = string.Format(CultureInfo.CurrentCulture, strErrDesc, byUserPassword.Length);
                throw (new ArgumentOutOfRangeException(strErrDesc, "userPassword"));
            }

            m_abytTxPkt = new byte[21];
            m_abytTxPkt[intIndex++] = (byte)ServiceCodes.AP_CMD_SECURITY;

            Array.Copy(byUserPassword, 0, m_abytTxPkt, intIndex, byUserPassword.Length);
            intIndex += byUserPassword.Length;

            m_ANSIL2.Send(m_abytTxPkt);
            m_ANSIL2.Receive(out m_abytRxPkt);

            if (0 < m_abytRxPkt.Length)
            {
                intIndex = 0;
                Result = (PSEMResponse)m_abytRxPkt[intIndex++];
            }

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
		/// PSEMResponse Result = m_ANSIL7.Identify();
		/// if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Negotiate();
		///	}
		///	if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Logon("username");
		///	}
		///	if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Security("userpassword");
		///	}
		///	if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Wait(0xFF);
		///	}
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        public virtual PSEMResponse Wait(byte seconds)
        {
            PSEMResponse Result = PSEMResponse.Err;
            int intIndex = 0;
            m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "PSEM Wait");

            if (0 == seconds)
            {
                seconds = 0xFF;
            }

            m_abytTxPkt = new byte[2];
            m_abytTxPkt[intIndex++] = (byte)ServiceCodes.AP_CMD_WAIT;
            m_abytTxPkt[intIndex++] = seconds;

            m_ANSIL2.Send(m_abytTxPkt);
            m_ANSIL2.Receive(out m_abytRxPkt);

            if (0 < m_abytRxPkt.Length)
            {
                intIndex = 0;
                Result = (PSEMResponse)m_abytRxPkt[intIndex++];
            }

            if (PSEMResponse.Isss == Result)
            {
                //If we got an Isss error then the meter must have timed out
                string strErrDesc = m_rmStrings.GetString("ACKMNT_RSPNS_TMOUT");
                strErrDesc += " " + m_rmStrings.GetString("ISSS");
                TimeOutException TOE = new TimeOutException(strErrDesc);


                //increment the timeout counter
                PSEMCommunicationsStatistics.CommStats.To++;

                if (m_Logger != null)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Session has timed out");
                }

                throw (TOE);
            }

            return Result;
        }
		
		/// <summary>
		/// Performs the terminate service.
		/// </summary>
		/// <returns>
		/// A PSEMResponse encapsulating the layer 7 response
		/// to the layer 7 request.
		/// </returns>
		/// 		/// <example>
		/// <code>
		/// PSEMResponse Result = m_ANSIL7.Identify();
		/// if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Negotiate();
		///	}
		///	if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Logon("username");
		///	}
		///	if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Security("userpassword");
		///	}
		///	if (PSEMResponse.OK == Result)
		/// {
		///		Result = m_ANSIL7.Terminate();
		///	}
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		public virtual PSEMResponse Terminate()
		{
			PSEMResponse Result = PSEMResponse.Err;
			int intIndex = 0;
            m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "PSEM Terminate");

			try
			{
				m_abytTxPkt = new byte[1] {(byte)ServiceCodes.AP_CMD_TERMINATE};
				m_ANSIL2.Send(m_abytTxPkt);
				m_ANSIL2.Receive(out m_abytRxPkt);
	
				if ( 0 < m_abytRxPkt.Length )
				{
					intIndex = 0;
					Result = (PSEMResponse) m_abytRxPkt[intIndex++];
				}
			}
			catch(Exception)
			{
				throw;
			}
            finally
            {
                // Reset the Baud Rate to 9600                
                // Sometimes the port changes the baud rate to early so wait
                Thread.Sleep(100);
                SetBaudRate(BaudRates.BR_9600);
            }
            
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
		/// The table for which to perform a full read.
		/// </param>
		/// <returns>
		/// A PSEMResponse encapsulating the layer 7 response
		/// to the layer 7 request.
		/// </returns>
		/// <example>
		/// <code>
		/// byte[] abytData = null;
		/// PSEMResponse Result = m_ANSIL7.FullRead(5, out abytData); 
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        public virtual PSEMResponse FullRead(ushort table, out byte[] data)
        {
            PSEMResponse Result = PSEMResponse.Err;
            int intIndex = 0;
            int intDataLen = 0;

            m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "PSEM Full Read of Table " + table);
            data = null;

            m_abytTxPkt = new byte[3];
            m_abytTxPkt[intIndex++] = (byte)ServiceCodes.AP_PREAD_FULL;
            m_abytTxPkt[intIndex++] = (byte)(table >> 8);
            m_abytTxPkt[intIndex++] = (byte)(table & 0x00FF);

            m_ANSIL2.Send(m_abytTxPkt);
            m_ANSIL2.Receive(out m_abytRxPkt);

            if (0 < m_abytRxPkt.Length)
            {
                intIndex = 0;
                Result = (PSEMResponse)m_abytRxPkt[intIndex++];

                if (PSEMResponse.Ok == Result)
                {
                    intDataLen = (m_abytRxPkt[intIndex++] << 8) | m_abytRxPkt[intIndex++];

                    //Validate the checksum before returning the data.
                    if (m_abytRxPkt[m_abytRxPkt.Length - 1] != GetChecksum(m_abytRxPkt, intIndex, intDataLen))
                    {
                        Result = PSEMResponse.Err;
                    }
                    else
                    {
                        //Copy the data.
                        data = new byte[intDataLen];
                        Array.Copy(m_abytRxPkt, intIndex, data, 0, data.Length);
                    }
                }
                else if (PSEMResponse.Isss == Result)
                {
                    //If we got an Isss error then the meter must have timed out
                    string strErrDesc = m_rmStrings.GetString("ACKMNT_RSPNS_TMOUT");
                    strErrDesc += " " + m_rmStrings.GetString("ISSS");
                    TimeOutException TOE = new TimeOutException(strErrDesc);

                    //increment the timeout counter
                    PSEMCommunicationsStatistics.CommStats.To++;

                    if (m_Logger != null)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Session has timed out");
                    }

                    throw (TOE);
                }
            }

            return Result;
        }

		
		/// <summary>Performs the offset read service.</summary>
		/// <remarks>
		/// When an error occurs, the byte array of data will be null.
		/// </remarks>
		/// <param name="data">The table's data.</param>
		/// <param name="table">
		/// The table for which to perform a full read.
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
		/// byte[] abytData = null;
		/// PSEMResponse Result = m_ANSIL7.OffsetRead(2049, 4261, 54, out abytData); 
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        public virtual PSEMResponse OffsetRead(ushort table, int offset, ushort count, out byte[] data)
        {
            PSEMResponse Result = PSEMResponse.Err;
            int intIndex = 0;
            int intDataLen = 0;

            m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "PSEM Offset Read of Table " +
                                table + " Offset " + offset + " Size " + count);
            data = null;

            m_abytTxPkt = new byte[8];
            m_abytTxPkt[intIndex++] = (byte)ServiceCodes.AP_PREAD_OFFSET;
            m_abytTxPkt[intIndex++] = (byte)(table >> 8);
            m_abytTxPkt[intIndex++] = (byte)(table & 0x00FF);
            m_abytTxPkt[intIndex++] = (byte)(offset >> 16);
            m_abytTxPkt[intIndex++] = (byte)(offset >> 8);
            m_abytTxPkt[intIndex++] = (byte)(offset & 0xFF);
            m_abytTxPkt[intIndex++] = (byte)(count >> 8);
            m_abytTxPkt[intIndex++] = (byte)(count & 0xFF);

            m_ANSIL2.Send(m_abytTxPkt);
            m_ANSIL2.Receive(out m_abytRxPkt);

            if (0 < m_abytRxPkt.Length)
            {
                intIndex = 0;
                Result = (PSEMResponse)m_abytRxPkt[intIndex++];

                if (PSEMResponse.Ok == Result)
                {
                    intDataLen = (m_abytRxPkt[intIndex++] << 8) | m_abytRxPkt[intIndex++];

                    //Validate the checksum before returning the data.
                    if (m_abytRxPkt[m_abytRxPkt.Length - 1] != GetChecksum(m_abytRxPkt, intIndex, intDataLen))
                    {
                        Result = PSEMResponse.Err;
                    }
                    else
                    {
                        //Copy the data.
                        data = new byte[intDataLen];
                        Array.Copy(m_abytRxPkt, intIndex, data, 0, data.Length);
                    }
                }
                else if (PSEMResponse.Isss == Result)
                {
                    //If we got an Isss error then the meter must have timed out
                    string strErrDesc = m_rmStrings.GetString("ACKMNT_RSPNS_TMOUT");
                    strErrDesc += " " + m_rmStrings.GetString("ISSS");
                    TimeOutException TOE = new TimeOutException(strErrDesc);

                    //increment the timeout counter
                    PSEMCommunicationsStatistics.CommStats.To++;

                    if (m_Logger != null)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Session has timed out");
                    }

                    throw (TOE);
                }
            }

            return Result;
        }

		/// <summary>Performs the index read service.</summary>
		/// <remarks>
		/// When an error occurs, the byte array of data will be null.
		/// </remarks>
		/// <param name="data">The table's data.</param>
		/// <param name="table">
		/// The table for which to perform a full read.
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
		/// byte[] abytData = null;
		/// PSEMResponse Result = m_ANSIL7.IndexRead(2049, 2, 54, out abytData); 
		/// </code>
		/// </example>
		/// <exception cref="NotImplementedException">
		/// IndexRead not yet implemented.
		/// </exception>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		public virtual PSEMResponse IndexRead(ushort table, ushort index, ushort count, out byte[] data)
		{
			data = null;
			throw ( new NotImplementedException() );
		}

		/// <summary>Performs the full write service.</summary>
		/// <param name="data">The data to write to the table.</param>
		/// <param name="table">
		/// The table for which to perform a full write.
		/// </param>
		/// <returns>
		/// A PSEMResponse encapsulating the layer 7 response
		/// to the layer 7 request.
		/// </returns>
		/// <example>
		/// <code>
		/// byte[] abytData = new byte[3]{0x01, 0x02, 0x03};
		/// PSEMResponse Result = m_ANSIL7.FullWrite(2049, abytData); 
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        public virtual PSEMResponse FullWrite(ushort table, byte[] data)
        {
            int intIndex = 0;
            PSEMResponse Result = PSEMResponse.Err;

            if (null == data)
            {
                throw (new ArgumentNullException());
            }

            if (0 == data.Length)
            {
                throw (new ArgumentOutOfRangeException());
            }

            m_abytTxPkt = new byte[6 + data.Length];
            m_abytTxPkt[intIndex++] = (byte)ServiceCodes.AP_PWRITE_FULL;
            m_abytTxPkt[intIndex++] = (byte)(table >> 8);
            m_abytTxPkt[intIndex++] = (byte)(table & 0x00FF);
            m_abytTxPkt[intIndex++] = (byte)(data.Length >> 8);
            m_abytTxPkt[intIndex++] = (byte)(data.Length & 0xFF);

            Array.Copy(data, 0, m_abytTxPkt, intIndex, data.Length);
            intIndex += data.Length;

            m_abytTxPkt[intIndex++] = GetChecksum(data, 0, data.Length);

            m_ANSIL2.Send(m_abytTxPkt);
            m_ANSIL2.Receive(out m_abytRxPkt);

            if (0 < m_abytRxPkt.Length)
            {
                intIndex = 0;
                Result = (PSEMResponse)m_abytRxPkt[intIndex++];
            }

            if (PSEMResponse.Isss == Result)
            {
                //If we got an Isss error then the meter must have timed out
                string strErrDesc = m_rmStrings.GetString("ACKMNT_RSPNS_TMOUT");
                strErrDesc += " " + m_rmStrings.GetString("ISSS");
                TimeOutException TOE = new TimeOutException(strErrDesc);

                //increment the timeout counter
                PSEMCommunicationsStatistics.CommStats.To++;

                if (m_Logger != null)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Session has timed out");
                }

                throw (TOE);
            }

            return Result;
        }

		/// <summary>Performs the index write service.</summary>
		/// <param name="data">The data to write to the table.</param>
		/// <param name="table">
		/// The table for which to perform a index write.
		/// </param>
		/// <param name="index">Index into the table.</param>
		/// <returns>
		/// A PSEMResponse encapsulating the layer 7 response
		/// to the layer 7 request.
		/// </returns>
		/// <example>
		/// <code>
		/// byte[] abytData = new byte[3]{0x01, 0x02, 0x03};
		/// PSEMResponse Result = m_ANSIL7.IndexWrite(2049, 2, abytData); 
		/// </code>
		/// </example>
		/// <exception cref="NotImplementedException">
		/// IndexWrite not yet implemented.
		/// </exception>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		public virtual PSEMResponse IndexWrite(ushort table, ushort index, byte[] data)
		{
			data = null;
			throw ( new NotImplementedException() );
		}

		
		/// <summary>Performs the offset write service.</summary>
		/// <param name="data">The data to write to the table.</param>
		/// <param name="table">
		/// The table for which to perform a offset write.
		/// </param>
		/// <param name="offset">Offset into the table.</param>
		/// <returns>
		/// A PSEMResponse encapsulating the layer 7 response
		/// to the layer 7 request.
		/// </returns>
		/// <example>
		/// <code>
		/// byte[] abytData = new byte[3]{0x01, 0x02, 0x03};
		/// PSEMResponse Result = m_ANSIL7.OffsetWrite(2049, 400, abytData); 
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        public virtual PSEMResponse OffsetWrite(ushort table, int offset, byte[] data)
        {
            //4F 08 00 00 10 A5 00 36
            PSEMResponse Result = PSEMResponse.Err;
            int intIndex = 0;

            m_abytTxPkt = new byte[9 + data.Length];
            m_abytTxPkt[intIndex++] = (byte)ServiceCodes.AP_PWRITE_OFFSET;
            m_abytTxPkt[intIndex++] = (byte)(table >> 8);
            m_abytTxPkt[intIndex++] = (byte)(table & 0x00FF);
            m_abytTxPkt[intIndex++] = (byte)(offset >> 16);
            m_abytTxPkt[intIndex++] = (byte)(offset >> 8);
            m_abytTxPkt[intIndex++] = (byte)(offset & 0xFF);
            m_abytTxPkt[intIndex++] = (byte)(data.Length >> 8);
            m_abytTxPkt[intIndex++] = (byte)(data.Length & 0xFF);

            Array.Copy(data, 0, m_abytTxPkt, intIndex, data.Length);
            intIndex += data.Length;

            m_abytTxPkt[intIndex++] = GetChecksum(data, 0, data.Length);

            m_ANSIL2.Send(m_abytTxPkt);
            m_ANSIL2.Receive(out m_abytRxPkt);

            if (0 < m_abytRxPkt.Length)
            {
                intIndex = 0;
                Result = (PSEMResponse)m_abytRxPkt[intIndex++];
            }

            if (PSEMResponse.Isss == Result)
            {
                //If we got an Isss error then the meter must have timed out
                string strErrDesc = m_rmStrings.GetString("ACKMNT_RSPNS_TMOUT");
                strErrDesc += " " + m_rmStrings.GetString("ISSS");
                TimeOutException TOE = new TimeOutException(strErrDesc);

                //increment the timeout counter
                PSEMCommunicationsStatistics.CommStats.To++;

                if (m_Logger != null)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Session has timed out");
                }

                throw (TOE);
            }

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
            PSEMResponse Result = PSEMResponse.Err;
            int intIndex = 0;

            m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "PSEM TimingSetup");

            m_abytTxPkt = new byte[5];
            m_abytTxPkt[intIndex++] = (byte)ServiceCodes.AP_TIMING_SETUP;
            m_abytTxPkt[intIndex++] = byCTO;
            m_abytTxPkt[intIndex++] = byITO;
            m_abytTxPkt[intIndex++] = byRTO;
            m_abytTxPkt[intIndex++] = byRetries;

            m_ANSIL2.Send(m_abytTxPkt);
            m_ANSIL2.Receive(out m_abytRxPkt);

            if (0 < m_abytRxPkt.Length)
            {
                intIndex = 0;
                Result = (PSEMResponse)m_abytRxPkt[intIndex++];
                if (PSEMResponse.Ok == Result)
                {
                    //Timeouts are all stored in milliseconds
                    m_ANSIL2.m_uintCTO = (uint)(m_abytRxPkt[intIndex++] * 1000);
                    m_ANSIL2.m_uintITO = (uint)(m_abytRxPkt[intIndex++] * 1000);
                    m_ANSIL2.m_uintRTO = (uint)(m_abytRxPkt[intIndex++] * 1000);
                    m_ANSIL2.m_usNumRetries = m_abytRxPkt[intIndex++];
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
		/// Supports retrieving the identification service reference standard.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		public virtual byte Standard
		{
			get
			{
				return m_bytRefStd;
			}
		}

        /// <summary>
        /// Gets the maximum size of a packet.
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/11/08 RCG 1.50.03 N/A	Created

        public virtual ushort MaxPacketSize
        {
            get
            {
                return m_ANSIL2.MaxPacketSize;
            }
        }

        /// <summary>
        /// Gets the current baud rate
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/11/08 RCG 1.50.03 N/A	Created
        // 05/16/16 AF  4.50.268 622562  Added the ability to set the baud rate for times
        //                               that the meter has timed out and we want to return to
        //                               base state for a retry.
        public virtual uint BaudRate
        {
            get
            {
                return m_ANSIL2.BaudRate;
            }
            set
            {
                m_ANSIL2.BaudRate = value;
            }
        }

		/// <summary>
		/// Supports retrieving the identification service revision.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		public virtual byte Revision
		{
			get
			{
				return m_bytRev;
			}
		}

		/// <summary>
		/// Supports retrieving the identifcation service version.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		public virtual byte Version
		{
			get
			{
				return m_bytVer;
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
                return m_ANSIL2.NextPacketToggleSet;
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Supports retrieving and setting the identity byte (reserved byte
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#         Description
        //  -------- --- ------- ------------   -------------------------------------------
        //  03/12/13 AF  2.80.08 TR7578, 7582   Created
        //
        internal virtual byte IdentityByte
        {
            get
            {
                return m_ANSIL2.m_bytReserved;
            }
            set
            {
                m_ANSIL2.m_bytReserved = value;
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
        internal uint CTO
        {
            get
            {
                return m_ANSIL2.m_uintCTO;
            }
            set
            {
                m_ANSIL2.m_uintCTO = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
		/// Converts a string to an ASCII encoded byte array. 
		/// </summary>
		/// <param name="stringToConvert">
		/// The string to convert.
		/// </param>
		/// <remarks>
		/// An empty or null string returns a null array.
		/// </remarks>
		/// <returns>
		/// A byte array containing each character's
		/// ASCII encoded value.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		private byte[] StringToByteArray(string stringToConvert)
		{
			
			byte[] bytTmp = null;
			
			if ( null != stringToConvert )
			{
				if ( 0 < stringToConvert.Length )
				{
					bytTmp = new System.Text.ASCIIEncoding().GetBytes(stringToConvert);
				}
			}

			return bytTmp;
		}


		/// <summary>
		/// Calculates the checksum of an array of bytes.
		/// </summary>
		/// <param name="buffer">
		/// The array of bytes that contains the bytes from which
		/// the checksum is to be calculated.
		/// </param>
		/// <param name="start">
		/// Index into the array of bytes from which
		/// the checksum is to start.
		/// </param>
		/// <param name="count">
		/// Number of bytes to include in the checksum.
		/// </param>
		/// <returns>
		/// A byte containing the checksum
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		private byte GetChecksum(byte[] buffer, int start, int count)
		{
			int intIndex = 0;
			int intCkSum = 0;
    
			for (intIndex = 0; intIndex < count; intIndex++)
				intCkSum += buffer[start + intIndex];

			intCkSum = ~intCkSum + 1;
			intCkSum &= 0x000000FF; //Mask off all but lower 8 bits
    
			return ( (byte) intCkSum);
		}

        /// <summary>
        /// Sets the baud rate of the SerialPort
        /// </summary>
        /// <param name="Rate">Baud rate code indicating the speed to set</param>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/17/06 KRC 7.35.00 N/A	Supporting real negotiate
        private void SetBaudRate(BaudRates Rate)
        {
            switch (Rate)
            {
                case BaudRates.BR_300:
                    {
                        m_ANSIL2.BaudRate = 300;
                        break;
                    }
                case BaudRates.BR_600:
                    {
                        m_ANSIL2.BaudRate = 600;
                        break;
                    }
                case BaudRates.BR_1200:
                    {
                        m_ANSIL2.BaudRate = 1200;
                        break;
                    }
                case BaudRates.BR_2400:
                    {
                        m_ANSIL2.BaudRate = 2400;
                        break;
                    }
                case BaudRates.BR_4800:
                    {
                        m_ANSIL2.BaudRate = 4800;
                        break;
                    }
                case BaudRates.BR_9600:
                    {
                        m_ANSIL2.BaudRate = 9600;
                        break;
                    }
                case BaudRates.BR_14400:
                    {
                        m_ANSIL2.BaudRate = 14400;
                        break;
                    }
                case BaudRates.BR_19200:
                    {
                        m_ANSIL2.BaudRate = 19200;
                        break;
                    }
                case BaudRates.BR_28800:
                    {
                        m_ANSIL2.BaudRate = 28800;
                        break;
                    }
            }
        }

        /// <summary>
        /// Translates the Baud rate specified as an integer into the byte
        /// format required by the ANSI C12.19 standard
        /// </summary>
        /// <param name="uintRate">
        /// The baud rate as an unsigned integer.
        /// </param>
        /// <returns>
        /// BaudRates
        /// </returns>
        /// <example>
        /// <code>
        /// BaudRates NewBaudRate = TranslateToBaudRates(9600);
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/17/06 KRC 7.35.00 N/A	Support real negotiate
        /// 06/17/10 AF  2.41.10        Added 38400 baud
        /// 
        private BaudRates TranslateToBaudRates(uint uintRate)
        {
            BaudRates BaudRate;

            switch (uintRate)
            {
                case 300:
                    {
                        BaudRate = BaudRates.BR_300;
                        break;
                    }
                case 600:
                    {
                        BaudRate = BaudRates.BR_600;
                        break;
                    }
                case 1200:
                    {
                        BaudRate = BaudRates.BR_1200;
                        break;
                    }
                case 2400:
                    {
                        BaudRate = BaudRates.BR_2400;
                        break;
                    }
                case 4800:
                    {
                        BaudRate = BaudRates.BR_4800;
                        break;
                    }
                case 9600:
                    {
                        BaudRate = BaudRates.BR_9600;
                        break;
                    }
                case 14400:
                    {
                        BaudRate = BaudRates.BR_14400;
                        break;
                    }
                case 19200:
                    {
                        BaudRate = BaudRates.BR_19200;
                        break;
                    }
                case 28800:
                    {
                        BaudRate = BaudRates.BR_28800;
                        break;
                    }
                case 38400:
                    {
                        BaudRate = BaudRates.BR_38400;
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Invalid baud rate.", "uintRate");
                    }
            }

            return BaudRate;
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// ANSI layer 2 support.
        /// </summary>
        protected CANSIL2 m_ANSIL2 = null;

        /// <summary>
        /// Byte array of the transmit packet.
        /// </summary>
        protected byte[] m_abytTxPkt = null;

        /// <summary>
        /// Byte array of the packet received.
        /// </summary>
        protected byte[] m_abytRxPkt = null;

        /// <summary>
        /// The identification service reference standard.
        /// </summary>
        protected byte m_bytRefStd = 0x0;

        /// <summary>
        /// The identification service version.
        /// </summary>
        protected byte m_bytVer = 0x0;

        /// <summary>
        /// The identification service revision.
        /// </summary>
        protected byte m_bytRev = 0x0;

        //Private members to support resource strings:
        private static readonly string RESOURCE_FILE_PROJECT_STRINGS = "Itron.Metering.Communications.PSEM.PSEMStrings";
        private System.Resources.ResourceManager m_rmStrings = null;

        private Logger m_Logger = Logger.TheInstance;

        #endregion
    }
}