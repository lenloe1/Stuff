using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using Itron.Ami.Facade.WebServices.Data.V200810.ClientProxy;
using Itron.Ami.CEWebServiceClient.Base;

namespace Itron.Ami.CEWebServiceClient.External
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DataSubscriberService : SubscriberService<DataSubscriberItem>, Itron.Ami.Facade.WebServices.Data.V200810.ClientProxy.DataSubscriberService
    {
        #region DataSubscriberService Members

        void Itron.Ami.Facade.WebServices.Data.V200810.ClientProxy.DataSubscriberService.DataArrived(DataArrivedInput input)
        {
            base.DataArrived(new DataSubscriberItem(input));
        }

        #endregion
    }
}
