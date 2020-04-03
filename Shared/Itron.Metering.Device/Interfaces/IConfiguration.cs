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
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Interface which needs to be implemented by device capable of
    /// configuring a meter
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 08/27/06 RCG 7.35.00    N/A Created
    ///    
    public interface IConfiguration
    {
        /// <summary>
        /// Configures a device with the specified program.
        /// </summary>
        /// <param name="sProgramName">Name or path of the program.</param>
        /// <returns>A ConfigurationError code</returns>
        ConfigurationResult Configure(string sProgramName);

        /// <summary>
        /// Configures a device with the specified program and Prompt for data.
        /// </summary>
        /// <param name="sProgramName">Name or path of the program.</param>
        /// <param name="PFData">The prompt for data for the program.</param>
        /// <returns>A ConfigurationError code.</returns>
        ConfigurationResult Configure(string sProgramName, PromptForData PFData);
    }
}