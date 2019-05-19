using System;

namespace Lib.DataFlow
{
    public interface ISub
    {
        void OnNext(Action pub, IScope scope);
    }

    public interface ISub<out T>
    {
        void OnNext(Action<T> pub, IScope scope);
    }
}