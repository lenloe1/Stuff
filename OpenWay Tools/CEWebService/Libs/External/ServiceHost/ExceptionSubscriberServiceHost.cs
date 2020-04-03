using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.ServiceModel;

using Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy;
using IExceptionSubscriberService = Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy.ExceptionSubscriberService;
using Itron.Ami.CEWebServiceClient.Base; 

namespace Itron.Ami.CEWebServiceClient.External
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This class is actually instantiated but it's done so by the SubscriberControllerBase class which creates the instance via a generic type reference.")]
    internal class ExceptionSubscriberHost : ServiceHost<ExceptionSubscriberService, IExceptionSubscriberService>
    {
        public ExceptionSubscriberHost() : base(new ExceptionSubscriberService()) { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionSubscriberHost(Uri endpointAddress) : base(new ExceptionSubscriberService(), endpointAddress) { }
    }
}
