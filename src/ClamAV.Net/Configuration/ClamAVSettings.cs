using System;
using System.Collections.Generic;
using System.Text;

namespace ClamAV.Net.Configuration
{
    class ClamAvSettings
    {
        public ClamAvSettings(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public string Host { get;  }
        public int Port { get; }

    }
}
