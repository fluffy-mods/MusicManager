namespace MusicManager {
    internal static class Log {
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Debug(string msg) {
            Message(msg);
        }

        public static void Message(string msg) {
            Verse.Log.Message($"MusicManager :: {msg}");
        }

        public static void Error(string msg) {
            Verse.Log.Error($"MusicManager :: {msg}");
        }
    }
}
