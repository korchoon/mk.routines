using System;
using Lib.DataFlow;

namespace Lib.Pooling
{
    internal sealed class Pooled<T> : IDisposable where T : class
    {
        public T Value { get; private set; }
        readonly Action<T> _onRelease;

        public Pooled(T value, Action<T> onRelease)
        {
            Value = value;
            _onRelease = onRelease;
        }

        bool _isCompleted;

        public void Dispose()
        {
            if (_isCompleted)
                return;

            _isCompleted = true;

            if (Value == null)
                return;

            _onRelease?.Invoke(Value);
            (Value as IDisposable)?.Dispose();
            Value = default(T);
        }
    }
}