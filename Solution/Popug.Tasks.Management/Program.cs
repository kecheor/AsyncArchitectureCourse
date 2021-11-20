using Microsoft.EntityFrameworkCore;
using Popug.Accounts.Authentication;
using Popug.Common;
using Popug.Common.Services;
using Popug.Messages.Contracts.Services;
using Popug.Messages.Kafka;
using Popug.Tasks.Management.Services;
using Popug.Tasks.Repository;
using Popug.Tasks.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<IJsonSerializer, CommonJsonSerializer>();
builder.Services.AddScoped<Confluent.Kafka.IProducer<Confluent.Kafka.Null, string>>(sp =>
{
    var settings = builder.Configuration.GetSection("KafkaClient").Get<KafkaClientConfiguration>();
    var config = new Confluent.Kafka.ProducerConfig { BootstrapServers = settings.BootstrapServer, ClientId = settings.ClientId };
    return new Confluent.Kafka.ProducerBuilder<Confluent.Kafka.Null, string>(config).Build();
});
builder.Services.AddSingleton<IMessageErrorLogger, MessageErrorLogger>();
builder.Services.AddSingleton<IEventValueSerializer, EventValueSerializer>();
builder.Services.AddScoped<IProducer, Producer>();
builder.Services.AddDbContext<TasksDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("TasksDatabase")));
builder.Services.AddScoped<ITasksRepository, TasksRepository>();
builder.Services.AddScoped(serviceProvider =>
{
    IProducer producer = serviceProvider.GetService<IProducer>()!;
    ITasksService concreteService = serviceProvider.GetService<TasksService>()!;
    ILogger<ITasksService> logger = serviceProvider.GetService<ILogger<ITasksService>>()!;
    ITasksService producerDecorator = new TasksEventsDecorator(concreteService, producer, logger);
    return producerDecorator;
});
builder.Services.AddAuthorization();
builder.Services.AddOidcAuthentication(builder.Configuration.GetSection("Authentication"));
builder.Services.AddBff();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();
app.UseBff();
app.UseAuthentication();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers().AsBffApiEndpoint();
    endpoints.MapBffManagementEndpoints();
});


app.Run();
