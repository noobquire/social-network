using System;
using System.Runtime.Serialization;

namespace SocialNetworkApi.Services.Exceptions
{
    [Serializable]
    public class UserAlreadyExistsException : ApplicationException
    {
        public UserAlreadyExistsException()
        {
        }

        public UserAlreadyExistsException(string message) : base(message)
        {
        }

        protected UserAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
