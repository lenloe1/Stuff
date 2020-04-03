using System;
using System.Text;

using Itron.Ami.Facade.WebServices.Internal.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.Internal
{
    public class InternalAPICallbackItem
    {
        private string _requestID;
        Itron.Ami.Facade.WebServices.Internal.ClientProxy.Result _result;
        System.Nullable<int> _jobKey;
        private DateTime _when;

        internal InternalAPICallbackItem(string RequestId, 
            Itron.Ami.Facade.WebServices.Internal.ClientProxy.Result Result, 
            System.Nullable<int> jobKey)
        {
            this._requestID = RequestId;
            this._result = Result;
            this._jobKey = jobKey;
            this._when = DateTime.UtcNow;
        }

        public string RequestID
        {
            get { return this._requestID; }
        }

        public Itron.Ami.Facade.WebServices.Internal.ClientProxy.Result Result
        {
            get { return this._result; }
        }

        public System.Nullable<int> JobKey
        {
            get { return this._jobKey; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public DateTime When
        {
            get { return _when; }
            internal set { _when = value; }
        }
    }
}
