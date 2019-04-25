using System;
using Lib;
using Lib.Async;
using Lib.DataFlow;
using Lib.Utility;
using NUnit.Framework;

namespace Game.Tests
{
    public class RoutineTests3
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
                    TestUtils.Track(ref evt.Outer1, ref assert1);
                    TestUtils.Track(ref evt.Outer2, ref assert1, 0);

                    TestUtils.Track(ref evt.Outer1, ref assert2);
                    TestUtils.Track(ref evt.Outer2, ref assert2);
                    TestUtils.Track(ref evt.Inner1, ref assert2);
                    TestUtils.Track(ref evt.Inner2, ref assert2, 0);

                    TestUtils.Track(ref evt.Outer1, ref assert3);
                    TestUtils.Track(ref evt.Outer2, ref assert3);
                    TestUtils.Track(ref evt.Inner1, ref assert3);
                    TestUtils.Track(ref evt.Inner2, ref assert3);
                    TestUtils.Track(ref evt.InnerInner1, ref assert3);
                    TestUtils.Track(ref evt.InnerInner2, ref assert3);
                    TestUtils.Track(ref evt.OuterBool, ref assert3, new[] {true.Some()});
                };


                var (pubSch, sch) = React.Channel(scope);
                TestTracer.Register(sch);
                Outer(sch).Scope(scope);

                assert1.Invoke();

                pubSch.Next();
                assert2.Invoke();

                pubSch.Next();
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
                    TestUtils.Track(ref evt.Outer1, ref assert1);
                    TestUtils.Track(ref evt.Outer2, ref assert1, 0);
                    TestUtils.Track(ref evt.Inner1, ref assert1, 0);
                    TestUtils.Track(ref evt.InnerInner1, ref assert1, 0);
                    TestUtils.Track(ref evt.InnerInner2, ref assert1, 0);
                    TestUtils.Track(ref evt.Inner2, ref assert1, 0);
                    TestUtils.Track(ref evt.OuterBool, ref assert1, new Option<bool>[0]);
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
                    TestUtils.Track(ref evt.Outer1, ref assert1);
                    TestUtils.Track(ref evt.Outer2, ref assert1);
                    TestUtils.Track(ref evt.Inner1, ref assert1);
                    TestUtils.Track(ref evt.InnerInner1, ref assert1);
                    TestUtils.Track(ref evt.InnerInner2, ref assert1, 0);
                    TestUtils.Track(ref evt.Inner2, ref assert1, 0);
                    TestUtils.Track(ref evt.OuterBool, ref assert1, new Option<bool>[0]);
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
                    TestUtils.Track(ref evt.Outer1, ref assert1);
                    TestUtils.Track(ref evt.Outer2, ref assert1);
                    TestUtils.Track(ref evt.Inner1, ref assert1);
                    TestUtils.Track(ref evt.InnerInner1, ref assert1);
                    TestUtils.Track(ref evt.InnerInner2, ref assert1);
                    TestUtils.Track(ref evt.Inner2, ref assert1);
                    TestUtils.Track(ref evt.OuterBool, ref assert1, new Option<bool>[0]);
                    TestUtils.Track(ref evt.OuterOuter, ref assert1);
                };


                var (pubSch, sch) = React.Channel(scope);
                TestTracer.Register(sch);
                OuterOuter(s =>
                {
                    var res =Outer(s);
                    res.Scope(scope);
                    return res;
                }, sch);
                pubSch.Next();
                pubSch.Next();
                brk.Dispose();
                assert1.Invoke();
            }
        }


        static async Routine OuterOuter(Func<ISub, Routine> inner, ISub sch)
        {
            await inner.Invoke(sch);
            TestTracer.Next(t => t.OuterOuter, sch);
        }
        
        static async Routine Outer(ISub sch)
        {
            TestTracer.Next(t => t.Outer1, sch);
            await sch;
            TestTracer.Next(t => t.Outer2, sch);
            var result = await Inner(sch);
            TestTracer.Next(t => t.OuterBool, result, sch);
        }

        static async Routine<Option<bool>> Inner(ISub sch)
        {
            TestTracer.Next(t => t.Inner1, sch);
            await InnerInner(sch);
            TestTracer.Next(t => t.Inner2, sch);
            await sch;
            return true;
        }

        static async Routine InnerInner(ISub sch)
        {
            TestTracer.Next(t => t.InnerInner1, sch);
            await sch;
            TestTracer.Next(t => t.InnerInner2, sch);
        }

        class TestTracer : DebugTracer<TestTracer, ISub>
        {
            public Action Outer1;
            public Action Outer2;
            public Action Inner1;
            public Action Inner2;
            public Action InnerInner1;
            public Action InnerInner2;
            public Action<Option<bool>> OuterBool;
            public Action OuterOuter;
        }
    }
}