var builder = DistributedApplication.CreateBuilder(args);

var sqlserver = builder.AddSqlServer("sqlserver")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDbGate().ExcludeFromManifest()
    .WithAdminer().ExcludeFromManifest();

var db = sqlserver.AddDatabase("CalendarDb");

builder.AddProject<Projects.Calendar_API>("calendar-api")
    .WithReference(db)  
    .WithEnvironment("JWT__Authority", "http://127.0.0.1:5556/dex")
    .WithEnvironment("JWT__Audience", "calendar-app");

builder.AddProject<Projects.Calendar_Migrator>("calendar-migrator")
    .WithReference(db)
    .WaitFor(db);

builder.AddProject<Projects.Calendar_Web>("calendar-web");


builder.Build().Run();
