using System.Text.Json.Serialization;
using Asp.Versioning;
using Calendar.API;
using Calendar.API.Endpoints;
using Calendar.API.OpenApi;
using Calendar.API.RateLimiting;
using Calendar.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddValidation();
builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<DomainExceptionHandler>();

builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAuthentication().AddJwtBearer(IdentityConstants.BearerScheme, options =>
{
    options.Authority = builder.Configuration["Jwt:Authority"];
    options.Audience = builder.Configuration["Jwt:Audience"];
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
});

builder.Services.AddAuthorization();

// builder.Services.AddDbContext<CalendarDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("CalendarDb"), sqlOptions => 
//         sqlOptions.UseCompatibilityLevel(170).EnableRetryOnFailure().UseNodaTime())); // use defaults

builder.AddSqlServerDbContext<CalendarDbContext>("CalendarDb", 
    configureDbContextOptions: options => options
        .UseSqlServer(builder.Configuration.GetConnectionString("CalendarDb"),
            optionsBuilder =>
            {
                optionsBuilder.UseCompatibilityLevel(170);
                optionsBuilder.EnableRetryOnFailure();
                optionsBuilder.UseNodaTime();
            }));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(o =>
{
    o.AddDocumentTransformer<BearerOpenApiTransformer>();
    o.AddOperationTransformer<BearerOpenApiTransformer>();
    o.AddSchemaTransformer<NodaTimeTransformer>();
    o.AddSchemaTransformer<ParsableTransformer>();
});

builder.Services.AddSingleton<IClock>(NodaTime.SystemClock.Instance);

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
    });

builder.Services.AddOpenTelemetry().WithTracing(o => o.AddSource("Microsoft.AspNetCore"));

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseExceptionHandler();
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

app.MapFileApi();
app.MapCalendarApi();

app.Run();
