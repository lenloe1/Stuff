using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.ServiceModel;

using Itron.Ami.Facade.WebServices.Data.V200810.ClientProxy;
using IDataSubscriberService = Itron.Ami.Facade.WebServices.Data.V200810.ClientProxy.DataSubscriberService;
using Itron.Ami.CEWebServiceClient.Base; 

namespace Itron.Ami.CEWebServiceClient.External
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This class is actually instantiated but it's done so by the SubscriberControllerBase class which creates the instance via a generic type reference.")]
    internal class DataSubscriberHost : ServiceHost<DataSubscriberService, IDataSubscriberService>
    {
        public DataSubscriberHost() : base(new DataSubscriberService()) { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal DataSubscriberHost(Uri endpointAddress) : base(new DataSubscriberService(), endpointAddress) { }
    }
}
