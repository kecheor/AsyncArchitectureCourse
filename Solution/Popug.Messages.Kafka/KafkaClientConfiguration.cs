namespace Popug.Messages.Kafka;
public class KafkaClientConfiguration
{
    public string BootstrapServer { init; get; }
    public string ClientId { init; get; }
    public string GroupId { init; get; }

    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { "bootstrap.servers", BootstrapServer },
            { "client.id", ClientId },
            { "group.id", GroupId },
            { "debug", "all" }
        };
    }
}