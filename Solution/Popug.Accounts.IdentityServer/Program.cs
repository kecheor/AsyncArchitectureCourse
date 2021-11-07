using Microsoft.EntityFrameworkCore;
using Popug.Accounts;
using Popug.Accounts.IdentityServer.Configuration;
using Popug.Accounts.Repository;
using Popug.Accounts.IdentityServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AccountsDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Accounts")));
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountIdentityService, AccountIdentityService>();
builder.Services.AddIdentityServer()
    .AddInMemoryClients(IdentityServerConfiguration.MapClients(builder.Configuration.GetSection("Clients")))
    .AddInMemoryApiScopes(IdentityServerConfiguration.MapScopes(builder.Configuration.GetSection("ApiScopes")))
    .AddInMemoryIdentityResources(IdentityServerConfiguration.MapResources(builder.Configuration.GetSection("Resources")));

var app = builder.Build();

app.UseStatusCodePagesWithReExecute("/Account/Error/{0}");
app.UseExceptionHandler("/Account/Error");
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.UseIdentityServer();

app.Run();
