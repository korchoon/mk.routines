// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using Lib;
using NUnit.Framework;
using UnityEngine;
using Utility.Asserts;

namespace AsyncTests.Channels
{
    [TestFixture]
    public class Scope
    {
        [Test]
        public void Completed_BeforeAndAfterDispose()
        {
            var dispose = React.Scope(out var scope);
            Assert.IsFalse(scope.Completed);
            dispose.Dispose();
            Assert.IsTrue(scope.Completed);
        }

        [Test]
        public void Dispose_ReverseCallbackOrder()
        {
            var queue = new Queue<int>();
            using (React.Scope(out var scope))
            {
                scope.OnDispose(() => queue.Enqueue(1));
                scope.OnDispose(() => queue.Enqueue(2));
                scope.OnDispose(() => queue.Enqueue(3));
            }

            Assert.AreEqual(3, queue.Dequeue());
            Assert.AreEqual(2, queue.Dequeue());
            Assert.AreEqual(1, queue.Dequeue());
            Assert.AreEqual(0, queue.Count);
        }

        [Test]
        public void Unsubscribe_SameDelegate()
        {
            using (React.Scope(out var scope))
            {
                scope.OnDispose(Disposal);
                scope.Unsubscribe(Disposal);
            }

            void Disposal()
            {
                Assert.Fail("Not going to happen");
            }
        }

        [Test]
        public void Unsubscribe_DifferentDelegates_Assert()
        {
            Assert.Catch<Asr.AssertException>(Test);

            void Test()
            {
                using (React.Scope(out var scope))
                {
                    scope.OnDispose(() => { });
                    scope.Unsubscribe(() => { }); // throws
                }
            }
        }

        [Test]
        public void AfterDispose_CallbackExecutesImmediately()
        {
            var dispose = React.Scope(out var scope);
            dispose.Dispose();
            var done = false;
            scope.OnDispose(() => done = true);
            Assert.IsTrue(done);
        }

        [Test]
        public void CompletedAfterLast()
        {
            var dispose = React.Scope(out var scope);
            scope.OnDispose(() => Assert.IsFalse(scope.Completed));
            dispose.Dispose();
            Assert.IsTrue(scope.Completed);
        }
    }
}