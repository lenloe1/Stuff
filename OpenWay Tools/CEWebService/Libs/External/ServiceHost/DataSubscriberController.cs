using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel; 

using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    public class DataSubscriberController
    {
        private DataSubscriberHost _host;
        string _systemName; 
        Uri _systemUri; 

        internal DataSubscriberController()
        {
        }

        public bool IsRunning
        {
            get { return this._host.IsRunning; }
        }

        public void StartService()
        {
            this._host = new DataSubscriberHost();
            this._host.StartService();

            _systemName = "CEWEBSERVICECLIENT_" + ServiceListener.HostName;
            _systemUri = this._host.HttpUri;

            Subscribe();
        }

        public void StopService()
        {
            if (null != this._host)
            {
                this._host.StopService();
                this._host = null;
            }

            if ((null != this._systemName) && (null != this._systemUri))
            {
                Unsubscribe();
            }
        }

        internal Uri callbackUri
        {
            get { return this._host.HttpUri; }
        }

        public void AddHandler(EventHandler<SubscriberDataEventArgs<DataSubscriberItem>> eventHandler)
        {
            if (null != this._host)
            {
                this._host.Service.CallbackOccurred += eventHandler;
            
            }
        }

        public void RemoveHandler(EventHandler<SubscriberDataEventArgs<DataSubscriberItem>> eventHandler)
        {
            if (null != this._host)
            {
                this._host.Service.CallbackOccurred -= eventHandler;

            }
        }

        private void Unsubscribe()
        {
            Client<SubscriptionServiceClient> c = null;
            try
            {
                c = ExternalClientFactory.ConstructSubscriptionServiceClient(ExternalLib.Endpoint.SubscriptionEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);

                CancelDataSubscriptionRequest CancelReq = new CancelDataSubscriptionRequest();
                CancelReq.SystemName = this._systemName;

                c.ServiceClient.CancelDataSubscription(CancelReq);
                c.ServiceClient.Close();
            }
            catch (FaultException<Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy.SubscriberEndpointNotFoundFault>)
            {
            }
            catch (FaultException<Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy.SubscriberNotFoundFault>)
            {
            }
            finally
            {
                if ((null != c) && (null != c.ServiceClient) &&
                    (c.ServiceClient.State != CommunicationState.Closed))
                    c.ServiceClient.Close();
            }
        }

        private void Subscribe()
        {
            Client<SubscriptionServiceClient> c = null;
            SubscribeToEndpointDataRequest SubscribeEndpointRequest = null;
            SubscriptionRequest SubscriptionRequest = null;

            try
            {
                c = ExternalClientFactory.ConstructSubscriptionServiceClient(ExternalLib.Endpoint.SubscriptionEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);

                SubscribeEndpointRequest = new SubscribeToEndpointDataRequest();
                SubscriptionRequest = new SubscriptionRequest();
                SubscriptionRequest.SystemName = this._systemName;
                SubscriptionRequest.UriCollection = new Uri[] { this._systemUri };
                SubscribeEndpointRequest.SubscriptionRequest = SubscriptionRequest;

                c.ServiceClient.SubscribeToEndpointData(SubscribeEndpointRequest);
                c.ServiceClient.Close();
            }
            catch (FaultException<Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy.DuplicateSubscriberFault>)
            {
            }
            finally
            {
                if ((null != c) && (null != c.ServiceClient) &&
                    (c.ServiceClient.State != CommunicationState.Closed))
                    c.ServiceClient.Close();
            }
        }
    }
}
