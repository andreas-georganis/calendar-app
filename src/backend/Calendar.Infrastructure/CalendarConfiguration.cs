using Calendar.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;

namespace Calendar.Infrastructure;

public class CalendarConfiguration : IEntityTypeConfiguration<Domain.Model.Calendar>
{
    public void Configure(EntityTypeBuilder<Domain.Model.Calendar> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                idValue => new CalendarId(idValue))
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.UserId)
            .HasConversion(
                userId => userId.Value,
                userIdValue => new UserId(userIdValue));
        
        builder.Property(c => c.Name)
            .HasConversion(
                name => name.Value,
                text => new CalendarName(text))
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(c => c.Todos)
            .WithOne()
            .HasForeignKey(t => t.CalendarId);
        
        builder.HasMany(c => c.Events)
            .WithOne()
            .HasForeignKey(e => e.CalendarId);
        
        builder.Property(ce=>ce.TimeZone)
            .HasConversion(
                tz =>  tz.Id,
                tzId => DateTimeZoneProviders.Tzdb[tzId]);

        //builder.Property(c => c.Version).IsConcurrencyToken();
    }
}
