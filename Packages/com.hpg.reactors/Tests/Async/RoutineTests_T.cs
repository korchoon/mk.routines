// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Lib;
using Lib.Async;
using Lib.DataFlow;
using NUnit.Framework;
#pragma warning disable 4014
#pragma warning disable 1998

namespace AsyncTests.Async
{
    [TestFixture]
    public class RoutineTests_T
    {
        [Test]
        public void EmptyRoutine_Completed_Immediately()
        {
            var routine = Empty();
            var aw = routine.GetAwaiter();
            Assert.IsTrue(aw.IsCompleted);
            Assert.AreEqual(42, aw.GetResult());

            async Routine<int> Empty()
            {
                return 42;
            }
        }

        [Test]
        public void FirstContinuation_Invoked_Immediately()
        {
            var visited = false;
            Sample();
            Assert.IsTrue(visited);

            async Routine<int> Sample()
            {
                visited = true;
                await _Never.Instance;
                return 42;
            }
        }

        [Test]
        public void SubAwaiter_Flow_Completed_AfterNext()
        {
            using (React.Scope(out var scope))
            {
                var (pub, sub) = scope.PubSub();
                var aw = Single(sub).GetAwaiter();
                Assert.IsFalse(aw.IsCompleted);
                pub.Next();
                Assert.IsTrue(aw.IsCompleted);
                Assert.AreEqual(42, aw.GetResult());
            }

            async Routine<int> Single(ISub sub)
            {
                await sub;
                return 42;
            }
        }

        [Test]
        public void Dispose_DoesNotInvokeContinuation_SubAwaiter()
        {
            var r = Sample();
            r.Dispose();

            async Routine<int> Sample()
            {
                await _Never.Instance;
                Assert.Fail("Should not happen");
                return 42;
            }
        }

        [Test]
        public void Dispose_StopsUsualFlow_SubAwaiter()
        {
            using (React.Scope(out var scope))
            {
                var (pub, sub) = scope.PubSub();
                var r = Sample();
                r.GetAwaiter();
                r.Dispose();
                pub.Next();

                async Routine<int> Sample()
                {
                    await sub;
                    Assert.Fail("Should not happen");
                    return 42;
                }
            }
        }


        [Test]
        public void OuterRoutine_Dispose_Breaks_Inner()
        {
            Routine<int> innerClosure = null;
            var outer = Outer();
            var innerAwaiter = innerClosure.GetAwaiter();
            outer.Dispose();
            Assert.IsTrue(innerAwaiter.IsCompleted);

            async Routine<int> Outer()
            {
                innerClosure = Inner();
                var res = await innerClosure;
                return res;
            }

            async Routine<int> Inner()
            {
                await _Never.Instance;
                return 42;
            }
        }

        [Test]
        public void GetScope_DoesntBreakRoutine()
        {
            using (React.Scope(out var scope))
            {
                Routine<int> closure = null;
                var r = Outer();
                var aw = closure.GetAwaiter();
                r.Dispose();
                Assert.IsFalse(aw.IsCompleted);

                async Routine<int> Outer()
                {
                    closure = Inner();
                    await closure.GetScope(scope);
                    return 42;
                }

                async Routine<int> Inner()
                {
                    await _Never.Instance;
                    return 42;
                }
            }
        }

        [Test]
        public void Dispose_AfterCompleted_DoesNothing()
        {
            using (React.Scope(out var scope))
            {
                var (pub, sub) = scope.PubSub();
                var r = Sample();
                pub.Next();
                var aw = r.GetAwaiter();
                int i = 0;
                aw.OnCompleted(() => ++i);
                Assert.AreEqual(1, i);
                Assert.IsTrue(aw.IsCompleted);
                Assert.AreEqual(42, aw.GetResult());


                r.Dispose();
                Assert.AreEqual(1, i);


                async Routine<int> Sample()
                {
                    await sub;
                    return 42;
                }
            }
        }

        [Test]
        public void DisposedRoutine_Awaiter_IsCompleted()
        {
            var r = Sample();
            r.Dispose();
            var aw = r.GetAwaiter();
            Assert.IsTrue(aw.IsCompleted);

            async Routine<int> Sample()
            {
                await _Never.Instance;
                return 42;
            }
        }

        [Test]
        public void CompletedRoutine_Awaiter_IsCompleted()
        {
            using (React.Scope(out var scope))
            {
                var (pub, sub) = scope.PubSub();
                var r = Sample();
                pub.Next();
                var aw = r.GetAwaiter();
                Assert.IsTrue(aw.IsCompleted);
                Assert.AreEqual(42, aw.GetResult());

                async Routine<int> Sample()
                {
                    await sub;
                    return 42;
                }
            }
        }

        [Test]
        public void Scope_Ends_Before_GetAwaiter_Continues()
        {
            using (React.Scope(out var outer))
            {
                var (pub, sub) = outer.PubSub();

                IScope r1Scope = default;

                bool completed = false;
                var aw = Routine1().GetAwaiter();
                aw.OnCompleted(Asserts);
                pub.Next();

                void Asserts()
                {
                    Assert.IsNotNull(r1Scope);
                    Assert.IsTrue(r1Scope.Disposing);
                    Assert.IsFalse(r1Scope.Completed);
                    Assert.IsTrue(completed);
                }

                async Routine<int> Routine1()
                {
                    r1Scope = await Routine.SelfScope();
                    r1Scope.Subscribe(() => completed = true);

                    await sub;
                    return 42;
                }
            }
        }


        class _Never : ISub
        {
            public static _Never Instance { get; } = new _Never();

            _Never()
            {
            }

            public void OnNext(Action pub, IScope scope)
            {
            }
        }
    }
}