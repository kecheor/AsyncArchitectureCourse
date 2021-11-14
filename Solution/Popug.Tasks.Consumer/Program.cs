using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Popug.Common.Services;
using Popug.Messages.Contracts.Services;
using Popug.Messages.Kafka;
using Popug.Tasks.Repository;

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", false)
               .Build();

services.AddSingleton<IJsonSerializer, CommonJsonSerializer>();
services.AddScoped<Confluent.Kafka.IConsumer<string, string>>(sp =>
{
    var settings = configuration.GetSection("KafkaClient").Get<KafkaClientConfiguration>();
    var config = new Confluent.Kafka.ConsumerConfig { BootstrapServers = settings.BootstrapServer, ClientId = settings.ClientId, GroupId = settings.GroupId };
    return new Confluent.Kafka.ConsumerBuilder<string, string>(config).Build();
});
services.AddSingleton<IConsumer, Consumer>();
services.AddDbContext<TasksDbContext>(o => o.UseNpgsql(configuration.GetConnectionString("Database")));
services.AddSingleton<IPerformerRepository, PerformerRepository>();
services.AddSingleton<PerformersConsumer>();

var provider = services.BuildServiceProvider();

provider.GetService<PerformersConsumer>()!.Run(default(CancellationToken));