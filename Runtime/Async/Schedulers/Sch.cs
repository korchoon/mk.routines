using System;
using Lib.DataFlow;
using Lib.Utility;

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
        public static IScope Scope { get; internal set; }

        #endregion

        public static bool QuittingApp => ScheduleRunner.WantsQuit;

        public static bool TryInit(Option<ScheduleSettings> settings = default) => ScheduleRunner.TryInit(settings);

        public static ISub LateUpdate { get; internal set; }
        public static ISub<float> UpdateTime { get; set; }

        public static ISub Physics { get; internal set; }
        public static ISub<float> PhysicsTime { get; internal set; }

        public static ISub Logic { get; internal set; }
    }

    public static class EdSch
    {
        public static IScope Scope => Sch.Scope;
        public static ISub Update => Sch.Update;

        public static bool IsGizmo { get; internal set; }


        public static ISub Gizmos { get; internal set; }
        public static ISub Handles { get; internal set; }
    }
}