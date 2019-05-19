using System;

namespace Lib.DataFlow
{
     interface IErrorScope<out T>
    {
        // todo rename to Subscribe
        void OnDispose(Action<T> onMsg);
    }
}