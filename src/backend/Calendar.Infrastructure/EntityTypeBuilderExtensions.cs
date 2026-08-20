using Calendar.Domain.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using NodaTime;

namespace Calendar.Infrastructure;


public static class EntityTypeBuilderExtensions
{
    extension<T>(EntityTypeBuilder<T> builder)
    where T : class
    {
        public EntityTypeBuilder<T> DateTimeProperty(
            System.Linq.Expressions.Expression<Func<T, Domain.Model.CalDateTime?>> propertyExpression)
        {
            builder.ComplexProperty<Domain.Model.CalDateTime>(propertyExpression, calDateTime =>
            {
                calDateTime.Property("_utc");

                calDateTime.Property("_date");

                calDateTime.Property("_dateTime");

                calDateTime.Property<DateTimeZone>("_zone")
                    .HasConversion(
                        zone => zone.Id,
                        id => DateTimeZoneProviders.Tzdb[id]);

                calDateTime.Property(p => p.Value);
            });

            return builder;
        }

        public EntityTypeBuilder<T> DurationProperty(
            System.Linq.Expressions.Expression<Func<T, Domain.Model.Duration?>> propertyExpression)
        {
            builder.ComplexProperty<Domain.Model.Duration>(propertyExpression, duration =>
            {
                duration.Property(p => p.Value);
                duration.Ignore(p => p.Weeks);
                duration.Ignore(p => p.Days);
                duration.Ignore(p => p.Hours);
                duration.Ignore(p => p.Minutes);
                duration.Ignore(p => p.Seconds);
            });

            return builder;
        }

        public EntityTypeBuilder<T> RecurrenceRuleProperty(
            System.Linq.Expressions.Expression<Func<T, Domain.Model.RecurrenceRule?>> propertyExpression)
        {
            builder.ComplexProperty<Domain.Model.RecurrenceRule>(propertyExpression, recurrenceRule =>
            {
                recurrenceRule.ToJson();

                recurrenceRule.Property(rr => rr.Frequency)
                    .HasConversion<string>()
                    .HasMaxLength(15);

                recurrenceRule.Property(rr => rr.Interval)
                    .HasConversion(
                        interval => interval.Value,
                        value => new Domain.Model.Interval(value));

                recurrenceRule.Property(rr => rr.Count)
                    .HasConversion(new ValueConverter<Count, int>(
                        count => count.Value,
                        value => new Count(value)));

                recurrenceRule.ComplexProperty(rr => rr.BySecond, bySecond =>
                {
                    bySecond.ToJson();

                    bySecond.PrimitiveCollection(value => value.Value)
                        .ElementType(element =>
                        element.HasConversion(
                            new ValueConverter<Second, int>(
                                second => second.Value,
                                value => new Second(value))));
                });

                recurrenceRule.ComplexProperty(rr => rr.ByMinute, byMinute =>
                {
                    byMinute.ToJson();

                    byMinute.PrimitiveCollection(value => value.Value)
                        .ElementType(element =>
                        element.HasConversion(
                            new ValueConverter<Minute, int>(
                                minute => minute.Value,
                                value => new Minute(value))));
                });

                recurrenceRule.ComplexProperty(rr => rr.ByHour, byHour =>
                {
                    byHour.ToJson();

                    byHour.PrimitiveCollection(value => value.Value)
                        .ElementType(element =>
                            element.HasConversion(
                                new ValueConverter<Hour, int>(
                                    hour => hour.Value,
                                    value => new Hour(value))));
                });

                recurrenceRule.ComplexProperty(rr => rr.ByDay, byDay =>
                {
                    byDay.ToJson();

                    byDay.PrimitiveCollection(value => value.Value)
                        .ElementType(element =>
                            element.HasConversion(
                                new ValueConverter<OrdinalWeekDay, string>(
                                    ordinalWeekDay => ordinalWeekDay.ToString(),
                                    value => OrdinalWeekDay.Parse(value, null))));
                });

                recurrenceRule.ComplexProperty(rr => rr.ByWeek, byWeek =>
                {
                    byWeek.ToJson();

                    byWeek.PrimitiveCollection(value => value.Value)
                        .ElementType(element =>
                            element.HasConversion(
                                new ValueConverter<YearWeek, string>(
                                    yearWeek => yearWeek.ToString(),
                                    value => YearWeek.Parse(value, null))));
                });

                recurrenceRule.ComplexProperty(rr => rr.ByMonthDay, byMonthDay =>
                {
                    byMonthDay.ToJson();

                    byMonthDay.PrimitiveCollection(value => value.Value)
                        .ElementType(element =>
                            element.HasConversion(
                                new ValueConverter<MonthDay, int>(
                                    monthDay => monthDay.Value,
                                    value => new MonthDay(value))));
                });

                recurrenceRule.ComplexProperty(rr => rr.ByMonth, byMonth =>
                {
                    byMonth.ToJson();

                    byMonth.PrimitiveCollection(value => value.Value)
                        .ElementType(element =>
                            element.HasConversion(
                                new ValueConverter<Month, int>(
                                    month => month.Value,
                                    value => new Month(value))));
                });

                recurrenceRule.ComplexProperty(rr => rr.ByYearDay, byYearDay =>
                {
                    byYearDay.ToJson();

                    byYearDay.PrimitiveCollection(value => value.Value)
                        .ElementType(element =>
                            element.HasConversion(
                                new ValueConverter<YearDay, int>(
                                    yearDay => yearDay.Value,
                                    value => new YearDay(value))));
                });

                recurrenceRule.ComplexProperty(rr => rr.BySetPos, bySetPos =>
                {
                    bySetPos.ToJson();

                    bySetPos.PrimitiveCollection(value => value.Value)
                        .ElementType(element =>
                            element.HasConversion(
                                new ValueConverter<SetPos, int>(
                                    setPos => setPos.Value,
                                    value => new SetPos(value))));
                });
            });

            return builder;
        }

        public EntityTypeBuilder<T> AlarmProperty(
            System.Linq.Expressions.Expression<Func<T, Domain.Model.Alarm?>> propertyExpression)
        {
            builder.ComplexProperty<Domain.Model.Alarm>(propertyExpression, alarm =>
            {
                alarm.ToJson();

                alarm.Property(a => a.Action)
                    .HasConversion<string>()
                    .HasMaxLength(15);

                alarm.Property(a => a.Description)
                    .HasConversion(
                        description => description.Value,
                        value => new Description(value))
                    .HasMaxLength(150);

                alarm.Property(a => a.Summary)
                    .HasConversion(
                        summary => summary.Value,
                        value => new Summary(value))
                    .HasMaxLength(30);

                alarm.ComplexCollection(a => a.Attachments, attachments => 
                {
                    attachments.ToJson();

                    attachments.Property(a => a.Uri)
                        .HasConversion(
                            uri => uri.ToString(),
                            value => new Uri(value));

                    attachments.Property(a => a.MediaType)
                        .HasConversion(
                           new ValueConverter<MediaType?, string?>(
                                mediaType => mediaType.HasValue ? mediaType.Value.ToString() : null,
                                value => value != null ? new MediaType(value) : null));
                });

                alarm.AttendeesProperty(a => a.Attendees);

                alarm.Property(a => a.Repeat)
                    .HasConversion(
                        new ValueConverter<Repeat, int>(
                            repeat => repeat.Value,
                            value => new Repeat(value)));

                alarm.ComplexProperty(a => a.Trigger, trigger =>
                {
                    trigger.Property(t => t.Utc);

                    trigger.DurationProperty(t => t.Duration);

                    trigger.Property(t => t.Relation)
                        .HasConversion<string>()
                        .HasMaxLength(15);
                });
            });

            return builder;
        }

        public EntityTypeBuilder<T> AttendeesProperty(
            System.Linq.Expressions.Expression<Func<T, IEnumerable<Domain.Model.Attendee>>> propertyExpression)
        {
            builder.ComplexCollection<Domain.Model.Attendee>(propertyExpression, attendees =>
            {
                attendees.ToJson();

                attendees.Property(a => a.Address)
                    .HasConversion(
                        address => address.ToString(),
                        value => new Uri(value));

                attendees.Property(a => a.CommonName)
                    .HasConversion(
                        commonName => commonName.Value,
                        value => new CommonName(value));

                attendees.Property(a => a.CuType)
                    .HasConversion<string>();

                attendees.Property(a => a.SentBy)
                    .HasConversion(
                        sentBy => sentBy.ToString(),
                        value => new Uri(value));

                attendees.Property(a => a.Rsvp);

                attendees.PrimitiveCollection(a => a.Members)
                    .ElementType(element =>
                    element.HasConversion(
                        new ValueConverter<Uri, string>(
                            uri => uri.ToString(),
                            value => new Uri(value))));

                attendees.PrimitiveCollection(a => a.DelegatedFrom)
                    .ElementType(element =>
                    element.HasConversion(
                        new ValueConverter<Uri, string>(
                            uri => uri.ToString(),
                            value => new Uri(value))));

                attendees.PrimitiveCollection(a => a.DelegatedTo)
                    .ElementType(element =>
                    element.HasConversion(
                        new ValueConverter<Uri, string>(
                            uri => uri.ToString(),
                            value => new Uri(value))));
            });

            return builder;
        }

        public EntityTypeBuilder<T> RecurrenceDatesProperty(
            System.Linq.Expressions.Expression<Func<T, Domain.Model.RecurrenceDates?>> propertyExpression)
        {
            builder.ComplexProperty<Domain.Model.RecurrenceDates>(propertyExpression, recurrenceDates =>
            {
                recurrenceDates.ToJson();

                recurrenceDates.PrimitiveCollection("_values")
                    .ElementType(element =>
                    element.HasConversion(
                        new ValueConverter<Domain.Model.CalDateTime, string>(
                            dateTime => dateTime.ToString(),
                            value => Domain.Model.CalDateTime.Parse(value, null))));
            });

            return builder;
        }

        public EntityTypeBuilder<T> ExceptionDatesProperty(
            System.Linq.Expressions.Expression<Func<T, Domain.Model.ExceptionDates?>> propertyExpression)
        {
            builder.ComplexProperty<Domain.Model.ExceptionDates>(propertyExpression, exceptionDates =>
            {
                exceptionDates.ToJson();

                exceptionDates.PrimitiveCollection("_values")
                    .ElementType(element =>
                    element.HasConversion(
                        new ValueConverter<Domain.Model.CalDateTime, string>(
                            dateTime => dateTime.ToString(),
                            value => Domain.Model.CalDateTime.Parse(value, null))));
            });
            
            return builder;
        }

        public EntityTypeBuilder<T> RecurrencePeriodsProperty(
            System.Linq.Expressions.Expression<Func<T, Domain.Model.RecurrencePeriods?>> propertyExpression)
        {
            builder.ComplexProperty<Domain.Model.RecurrencePeriods>(propertyExpression, recurrencePeriods =>
            {
                recurrencePeriods.ToJson();

                recurrencePeriods.PrimitiveCollection("_values")
                    .ElementType(element =>
                    element.HasConversion(
                        new ValueConverter<Domain.Model.Period, string>(
                            period => period.ToString(),
                            value => Domain.Model.Period.Parse(value, null))));

            });

            return builder;
        }
    }
}