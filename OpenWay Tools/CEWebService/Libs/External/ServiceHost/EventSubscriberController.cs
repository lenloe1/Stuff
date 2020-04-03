using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel; 

using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    public class EventSubscriberController
    {
        private EventSubscriberHost _host;
        string _systemName; 
        Uri _systemUri; 

        internal EventSubscriberController()
        {
        }

        public bool IsRunning
        {
            get { return this._host.IsRunning; }
        }

        public void StartService()
        {
            if (null == this._host)
                this._host = new EventSubscriberHost();

            if (false == this._host.IsRunning)
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
            
            Unsubscribe();
        }

        internal Uri callbackUri
        {
            get { return this._host.HttpUri; }
        }

        public void AddHandler(EventHandler<SubscriberDataEventArgs<EventSubscriberItem>> eventHandler)
        {
            if (null != this._host)
            {
                this._host.Service.CallbackOccurred += eventHandler;
            
            }
        }

        public void RemoveHandler(EventHandler<SubscriberDataEventArgs<EventSubscriberItem>> eventHandler)
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
                if ((null != this._systemName) && (null != this._systemUri))
                {
                    c = ExternalClientFactory.ConstructSubscriptionServiceClient(ExternalLib.Endpoint.SubscriptionEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);

                    SubscriberRequest req = new SubscriberRequest();
                    req.SystemName = this._systemName;
                    req.Uri = this._systemUri;

                    c.ServiceClient.DeleteEventSubscriberEndpointAddress(req);
                    c.ServiceClient.Close();
                }
            }
            catch (FaultException<Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy.ArgumentFault>)
            {
                // Looks like this is the last endpoint for the service.  So, we'll simply cancel the subscription at this point.

                CancelEventSubscriptionRequest request = new CancelEventSubscriptionRequest();
                request.SystemName = this._systemName;
                c.ServiceClient.CancelEventSubscription(request);
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
            SubscriptionRequest subRequest = null;

            try
            {
                if ((null != this._systemName) && (null != this._systemUri))
                {
                    c = ExternalClientFactory.ConstructSubscriptionServiceClient(ExternalLib.Endpoint.SubscriptionEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);

                    subRequest = new SubscriptionRequest();
                    subRequest.SystemName = this._systemName;
                    subRequest.UriCollection = new Uri[] { this._systemUri };

                    SubscribeToEndpointEventsRequest request = new SubscribeToEndpointEventsRequest();
                    request.SubscriptionRequest = subRequest;

                    c.ServiceClient.SubscribeToEndpointEvents(request);
                    c.ServiceClient.Close();
                }
            }
            catch (FaultException<Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy.DuplicateSubscriberFault>)
            {
                bool duplicate = false;

                // Subscription already exists (i.e. a data subscription with the same system name already exists)
                // we need to determine whether or not we're going to add this endpoint to the subscription.
                try
                {
                    SubscriberRequest req = new SubscriberRequest();
                    req.SystemName = this._systemName;
                    req.Uri = this._systemUri;

                    c.ServiceClient.AddEventSubscriberEndpointAddress(req);
                    c.ServiceClient.Close();
                }
                catch (FaultException<Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy.DuplicateSubscriberEndpointFault>)
                {
                    // Ok, it's clear now that the subscription already exists and that this endpoint address is already a part of that
                    // subscription.  So, we're good to go.
                    duplicate = true;
                }

                if (!duplicate)
                    throw;
            }
            finally
            {
                if ((null != c) && (null!= c.ServiceClient) &&
                    (c.ServiceClient.State != CommunicationState.Closed))
                    c.ServiceClient.Close();
            }
        }
    }
}
