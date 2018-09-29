using System;
using System.Runtime.Serialization;

namespace expunit.framework.Exp
{
    [Serializable]
    public class PropertyNotFoundException : Exception
    {
        public PropertyNotFoundException()
        {
        }

        public PropertyNotFoundException(string message) : base(message)
        {
        }

        public PropertyNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PropertyNotFoundException(string propertyName, string fullName)
            : base($"{propertyName} is not found in {fullName} class")
        {
        }

        protected PropertyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}