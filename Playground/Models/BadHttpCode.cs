using System;
using System.Runtime.Serialization;

namespace Playground.Models
{
    [Serializable]
    internal class BadHttpCode : Exception
    {
        public BadHttpCode()
        {
        }

        public BadHttpCode(string message) : base(message)
        {
        }

        public BadHttpCode(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadHttpCode(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}