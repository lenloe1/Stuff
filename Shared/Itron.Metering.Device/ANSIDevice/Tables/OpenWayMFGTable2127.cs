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
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class used for reading and validating the Enhanced Security Public Keys.
    /// </summary>

    internal class MFGTable2127EnhancedSecurityKeys : ANSISubTable
    {
        #region Constants

        private const int KEY_LENGTH = 37;
        private const int OFFSET = DataFlashBlockRecord.RECORD_SIZE * 762;
        private const ushort SIZE = DataFlashBlockRecord.RECORD_SIZE * 2;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/26/09 RCG 2.10.02        Created

        public MFGTable2127EnhancedSecurityKeys(CPSEM psem)
            : base(psem, 2127, OFFSET, SIZE)
        {
        }

        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/26/09 RCG 2.10.02        Created

        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_Block762 = new DataFlashBlockRecord();
                m_Block763 = new DataFlashBlockRecord();

                m_Block762.Parse(m_Reader);
                m_Block763.Parse(m_Reader);
            }

            return Result;
        }

        /// <summary>
        /// Validates the security keys in the meter against the keys that are specified in the file.
        /// </summary>
        /// <param name="EnhancedSecurityFile">The file that contains the public keys for the meters.</param>
        /// <returns>True of the keys are valid. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/26/09 RCG 2.10.02        Created

        public bool ValidateKeys(string EnhancedSecurityFile)
        {
            DKUSFile KeyFile = new DKUSFile(EnhancedSecurityFile);
            PSEMResponse Result = PSEMResponse.Ok;
            bool bAreKeysValid = true;
            byte[] CurrentMeterKey = new byte[KEY_LENGTH];
            int CurrentOffset = 0;

            if (State != TableState.Loaded)
            {
                Result = Read();

                if (Result != PSEMResponse.Ok)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                        Result, "Error Reading Enhanced Security Keys");
                }
            }

            // Pull the first 4 keys out of block 762 and validate them

            for (int iKey = 0; iKey < 4; iKey++)
            {
                // Get the key from the meter.
                Array.Copy(m_Block762.Data, CurrentOffset, CurrentMeterKey, 0, KEY_LENGTH);
                CurrentOffset += KEY_LENGTH;

                // Validate the key
                if (CheckKeysEqual(CurrentMeterKey, KeyFile.Keys[iKey]) == false)
                {
                    bAreKeysValid = false;
                    break;
                }
            }

            if (bAreKeysValid)
            {
                // Pull the last 2 keys out of block 763 and validate them
                CurrentOffset = 0;

                for (int iKey = 4; iKey < 6; iKey++)
                {
                    // Get the key from the meter.
                    Array.Copy(m_Block763.Data, CurrentOffset, CurrentMeterKey, 0, KEY_LENGTH);
                    CurrentOffset += KEY_LENGTH;

                    // Validate the key
                    if (CheckKeysEqual(CurrentMeterKey, KeyFile.Keys[iKey]) == false)
                    {
                        bAreKeysValid = false;
                        break;
                    }
                }
            }

            return bAreKeysValid;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks to see if the two keys are equal.
        /// </summary>
        /// <param name="firstKey">The first key to check</param>
        /// <param name="secondKey">The second key to check</param>
        /// <returns>True if the keys are equal. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/27/09 RCG 2.10.02        Created

        private bool CheckKeysEqual(byte[] firstKey, byte[] secondKey)
        {
            bool IsEqual = true;

            if (firstKey == null || secondKey == null || firstKey.Length != secondKey.Length)
            {
                IsEqual = false;
            }
            else
            {
                for (int iIndex = 0; iIndex < firstKey.Length; iIndex++)
                {
                    if (firstKey[iIndex] != secondKey[iIndex])
                    {
                        IsEqual = false;
                        break;
                    }
                }
            }

            return IsEqual;
        }

        #endregion

        #region Member Variables

        private DataFlashBlockRecord m_Block762;
        private DataFlashBlockRecord m_Block763;

        #endregion
    }

    /// <summary>
    /// Class that represents a single block of the Data Flash table.
    /// </summary>

    internal class DataFlashBlockRecord
    {

        #region Constants

        public const ushort RECORD_SIZE = 264;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/26/09 RCG 2.10.02        Created

        public DataFlashBlockRecord()
        {
        }

        /// <summary>
        /// Parse the Data Flash block from the specified binary reader.
        /// </summary>
        /// <param name="Reader">The binary reader that contains the data for the block</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/26/09 RCG 2.10.02        Created

        public void Parse(PSEMBinaryReader Reader)
        {
            m_Data = Reader.ReadBytes(256);
            m_WriteCount = Reader.ReadUInt16();
            m_CRC = Reader.ReadUInt16();
            m_Reserved = Reader.ReadUInt32();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the data that is contained in the Data Flash Block
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/26/09 RCG 2.10.02        Created

        public byte[] Data
        {
            get
            {
                return m_Data;
            }
        }

        /// <summary>
        /// Gets the number of times this block has been written to.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/26/09 RCG 2.10.02        Created

        public ushort WriteCount
        {
            get
            {
                return m_WriteCount;
            }
        }

        /// <summary>
        /// Gets the CRC for the data in the block
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/26/09 RCG 2.10.02        Created

        public ushort CRC
        {
            get
            {
                return m_CRC;
            }
        }

        #endregion

        #region Member Variables

        private byte[] m_Data;
        private ushort m_WriteCount;
        private ushort m_CRC;
        private uint m_Reserved;

        #endregion
    }
}
