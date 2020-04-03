using System;
using System.ServiceModel;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;

using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Internal.ClientProxy;
using Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.Internal
{
    public class InteractiveRead : InternalCEAction
    {
        private string _esn;
        private InternalInterrogationParameters _params;
        private Client<InternalAPIManagerServiceClient> _client = null;

        internal InteractiveRead(string esn,
            bool needsFastResponseFromMeter,
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
            : base()
        {

            this._esn = esn;
            this._params = new InternalInterrogationParameters();

            //TODO: not sure the logic is correct across local times and DST.  Needs to be tested.
            //MinUCETime is Jan 1, 1970, midnight
            DateTime minUCETime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            ContextTime contextTime = new ContextTime();
            uce_t ucet = new uce_t();
            TimeSpan timeSpan = new TimeSpan(readingStartTime.Ticks - minUCETime.Ticks);
            ucet.m_SecondsSinceMinUCETime = (int)timeSpan.TotalSeconds;
            contextTime.m_AbsoluteTimeValue = ucet;
            this._params.ReadingStartTime = contextTime;

            timeSpan = new TimeSpan(readingEndTime.Ticks - minUCETime.Ticks);
            contextTime = new ContextTime();
            ucet.m_SecondsSinceMinUCETime = (int)timeSpan.TotalSeconds;
            contextTime.m_AbsoluteTimeValue = ucet;
            this._params.ReadingEndTime = contextTime;

            this._params.NeedsFastResponseFromMeter = needsFastResponseFromMeter;
            this._params.PerformDemandReset = performDemandReset;
            this._params.RetrieveBlockStatus = retrieveBlockStatus;
            this._params.RetrieveCommLogEvents = retrieveCommLogEvents;
            this._params.RetrieveDiagnosticData = retrieveDiagnosticData;
            this._params.RetrieveHomeNetworkData = retrieveHomeNetworkData;
            this._params.RetrieveInstantaneousData = retrieveInstantaneousData;
            this._params.RetrieveLastDemandReset = retrieveLastDemandReset;
            this._params.RetrieveLoadProfileData = retrieveLoadProfileData;
            this._params.RetrieveLogEvents = retrieveLogEvents;
            this._params.RetrieveNetworkStatistics = retrieveNetworkStatistics;
            this._params.RetrievePriorSelfRead = retrievePriorSelfRead;
            this._params.RetrieveRecentRegisterData = retrieveRecentRegisterData;
            this._params.RetrieveVoltageMonitorData = retrieveVoltageMonitorData;
        }

        internal override void Invoke()
        {
            if (string.IsNullOrEmpty(UniqueID))
            {
                _requestID = Guid.NewGuid().ToString();

                _client = InternalClientFactory.ConstructInternalAPIManagerServiceClient(InternalLib.Endpoint.InternalEndpointName, InternalLib.Endpoint.InternalUserName, InternalLib.Endpoint.InternalUserPassword);
                _client.ServiceClient.InternalInteractiveRead(this._esn, this._params, this._requestID, InternalLib.Listener.InternalAPIController.HttpUri);
                _client.ServiceClient.Close();

                _uniqueID = _requestID;
            }
        }

        internal override void Abort()
        {
            if ((null != this._client) && (null != this._client.ServiceClient) &&
                (this._client.ServiceClient.State != CommunicationState.Closed))
            {
                _client.ServiceClient.Abort();
                this._aborted = true;
            }
        }

        public override void Cancel()
        {
            if (!string.IsNullOrEmpty(UniqueID))
            {
                RequestToken requestToken = new RequestToken();
                requestToken.Id = new Guid(UniqueID);

                Client<RequestServiceClient> c = null;
                c = InternalClientFactory.ConstructRequestServiceClient(InternalLib.Endpoint.RequestEndpointName, InternalLib.Endpoint.ExternalUserName, InternalLib.Endpoint.ExternalUserPassword);
                c.ServiceClient.CancelRequest(requestToken);
                c.ServiceClient.Close();
            }
        }
    }
}
