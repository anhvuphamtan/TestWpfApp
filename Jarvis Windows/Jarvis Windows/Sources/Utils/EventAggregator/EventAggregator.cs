using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class EventAggregator
{
    public static event EventHandler<EventArgs> JarvisActionPositionChanged;

    // Notify Jarvis Action Position Changed to MenuOperatorsView.xaml.cs -> Close the Main Window if Jarvis Action is not in it.
    // Click Textbox in Menu -> Click Action. By default it will lose focus on the Main Window -> cannot execute Action.
    // Need to handle Lost Focus, in there we activate the Main Window again, but if the Main Window is not hide, it will show up in the foreground -> annoying.
    // Therefore, we close the Main Window if Jarvis Action is not in it (user not testing on the Main Window).
    // Publish in UIElementDetector.cs: _focusingElement.Current.AutomationID
    public static void PublishJarvisActionPositionChanged(object sender, EventArgs e)
    {
        JarvisActionPositionChanged?.Invoke(sender, e);
    }
}
