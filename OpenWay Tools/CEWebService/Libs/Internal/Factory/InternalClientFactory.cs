using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;

using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Internal.ClientProxy;
using Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy;
using Itron.Ami.Facade.WebServices.Reporting.V200908.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.Internal
{
    internal static class InternalClientFactory
    {
        private static Dictionary<string, List<ChannelEndpointElement>> _configDictionary = ClientFactory.ReadClientConfigurations(GetServiceContractList());
        private static EventHandler _configChangedEventHandler;

        #region SetClientEndpointAddress Method ----
        public static bool SetClientEndpointAddress(string clientEndpointName, string address)
        {
            return ClientFactory.SetClientEndpointAddress(_configDictionary, clientEndpointName, address);
        }
        #endregion SetClientEndpointAddress Method

        #region ConfigurationChanged Event ----
        internal static event EventHandler ConfigurationChanged
        {
            add { _configChangedEventHandler += value; }
            remove { _configChangedEventHandler -= value; }
        }
        #endregion ConfigurationChanged Event

        #region GetServiceContractList Method ----
        internal static List<Type> GetServiceContractList()
        {
            List<Type> list = new List<Type>();
            list.Add(typeof(IInternalAPIManagerService));
            list.Add(typeof(ReportService));
            list.Add(typeof(RequestService)); 
            
            return list;
        }
        #endregion GetServiceContractList Method

        #region ConstructInternalManagerClient Method ----
        internal static Client<InternalAPIManagerServiceClient> ConstructInternalAPIManagerServiceClient(string endpointConfigName, string userName, string userPassword)
        {
            InternalAPIManagerServiceClient client = null;
            ChannelEndpointElement channelEE = _configDictionary[typeof(IInternalAPIManagerService).FullName].Find(delegate(ChannelEndpointElement cee) { return cee.Name == endpointConfigName; });

            if (null != channelEE)
            {
                client = new InternalAPIManagerServiceClient(endpointConfigName, channelEE.Address.ToString());
                client.ClientCredentials.UserName.UserName = userName;
                client.ClientCredentials.UserName.Password = userPassword;
            }

            return ClientFactory.CreateClient<InternalAPIManagerServiceClient, IInternalAPIManagerService>(client);
        }
        #endregion ConstructInternalManagerClient Method

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

        #region GetClientConfigurationNames<T> Method ----
        internal static List<ChannelEndpointElement> GetClientConfigurationNames<T>()
        {
            return ClientFactory.GetClientConfigurationNames<T>(_configDictionary);
        }
        #endregion GetClientConfigurationNames<T> Method
    }
}
