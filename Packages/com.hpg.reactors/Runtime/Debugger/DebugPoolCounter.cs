// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;

namespace Lib.Pooling
{
    [Serializable]
    public class DebugPoolCounter
    {
        int _total;
        int _inactive;

        public void Release()
        {
            ++_inactive;
        }

        public void Get()
        {
            --_inactive;
        }

        public void New()
        {
            ++_total;
        }

        public override string ToString() => $"{_inactive}/{_total}";
    }
}