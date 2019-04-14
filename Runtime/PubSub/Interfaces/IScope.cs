using System;

namespace Lib.DataFlow
{
    public interface IScope<out T>
    {
        void OnDispose(Action<T> onMsg);
    }
}