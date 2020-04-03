using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Itron.Ami.Facade.WebServices.Data.V200810.ClientProxy;
using Itron.Ami.CEWebServiceClient.Base;

namespace Itron.Ami.CEWebServiceClient.External
{
    internal abstract class SubscriberControllerBase<THost, TService, KContract, TEventArgs>
        where THost : ServiceHost<TService, KContract>, new()
        where TService : SubscriberService<TEventArgs>
    {
        private THost _host;
        private EventHandler<SubscriberDataEventArgs<TEventArgs>> _eventHandler;

        internal SubscriberControllerBase()
        {
        }

        public void SubscriberControl_StartService(object sender, EventArgs e)
        {
            this.StartService();
        }

        private void SubscriberControl_StopService(object sender, EventArgs e)
        {
            this.StopService();
        }

        private void StartService()
        {
            this._host = new THost();
            this._host.Service.CallbackOccurred += this._eventHandler;
            this._host.StartService();
        }

        internal void StopService()
        {
            if (null != this._host)
            {
                this._host.Service.CallbackOccurred -= this._eventHandler;
                this._host.StopService();

                this._eventHandler = null;
                this._host = null;

                try
                {
                }
                catch (CommunicationException )
                {
                }
                catch (InvalidMessageContractException )
                {
                }
                catch (QuotaExceededException )
                {
                }
            }
        }
    }
}
