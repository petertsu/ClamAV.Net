namespace ClamAV.Net.Configuration
{
    internal class ClamAvSettings
    {
        public ClamAvSettings(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public string Host { get; }
        public int Port { get; }
    }
}