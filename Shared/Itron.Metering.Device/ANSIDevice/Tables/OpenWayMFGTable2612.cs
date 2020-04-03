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
//                              Copyright © 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    internal class OpenWayMFGTable2612 : AnsiTable
    {
        #region Constants

        private const int TABLE_2612_HEADER_LENGTH = 8;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/19/11 AF  2.53.21 184509 Created
        //
        public OpenWayMFGTable2612(CPSEM psem)
            : base(psem, 2612, TABLE_2612_HEADER_LENGTH)
        {
            m_TLVData = null;
        }

        /// <summary>
        /// Reads Mfg table 564 out of the meter
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/19/11 AF  2.53.21 184509 Created
        //  11/07/12 AF  2.70.36 242041 This is primarily a firmware issue but if the data
        //                              are missing, don't try to read 0 bytes.
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Table " + m_TableID.ToString(CultureInfo.CurrentCulture));

            // Read the table - Since Read does a full read and checks the length
            // which should never be the same size as the whole table we need to do
            // an offset read here
            PSEMResponse Result = base.Read(0, (ushort)TABLE_2612_HEADER_LENGTH);

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_TLVID = m_Reader.ReadUInt32();
                m_TLVDataLength = m_Reader.ReadUInt32();

                //Handle when meter returns a TLVDataLength of 0xFFFF in error.
                if (m_TLVDataLength > 0 && m_TLVDataLength < 0xFFFF)
                {
                    m_TLVData = new byte[m_TLVDataLength];
                    m_Size += m_TLVDataLength;

                    m_DataStream.Position = TABLE_2612_HEADER_LENGTH;

                    // Resize the data array
                    byte[] ResizedData = new byte[m_Size];
                    Array.Copy(m_Data, 0, ResizedData, 0, m_Data.Length);
                    m_Data = ResizedData;

                    m_DataStream = new MemoryStream(m_Data);
                    m_Reader = new PSEMBinaryReader(m_DataStream);
                    m_Writer = new PSEMBinaryWriter(m_DataStream);

                    Result = base.Read((ushort)TABLE_2612_HEADER_LENGTH, (ushort)m_TLVDataLength);

                    if (Result == PSEMResponse.Ok)
                    {
                        m_TLVData = m_Reader.ReadBytes((int)m_TLVDataLength);
                    }
                }
                else
                {
                    Result = PSEMResponse.Onp;
                }
            }
            else
            {
                Result = PSEMResponse.Onp;
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Reads the TLV ID field that stores which TLV was requested
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/19/11 AF  2.53.21 184509 Created
        //
        public UInt32 TLVID
        {
            get
            {
                ReadUnloadedTable();

                return m_TLVID;
            }
        }

        /// <summary>
        /// Reads the TLV data stored in table 2612 for the requested TLV ID.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/19/11 AF  2.53.21 184509 Created
        //
        public byte[] TLVData
        {
            get
            {
                ReadUnloadedTable();

                return m_TLVData;
            }
        }

        #endregion

        #region Members

        private UInt32 m_TLVID;
        private UInt32 m_TLVDataLength;
        private byte[] m_TLVData;

        #endregion
    }
}
