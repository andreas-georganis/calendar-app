using System.Collections;
using Calendar.Contracts;

namespace Calendar.Web.Client;

public class StateContainer 
{
    public IEnumerable<Calendar> ActiveCalendars
    {
        get => field?? [];
        set
        {
            field = value;
            NotifyStateChanged();
        }
    }

    public event Action? OnChange;

    private void NotifyStateChanged()
        => OnChange?.Invoke();
}
