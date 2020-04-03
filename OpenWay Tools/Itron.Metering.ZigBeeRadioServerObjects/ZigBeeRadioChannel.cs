using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Itron.Metering.ZigBeeRadioServerObjects
{

    /// <summary>
    /// Client channel class for the ZigBee radio management service.
    /// </summary>

    public class ZigBeeRadioChannel : DuplexClientBase<IZigBeeRadioService>, IZigBeeRadioService
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CallBack">The client's CallBack object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        public ZigBeeRadioChannel(ZigBeeRadioCallBack CallBack) 
            : base(CallBack)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CallBack">The client's CallBack object.</param>
        /// <param name="endpointConfigurationName">The name of the endpoint configuration to use.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        public ZigBeeRadioChannel(ZigBeeRadioCallBack CallBack, string endpointConfigurationName) 
            : base(CallBack, endpointConfigurationName)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="CallBack">The client's CallBack object.</param>
        /// <param name="binding">The binding to use for the connection.</param>
        /// <param name="remoteAddress">The endpoint address for the connection.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        public ZigBeeRadioChannel(ZigBeeRadioCallBack CallBack, Binding binding, EndpointAddress remoteAddress) 
            : base(CallBack, binding, remoteAddress)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CallBack">The client's CallBack object.</param>
        /// <param name="endpointConfigurationName">The name of the endpoint configuration to use.</param>
        /// <param name="remoteAddress">The enpoint address to use for the connection.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        public ZigBeeRadioChannel(ZigBeeRadioCallBack CallBack, string endpointConfigurationName, EndpointAddress remoteAddress) 
            : base(CallBack, endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="CallBack">The client's CallBack object.</param>
        /// <param name="endpointConfigurationName">The name of the configuration to use for the connection.</param>
        /// <param name="remoteAddress">The enpoint address to use for the connection.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        public ZigBeeRadioChannel(ZigBeeRadioCallBack CallBack, string endpointConfigurationName, string remoteAddress) 
            : base(CallBack, endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Subscribes to device scanned events.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/14/08 RCG 1.00           Created

        public void SubscribeToScans()
        {
            base.Channel.SubscribeToScans();
        }

        /// <summary>
        /// Unsubscribe to device scanned events.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/14/08 RCG 1.00           Created

        public void UnsubscribeFromScans()
        {
            base.Channel.UnsubscribeFromScans();
        }

        /// <summary>
        /// Gets whether or not the host is currently scanning for devices.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        public bool IsScanningDevices
        {
            get 
            {
                return base.Channel.IsScanningDevices;
            }
        }

        /// <summary>
        /// Gets the current count of available radios.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        public int AvailableRadioCount
        {
            get 
            {
                return base.Channel.AvailableRadioCount; 
            }
        }

        /// <summary>
        /// Requests a radio from the host.
        /// </summary>
        /// <returns>Null if no radio is available or a token to an available radio.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        public ZigBeeRadioToken RequestZigBeeRadio()
        {
            ZigBeeRadioToken Radio = null;

            for (int iRetries = 0; iRetries < 40; iRetries++)
            {
                Radio = base.Channel.RequestZigBeeRadio();

                if (Radio != null)
                {
                    // We have a radio so we are done
                    break;
                }
                else
                {
                    Thread.Sleep(800);
                }
            }

            return Radio;
        }

        /// <summary>
        /// Releases the specified radio back to the host.
        /// </summary>
        /// <param name="Radio">The radio token to release.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        public void ReleaseZigBeeRadio(ZigBeeRadioToken Radio)
        {
            base.Channel.ReleaseZigBeeRadio(Radio);
        }

        /// <summary>
        /// Gets the list of devices that have been seen during the latest scans.
        /// </summary>
        /// <returns>The list of devices seen.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        public List<ZigBeeDevice> GetVisibleDevices()
        {
            return base.Channel.GetVisibleDevices();
        }

        /// <summary>
        /// Gets a list of all radios that are being managed by the service.
        /// </summary>
        /// <returns>The list of radios.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/14/08 RCG 1.00           Created

        public List<ZigBeeRadioToken> GetRadioInformation()
        {
            return base.Channel.GetRadioInformation();
        }
    }
}
