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
using System.Linq;
using System.Text;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// MFG Table 2281 - ZigBee Link Key - This is a HW 3.0 only table that can only be read if the Misc Meterkey value is set correctly
    /// </summary>
    internal class OpenWayMFGTable2281 : AnsiTable
    {
        #region Constants

        private const uint KEY_SIZE = 32;
        private const int TABLE_TIMEOUT = 1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="table2098">The table 2098 object for the current device</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created
        
        public OpenWayMFGTable2281(CPSEM psem, CHANMfgTable2098 table2098)
            : base(psem, 2281, GetTableSize(table2098.NumberHANClients), TABLE_TIMEOUT)
        {
            m_ZigBeeKeys = null;
            m_NumClients = table2098.NumberHANClients;
        }

        /// <summary>
        /// Reads the table from the meter
        /// </summary>
        /// <returns>The result of the read</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created
        
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2281.Read");

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the set of ZigBee Keys
        /// </summary>
        /// <exception cref="Itron.Metering.Device.PSEMException">
        /// When reading this table a PSEMException may occur with PSEMResponse.Isc response if the appropriate
        /// meterkey bit is not set when trying to read this meter.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created

        public List<ZigBeeSecurityKeyPair> ZigBeeKeys
        {
            get
            {                
                ReadUnloadedTable();

                return m_ZigBeeKeys;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data that was just read from the meter
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created
        
        private void ParseData()
        {
            m_ZigBeeKeys = new List<ZigBeeSecurityKeyPair>();

            for (int iIndex = 0; iIndex < m_NumClients; iIndex++)
            {
                ZigBeeSecurityKeyPair NewKeyPair = new ZigBeeSecurityKeyPair();
                NewKeyPair.Parse(m_Reader);

                m_ZigBeeKeys.Add(NewKeyPair);
            }
        }

        /// <summary>
        /// Gets the size of the table in bytes
        /// </summary>
        /// <param name="numberOfClients">The number of HAN clients supported by the meter</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created

        private static uint GetTableSize(byte numberOfClients)
        {
            return KEY_SIZE * numberOfClients;
        }

        #endregion

        #region Member Variables

        private List<ZigBeeSecurityKeyPair> m_ZigBeeKeys;
        private byte m_NumClients;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class ZigBeeSecurityKeyPair
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created
        
        public ZigBeeSecurityKeyPair()
        {
            m_DeviceAddress = null;
            m_LinkKey = null;
            m_OutFrameCounter = 0;
            m_InFrameCounter = 0;
        }

        /// <summary>
        /// Parses the key pair from the binary reader
        /// </summary>
        /// <param name="reader">The binary reader containing the key data</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created
        
        public void Parse(PSEMBinaryReader reader)
        {
            m_DeviceAddress = reader.ReadBytes(8);
            m_LinkKey = reader.ReadBytes(16);
            m_OutFrameCounter = reader.ReadUInt32();
            m_InFrameCounter = reader.ReadUInt32();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Device Address for the device the key is assigned to
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created
        
        public byte[] DeviceAddress
        {
            get
            {
                return m_DeviceAddress;
            }
        }

        /// <summary>
        /// Gets the Device Address as a string
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created
        
        public string ReadableDeviceAddress
        {
            get
            {
                string strDeviceAddress = "";

                if (m_DeviceAddress != null)
                {
                    for (int iIndex = 0; iIndex < m_DeviceAddress.Length; iIndex++)
                    {
                        strDeviceAddress += m_DeviceAddress[iIndex].ToString("X2", CultureInfo.InvariantCulture);
                    }
                }

                return strDeviceAddress;
            }
        }

        /// <summary>
        /// Gets the link key for the device
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created
        
        public byte[] LinkKey
        {
            get
            {
                return m_LinkKey;
            }
        }

        /// <summary>
        /// Gets the Link Key as a string
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created

        public string ReadableLinkKey
        {
            get
            {
                string strLinkKey = "";

                if (m_DeviceAddress != null)
                {
                    for (int iIndex = 0; iIndex < m_LinkKey.Length; iIndex++)
                    {
                        strLinkKey += m_LinkKey[iIndex].ToString("X2", CultureInfo.InvariantCulture);
                    }
                }

                return strLinkKey;
            }
        }

        /// <summary>
        /// Gets the number of frames sent
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created
        
        private uint OutFrameCounter
        {
            get
            {
                return m_OutFrameCounter;
            }
        }

        /// <summary>
        /// Gets the number of frames received
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/12/11 RCG 2.51.23        Created
        
        private uint InFrameCounter
        {
            get
            {
                return m_InFrameCounter;
            }
        }

        #endregion

        #region Member Variables

        private byte[] m_DeviceAddress;
        private byte[] m_LinkKey;
        private uint m_OutFrameCounter;
        private uint m_InFrameCounter;

        #endregion
    }
}
