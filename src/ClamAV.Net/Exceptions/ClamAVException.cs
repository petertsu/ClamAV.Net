using System;
using System.Runtime.Serialization;

namespace ClamAV.Net.Exceptions
{
    /// <summary>
    /// ClamAV client exception
    /// </summary>
    [Serializable]
    public class ClamAvException : Exception
    {
        /// <summary>
        /// Serialization ctor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
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