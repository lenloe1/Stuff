using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;
using System.Globalization;
using System.ServiceModel;

using Itron.Ami.CEWebServiceClient.Base;
using CommonClientProxy = Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy;
using Itron.Ami.Facade.WebServices.Han.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    public class RemoveHANDevice : ExternalAction
    {
        #region Public Methods

        public RemoveHANDevice(string strESN,ulong uMacAddress)
        {
            CreateRequest(strESN, uMacAddress);
        }

        internal override void Invoke()
        {
            this.m_client = ExternalClientFactory.ConstructHanServiceClient(ExternalLib.Endpoint.HANEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);
            this.m_rqstToken = this.m_client.ServiceClient.RemoveDeviceByMeter(m_Request);
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

        private void CreateRequest(string strESN,ulong uMacAddress)
        {
            m_Request = new RemoveDeviceByMeterRequest();
            m_Request.MeterRequest = new EndpointRequest();
            m_Request.MeterRequest.ElectronicSerialNumber = strESN;
            m_Request.MacAddress = uMacAddress;

        }

        #endregion


        #region Members

        private RemoveDeviceByMeterRequest m_Request;
        private Client<HanServiceClient> m_client = null;
        private Itron.Ami.Facade.WebServices.Han.V200810.ClientProxy.RequestToken m_rqstToken = null;

        #endregion


    }
}

