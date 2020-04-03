///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential  
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or  
//  otherwise. Including photocopying and recording or in connection with any 
//  information storage or retrieval system without the permission in writing 
//  from Itron, Inc.
//
//                              Copyright © 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Itron.Metering.ZigBeeRadioServerObjects
{
    /// <summary>
    /// A Token that is used by the ZigBeeRadioManager to represent a specific ZigBee radio.
    /// </summary>
    [DataContract]
    public class ZigBeeRadioToken : IEquatable<ZigBeeRadioToken>
    {
        #region Definitions

        /// <summary>
        /// Enumeration for the radio type that this token represents.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/08 RCG 1.00           Created

        [DataContract]
        public enum ZigBeeRadioType
        {
            /// <summary>
            /// Integration Associates USB dongle radio.
            /// </summary>
            [EnumMember]
            IADongle,
            /// <summary>
            /// Itron ZigBee radio connected by Bluetooth
            /// </summary>
            [EnumMember]
            BluetoothRadio,
            /// <summary>
            /// Itron ZigBee radio connected by USB
            /// </summary>
            [EnumMember]
            USBRadio,
            /// <summary>
            /// Telegesis radio running BCR code.
            /// </summary>
            [EnumMember]
            TelegesisRadio,
        }

        /// <summary>
        /// Enumeration for the status of a ZigBee Radio
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/03/08 RCG 1.00           Created

        [DataContract]
        public enum ZigBeeRadioStatus
        {
            /// <summary>
            /// The radio is currently available for use.
            /// </summary>
            [EnumMember]
            Available,

            /// <summary>
            /// The radio is currently in use.
            /// </summary>
            [EnumMember]
            InUse,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="radioType">The type of the radio the token represents.</param>
        /// <param name="radioIdentifier">The identifier of the radio the token represents.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/08 RCG 1.00           Created

        public ZigBeeRadioToken(ZigBeeRadioType radioType, string radioIdentifier)
        {
            m_RadioType = radioType;
            m_strRadioID = radioIdentifier;
            m_CurrentStatus = ZigBeeRadioStatus.Available;
        }

        /// <summary>
        /// Determines whether or not the specified radio token is equal to the current
        /// radio token.
        /// </summary>
        /// <param name="other">The radio token to check.</param>
        /// <returns>True if the radio tokens are equal false otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/03/08 RCG 1.00           Created

        public bool Equals(ZigBeeRadioToken other)
        {
            bool bIsEqual = false;

            if (other != null && RadioType == other.RadioType 
                && RadioIdentifier.Equals(other.RadioIdentifier, StringComparison.Ordinal))
            {
                bIsEqual = true;
            }

            return bIsEqual;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the radio type for the radio this token represents.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/08 RCG 1.00           Created

        public ZigBeeRadioType RadioType
        {
            get
            {
                return m_RadioType;
            }
        }

        /// <summary>
        /// Gets the identifier for the radio this token represents.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/08 RCG 1.00           Created
        public string RadioIdentifier
        {
            get
            {
                return m_strRadioID;
            }
        }

        /// <summary>
        /// Gets the current status of the radio.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/03/08 RCG 1.00           Created
        public ZigBeeRadioStatus Status
        {
            get
            {
                return m_CurrentStatus;
            }
            internal set
            {
                m_CurrentStatus = value;
            }
        }

        #endregion

        #region Member Variables
        /// <summary>
        /// The type of radio that this token represents.
        /// </summary>
        [DataMember]
        private ZigBeeRadioType m_RadioType;

        /// <summary>
        /// The identifier for the radio this token represents.
        /// </summary>
        [DataMember]
        private string m_strRadioID;

        /// <summary>
        /// The current status of the radio.
        /// </summary> 
        [DataMember]
        private ZigBeeRadioStatus m_CurrentStatus;

        #endregion
    }
}
