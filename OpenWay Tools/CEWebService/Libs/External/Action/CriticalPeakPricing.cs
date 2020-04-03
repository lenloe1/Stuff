using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using Itron.Ami.CEWebServiceClient.Base; 
using Itron.Ami.Facade.WebServices.Provisioning.V200908.ClientProxy;
using Itron.Ami.Facade.WebServices.Control.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    public class CPPActivation : ExternalAction
    {

        #region Public Methods

        public CPPActivation(string[] m_ESN, DateTime stTime, DateTime endTime)
        {
            CreateRequest(m_ESN, stTime, endTime);
        }

        internal override void Invoke()
        {
            this.m_client = ExternalClientFactory.ConstructControlServiceClient(ExternalLib.Endpoint.ControlEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);
            this._rqstToken = this.m_client.ServiceClient.ActivateCriticalPeakPricingByMeters(_epRequest);
            Thread.Sleep(300000);
            this.m_client.ServiceClient.Close();
        }

        internal override void Abort()
        {
            if ((null != this.m_client) &&
                (null != this.m_client.ServiceClient) &&
                (this.m_client.ServiceClient.State != CommunicationState.Closed))
            {
                this._aborted = true;
                this.m_client.ServiceClient.Abort();
            }
        }

        #endregion

        #region Private Methods

        private void CreateRequest(string[] m_ESN, DateTime stTime, DateTime endTime)
        {
            _epRequest = new CriticalPeakPricingByMetersRequest();
            _epRequest.EndpointCollectionRequest = new EndpointCollectionRequest();
            _epRequest.Parameters = new CriticalPeakPricingParameters();
            _epRequest.EndpointCollectionRequest.ElectronicSerialNumbers = m_ESN;
            _epRequest.Parameters.StartTime = stTime;
            _epRequest.Parameters.EndTime = endTime;   
        }

        #endregion

        #region Members

        private Client<ControlServiceClient> m_client = null;
        private CriticalPeakPricingByMetersRequest _epRequest;
        private RequestToken _rqstToken = null;
        

        #endregion

    }

}
