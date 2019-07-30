// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

namespace Lib.DataFlow
{
    public interface IPubIterator
    {
        bool Next();
    }

    public interface IPubIterator<in T>
    {
        bool Next(T msg);
    }
}