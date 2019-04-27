using System;

namespace Lib.DataFlow
{
    public interface IScope
    {
        // todo rename to Subscribe
        void OnDispose(Action dispose);
    }

    public interface IScope<out T>
    {
        // todo rename to Subscribe
        void OnDispose(Action<T> onMsg);
    }
}