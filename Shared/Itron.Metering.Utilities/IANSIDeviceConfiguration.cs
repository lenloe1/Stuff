using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Utilities
{ 
    /// <summary>
    /// Interface that defines Configuration items specified at the ANSI Device level.
    /// </summary>
    public interface IANSIDeviceConfiguration
    {
        /// <summary>Gets the CT Ratio for the current device</summary>
        float CTRatio
        {
            get;
        }

        /// <summary>Gets the VT Ratio for the current device</summary>
        float VTRatio
        {
            get;
        }

        /// <summary>Gets the Register Multiplier for the current device</summary>
        float RegisterMultiplier
        {
            get;
        }

        /// <summary>Gets the Outage Length before Cold Load Pickup in seconds</summary>
        int OutageLength
        {
            get;
        }

        /// <summary>Gets the Display mode timeout in minutes</summary>
        int DisplayModeTimeout
        {
            get;
        }

    }
}
