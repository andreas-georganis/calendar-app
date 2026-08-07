using Microsoft.EntityFrameworkCore;

namespace CalendarApp.API.Migrator;

public interface IDbSeeder<in TContext> where TContext : DbContext
{
    Task SeedAsync(TContext context, CancellationToken cancellationToken = default);
}