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
//                              Copyright © 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// MFG Table 173 (2221) - Security Information
    /// </summary>
    public class OpenWayMFGTable2221 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 1;
        private const int TABLE_TIMEOUT = 500;

        private const int ACTIVE_KEY_SECT283K1 = 0x01;
        private const int ACTIVE_KEY_SECP256R1 = 0x02;
        private const int KEYS_NOT_INJECTED = 0xFF;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/26/13 mah 2.85.03        Created

        public OpenWayMFGTable2221(CPSEM psem)
            : base(psem, 2221, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Constructor used by EDL file.
        /// </summary>
        /// <param name="binaryReader">The binary reader containing the table data.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 07/26/13 mah 2.85.03        Created
        //
        public OpenWayMFGTable2221(PSEMBinaryReader binaryReader)
            : base(2221, TABLE_SIZE)
        {
            m_TableState = TableState.Loaded;
            m_Reader = binaryReader;
            ParseData();
        }

        /// <summary>
        /// Reads the sub table from the meter
        /// </summary>
        /// <returns>The response of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/26/13 mah 2.85.03        Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2221.Read");

            PSEMResponse Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the meter is Canadian
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/26/13 mah 2.85.03        Created

        public bool KeysInjected
        {
            get
            {
                ReadUnloadedTable();

                if ((m_byteKeyType == ACTIVE_KEY_SECT283K1) ||
                    (m_byteKeyType == ACTIVE_KEY_SECP256R1))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets whether or not the meter is Canadian
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/26/13 mah 2.85.03        Created

        public EnhancedSecurityAlgorithmUsed KeyType
        {
            get
            {
                EnhancedSecurityAlgorithmUsed eAlgorithmUsed = EnhancedSecurityAlgorithmUsed.UNDEFINED;
                
                ReadUnloadedTable();

                switch ( m_byteKeyType )
                {
                    case ACTIVE_KEY_SECT283K1:
                            eAlgorithmUsed = EnhancedSecurityAlgorithmUsed.K_CURVE;
                            break;
                    case ACTIVE_KEY_SECP256R1:
                            eAlgorithmUsed = EnhancedSecurityAlgorithmUsed.P_CURVE;
                            break;
                    default:
                            eAlgorithmUsed = EnhancedSecurityAlgorithmUsed.UNDEFINED;
                            break;

                }

                return eAlgorithmUsed;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data that has just been read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/26/13 mah 2.85.03        Created

        private void ParseData()
        {
            m_byteKeyType = m_Reader.ReadByte();
        }

        #endregion

        #region Member Variables

        private byte m_byteKeyType;

        #endregion
    }
}
