using System;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Itron.Ami.Facade.WebServices.Internal.ClientProxy;
using Itron.Ami.CEWebServiceClient.Base;

namespace Itron.Ami.CEWebServiceClient.Internal
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    internal class InternalAPIManagerCallbackService : SubscriberService<InternalAPICallbackItem>, Itron.Ami.Facade.WebServices.Internal.ClientProxy.IInternalAPIManagerCallback
    {
        #region InternalAPIManagerCallbackService Members

        //CE callback
        void Itron.Ami.Facade.WebServices.Internal.ClientProxy.IInternalAPIManagerCallback.CallbackResult(string requestId, Itron.Ami.Facade.WebServices.Internal.ClientProxy.Result result, System.Nullable<int> jobKey)
        {
            base.DataArrived(new InternalAPICallbackItem(requestId, result, jobKey));
        }

        //Internal mock callback
        internal void DataArrived(string requestId, Itron.Ami.Facade.WebServices.Internal.ClientProxy.Result result, System.Nullable<int> jobKey)
        {
            base.DataArrived(new InternalAPICallbackItem(requestId, result, jobKey));
        }
        #endregion
    }
}
