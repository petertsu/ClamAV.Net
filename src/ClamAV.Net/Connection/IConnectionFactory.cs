using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClamAV.Net.Connection
{
    internal interface IConnectionFactory
    {
        Task<IConnection> CreateAsync(Uri connectionUri, CancellationToken cancellationToken = default);
    }
}