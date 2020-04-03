using System;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Itron.Ami.Facade.WebServices.Data.V200810.ClientProxy;
using Itron.Ami.CEWebServiceClient.Base;
using CommonClientProxy = Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    public class GetExternalInteractiveReadResult : ExternalAction
    {   
        private Client<DataServiceClient> _client = null;
        private InteractiveReadByEndpointResult _result;
        private string _identifier;

        internal GetExternalInteractiveReadResult(string strUniqueID, string strIdentifier)
        {
            _uniqueID = strUniqueID;
            _identifier = strIdentifier;
        }

        internal override void Invoke()
        {
            this._client = ExternalClientFactory.ConstructDataServiceClient(ExternalLib.Endpoint.DataEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);

            RequestToken Token = new RequestToken();
            Token.Id = new Guid(_uniqueID);

            this._result = this._client.ServiceClient.GetInteractiveReadByEndpointResult(Token);

            this._client.ServiceClient.Close();
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

        #region Public Properties

        public MeterEvent[] MeterEvents
        {
            get
            {
                MeterEvent[] Events = null;

                if (null != _result.ReadDataCollection)
                {
                    foreach (ReadData DataSet in _result.ReadDataCollection)
                    {
                        if (_identifier == DataSet.Identifier)
                        {
                            if (null != DataSet.EventLog && null != DataSet.EventLog.MeterEvents)
                            {
                                Events = DataSet.EventLog.MeterEvents;
                            }
                        }
                    }
                }

                return Events;
            }
        }

        public ReadData InteractiveReadData
        {
            get
            {
                ReadData IRData = null;

                if (null != _result.ReadDataCollection)
                {
                    foreach (ReadData DataSet in _result.ReadDataCollection)
                    {
                        if (_identifier == DataSet.Identifier)
                        {
                            IRData = DataSet;
                            break;
                        }
                    }
                }

                return IRData;
            }
        }

        #endregion
        
    }

}
