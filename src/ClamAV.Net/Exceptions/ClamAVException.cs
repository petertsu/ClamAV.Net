using System;
using System.Runtime.Serialization;

namespace ClamAV.Net.Exceptions
{
    [Serializable]
    public class ClamAvException : Exception
    {
        protected ClamAvException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        internal ClamAvException(string message) : base(message)
        {
        }

        internal ClamAvException() : base("ClamAV client error occured")
        {
        }

        internal ClamAvException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}