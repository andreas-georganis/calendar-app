namespace Calendar.Domain.Exceptions;

public class CalendarDomainException : Exception
{
    public CalendarDomainException(string message) : base(message)
    {
    }

    public CalendarDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
