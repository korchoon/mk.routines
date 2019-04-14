using System;
using System.Runtime.CompilerServices;

namespace Lib.DataFlow
{
    internal interface IPubSubDispose 
    {
        IDisposable Pub { get; }
        IScope Scope { get; }
    }
}