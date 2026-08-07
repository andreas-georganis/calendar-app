using Calendar.ServiceDefaults;
using Calendar.Web;
using Calendar.Web.Apis;
using Calendar.Web.Authentication;
using Calendar.Web.Client;
using Calendar.Web.Client.Services;
using Calendar.Web.Components;
using Calendar.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMudServices();

builder.Services.AddDataProtection(o => o.ApplicationDiscriminator = nameof(CalendarApp));

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie()
    .AddCookie(IdentityConstants.ExternalScheme)
    .AddExternalProviders(builder.Configuration);

builder.Services.AddAuthorizationBuilder();

builder.Services.AddCascadingAuthenticationState();

// Replace services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization(options => options.SerializeAllClaims = true);

// reverse proxy for api calls
builder.Services.AddHttpForwarderWithServiceDiscovery();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<TokenHandler>();

Uri calendarApi = new("http://calendar-app-api");

builder.Services
    .AddHttpClient<Calendar.Web.Services.IdentityClient>(httpClient => httpClient.BaseAddress = calendarApi)
    .AddHttpMessageHandler<TokenHandler>();

builder.Services
    .AddHttpClient<IUserClient, UserClient>(httpClient => httpClient.BaseAddress = calendarApi)
    .AddHttpMessageHandler<TokenHandler>();

builder.Services
    .AddHttpClient<ISessionClient, SessionClient>(httpClient => httpClient.BaseAddress = calendarApi)
    .AddHttpMessageHandler<TokenHandler>();

builder.Services
    .AddHttpClient<ICalendarClient, Calendar.Web.Services.CalendarClient>(httpClient => httpClient.BaseAddress = calendarApi)
    .AddHttpMessageHandler<TokenHandler>();

builder.Services
    .AddHttpClient<IEntryClient, Calendar.Web.Services.EntryClient>(httpClient => httpClient.BaseAddress = calendarApi)
    .AddHttpMessageHandler<TokenHandler>();

builder.Services.AddScoped<StateContainer>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Calendar.Web.Client._Imports).Assembly);

app.MapIdentityApi().MapCalendarApi();

app.Run();
