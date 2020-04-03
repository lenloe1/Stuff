using System;
using System.Text;
using System.Runtime.Serialization;

using Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    [DataContract()]
    public class EventSubscriberItem
    {
        private EventsArrivedInput _input;
        private DateTime _when;

        internal EventSubscriberItem(EventsArrivedInput input)
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
        public EventsArrivedInput EventsArrivedInput
        {
            get { return _input; }
            set { this._input = value; }
      }
    }
}
