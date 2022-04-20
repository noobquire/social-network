using System;
using System.Runtime.Serialization;

namespace SocialNetworkApi.Services.Exceptions
{
    [Serializable]
    public class RegisterUserException : ApplicationException
    {
        public RegisterUserException()
        {
        }

        public RegisterUserException(string message) : base(message)
        {
        }

        protected RegisterUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
