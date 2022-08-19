using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp.Serializers;

namespace ShareDrawing.HttpClient.Http
{
    public class NewtonsoftJsonSerializer : ISerializer
    {
        public NewtonsoftJsonSerializer()
        {
            ContentType = "application/json";
        }
        public string ContentType { get; set; }

        public string Serialize(object obj)
        {
            JsonSerializerSettings set = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore // 让序列化忽略循环引用
            };
            set.Converters.Add(new StringEnumConverter()); // 序列化枚举时取名字而不是数字值
            return JsonConvert.SerializeObject(obj, set);
        }
    }
}
