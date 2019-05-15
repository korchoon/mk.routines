using System;

namespace Lib.DataFlow
{
    public interface ISub
    {
        // todo rename to Subscribe
        void OnNext(Func<bool> pub);
        // todo rename to Subscribe
        void OnNext(Action pub, IScope scope);
    }

    public interface ISub<out T>
    {
        // todo rename to Subscribe
        void OnNext(Func<T, bool> pub);
        // todo rename to Subscribe
        void OnNext(Action<T> pub, IScope scope);
    }
}