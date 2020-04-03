using System;
using System.Collections.Generic;

namespace Itron.Ami.CEWebServiceClient.Base
{
    public class SubscriberDataEventArgs<T> : EventArgs
    {
        public SubscriberDataEventArgs(List<T> list)
        {
            this.DataList = list;
        }

        public List<T> DataList { get; private set; }
    }
}
