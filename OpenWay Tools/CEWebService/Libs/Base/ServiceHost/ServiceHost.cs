using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Reflection;
using System.Globalization;

namespace Itron.Ami.CEWebServiceClient.Base
{
    public abstract class ServiceHost<TService, KContract>
    {
        private readonly static Uri _defaultUri = new Uri(string.Format(CultureInfo.CurrentCulture, "http://{0}:{1}/{2}/{3}"
                                                                        , ServiceListener.HostName
                                                                        , ServiceListener.Port.ToString(CultureInfo.InvariantCulture)
                                                                        , Assembly.GetExecutingAssembly().GetName().Name
                                                                        , typeof(TService).Name));

        private TService _subscriber;
        private Uri _httpUri;
        private ServiceHost _serviceHost;
        private bool _isRunning;
        private string _serviceName;

        protected ServiceHost(TService service) : this(service, null) { }

        protected ServiceHost(TService service, Uri endpointAddress)
        {
            this._subscriber = service;
            this._serviceName = typeof(TService).Name;
            this._serviceHost = ServiceHost<TService, KContract>.CreateSingletonServiceHost(this._subscriber, endpointAddress);
            this._httpUri = this._serviceHost.Description.Endpoints[0].Address.Uri;
        }

        public void StartService()
        {
            if (!this._isRunning)
            {
                this._serviceHost.Open();
                this._isRunning = true;
            }
        }

        public void StopService()
        {
            if (this._serviceHost.State != CommunicationState.Closed)
            {
                try
                {
                    this._serviceHost.Close();
                }
                catch (CommunicationObjectFaultedException)
                {
                    this._serviceHost.Abort();
                    this._isRunning = false;
                }
            }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public Uri HttpUri
        {
            get { return this._httpUri; }
        }

        internal CommunicationState ServiceState
        {
            get { return this._serviceHost.State; }
        }

        public TService Service
        {
            get { return this._subscriber; }
        }

        internal string ServiceName
        {
            get { return this._serviceName; }
        }

        private static ServiceHost CreateSingletonServiceHost(object singletonInstance, Uri endpointAddress)
        {
            ServiceHost host = new ServiceHost(singletonInstance);

            // See if there are already service endpoints
            ServiceEndpointCollection configFileEndPoints = host.Description.Endpoints;
            if ((configFileEndPoints == null) || (configFileEndPoints.Count == 0))
            {
                ContractDescription cd = ContractDescription.GetContract(typeof(KContract));

                Uri uri = endpointAddress;

                if (null == uri)
                {
                    uri = ServiceHost<TService, KContract>._defaultUri;
                }

                host = new ServiceHost(singletonInstance, uri);

                EndpointAddress epa = new EndpointAddress(uri);

                //Create the service binding
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = 1000000000;  // One Billion (approx 1 GB)
                binding.SendTimeout = TimeSpan.FromMinutes(10);
                binding.ReceiveTimeout = TimeSpan.FromMinutes(10);

                ServiceEndpoint sep = new ServiceEndpoint(cd, binding, epa);

                host.Description.Endpoints.Add(sep);

                // See if there is already throttling behavior
                ServiceThrottlingBehavior stb = host.Description.Behaviors.Find<ServiceThrottlingBehavior>();
                if (stb == null)
                {
                    stb = new ServiceThrottlingBehavior();
                    stb.MaxConcurrentCalls = 1000;
                    stb.MaxConcurrentSessions = 1000;
                    stb.MaxConcurrentInstances = 1000;
                    host.Description.Behaviors.Add(stb);
                }

                // Provide metadata
                ServiceMetadataBehavior smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (null == smb)
                {
                    smb = new ServiceMetadataBehavior();
                    smb.HttpGetEnabled = true;
                    //smb.HttpGetUrl = new Uri(uri.ToString() + "/mex");
                    smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;

                    host.Description.Behaviors.Add(smb);

                    // Add MEX endpoint
                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName
                                            , MetadataExchangeBindings.CreateMexHttpBinding()
                                            , "mex");
                }
            }

            return host;
        }
    }
}
