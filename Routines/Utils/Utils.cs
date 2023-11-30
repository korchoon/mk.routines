namespace Mk.Routines {
    static class Utils {
        public static bool TrySetNull<T>(ref T arg, out T buf) where T : class {
            if (arg == null) {
                buf = null;
                return false;
            }

            buf = arg;
            arg = null;
            return true;
        }
    }
}