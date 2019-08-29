// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class MustUseReturnValueAttribute : Attribute
    {
        [CanBeNull] public string Justification { get; private set; }

        public MustUseReturnValueAttribute()
        {
        }

        public MustUseReturnValueAttribute([NotNull] string justification)
        {
            this.Justification = justification;
        }
    }
}