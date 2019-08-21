// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using Lib.DataFlow;
using Lib.Utility;

[assembly: InternalsVisibleTo("Lib.Reactors.Editor")]

namespace Lib.Async
{
    public static class SchPub
    {
        internal static IPub<Exception> PubError;
    }

    public static class Sch
    {
        #region Editor, Playmode, Player

        public static ISub<Exception> OnError { get; internal set; }
        public static ISub Update { get; internal set; }
        internal static IScope Scope { get; set; }

        #endregion

        public static bool TryInit(Option<ScheduleSettings> settings = default) => ScheduleRunner.TryInit(settings);

        public static ISub LateUpdate { get; internal set; }
        public static ISub<float> UpdateTime { get; set; }

        public static ISub Physics { get; internal set; }
        public static ISub<float> PhysicsTime { get; internal set; }

        public static ISub Logic { get; internal set; }
    }
}