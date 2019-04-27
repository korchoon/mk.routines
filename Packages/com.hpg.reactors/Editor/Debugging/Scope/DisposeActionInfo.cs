using System;
using Sirenix.OdinInspector;

namespace Lib.DataFlow
{
    [InlineProperty, HideLabel, HideReferenceObjectPicker]
    public class DisposeActionInfo
    {
        public StackTraceHolder St;

        public DisposeActionInfo(Action a)
        {
            St = StackTraceHolder.New(1);
        }
    }
}