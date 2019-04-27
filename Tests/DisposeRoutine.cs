using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lib;
using Lib.Async;
using Lib.DataFlow;
using Lib.UnitTests;
using NUnit.Framework;
 
namespace Game.Tests
{
    public class RoutineTest
    {
        class TooSlowException : Exception
        {
        }

        class Ctx
        {
            public ISub Scheduler { get; }
            public TestCheckpoint<Check> Checkpoint { get; }

            internal HashSet<Routine> _SubRoutines { get; }

            public Routine Add(Routine r)
            {
                _SubRoutines.Add(r);
                return r;
            }

            public Ctx(out IPub timer, IScope scope)
            {
                (timer, Scheduler) = React.Channel(scope);
                Checkpoint = new TestCheckpoint<Check>();
                _SubRoutines = new HashSet<Routine>();
            }
        }


        [Test]
        public void DisposeTest()
        {
#if M_REFACTOR
            using (React.Scope(out var scope))
            {
                var ctx = new Ctx(out var timer, scope);

                var task = ctx.Add(AsyncRoot(ctx));
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                while (!task._isCompleted)
                {
                    timer.Next();
                    if (stopwatch.Elapsed.TotalSeconds > 5)
                        throw new TooSlowException();
                }

                Assert.IsTrue(task._isCompleted);
                Assert.IsTrue(ctx.Checkpoint.VisitedAllExcept(out var err, Check.DENY_ContinueBreakable, Check.DENY_ContinueSubTask2), err);
                Assert.IsTrue(ctx.Checkpoint.Count(Check.BeforYieldSubTask3) > ctx.Checkpoint.Count(Check.AfterYieldSubTask3));
                foreach (var routine in ctx._SubRoutines)
                {
                    Assert.IsTrue(routine._isCompleted);
                }
            }
#endif
        }


        enum Check
        {
            DisposePub,
            AfterBreakable,
            DENY_ContinueBreakable,
            FinallyBreakable,
            FinallySubTask,
            DENY_ContinueSubTask2,
            FinallySubTask2,
            BeforYieldSubTask3,
            AfterYieldSubTask3,
            FinalizeSubTask3
        }

        static async Routine AsyncRoot(Ctx ctx)
        {
            var dispose = React.Scope(out var scope);
            var br = Breakable(ctx).DisposeOn(scope);
            ctx.Add(br);

            await GetAwaiters.Delay(1, ctx.Scheduler);


            ctx.Checkpoint.Visit(Check.DisposePub);
            dispose.Dispose();

            await br;
            ctx.Checkpoint.Visit(Check.AfterBreakable);
        }

        static async Routine Breakable(Ctx ctx)
        {
            try
            {
                await ctx.Add(SubTask(ctx));
                ctx.Checkpoint.Visit(Check.DENY_ContinueBreakable);
            }
            finally
            {
                ctx.Checkpoint.Visit(Check.FinallyBreakable);
            }
        }

        static async Routine SubTask(Ctx ctx)
        {
            try
            {
                await ctx.Add(SubTask2(ctx));
            }
            finally
            {
                ctx.Checkpoint.Visit(Check.FinallySubTask);
            }
        }

        static async Routine SubTask2(Ctx ctx)
        {
            try
            {
                await ctx.Add(SubTask3(ctx));
                ctx.Checkpoint.Visit(Check.DENY_ContinueSubTask2);
                await ctx.Add(SubTask3(ctx));
            }
            finally
            {
                ctx.Checkpoint.Visit(Check.FinallySubTask2);
            }
        }

        static async Routine SubTask3(Ctx ctx)
        {
            try
            {
                while (true)
                {
                    ctx.Checkpoint.Visit(Check.BeforYieldSubTask3);
                    await GetAwaiters.DelayEditor(0.5f, ctx.Scheduler);
                    ctx.Checkpoint.Visit(Check.AfterYieldSubTask3);
                }
            }
            finally
            {
                ctx.Checkpoint.Visit(Check.FinalizeSubTask3);
            }
        }
    }
}