namespace Lib.Timers
{
    public struct TimeToken
    {
        float _start;
        float _delay;

        public TimeToken(float delay, float now)
        {
            _delay = delay;
            _start = now;
        }

        public float Progress(float now) => 1f - TimeLeft(now) / _delay;

        public bool KeepWaiting(float now) => TimeLeft(now) > 0;

        float TimeLeft(float now) => _start - now + _delay;

#if legacy
        [Conditional(FLAGS.NON_EDITOR)]
        static void _Runtime(ref float time)
        {
            time = Time.time;
        }

        [Conditional(FLAGS.UNITY_EDITOR)]
        static void _Editor(ref float time)
        {
            if (Application.isPlaying)
                time = Time.time;

#if UNITY_EDITOR
            time = (float) EditorApplication.timeSinceStartup;
#endif

        }
#endif
    }
}