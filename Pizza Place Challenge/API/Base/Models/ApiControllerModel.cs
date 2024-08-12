using Newtonsoft.Json;
using System.Net;

namespace Pizza_Place_Challenge.API.Base.Models
{
    public class ApiControllerModel(HttpStatusCode status = HttpStatusCode.OK, string message = "OK")
    {
        [JsonProperty(PropertyName = "status")]
        public HttpStatusCode Status { get; set; } = status;

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; } = message;

        public void SetStatus(HttpStatusCode status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}
