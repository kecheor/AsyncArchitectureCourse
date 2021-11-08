using Popug.Accounts;
using System.IdentityModel.Tokens.Jwt;
using Popug.Accounts.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AccountsDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Accounts")));
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped(serviceProvider =>
{
    IAccountRepository concreteService = serviceProvider.GetService<AccountRepository>();
    IAccountRepository cudDecorator = new AccountsCudDecorator(concreteService);
    return cudDecorator;
});


JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication(options =>
        {
            options.DefaultScheme = "Cookies";
            options.DefaultChallengeScheme = "oidc";
            options.DefaultSignOutScheme = "oidc";
        })
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", options =>
        {
            options.Authority = "https://localhost:32776/";
            options.ClientId = "accounts";
            options.ClientSecret = "secret";
            options.ResponseType = "code";
            options.Scope.Add("accounts");
            options.SaveTokens = true;
        });
builder.Services.AddBff();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.UseBff();

app.UseEndpoints(endpoints =>
{
    endpoints.MapBffManagementEndpoints();
});


app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/all", async (IAccountRepository repository) => await repository.GetAll());
app.MapPost("/api/add", async (Account account, IAccountRepository repository) => await repository.Add(account));
app.MapPost("/api/update", async (Account account, IAccountRepository repository) => await repository.Update(account));

app.Run();