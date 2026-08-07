using CalendarApp.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CalendarApp.API.Infrastructure.EntityConfigurations;

public class EntryConfiguration : EntryConfigurationBase<Entry>
{
    protected override void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();
        
        builder.HasDiscriminator().IsComplete(false);
        
        builder.Property(e => e.Location);

        builder.ComplexProperty(e => e.GeographicPosition, geolocation =>
        {
            geolocation.IsRequired(false);
            geolocation.Property(g => g.Latitude);
            geolocation.Property(g => g.Longitude);
        });

        builder.ComplexProperty(e => e.Alarm, alarm =>
        {
            alarm.Property(a => a.Action);
            alarm.Property(a => a.Description);
            alarm.Property(a => a.Summary);
            alarm.ComplexProperty(a => a.Attachment, attachment =>
            {
                attachment.Property(a => a.Uri).HasConversion(
                    attach => attach != null ? attach.OriginalString : null,
                    attach => attach != null ? new Uri(attach, UriKind.RelativeOrAbsolute) : null);
                attachment.Property(a => a.Data);
            });
                
            alarm.ComplexProperty(a => a.Repeat, repeat =>
            {
                repeat.Property(r => r.Value);
                repeat.DurationProperty(t=> t.Duration);
            });
            
            alarm.ComplexProperty(a => a.Trigger, trigger =>
            {
                trigger.Property(t => t.Relation);
                trigger.DateTimeProperty(t => t.DateTime);
                trigger.DurationProperty(t=> t.Duration);
            });

            alarm.ComplexCollection(a => a.Attendees, attendee =>
            {
                attendee.Property(a => a.Email);
                attendee.Property(a => a.Name);
                attendee.Property(a => a.Rsvp);
                attendee.Property(a => a.Role);
                attendee.Property(a => a.ParticipationStatus);
            });
            
            alarm.ToJson();
        });

        builder.ComplexProperty(ce => ce.Recurrence, rule =>
        {
            rule.Property(r => r.Frequency);
            rule.Property(r => r.WeekStart);

            rule.ComplexProperty(r => r.Interval, interval =>
            {
                interval.Property(i => i.Value);
            });

            rule.ComplexProperty(r => r.End, end =>
            {
                end.DateTimeProperty(e => e.Until);
                end.ComplexProperty(e => e.Count, count =>
                {
                    count.Property(c => c.Value);
                });
            });

            rule.ComplexProperty(r => r.ByDay, byDay =>
            {
                byDay.ComplexCollection(day => day.Values, day =>
                {
                    day.Property(d => d.Value);
                    day.ComplexProperty(d => d.Ordinal);
                });
            });

            rule.ComplexProperty(r => r.ByMonthDay, byMonthDay => 
                byMonthDay.ComplexCollection(m => m.Value, monthday =>
                {
                    monthday.Property(m => m.Value);
                }));
            
            rule.ComplexProperty(r => r.ByMonth, byMonth => 
                byMonth.ComplexCollection(m => m.Value, month =>
                {
                    month.Property(m => m.Value);
                }));
            
            rule.ComplexProperty(r => r.ByYearDay, byYearDay => 
                byYearDay.ComplexCollection(y => y.Value, yearDay =>
                {
                    yearDay.Property(y => y.Value);
                }));
            
            rule.ComplexProperty(r => r.ByHour, byHour => 
                byHour.ComplexCollection(h => h.Value, hour =>
                {
                    hour.Property(h => h.Value);
                }));
            
            rule.ComplexProperty(r => r.ByMinute, byMinute => 
                byMinute.ComplexCollection(m => m.Value, minute =>
                {
                    minute.Property(m => m.Value);
                }));
            
            rule.ComplexProperty(r => r.BySecond, bySecond => 
                bySecond.ComplexCollection(s => s.Value, second =>
                {
                    second.Property(s => s.Value);
                }));
            
            rule.ComplexProperty(r => r.BySetPos, bySetPos => 
                bySetPos.ComplexCollection(p => p.Value, setPos =>
                {
                    setPos.Property(s => s.Value);
                }));
            
            rule.ComplexProperty(r => r.ByWeek, byWeek => 
                byWeek.ComplexCollection(w => w.Value, week =>
                {
                    week.Property(w => w.Value);
                }));

            rule.ToJson();
        });
        
        builder.ComplexProperty(e => e.RecurrenceIdentifier, recurrenceIdentifier =>
        {
            recurrenceIdentifier.DateTimeProperty(r => r.Start);
            recurrenceIdentifier.Property(r => r.Range);
        });
        
        builder.ComplexProperty(e => e.Sequence, sequence =>
        { 
            sequence.Property(s => s.Number); 
        } );
    }
}

