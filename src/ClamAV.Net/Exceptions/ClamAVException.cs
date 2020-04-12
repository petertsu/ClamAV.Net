using System;

namespace ClamAV.Net.Exceptions
{
    public class ClamAvException : Exception
    {
        internal ClamAvException(string message) : base(message)
        {
        }

        internal ClamAvException()
        {
        }

        internal ClamAvException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}