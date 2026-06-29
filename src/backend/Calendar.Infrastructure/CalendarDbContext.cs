using Calendar.Domain.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Infrastructure;

public class CalendarDbContext : IdentityUserContext<User, Guid>
{
    public CalendarDbContext(DbContextOptions<CalendarDbContext> options): base(options)
    {
        
    }
    
    public DbSet<Domain.Model.Calendar> Calendars => Set<Domain.Model.Calendar>();
    
    public DbSet<Event> Events => Set<Event>();
    
    public DbSet<Todo> Todos => Set<Todo>();
    
    public DbSet<Entry> Entries => Set<Entry>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.UsePropertyAccessMode(PropertyAccessMode.Property);

        //apply entity configurations
        modelBuilder.ApplyConfiguration(new CalendarConfiguration());
        // modelBuilder.ApplyConfiguration(new EntryConfiguration());
        // modelBuilder.ApplyConfiguration(new EventConfiguration());
        // modelBuilder.ApplyConfiguration(new TodoConfiguration());
        //or
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(CalendarDbContext).Assembly);
        
    }
}
