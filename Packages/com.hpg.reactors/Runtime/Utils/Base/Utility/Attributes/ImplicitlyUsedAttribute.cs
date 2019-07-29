// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;

namespace Lib.EditorLib.Inspector
{
    /// <summary>
    /// Goal is strictly informative: report developer to know where to find implicit dependencies
    /// i.e. referenced by UnityEvent, 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ImplicitlyUsedAttribute : Attribute
    {
        public ImplicitlyUseKind Kind;
        public ImplicitlyUsedAttribute(ImplicitlyUseKind kind)
        {
            Kind = kind;
        }
    }

    public enum ImplicitlyUseKind
    {
        Other,
        UnityEvent,
        NetworkRpc,
    }
}