using System;
using System.Runtime.Serialization;

namespace expunit.framework.Exp
{
    [Serializable]
    public class FieldNotFoundException : Exception
    {
        public FieldNotFoundException()
        {
        }

        public FieldNotFoundException(string message) : base(message)
        {
        }

        public FieldNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public FieldNotFoundException(string fieldName, string fullName)
            : base($"{fieldName} is not found in {fullName} class")
        {
        }

        protected FieldNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}