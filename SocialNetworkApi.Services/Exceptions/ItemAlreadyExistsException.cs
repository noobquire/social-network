using System;
using System.Runtime.Serialization;

namespace SocialNetworkApi.Services.Exceptions
{
    [Serializable]
    public class ItemAlreadyExistsException : ApplicationException
    {
        public ItemAlreadyExistsException()
        {
        }

        public ItemAlreadyExistsException(string message) : base(message)
        {
        }

        protected ItemAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}