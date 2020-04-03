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
//                           Copyright © 2009
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using Itron.Metering.Utilities;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    /// <summary>
    /// The PSEM Security level.
    /// </summary>
    public enum SecurityLevel : byte
    {
        /// <summary>
        /// Level 0 - None
        /// </summary>
        [EnumDescription("Level 0")]
        Level0 = 0,
        /// <summary>
        /// Level 1 - Quaternary
        /// </summary>
        [EnumDescription("Level 1")]
        Level1 = 1,
        /// <summary>
        /// Level 2 - Tertiary
        /// </summary>
        [EnumDescription("Level 2")]
        Level2 = 2,
        /// <summary>
        /// Level 3 - Secondary
        /// </summary>
        [EnumDescription("Level 3")]
        Level3 = 3,
        /// <summary>
        /// Level 4 - Primary
        /// </summary>
        [EnumDescription("Level 4")]
        Level4 = 4,
    }

    /// <summary>
    /// Data object for the OpenWay Signed Authorization key
    /// </summary>
    [Serializable]
    [XmlRoot(IsNullable = true, ElementName = "SignedAuthorizationKey")]
    public class SignedAuthorizationKey
    {
        #region Constants

        private const int DATA_START_POSITION = 175;

        private const string DATA = "data";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/16/09 RCG 2.30.10	    Created

        public SignedAuthorizationKey()
        {
            InitializeData();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">The authorization key data</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/16/09 RCG 2.30.10	    Created

        public SignedAuthorizationKey(byte[] data)
        {
            m_Data = data;
            ParseData();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the encrypted data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/09 RCG 2.30.11	    Created

        [XmlElement(IsNullable=true, Type=typeof(byte[]), ElementName=DATA)]
        public byte[] EncryptedData
        {
            get
            {
                if (m_Data != null)
                {
                    MemoryStream DataStream = new MemoryStream(m_Data);
                    MemoryStream EncryptedStream = new MemoryStream();

                    Encryption.EncryptData(CreateEncryptionAlgorithm(), DataStream, EncryptedStream);

                    return EncryptedStream.ToArray();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    MemoryStream EncryptedStream = new MemoryStream(value);
                    MemoryStream DataStream = new MemoryStream();

                    Encryption.DecryptData(CreateEncryptionAlgorithm(), EncryptedStream, DataStream);

                    m_Data = DataStream.ToArray();
                    ParseData();
                }
                else
                {
                    InitializeData();
                }
            }
        }

        /// <summary>
        /// Gets the full key Data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/09 RCG 2.30.11	    Created

        public byte[] Data
        {
            get
            {
                return m_Data;
            }
        }

        /// <summary>
        /// Gets the start date for the credential
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/09 RCG 2.30.11	    Created

        public DateTime StartDate
        {
            get
            {
                return m_StartDate;
            }
        }

        /// <summary>
        /// Gets the end date for the credential
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/09 RCG 2.30.11	    Created

        public DateTime EndDate
        {
            get
            {
                return m_EndDate;
            }
        }

        /// <summary>
        /// Gets the manufacturer serial number for the authorized device.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/09 RCG 2.30.11	    Created

        public string MFGSerialNumber
        {
            get
            {
                return m_MFGSerialNumber;
            }
        }

        /// <summary>
        /// Gets the user name for the authorized user.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/09 RCG 2.30.11	    Created

        public string UserName
        {
            get
            {
                return m_UserName;
            }
        }

        /// <summary>
        /// Gets the User ID for the authorized user.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/09 RCG 2.30.11	    Created

        public ushort UserID
        {
            get
            {
                return m_UserID;
            }
        }

        /// <summary>
        /// Gets the authorized security level.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/09 RCG 2.30.11	    Created

        public SecurityLevel Level
        {
            get
            {
                return (SecurityLevel)m_PasswordRef;
            }
        }

        /// <summary>
        /// Gets whether or not the key is currently valid based on the time.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/09 RCG 2.30.11	    Created

        public bool IsValid
        {
            get
            {
                return StartDate <= DateTime.Now && EndDate >= DateTime.Now;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the encryption algorithm to use.
        /// </summary>
        /// <returns>The encryption algorithm</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/16/09 RCG 2.30.10	    Created

        protected virtual SymmetricAlgorithm CreateEncryptionAlgorithm()
        {
            TripleDES Algorithm = TripleDES.Create();
            SecureDataStorage DataStorage = new SecureDataStorage(SecureDataStorage.DEFAULT_LOCATION);

            Algorithm.KeySize = 192;
            Algorithm.Key = DataStorage.RetrieveSecureData(SecureDataStorage.SIGNED_AUTH_KEY_ID);
            Algorithm.IV = DataStorage.RetrieveSecureData(SecureDataStorage.SIGNED_AUTH_IV_ID);

            return Algorithm;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the member variables.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/16/09 RCG 2.30.10	    Created

        private void InitializeData()
        {
            m_Data = null;
            m_EndDate = DateTime.MinValue;
            m_StartDate = DateTime.MinValue;
            m_UserID = 0;
            m_UserName = null;
            m_MFGSerialNumber = null;
        }

        /// <summary>
        /// Parses the Authorization information from the data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/16/09 RCG 2.30.10	    Created

        private void ParseData()
        {
            MemoryStream DataStream = new MemoryStream(m_Data);
            PSEMBinaryReader BinaryReader = new PSEMBinaryReader(DataStream);

            DataStream.Position = DATA_START_POSITION;

            // These dates are stored as GMT so we need to convert them to local time.
            m_StartDate = BinaryReader.ReadSTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME).ToLocalTime();
            m_EndDate = BinaryReader.ReadSTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME).ToLocalTime();
            m_PasswordRef = BinaryReader.ReadByte();
            m_UserID = BinaryReader.ReadUInt16();
            m_MFGSerialNumber = BinaryReader.ReadString(16).Trim();
            m_UserName = BinaryReader.ReadString(10);
        }

        #endregion

        #region Member Variables

        private byte[] m_Data;
        private DateTime m_StartDate;
        private DateTime m_EndDate;
        private byte m_PasswordRef;
        private ushort m_UserID;
        private string m_MFGSerialNumber;
        private string m_UserName;

        #endregion
    }

    /// <summary>
    /// Data object for the RMA Signed Authorization key. The intention here
    /// is that this key is encrypted differently so that one can not copy this
    /// key into Field-Pro to use.
    /// </summary>
    [Serializable]
    [XmlRoot(IsNullable = true, ElementName = "RMASignedAuthorizationKey")]
    public class RMASignedAuthorizationKey : SignedAuthorizationKey
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/10/09 RCG 2.30.16	    Created

        public RMASignedAuthorizationKey()
            : base()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">The Authorization key data</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/10/09 RCG 2.30.16	    Created

        public RMASignedAuthorizationKey(byte[] data)
            : base(data)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the encryption algorithm to use.
        /// </summary>
        /// <returns>The encryption algorithm</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/10/09 RCG 2.30.16	    Created

        protected override SymmetricAlgorithm CreateEncryptionAlgorithm()
        {
            TripleDES Algorithm = TripleDES.Create();
            SecureDataStorage DataStorage = new SecureDataStorage(SecureDataStorage.DEFAULT_LOCATION);

            Algorithm.KeySize = 192;
            Algorithm.Key = DataStorage.RetrieveSecureData(SecureDataStorage.RMA_SIGNED_AUTH_KEY_ID);
            Algorithm.IV = DataStorage.RetrieveSecureData(SecureDataStorage.RMA_SIGNED_AUTH_IV_ID);

            return Algorithm;
        }

        #endregion
    }
}
