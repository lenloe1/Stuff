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
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Interface which needs to be implemented by devices capable of 
    /// supporting the ERT Check in HH-Pro (Sentinel only).
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 04/12/06 mrj 7.30.00    N/A Created
    ///
    public interface IERTCheck
    {
        /// <summary>
        /// Gets a boolean indicating whether the device is configured
        /// for RF or not
        /// </summary>
        bool RFConfigured
        {
            get;
        }

        /// <summary>
        /// Gets the ERT Check data.  The array should be sized by the number
        /// of SCM ERTs configured in the meter.
        /// </summary>
        ERTConfig[] ERTCheck
        {
            get;
        }
    }

    /// <summary>
    /// This structure represent all of the data needed for the HH-Pro ERT check. 
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 04/12/06 mrj 7.30.00    N/A Created
    ///
    public struct ERTConfig
    {
        /// <summary>
        /// The ERT ID.
        /// </summary>
        public int m_iERTID;
        /// <summary>
        /// String representing the quantity.
        /// </summary>
        public string m_strQty;
        /// <summary>
        /// Total number of digits.  If 0, then the ERT represents a date/time and
        /// "/" must be added when formatting.
        /// </summary>
        public byte m_bytTotalDigits;
        /// <summary>
        /// Number of decimal digits.
        /// </summary>
        public byte m_bytDecDigits;
    }
}