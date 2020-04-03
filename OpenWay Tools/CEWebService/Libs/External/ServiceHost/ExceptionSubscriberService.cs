using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy;
using Itron.Ami.CEWebServiceClient.Base;

namespace Itron.Ami.CEWebServiceClient.External
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ExceptionSubscriberService : SubscriberService<ExceptionSubscriberItem>, Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy.ExceptionSubscriberService
    {
        #region ExceptionSubscriberService Members

        void Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy.ExceptionSubscriberService.ExceptionsArrived(ExceptionsArrivedInput input)
        {
            base.DataArrived(new ExceptionSubscriberItem(input));
        }

        #endregion
    }
}
