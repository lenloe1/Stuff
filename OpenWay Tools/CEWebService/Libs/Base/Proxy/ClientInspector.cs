using System;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Itron.Ami.CEWebServiceClient.Base
{
    public class ClientInspector : IClientMessageInspector
    {
        /// <summary>
        /// The most recent request message sent by a client.
        /// </summary>
        public string Request { get; set; }

        /// <summary>
        /// The most recent response message received from a service operation by a client.
        /// </summary>
        public string Response { get; set; }

        #region IClientMessageInspector Members

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            this.Response = reply.ToString();
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            this.Request = request.ToString();
            return null;
        }

        #endregion
    }
}
