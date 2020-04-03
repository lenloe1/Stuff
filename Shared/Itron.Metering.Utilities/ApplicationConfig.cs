using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Class used for setting and retrieving application settings stored in the app.config file
    /// </summary>
    public class ApplicationConfig : ConfigurationSection
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/15/08 RCG 1.51.01		Created

        public ApplicationConfig()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the ZigBeeScanning configuration data
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/15/08 RCG 1.51.01		Created

        [ConfigurationProperty("ZigBeeScanning", IsRequired = false)]
        public ZigBeeScanningConfig ZigBeeScanning
        {
            get
            {
                return (ZigBeeScanningConfig)this["ZigBeeScanning"];
            }
            set
            {
                this["ZigBeeScanning"] = value;
            }
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class ZigBeeScanningConfig : ConfigurationElement
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/15/08 RCG 1.51.01		Created

        public ZigBeeScanningConfig()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the scan duration for ZigBee in number of scans
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/15/08 RCG 1.51.01		Created

        [ConfigurationProperty("Duration", DefaultValue = 4, IsRequired = true)]
        public int Duration
        {
            get
            {
                return (int)this["Duration"];
            }
            set
            {
                this["Duration"] = value;
            }
        }

        #endregion
    }
}
