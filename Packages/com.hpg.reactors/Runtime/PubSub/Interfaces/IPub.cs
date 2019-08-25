﻿// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

namespace Reactors.DataFlow
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