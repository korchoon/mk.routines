using System;

namespace Lib.DataFlow
{
    public static class ActionEmpty<T>
    {
        public static readonly Action<T> Empty = _ => { };
    }

    public static class ActionEmpty
    {
        public static readonly Action Empty = () => { };
    }
}