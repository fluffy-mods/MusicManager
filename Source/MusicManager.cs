// MusicManager.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace MusicManager
{
    public class MusicManager : Mod
    {
        private static readonly MethodInfo _appropriateNow_MI =
            AccessTools.Method( typeof( MusicManagerPlay ), "AppropriateNow" );

        private static readonly FieldInfo audioSource_FieldInfo =
            AccessTools.Field( typeof( MusicManagerPlay ), "audioSource" );

        private static readonly FieldInfo lastStartedSong_FieldInfo =
            AccessTools.Field( typeof( MusicManagerPlay ), "lastStartedSong" );

        public MusicManager( ModContentPack content ) : base( content )
        {
            // initialize settings
            Settings = GetSettings<Settings>();
            Instance = this;

#if DEBUG
            Harmony.DEBUG = true;
#endif
            var harmony = new Harmony( "Fluffy.GameComp_MusicManager" );
            harmony.PatchAll();
        }

        public static List<SongDef> AppropriateSongs => Songs.Where( AppropriateNow ).ToList();

        public static AudioSource AudioSource =>
            audioSource_FieldInfo.GetValue( Find.MusicManagerPlay ) as AudioSource;

        public static SongDef CurrentSong =>
            lastStartedSong_FieldInfo.GetValue( Find.MusicManagerPlay ) as SongDef;

        public static MusicManager Instance { get; private set; }

        public static bool isPaused { get; private set; }

        public static Settings Settings { get; private set; }

        public static List<SongDef> Songs => DefDatabase<SongDef>.AllDefsListForReading;


        public static bool AppropriateNow( SongDef song )
        {
            return (bool) _appropriateNow_MI.Invoke( Find.MusicManagerPlay, new object[] {song} );
        }

        public static void Next()
        {
            Log.Debug( "next" );
            Play();
        }

        public static void Pause()
        {
            Log.Debug( "pause" );
            AudioSource.Pause();
            isPaused = true;
        }

        public static void Play( [CanBeNull] SongDef song = null )
        {
            Log.Debug( "play" );
            Find.MusicManagerPlay.ForceStartSong( song, false );
            AudioSource.time = 0;

            Messages.Message( $"Now playing: {CurrentSong?.Name() ?? "NONE"}",
                              MessageTypeDefOf.SilentInput );
        }

        public static void Previous()
        {
            Log.Debug( "previous" );
            if ( AudioSource.time < 5 )
                Play();
            else
                AudioSource.time = 0;
        }

        public static void Resume()
        {
            Log.Debug( "resume" );
            AudioSource.UnPause();
            isPaused = false;
        }

        public override void DoSettingsWindowContents( Rect inRect )
        {
            base.DoSettingsWindowContents( inRect );
            GetSettings<Settings>().DoWindowContents( inRect );
        }

        public override string SettingsCategory()
        {
            return I18n.MusicManager;
        }
    }
}