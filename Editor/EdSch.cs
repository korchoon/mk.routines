using Lib.Async;
using Lib.DataFlow;

namespace Unknown
{
    public static class EdSch
    {
        public static IScope Scope => Sch.Scope;
        public static ISub Update => Sch.Update;

        public static ISub Gizmos { get; internal set; }
        public static ISub Handles { get; internal set; }
    }
}