// MusicManagerPlay.cs
// Copyright Karel Kroeze, 2020-2020

using HarmonyLib;
using RimWorld;

namespace MusicManager.Patches {
    [HarmonyPatch(typeof(MusicManagerPlay), "StartNewSong")]
    public class MusicManagerPlay_StartNewSong {
        public static bool Prefix() {
            // do not allow starting the next song if we're currently paused
            return !MusicManager.IsPaused;
        }

        public static void Postfix() {
            MusicManager.AudioSource.time = 0;
        }
    }

    [HarmonyPatch(typeof(MusicManagerPlay), nameof(MusicManagerPlay.ForceStartSong))]
    public class MusicManagerPlay_ForceStartSong {
        public static void Prefix() {
            // unpause when force starting next song
            MusicManager.Resume();
        }
    }
}
