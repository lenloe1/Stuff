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
//                              Copyright © 2012
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// MFG Table 2429 (Itron 381) class
    /// </summary>
    public class OpenWayMFGTable2429 : AnsiTable
    {
        #region Constants

        private const uint BRIDGE_P1_TABLE_SIZE = 13;

        #endregion

        #region Definitions

        /// <summary>
        /// Constant describing Bubble-up LID for Watt Hours Delivered
        /// </summary>
        public const UInt32 BUP_LID_WATT_HOUR_DELIVERED = 0x14000081;

        /// <summary>
        /// Constant describing Bubble-up LID for Watt Hours Net
        /// </summary>
        public const UInt32 BUP_LID_WATT_HOUR_NET = 0x140000A4;

        /// <summary>
        /// Constant describing Bubble-up LID for Watt Hours Unidirectional
        /// </summary>
        public const UInt32 BUP_LID_WATT_HOUR_UNIDIR = 0x140000A7;

        /// <summary>
        /// Constant describing ChoiceConnect Security State of Unsecured
        /// </summary>
        public const byte CC_SECURITY_STATE_UNSECURED = 0;

        /// <summary>
        /// Constant describing ChoiceConnect Security State of Command Security
        /// </summary>
        public const byte CC_SECURITY_STATE_COMMAND = 1;

        /// <summary>
        /// Constant describing ChoiceConnect Security State of Read Security
        /// </summary>
        public const byte CC_SECURITY_STATE_READ = 2;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/05/12 JJJ 2.60.xx N/A    Created
        // 02/24/14 jrf 3.50.37 460693 Allowing flexible table size since extra field was added in Bridge P2.

        public OpenWayMFGTable2429(CPSEM psem)
            : base(psem, 2429, BRIDGE_P1_TABLE_SIZE)
        {
            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Gets the text string for the specified Bubble Up LID
        /// </summary>
        /// <param name="uiLID">The bubble-up LID to get the string for.</param>
        /// <returns>The string associated with the LID value.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/05/12 JJJ 2.60.xx N/A    Created
        // 02/13/14 jrf 3.50.32 442686 Improved to use LID class to decipher numeric LID.

        public static string GetBubbleUpLIDString(UInt32 uiLID)
        {
            string strLid = null;

            try
            {
                LID BubbleUpLID = new LID(uiLID);

                strLid = BubbleUpLID.lidDescription;
            }
            catch
            {
                strLid = "Unknown Quantity";
            }

            return strLid;
        }

        /// <summary>
        /// Gets the text string for the specified CHoiceConnect Security State
        /// </summary>
        /// <param name="SecurityState">The security state to translate into a string.</param>
        /// <returns>The string associated with the security state value.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/20/12 JJJ 2.60.xx N/A    Created

        public static string GetChoiceConnectSecurityTranslation(byte SecurityState)
        {
            string strSecurityState = null;

            switch (SecurityState)
            {
                case CC_SECURITY_STATE_UNSECURED:
                    {
                        strSecurityState = "Unsecured";
                        break;
                    }

                case CC_SECURITY_STATE_COMMAND:
                    {
                        strSecurityState = "Command Security";
                        break;
                    }

                case CC_SECURITY_STATE_READ:
                    {
                        strSecurityState = "Read Security";
                        break;
                    }
                // Any value that we don't recognize should be "unknown"
                default:
                    {
                        strSecurityState = "Unknown Security State";
                        break;
                    }
            }

            return strSecurityState;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/05/12 JJJ 2.60.xx N/A    Created
        
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2429.Read");

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            base.State = TableState.Loaded;

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception. 
        /// Not supporting full writes. This table's data is read from Comm. Module.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/05/12 JJJ 2.60.xx         Created
        //
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  
        /// Not supporting offset writes. This table's data is read from Comm. Module.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/05/12 JJJ 2.60.xx         Created
        //
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Comm Type from ChoiceConnect Comm. Module data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/05/12 JJJ 2.60.xx N/A    Created

        public byte ChoiceConnectCommType
        {
            get
            {
                ReadUnloadedTable();
                return m_byChoiceConnectCommType;
            }
        }


        /// <summary>
        /// Gets the ChoiceConnect Comm. Ert Id Number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/05/12 JJJ 2.60.xx N/A    Created

        public uint ErtIdNumber
        {
            get
            {
                ReadUnloadedTable();
                return m_uintErtId;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect Comm. Bubble Up LID value
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/19/12 JJJ 2.60.xx N/A    Created

        public uint BubbleUpLIDValue
        {
            get
            {
                ReadUnloadedTable();
                return m_uintBubbleUpLID;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect Comm. Security State value
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/20/12 JJJ 2.60.xx N/A    Created

        public byte CCSecurityStateValue
        {
            get
            {
                ReadUnloadedTable();
                return m_byChoiceConnectSecurityState;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect Comm. ERT ID
        /// as a formatted string
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/05/12 JJJ 2.60.xx N/A    Created

        public string ErtIdString
        {
            get
            {
                string strErtId= "0";

                ReadUnloadedTable();
                strErtId = m_uintErtId.ToString(CultureInfo.InvariantCulture);

                return strErtId;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect Comm. Security State as a translated string value
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/20/12 JJJ 2.60.xx N/A    Created

        public string CCSecurityStateString
        {
            get
            {
                ReadUnloadedTable();

                string strSecurityState = GetChoiceConnectSecurityTranslation(m_byChoiceConnectSecurityState);
                
                return strSecurityState;
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
        // 03/05/12 JJJ 2.60.xx N/A    Created

        private void ParseData()
        {
            m_byChoiceConnectCommType = m_Reader.ReadByte();
            m_byChoiceConnectCommFwVer = m_Reader.ReadByte();
            m_byChoiceConnectCommFwRev = m_Reader.ReadByte();
            m_byChoiceConnectCommFwBuild = m_Reader.ReadByte();
            m_uintErtId = m_Reader.ReadUInt32();
            m_uintBubbleUpLID = m_Reader.ReadUInt32();
            m_byChoiceConnectSecurityState = m_Reader.ReadByte();
            //Check table size before attempting to read any more fields
        }

        #endregion

        #region Member Variables

        private byte m_byChoiceConnectCommType;

        // These ChoiceConnect Values are to be written by the ChoiceConnect FW to the Register
        // Firmware values should be read from the register in Table 2428
        private byte m_byChoiceConnectCommFwVer;
        private byte m_byChoiceConnectCommFwRev;
        private byte m_byChoiceConnectCommFwBuild;

        private uint m_uintErtId;
        private uint m_uintBubbleUpLID;
        private byte m_byChoiceConnectSecurityState;

        #endregion
    }
}
