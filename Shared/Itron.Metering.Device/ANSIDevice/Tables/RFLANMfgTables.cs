///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
// embodying substantial creative efforts and trade secrets, confidential 
// information, ideas and expressions. No part of which may be reproduced or 
// transmitted in any form or by any means electronic, mechanical, or 
// otherwise.  Including photocopying and recording or in connection with any
// information storage or retrieval system without the permission in writing 
// from Itron, Inc.
//
//                            Copyright © 2008 - 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using System.Globalization;
using System.Threading;

namespace Itron.Metering.Device
{
    /// <summary>
    /// 
    /// </summary>
    public class RFLANMfgTable2065 : AnsiTable
    {
        #region Definitions

        private const uint TABLE_LENGTH_2065 = 20;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// RFLANMfgTable2065 Table2065 = new RFLANMfgTable2065(m_PSEM);
        /// </code></example>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/28/11 AF  2.53.02        Created
        //
        public RFLANMfgTable2065(CPSEM psem)
            : base(psem, 2065, TABLE_LENGTH_2065)
        {
            m_Identification = "";
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The PSEM result of the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/28/11 AF  2.53.02        Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_Identification = m_Reader.ReadString((int)TABLE_LENGTH_2065);
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //
        //
        public string RFLANIdentification
        {
            get
            {
                ReadUnloadedTable();

                return m_Identification;
            }
        }

        #endregion

        #region Members

        private string m_Identification;

        #endregion
    }

    /// <summary>
    /// The RFLANMfgTable2068 class handles the reading of the RFLAN Information
    /// table.
    /// </summary>
    internal class RFLANMfgTable2068 : AnsiTable
    {

        #region Definitions

        private const uint TABLE_LENGTH_2068_LONG = 273;
        private const uint TABLE_LENGTH_2068_SHORT = 247;
        private const int ITPOffset = 238;  // The offset seems to be fixed over all RFLAN versions (but doesn't have to be)
        private const string RFLAN_FW_0_009_044 = "0.009.044";
        private const string RFLAN_FW_0_012_050 = "0.012.050";
        private const int MAC_ADDRESS_AND_LEVEL_LENGTH = 5;
        private const int ITP_READ_LENGTH = 243;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="strRFLANVersion">The firmware version of the RFLAN module including the build</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("username");
        /// RFLANMfgTable2068 Table2068 = new RFLANMfgTable2068(m_PSEM, strRFLANVersion);
        /// </code></example>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/07/08 AF  1.51.04 116680 Created
        //  12/20/10 AF  2.45.20 163738 Changed the input parameter to a string representation
        //                              of the RFLAN f/w version
        //  11/07/12 jrf 2.70.36 240583 Added code to automatically resize table to match size of
        //                              data returned from a full read.
        //
        public RFLANMfgTable2068(CPSEM psem, string strRFLANVersion)
            : base(psem, 2068, GetTableSize(strRFLANVersion))
        {
            m_RFLANMACAddress = new CachedUint();
            m_bytLevel = 0;
            m_strRFLANVersion = strRFLANVersion;
            //QC tool is now requiring full reads of this table.  Our current table size 
            //determination logic is incorrect for full table reads.  RF LAN team could not 
            //easily confirm logic for determining this table's size without manually researching code 
            //for all previously released RF LAN firmware versions.  Because this could take some time
            //and in an effort to get the factory up and running in an expedient manner, this work around 
            //is being implemented to allow this table to automatically resize itself based on the size
            //of the data returned from a full read.  This is not ideal but it was decided that this is the
            //best course of action in this instance.
            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Reads the table from the meter.  
        /// </summary>
        /// <returns>The PSEM result of the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/15/10 AF  2.45.05 162320 Created
        //  12/20/10 AF  2.45.20 163738 Read ITP without using ReadLTime() - it sets the
        //                              time object to local time and this is GMT.
        //  11/07/12 jrf 2.70.36 240583 Since table can now resize itself we need to ensure
        //                              that the size supports the data being read in a full 
        //                              read before trying to parse it.
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                byte[] abytData;
                uint uiMinutes;
                byte bySeconds;

                if (MAC_ADDRESS_AND_LEVEL_LENGTH <= m_Size)
                {
                    m_RFLANMACAddress.Value = m_Reader.ReadUInt32();
                    m_bytLevel = m_Reader.ReadByte();
                }
                else
                {
                    m_RFLANMACAddress.Value = 0;
                    m_bytLevel = 0;
                }

                if (ITP_READ_LENGTH <= m_Size)
                {
                    abytData = m_Reader.ReadBytes(ITPOffset - MAC_ADDRESS_AND_LEVEL_LENGTH);

                    uiMinutes = m_Reader.ReadUInt32();
                    bySeconds = m_Reader.ReadByte();

                    m_ITP = new DateTime(1970, 1, 1, 0, 0, 0);
                    m_ITP = m_ITP.AddMinutes(uiMinutes);
                    m_ITP = m_ITP.AddSeconds(bySeconds);
                    m_ITP = m_ITP.ToLocalTime();
                }
                else
                {
                    m_ITP = new DateTime(1970, 1, 1, 0, 0, 0);
                    m_ITP = m_ITP.ToLocalTime();
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Retrieves the RFLAN MAC Address from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/07/08 AF  1.51.04 116680 Created
        //  04/05/11 jrf 2.50.21        Changed to always reread value from meter if 
        //                              it is zero.
        //  01/30/12 MSC 2.53.34 184883 Added an extra check when the MAC address is 0.
        //  10/15/12 jrf 2.70.30 240583 Removing extra check of MAC (full table read), since 
        //                              in some cases this was leading to fatal errors of 
        //                              cell relays.
        //
        public UInt32 RFLANMACAddress
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] byaData;

                if (m_RFLANMACAddress.Cached == false || 0 == m_RFLANMACAddress.Value)
                {
                    PSEMResult = m_PSEM.OffsetRead(2068, 0, 4, out byaData);

                    if (PSEMResponse.Ok != PSEMResult)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                            "Error Reading Comm Module MAC Address"));
                    }
                    else
                    {
                        //Convert the bytes read to something useful.
                        MemoryStream DataStream = new MemoryStream(byaData);
                        PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                        m_RFLANMACAddress.Value = Reader.ReadUInt32();
                    }
                }

                return m_RFLANMACAddress.Value;
            }
        }

        /// <summary>
        /// Retrieves the RFLAN MAC Address from the meter from a full table read rather than
        /// an offset read.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/15/10 AF  2.45.05 162320 Created
        //  04/05/11 jrf 2.50.21        Changed to always reread value from meter if 
        //                              it is zero.
        //
        public UInt32 RFLANMACAddressFromFullRead
        {
            get
            {
                if (m_RFLANMACAddress.Cached == false || 0 == m_RFLANMACAddress.Value)
                {
                    State = TableState.Expired;
                }

                ReadUnloadedTable();

                return m_RFLANMACAddress.Value;
            }
        }

        /// <summary>
        /// Retrieves the previously read RFLAN MAC address previously read.  This property
        /// relies on Read() being called to retrieve this value.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/15/12 jrf 2.70.30 240583 Created
        //
        public UInt32 RFLANMACAddressNoRead
        {
            get
            {
                UInt32 uiRFLANMACAddress = 0;

                if (m_RFLANMACAddress.Cached == true)
                {
                    uiRFLANMACAddress = m_RFLANMACAddress.Value;
                }

                return uiRFLANMACAddress;
            }
        }

        /// <summary>
        /// Retrieves the RFLAN level directly.  This property 
        /// should not used unless a conscience decision is made to use 
        /// this value instead of the level returned from the vendor field.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/28/09 jrf 2.30.04        Created
        //  10/15/12 jrf 2.70.30 240583 Renamed method.
        //
        public byte RFLANLevelFromOffsetRead
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] abytData;

                PSEMResult = m_PSEM.OffsetRead(2068, 4, 1, out abytData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading Factory Comm Module Level"));
                }
                else
                {
                    m_bytLevel = abytData[0];
                }


                return m_bytLevel;
            }
        }

        /// <summary>
        /// Retrieves the previously read RFLAN level previously read.  This property
        /// relies on Read() being called to retrieve this value.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/15/12 jrf 2.70.30 240583 Created
        //
        public byte RFLANLevelNoRead
        {
            get
            {
                return m_bytLevel;
            }
        }

        /// <summary>
        /// Retrieves the ITP time via an offset read of the table
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  12/10/10 AF  2.45.20  163738  Function created.
        //
        public DateTime ITP
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] abytData;
                uint uiMinutes;
                byte bySeconds;

                PSEMResult = m_PSEM.OffsetRead(2068, ITPOffset, 5, out abytData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading ITP"));
                }
                else
                {
                    //Convert the bytes read to something useful.
                    MemoryStream DataStream = new MemoryStream(abytData);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                    uiMinutes = Reader.ReadUInt32();
                    bySeconds = Reader.ReadByte();
                }

                m_ITP = new DateTime(1970, 1, 1, 0, 0, 0);
                m_ITP = m_ITP.AddMinutes(uiMinutes);
                m_ITP = m_ITP.AddSeconds(bySeconds);
                m_ITP = m_ITP.ToLocalTime();

                return m_ITP;
            }
        }

        /// <summary>
        /// Returns the ITP from a full read of 2068
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  12/10/10 AF  2.45.20  163738  Function created.
        //
        public DateTime ITPFromFullRead
        {
            get
            {
                ReadUnloadedTable();
                
                return m_ITP;
            }
        }

        /// <summary>
        /// Determines if the meter is net registered. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/16/10 jrf 2.30.26        Created
        //
        public bool IsNetRegistered
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] abytData;
                bool blnNetRegistered = false;

                PSEMResult = m_PSEM.OffsetRead(2068, 217, 1, out abytData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading Net Registered value"));
                }
                else
                {
                    if (null != abytData && 0 < abytData.Length)
                    {
                        if (0 != abytData[0])
                        {
                            blnNetRegistered = true;
                        }
                    }
                }

                return blnNetRegistered;
            }
        }

        /// <summary>
        /// Gets the # of endpoints connected to Cell-Relay
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/19/10 MMD 2.41.03       Created
        //
        public int NetRoutingCount
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] abytData;
                int iNetCount = 0;

                PSEMResult = m_PSEM.OffsetRead(2068, 166, 1, out abytData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading Net Registered value"));
                }
                else
                {
                    if (null != abytData && 0 < abytData.Length)
                    {
                        iNetCount = abytData[0];
                    }
                }

                return iNetCount;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the size of the 2068 table
        /// </summary>
        /// <param name="strRFLANVersion">The RFLAN f/w version including the build number</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  12/10/10 AF  2.45.19  163738  Function created.
        //  12/20/10 AF  2.45.20  163738  Changed the input parameter from HW ver to RFLAN
        //                                f/w version.  The table size varied among the different
        //                                builds.
        //
        private static uint GetTableSize(string strRFLANVersion)
        {
            uint uiTableSize;
            if ((strRFLANVersion == RFLAN_FW_0_009_044) || (strRFLANVersion == RFLAN_FW_0_012_050))
            {
                uiTableSize = TABLE_LENGTH_2068_LONG;
            }
            else
            {
                uiTableSize = TABLE_LENGTH_2068_SHORT;
            }

            return uiTableSize;
        }

        #endregion

        #region Members

        private CachedUint m_RFLANMACAddress;
        private byte m_bytLevel;
        private DateTime m_ITP;
        private string m_strRFLANVersion;

        #endregion

    }

    /// <summary>
    /// The RFLANMfgTable2113 class handles the reading of the Reg Copy RFLAN Factory Config
    /// table.
    /// </summary>
    internal class RFLANMFGTable2113 : AnsiTable
    {
        #region Constants

        private const ushort TABLE_LENGTH = 112;
        private const ushort UTILITY_ID_OFFSET = 30;
        private const ushort RFLAN_MAC_ADDRESS_OFFSET = 26;

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
        public RFLANMFGTable2113(CPSEM psem)
            : base(psem, 2113, TABLE_LENGTH)
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
                m_Reader.ReadUInt32();

                //Utility ID
                m_bytUtilityID = m_Reader.ReadByte();

                //Add in other items as needed.
                //...

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
        /// write the value to the table.  
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
        /// This property gets/sets the MAC Address.  Setting this property will 
        /// write the value to the table.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/17/14 BLC                Created
        //
        public uint MacAddress
        {
            get
            {
                PSEMResponse Result = base.Read(RFLAN_MAC_ADDRESS_OFFSET, 4);

                if (PSEMResponse.Ok == Result)
                {
                    m_MACaddress = m_Reader.ReadUInt32();
                    return m_MACaddress;
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the MAC Address"));
                }
            }
            set
            {
                m_DataStream.Position = RFLAN_MAC_ADDRESS_OFFSET;
                m_Writer.Write(value);

                PSEMResponse Result = base.Write(RFLAN_MAC_ADDRESS_OFFSET, 4);

                if (PSEMResponse.Ok != Result)
                {
                    //We could not write the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_WRITE, Result,
                        "Error Writing the MAC Address"));
                }
            }
        }

        #endregion

        #region Member Variables

        private byte m_bytUtilityID;
        private uint m_MACaddress;

        #endregion

    }


    /// <summary>
    /// The C1222DebugInfoTable2114 class handles the reading of the C1222 Debug Info
    /// table.
    /// </summary>
    internal class C1222DebugInfoTable2114 : AnsiTable
    {
        #region Constants

        private const ushort TABLE_LENGTH = 559;
        private const byte CURRENT_EXCEPTIONS_OFFSET_SP51 = 0xAD;
        private const byte CURRENT_EXCEPTIONS_OFFSET_SR30 = 0xAF;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        /// <param name="fltFWRev">The register f/w version running in the meter.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/09/10 jrf 2.40.12         Created
        //  04/06/10 jrf 2.40.32         Passing in FW revision.  Table length and stucture 
        //                               are firmware version dependent.
        //
        public C1222DebugInfoTable2114(CPSEM psem, float fltFWRev)
            : base(psem, 2114, TABLE_LENGTH)
        {
            m_fltFWRev = fltFWRev;
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads some fwdl info.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/09/10 jrf 2.40.12         Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                //Add in other items as needed...

                // Misc Info
                m_Reader.ReadBytes(60);
                
                // Boot Loader Info
                m_Reader.ReadBytes(13);
                
                // Msg Reciever Info
                m_Reader.ReadBytes(54);
                
                // Msg Sender Info
                m_Reader.ReadBytes(16);
                
                // Lan Wan Table Intf
                m_Reader.ReadBytes(23);
                
                // More Debug Info
                //Read beginning of more debug info
                m_Reader.ReadBytes(7);
                
                //NumberOfCurrentExceptions 
                m_Reader.ReadByte();

                //Read remainder of more debug info
                m_Reader.ReadBytes(23);
                
                // FWDL Info
                //FWDLBlockCount
                m_Reader.ReadUInt16();
                //FWDLTotalBlockCount
                m_Reader.ReadUInt16();
                //IsActivateInProgress
                m_Reader.ReadByte();
                //FWDLEnabled
                m_Reader.ReadByte();

                // Rest of FWDL Info...
                m_Reader.ReadBytes(26);

                // More Debug Info 2
                m_Reader.ReadBytes(64);
                
                // More Debug Info 3
                m_Reader.ReadBytes(46);

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
        //  02/09/10 jrf 2.40.12         Created
        //
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
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/09/10 jrf 2.40.12         Created
        //
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets the current firmware download block count.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/09/10 jrf 2.40.12         Created
        //  03/17/10 jrf 2.40.26         Making this an offset read.
        //
        public ushort FWDLBlockCount
        {
            get
            {
                ushort usFWDLBlockCount = 0;
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] abytData;
                
                PSEMResult = m_PSEM.OffsetRead(2114, 199, 2, out abytData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading FWDL Block Count"));
                }
                else
                {
                    //Convert the bytes read to something useful.
                    MemoryStream DataStream = new MemoryStream(abytData);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                    usFWDLBlockCount = Reader.ReadUInt16();
                }

                return usFWDLBlockCount;
            }
        }

        /// <summary>
        /// This property gets the total firmware download block count.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/09/10 jrf 2.40.12         Created
        //  03/17/10 jrf 2.40.26         Making this an offset read.
        //
        public ushort TotalFWDLBlockCount
        {
            get
            {
                ushort usFWDLTotalBlockCount = 0;
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] abytData;

                PSEMResult = m_PSEM.OffsetRead(2114, 201, 2, out abytData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading FWDL Total Block Count"));
                }
                else
                {
                    //Convert the bytes read to something useful.
                    MemoryStream DataStream = new MemoryStream(abytData);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                    usFWDLTotalBlockCount = Reader.ReadUInt16();
                }

                return usFWDLTotalBlockCount;
            }
        }

        /// <summary>
        /// This property gets whether or not activation is occurring.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/09/10 jrf 2.40.12         Created
        //  03/17/10 jrf 2.40.26         Making this an offset read.
        //
        public bool IsActivateInProgress
        {
            get
            {
                bool blnActivateInProgress = false;
                byte bytIsActivateInProgress = 0;
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] abytData;

                PSEMResult = m_PSEM.OffsetRead(2114, 203, 1, out abytData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading Activate In Progress"));
                }
                else
                {
                    bytIsActivateInProgress = abytData[0];
                }

                if (0 != bytIsActivateInProgress)
                {
                    blnActivateInProgress = true;
                }

                return blnActivateInProgress;
            }
        }

        /// <summary>
        /// This property gets whether or not firmware download is enabled. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/09/10 jrf 2.40.12         Created
        //  03/17/10 jrf 2.40.26         Making this an offset read.
        //
        public bool FWDLEnabled
        {
            get
            {
                bool blnFWDLEnabled = false;
                byte bytFWDLEnabled = 0;
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] abytData;

                PSEMResult = m_PSEM.OffsetRead(2114, 204, 1, out abytData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading FWDL Enabled"));
                }
                else
                {
                    bytFWDLEnabled = abytData[0];
                }

                if (1 == bytFWDLEnabled)
                {
                    blnFWDLEnabled = true;
                }

                return blnFWDLEnabled;
            }
        }

        /// <summary>
        /// This property gets the number of exceptions currently queued up to be sent.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/10 jrf 2.40.26         Created
        //  04/06/10 jrf 2.40.32         Making current exceptions position based on
        //                               FW version.
        //
        public byte NumberOfCurrentExceptions
        {
            get
            {
                byte bytNumberOfCurrentExceptions = 0;
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] abytData;
                byte bytOffset = 0;

                if (VersionChecker.CompareTo(m_fltFWRev, CENTRON_AMI.VERSION_2_SP5_1) <= 0)
                {
                    bytOffset = CURRENT_EXCEPTIONS_OFFSET_SP51;
                }
                else
                {
                    bytOffset = CURRENT_EXCEPTIONS_OFFSET_SR30;
                }

                PSEMResult = m_PSEM.OffsetRead(2114, bytOffset, 1, out abytData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading Number of Current Exceptions"));
                }
                else
                {
                    bytNumberOfCurrentExceptions = abytData[0];
                }

                return bytNumberOfCurrentExceptions;
            }
        }

        #endregion

        #region Member Variables

        float m_fltFWRev;

        #endregion
    }

    /// <summary>
    /// RFLAN Information II table
    /// </summary>
    internal class RFLANMFGTable2119 : AnsiTable
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/14/10 RCG 2.40.06         Created

        public RFLANMFGTable2119(CPSEM psem)
            :base(psem, 2119, GetTableSize(psem.TimeFormat))
        {
        }

        /// <summary>
        /// Reads the table from the meter
        /// </summary>
        /// <returns>The result of the read</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/14/10 RCG 2.40.06         Created

        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2119.Read");

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
        /// Gets the RFLAN FW Boot Loader Revision
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/14/10 RCG 2.40.06         Created

        public byte RFLANFWLoaderRevision
        {
            get
            {
                ReadUnloadedTable();

                return m_byFWBootRevision;
            }
        }

        /// <summary>
        /// Gets the RFLAN FW Boot Loader Build
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/14/10 RCG 2.40.06         Created

        public byte RFLANFWLoaderBuild
        {
            get
            {
                ReadUnloadedTable();

                return m_byFWBootBuild;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of the table
        /// </summary>
        /// <param name="timeFormat">The time format of the meter.</param>
        /// <returns>The size of the table in bytes</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/14/10 RCG 2.40.06         Created

        private static ushort GetTableSize(int timeFormat)
        {
            return (ushort)(162 + 2 * CTable00.DetermineLTIMESize(timeFormat));
        }

        /// <summary>
        /// Parses the data from the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/14/10 RCG 2.40.06         Created

        private void ParseData()
        {
            m_usLLCRetryBucketNone = m_Reader.ReadUInt16();
            m_usLLCRetryBucket1 = m_Reader.ReadUInt16();
            m_usLLCRetryBucket2 = m_Reader.ReadUInt16();
            m_usLLCRetryBucket3 = m_Reader.ReadUInt16();
            m_usLLCRetryBucketLessThan10 = m_Reader.ReadUInt16();
            m_usLLCRetryBucketLessThan15 = m_Reader.ReadUInt16();
            m_usLLCRetryBucketGreaterThan15 = m_Reader.ReadUInt16();
            m_dtLastCellRegistrationRequestTime = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
            m_dtLastRegistrationChangeTime = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
            m_uiRoutingTableRemovalCount = m_Reader.ReadUInt32();
            m_usCurrentRoutingTableRemovalPeriod = m_Reader.ReadUInt16();
            m_usNetRxTotalRxCount = m_Reader.ReadUInt16();
            m_usNetRxUplink = m_Reader.ReadUInt16();
            m_usNetRxDownlink = m_Reader.ReadUInt16();
            m_usNetRxBroadcast = m_Reader.ReadUInt16();
            m_usNetRxBrokenLink = m_Reader.ReadUInt16();
            m_usNetRxNeighborList = m_Reader.ReadUInt16();
            m_usNetRxOutage = m_Reader.ReadUInt16();
            m_usNetRxUplinkWithNeighbors = m_Reader.ReadUInt16();
            m_usNetRxCellRegistration = m_Reader.ReadUInt16();
            m_usNetRxCellRegConf = m_Reader.ReadUInt16();
            m_usNetRxCellOutNotification = m_Reader.ReadUInt16();
            m_usNetRxCellLeavingNotification = m_Reader.ReadUInt16();
            m_usNetRxITP = m_Reader.ReadUInt16();
            m_usNetRxReserved12 = m_Reader.ReadUInt16();
            m_usNetRxReserved13 = m_Reader.ReadUInt16();
            m_usNetRxReserved14 = m_Reader.ReadUInt16();
            m_usNetRxTrace = m_Reader.ReadUInt16();
            m_usNetTxTotalTxCount = m_Reader.ReadUInt16();
            m_usNetTxUplink = m_Reader.ReadUInt16();
            m_usNetTxDownlink = m_Reader.ReadUInt16();
            m_usNetTxBroadcast = m_Reader.ReadUInt16();
            m_usNetTxBrokenLink = m_Reader.ReadUInt16();
            m_usNetTxNeighborList = m_Reader.ReadUInt16();
            m_usNetTxOutage = m_Reader.ReadUInt16();
            m_usNetTxUplinkWithNeighbors = m_Reader.ReadUInt16();
            m_usNetTxCellRegistration = m_Reader.ReadUInt16();
            m_usNetTxCellRegConf = m_Reader.ReadUInt16();
            m_usNetTxCellOutNotification = m_Reader.ReadUInt16();
            m_usNetTxCellLeavingNotification = m_Reader.ReadUInt16();
            m_usNetTxITP = m_Reader.ReadUInt16();
            m_usNetTxReserved12 = m_Reader.ReadUInt16();
            m_usNetTxReserved13 = m_Reader.ReadUInt16();
            m_usNetTxReserved14 = m_Reader.ReadUInt16();
            m_usNetTxTrace = m_Reader.ReadUInt16();
            m_byCurrentMACEndPointType = m_Reader.ReadByte();
            m_byHW15CRReadITPTimeFailureCount = m_Reader.ReadByte();
            m_uiHW15OutageMessageDroppedCount = m_Reader.ReadUInt32();
            m_byHW15C1222Flags = m_Reader.ReadByte();
            m_uiNetworkThrashingTimerDuration = m_Reader.ReadUInt32();
            m_uiNetworkThrashingTimerTickLatch = m_Reader.ReadUInt32();
            m_uiNetworkThrashingTimerCounter = m_Reader.ReadUInt32();
            m_byNetworkThrashingTimerEnabled = m_Reader.ReadByte();
            m_usNetCloneFailure = m_Reader.ReadUInt16();
            m_ui5msTimerDeltaAvg = m_Reader.ReadUInt32();
            m_ui5msTimerDeltaCnt = m_Reader.ReadUInt32();
            m_ui5msTimerDeltaMax = m_Reader.ReadUInt32();
            m_ui5msTimerDeltaMin = m_Reader.ReadUInt32();
            m_uiXTALDriftDeltaAvg = m_Reader.ReadUInt32();
            m_uiXTALDriftDeltaCnt = m_Reader.ReadUInt32();
            m_uiXTALDriftDeltaMax = m_Reader.ReadUInt32();
            m_uiXTALDriftDeltaMin = m_Reader.ReadUInt32();
            m_byMACHyperFrameNumber = m_Reader.ReadByte();
            m_byNetCurrentRegRetry = m_Reader.ReadByte();
            m_byFWApplicationRevision = m_Reader.ReadByte();
            m_byFWApplicationBuild = m_Reader.ReadByte();
            m_usOCR1A = m_Reader.ReadUInt16();
            m_usOutageSubTimSlot0 = m_Reader.ReadUInt16();
            m_usOutageSubTimeSlot1 = m_Reader.ReadUInt16();
            m_usOutageSubTimeSlot2 = m_Reader.ReadUInt16();
            m_usOutageID = m_Reader.ReadUInt16();
            m_uiRFTxTimeOutCounter = m_Reader.ReadUInt32();
            m_byFWBootRevision = m_Reader.ReadByte();
            m_byFWBootBuild = m_Reader.ReadByte();
        }

        #endregion

        #region Member Variables

        private ushort m_usLLCRetryBucketNone;
        private ushort m_usLLCRetryBucket1;
        private ushort m_usLLCRetryBucket2;
        private ushort m_usLLCRetryBucket3;
        private ushort m_usLLCRetryBucketLessThan10;
        private ushort m_usLLCRetryBucketLessThan15;
        private ushort m_usLLCRetryBucketGreaterThan15;
        private DateTime m_dtLastCellRegistrationRequestTime;
        private DateTime m_dtLastRegistrationChangeTime;
        private uint m_uiRoutingTableRemovalCount;
        private ushort m_usCurrentRoutingTableRemovalPeriod;
        private ushort m_usNetRxTotalRxCount;
        private ushort m_usNetRxUplink;
        private ushort m_usNetRxDownlink;
        private ushort m_usNetRxBroadcast;
        private ushort m_usNetRxBrokenLink;
        private ushort m_usNetRxNeighborList;
        private ushort m_usNetRxOutage;
        private ushort m_usNetRxUplinkWithNeighbors;
        private ushort m_usNetRxCellRegistration;
        private ushort m_usNetRxCellRegConf;
        private ushort m_usNetRxCellOutNotification;
        private ushort m_usNetRxCellLeavingNotification;
        private ushort m_usNetRxITP;
        private ushort m_usNetRxReserved12;
        private ushort m_usNetRxReserved13;
        private ushort m_usNetRxReserved14;
        private ushort m_usNetRxTrace;
        private ushort m_usNetTxTotalTxCount;
        private ushort m_usNetTxUplink;
        private ushort m_usNetTxDownlink;
        private ushort m_usNetTxBroadcast;
        private ushort m_usNetTxBrokenLink;
        private ushort m_usNetTxNeighborList;
        private ushort m_usNetTxOutage;
        private ushort m_usNetTxUplinkWithNeighbors;
        private ushort m_usNetTxCellRegistration;
        private ushort m_usNetTxCellRegConf;
        private ushort m_usNetTxCellOutNotification;
        private ushort m_usNetTxCellLeavingNotification;
        private ushort m_usNetTxITP;
        private ushort m_usNetTxReserved12;
        private ushort m_usNetTxReserved13;
        private ushort m_usNetTxReserved14;
        private ushort m_usNetTxTrace;
        private byte m_byCurrentMACEndPointType;
        private byte m_byHW15CRReadITPTimeFailureCount;
        private uint m_uiHW15OutageMessageDroppedCount;
        private byte m_byHW15C1222Flags;
        private uint m_uiNetworkThrashingTimerDuration;
        private uint m_uiNetworkThrashingTimerTickLatch;
        private uint m_uiNetworkThrashingTimerCounter;
        private byte m_byNetworkThrashingTimerEnabled;
        private ushort m_usNetCloneFailure;
        private uint m_ui5msTimerDeltaAvg;
        private uint m_ui5msTimerDeltaCnt;
        private uint m_ui5msTimerDeltaMax;
        private uint m_ui5msTimerDeltaMin;
        private uint m_uiXTALDriftDeltaAvg;
        private uint m_uiXTALDriftDeltaCnt;
        private uint m_uiXTALDriftDeltaMax;
        private uint m_uiXTALDriftDeltaMin;
        private byte m_byMACHyperFrameNumber;
        private byte m_byNetCurrentRegRetry;
        private byte m_byFWApplicationRevision;
        private byte m_byFWApplicationBuild;
        private ushort m_usOCR1A;
        private ushort m_usOutageSubTimSlot0;
        private ushort m_usOutageSubTimeSlot1;
        private ushort m_usOutageSubTimeSlot2;
        private ushort m_usOutageID;
        private uint m_uiRFTxTimeOutCounter;
        private byte m_byFWBootRevision;
        private byte m_byFWBootBuild;

        #endregion
    }

    /// <summary>
    /// The RFLANMfgTable2121 class handles the reading of the RFLAN Factory Config
    /// table.
    /// </summary>
    internal class RFLANMFGTable2121 : AnsiTable
    {
        #region Constants

        private const int TABLE_LENGTH_LDR = 112;
        private const int TABLE_LENGTH_HDR = 116;
        private const byte LDR_RFLAN_REVISION = 10;
        private const ushort RFLAN_MAC_ADDRESS_OFFSET = 26;
        private const ushort UTILITY_ID_OFFSET = 30;
        private const ushort EXPANSION_BITS_OFFSET = 103;
        private const ushort SCM_ERT_ID_OFFSET = 111;

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

        public RFLANMFGTable2121(CPSEM psem)
            : base(psem, 2121, TABLE_LENGTH_LDR)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        /// <param name="bytRFLANRev">The RFLAN firmware revision of the meter.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/26/10 jrf 2.42.06        Created.
        //  10/29/10 jrf 2.45.08        Changing to pass in RFLAN firmware revision.
        public RFLANMFGTable2121(CPSEM psem, byte bytRFLANRev)
            : base(psem, 2121, GetTableLength(bytRFLANRev))
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
                m_Reader.ReadUInt32();

                //Utility ID
                m_bytUtilityID = m_Reader.ReadByte();

                //Add in other items as needed.
                //...

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
        /// write the value to the table.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/21/09 jrf 2.30.12        Created
        //  04/21/10 jrf 2.40.41        Added exception if offset read failed.
        //
        public byte UtilityID
        {
            get
            {
                PSEMResponse Result = base.Read(UTILITY_ID_OFFSET, 1);

                if (PSEMResponse.Ok == Result)
                {
                   m_bytUtilityID = m_Reader.ReadByte();
                   return m_bytUtilityID;
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the Utility ID"));
                }
            }
            set
            {
                m_DataStream.Position = UTILITY_ID_OFFSET;
                m_Writer.Write(value);

                PSEMResponse Result = base.Write(UTILITY_ID_OFFSET, 1);

                if (PSEMResponse.Ok != Result)
                {
                    //We could not write the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_WRITE, Result,
                        "Error Writing the Utility ID"));
                }
            }
        }


        /// <summary>
        /// This property gets/sets the MAC Address.  Setting this property will 
        /// write the value to the table.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/17/14 BLC                Created
        //
        public uint MacAddress
        {
            get
            {
                PSEMResponse Result = base.Read(RFLAN_MAC_ADDRESS_OFFSET, 4);

                if (PSEMResponse.Ok == Result)
                {
                    m_MACaddress = m_Reader.ReadUInt32();
                    return m_MACaddress;
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the Utility ID"));
                }
            }
            set
            {
                m_DataStream.Position = RFLAN_MAC_ADDRESS_OFFSET;
                m_Writer.Write(value);

                PSEMResponse Result = base.Write(RFLAN_MAC_ADDRESS_OFFSET, 4);

                if (PSEMResponse.Ok != Result)
                {
                    //We could not write the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_WRITE, Result,
                        "Error Writing the MAC Address"));
                }
            }
        }

        /// <summary>
        /// This property gets/sets the Expansion Control Bits, which includes the Cell Switch Parameter Selection.
        /// The Cell Switch Parameter Selection can be used to get/set the RFLAN level.
        /// Setting this property will write the value to the table.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  06/30/14 AF  3.51.01 WR 518450 Created
        //
        public byte ExpansionControlBits
        {
            get
            {
                PSEMResponse Result = base.Read(EXPANSION_BITS_OFFSET, 1);

                if (PSEMResponse.Ok == Result)
                {
                    m_bytExpansionControlBits = m_Reader.ReadByte();
                    return m_bytExpansionControlBits;
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the Expansion Control Bits"));
                }
            }
            set
            {
                m_DataStream.Position = EXPANSION_BITS_OFFSET;
                m_Writer.Write(value);

                PSEMResponse Result = base.Write(EXPANSION_BITS_OFFSET, 1);

                if (PSEMResponse.Ok != Result)
                {
                    //We could not write the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_WRITE, Result,
                        "Error Writing the Expansion Control Bits"));
                }
            }
        }

        /// <summary>
        /// This property gets/sets the Utility ID.  Setting this property will 
        /// write the value to the table.  
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/26/10 jrf 2.42.06        Created.
        //  10/29/10 jrf 2.45.08        This value is not available for LDR.
        public uint SCMERTID
        {
            get
            {
                m_uiSCMERTID = 0;
   
                if (TABLE_LENGTH_LDR < m_Size)
                {
                    PSEMResponse Result = base.Read(SCM_ERT_ID_OFFSET, 4);

                    if (PSEMResponse.Ok == Result)
                    {
                        m_uiSCMERTID = m_Reader.ReadUInt32();
                    }
                    else
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading the SCM ERT ID"));
                    }
                }

                return m_uiSCMERTID;
            }
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Determines the length of table 2121 based on the firmware version
        /// in the meter
        /// </summary>
        /// <param name="bytRFLANRev">the firmware revision of the RFLAN f/w</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/19/10 MMD  2.30.01       Created to handle new fields added in SR 3.0
        //  10/29/10 jrf  2.45.08       Making table length conditional on the RFLAN 
        //                              firmware revision.
        static private uint GetTableLength(byte bytRFLANRev)
        {
            uint uiTableLength;

            if (bytRFLANRev > LDR_RFLAN_REVISION)
            {
                uiTableLength = TABLE_LENGTH_HDR;
            }
            else
            {
                uiTableLength = TABLE_LENGTH_LDR;
            }

            return uiTableLength;
        }

        #endregion


        #region Member Variables

        private uint m_MACaddress;
        private byte m_bytUtilityID;
        private uint m_uiSCMERTID;
        private byte m_bytExpansionControlBits;

        #endregion

    }
}
