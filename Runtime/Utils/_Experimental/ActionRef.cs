using System;
using UnityEngine.Events;

namespace Lib
{
    // overcome ref in delegates restriction
    public delegate ref UnityAction UnityActionRef();
    public delegate ref Action ActionRef();

    // overcome ref in delegates restriction
    public delegate ref Action<T> ActionRef<T>();
}