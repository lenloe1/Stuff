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
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Interface to all Device Status Items defined in Itron Device
    /// </summary>
    public interface IItronDeviceStatus
    {
        /// <summary>
        /// Property used to get the Date Programmed from the meter
        /// </summary>
        DateTime DateProgrammed
        {
            get;
        }
        /// <summary>
        /// Property used to get the Date of Last Demand Reset from the meter
        /// </summary>
        DateTime DateLastDemandReset
        {
            get;
        }
        /// <summary>
        /// Property used to get the Date of Last Outage from the meter
        /// </summary>
        DateTime DateLastOutage
        {
            get;
        }
        /// <summary>
        /// Property used to get the date of the TOU expiration
        /// </summary>
        DateTime TOUExpirationDate
        {
            get;
        }

        /// <summary>
        /// Property used to get the Number of Times Programmed from the meter
        /// </summary>
        int NumTimeProgrammed
        {
            get;
        }
        /// <summary>
        /// Property used to get the Number of Demand Resets from the meter
        /// </summary>
        int NumDemandResets
        {
            get;
        }
        /// <summary>
        /// Property used to get the Number of Outages from the meter
        /// </summary>
        int NumOutages
        {
            get;
        }

        /// <summary>
        /// This property returns the number of minutes that the
        /// meter run on battery power.  This is a read-only value.
        /// </summary>
        uint NumberOfMinutesOnBattery
        {
            get;
        }

        /// <summary>
        /// Indicates whether or not the meter is currently recording
        /// load profile data
        /// </summary>
        bool LPRunning
        {
            get;
        }

        /// <summary>
        /// Property to get the line frequency from the device.
        /// </summary>
        float LineFrequency
        {
            get;
        }

    }
}
