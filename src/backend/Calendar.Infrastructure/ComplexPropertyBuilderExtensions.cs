using Calendar.Domain.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using NodaTime;

namespace Calendar.Infrastructure;

public static class ComplexPropertyBuilderExtensions
{
    extension<T>(ComplexPropertyBuilder<T> builder)
    where T : class
    {
        public ComplexPropertyBuilder<T> DateTimeProperty(
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

        

        public ComplexPropertyBuilder<T> DurationProperty(
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

        public ComplexPropertyBuilder<T> AttendeesProperty(
            System.Linq.Expressions.Expression<Func<T, IEnumerable<Domain.Model.Attendee>?>> propertyExpression)
        {
            builder.ComplexCollection<Domain.Model.Attendee>(propertyExpression, attendee =>
            {
                    attendee.ToJson();

                    attendee.Property(a => a.Address)
                        .HasConversion(
                            address => address.ToString(),
                            value => new Uri(value));

                    attendee.Property(a => a.CommonName)
                    .HasConversion(
                        commonName => commonName.Value,
                        value => new CommonName(value));

                    attendee.Property(a => a.CuType)
                        .HasConversion<string>();

                    attendee.Property(a => a.SentBy)
                        .HasConversion(
                            sentBy => sentBy.ToString(),
                            value => new Uri(value));

                    attendee.Property(a => a.Rsvp);

                    attendee.PrimitiveCollection(a => a.Members)
                        .ElementType(element =>
                        element.HasConversion(
                            new ValueConverter<Uri, string>(
                                uri => uri.ToString(),
                                value => new Uri(value))));

                    attendee.PrimitiveCollection(a => a.DelegatedFrom)
                        .ElementType(element =>
                        element.HasConversion(
                            new ValueConverter<Uri, string>(
                                uri => uri.ToString(),
                                value => new Uri(value))));

                    attendee.PrimitiveCollection(a => a.DelegatedTo)
                        .ElementType(element =>
                        element.HasConversion(
                            new ValueConverter<Uri, string>(
                                uri => uri.ToString(),
                                value => new Uri(value))));
                });

                return builder;
        }
    
    }
}