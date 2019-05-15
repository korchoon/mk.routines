using System;

namespace Lib.DataFlow
{
    public interface IScope
    {
        void OnDispose(Action dispose);
        void Unsubscribe(Action dispose);
    }


    public interface IScope<out T>
    {
        // todo rename to Subscribe
        void OnDispose(Action<T> onMsg);
    }
}