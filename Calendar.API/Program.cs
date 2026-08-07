using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Asp.Versioning;
using Asp.Versioning.Builder;
using CalendarApp.API;
using CalendarApp.API.Endpoints;
using CalendarApp.API.Infrastructure;
using CalendarApp.API.OpenApi;
using CalendarApp.API.RateLimiting;
using CalendarApp.Contracts;
using CalendarApp.ServiceDefaults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Scalar.AspNetCore;
using User = CalendarApp.API.Model.User;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDataProtection(o => o.ApplicationDiscriminator = nameof(CalendarApp));

builder.Services.AddValidation();

builder.Services.AddProblemDetails();

builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddAuthorizationBuilder()
    .AddCurrentUserHandler()
    .AddCurrentUser();

builder.Services.AddIdentityCore<User>(o=>o.User.RequireUniqueEmail = true)
    .AddEntityFrameworkStores<CalendarAppDbContext>();
    //.AddApiEndpoints();

builder.AddSqlServerDbContext<CalendarAppDbContext>("CalendarAppDb", 
    configureDbContextOptions: options => options
        .UseSqlServer(builder.Configuration.GetConnectionString("CalendarAppDb"),
            optionsBuilder =>
            {
                optionsBuilder.UseCompatibilityLevel(170);
                optionsBuilder.EnableRetryOnFailure(); // use defaults
            }));

builder.Services.AddOpenApi(o =>
{
    o.AddDocumentTransformer<BearerOpenApiTransformer>();
    o.AddOperationTransformer<BearerOpenApiTransformer>();
});

builder.Services.AddRateLimiting(builder.Configuration);

builder.Services.AddHttpLogging(o => { });

builder.Services.AddApiVersioning(o => 
    {
        o.DefaultApiVersion = new ApiVersion(1, 0);
        o.AssumeDefaultVersionWhenUnspecified = true;
        o.ReportApiVersions = true;
        o.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version"));
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    })
    .EnableApiVersionBinding();

builder.Services.AddOpenTelemetry().WithTracing(o=>o.AddSource("Microsoft.AspNetCore"));


var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Calendar API";
        options.Servers = [];
        options.Authentication = new ScalarAuthenticationOptions { PreferredSecuritySchemes = ["Bearer"] };
        options.Theme = ScalarTheme.Saturn;
        options.Layout = ScalarLayout.Modern;
        options.HideClientButton = true;
        options.DefaultFonts = false;
    });
}

app.UseHttpsRedirection();

app.MapGet("/", () => TypedResults.Redirect("/scalar/v1")).ExcludeFromDescription();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder group = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

group.MapIdentityApi()
    .MapCalendarApi()
    .MapCalendarEntryApi();

app.Run();
