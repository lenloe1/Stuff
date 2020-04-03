using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;
using System.Globalization;
using System.ServiceModel;

using Itron.Ami.Facade.WebServices.Data.V200810.ClientProxy;
using Itron.Ami.CEWebServiceClient.Base;
using CommonClientProxy = Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy;
using Itron.Ami.Facade.WebServices.Membership.V200810.ClientProxy;



namespace Itron.Ami.CEWebServiceClient.External
{
    public class GetMemberESNList : ExternalAction
    {

        #region Public Methods

        public GetMemberESNList(
            string strGroupName)
        {
            m_strGroupNameRequest = strGroupName;
        }

        internal override void Invoke()
        {
            this.m_client = ExternalClientFactory.ConstructEndpointMembershipServiceClient(ExternalLib.Endpoint.MembershipEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);
            System.IO.Stream ESNStream = this.m_client.ServiceClient.GetMemberEsnListByGroup(m_strGroupNameRequest);

            System.IO.StreamReader strmReader = new System.IO.StreamReader(ESNStream);

            string strESN = string.Empty;
            while (false == strmReader.EndOfStream)
            {
                strESN = strmReader.ReadLine();

                if (false == string.IsNullOrEmpty(strESN))
                {
                    m_lstESNs.Add(strESN);
                }
            }

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

        #region Public Properties

        public List<string> ESNs
        {
            get
            {
                return m_lstESNs;
            }
        }

        #endregion


        #region Members

        private string m_strGroupNameRequest;
        private List<string> m_lstESNs = new List<string>();
        private Client<EndpointMembershipServiceClient> m_client = null;

        #endregion

    }


}
