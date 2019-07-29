// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

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