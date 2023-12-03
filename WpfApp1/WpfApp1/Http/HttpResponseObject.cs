using Newtonsoft.Json;

namespace WpfApp1.Http;
internal sealed class HttpResponseObject<T> : HttpResponseBase
{
    [JsonProperty(PropertyName = "data")] public T Data { get; set; }
}