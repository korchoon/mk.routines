using System.Diagnostics;
using Mk.Debugs;

namespace Mk.Routines {
    static class CalledOnceGuardApi {
        [Conditional (FLAGS.DEBUG)]
        public static void Assert (this ref CalledOnceGuard t) {
            Asr.IsFalse (t._called, "Awaiter should be called once");
            t._called = true;
        }
    }

    struct CalledOnceGuard {
        public bool _called;
    }
}