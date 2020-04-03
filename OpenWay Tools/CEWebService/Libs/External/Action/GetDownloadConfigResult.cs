using System;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Itron.Ami.Facade.WebServices.Control.V200810.ClientProxy;
using Itron.Ami.CEWebServiceClient.Base;
using CommonClientProxy = Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    public class GetDownloadConfigurationResult : ExternalAction
    {
        Client<ControlServiceClient> _client = null;
        private GroupRequestResult _result;

        internal GetDownloadConfigurationResult(string strUniqueID)
        {
            _uniqueID = strUniqueID;
        }

        internal override void Invoke()
        {
            this._client = ExternalClientFactory.ConstructControlServiceClient(ExternalLib.Endpoint.ControlEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);
                
            RequestToken Token = new RequestToken();
            Token.Id = new Guid(_uniqueID);

            this._result = this._client.ServiceClient.GetDownloadConfigurationByGroupResult(Token);

            this._client.ServiceClient.Close();
        }

        
        internal override void Abort()
        {
            if ((null != this._client) &&
                (null != this._client.ServiceClient) &&
                (this._client.ServiceClient.State != CommunicationState.Closed))
            {
                this._aborted = true;
                _client.ServiceClient.Abort();
            }
        }

        public GroupRequestResult Result
        {
            get
            {
                return _result;
            }
        }
        
    }

}
