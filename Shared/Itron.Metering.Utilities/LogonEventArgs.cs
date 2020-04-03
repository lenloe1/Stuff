using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Utilities
{

    /// <summary>
    /// Event arguments for logging on to a meter.
    /// </summary>
    public class LogonEventArgs : EventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bUseZigBeeLogon">Whether or not to log on using ZigBee.</param>
        /// <param name="ulRadioID">The ZigBee Device to log on to.</param>
        /// <param name="uiChannel">The radio frequency channel to use for communication</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/31/08 AF  1.50.13        Created
        //
        public LogonEventArgs(bool bUseZigBeeLogon, ulong ulRadioID, uint uiChannel)
        {
            m_bLogonUsingZigBee = bUseZigBeeLogon;
            m_ulRadioID = ulRadioID; 
            m_uiChannel = uiChannel;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ulRadioID">The ZigBee Device to log on to.</param>
        /// <param name="bUseZigBeeLogon">Whether or not to log on using ZigBee.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //	03/07/08 AF  				Created

        public LogonEventArgs(bool bUseZigBeeLogon, ulong ulRadioID)
        {
            m_bLogonUsingZigBee = bUseZigBeeLogon;
            m_ulRadioID = ulRadioID;  
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //	03/07/08 AF  				Created

        public LogonEventArgs() 
            : this(false, 0) 
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the ID of the ZigBee device to log on to.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //	03/07/08 AF  				Created

        public ulong RadioID
        {
            get
            {
                return m_ulRadioID;
            }
            set
            {
                m_ulRadioID = value;
            }
        }

        /// <summary>
        /// Gets or sets whether or not to log on using ZigBee.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //	03/13/08 RCG 1.50.05		Created

        public bool UseZigBeeLogon
        {
            get
            {
                return m_bLogonUsingZigBee;
            }
            set
            {
                m_bLogonUsingZigBee = value;
            }
        }

        /// <summary>
        /// Gets or sets the radio channel to use
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/31/08 AF  1.50.13        Created
        //
        public uint RadioChannel
        {
            get
            {
                return m_uiChannel;
            }
            set
            {
                m_uiChannel = value;
            }
        }

        #endregion

        #region Member Variables

        private ulong m_ulRadioID;
        private bool m_bLogonUsingZigBee;
        private uint m_uiChannel;

        #endregion

    }
}
