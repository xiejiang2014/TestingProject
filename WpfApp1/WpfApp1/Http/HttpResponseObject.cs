using Newtonsoft.Json;

namespace WpfApp1.Http
{
    public class HttpResponseObject<T> : HttpResponseBase
    {
        [JsonProperty(PropertyName = "data")] 
        public T? Data { get; set; }
    }
}