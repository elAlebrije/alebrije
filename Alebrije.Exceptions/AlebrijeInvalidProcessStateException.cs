using System;

namespace Alebrije.Exceptions
{
    public class AlebrijeInvalidProcessStateException : AlebrijeException
    {
        public AlebrijeInvalidProcessStateException(Exception innerException) : base(innerException)
        {
        }

        public AlebrijeInvalidProcessStateException(string state, Exception innerException = null) : base($"Invalid Status {state} to execute this process.", innerException)
        {
        }
    }
}