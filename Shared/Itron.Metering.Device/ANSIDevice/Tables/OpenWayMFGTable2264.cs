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
//                              Copyright © 2010
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
    /// MFG Table 2264 (Itron 216)
    /// </summary>
    public class OpenWayMFGTable2264 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 36;
        private const int TABLE_TIMEOUT = 5000;

        #endregion

        #region Definitions

        /// <summary>
        /// Enumeration of the possible sources for configuring the meter.
        /// </summary>
        public enum ConfigSource: byte
        {
            /// <summary>
            /// Source was internal to the meter.
            /// </summary>
            INTERNAL = 0,
            /// <summary>
            /// Source was from the optical port.
            /// </summary>
            OPTICAL = 1,
            /// <summary>
            /// Source was an OTA exception host.
            /// </summary>
            EXCEPTION_HOST = 2,
            /// <summary>
            /// Source was an OTA non-exception host.
            /// </summary>
            NON_EXCEPTION_HOST = 3,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The current PSEM communications object</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/10 jrf 2.40.50        Created
        //
        public OpenWayMFGTable2264(CPSEM psem)
            : base(psem, 2264, TABLE_SIZE, TABLE_TIMEOUT)
        {
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/10 jrf 2.40.50        Created
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2264.Read");

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
        /// This property retrieves the current configuration count.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/10 jrf 2.40.50        Created

        public ushort ProgramCount
        {
            get
            {
                PSEMResponse Result = base.Read(6, 2);

                if (PSEMResponse.Ok == Result)
                {
                    m_usProgramCount = m_Reader.ReadUInt16();
                    return m_usProgramCount;
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the Program Count"));
                }
            }
        }

        /// <summary>
        /// This property retrieves the date and time of the last configuration.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/10 jrf 2.40.50        Created

        public DateTime LastConfigurationTime
        {
            get
            {
                PSEMResponse Result = base.Read(5, 0);

                if (PSEMResponse.Ok == Result)
                {
                    m_dtLastConfigurationTime = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
                    return m_dtLastConfigurationTime;
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the Last Configuration Time"));
                }
            }
        }

        /// <summary>
        /// This property retrieves the date and time of the last configuration.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/10 jrf 2.40.50        Created

        public ConfigSource LastConfigurationSource
        {
            get
            {
                PSEMResponse Result = base.Read(5, 1);

                if (PSEMResponse.Ok == Result)
                {
                    m_LastConfigSource = (ConfigSource)(m_Reader.ReadByte());
                    return m_LastConfigSource;
                }
                else
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the Last Configuration Source"));
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parse the data read from the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/10 jrf 2.40.50        Created

        private void ParseData()
        {
            m_dtLastConfigurationTime = m_Reader.ReadLTIME((PSEMBinaryReader.TM_FORMAT)m_PSEM.TimeFormat);
            m_LastConfigSource = (ConfigSource)(m_Reader.ReadByte());
            m_usProgramCount = m_Reader.ReadUInt16();
        }

        #endregion

        #region Members

        private DateTime m_dtLastConfigurationTime;
        private ConfigSource m_LastConfigSource;
        private ushort m_usProgramCount;

        #endregion
    }
}
