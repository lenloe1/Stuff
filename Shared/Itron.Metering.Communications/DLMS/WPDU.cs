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
//                              Copyright © 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Itron.Metering.Communications.DLMS
{
    /// <summary>
    /// DLMS Wrapper PDU
    /// </summary>
    public class WPDU
    {
        #region Constants

        private const ushort VERSION = 0x0001;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public WPDU()
        {
            m_SourcePort = 0;
            m_DestinationPort = 0;
            m_Payload = new byte[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">The Source Port</param>
        /// <param name="destination">The Destination Port</param>
        /// <param name="payload">The WPDU Payload</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public WPDU(ushort source, ushort destination, byte[] payload)
        {
            m_SourcePort = source;
            m_DestinationPort = destination;
            m_Payload = payload;
        }

        /// <summary>
        /// Parses the WPDU from the stream. Assumes the stream is at the start of a valid WPDU
        /// </summary>
        /// <param name="dataStream">The stream to parse from</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public void Parse(Stream dataStream)
        {
            DLMSBinaryReader DataReader = new DLMSBinaryReader(dataStream);
            ushort PayloadLength = 0;

            if (DataReader.ReadUInt16() == VERSION)
            {
                m_SourcePort = DataReader.ReadUInt16();
                m_DestinationPort = DataReader.ReadUInt16();
                PayloadLength = DataReader.ReadUInt16();

                if (dataStream.Length - dataStream.Position >= PayloadLength)
                {
                    m_Payload = DataReader.ReadBytes(PayloadLength);
                }
                else
                {
                    throw new ArgumentException("The stream does not currently contain the entire WPDU.");
                }
            }
            else
            {
                throw new ArgumentException("The stream is not at the start of a valid WPDU.");
            }
        }

        /// <summary>
        /// Finds the start index of the WPDU
        /// </summary>
        /// <param name="source">The source port to search for</param>
        /// <param name="destination">The destination port to search for</param>
        /// <param name="data">That data to search</param>
        /// <returns>The index of the start of the WPDU or -1 if not found</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public static int FindStartOfWPDU(ushort source, ushort destination, List<byte> data)
        {
            int StartIndex = -1;

            // Generate the sequence to find
            MemoryStream SearchStream = new MemoryStream();
            DLMSBinaryWriter SearchWriter = new DLMSBinaryWriter(SearchStream);

            SearchWriter.Write(VERSION);
            SearchWriter.Write(source);
            SearchWriter.Write(destination);

            byte[] SearchSequence = SearchStream.ToArray();

            for (int iStartIndex = 0; iStartIndex < data.Count - SearchSequence.Length; iStartIndex++)
            {
                bool Found = true;

                for (int iSequenceIndex = 0; iSequenceIndex < SearchSequence.Length; iSequenceIndex++)
                {
                    if(SearchSequence[iSequenceIndex] != data[iStartIndex + iSequenceIndex])
                    {
                        Found = false;
                        break;
                    }
                }

                if(Found)
                {
                    StartIndex = iStartIndex;
                    break;
                }
            }

            return StartIndex;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Source Port of the WPDU
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created

        public ushort SourcePort
        {
            get
            {
                return m_SourcePort;
            }
        }

        /// <summary>
        /// Gets the Destination Port of the WPDU
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public ushort DestinationPort
        {
            get
            {
                return m_DestinationPort;
            }
        }

        /// <summary>
        /// Gets the payload of the WPDU
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] Payload
        {
            get
            {
                return m_Payload;
            }
        }

        /// <summary>
        /// Gets the raw data of the APDU
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/04/12 RCG 2.70.63 N/A    Created
        
        public byte[] Data
        {
            get
            {
                MemoryStream DataStream = new MemoryStream();
                DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

                DataWriter.Write(VERSION);
                DataWriter.Write(m_SourcePort);
                DataWriter.Write(m_DestinationPort);
                DataWriter.Write((ushort)m_Payload.Length);
                DataWriter.Write(m_Payload);

                return DataStream.ToArray();
            }
        }

        #endregion

        #region Member Variables

        private ushort m_SourcePort;
        private ushort m_DestinationPort;
        private byte[] m_Payload;

        #endregion
    }
}
