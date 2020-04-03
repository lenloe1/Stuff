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
//                              Copyright © 2015 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using System.Globalization;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class that describes OpenWay MFG table 2377 - Instantaneous Phase Current
    /// </summary>
    public class OpenWayMFGTable2377 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 29;
        private const int TABLE_TIMEOUT = 500;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  12/07/15 PGH 4.50.219 627380 Created
        //
        public OpenWayMFGTable2377(CPSEM psem)
            : base(psem, 2377, TABLE_SIZE, TABLE_TIMEOUT)
        {
            m_IPCDataRcd = null;
        }

        /// <summary>
        /// Constructor used to get data from the EDL file
        /// </summary>
        /// <param name="reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  12/07/15 PGH 4.50.219 627380 Created
        //
        public OpenWayMFGTable2377(PSEMBinaryReader reader)
            : base(2377, TABLE_SIZE)
        {
            m_IPCDataRcd = null;
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the Instantaneous Phase Current out of Mfg table 2377
        /// </summary>
        /// <returns>PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  12/07/15 PGH 4.50.219 627380 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "InstantaneousPhaseCurrent.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;
                ParseData();
            }

            return Result;
        }

        /// <summary>
        ///  Writes the data to the log file for debugging purposes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  12/07/15 PGH 4.50.219 627380 Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.DateTime.ToString")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Double.ToString(System.String)")]
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                   "Dump of Instantaneous Phase Current.");

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Instantaneous Current RMS Phase A: " + m_IPCDataRcd.InstCurrentRMSPhaseA.ToString("F2"));
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Instantaneous Current RMS Phase B: " + m_IPCDataRcd.InstCurrentRMSPhaseB.ToString("F2"));
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Instantaneous Current RMS Phase C: " + m_IPCDataRcd.InstCurrentRMSPhaseC.ToString("F2"));
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol, "Last Update: " + m_IPCDataRcd.LastUpdate.ToString());
            }
            catch (Exception e)
            {
                try
                {
                    m_Logger.WriteException(this, e);
                }
                catch
                {
                    // No exceptions thrown from this debugging method
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Instantaneous Phase Current Record from table 2377
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  12/07/15 PGH 4.50.219 627380 Created
        //  05/20/16 PGH 4.50.270 687608 Read table instantaneously
        //  05/24/16 PGH 4.50.271 687608 The table must be in synch with the meter for the EDL viewer
        //
        public IPCDataRcd IPCDataRecord
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Instantaneous Phase Current Record."));
                    }
                }

                return m_IPCDataRcd;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the Instantaneous Phase Current Record out of the stream.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  12/07/15 PGH 4.50.219 627380 Created
        //  01/26/16 PGH 4.50.224 627380 Design changed
        //
        private void ParseData()
        {
            m_IPCDataRcd = new IPCDataRcd();

            m_IPCDataRcd.InstCurrentRMSPhaseA = m_Reader.ReadDouble(); // 8
            m_IPCDataRcd.InstCurrentRMSPhaseB = m_Reader.ReadDouble(); // 8
            m_IPCDataRcd.InstCurrentRMSPhaseC = m_Reader.ReadDouble(); // 8
            m_IPCDataRcd.LastUpdate = m_Reader.ReadLTIME(PSEMBinaryReader.TM_FORMAT.UINT32_TIME).ToLocalTime(); // 5
        }

        #endregion


        #region Members

        private IPCDataRcd m_IPCDataRcd;

        #endregion

    }

    /// <summary>
    /// Class that represents a Instantaneous Phase Current Data Record
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version  Issue# Description
    //  -------- --- -------  ------ -------------------------------------------
    //  12/07/15 PGH 4.50.219 627380 Created
    //
    public class IPCDataRcd
    {
        #region Constants

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  Issue# Description
        //  -------- --- -------  ------ -------------------------------------------
        //  12/07/15 PGH 4.50.219 627380 Created
        //  01/26/16 PGH 4.50.224 627380 Design changed
        //
        public IPCDataRcd()
        {
            m_uiInstCurrentRMSPhaseA = 0.0;
            m_uiInstCurrentRMSPhaseB = 0.0;
            m_uiInstCurrentRMSPhaseC = 0.0;
            m_dtLastUpdate = DateTime.MinValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Instantaneous Current RMS Phase A
        /// </summary>
        public double InstCurrentRMSPhaseA
        {
            get
            {
                return m_uiInstCurrentRMSPhaseA;
            }
            set
            {
                m_uiInstCurrentRMSPhaseA = value;
            }
        }

        /// <summary>
        /// Instantaneous Current RMS Phase B
        /// </summary>
        public double InstCurrentRMSPhaseB
        {
            get
            {
                return m_uiInstCurrentRMSPhaseB;
            }
            set
            {
                m_uiInstCurrentRMSPhaseB = value;
            }
        }

        /// <summary>
        /// Instantaneous Current RMS Phase C
        /// </summary>
        public double InstCurrentRMSPhaseC
        {
            get
            {
                return m_uiInstCurrentRMSPhaseC;
            }
            set
            {
                m_uiInstCurrentRMSPhaseC = value;
            }
        }

        /// <summary>
        /// Last Update
        /// </summary>
        public DateTime LastUpdate
        {
            get
            {
                return m_dtLastUpdate;
            }
            set
            {
                m_dtLastUpdate = value;
            }
        }

        #endregion

        #region Members

        private double m_uiInstCurrentRMSPhaseA;
        private double m_uiInstCurrentRMSPhaseB;
        private double m_uiInstCurrentRMSPhaseC;
        private DateTime m_dtLastUpdate;

        #endregion
    }

}
