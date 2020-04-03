using System;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;
using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Control.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{

    public class ReconnectMeter : ExternalAction
    {
        Client<ControlServiceClient> _client = null;  
        ReconnectMeterRequest _requestReCon = null;

        public ReconnectMeter(string retESN, bool bUserInterv)
        {
            this.CreateRequest(retESN, bUserInterv);
        }

        private void CreateRequest(string retESN, bool bUserInterv)
        {
            this._requestReCon = new ReconnectMeterRequest();
            this._requestReCon.MeterRequest = new EndpointRequest();
            this._requestReCon.MeterRequest.ElectronicSerialNumber = retESN;
            this._requestReCon.IsUserInterventionRequired = bUserInterv;
            this._requestReCon.ExtensionData = null;
            this._requestReCon.Metadata = null;
        }

        internal override void Invoke()
        {
            this._client = ExternalClientFactory.ConstructControlServiceClient(ExternalLib.Endpoint.ControlEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);
            this._rqstToken = this._client.ServiceClient.ReconnectMeter(_requestReCon);
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
                    this._rqstResult = this._client.ServiceClient.GetReconnectMeterResult(this._rqstToken);
                    if (this._rqstResult != null)
                    {
                        blTryAgain = false;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception from ReconnectMeter: " + e);
                    retryCount--;
                }
                if (this._rqstResult == null)
                {
                    recReslt = "Unknown";
                }
                else
                {
                    recReslt = this._rqstResult.Result.ToString();
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
       
        public string recReslt;
       
        #endregion
                
        #region Private Properties

        private RequestToken _rqstToken = null;
        private EndpointRequestResult _rqstResult = null;

        #endregion
   }
}
