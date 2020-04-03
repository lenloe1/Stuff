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
//                           Copyright © 2011 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Comm Module object for a Cisco Comm Module
    /// </summary>
    public class CiscoCommModule : CommModuleBase
    {
        #region Constants

        /// <summary>
        /// Length of the PPP key
        /// </summary>
        public const int PPP_KEY_LENGTH = 32;

        private const int MIN_VALID_COMMENCE_TIMER = -1;

        private const string HARDWARE_DESCRIPTION_TLV = "11";
        private const byte MAC_ADDRESS_FIELD_NUMBER = 11;
        private const int FROM_BASE_SIXTEEN = 16;

        private const string NEIGHBOR_802154G_TLV = "52";
        private const byte NEIGHBOR_INDEX_FIELD_NUMBER = 1;
        private const byte NEIGHBOR_MAC_ADDRESS_FIELD_NUMBER = 2;
        private const byte NEIGHBOR_LAST_CHANGED_FIELD_NUMBER = 3;
        private const byte NEIGHBOR_RSSI_FORWARD_FIELD_NUMBER = 4;
        private const byte NEIGHBOR_RSSI_REVERSE_FIELD_NUMBER = 5;

        private const string INTERFACE_SETTINGS_TLV = "19";
        private const byte INTERFACE_SETTINGS_OPT_OUT_FIELD_NUMBER = 2;
        private const uint OPT_OUT_UP = 1;
        private const uint OPT_OUT_DOWN = 2;


        #endregion

        #region Definitions

        /// <summary>
        /// Wire type provides the information needed to determine the length of the field in a TLV
        /// </summary>
        public enum WireType
        {
            /// <summary>
            /// Used for int32, int64, uint32, uint64, sint32, sint64, bool, enum
            /// </summary>
            VARINT = 0,
            /// <summary>
            /// Used for fixed64, sfixed64, double
            /// </summary>
            I64_BIT = 1,
            /// <summary>
            /// Used for string, bytes, embedded messages, packed repeated fields
            /// </summary>
            LENGTH_DELIMITED = 2,
            /// <summary>
            /// groups (deprecated)
            /// </summary>
            START_GROUP = 3,
            /// <summary>
            /// groups (deprecated)
            /// </summary>
            END_GROUP = 4,
            /// <summary>
            /// Used for fixed32, sfixed32, float
            /// </summary>
            I32_BIT = 5,
        }

        /// <summary>
        /// Link Status provides the IPv6 stack state
        /// </summary>
        public enum IPv6StackState
        {
            /// <summary>
            /// 
            /// </summary>
            IPV6_STACK_NULL = 0,
            /// <summary>
            /// 
            /// </summary>
            IPV6_STACK_INIT = 1,
            /// <summary>
            /// 
            /// </summary>
            IPV6_STACK_STARTED = 2,
            /// <summary>
            /// 
            /// </summary>
            IPV6_STACK_NEGOTIATING = 3,
            /// <summary>
            /// 
            /// </summary>
            IPV6_STACK_UP = 4,
            /// <summary>
            /// 
            /// </summary>
            IPV6_STACK_BAD = 64,
            /// <summary>
            /// 
            /// </summary>
            IPV6_STACK_STOPPED = 65,
            /// <summary>
            /// 
            /// </summary>
            IPV6_STACK_FAILED = 66,
            /// <summary>
            /// 
            /// </summary>
            IPV6_STACK_GOOD = 128,
            /// <summary>
            /// 
            /// </summary>
            IPV6_STACK_READY = 129,
            /// <summary>
            /// 
            /// </summary>
            IPV6_STACK_RUNNING = 130,
        }

        /// <summary>
        /// Current time source id
        /// </summary>
        public enum IPv6SourceID : uint
        {
            /// <summary>
            /// No external sync
            /// </summary>
            NOT_SYNC = 0,
            /// <summary>
            /// Local sync (N/A for itron cg-mesh)
            /// </summary>
            LOCAL_SYNC = 1,
            /// <summary>
            /// Adminstrative sync (i.e. register sets time using CurrentTime TLV)
            /// </summary>
            REGISTER_SYNC = 2,
            /// <summary>
            /// Net (synced to network time)
            /// </summary>
            NET_SYNC = 3,
            /// <summary>
            /// Not read (or not available)
            /// </summary>
            NOT_READ = 0xFFFFFFFF
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem"></param>
        /// <param name="amiDevice"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/10/11 AF  2.53.00        Created
        //  01/17/12 AF  2.53.31 183921 Added initialization of the Table 2580 object.
        //  11/14/13 AF  3.50.03	    Class re-architecture - replace CENTRON_AMI parameter 
        //                              with CANSIDevice
        //
        public CiscoCommModule(CPSEM psem, CANSIDevice amiDevice)
            : base(psem, amiDevice)
        {
            m_Table2580 = null;
            m_AMIDevice = amiDevice;
        }

        /// <summary>
        /// This procedure enables or disables access to the Boron tables for Boron 5.0 meters.
        /// This method is obsolete but must be kept so long as there are Boron 5.0 meters in the field.
        /// </summary>
        /// <param name="Activate">true means to activate the tables; false means to disable</param>
        /// <returns>PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/02/12 AF  2.53.37 TREQ4721, 4722 Created
        //  12/12/12 AF  2.70.50 263052  Renamed to make it more clear which procedure this uses
        //
        public ProcedureResultCodes SetDiagnosticsTablesActivationProc150(bool Activate)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            MemoryStream ParameterStream = new MemoryStream();
            PSEMBinaryWriter BinaryWriter = new PSEMBinaryWriter(ParameterStream);
            byte[] ProcResponse;
            byte[] byParameter;

            BinaryWriter.Write((byte)IPDiagnosticsFunction150ID.SET_DIAGNOSTICS_TABLES_ACTIVATION);

            if (Activate)
            {
                BinaryWriter.Write(1);
            }
            else
            {
                BinaryWriter.Write(0);
            }
            byParameter = ParameterStream.ToArray();

            ProcResult = m_AMIDevice.ExecuteProcedureForBoron((ushort)Procedures.IP_DIAGNOSTICS, byParameter, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// This procedure enables or disables access to the Boron tables.  This method replaces 
        /// SetDiagnosticsTablesActivationProc150() but can only be used on register firmware 5.2 or later.
        /// </summary>
        /// <param name="Activate">true means to activate the tables; false means to disable</param>
        /// <returns>PSEMResponse encapsulating the layer 7 response
        /// to the layer 7 request</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/30/12 AF  2.70.09 TC9229 Created
        //  12/12/12 AF  2.70.50 263052  Renamed to make it more clear which procedure this uses
        //
        public ProcedureResultCodes SetDiagnosticsTablesActivationProc159(bool Activate)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            MemoryStream ParameterStream = new MemoryStream();
            PSEMBinaryWriter BinaryWriter = new PSEMBinaryWriter(ParameterStream);
            byte[] ProcResponse;
            byte[] byParameter;

            BinaryWriter.Write((byte)IPDiagnosticsFunction159ID.SET_DIAGNOSTICS_TABLES_ACTIVATION);

            if (Activate)
            {
                BinaryWriter.Write(1);
            }
            else
            {
                BinaryWriter.Write(0);
            }
            byParameter = ParameterStream.ToArray();

            ProcResult = m_AMIDevice.ExecuteProcedureForBoron((ushort)Procedures.IP_DIAGNOSTICS_159, byParameter, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Causes the register to request a specific TLV from the comm module
        /// </summary>
        /// <param name="TlvId"></param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/28/12 AF  2.60.38 TREQ6552 Created
        //
        public ProcedureResultCodes RequestOnDemandTLV(string TlvId)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            MemoryStream ParameterStream = new MemoryStream();
            PSEMBinaryWriter BinaryWriter = new PSEMBinaryWriter(ParameterStream);
            byte[] ProcResponse;
            byte[] byParameter;

            // Write the function id
            BinaryWriter.Write((byte)IPDiagnosticsFunction150ID.SEND_RAW_CSMP);
            // followed by the length of the CSMP message
            BinaryWriter.Write((UInt32)(7 + TlvId.Length));
            // Construct the CSMP packet to send
            // 42 01 00 01 91 63 common to all (using msg id = 0001 for all)
            BinaryWriter.WriteUInt48(0x639101000142);
            // followed by length of the TlvId string in bytes
            BinaryWriter.Write((byte)TlvId.Length);
            // followed by the TlvId as ascii codes
            BinaryWriter.Write(TlvId, TlvId.Length);

            // Call the procedure to read the TLV from the comm module
            byParameter = ParameterStream.ToArray();

            ProcResult = m_AMIDevice.ExecuteProcedureForBoron((ushort)Procedures.IP_DIAGNOSTICS, byParameter, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Sets the PPP key to the register.  This must be followed by SendPPPKey() to make the key active
        /// </summary>
        /// <param name="PPPKey">The the PPP key in the form of a 32-byte array</param>
        /// <returns>the result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/09/13 AF  2.70.56        Created
        //
        public ProcedureResultCodes SetPPPKey(byte[] PPPKey)
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            if (PPPKey.Length == PPP_KEY_LENGTH)
            {
                // calculate the CRC on the key
                ushort usCRC = CRC.CalculateCRC(PPPKey);
                
                // call the procedure
                byte[] ProcResponse;
                byte[] byParameter = new byte[34];
                Array.Copy(PPPKey, 0, byParameter, 0, PPPKey.Length);
                Array.Copy(BitConverter.GetBytes(usCRC), 0, byParameter, PPP_KEY_LENGTH, sizeof(ushort));
                ProcResult = m_AMIDevice.ExecuteProcedureForBoron((ushort)Procedures.SET_PPP_KEY, byParameter, out ProcResponse);
            }

            return ProcResult;
        }

        /// <summary>
        /// Clears the existing PPP key from the register
        /// </summary>
        /// <returns>the result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/09/13 AF  2.70.56        Created
        //
        public ProcedureResultCodes ClearPPPKey()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            byte[] ProcResponse;
            byte[] byParameter = new byte[0];

            ProcResult = m_AMIDevice.ExecuteProcedureForBoron((ushort)Procedures.CLEAR_PPP_KEY, byParameter, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Sends the PPP key to the register.  This must be called after SetPPPKey()
        /// </summary>
        /// <returns>the result of the procedure call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/09/13 AF  2.70.56        Created
        //
        public ProcedureResultCodes SendPPPKey()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            byte[] ProcResponse;
            byte[] byParameter = new byte[0];

            ProcResult = m_AMIDevice.ExecuteProcedureForBoron((ushort)Procedures.SEND_PPP_KEY, byParameter, out ProcResponse);

            return ProcResult;
        }

        /// <summary>
        /// Reads the PSK Resident field from table 511 to determine if PPP security is currently in use in the meter
        /// </summary>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/09/13 AF  2.70.56        Created
        //
        public bool IsPSKResident()
        {
            return Table2559.PSKResident;
        }

        /// <summary>
        /// This procedure will reset the stack from IP to C12.22 and back to IP.
        /// It can be used to give the IP stack a kick to get it unstuck.
        /// </summary>
        /// <returns>The result of the procedures call</returns>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/13/14 AF  3.70.01 WR 529116 Created
        //
        public ProcedureResultCodes ResetIPStack()
        {
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;

            MemoryStream ParameterStream = new MemoryStream();
            PSEMBinaryWriter BinaryWriter = new PSEMBinaryWriter(ParameterStream);
            byte[] ProcResponse;
            byte[] byParameter;

            BinaryWriter.Write((byte)IPDiagnosticsFunction159ID.SET_STACK_TYPE);

            // Set the stack type to C12.22
            BinaryWriter.Write(0);

            byParameter = ParameterStream.ToArray();

            ProcResult = m_AMIDevice.ExecuteProcedureForBoron((ushort)Procedures.IP_DIAGNOSTICS_159, byParameter, out ProcResponse);

            if (ProcResult == ProcedureResultCodes.COMPLETED)
            {
                // Give it a moment to finish
                Thread.Sleep(1000);

                ParameterStream = new MemoryStream();
                BinaryWriter = new PSEMBinaryWriter(ParameterStream);
                BinaryWriter.Write((byte)IPDiagnosticsFunction159ID.SET_STACK_TYPE);

                // Set the stack type back to IP
                BinaryWriter.Write(1);

                byParameter = ParameterStream.ToArray();

                ProcResult = m_AMIDevice.ExecuteProcedureForBoron((ushort)Procedures.IP_DIAGNOSTICS_159, byParameter, out ProcResponse);
            }

            return ProcResult;
        }

        /// <summary>
        /// Translates the source id into a human understandable string
        /// </summary>
        /// <param name="sourceID">The source id field from mfg table 549</param>
        /// <returns>a string representing the value of the source id</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  09/21/15 AF  4.21.04  WR 616166  Created
        //
        public string TranslateSourceId(UInt32 sourceID)
        {
            string lastSyncSource = "Not Read";

            switch ((IPv6SourceID)sourceID)
            {
                case IPv6SourceID.NOT_SYNC:
                {
                    lastSyncSource = "Not Synced";
                    break;
                }
                case IPv6SourceID.LOCAL_SYNC:
                {
                    lastSyncSource = "Local Sync";
                    break;
                }
                case IPv6SourceID.REGISTER_SYNC:
                {
                    lastSyncSource = "Administrative Synced";
                    break;
                }
                case IPv6SourceID.NET_SYNC:
                {
                    lastSyncSource = "Net Synced";
                    break;
                }
                default:
                {
                    lastSyncSource = "Not Read";
                    break;
                }
            }

            return lastSyncSource;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the register firmware type from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public byte IPMeterRegFWType
        {
            get
            {
                byte byType = 0;

                if (Table2580 != null)
                {
                    byType = Table2580.RegFWVersionInfo.Type;
                }

                return byType;
            }
        }

        /// <summary>
        /// Gets the register firmware version/revision from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public string IPMeterRegFWVersionRevision
        {
            get
            {
                string strVersionInfo = "";

                if (Table2580 != null)
                {
                    strVersionInfo = Table2580.RegFWVersionInfo.Version.ToString(CultureInfo.InvariantCulture) + "." 
                                + Table2580.RegFWVersionInfo.Revision.ToString("d3", CultureInfo.CurrentCulture);
                }

                return strVersionInfo;
            }
        }

        /// <summary>
        /// Gets the register firmware build from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        
        public byte IPMeterRegFWBuild
        {
            get
            {
                byte byBuild = 0;

                if (Table2580 != null)
                {
                    byBuild = Table2580.RegFWVersionInfo.Build;
                }

                return byBuild;
            }
        }

        /// <summary>
        /// Gets the register bootloader firmware type from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public byte IPMeterRegBootloaderType
        {
            get
            {
                byte byType = 0;

                if (Table2580 != null)
                {
                    byType = Table2580.RegBootloaderVersionInfo.Type;
                }

                return byType;
            }
        }

        /// <summary>
        /// Gets the register bootloader version from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public byte IPMeterRegBootloaderVersion
        {
            get
            {
                byte byVer = 0;

                if (Table2580 != null)
                {
                    byVer = Table2580.RegBootloaderVersionInfo.Version;
                }

                return byVer;
            }
        }

        /// <summary>
        /// Gets the register bootloader revision from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public byte IPMeterRegBootloaderRevision
        {
            get
            {
                byte byRevision = 0;

                if (Table2580 != null)
                {
                    byRevision = Table2580.RegBootloaderVersionInfo.Revision;
                }

                return byRevision;
            }
        }

        /// <summary>
        /// Gets the register bootloader build from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public byte IPMeterRegBootloaderBuild
        {
            get
            {
                byte byBuild = 0;

                if (Table2580 != null)
                {
                    byBuild = Table2580.RegBootloaderVersionInfo.Build;
                }

                return byBuild;
            }
        }

        /// <summary>
        /// Gets the comm module firmware type from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public byte IPMeterModuleFWType
        {
            get
            {
                byte byType = 0;

                if (Table2580 != null)
                {
                    byType = Table2580.ModuleFWVersionInfo.Type;
                }

                return byType;
            }
        }

        /// <summary>
        /// Gets the comm module firmware version from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public byte IPMeterModuleFWVersion
        {
            get
            {
                byte byVer = 0;

                if (Table2580 != null)
                {
                    byVer = Table2580.ModuleFWVersionInfo.Version;
                }

                return byVer;
            }
        }

        /// <summary>
        /// Gets the comm module firmware revision
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public byte IPMeterModuleFWRevision
        {
            get
            {
                byte byRevision = 0;

                if (Table2580 != null)
                {
                    byRevision = Table2580.ModuleFWVersionInfo.Revision;
                }

                return byRevision;
            }
        }

        /// <summary>
        /// Gets the comm module firmware build from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public byte IPMeterModuleFWBuild
        {
            get
            {
                byte byBuild = 0;

                if (Table2580 != null)
                {
                    byBuild = Table2580.ModuleFWVersionInfo.Build;
                }

                return byBuild;
            }
        }

        /// <summary>
        /// Gets the comm module bootloader firmware type from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public byte IPMeterModuleBootloaderType
        {
            get
            {
                byte byType = 0;

                if (Table2580 != null)
                {
                    byType = Table2580.ModuleBootloaderVersionInfo.Type;
                }

                return byType;
            }
        }

        /// <summary>
        /// Gets the comm module bootloader firmware version from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public byte IPMeterModuleBootloaderVersion
        {
            get
            {
                byte byVer = 0;

                if (Table2580 != null)
                {
                    byVer = Table2580.ModuleBootloaderVersionInfo.Version;
                }

                return byVer;
            }
        }

        /// <summary>
        /// Gets the comm module bootloader firmware revision from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public byte IPMeterModuleBootloaderRevision
        {
            get
            {
                byte byRevision = 0;

                if (Table2580 != null)
                {
                    byRevision = Table2580.ModuleBootloaderVersionInfo.Revision;
                }

                return byRevision;
            }
        }

        /// <summary>
        /// Gets the comm module bootloader firmware build from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/11/12 AF  2.70.28        Created
        //
        public byte IPMeterModuleBootloaderBuild
        {
            get
            {
                byte byBuild = 0;

                if (Table2580 != null)
                {
                    byBuild = Table2580.ModuleBootloaderVersionInfo.Build;
                }

                return byBuild;
            }
        }

        /// <summary>
        /// Gets the comm module MAC address from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 01/17/12 AF  2.53.31 183921  Created
        // 05/30/12 AF  2.60.27 196801  Added a catch for a timeout exception.  We can't
        //                              just ignore it if we've lost communication with the meter.
        // 03/16/15 AF  4.10.07 571287  Get the MAC address from a TLV request instead of reading from a table
        //
        public ulong CommModuleMACAddress
        {
            get
            {
                ulong ulMAC = 0;
                ProcedureResultCodes Result = ProcedureResultCodes.COMPLETED;
                CENTRON_AMI AMIDevice = m_AMIDevice as CENTRON_AMI;

                try
                {
                    if (AMIDevice != null)
                    {
                        if (!AMIDevice.IsTableUsed(2612))
                        {
                            // We can't see the table, call the procedure to make it visible
                            // This table will only be hidden for 5.0.x meters so mfg proc 150 is all we need
                            Result = SetDiagnosticsTablesActivationProc150(true);
                        }

                        if (Result == ProcedureResultCodes.COMPLETED)
                        {
                            Result = RequestOnDemandTLV(HARDWARE_DESCRIPTION_TLV);
                            byte[] abyData = null;

                            if (Result == ProcedureResultCodes.COMPLETED)
                            {
                                abyData = AMIDevice.TLVIdRequestedData;
                                if (abyData != null)
                                {
                                    List<TLVData> TLVList = new List<TLVData>();
                                    TLV.ParseTLVData(abyData, ref TLVList);

                                    if (TLVList.Count > 0)
                                    {
                                        foreach (TLVOptionsData option in TLVList[0].Options)
                                        {
                                            if (option.FieldNumber == MAC_ADDRESS_FIELD_NUMBER)
                                            {
                                                string strFormated = string.Empty;
                                                UTF8Encoding enc = new UTF8Encoding();
                                                strFormated = enc.GetString(option.FieldValue);
                                                ulMAC = Convert.ToUInt64(strFormated, FROM_BASE_SIXTEEN);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Do nothing. Just return 0.
                        }
                    }
                }
                catch
                {
                    // Do nothing. Just return 0.
                }

                return ulMAC;
            }
        }

        /// <summary>
        /// Gets the Comm module PAN id from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/02/12 AF  2.53.37 TREQ4720 Created
        //  07/12/12 AF  2.60.44 200945   If we get an exception, return null rather than
        //                                0, so that we can tell that it's not a valid value
        //  02/01/13 AF  2.70.62 288194   PAN ID byte order was corrected in Carbon at build 35.
        //                                Tweak the byte order here if needed.
        //  08/15/14 jrf 3.70.03 529314 Modified reverse array to use the correct logic for detecting 
        //                              the desired firmware versions.
        //  08/18/15 AF  4.21.01 606516 The IPv6 M2 Gateway doesn't match the fw version check but needs the
        //                              PAN id byte order reversed
        //
        public UInt32? CommModulePANId
        {
            get
            {
                UInt32? uiPANId = null;
                byte[] byaPANId = null;
                M2_Gateway M2Gateway = m_AMIDevice as M2_Gateway;

                if (Table2580 != null)
                {
                    try
                    {
                        byaPANId = new byte[4];

                        Array.Copy(Table2580.ModulePANId, byaPANId, Table2580.ModulePANId.Length);

                        if ((VersionChecker.CompareTo(m_AMIDevice.FWRevision, CENTRON_AMI.VERSION_5_5_CARBON) > 0 ||
                            ((VersionChecker.CompareTo(m_AMIDevice.FWRevision, CENTRON_AMI.VERSION_5_5_CARBON) == 0)
                            && (VersionChecker.CompareTo(m_AMIDevice.FirmwareBuild, 35) >= 0))) || (M2Gateway != null))
                        {
                            Array.Reverse(byaPANId);
                        }

                        uiPANId = BitConverter.ToUInt32(byaPANId, 0);
                    }
                    catch (Exception)
                    {
                        // Do nothing.  Just return null
                    }
                }

                return uiPANId;
            }
        }

        /// <summary>
        /// Gets the Comm module JTAG security status from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/26/12 jrf 2.70.24 235138 Created
        //
        public bool? CommModuleJTAGSecurityEnabled
        {
            get
            {
                bool? blnEnabled = null;

                if (Table2580 != null)
                {
                    try
                    {
                        blnEnabled = Table2580.ModuleJTAGSecurityEnabled;
                    }
                    catch (Exception)
                    {
                        // Do nothing.  Just return null
                    }
                }

                return blnEnabled;
            }
        }

        /// <summary>
        /// Gets the Comm module link status from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/26/12 jrf 2.70.24 235138 Created
        //  10/03/12 jkw 2.70.xx        Renamed to include 'Byte' in the name to distinguish
        //                              between this one used in the factoru QC tool and the 
        //                              enum
        //
        public byte? LinkStatusByte
        {
            get
            {
                byte? byStatus = null;

                if (Table2580 != null)
                {
                    try
                    {
                        byStatus = Table2580.ModuleLinkStatus;
                    }
                    catch (Exception)
                    {
                        // Do nothing.  Just return null
                    }
                }

                return byStatus;
            }
        }

        /// <summary>
        /// Gets the Comm module link status from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 10/03/12 JKW 2.70.xx          Created
        public IPv6StackState? LinkStatus
        {
            get
            {
                IPv6StackState? byLinkStatus = null;

                if (Table2580 != null)
                {
                    try
                    {
                        byLinkStatus = (IPv6StackState)Table2580.ModuleLinkStatus;
                    }
                    catch (Exception)
                    {
                        // Do nothing.  Just return null
                    }
                }

                return byLinkStatus;
            }
        }

        /// <summary>
        /// Gets the neighbor MAC address from table 2580
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/26/12 jrf 2.70.24 235138 Created
        //
        public ulong NeighborMACAddress
        {
            get
            {
                ulong ulMAC = 0;

                if (Table2580 != null)
                {
                    try
                    {
                        ulMAC = Table2580.NeighborMACAddress;
                    }
                    catch (Itron.Metering.Communications.TimeOutException e)
                    {
                        throw (e);
                    }
                    catch (Exception)
                    {
                        // Do nothing - just display 0
                    }
                }

                return ulMAC;
            }
        }

        /// <summary>
        /// Reads the SSID from Mfg table 532. Warning! Only available for
        /// firmware versions 5.2.33 and beyond.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/12 AF  2.70.28 TC11217 Created
        //
        public string SSId
        {
            get
            {
                string strSSId = "";

                if (Table2580 != null)
                {
                    try
                    {
                        strSSId = Table2580.SSID;
                    }
                    catch (Itron.Metering.Communications.TimeOutException e)
                    {
                        throw (e);
                    }
                    catch (Exception)
                    {
                        // Do nothing - just display an empty string
                    }
                }

                return strSSId;
            }
        }


        /// <summary>
        /// Reads the long version of the IP Address from Mfg table 501
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/26/12 AF  2.70.34 TC11708 Created
        //
        public string CommModuleIPv6AddressLong
        {
            get
            {
                string strIPAddress = "";

                if (Table2549 != null)
                {
                    try
                    {
                        strIPAddress = Table2549.IPv6AddressLong;
                    }
                    catch (Exception)
                    {
                        // Do nothing. Ok if the IP address isn't available
                    }
                }

                return strIPAddress;
            }
        }

        /// <summary>
        /// Gets the short version of the comm module IPv6 address from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/12 AF  2.53.37 TREQ4719 Created
        //
        public string CommModuleIPv6AddressShort
        {
            get
            {
                string strIPAddress = "";

                if (Table2549 != null)
                {
                    try
                    {
                        strIPAddress = Table2549.IPv6AddressShort;
                    }
                    catch (Exception)
                    {
                        // Do nothing. Ok if the IP address isn't available
                    }
                }

                return strIPAddress;
            }
        }

        /// <summary>
        /// Gets the comm module global address present flag from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/02/12 AF  2.53.37 TREQ4719 Created
        //
        public bool CommModuleGlobalIPAddrPresent
        {
            get
            {
                bool blnAddressPresent = false;

                if (Table2549 != null)
                {
                    try
                    {
                        blnAddressPresent = Table2549.IPv6GlobalAddressPresent;
                    }
                    catch (Exception)
                    {
                        // Do nothing.  Ok if we can't read this table
                        blnAddressPresent = false;
                    }
                }
                return blnAddressPresent;
            }
        }

        /// <summary>
        /// Gets the Comm module's time from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/12 AF  2.53.38 TREQ4721 Created
        //
        public DateTime CommModuleCurrentTime
        {
            get
            {
                DateTime dtTime = new DateTime(1970, 1, 1);

                if (Table2597 != null)
                {
                    try
                    {
                        dtTime = Table2597.CommModuleCurrentTime;
                    }
                    catch (Exception)
                    {
                        // Do nothing.  Just let the 1970 date appear.
                    }
                }

                return dtTime;
            }
        }

        /// <summary>
        /// Last time sync source
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  09/21/15 AF  4.21.04  WR 616166  Created
        //
        public UInt32 SourceID
        {
            get
            {
                UInt32 sourceId = 0xFFFFFFFF;

                if (Table2597 != null)
                {
                    try
                    {
                        sourceId = Table2597.SourceID;
                    }
                    catch (Exception)
                    {
                        // Do nothing. The default value means it was not read.
                    }
                }

                return sourceId;
            }
        }

        /// <summary>
        /// Gets the comm module neighbor list
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 AF  2.53.38 TREQ4722 Created
        //  01/17/13 AF  2.70.58 278324  Added the table activation code so that
        //                               users of this property don't have to know about it.
        //  07/27/15 AF  4.20.18 597247  Retrieve the neighbor list using a TLV request instead of 
        //                               reading the shadow table
        //
        public List<TLVNeighborsItem> TLVNeighbors
        {
            get
            {
                List<TLVNeighborsItem> Neighbors = null;
                ProcedureResultCodes Result = ProcedureResultCodes.COMPLETED;
                CENTRON_AMI AMIDevice = m_AMIDevice as CENTRON_AMI;

                try
                {
                    if (AMIDevice != null && !AMIDevice.IsTableUsed(2612))
                    {
                        // We can't see table 540 so call the procedure to make it visible
                        // Use mfg proc 150 if it is an early Boron meter
                        if (VersionChecker.CompareTo(AMIDevice.FWRevision, CENTRON_AMI.VERSION_BORON_5_0) == 0)
                        {
                            Result = SetDiagnosticsTablesActivationProc150(true);
                        }
                        else
                        {
                            // Mfg proc 159 took over this task in reg f/w 5.2.x
                            Result = SetDiagnosticsTablesActivationProc159(true);
                        }
                    }

                    if (Result == ProcedureResultCodes.COMPLETED)
                    {
                        Result = RequestOnDemandTLV(NEIGHBOR_802154G_TLV);
                        byte[] abyData = null;

                        if (Result == ProcedureResultCodes.COMPLETED)
                        {
                            abyData = AMIDevice.TLVIdRequestedData;
                            if (abyData != null)
                            {
                                List<TLVData> TLVList = new List<TLVData>();
                                TLV.ParseTLVData(abyData, ref TLVList);
                                TLVNeighborsItem TLVNeighbor = null;

                                if (TLVList.Count > 0)
                                {
                                    Neighbors = new List<TLVNeighborsItem>();

                                    for (int index = 0; index < TLVList.Count; index++)
                                    {
                                        TLVNeighbor = new TLVNeighborsItem();

                                        foreach (TLVOptionsData option in TLVList[index].Options)
                                        {
                                            if (option.FieldNumber == NEIGHBOR_INDEX_FIELD_NUMBER)
                                            {
                                                TLVNeighbor.Index = BitConverter.ToInt32(option.FieldValue, 0);
                                            }
                                            else if (option.FieldNumber == NEIGHBOR_MAC_ADDRESS_FIELD_NUMBER)
                                            {
                                                TLVNeighbor.PhysicalAddress = option.FieldValue;
                                            }
                                            else if (option.FieldNumber == NEIGHBOR_LAST_CHANGED_FIELD_NUMBER)
                                            {
                                                TLVNeighbor.LastChanged = BitConverter.ToUInt32(option.FieldValue, 0);
                                            }
                                            else if (option.FieldNumber == NEIGHBOR_RSSI_FORWARD_FIELD_NUMBER)
                                            {
                                                TLVNeighbor.RssiForward = DecodeSInt32(BitConverter.ToInt32(option.FieldValue, 0));
                                            }
                                            else if (option.FieldNumber == NEIGHBOR_RSSI_REVERSE_FIELD_NUMBER)
                                            {
                                                TLVNeighbor.RssiReverse = DecodeSInt32(BitConverter.ToInt32(option.FieldValue, 0));
                                            }
                                        }

                                        bool bNonZeroMAC = false;

                                        for (int i = 0; i < TLVNeighbor.PhysicalAddress.Length; i++)
                                        {
                                            if (TLVNeighbor.PhysicalAddress[i] != 0)
                                            {
                                                bNonZeroMAC = true;
                                                break;      
                                            }
                                        }
                                        if (bNonZeroMAC)
                                        {
                                            Neighbors.Add(TLVNeighbor);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return Neighbors;
                }
                catch (Exception e)
                {
                    throw (e);
                }
                finally
                {
                    if (!m_AMIDevice.IsTableUsed(2588))
                    {
                        // If we made the table visible above, then hide it again here
                        if (VersionChecker.CompareTo(m_AMIDevice.FWRevision, CENTRON_AMI.VERSION_BORON_5_0) == 0)
                        {
                            Result = SetDiagnosticsTablesActivationProc150(false);
                        }
                        else
                        {
                            Result = SetDiagnosticsTablesActivationProc159(false);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Gets the comm module neighbor list from the shadow table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 AF  2.53.38 TREQ4722 Created
        //  01/17/13 AF  2.70.58 278324  Added the table activation code so that
        //                               users of this property don't have to know about it.
        //  05/17/16 AF  4.50.269 683438 Adding this old version of the TLVNeighbors for meters
        //                               that have a buffer size problem for TLV requests
        //
        public List<TLVNeighborsItem> TLVNeighborsFromShadowTable
        {
            get
            {
                List<TLVNeighborsItem> Neighbors = null;
                ProcedureResultCodes Result = ProcedureResultCodes.COMPLETED;

                try
                {
                    if (!m_AMIDevice.IsTableUsed(2588))
                    {
                        // We can't see table 540 so call the procedure to make it visible
                        // Use mfg proc 150 if it is an early Boron meter
                        if (VersionChecker.CompareTo(m_AMIDevice.FWRevision, CENTRON_AMI.VERSION_BORON_5_0) == 0)
                        {
                            Result = SetDiagnosticsTablesActivationProc150(true);
                        }
                        else
                        {
                            // Mfg proc 159 took over this task in reg f/w 5.2.x
                            Result = SetDiagnosticsTablesActivationProc159(true);
                        }
                    }

                    if (Result == ProcedureResultCodes.COMPLETED)
                    {
                        if (Table2604 != null)
                        {
                            Neighbors = Table2604.Neighbors;
                        }
                    }

                    return Neighbors;
                }
                catch (Exception e)
                {
                    throw (e);
                }
                finally
                {
                    if (!m_AMIDevice.IsTableUsed(2588))
                    {
                        // If we made the table visible above, then hide it again here
                        if (VersionChecker.CompareTo(m_AMIDevice.FWRevision, CENTRON_AMI.VERSION_BORON_5_0) == 0)
                        {
                            Result = SetDiagnosticsTablesActivationProc150(false);
                        }
                        else
                        {
                            Result = SetDiagnosticsTablesActivationProc159(false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the IP Route Metrics list
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //  01/17/13 AF  2.70.58 278324  Added the table activation code so that
        //                               users of this property don't have to know about it.
        //
        public List<TLVRouteMetricsItem> TLVIPRouteMetrics
        {
            get
            {
                List<TLVRouteMetricsItem> RouteMetrics = null;
                ProcedureResultCodes Result = ProcedureResultCodes.COMPLETED;

                try
                {
                    if (!m_AMIDevice.IsTableUsed(2588))
                    {
                        // We can't see table 540 so call the procedure to make it visible
                        // Use mfg proc 150 if it is an early Boron meter
                        if (VersionChecker.CompareTo(m_AMIDevice.FWRevision, CENTRON_AMI.VERSION_BORON_5_0) == 0)
                        {
                            Result = SetDiagnosticsTablesActivationProc150(true);
                        }
                        else
                        {
                            // Mfg proc 159 took over this task in reg f/w 5.2.x
                            Result = SetDiagnosticsTablesActivationProc159(true);
                        }
                    }

                    if (Result == ProcedureResultCodes.COMPLETED)
                    {
                        if (Table2602 != null)
                        {
                            RouteMetrics = Table2602.IPRouteMetrics;
                        }                        
                    }

                    return RouteMetrics;
                }
                catch (Exception e)
                {
                    throw (e);
                }
                finally
                {
                    if (!m_AMIDevice.IsTableUsed(2588))
                    {
                        // If we made the table visible above, then hide it again here
                        if (VersionChecker.CompareTo(m_AMIDevice.FWRevision, CENTRON_AMI.VERSION_BORON_5_0) == 0)
                        {
                            Result = SetDiagnosticsTablesActivationProc150(false);
                        }
                        else
                        {
                            Result = SetDiagnosticsTablesActivationProc159(false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the RF Mesh comm module's current time
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#  Description
        //  -------- --- -------  ------  ---------------------------------------------
        //  06/20/12 AF  2.60.33  197712  Function created.
        //
        public override DateTime? ITPTime
        {
            get
            {
                DateTime? ITP = null;
                if (Table2597 != null)
                {
                    try
                    {
                        ITP = Table2597.CommModuleCurrentTime.ToLocalTime();
                    }
                    catch
                    {
                        ITP = null;
                    }
                }

                return ITP;
            }
        }

        /// <summary>
        /// Gets the number of IP sent from 2611
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/21/12 JKW 2.70.xx TC11213 Created
        //
        public UInt32? IPSent
        {
            get
            {
                UInt32? uiDataGramsSent = null;
                if (Table2611 != null)
                {
                    uiDataGramsSent = Table2611.IPSent;
                }

                return uiDataGramsSent;
            }
        }

        /// <summary>
        /// Gets the number of IP received from 2611
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/21/12 JKW 2.70.xx TC11213 Created
        //
        public UInt32? IPReceived
        {
            get
            {
                UInt32? uiDataGramsReceived = null;
                if (Table2611 != null)
                {
                    uiDataGramsReceived = Table2611.IPReceived;
                }

                return uiDataGramsReceived;
            }
        }

        /// <summary>
        /// Gets the IPV6 commence timer.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/13/14 jrf 3.70.01 529116 Created.
        public Int32? CommenceTimer
        {
            get
            {
                Int32? Timer = null;
                ProcedureResultCodes Result = ProcedureResultCodes.COMPLETED;

                try
                {
                    if (!m_AMIDevice.IsTableUsed(2608))
                    {
                        // We can't see table 540 so call the procedure to make it visible
                        // Use mfg proc 150 if it is an early Boron meter
                        if (VersionChecker.CompareTo(m_AMIDevice.FWRevision, CENTRON_AMI.VERSION_BORON_5_0) == 0)
                        {
                            Result = SetDiagnosticsTablesActivationProc150(true);
                        }
                        else
                        {
                            // Mfg proc 159 took over this task in reg f/w 5.2.x
                            Result = SetDiagnosticsTablesActivationProc159(true);
                        }
                    }

                    if (Result == ProcedureResultCodes.COMPLETED)
                    {
                        if (Table2608 != null)
                        {
                            Timer = Table2608.CommenceTimer;
                        }
                    }                    
                }
                catch (Exception e)
                {
                    throw (e);
                }
                finally
                {
                    if (!m_AMIDevice.IsTableUsed(2588))
                    {
                        // If we made the table visible above, then hide it again here
                        if (VersionChecker.CompareTo(m_AMIDevice.FWRevision, CENTRON_AMI.VERSION_BORON_5_0) == 0)
                        {
                            Result = SetDiagnosticsTablesActivationProc150(false);
                        }
                        else
                        {
                            Result = SetDiagnosticsTablesActivationProc159(false);
                        }
                    }
                }

                return Timer;
            }
        }

        /// <summary>
        /// Determines whether or not the IPV6 link is locked.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/13/14 jrf 3.70.01 529116 Created.
        //  08/15/14 jrf 3.70.02 529116 Updated min commence timer value to constant with a new minimum.
        public bool IsIPV6LinkLocked
        {
            get
            {
                bool Locked = false;

                if (null != CommenceTimer)
                {
                    //Anything less than this value prevents the IPV6 stack from coming up.
                    if (MIN_VALID_COMMENCE_TIMER > CommenceTimer)
                    {
                        Locked = true;
                    }
                }

                return Locked;
            }
        }

        /// <summary>
        /// Reads the IsSynchronizedOrConnected field out of Mfg table 14 (2062).
        /// Written for the IPv6 M2 Gateway.  If tempted to use for a different meter,
        /// be sure to check that the field exists and the offset is correct.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  09/01/15 AF  4.21.03  WR 610037  Created
        //
        public bool IsC1222StatusSynchronized
        {
            get
            {
                bool Synced = false;
                byte[] Data;
                int IsSynchronizedOrConnectedFieldOffset = 9;
                ushort IsSynchronizedOrConnectedFieldSize = 1;

                try
                {
                    CTable00 Table0 = new CTable00(m_PSEM);

                    if (Table0.IsTableUsed(2062))
                    {
                        if (m_PSEM.OffsetRead(2062, IsSynchronizedOrConnectedFieldOffset, IsSynchronizedOrConnectedFieldSize, out Data) == PSEMResponse.Ok)
                        {
                            if (Data[0] != 0)
                            {
                                Synced = true;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Do nothing. Just assumed it is not synced
                }

                return Synced;
            }
        }

        /// <summary>
        /// Gets whether or not the comm module radio is on
        /// </summary>
        public override bool? IsRadioOn
        {
            get
            {
                bool? IsOn = null;

                uint OptOutStatus = 0;
                ProcedureResultCodes Result = ProcedureResultCodes.COMPLETED;
                CENTRON_AMI AMIDevice = m_AMIDevice as CENTRON_AMI;

                try
                {
                    if (AMIDevice != null)
                    {
                        if (!AMIDevice.IsTableUsed(2612))
                        {
                            // We can't see the table so call the procedure to make it visible
                            // Use mfg proc 150 if it is an early Boron meter
                            if (VersionChecker.CompareTo(m_AMIDevice.FWRevision, CENTRON_AMI.VERSION_BORON_5_0) == 0)
                            {
                                Result = SetDiagnosticsTablesActivationProc150(true);
                            }
                            else
                            {
                                // Mfg proc 159 took over this task in reg f/w 5.2.x
                                Result = SetDiagnosticsTablesActivationProc159(true);
                            }
                        }

                        if (Result == ProcedureResultCodes.COMPLETED)
                        {
                            Result = RequestOnDemandTLV(INTERFACE_SETTINGS_TLV);
                            byte[] abyData = null;

                            if (Result == ProcedureResultCodes.COMPLETED)
                            {
                                abyData = AMIDevice.TLVIdRequestedData;
                                if (abyData != null)
                                {
                                    List<TLVData> TLVList = new List<TLVData>();
                                    TLV.ParseTLVData(abyData, ref TLVList);

                                    if (TLVList.Count > 0)
                                    {
                                        foreach (TLVOptionsData option in TLVList[1].Options)
                                        {
                                            if (option.FieldNumber == INTERFACE_SETTINGS_OPT_OUT_FIELD_NUMBER)
                                            {
                                                OptOutStatus = BitConverter.ToUInt32(option.FieldValue, 0);

                                                if (OPT_OUT_UP == OptOutStatus)
                                                {
                                                    IsOn = true;
                                                }
                                                else if (OPT_OUT_DOWN == OptOutStatus)
                                                {
                                                    IsOn = false;
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else { Result = ProcedureResultCodes.INVALID_PARAM; }
                        }
                        //else
                        {
                            // Do nothing. Just return 0.
                        }
                    }
                }
                catch
                {
                    // Do nothing. Just return 0.
                }
                finally
                {
                    if (!AMIDevice.IsTableUsed(2612))
                    {
                        // If we made the table visible above, then hide it again here
                        if (VersionChecker.CompareTo(m_AMIDevice.FWRevision, CENTRON_AMI.VERSION_BORON_5_0) == 0)
                        {
                            Result = SetDiagnosticsTablesActivationProc150(false);
                        }
                        else
                        {
                            Result = SetDiagnosticsTablesActivationProc159(false);
                        }
                    }
                }

                return IsOn;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Signed integers are zig zag encoded in CSMP, but there is no way
        /// to tell that it is a signed int from the raw CoAP. We will handle it here
        /// when we know that the field needs further processing.
        /// </summary>
        /// <param name="value">the decoded varint value</param>
        /// <returns>the actual value of the item</returns>
        /// <remarks>Zig zag encoding: 0 -> 0, -1 -> 1, 1 -> 2, -2 -> 3, 2 -> 4, etc.</remarks>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  07/27/15 AF  4.20.18  WR 597247  Created
        //
        private int DecodeSInt32(int value)
        {
            int decodedValue = 0;
            // Check whether the encoded value is odd or even
            int oddEven = value & 1;    //1: odd (negative), 0: even (positive)

            // In either case, divide the encoded value by 2
            value = value >> 1;

            if (oddEven > 0)
            {
                // An odd encoded value indicates a negative number - add one and change the sign
                value++;
                decodedValue = (int)value;
                decodedValue *= -1;
            }
            else
            {
                // No further processing needed for a positive number
                decodedValue = (int)value;
            }

            return decodedValue;
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the IPv6 Address Information table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/01/12 AF  2.53.37 TREQ4719 Created
        //
        private IPMfgTable2549 Table2549
        {
            get
            {
                if (m_Table2549 == null)
                {
                    m_Table2549 = new IPMfgTable2549(m_PSEM);
                }

                return m_Table2549;
            }
        }

        /// <summary>
        /// Gets the IPv6 Secure PPP Information object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  08/26/15 AF  4.21.02  WR 608214  Removed the dependence on fw version
        //
        private IPMfgTable2559 Table2559
        {
            get
            {
                if (m_Table2559 == null)
                {
                    m_Table2559 = new IPMfgTable2559(m_PSEM);
                }

                return m_Table2559;
            }
        }

        /// <summary>
        /// Gets the RFLAN Information table object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 01/17/12 AF  2.53.31 183921 Created
        // 08/26/15 AF  4.21.02 608214 Removed the dependence on fw version
        //
        private IPMfgTable2580 Table2580
        {
            get
            {
                if (m_Table2580 == null)
                {
                    m_Table2580 = new IPMfgTable2580(m_PSEM);
                }

                return m_Table2580;
            }
        }

        /// <summary>
        /// Gets the IPMfgTable2588 table object (creates it if needed)
        /// This is the dimension table for tables 554 and 556
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/12/12 AF  2.70.50 263052 Created
        //
        private IPMfgTable2588 Table2588
        {
            get
            {
                if (m_Table2588 == null)
                {
                    m_Table2588 = new IPMfgTable2588(m_PSEM);
                }

                return m_Table2588;
            }
        }

        /// <summary>
        /// Gets the TLV Current Time table object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/03/12 AF  2.53.38 TREQ4721 Created
        //  12/06/12 AF  2.70.47 262897   Table size increased by 4 bytes in build 5.5.8
        //  09/21/15 AF  4.21.04 616166   Removed the firmware version parameters. The size is now handled
        //                                in the Read() method.
        //
        private IPMfgTable2597 Table2597
        {
            get
            {
                if (m_Table2597 == null)
                {
                    m_Table2597 = new IPMfgTable2597(m_PSEM);
                }

                return m_Table2597;
            }
        }

        /// <summary>
        /// Gets the IPMfgTable2602 table object (creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/15/12 AF  2.53.41 TREQ5578 Created
        //  12/12/12 AF  2.70.50 263124   Added the dimension table (540) as a parameter
        //
        private IPMfgTable2602 Table2602
        {
            get
            {
                if (m_Table2602 == null)
                {
                    m_Table2602 = new IPMfgTable2602(m_PSEM, Table2588);
                }

                return m_Table2602;
            }
        }

        /// <summary>
        /// Gets the IPMfgTable2604 table object (creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 AF  2.53.38 TREQ4722 Created
        //  12/12/12 AF  2.70.50 263052   Added the dimension table (540) as a parameter
        //
        private IPMfgTable2604 Table2604
        {
            get
            {
                if (m_Table2604 == null)
                {
                    m_Table2604 = new IPMfgTable2604(m_PSEM, Table2588);
                }

                return m_Table2604;
            }
        }

        /// <summary>
        /// Gets the IPMfgTable2608 table object (creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/13/14 jrf 3.70.01 529116 Created.
        private IPMfgTable2608 Table2608
        {
            get
            {
                if (m_Table2608 == null)
                {
                    m_Table2608 = new IPMfgTable2608(m_PSEM);
                }

                return m_Table2608;
            }
        }

        /// <summary>
        /// Gets the IPMfgTable2604 table object (creates it if needed)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/06/12 AF  2.53.38 TREQ4722 Created
        //
        private IPMfgTable2611 Table2611
        {
            get
            {
                CTable00 Table0 = new CTable00(m_PSEM);
                CTable01 Table1 = new CTable01(m_PSEM, Table0.StdVersion);

                if (m_Table2611 == null)
                {
                    m_Table2611 = new IPMfgTable2611(m_PSEM, Table1.FW_Rev);
                }

                return m_Table2611;
            }
        }

        #endregion

        #region Members

        private IPMfgTable2549 m_Table2549;
        private IPMfgTable2559 m_Table2559;
        private IPMfgTable2580 m_Table2580;
        private IPMfgTable2588 m_Table2588;
        private IPMfgTable2597 m_Table2597;
        private IPMfgTable2602 m_Table2602;
        private IPMfgTable2604 m_Table2604;
        private IPMfgTable2608 m_Table2608;
        private IPMfgTable2611 m_Table2611;

        #endregion
    }
}
