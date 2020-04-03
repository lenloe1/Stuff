using System;

using Itron.Ami.Facade.WebServices.Internal.ClientProxy;
using Itron.Ami.CEWebServiceClient.Base;

using IInternalAPICallback = Itron.Ami.Facade.WebServices.Internal.ClientProxy.IInternalAPIManagerCallback;

namespace Itron.Ami.CEWebServiceClient.Internal
{
    internal class InternalAPICallbackHost : ServiceHost<InternalAPIManagerCallbackService, IInternalAPICallback>
    {
        internal InternalAPICallbackHost() : base(new InternalAPIManagerCallbackService()) { }
}

}
