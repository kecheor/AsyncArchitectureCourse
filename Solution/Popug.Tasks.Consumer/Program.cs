using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Popug.Messages.Kafka;
using Popug.Tasks.Consumer;
using Popug.Tasks.Repository;
using Popug.Tasks.Repository.Repositories;

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", false)
               .Build();

services
    .AddCommonKafkaServices()
    .AddKafkaConsumer(configuration.GetSection("KafkaClient"));
services.AddDbContext<TasksDbContext>(o => o.UseNpgsql(configuration.GetConnectionString("Database")));
services.AddSingleton<IPerformerRepository, PerformerRepository>();
services.AddSingleton<PerformersConsumer>();

var provider = services.BuildServiceProvider();

provider.GetService<PerformersConsumer>()!.Run(default(CancellationToken));