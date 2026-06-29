var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Calendar_API>("calendar-api");

builder.AddProject<Projects.Calendar_Web>("calendar-web");

builder.AddProject<Projects.Calendar_API_Migrator>("calendar-api-migrator");

builder.Build().Run();
