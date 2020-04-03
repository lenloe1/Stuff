using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace Itron.Metering.ZigBeeRadioServerObjects
{
    /// <summary>
    /// CallBack class that handles the call backs from the ZigBee Radio manager.
    /// </summary>

    [CallbackBehavior(ConcurrencyMode=ConcurrencyMode.Single)]
    public class ZigBeeRadioCallBack : IZigBeeRadioCallBack
    {
        #region Public Events

        /// <summary>
        /// Event that is raised when a new network scan is available.
        /// </summary>
        public event ZigBeeRadioScannedEvent NetworkScanned;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/22/08 RCG 1.00           Created

        public ZigBeeRadioCallBack()
        {
        }

        /// <summary>
        /// Notifies the client that a new scan is available.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/22/08 RCG 1.00           Created

        public void NotifyNetworkScanned(List<ZigBeeDevice> Devices)
        {
            OnNetworkScanned(new ZigBeeRadioScannedEventArgs(Devices));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Raises the NetworkScanned event.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/22/08 RCG 1.00           Created

        private void OnNetworkScanned(ZigBeeRadioScannedEventArgs e)
        {
            if (NetworkScanned != null)
            {
                NetworkScanned(this, e);
            }
        }

        #endregion
    }
}
