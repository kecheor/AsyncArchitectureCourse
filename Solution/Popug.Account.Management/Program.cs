using Popug.Accounts;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication;
using Popug.Accounts.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped(serviceProvider =>
{
    IAccountRepository concreteService = new AccountRepository();
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
            options.Authority = "https://localhost:5001";
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












