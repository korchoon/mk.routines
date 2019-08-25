﻿// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Game.Proto;
using JetBrains.Annotations;
using Reactors.DataFlow;

namespace Reactors.Utility
{
    internal class ActionOnDispose : IDisposable
    {
        Action _onDispose;

        public ActionOnDispose(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            if (_isCompleted) return;

            _isCompleted = true;
            _onDispose?.Invoke();
            _onDispose = null;
        }

        bool _isCompleted;
    }
}