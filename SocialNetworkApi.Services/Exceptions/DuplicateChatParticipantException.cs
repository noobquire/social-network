using System;
using System.Runtime.Serialization;

namespace SocialNetworkApi.Services.Exceptions
{
    [Serializable]
    public class DuplicateChatParticipantException : ApplicationException
    {
        public DuplicateChatParticipantException()
        {

        }

        public DuplicateChatParticipantException(string message) : base(message)
        {

        }

        protected DuplicateChatParticipantException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
