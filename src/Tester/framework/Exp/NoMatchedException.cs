using System;

namespace expunit.framework.Exp
{
    [Serializable]
    public class NoMatchedException : Exception
    {
        public NoMatchedException(string message) : base(message)
        {
        }

        public NoMatchedException()
        {
        }

        public NoMatchedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoMatchedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}