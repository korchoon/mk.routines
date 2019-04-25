using System;
using System.Diagnostics;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

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