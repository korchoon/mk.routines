using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Lib.Utility;

namespace Lib.DataFlow
{
    internal static class Disp
    {
        public static IScope Sub(out IDisposable onDispose)
        {
            var t = new OneTimeSubs();
            onDispose = t;
            return t;
        }

        public static IDisposable Pub(out IScope onDispose)
        {
            var t = new OneTimeSubs();
            onDispose = t;
            return t;
        }

        public static IDisposable Scope(out IScope o) => Pub(out o);
    }
}