using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Linq;
using System.Text;

namespace Itron.Ami.CEWebServiceClient.Base
{
    public static class ClientFactory
    {

        #region GetClientConfigurationNames<T> Method ----
        public static List<ChannelEndpointElement> GetClientConfigurationNames<T>(Dictionary<string, List<ChannelEndpointElement>> configDictionary)
        {
            List<ChannelEndpointElement> list = null;

            string typeName = typeof(T).FullName;

            if (configDictionary.ContainsKey(typeName))
            {
                list = configDictionary[typeName];
            }

            return list;
        }
        #endregion GetClientConfigurationNames<T> Method

        public static Client<TClient> CreateClient<TClient, TChannel>(TClient client)
            where TClient : ClientBase<TChannel>
            where TChannel : class
        {
            ClientInspectorEndpointBehavior behavior = new ClientInspectorEndpointBehavior();
            client.Endpoint.Behaviors.Add(behavior);

            return new Client<TClient>(client, behavior.Inspector);
        }


        #region ReadClientConfigurations Method ----
        public static Dictionary<string, List<ChannelEndpointElement>> ReadClientConfigurations(List<Type> serviceContractList)
        {
            Dictionary<string, List<ChannelEndpointElement>> dict = new Dictionary<string, List<ChannelEndpointElement>>();

            foreach (Type service in serviceContractList)
            {
                dict.Add(service.FullName, new List<ChannelEndpointElement>());
            }

            try
            {
                Configuration appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if (null != appConfig)
                {
                    ServiceModelSectionGroup serviceModel = ServiceModelSectionGroup.GetSectionGroup(appConfig);

                    if (null != serviceModel)
                    {
                        ClientSection clientSection = serviceModel.Client;

                        if (null != clientSection)
                        {
                            if (null != clientSection.Endpoints)
                            {
                                if (clientSection.Endpoints.Count > 0)
                                {
                                    foreach (ChannelEndpointElement element in clientSection.Endpoints)
                                    {
                                        if (dict.ContainsKey(element.Contract))
                                        {
                                            dict[element.Contract].Add(element);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ConfigurationErrorsException)
            {
                throw;
            }

            return dict;
        }
        #endregion ReadClientConfigurations Method

        #region SetClientEndpointAddress Method ----
        public static bool SetClientEndpointAddress(Dictionary<string, List<ChannelEndpointElement>> dict, string clientEndpointName, string address)
        {
            ChannelEndpointElement channel = null; 

            foreach (List<ChannelEndpointElement> ceeList in dict.Values)
            {
                channel = ceeList.Find(delegate(ChannelEndpointElement cee) { return cee.Name == clientEndpointName; });

                if (null != channel)
                {
                    channel.Address = new Uri(address);
                    break;
                }
            }

            return (null != channel);
        }
        #endregion
    }
}
