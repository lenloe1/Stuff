using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy;
using Itron.Ami.CEWebServiceClient.Base;

namespace Itron.Ami.CEWebServiceClient.External
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class EventSubscriberService : SubscriberService<EventSubscriberItem>, Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy.EventSubscriberService
    {
        #region EventSubscriberService Members

        void Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy.EventSubscriberService.EventsArrived(EventsArrivedInput input)
        {
            base.DataArrived(new EventSubscriberItem(input));
        }

        #endregion
    }
}
