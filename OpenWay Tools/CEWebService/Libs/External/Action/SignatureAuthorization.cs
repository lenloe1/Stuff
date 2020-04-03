using System;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Security.V200912.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    public class SignatureAuthorization : ExternalAction
    {
        Client<OpticalSignedAuthorizationServiceClient> _client; 
        RequestSignedAuthorizationRequest _rsaRequest;
        RequestSignedAuthorizationByEndpointRequest _rsaEpRequest; 
        SignedAuthorizationResult _saResult;
        
        public SignedAuthorizationResult Result
        {
            get { return _saResult; }
        }
        
        internal SignatureAuthorization(string serialNumber, 
            Itron.Ami.Facade.WebServices.Security.V200912.ClientProxy.PermissionLevel permissionLevel, 
            TimeSpan validityPeriod)
        {
            if (validityPeriod.Ticks < 0)
                throw new ArgumentOutOfRangeException("validityPeriod");

            if (string.IsNullOrEmpty(serialNumber))
            {
                _rsaRequest = new RequestSignedAuthorizationRequest();
                _rsaRequest.PermissionLevel = permissionLevel;
                _rsaRequest.ValidityPeriod = validityPeriod;
            }
            else
            {
                _rsaEpRequest = new RequestSignedAuthorizationByEndpointRequest();
                _rsaEpRequest.PermissionLevel = permissionLevel;
                _rsaEpRequest.ValidityPeriod = validityPeriod;
                _rsaEpRequest.MfgSerialNumber = serialNumber;
            }
                
        }

        internal override void Invoke()
        {
            
            this._client = ExternalClientFactory.ConstructOpticalSignedAuthorizationService(ExternalLib.Endpoint.SecurityEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);
            
            if (null != _rsaRequest)
                _saResult = this._client.ServiceClient.RequestSignedAuthorization(_rsaRequest);
            
            if (null != _rsaEpRequest)
                _saResult = this._client.ServiceClient.RequestSignedAuthorizationByEndpoint(_rsaEpRequest);
            
            this._client.ServiceClient.Close();
        }

        internal override void Abort()
        {
            if ((null != this._client) &&
                (null != this._client.ServiceClient) &&
                this._client.ServiceClient.State != CommunicationState.Closed)
            {
                this._aborted = true;
                _client.ServiceClient.Abort();
            }
        }

        public override RequestStatusChangedItem Execute(TimeSpan syncWaitTime)
        {
            throw new NotSupportedException();
        }

        public override void Cancel()
        {
            base.Cancel();
        }
   }
}
