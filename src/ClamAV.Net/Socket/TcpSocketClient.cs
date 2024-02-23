﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.ClamdProtocol;
using ClamAV.Net.Commands.Base;
using ClamAV.Net.Configuration;
using ClamAV.Net.Connection;
using Microsoft.Extensions.Logging;

namespace ClamAV.Net.Socket
{
    internal class TcpSocketClient : IConnection
    {
        private readonly ClamAvSettings mClamAvSettings;
        private readonly ILogger<TcpSocketClient> mLogger;
        private readonly TcpClient mClient;
        private bool mDisposed;
        private NetworkStream mNetworkStream;

        public bool IsConnected => mClient.Connected;

        public TcpSocketClient(ClamAvSettings clamAvSettings, ILogger<TcpSocketClient> logger)
        {
            mClamAvSettings = clamAvSettings ?? throw new ArgumentNullException(nameof(clamAvSettings));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
            mClient = new TcpClient();
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            mLogger.LogTrace($"Connecting to {mClamAvSettings}");

            await mClient.ConnectAsync(mClamAvSettings.Host, mClamAvSettings.Port).ConfigureAwait(false);

            mLogger.LogTrace($"Connected to {mClamAvSettings} {mClient.SendBufferSize} {mClient.ReceiveBufferSize}");

            mClient.NoDelay = true;
            mNetworkStream = mClient.GetStream();
        }

        private async Task<byte[]> ReadResponse(CancellationToken cancellationToken)
        {
            using MemoryStream memoryStream = new MemoryStream();

            do
            {
                byte[] answerBytes = new byte[mClamAvSettings.ReadBufferSize];

                int numBytesRead;

                while ((numBytesRead = await mNetworkStream
                    .ReadAsync(answerBytes, 0, answerBytes.Length, cancellationToken)
                    .ConfigureAwait(false)) > 0)
                {
                    if (numBytesRead < answerBytes.Length &&
                        answerBytes[numBytesRead - 1] == Consts.TERMINATION_BYTE)
                    {
                        memoryStream.Write(answerBytes, 0, numBytesRead - 1);
                        break;
                    }

                    memoryStream.Write(answerBytes, 0, numBytesRead);
                }
            } while (mClient.Available > 0);

            return memoryStream.ToArray();
        }

        public async Task SendCommandAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            mLogger.LogTrace($"Start writing command '{command.Name}' to the network stream");

            await command.WriteCommandAsync(mNetworkStream, cancellationToken).ConfigureAwait(false);

            mLogger.LogTrace($"End writing command '{command.Name}' to the network stream");
        }

        public async Task<TResponse> SendCommandAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            await SendCommandAsync(command as ICommand, cancellationToken).ConfigureAwait(false);

            mLogger.LogTrace($"Start reading command '{command.Name}' response");

            byte[] rawResponse = await ReadResponse(cancellationToken).ConfigureAwait(false);

            mLogger.LogTrace($"End reading command '{command.Name}' response. Total {rawResponse.Length} bytes");

            return command.ProcessRawResponse(rawResponse);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed)
                return;

            mDisposed = true;

            mClient?.Dispose();

            mLogger?.LogTrace("Socket disposed");
        }

        ~TcpSocketClient()
        {
            Dispose(false);
        }
    }
}