using System;

namespace Lib.DataFlow
{
    public interface ISubIterator
    {
        void OnNext(Func<bool> pub);
        void OnNext(IPubIterator pub);
    }

    public interface ISubIterator<out T>
    {
        void OnNext(Func<T, bool> pub);
        void OnNext(IPubIterator<T> pub);
    }
}