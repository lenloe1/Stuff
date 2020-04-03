using System;
using System.Text;

using Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy;
using Itron.Ami.CEWebServiceClient.Base;

using IRequestStatusChangedService = Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy.RequestStatusChangedService;

namespace Itron.Ami.CEWebServiceClient.External
{
    internal class RequestStatusChangedHost : ServiceHost<RequestStatusChangedService, IRequestStatusChangedService>
    {
        internal RequestStatusChangedHost() : base(new RequestStatusChangedService()) { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal RequestStatusChangedHost(Uri endpointAddress) : base(new RequestStatusChangedService(), endpointAddress) { }
    }
}
