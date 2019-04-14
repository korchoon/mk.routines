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

        public static IPubSubDispose PubSub()
        {
            var p = Pub(out var sub);
            return new PubSubDisp() {Pub = p, Scope = sub};
        }

        class PubSubDisp : IPubSubDispose
        {
            public void Dispose() => Pub.Dispose();

            public IDisposable Pub { get; set; }
            public IScope Scope { get; set; }
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