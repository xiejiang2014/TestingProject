using Newtonsoft.Json;

namespace ShareDrawing.HttpClient.Http
{
    public class HttpResponseObject<T> : HttpResponseBase
    {
        [JsonProperty(PropertyName = "data")] public T Data { get; set; }
    }
}