// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;

namespace Reactors.DataFlow
{
    internal class DisposableNever : IDisposable
    {
        public static DisposableNever Ever { get; } = new DisposableNever();

        public void Dispose()
        {
        }
    }
}