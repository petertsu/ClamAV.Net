using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ClamAV.Net.ClamdProtocol;

namespace ClamAV.Net.Tests.ClamAvServer
{
    internal class ClamAvServerMock
    {
        private readonly TcpListener mTcpListener;
        private bool mStopped;

        public ClamAvServerMock(int port)
        {
            mTcpListener = new TcpListener(IPAddress.Loopback, port);
        }

        public void Stop()
        {
            mStopped = true;
        }

        public void Start(Func<string> fakeResponse)
        {
            mTcpListener.Start();

            Task.Run(() =>
                {
                    while (!mStopped)
                    {
                        TcpClient client = mTcpListener.AcceptTcpClient();

                        NetworkStream stream = client.GetStream();

                        byte[] bytes = new byte[10 * 1024];

                        while (stream.Read(bytes, 0, bytes.Length) != 0)
                        {
                            string dataToWrite = fakeResponse();

                            byte[] response = Encoding.UTF8.GetBytes(dataToWrite + (char)Consts.TERMINATION_BYTE);
                            stream.Write(response, 0, response.Length);
                            stream.Flush();
                        }
                    }
                }
            );
        }
    }
}