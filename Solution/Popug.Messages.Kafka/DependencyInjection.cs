using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Popug.Common;
using Popug.Messages.Contracts.Services;

namespace Popug.Messages.Kafka;
public static class DependencyInjection
{
    public static IServiceCollection AddKafkaProducer(this IServiceCollection services, IConfiguration configuration)
    {
        var setting = configuration.Get<KafkaClientConfiguration>();
        return services.AddKafkaProducer(setting);
    }

    public static IServiceCollection AddKafkaProducer(this IServiceCollection services, KafkaClientConfiguration settings)
    {
        services.AddScoped(sp =>
        {
            var config = new Confluent.Kafka.ProducerConfig { 
                BootstrapServers = settings.BootstrapServer, 
                ClientId = settings.ClientId 
            };
            return new Confluent.Kafka.ProducerBuilder<Confluent.Kafka.Null, string>(config).Build();
        });
        services.AddScoped<IProducer, Producer>();
        return services;
    }

    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        var setting = configuration.Get<KafkaClientConfiguration>();
        return services.AddKafkaConsumer(setting);
    }
    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, KafkaClientConfiguration settings)
    {
        services.AddScoped(sp =>
        {
            var config = new Confluent.Kafka.ConsumerConfig { 
                BootstrapServers = settings.BootstrapServer, 
                ClientId = settings.ClientId, 
                GroupId = settings.GroupId 
            };
            return new Confluent.Kafka.ConsumerBuilder<string, string>(config).Build();
        });
        services.AddSingleton<IConsumer, Consumer>();
        return services;
    }

    public static IServiceCollection AddCommonKafkaServices(this IServiceCollection services)
    {
        services.AddSingleton<IEventValueSerializer, EventValueSerializer>();
        services.AddSingleton<IMessageErrorLogger, MessageErrorLogger>();
        return services;
    }
}

