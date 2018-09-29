using System;
using System.Runtime.Serialization;

namespace expunit.framework.Exp
{
    [Serializable]
    public class MethodNotFoundException : Exception
    {
        public MethodNotFoundException()
        {
        }

        public MethodNotFoundException(string message) : base(message)
        {
        }

        public MethodNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MethodNotFoundException(string methodName, string fullName)
            : base($"{methodName} is not found in {fullName} class")
        {
        }

        protected MethodNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}