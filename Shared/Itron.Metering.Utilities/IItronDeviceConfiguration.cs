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
//                              Copyright © 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Interface that provide Configuration Properties for an ItronDevice
    /// </summary>
    public interface IItronDeviceConfiguration
    {
        /// <summary>
        /// Property used to get the unit ID (string) from the meter
        /// </summary>
        string UnitID
        {
            get;
        }
        /// <summary>
        /// Property used to get the program ID (int) from the meter
        /// </summary>
        int ProgramID
        {
            get;
        }

        /// <summary>
        /// Provides access to the meter's time of use schedule ID.
        /// Note that this is returned as a string since one or more
        /// meters allow non-numeric TOU schedule identifiers
        /// </summary>
        String TOUScheduleID
        {
            get;
        }

        /// <summary>
        /// Property used to get the device time (DateTime) from the meter
        /// </summary>
        DateTime DeviceTime
        {
            get;
        }

        /// <summary>
        /// Property used to get the software version from the meter
        /// </summary>
        String SWRevision
        {
            get;
        }

        /// <summary>
        /// Property used to get the firmware version (float) from the meter
        /// </summary>
        float FWRevision
        {
            get;
        }

        /// <summary>
        /// Property used to get the serial number (string) from the meter
        /// </summary>
        string SerialNumber
        {
            get;
        }

        /// <summary>
        /// Property used to get the DST enabled flag (bool) from the meter
        /// </summary>
        bool DSTEnabled
        {
            get;
        }

        /// <summary>
        /// TOU is considered enabled if the clock is running and the meter
        /// is configured to follow a TOU schedule.  TOU does not have to be
        /// running for this property to return true.  For example an expired
        /// TOU schedule is enabled but not running.
        /// </summary>
        bool TOUEnabled
        {
            get;
        }

        /// <summary>
        /// This property returns a list of user data strings.  If the meter has 3 user data fields
        /// then the list will contain 3 strings corresponding to each user data  field
        /// </summary>
        List<String> UserData
        {
            get;
        }

        /// <summary>
        /// Gets the Cold Load Pickup Time in minutes
        /// </summary>
        uint ColdLoadPickupTime
        {
            get;
        }

        /// <summary>
        /// Gets the Interval Length for Demands
        /// </summary>
        int DemandIntervalLength
        {
            get;
        }

        /// <summary>
        /// Gets the Number of Sub Intervals for Demands
        /// </summary>
        int NumberOfSubIntervals
        {
            get;
        }

        /// <summary>
        /// Gets the Number of Test Mode Sub Intervals for Demands
        /// </summary>
        int NumberOfTestModeSubIntervals
        {
            get;
        }

        /// <summary>
        /// Gets the Test Mode Interval Length for Demands
        /// </summary>
        int TestModeIntervalLength
        {
            get;
        }

        /// <summary>
        /// Returns the number of minutes per load profile interval
        /// </summary>
        int LPIntervalLength
        {
            get;
        }

        /// <summary>
        /// Returns the number of load profile channels the meter is 
        /// currently recording
        /// </summary>
        int NumberLPChannels
        {
            get;
        }
    }
}
