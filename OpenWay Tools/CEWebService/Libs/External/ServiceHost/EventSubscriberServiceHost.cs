using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.ServiceModel;

using Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy;
using IEventSubscriberService = Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy.EventSubscriberService;
using Itron.Ami.CEWebServiceClient.Base; 

namespace Itron.Ami.CEWebServiceClient.External
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This class is actually instantiated but it's done so by the SubscriberControllerBase class which creates the instance via a generic type reference.")]
    internal class EventSubscriberHost : ServiceHost<EventSubscriberService, IEventSubscriberService>
    {
        public EventSubscriberHost() : base(new EventSubscriberService()) { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal EventSubscriberHost(Uri endpointAddress) : base(new EventSubscriberService(), endpointAddress) { }
    }
}
