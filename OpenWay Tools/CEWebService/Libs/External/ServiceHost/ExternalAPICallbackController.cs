using System;
using System.Linq;
using System.Text;

using Itron.Ami.CEWebServiceClient.Base;

namespace Itron.Ami.CEWebServiceClient.External
{
    public class RequestStatusChangedController
    {
        private RequestStatusChangedHost _host;

        internal RequestStatusChangedController()
        {
        }

        public bool IsRunning
        {
            get { return this._host.IsRunning; }
        }

        public void StartService()
        {
            if (null == this._host)
                this._host = new RequestStatusChangedHost();

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

        public void AddHandler(EventHandler<SubscriberDataEventArgs<RequestStatusChangedItem>> eventHandler)
        {
            if (null != this._host)
            {
                this._host.Service.CallbackOccurred += eventHandler;
            
            }
        }

        public void RemoveHandler(EventHandler<SubscriberDataEventArgs<RequestStatusChangedItem>> eventHandler)
        {
            if (null != this._host)
            {
                this._host.Service.CallbackOccurred -= eventHandler;

            }
        }
    }
}
