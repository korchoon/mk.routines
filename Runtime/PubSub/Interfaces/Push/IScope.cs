using System;
using Lib.Async;

namespace Lib.DataFlow
{
    public interface IScope
    {
        void OnDispose(Action dispose);
    }
}