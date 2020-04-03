using System;
using System.Text;
using System.Runtime.Serialization;

using Itron.Ami.Facade.WebServices.Data.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    [DataContract()]
    public class DataSubscriberItem
    {
        private DataArrivedInput _input;
        private DateTime _when;

        internal DataSubscriberItem(DataArrivedInput input)
        {
            this._when = DateTime.UtcNow;
            this._input = input;
        }

        [DataMember(Order = 1)]
        public DateTime Timestamp
        {
            get { return this._when; }
            set { this._when = value; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DataMember(Order = 2)]
        public DataArrivedInput DataArrivedInput
        {
            get { return _input; }
            set { this._input = value; }
      }
    }
}
