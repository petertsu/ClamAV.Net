using System;

namespace ClamAV.Net.Socket
{
    public class SocketClientFactory
    {
        public static ISocketClient CreateSocketClient(Uri connectionString)
        {
            if (connectionString.Scheme == "tcp")
                return new TcpSocketClient(connectionString);

            throw new NotImplementedException($"Protocol '{connectionString.Scheme}' is not supported");
        }
    }
}