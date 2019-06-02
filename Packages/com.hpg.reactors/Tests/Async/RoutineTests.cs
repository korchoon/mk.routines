using System;
using Lib;
using Lib.Async;
using Lib.Async.Debugger;
using Lib.DataFlow;
using NUnit.Framework;

namespace AsyncTests.Async
{
    [TestFixture]
    public class RoutineTests
    {
        [Test]
        public void EmptyRoutine_Completed_Immediately()
        {
            var routine = Empty();
            var aw = routine.GetAwaiter();
            Assert.IsTrue(aw.IsCompleted);

            async Routine Empty()
            {
            }
        }

        [Test]
        public void FirstContinuation_Invoked_Immediately()
        {
            var visited = false;
            Sample();
            Assert.IsTrue(visited);

            async Routine Sample()
            {
                visited = true;
                await _Never.Instance;
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
            }

            async Routine Single(ISub sub)
            {
                await sub;
            }
        }

        [Test]
        public void Dispose_DoesNotInvokeContinuation_SubAwaiter()
        {
            var r = Name();
            r.Dispose();

            async Routine Name()
            {
                await _Never.Instance;
                Assert.Fail("Should not happen");
            }
        }

        [Test]
        public void Dispose_StopsUsualFlow_SubAwaiter()
        {
            using (React.Scope(out var scope))
            {
                var (pub, sub) = scope.PubSub();
                var r = Name();
                r.Dispose();
                pub.Next();

                async Routine Name()
                {
                    await sub;
                    Assert.Fail("Should not happen");
                }
            }
        }


        [Test]
        public void OuterRoutine_Dispose_Breaks_Inner()
        {
            Routine innerClosure = null;
            var outer = Outer();
            var innerAwaiter = innerClosure.GetAwaiter();
            outer.Dispose();
            Assert.IsTrue(innerAwaiter.IsCompleted);

            async Routine Outer()
            {
                innerClosure = Inner();
                await innerClosure;
            }

            async Routine Inner()
            {
                await _Never.Instance;
            }
        }

        [Test]
        public void GetScope_DoesntBreakRoutine()
        {
            using (React.Scope(out var scope))
            {
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
                    await _Never.Instance;
                }
            }
        }

        [Test]
        public void SelfScope_Await()
        {
            var flag = false;
            var s = Sample();
            Assert.IsTrue(flag);

            var aw = s.GetAwaiter();
            Assert.IsFalse(aw.IsCompleted);
            
            s.Dispose();
            Assert.IsTrue(aw.IsCompleted);

            async Routine Sample()
            {
                var scope = await Routine.SelfScope();
                flag = true;

                await scope;
            }
        }

        [Test]
        public void GetScope_Disposed_TODO()
        {
#if M_TODO
             using (React.Scope(out var scope))
            {
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
                    await _Never.Instance;
                }
            }
#endif
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


                r.Dispose();
                Assert.AreEqual(1, i);


                async Routine Sample()
                {
                    await sub;
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

            async Routine Sample()
            {
                await _Never.Instance;
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

                async Routine Sample()
                {
                    await sub;
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