using System;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Itron.Ami.Facade.WebServices.Data.V200810.ClientProxy;
using Itron.Ami.CEWebServiceClient.Base;
using CommonClientProxy = Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    public class ExternalInteractiveRead : ExternalAction
    {   
        private Client<DataServiceClient> _client = null; 
        private InteractiveReadByEndpointRequest _epRequest;
        private RequestToken _rqstToken = null;

        internal ExternalInteractiveRead(
            string strTarget,
            DateTime readingStartTime,
            DateTime readingEndTime,
            bool performDemandReset,
            bool retrieveHomeNetworkData,
            bool retrieveInstantaneousData,
            bool retrieveLastDemandReset,
            bool retrieveLoadProfileData,
            bool retrieveLogEvents,
            bool retrieveNetworkStatistics,
            bool retrievePriorSelfRead,
            bool retrieveVoltageMonitorData,
            bool retrieveBlockStatus,
            bool retrieveCommLogEvents,
            bool retrieveDiagnosticData,
            bool retrieveRecentRegisterData)
        {
            this.CreateRequest(
                strTarget,
                readingStartTime,
                readingEndTime,
                performDemandReset,
                retrieveHomeNetworkData,
                retrieveInstantaneousData,
                retrieveLastDemandReset,
                retrieveLoadProfileData,
                retrieveLogEvents,
                retrieveNetworkStatistics,
                retrievePriorSelfRead,
                retrieveVoltageMonitorData,
                retrieveBlockStatus,
                retrieveCommLogEvents,
                retrieveDiagnosticData,
                retrieveRecentRegisterData);
        }

        private void CreateRequest(string strTarget,
            DateTime readingStartTime,
            DateTime readingEndTime,
            bool performDemandReset,
            bool retrieveHomeNetworkData,
            bool retrieveInstantaneousData,
            bool retrieveLastDemandReset,
            bool retrieveLoadProfileData,
            bool retrieveLogEvents,
            bool retrieveNetworkStatistics,
            bool retrievePriorSelfRead,
            bool retrieveVoltageMonitorData,
            bool retrieveBlockStatus,
            bool retrieveCommLogEvents,
            bool retrieveDiagnosticData,
            bool retrieveRecentRegisterData)
        {

            _epRequest = new InteractiveReadByEndpointRequest();
            _epRequest.EndpointRequest = new EndpointRequest();
            _epRequest.EndpointRequest.ElectronicSerialNumber = strTarget;
            _epRequest.EndpointRequest.StatusChangedService = ExternalLib.Listener.RequestStatusChangedController.HttpUri;
            _epRequest.Parameters = new InteractiveReadParameters();
            _epRequest.Parameters.ReadingEndTime = readingEndTime;
            _epRequest.Parameters.ReadingStartTime = readingStartTime;
            _epRequest.Parameters.PerformDemandReset = performDemandReset;
            _epRequest.Parameters.RetrieveCommunicationLog = retrieveCommLogEvents;
            _epRequest.Parameters.RetrieveHomeNetworkData = retrieveHomeNetworkData;
            _epRequest.Parameters.RetrieveInstantaneousData = retrieveInstantaneousData;
            _epRequest.Parameters.RetrieveLastDemandReset = retrieveLastDemandReset;
            _epRequest.Parameters.RetrieveLoadProfileData = retrieveLoadProfileData;
            _epRequest.Parameters.RetrieveLogEvents = retrieveLogEvents;
            _epRequest.Parameters.RetrieveNetworkStatistics = retrieveNetworkStatistics;
            _epRequest.Parameters.RetrievePriorSelfRead = retrievePriorSelfRead;
            _epRequest.Parameters.RetrieveRecentRegisterData = retrieveRecentRegisterData;
            _epRequest.Parameters.RetrieveVoltageMonitorData = retrieveVoltageMonitorData;
        }

        internal override void Invoke()
        {
            this._client = ExternalClientFactory.ConstructDataServiceClient(ExternalLib.Endpoint.DataEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);

            this._rqstToken = this._client.ServiceClient.InteractiveReadByEndpoint(this._epRequest);
            

            this._client.ServiceClient.Close();

            this._uniqueID = this._rqstToken.Id.ToString();
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
        
    }

}
