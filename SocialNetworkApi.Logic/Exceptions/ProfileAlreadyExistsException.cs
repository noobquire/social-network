using System;
using System.Runtime.Serialization;

namespace SocialNetworkApi.Services.Exceptions
{
    [Serializable]
    public class ProfileAlreadyExistsException : ApplicationException
    {
        public ProfileAlreadyExistsException()
        {
        }

        public ProfileAlreadyExistsException(string message) : base(message)
        {
        }

        protected ProfileAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
