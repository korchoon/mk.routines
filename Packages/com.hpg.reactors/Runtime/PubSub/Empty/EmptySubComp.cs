﻿using System;
using System.Runtime.CompilerServices;
using Lib.Async;

namespace Lib.DataFlow
{
    internal class ScopeNever : IScope
    {
        public static ScopeNever Never { get; } = new ScopeNever(true);
        public static ScopeNever Already { get; } = new ScopeNever(true);

        ScopeNever(bool completed)
        {
            Completed = completed;
        }

        public bool Completed { get; }

        public void OnDispose(Action dispose)
        {
        }

        public void Unsubscribe(Action dispose)
        {
        }
    }
}