using Newtonsoft.Json;

namespace ShareDrawing.HttpClient.Http
{
    /// <summary>
    /// http回复包格式
    /// </summary>
    public class HttpResponseBase
    {
        /// <summary>
        /// http响应码
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [JsonProperty(PropertyName = "timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        [JsonProperty(PropertyName = "msg")]
        public string Message { get; set; }
    }
}