using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Lib;
using Lib.Async;
using Lib.DataFlow;
using Lib.UnitTests;
using Lib.Utility;
using NUnit.Framework;

namespace Game.Tests
{
    public class RoutineTests2
    {
        [Test]
        public void TestFlow()
        {
            using (React.Scope(out var scope))
            {
                var assert1 = Empty.Action();
                var assert2 = Empty.Action();
                var assert3 = Empty.Action();

                TestTracer.OnNew += evt =>
                {
                    TestUtils.Track(ref evt.Break1, ref assert1);
                    TestUtils.Track(ref evt.Break2, ref assert1, 0);

                    TestUtils.Track(ref evt.Break1, ref assert2);
                    TestUtils.Track(ref evt.Break2, ref assert2);
                    TestUtils.Track(ref evt.Break3, ref assert2);
                    TestUtils.Track(ref evt.Break4, ref assert2, 0);

                    TestUtils.Track(ref evt.Break1, ref assert3);
                    TestUtils.Track(ref evt.Break2, ref assert3);
                    TestUtils.Track(ref evt.Break3, ref assert3);
                    TestUtils.Track(ref evt.Break4, ref assert3);
                    TestUtils.Track(ref evt.BreakBool, ref assert3, new[] {Option.Some(true)});
                };


                var (pubSch, sch) = React.Channel(scope);
                TestTracer.Register(sch);
                Outer(sch).Scope(scope);

                assert1.Invoke();

                pubSch.Next();
                assert2.Invoke();

                pubSch.Next();
                assert3.Invoke();
            }
        }

        [Test]
        public void TestFlow_Break1()
        {
            using (var brk = React.Scope(out var scope))
            {
                var assert1 = Empty.Action();

                TestTracer.OnNew += evt =>
                {
                    TestUtils.Track(ref evt.Break1, ref assert1);
                    TestUtils.Track(ref evt.Break2, ref assert1, 0);
                    TestUtils.Track(ref evt.Break3, ref assert1, 0);
                    TestUtils.Track(ref evt.Break4, ref assert1, 0);
                    TestUtils.Track(ref evt.BreakBool, ref assert1, new Option<bool>[0]);
                };


                var (pubSch, sch) = React.Channel(scope);
                TestTracer.Register(sch);
                Outer(sch).Scope(scope);

                brk.Dispose();
                assert1.Invoke();
            }
        }

        [Test]
        public void TestFlow_Break2()
        {
            using (var brk = React.Scope(out var scope))
            {
                var assert1 = Empty.Action();

                TestTracer.OnNew += evt =>
                {
                    TestUtils.Track(ref evt.Break1, ref assert1);
                    TestUtils.Track(ref evt.Break2, ref assert1);
                    TestUtils.Track(ref evt.Break3, ref assert1);
                    TestUtils.Track(ref evt.Break4, ref assert1, 0);
                    TestUtils.Track(ref evt.BreakBool, ref assert1, new Option<bool>[0]);
                };


                var (pubSch, sch) = React.Channel(scope);
                TestTracer.Register(sch);
                Outer(sch).Scope(scope);
                pubSch.Next();
                brk.Dispose();
                assert1.Invoke();
            }
        }

        [Test]
        public void TestFlow_Break3()
        {
            using (var brk = React.Scope(out var scope))
            {
                var assert1 = Empty.Action();

                TestTracer.OnNew += evt =>
                {
                    TestUtils.Track(ref evt.Break1, ref assert1);
                    TestUtils.Track(ref evt.Break2, ref assert1);
                    TestUtils.Track(ref evt.Break3, ref assert1);
                    TestUtils.Track(ref evt.Break4, ref assert1);
                    TestUtils.Track(ref evt.BreakBool, ref assert1, new []{Option.Some(true)});
                };


                var (pubSch, sch) = React.Channel(scope);
                TestTracer.Register(sch);
                Outer(sch).Scope(scope);
                pubSch.Next();
                pubSch.Next();
                brk.Dispose();
                assert1.Invoke();
            }
        }


        static async Routine Outer(ISub sch)
        {
            TestTracer.Next(t => t.Break1, sch);
            await sch;
            TestTracer.Next(t => t.Break2, sch);
            var result = await BoolRoutine(sch);
            TestTracer.Next(t => t.BreakBool, result, sch);
        }

        static async Routine<Option<bool>> BoolRoutine(ISub sch)
        {
            TestTracer.Next(t => t.Break3, sch);
            await sch;
            TestTracer.Next(t => t.Break4, sch);
            return true;
        }

        class TestTracer : DebugTracer<TestTracer, ISub>
        {
            public Action Break1;
            public Action Break2;
            public Action Break3;
            public Action Break4;
            public Action<Option<bool>> BreakBool;
        }
    }

    public class TestUtils
    {
        public static void Track<T>(ref Action<T> onNext, ref Action onAssert, T[] target) where T : IEquatable<T>
        {
            var frame = new StackTrace(true).GetFrame(1);
            var sourceLine = frame.SourceLine();
            var queue = new Queue<T>();
            onNext += t => queue.Enqueue(t);
            onAssert += () =>
            {
                Assert.IsTrue(target.Length == queue.Count, sourceLine);

                foreach (var t in target)
                {
                    Assert.IsTrue(queue.Count > 0, sourceLine);
                    var q = queue.Dequeue();
                    Assert.IsTrue(q.Equals(t), sourceLine);
                }
            };
        }

        public static void Track(ref Action onNext, ref Action onAssert, int targetCount = 1)
        {
            var frame = new StackTrace(true).GetFrame(1);
            var sourceLine = frame.SourceLine();
            var items = 0;
            onNext += () => ++items;
            onAssert += () => { Assert.IsTrue(items == targetCount, sourceLine); };
        }
    }
}