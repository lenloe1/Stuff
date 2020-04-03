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
//                           Copyright © 2005 - 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Threading;
using Itron.Metering.Communications;
using Itron.Metering.Utilities;

namespace Itron.Metering.Communications.PSEM
{
	#region "NAKException"
	/// <summary>
	/// Exception occurs when a the maximum number of naks has been
	/// received from the device.
	/// </summary>
	/// <example>
	/// <code>
	/// try{...}
	/// catch(NAKException e){...}
	/// </code>
	/// </example>
	/// Revision History
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ ---------------------------------------
	/// 08/01/05 bdm 7.13.00 N/A	Created
	public class NAKException : Exception
	{
		/// <summary>
		/// Constructor to create a NAKException.
		/// </summary>
		/// <example>
		/// <code>
		/// NAKException NE = new NAKException();
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// /// 08/01/05 bdm 7.13.00 N/A	Created
		public NAKException() : base() {}
		
		
		/// <summary>
		/// Constructor to create a NAKException.
		/// </summary>
		/// <param name="description">
		/// Description of the NAKException.
		/// </param>/>
		/// <example>
		/// <code>
		/// NAKException NE = new NAKException("Maximum naks received.");
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		public NAKException(string description) : base(description) {}
		
		
		/// <summary>
		/// Constructor to create a NAKException.
		/// </summary>
		/// <param name="description">
		/// Description of the NAKException.
		/// </param>/>
		/// <param name="systemException">
		/// The System.Exception caught to be nested into
		/// the NAKException exception.
		/// </param>/>		
		/// <example>
		/// <code>
		/// try{...}
		/// catch(Exception e)
		/// {
		///		throw(new NAKException("Maximum naks received.", e));
		///	}
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		public NAKException(string description, 
			                System.Exception systemException ) 
			                : base(description, systemException) {}
		
	}
	#endregion
    
	#region "CANSIL2"
	/// <summary>
	/// CANSIL2 supports the ANSI datalink layer 2 communication with
	/// a device. 
	/// </summary>
	/// <remarks>
	/// CANSIL2 is internal abstract which implies it is not visible 
	/// outside the assembly and must be inherited.
	///  </remarks>
	/// <example>
	/// <code>
	/// internal class C1218L2 : CANSIL2
	/// </code>
	/// </example>
	/// Revision History
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ ---------------------------------------
	/// 08/01/05 bdm 7.13.00 N/A	Created
	internal abstract class CANSIL2
	{	
		/// <summary>
		/// PSEMInfo eunumeration supports PSEM specific constants.
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		internal enum PSEMInfo : uint
		{
			PSEM_ACK = 0x06,
			PSEM_NAK = 0x15,
			PSEM_STP = 0xEE,

			PACKET_OVERHEAD_C12_18 = 8,
			PACKET_POSITION_STP = 0,
			PACKET_POSITION_ADDRESS = 1,
			PACKET_POSITION_CONTROL = 2,
			PACKET_POSITION_SEQNUM = 3,
			PACKET_POSITION_LENGTH_START = 4,
			PACKET_POSITION_DATA_START = 6,
			PACKET_CRC_LENGTH = 2,
			PACKET_CONTROL_MULTIPACKET = 0x80,
			PACKET_CONTROL_FIRSTPACKET = 0x40,
			PACKET_CONTROL_TOGGLE_BIT = 0x20,

			MIN_TIMEOUT = 1,
			MAX_TIMEOUT = 0x7FFE,
			MAX_NAK_COUNT = 3,
			TIMEOUT_CHNL_C12_18 = 6000,
			TIMEOUT_CHNL_C12_21 = 30000,
			TIMEOUT_IC_C12_18 = 500,
			TIMEOUT_IC_C12_21 = 1000,
			TIMEOUT_ACK_RSPNS_C12_18 = 2000, 
			TIMEOUT_RCV_RSPNS_C12_18 = 6000,
			TIMEOUT_RESPONSE_C12_21 = 4000,
			DEFAULT_PACKET_SIZE_C12_18 = 64,
			INBOUND_ADDRESS = 0xFF,
			STANDARD_RETRY_ATTEMPTS = 3,
			NONSTANDARD_SENTINEL_RETRY_ATTEMPTS = 2,
            DEFAULT_BAUD_RATE = 9600,
		}

		//Communication object that does the 'real' transmit/receive.
		protected ICommunications m_Comm;

		#region "PSEM protected members default to C12.18"
		/// <summary>
		/// Inter-character timeout.
		/// </summary>
		internal uint m_uintITO   = (uint) PSEMInfo.TIMEOUT_IC_C12_18;
		
		/// <summary>
		/// Reasponse timeout.
		/// </summary>
		internal uint m_uintRTO   = (uint) PSEMInfo.TIMEOUT_ACK_RSPNS_C12_18;
		
		/// <summary>
		/// Channel Traffic timeout.
		/// </summary>
		internal uint m_uintCTO   = (uint) PSEMInfo.TIMEOUT_RCV_RSPNS_C12_18;
		
		/// <summary>
		/// Maximum packet size allowed. Typically a result of a layer 7 negotiate.
		/// </summary>
		protected ushort m_usMaxPktSize  = (ushort) PSEMInfo.DEFAULT_PACKET_SIZE_C12_18;

        /// <summary>
        /// Baud Rate to use
        /// </summary>
        protected uint m_uintBaudRate = (uint)PSEMInfo.DEFAULT_BAUD_RATE;

		/// <summary>
		/// Number of transmit (send) retries to attempt.
		/// </summary>
		internal ushort m_usNumRetries  = (ushort) PSEMInfo.STANDARD_RETRY_ATTEMPTS;
		
		/// <summary>
		/// Number of datalink (ANSI layer 2) bytes of overhead.
		/// </summary>
		protected ushort m_usPktOverhead = (ushort) PSEMInfo.PACKET_OVERHEAD_C12_18;

		/// <summary>
		/// Datalink (ANSI L2) reserved byte.
		/// </summary>
		internal byte m_bytReserved = 0x0;

        /// <summary>
        /// Comm Logger
        /// </summary>
        protected Itron.Metering.Utilities.Logger m_Logger;

		#endregion
		
		#region "Toggle/CRC protected members supporting duplicate packet tests"
		/// <summary>
		/// State of toggle for the next tx packet.
		/// </summary>
		protected byte m_bytTxToggle;  
		
		/// <summary>
		/// State of toggle for the previously rx packet used for testing
		/// duplicate packets.
		/// </summary>
		protected byte m_bytRxToggle;  
		
		/// <summary>
		/// CRC value of the previously rx packet used for testing
		/// duplicate packets.
		/// </summary>
		protected ushort  m_usCRC;        
		#endregion

		#region "Protected members supporting rx/tx packets and L2 data"

		/// <summary>
		/// The complete datalink (ANSI layer 2) tx packet.
		/// </summary>
		protected byte[] m_abytTxPkt = null;  
		
		/// <summary>
		/// The datalink (ANSI layer 2) tx packet data portion.
		/// </summary>
		protected byte[] m_abytTxL2Data = null;  
		
		// /// <summary>
		// /// The complete datalink (ANSI layer 2) rx packet.
		// /// </summary>
		// protected byte[] m_abytRxPkt = null;  
		
		/// <summary>
		/// The datalink (ANSI layer 2) rx packet data portion.
		/// </summary>
		protected byte[] m_abytRxL2Data = null; 
		#endregion
        
		#region "Private members supporting timeouts and responses"
		private bool m_blnAckNakTmout = false;
		private bool m_blnAckNakRcvd  = false;
		private bool m_blnRxDataTmout = false;
		private bool m_blnRxDataRcvd  = false;
		#endregion

		#region "Private members supporting strings"
		private System.Resources.ResourceManager m_rmStrings = null;
		private static readonly string RESOURCE_FILE_PROJECT_STRINGS = "Itron.Metering.Communications.PSEM.PSEMStrings";
		#endregion


		/// <summary>
		/// Constructor.  
		/// </summary>
		/// <param name="comm">
		/// The communication object supporting communication over the 
		/// physical layer. 
		/// </param>
		/// <exception cref="System.Exception">
		/// Thrown when the CANSIL2 fails to instantiate correctly.
		/// </exception>
		/// <example>
		/// <code>
		/// public C1218L2(Communication comm):base(comm){}
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		public CANSIL2(ICommunications comm)
		{			
			m_Comm = comm;
			m_bytTxToggle = 1;     //First packet sent has toggle bit set
			m_bytRxToggle = 0xFF;  //Toggle bit of first rx packet is unknown
			m_usCRC = 0xFFFF;      //CRC of first packet is unknown;

            m_Logger = Itron.Metering.Utilities.Logger.TheInstance;

			ResizeBuffers();

			m_rmStrings = new ResourceManager( RESOURCE_FILE_PROJECT_STRINGS, 
				this.GetType().Assembly );

			if ( null == m_rmStrings )
			{
				throw( new Exception("Error creating CANSIL2 object.") );
			}			
		}


		/// <summary>
		/// Destructor to release all resources.  
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		~CANSIL2()
		{
			m_rmStrings.ReleaseAllResources();
			m_rmStrings = null;
		}

		/// <summary>
		/// Manages sending the ansiL7Pkt parameter according to the 
		/// ANSI layer 2 protocol.
		/// </summary>
		/// <param name="ansiL7Pkt">
		/// The complete ANSI layer 7 packet to send.
		/// </param>
		/// <exception cref="TimeOutException">
		/// Thrown when a communication timeout occurs with a device.
		/// </exception>
		/// <exception cref="NAKException">
		/// Thrown when the maximum number of naks have been received.
		/// </exception>
		/// <exception cref="CommPortException">
		/// Thrown when a port failure occurs.
		/// </exception>
		/// <example>
		/// <code>
		/// byte[] txPacket = new byte[3]{0x30, 0x00, 0x05};
		/// byte[] rxPacket = null;
		/// m_ANSIL2.Send(txPacket);
		/// m_ANSIL2.Receive(out rxPacket);
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        public void Send(byte[] ansiL7Pkt)
        {
            bool blnFirstPacket;
            bool blnMultiPacket;
            byte ucPacketCount;
            byte ucSeqNum;

            //ansiL7Pkt represents the layer 2 data
            m_abytTxL2Data = new byte[ansiL7Pkt.Length];
            Array.Copy(ansiL7Pkt,
                       0,
                       m_abytTxL2Data,
                       0,
                       m_abytTxL2Data.Length);

            //This routine manages the packet count (a.k.a. SeqNum), 
            //first packet flag and multi packet flag.
            ucPacketCount = (byte)(m_abytTxL2Data.Length /
                                  (m_usMaxPktSize - m_usPktOverhead));
            if ((ucPacketCount * (m_usMaxPktSize - m_usPktOverhead)) != m_abytTxL2Data.Length)
            {
                ucPacketCount++;
            }

            ucSeqNum = ucPacketCount;
            blnMultiPacket = (ucPacketCount > 1);

            //First packet is true only when there are multiple packets.
            blnFirstPacket = blnMultiPacket;

            do
            {
                --ucSeqNum;

                //BuildPacket will create the full layer 2 packet, 
                //possibly segmented
                BuildPacket(ucSeqNum, blnFirstPacket, blnMultiPacket);
                blnFirstPacket = false;
                SendPacket();

                //Switch the toggle bit.
                if (0x01 == m_bytTxToggle)
                {
                    m_bytTxToggle = 0x00;
                }
                else
                {
                    m_bytTxToggle = 0x01;
                }

            } while (0 != ucSeqNum);
        }

        /// <summary>
        /// Manages receiving the ansiL2Pkt packet, parsing according to the 
        /// ANSI layer 2 protocol and returning the ANSI layer 7 packet.
        /// </summary>
        /// <param name="ansiL7Pkt">
        /// The complete ANSI layer 7 packet received from the device.
        /// </param>
        /// <exception cref="TimeOutException">
        /// Thrown when a communication timeout occurs with a device.
        /// </exception>
        /// <exception cref="NAKException">
        /// Thrown when the maximum number of naks have been received.
        /// </exception>
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// byte[] txPacket = new byte[3]{0x30, 0x00, 0x05};
        /// byte[] rxPacket = null;
        /// m_ANSIL2.Send(txPacket);
        /// m_ANSIL2.Receive(out rxPacket);
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        /// 07/06/06 mrj 7.30.00        Fixed timing issues
        /// 08/03/06 rcg 7.35.00        Fixed several issues related to using .NET
        ///                             2.0 SerialPort object
        /// 09/26/14 AF  4.00.62 523633 Fixed a corner case where the timer expired but 
        ///                             we received the whole packet. In that case we can
        ///                             ignore the timeout
        ///  08/30/16 jrf 4.70.15 No WR Quieting compiler warning and disposing of tmrRxDataRcvd in a finally block.
        public void Receive(out byte[] ansiL7Pkt)
        {
            //Assumption: This method is called only after receiving
            //an ACK to the request stored in m_abytTxPkt AND the request's
            //response includes data, such as when a table read is done.

            ansiL7Pkt = null;
            byte[] inputBuffer = null;
            byte[] inputPacket = null;
            uint iBytesToRead = 0;
            int iBytesRead = 0;
            int iTotalBytesRead = 0;
            bool bDataLengthFound = false;


            //m_abytRxPkt = null;
            m_abytRxL2Data = null;

            //Create a receive data timer.
            TimerCallback timerDelegate = null;
            Timer tmrRxDataRcvd = null;

            try
            {

                //Create a receive data timer.
                timerDelegate = new TimerCallback(RxDataTimout_Tick);
                tmrRxDataRcvd = new Timer(timerDelegate,
                    null,
                    Timeout.Infinite,
                    Timeout.Infinite);
                tmrRxDataRcvd.Change(m_uintCTO, Timeout.Infinite);

                m_blnRxDataTmout = false;
                m_blnRxDataRcvd = false;

                //Stop reading after a response timeout. The timer event will 
                //set m_blnRxDataTmout.  
                //Stop reading after the full layer 2 data (layer 7 packet) 
                //has been received.  ParseRxBuffer() will set m_blnRxDataRcvd.
                while ((false == m_blnRxDataTmout) &&
                        (false == m_blnRxDataRcvd))
                {
                    // First we need to read one byte at a time until a start of packet is found

                    if (iTotalBytesRead == 0)
                    {
                        // Set the inputBuffer to the size of the header
                        inputBuffer = new byte[(uint)PSEMInfo.PACKET_POSITION_DATA_START];

                        iBytesRead = m_Comm.Read(1, 0);

                        if (iBytesRead > 0)
                        {
                            //Something was read, clear the timer
                            tmrRxDataRcvd.Change(Timeout.Infinite, Timeout.Infinite);

                            // We always want to start this copy at the start of the inputBuffer 
                            // so that any garbage is always overwritten
                            Array.Copy(m_Comm.Input, 0, inputBuffer, 0, iBytesRead);
                            if (inputBuffer[0] == (byte)PSEMInfo.PSEM_STP)
                            {
                                // We have found the start of packet
                                iTotalBytesRead += iBytesRead;
                            }

                            //Set the timer again and Read() for more packets
                            tmrRxDataRcvd.Change(Timeout.Infinite, Timeout.Infinite);
                            tmrRxDataRcvd.Change(m_uintCTO, Timeout.Infinite);
                        }
                    }

                    //Read the rest of the packet header, up to the data, to 
                    //determine the packet length
                    if (iTotalBytesRead > 0)
                    {
                        // Now check to see if we have the whole packet header
                        if (iTotalBytesRead < (int)PSEMInfo.PACKET_POSITION_DATA_START)
                        {
                            // We don't have the header yet so try to read more data                            
                            iBytesRead = m_Comm.Read((uint)PSEMInfo.PACKET_POSITION_DATA_START - (uint)iTotalBytesRead, 50);


                            if (iBytesRead > 0)
                            {
                                //Something was read, clear the timer
                                tmrRxDataRcvd.Change(Timeout.Infinite, Timeout.Infinite);

                                // More data was read so copy it to the end of the  inputBuffer
                                Array.Copy(m_Comm.Input, 0, inputBuffer, iTotalBytesRead, iBytesRead);
                                iTotalBytesRead += iBytesRead;

                                //Set the timer again and Read() for more packets
                                tmrRxDataRcvd.Change(Timeout.Infinite, Timeout.Infinite);
                                tmrRxDataRcvd.Change(m_uintCTO, Timeout.Infinite);
                            }
                        }

                        if (iTotalBytesRead >= (int)PSEMInfo.PACKET_POSITION_DATA_START)
                        {
                            if (bDataLengthFound == false)
                            {
                                //Figure out how many more bytes are to be read						
                                iBytesToRead = (uint)((inputBuffer[4] << 8) | inputBuffer[5]);
                                iBytesToRead = iBytesToRead + (uint)PSEMInfo.PACKET_CRC_LENGTH;  //add crc

                                //We now know how big this packet is going to be so
                                //create the packet
                                inputPacket = new byte[iTotalBytesRead + iBytesToRead];
                                Array.Copy(inputBuffer, 0, inputPacket, 0, iTotalBytesRead);

                                bDataLengthFound = true;
                            }

                            if (iBytesToRead > 0)
                            {
                                iBytesRead = m_Comm.Read(iBytesToRead, 50);

                                if (iBytesRead > 0)
                                {
                                    //Something was read, clear the timer
                                    tmrRxDataRcvd.Change(Timeout.Infinite, Timeout.Infinite);

                                    //Add more bytes to the packet
                                    inputBuffer = new byte[iBytesRead];
                                    Array.Copy(m_Comm.Input, 0, inputBuffer, 0, iBytesRead);
                                    Array.Copy(inputBuffer, 0, inputPacket, iTotalBytesRead, inputBuffer.Length);

                                    //Setup for the next read
                                    iTotalBytesRead = iTotalBytesRead + iBytesRead;
                                    iBytesToRead = (uint)(iBytesToRead - iBytesRead);

                                    if (0 < iBytesToRead)
                                    {
                                        //Set the timer again and Read() for more packets
                                        tmrRxDataRcvd.Change(Timeout.Infinite, Timeout.Infinite);
                                        tmrRxDataRcvd.Change(m_uintCTO, Timeout.Infinite);
                                    }
                                }
                            }

                            if (iBytesToRead == 0)
                            {
                                // In case we have a multi packet read we need to reset for a new packet
                                iTotalBytesRead = 0;
                                bDataLengthFound = false;

                                //Parse the packet(s) received in the buffer
                                ParseRxBuffer(inputPacket);

                                //Does m_abytRxL2Data have a complete layer 7 response
                                //packet?
                                if (true == m_blnRxDataRcvd)
                                {
                                    ansiL7Pkt = new byte[m_abytRxL2Data.Length];
                                    Array.Copy(m_abytRxL2Data, 0, ansiL7Pkt, 0, ansiL7Pkt.Length);

                                    //Send the first byte of the packet containing the
                                    //communication result to the singleton to 
                                    //increment the correct counter
                                    PSEMCommunicationsStatistics.CommStats.Increment((int)ansiL7Pkt[0]);
                                }
                                else
                                {
                                    //Set the timer again and Read() for more packets
                                    tmrRxDataRcvd.Change(Timeout.Infinite, Timeout.Infinite);
                                    tmrRxDataRcvd.Change(m_uintCTO, Timeout.Infinite);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (null != tmrRxDataRcvd)
                {
                    tmrRxDataRcvd.Dispose();
                }
            }

            //Why did we stop?
            //If the timer went off but the packet is complete, we should keep going
            if ((true == m_blnRxDataTmout) && (false == m_blnRxDataRcvd))
            {
                string strErrDesc = m_rmStrings.GetString("RX_RSPNS_TMOUT");
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
		
		/// <summary>
		/// Maximum packet size that the datalink (layer 2) can
		/// support.  Typically determined by using layer 7
		/// negotiate service.
		/// </summary>
		/// <exception cref="ArgumentException">
		/// Thrown when the maximum packet size being assigned
		/// is not supported.
		/// </exception>
		/// <example>
		/// <code>
		/// m_ANSIL2.MaxPacketSize = 128;
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		public ushort MaxPacketSize
		{
			get
			{
				return m_usMaxPktSize;
			}
			set
			{
				if ( (ushort)PSEMInfo.DEFAULT_PACKET_SIZE_C12_18 <= value )
				{
					m_usMaxPktSize = value;
					ResizeBuffers();
				}
				else
				{
					string strErrDesc = m_rmStrings.GetString("PARAM_GRTR_THAN");
					strErrDesc = string.Format(CultureInfo.CurrentCulture, strErrDesc, 
						                       PSEMInfo.DEFAULT_PACKET_SIZE_C12_18.ToString());
					throw( new ArgumentException(strErrDesc, "MaxPacketSize"));
				}
			}
		}

        /// <summary>
        /// Maximum packet size that the datalink (layer 2) can
        /// support.  Typically determined by using layer 7
        /// negotiate service.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when the maximum packet size being assigned
        /// is not supported.
        /// </exception>
        /// <example>
        /// <code>
        /// m_ANSIL2.MaxPacketSize = 128;
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/17/06 KRC 7.35.00 N/A	Adding real negotiate support
        public uint BaudRate
        {
            get
            {
                return m_uintBaudRate;
            }
            set
            {
                m_uintBaudRate = value;
                m_Comm.BaudRate = m_uintBaudRate;
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
                return (m_bytTxToggle != 0);
            }
        }

		/// <summary>
		/// Builds the complete datalink (ANSI layer 2) packet.
		/// </summary>
		/// <param name="sequenceNumber">
		/// Number that is decremented by one for each new packet sent. 
		/// The first packet in a multiple packet transmission shall have
		/// a sequence number equal to the total number of packets minus one.
		/// A value of zero in this field indicates that this packet is 
		/// the last packet of a multiple packet transmission.  
		/// If only one packet in a transmission, this field shall be set 
		/// to zero.
		/// </param>
		/// <param name="firstPacket">
		/// True if this packet is the first packet of a multiple packet
		/// transmission.
		/// </param>
		/// <param name="multiPacket">
		/// True if this packet is part of a multiple packet transmission.
		/// </param>
		/// <exception cref="System.Exception">
		/// Thrown when a system exception occurs.
		/// </exception>
		/// <example>
		/// <code>
		/// byte[] abytL7Pkt = new byte[3]{0x30, 0x00, 0x05};
		/// byte   bytSeqNum = 0x00;
		/// m_abytTxL2Data = new byte[abytL7.Length];
		/// bool blnFirstPacket = false;
		/// bool blnMultiPacket = false;
		/// Array.Copy(abytL7Pkt, 0, m_abytTxL2Data, 0, m_abytTxL2Data.Length);
		/// BuildPacket(bytSeqNum, blnFirstPacket, blnMultiPacket);
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		protected abstract void BuildPacket( 
			byte sequenceNumber,
			bool firstPacket, 
			bool multiPacket);



		/// <summary>
		/// Calculates the receive buffer CRC.	
		/// </summary>
		/// <param name="buffer">Array of bytes from which the
		/// CRC will be calculated.</param>
		/// <param name="index">Index into the array from which
		/// the CRC calculation will start. </param>
		/// <param name="count">The number of bytes to be included
		/// in the CRC calculation.</param>
		/// <returns>
		/// An unsigned short containing the value of the CRC.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		protected ushort CalcCRC( byte[] buffer, ushort index, ushort count)
		{
			ushort usCRC = 0;
			usCRC = (ushort)((~buffer[index + 1] << 8) | 
				             (~buffer[index] & 0xFF));
			index += 2;
			count -= 2;

			while ( 0 < count )
			{
				usCRC = CalcCRC16(buffer[index++], usCRC);
				count--;
			}

			usCRC = CalcCRC16(0x00, usCRC);
			usCRC = CalcCRC16(0x00, usCRC);
			usCRC = (ushort)((~usCRC));
			usCRC = (ushort)((usCRC >> 8 | usCRC << 8));

			return usCRC;
		}

		
		/// <summary>
		/// Assists calculating the CRC.
		/// </summary>
		/// <param name="octet"></param>
		/// <param name="crc"></param>
		/// <returns>
		/// An unsigned short containing the
		/// calcualted crc 16.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		static protected ushort CalcCRC16( byte octet, ushort crc )
		{
			int i;

			for (i = 8; i>0; i--)
			{
				if( 0 != (crc & 0x0001) )
				{
					crc >>= 1;
					if( 0 != (octet & 0x01) )
					{
						crc |= 0x8000;
					}
					crc = (ushort)((crc ^ 0x8408));  // 0x1021 inverted = 1000 0100 0000 0001
					octet >>= 1;
				}
				else
				{
					crc >>= 1;
					if( 0 != (octet & 0x01) )
					{
						crc |= 0x8000;
					}
					octet >>= 1;
				}
			}
		   
			return crc;
		}


		/// <summary>
		/// Writes an unsigned short value into a buffer as two separate bytes.
		/// </summary>
		/// <param name="buffer">Reference to a byte array.</param>
		/// <param name="index">Index into the buffer where the 
		/// two bytes of data should be stored.</param>
		/// <param name="value">The unsigned short representing the value
		/// to be written into the buffer.</param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		protected void Write16Bits( ref byte[] buffer, int index, ushort value)
		{
			buffer[index] = (byte)((value & 0xff00) >> 8);
			buffer[index + 1] = (byte)(value & 0xff);
		}

        /// <summary>
        /// Manages (re)sending a packet and the expected response according
        /// to the datalink layer 2 defined by the ANSI specifications.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version     Issue# Description
        //  -------- --- ----------- ------ ---------------------------------------------
        //  08/01/05 bdm 7.13.00     N/A	Created
        //	04/10/08 mrj 10.00.00	 	    Fixed issue with duplicate packets	
        //  06/26/09 jrf 2.20.09     130051 We are backing out of the change for duplicate packets
        //                                  to address communication timeouts we are now seeing.
        //  10/19/10 AF  2.45.06     161794 Added code to deal with garbage coming in
        //  11/30/10 RCG 2.45.13     160529 Added code to handle non duplicate packets without an ack
        //  04/28/11 jrf 2.50.37            Modfied code to better handle when we recieve a packet without
        //                                  an ACK.
        //  05/06/11 jrf 2.50.42            Correcting CRC offsets when trying to determine if an unexpectedly 
        //                                  received packet is a duplicate.
        //  07/01/11 jrf 2.51.18     176515 Adding code to remove garbage bytes from the comm port, when the 
        //                                  beginning of a response packet is lost or not recieved, before resending packet.
        //  07/13/11 AF  2.51.25            Added debugging code to display the value of a byte we think is garbage
        //  08/30/16 jrf 4.70.15     No WR  Quieting compiler warning and disposing of tmrAckNakRcvd in a finally block.
        //  06/09/17 DLG 1.2017.6.25        Suppressed Globalization warnings since this is related to the logger.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString(System.String)")]
        private void SendPacket()
        {
            byte bytRxByte;
            ushort usNumRetries = (ushort)(m_usNumRetries + 1);
            ushort usNakCount = 0;
            bool blnAckRcvd = false;
            byte[] bytRxBuffer;
            string strErrDesc;
            bool bInvalidCharacter = false;
            TimerCallback timerDelegate = null;
            Timer tmrAckNakRcvd = null;

            try
            {
                //Create the a response timeout timer
                timerDelegate = new TimerCallback(AckNakRcvdTmout);
                tmrAckNakRcvd = new Timer(timerDelegate,
                                                null,
                                                Timeout.Infinite,
                                                Timeout.Infinite);

                //Exceptions are most likely due to the communication port or bad
                //memory management.  Let the client handle them.

                // This loop is repeatedly send the current packet until we
                // have the appropriate ACK, NAK or ran out of retries.

                //Stop sending when an ACK has been received.
                //Stop sending when 3 NAKs have been received
                //Stop sending after number of retries

                //Note: blnAckRcvd indicates an Ack was received
                //      m_blnAckNakRcvd indicates an Ack or Nak was received.
                while ((false == blnAckRcvd) &&
                        ((ushort)PSEMInfo.MAX_NAK_COUNT > usNakCount) &&
                        (0 < usNumRetries))
                {
                    m_Comm.Send(m_abytTxPkt);
                    usNumRetries--;


                    m_blnAckNakTmout = false;
                    m_blnAckNakRcvd = false;
                    bInvalidCharacter = false;

                    //Set the acknowledgement timer
                    tmrAckNakRcvd.Change(m_uintRTO, Timeout.Infinite);

                    //Stop reading after a response timeout.
                    //Stop reading after a response has been received.
                    while ((false == m_blnAckNakTmout) &&
                            (false == m_blnAckNakRcvd) &&
                            (false == bInvalidCharacter))
                    {
                        if (1 == m_Comm.Read(1, 0))
                        {
                            //ACK, NAK, or garbage
                            bytRxByte = m_Comm.Input[0];

                            switch (bytRxByte)
                            {
                                case (byte)PSEMInfo.PSEM_ACK:
                                    {
                                        //Ack received, clear the timer
                                        tmrAckNakRcvd.Change(Timeout.Infinite,
                                                             Timeout.Infinite);
                                        m_blnAckNakRcvd = true;
                                        blnAckRcvd = true;

                                        PSEMCommunicationsStatistics.CommStats.Ack++;
                                        break;
                                    }
                                case (byte)PSEMInfo.PSEM_NAK:
                                    {
                                        //Nak received, clear the timer
                                        tmrAckNakRcvd.Change(Timeout.Infinite,
                                                             Timeout.Infinite);
                                        m_blnAckNakRcvd = true;
                                        usNakCount++;

                                        PSEMCommunicationsStatistics.CommStats.Nak++;
                                        break;
                                    }
                                default:
                                    {
                                        if ((byte)PSEMInfo.PSEM_STP == bytRxByte)
                                        {
                                            byte byCurrentToggle = 0xFF;
                                            ushort usCurrentCRC = 0;
                                            ushort usDataLength = 0;


                                            //Start of packet received.  That's 
                                            //unexpected! A packet sent to the 
                                            //device requires the device to 
                                            //ack/nak. Apparently, packets have 
                                            //been lost!

                                            //NEVER assume the packet received is 
                                            //the response to the packet just sent.  
                                            //If our Ack to the previous response 
                                            //was lost, the packet received is 
                                            //likely a duplicate response to the
                                            //previous packet sent. 

                                            //Clean up. Clear the receive buffer 
                                            //but ignore the packets received.  
                                            //Send an ack to let the device 
                                            //believe all is well and resend
                                            //the request.																				

                                            //Changing the bytes to read to the maximum possible 
                                            //to make sure we get all of the packet.
                                            if (0 < m_Comm.Read((uint)(MaxPacketSize - 1), 100))
                                            {
                                                //Clear the buffer's bytes received
                                                //and send an ack.
                                                bytRxBuffer = new byte[m_Comm.InputLen];
                                                Array.Copy(m_Comm.Input,
                                                           0,
                                                           bytRxBuffer,
                                                           0,
                                                           bytRxBuffer.Length);

                                                if (bytRxBuffer.Length > 1)
                                                {
                                                    byCurrentToggle = ((byte)PSEMInfo.PACKET_CONTROL_TOGGLE_BIT == (bytRxBuffer[1] & (byte)PSEMInfo.PACKET_CONTROL_TOGGLE_BIT)) ? (byte)0x01 : (byte)0x00;

                                                    if (bytRxBuffer.Length > 4)
                                                    {
                                                        usDataLength = (ushort)((bytRxBuffer[3] << 8) | bytRxBuffer[4]);

                                                        if (bytRxBuffer.Length > usDataLength + 5)
                                                        {
                                                            //0xEE start of packet has been read so buffer should consist of 
                                                            //Reserved Byte, Control Byte, Sequence Number Byte, 2 Length of Data Bytes, Data Bytes and CRC
                                                            //So offset to get to first byte of CRC would be 5 + usDataLength
                                                            usCurrentCRC = (ushort)((bytRxBuffer[usDataLength + 5] << 8) |
                                                                                     bytRxBuffer[usDataLength + 6]);
                                                        }
                                                    }
                                                }
                                            }

                                            if (m_bytRxToggle == byCurrentToggle && m_usCRC == usCurrentCRC)
                                            {
                                                // We have a duplicate packet so send an ack
                                                SendAck();

                                                m_Logger.WriteLine(Itron.Metering.Utilities.Logger.LoggingLevel.Detailed, "Duplicate packet received");
                                            }
                                            else
                                            {
                                                // We got a response without the ack. Do nothing and see if the problem corrects itself.
                                                m_Logger.WriteLine(Itron.Metering.Utilities.Logger.LoggingLevel.Detailed, "Unexpected packet received. Packet has been ignored.");
                                            }

                                            //Data received so reset the timer
                                            tmrAckNakRcvd.Change(Timeout.Infinite,
                                                                 Timeout.Infinite);
                                            tmrAckNakRcvd.Change(m_uintRTO,
                                                                 Timeout.Infinite);
                                        }
                                        else
                                        {
                                            // We received garbage.  We should break out of the inner loop and resend
                                            bInvalidCharacter = true;

                                            // Clear the timer
                                            tmrAckNakRcvd.Change(Timeout.Infinite,
                                                                 Timeout.Infinite);
                                            m_Logger.WriteLine(Itron.Metering.Utilities.Logger.LoggingLevel.Detailed, "Received garbage. (" + bytRxByte.ToString("X2") + ") Resending packet.");

                                            //try to clear out any data on the line.
                                            m_Comm.Read(MaxPacketSize, 100);
                                        }

                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (null != tmrAckNakRcvd)
                {
                    tmrAckNakRcvd.Dispose();
                }
            }

            //Throw exception if an Ack was not received. 
            if (false == blnAckRcvd)
            {
                if ((ushort)PSEMInfo.MAX_NAK_COUNT <= usNakCount)
                {
                    //Too many naks!
                    strErrDesc = m_rmStrings.GetString("NAK_MAX_REACHED");
                    NAKException NE = new NAKException(strErrDesc);
                    throw (NE);
                }

                if ((false == m_blnAckNakRcvd) ||
                     (true == m_blnAckNakTmout))
                {
                    //Either test indicates an ack/nak response was not
                    //received. No recovering from a timeout.
                    strErrDesc = m_rmStrings.GetString("ACKMNT_RSPNS_TMOUT");
                    TimeOutException TOE = new TimeOutException(strErrDesc);

                    PSEMCommunicationsStatistics.CommStats.To++;

                    if (m_Logger != null)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Session has timed out");
                    }

                    throw (TOE);
                }
                else
                {
                    //In this case we did not get an ACK and we did not timeout so we
                    //must have recieved a NAK on the last packet						
                    NAKException NE = new NAKException("NAK received on last packet.");
                    throw (NE);
                }
            }
        }

		/// <summary>
		/// Sends an ack over the communication port.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        private void SendAck()
        {
            PSEMCommunicationsStatistics.CommStats.AckSent++;
            byte[] byt = new byte[1] { (byte)PSEMInfo.PSEM_ACK };
            m_Comm.Send(byt);
        }

		/// <summary>
		/// Sends a nak over the communication port.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        private void SendNak()
        {
            PSEMCommunicationsStatistics.CommStats.NakSent++;
            byte[] byt = new byte[1] { (byte)PSEMInfo.PSEM_NAK };
            m_Comm.Send(byt);
        }

		/// <summary>
		/// Parses the ANSI layer 2 packet received and
		/// stores the data (layer 7 packet) in m_abytRxL2Data.
		/// </summary>
		/// <remarks>
		/// Sentinel firmware version 2.067 issue: After sending a multiple
		/// packet write request, the response has the multi-packet control
		/// bit set.  
		/// </remarks>
		/// <param name="buffer">The array of bytes containing
		/// one or more datalink (ANSI layer 2) packets.
		/// </param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        private void ParseRxBuffer(byte[] buffer)
        {
            //Process the buffer which may contain multiple
            //PSEM packets.

            int intIndex = 0;
            bool blnMultiPkt = false;
            bool blnFirstPkt = false;
            byte bytToggle = 0;
            byte bytSeqNum = 0x0;
            int intDataLen = 0;
            ushort usCRCStartIndex = 0;
            ushort usPktCRC = 0;
            ushort usCRC = 0;

            //When all layer 2 <data> has been retrieved 
            //(multi-packets reassembled if need be), 
            //then the response is stored in m_abytRxL2Data 
            //and considered received.
            m_blnRxDataRcvd = false;

            intIndex = 0;
            while (intIndex < buffer.Length)
            {
                //Find start of packet
                if ((byte)PSEMInfo.PSEM_STP == buffer[intIndex++])
                {
                    //EE byte (a.k.a Start of packet)
                    usCRCStartIndex = (ushort)(intIndex - 1);

                    //Reserved byte
                    intIndex++;

                    //Control byte
                    blnMultiPkt = ((byte)PSEMInfo.PACKET_CONTROL_MULTIPACKET == (buffer[intIndex] & (byte)PSEMInfo.PACKET_CONTROL_MULTIPACKET)) ? true : false;
                    blnFirstPkt = ((byte)PSEMInfo.PACKET_CONTROL_FIRSTPACKET == (buffer[intIndex] & (byte)PSEMInfo.PACKET_CONTROL_FIRSTPACKET)) ? true : false;
                    bytToggle = ((byte)PSEMInfo.PACKET_CONTROL_TOGGLE_BIT == (buffer[intIndex++] & (byte)PSEMInfo.PACKET_CONTROL_TOGGLE_BIT)) ? (byte)0x01 : (byte)0x00;

                    //Sequence number byte
                    bytSeqNum = buffer[intIndex++];

                    //Data length bytes
                    intDataLen = (buffer[intIndex++] << 8) | buffer[intIndex++];

                    //Check CRCs to make sure the PSEM packet in the buffer
                    //is not corrupt
                    usPktCRC = (ushort)((buffer[intDataLen + intIndex] << 8) |
                                         buffer[intDataLen + intIndex + 1]);
                    usCRC = CalcCRC(buffer,
                                    usCRCStartIndex,
                                    (ushort)(intIndex + intDataLen - usCRCStartIndex));

                    if (usPktCRC != usCRC)
                    {
                        //Corrupt packet. Send NAK, break out of while loop
                        //and ignore any additional packets in the buffer.
                        SendNak();
                        m_blnRxDataRcvd = false;
                        break;
                    }
                    else
                    {
                        //Packet is not corrupt. 
                        //Test for duplicate packet by comparing previous 
                        //packet's toggle bit and CRC.
                        if (!((m_bytRxToggle == bytToggle) &&
                                 (m_usCRC == usPktCRC)))
                        {
                            //Not a duplicate.
                            //If a single packet or the first packet of multiple packets
                            if ((false == blnMultiPkt) ||
                                 (true == blnFirstPkt))
                            {
                                m_abytRxL2Data = new byte[intDataLen];
                                System.Array.Copy(buffer,
                                                  intIndex,
                                                  m_abytRxL2Data,
                                                  0,
                                                  m_abytRxL2Data.Length);
                            }
                            else
                            {
                                if ((true == blnMultiPkt) &&
                                    (false == blnFirstPkt) &&
                                    (null == m_abytRxL2Data))
                                {
                                    //Sentinel firmware version 2.067 support.
                                    //See remark above.
                                    m_abytRxL2Data = new byte[intDataLen];
                                    System.Array.Copy(buffer,
                                                      intIndex,
                                                      m_abytRxL2Data,
                                                      0,
                                                      m_abytRxL2Data.Length);
                                }
                                else
                                {
                                    //concatenate layer 2 <data> packets
                                    byte[] bytTemp = new byte[m_abytRxL2Data.Length + intDataLen];
                                    System.Array.Copy(m_abytRxL2Data,
                                        0,
                                        bytTemp,
                                        0,
                                        m_abytRxL2Data.Length);
                                    System.Array.Copy(buffer,
                                        intIndex,
                                        bytTemp,
                                        m_abytRxL2Data.Length,
                                        intDataLen);

                                    m_abytRxL2Data = new byte[bytTemp.Length];
                                    System.Array.Copy(bytTemp,
                                                      0,
                                                      m_abytRxL2Data,
                                                      0,
                                                      m_abytRxL2Data.Length);
                                }
                            }

                            //The packet received is not a duplicate packet 
                            //so store the received packet's toggle and CRC 
                            //to determine if next packet received is a 
                            //duplicate.
                            m_bytRxToggle = bytToggle;
                            m_usCRC = usPktCRC;

                            //The response is (fully) received when the 
                            //sequence number is 0
                            if (0 == bytSeqNum)
                            {
                                m_blnRxDataRcvd = true;
                            }
                        }

                        //Send ACK in case of duplicate packet or 
                        //layer 2 <data> received.
                        SendAck();
                    }

                    //Increment index past the layer 2 <data> and 
                    //the crc
                    intIndex += intDataLen;
                    intIndex += (int)PSEMInfo.PACKET_CRC_LENGTH;
                }
            }
        }

		/// <summary>
		/// Method called when a datalink (ANSI layer 2) acknowlegement
		/// timeout occurs.
		/// </summary>
		/// <param name="stateInfo">
		/// The state information object associated
		/// with the timer that was triggered.
		/// </param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		private void AckNakRcvdTmout(object stateInfo)
		{
			m_blnAckNakTmout = (false == m_blnAckNakRcvd);
		}
		
		/// <summary>
		/// Method called when a datalink (ANSI layer 2) receive data 
		/// timeout occurs.
		/// </summary>
		/// <param name="stateInfo">
		/// The state information object associated
		/// with the timer that was triggered.
		/// </param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
        private void RxDataTimout_Tick(object stateInfo)
        {
            m_blnRxDataTmout = (false == m_blnRxDataRcvd);
        }


		/// <summary>
		/// Resizes the member buffers according to the maximum
		/// packet size member.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		private void ResizeBuffers()
		{
			if ( (ushort)PSEMInfo.DEFAULT_PACKET_SIZE_C12_18 <=  m_usMaxPktSize )
			{
				m_abytTxPkt = new byte[m_usMaxPktSize];
			}
		}

	}
	#endregion 

}
