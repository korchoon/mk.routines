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
                bool visited = false;
                Single(() => visited = true, sub);
                Assert.IsTrue(visited);
            }
        }
    }

}