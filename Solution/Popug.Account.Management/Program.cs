using System.IdentityModel.Tokens.Jwt;
using Popug.Accounts.Repository;
using Microsoft.EntityFrameworkCore;
using Popug.Accounts.Authentication;
using Popug.Messages.Kafka;
using Popug.Messages.Contracts.Services;
using Popug.Common.Services;
using Popug.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IJsonSerializer, CommonJsonSerializer>();

builder.Services.AddDbContext<AccountsDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Accounts")));
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped(serviceProvider =>
{
    IProducer producer = serviceProvider.GetService<IProducer>()!;
    IAccountRepository concreteService = serviceProvider.GetService<AccountRepository>()!;
    IAccountRepository cudDecorator = new AccountsRepositoryCudDecorator(concreteService, producer);
    return cudDecorator;
});
builder.Services
    .AddCommonKafkaServices()
    .AddKafkaConsumer(builder.Configuration.GetSection("KafkaClient"));


JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthorization();
builder.Services.AddOidcAuthentication(builder.Configuration.GetSection("Authentication"));
builder.Services.AddBff();
builder.Services.AddControllers();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
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