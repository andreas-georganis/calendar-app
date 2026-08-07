var builder = DistributedApplication.CreateBuilder(args);

var sqlserver = builder
    .AddSqlServer("sql-server")
    .WithDataVolume("calendar-app-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDbGate().ExcludeFromManifest()
    .WithAdminer().ExcludeFromManifest();
    
var calendarAppDb = sqlserver
    .AddDatabase("CalendarAppDb");

var migrator = builder
    .AddProject<Projects.CalendarApp_API_Migrator>("calendar-app-api-migrator")
    .WithReference(calendarAppDb)
    .WaitFor(calendarAppDb);

var api = builder
    .AddProject<Projects.CalendarApp_API>("calendar-app-api")
    .WithReference(calendarAppDb)
    .WaitForCompletion(migrator);

builder.AddProject<Projects.CalendarApp_Web>("calendar-app-web")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
