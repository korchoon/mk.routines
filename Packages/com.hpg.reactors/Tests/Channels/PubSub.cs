// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------
using Lib;
using NUnit.Framework;
using Utility.Asserts;

namespace AsyncTests.Channels
{
    // todo PubSub<T>
    [TestFixture]
    public class PubSub
    {
        [Test]
        public void Pub_Next_ExecuteCallbackImmediately()
        {
            using (React.Scope(out var scope))
            {
                var received = false;
                var (pub, sub) = scope.PubSub();
                sub.OnNext(() => received = true, scope);
                Assert.IsFalse(received);
                pub.Next();
                Assert.IsTrue(received);
            }
        }

        [Test]
        public void Pub_Next_ExecutesMultipleTimes()
        {
            const int times = 5;
            using (React.Scope(out var scope))
            {
                var i = 0;
                var (pub, sub) = scope.PubSub();
                sub.OnNext(() => ++i, scope);

                for (var ii = 0; ii < times; ii++)
                    pub.Next();

                Assert.AreEqual(times, i);
            }
        }

        [Test]
        public void Pub_Next_DeliveredToAllSubscribers()
        {
            const int times = 5;
            using (React.Scope(out var scope))
            {
                var timesReceived = 0;
                var (pub, sub) = scope.PubSub();
                sub.OnNext(() => ++timesReceived, scope);

                for (var ii = 0; ii < times; ii++)
                    pub.Next();

                Assert.AreEqual(times, timesReceived);
            }
        }


        [Test]
        public void CreateFromDisposedScope_Assert()
        {
            Assert.Catch<Asr.AssertException>(Inner);

            void Inner()
            {
                React.Scope(out var disposedScope).Dispose();
                var _ = disposedScope.PubSub(); // throws
            }
        }

        [Test]
        public void SubscribeWithDisposedScope_Assert()
        {
            Assert.Catch<Asr.AssertException>(Inner);

            void Inner()
            {
                using (React.Scope(out var scope))
                {
                    var (_, sub) = scope.PubSub();

                    React.Scope(out var disposedScope).Dispose();

                    sub.OnNext(() => { }, disposedScope); // throws 
                }
            }
        }

        [Test]
        public void NextToDisposedPub_Assert()
        {
            Assert.Catch<Asr.AssertException>(Inner);

            void Inner()
            {
                var d = React.Scope(out var disposedAfterPubSubCreated);
                var (pub, _) = disposedAfterPubSubCreated.PubSub();

                d.Dispose();
                pub.Next(); // throws
            }
        }

        [Test]
        public void NextSkipsCallbacksWithDisposedScopes()
        {
            using (React.Scope(out var scope))
            {
                var d = React.Scope(out var disposedAfterOnNext);
                var (pub, sub) = scope.PubSub();
                sub.OnNext(() => Assert.Fail("Not executed"), disposedAfterOnNext);

                d.Dispose();
                pub.Next();
            }
        }
    }
}