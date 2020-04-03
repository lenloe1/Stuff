using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.ReplicaSettings;
using Itron.Metering.Utilities;
using Itron.Metering.DeviceDataTypes;
using System.Globalization;

namespace Itron.Metering.Device
{
    /// <summary>
    /// CANSIDevice class - This is the "device server" for the ANSI device.
    /// (IConfiguration implementation)
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    // 01/17/07 jrf 8.00    N/A    Created
    //
    public partial class ANSIMeter : IConfiguration
    {

        #region Methods
        
        /// <summary>
        /// Implements the IConfiguration interface.  Initializes the meter
        /// based on the given program name.
        /// </summary>
        /// <param name="strProgramName">The name of the program to use to 
        /// initialize the meter</param>
        /// <returns>ConfigurationResult</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 01/17/07 jrf 8.00    N/A    Created
        // 01/25/07 jrf 8.00    N/A    Modifed to reflect changes to IntializeDevice(),
        //                             now pass in optical probe and not device ID
        // 02/05/07 jrf 8.00    N/A    Added check for reset billing registers since the 
        //                             call to the device server's initialize was not 
        //                             handling this
        // 02/20/07 jrf 8.00.13        Removed check for reset billing registers 
        // 02/21/07 jrf 8.00.13        Updating the name of the namespace and class
        //                             for calling initialize device.
        // 03/28/07 jrf 8.00.23 2748   Determine from replica setting the reset billing 
        //                             registers on initialization option and pass in 
        //                             call to DeviceWrapper.

        public virtual ConfigurationResult Configure(string strProgramName)
        {
            return ConfigurationResult.ERROR;
        }

        /// <summary>
        /// Configures a device with the specified program and Prompt for data.
        /// </summary>
        /// <param name="sProgramName">Name or path of the program.</param>
        /// <param name="PFData">The prompt for data for the program.</param>
        /// <returns>A ConfigurationError code.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/27/08 RCG 10.00          Created

        public virtual ConfigurationResult Configure(string sProgramName, PromptForData PFData)
        {
            // We currently don't have a prompt for for most devices so just call configure.
            return Configure(sProgramName);
        }

        #endregion
    }
}
