using CalendarApp.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;

namespace CalendarApp.API.Infrastructure.EntityConfigurations;

public class CalendarConfiguration : IEntityTypeConfiguration<Calendar>
{
    public void Configure(EntityTypeBuilder<Calendar> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(c => c.Todos)
            .WithOne()
            .HasForeignKey(t => t.CalendarId);
        
        builder.HasMany(c => c.Events)
            .WithOne()
            .HasForeignKey(e => e.CalendarId);
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .HasPrincipalKey(c => c.Id);
        
        builder.Property(ce=>ce.TimeZone)
            .HasConversion(
                tz =>  tz.Id,
                tzId => DateTimeZoneProviders.Tzdb[tzId]);

        //builder.Property(c => c.Version).IsConcurrencyToken();
    }
}
