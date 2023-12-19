using System;
using System.Diagnostics;
using System.Threading;

class InfLoopSafety {
    readonly int _max;
    int _cachedFrameCount;
    int _timesPerFrame;
    int FrameCount () => UnityEngine.Time.frameCount;

    public InfLoopSafety (int max) {
        _max = max;
    }

    [Conditional ("PREVENT_INF_LOOP")]
    public void Check () {
        _timesPerFrame += 1;
        if (_timesPerFrame > _max) {
            throw new Exception ("Inf loop detected");
        }

        var curFrameCount = FrameCount ();
        if (curFrameCount > _cachedFrameCount) {
            _cachedFrameCount = curFrameCount;
            _timesPerFrame = 0;
        }
    }
}

public class CallWithTimeout2 {
    readonly int _timeoutMs;
    Action one;

    public CallWithTimeout2 (Action d, int timeoutMs) {
        _timeoutMs = timeoutMs;
        one = proxy;

        void proxy () => d ();
    }

    public void Execute () {
        var result = one.BeginInvoke (null, null);
        if (result.AsyncWaitHandle.WaitOne (_timeoutMs)) {
            one.EndInvoke (result);
        }
        else {
            Thread.CurrentThread.Abort ();
            throw new TimeoutException ();
        }
    }

    static void CallWithTimeout (Action action, int timeoutMilliseconds) {
        Thread threadToKill = null;
        Action wrappedAction = () => {
            threadToKill = Thread.CurrentThread;
            try {
                action ();
            }
            catch (ThreadAbortException) {
                Thread.ResetAbort ();
            }
        };

        var result = wrappedAction.BeginInvoke (null, null);
        if (result.AsyncWaitHandle.WaitOne (timeoutMilliseconds)) {
            wrappedAction.EndInvoke (result);
        }
        else {
            threadToKill.Abort ();
            throw new TimeoutException ();
        }
    }
}