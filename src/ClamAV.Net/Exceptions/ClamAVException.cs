using System;

namespace ClamAV.Net.Exceptions
{
    public class ClamAVException : Exception
    {
        internal ClamAVException(string message) : base(message)
        {
        }

        internal ClamAVException()
        {
        }

        internal ClamAVException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}