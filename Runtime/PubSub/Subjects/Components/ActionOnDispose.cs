using System;
using Game.Proto;
using JetBrains.Annotations;
using Lib.DataFlow;

namespace Lib.Utility
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