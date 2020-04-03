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
    /// supporting custom schedules.
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 04/12/06 mrj 7.30.00    N/A Created
    ///
    public interface ICustomSchedule
    {
        /// <summary>
        /// Gets a boolean indicating whether the device is configured
        /// for custom schedules or not.
        /// </summary>
        bool CustomScheduleConfigured
        {
            get;
        }

        /// <summary>
        /// Gets a boolean indicating whether the device is configured
        /// for multiple custom schedules or not (in 2084).
        /// </summary>
        bool MultipleCustomScheduleConfigured
        {
            get;
        }

        /// <summary>
        /// Reconfigure the custom schedule using the supplied schedule
        /// </summary>
        /// <param name="sPathName">Path to the custom schedule</param>
        /// <param name="sScheduleName">custom schedule name</param>
        /// <param name="bWriteUserData2">Write schedule name to user data 2 flag</param>
        /// <returns>A CSReconfigResults</returns>
        CSReconfigResult Reconfigure(string sPathName, string sScheduleName, bool bWriteUserData2);
    }
}