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
//                           Copyright © 2012 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Sub table into the 2061 factory data table that is used to validate the HAN link keys.
    /// </summary>
    internal class MFGTable2061HANSecurityKeys : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 461;
        private const ushort TABLE_LENGTH = 34;
        private const int KEY_LENGTH = 16;
        private const int NUM_KEYS = 2;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/16/08 RCG 1.50.36        Created

        public MFGTable2061HANSecurityKeys(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/16/08 RCG 1.50.36        Created
        //  06/24/08 AF  1.50.42        KeyType property was changed to an enum, which
        //                              required a cast for the read

        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_Keys = new HANKeyRcd[NUM_KEYS];

                for (int iIndex = 0; iIndex < NUM_KEYS; iIndex++)
                {
                    m_Keys[iIndex] = new HANKeyRcd();
                    m_Keys[iIndex].KeyType = (HANKeyRcd.HANKeyType)(m_Reader.ReadByte());
                    m_Keys[iIndex].HANKey = m_Reader.ReadBytes(KEY_LENGTH);
                }

            }

            return Result;
        }

        /// <summary>
        /// Validates the specified key against the key in the meter.
        /// </summary>
        /// <param name="Key">The key to validate.</param>
        /// <returns>True if the keys match. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/16/08 RCG 1.50.36        Created

        public bool ValidateKey(HANKeyRcd Key)
        {
            PSEMResponse Result = PSEMResponse.Ok;
            bool bIsKeyValid = false;

            if (State != TableState.Loaded)
            {
                Result = Read();

                if (Result != PSEMResponse.Ok)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, 
                        Result, "Error Reading Link Keys");
                }
            }

            // Check each of the keys since we may not be able to guarantee order.
            foreach (HANKeyRcd CurrentKey in m_Keys)
            {
                if (Key.Equals(CurrentKey) == true)
                {
                    bIsKeyValid = true;
                    break;
                }
            }

            return bIsKeyValid;
        }

        #endregion

        #region Member Variables

        private HANKeyRcd[] m_Keys;

        #endregion

    }

    /// <summary>
    /// Sub table into the 2061 factory data table that is used to validate the RFLAN Opt Out.
    /// </summary>
    public class Table2061RFLANOptOut : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 202;
        private const int TABLE_LENGTH = 1;
        private const int TABLE_TIME_OUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/12 MSC 2.53.30            Created
        public Table2061RFLANOptOut(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH, TABLE_TIME_OUT)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the Opt Out Byte.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/12 MSC 2.53.30            Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_bytOptOut = m_Reader.ReadByte();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/12 MSC 2.53.30            Created
        //
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported until this table is fully implemented!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/12 MSC 2.53.30            Created
        //
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported until this table is fully implemented!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the RFLAN Opt Out Byte from Factory Data: 0 = enabled, 1 = disabled. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/18/12 MSC 2.53.30            Created
        public byte RFLANOptOutByte
        {
            get
            {
                ReadUnloadedTable();
                return m_bytOptOut;
            }
        }

        #endregion

        #region Member Variables

        private byte m_bytOptOut;

        #endregion
    }

    /// <summary>
    /// Sub table into the 2061 factory data table that is used to validate the RFLAN Configuration.
    /// </summary>
    public class MFGTable2061RFLANConfig : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 349;
        private const ushort TABLE_LENGTH = 37;
        private const ushort UTILITY_ID_OFFSET = 30;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/21/09 jrf 2.30.12         Created
        //
        public MFGTable2061RFLANConfig(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the utility ID.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/21/09 jrf 2.30.12        Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                //Add in other items as needed.
                m_Reader.ReadUInt16();
                m_Reader.ReadUInt16();
                m_Reader.ReadByte();
                m_Reader.ReadUInt16();
                m_Reader.ReadUInt32();
                m_Reader.ReadUInt32();
                m_Reader.ReadUInt16();
                m_Reader.ReadByte();
                m_Reader.ReadUInt32();
                m_Reader.ReadUInt32();

                // RFLAN MAC Address
                m_uiRFLANMACAddress = m_Reader.ReadUInt32();

                //Utility ID
                m_bytUtilityID = m_Reader.ReadByte();

                //Add in other items as needed.
                m_Reader.ReadByte();
                m_Reader.ReadByte();
                m_Reader.ReadByte();
                m_Reader.ReadByte();
                m_Reader.ReadByte();
                m_Reader.ReadByte();
                
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/21/09 jrf 2.30.12        Created
        //
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported until this table is fully implemented!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/21/09 jrf 2.30.12        Created
        //
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported until this table is fully implemented!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets/sets the Utility ID.  Setting this property will 
        /// write the value to the table and cause the meter to 3 button reset 
        /// after logging off.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/21/09 jrf 2.30.12        Created
        //
        public byte UtilityID
        {
            get
            {
                ReadUnloadedTable();
                return m_bytUtilityID;
            }
            set
            {
                m_DataStream.Position = UTILITY_ID_OFFSET;
                m_Writer.Write(value);

                base.Write(UTILITY_ID_OFFSET, 1);
            }
        }

        /// <summary>
        /// Gets the RFLAN MAC address from factory data.  Most
        /// functions should use Mfg Table 20 for this but this was
        /// needed for the Boron Upgrade Tool
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/14/11 AF  2.53.06        Created
        //
        public UInt32 RFLANMacAddress
        {
            get
            {
                ReadUnloadedTable();
                return m_uiRFLANMACAddress;
            }
        }

       


        #endregion

        #region Member Variables

        private UInt32 m_uiRFLANMACAddress;
        private byte m_bytUtilityID;

        #endregion

    }



    /// <summary>
    /// Sub table into the 2061 factory data table to read/write Canadian Meter Values
    /// </summary>
    public class MFGTable2061CanadianConfig : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 136;
        private const ushort TABLE_LENGTH = 2;
        private const ushort CANADIAN_METER_OFFSET = 0;
        private const ushort CANADIAN_SEALED_OFFSET = 1;  // Do Canadian Seals say Eh?


        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/05/10 BLC 2.30.12         Created
        //
        public MFGTable2061CanadianConfig(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the utility ID.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/05/10 jrf 2.30.12        Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_bytCanadianMeter = m_Reader.ReadByte();
                m_bytCanadianSealed = m_Reader.ReadByte();               
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/05/10 BLC 2.30.12        Created
        //
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported until this table is fully implemented!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/05/10 BLC 2.30.12        Created
        //
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported until this table is fully implemented!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets/sets the Canadian Meter.  Setting this property will 
        /// write the value to the table and cause the meter to 3 button reset 
        /// after logging off.
        /// This changes a meter into a Canadian Meter.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/05/10 BLC 2.30.12        Created
        //
        public byte CanadianMeter
        {
            get
            {
                ReadUnloadedTable();
                return m_bytCanadianMeter;
            }
            set
            {
                m_DataStream.Position = CANADIAN_METER_OFFSET;
                m_Writer.Write(value);

                base.Write(CANADIAN_METER_OFFSET, 1);
            }
        }

        /// <summary>
        /// This property gets/sets the Canadian Seal.  Setting this property will 
        /// write the value to the table and cause the meter to 3 button reset 
        /// after logging off.
        /// Once a meter is converted to Canadian, then it is sealed to stop changes
        /// to various tables as outlined OpenWay Centron Candian Firmware Design
        /// by Dan Fasalino
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/05/10 BLC 2.30.12        Created
        //
        public byte CanadianSealed
        {
            get
            {
                ReadUnloadedTable();
                return m_bytCanadianSealed;
            }
            set
            {
                m_DataStream.Position = CANADIAN_SEALED_OFFSET;
                m_Writer.Write(value);

                base.Write(CANADIAN_SEALED_OFFSET, 1);
            }
        }

        #endregion

        #region Member Variables

        private byte m_bytCanadianMeter;
        private byte m_bytCanadianSealed;

        #endregion

    }

    /// <summary>
    /// Sub table into the 2061 factory data table to read/write Misc Meter Values
    /// </summary>
    public class MFGTable2061MiscFactoryData : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 193;
        private const ushort TABLE_LENGTH = 2;
        private const ushort FACTORY_FLAGS_OFFSET = 1;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/14/11 MAH 2.51.11         Created
        //
        public MFGTable2061MiscFactoryData(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/14/11 MAH 2.51.11         Created
        //  01/21/15 jrf 4.50.40 TC11229 Added reading mfg. as msm byte.
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_ManufacturedAsMSM = m_Reader.ReadByte();
                m_bytFactoryFlags = m_Reader.ReadByte();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/14/11 MAH 2.51.11         Created
        //
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported until this table is fully implemented!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/14/11 MAH 2.51.11         Created

        //
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported until this table is fully implemented!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets/sets the Canadian Meter.  Setting this property will 
        /// write the value to the table and cause the meter to 3 button reset 
        /// after logging off.
        /// This changes a meter into a Canadian Meter.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/14/11 MAH 2.51.11         Created
        //  03/09/15 jrf 4.50.75 565759 This sub table was previously modified to read two bytes. 
        //                              Updated the table write offset in this property's set 
        //                              to account for this.
        public bool FactoryTestingComplete
        {
            get
            {
                ReadUnloadedTable();

                if ((m_bytFactoryFlags & 0x01) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                ReadUnloadedTable();

                if (value) // set bit 0 
                {
                    m_bytFactoryFlags = (byte)(m_bytFactoryFlags | 0x01);
                }
                else // Clear bit 0
                {
                    m_bytFactoryFlags = (byte)(m_bytFactoryFlags & 0xFE);
                }
                
                m_DataStream.Position = FACTORY_FLAGS_OFFSET;
                m_Writer.Write(m_bytFactoryFlags);

                base.Write(FACTORY_FLAGS_OFFSET, 1);
            }
        }

        /// <summary>
        /// This property gets the Manufactured as MSM value. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/21/15 jrf 4.50.40 TC11229 Created
        // 
        public byte ManufacturedAsMSM
        {
            get
            {
                ReadUnloadedTable();

                return m_ManufacturedAsMSM;
            }
        }

        #endregion

        #region Member Variables

        private byte m_bytFactoryFlags;
        private byte m_ManufacturedAsMSM = 0xFF;

        #endregion

    }


    /// <summary>
    /// Sub table into the 2061 factory data table to read/write Line Frequency
    /// </summary>
    public class MFGTable2061LineFrequency : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 236;
        private const ushort TABLE_LENGTH = 1;
        private const ushort FREQUENCY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/31/13 jkw 2.70.62         Created
        //
        public MFGTable2061LineFrequency(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/31/13 jkw 2.70.62         Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_bytFrequency = m_Reader.ReadByte();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/31/13 jkw 2.70.62         Created
        //
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported until this table is fully implemented!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/31/13 jkw 2.70.62         Created

        //
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported until this table is fully implemented!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets/sets the Canadian Meter.  Setting this property will 
        /// write the value to the table and cause the meter to 3 button reset 
        /// after logging off.
        /// This changes a meter into a Canadian Meter.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/14/11 MAH 2.51.11         Created
        //
        public byte Frequency
        {
            get
            {
                ReadUnloadedTable();
                return m_bytFrequency;
            }
            set
            {
                m_DataStream.Position = FREQUENCY_OFFSET;
                m_Writer.Write(value);

                base.Write(FREQUENCY_OFFSET, 1);
            }
        }
        #endregion

        #region Member Variables

        private byte m_bytFrequency;

        #endregion
    }

    /// <summary>
    /// Sub table into the 2061 factory data table to read/write RCD Threshold Value
    /// </summary>
    public class MFGTable2061RCDThreshold : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 191;
        private const ushort TABLE_LENGTH =1;
        private const ushort RCD_THRESHOLD_OFFSET = 0;
       


        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/12/11 MMD                Created
        //
        public MFGTable2061RCDThreshold(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the RCD ThresholdValue
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/12/11 MMD                Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_bytRCDThreshold = m_Reader.ReadByte();
           
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/12/11 MMD                Created
        //
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported until this table is fully implemented!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/12/11 MMD                Created
        //
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported until this table is fully implemented!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets/sets the RCD Threshold Value.  Setting this property will 
        /// write the value to the table and cause the meter to 3 button reset 
        /// after logging off.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/12/11 MMD                Created
        //
        public byte RCDThreshold
        {
            get
            {
                ReadUnloadedTable();
                return m_bytRCDThreshold;
            }
            set
            {
                m_DataStream.Position = RCD_THRESHOLD_OFFSET;
                m_Writer.Write(value);

                base.Write(RCD_THRESHOLD_OFFSET, 1);
            }
        }

   
        #endregion

        #region Member Variables

        private byte m_bytRCDThreshold;
      

        #endregion

    }

    /// <summary>
    /// Sub table into the 2061 factory data table to read/write Extended Power Outage Values
    /// </summary>
    public class MFGTable2061ExtendedOutageConfig : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 196;
        private const ushort TABLE_LENGTH = 5;
        private const ushort EXTENDED_OUTAGE_CONFIG_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/11 MAH 2.50.30         Created
        //
        public MFGTable2061ExtendedOutageConfig(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the extended outage config byte.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/11 MAH 2.50.30         Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_bytExtendedOutageConfig = m_Reader.ReadByte();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/11 MAH 2.50.30         Created
        //
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported until this table is fully implemented!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/11 MAH 2.50.30         Created
        //
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported until this table is fully implemented!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property enables/disabled extended outages.  Setting this property will 
        /// write the value to the table and cause the meter to 3 button reset 
        /// after logging off.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/11 MAH 2.50.30         Created
        //
        public bool ExtendedOutage
        {
            get
            {
                ReadUnloadedTable();

                if ( m_bytExtendedOutageConfig == 0xAA )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                m_DataStream.Position = EXTENDED_OUTAGE_CONFIG_OFFSET;

                if (value)
                {
                    m_Writer.Write(0xAA);
                }
                else
                {
                    m_Writer.Write(0x00);
                }

                base.Write(EXTENDED_OUTAGE_CONFIG_OFFSET, 1);
            }
        }

        #endregion

        #region Member Variables

        private byte m_bytExtendedOutageConfig;

        #endregion

    }

    /// <summary>
    /// Sub table into the 2061 factory data table to read Energy 1 Meter key Bit
    /// </summary>
    public class MFGTable2061Energy1MeterKey : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 8;
        private const ushort TABLE_LENGTH = 4;
        private const ushort ENERGY_1_KEY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  07/25/16 jrf TC 4.70.09 63216  Created
        public MFGTable2061Energy1MeterKey(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the Energy 1 Meter key byte.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  07/25/16 jrf TC 4.70.09 63216  Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_Energy1MeterKey = m_Reader.ReadUInt32();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  07/25/16 jrf TC 4.70.09 63216  Created
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  07/25/16 jrf TC 4.70.09 63216  Created
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets and sets the energy 1 meter key bit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  07/25/16 jrf TC 4.70.09 63216  Created
        public UInt32 Energy1MeterKey
        {
            get
            {
                Read();
                return m_Energy1MeterKey;

            }
            set
            {
                m_DataStream.Position = ENERGY_1_KEY_OFFSET;


                m_Writer.Write(value);


                base.Write(ENERGY_1_KEY_OFFSET, 4);
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_Energy1MeterKey;

        #endregion

    }

    /// <summary>
    /// Sub table into the 2061 factory data table to read Demand Meter key Bit
    /// </summary>
    public class MFGTable2061DemandMeterKey : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 12;
        private const ushort TABLE_LENGTH = 4;
        private const ushort DEMAND_KEY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public MFGTable2061DemandMeterKey(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the Demand Meter key byte.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_DemandMeterKey = m_Reader.ReadUInt32();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets and sets the demand meter key bit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public UInt32 DemandMeterKey
        {
            get
            {
                Read();
                return m_DemandMeterKey;

            }
            set
            {
                m_DataStream.Position = DEMAND_KEY_OFFSET;


                m_Writer.Write(value);


                base.Write(DEMAND_KEY_OFFSET, 4);
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_DemandMeterKey;

        #endregion

    }

    /// <summary>
    /// Sub table into the 2061 factory data table to read TOU Meter key Bit
    /// </summary>
    public class MFGTable2061TOUMeterKey : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 16;
        private const ushort TABLE_LENGTH = 4;
        private const ushort TOU_KEY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public MFGTable2061TOUMeterKey(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the TOU Meter key byte.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_TOUMeterKey = m_Reader.ReadUInt32();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets and sets the TOU meter key bit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public UInt32 TOUMeterKey
        {
            get
            {
                Read();
                return m_TOUMeterKey;

            }
            set
            {
                m_DataStream.Position = TOU_KEY_OFFSET;


                m_Writer.Write(value);


                base.Write(TOU_KEY_OFFSET, 4);
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_TOUMeterKey;

        #endregion

    }

    /// <summary>
    /// Sub table into the 2061 factory data table to read Load Profile Meter key Bit
    /// </summary>
    public class MFGTable2061LoadProfileMeterKey : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 20;
        private const ushort TABLE_LENGTH = 4;
        private const ushort LOAD_PROFILE_KEY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public MFGTable2061LoadProfileMeterKey(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the Load Profile Meter key byte.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_LoadProfileMeterKey = m_Reader.ReadUInt32();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets and sets the Load Profile meter key bit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public UInt32 LoadProfileMeterKey
        {
            get
            {
                Read();
                return m_LoadProfileMeterKey;

            }
            set
            {
                m_DataStream.Position = LOAD_PROFILE_KEY_OFFSET;


                m_Writer.Write(value);


                base.Write(LOAD_PROFILE_KEY_OFFSET, 4);
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_LoadProfileMeterKey;

        #endregion

    }

    /// <summary>
    /// Sub table into the 2061 factory data table to read Power Quality Meter key Bit
    /// </summary>
    public class MFGTable2061PowerQualityMeterKey : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 24;
        private const ushort TABLE_LENGTH = 4;
        private const ushort POWER_QUALITY_KEY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public MFGTable2061PowerQualityMeterKey(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the Power Quality Meter key byte.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_PowerQualityMeterKey = m_Reader.ReadUInt32();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets and sets the Power Quality meter key bit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public UInt32 PowerQualityMeterKey
        {
            get
            {
                Read();
                return m_PowerQualityMeterKey;

            }
            set
            {
                m_DataStream.Position = POWER_QUALITY_KEY_OFFSET;


                m_Writer.Write(value);


                base.Write(POWER_QUALITY_KEY_OFFSET, 4);
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_PowerQualityMeterKey;

        #endregion

    }

    /// <summary>
    /// Sub table into the 2061 factory data table to read/write Misc Meter key Bit
    /// </summary>
    public class MFGTable2061MiscMeterKey : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 28;
        private const ushort TABLE_LENGTH = 4;
        private const ushort MISC_KEY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/11 MMD 2.50.30         Created
        //
        public MFGTable2061MiscMeterKey(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the Misc Meter key byte.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/11 MMD 2.50.30         Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_uiMeterKeyBit = m_Reader.ReadUInt32();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/11 MMD 2.50.30         Created
        //
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported until this table is fully implemented!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/11 MMD 2.50.30         Created
        //
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported until this table is fully implemented!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets and sets the Misc meter key bit for injecting keys
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/11 MMD 2.50.30         Created
        //
        public UInt32 MiscMeterKey
        {
            get
            {
                Read();
                return m_uiMeterKeyBit;

             }
            set
            {
                m_DataStream.Position = MISC_KEY_OFFSET;


                m_Writer.Write(value);


                base.Write(MISC_KEY_OFFSET, 4);
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_uiMeterKeyBit;

        #endregion

    }

    /// <summary>
    /// Sub table into the 2061 factory data table to read IO Meter key Bit
    /// </summary>
    public class MFGTable2061IOMeterKey : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 32;
        private const ushort TABLE_LENGTH = 4;
        private const ushort IO_KEY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public MFGTable2061IOMeterKey(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the IO Meter key byte.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_IOMeterKey = m_Reader.ReadUInt32();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets and sets the IO meter key bit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public UInt32 IOMeterKey
        {
            get
            {
                Read();
                return m_IOMeterKey;

            }
            set
            {
                m_DataStream.Position = IO_KEY_OFFSET;


                m_Writer.Write(value);


                base.Write(IO_KEY_OFFSET, 4);
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_IOMeterKey;

        #endregion

    }


    /// <summary>
    /// Sub table into the 2061 factory data table to read Option Board Meter key Bit
    /// </summary>
    public class MFGTable2061OptionBoardMeterKey : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 36;
        private const ushort TABLE_LENGTH = 4;
        private const ushort OPTION_BOARD_KEY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public MFGTable2061OptionBoardMeterKey(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the Option Board Meter key byte.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_OptionBoardMeterKey = m_Reader.ReadUInt32();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets and sets the Option Board meter key bit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public UInt32 OptionBoardMeterKey
        {
            get
            {
                Read();
                return m_OptionBoardMeterKey;

            }
            set
            {
                m_DataStream.Position = OPTION_BOARD_KEY_OFFSET;


                m_Writer.Write(value);


                base.Write(OPTION_BOARD_KEY_OFFSET, 4);
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_OptionBoardMeterKey;

        #endregion

    }

    /// <summary>
    /// Sub table into the 2061 factory data table to read Instantaneous Meter key Bit
    /// </summary>
    public class MFGTable2061InstantaneousMeterKey : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 40;
        private const ushort TABLE_LENGTH = 4;
        private const ushort INSTANTANEOUS_KEY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public MFGTable2061InstantaneousMeterKey(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the Instantaneous Meter key byte.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_InstantaneousMeterKey = m_Reader.ReadUInt32();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets and sets the Instantaneous meter key bit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public UInt32 InstantaneousMeterKey
        {
            get
            {
                Read();
                return m_InstantaneousMeterKey;

            }
            set
            {
                m_DataStream.Position = INSTANTANEOUS_KEY_OFFSET;


                m_Writer.Write(value);


                base.Write(INSTANTANEOUS_KEY_OFFSET, 4);
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_InstantaneousMeterKey;

        #endregion

    }

    /// <summary>
    /// Sub table into the 2061 factory data table to read Self Read Meter key Bit
    /// </summary>
    public class MFGTable2061SelfReadMeterKey : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 44;
        private const ushort TABLE_LENGTH = 4;
        private const ushort SELF_READ_KEY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public MFGTable2061SelfReadMeterKey(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the Self Read Meter key byte.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_SelfReadMeterKey = m_Reader.ReadUInt32();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets and sets the Self Read meter key bit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public UInt32 SelfReadMeterKey
        {
            get
            {
                Read();
                return m_SelfReadMeterKey;

            }
            set
            {
                m_DataStream.Position = SELF_READ_KEY_OFFSET;


                m_Writer.Write(value);


                base.Write(SELF_READ_KEY_OFFSET, 4);
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_SelfReadMeterKey;

        #endregion

    }

    /// <summary>
    /// Sub table into the 2061 factory data table to read Calendar Meter key Bit
    /// </summary>
    public class MFGTable2061CalendarMeterKey : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 48;
        private const ushort TABLE_LENGTH = 4;
        private const ushort CALENDAR_KEY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public MFGTable2061CalendarMeterKey(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the Calendar Meter key byte.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_CalendarMeterKey = m_Reader.ReadUInt32();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets and sets the Calendar meter key bit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public UInt32 CalendarMeterKey
        {
            get
            {
                Read();
                return m_CalendarMeterKey;

            }
            set
            {
                m_DataStream.Position = CALENDAR_KEY_OFFSET;


                m_Writer.Write(value);


                base.Write(CALENDAR_KEY_OFFSET, 4);
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_CalendarMeterKey;

        #endregion

    }

    /// <summary>
    /// Sub table into the 2061 factory data table to read Energy 2 Meter key Bit
    /// </summary>
    public class MFGTable2061Energy2MeterKey : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 52;
        private const ushort TABLE_LENGTH = 4;
        private const ushort ENERGY_2_KEY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  07/25/16 jrf TC 4.70.09 63216  Created
        public MFGTable2061Energy2MeterKey(CPSEM psem)
            : base(psem, 2061, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the Energy 2 Meter key byte.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  07/25/16 jrf TC 4.70.09 63216  Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_Energy2MeterKey = m_Reader.ReadUInt32();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  07/25/16 jrf TC 4.70.09 63216  Created
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  07/25/16 jrf TC 4.70.09 63216  Created
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets and sets the energy 1 meter key bit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  07/25/16 jrf TC 4.70.09 63216  Created
        public UInt32 Energy2MeterKey
        {
            get
            {
                Read();
                return m_Energy2MeterKey;

            }
            set
            {
                m_DataStream.Position = ENERGY_2_KEY_OFFSET;


                m_Writer.Write(value);


                base.Write(ENERGY_2_KEY_OFFSET, 4);
            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_Energy2MeterKey;

        #endregion

    }


    /// <summary>
    /// Full table read/write 2061 factory data table
    /// </summary>
    /// 
    // Revision History	
    // MM/DD/YY who  Version Issue# Description
    // -------- ---  ------- ------ -------------------------------------------
    // 12/06/11 BLC  7.35.00  N/A	Created

    public class OpenWayMfgTable2061 : AnsiTable
    {

        #region Constants

        private const int TABLE_TIMEOUT = 5000;
        private const int MCU_TABLE_LENGTH_2061 = 501;
        private const uint PASSCODE_SIZE = 20;
        private const uint RESERVED_BYTE_LENGTH = 27;
        private const uint SERIAL_NUMBER_LENGTH = 16;
        private const uint SPECIFICATION_NUMBER_LENGTH = 10;
        private const uint SECURITY_KEY_LENGTH = 27;
        private const uint POWER_TABLE_LENGTH = 8;
        private const uint ESN_LENGTH = 20;
        private const uint HAN_MAC_LENGTH = 8;
        private const uint HAN_KEY_LENGTH = 16;

        #endregion Constants


        #region Public Methods


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/12 BLC         N/A    Created for OpenWay
        public OpenWayMfgTable2061(CPSEM psem)
            : base(psem, 2061, MCU_TABLE_LENGTH_2061, TABLE_TIMEOUT)
        {
            InitializeVariables();
        }



        /// <summary>
        /// Reads the data from the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/12 BLC         N/A    Created for OpenWay

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            //m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable27.Read");

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Writes the data to the meter.
        /// </summary>
        /// <returns>The result of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/18 BLC         N/A    Created for OpenWay

        public override PSEMResponse Write()
        {

            // Resynch our members to the base's data array
            m_DataStream.Position = 0;

            // m_Writer.Write((short)m_iNormalKh);



            // METER KEYS
            m_Writer.Write((byte)m_SW_Version);              // OFFSET 0 UINT8 Meter_Key.SW_Version;
            m_Writer.Write((byte)m_SW_Revision);            // UINT8 Meter_Key.SW_Revision;
            m_Writer.Write((short)m_Flavor);                // UINT16 Meter_Key.Flavor;
            m_Writer.Write((UInt32)m_TimeStamp);             // UINT32 Meter_Key.Time_Stamp;
            m_Writer.Write((UInt32)m_Energy1);               // UINT32 Meter_Key.Energy_1;
            m_Writer.Write((UInt32)m_Demand);                // UINT32 Meter_Key.Demand;
            m_Writer.Write((UInt32)m_TOU);                   // UINT32 Meter_Key.TOU;
            m_Writer.Write((UInt32)m_LoadProfile);  // UINT32 Meter_Key.Load_Prof;
            m_Writer.Write((UInt32)m_PowerQuality); // UINT32 Meter_Key.Power_Quality;
            m_Writer.Write((UInt32)m_Misc);         // UINT32 Meter_Key.Misc;
            m_Writer.Write((UInt32)m_IO);           // UINT32 Meter_Key.IO; ITR1;
            m_Writer.Write((UInt32)m_OptionBoard);  // UINT32 Meter_Key.Opt_Brd;
            m_Writer.Write((UInt32)m_Instant);      // UINT32 Meter_Key.Instant;
            m_Writer.Write((UInt32)m_SelfRead);     // UINT32 Meter_Key.Self_Read;
            m_Writer.Write((UInt32)m_Calendar);     // UINT32 Meter_Key.Calendar;
            m_Writer.Write((UInt32)m_Energy2);      // OFFSET 52 UINT32 Meter_Key.Energy_2;

            // Pass Codes
            m_Writer.Write(m_PassCode0);
            m_Writer.Write(m_PassCode1);
            m_Writer.Write(m_PassCode2);
            m_Writer.Write(m_PassCode3);


            // Canadian Meter
            m_Writer.Write((byte)m_CanadianMeter);   // OFFSET 136
            m_Writer.Write((byte)m_CanadianSealed);

            // RFLAN Expansion
            m_Writer.Write((UInt32)m_BroadcastTimeOut);
            m_Writer.Write((byte)m_BroadcastRetries);
            m_Writer.Write((byte)m_LLC_Tx_RetryExpansionOffset);
            m_Writer.Write((byte)m_LLC_DownlinkRetries);
            m_Writer.Write((byte)m_API_MinSendInterval);
            m_Writer.Write((byte)m_NET_MaxLife);
            m_Writer.Write((short)m_NET_RouteEntryMaxLife);
            m_Writer.Write((short)m_NET_SendRegMin);
            m_Writer.Write((short)m_NET_SendRegMax);
            m_Writer.Write((short)m_NET_RegInterval);
            m_Writer.Write((byte)m_NET_MaxRegAttempts);
            m_Writer.Write((byte)m_API_NetTimeRefreshPeriod);
            m_Writer.Write((byte)m_RFLAN_ControlBits);
            m_Writer.Write((byte)m_MAC_CellSwitchProbability);
            m_Writer.Write((short)m_MAC_XDriftLeapTS);
            m_Writer.Write((byte)HardCodeKey);
            m_Writer.Write((short)AvailableBytes);    // OFFSET 162

            // Reserved For Alignment, 27 bytes
            m_Writer.Write(ReservedBytes);

            // Miscellanous Values
            m_Writer.Write((byte)m_RCD_VcapThreshold);               // OFFSET 191 RCD_Vcap_Threshold
            m_Writer.Write((byte)m_RCD_PulseWidth);                  // RCD_Pulse_Width
            m_Writer.Write((byte)m_Bridge_OR_C1212SendTimeoutSeconds);          // Manufactured_As_Mobile C1222SendTimeoutSeconds
            m_Writer.Write((byte)m_MiscFactoryDataBits);              // MiscFactoryDatabits
            m_Writer.Write((byte)m_RemoteConnectDisconnectDelay);     // RemoteConnectDisconnectDelay (0= No delay, 1-254= Specified delay, 255= Use #define delay)
            m_Writer.Write((byte)m_ExternalPowerOutageConfig);        // HW3.0 Extended Power Outage Config Byte
            m_Writer.Write((byte)m_TimeAdjustPollDeltaSecs);          // TimeAdjustMaximumDeltaSeconds
            m_Writer.Write((byte)m_TimeAdjustPollPeriodMins);         // TimeAdjustPollPeriodMinutes
            m_Writer.Write((byte)m_TimeAdjustPollRoundTripSecs);      // TimeAdjustMaximumPollRoundTripSeconds
            m_Writer.Write((byte)m_TimeAdjustPollFailThresholdMins);;  // TimeAdjustPollFailureThresholdMinutes
            m_Writer.Write((byte)m_C1222ClientPacingInSeconds);       // C1222ClientPacing - 0xFF will default to 4 second minimum delay between requests
            m_Writer.Write((byte)m_RFLAN_OptOut);                     // OFFSET 202 UINT8 RFLAN_OK;

            // Manufacturing Configuration Data
            m_Writer.Write(m_SerialNumber);                       // OFFSET 203,   16 Bytes
            m_Writer.Write(m_SpecificationNumber);                // 10 Bytes
            m_Writer.Write((UInt32)m_ManufacturingTimeStamp);
            m_Writer.Write((byte)m_HW_Version);
            m_Writer.Write((byte)m_HW_Revision);
            m_Writer.Write((byte)m_FactoryProgramed);
            m_Writer.Write((byte)m_Frequency);                    // OFFSET 236

            // New Factory Data Start
            m_Writer.Write((short)m_SturctureFormatMarker);
            m_Writer.Write((short)m_LengthOfAddedData);

            // Security Key Records
            m_Writer.Write(m_key1);  // Each is 27 bytes long
            m_Writer.Write(m_key2);
            m_Writer.Write(m_key3);
            m_Writer.Write(m_key4);

            // RFLAN MAC Configuration
            m_Writer.Write((short)m_MAC_RecepRatePeriodTimeslot);
            m_Writer.Write((short)m_MAC_ListenWindowLength);
            m_Writer.Write((byte)m_MAC_MinDiscoveryPhaseTimeSlot);
            m_Writer.Write((short)m_MAC_BeaconPeridicitySyncTimeSlot);
            m_Writer.Write((UInt32)m_MAC_NeighborRequestBeaconTimeOut);
            m_Writer.Write((UInt32)m_MAC_NeighborTimeOut);
            m_Writer.Write((short)m_MAC_SynchFatherRequestBeaconTimeOut);
            m_Writer.Write((byte)m_MAC_XDriftTime);
            m_Writer.Write((UInt32)m_MAC_XDriftFilterA);
            m_Writer.Write((UInt32)m_MAC_XDriftFilterB); ;
            m_Writer.Write((UInt32)m_MAC_RFLAN_Address);
            m_Writer.Write((byte)m_MAC_UtilityID);  // default parking ID for Oconee
            m_Writer.Write((byte)m_MAC_SynchByte1);
            m_Writer.Write((byte)m_MAC_SynchByte2);
            m_Writer.Write((byte)m_MAC_NeighborListStableSeconds);
            m_Writer.Write((byte)m_MAC_RxAntenna);
            m_Writer.Write((byte)m_MAC_NetBuildPathTimeOut);
            m_Writer.Write((byte)m_MAC_SSC_Timeout);

            // C1222 Config
            m_Writer.Write((UInt32)m_C1222_Config);


            // Logical Link Control
            m_Writer.Write((byte)m_LLC_MacNumberTransmissions);
            m_Writer.Write((UInt32)m_LLC_MissingPacketTimout);
            m_Writer.Write((byte)m_LLC_TxRetryExponetialStart);
            m_Writer.Write((byte)m_LLC_TxRetryExponetialRange);
            m_Writer.Write((short)m_LLC_NeighborListPeriodicity);

            // Physical Layer RF Transceiver Configuration
            m_Writer.Write((byte)m_PL_pktctrlo);                  // UINT8 PKTCTRLO - Packet automation Control
            m_Writer.Write((byte)m_PL_fsctrl1);                  // UINT8 FSCTRL1 - Frequency Synthesizer Control 1
            m_Writer.Write((byte)m_PL_fsctrl0);                  // UINT8 FSCTRL0 - Frequency Synthesizer Control 0
            m_Writer.Write((byte)m_PL_freq2);                  // UINT8 FREQ2 - Frequency Control 2
            m_Writer.Write((byte)m_PL_freq1);                  // UINT8 FREQ1 - Frequency Control 1
            m_Writer.Write((byte)m_PL_freq0);                  // UINT8 FREQ0 - Frequency Control 0
            m_Writer.Write((byte)m_PL_mdmcfg4);                  // UINT8 MDMCFG4 - Modem Configuration 4
            m_Writer.Write((byte)m_PL_mdmcfg3);                  // UINT8 MDMCFG3 - Modem Configuration 3
            m_Writer.Write((byte)m_PL_mdmcfg2);                  // UINT8 MDMCFG2 - Modem Configuration 2
            m_Writer.Write((byte)m_PL_mdmcfg1);                  // UINT8 MDMCFG1 - Modem Configuration 1
            m_Writer.Write((byte)m_PL_mdmcfg0);                  // UINT8 MDMCFG0 - Modem Configuration 0
            m_Writer.Write((byte)m_PL_deviatn);                  // UINT8 DEVIATN - Modem Deviation Setting
            m_Writer.Write((byte)m_PL_foccfg);                  // UINT8 FOCCFG - Frequency Offset Compensation
            m_Writer.Write((byte)m_PL_bscfg);                  // UINT8 BSCFG - Bit Sync Configuration
            m_Writer.Write((byte)m_PL_agctrl2);                  // UINT8 AGCTRL2 - AGC Control 2
            m_Writer.Write((byte)m_PL_agctrl1);                  // UINT8 AGCTRL1 - AGC Control 1
            m_Writer.Write((byte)m_PL_agctrl0);                  // UINT8 AGCTRLO - AGC Control 0
            m_Writer.Write((byte)m_PL_frend1);                  // UINT8 FREND1 - Front End Tx Configuration 1
            m_Writer.Write((byte)m_PL_frend0);                  // UINT8 FREND0 - Front End Tx Configuration 0
            m_Writer.Write((byte)m_PL_fscal3);                  // UINT8 FSCAL3 - Frequency Synthesizer Calibration 3
            m_Writer.Write((byte)m_PL_fscal2);                  // UINT8 FSCAL2 - Frequency Synthesizer Calibration 2
            m_Writer.Write((byte)m_PL_fscal1);                  // UINT8 FSCAL1 - Frequency Synthesizer Calibration 1
            m_Writer.Write((byte)m_PL_fscal0);                  // UINT8 FSCAL0 - Frequency Synthesizer Calibration 0


            // RF Transmit Power Control;
            m_Writer.Write(m_PowerAmpTable); // 8 Bytes
            //  0x00, 0x33, 0x34, 0x35, 0x6B, 0x2D, 0x64, 0x51,    // UINT8 PATABLE - Power Amplifier Table

            // Physical Layer RF Synthesizer Config
            m_Writer.Write((byte)m_PhaseDetGain); // UINT8 Phase Detector Gain

            // FCC Mode
            m_Writer.Write((byte)m_FCC_mode0);                  // UINT8 MODE0
            m_Writer.Write((byte)m_FCC_mode1);                  // UINT8 MODE1

            // ESC, Electronic Serial Number
            m_Writer.Write(m_ESN);  // 20 Bytes

            // HAN MAC
            m_Writer.Write(m_HAN_MAC); // 8 Bytes
            // 0x00, 0x07, 0x81, 0xFF

            // HAN Security
            m_Writer.Write((byte)m_HAN_KeyType);      // Network Key Type
            m_Writer.Write(m_HAN_SecurityKey); // 16 Bytes
            //0x56,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,    // Application Key

            // CRC and Magic Key
            m_Writer.Write((short)m_FactoryDataCRC);    // UINT16 CRC
            m_Writer.Write((short)m_MagicKey);                // UINT16 Magic_Key;


            return base.Write();
        }


        #endregion Public Methods

        #region Public Properties




        /// <summary>
        /// Gets the Serial Number of the meter
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/12 BLC         N/A    Created for OpenWay

        public String SerialNumber
        {
            get
            {
                String text = System.Text.Encoding.ASCII.GetString(m_SerialNumber);
                return text;
            }
            set
            {

                char[] text = value.ToCharArray();
                for (uint i = 0; i < text.Length; i++)
                {
                    if (i <= SERIAL_NUMBER_LENGTH)
                    {
                        m_SerialNumber[i] = (byte)text[i];
                    }
                }
                
            }
        }


        /// <summary>
        /// Gets the Electronic Serial Number of the meter
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/12 BLC         N/A    Created for OpenWay

        public byte[] ESN
        {
            get
            {
                return m_ESN;
            }
            set
            {
                // clean the ESN before copy
                for (uint i = 0; i < m_ESN.Length; i++)
                {
                    m_ESN[i] = 0;
                }
                
                for (uint i = 0; i < value.Length; i++)
                {
                    if (i <= m_ESN.Length)
                    {
                        m_ESN[i] = value[i];
                    }
                }

            }
        }


        /// <summary>
        /// Gets the RFLAN Mac Address of the meter
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/12 BLC         N/A    Created for OpenWay

        public UInt32 MacAddress
        {
            get
            {
                return m_MAC_RFLAN_Address;

            }
            set
            {
                m_MAC_RFLAN_Address = value;

            }
        }


        /// <summary>
        /// Gets the RFLAN Utility ID 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/12 BLC         N/A    Created for OpenWay

        public byte UID
        {
            get
            {
                return m_MAC_UtilityID;

            }
            set
            {
                m_MAC_UtilityID = value;

            }
        }


        /// <summary>
        /// Gets the HW Version 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/12 BLC         N/A    Created for OpenWay

        public byte HW_Version
        {
            get
            {
                return m_HW_Version;

            }
            set
            {
                m_HW_Version = value;

            }
        }

        /// <summary>
        /// Gets the HW Revision 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/12 BLC         N/A    Created for OpenWay

        public byte HW_Revision
        {
            get
            {
                return m_HW_Revision;

            }
            set
            {
                m_HW_Revision = value;

            }
        }


        /// <summary>
        /// Gets the IO Meter Key
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/12 BLC         N/A    Created for OpenWay

        public UInt32 MeterKeyIO
        {
            get
            {
                return m_IO;

            }
            set
            {
                m_IO = value;

            }
        }


        /// <summary>
        /// Gets the Miscellanous Meter Key
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/12 BLC         N/A    Created for OpenWay

        public UInt32 MeterKeyMisc
        {
            get
            {
                return m_Misc;

            }
            set
            {
                m_Misc = value;

            }
        }

        /// <summary>
        /// Gets the Canadian Meter Value
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/12 BLC         N/A    Created for OpenWay

        public byte CanadianMeter
        {
            get
            {
                return m_CanadianMeter;

            }
            set
            {
                m_CanadianMeter = value;

            }
        }

        /// <summary>
        /// Gets the Zigbee MAC ID
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/12 BLC         N/A    Created for OpenWay

        public byte[] HanMAC
        {
            get
            {
                return m_HAN_MAC;
            }
            set
            {
                // clean the ESN before copy
                for (uint i = 0; i < m_HAN_MAC.Length; i++)
                {
                    m_HAN_MAC[i] = 0;
                }

                for (uint i = 0; i < value.Length; i++)
                {
                    if (i <= m_HAN_MAC.Length)
                    {
                        m_HAN_MAC[i] = value[i];
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Bridge Identifcation (MSM Meter)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/21/14 BLC         N/A    Created for OpenWay

        public byte Bridge
        {
            get
            {
                return m_Bridge_OR_C1212SendTimeoutSeconds;
            }
            set
            {
                m_Bridge_OR_C1212SendTimeoutSeconds = value;
            }
        }


        #endregion



        #region Members

        //The table's member variables which represent the table 

        // METER KEYS
        private byte m_SW_Version;
        private byte m_SW_Revision;
        private UInt16 m_Flavor;
        private UInt32 m_TimeStamp;
        private UInt32 m_Energy1;
        private UInt32 m_Demand;
        private UInt32 m_TOU;
        private UInt32 m_LoadProfile;
        private UInt32 m_PowerQuality;
        private UInt32 m_Misc;
        private UInt32 m_IO;
        private UInt32 m_OptionBoard;
        private UInt32 m_Instant;
        private UInt32 m_SelfRead;
        private UInt32 m_Calendar;
        private UInt32 m_Energy2;

        // Pass Codes
        private byte[] m_PassCode0;
        private byte[] m_PassCode1;
        private byte[] m_PassCode2;
        private byte[] m_PassCode3;

        // Canadian Meter
        private byte m_CanadianMeter;
        private byte m_CanadianSealed;

        // RFLAN Expansion
        private UInt32 m_BroadcastTimeOut;
        private byte m_BroadcastRetries;
        private byte m_LLC_Tx_RetryExpansionOffset;
        private byte m_LLC_DownlinkRetries;
        private byte m_API_MinSendInterval;
        private byte m_NET_MaxLife;
        private UInt16 m_NET_RouteEntryMaxLife;
        private UInt16 m_NET_SendRegMin;
        private UInt16 m_NET_SendRegMax;
        private UInt16 m_NET_RegInterval;
        private byte m_NET_MaxRegAttempts;
        private byte m_API_NetTimeRefreshPeriod;
        private byte m_RFLAN_ControlBits;
        private byte m_MAC_CellSwitchProbability;
        private UInt16 m_MAC_XDriftLeapTS;
        private byte HardCodeKey;
        private UInt16 AvailableBytes;

        // Reserved For Alignment, 27 bytes
        private byte[] ReservedBytes;

        // Miscellanous Values
        private byte m_RCD_VcapThreshold;
        private byte m_RCD_PulseWidth;
        private byte m_Bridge_OR_C1212SendTimeoutSeconds;
        private byte m_MiscFactoryDataBits;
        private byte m_RemoteConnectDisconnectDelay;
        private byte m_ExternalPowerOutageConfig;
        private byte m_TimeAdjustPollDeltaSecs;
        private byte m_TimeAdjustPollPeriodMins;
        private byte m_TimeAdjustPollRoundTripSecs;
        private byte m_TimeAdjustPollFailThresholdMins;
        private byte m_C1222ClientPacingInSeconds;
        private byte m_RFLAN_OptOut;

        // Manufacturing Configuration Data
        private byte[] m_SerialNumber; // 16 Bytes
        private byte[] m_SpecificationNumber; // 10 Bytes
        private UInt32 m_ManufacturingTimeStamp;
        private byte m_HW_Version;
        private byte m_HW_Revision;
        private byte m_FactoryProgramed;
        private byte m_Frequency;
        private UInt16 m_SturctureFormatMarker;
        private UInt16 m_LengthOfAddedData;
    
        // Security Key Records
        private byte[] m_key1;  // Each is 27 bytes long
        private byte[] m_key2;
        private byte[] m_key3;
        private byte[] m_key4;


        // RFLAN MAC Configuration
        private UInt16 m_MAC_RecepRatePeriodTimeslot;
        private UInt16 m_MAC_ListenWindowLength;
        private byte m_MAC_MinDiscoveryPhaseTimeSlot;
        private UInt16 m_MAC_BeaconPeridicitySyncTimeSlot;
        private UInt32 m_MAC_NeighborRequestBeaconTimeOut;
        private UInt32 m_MAC_NeighborTimeOut;
        private UInt16 m_MAC_SynchFatherRequestBeaconTimeOut;
        private byte m_MAC_XDriftTime;
        private UInt32 m_MAC_XDriftFilterA;
        private UInt32 m_MAC_XDriftFilterB;
        private UInt32 m_MAC_RFLAN_Address;
        private byte m_MAC_UtilityID;
        private byte m_MAC_SynchByte1;
        private byte m_MAC_SynchByte2;
        private byte m_MAC_NeighborListStableSeconds;
        private byte m_MAC_RxAntenna;
        private byte m_MAC_NetBuildPathTimeOut;
        private byte m_MAC_SSC_Timeout;

        // C1222 Config
        private UInt32 m_C1222_Config;


        // Logical Link Control
        private byte m_LLC_MacNumberTransmissions;
        private UInt32 m_LLC_MissingPacketTimout;
        private byte m_LLC_TxRetryExponetialStart;
        private byte m_LLC_TxRetryExponetialRange;
        private UInt16 m_LLC_NeighborListPeriodicity;

        // Physical Layer RF Transceiver Configuration
        private byte m_PL_pktctrlo;
        private byte m_PL_fsctrl1;
        private byte m_PL_fsctrl0;
        private byte m_PL_freq2;
        private byte m_PL_freq1;
        private byte m_PL_freq0;
        private byte m_PL_mdmcfg4;
        private byte m_PL_mdmcfg3;
        private byte m_PL_mdmcfg2;
        private byte m_PL_mdmcfg1;
        private byte m_PL_mdmcfg0;
        private byte m_PL_deviatn;
        private byte m_PL_foccfg;
        private byte m_PL_bscfg;
        private byte m_PL_agctrl2;
        private byte m_PL_agctrl1;
        private byte m_PL_agctrl0;
        private byte m_PL_frend1;
        private byte m_PL_frend0;
        private byte m_PL_fscal3;
        private byte m_PL_fscal2;
        private byte m_PL_fscal1;
        private byte m_PL_fscal0;

        // RF Transmit Power Control;
        private byte[] m_PowerAmpTable; // 8 Bytes
 
        // Physical Layer RF Synthesizer Config
        private byte m_PhaseDetGain;

        // FCC Mode
        private byte m_FCC_mode0;
        private byte m_FCC_mode1;

        // ESC, Electronic Serial Number
        private byte[] m_ESN;  // 20 Bytes

        // HAN MAC
        private byte[] m_HAN_MAC; // 8 Bytes

        // HAN Security
        private byte m_HAN_KeyType;
        private byte[] m_HAN_SecurityKey; // 16 Bytes

        // CRC and Magic Key
        private UInt16 m_FactoryDataCRC;
        private UInt16 m_MagicKey;



        #endregion

        #region Private

        /// <summary>
        /// Initialize Variables
        /// </summary>

        private void InitializeVariables()
        {
            // Values are set for a basic ITR1 and then modified later
            // METER KEYS
            m_SW_Version = 0;            // UINT8 Meter_Key.SW_Version;
            m_SW_Revision = 0;           // UINT8 Meter_Key.SW_Revision;
            m_Flavor = 2;                // UINT16 Meter_Key.Flavor;
            m_TimeStamp = 0;             // UINT32 Meter_Key.Time_Stamp;
            m_Energy1 = 0x00002181;      // UINT32 Meter_Key.Energy_1;
            m_Demand =  0x00008211;      // UINT32 Meter_Key.Demand;
            m_TOU =     0x00000005;      // UINT32 Meter_Key.TOU;
            m_LoadProfile = 0x00000090;  // UINT32 Meter_Key.Load_Prof;
            m_PowerQuality = 0x00000000; // UINT32 Meter_Key.Power_Quality;
            m_Misc = 0x00000005;         // UINT32 Meter_Key.Misc;
            m_IO = 0x00000000;           // UINT32 Meter_Key.IO; ITR1;
            m_OptionBoard = 0x00000000;  // UINT32 Meter_Key.Opt_Brd;
            m_Instant = 0x00000000;      // UINT32 Meter_Key.Instant;
            m_SelfRead = 0x00000016;     // UINT32 Meter_Key.Self_Read;
            m_Calendar = 0x00000001;     // UINT32 Meter_Key.Calendar;
            m_Energy2 = 0x00000024;      // UINT32 Meter_Key.Energy_2;

            // Pass Codes
            m_PassCode0 = new byte[PASSCODE_SIZE]; // 20 Bytes
            m_PassCode1 = new byte[PASSCODE_SIZE];
            m_PassCode2 = new byte[PASSCODE_SIZE];
            m_PassCode3 = new byte[PASSCODE_SIZE];


            // Canadian Meter
            m_CanadianMeter = 0;
            m_CanadianSealed = 0;

            // RFLAN Expansion
            m_BroadcastTimeOut = 0xFFFFFFFF;              // UINT16 MAC Reception Rate Period Timeslots
            m_BroadcastRetries = 0xFF;                    // UINT16 MAC Listening Window Length
            m_LLC_Tx_RetryExpansionOffset = 0xFF;         // UINT8 MAC Min Discovery Phase Timeslots
            m_LLC_DownlinkRetries = 0xFF;
            m_API_MinSendInterval = 0xFF;
            m_NET_MaxLife = 0xFF;
            m_NET_RouteEntryMaxLife = 0xFFFF;
            m_NET_SendRegMin = 0xFFFF;
            m_NET_SendRegMax = 0xFFFF;
            m_NET_RegInterval = 0xFFFF;
            m_NET_MaxRegAttempts = 0xFF;
            m_API_NetTimeRefreshPeriod = 0xFF;
            m_RFLAN_ControlBits = 0xFF;
            m_MAC_CellSwitchProbability = 0xFF;
            m_MAC_XDriftLeapTS = 0xFFFF;
            HardCodeKey = 0xFF;
            AvailableBytes = 0xFFFF;

            // Reserved For Alignment, 27 bytes
            ReservedBytes = new byte[RESERVED_BYTE_LENGTH];

            // Miscellanous Values
            m_RCD_VcapThreshold = 0xFF;
            m_RCD_PulseWidth = 0xFF;
            m_Bridge_OR_C1212SendTimeoutSeconds = 0xFF;
            m_MiscFactoryDataBits  = 0x01;
            m_RemoteConnectDisconnectDelay = 0x00;
            m_ExternalPowerOutageConfig = 0x00;
            m_TimeAdjustPollDeltaSecs = 0xFF;
            m_TimeAdjustPollPeriodMins = 0xFF;
            m_TimeAdjustPollRoundTripSecs = 0xFF;
            m_TimeAdjustPollFailThresholdMins = 0xFF;
            m_C1222ClientPacingInSeconds = 0xFF;
            m_RFLAN_OptOut = 0x00; // RFLAN_OK;

            // Manufacturing Configuration Data
            m_SerialNumber = new byte[SERIAL_NUMBER_LENGTH]; // 16 Bytes
            m_SpecificationNumber = new byte[SPECIFICATION_NUMBER_LENGTH]; // 10 Bytes
            m_ManufacturingTimeStamp = 0;
            m_HW_Version = 3;
            m_HW_Revision = 50;
            m_FactoryProgramed = 1;
            m_Frequency = 1;
            m_SturctureFormatMarker = 0xABCD;
            m_LengthOfAddedData = 256;
    
            // Security Key Records
            m_key1 = new byte[SECURITY_KEY_LENGTH];  // Each is 27 bytes long
            m_key2 = new byte[SECURITY_KEY_LENGTH];
            m_key3 = new byte[SECURITY_KEY_LENGTH];
            m_key4 = new byte[SECURITY_KEY_LENGTH];

            // RFLAN MAC Configuration
            m_MAC_RecepRatePeriodTimeslot = 0x0FA0;
            m_MAC_ListenWindowLength = 0x09C4;
            m_MAC_MinDiscoveryPhaseTimeSlot = 0x16;
            m_MAC_BeaconPeridicitySyncTimeSlot = 0x271;
            m_MAC_NeighborRequestBeaconTimeOut = 0x001B77F0;
            m_MAC_NeighborTimeOut = 0x0036EE80;
            m_MAC_SynchFatherRequestBeaconTimeOut = 0x271;
            m_MAC_XDriftTime = 0x05;
            m_MAC_XDriftFilterA = 0x00000803;
            m_MAC_XDriftFilterB = 0x5A643B3F;;
            m_MAC_RFLAN_Address = 0;
            m_MAC_UtilityID = 0xF5;  // default parking ID for Oconee
            m_MAC_SynchByte1 = 0xFF;
            m_MAC_SynchByte2 = 0xFF;
            m_MAC_NeighborListStableSeconds = 10;
            m_MAC_RxAntenna = 0xFF;
            m_MAC_NetBuildPathTimeOut = 0xFF;
            m_MAC_SSC_Timeout = 0xFF;

            // C1222 Config
            m_C1222_Config = 0xFFFFFFFF;


            // Logical Link Control
            m_LLC_MacNumberTransmissions = 0x0F;
            m_LLC_MissingPacketTimout = 0x249F0;
            m_LLC_TxRetryExponetialStart = 0x05;
            m_LLC_TxRetryExponetialRange = 0x05;
            m_LLC_NeighborListPeriodicity = 0x02B4;

            // Physical Layer RF Transceiver Configuration
            m_PL_pktctrlo =  0x02;                  // UINT8 PKTCTRLO - Packet automation Control
            m_PL_fsctrl1 = 0x06;                  // UINT8 FSCTRL1 - Frequency Synthesizer Control 1
            m_PL_fsctrl0 = 0x00;                  // UINT8 FSCTRL0 - Frequency Synthesizer Control 0
            m_PL_freq2 = 0x0F;                  // UINT8 FREQ2 - Frequency Control 2
            m_PL_freq1 = 0x62;                  // UINT8 FREQ1 - Frequency Control 1
            m_PL_freq0 = 0x76;                  // UINT8 FREQ0 - Frequency Control 0
            m_PL_mdmcfg4 = 0xC9;                  // UINT8 MDMCFG4 - Modem Configuration 4
            m_PL_mdmcfg3 = 0x83;                  // UINT8 MDMCFG3 - Modem Configuration 3
            m_PL_mdmcfg2 = 0x02;                  // UINT8 MDMCFG2 - Modem Configuration 2
            m_PL_mdmcfg1 = 0x22;                  // UINT8 MDMCFG1 - Modem Configuration 1
            m_PL_mdmcfg0 = 0xF8;                  // UINT8 MDMCFG0 - Modem Configuration 0
            m_PL_deviatn = 0x34;                  // UINT8 DEVIATN - Modem Deviation Setting
            m_PL_foccfg = 0x16;                  // UINT8 FOCCFG - Frequency Offset Compensation
            m_PL_bscfg = 0x6C;                  // UINT8 BSCFG - Bit Sync Configuration
            m_PL_agctrl2 = 0x43;                  // UINT8 AGCTRL2 - AGC Control 2
            m_PL_agctrl1 = 0x40;                  // UINT8 AGCTRL1 - AGC Control 1
            m_PL_agctrl0 = 0x91;                  // UINT8 AGCTRLO - AGC Control 0
            m_PL_frend1 = 0x56;                  // UINT8 FREND1 - Front End Tx Configuration 1
            m_PL_frend0 = 0x17;                  // UINT8 FREND0 - Front End Tx Configuration 0
            m_PL_fscal3 = 0xE9;                  // UINT8 FSCAL3 - Frequency Synthesizer Calibration 3
            m_PL_fscal2 = 0x0A;                  // UINT8 FSCAL2 - Frequency Synthesizer Calibration 2
            m_PL_fscal1 = 0x00;                  // UINT8 FSCAL1 - Frequency Synthesizer Calibration 1
            m_PL_fscal0 = 0x1F;                  // UINT8 FSCAL0 - Frequency Synthesizer Calibration 0


            // RF Transmit Power Control;
            m_PowerAmpTable = new byte[POWER_TABLE_LENGTH]; // 8 Bytes
            //  0x00, 0x33, 0x34, 0x35, 0x6B, 0x2D, 0x64, 0x51,    // UINT8 PATABLE - Power Amplifier Table
 
            // Physical Layer RF Synthesizer Config
            m_PhaseDetGain = 0x14; // UINT8 Phase Detector Gain

            // FCC Mode
            m_FCC_mode0 = 0x00;                  // UINT8 MODE0
            m_FCC_mode1 = 0x00;                  // UINT8 MODE1

            // ESC, Electronic Serial Number
            m_ESN = new byte[ESN_LENGTH];  // 20 Bytes

            // HAN MAC
            m_HAN_MAC = new byte[HAN_MAC_LENGTH]; // 8 Bytes
            // 0x00, 0x07, 0x81, 0xFF

            // HAN Security
            m_HAN_KeyType = 0x01;      // Network Key Type
            m_HAN_SecurityKey = new byte[HAN_KEY_LENGTH]; // 16 Bytes
            //0x56,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,    // Application Key

            // CRC and Magic Key
            m_FactoryDataCRC = 0x88FF;    // UINT16 CRC
            m_MagicKey = 0xFFFF;                // UINT16 Magic_Key;
        }


        /// <summary>
        /// Parses the data that was just read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/12/12 RCG          N/A    Created for OpenWay

        private void ParseData()
        {


            // METER KEYS
            m_SW_Version = m_Reader.ReadByte();              // OFFSET 0 UINT8 Meter_Key.SW_Version;
            m_SW_Revision = m_Reader.ReadByte();             // UINT8 Meter_Key.SW_Revision;
            m_Flavor = m_Reader.ReadUInt16();                // UINT16 Meter_Key.Flavor;
            m_TimeStamp = m_Reader.ReadUInt32();             // UINT32 Meter_Key.Time_Stamp;
            m_Energy1 = m_Reader.ReadUInt32();               // UINT32 Meter_Key.Energy_1;
            m_Demand = m_Reader.ReadUInt32();                // UINT32 Meter_Key.Demand;
            m_TOU = m_Reader.ReadUInt32();                   // UINT32 Meter_Key.TOU;
            m_LoadProfile = m_Reader.ReadUInt32();  // UINT32 Meter_Key.Load_Prof;
            m_PowerQuality = m_Reader.ReadUInt32(); // UINT32 Meter_Key.Power_Quality;
            m_Misc = m_Reader.ReadUInt32();         // UINT32 Meter_Key.Misc;
            m_IO = m_Reader.ReadUInt32();           // UINT32 Meter_Key.IO; ITR1;
            m_OptionBoard = m_Reader.ReadUInt32();  // UINT32 Meter_Key.Opt_Brd;
            m_Instant = m_Reader.ReadUInt32();      // UINT32 Meter_Key.Instant;
            m_SelfRead = m_Reader.ReadUInt32();     // UINT32 Meter_Key.Self_Read;
            m_Calendar = m_Reader.ReadUInt32();     // UINT32 Meter_Key.Calendar;
            m_Energy2 = m_Reader.ReadUInt32();      // OFFSET 52 UINT32 Meter_Key.Energy_2;

            // Pass Codes
            m_PassCode0 = m_Reader.ReadBytes((int)PASSCODE_SIZE);
            m_PassCode1 = m_Reader.ReadBytes((int)PASSCODE_SIZE);
            m_PassCode2 = m_Reader.ReadBytes((int)PASSCODE_SIZE);
            m_PassCode3 = m_Reader.ReadBytes((int)PASSCODE_SIZE);


            // Canadian Meter
            m_CanadianMeter = m_Reader.ReadByte();   // OFFSET 136
            m_CanadianSealed = m_Reader.ReadByte();
                
            // RFLAN Expansion
            m_BroadcastTimeOut = m_Reader.ReadUInt32();   
            m_BroadcastRetries = m_Reader.ReadByte();  
            m_LLC_Tx_RetryExpansionOffset = m_Reader.ReadByte();   
            m_LLC_DownlinkRetries = m_Reader.ReadByte();
            m_API_MinSendInterval = m_Reader.ReadByte();
            m_NET_MaxLife = m_Reader.ReadByte();
            m_NET_RouteEntryMaxLife = m_Reader.ReadUInt16();
            m_NET_SendRegMin = m_Reader.ReadUInt16();
            m_NET_SendRegMax = m_Reader.ReadUInt16();
            m_NET_RegInterval = m_Reader.ReadUInt16();
            m_NET_MaxRegAttempts = m_Reader.ReadByte();
            m_API_NetTimeRefreshPeriod = m_Reader.ReadByte();
            m_RFLAN_ControlBits = m_Reader.ReadByte();
            m_MAC_CellSwitchProbability = m_Reader.ReadByte();
            m_MAC_XDriftLeapTS = m_Reader.ReadUInt16();
            HardCodeKey = m_Reader.ReadByte();
            AvailableBytes = m_Reader.ReadUInt16();    // OFFSET 162

            // Reserved For Alignment, 27 bytes
            ReservedBytes = m_Reader.ReadBytes((int)RESERVED_BYTE_LENGTH);

            // Miscellanous Values
            m_RCD_VcapThreshold = m_Reader.ReadByte();                // OFFSET 191 RCD_Vcap_Threshold
            m_RCD_PulseWidth = m_Reader.ReadByte();                   // RCD_Pulse_Width
            m_Bridge_OR_C1212SendTimeoutSeconds = m_Reader.ReadByte();// C1222SendTimeoutSeconds
            m_MiscFactoryDataBits = m_Reader.ReadByte();              // MiscFactoryDatabits
            m_RemoteConnectDisconnectDelay = m_Reader.ReadByte();     // RemoteConnectDisconnectDelay (0= No delay, 1-254= Specified delay, 255= Use #define delay)
            m_ExternalPowerOutageConfig = m_Reader.ReadByte();        // HW3.0 Extended Power Outage Config Byte
            m_TimeAdjustPollDeltaSecs = m_Reader.ReadByte();          // TimeAdjustMaximumDeltaSeconds
            m_TimeAdjustPollPeriodMins = m_Reader.ReadByte();         // TimeAdjustPollPeriodMinutes
            m_TimeAdjustPollRoundTripSecs = m_Reader.ReadByte();      // TimeAdjustMaximumPollRoundTripSeconds
            m_TimeAdjustPollFailThresholdMins = m_Reader.ReadByte();  // TimeAdjustPollFailureThresholdMinutes
            m_C1222ClientPacingInSeconds = m_Reader.ReadByte();       // C1222ClientPacing - 0xFF will default to 4 second minimum delay between requests
            m_RFLAN_OptOut = m_Reader.ReadByte();                     // OFFSET 202 UINT8 RFLAN_OK;

            // Manufacturing Configuration Data
            m_SerialNumber = m_Reader.ReadBytes((int)SERIAL_NUMBER_LENGTH);               // OFFSET 203,   16 Bytes
            m_SpecificationNumber = m_Reader.ReadBytes((int)SPECIFICATION_NUMBER_LENGTH); // 10 Bytes
            m_ManufacturingTimeStamp = m_Reader.ReadUInt32();
            m_HW_Version = m_Reader.ReadByte();
            m_HW_Revision = m_Reader.ReadByte();
            m_FactoryProgramed = m_Reader.ReadByte();
            m_Frequency = m_Reader.ReadByte();                                            // OFFSET 236

            // New Factory Data Start
            m_SturctureFormatMarker = m_Reader.ReadUInt16();
            m_LengthOfAddedData = m_Reader.ReadUInt16();

            // Security Key Records
            m_key1 = m_Reader.ReadBytes((int)SECURITY_KEY_LENGTH);  // Each is 27 bytes long
            m_key2 = m_Reader.ReadBytes((int)SECURITY_KEY_LENGTH);
            m_key3 = m_Reader.ReadBytes((int)SECURITY_KEY_LENGTH);
            m_key4 = m_Reader.ReadBytes((int)SECURITY_KEY_LENGTH);

            // RFLAN MAC Configuration
            m_MAC_RecepRatePeriodTimeslot = m_Reader.ReadUInt16();
            m_MAC_ListenWindowLength = m_Reader.ReadUInt16();
            m_MAC_MinDiscoveryPhaseTimeSlot = m_Reader.ReadByte();
            m_MAC_BeaconPeridicitySyncTimeSlot = m_Reader.ReadUInt16();
            m_MAC_NeighborRequestBeaconTimeOut = m_Reader.ReadUInt32();
            m_MAC_NeighborTimeOut = m_Reader.ReadUInt32();
            m_MAC_SynchFatherRequestBeaconTimeOut = m_Reader.ReadUInt16();
            m_MAC_XDriftTime = m_Reader.ReadByte();
            m_MAC_XDriftFilterA = m_Reader.ReadUInt32();
            m_MAC_XDriftFilterB = m_Reader.ReadUInt32(); ;
            m_MAC_RFLAN_Address = m_Reader.ReadUInt32();
            m_MAC_UtilityID = m_Reader.ReadByte();  // default parking ID for Oconee
            m_MAC_SynchByte1 = m_Reader.ReadByte();
            m_MAC_SynchByte2 = m_Reader.ReadByte();
            m_MAC_NeighborListStableSeconds = m_Reader.ReadByte();
            m_MAC_RxAntenna = m_Reader.ReadByte();
            m_MAC_NetBuildPathTimeOut = m_Reader.ReadByte();
            m_MAC_SSC_Timeout = m_Reader.ReadByte();

            // C1222 Config
            m_C1222_Config = m_Reader.ReadUInt32();


            // Logical Link Control
            m_LLC_MacNumberTransmissions = m_Reader.ReadByte();
            m_LLC_MissingPacketTimout = m_Reader.ReadUInt32();
            m_LLC_TxRetryExponetialStart = m_Reader.ReadByte();
            m_LLC_TxRetryExponetialRange = m_Reader.ReadByte();
            m_LLC_NeighborListPeriodicity = m_Reader.ReadUInt16();

            // Physical Layer RF Transceiver Configuration
            m_PL_pktctrlo = m_Reader.ReadByte();                  // UINT8 PKTCTRLO - Packet automation Control
            m_PL_fsctrl1 = m_Reader.ReadByte();                  // UINT8 FSCTRL1 - Frequency Synthesizer Control 1
            m_PL_fsctrl0 = m_Reader.ReadByte();                  // UINT8 FSCTRL0 - Frequency Synthesizer Control 0
            m_PL_freq2 = m_Reader.ReadByte();                  // UINT8 FREQ2 - Frequency Control 2
            m_PL_freq1 = m_Reader.ReadByte();                  // UINT8 FREQ1 - Frequency Control 1
            m_PL_freq0 = m_Reader.ReadByte();                  // UINT8 FREQ0 - Frequency Control 0
            m_PL_mdmcfg4 = m_Reader.ReadByte();                  // UINT8 MDMCFG4 - Modem Configuration 4
            m_PL_mdmcfg3 = m_Reader.ReadByte();                  // UINT8 MDMCFG3 - Modem Configuration 3
            m_PL_mdmcfg2 = m_Reader.ReadByte();                  // UINT8 MDMCFG2 - Modem Configuration 2
            m_PL_mdmcfg1 = m_Reader.ReadByte();                  // UINT8 MDMCFG1 - Modem Configuration 1
            m_PL_mdmcfg0 = m_Reader.ReadByte();                  // UINT8 MDMCFG0 - Modem Configuration 0
            m_PL_deviatn = m_Reader.ReadByte();                  // UINT8 DEVIATN - Modem Deviation Setting
            m_PL_foccfg = m_Reader.ReadByte();                  // UINT8 FOCCFG - Frequency Offset Compensation
            m_PL_bscfg = m_Reader.ReadByte();                  // UINT8 BSCFG - Bit Sync Configuration
            m_PL_agctrl2 = m_Reader.ReadByte();                  // UINT8 AGCTRL2 - AGC Control 2
            m_PL_agctrl1 = m_Reader.ReadByte();                  // UINT8 AGCTRL1 - AGC Control 1
            m_PL_agctrl0 = m_Reader.ReadByte();                  // UINT8 AGCTRLO - AGC Control 0
            m_PL_frend1 = m_Reader.ReadByte();                  // UINT8 FREND1 - Front End Tx Configuration 1
            m_PL_frend0 = m_Reader.ReadByte();                  // UINT8 FREND0 - Front End Tx Configuration 0
            m_PL_fscal3 = m_Reader.ReadByte();                  // UINT8 FSCAL3 - Frequency Synthesizer Calibration 3
            m_PL_fscal2 = m_Reader.ReadByte();                  // UINT8 FSCAL2 - Frequency Synthesizer Calibration 2
            m_PL_fscal1 = m_Reader.ReadByte();                  // UINT8 FSCAL1 - Frequency Synthesizer Calibration 1
            m_PL_fscal0 = m_Reader.ReadByte();                  // UINT8 FSCAL0 - Frequency Synthesizer Calibration 0


            // RF Transmit Power Control;
            m_PowerAmpTable = m_Reader.ReadBytes((int)POWER_TABLE_LENGTH); // 8 Bytes
            //  0x00, 0x33, 0x34, 0x35, 0x6B, 0x2D, 0x64, 0x51,    // UINT8 PATABLE - Power Amplifier Table

            // Physical Layer RF Synthesizer Config
            m_PhaseDetGain = m_Reader.ReadByte(); // UINT8 Phase Detector Gain

            // FCC Mode
            m_FCC_mode0 = m_Reader.ReadByte();                  // UINT8 MODE0
            m_FCC_mode1 = m_Reader.ReadByte();                  // UINT8 MODE1

            // ESC, Electronic Serial Number
            m_ESN = m_Reader.ReadBytes((int)ESN_LENGTH);  // 20 Bytes

            // HAN MAC
            m_HAN_MAC = m_Reader.ReadBytes((int)HAN_MAC_LENGTH); // 8 Bytes
            // 0x00, 0x07, 0x81, 0xFF

            // HAN Security
            m_HAN_KeyType = m_Reader.ReadByte();      // Network Key Type
            m_HAN_SecurityKey = m_Reader.ReadBytes((int)HAN_KEY_LENGTH); // 16 Bytes
            //0x56,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,0x77,    // Application Key

            // CRC and Magic Key
            m_FactoryDataCRC = m_Reader.ReadUInt16();    // UINT16 CRC
            m_MagicKey = m_Reader.ReadUInt16();                // UINT16 Magic_Key;


        }

        #endregion

    }

}
