using System;
using System.Runtime.Serialization;

namespace NAuth.Domain.Exceptions
{
    // Definição de exceção customizada para uso em validações de domínio
    [Serializable]
    public class UserValidationException : Exception
    {
        public UserValidationException() { }
        public UserValidationException(string message) : base(message) { }
        public UserValidationException(string message, Exception inner) : base(message, inner) { }
        protected UserValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
