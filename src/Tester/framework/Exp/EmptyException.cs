using System;

namespace expunit.framework.Exp
{
    [Serializable]
    public class EmptyException : Exception
    {
        public EmptyException(string parameterName, int fieldIndex)
            : base($"Test Exception for {parameterName}({fieldIndex})")
        {
        }

        public EmptyException()
        {
        }

        public EmptyException(string message) : base(message)
        {
        }

        public EmptyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EmptyException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}