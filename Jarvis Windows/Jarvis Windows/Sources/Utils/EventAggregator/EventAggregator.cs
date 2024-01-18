using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.Utils.EventAggregator;

public static class EventAggregator
{
    public static event EventHandler<EventArgs> LanguageSelectionChanged;
    
    public static void PublishLanguageSelectionChanged(object sender, EventArgs e)
    {
        LanguageSelectionChanged?.Invoke(sender, e);
    }

}
