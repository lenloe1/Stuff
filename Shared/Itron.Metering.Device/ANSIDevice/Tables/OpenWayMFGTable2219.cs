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
//                              Copyright © 2009
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
    /// Enum used for the state of ZigBee and Signed Authorization
    /// </summary>
    public enum FeatureState : byte
    {
        /// <summary>
        /// The feature is currently enabled
        /// </summary>
        [EnumDescription("Enabled")]
        Enabled = 0,
        /// <summary>
        /// The feature is permanently disabled
        /// </summary>
        [EnumDescription("Disabled")]
        Disabled = 1,
        /// <summary>
        /// The feature has been temporarily disabled for an amount of time
        /// </summary>
        [EnumDescription("Temporarily Disabled")]
        DisabledForPeriod = 2,
    }

    /// <summary>
    /// MFG Table 2219 (Itron 171) 
    /// </summary>
    public class OpenWayMFGTable2219 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 7;
        private const int TABLE_TIMEOUT = 1000;

        private const byte ZIGBEE_ENABLED_MASK = 0x03;
        private const byte SIGNED_AUTH_ENABLED_MASK = 0x0C;
        private const byte PASSWORDS_HASHED_MASK = 0x10;
        private const byte JTAG_ENABLED_MASK = 0x20;
        private const byte ENHANCED_INJECTED_MASK = 0x40;
        private const byte HAN_PRICING_ENABLED_MASK = 0x01;
        private const int STATUS_RCD_1_OFFSET = 0;
        private const int STATUS_RCD_3_OFFSET = 2;
        private const int STATUS_RCD_4_OFFSET = 4;
        private const int STATUS_RCD_5_OFFSET = 6;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/09 RCG 2.30.09 N/A    Created

        public OpenWayMFGTable2219(CPSEM psem)
            : base (psem, 2219, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/09 RCG 2.30.09 N/A    Created
        // 12/12/12 jrf 2.70.50 263895 Made all reads of this table offset reads.
        //
        public override PSEMResponse Read()
        {
            //WR 263895 - Beryllium 5.7.6 changed this table's size from 6 to 7 bytes. So 
            //the only way to determine the table's size is with knowledge of the firmware version.
            //However signed authorization state is read from this table sometimes before security 
            //is issued. Since firmware verison info is not available until after security is issued
            //and we require knowledge of the table's size to perform a full read, this presents a problem
            //for full reads. Current solution is to only support full reads of this table.  Any attempt
            //to reinstate full reads of this table should take the preceeding information into careful
            //consideration.
            throw new NotSupportedException("This table does not currently support full reads.");
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current state of ZigBee
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/09 RCG 2.30.09 N/A    Created
        // 12/12/12 jrf 2.70.50 263895 Made an offset read.
        //
        public FeatureState ZigBeeState
        {
            get
            {
                PSEMResponse Result = base.Read(STATUS_RCD_1_OFFSET, 1);

                if (PSEMResponse.Ok == Result)
                {
                    m_byStatusByte1 = m_Reader.ReadByte();
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the ZigBee state."));
                }

                return (FeatureState)(m_byStatusByte1 & ZIGBEE_ENABLED_MASK);
            }
        }

        /// <summary>
        /// Gets the current state of Signed Authorization in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/09 RCG 2.30.09 N/A    Created
        // 12/12/12 jrf 2.70.50 263895 Made an offset read.
        //
        public FeatureState SignedAuthorizationState
        {
            get
            {
                PSEMResponse Result = base.Read(STATUS_RCD_1_OFFSET, 1);

                if (PSEMResponse.Ok == Result)
                {
                    m_byStatusByte1 = m_Reader.ReadByte();
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the Signed Authorization state."));
                }

                return (FeatureState)((m_byStatusByte1 & SIGNED_AUTH_ENABLED_MASK) >> 2);
            }
        }

        /// <summary>
        /// Gets the amount of time remaining for ZigBee to be disabled.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/09 RCG 2.30.09 N/A    Created
        // 12/12/12 jrf 2.70.50 263895 Made an offset read.
        //
        public TimeSpan ZigBeeDisabledTimeRemaining
        {
            get
            {
                PSEMResponse Result = base.Read(STATUS_RCD_3_OFFSET, 2);

                if (PSEMResponse.Ok == Result)
                {
                    m_usZigBeeMinutesRemaining = m_Reader.ReadUInt16();
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the ZigBee disabled time remaining."));
                }

                return TimeSpan.FromMinutes(m_usZigBeeMinutesRemaining);
            }
        }

        /// <summary>
        /// Gets the amount of time remaining for Signed Authorization to be disabled
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/09 RCG 2.30.09 N/A    Created
        // 12/12/12 jrf 2.70.50 263895 Made an offset read.
        //
        public TimeSpan SignedAuthorizationDisabledTimeRemaining
        {
            get
            {
                PSEMResponse Result = base.Read(STATUS_RCD_4_OFFSET, 2);

                if (PSEMResponse.Ok == Result)
                {
                    m_usSignedAuthMinutesRemaing = m_Reader.ReadUInt16();
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the Signed Authorization disabled time remaining."));
                }

                return TimeSpan.FromMinutes(m_usSignedAuthMinutesRemaing);
            }
        }

        /// <summary>
        /// Gets whether or not the C12.18 Passwords are hashed in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/09 RCG 2.30.09 N/A    Created
        // 12/12/12 jrf 2.70.50 263895 Made an offset read.
        //
        public bool AreC1218PasswordsHashed
        {
            get
            {
                PSEMResponse Result = base.Read(STATUS_RCD_1_OFFSET, 1);

                if (PSEMResponse.Ok == Result)
                {
                    m_byStatusByte1 = m_Reader.ReadByte();
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the C1218 passwords hashed state."));
                }

                return (m_byStatusByte1 & PASSWORDS_HASHED_MASK) == PASSWORDS_HASHED_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the JTAG Security bit has been enabled.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/12/09 RCG 2.30.09 N/A    Created
        // 12/12/12 jrf 2.70.50 263895 Made an offset read.
        //
        public bool IsJTAGSecurityEnabled
        {
            get
            {
                PSEMResponse Result = base.Read(STATUS_RCD_1_OFFSET, 1);

                if (PSEMResponse.Ok == Result)
                {
                    m_byStatusByte1 = m_Reader.ReadByte();
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the JTAG security enabled state."));
                }

                return (m_byStatusByte1 & JTAG_ENABLED_MASK) == JTAG_ENABLED_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the Enhanced Security Keys has been injected.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/23/09 MMD 2.30.20 N/A    Created
        // 12/12/12 jrf 2.70.50 263895 Made an offset read.
        //
        public bool EnhancedSecurityKeysInjected
        {
            get
            {
                PSEMResponse Result = base.Read(STATUS_RCD_1_OFFSET, 1);

                if (PSEMResponse.Ok == Result)
                {
                    m_byStatusByte1 = m_Reader.ReadByte();
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the enhanced security keys injected state."));
                }

                return (m_byStatusByte1 & ENHANCED_INJECTED_MASK) == ENHANCED_INJECTED_MASK;
            }
        }

        /// <summary>
        /// Gets whether or not the HAN Pricing bit has been enabled.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/05/13 AR  2.80.35 N/A    Created
        //
        public bool IsHANPricingEnabled
        {
            get
            {
                PSEMResponse Result = base.Read(STATUS_RCD_5_OFFSET, 1);

                if (PSEMResponse.Ok == Result)
                {
                    m_byPricingStatus = m_Reader.ReadByte();
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the HAN Pricing enabled state."));
                }

                return (m_byPricingStatus & HAN_PRICING_ENABLED_MASK) == HAN_PRICING_ENABLED_MASK;
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
        // 10/12/09 RCG 2.30.09 N/A    Created

        private void ParseData()
        {
            m_byStatusByte1 = m_Reader.ReadByte();
            m_byStatusByte2 = m_Reader.ReadByte();
            m_usZigBeeMinutesRemaining = m_Reader.ReadUInt16();
            m_usSignedAuthMinutesRemaing = m_Reader.ReadUInt16();
        }

        #endregion

        #region Member Variables

        private byte m_byStatusByte1;
        private byte m_byStatusByte2;
        private byte m_byPricingStatus;
        private ushort m_usZigBeeMinutesRemaining;
        private ushort m_usSignedAuthMinutesRemaing; 

        #endregion
    }
}
