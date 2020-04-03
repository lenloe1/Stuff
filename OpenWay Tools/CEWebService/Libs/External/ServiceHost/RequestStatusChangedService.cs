using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy;
using Itron.Ami.CEWebServiceClient.Base;

namespace Itron.Ami.CEWebServiceClient.External
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    internal class RequestStatusChangedService : SubscriberService<RequestStatusChangedItem>, Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy.RequestStatusChangedService
    {
        #region RequestStatusChangedService Members

        void Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy.RequestStatusChangedService.StatusChanged(StatusChangedInput input)
        {
            base.DataArrived(new RequestStatusChangedItem(input));
        }

        #endregion
    }
}
