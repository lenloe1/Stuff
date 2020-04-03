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
    /// MFG Table 2428 (Itron 380) class
    /// </summary>
    public class OpenWayMFGTable2428 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 11;

        #endregion

        #region Definitions

        /// <summary>
        /// ChoiceConnect Comm.s Operational Modes
        /// </summary>
        public enum ChoiceConnectCommOpMode : byte
        {
            /// <summary>
            /// Unknown, Unrecognized, or Unsupported Operational Mode
            /// </summary>
            [EnumDescription("Unknown Mode")]
            UnknownOperationalMode = 0,
            /// <summary>
            /// OpenWay Operational Mode
            /// </summary>
            [EnumDescription("OpenWay Mode")]
            OpenWayOperationalMode = 1,
            /// <summary>
            /// ChoiceConnect Operational Mode
            /// </summary>
            [EnumDescription("ChoiceConnect Mode")]
            ChoiceConnectOperationalMode = 2,
        }

        /// <summary>
        /// ChoiceConnect Comm.s Manufacturing Modes
        /// </summary>
        public enum ChoiceConnectCommMfgMode : byte
        {
            /// <summary>
            /// Unknown, Unrecognized, or Unsupported Manufacturing Mode
            /// </summary>
            UnknownManufacturingMode = 0,
            /// <summary>
            /// ChoiceConnect Manufacturing Mode for RF LAN migration
            /// </summary>
            ChoiceConnectManufacturingModeRFLAN = 0xA1,
            /// <summary>
            /// ChoiceConnect Manufacturing Mode for RF Mesh migration
            /// </summary>
            ChoiceConnectManufacturingModeRFMesh = 0xA2,
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
        // 03/05/12 JJJ 2.60.xx N/A    Created

        public OpenWayMFGTable2428(CPSEM psem)
            : base (psem, 2428, TABLE_SIZE)
        {
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 11/18/13 jrf 3.50.06 TQ 9479 Created 
        //
        public OpenWayMFGTable2428(PSEMBinaryReader reader)
            : base(2439, TABLE_SIZE)
        {
            m_Reader = reader;

            ParseData();
            
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Gets the text string for the specified ChoiceConnect Comm.s Operational Mode
        /// </summary>
        /// <param name="mode">The communications mode to get the string for.</param>
        /// <returns>The string for the communications mode.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/05/12 JJJ 2.60.xx N/A    Created

        public static string GetChoiceConnectCommOpModeString(ChoiceConnectCommOpMode mode)
        {
            string strMode = null;

            switch(mode)
            {
                case ChoiceConnectCommOpMode.OpenWayOperationalMode:
                {
                    strMode = "OpenWay Mode";
                    break;
                }
                case ChoiceConnectCommOpMode.ChoiceConnectOperationalMode:
                {
                    strMode = "ChoiceConnect Mode";
                    break;
                }

                // This is 0 entry, but also any other value that we don't recognize should be "unknown"
                case ChoiceConnectCommOpMode.UnknownOperationalMode:
                default:
                {
                    strMode = "Unknown Mode";
                    break;
                }
            }

            return strMode;
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

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2428.Read");

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

        /// <summary>
        /// Provides a method for requesting that the table be refreshed the next  
        /// time the table data is accessed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/14/12 JJJ 2.60.xx         Created
        //
        public void Refresh()
        {
            m_TableState = TableState.Expired;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the original ChoiceConnect Comm. Mode at manufacture time.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/05/12 JJJ 2.60.xx N/A    Created

        public ChoiceConnectCommMfgMode ManufacturedMode
        {
            get
            {
                ReadUnloadedTable();
                return (ChoiceConnectCommMfgMode)m_byManufacturedMode;
            }
        }

        /// <summary>
        /// Gets the last requested ChoiceConnect Comm. Mode.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/05/12 JJJ 2.60.xx N/A    Created
        // 10/08/12 jrf 2.61.00 273251 Modified to have this value re-read every
        //                             time it is requested.
        //
        public ChoiceConnectCommOpMode RequestedMode
        {
            get
            {
                Read();  //This value should not be cached.
                return (ChoiceConnectCommOpMode)m_byRequestedMode;
            }
        }

        /// <summary>
        /// Gets the Register's Current ChoiceConnect Comm. Mode.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/05/12 JJJ 2.60.xx N/A    Created
        // 10/08/12 jrf 2.61.00 273251 Modified to have this value re-read every
        //                             time it is requested.
        //
        public ChoiceConnectCommOpMode CurrentRegisterMode
        {
            get
            {
                Read();  //This value should not be cached.
                return (ChoiceConnectCommOpMode)m_byCurrentRegisterMode;
            }
        }

        /// <summary>
        /// Gets the Comm. Module's Current ChoiceConnect Comm. Mode.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/05/12 JJJ 2.60.xx N/A    Created
        // 10/08/12 jrf 2.61.00 273251 Modified to have this value re-read every
        //                             time it is requested.
        //
        public ChoiceConnectCommOpMode CurrentCommModuleMode
        {
            get
            {
                Read();  //This value should not be cached.
                return (ChoiceConnectCommOpMode)m_byCurrentCommModuleMode;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect Communicaitons Error value.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/05/12 JJJ 2.60.xx N/A    Created
        // 10/08/12 jrf 2.61.00 273251 Modified to have this value re-read every
        //                             time it is requested.
        //
        public byte ErrorVal
        {
            get
            {
                Read();  //This value should not be cached.
                return m_byError;
            }
        }

        /// <summary>
        /// Gets the OpenWay Comm. FW Version saved in the ChoiceConnect table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/19/12 JJJ 2.60.xx N/A    Created

        public byte CCOpenWayCommFwVer
        {
            get
            {
                ReadUnloadedTable();
                return m_byCCOpenWayCommFwVer;
            }
        }

        /// <summary>
        /// Gets the OpenWay Comm. FW Revision saved in the ChoiceConnect table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/19/12 JJJ 2.60.xx N/A    Created

        public byte CCOpenWayCommFwRev
        {
            get
            {
                ReadUnloadedTable();
                return m_byCCOpenWayCommFwRev;
            }
        }

        /// <summary>
        /// Gets the OpenWay Comm. FW Build number saved in the ChoiceConnect table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/19/12 JJJ 2.60.xx N/A    Created

        public byte CCOpenWayCommFwBuild
        {
            get
            {
                ReadUnloadedTable();
                return m_byCCOpenWayCommFwBuild;
            }
        }

        /// <summary>
        /// Gets the OpenWay Comm. FW Version and revision, saved in the
        /// ChoiceConnect table, as a formatted string xxx.xxx
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/19/12 JJJ 2.60.xx N/A    Created

        public string CCOpenWayCommFwVerRev
        {
            get
            {
                string strFWVerRev = "0.000";

                ReadUnloadedTable();
                strFWVerRev = m_byCCOpenWayCommFwVer.ToString(CultureInfo.InvariantCulture) + "." +
                    m_byCCOpenWayCommFwRev.ToString("d3", CultureInfo.InvariantCulture);

                return strFWVerRev;
            }
        }

        /// <summary>
        /// Gets the OpenWay Comm. FW Build, saved in the ChoiceConnect table,
        /// as a 3 digit formatted string 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/19/12 JJJ 2.60.xx N/A    Created

        public string CCOpenWayCommFwBuildString
        {
            get
            {
                string strFWBuild = "000";

                ReadUnloadedTable();
                strFWBuild = m_byCCOpenWayCommFwBuild.ToString("d3", CultureInfo.InvariantCulture);

                return strFWBuild;
            }
        }
        /// <summary>
        /// Gets the ChoiceConnect Comm. FW Version
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/19/12 JJJ 2.60.xx N/A    Created

        public byte CCChoiceConnectCommFwVer
        {
            get
            {
                ReadUnloadedTable();
                return m_byCCChoiceConnectCommFwVer;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect Comm. FW Revision
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/19/12 JJJ 2.60.xx N/A    Created

        public byte CCChoiceConnectCommFwRev
        {
            get
            {
                ReadUnloadedTable();
                return m_byCCChoiceConnectCommFwRev;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect Comm. FW Build number
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/19/12 JJJ 2.60.xx N/A    Created

        public byte CCChoiceConnectCommFwBuild
        {
            get
            {
                ReadUnloadedTable();
                return m_byCCChoiceConnectCommFwBuild;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect Comm. FW Version and revision
        /// as a formatted string xxx.xxx
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/19/12 JJJ 2.60.xx N/A    Created

        public string CCChoiceConnectCommFwVerRev
        {
            get
            {
                string strFWVerRev = "0.000";

                ReadUnloadedTable();
                strFWVerRev = m_byCCChoiceConnectCommFwVer.ToString(CultureInfo.InvariantCulture) + "." +
                    m_byCCChoiceConnectCommFwRev.ToString("d3", CultureInfo.InvariantCulture);

                return strFWVerRev;
            }
        }

        /// <summary>
        /// Gets the ChoiceConnect Comm. FW Build
        /// as a 3 digit formatted string 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/19/12 JJJ 2.60.xx N/A    Created

        public string CCChoiceConnectCommFwBuildString
        {
            get
            {
                string strFWBuild = "000";

                ReadUnloadedTable();
                strFWBuild = m_byCCChoiceConnectCommFwBuild.ToString("d3", CultureInfo.InvariantCulture);

                return strFWBuild;
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
            m_byManufacturedMode = m_Reader.ReadByte();
            m_byRequestedMode = m_Reader.ReadByte();
            m_byCurrentRegisterMode = m_Reader.ReadByte();
            m_byCurrentCommModuleMode = m_Reader.ReadByte();
            m_byError = m_Reader.ReadByte();
            m_byCCOpenWayCommFwVer = m_Reader.ReadByte();
            m_byCCOpenWayCommFwRev = m_Reader.ReadByte();
            m_byCCOpenWayCommFwBuild = m_Reader.ReadByte();
            m_byCCChoiceConnectCommFwVer = m_Reader.ReadByte();
            m_byCCChoiceConnectCommFwRev = m_Reader.ReadByte();
            m_byCCChoiceConnectCommFwBuild = m_Reader.ReadByte();
        }

        #endregion

        #region Member Variables

        private byte m_byManufacturedMode;
        private byte m_byRequestedMode;
        private byte m_byCurrentRegisterMode;
        private byte m_byCurrentCommModuleMode;
        private byte m_byError;
        private byte m_byCCOpenWayCommFwVer;
        private byte m_byCCOpenWayCommFwRev;
        private byte m_byCCOpenWayCommFwBuild;
        private byte m_byCCChoiceConnectCommFwVer;
        private byte m_byCCChoiceConnectCommFwRev;
        private byte m_byCCChoiceConnectCommFwBuild;
        
        #endregion
    }
}
