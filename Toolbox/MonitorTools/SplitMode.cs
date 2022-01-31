using System;
using System.Collections.Generic;
using System.Text;

namespace aemarcoCommons.Toolbox.MonitorTools
{
    [Flags]
    public enum SplitMode
    {
        None = 0, // 0
        Horizontal = 1, //1
        Vertical = 1 << 1, //2
    }
}
