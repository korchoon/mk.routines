using System;

namespace Lib.DataFlow
{
    public interface IScope
    {
        bool Completed { get; }
        void OnDispose(Action dispose);
        void Unsubscribe(Action dispose);
    }
}