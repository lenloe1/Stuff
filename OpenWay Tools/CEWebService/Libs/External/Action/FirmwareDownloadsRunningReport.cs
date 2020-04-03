using System;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Reporting.V200908.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{

    public class GetFirmwareDownloadsRunningReport: ExternalAction
    {
        Client<ReportServiceClient> _client = null;
        FirmwareDownloadsReport _FWDLReport = null;

        internal GetFirmwareDownloadsRunningReport()
        {
        }

        internal override void Invoke()
        {
            if (string.IsNullOrEmpty(UniqueID))
            {
                this._client = ExternalClientFactory.ConstructReportServiceClient(ExternalLib.Endpoint.ReportEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);
                this._FWDLReport = this._client.ServiceClient.GetFirmwareDownloadsRunningReport();
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

        public FirmwareDownloadsReport FWDLReport
        {
            get
            {
                return _FWDLReport;
            }
        }
   }
}
