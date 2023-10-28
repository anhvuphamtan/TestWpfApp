using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWindows.Sources.Utils.WindowsNative
{
    public class WindowsInjection
    {
        public WindowsInjection()
        {
            
        }

        public IntPtr GetForegroundWindow() {  return IntPtr.Zero; }
    }
}

