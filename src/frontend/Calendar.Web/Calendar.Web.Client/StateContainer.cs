using System.Collections;

namespace Calendar.Web.Client;

public class StateContainer 
{
    public IEnumerable<object> ActiveCalendars
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
