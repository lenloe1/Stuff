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
//                              Copyright © 20?? - 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Table 121 - Network Table
    /// </summary>
    public class CStdTable121 : AnsiTable
    {
        #region Constants

        private const int TABLE121_SIZE = 14;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CStdTable121 Table121 = new CStdTable127( PSEM ); 
        /// </code>
        /// </example>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/16/06 KRC 7.35.00 N/A    Created
        //  10/03/06 AF  7.40.00 N/A    Added native address length and number of
        //                              registrations variable initializations
        //  05/21/09 jrf 2.20.05 127678 Adding previously unretrieved table items.
        // 
        public CStdTable121(CPSEM psem)
            : base(psem, 121, TABLE121_SIZE, TABLE_TIMEOUT)
        {
            m_usNbrStatistics = 0;
            m_usNbrFilteringRules = 0;
            m_usNbrExceptionHosts = 0;
            m_usNbrExceptionEvents = 0;
            m_bytNativeAddressLength = 0;
            m_bytNbrRegistrations = 0;
            m_blnProgNativeAddress = false;
            m_blnProgBroadcastAddress = false;
            m_blnStaticRelay = false;
            m_blnStaticAptitle = false;
            m_blnStaticMasterRelay = false;
            m_blnClientResponseCtrl = false;
            m_bytNbrInterfaces = 0;
            m_bytNbrMulticastAddresses = 0;
        }

        /// <summary>
        /// Table 121 Constructor used when parsing an EDL file
        /// </summary>
        /// <param name="BinaryReader">A binary Reader contain the stream for 121</param>
        public CStdTable121(PSEMBinaryReader BinaryReader)
            : base(121, TABLE121_SIZE)
        {
            m_TableState = TableState.Loaded;
            m_Reader = BinaryReader;
            ParseData();
        }

        /// <summary>
        /// Reads table 121 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/15/06 KRC 7.35.00 N/A    Created
        //  10/03/06 AF  7.40.xx N/A    Added read of number of registrations and
        //                              native address length.
        //  03/06/08 KRC 1.50.00 N/A    Adding EDL support to the class
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CStdTable121.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the number of network interfaces supported
        /// </summary>
        /// <returns>A byte that is the number of registrations</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/12/06 AF  7.40.00 N/A    Created
        //
        public byte NumberOfInterfaces
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return m_bytNbrInterfaces;
            }
        }

        /// <summary>
        /// Returns the number of registrations
        /// </summary>
        /// <returns>A byte that is the number of registrations</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/03/06 AF  7.40.00 N/A    Created
        //
        public byte NumberOfRegistrations
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Registrations"));
                    }
                }

                return m_bytNbrRegistrations;
            }
        }

        /// <summary>
        /// Returns the Number of Statistics
        /// </summary>
        /// <returns>A ushort that is the number of statistics </returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/15/06 KRC 7.35.00 N/A    Created
        //  05/21/09 jrf 2.20.05        Updated to return a UInt16 per the configuration
        //                              data spec.
        //  
        public UInt16 NumberOfStatistics
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of statistics"));
                    }
                }

                return m_usNbrStatistics;
            }
        }

        /// <summary>
        /// Returns the number of multicast addresses supported
        /// </summary>
        /// <returns>A byte that is the number of multicast addresses.</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/12/06 AF  7.40.00 N/A    Created
        //
        public byte NumberOfMulticastAddresses
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Number of Interfaces"));
                    }
                }

                return m_bytNbrMulticastAddresses;
            }
        }

        /// <summary>
        /// Returns the native address length
        /// </summary>
        /// <returns>A byte that is the native address length</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/03/06 AF  7.40.00 N/A    Created
        //
        public byte NativeAddressLength
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Native Address Length"));
                    }
                }

                return m_bytNativeAddressLength;
            }
        }

        /// <summary>
        /// Returns whether or not this node can be programmed with a static ApTitle
        /// </summary>
        /// <returns>true or false</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/12/06 AF  7.40.00 N/A    Created
        //
        public bool StaticAptitle
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Static ApTitle"));
                    }
                }

                return m_blnStaticAptitle;
            }
        }

        /// <summary>
        /// Returns whether or not interface native addresses can be set in table 122
        /// </summary>
        /// <returns>true or false</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/12/06 AF  7.40.00 N/A    Created
        //
        public bool ProgNativeAddress
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Prog Native Address"));
                    }
                }

                return m_blnProgNativeAddress;
            }
        }

        /// <summary>
        /// Returns whether or not interface broadcast addresses can be set in table 122
        /// </summary>
        /// <returns>true or false</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/12/06 AF  7.40.00 N/A    Created
        //
        public bool ProgBroadcastAddress
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Prog Broadcast Address"));
                    }
                }

                return m_blnProgBroadcastAddress;
            }
        }

        /// <summary>
        /// Returns whether or not interface broadcast addresses can be set in table 122
        /// </summary>
        /// <returns>true or false</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/12/06 AF  7.40.00 N/A    Created
        //
        public bool StaticRelay
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Static Relay"));
                    }
                }

                return m_blnStaticRelay;
            }
        }

        /// <summary>
        /// Returns whether or not node can be programmed with a static ApTitle
        /// </summary>
        /// <returns>true or false</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/12/06 AF  7.40.00 N/A    Created
        //
        public bool StaticApTitle
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Static ApTitle"));
                    }
                }

                return m_blnStaticAptitle;
            }
        }

        /// <summary>
        /// Returns whether or not the association with a master relay can be
        /// programmed with a static ApTitle in table 122
        /// </summary>
        /// <returns>true or false</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/12/06 AF  7.40.00 N/A    Created
        //
        public bool StaticMasterRelay
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Static Master Relay"));
                    }
                }

                return m_blnStaticMasterRelay;
            }
        }

        /// <summary>
        /// Returns whether or not table 122 is capable of providing client response
        /// control parameters
        /// </summary>
        /// <returns>true or false</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/12/06 AF  7.40.00 N/A    Created
        //
        public bool ClientResponseControl
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Client Response Control"));
                    }
                }

                return m_blnClientResponseCtrl;
            }
        }

        /// <summary>
        /// Returns the number of exception hosts in table 123.
        /// </summary>
        /// <returns>A ushort that is the number of exception hosts.</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/20/09 jrf 2.20.05 127678 Created.
        //
        public UInt16 NumberOfExceptionHosts
        {
            get
            {
                ReadUnloadedTable();

                return m_usNbrExceptionHosts;
            }
        }

        /// <summary>
        /// Returns the number of exception events in table 123.
        /// </summary>
        /// <returns>A ushort that is the number of exception events.</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/20/09 jrf 2.20.05 127678 Created.
        //
        public UInt16 NumberOfExceptionEvents
        {
            get
            {
                ReadUnloadedTable();

                return m_usNbrExceptionEvents;
            }
        }

        /// <summary>
        /// Returns the size of standard table 122
        /// </summary>
        /// <returns>An unsigned integer that is the size of table 122</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/12/06 AF  7.40.00 N/A    Created
        //
        public uint SizeOfTable122
        {
            get
            {
                int iEntryLength = 0;
                int iTable122Size = 0;

                // Add the length of the electronic serial number if supported
                if (false == StaticAptitle)
                {
                    iTable122Size += 20;
                }

                // Add length of the preconfigured native address if supported
                if (false != ProgNativeAddress)
                {
                    iEntryLength += NativeAddressLength;
                }

                // Add length of preconfigured broadcast address if supported
                if (false != ProgBroadcastAddress)
                {
                    iEntryLength += NativeAddressLength;
                }

                // Add length of relay native address if supported
                if (false != StaticRelay)
                {
                    iEntryLength += NativeAddressLength;
                }

                // Add length of node aptitle if preconfigured
                if (false != StaticApTitle)
                {
                    iEntryLength += 20;
                }

                // Add length of master relay aptitle if preconfigured
                if (false != StaticMasterRelay)
                {
                    iEntryLength += 20;
                }

                // Add nbr of retries and response timeout if table 122 is
                // capable of providing client response control parameters
                if (false != ClientResponseControl)
                {
                    iEntryLength += 3;
                }

                iTable122Size += (iEntryLength * NumberOfInterfaces);
                iTable122Size += (NumberOfMulticastAddresses * 20);

                return (uint)iTable122Size;
            }
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Parses the data out of the reader and into the member variables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Adding parsing of previously ignored
        //                              table items.
        //
        private void ParseData()
        {
            // Populate the member variables that represent the table
            byte bytDimNetworkBfld = m_Reader.ReadByte();
            // Mask off the various fields in the bit field
            m_blnProgNativeAddress = (0 != (bytDimNetworkBfld & 0x02));
            m_blnProgBroadcastAddress = (0 != (bytDimNetworkBfld & 0x04));
            m_blnStaticRelay = (0 != (bytDimNetworkBfld & 0x08));
            m_blnStaticAptitle = (0 != (bytDimNetworkBfld & 0x10));
            m_blnStaticMasterRelay = (0 != (bytDimNetworkBfld & 0x20));
            m_blnClientResponseCtrl = (0 != (bytDimNetworkBfld & 0x40));

            m_bytNbrInterfaces = m_Reader.ReadByte();
            m_bytNbrRegistrations = m_Reader.ReadByte();
            m_usNbrFilteringRules = m_Reader.ReadUInt16();
            m_usNbrExceptionHosts = m_Reader.ReadUInt16();
            m_usNbrExceptionEvents = m_Reader.ReadUInt16();
            m_usNbrStatistics = m_Reader.ReadUInt16();
            m_bytNbrMulticastAddresses = m_Reader.ReadByte();
            m_bytNativeAddressLength = m_Reader.ReadByte();
        }

        #endregion Private Method

        #region Members

        private UInt16 m_usNbrStatistics;
        private UInt16 m_usNbrFilteringRules;
        private UInt16 m_usNbrExceptionHosts;
        private UInt16 m_usNbrExceptionEvents;
        private byte m_bytNbrRegistrations;
        private byte m_bytNativeAddressLength;
        private byte m_bytNbrInterfaces;
        private byte m_bytNbrMulticastAddresses;
        private bool m_blnProgNativeAddress;
        private bool m_blnProgBroadcastAddress;
        private bool m_blnStaticRelay;
        private bool m_blnStaticAptitle;
        private bool m_blnStaticMasterRelay;
        private bool m_blnClientResponseCtrl;

        #endregion
    }

    /// <summary>
    /// Table 122 - Interface Control Table
    /// </summary>
    public class CStdTable122 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;
        private const int COMM_MODULE = 0;
        private const int BYTES_IN_MULTICAST_ADDRESS = 20;
        private const int BYTES_IN_APTITLE = 20;
        private const int BYTES_IN_ESN = 20;
        private const int NATIVE_ADDRESS_UTILITY_ID_OFFSET = 4;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="tbl121">Pointer to Table 121</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CStdTable127 Table127 = new CStdTable127( PSEM, uiLenght ); 
        /// </code>
        /// </example>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/28/06 KRC 7.35.00 N/A    Created
        //  05/21/09 jrf 2.20.05 127678 Adding retrieval of interfaces and 
        //                              multicast addresses.
        //
        public CStdTable122(CPSEM psem, CStdTable121 tbl121)
            : base(psem, 122, tbl121.SizeOfTable122, TABLE_TIMEOUT)
        {
            m_tbl121 = tbl121;
            m_lstInterfaces = new List<InterfaceControlEntryRecord>();
            m_lstMulticastAddresses = new List<byte[]>();
            m_lstRawNativeAddresses = new List<byte[]>();
        }

        /// <summary>
        /// Reads table 122 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/15/06 KRC 7.35.00 N/A    Created
        // 05/21/09 jrf 2.20.05 127678 Expanded to fully read the table.
        //
        public override PSEMResponse Read()
        {
            byte[] byRawESN = new byte[BYTES_IN_ESN];
            byte[] abytTemp = new byte[m_tbl121.NativeAddressLength];
            m_lstInterfaces.Clear();
            m_lstMulticastAddresses.Clear();
            m_lstRawNativeAddresses.Clear();

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CStdTable122.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                // Get the ESN
                if (false == m_tbl121.StaticApTitle)
                {
                    //Populate the member variables that represent the table
                    byRawESN = m_Reader.ReadBytes(BYTES_IN_ESN);

                    // Now decode the ESN
                    m_strElectronicSerialNumber = ESNConverter.Decode(byRawESN);

                }

                // Get the interface records
                for (int i = 0; i < m_tbl121.NumberOfInterfaces; i++)
                {
                    InterfaceControlEntryRecord InterfaceRecord = new InterfaceControlEntryRecord();

                    // Check for native address
                    if (true == m_tbl121.ProgNativeAddress)
                    {
                        abytTemp = m_Reader.ReadBytes(m_tbl121.NativeAddressLength);
                        
                        //Keeping track of the raw native addresses.
                        m_lstRawNativeAddresses.Add(abytTemp);

                        for (int intIndex = 0; intIndex < m_tbl121.NativeAddressLength; intIndex++)
                        {
                            InterfaceRecord.NativeAddress += abytTemp[intIndex].ToString(CultureInfo.InvariantCulture) + " ";
                        }

                        InterfaceRecord.NativeAddress.Trim();
                    }

                    // Check for broadcast address
                    if (true == m_tbl121.ProgBroadcastAddress)
                    {
                        abytTemp = m_Reader.ReadBytes(m_tbl121.NativeAddressLength);

                        for (int intIndex = 0; intIndex < m_tbl121.NativeAddressLength; intIndex++)
                        {
                            InterfaceRecord.BroadcastAddress += abytTemp[intIndex].ToString(CultureInfo.InvariantCulture) + " ";
                        }

                        InterfaceRecord.BroadcastAddress.Trim();
                    }

                    // Check for relay native address
                    if (true == m_tbl121.StaticRelay)
                    {
                        abytTemp = m_Reader.ReadBytes(m_tbl121.NativeAddressLength);

                        for (int intIndex = 0; intIndex < m_tbl121.NativeAddressLength; intIndex++)
                        {
                            InterfaceRecord.RelayNativeAddress += abytTemp[intIndex].ToString(CultureInfo.InvariantCulture) + " ";
                        }

                        InterfaceRecord.RelayNativeAddress.Trim();
                    }

                    // Check for aptitle
                    if (true == m_tbl121.StaticApTitle)
                    {
                        InterfaceRecord.ApTitle = m_Reader.ReadBytes(BYTES_IN_APTITLE);
                    }

                    // Check for master aptitle
                    if (true == m_tbl121.StaticMasterRelay)
                    {
                        InterfaceRecord.MasterRelayApTitle = m_Reader.ReadBytes(BYTES_IN_APTITLE);
                    }

                    // Check for client response control items
                    if (true == m_tbl121.ClientResponseControl)
                    {
                        InterfaceRecord.NumberRetries = m_Reader.ReadByte();
                        InterfaceRecord.ResponseTimeout = m_Reader.ReadUInt16();
                    }

                    // Store off interfaces
                    m_lstInterfaces.Add(InterfaceRecord);
                }

                // Get the multicast addresses
                for (int i = 0; i < m_tbl121.NumberOfMulticastAddresses; i++)
                {
                    abytTemp = m_Reader.ReadBytes(BYTES_IN_MULTICAST_ADDRESS);
                    m_lstMulticastAddresses.Add(abytTemp);
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the Electronic Serial Number
        /// </summary>
        /// <returns>A string that is the Electronic Serial Number
        /// </returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/15/06 KRC 7.35.00 N/A    Created
        //  
        public string ElectronicSerialNumber
        {
            get
            {
                ReadUnloadedTable();

                return m_strElectronicSerialNumber;
            }
            
        }

        /// <summary>
        /// Native Address assigned to this interface
        /// </summary>
        /// <returns>A string that is the Native Address</returns>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/09/06 AF  7.40.00 N/A    Created
        //  05/21/09 jrf 2.20.05 127678 Pulling native address from the comm module
        //                              interface.
        public string NativeAddress
        {
            get
            {
                ReadUnloadedTable();

                return m_lstInterfaces[COMM_MODULE].NativeAddress;
            }
        }

        /// <summary>
        /// Raw Native Address assigned to this interface
        /// </summary>
        /// <returns>A string that is the Native Address</returns>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/19/10 jrf 2.40.27 N/A    Created
        //  04/06/10 jrf 2.40.32 n/a    Setting datastream's position to usOffset.
        // 
        public byte[] RawNativeAddress
        {
            get
            {
                ReadUnloadedTable();
                byte[] abytTemp = new byte[0];

                if (0 < m_tbl121.NumberOfInterfaces)
                {
                    abytTemp = m_lstRawNativeAddresses[COMM_MODULE];
                }

                return abytTemp;
            }
            set
            {
                // Set the Native Address
                ushort usOffset = 0;

                if (false == m_tbl121.StaticApTitle)
                {
                    usOffset = BYTES_IN_ESN;
                }

                if (0 < m_tbl121.NumberOfInterfaces)
                {

                    m_lstRawNativeAddresses[COMM_MODULE] = value;

                    m_DataStream.Position = usOffset;
                    m_Writer.Write(value);

                    PSEMResponse Resp = base.Write(usOffset, (ushort)(value.Length));
                    State = TableState.Dirty;
                }

            }
        }

        /// <summary>
        /// Utility ID portion of the Raw Native Address.
        /// </summary>
        /// <returns>A byte that is the Utility ID</returns>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/10/13 jrf 2.71.01 417021 Created.
        // 
        public byte? UtilityID
        {
            get
            {
                ReadUnloadedTable();
                byte[] abytTemp = new byte[0];
                byte? byUtilityID = null;

                if (0 < m_tbl121.NumberOfInterfaces)
                {
                    abytTemp = m_lstRawNativeAddresses[COMM_MODULE];
                }

                if (null != abytTemp)
                {
                    byUtilityID = abytTemp[NATIVE_ADDRESS_UTILITY_ID_OFFSET];
                }

                return byUtilityID;
            }
            set
            {
                if (null != value)
                {
                    ushort usOffset = 0;

                    if (false == m_tbl121.StaticApTitle)
                    {
                        usOffset = BYTES_IN_ESN;
                    }

                    if (0 < m_tbl121.NumberOfInterfaces)
                    {
                        //Making sure native address value is populated.
                        ReadUnloadedTable();
                        
                        usOffset += NATIVE_ADDRESS_UTILITY_ID_OFFSET;

                        (m_lstRawNativeAddresses[COMM_MODULE])[NATIVE_ADDRESS_UTILITY_ID_OFFSET] = value.Value;

                        m_DataStream.Position = usOffset;
                        m_Writer.Write(value.Value);

                        base.Write(usOffset, sizeof(byte));
                        State = TableState.Dirty;
                    }
                }

            }
        }

        /// <summary>
        /// Multicast Addresses assigned to this interface.
        /// </summary>
        /// <returns>A string[] that contains the multicast addresses.</returns>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// 		</exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //  03/15/10 RCG 2.40.25 143594 Displaying Multicast Addresses as an ESN
 
        public List<string> MulticastAddresses
        {
            get
            {
                List<string> Addresses = new List<string>();

                ReadUnloadedTable();

                foreach (byte[] CurrentAddress in m_lstMulticastAddresses)
                {
                    Addresses.Add(ESNConverter.Decode(CurrentAddress));
                }

                return Addresses;
            }
        }

        #endregion

        #region Members

        private CStdTable121 m_tbl121;
        private string m_strElectronicSerialNumber;
        private List<InterfaceControlEntryRecord> m_lstInterfaces;
        private List<byte[]> m_lstMulticastAddresses;
        private List<byte[]> m_lstRawNativeAddresses;

        #endregion
    }

    /// <summary>
    /// Table 123 - Exception Report Table
    /// </summary>
    public class CStdTable123 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 5000;
        private const uint EXCEPTION_HOST_RCD_STATIC_SIZE = 25;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="Table121">Pointer to Table 121</param>
        /// <param name="eventDictionary">The device's event dictionary</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        public CStdTable123(CPSEM psem, CStdTable121 Table121, ANSIEventDictionary eventDictionary)
            : base(psem, 123, GetTableSize(Table121), TABLE_TIMEOUT)
        {
            m_Table121 = Table121;
            m_lstExceptionHostRecords = new List<ExceptionReportEntryRecord>();
            m_EventDictionary = eventDictionary;
        }

        /// <summary>
        /// Table 123 Constructor used when parsing an EDL file
        /// </summary>
        /// <param name="BinaryReader">A binary Reader contain the stream for 123</param>
        /// <param name="Table121">A table 121 object.</param>
        /// <param name="eventDictionary">The device's event dictionary</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        public CStdTable123(PSEMBinaryReader BinaryReader, CStdTable121 Table121, ANSIEventDictionary eventDictionary)
            : base(123, GetTableSize(Table121))
        {
            m_Table121 = Table121;
            m_lstExceptionHostRecords = new List<ExceptionReportEntryRecord>();
            
            m_TableState = TableState.Loaded;
            m_Reader = BinaryReader;
            m_EventDictionary = eventDictionary;
            ParseData();
        }

        /// <summary>
        /// Reads table 123 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/20/09 jrf 2.20.05 127678 Created.
        //  06/12/09 krc 2.20.07 129652 Don't try to read table 123 if it isn't
        //                              populated
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CStdTable123.Read");

            PSEMResponse Result = PSEMResponse.Ok;

            // Don't read Table 123 if there are no exception hosts
            if (m_Table121.NumberOfExceptionHosts > 0)
            {
                //Read the table			
                Result = base.Read();

                if (PSEMResponse.Ok == Result)
                {
                    m_DataStream.Position = 0;

                    ParseData();
                }
            }
            else
            {
                m_lstExceptionHostRecords.Clear();
                State = TableState.Loaded;
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns an array of exception report entry records.
        /// </summary>
        /// <returns>An array of exception report entry records.
        /// </returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/20/09 jrf 2.20.05 127678 Created.
        // 
        public ExceptionReportEntryRecord[] ExceptionHostRecords
        {
            get
            {
                ReadUnloadedTable();

                return m_lstExceptionHostRecords.ToArray();
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the data out of the reader and into the member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/20/09 jrf 2.20.05 127678 Created.
        //  07/31/09 jrf 2.20.19 138950 cleared exception hosts before reading them.
        //
        protected void ParseData()
        {
            ExceptionReportEntryRecord ExceptionReportRecord = new ExceptionReportEntryRecord(m_EventDictionary);
            UInt16[] ausExceptionEvents = null;

            m_lstExceptionHostRecords.Clear();

            for (int iHost = 0; iHost < m_Table121.NumberOfExceptionHosts; iHost++)
            {
                ExceptionReportRecord.ApTitleNotify = m_Reader.ReadBytes(20);
                ExceptionReportRecord.MaxNbrRetries = m_Reader.ReadByte();
                ExceptionReportRecord.RetryDelay = m_Reader.ReadUInt16();
                ExceptionReportRecord.ExclusionPeriod = m_Reader.ReadUInt16();

                ausExceptionEvents = new ushort[m_Table121.NumberOfExceptionEvents];
                
                for (int iEvent = 0; iEvent < m_Table121.NumberOfExceptionEvents; iEvent++)
                {
                    ausExceptionEvents[iEvent] = m_Reader.ReadUInt16();
                }

                ExceptionReportRecord.RawEvents = ausExceptionEvents;

                m_lstExceptionHostRecords.Add(ExceptionReportRecord);

            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of table 123.
        /// </summary>
        /// <param name="Table121">Table 121 object for the current device.</param>
        /// <returns>The size of the table in bytes.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created

        public static uint GetTableSize(CStdTable121 Table121)
        {
            uint uiTableSize = 0;
            uint uiEntryLength = 0;

            // Get the size of each exception host
            uiEntryLength = (uint)(EXCEPTION_HOST_RCD_STATIC_SIZE + (Table121.NumberOfExceptionEvents * sizeof(UInt16)));
            // Get the size of all exception hosts
            uiTableSize += (uiEntryLength * Table121.NumberOfExceptionHosts);

            return uiTableSize;
        }

        #endregion

        #region Members

        private CStdTable121 m_Table121;
        private List<ExceptionReportEntryRecord> m_lstExceptionHostRecords;
        private ANSIEventDictionary m_EventDictionary;

        #endregion


    }

    /// <summary>
    /// Table 126 - Registration Status Table
    /// </summary>
    public class CStdTable126 : AnsiTable
    {
        #region Constants

        private const int TABLE126_FIXED_SIZE = 47;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="tbl121">Pointer to Table 121</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CStdTable126 Table126 = new CStdTable126( PSEM, Table121 ); 
        /// </code>
        /// </example>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/03/06 AF  7.40.00 N/A    Created
        // 
        public CStdTable126(CPSEM psem, CStdTable121 tbl121)
            : base(psem, 126, tbl121.NumberOfRegistrations * (TABLE126_FIXED_SIZE + (uint)(tbl121.NativeAddressLength)), TABLE_TIMEOUT)
        {
            m_tbl121 = tbl121;
            InitVariable();
        }

        /// <summary>
        /// Constructor used when reading from an EDL file.
        /// </summary>
        /// <param name="Binaryreader">Binary reader associated with the tables data array.</param>
        /// <param name="tbl121">Pointer to Table 121.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/25/08 jrf 1.50    N/A    Created
        // 
        public CStdTable126(PSEMBinaryReader Binaryreader, CStdTable121 tbl121)
            : base(126, tbl121.NumberOfRegistrations * (TABLE126_FIXED_SIZE + (uint)(tbl121.NativeAddressLength)))
        {
            m_tbl121 = tbl121;
            InitVariable();

            m_Reader = Binaryreader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Protected constructor used by CStdTable126_2008
        /// </summary>
        /// <param name="psem">PSEM Protocol</param>
        /// <param name="TableID">The Table ID - 126</param>
        /// <param name="Size">The calculated size of the table</param>
        /// <param name="tbl121">Pointer to Table 121</param>
        protected CStdTable126(CPSEM psem, ushort TableID, uint Size, CStdTable121 tbl121)
            : base(psem, TableID, Size, TABLE_TIMEOUT)
        {
            m_tbl121 = tbl121;
            InitVariable();
        }

        /// <summary>
        /// Protected Constructor used by CStdTable126_2008
        /// </summary>
        /// <param name="Binaryreader">Binary reader associated with the tables data array.</param>
        /// <param name="TableID">The Table ID - 126</param>
        /// <param name="Size">The calculated size of the table</param>
        /// <param name="tbl121">Pointer to Table 121</param>
        protected CStdTable126(PSEMBinaryReader Binaryreader, ushort TableID, uint Size, CStdTable121 tbl121)
            : base(TableID, Size)
        {
            m_tbl121 = tbl121;
            InitVariable();

            m_Reader = Binaryreader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads Standard table 126 out of the meter
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/06/06 AF  7.40.00 N/A    Created
        //  03/25/08 jrf 1.50    N/A    Refactored most of method into ParseData().
        // 
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CStdTable126.Read");

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Native address used to access the C12.22 Relay on this
        /// route for the C12.22 Node's local C12.22 Network Segment.
        /// </summary>
        /// <returns>Returns the Relay Native Address</returns>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// 		</exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/10/06 AF  7.40.00 N/A    Created
        // 
        public string RelayNativeAddress
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Relay Native Address"));
                    }
                }

                return m_strRelayNativeAddress;
            }
        }

        /// <summary>
        /// Relative or absolute object identifier assigned to the C12.22 Master
        /// Relay responsible for this C12.22 node.
        /// </summary>
        /// <returns>Returns the Master Relay Aptitle</returns>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// 		</exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/10/06 AF  7.40.00 N/A    Created
        // 
        public string MasterRelayAptitle
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Relay Native Address"));
                    }
                }

                return m_strMasterRelayAptitle;
            }
        }

        /// <summary>
        /// Relative or absolute object identifier assigned to this C12.22 node
        /// </summary>
        /// <returns>Returns the Node Aptitle</returns>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// 		</exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/10/06 AF  7.40.00 N/A    Created
        // 
        public string NodeAptitle
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Relay Native Address"));
                    }
                }

                return m_strNodeAptitle;
            }
        }

        /// <summary>
        /// Maximum random delay, in seconds, between each power up
        /// and the automatic issuance of the first Registration Service
        /// request by the C12.22 node.  This function is disabled when
        /// this field is set to zero.
        /// </summary>
        /// <returns>Returns the Registration Delay</returns>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// 		</exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/10/06 AF  7.40.00 N/A    Created
        // 
        public UInt16 RegistrationDelay
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Native Address"));
                    }
                }

                return m_usRegistrationDelay;
            }
        }
  
        /// <summary>
        /// Maximum duration, in minutes, before the C12.22 Node's registration
        /// expires.  The C12.22 Node needs to reregister itself before this 
        /// period lapses in order to remain registered.
        /// </summary>
        /// <returns>Returns the Registration Period</returns>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// 		</exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/10/06 AF  7.40.00 N/A    Created
        //  06/04/08 KRC 1.50.31        Changing to TimeSpan
        //
        public TimeSpan RegistrationPeriod
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Registration Period"));
                    }
                }

                return m_tsRegistrationPeriod;
            }
        }

        /// <summary>
        /// The amount of time in minutes left before the registration period
        /// expires.
        /// </summary>
        /// <returns>Returns the Registration Count Down</returns>
        /// <exception>
        /// Throws: PSEMException for communication errors.
        /// 		</exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/10/06 AF  7.40.00 N/A    Created
        //  06/04/08 KRC 1.50.31        Changing to TimeSpan
        //
        public TimeSpan RegistrationCountDown
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (State != TableState.Loaded)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                                "Error reading Native Address"));
                    }
                }

                return m_tsRegistrationCountDown;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Method to get data out of the Binary Reader and into member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/25/08 jrf 1.50    N/A    Created
        //  06/04/08 KRC 1.50.31        Make protected so we can override method in New 126
        protected virtual void ParseData()
        {
            byte[] abytTemp = new byte[m_tbl121.NativeAddressLength];
            byte[] byRawAptitle = new byte[20];

            // Read unused byte (Interface ID)
            abytTemp = m_Reader.ReadBytes(1);

            abytTemp = m_Reader.ReadBytes(m_tbl121.NativeAddressLength);
            m_strRelayNativeAddress = "";

            // The standard describes this field as a character array but it's being used
            // as a byte array
            for (int intIndex = 0; intIndex < m_tbl121.NativeAddressLength; intIndex++)
            {
                m_strRelayNativeAddress += abytTemp[intIndex].ToString(CultureInfo.InvariantCulture) + " ";
            }

            m_strRelayNativeAddress.Trim();

            byRawAptitle = m_Reader.ReadBytes(20);
            m_strMasterRelayAptitle = ESNConverter.Decode(byRawAptitle);

            byRawAptitle = m_Reader.ReadBytes(20);
            m_strNodeAptitle = ESNConverter.Decode(byRawAptitle);

            m_usRegistrationDelay = m_Reader.ReadUInt16();

            int iRegPeriodNumMinutes = m_Reader.ReadUInt16();
            m_tsRegistrationPeriod = new TimeSpan(0, iRegPeriodNumMinutes, 0);

            int iRegCountDownNumMinutes = m_Reader.ReadUInt16();
            m_tsRegistrationCountDown = new TimeSpan(0, iRegCountDownNumMinutes, 0);

        }

        #endregion

        #region Private Methods
        
        private void InitVariable()
        {
            m_strRelayNativeAddress = "";
            m_strMasterRelayAptitle = "";
            m_strNodeAptitle = "";
            m_usRegistrationDelay = 0;
        }

        #endregion

        #region Members

        /// <summary>Table 121 Object</summary>
        protected CStdTable121 m_tbl121;
        /// <summary>Relay Native Address</summary>
        protected string m_strRelayNativeAddress;
        /// <summary>Master Relay Aptitle</summary>
        protected string m_strMasterRelayAptitle;
        /// <summary>Node Aptitle</summary>
        protected string m_strNodeAptitle;
        /// <summary>Registration Delay</summary>
        protected UInt16 m_usRegistrationDelay;
        /// <summary>Registration Period</summary>
        protected TimeSpan m_tsRegistrationPeriod;
        /// <summary>Registration Countdown</summary>
        protected TimeSpan m_tsRegistrationCountDown;

        #endregion
    }

    /// <summary>
    /// Table 126 - This is the 126 Table as finally balloted in 2008.
    ///     This Version of the table is used in 1.50.40 and higher.
    /// </summary>
    public class CStdTable126_2008 : CStdTable126
    {
        #region Constants

        private const int TABLE126_2008_FIXED_SIZE = 49;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="tbl121">Pointer to Table 121</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CStdTable126_2008 Table126 = new CStdTable126( PSEM, Table121 ); 
        /// </code>
        /// </example>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/04/08 KRC 1.50.31        Implement with new Data Size
        //
        public CStdTable126_2008(CPSEM psem, CStdTable121 tbl121)
            : base(psem, 126, tbl121.NumberOfRegistrations * (TABLE126_2008_FIXED_SIZE + (uint)(tbl121.NativeAddressLength)), tbl121)
        {
        }

        /// <summary>
        /// Constructor used when reading from an EDL file.
        /// </summary>
        /// <param name="Binaryreader">Binary reader associated with the tables data array.</param>
        /// <param name="tbl121">Pointer to Table 121.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/04/08 KRC 1.50.31        Implement with new Data Size
        //
        public CStdTable126_2008(PSEMBinaryReader Binaryreader, CStdTable121 tbl121)
            : base(Binaryreader, 126, tbl121.NumberOfRegistrations * (TABLE126_2008_FIXED_SIZE + (uint)(tbl121.NativeAddressLength)), tbl121)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Method to get data out of the Binary Reader and into member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/04/08 KRC 1.50.31        Implement with new Data Size
        //
        protected override void ParseData()
        {
            byte[] abytTemp = new byte[m_tbl121.NativeAddressLength];
            byte[] byRawAptitle = new byte[20];

            // Read unused byte (Interface ID)
            abytTemp = m_Reader.ReadBytes(1);

            abytTemp = m_Reader.ReadBytes(m_tbl121.NativeAddressLength);
            m_strRelayNativeAddress = "";

            // The standard describes this field as a character array but it's being used
            // as a byte array
            for (int intIndex = 0; intIndex < m_tbl121.NativeAddressLength; intIndex++)
            {
                m_strRelayNativeAddress += abytTemp[intIndex].ToString(CultureInfo.InvariantCulture) + " ";
            }

            m_strRelayNativeAddress.Trim();

            byRawAptitle = m_Reader.ReadBytes(20);
            m_strMasterRelayAptitle = ESNConverter.Decode(byRawAptitle);

            byRawAptitle = m_Reader.ReadBytes(20);
            m_strNodeAptitle = ESNConverter.Decode(byRawAptitle);

            m_usRegistrationDelay = m_Reader.ReadUInt16();

            int iRegPeriodNumSeconds = (int)m_Reader.ReadUInt24();
            m_tsRegistrationPeriod = new TimeSpan(0, 0, iRegPeriodNumSeconds);

            int iRegCountDownNumSeconds = (int)m_Reader.ReadUInt24();
            m_tsRegistrationCountDown = new TimeSpan(0, 0, iRegCountDownNumSeconds);

        }

        #endregion
    }

    /// <summary>
    /// Table 127 - Network Statistics Table
    /// </summary>
    public class CStdTable127 : AnsiTable
    {
        #region Constants
        private const int TABLE_TIMEOUT = 5000;
        private const long CORE_DUMP_MASK = 0x0F;
        private const long NETWORK_STATE_MASK = 0x30;
        private const long BROADCAST_STATE_MASK = 0x40;
        private const long OUTAGE_ID_MASK = 0xFF00;
        private const long LEVEL_MASK = 0xFF;
        private const long LPD_MASK = 0xFF00;
        private const long GPD_MASK = 0xFFFF0000;
        private const long RXI_MASK = 0xFF00000000;
        private const long AVG_RSSI_MASK = 0xFF0000000000;
        private const short TYPE_MASK = 0x800;
        private const long TOWER_ID_MASK = 0x0000FFFF;
        private const long SECTOR_ID_MASK = 0xFFFF0000;
        private const long INVERSION_TAMPER_MASK = 0x00FF;
        private const long REMOVAL_TAMPER_MASK = 0xFF00;
        private const long PPP_ERROR_COUNT_MASK =  0x00000000FFFF;
        private const long IPV4_ERROR_COUNT_MASK = 0x0000FFFF0000;
        private const long TCP_ERROR_COUNT_MASK =  0xFFFF00000000;
        private const string RESOURCE_FILE_PROJECT_STRINGS =
                                    "Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings";

        private enum NetworkState
        {
            Not_Synchronzied = 0x00,
            Synching = 0x10,
            Synched = 0x20,
            Synch_Net = 0x30,
        }

        private enum BroadcastState
        {
            No_Active_Broadcast = 0x00,
            Broadcast_Received = 0x40,
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for this current session</param>
        /// <param name="tbl121">Pointer to Table 121</param>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CStdTable127 Table127 = new CStdTable127( PSEM, uiLenght ); 
        /// </code>
        /// </example>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 
        public CStdTable127(CPSEM psem, CStdTable121 tbl121)
            : base(psem, 127, (uint)(tbl121.NumberOfStatistics * 9), TABLE_TIMEOUT)
        {
            m_tbl121 = tbl121;
            m_colStatCollection = new List<CStatistic>();
            m_dicStatValues = new Dictionary<int, string>();
            m_dicStatDescription = new Dictionary<int, string>();

            //Get the resource manager
            m_StringRes = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                               this.GetType().Assembly);
            
            // Now that all of the Variables have been intialized, lets setup our dictionary.
            PopulateStatisticDescriptions();
        }

        /// <summary>
        /// Table 127 constructor used when reading from an EDL file.
        /// </summary>
        /// <param name="Binaryreader"></param>
        /// <param name="tbl121"></param>
        public CStdTable127(PSEMBinaryReader Binaryreader, CStdTable121 tbl121)
            : base(127, (uint)(tbl121.NumberOfStatistics * 9))
        {
            m_tbl121 = tbl121;
            m_colStatCollection = new List<CStatistic>();
            m_dicStatValues = new Dictionary<int, string>();
            m_dicStatDescription = new Dictionary<int, string>();

            //Get the resource manager
            m_StringRes = new ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                               this.GetType().Assembly);
            
            // Now that all of the Variables have been intialized, lets setup our dictionary.
            PopulateStatisticDescriptions();

            m_Reader = Binaryreader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads table 127 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/15/06 KRC 7.35.00 N/A    Created
        ///
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CStdTable127.Read");
            

            //Read the table			
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Returns the Statistic Collection
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/15/06 KRC 7.35.00 N/A    Created
        ///
        public List<CStatistic> GetStatistics()
        {
            if (State != TableState.Loaded)
            {
                // If the Table is set as Loaded, then it must be a file, else read it
                //  and set the property back to unloaded so we will read it again.
                if (PSEMResponse.Ok != Read())
                {
                    throw (new Exception("Table Could Not be Read"));
                }
                m_TableState = TableState.Unloaded;
            }

            return m_colStatCollection;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method to get data out of Binary Reader and into member variables
        /// </summary>
        private void ParseData()
        {
            char[] strTemp = new char[1];
            long lValue = 0;
            int intID;

            // Make sure our Statistic collection is clear
            m_colStatCollection.Clear();

            //Populate the member variable that represent the table
            for (int iCount = 0; iCount < m_tbl121.NumberOfStatistics; iCount++)
            {
                // Red unused byte (Interface ID)
                strTemp = m_Reader.ReadChars(1);
                // Read the Statistic ID
                intID = m_Reader.ReadInt16();
                lValue = m_Reader.ReadInt48();

                // Don't add ID 0 
                if (0 != intID)
                {
                    List<CStatistic> statList = BuildStatistics(intID, lValue);

                    foreach (CStatistic stat in statList)
                    {
                        m_colStatCollection.Add(stat);
                    }
                }
            }
        }

        /// <summary>
        /// Populates the Statistic Description Dictionary
        /// </summary>
        /// <returns>void</returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/24/06 KRC 7.35.00 N/A    Created
        /// 05/09/14 AF  3.50.91 WR504154 Added ICS Statistics
        ///
        private void PopulateStatisticDescriptions()
        {
            m_dicStatDescription.Add(0, m_StringRes.GetString("NO_STATISTIC"));
            m_dicStatDescription.Add(1, m_StringRes.GetString("NUM_BYTES_SENT"));
            m_dicStatDescription.Add(2, m_StringRes.GetString("NUM_BYTES_REC"));
            m_dicStatDescription.Add(3, m_StringRes.GetString("NUM_PDU_SENT"));
            m_dicStatDescription.Add(4, m_StringRes.GetString("NUM_PDU_REC"));
            m_dicStatDescription.Add(5, m_StringRes.GetString("NUM_PDU_FWD"));
            m_dicStatDescription.Add(6, m_StringRes.GetString("NUM_PDU_DPD"));
            m_dicStatDescription.Add(7, m_StringRes.GetString("NUM_TRANS_ERRS"));
            m_dicStatDescription.Add(8, m_StringRes.GetString("NUM_RECEP_ERRS"));
            m_dicStatDescription.Add(9, m_StringRes.GetString("NUM_COLLISIONS"));
            m_dicStatDescription.Add(10, m_StringRes.GetString("NUM_MSG_OVERRUNS"));
            m_dicStatDescription.Add(11, m_StringRes.GetString("NUM_FRAMING_ERRS"));
            m_dicStatDescription.Add(12, m_StringRes.GetString("NUM_CHECKSUM_ERRS"));
            m_dicStatDescription.Add(13, m_StringRes.GetString("NUM_ACTIVE_ASSOC"));
            m_dicStatDescription.Add(14, m_StringRes.GetString("NUM_ASSOC_TIMEOUTS"));
            m_dicStatDescription.Add(15, m_StringRes.GetString("NUM_SIGNAL_CARRIERS_LOST"));
            m_dicStatDescription.Add(16, m_StringRes.GetString("SIGNAL_STRENGTH_PERCENT"));
            m_dicStatDescription.Add(17, m_StringRes.GetString("SIGNAL_STRENGTH_DBM"));
            m_dicStatDescription.Add(18, m_StringRes.GetString("NUM_REG_SENT"));
            m_dicStatDescription.Add(19, m_StringRes.GetString("NUM_REG_REC"));
            m_dicStatDescription.Add(20, m_StringRes.GetString("NUM_REG_DENIED"));
            m_dicStatDescription.Add(21, m_StringRes.GetString("NUM_REG_FAILED"));
            m_dicStatDescription.Add(22, m_StringRes.GetString("NUM_DEREG_REQUESTED"));
            // MFG Statistics
            m_dicStatDescription.Add(2049, m_StringRes.GetString("IS_SYNCH_NOW"));
            m_dicStatDescription.Add(2050, m_StringRes.GetString("RESYNCH_COUNT"));
            m_dicStatDescription.Add(2051, m_StringRes.GetString("NUM_SUCCESSFUL_REG"));
            m_dicStatDescription.Add(2052, m_StringRes.GetString("RFLAN_CELL_ID"));
            m_dicStatDescription.Add(2053, m_StringRes.GetString("NUM_RFLAN_CELL_CHANGES"));
            m_dicStatDescription.Add(2054, m_StringRes.GetString("DEREGISTRATION_SUCCEEDED"));
            m_dicStatDescription.Add(2055, m_StringRes.GetString("RFLAN_CURRENT_LEVEL"));
            m_dicStatDescription.Add(2056, m_StringRes.GetString("NUMBER_RFLAN_LEVEL_CHANGES"));
            // These four all come from 2057
            m_dicStatDescription.Add(20571, m_StringRes.GetString("CORE_DUMP_STATE"));
            m_dicStatDescription.Add(20572, m_StringRes.GetString("NETWORK_STATE"));
            m_dicStatDescription.Add(20573, m_StringRes.GetString("BROADCAST_STATE"));
            m_dicStatDescription.Add(20574, m_StringRes.GetString("OUTAGE_ID"));
            m_dicStatDescription.Add(2058, m_StringRes.GetString("RFLAN_GPD"));
            m_dicStatDescription.Add(2059, m_StringRes.GetString("RFLAN_FATHER_COUNT"));
            m_dicStatDescription.Add(2060, m_StringRes.GetString("RFLAN_SYNCH_FATHER_MAC_ADDRESS"));
            // These four all come from 2061
            m_dicStatDescription.Add(20611, m_StringRes.GetString("SYNCH_FATHER_LEVEL"));
            m_dicStatDescription.Add(20612, m_StringRes.GetString("SYNCH_FATHER_LPD"));
            m_dicStatDescription.Add(20613, m_StringRes.GetString("SYNCH_FATHER_GPD"));
            m_dicStatDescription.Add(20614, m_StringRes.GetString("SYNCH_FATHER_RXI"));
            m_dicStatDescription.Add(20615, m_StringRes.GetString("SYNCH_FATHER_RSSI"));
            m_dicStatDescription.Add(2062, m_StringRes.GetString("RFLAN_BEST_FATHER_MAC_ADDRESS"));
            // These four all come from 2063
            m_dicStatDescription.Add(20631, m_StringRes.GetString("BEST_FATHER_LEVEL"));
            m_dicStatDescription.Add(20632, m_StringRes.GetString("BEST_FATHER_LPD"));
            m_dicStatDescription.Add(20633, m_StringRes.GetString("BEST_FATHER_GPD"));
            m_dicStatDescription.Add(20634, m_StringRes.GetString("BEST_FATHER_RXI"));
            m_dicStatDescription.Add(20635, m_StringRes.GetString("BEST_FATHER_RSSI"));
            m_dicStatDescription.Add(2064, m_StringRes.GetString("RFLAN_NET_DOWNLINK_PACKET_COUNT"));
            m_dicStatDescription.Add(2065, m_StringRes.GetString("RFLAN_NET_UPLINK_PACKET_COUNT"));
            m_dicStatDescription.Add(2066, m_StringRes.GetString("RFLAN_MAC_MONOCAST_RX_COUNT"));
            m_dicStatDescription.Add(2067, m_StringRes.GetString("RLAN_NET_ROUTING_COUNT"));
            // PLAN statistics
            m_dicStatDescription.Add(2080, m_StringRes.GetString("PLAN_LOCAL_MAC"));
            m_dicStatDescription.Add(2081, m_StringRes.GetString("PLAN_MASTER_MAC"));
            m_dicStatDescription.Add(2082, m_StringRes.GetString("PLAN_MASTER_SYS_TITLE"));
            m_dicStatDescription.Add(2083, m_StringRes.GetString("PLAN_IS_SYNC"));
            m_dicStatDescription.Add(2084, m_StringRes.GetString("PLAN_CELL_ID"));
            m_dicStatDescription.Add(2085, m_StringRes.GetString("PLAN_LOCAL_SYS_TITLE"));
            m_dicStatDescription.Add(2086, m_StringRes.GetString("PLAN_ELECTRICAL_STATUS"));
            m_dicStatDescription.Add(2087, m_StringRes.GetString("PLAN_CREDIT_LEVEL"));
            m_dicStatDescription.Add(2088, m_StringRes.GetString("PLAN_REPEATER_STATUS"));
            m_dicStatDescription.Add(2200, m_StringRes.GetString("ICS_REGISTRATION_STATUS"));
            m_dicStatDescription.Add(2202, m_StringRes.GetString("ICS_TOWER_AND_SECTOR_ID"));
            m_dicStatDescription.Add(2203, m_StringRes.GetString("ICS_NUMBER_OF_CELL_TOWER_CHANGES"));
            m_dicStatDescription.Add(2204, m_StringRes.GetString("ICS_CONNECTION_STATE"));
            m_dicStatDescription.Add(2205, m_StringRes.GetString("ICS_PACKETS_SENT"));
            m_dicStatDescription.Add(2206, m_StringRes.GetString("ICS_PACKETS_RECEIVED"));
            m_dicStatDescription.Add(2207, m_StringRes.GetString("ICS_CHECKSUM_ERRORS"));
            m_dicStatDescription.Add(2210, m_StringRes.GetString("ICS_OUTAGE_ID"));
            m_dicStatDescription.Add(2211, m_StringRes.GetString("ICS_TAMPERS"));
        }

        /// <summary>
        /// Builds a list of CStatistic objects
        /// </summary>
        /// <returns>void</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/20/07 KRC 8.10.15 3051    Supporting new statistics
        // 07/30/09 jrf 2.20.19 137693 Setting whether statistic is a standard
        //                             statistic.
        //
        private List<CStatistic> BuildStatistics(int intID, long lValue)
        {
            List<CStatistic> statList = new List<CStatistic>();

            string strValue = "";

            switch (intID)
            {
                case 2057:
                {
                    // Build Core Dump Statistic
                    CStatistic statCoreDump = new CStatistic(); 
                    statCoreDump.StatName = m_dicStatDescription[20571];
                    statCoreDump.StatValue = TranslateStatisticValue(20571, lValue);
                    statList.Add(statCoreDump);

                    // Build Network State Statistic
                    CStatistic statNetworkState = new CStatistic();
                    statNetworkState.StatName = m_dicStatDescription[20572];
                    statNetworkState.StatValue = TranslateStatisticValue(20572, lValue);
                    statList.Add(statNetworkState);

                    // Build Broadcast State Statistic
                    CStatistic statBroadcastState = new CStatistic();
                    statBroadcastState.StatName = m_dicStatDescription[20573];
                    statBroadcastState.StatValue = TranslateStatisticValue(20573, lValue);
                    statList.Add(statBroadcastState);

                    // Build Outage ID Statistic
                    CStatistic statOutageID = new CStatistic();
                    statOutageID.StatName = m_dicStatDescription[20574];
                    statOutageID.StatValue = TranslateStatisticValue(20574, lValue);
                    statList.Add(statOutageID);
                    break;
                }
                case 2061:  // RFLAN Synch Father Info
                {
                    // Build Level
                    CStatistic statLevel = new CStatistic();
                    statLevel.StatName = m_dicStatDescription[20611];
                    statLevel.StatValue = TranslateStatisticValue(20611, lValue);
                    statList.Add(statLevel);

                    // Build LPD
                    CStatistic statLPD = new CStatistic();
                    statLPD.StatName = m_dicStatDescription[20612];
                    statLPD.StatValue = TranslateStatisticValue(20612, lValue);
                    statList.Add(statLPD);

                    // Build GPD
                    CStatistic statGPD = new CStatistic();
                    statGPD.StatName = m_dicStatDescription[20613];
                    statGPD.StatValue = TranslateStatisticValue(20613, lValue);
                    statList.Add(statGPD);

                    // Build RXI
                    CStatistic statRXI = new CStatistic();
                    statRXI.StatName = m_dicStatDescription[20614];
                    statRXI.StatValue = TranslateStatisticValue(20614, lValue);
                    statList.Add(statRXI);

                    // Build RSSI
                    CStatistic statRSSI = new CStatistic();
                    statRSSI.StatName = m_dicStatDescription[20615];
                    statRSSI.StatValue = TranslateStatisticValue(20615, lValue);
                    statList.Add(statRSSI);
                    break;
                }
                case 2063:  // RFLAN Best Father Info
                {
                    // Build Level
                    CStatistic statLevel = new CStatistic();
                    statLevel.StatName = m_dicStatDescription[20631];
                    statLevel.StatValue = TranslateStatisticValue(20631, lValue);
                    statList.Add(statLevel);

                    // Build LPD
                    CStatistic statLPD = new CStatistic();
                    statLPD.StatName = m_dicStatDescription[20632];
                    statLPD.StatValue = TranslateStatisticValue(20632, lValue);
                    statList.Add(statLPD);

                    // Build GPD
                    CStatistic statGPD = new CStatistic();
                    statGPD.StatName = m_dicStatDescription[20633];
                    statGPD.StatValue = TranslateStatisticValue(20633, lValue);
                    statList.Add(statGPD);

                    // Build RXI
                    CStatistic statRXI = new CStatistic();
                    statRXI.StatName = m_dicStatDescription[20634];
                    statRXI.StatValue = TranslateStatisticValue(20634, lValue);
                    statList.Add(statRXI);

                    // Build RSSI
                    CStatistic statRSSI = new CStatistic();
                    statRSSI.StatName = m_dicStatDescription[20635];
                    statRSSI.StatValue = TranslateStatisticValue(20635, lValue);
                    statList.Add(statRSSI);
                    break;
                }
                default:
                {
                    CStatistic stat = new CStatistic();
                    strValue = TranslateStatisticValue(intID, lValue);

                    if (m_dicStatDescription.ContainsKey(intID))
                    {
                        stat.StatName = m_dicStatDescription[intID];

                        if (stat.StatName == null)
                        {
                            stat.StatName = "Null Description - Statistic: " + intID.ToString(CultureInfo.CurrentCulture);
                        }
                    }
                    else
                    {
                        stat.StatName = "Missing Description - Statistic: " + intID.ToString(CultureInfo.CurrentCulture);
                    }
                    stat.StatValue = strValue.ToString();

                    //Mark the statistic as standard if it is.
                    if (0 == (TYPE_MASK & intID))
                    {
                        stat.IsStandard = true;
                    }

                    statList.Add(stat);
                    break;
                }
            }
            return statList;
        }

        /// <summary>
        /// Translates numeric statisic data into something meaningful
        /// </summary>
        /// <returns>void</returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/60/07 KRC 8.01.05 N/A    Created
        /// 10/11/12 jrf 2.70.28 237120 Modified to interpret RSSI as a signed byte.
        /// 05/09/14 AF  3.50.91 WR504154 Added missing statistics translations for ICM statistics
        ///
        private string TranslateStatisticValue(int intID, long lValue)
        {
            string strValue = "";

            switch (intID)
            {
                case 2049:  // Is Synchronized Now
                case 2083:  // PLAN Is Synch
                {
                    // 0 == No; 1 == Yes;
                    if (0 == lValue)
                    {
                        strValue = m_StringRes.GetString("NO");
                    }
                    else
                    {
                        strValue = m_StringRes.GetString("YES");
                    }
                    break;
                }
                case 20571: // Core Dump state
                {
                    // This is just a value from 0-9.
                    lValue = lValue & CORE_DUMP_MASK;
                    strValue = lValue.ToString(CultureInfo.CurrentCulture);
                    break;
                }
                case 20572: // Network State
                {
                    // Network State: 0 = Not Synchronized, 1 = Synching, 2 = Synched, 3 = Synch Net
                    lValue = lValue & NETWORK_STATE_MASK;
                    if ((long)NetworkState.Not_Synchronzied == lValue)
                    {
                        strValue = m_StringRes.GetString("NOT_SYNCHRONIZED");
                    }
                    else if ((long)NetworkState.Synching == lValue)
                    {
                        strValue = m_StringRes.GetString("SYNCHING");
                    }
                    else if ((long)NetworkState.Synched == lValue)
                    {
                        strValue = m_StringRes.GetString("SYNCHED");
                    }
                    else
                    {
                        strValue = m_StringRes.GetString("SYNCH_NET");
                    }
                    break;
                }
                case 20573: // Broadcast State
                {
                    // Broadcast State: 0 = No active Broadcast, 1 = Broadcast Received
                    lValue = lValue & BROADCAST_STATE_MASK;
                    if ((long)BroadcastState.No_Active_Broadcast == lValue)
                    {
                        strValue = m_StringRes.GetString("NO_ACTIVE_BROADCAST");
                    }
                    else
                    {
                        strValue = m_StringRes.GetString("BROADCAST_RECEIVED");
                    }
                    break;
                }
                case 20574: //Outage ID
                {
                    lValue = (lValue & OUTAGE_ID_MASK) >> 8;
                    strValue = lValue.ToString(CultureInfo.CurrentCulture);
                    break;
                }
                case 20611: // Level
                case 20631:
                {
                    lValue = lValue & LEVEL_MASK;
                    strValue = lValue.ToString(CultureInfo.CurrentCulture);
                    break;
                }
                case 20612: // LPD
                case 20632:
                {
                    lValue = (lValue & LPD_MASK) >> 8;
                    strValue = lValue.ToString(CultureInfo.CurrentCulture);
                    break;
                }
                case 20613: // GPD
                case 20633:
                {
                    lValue = (lValue & GPD_MASK) >> 16;
                    strValue = lValue.ToString(CultureInfo.CurrentCulture);
                    break;
                }
                case 20614: // RXI   
                case 20634:
                {
                    lValue = (lValue & RXI_MASK) >> 32;
                    strValue = lValue.ToString(CultureInfo.CurrentCulture);
                    break;
                }
                case 20615: // RSSI
                case 20635:
                {
                    lValue = (lValue & AVG_RSSI_MASK) >> 40;

                    //Reinterpret RSSI value as a signed byte. 
                    SByte sbyValue = (sbyte)lValue;

                    strValue = sbyValue.ToString(CultureInfo.CurrentCulture);
                    break;
                }
                case 2060: // RFLAN MAC addresses
                case 2062:
                {
                    strValue = lValue.ToString("X8", CultureInfo.CurrentCulture);
                    break;
                }
                case 2080: // PLAN Local MAC
                case 2081: // PLAN MAster MAC
                {
                    strValue = lValue.ToString("X4", CultureInfo.CurrentCulture);
                    break;
                }
                case 2082: // PLAN Master System Title
                case 2085: // PLAN Local System Title
                {
                    // These values are really 6 byte arrays so when we read them they get reversed
                    // We need to put them back like they are supposed to be.
                    byte[] byaValue = BitConverter.GetBytes(lValue);

                    for (int iIndex = 0; iIndex < 6; iIndex++)
                    {
                        strValue += byaValue[iIndex].ToString("X2", CultureInfo.CurrentCulture);
                    }

                    break;
                }
                case 2086: // PLAN Electrical Status
                {
                    byte byValue = (byte)lValue;
                    strValue = EnumDescriptionRetriever.RetrieveDescription((PLANElectricalPhase)byValue);
                    break;
                }
                case 2088: // PLAN Repeater Status
                {
                    byte byValue = (byte)lValue;
                    strValue = EnumDescriptionRetriever.RetrieveDescription((PLANRepeaterModes)byValue);
                    break;
                }
                case 2202: // ICS Tower and Sector ID
                {
                    long lTemp = lValue;
                    lTemp = lValue & TOWER_ID_MASK;
                    strValue = m_StringRes.GetString("TOWER_ID") + ": " + lTemp.ToString(CultureInfo.CurrentCulture);

                    lTemp = (lValue & SECTOR_ID_MASK) >> 16;
                    strValue += " " + m_StringRes.GetString("SECTOR_ID") + ": " + lTemp.ToString(CultureInfo.CurrentCulture);
                    break;
                }
                case 2207:  // ICM Checksum Errors
                {
                    long lTemp = lValue;
                    lTemp = lValue & PPP_ERROR_COUNT_MASK;
                    strValue = m_StringRes.GetString("PPP_ERROR_COUNT") + ": " + lTemp.ToString(CultureInfo.CurrentCulture);

                    lTemp = (lValue & IPV4_ERROR_COUNT_MASK) >> 16;
                    strValue += " " + m_StringRes.GetString("IPV4_ERROR_COUNT") + ": " + lTemp.ToString(CultureInfo.CurrentCulture);

                    lTemp = (lValue & TCP_ERROR_COUNT_MASK) >> 32;
                    strValue += " " + m_StringRes.GetString("TCP_ERROR_COUNT") + ": " + lTemp.ToString(CultureInfo.CurrentCulture);
                    break;
                }
                case 2211:  // ICS Tampers
                {
                    long lTemp = lValue;
                    lTemp = lValue & INVERSION_TAMPER_MASK;
                    strValue = m_StringRes.GetString("NUMBER_INVERSION_TAMPERS") + ": " + lTemp.ToString(CultureInfo.CurrentCulture);

                    lTemp = (lValue & REMOVAL_TAMPER_MASK) >> 8;
                    strValue += " " + m_StringRes.GetString("NUMBER_REMOVAL_TAMPERS") + ": " + lTemp.ToString(CultureInfo.CurrentCulture);
                    break;
                }
                // Add any other translation items here...
                default:
                {
                    // For all other items we will just display the numeric value
                    strValue = lValue.ToString(CultureInfo.CurrentCulture);
                    break;
                }
            }

            return strValue;
        }

        #endregion

        #region Members

        private List<CStatistic> m_colStatCollection;
        private Dictionary<int, string> m_dicStatValues;
        private Dictionary<int, string> m_dicStatDescription;
        private CStdTable121 m_tbl121;
        private System.Resources.ResourceManager m_StringRes;

        #endregion
    }

    /// <summary>
    /// Exception report entry record.  Table 123 is an array of these records.
    /// </summary>
    public class ExceptionReportEntryRecord
    {
        #region Constants

        private const UInt16 EXCEPTION_REPORT_SELECTOR_MASK = 0x1000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        public ExceptionReportEntryRecord(ANSIEventDictionary eventDictionary)
        {
            m_abyApTitleNotify = null;
            m_byMaxNbrRetries = 0;
            m_usRetryDelay = 0;
            m_usExclusionPeriod = 0;
            m_ausEvents = null;
            m_lstEvents = null;
            m_EventDictionary = eventDictionary;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets an array of strings that represent the configured exception host events.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        public string[] Events
        {
            get
            {
                if (m_lstEvents == null)
                {
                    PopulateEvents();
                }

                return m_lstEvents.ToArray();
            }
        }

        /// <summary>
        /// Gets a string that represents the ApTitle for the exception host.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        public string ExceptionHostApTitle
        {
            get
            {
                return ESNConverter.Decode(m_abyApTitleNotify);
            }
        }

        #endregion


        #region Internal Properties

        /// <summary>
        /// Sets the raw ApTitle of the exception host.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        internal byte[] ApTitleNotify
        {
            set
            {
                m_abyApTitleNotify = value;
            }
        }

        /// <summary>
        /// Sets the maximum number of times that an exception will be repeated
        /// if an EPSEM &lt;ok&gt; response is not recieved. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        internal byte MaxNbrRetries
        {
            set
            {
                m_byMaxNbrRetries = value;
            }
        }

        /// <summary>
        /// Sets the minimum delay between retries of an event.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        internal UInt16 RetryDelay
        {
            set
            {
                m_usRetryDelay = value;
            }
        }

        /// <summary>
        /// Sets the lockout period after an event occurs before an event of the same 
        /// type can generate a new exception.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        internal UInt16 ExclusionPeriod
        {
            set
            {
                m_usExclusionPeriod = value;
            }
        }

        /// <summary>
        /// Sets the raw exception event ids.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        internal UInt16[] RawEvents
        {
            set
            {
                m_ausEvents = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method populates the list of events with meaningful names.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //  04/28/11 AF  2.50.36 172845 Added code to mask off the Selector bit.  It
        //                              shouldn't be set but we should ignore it if it is.
        //
        private void PopulateEvents()
        {
            m_lstEvents = new List<string>();

            foreach (UInt16 usEvent in m_ausEvents)
            {
                string strEvent = "";

                if (usEvent != 0)
                {
                    if (m_EventDictionary.TryGetValue((int)usEvent, out strEvent) == false)
                    {
                        UInt16 usTempEvent = usEvent;
                        if ((usTempEvent & EXCEPTION_REPORT_SELECTOR_MASK) == EXCEPTION_REPORT_SELECTOR_MASK)
                        {
                            usTempEvent -= EXCEPTION_REPORT_SELECTOR_MASK;
                        }

                        if (m_EventDictionary.TryGetValue((int)usTempEvent, out strEvent) == false)
                        {
                            // The TryGetValue failed so say it is an unknown event.
                            strEvent = "Unknown Event " + usEvent.ToString(CultureInfo.InvariantCulture);
                        }
                    }

                    m_lstEvents.Add(strEvent);
                }
            }
        }

        #endregion

        #region Members

        private byte[] m_abyApTitleNotify;
        private byte m_byMaxNbrRetries;
        private UInt16 m_usRetryDelay;
        private UInt16 m_usExclusionPeriod;
        private UInt16[] m_ausEvents;
        private List<string> m_lstEvents;
        private ANSIEventDictionary m_EventDictionary;

        #endregion

    }

    /// <summary>
    /// Interface Control Record - Contains the configuration of the network interface.
    /// </summary>
    internal class InterfaceControlEntryRecord
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        public InterfaceControlEntryRecord()
        {
            m_strNativeAddress = "";
            m_strBroadcastAddress = "";
            m_strRelayNativeAddress = "";
            m_abyApTitle = null;
            m_abyMasterRelayApTitle = null;
            m_byNbrRetries = 0;
            m_usResponseTimeout = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets/Sets a string that represents the native address.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        public string NativeAddress
        {
            get
            {
                return m_strNativeAddress;
            }
            set
            {
                m_strNativeAddress = value;
            }
        }

        /// <summary>
        /// Gets/Sets a string that represents the broadcast address.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        public string BroadcastAddress
        {
            get
            {
                return m_strBroadcastAddress;
            }
            set
            {
                m_strBroadcastAddress = value;
            }
        }

        /// <summary>
        /// Gets/Sets a string that represents the relay native address.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        public string RelayNativeAddress
        {
            get
            {
                return m_strRelayNativeAddress;
            }
            set
            {
                m_strRelayNativeAddress = value;
            }
        }

        /// <summary>
        /// Gets/Sets a byte array that represents the ApTitle.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        public byte[] ApTitle
        {
            get
            {
                return m_abyApTitle;
            }
            set
            {
                m_abyApTitle = value;
            }
        }

        /// <summary>
        /// Gets/Sets a byte array that represents the master relay ApTitle.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        public byte[] MasterRelayApTitle
        {
            get
            {
                return m_abyMasterRelayApTitle;
            }
            set
            {
                m_abyMasterRelayApTitle = value;
            }
        }

        /// <summary>
        /// Gets/Sets a byte that represents the number of retries allowed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        public byte NumberRetries
        {
            get
            {
                return m_byNbrRetries;
            }
            set
            {
                m_byNbrRetries = value;
            }
        }

        /// <summary>
        /// Gets/Sets a UInt16 that represents the response timeout.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/21/09 jrf 2.20.05 127678 Created.
        //
        public UInt16 ResponseTimeout
        {
            get
            {
                return m_usResponseTimeout;
            }
            set
            {
                m_usResponseTimeout = value;
            }
        }

        #endregion

        #region Members

        private string m_strNativeAddress;
        private string m_strBroadcastAddress;
        private string m_strRelayNativeAddress;
        private byte[] m_abyApTitle;
        private byte[] m_abyMasterRelayApTitle;
        private byte m_byNbrRetries;
        private UInt16 m_usResponseTimeout;

        #endregion

    }
}
