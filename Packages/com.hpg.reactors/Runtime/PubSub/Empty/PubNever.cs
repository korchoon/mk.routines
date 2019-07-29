// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

namespace Lib.DataFlow
{
    internal class PubNever : IPub
    {
        public static PubNever Never { get; } = new PubNever();
        public static PubNever Already { get; } = new PubNever();

        PubNever()
        {
        }

        public void Next()
        {
        }
    }

    internal class PubNever<T> : IPub<T>
    {
        public static PubNever<T> Never { get; } = new PubNever<T>();
        public static PubNever<T> Already { get; } = new PubNever<T>();


        PubNever()
        {
        }

        public void Next(T msg)
        {
        }
    }
}