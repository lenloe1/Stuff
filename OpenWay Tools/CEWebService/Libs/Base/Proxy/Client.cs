using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itron.Ami.CEWebServiceClient.Base
{
    public class Client<TClient> : IDisposable
        where TClient : class, IDisposable
    {
        public Client(TClient client, ClientInspector inspector)
        {
            this.ServiceClient = client;
            this.Inspector = inspector;
        }

        ~Client()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ServiceClient.Dispose();
            }
        }

        public TClient ServiceClient { get; private set; }

        public ClientInspector Inspector { get; private set; }
    }
}
