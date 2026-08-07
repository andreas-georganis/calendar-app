using Calendar.Migrator;
using Calendar.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMigration<CalendarDbContext>();

builder.Services.AddPooledDbContextFactory<CalendarDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CalendarDb"), sqlOptions => 
        sqlOptions.MigrationsAssembly("Calendar.Migrator")));

builder.EnrichSqlServerDbContext<CalendarDbContext>();

var host = builder.Build();

host.Run();
