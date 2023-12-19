using System;
using JetBrains.Annotations;
using Mk.Debugs;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Mk.Routines {
#line hidden
    public static class RoutineApi {
        static float Time {
            get {
#if UNITY_EDITOR
                if (Application.isPlaying) {
                    return UnityEngine.Time.realtimeSinceStartup;
                }

                return (float)EditorApplication.timeSinceStartup;
#else
                return UnityEngine.Time.realtimeSinceStartup;
#endif
            }
        }

        // todo replace with local timer component
        public static TryAwaiter GetAwaiter (this float seconds) {
            var endTime = Time + seconds;
            return Routine.When (() => Time >= endTime);
        }

        public static void DisposeAndUpdateParent<T> (this T r) where T : IRoutine {
            r.Dispose ();
            r.UpdateParent ();
        }

        [MustUseReturnValue]
        public static ProxyAwaiter Configure (this IRoutine r
            , Func<bool> doWhile = default
            , Action beforeUpdate = default
            , Action afterUpdate = default
            , Action beforeDispose = default
            , Action onBreak = default
            , Action<IRollback> onStart = default
        ) {
            Asr.IsFalse (typeof (Routine<>).IsAssignableFrom (r.GetType ()));
            return new ProxyAwaiter () {
                DoWhile = doWhile,
                Main = r,
                BeforeDispose = beforeDispose,
                BeforeUpdate = beforeUpdate,
                AfterUpdate = afterUpdate,
                OnBreak = onBreak,
                OnStart = onStart,
            };
        }

        [MustUseReturnValue]
        public static ProxyAwaiter Configure (this IRoutine main
            , ProxyAwaiter arg) {
            Asr.IsTrue (arg.Main == null);
            Asr.IsFalse (typeof (Routine<>).IsAssignableFrom (main.GetType ()));
            arg.Main = main;
            return arg;
        }


        public static ProxyAwaiter BreakOn (this IRoutine routine, Func<bool> breakOn, Action onBreak = default) =>
            routine.Configure (doWhile: () => !breakOn (), onBreak: onBreak);

        public static async Routine ToRoutine (this IRoutine routine) {
            var rollback = await Routine.GetRollback ();
            rollback.Defer (routine.Dispose);
            await Routine.When (() => {
                routine.Tick ();
                return routine.IsCompleted;
            });
        }


        public static Routine<Option<T>> BreakOn<T> (this IRoutine<T> routine, Func<bool> breakOn) => routine.While (() => !breakOn ());

        public static Routine<Option<T>> BreakOn<T> (this IRoutine<T> routine, IRoutine breakOn) => routine.While (() => {
            breakOn.Tick ();
            return !breakOn.IsCompleted;
        });

        public static async Routine<T> BreakOn<T> (this IRoutine<T> routine, Func<bool> breakOn, T onBreak, Action onBreakAction = default) {
            var t = await routine.While (() => !breakOn ());
            if (t.TryGet (out var value)) {
                return value;
            }

            onBreakAction?.Invoke ();
            return onBreak;
        }

        public static Routine<Option<T>> While<T> (this IRoutine<T> routine, Func<bool> doWhile) {
            return inner ();

            async Routine<Option<T>> inner () {
                var rollback = await Routine.GetRollback ();
                rollback.Defer (routine.Dispose);
                while (true) {
                    routine.Tick ();
                    if (routine.IsCompleted) {
                        return routine.GetResult ();
                    }

                    if (!doWhile.Invoke ()) {
                        return default;
                    }

                    await Routine.Yield;
                }
            }
        }

        [MustUseReturnValue]
        public static ProxyAwaiter<T> Configure<T> (this IRoutine<T> r
            , Action beforeUpdate = default
            , Action afterUpdate = default
            , Action beforeDispose = default
        ) =>
            new ProxyAwaiter<T> {
                MainRoutine = r,
                BeforeDisposeAction = beforeDispose,
                BeforeUpdateAction = beforeUpdate,
                AfterUpdateAction = afterUpdate,
            };
    }
#line default
}