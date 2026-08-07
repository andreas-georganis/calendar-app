using System.Linq.Expressions;
using CalendarApp.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DateTime = CalendarApp.API.Model.DateTime;

namespace CalendarApp.API.Infrastructure.EntityConfigurations;

public static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<TEntity> DateTimeProperty<TEntity>(
        this EntityTypeBuilder<TEntity> builder, 
        Expression<Func<TEntity, DateTime?>> propertyExpression) 
        where TEntity : class
    {
        builder.ComplexProperty(propertyExpression, ConfigureDateTimeProperty());
        return builder;
    }

    public static ComplexPropertyBuilder<TEntity> DateTimeProperty<TEntity>(
        this ComplexPropertyBuilder<TEntity> builder,
        Expression<Func<TEntity, DateTime?>> propertyExpression)
        where TEntity : class
    {
        builder.ComplexProperty(propertyExpression, ConfigureDateTimeProperty());
        return builder;
    }
    
    public static ComplexPropertyBuilder<TEntity> DurationProperty<TEntity>(
        this ComplexPropertyBuilder<TEntity> builder,
        Expression<Func<TEntity, Duration?>> propertyExpression)
        where TEntity : class
    {
        builder.ComplexProperty(propertyExpression, duration =>
        {
            duration.Property(d => d.Weeks);
            duration.Property(d => d.Days);
            duration.Property(d => d.Hours);
            duration.Property(d => d.Minutes);
            duration.Property(d => d.Seconds);
        });
        return builder;
    }
    
    private static Action<ComplexPropertyBuilder<DateTime>> ConfigureDateTimeProperty()
    {
        return dateTime =>
        {
            dateTime.Property(dt => dt.CalDate);
            dateTime.Property(dt => dt.Time);
            dateTime.Property(dt => dt.TimeZone)
                .HasConversion(
                    tz => tz != null ? tz.Id : null,
                    tzId => tzId != null ? TimeZoneInfo.FindSystemTimeZoneById(tzId) : null);
            dateTime.Property(dt => dt.Value);
        };
    }
}
