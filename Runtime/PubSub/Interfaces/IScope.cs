using System;

namespace Lib.DataFlow
{
    public interface IScope
    {
        void OnDispose(Action dispose);
    }

    public interface IScope<out T>
    {
        void OnDispose(Action<T> onMsg);
    }
}