using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WpfApp1;

public static class JsonExt
{
    public static JsonSerializerSettings SerializeSettings { get; set; } = new JsonSerializerSettings()
        {DateFormatString = "yyyy-MM-dd HH:mm:ss"};

    public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj, SerializeSettings);

    public static string ToJson(
        this   object          obj,
        params JsonConverter[] converters
    ) => JsonConvert.SerializeObject(obj, converters);

    public static void ToJsonFile(this object obj,
                                  string      fileName
    )
    {
        MakeSureDirectoryIsExist(fileName);

        var json = JsonConvert.SerializeObject(obj, SerializeSettings);
        File.WriteAllText(fileName, json);
    }


    public static void ToJsonFile(this object            obj,
                                  string                 fileName,
                                  params JsonConverter[] converters
    )
    {
        MakeSureDirectoryIsExist(fileName);

        var json = JsonConvert.SerializeObject(obj, converters);
        File.WriteAllText(fileName, json);
    }

    private static void MakeSureDirectoryIsExist(string fileName)
    {
        var dir = Path.GetDirectoryName(fileName);

        if (string.IsNullOrWhiteSpace(dir))
        {
            throw new ArgumentException("无效的文件路径", nameof(fileName));
        }

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    public static JsonSerializerSettings DeserializeSettings { get; set; } = new JsonSerializerSettings()
        {ContractResolver = new CamelCasePropertyNamesContractResolver()};

    public static string ToIndentedJson(this object obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);

    public static T? CloneByJson<T>(this T source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return JsonConvert.DeserializeObject<T>(source.ToJson());
    }

    public static T? FromJson<T>(string json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new FormatException($"文件格式错误,指定的内容不符合json格式.{Environment.NewLine}{json}");
        }

        return JsonConvert.DeserializeObject<T>(json, DeserializeSettings);
    }

    public static T? FromJsonFile<T>(string jsonFile) where T : class
    {
        if (string.IsNullOrWhiteSpace(jsonFile) || !File.Exists(jsonFile))
        {
            throw new FileNotFoundException("未找到指定的json文件.", jsonFile);
        }

        var json = File.ReadAllText(jsonFile);

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new FormatException(
                $"文件格式错误,指定的文件不符合json格式.{Environment.NewLine}文件:{Environment.NewLine}{jsonFile}{Environment.NewLine}内容:{Environment.NewLine}{json}");
        }

        return JsonConvert.DeserializeObject<T>(json, DeserializeSettings);
    }
}