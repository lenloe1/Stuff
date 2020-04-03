using System;
using System.Linq;
using System.Text;

using Itron.Ami.CEWebServiceClient.Base;

namespace Itron.Ami.CEWebServiceClient.Internal
{
    public class InternalAPIController
    {
        private InternalAPICallbackHost _host;

        internal InternalAPIController()
        {
        }

        public void StartService()
        {
            if (null == this._host)
                this._host = new InternalAPICallbackHost();

            if (false == this._host.IsRunning)
                this._host.StartService();
        }

        public void StopService()
        {
            if (null != this._host)
            {
                this._host.StopService();
                this._host = null;
            }
        }

        public Uri HttpUri
        {
            get { return this._host.HttpUri; }
        }

        public void AddHandler(EventHandler<SubscriberDataEventArgs<InternalAPICallbackItem>> eventHandler)
        {
            if (null != this._host)
            {
                this._host.Service.CallbackOccurred += eventHandler;
            
            }
        }

        public void RemoveHandler(EventHandler<SubscriberDataEventArgs<InternalAPICallbackItem>> eventHandler)
        {
            if (null != this._host)
            {
                this._host.Service.CallbackOccurred -= eventHandler;

            }
        }

        internal void DataArrived(string requestId, Itron.Ami.Facade.WebServices.Internal.ClientProxy.Result result, System.Nullable<int> jobKey)
        {
            if (null != this._host)
            {
                this._host.Service.DataArrived(requestId, result, jobKey);
            }
        }
    }
}
