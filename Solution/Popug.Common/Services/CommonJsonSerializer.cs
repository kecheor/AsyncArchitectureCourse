using System.Text.Json;
namespace Popug.Common.Services;
public class CommonJsonSerializer : IJsonSerializer
{
    public T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }

    public string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj);
    }
}