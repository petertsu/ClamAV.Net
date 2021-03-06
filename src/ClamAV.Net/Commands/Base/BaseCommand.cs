﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.ClamdProtocol;

namespace ClamAV.Net.Commands.Base
{
    internal abstract class BaseCommand : ICommand
    {
        public string Name { get; }
        private readonly byte[] mClamdCommandName;

        protected BaseCommand(string name)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
            mClamdCommandName =
                Encoding.ASCII.GetBytes($"{Consts.COMMAND_PREFIX_CHARACTER}{Name}{(char)Consts.TERMINATION_BYTE}");
        }

        public virtual async Task WriteCommandAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            await stream.WriteAsync(mClamdCommandName, 0, mClamdCommandName.Length, cancellationToken).ConfigureAwait(false);
            await WriteCommandDataAsync(stream, cancellationToken).ConfigureAwait(false);
        }

        protected virtual Task WriteCommandDataAsync(Stream stream, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}