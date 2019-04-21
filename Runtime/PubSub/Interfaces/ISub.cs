using System;

namespace Lib.DataFlow
{
    public interface ISub
    {
        void OnNext(Func<bool> pub);
        void OnNext(Action pub, IScope sd);
    }

    public interface ISub<out T>
    {
        void OnNext(Func<T, bool> pub);
        void OnNext(Action<T> pub, IScope sd);
    }
}