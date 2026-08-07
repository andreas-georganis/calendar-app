namespace Calendar.Domain.Model;

#pragma warning disable IDE1006 // Naming Styles
public interface Recurrable
#pragma warning restore IDE1006 // Naming Styles
{
    DateTime? Start { get; }

    RecurrenceRule? RecurrenceRule { get; }

    ExceptionDates? ExceptionDates { get; }

    RecurrenceDates? RecurrenceDates { get; }

    RecurrencePeriods? RecurrencePeriods { get; }

    RecurrenceIdentifier? RecurrenceIdentifier { get; }
}