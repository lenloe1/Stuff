using System;
using System.Text;

using Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    public class RequestStatusChangedItem
    {
        private StatusChangedInput _input;
        private DateTime _when;

        internal RequestStatusChangedItem(StatusChangedInput input)
        {
            this._when = DateTime.UtcNow;
            this._input = input;
        }

        internal StatusChangedInput Input
        {
            get { return _input; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal DateTime When
        {
            get { return _when; }
            set { _when = value; }
        }
    }
}
