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
//                            Copyright © 2008 - 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class that represents ITRN 16 (Mirror of Comm Module table 0)
    /// </summary>
    public class OpenWayCommModule_2064 : CTable00
    {
        #region Public Methods

        /// <summary>
        /// Constructor for the Comm Module Limiting Table (Table 2064)
        ///     This is a mirror of Table 0 from the Comm Module
        /// </summary>
        /// <param name="psem"></param>
        public OpenWayCommModule_2064(CPSEM psem)
            : base(psem, 2064)
        {
        }

        /// <summary>
        /// Constructor for use in the EDL File for the Comm Module Limiting Table (Table 2064)
        /// This is a mirror of Table 0 from the Comm Module
        /// </summary>
        /// <param name="reader">PSEM Binary Reader contain the part of the table we are interested in</param>
        /// <param name="length">The length of the data</param>
        public OpenWayCommModule_2064(PSEMBinaryReader reader, uint length)
            : base(reader, 2064, length)
        {
        }

        /// <summary>
        /// Reads table 2064 out of the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Err;
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Table " + m_TableID.ToString(CultureInfo.CurrentCulture));

                //The base class was using a partial read which was failing on SBR base meter. Switching to full read as a work around.
                Result = base.FullRead();

                if (PSEMResponse.Ok == Result)
                {
                    m_DataStream.Position = 0;

                    ParseHeaderInfo();
                    ParseTableInfo();

                    m_TableState = TableState.Loaded;
                }
            }
            catch { }
            finally
            {
                if (PSEMResponse.Ok != Result)
                {
                    //If the full read fails then revert back to the way things were before.
                    Result = base.Read();
                }
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Retrieves the Comm Module Device Class of the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/08/10 AF  2.40.12        Cloned from Table 00 code so that an absent
        //                              Comm module does not cause an exception
        //  02/09/10 AF  2.40.12        Changed the variable to a member variable to
        //                              preserve the result of the first read.
        //  03/29/10 RCG 2.40.30        Previous change caused problems with EDL files changing to catch exception
        public override string DeviceClass
        {
            get
            {
                string strDeviceClass = "";

                try
                {
                    strDeviceClass = base.DeviceClass;
                }
                catch (PSEMException)
                {
                    // If this fails we should allow it to continue
                    strDeviceClass = "";
                }

                return strDeviceClass;
            }
        }

        #endregion
    }
}
