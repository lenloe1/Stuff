using System;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;
using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Control.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{

    public class DisconnectMeter : ExternalAction
    {
        Client<ControlServiceClient> _client = null;
        DisconnectMeterRequest _requestDisCon = null;       

        public DisconnectMeter(string retESN)
        {
            this.CreateRequest(retESN);
        }

        private void CreateRequest(string retESN)
        {
            this._requestDisCon = new DisconnectMeterRequest();
            this._requestDisCon.MeterRequest = new EndpointRequest();
            this._requestDisCon.MeterRequest.ElectronicSerialNumber = retESN;
            this._requestDisCon.ExtensionData = null;
            this._requestDisCon.Metadata = null;
        }

        internal override void Invoke()
        {
            this._client = ExternalClientFactory.ConstructControlServiceClient(ExternalLib.Endpoint.ControlEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);
            this._rqstToken = this._client.ServiceClient.DisconnectMeter(_requestDisCon);
            bool blTryAgain = true;
            int retryCount = 6;
            while (blTryAgain)
            {
                if (retryCount == 0)
                {
                    blTryAgain = false;
                }
                try
                {
                    Thread.Sleep(60000);
                    this._rqstResult = this._client.ServiceClient.GetDisconnectMeterResult(this._rqstToken); 
                    if (this._rqstResult != null)
                    {
                        blTryAgain = false;
                    }
                }
                catch (Exception  e)
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
        private EndpointRequestResult _rqstResult = null;

        #endregion
   }
}
