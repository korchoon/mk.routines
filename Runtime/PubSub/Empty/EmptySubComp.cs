using System;
using System.Runtime.CompilerServices;
using Lib.Async;

namespace Lib.DataFlow
{
    internal class ScopeNever : IScope
    {
        public static ScopeNever Ever { get; } = new ScopeNever();

        ScopeNever()
        {
        }

        public void OnDispose(Action dispose)
        {
        }

        public void Unsubscribe(Action dispose)
        {
        }
    }
}