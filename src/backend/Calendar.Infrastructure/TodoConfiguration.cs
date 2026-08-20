using System.Collections.Immutable;

using Calendar.Domain.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using NodaTime;

namespace Calendar.Infrastructure;

public class TodoConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                idValue => new TodoId(idValue))
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .HasConversion(
                userId => userId.Value,
                userIdValue => new UserId(userIdValue));

        builder.Property(x => x.CalendarId)
            .HasConversion(
                calendarId => calendarId.Value,
                calendarIdValue => new CalendarId(calendarIdValue));

        builder.Property(x => x.Classification)
            .HasConversion<string>()
            .HasMaxLength(15);
        
        builder.Property(x => x.Summary)
            .HasConversion(
                summary => summary.Value,
                value => new Summary(value))
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Description)
            .HasConversion(
                description => description.Value,
                value => new Description(value))
            .HasMaxLength(150);

        builder.Property(x => x.Location)
            .HasConversion(
                location => location.Value,
                value => new Location(value))
            .HasMaxLength(50);

        builder.ComplexProperty(x => x.GeographicPosition, geographicPosition =>
        {
            geographicPosition.Property(gp => gp.Latitude)
                .HasColumnName("Latitude");

            geographicPosition.Property(gp => gp.Longitude)
                .HasColumnName("Longitude");
        });

        builder.Property(x => x.Created);
        builder.Property(x => x.LastModified);

        builder.Property(x => x.SequenceNumber)
            .HasConversion(
                sequenceNumber => sequenceNumber.Value,
                value => new SequenceNumber(value));

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(15);

        builder.DateTimeProperty(e => e.Start);

        builder.DateTimeProperty(e => e.Due);

        builder.RecurrenceRuleProperty(x => x.RecurrenceRule);

        builder.RecurrenceDatesProperty(x => x.RecurrenceDates);

        builder.RecurrencePeriodsProperty(x => x.RecurrencePeriods);

        builder.ExceptionDatesProperty(x => x.ExceptionDates);

        builder.AlarmProperty(x => x.Alarm);

        builder.DurationProperty(x => x.Duration);
    }
}