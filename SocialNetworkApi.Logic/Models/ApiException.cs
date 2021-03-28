using System;
using System.Net;
using System.Runtime.Serialization;

namespace SocialNetworkApi.Services.Models
{
    [Serializable]
    public class ApiException : ApplicationException
    {
        public string Code { get; set; }
        public ApiException(string message, HttpStatusCode code) : base(message)
        {
            Code = code.ToString();
        }

        protected ApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
