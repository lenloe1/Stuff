using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.ZigBeeRadioServerObjects
{
    /// <summary>
    /// Delegate for the Network Scanned event.
    /// </summary>
    /// <param name="sender">The object that sent the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ZigBeeRadioScannedEvent (object sender, ZigBeeRadioScannedEventArgs e);

    /// <summary>
    /// Event arguments for the network scanned event.
    /// </summary>

    public class ZigBeeRadioScannedEventArgs : EventArgs
    {

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="devices">The list of events seen during the most recent scan.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/28/08 RCG 1.00           Created

        public ZigBeeRadioScannedEventArgs(List<ZigBeeDevice> devices)
            : base()
        {
            m_Devices = devices;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of devices in the scan.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/28/08 RCG 1.00           Created

        public List<ZigBeeDevice> Devices
        {
            get
            {
                return m_Devices;
            }
        }

        #endregion

        #region Member Variables

        private List<ZigBeeDevice> m_Devices;

        #endregion
    }
}
