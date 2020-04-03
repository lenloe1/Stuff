using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Interface that defines the configuration items unique to the CENTRON AMI
    /// </summary>
    public interface ICentronAMIDeviceConfiguration
    {
        /// <summary>
        /// Determines if User Intervention is required after a load limiting disconnect
        /// </summary>	
        string LoadLimitingConnectWithoutUserIntervetion
        {
            get;
        }

        /// <summary>
        /// Determines if Load Control is enabled and what the Threshold is if it is enabled
        /// </summary>
        string LoadControlDisconnectThreshold
        {
            get;
        }

        /// <summary>
        /// Gets the configured daily self read time.
        /// </summary>
        string DailySelfReadTime
        {
            get;
        }
    }
}
