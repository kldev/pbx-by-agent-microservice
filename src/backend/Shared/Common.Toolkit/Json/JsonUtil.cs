using Newtonsoft.Json;

namespace Common.Toolkit.Json;

public static class JsonUtil
{
    public static T? Read<T>(string input)
    {
        return JsonConvert.DeserializeObject<T>(input, new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Populate
        });
    }

    public static string Write(object data)
    {
        return JsonConvert.SerializeObject(data);
    }
}
