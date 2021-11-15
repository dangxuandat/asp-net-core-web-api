using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Entities.ErrorModel
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}