using System.Diagnostics;
using Calendar.Migrator;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Migrator;

internal static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMigration<TContext>()
            where TContext : DbContext
        {
            return services.AddHostedService<Worker<TContext>>();
        }

        public IServiceCollection AddMigration<TContext, TDbSeeder>()
            where TContext : DbContext
            where TDbSeeder : class, IDbSeeder<TContext>
        {
            services.AddSingleton<IDbSeeder<TContext>, TDbSeeder>();
            return services.AddMigration<TContext>();
        }
    }
}