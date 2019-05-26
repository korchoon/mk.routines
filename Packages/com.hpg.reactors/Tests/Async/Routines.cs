using System;
using Lib;
using Lib.Async;
using Lib.DataFlow;
using NUnit.Framework;

namespace AsyncTests.Async
{
    [TestFixture]
    public class Routines
    {
        static async Routine Empty()
        {
        }

        static async Routine Single(Action a, ISub sub)
        {
            a.Invoke();
            await sub;
        }

        [Test]
        public void EmptyCompletesImmediately()
        {
            var routine = Empty();
            var aw = routine.GetAwaiter();
            Assert.IsTrue(aw.IsCompleted);
        }

        [Test]
        public void FirstContinuationInvokedImmediately()
        {
            using (React.Scope(out var scope))
            {
                var (pub, sub) = scope.PubSub();
                var visited = false;
                Single(() => visited = true, sub);
                Assert.IsTrue(visited);
            }
        }

        [Test]
        public void CompletedAfterFirstSub()
        {
            using (React.Scope(out var scope))
            {
                var (pub, sub) = scope.PubSub();
                var aw = Single(() => { }, sub).GetAwaiter();
                Assert.IsFalse(aw.IsCompleted);
                pub.Next();
                Assert.IsTrue(aw.IsCompleted);
            }
        }

        [Test]
        public void Dispose_StopsUsualFlow()
        {
            using (React.Scope(out var scope))
            {
                var (_, sub) = scope.PubSub();

                var r = Name();
                r.Dispose();

                async Routine Name()
                {
                    await sub;
                    Assert.Fail("Not invoked");
                }
            }
        }

        [Test]
        public void AwaiterBreaksRoutine()
        {
            using (React.Scope(out var scope))
            {
                var (pub, sub) = scope.PubSub();
                Routine closure = null;
                var r = Outer();
                var aw = closure.GetAwaiter();
                r.Dispose();
                Assert.IsTrue(aw.IsCompleted);

                async Routine Outer()
                {
                    closure = Inner();
                    await closure;
                }

                async Routine Inner()
                {
                    await sub;
                }
            }
        }

        [Test]
        public void GetScope_DoesntBreakRoutine()
        {
            using (React.Scope(out var scope))
            {
                var (pub, sub) = scope.PubSub();
                Routine closure = null;
                var r = Outer();
                var aw = closure.GetAwaiter();
                r.Dispose();
                Assert.IsFalse(aw.IsCompleted);

                async Routine Outer()
                {
                    closure = Inner();
                    await closure.GetScope(scope);
                }

                async Routine Inner()
                {
                    await sub;
                }
            }
        }
    }
}