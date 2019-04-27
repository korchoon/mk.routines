using System;

namespace Game.Proto
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.All | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true )]
    public class TodoAttribute : Attribute
    {
    }
}