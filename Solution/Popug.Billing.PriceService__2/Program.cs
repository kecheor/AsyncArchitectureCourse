using Microsoft.Extensions.Configuration;

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", false)
               .Build();

services
    .AddCommonKafkaServices()
    .AddKafkaConsumer(configuration.GetSection("KafkaClient"));