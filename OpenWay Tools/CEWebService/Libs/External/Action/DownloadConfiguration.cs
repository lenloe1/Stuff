using System;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Control.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{

    public class DownloadConfiguration : ExternalAction
    {
        Client<ControlServiceClient> _client = null;
        DownloadConfigurationByGroupRequest _requestDownloadConfig = null;
        private RequestToken _rqstToken = null;

        internal DownloadConfiguration(string target)
        {
            this.CreateRequest(target);
        }

        private void CreateRequest(string target)
        {
            this._requestDownloadConfig = new DownloadConfigurationByGroupRequest();
            this._requestDownloadConfig.GroupRequest = new GroupRequest();
            this._requestDownloadConfig.GroupRequest.GroupName = target;
            this._requestDownloadConfig.GroupRequest.StatusChangedService = ExternalLib.Listener.RequestStatusChangedController.HttpUri;
        }

        internal override void Invoke()
        {
            if (string.IsNullOrEmpty(UniqueID))
            {
                this._client = ExternalClientFactory.ConstructControlServiceClient(ExternalLib.Endpoint.ControlEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);
                this._rqstToken = this._client.ServiceClient.DownloadConfigurationByGroup(_requestDownloadConfig);
                this._client.ServiceClient.Close();

                _uniqueID = _rqstToken.Id.ToString();
            }
        }

        internal override void Abort()
        {
            if ((null != this._client) &&
                (null != this._client.ServiceClient) &&
                (this._client.ServiceClient.State != CommunicationState.Closed))
            {
                this._aborted = true;
                this._client.ServiceClient.Abort();
            }
        }
   }
}
