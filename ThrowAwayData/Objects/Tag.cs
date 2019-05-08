using System.Collections.Generic;
using ThrowAwayDataBackground;

namespace ThrowAwayData
{
    public class Tag : BaseObject
    {
        public string Name { get; set; }
        public IEnumerable<Thread> Thread { get; set; }
    }
}
