using System;

namespace Alebrije.Exceptions
{
    public class AlebrijeException : Exception
    {
        public AlebrijeException(Exception innerException) :
            base("An unexpected error occurred.", innerException)
        {

        }

        public AlebrijeException(string message, Exception innerException = null) : base(message, innerException)
        {

        }
    }
}