using Calendar.API.Infrastructure;
using Calendar.API.Migrator;
using CalendarApp.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMigration<CalendarAppDbContext>();

builder.Services.AddPooledDbContextFactory<CalendarAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CalendarAppDb"), sqlOptions => 
        sqlOptions.MigrationsAssembly("Calendar.API")));

builder.EnrichSqlServerDbContext<CalendarAppDbContext>();

var host = builder.Build();

host.Run();
