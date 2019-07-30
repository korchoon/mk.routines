// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;

namespace Lib.DataFlow
{
    public class EmptyException : Exception
    {
        EmptyException()
        {
        }
        
        public static EmptyException Empty { get; } = new EmptyException();
    }
}