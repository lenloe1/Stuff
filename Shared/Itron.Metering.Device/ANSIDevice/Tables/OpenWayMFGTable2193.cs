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
//                           Copyright © 2010 - 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// MFG Table 2193 (Itron 145) class
    /// </summary>
    public class OpenWayMFGTable2193 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 2;

        private const byte SEC_REQUIRED_MASK = 0x01;
        private const byte SEC_FORMAT_MASK = 0x0E;
        private const byte C1218_OVER_ZIGBEE_MASK = 0x10;
        private const int SEC_FORMAT_SHIFT = 1;
        private const byte PRIVATE_PROFILE_MASK = 0x20;
        private const byte ZIGBEE_ENABLED_MASK = 0x40;

        private const byte SIGNED_AUTH_MASK = 0x01;
        

        #endregion

        #region Definitions

        /// <summary>
        /// Security formats
        /// </summary>
        public enum SecurityFormat : byte
        {
#pragma warning disable 1591 // Ignores the XML comment warnings
            None = 0,
            Reserved = 1,
            C1222StandardSecurity = 2,
            EnhancedItronSecurity = 3,
#pragma warning restore 1591
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/03/08 RCG 2.00.02 N/A    Created

        public OpenWayMFGTable2193(CPSEM psem)
            : base (psem, 2193, TABLE_SIZE)
        {
        }

        /// <summary>
        /// Gets the text string for the specified Security format
        /// </summary>
        /// <param name="format">The format to get the string for.</param>
        /// <returns>The string for the format.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/03/08 RCG 2.00.02 N/A    Created

        public static string GetSecurityFormatString(SecurityFormat format)
        {
            string strFormat = null;

            switch(format)
            {
                case SecurityFormat.None:
                {
                    strFormat = "None";
                    break;
                }
                case SecurityFormat.EnhancedItronSecurity:
                {
                    strFormat = "Enhanced Itron Security";
                    break;
                }
                case SecurityFormat.C1222StandardSecurity:
                {
                    strFormat = "Standard C12.22 Security";
                    break;
                }
            }

            return strFormat;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/03/08 RCG 2.00.02 N/A    Created
        
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2193.Read");

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Writes the table to the meter.
        /// </summary>
        /// <returns>The result of the write</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/27/10 RCG 2.44.06 N/A    Created

        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2193.Write");

            // Write all of the data back to the stream so we
            // write the data that we have stored.

            m_DataStream.Position = 0;

            m_Writer.Write(m_bySecurityOptions);
            m_Writer.Write(m_byConfigByte2);

            return base.Write();
        }

        /// <summary>
        /// This method will either take down the ZigBee network permanently or reenable permanently
        /// depending on the bool parameter passed in.  Pass true to enable; false, to disable.
        /// </summary>
        /// <param name="Enable"></param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/23/13 AF  2.71.02 WR417928 Created
        //
        public PSEMResponse EnableDisableZigBeePermanently(bool Enable)
        {
            PSEMResponse Result = PSEMResponse.Err;
            ReadUnloadedTable();

            if (Enable)
            {
                // We need to clear the bit so bitwise and the inverse of the mask
                m_bySecurityOptions = (byte)(m_bySecurityOptions & ~ZIGBEE_ENABLED_MASK);
            }
            else
            {
                // We need to make sure the bit is set so bitwise or it in
                m_bySecurityOptions = (byte)(m_bySecurityOptions | ZIGBEE_ENABLED_MASK);
            }

            //Write the table. Since we read the table at the beginning we will update only the security options
            Result = Write();

            // Make sure we don't use the old values
            State = TableState.Dirty;

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the exception model to use for enhanced security
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/03/08 RCG 2.00.02 N/A    Created

        public SecurityFormat ExceptionModel
        {
            get
            {
                ReadUnloadedTable();
                return (SecurityFormat)((m_bySecurityOptions & SEC_FORMAT_MASK) >> SEC_FORMAT_SHIFT);
            }
        }

        /// <summary>
        /// Gets whether or not the enhanced security is required.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/03/08 RCG 2.00.02 N/A    Created

        public bool IsEnhancedSecurityRequired
        {
            get
            {
                ReadUnloadedTable();
                return (m_bySecurityOptions & SEC_REQUIRED_MASK) == SEC_REQUIRED_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not C12.18 over ZigBee is enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/03/09 RCG 2.21.06 N/A    Created

        public bool IsC1218OverZigBeeEnabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_bySecurityOptions & C1218_OVER_ZIGBEE_MASK) == C1218_OVER_ZIGBEE_MASK;
            }
            set
            {
                State = TableState.Dirty;

                if (value)
                {
                    // We need to make sure the bit is set so bitwise or it in
                    m_bySecurityOptions = (byte)(m_bySecurityOptions | C1218_OVER_ZIGBEE_MASK);
                }
                else
                {
                    // We need to clear the bit so bitwise and the inverse of the mask
                    m_bySecurityOptions = (byte)(m_bySecurityOptions & ~C1218_OVER_ZIGBEE_MASK);
                }
            }
        }

        /// <summary>
        /// Gets whether or not ZigBee Private Profile enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/06/09 MMD 2.30.07 N/A    Created
        // 03/31/10 AF  2.40.31 N/A     Renamed since true means disabled not enabled
        public bool IsZigBeePrivateProfileDisabled
        {
            get
            {
                ReadUnloadedTable();

                return (m_bySecurityOptions & PRIVATE_PROFILE_MASK) == PRIVATE_PROFILE_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not ZigBee is enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/06/09 MMD 2.30.07 N/A    Created
        // 03/31/10 AF  2.40.31         Renamed the property to make the logic more intuitive
        // 02/27/13 MP  2.80.07 N/A    Added set
        // 07/23/13 AF  2.71.02 WR417928 Changed set to use new method
        //
        public bool IsZigBeeDisabled
        {
            get
            {
                State = TableState.Expired;
                ReadUnloadedTable();

                return (m_bySecurityOptions & ZIGBEE_ENABLED_MASK) == ZIGBEE_ENABLED_MASK;
            }
            set
            {
                if (value)
                {
                    EnableDisableZigBeePermanently(false);
                }
                else
                {
                    EnableDisableZigBeePermanently(true);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data that was just read. 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/03/08 RCG 2.00.02 N/A    Created

        private void ParseData()
        {
            m_bySecurityOptions = m_Reader.ReadByte();
            m_byConfigByte2 = m_Reader.ReadByte();
        }

        #endregion

        #region Member Variables

        private byte m_bySecurityOptions;
        private byte m_byConfigByte2;

        #endregion
    }
}
