using System;
using System.Net;
using System.Net.Sockets;

namespace Itron.Ami.CEWebServiceClient.Base
{
    public static class ServiceListener
    {
        private readonly static int _port = ServiceListener.GetPort();
        private readonly static string _hostName = ServiceListener.GetHostName();

        internal static int Port
        {
            get { return ServiceListener._port; }
        }

        public static string HostName
        {
            get { return ServiceListener._hostName; }
        }

        private static string GetHostName()
        {
            string hostName = null;

            try
            {
                hostName = Dns.GetHostName();

                if (string.IsNullOrEmpty(hostName))
                {
                    hostName = "localhost";
                }
            }
            catch (SocketException)
            {
                hostName = "localhost";
            }

            return hostName;
        }

        private static int GetPort()
        {
            int port = 9333;    // The default port.

            try
            {
                // We'll try to get the default port first.

                TcpListener tl = new TcpListener(new IPEndPoint(IPAddress.Loopback, port));
                tl.Start();
                port = ((IPEndPoint)tl.LocalEndpoint).Port;
                tl.Stop();
            }
            catch (SocketException)
            {
                // Since the default port is being used on this machine we'll try to get any available port.

                TcpListener tl = new TcpListener(new IPEndPoint(IPAddress.Loopback, 0));
                tl.Start();
                port = ((IPEndPoint)tl.LocalEndpoint).Port;
                tl.Stop();
            }

            return port;
        }
    }
}
