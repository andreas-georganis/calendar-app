using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Calendar.Web.Client;
using Calendar.Web.Client.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddTransient<CookieHandler>();

builder.Services.AddHttpClient<IdentityClient>(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);

}).AddHttpMessageHandler<CookieHandler>();

builder.Services.AddHttpClient<ICalendarClient, CalendarClient>(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    
}).AddHttpMessageHandler<CookieHandler>();

builder.Services.AddHttpClient<IEntryClient, EntryClient>(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    
}).AddHttpMessageHandler<CookieHandler>();

builder.Services.AddScoped<StateContainer>();

await builder.Build().RunAsync();
