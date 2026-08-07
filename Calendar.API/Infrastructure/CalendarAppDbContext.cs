using CalendarApp.API.Infrastructure.EntityConfigurations;
using CalendarApp.API.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.API.Infrastructure;

public class CalendarAppDbContext : IdentityUserContext<User, Guid>
{
    public CalendarAppDbContext(DbContextOptions<CalendarAppDbContext> options): base(options)
    {
        
    }
    
    public DbSet<Calendar> Calendars => Set<Calendar>();
    
    public DbSet<Event> Events => Set<Event>();
    
    public DbSet<Todo> Todos => Set<Todo>();
    
    public DbSet<Entry> Entries => Set<Entry>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.UsePropertyAccessMode(PropertyAccessMode.Property);

        //apply entity configurations
        modelBuilder.ApplyConfiguration(new CalendarConfiguration());
        modelBuilder.ApplyConfiguration(new EntryConfiguration());
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new TodoConfiguration());
        //or
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(CalendarAppDbContext).Assembly);
        
    }
}
