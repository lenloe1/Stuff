using System;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;

using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Control.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{

    public class Ping : ExternalAction
    {
        Client<ControlServiceClient> _client = null;
        PingByEndpointsRequest _requestPing = null;


        public Ping(string[] target)
        {
            this.CreateRequest(target);
        }

        private void CreateRequest(string[] target)
        {
            this._requestPing = new PingByEndpointsRequest();
            this._requestPing.EndpointCollectionRequest = new EndpointCollectionRequest();
            this._requestPing.EndpointCollectionRequest.ElectronicSerialNumbers = target;
        }

        internal override void Invoke()
        {
            if (string.IsNullOrEmpty(UniqueID))
            {
                this._client = ExternalClientFactory.ConstructControlServiceClient(ExternalLib.Endpoint.ControlEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);
                this._rqstToken = this._client.ServiceClient.PingByEndpoints(_requestPing);
                bool blTryAgain = true;
                int retryCount = 4;
                _uniqueID = _rqstToken.Id.ToString();
                while (blTryAgain)
                {
                    if (retryCount == 0)
                    {
                        blTryAgain = false;
                    }
                    try
                    {
                        Thread.Sleep(60000);
                        this._rqstResult = this._client.ServiceClient.GetPingByEndpointsResult(this._rqstToken);
                        if (this._rqstResult != null)
                        {
                            blTryAgain = false;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception from DisconnectMeter: " + e);
                        retryCount--;
                    }
                    if (this._rqstResult == null)
                    {
                        retReslt = "Unknown";
                    }
                    else
                    {
                        retReslt = this._rqstResult.Result.ToString();
                    }

                }            
                this._client.ServiceClient.Close();

                
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
        #region Public Properties

        public string retReslt;

        #endregion

        #region Private Properties

        private RequestToken _rqstToken = null;
        private EndpointCollectionRequestResult _rqstResult = null;

        #endregion
   }

}
