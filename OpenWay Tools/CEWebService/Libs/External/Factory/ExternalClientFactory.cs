using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Configuration;

using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy;
using Itron.Ami.Facade.WebServices.Control.V200810.ClientProxy;
using Itron.Ami.Facade.WebServices.Data.V200810.ClientProxy;
using Itron.Ami.Facade.WebServices.Han.V200810.ClientProxy;
using Itron.Ami.Facade.WebServices.Membership.V200810.ClientProxy;
using Itron.Ami.Facade.WebServices.Provisioning.V200908.ClientProxy;
using Itron.Ami.Facade.WebServices.Reporting.V200908.ClientProxy;
using Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy;
using Itron.Ami.Facade.WebServices.Security.V200912.ClientProxy;

using ReportingV200810ClientProxy = Itron.Ami.Facade.WebServices.Reporting.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    internal static class ExternalClientFactory
    {
        private static Dictionary<string, List<ChannelEndpointElement>> _configDictionary = ClientFactory.ReadClientConfigurations(GetServiceContractList());
        private static EventHandler _configChangedEventHandler;

        #region SetClientEndpointAddress Method ----
        public static bool SetClientEndpointAddress(string clientEndpointName, string address)
        {
            return ClientFactory.SetClientEndpointAddress(_configDictionary, clientEndpointName, address);
        }
        #endregion SetClientEndpointAddress Method

        #region GetServiceContractList Method ----
        internal static List<Type> GetServiceContractList()
        {
            List<Type> list = new List<Type>();
            list.Add(typeof(RequestService));
            list.Add(typeof(ControlService));
            list.Add(typeof(DataService));
            list.Add(typeof(HanService));
            list.Add(typeof(EndpointMembershipService));
            list.Add(typeof(ProvisioningService));
            list.Add(typeof(ReportService));
            list.Add(typeof(ReportingV200810ClientProxy.IReportService));
            list.Add(typeof(SubscriptionService));
            list.Add(typeof(OpticalSignedAuthorizationService));
            return list;
        }
        #endregion GetServiceContractList Method

        #region ConfigurationChanged Event ----
        internal static event EventHandler ConfigurationChanged
        {
            add { _configChangedEventHandler += value; }
            remove { _configChangedEventHandler -= value; }
        }
        #endregion ConfigurationChanged Event

        #region GetClientConfigurationNames<T> Method ----
        internal static List<ChannelEndpointElement> GetClientConfigurationNames<T>()
        {
            return ClientFactory.GetClientConfigurationNames<T>(_configDictionary);
        }
        #endregion GetClientConfigurationNames<T> Method

        #region ConstructRequestServiceClient Method ----
        internal static Client<RequestServiceClient> ConstructRequestServiceClient(string endpointConfigName, string userName, string userPassword)
        {
            RequestServiceClient client = null;
            ChannelEndpointElement channelEE = _configDictionary[typeof(RequestService).FullName].Find(delegate(ChannelEndpointElement cee) { return cee.Name == endpointConfigName; });

            if (null != channelEE)
            {
                client = new RequestServiceClient(endpointConfigName, channelEE.Address.ToString());
                client.ClientCredentials.UserName.UserName = userName;
                client.ClientCredentials.UserName.Password = userPassword;
            }

            return ClientFactory.CreateClient<RequestServiceClient, RequestService>(client);
        }
        #endregion ConstructRequestServiceClient Method

        #region ConstructProvisioningServiceClient Method ----
        internal static Client<ProvisioningServiceClient> ConstructProvisioningServiceClient(string endpointConfigName, string userName, string userPassword)
        {
            ProvisioningServiceClient client = null;
            ChannelEndpointElement channelEE = _configDictionary[typeof(ProvisioningService).FullName].Find(delegate(ChannelEndpointElement cee) { return cee.Name == endpointConfigName; });

            if (null != channelEE)
            {
                client = new ProvisioningServiceClient(endpointConfigName, channelEE.Address.ToString());
                client.ClientCredentials.UserName.UserName = userName;
                client.ClientCredentials.UserName.Password = userPassword;
            }

            return ClientFactory.CreateClient<ProvisioningServiceClient, ProvisioningService>(client);
        }
        #endregion ConstructProvisioningServiceClient Method

        #region ConstructEndpointMembershipServiceClient Method ----
        internal static Client<EndpointMembershipServiceClient> ConstructEndpointMembershipServiceClient(string endpointConfigName, string userName, string userPassword)
        {
            EndpointMembershipServiceClient client = null;
            ChannelEndpointElement channelEE = _configDictionary[typeof(EndpointMembershipService).FullName].Find(delegate(ChannelEndpointElement cee) { return cee.Name == endpointConfigName; });

            if (null != channelEE)
            {
                client = new EndpointMembershipServiceClient(endpointConfigName, channelEE.Address.ToString());
                client.ClientCredentials.UserName.UserName = userName;
                client.ClientCredentials.UserName.Password = userPassword;
            }

            return ClientFactory.CreateClient<EndpointMembershipServiceClient, EndpointMembershipService>(client);
        }
        #endregion ConstructEndpointMembershipServiceClient Method

        #region ConstructControlServiceClient Method ----
        internal static Client<ControlServiceClient> ConstructControlServiceClient(string endpointConfigName, string userName, string userPassword)
        {
            ControlServiceClient client = null;
            ChannelEndpointElement channelEE = _configDictionary[typeof(ControlService).FullName].Find(delegate(ChannelEndpointElement cee) { return cee.Name == endpointConfigName; });

            if (null != channelEE)
            {
                client = new ControlServiceClient(endpointConfigName, channelEE.Address.ToString());
                client.ClientCredentials.UserName.UserName = userName;
                client.ClientCredentials.UserName.Password = userPassword;
            }

            return ClientFactory.CreateClient<ControlServiceClient, ControlService>(client);
        }
        #endregion ConstructControlServiceClient Method

        #region ConstructDataServiceClient Method ----
        internal static Client<DataServiceClient> ConstructDataServiceClient(string endpointConfigName, string userName, string userPassword)
        {
            DataServiceClient client = null;
            ChannelEndpointElement channelEE = _configDictionary[typeof(DataService).FullName].Find(delegate(ChannelEndpointElement cee) { return cee.Name == endpointConfigName; });

            if (null != channelEE)
            {
                client = new DataServiceClient(endpointConfigName, channelEE.Address.ToString());
                client.ClientCredentials.UserName.UserName = userName;
                client.ClientCredentials.UserName.Password = userPassword;
             }

            return ClientFactory.CreateClient<DataServiceClient, DataService>(client);
        }
        #endregion ConstructDataServiceClient Method

        #region ConstructHanServiceClient Method ----
        internal static Client<HanServiceClient> ConstructHanServiceClient(string endpointConfigName, string userName, string userPassword)
        {
            HanServiceClient client = null;
            ChannelEndpointElement channelEE = _configDictionary[typeof(HanService).FullName].Find(delegate(ChannelEndpointElement cee) { return cee.Name == endpointConfigName; });

            if (null != channelEE)
            {
                client = new HanServiceClient(endpointConfigName, channelEE.Address.ToString());
                client.ClientCredentials.UserName.UserName = userName;
                client.ClientCredentials.UserName.Password = userPassword;
            }

            return ClientFactory.CreateClient<HanServiceClient, HanService>(client);
        }
        #endregion ConstructHanServiceClient Method

        #region OpticalSignedAuthorizationService Method ----
        internal static Client<OpticalSignedAuthorizationServiceClient> ConstructOpticalSignedAuthorizationService(string endpointConfigName, string userName, string userPassword)
        {
            OpticalSignedAuthorizationServiceClient client = null;
            ChannelEndpointElement channelEE = _configDictionary[typeof(OpticalSignedAuthorizationService).FullName].Find(delegate(ChannelEndpointElement cee) { return cee.Name == endpointConfigName; });

            if (null != channelEE)
            {
                client = new OpticalSignedAuthorizationServiceClient(endpointConfigName, channelEE.Address.ToString());
                client.ClientCredentials.UserName.UserName = userName;
                client.ClientCredentials.UserName.Password = userPassword;
            }

            return ClientFactory.CreateClient<OpticalSignedAuthorizationServiceClient, OpticalSignedAuthorizationService>(client);
        }
        #endregion ConstructSubscriptionServiceClient Method

        #region ConstructSubscriptionServiceClient Method ----
        internal static Client<SubscriptionServiceClient> ConstructSubscriptionServiceClient(string endpointConfigName, string userName, string userPassword)
        {
            SubscriptionServiceClient client = null;
            ChannelEndpointElement channelEE = _configDictionary[typeof(SubscriptionService).FullName].Find(delegate(ChannelEndpointElement cee) { return cee.Name == endpointConfigName; });

            if (null != channelEE)
            {
                client = new SubscriptionServiceClient(endpointConfigName, channelEE.Address.ToString());
                client.ClientCredentials.UserName.UserName = userName;
                client.ClientCredentials.UserName.Password = userPassword;
            }

            return ClientFactory.CreateClient<SubscriptionServiceClient, SubscriptionService>(client);
        }
        #endregion ConstructSubscriptionServiceClient Method

        #region ConstructReportServiceClientV200810 Method ----
        internal static ReportingV200810ClientProxy.ReportServiceClient ConstructReportServiceClientV200810(string endpointConfigName, string userName, string userPassword)
        {
            ReportingV200810ClientProxy.ReportServiceClient client = null;
            ChannelEndpointElement channelEE = _configDictionary[typeof(ReportingV200810ClientProxy.IReportService).FullName].Find(delegate(ChannelEndpointElement cee) { return cee.Name == endpointConfigName; });

            if (null != channelEE)
            {
                client = new ReportingV200810ClientProxy.ReportServiceClient(endpointConfigName, channelEE.Address.ToString());
                client.ClientCredentials.UserName.UserName = userName;
                client.ClientCredentials.UserName.Password = userPassword;
            }

            return client;
        }
        #endregion ConstructReportServiceClientV200810 Method

        #region ConstructReportServiceClient Method ----
        internal static Client<ReportServiceClient> ConstructReportServiceClient(string endpointConfigName, string userName, string userPassword)
        {
            ReportServiceClient client = null;
            ChannelEndpointElement channelEE = _configDictionary[typeof(ReportService).FullName].Find(delegate(ChannelEndpointElement cee) { return cee.Name == endpointConfigName; });

            if (null != channelEE)
            {
                client = new ReportServiceClient(endpointConfigName, channelEE.Address.ToString());
                client.ClientCredentials.UserName.UserName = userName;
                client.ClientCredentials.UserName.Password = userPassword;
            }

            return ClientFactory.CreateClient<ReportServiceClient, ReportService>(client);
        }
        #endregion ConstructReportServiceClient Method
    }
}
