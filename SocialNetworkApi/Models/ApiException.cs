using System.Net;

namespace SocialNetworkApi.Models
{
    public class ApiError
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public ApiError(string message, HttpStatusCode code)
        {
            Status = code.ToString();
            Message = message;
        }
    }
}
