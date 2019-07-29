// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

namespace Lib.Utility
{
    public interface IOption
    {
        bool HasValue { get; }
    }
    public interface IOption<T>
    {
        bool TryGet(out T value);
    }
}