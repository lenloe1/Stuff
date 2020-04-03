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
//                              Copyright © 2005 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using Itron.Metering.Communications;

namespace Itron.Metering.Communications.PSEM
{
	/// <summary>
	/// C1218L2 inherits from CANSIL2 and supports the ANSI datalink 
	/// layer 2 for C12.18 communication with a device. 
	/// </summary>
	/// <remarks>
	/// CANSIL2 is internal which implies it is not visible 
	/// outside the assembly.
	///  </remarks>
	/// Revision History
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ ---------------------------------------
	/// 08/01/05 bdm 7.13.00 N/A	Created
	internal class CC1218L2 : CANSIL2
	{

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="comm">
		/// Communication object that supports communication over the 
		/// physical layer
		/// </param>
		/// <example>
		/// <code>
		/// Communication comm = new Communication();
		/// comm.OpenPort("COM4:");
		/// C1218L2 c1218l2 = new C1218L2(comm)
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/01/05 bdm 7.13.00 N/A	Created
		public CC1218L2(ICommunications comm):base(comm)
		{
		}


		/// <summary>
		/// Builds the complete datalink (ANSI layer 2) packet for C12.18.
		/// Overrides the base class abstract.
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
        protected override void BuildPacket(
            byte sequenceNumber,
            bool firstPacket,
            bool multiPacket)
        {
            ushort usCRC = 0;
            ushort usPacketLength = 0;
            byte[] txPacket = new byte[this.m_usMaxPktSize];
            int intIndex = 0;

            //Start of packet byte
            txPacket[intIndex++] = (byte)PSEMInfo.PSEM_STP;

            //Reserved byte
            txPacket[intIndex++] = m_bytReserved;

            //Control byte.
            txPacket[intIndex] = 0;
            if ((multiPacket) || (sequenceNumber > 0))
            {
                txPacket[intIndex] |= (byte)PSEMInfo.PACKET_CONTROL_MULTIPACKET;
            }
            if ((firstPacket) && (sequenceNumber > 0))
            {
                txPacket[intIndex] |= (byte)PSEMInfo.PACKET_CONTROL_FIRSTPACKET;
            }
            if (0x01 == m_bytTxToggle)
            {
                txPacket[intIndex] |= (byte)PSEMInfo.PACKET_CONTROL_TOGGLE_BIT;
            }
            intIndex++;

            //Sequence number byte
            txPacket[intIndex++] = sequenceNumber;

            if (sequenceNumber > 0)
            {
                usPacketLength = m_usMaxPktSize;
            }
            else if (m_abytTxL2Data.Length >= m_usMaxPktSize - m_usPktOverhead)
            {
                // Special case for boundary condition.
                usPacketLength = m_usMaxPktSize;
            }
            else
            {
                usPacketLength = (ushort)(m_usPktOverhead + (m_abytTxL2Data.Length % (m_usMaxPktSize - m_usPktOverhead)));
            }

            // Write the length of data into the packet.
            Write16Bits(ref txPacket, intIndex, (ushort)(usPacketLength - m_usPktOverhead));
            intIndex += 2;

            Array.Copy(m_abytTxL2Data, 0, txPacket, intIndex, usPacketLength - m_usPktOverhead);
            intIndex += (usPacketLength - m_usPktOverhead);

            if (m_abytTxL2Data.Length > (usPacketLength - m_usPktOverhead))
            {
                //Remove the bytes that are being sent
                byte[] temp = new byte[m_abytTxL2Data.Length - usPacketLength + m_usPktOverhead];
                Array.Copy(m_abytTxL2Data, usPacketLength - m_usPktOverhead, temp, 0, temp.Length);
                m_abytTxL2Data = new byte[temp.Length];
                Array.Copy(temp, 0, m_abytTxL2Data, 0, m_abytTxL2Data.Length);
            }

            usCRC = (ushort)(CalcCRC(txPacket, 0, (ushort)((usPacketLength - (ushort)PSEMInfo.PACKET_CRC_LENGTH))));
            Write16Bits(ref txPacket, intIndex, usCRC);
            intIndex += 2;
            m_abytTxPkt = new byte[intIndex];

            Array.Copy(txPacket, 0, m_abytTxPkt, 0, m_abytTxPkt.Length);
        }
	}
}
