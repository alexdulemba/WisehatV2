using Auth0.AspNetCore.Authentication;
using Microsoft.FeatureManagement;
using Wisehat.Web.Components;
using Wisehat.Domain.Web;
using System.Diagnostics;
using Wisehat.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddDomainValidators();

builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFeatureManagement();

builder.Services.AddControllers();
builder.Services.AddAuth0WebAppAuthentication(options =>
{
  options.Domain = builder.Configuration["Auth0:Domain"]
    ?? throw new NullReferenceException("Auth0 Domain not provided");
  options.ClientId = builder.Configuration["Auth0:ClientId"]
    ?? throw new NullReferenceException("Auth0 ClientId not provided");
});

//builder.WebHost.UseStaticWebAssets();
var services = builder.Services.Select(s => s.ServiceType.Name).ToArray();
Debug.WriteLine(services);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
  .AddInteractiveServerRenderMode();

app.MapHub<WebProjectHub>("/webprojects");

app.Run();
