using System;

namespace Lib.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class NonPerformant : Attribute
    {
        public PerfKind KindValue;
        public NonPerformant(PerfKind kind = PerfKind.Other)
        {
            KindValue = kind;
        }
    }

    [Flags]
    public enum PerfKind
    {
        Other = 0,
        GC = 1 << 1,
        TimeHeavy = 1 << 2,
        Deserialization = 1 << 3,
        MemoryHeavy = 1 << 4,
        PreventsGC = 1 << 5,
        Recursive = 1 << 6,
    }
}