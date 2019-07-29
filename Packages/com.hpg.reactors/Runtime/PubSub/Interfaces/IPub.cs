// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

namespace Lib.DataFlow
{
    public interface IPub
    {
        void Next();
    }

    public interface IPub<in T>
    {
        void Next(T msg);
    }
}