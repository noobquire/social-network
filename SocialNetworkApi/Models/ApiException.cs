using System.Net;

namespace SocialNetworkApi.Models
{
    public class ApiError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public ApiError(string message, HttpStatusCode code)
        {
            Code = code.ToString();
            Message = message;
        }
    }
}
