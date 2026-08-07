namespace Calendar.Domain.Model;

public sealed class OccurrenceCalculator<T> where T : Recurrable
{
    public IEnumerable<T> For(T recurrable, DateTime start, DateTime end)
    {
       return default;
    }
}