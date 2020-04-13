namespace ClamAV.Net.Configuration
{
    internal class ClamAvSettings
    {
        public ClamAvSettings(string host, int port, int readBufferSize = 1024)
        {
            Host = host;
            Port = port;
            ReadBufferSize = readBufferSize;
        }

        public string Host { get; }
        public int Port { get; }
        public int ReadBufferSize { get; }
    }
}